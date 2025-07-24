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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Threading;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Serilog;
using System.Timers;
using System.Globalization;

namespace Elbitin.Applications.RAAS.RAASClient.RAASClient
{
    public partial class RAASClientForm : Form
    {
        private Dictionary<String, ServerManager> serverManagers = new Dictionary<string, ServerManager>();
        private AboutForm aboutForm = new AboutForm();
        private bool displayChangeMessageEnabled = true;
        private ServiceHost host;
        private FileSystemWatcher xmlServersWatcher;
        private NotifyIcon notifyIcon = new NotifyIcon();
        private RAASClientService raasClient;
        private Process shortcutsProcess;
        private const int timeOutMillis = 50000;
        private ContextMenuStrip serversMenu;
        private Stopwatch timeSinceUnlock = new Stopwatch();
        private Queue balloonTipQueue = new Queue();
        private bool balloonTipInProgress = false;
        private const int OFFSCREEN = 100000;
        private const int CONTEXT_MENU_ICON_SIZE = 16;
        private const int UPDATECONNECTIONSTIMER_INTERVAL_MS = 1000;
        private const int UPDATECONNECTEDTIMER_INTERVAL_MS = 1000;
        private const int SERVERSSUBSCRIBETIMER_INTERVAL_MS = 1000;
        private const int UPDATESHARESTIMER_INTERVAL_MS = 2000;
        private delegate void showMessageDelegate(String message);
        private showMessageDelegate showMessageDel;
        private delegate void replaceContextMenuDelegate();
        private replaceContextMenuDelegate replaceContextMenuDel;
        private delegate void updateContextMenuDelegate();
        private updateContextMenuDelegate updateContextMenuDel;
        private const int BALLOONTIP_TIME_MS = 3000;
        private bool handlingReconnectMessage = false;
        private const String APP_ID = "Elbitin.RAASClient";
        private bool showMessages = true;

        public RAASClientForm()
        {
            InitializeDelegates();
            RegisterServerEventHandlers();
            UpdateServerManagersFromConfig(ref serverManagers);
            aboutForm.Hide();
            SetFormProperties();
            InitializeComponent();
            Hide();
            RAASClientPathHelper.CreateMissingAppDataRAASClientDirectories();
            if (RAASClientFeatureHelper.ShortcutsInstalled())
                StartShortcuts();
            timeSinceUnlock.Start();
            WatchServerConfig();
            RegisterRAASClientServiceEventHandlers();
            StartRAASClientService();
            PrepareNotify();
            RegisterSystemEventHandlers();
            this.HandleCreated += WindowHandleCreated;
        }

        private void WindowHandleCreated(object sender, EventArgs e)
        {
            try
            {
                Invoke(updateContextMenuDel);
            }
            catch { }
        }

        private void InitializeDelegates()
        {
            showMessageDel = new showMessageDelegate(ShowMessage);
            replaceContextMenuDel = new replaceContextMenuDelegate(ReplaceContextMenu);
            updateContextMenuDel = new updateContextMenuDelegate(UpdateContextMenu);
        }

        private void UpdateContextMenu()
        {
            try
            {
                ContextMenuStrip newMenu = CreateServersMenu();
                foreach (ToolStripMenuItem item in notifyIcon.ContextMenuStrip.Items.OfType<ToolStripMenuItem>())
                {
                    foreach (ToolStripMenuItem newItem in newMenu.Items.OfType<ToolStripMenuItem>())
                    {
                        if (newItem.Name == item.Name)
                        {
                            item.Text = newItem.Text;
                            item.Image = newItem.Image;
                            item.Enabled = newItem.Enabled;
                            foreach (ToolStripDropDownItem dropItem in item.DropDownItems)
                            {
                                foreach (ToolStripDropDownItem newDropItem in newItem.DropDownItems)
                                {
                                    if (dropItem.Text == newDropItem.Text)
                                        dropItem.Enabled = newDropItem.Enabled;
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void RegisterSystemEventHandlers()
        {
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplayChange);
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        private void PrepareNotify()
        {
            try
            {
                serversMenu = CreateServersMenu();

                // Configure notify icon with context menu
                notifyIcon.ContextMenuStrip = serversMenu;
                notifyIcon.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
                notifyIcon.Visible = true;
                notifyIcon.Text = "RAAS Client";
            }
            catch { }
        }

        private void StartRAASClientService()
        {
            raasClient = new RAASClientService(ref serverManagers);
            host = new ServiceHost(raasClient);
            host.Open();
        }

        private void RegisterRAASClientServiceEventHandlers()
        {
            RAASClientService.RemoteApplicationEvent += StartRemoteApplication;
            RAASClientService.AutostartApplicationEvent += StartAutostartApplication;
            RAASClientService.KeepAliveApplicationEvent += StartKeepAliveApplication;
            RAASClientService.ShortcutsServerApplicationEvent += StartShortcutsServerApplication;
            RAASClientService.ConnectServerEvent += ConnectServer;
            RAASClientService.DisconnectServerEvent += DisconnectServer;
        }

        private void WatchServerConfig()
        {
            String serversConfigPath = RAASClientPathHelper.GetServersConfigFilePath();
            xmlServersWatcher = new FileSystemWatcher(Path.GetDirectoryName(serversConfigPath), Path.GetFileName(serversConfigPath));
            xmlServersWatcher.IncludeSubdirectories = false;
            xmlServersWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            xmlServersWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.EnableRaisingEvents = true;
        }

        private void SetFormProperties()
        {
            Opacity = 0;
            Visible = false;
            ShowInTaskbar = false;
            Location = new Point(OFFSCREEN, OFFSCREEN);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void RegisterServerEventHandlers()
        {
            ServerManager.FailedMessageEvent += ShowFailedMessage;
            ServerManager.DisconnectedMessageEvent += ShowDisconnectedMessage;
            ServerManager.ConnectedMessageEvent += ShowConnectedMessage;
            ServerManager.StatusChangeEvent += StatusChange;
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams Params = base.CreateParams;
                Params.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE;
                Params.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_TOOLWINDOW;
                return Params;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // Clean up
                Log.CloseAndFlush();
                host.Close();
                notifyIcon.Icon = null;
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                if (RAASClientFeatureHelper.ShortcutsInstalled())
                    CloseShortcuts();
                base.OnFormClosing(e);
                Application.Exit();
            }
            catch { }
        }

        private static void UpdateServerManagersFromConfig(ref Dictionary<String, ServerManager> serverManagers)
        {
            // Update server managers with current settings
            Dictionary<String, ServerSettings> serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
            foreach (String serverName in serverSettings.Keys.ToArray())
            {
                if (serverSettings[serverName].ServerEnabled)
                {
                    if (serverManagers.Keys.Contains(serverName))
                    {
                        bool reconnect = false;
                        if (serverManagers[serverName].ServerSettings.UserName != serverSettings[serverName].UserName || serverManagers[serverName].ServerSettings.Password != serverSettings[serverName].Password || serverManagers[serverName].ServerSettings.Domain != serverSettings[serverName].Domain)
                            reconnect = true;
                        serverManagers[serverName].ServerSettings = serverSettings[serverName];
                        if (reconnect)
                        {
                            serverManagers[serverName].RequireReconnect();
                        }
                        serverManagers[serverName].InvokeConfigureRAASServerServiceClient();
                    }
                    else
                        serverManagers[serverName] = new ServerManager(serverSettings[serverName]);
                }
            }

            // Remove server managers for servers which do not occur in settings
            foreach (String serverName in serverManagers.Keys.ToArray())
                if (!serverSettings.Keys.Contains(serverName) || !serverSettings[serverName].ServerEnabled)
                {
                    serverManagers[serverName].Dispose();
                    serverManagers.Remove(serverName);
                }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                // Do not show display change messages if display changes during lock
                displayChangeMessageEnabled = false;
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                // Show display change messages
                displayChangeMessageEnabled = true;
            }
        }

        private void StartShortcuts()
        {
            shortcutsProcess = RAASClientProgramHelper.StartShortcuts();
        }

        private void CloseShortcuts()
        {
            try
            {
                // Quit shortcuts application if it is alive
                if (!shortcutsProcess.HasExited)
                    ProcessHelper.CloseProcess(shortcutsProcess);
            }
            catch { }
        }

        private void FileSystemWatcher_OnChangeXmlServers(object sender, FileSystemEventArgs e)
        {
            try
            {
                Dictionary<String, ServerSettings> serverSettings;
                try
                {
                    serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
                }
                catch
                {
                    MessageBox.Show(new Form() { TopMost = true }, Properties.Resources.ServerSettings_LoadFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ExitApplication();
                    return;
                }

                // Disconnect disabled servers
                foreach (String serverName in serverSettings.Keys)
                    if (serverManagers.Keys.Contains(serverName) && !serverSettings[serverName].ServerEnabled)
                        serverManagers[serverName].Disconnect();

                // Disconnect servers which do not exist in server settings anymore
                foreach (String serverName in serverManagers.Keys)
                    if (!serverSettings.Keys.Contains(serverName))
                        serverManagers[serverName].Disconnect();

                UpdateServerManagersFromConfig(ref serverManagers);
                ReplaceContextMenu();
            }
            catch { }
        }

        private void ReplaceContextMenu()
        {
            serversMenu = CreateServersMenu();
            notifyIcon.ContextMenuStrip = serversMenu;
        }

        private void ShowFailedMessage(String serverDisplayName)
        {
            try
            {
                Invoke(showMessageDel, String.Format(Properties.Resources.Notification_ConnectionFailedMessage, serverDisplayName));
            }
            catch { }
        }

        private void ShowDisconnectedMessage(String serverDisplayName)
        {
            try
            {
                Invoke(showMessageDel, String.Format(Properties.Resources.Notification_DisconnectedMessage, serverDisplayName));
            }
            catch { }
        }

        private void ShowConnectedMessage(String serverDisplayName)
        {
            try
            {
                Invoke(showMessageDel, String.Format(Properties.Resources.Notification_ConnectedMessage, serverDisplayName));
            }
            catch { }
        }

        private void StatusChange(String serverDisplayName)
        {
            try
            {
                Invoke(updateContextMenuDel);
            }
            catch { }
        }

        private void ToastNotificationExited()
        {
            // Indicate that balloon tip is no longer in progress
            balloonTipInProgress = false;

            // Show next message in queue if one exist
            if (balloonTipQueue.Count > 0)
                Invoke(showMessageDel, (String)balloonTipQueue.Dequeue());
        }

        private void ToastNotificationDismissed(ToastNotification toastNotification, ToastDismissedEventArgs toastDismissedEventArgs)
        {
            ToastNotificationExited();
        }

        private void ToastNotificationFailed(ToastNotification toastNotification, ToastFailedEventArgs toastFailedEventArgs)
        {
            ToastNotificationExited();
        }

        private void ShowMessage(String message)
        {
            if (showMessages)
                try
                {
                    // Display message or enque message to be shown later if ballon tip is in progress
                    if (!balloonTipInProgress)
                    {
                        XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);

                        // Fill in the text elements
                        XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                        for (int i = 0; i < stringElements.Length; i++)
                        {
                            stringElements[i].AppendChild(toastXml.CreateTextNode(message));
                        }

                        // Specify the absolute path to raas client image
                        String imagePath = "file:///" + RAASClientPathHelper.GetRAASClientIconPath();
                        XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                        imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

                        // Create the toast and attach event listeners
                        ToastNotification toast = new ToastNotification(toastXml);
                        //toast.Activated += ToastActivated;
                        toast.Dismissed += ToastNotificationDismissed;
                        toast.Failed += ToastNotificationFailed;

                        // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
                        ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                    }
                    else
                        balloonTipQueue.Enqueue(message);
                }
                catch { }
        }
        
        private void ConnectServer(String serverName)
        {
            try
            {
                serverManagers[serverName].Connect();
            }
            catch { }
        }

        private void DisconnectServer(String serverName)
        {
            try
            {
                serverManagers[serverName].Disconnect();
            }
            catch { }
        }

        private void StartAutostartApplication(String arguments, String serverName)
        {
            try
            {
                serverManagers[serverName].StartAutostartApplication(arguments);
            }
            catch { }
        }

        private void StartKeepAliveApplication(String arguments, String serverName)
        {
            try
            {
                serverManagers[serverName].StartKeepAliveApplication(arguments);
            }
            catch { }
        }

        private void StartShortcutsServerApplication(String arguments, String serverName)
        {
            try
            {
                serverManagers[serverName].StartShortcutsServerApplication(arguments);
            }
            catch { }
        }

        private void StartRemoteApplication(String application, String arguments, String serverName)
        {
            try
            {
                serverManagers[serverName].StartRemoteApplication(application, arguments);
            }
            catch { }
        }

        private ContextMenuStrip CreateServersMenu()
        {
            // Clear any existing notify meny entries
            ContextMenuStrip newServersMeny = new ContextMenuStrip();

            // Determine RAAS Server alias for all existing servers
            List<String> notifyServers = new List<String>();
            Dictionary<String, String> notifyServerDisplayNames = new Dictionary<String, String>();
            foreach (String serverName in serverManagers.Keys)
            {
                notifyServerDisplayNames[serverName] = serverManagers[serverName].ServerSettings.Alias;
            }

            // Order servers by display names
            IOrderedEnumerable<System.Collections.Generic.KeyValuePair<string, string>> sortedDisplayNames = from entry in notifyServerDisplayNames orderby entry.Value ascending select entry;

            // Init bitmaps
            Bitmap serverConfigurationBitmap = new Bitmap(Properties.Resources.Serverconfig); ;
            Bitmap rdesktopBitmap = new Bitmap(Properties.Resources.Rdesktop);
            Bitmap helpBitmap = new Bitmap(Properties.Resources.Help);
            Bitmap aboutBitmap = new Bitmap(Properties.Resources.About);
            Bitmap exitBitmap = new Bitmap(Properties.Resources.Exit);
            Bitmap connectBitmap = new Bitmap(Properties.Resources.Connect);
            Bitmap disconnectBitmap = new Bitmap(Properties.Resources.Disconnect);
            Bitmap reconnectBitmap = new Bitmap(Properties.Resources.Reconnect);
            Bitmap logoutBitmap = new Bitmap(Properties.Resources.Logout);
            Bitmap rebootBitmap = new Bitmap(Properties.Resources.Reboot);
            Bitmap shortcutsBitmap = new Bitmap(Properties.Resources.Shortcuts);

            // Add menu items for all enabled servers and determine which drop down menu items should be active
            foreach (KeyValuePair<String, String> server in sortedDisplayNames)
            {
                try
                {
                    ToolStripMenuItem serverItem = new ToolStripMenuItem();
                    Bitmap bitmap;
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_ConnectItem, connectBitmap, new EventHandler(ConnectFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_DisconnectItem, disconnectBitmap, new EventHandler(DisconnectFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_ReconnectItem, reconnectBitmap, new EventHandler(ConnectFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_LogoffItem, logoutBitmap, new EventHandler(LogOffFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_RebootItem, rebootBitmap, new EventHandler(RebootFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ContextMenu_UpdateShortcutstItem, shortcutsBitmap, new EventHandler(UpdateShortcutsFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ConextMenu_RemoteDesktopItem, rdesktopBitmap, new EventHandler(RemoteDesktopFromMenu));
                    serverItem.DropDownItems.Add("&" + Properties.Resources.ConextMenu_RAASServerConfigurationItem, serverConfigurationBitmap, new EventHandler(RAASServerConfigFromMenu));
                    ServerStatus serverStatus = serverManagers[server.Key].ServerStates.GetServerStatus();
                    if (serverStatus == ServerStatus.Connected)
                    {
                        bitmap = CreateServerIconBitmap("#ff00ff00");
                        serverItem.Text = String.Format(Properties.Resources.ContextMenu_ServerItem, server.Value, Properties.Resources.ContextMenu_ConnectedSubString);
                        foreach (ToolStripMenuItem toolItem in serverItem.DropDownItems)
                        {
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ConnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_DisconnectItem)
                                toolItem.Enabled = true;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ReconnectItem)
                                toolItem.Enabled = true;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_LogoffItem)
                                toolItem.Enabled = serverManagers[server.Key].ServerStates.RAASServerCanLogOff;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_RebootItem)
                                toolItem.Enabled = true;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_UpdateShortcutstItem)
                                toolItem.Enabled = true;
                        }
                    }
                    else if (serverStatus == ServerStatus.Available)
                    {
                        bitmap = CreateServerIconBitmap("#ffffff00");
                        serverItem.Text = serverItem.Text = String.Format(Properties.Resources.ContextMenu_ServerItem, server.Value, Properties.Resources.ContextMenu_AvailableSubString);
                        foreach (ToolStripMenuItem toolItem in serverItem.DropDownItems)
                        {
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ConnectItem)
                                toolItem.Enabled = true;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_DisconnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ReconnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_LogoffItem)
                                toolItem.Enabled = serverManagers[server.Key].ServerStates.RAASServerCanLogOff;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_RebootItem)
                                toolItem.Enabled = serverManagers[server.Key].ServerStates.RAASServerCanReboot;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_UpdateShortcutstItem)
                                toolItem.Enabled = true;
                        }
                    }
                    else
                    {
                        bitmap = CreateServerIconBitmap("#ffff0000");
                        serverItem.Text = serverItem.Text = String.Format(Properties.Resources.ContextMenu_ServerItem, server.Value, Properties.Resources.ContextMenu_OfflineSubString);
                        foreach (ToolStripMenuItem toolItem in serverItem.DropDownItems)
                        {
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ConnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_DisconnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_ReconnectItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_LogoffItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_RebootItem)
                                toolItem.Enabled = false;
                            if (toolItem.Text == "&" + Properties.Resources.ContextMenu_UpdateShortcutstItem)
                                toolItem.Enabled = false;
                        }
                    }
                    serverItem.Image = bitmap;
                    serverItem.Name = server.Key;
                    newServersMeny.Items.Add(serverItem);
                }
                catch { }
            }

            // Add separator if there exists server items
            if (newServersMeny.Items.Count > 0)
            {
                ToolStripSeparator separator = new ToolStripSeparator();
                separator.Name = "Separator";
                newServersMeny.Items.Add(separator);
            }

            // Add root menu items which should exist regardless of servers
            String remoteDesktopIconLocation = Path.Combine(Environment.SystemDirectory, "mstscax.dll");
            ToolStripMenuItem remoteDesktopItem = new ToolStripMenuItem();
            remoteDesktopItem.Name = "RemoteDesktop";
            remoteDesktopItem.Text = "&" + Properties.Resources.ConextMenu_RemoteDesktopItem;
            remoteDesktopItem.Image = rdesktopBitmap;
            remoteDesktopItem.Click += new EventHandler(StartRemoteDesktop);
            newServersMeny.Items.Add(remoteDesktopItem);
            ToolStripMenuItem serverConfigurationItem = new ToolStripMenuItem();
            serverConfigurationItem.Name = "ServerConfiguration";
            serverConfigurationItem.Text = "&" + Properties.Resources.ConextMenu_RAASServerConfigurationItem;
            serverConfigurationItem.Image = serverConfigurationBitmap;
            serverConfigurationItem.Click += new EventHandler(StartServerConfig);
            newServersMeny.Items.Add(serverConfigurationItem);
            ToolStripMenuItem helpItem = new ToolStripMenuItem();
            helpItem.Name = "Help";
            helpItem.Text = "&" + Properties.Resources.ContextMenu_HelpItem;
            helpItem.Image = helpBitmap;
            helpItem.Click += new EventHandler(ShowHelp);
            newServersMeny.Items.Add(helpItem);
            ToolStripMenuItem aboutItem = new ToolStripMenuItem();
            aboutItem.Name = "About";
            aboutItem.Text = "&" + Properties.Resources.ContextMenu_AboutItem;
            aboutItem.Image = aboutBitmap;
            aboutItem.Click += new EventHandler(ShowAbout);
            newServersMeny.Items.Add(aboutItem);
            ToolStripMenuItem exitItem = new ToolStripMenuItem();
            exitItem.Name = "Exit";
            exitItem.Text = "&" + Properties.Resources.ContextMenu_ExitItem;
            exitItem.Image = exitBitmap;
            exitItem.Click += new EventHandler(CloseApplication);
            newServersMeny.Items.Add(exitItem);
            newServersMeny.Opened += new EventHandler(ContextMenuOpening);
            return newServersMeny;
        }

        private Bitmap CreateServerIconBitmap(string serverIconColor)
        {
            Bitmap bitmap = new Bitmap(20, 20);
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    using (Brush b = new SolidBrush(ColorTranslator.FromHtml(serverIconColor)))
                    {
                        g.FillEllipse(b, 5, 5, 10, 10);
                    }
                    using (Pen p = new Pen(ColorTranslator.FromHtml("#ff000000")))
                    {
                        g.DrawEllipse(p, 5, 5, 10, 10);
                    }
                }
            }
            return bitmap;
        }

        private void RemoteDesktopFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                RAASClientProgramHelper.StartRDesktop(serverName);
            }
            catch { }
        }

        private void RAASServerConfigFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                RAASClientProgramHelper.StartServerConfig(serverName);
            }
            catch { }
        }

        private void ConnectFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                Thread thread = new Thread(() => Connect(serverName));
                thread.Start();
            }
            catch { }
        }

        private void DisconnectFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                Thread thread = new Thread(() => Disconnect(serverName));
                thread.Start();
            }
            catch { }
        }

        private void RebootFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                Thread thread = new Thread(() => Reboot(serverName));
                thread.Start();
            }
            catch { }
        }

        private void UpdateShortcuts(String serverName)
        {
            try
            {
                if (!serverManagers[serverName].UpdateShortcuts())
                {
                    // Inform the user that reboot failed
                    String serverDisplayName = serverManagers[serverName].ServerSettings.Alias;
                    (new System.Threading.Thread(() => {
                        MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.Actions_UpdateShortcutsFailedMessage, serverDisplayName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    })).Start();
                }
            }
            catch { }
        }

        private void UpdateShortcutsFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                Thread thread = new Thread(() => UpdateShortcuts(serverName));
                thread.Start();
            }
            catch { }
        }

        private void Disconnect(String serverName)
        {
            try
            {
                serverManagers[serverName].Disconnect();
            }
            catch { }
        }

        private void Connect(String serverName)
        {
            try
            {
                serverManagers[serverName].Connect();
            }
            catch { }
        }

        private void Reboot(String serverName)
        {
            try
            {
                if (!serverManagers[serverName].Reboot())
                {
                    // Inform the user that reboot failed
                    String serverDisplayName = serverManagers[serverName].ServerSettings.Alias;
                    (new System.Threading.Thread(() => {
                        MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.Actions_RebootFailedMessage, serverDisplayName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    })).Start();
                }
            }
            catch { }
        }

        private void LogOffFromMenu(object sender, EventArgs e)
        {
            try
            {
                String serverName = ((ToolStripMenuItem)sender).OwnerItem.Name;
                Thread thread = new Thread(() => LogOff(serverName));
                thread.Start();
            }
            catch { }
        }

        private void LogOff(String serverName)
        {
            try
            {
                if (!serverManagers[serverName].LogOff())
                {
                    // Inform the user that log off failed
                    String serverDisplayName = serverManagers[serverName].ServerSettings.Alias;
                    (new System.Threading.Thread(() => {
                        MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.Actions_LogOffFailedMessage, serverDisplayName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    })).Start();
                }
            }
            catch { }
        }

        private void ShowHelp(object sender, EventArgs e)
        {
            // Open help pdf
            System.Diagnostics.Process.Start(RAASClientPathHelper.GetHelpPdfPath());
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            // Show about dialog in center of screen and in front of all windows
            aboutForm.CenterForm();
            aboutForm.BringToFront();
            aboutForm.Show();
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void ContextMenuOpening(object sender, EventArgs e)
        {
            UpdateContextMenu();
        }

        private void StartRemoteDesktop(object sender, EventArgs e)
        {
            RAASClientProgramHelper.StartRDesktop();
        }

        private void StartServerConfig(object sender, EventArgs e)
        {
            RAASClientProgramHelper.StartServerConfig();
        }

        private void ExitApplication()
        {
            try
            {
                // Do not show messages
                showMessages = false;

                if (RAASClientFeatureHelper.ShortcutsInstalled())
                    // Close shortuts application
                    CloseShortcuts();

                // Clean up
                host.Close();
                notifyIcon.Icon = null;
                notifyIcon.Visible = false;
                notifyIcon.Dispose();

                // Disconnect all RAAS Clients
                foreach (String serverName in serverManagers.Keys)
                {
                    try
                    {
                        serverManagers[serverName].Disconnect();
                    }
                    catch { }
                }

                // Exit RAAS
                Application.Exit();
            }
            catch { }
        }

        private void SystemEvents_DisplayChange(object sender, EventArgs e)
        {
            // Ask user only once if all RAAS Clients should be reconnected if state indicates that the message should be shown
            if (displayChangeMessageEnabled && !handlingReconnectMessage)
            {
                try
                {
                    handlingReconnectMessage = true;
                    bool reconnect = false;
                    foreach (String serverName in serverManagers.Keys.ToArray())
                    {
                        try
                        {
                            if (serverManagers[serverName].ServerStates.GetServerStatus() == ServerStatus.Connected)
                                reconnect = true;
                        }
                        catch { }
                    }
                    if (reconnect)
                    {
                        Dictionary<String, bool> reconnectServer = new Dictionary<String, bool>();
                        foreach (String serverName in serverManagers.Keys.ToArray())
                        {
                            try
                            {
                                if (serverManagers[serverName].ServerStates.GetServerStatus() == ServerStatus.Connected)
                                    reconnectServer[serverName] = true;
                            }
                            catch { }
                        }
                        foreach (String serverName in serverManagers.Keys.ToArray())
                        {
                            try
                            {
                                if (reconnectServer.Keys.Contains(serverName) && reconnectServer[serverName])
                                    serverManagers[serverName].Connect();
                            }
                            catch { }
                        }
                    }
                }
                finally
                {
                    handlingReconnectMessage = false;
                }
            }
        }
    }
}
