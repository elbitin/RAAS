/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Client.
 *
 * RAAS Client is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Client is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Client. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Timers;
using Microsoft.Win32;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.RAASClient.RAASClient.RemoteAppsServiceRef;
using Elbitin.Applications.RAAS.RAASClient.RAASClient.RAASServerServiceRef;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.RAASClient
{
    public class ServerManager : IDisposable
    {
        public ServerStates ServerStates { get; set; } = new ServerStates();
        public ServerSettings ServerSettings { get; set; }
        public String RAASServerVersion { get; set; } = null;
        public String AutostartPath { get; set; } = null;
        public String KeepAlivePath { get; set; } = null;
        public String ShortcutsServerPath { get; set; } = null;
        public String ShareXml { get; set; } = "";
        public delegate void ServerDelagate(String serverName);
        public static event ServerDelagate FailedMessageEvent;
        public static event ServerDelagate DisconnectedMessageEvent;
        public static event ServerDelagate ConnectedMessageEvent;
        public static event ServerDelagate StatusChangeEvent;
        private delegate void configureDelegate();
        private configureDelegate configureDel;
        private delegate void getSharesDelegate();
        private getSharesDelegate getSharesDel;
        private delegate void updateLoggedInStatesDelegate();
        private updateLoggedInStatesDelegate updateLoggedInStatesDel;
        private RAASServerServiceClient raasServerServiceClient = null;
        private RemoteAppsServiceClient remoteAppsServiceClient = null;
        private RemoteAppsServiceCallback remoteAppServiceCallback;
        private Process raasClientProcess;
        private System.Timers.Timer updateConnectionsTimer = new System.Timers.Timer();
        private System.Timers.Timer updateConnectedTimer = new System.Timers.Timer();
        private System.Timers.Timer serverSubscribeTimer = new System.Timers.Timer();
        private readonly object raasServerSubscribeLock = new object();
        private SessionSwitchEventHandler sessionSwitchEventHandler;
        private Stopwatch timeSinceStarted = new Stopwatch();
        private bool failTimeOutHandled = false;
        private bool running = true;
        private const int UPDATECONNECTIONSTIMER_INTERVAL_MS = 1000;
        private const int UPDATECONNECTEDTIMER_INTERVAL_MS = 1000;
        private const int SERVERSSUBSCRIBETIMER_INTERVAL_MS = 1000;
        private const int FAIL_TIMEOUT_MS = 120000;
        private const int RAPPS_DISCONNECTION_RETRY_COUNT = 100;
        private const int RAPPS_DISCONNECTION_RETRY_TIME_MS = 100;
        private const int RAPPS_CONNECTION_RETRY_COUNT = 400;
        private const int RAPPS_CONNECTION_RETRY_TIME_MS = 100;
        private const int RUN_RAPPS_RETRY_COUNT = 60;
        private const int RUN_RAPPS_RETRY_TIME_MS = 100;
        private const int SHARE_XML_SIZE_LIMIT = 5000;
        private const int RECONNECT_SHARE_UPDATE_TIME_S = 60;
        private const int TIME_SECOND_MS = 1000;

        public ServerManager(ServerSettings serverSettings)
        {
            ServerSettings = serverSettings;
            ReadShares();
            PrepareUpdateConnectionsTimer();
            PrepareUpdateConnectedTimer();
            PrepareSubscribeTimer();
            timeSinceStarted.Start();
            RegisterSessionSwitchEventHandler();
            InvokeConfigureRAASServerServiceClient();
        }

        public void InvokeConfigureRAASServerServiceClient()
        {
            configureDel = new configureDelegate(ConfigureRAASServerServiceClient);
            configureDel.BeginInvoke(null, null);
        }

        private void RegisterSessionSwitchEventHandler()
        {
            sessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += sessionSwitchEventHandler;
        }

        private void PrepareSubscribeTimer()
        {
            serverSubscribeTimer.Interval = SERVERSSUBSCRIBETIMER_INTERVAL_MS;
            serverSubscribeTimer.AutoReset = false;
            serverSubscribeTimer.Elapsed += ServerSubscribeTimer_Elapsed;
            serverSubscribeTimer.Enabled = true;
            serverSubscribeTimer.Start();
        }

        private void PrepareUpdateConnectedTimer()
        {
            updateConnectedTimer.Interval = UPDATECONNECTEDTIMER_INTERVAL_MS;
            updateConnectedTimer.AutoReset = false;
            updateConnectedTimer.Elapsed += UpdateConnectedTimer_Elapsed;
            updateConnectedTimer.Enabled = true;
            updateConnectedTimer.Start();
        }

        private void PrepareUpdateConnectionsTimer()
        {
            updateConnectionsTimer.Interval = UPDATECONNECTIONSTIMER_INTERVAL_MS;
            updateConnectionsTimer.AutoReset = false;
            updateConnectionsTimer.Elapsed += UpdateConnectionsTimer_Elapsed;
            updateConnectionsTimer.Enabled = true;
            updateConnectionsTimer.Start();
        }

        public void Dispose()
        {
            running = false;
            updateConnectedTimer.Stop();
            updateConnectionsTimer.Stop();
            try
            {
                raasServerServiceClient?.Abort();
            }
            catch { }
            try
            {
                remoteAppsServiceClient?.Abort();
            }
            catch { }
            SystemEvents.SessionSwitch -= sessionSwitchEventHandler;
        }

        private void UpdateConnectionsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateConnectionsTimer.Stop();
            if (running)
            {
                // Fail after time period
                if (!failTimeOutHandled && timeSinceStarted.ElapsedMilliseconds >= FAIL_TIMEOUT_MS)
                {
                    try
                    {
                        if (ServerStates.ShouldFailOnTimeOut(ServerSettings))
                        {
                            if (ServerSettings.ShowNotifications)
                                FailedMessageEvent.Invoke(ServerSettings.Alias);
                            ServerStates.HasFailedSinceUnlock = true;
                        }
                    }
                    catch { }
                    failTimeOutHandled = true;
                }

                // Validate if RAAS Client service is connected
                try
                {
                    if (ServerStates.RemoteApplicationsServiceConnected)
                        remoteAppsServiceClient.Subscribe();
                }
                catch
                {
                    ServerStates.RemoteApplicationsServiceConnected = false;
                }

                // Try to connect to raas client service if it is not connected 
                if (!ServerStates.RemoteApplicationsServiceConnected)
                    try
                    {
                        OpenRemoteAppServiceClient();
                    }
                    catch { }

                // Reconfigure closed RAAS Server service clients
                if (!ServerStates.RAASServerServiceSubscribed && raasServerServiceClient.State != CommunicationState.Opened && raasServerServiceClient.State != CommunicationState.Opening)
                {
                    ConfigureRAASServerServiceClient();
                }

                // Auto-start raas client if desired
                if (timeSinceStarted.ElapsedMilliseconds <= FAIL_TIMEOUT_MS &&
                    ServerStates.ShouldConnectBeforeTimeout(ServerSettings))
                {
                    Connect();
                    ServerStates.AutoConnectStarted = true;
                }

                if (ServerStates.RemoteApplicationsShouldClose)
                    try
                    {
                        remoteAppsServiceClient.Disconnect();
                    }
                    catch { }
                if (ServerStates.ShouldRestartTimers())
                    updateConnectionsTimer.Start();
            }
        }

        private void UpdateConnectedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateConnectedTimer.Stop();
            if (running)
            {
                if (ServerStates.ShouldUpdateRemoteApplicationsStatus())
                    try
                    {
                        int status = remoteAppsServiceClient.GetStatus();
                        if (status == 1)
                        {
                            UpdateConnected();
                        }
                    }
                    catch { }
                if (ServerStates.ShouldRestartTimers())
                    updateConnectedTimer.Start();
            }
        }

        private void ServerSubscribeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            serverSubscribeTimer.Stop();
            try
            {
                if (raasServerServiceClient.State == CommunicationState.Faulted || raasServerServiceClient.State == CommunicationState.Closed)
                {
                    if (ServerStates.RAASServerServiceSubscribed)
                    {
                        ServerStates.RAASServerServiceSubscribed = false;
                        StatusChangeEvent.Invoke(ServerSettings.Alias);
                    }
                }
            }
            catch { }
            if (!ServerStates.RAASServerServiceSubscribed)
            {
                bool contact = ServerHelper.Contact(ServerSettings.ServerName);
                ServerStates.ContactUpdate(contact);
                if (contact)
                    SubscribeRAASServerService();
            }
            if (ServerStates.ShouldRestartTimers())
                serverSubscribeTimer.Start();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                ServerStates.SessionLockStartedUpdate();
                serverSubscribeTimer.Stop();
                updateConnectedTimer.Stop();
                updateConnectionsTimer.Stop();
                Disconnect();
                ServerStates.SessionLockDoneUpdate();
                StatusChangeEvent.Invoke(ServerSettings.Alias);
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                ServerStates.SessionUnlockStartedUpdate();

                // Reset time since unlock variables
                timeSinceStarted.Stop();
                timeSinceStarted.Reset();
                timeSinceStarted.Start();
                failTimeOutHandled = false;

                serverSubscribeTimer.Start();
                updateConnectedTimer.Start();
                updateConnectionsTimer.Start();
                ServerStates.SessionUnlockDoneUpdate(ServerSettings);
                StatusChangeEvent.Invoke(ServerSettings.Alias);
            }
        }

        private void UpdateConnected()
        {
            if (!ServerStates.RemoteApplicationsConnected)
            {
                ServerStates.ConnectedUpdate();
                Thread thread = new Thread(UpdateLoggedInStates);
                thread.Start();

                if (ServerSettings.ShowNotifications)
                    ConnectedMessageEvent.Invoke(ServerSettings.Alias);
                StatusChangeEvent.Invoke(ServerSettings.Alias);
            }
        }

        private void ConnectShares()
        {
            // Ping server
            bool contact = ServerHelper.Contact(ServerSettings.ServerName);

            // Try to get disk access to server shares if contact successful
            if (contact)
            {
                ServerStates.RAASServerContact = true;
                ServerStates.RAASServerSharesConnected = ServerHelper.ConnectShares(ServerSettings);
            }
            else
                ServerStates.RAASServerContact = false;
        }

        private void ShareXmlChange()
        {
            ServerStates.RAASServerSharesShouldReload = true;
            try
            {
                getSharesDel = new getSharesDelegate(GetShares);
                getSharesDel.BeginInvoke(null, null);
            }
            catch { }
        }

        public void ReadShares()
        {
            try
            {
                String serverShareXmlPath = RAASClientPathHelper.GetSharesFilePath(ServerSettings.ServerName);
                ShareXml = File.ReadAllText(serverShareXmlPath);
            }
            catch { }
        }

        private void GetShares()
        {
            String newShareXml = "";

            // Get share XML
            newShareXml = raasServerServiceClient.GetShareXml();

            // Store share XML file and report to shell namespace extension if valid share XML was recieved
            if (newShareXml != null && newShareXml != ShareXml)
            {
                // Store share XML file
                String serverShareXmlPath = RAASClientPathHelper.GetSharesFilePath(ServerSettings.ServerName);
                TextWriter tw = File.CreateText(serverShareXmlPath);
                if (newShareXml.Length < SHARE_XML_SIZE_LIMIT)
                    tw.Write(newShareXml.Replace("..", "")); // Save without supporting references to parent folders, the file should not contain '..'
                tw.Close();

                // Update shell namespace extension
                ServerHelper.UpdateNamespaceExtension(ServerSettings.Alias);

                // Update share XML
                ShareXml = newShareXml;
            }

            ServerStates.RAASServerSharesShouldReload = false;
        }

        public void StartAutostartApplication(String arguments)
        {
            StartRemoteApplication(AutostartPath, arguments);
        }

        public void StartKeepAliveApplication(String arguments)
        {
            StartRemoteApplication(KeepAlivePath, arguments);
        }

        public void StartShortcutsServerApplication(String arguments)
        {
            StartRemoteApplication(ShortcutsServerPath, arguments);
        }

        public void StartRemoteApplication(String application, String arguments)
        {
            try
            {
                // Set server states
                ServerStates.UserInitiatedLastDisconnect = false;

                // Fail instantly if no connection to RAAS Server service exist
                if (!ServerStates.RAASServerServiceSubscribed && !ServerStates.RemoteApplicationsConnected)
                {
                    if (ServerSettings.ShowNotifications)
                        FailedMessageEvent.Invoke(ServerSettings.Alias);
                    ServerStates.HasFailedSinceUnlock = true;
                    return;
                }

                // Try to start remote application
                if (ServerStates.RemoteApplicationsProcessRunning)
                    if (ServerStates.RemoteApplicationsServiceConnected)
                    {
                        try
                        {
                            remoteAppsServiceClient.StartRemoteApplication(application, arguments);
                        }
                        catch
                        {
                            try
                            {
                                OpenRemoteAppServiceClient();
                                remoteAppsServiceClient.StartRemoteApplication(application, arguments);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        try
                        {
                            OpenRemoteAppServiceClient();
                            remoteAppsServiceClient.StartRemoteApplication(application, arguments);
                        }
                        catch { }
                    }
                else
                {
                    Connect();
                    try
                    {
                        remoteAppsServiceClient.StartRemoteApplication(application, arguments);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void OpenRemoteAppServiceClient()
        {
            try
            {
                if (remoteAppsServiceClient?.State == CommunicationState.Opened || remoteAppsServiceClient?.State == CommunicationState.Opening)
                    return;
                else if (remoteAppsServiceClient?.State == CommunicationState.Created)
                    remoteAppsServiceClient?.Close();
                else if (remoteAppsServiceClient?.State == CommunicationState.Faulted)
                    remoteAppsServiceClient?.Abort();
            }
            catch { }

            // Create RAAS Client service callback for server
            NetNamedPipeBinding nnpb = new NetNamedPipeBinding();
            nnpb.MaxBufferPoolSize = 20000000;
            nnpb.MaxReceivedMessageSize = 20000000;
            remoteAppServiceCallback = new RemoteAppsServiceCallback();
            remoteAppServiceCallback.ConnectedEvent += UpdateConnected;

            // Create RAAS Client service client for server
            InstanceContext context = new InstanceContext(remoteAppServiceCallback);
            remoteAppsServiceClient = new RemoteAppsServiceClient(context, nnpb, new EndpointAddress(new Uri("net.pipe://localhost/RemoteAppsService/" + ServerSettings.ServerName)));
            remoteAppsServiceClient.Open();
            remoteAppsServiceClient.InnerChannel.Faulted += new EventHandler(RAASClientServiceDisconnected);
            remoteAppsServiceClient.InnerChannel.Closed += new EventHandler(RAASClientServiceDisconnected);

            // Try subscribing to RAAS Client service
            remoteAppsServiceClient.Subscribe();
            ServerStates.RemoteApplicationsServiceConnected = true;
        }

        public void Connect()
        {
            try
            {
                ServerStates.ConnectStartedUpdate();

                // Diconnect any existing RAAS Client
                if (ServerStates.RemoteApplicationsServiceConnected)
                {
                    Disconnect();
                }
                ServerStates.RemoteApplicationsServiceConnected = false;

                if (!ServerStates.RAASServerServiceSubscribed)
                    return;

                // Try to wait until existing RAAS Clients have disconnected
                for (int j = 0; j < RAPPS_DISCONNECTION_RETRY_COUNT; j++)
                {
                    if (!ServerStates.RemoteApplicationsProcessRunning)
                        break;
                    Thread.Sleep(RAPPS_DISCONNECTION_RETRY_TIME_MS);
                }

                // Run a new instance of RAAS Client if none exist
                if (!ServerStates.RemoteApplicationsProcessRunning)
                {
                    RunRemoteAppClient();
                }

                // Try connecting to RAAS Client service, with retries
                for (int j = 0; j < RAPPS_CONNECTION_RETRY_COUNT; j++)
                    try
                    {
                        Thread.Sleep(RAPPS_CONNECTION_RETRY_TIME_MS);
                        OpenRemoteAppServiceClient();
                        break;
                    }
                    catch { }
            }
            catch { }
        }

        public void Disconnect()
        {
            if (ServerStates.RemoteApplicationsProcessRunning)
                ServerStates.RemoteApplicationsShouldClose = true;

            // Disconnect any existing RAAS Client service
            if (ServerStates.RemoteApplicationsServiceConnected)
            {
                try
                {
                    remoteAppsServiceClient.Disconnect();
                }
                catch
                {
                    try
                    {
                        OpenRemoteAppServiceClient();
                        remoteAppsServiceClient.Disconnect();
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    OpenRemoteAppServiceClient();
                    remoteAppsServiceClient.Disconnect();
                }
                catch { }
            }
        }

        public bool Reboot()
        {
            bool rebootSuccessFul = false;
            if (ServerStates.RAASServerServiceSubscribed)
                try
                {
                    raasServerServiceClient.Reboot();
                    rebootSuccessFul = true;
                }
                catch { };
            return rebootSuccessFul;
        }

        public bool UpdateShortcuts()
        {
            bool updateShortcutsSuccessFul = false;
            if (ServerStates.RAASServerServiceSubscribed)
                try
                {
                    raasServerServiceClient.UpdateShortcuts();
                    updateShortcutsSuccessFul = true;
                }
                catch { };
            return updateShortcutsSuccessFul;
        }

        public bool LogOff()
        {
            bool logOffSuccessful = false;
            if (ServerStates.RAASServerServiceSubscribed)
            {
                try
                {
                    raasServerServiceClient.LogOff();
                    logOffSuccessful = true;
                }
                catch { }
            }
            return logOffSuccessful;
        }

        private void RAASClientServiceDisconnected(Object sender, EventArgs e)
        {
            // Try to reconnect RAAS Client service clients if desired
            try
            {
                if (!ServerStates.RemoteApplicationsServiceConnected)
                    OpenRemoteAppServiceClient();
            }
            catch { }
        }

        private void LoggedInChange()
        {
            try
            {
                updateLoggedInStatesDel = new updateLoggedInStatesDelegate(UpdateLoggedInStates);
                updateLoggedInStatesDel.BeginInvoke(null, null);
            }
            catch { }
        }

        public void RequireReconnect()
        {
            try
            {
                raasServerServiceClient.Close();
                ServerStates.RAASServerServiceSubscribed = false;
            }
            catch
            {
                ServerStates.RAASServerServiceSubscribed = false;
            }
            try
            {
                Disconnect();
            }
            catch { }
        }

        private void UpdateLoggedInStates()
        {
            // Indicate if log off can be performed on the server
            if (raasServerServiceClient.State == CommunicationState.Opened)
                try
                {
                    ServerStates.RAASServerCanLogOff = raasServerServiceClient.GetLoggedInState();
                }
                catch
                {
                    ServerStates.RAASServerCanLogOff = true;
                }
            else
                ServerStates.RAASServerCanLogOff = false;
            StatusChangeEvent.Invoke(ServerSettings.Alias);
        }

        private void RunRemoteAppClient()
        {
            // Set default raas client status to disconnected
            int raasClientStatus = 0;

            ServerStates.ResetRemoteApplicationsUpdate();

            // Check if raasclient already running
            try
            {
                OpenRemoteAppServiceClient();
            }
            catch { }
            if (ServerStates.RemoteApplicationsServiceConnected)
                try
                {
                    raasClientStatus = remoteAppsServiceClient.GetStatus();
                }
                catch { }

            // Start raasclient if it is not running
            if (raasClientStatus == 0)
            {
                try
                {
                    // Start raas client for server if not already started, with retries
                    for (int i = 0; i < RUN_RAPPS_RETRY_COUNT; i++)
                    {
                        try
                        {
                            if (!ServerStates.RemoteApplicationsProcessRunning)
                            {
                                // Start raas client for server and enable events
                                raasClientProcess = RAASClientProgramHelper.StartRemoteApps(ServerSettings.ServerName, RAASClientProcessExited);
                                ServerStates.RemoteApplicationsStartedUpdate();
                                break;
                            }

                            // Sleep between retries
                            Thread.Sleep(RUN_RAPPS_RETRY_TIME_MS);

                            break;
                        }
                        catch { }
                    }

                    // Connect to raas client service if necessary
                    if (!ServerStates.RemoteApplicationsServiceConnected)
                        try
                        {
                            OpenRemoteAppServiceClient();
                        }
                        catch { }
                }
                catch { }
            }
        }

        private void RAASClientProcessExited(object sender, EventArgs e)
        {
            ServerStates.RemoteApplicationsShouldClose = false;
            if (ServerStates.FailedOnNextExit)
            {
                if (ServerSettings.ShowNotifications)
                    FailedMessageEvent.Invoke(ServerSettings.Alias);
                ServerStates.HasFailedSinceUnlock = true;
            }
            else if (ServerStates.DisconnectedOnNextExit)
                if (ServerSettings.ShowNotifications)
                    DisconnectedMessageEvent.Invoke(ServerSettings.Alias);
            ServerStates.RemoteApplicationsProcessNotRunning();
            StatusChangeEvent.Invoke(ServerSettings.Alias);
        }

        private void CreateNewRAASServerServiceClient()
        {
            try
            {
                if (raasServerServiceClient?.State == CommunicationState.Opened || raasServerServiceClient?.State == CommunicationState.Opening)
                    return;
                else if (raasServerServiceClient?.State == CommunicationState.Created)
                    raasServerServiceClient?.Close();
                else if (raasServerServiceClient?.State == CommunicationState.Faulted)
                    raasServerServiceClient?.Abort();
            }
            catch { }

            // Update states
            if (ServerStates.RAASServerServiceSubscribed)
            {
                ServerStates.RAASServerServiceSubscribed = false;
                StatusChangeEvent.Invoke(ServerSettings.Alias);
            }

            try
            {
                // Add RAAS Server service client
                RAASServerServiceCallback serverServiceCallback = new RAASServerServiceCallback();
                serverServiceCallback.ShareXmlChangeEvent += ShareXmlChange;
                serverServiceCallback.LoggedInChangeEvent += LoggedInChange;
                raasServerServiceClient = new RAASServerServiceClient(new InstanceContext(serverServiceCallback));
                EndpointAddress endpoint = new EndpointAddress(new Uri(@"net.tcp://" + ServerSettings.ServerName + @":43000/RAASServerService"), new DnsEndpointIdentity("localhost"), new AddressHeaderCollection());
                raasServerServiceClient.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 10, 0);
                raasServerServiceClient.Endpoint.Binding.CloseTimeout = new TimeSpan(0, 10, 0);
                raasServerServiceClient.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                raasServerServiceClient.Endpoint.Binding.SendTimeout = new TimeSpan(0, 10, 0);
                raasServerServiceClient.Endpoint.Address = endpoint;
                raasServerServiceClient.ClientCredentials.Windows.ClientCredential.UserName = ServerSettings.UserName;
                raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Password = ServerSettings.Password;
                raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Domain = ServerSettings.Domain;
                raasServerServiceClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                raasServerServiceClient.ChannelFactory.Faulted += new EventHandler(RAASServerServiceClientOffline);
                raasServerServiceClient.ChannelFactory.Closed += new EventHandler(RAASServerServiceClientOffline);
            }
            catch { }
        }

        public void ConfigureRAASServerServiceClient()
        {
            // Update raas service client
            CreateNewRAASServerServiceClient();

            // Subscribe to server and update states if needed
            if (ServerStates.RAASServerServiceSubscribed != true)
                SubscribeRAASServerService();
        }

        private void SubscribeRAASServerService()
        {
            // Try to subscibe and update states
            try
            {
                bool subscribe = false;
                lock (raasServerSubscribeLock)
                {
                    if (!ServerStates.RAASServerServiceSubscribed && !ServerStates.RAASServerServiceSubscribing)
                    {
                        ServerStates.RAASServerServiceSubscribing = true;
                        subscribe = true;
                    }
                }
                if (subscribe)
                {
                    raasServerServiceClient.Subscribe();
                    RAASServerVersion = raasServerServiceClient.GetVersion();
                    AutostartPath = raasServerServiceClient.GetAutostartPath();
                    KeepAlivePath = raasServerServiceClient.GetKeepAliveAgentPath();
                    ShortcutsServerPath = raasServerServiceClient.GetShortcutsServerPath();
                    UpdateLoggedInStates();
                    try
                    {
                        GetShares();
                    }
                    catch { }
                    ServerStates.RAASServerServiceSubscribed = true;
                    ServerStates.RAASServerSharesShouldReload = true;
                    StatusChangeEvent.Invoke(ServerSettings.Alias);
                    ServerStates.RAASServerServiceSubscribing = false;
                    if (RAASClientFeatureHelper.NSExtInstalled())
                    {
                        // Update explorer namespace extension
                        if (ServerSettings.ServerEnabled)
                        {
                            if (RAASClientFeatureHelper.NSExtInstalled())
                            {
                                // Update explorer namespace extension
                                if (ServerSettings.ServerEnabled)
                                {
                                    new Thread(() =>
                                    {
                                        Thread.CurrentThread.IsBackground = true;
                                        for (int i = 0; i < RECONNECT_SHARE_UPDATE_TIME_S; i++)
                                        {
                                            ServerHelper.UpdateNamespaceExtension(ServerSettings.Alias);
                                            Thread.Sleep(TIME_SECOND_MS);

                                        }
                                    }).Start();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                ServerStates.RAASServerServiceSubscribing = false;
                ServerStates.RAASServerServiceSubscribed = false;
                StatusChangeEvent.Invoke(ServerSettings.Alias);
                bool contact = ServerHelper.Contact(ServerSettings.ServerName);
                ServerStates.ContactUpdate(contact);
            }
        }

        private void RAASServerServiceClientOffline(object sender, EventArgs e)
        {
            ServerStates.RAASServerServiceSubscribed = false;
            StatusChangeEvent.Invoke(ServerSettings.Alias);
        }
    }
}