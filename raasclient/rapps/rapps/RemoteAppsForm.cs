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
﻿// Copyright (c) Elbitin
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MSTSCLib;
using AxMSTSCLib;
using System.ServiceModel;
using System.ServiceModel.Description;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Diagnostics;
using Microsoft.Win32;
using Elbitin.Applications.RAAS.RAASClient.RemoteApps.RAASServerServiceRef;
using System.ServiceModel.Channels;
using System.Threading;
using System.Globalization;
using static Elbitin.Applications.RAAS.Common.Helpers.Win32Helper;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    public partial class RemoteAppsForm : Form
    {
        public String AutostartPath { get; set; } = null;
        public String KeepAlivePath { get; set; } = null;
        public String AppNamesPath { get; set; } = null;
        private Stopwatch connectTime = new Stopwatch();
        private ServiceHost host;
        private VisualizationsForm visualizations;
        private RemoteAppsService raasClientService = new RemoteAppsService();
        private RAASServerServiceClient raasServerServiceClient = null;
        private String serverName = "";
        private IntPtr storedHwnd;
        private bool exitOnDisconnect = true;
        private bool canActivate = true;
        private static Object disconnectLock = new object();
        private bool visualizationsAvailable = false;
        private SessionSwitchEventHandler sessionSwitchEventHandler;
        private List<ApplicationParameters> startApplications = new List<ApplicationParameters>();
        private AxMSTSCLib.AxMsRdpClient7NotSafeForScripting rdpClient;
        private const int TS_PERF_ENABLE_ENHANCED_GRAPHICS = 0x00000010;
        private const int TS_PERF_ENABLE_FONT_SMOOTHING = 0x00000080;
        private const int RAAS_SERVER_SERVICE_RETRY_COUNT = 20;
        private const int RAAS_SERVER_SERVICE_RETRY_INTERVAL_MS = 200;
        private const String REMOTE_APPS_SERVICE_ADDRESS_FMT = "net.pipe://localhost/RemoteAppsService/{0}";
        private const int RECONNECT_INTERVAL_MS = 30000;
        private delegate void startRemoteApplicationDelegate(String application, String arguments);
        private delegate void connectDelegate();
        private delegate void disconnectDelegate();
        private ServerSettings serverSettings;

        public RemoteAppsForm(String serverName)
        {
            this.serverName = serverName.ToUpperInvariant();
            GetServerSettings();
            InitializeRAASServerStrings();
            SetFormProperties();
            InitializeComponent();
            BringToFront();
            InitializeRemoteAppsService();
            RegisterSessionSwitchEventHandler();

            // Force focus of window
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            StartVisualizations();
        }

        private void GetServerSettings()
        {
            try
            {
                serverSettings = new ServerSettings(serverName);
            }
            catch
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.ServerSettings_LoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Disconnect();
                return;
            }
        }

        private void InitializeRAASServerStrings()
        {
            bool raasServerCommunicationSucceeded = false;
            for (int i = 0; i < RAAS_SERVER_SERVICE_RETRY_COUNT; i++)
            {
                try
                {
                    // Communicate with RAAS Server service
                    RAASServerServiceCallback serverServiceCallback = new RAASServerServiceCallback();
                    raasServerServiceClient = new RAASServerServiceClient(new InstanceContext(serverServiceCallback));
                    EndpointAddress endpoint = new EndpointAddress(new Uri(@"net.tcp://" + serverSettings.ServerName + @":43000/RAASServerService"), new DnsEndpointIdentity("localhost"), new AddressHeaderCollection());
                    raasServerServiceClient.Endpoint.Address = endpoint;
                    raasServerServiceClient.ClientCredentials.Windows.ClientCredential.UserName = serverSettings.UserName;
                    raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Password = serverSettings.Password;
                    raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Domain = serverSettings.Domain;
                    raasServerServiceClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                    AutostartPath = raasServerServiceClient.GetAutostartPath();
                    KeepAlivePath = raasServerServiceClient.GetKeepAliveAgentPath();
                    AppNamesPath = raasServerServiceClient.GetAppNamesPath();
                    raasServerServiceClient.Close();
                    raasServerCommunicationSucceeded = true;
                    break;
                }
                catch
                {
                    Thread.Sleep(RAAS_SERVER_SERVICE_RETRY_INTERVAL_MS);
                }
            }
            if (!raasServerCommunicationSucceeded)
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RAASServerService_NoCommunicationMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Disconnect();
                return;
            }
        }

        private void InitializeRemoteAppsService()
        {
            RemoteAppsService.RemoteApplicationEvent += InvokeStartRemoteApplication;
            RemoteAppsService.DisconnectEvent += InvokeDisconnect;
            NetNamedPipeBinding nnpb = new NetNamedPipeBinding();
            nnpb.MaxBufferPoolSize = 20000000;
            nnpb.MaxReceivedMessageSize = 20000000;
            host = new ServiceHost(raasClientService);
            host.AddServiceEndpoint(typeof(IRemoteAppsService), nnpb, new Uri(String.Format(REMOTE_APPS_SERVICE_ADDRESS_FMT, serverName)));
            host.AddServiceEndpoint(
                ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                String.Format(REMOTE_APPS_SERVICE_ADDRESS_FMT, serverName) + "/mex"
            );
            Hide();
            try
            {
                host.Open();
                InvokeConnect();
                connectTime.Start();
            }
            catch
            {
                InvokeDisconnect();
            }
        }

        private void RegisterSessionSwitchEventHandler()
        {
            sessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += sessionSwitchEventHandler;
        }

        private void SetFormProperties()
        {
            Opacity = 0;
            Visible = true;
            ShowInTaskbar = false;
            Size = new Size(0, 0);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (canActivate)
            {
                Visible = true;
                ShowInTaskbar = false;
            }
            else
            {
                Visible = false;
                ShowInTaskbar = false;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                if (canActivate)
                    return false;
                else
                    return true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            storedHwnd = Handle;
            base.OnLoad(e);
        }

        private void RdpOnConnected(object sender, EventArgs e)
        {
            Hide();
            WindowState = FormWindowState.Minimized;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            canActivate = false;
            var hWnd = Handle;
            int style = GetWindowLong(hWnd, (int)GWLParameter.GWL_EXSTYLE);
            SetWindowLong(hWnd, GWLParameter.GWL_EXSTYLE, style | (int)WindowStyles.WS_EX_NOACTIVATE);
            visualizations.UpdateThreads();
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                InvokeDisconnect();
            }
        }

        private void RdpOnLoginComplete(object sender, EventArgs e)
        {
            if (visualizationsAvailable)
            {
                visualizations.UpdateThreads();
                visualizations.ActivateVisualizations();
            }
            if (rdpClient == (AxMSTSCLib.AxMsRdpClient7NotSafeForScripting)sender)
            {
                if (raasClientService.ClientHasSubscribed)
                {
                    if (!raasClientService.HasReportedConnection)
                    {
                        try
                        {
                            // Report that RAAS Client has connected to subscribers
                            raasClientService.ServiceCallback.Connected();
                            raasClientService.HasReportedConnection = true;
                        }
                        catch { }
                    }
                }
                raasClientService.Connected = true;

                // Start listed applications
                for (int i = 0; i < startApplications.Count(); i++)
                {
                    try
                    {
                        rdpClient.RemoteProgram2.ServerStartProgram(startApplications[i].Application, "", "", false, startApplications[i].Arguments, false);
                    }
                    catch { }
                }

                // Start app names
                try
                {
                    rdpClient.RemoteProgram2.ServerStartProgram(AppNamesPath, "", "", false, "", false);
                }
                catch { }

                // Start keep alive agent if keep alive agent is active in settings
                if (serverSettings.KeepAliveAgent)
                    try
                    {
                        rdpClient.RemoteProgram2.ServerStartProgram(KeepAlivePath, "", "", false, "", false);
                    }
                    catch { }
                else
                    try
                    {
                        rdpClient.RemoteProgram2.ServerStartProgram(KeepAlivePath, "", "", false, "-kill", false);
                    }
                    catch { }

                // Start auto start programs application if auto start programs is active in settings
                if (serverSettings.AutoStartPrograms)
                    try
                    {
                        rdpClient.RemoteProgram2.ServerStartProgram(AutostartPath, "", "", false, "", false);
                    }
                    catch { }
            }
            else
                try
                {
                    // Disconnect sender if it is not the intended client
                    ((AxMSTSCLib.AxMsRdpClient7NotSafeForScripting)sender).Disconnect();
                }
                catch { }
        }

        private void RdpOnDisconnect(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            if (visualizationsAvailable)
                visualizations.DeactivateVisualizations();
            if (e.discReason == 0x2 && connectTime.ElapsedMilliseconds <= RECONNECT_INTERVAL_MS)
            {
                BeginInvoke(new connectDelegate(Connect));
            }
            else if (exitOnDisconnect)
                InvokeDisconnect();
        }

        private void RdpOnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
        {
            if (e.lError == 0 && rdpClient == (AxMSTSCLib.AxMsRdpClient7NotSafeForScripting)sender)
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RemoteApplications_LoginFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error) ;
                InvokeDisconnect();
            }
        }

        private void RdpOnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RemoteApplications_FatalErrorMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            InvokeDisconnect();
        }

        private void RdpHandleCreated(object sender, EventArgs e)
        {
        }

        private void InvokeConnect()
        {
            try
            {
                BeginInvoke(new connectDelegate(Connect));
            }
            catch{}
        }

        private void VisualizationsFormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }

        private void StartVisualizations()
        {
            VisualizationsSettings visualizationsSettings;

            // Try to read Visualizations settings
            try
            {
                visualizationsSettings = new VisualizationsSettings(serverName);
            }
            catch
            {
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.VisualizationsSettings_LoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Disconnect();
                return;
            }

            // Start Visualizations if it is active in settings
            if (visualizationsSettings.VisualizationsActive && RAASClientFeatureHelper.VisualizationsInstalled())
            {
                try
                {
                    visualizations = new VisualizationsForm(serverName);
                    visualizations.IgnoreHWnd(Handle);
                    visualizations.FormClosed += new FormClosedEventHandler(VisualizationsFormClosed);
                    visualizationsAvailable = true;
                }
                catch { }
            }

            visualizations.UpdateThreads();
        }

        private void Connect()
        {
            // Remove old rdp client if it exists
            RemoveRdpClient();

            InitializeRdpClient(serverSettings);

            // Connect RDP Client
            rdpClient.Connect();
        }

        private void RdpOnRemoteProgramResult(object sender, IMsTscAxEvents_OnRemoteProgramResultEvent e)
        {
            if (e.lError == 0)
            {
                for (int i = 0; i < startApplications.Count(); i++)
                {
                    try
                    {
                        if (startApplications[i].Application.ToLowerInvariant() == e.bstrRemoteProgram.ToLowerInvariant())
                        {
                            startApplications.Remove(startApplications[i]);
                            break;
                        }
                    }
                    catch { }
                }
            }
        }

        private void InitializeRdpClient(ServerSettings serverSettings)
        {
            // Create RDP client
            rdpClient = new AxMsRdpClient7NotSafeForScripting();

            // Register handlers
            rdpClient.HandleCreated += RdpHandleCreated;
            rdpClient.OnConnected += new EventHandler(RdpOnConnected);
            rdpClient.OnLoginComplete += new EventHandler(RdpOnLoginComplete);
            rdpClient.OnLogonError += new AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEventHandler(RdpOnLogonError);
            rdpClient.OnFatalError += new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(RdpOnFatalError);
            rdpClient.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(RdpOnDisconnect);
            rdpClient.OnRemoteProgramResult += new AxMSTSCLib.IMsTscAxEvents_OnRemoteProgramResultEventHandler(RdpOnRemoteProgramResult);

            Controls.Add(rdpClient);
            InitializeLifetimeService();
            InitLayout();
            rdpClient.CreateControl();
            rdpClient.CreateGraphics();

            // Set settings for RDP client
            IMsRdpClientNonScriptable5 UseMultiSetting = (IMsRdpClientNonScriptable5)(rdpClient.GetOcx());
            UseMultiSetting.UseMultimon = true;
            rdpClient.AdvancedSettings.ContainerHandledFullScreen = 1;
            rdpClient.Server = serverName;
            rdpClient.UserName = serverSettings.UserName;
            rdpClient.Domain = serverSettings.Domain;
            rdpClient.SecuredSettings3.KeyboardHookMode = 0;
            rdpClient.AdvancedSettings7.PerformanceFlags = TS_PERF_ENABLE_ENHANCED_GRAPHICS | TS_PERF_ENABLE_FONT_SMOOTHING;
            rdpClient.AdvancedSettings7.ClearTextPassword = serverSettings.Password;
            rdpClient.AdvancedSettings7.RedirectDrives = false;
            rdpClient.AdvancedSettings7.RedirectPrinters = serverSettings.RedirectPrinters;
            rdpClient.AdvancedSettings7.RedirectSmartCards = false;
            rdpClient.AdvancedSettings7.RedirectClipboard = serverSettings.RedirectClipboard;
            rdpClient.AdvancedSettings7.RedirectDevices = serverSettings.RedirectUsb;
            rdpClient.AdvancedSettings7.RedirectDrives = false;
            rdpClient.AdvancedSettings7.MinutesToIdleTimeout = 0;
            rdpClient.AdvancedSettings7.overallConnectionTimeout = 180;
            rdpClient.AdvancedSettings7.AuthenticationLevel = (uint)serverSettings.AuthenticationLevel;
            rdpClient.AdvancedSettings7.DisableRdpdr = 0;
            rdpClient.AdvancedSettings7.DisplayConnectionBar = true;
            rdpClient.AdvancedSettings7.PublicMode = false;
            rdpClient.AdvancedSettings7.SmartSizing = true;
            rdpClient.AdvancedSettings7.EnableAutoReconnect = false;
            rdpClient.AdvancedSettings7.MaxReconnectAttempts = 0;
            rdpClient.RemoteProgram2.RemoteProgramMode = true;
            rdpClient.AdvancedSettings7.keepAliveInterval = 10000;
            rdpClient.DesktopHeight = SystemInformation.VirtualScreen.Height;
            rdpClient.DesktopWidth = SystemInformation.VirtualScreen.Width;
            rdpClient.FullScreen = false;
            rdpClient.Width = 800;
            rdpClient.Height = 600;
            rdpClient.AdvancedSettings8.EncryptionEnabled = 1;
            rdpClient.Visible = false;
        }

        private void InvokeDisconnect()
        {
            Disconnect();
        }

        private void RemoveRdpClient()
        {
            try
            {
                if (rdpClient != null)
                {
                    try
                    {
                        rdpClient.RequestClose();
                        if (rdpClient.Connected == 1)
                            rdpClient.Disconnect();
                    }
                    catch { }
                    rdpClient = null;
                }
                Application.DoEvents();
            }
            catch { };
        }

        private void Disconnect()
        {
            Environment.Exit(0);
        }

        private void InvokeStartRemoteApplication(String application, String arguments)
        {
            try
            {
                BeginInvoke(new startRemoteApplicationDelegate(StartRemoteApplication), new object[] { application, arguments });
            }
            catch (Exception exc)
            {
                (new System.Threading.Thread(() => {
                    MessageBox.Show(new Form() { TopMost = true }, "invokeStartRemoteApplication error:" + exc.Message, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                })).Start();
            }
        }

        private void StartRemoteApplication(String application, String arguments)
        {
            try
            {
                // Determine if RDP client is connected
                if (rdpClient != null && rdpClient.Connected == 1)
                {
                    // Start application instantly since RDP client is connected
                    try
                    {
                        rdpClient.RemoteProgram2.ServerStartProgram(application, "", "", false, arguments, false);
                    }
                    catch (Exception exc)
                    {
                        (new System.Threading.Thread(() => {
                            MessageBox.Show(new Form() { TopMost = true }, "StartApplication:startprogram:" + exc.Message, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        })).Start();
                    }
                    ApplicationParameters ap = new ApplicationParameters(application, arguments);
                    startApplications.Add(ap);
                }
                else
                {
                    // Queue application for getting started when RDP client get connected
                    try
                    {
                        ApplicationParameters ap = new ApplicationParameters(application, arguments);
                        startApplications.Add(ap);
                    }
                    catch (Exception exc)
                    {
                        (new System.Threading.Thread(() => {
                            MessageBox.Show(new Form() { TopMost = true }, "StartApplication:queue:" + exc.Message, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        })).Start();
                    }
                }
            }
            catch (Exception exc)
            {
                (new System.Threading.Thread(() => {
                    MessageBox.Show(new Form() { TopMost = true }, "StartApplication:catchall" + exc.Message, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                })).Start();
            }
            return;
        }
    }
}
