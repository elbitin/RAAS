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
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using Elbitin.Applications.RAAS.RAASClient.ServerCfg.RAASClientServiceRef;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Media;
using System.IO;
using System.Diagnostics;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASClient.ServerCfg
{
    public partial class ServerConfigForm : Form
    {
        private String serverName;
        private RAASClientServiceClient raasServiceClient = new RAASClientServiceClient();
        private bool serverSettingsAreDirty = false;
        private bool shortcutsSettingsAreDirty = false;
        private bool visualizationsSettingsAreDirty = false;
        private bool explorerSettingsAreDirty = false;
        private bool keepAliveAgentDirty = false;
        private bool autostartProgramsDirty = false;
        private int status = (int)ServerStatus.NoContact;
        private bool connected = false;
        private bool reconnectRequiredOnConnected = false;
        private System.Timers.Timer updateTimer = new System.Timers.Timer();
        private bool versionShowing = false;
        private bool serverSupplied = false;
        private FileSystemWatcher xmlServersWatcher;
        private ServerSettings serverSettings;
        private VisualizationsSettings visualizationsSettings;
        private ExplorerExtensionSettings explorerExtensionSettings;
        private ErrorProvider aliasErrorProvider = new ErrorProvider();
        private ErrorProvider accountErrorProvider = new ErrorProvider();
        private ErrorProvider passwordErrorProvider = new ErrorProvider();
        private const int UPDATETIMER_INTERVAL_MS = 1000;
        private const int ALIAS_MAX_LENGTH = 20;
        private const int ALIAS_TOOLTIP_DURATION = 6000;
        private bool serversComboboxMonitorChange = false;
        private Dictionary<String, int> serverIndexes;
        private delegate void updateAvailableServersDelegate();
        private checkRAASConnectionDelegate callbackHandlerCheckRAASConnection;
        private TableLayoutPanel fixedSizePanel = new TableLayoutPanel();
        private delegate void checkRAASConnectionDelegate();
        private delegate void clearVersionDelegate();
        private delegate void setVersionDelegate(String version);
        private delegate void setConnectedDelegate();
        private delegate void setAvailableDelegate();
        private delegate void setContactDelegate();
        private delegate void setNoMessageDelegate();
        private delegate void setNoServerDelegate();
        private delegate void setNoContactDelegate();
        private delegate void setRAASNotRespondingDelegate();

        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public ServerConfigForm()
        {
            InitializeComponent();
            serverSupplied = false;
            InitializeServerConfig();
        }

        public ServerConfigForm(string serverPath)
        {
            InitializeComponent();
            serverName = serverPath.Split('\\').Last().ToUpperInvariant();
            Dictionary<String, ServerSettings> currentServers;
            try
            {
                currentServers = ServerSettingsHelper.GetServerSettingsFromConfig();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ServerSettings_LoadFailedMessage, null, MessageBoxButtons.OK);
                Application.Exit();
                return;
            }
            if (!currentServers.Keys.Contains(serverName.ToUpperInvariant()))
            {
                DialogResult addServerResponse = MessageBox.Show(String.Format(Properties.Resources.Server_NotConfiguredMessage, serverName), Properties.Resources.Server_NotConfiguredMessageTitle, MessageBoxButtons.YesNoCancel);
                if (addServerResponse == System.Windows.Forms.DialogResult.Yes)
                {
                    AddServer(serverName.ToUpperInvariant());
                    serverSupplied = true;
                    InitializeServerConfig(serverName);
                }
                else if (addServerResponse == System.Windows.Forms.DialogResult.No)
                {
                    serverSupplied = false;
                    InitializeServerConfig();
                }
                else
                    Environment.Exit(0);
            }
            else
            {
                serverSupplied = true;
                InitializeServerConfig(serverName);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            WatchServersConfig();
            UpdateAvailableServers();
            base.OnHandleCreated(e);
        }

        private void FileSystemWatcher_OnChangeXmlServers(object sender, FileSystemEventArgs e)
        {
            Invoke(new updateAvailableServersDelegate(UpdateAvailableServers));
        }

        private void WatchServersConfig()
        {
            // Watch changes in servers config
            String serversConfigPath = RAASClientPathHelper.GetServersConfigFilePath();
            xmlServersWatcher = new FileSystemWatcher(Path.GetDirectoryName(serversConfigPath), Path.GetFileName(serversConfigPath));
            xmlServersWatcher.IncludeSubdirectories = false;
            xmlServersWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            xmlServersWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnChangeXmlServers);
            xmlServersWatcher.EnableRaisingEvents = true;
        }

        private void InitializeServerConfig(String serverName)
        {
            this.serverName = serverName;
            status = (int)ServerStatus.NoContact;

            // Read settings
            try
            {
                serverSettings = new ServerSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.ServerSettings_ServerLoadFailedMessage,  serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                visualizationsSettings = new VisualizationsSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.VisualizationsSettings_ServerLoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                explorerExtensionSettings = new ExplorerExtensionSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.VisualizationsSettings_ServerLoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Initialize
            InitializeServerConfig();
        }

        private string ExtractStringFromDLL(string file, int number)
        {
            IntPtr lib = Win32Helper.LoadLibrary(file);
            StringBuilder result = new StringBuilder(2048);
            Win32Helper.LoadString(lib, number, result, result.Capacity);
            Win32Helper.FreeLibrary(lib);
            return result.ToString();
        }

        private void InitializeLocalizedStrings()
        {
            this.cancelButton.Text = Properties.Resources.MainForm_CancelButton;
            this.okButton.Text = Properties.Resources.MainForm_OkButton;
            this.accountLabel.Text = Properties.Resources.General_AccountLabel;
            this.applyButton.Text = Properties.Resources.MainForm_ApplyButton;
            this.passwordLabel.Text = Properties.Resources.General_PasswordLabel;
            this.settingsGroupBox.Text = Properties.Resources.General_SettingsGroupBox;
            this.aliasLabel.Text = Properties.Resources.General_AliasLabel;
            this.notificationsCheckBox.Text = Properties.Resources.General_ShowNotificationsCheckBox;
            this.printersCheckBox.Text = Properties.Resources.General_RedirectPrintersCheckBox;
            this.keepAliveCheckBox.Text = Properties.Resources.General_KeepAliveAgentCheckBox;
            this.usbCheckBox.Text = Properties.Resources.General_RedirectDevicesCheckBox;
            this.clipboardCheckBox.Text = Properties.Resources.General_RedirectClipboardCheckBox;
            this.autoReconnectCheckBox.Text = Properties.Resources.General_AutoReconnectCheckBox;
            this.domainLabel.Text = Properties.Resources.General_DomainLabel;
            this.autostartProgramsCheckBox.Text = Properties.Resources.General_AutoStartProgramsCheckBox;
            this.authenticationLabel.Text = Properties.Resources.General_RDPAuthenticationLabel;
            this.enableServerCheckBox.Text = Properties.Resources.General_EnableServerCheckBox;
            this.serverConnectionGroupBox.Text = Properties.Resources.Connection_ServerConnectionGroupBox;
            this.connectButton.Text = Properties.Resources.Connection_ConnectButton;
            this.disconnectButton.Text = Properties.Resources.Connection_DisconnectButton;
            this.statusLabel.Text = Properties.Resources.Connection_StatusLabel;
            this.serverVersionLabel.Text = Properties.Resources.Connection_ServerVersionLabel;
            this.generalTab.Text = Properties.Resources.General_GeneralTab;
            this.remoteShortcutsTab.Text = Properties.Resources.RemoteShortcuts_RemoteShortcutsTab;
            this.enableShortcutsCheckBox.Text = Properties.Resources.RemoteShortcuts_EnableRemoteShortcutsCheckBox;
            this.shortcutsGroupBox.Text = Properties.Resources.RemoteShortcuts_RemoteShortcutsSettingsGroupBox;
            this.startMenuShortcutsCheckBox.Text = Properties.Resources.RemoteShortcuts_StartMenuCheckBox;
            this.uwpApplicationShortcutsCheckBox.Text = Properties.Resources.RemoteShortcuts_UWPApplicationCheckBox;
            this.shortcutsAppendAliasCheckBox.Text = Properties.Resources.RemoteShortcuts_AppendAliasCheckBox;
            this.desktopShortcutsCheckBox.Text = Properties.Resources.RemoteShortcuts_DesktopCheckBox;
            this.desktopRootCheckBox.Text = Properties.Resources.RemoteShortcuts_DesktopRootCheckBox;
            this.localizeShortcutsCheckBox.Text = Properties.Resources.RemoteShortcuts_LocalizeShortcutsCheckBox;
            this.visualizationsTab.Text = Properties.Resources.Visualizations_VisualizationsTab;
            this.visualizationsCheckBox.Text = Properties.Resources.Visualizations_EnableVisualizationsCheckBox;
            this.visualizationsSettingsGroupBox.Text = Properties.Resources.Visualizations_VisualizationsGroupBox;
            this.mainColorButton.Text = Properties.Resources.Visualizations_MainColorButton;
            this.connectionBarCheckBox.Text = Properties.Resources.Visualizations_ConnectionBarCheckBox;
            this.connectionOpacityLabel.Text = Properties.Resources.Visualizations_ConnectionBarOpacityLabel;
            this.textColorButton.Text = Properties.Resources.Visualizations_TextColorButton;
            this.lineColorButton.Text = Properties.Resources.Visualizations_LineColorButton;
            this.framesCheckBox.Text = Properties.Resources.Visualizations_FramesCheckBox;
            this.crosslinesCheckBox.Text = Properties.Resources.Visualizations_CrossLinesCheckBox;
            this.sharesTab.Text = Properties.Resources.ExplorerExtension_ExplorerExtensionTab;
            this.explorerCheckBox.Text = Properties.Resources.ExplorerExtension_EnableExplorerExtensionCheckBox;
            this.explorerGroupBox.Text = Properties.Resources.ExplorerExtension_VisibleFoldersGroupBox;
            this.devicesAndDrivesGroupBox.Text = Properties.Resources.ExplorerExtension_DevicesAndDrivesGroupBox;
            this.removableDrivesCheckBox.Text = Properties.Resources.ExplorerExtension_RemovableDrivesCheckBox;
            this.cdDrivesCheckBox.Text = Properties.Resources.ExplorerExtension_CDDrivesCheckBox;
            this.downloadsCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21798);
            this.hardDrivesCheckBox.Text = Properties.Resources.ExplorerExtension_HardDrivesCheckBox;
            this.desktopCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21769); ;
            this.disketteDrivesCheckBox.Text = Properties.Resources.ExplorerExtension_FloppyDiskDrivesCheckBox;
            this.picturesCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21779); ;
            this.videosCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21791);
            this.documentsCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21770);
            this.favoritesCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21796);
            this.savedGamesCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21814);
            this.linksCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21810);
            this.musicCheckBox.Text = ExtractStringFromDLL("shell32.dll", 21790);
            this.serverLabel.Text = Properties.Resources.MainForm_ServerLabel;
            this.removeButton.Text = Properties.Resources.MainForm_RemoveButton;
            this.addButton.Text = Properties.Resources.MainForm_AddButton;
            this.Text = Properties.Resources.MainForm_MainFormTitle;
            ComboboxItem cbi1 = new ComboboxItem();
            cbi1.Text = Properties.Resources.General_AuthenticationRequiredOption;
            cbi1.Value = 1;
            ComboboxItem cbi2 = new ComboboxItem();
            cbi2.Text = Properties.Resources.General_AuthenticationWarningOption;
            cbi2.Value = 0;
            authenticationLevelComboBox.Items.Clear();
            authenticationLevelComboBox.Items.Add(cbi1);
            authenticationLevelComboBox.Items.Add(cbi2);
            reorderVisibleFoldersAlphabetically();
        }

        private void reorderVisibleFoldersAlphabetically()
        {
            List<CheckBox> folderCheckBoxes = new List<CheckBox>();
            folderCheckBoxes.Add(desktopCheckBox);
            folderCheckBoxes.Add(documentsCheckBox);
            folderCheckBoxes.Add(downloadsCheckBox);
            folderCheckBoxes.Add(favoritesCheckBox);
            folderCheckBoxes.Add(linksCheckBox);
            folderCheckBoxes.Add(musicCheckBox);
            folderCheckBoxes.Add(picturesCheckBox);
            folderCheckBoxes.Add(savedGamesCheckBox);
            folderCheckBoxes.Add(videosCheckBox);
            List<CheckBox> orderedFolderCheckBoxes = folderCheckBoxes.OrderBy(o => o.Text).ToList();
            for (int i = 0; i < orderedFolderCheckBoxes.Count(); i++)
            {
                orderedFolderCheckBoxes[i].TabIndex = i + 1;
                visibleFoldersTableLayoutPanel.Controls.Add(orderedFolderCheckBoxes[i], 0, i);
            }
        }

        private void InitializeServerConfig()
        {
            RAASClientPathHelper.CreateMissingAppDataRAASClientDirectories();
            InitializeLocalizedStrings();

            // Clear all errors
            aliasErrorProvider.Clear();
            accountErrorProvider.Clear();
            passwordErrorProvider.Clear();

            // Set initial states of components
            enableServerCheckBox.Checked = false;
            serverVersionLabel.Visible = false;
            serverVersionText.Visible = false;
            SetTitle();
            versionShowing = false;
            if (!serverSupplied)
            {
                removeButton.Enabled = false;
                tabControl.TabStop = false;
                tabControl.Visible = false;
                SetNoServer();
            }
            else
            {
                removeButton.Enabled = true;
                tabControl.TabStop = true;
                tabControl.Visible = true;

                // Update components according to settings
                UpdateExplorerExtensionComponents();
                UpdateVisualizationsComponents();
                UpdateServerComponents();

                SetNoMessage();
            }
            NothingDirty();
            UpdateAvailableServers();
            UpdateEnabledStates();

            ConnectRAAS();
        }

        private void NothingDirty()
        {
            // Set application states
            serverSettingsAreDirty = false;
            shortcutsSettingsAreDirty = false;
            explorerSettingsAreDirty = false;
            keepAliveAgentDirty = false;
            autostartProgramsDirty = false;
            visualizationsSettingsAreDirty = false;
            reconnectRequiredOnConnected = false;
        }

        private void AddServer(String newServerName)
        {
            RAASClientPathHelper.CreateMissingServerAppDataRAASDirectories(newServerName);

            // Read settings
            serverSettings = new ServerSettings(newServerName);
            visualizationsSettings = new VisualizationsSettings(newServerName);
            explorerExtensionSettings = new ExplorerExtensionSettings(newServerName);

            // Save settings
            try
            {
                serverSettings.SaveServerSettings();
            }
            catch (AliasAlreadyExistException /* e */)
            {
                MessageBox.Show(null, Properties.Resources.ServerSettings_AliasOccupiedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                aliasErrorProvider.SetError(aliasTextBox, Properties.Resources.ServerSettings_AliasOccupiedErrorInfo);
            }
            catch
            {
                MessageBox.Show(null, Properties.Resources.ServerSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            visualizationsSettings.SaveVisualizationsSettings();
            explorerExtensionSettings.SaveExplorerExtensionSettings();
        }

        protected override void OnShown(EventArgs e)
        {
            callbackHandlerCheckRAASConnection = new checkRAASConnectionDelegate(CheckRAASConnection);
            if (serverSupplied)
                callbackHandlerCheckRAASConnection.Invoke();
            updateTimer.Interval = UPDATETIMER_INTERVAL_MS;
            updateTimer.AutoReset = false;
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Enabled = true;
            updateTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try
            {
                raasServiceClient.Abort();
            }
            catch { }
            Application.Exit();
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateTimer.Stop();
            if (serverSupplied)
                try
                {
                    callbackHandlerCheckRAASConnection.Invoke();
                }
                catch { }
            updateTimer.Start();
        }

        private void UpdateAvailableServers()
        {
            serversComboboxMonitorChange = false;
            serversComboBox.Items.Clear();
            Dictionary<String, ServerSettings> servers;
            try
            {
                servers = ServerSettingsHelper.GetServerSettingsFromConfig();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ServerSettings_LoadFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            IEnumerable<String> sortedServerNames = servers.Keys.OrderBy(server => servers[server].ServerName);
            serverIndexes = new Dictionary<string, int>();
            int serverIndex = 0;
            if (serverSupplied)
            {
                bool serverExist = servers.Keys.Contains(serverName);
                if (!serverExist)
                {
                    MessageBox.Show(String.Format(Properties.Resources.Server_RemovedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    serverSupplied = false;
                    InitializeServerConfig();
                }
            }
            if (!serverSupplied)
            {
                ComboboxItem cbServer = new ComboboxItem();
                cbServer.Text = "";
                cbServer.Value = serverIndex;
                serversComboBox.Items.Add(cbServer);
                serverIndex++;
            }
            foreach (String serverName in sortedServerNames)
            {
                ComboboxItem cbServer = new ComboboxItem();
                if (servers[serverName].Alias.Length > 0)
                    cbServer.Text = serverName + " (" + servers[serverName].Alias + ")";
                else
                    cbServer.Text = serverName;
                cbServer.Value = serverIndex;
                serverIndexes[serverName] = serverIndex;
                serversComboBox.Items.Add(cbServer);
                serverIndex++;
            }
            if (!serverSupplied)
                serversComboBox.SelectedIndex = 0;
            else
                serversComboBox.SelectedIndex = serverIndexes[serverName];
            serversComboboxMonitorChange = true;
        }

        private void SetServerSettingsDirty()
        {
            serverSettingsAreDirty = true;
            applyButton.Enabled = true;
        }

        private void SetShortcutsSettingsDirty()
        {
            shortcutsSettingsAreDirty = true;
            applyButton.Enabled = true;
        }

        private void SetVisualizationsSettingsDirty()
        {
            visualizationsSettingsAreDirty = true;
            applyButton.Enabled = true;
        }

        private void SetExplorerSettingsDirty()
        {
            explorerSettingsAreDirty = true;
            applyButton.Enabled = true;
        }

        private bool Apply()
        {
            RAASClientPathHelper.CreateMissingServerAppDataRAASDirectories(serverName);

            if (!ValidateForm())
                return false;

            // Try to save all setting which have changed
            if (serverSettingsAreDirty)
            {
                if(!TryUpdateSeverSettings())
                    return false;
            }
            if (shortcutsSettingsAreDirty || serverSettingsAreDirty)
            {
                UpdateShortcuts();
            }
            if (visualizationsSettingsAreDirty)
                if (!TryUpdateVisualizationsSettings())
                    return false;
            if (explorerSettingsAreDirty)
                if (!TryUpdateExplorerSettings())
                    return false;

            // Update keep alive agent application if needed
            if (keepAliveAgentDirty)
                if (status == (int)ServerStatus.Connected)
                    UpdateKeepAliveAgent();

            // Update autostart programs application if needed
            if (autostartProgramsDirty)
                if (status == (int)ServerStatus.Connected)
                    if (autostartProgramsCheckBox.Checked)
                        UpdateAutostart();

            if (!enableServerCheckBox.Checked)
            {
                // Disconnect if server is connected but no longer enabled
                if (connected)
                    DisconnectServer();
            }
            else if (reconnectRequiredOnConnected)
            {
                // Question the user if a reconnect should take place
                if (connected && enableServerCheckBox.Checked)
                {
                    DialogResult reconnetNow = MessageBox.Show(Properties.Resources.ServerSettings_ReconnectRequiredMessage, Properties.Resources.ServerSettings_ReconnectRequiredMessageTitle, MessageBoxButtons.YesNo);
                    if (reconnetNow == System.Windows.Forms.DialogResult.Yes)
                    {
                        ReconnectServer();
                    }
                }
            }

            // Set application states
            NothingDirty();
            UpdateEnabledStates();
            reconnectRequiredOnConnected = false;
            UpdateAvailableServers();

            return true;
        }

        private static void UpdateShortcuts()
        {
            Process shortcuts = RAASClientProgramHelper.StartShortcuts("-update");
            shortcuts.WaitForExit();
        }

        private void ReconnectServer()
        {
            try
            {
                raasServiceClient.Abort();
                raasServiceClient = new RAASClientServiceClient();
                raasServiceClient.ConnectServer(serverName);
            }
            catch { }
        }

        private void DisconnectServer()
        {
            try
            {
                raasServiceClient.DisconnectServer(serverName);
            }
            catch { }
        }

        private void UpdateAutostart()
        {
            try
            {
                raasServiceClient.StartAutostart("", serverName);
            }
            catch { }
        }

        private void UpdateKeepAliveAgent()
        {
            if (keepAliveCheckBox.Checked)
            {
                try
                {
                    raasServiceClient.StartKeepAlive("", serverName);
                }
                catch { }
            }
            else
            {
                try
                {
                    raasServiceClient.StartKeepAlive("-kill", serverName);
                }
                catch { }
            }
        }

        private bool TryUpdateExplorerSettings()
        {
            try
            {
                UpdateExplorerSettings();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ExplorerSettings_SaveFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool TryUpdateVisualizationsSettings()
        {
            try
            {
                UpdateVisualizationsSettings();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.VisualizationsSettings_SaveFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool TryUpdateSeverSettings()
        {
            try
            {
                UpdateServerSettings();
            }
            catch (Exception exc)
            {
                if (exc is AliasAlreadyExistException)
                {
                    MessageBox.Show(Properties.Resources.ServerSettings_SaveFailedAliasOccupiedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    aliasErrorProvider.SetError(aliasTextBox, Properties.Resources.ServerSettings_AliasOccupiedErrorInfo);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ServerSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            return true;
        }

        private bool ValidateForm()
        {

            // Clear error providers
            aliasErrorProvider.Clear();
            accountErrorProvider.Clear();
            passwordErrorProvider.Clear();

            if (enableServerCheckBox.Checked)
            {
                if (aliasTextBox.Text.Length == 0 || userNameTextBox.Text.Length == 0 || passwordTextBox.Text.Length == 0)
                {
                    tabControl.SelectedIndex = 0;

                    // Set error providers for missing information if desirable
                    if (aliasTextBox.Text.Length == 0)
                        aliasErrorProvider.SetError(aliasTextBox, Properties.Resources.Input_AliasRequiredErrorInfo);
                    else if (aliasTextBox.Text.Length > ALIAS_MAX_LENGTH)
                        aliasErrorProvider.SetError(aliasTextBox, String.Format(Properties.Resources.Input_AliasTooLongErrorInfo, ALIAS_MAX_LENGTH.ToString()));
                    else
                        aliasErrorProvider.Clear();
                    if (userNameTextBox.Text.Length == 0)
                        accountErrorProvider.SetError(userNameTextBox, Properties.Resources.Input_AccountRequiredErrorInfo);
                    else
                        accountErrorProvider.Clear();
                    if (passwordTextBox.Text.Length == 0)
                        passwordErrorProvider.SetError(passwordTextBox, Properties.Resources.Input_PasswordRequiredErrorInfo);
                    else
                        passwordErrorProvider.Clear();

                    MessageBox.Show(Properties.Resources.Input_MissingInformationMessage, Properties.Resources.Input_MissingInformationMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void UpdateVisualizationsComponents()
        {
            // Update form according to settings
            visualizationsCheckBox.Checked = visualizationsSettings.VisualizationsActive;
            mainColorTextBox.BackColor = visualizationsSettings.MainColor;
            textColorTextBox.BackColor = visualizationsSettings.TextColor;
            lineColorTextBox.BackColor = visualizationsSettings.LineColor;
            framesCheckBox.Checked = visualizationsSettings.Frames;
            if (visualizationsSettings.ConnectionBar)
            {
                double connectionBarOpacity = 0;
                connectionBarOpacity = visualizationsSettings.ConnectionBarOpacity;
                connectionBarTrackBar.Maximum = 10;
                connectionBarTrackBar.Minimum = 0;
                connectionBarTrackBar.Value = (int)(10 * connectionBarOpacity);
                connectionBarTrackBar.Enabled = true;
                connectionBarCheckBox.Checked = true;
            }
            else
            {
                connectionBarCheckBox.Checked = false;
                connectionBarTrackBar.Enabled = false;
            }
        }

        private void UpdateExplorerExtensionComponents()
        {
            // Update form according to settings
            explorerCheckBox.Checked = explorerExtensionSettings.ExplorerExtensionActive;
            desktopCheckBox.Checked = explorerExtensionSettings.DesktopFolder;
            documentsCheckBox.Checked = explorerExtensionSettings.DocumentsFolder;
            downloadsCheckBox.Checked = explorerExtensionSettings.DownloadsFolder;
            favoritesCheckBox.Checked = explorerExtensionSettings.FavoritesFolder;
            linksCheckBox.Checked = explorerExtensionSettings.LinksFolder;
            musicCheckBox.Checked = explorerExtensionSettings.MusicFolder;
            picturesCheckBox.Checked = explorerExtensionSettings.PicturesFolder;
            savedGamesCheckBox.Checked = explorerExtensionSettings.SavedGamesFolder;
            videosCheckBox.Checked = explorerExtensionSettings.VideosFolder;
            disketteDrivesCheckBox.Checked = explorerExtensionSettings.DisketteDrives;
            hardDrivesCheckBox.Checked = explorerExtensionSettings.HardDrives;
            cdDrivesCheckBox.Checked = explorerExtensionSettings.CDDrives;
            removableDrivesCheckBox.Checked = explorerExtensionSettings.RemovableDrives;
        }

        private void UpdateServerComponents()
        {
            // Update form according to settings
            authenticationLevelComboBox.SelectedIndex = Convert.ToInt32(serverSettings.AuthenticationLevel) - 1;
            userNameTextBox.Text = serverSettings.UserName;
            aliasTextBox.Text = serverSettings.Alias;
            passwordTextBox.Text = serverSettings.Password;
            domainTextBox.Text = serverSettings.Domain;
            autoReconnectCheckBox.Checked = serverSettings.AutoReconnect;
            autostartProgramsCheckBox.Checked = serverSettings.AutoStartPrograms;
            keepAliveCheckBox.Checked = serverSettings.KeepAliveAgent;
            enableShortcutsCheckBox.Checked = serverSettings.CreateShortcuts;
            startMenuShortcutsCheckBox.Checked = serverSettings.CreateStartMenuShortcuts;
            uwpApplicationShortcutsCheckBox.Checked = serverSettings.CreateUWPApplicationShortcuts;
            desktopShortcutsCheckBox.Checked = serverSettings.CreateDesktopShortcuts;
            desktopRootCheckBox.Checked = serverSettings.DesktopRoot;
            clipboardCheckBox.Checked = serverSettings.RedirectClipboard;
            usbCheckBox.Checked = serverSettings.RedirectUsb;
            printersCheckBox.Checked = serverSettings.RedirectPrinters;
            enableServerCheckBox.Checked = serverSettings.ServerEnabled;
            shortcutsAppendAliasCheckBox.Checked = serverSettings.ShortcutsAppendAlias;
            notificationsCheckBox.Checked = serverSettings.ShowNotifications;
            localizeShortcutsCheckBox.Checked = serverSettings.LocalizeShortcuts;
        }

        private void UpdateVisualizationsSettings()
        {
            // Initialize visual aids settings from config
            visualizationsSettings = new VisualizationsSettings(serverName);

            // Store form states in settings
            visualizationsSettings.VisualizationsActive = visualizationsCheckBox.Checked;
            visualizationsSettings.MainColor = mainColorTextBox.BackColor;
            visualizationsSettings.TextColor = textColorTextBox.BackColor;
            visualizationsSettings.LineColor = lineColorTextBox.BackColor;
            visualizationsSettings.ConnectionBar = connectionBarCheckBox.Checked;
            visualizationsSettings.ConnectionBarOpacity = (double)connectionBarTrackBar.Value / 10;
            visualizationsSettings.MainColor = mainColorTextBox.BackColor;
            visualizationsSettings.Frames = framesCheckBox.Checked;

            visualizationsSettings.SaveVisualizationsSettings();
        }

        private void UpdateExplorerSettings()
        {
            // Inititalize explorer settings from config
            explorerExtensionSettings = new ExplorerExtensionSettings(serverName);

            // Store form states in settings
            explorerExtensionSettings.ExplorerExtensionActive = explorerCheckBox.Checked;
            explorerExtensionSettings.ThreeDObjectsFolder = false;
            explorerExtensionSettings.ContactsFolder = false;
            explorerExtensionSettings.DesktopFolder = desktopCheckBox.Checked;
            explorerExtensionSettings.DocumentsFolder = documentsCheckBox.Checked;
            explorerExtensionSettings.DownloadsFolder = downloadsCheckBox.Checked;
            explorerExtensionSettings.FavoritesFolder = favoritesCheckBox.Checked;
            explorerExtensionSettings.LinksFolder = linksCheckBox.Checked;
            explorerExtensionSettings.MusicFolder = musicCheckBox.Checked;
            explorerExtensionSettings.PicturesFolder = picturesCheckBox.Checked;
            explorerExtensionSettings.SavedGamesFolder = savedGamesCheckBox.Checked;
            explorerExtensionSettings.SearchesFolder = false;
            explorerExtensionSettings.VideosFolder = videosCheckBox.Checked;
            explorerExtensionSettings.DisketteDrives = disketteDrivesCheckBox.Checked;
            explorerExtensionSettings.HardDrives = hardDrivesCheckBox.Checked;
            explorerExtensionSettings.CDDrives = cdDrivesCheckBox.Checked;
            explorerExtensionSettings.RemovableDrives = removableDrivesCheckBox.Checked;

            explorerExtensionSettings.SaveExplorerExtensionSettings();
            UpdateNamespaceExtension();
        }

        private void UpdateNamespaceExtension()
        {
            if (RAASClientFeatureHelper.NSExtInstalled())
            {
                // Update explorer namespace extension
                if (serverSettings.ServerEnabled && explorerExtensionSettings.ExplorerExtensionActive)
                    ServerHelper.UpdateNamespaceExtension(serverSettings.Alias);
                else
                    ServerHelper.UpdateNamespaceExtension();
            }
        }

        private void UpdateServerSettings()
        {
            String oldAlias = serverSettings.Alias;
            // Initialize server settings from config
            serverSettings = new ServerSettings(serverName);

            // Determine if shortcuts should be removed
            if (!enableServerCheckBox.Checked || !enableShortcutsCheckBox.Checked)
            {
                serverSettings.RemoveShortcuts = true;
                serverSettings.ShortcutsRemoved = false;
            }
            else if (enableServerCheckBox.Checked && enableShortcutsCheckBox.Checked)
            {
                serverSettings.RemoveShortcuts = false;
                serverSettings.ShortcutsRemoved = false;
            }

            // Store form states in settings
            serverSettings.AuthenticationLevel = (authenticationLevelComboBox.SelectedIndex + 1);
            serverSettings.Password = passwordTextBox.Text;
            serverSettings.AutoStartPrograms = autostartProgramsCheckBox.Checked;
            serverSettings.KeepAliveAgent = keepAliveCheckBox.Checked;
            serverSettings.RedirectClipboard = clipboardCheckBox.Checked;
            serverSettings.RedirectUsb = usbCheckBox.Checked;
            serverSettings.RedirectPrinters = printersCheckBox.Checked;
            serverSettings.AutoReconnect = autoReconnectCheckBox.Checked;
            serverSettings.CreateShortcuts = enableShortcutsCheckBox.Checked;
            serverSettings.CreateStartMenuShortcuts = startMenuShortcutsCheckBox.Checked;
            serverSettings.CreateUWPApplicationShortcuts = uwpApplicationShortcutsCheckBox.Checked;
            serverSettings.CreateDesktopShortcuts = desktopShortcutsCheckBox.Checked;
            serverSettings.DesktopRoot = desktopRootCheckBox.Checked;
            serverSettings.ServerEnabled = enableServerCheckBox.Checked;
            serverSettings.ShortcutsAppendAlias = shortcutsAppendAliasCheckBox.Checked;
            serverSettings.ShowNotifications = notificationsCheckBox.Checked;
            serverSettings.LocalizeShortcuts = localizeShortcutsCheckBox.Checked;
            serverSettings.UserName = userNameTextBox.Text;
            serverSettings.Alias = aliasTextBox.Text;
            serverSettings.Domain = domainTextBox.Text;

            try
            {
                serverSettings.SaveServerSettings();
            }
            catch (AliasAlreadyExistException /* e */)
            {
                MessageBox.Show(null, Properties.Resources.ServerSettings_AliasOccupiedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                aliasErrorProvider.SetError(aliasTextBox, Properties.Resources.ServerSettings_AliasOccupiedErrorInfo);
            }
            catch
            {
                MessageBox.Show(null, Properties.Resources.ServerSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (oldAlias != serverSettings.Alias)
                ServerHelper.UpdateNamespaceExtensionDeletedServer(oldAlias);
            UpdateNamespaceExtension();
            SetTitle();
        }

        private void SetTitle()
        {
            this.Text = Properties.Resources.MainForm_MainFormTitle;
        }

        private void UpdateEnabledStates()
        {
            if (!RAASClientFeatureHelper.ShortcutsInstalled())
                tabControl.TabPages.Remove(remoteShortcutsTab);
            if(!RAASClientFeatureHelper.VisualizationsInstalled())
                tabControl.TabPages.Remove(visualizationsTab);
            if (!RAASClientFeatureHelper.NSExtInstalled())
                tabControl.TabPages.Remove(sharesTab);
            settingsGroupBox.Enabled = true;
            explorerCheckBox.Enabled = true;
            explorerGroupBox.Enabled = true;
            visualizationsCheckBox.Enabled = true;
            visualizationsSettingsGroupBox.Enabled = true;
            enableServerCheckBox.Enabled = true;
            if (serverSupplied)
            {
                if (status == (int)ServerStatus.NoContact)
                {
                    disconnectButton.Enabled = false;
                    connectButton.Enabled = false;
                }
                else if (status == (int)ServerStatus.Contact)
                {
                    disconnectButton.Enabled = false;
                    connectButton.Enabled = false;
                }
                else if (status == (int)ServerStatus.Available)
                {
                    disconnectButton.Enabled = false;
                    connectButton.Enabled = true;
                }
                else if (status == (int)ServerStatus.Connected)
                {
                    disconnectButton.Enabled = true;
                    connectButton.Enabled = true;
                }
            }
            if (serverSettingsAreDirty || explorerSettingsAreDirty || visualizationsSettingsAreDirty)
                applyButton.Enabled = true;
            else
                applyButton.Enabled = false;
        }

        private void CheckRAASConnection()
        {
            try
            {
                if (raasServiceClient.State == System.ServiceModel.CommunicationState.Opened || raasServiceClient.State == System.ServiceModel.CommunicationState.Opening)
                    ConnectRAAS();

                // Get status from RAAS Service
                if (raasServiceClient.State == System.ServiceModel.CommunicationState.Opened || raasServiceClient.State == System.ServiceModel.CommunicationState.Opening)
                {
                    if (serverSettings.ServerEnabled)
                    {
                        try
                        {
                            status = raasServiceClient.GetServerStatus(serverName);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    else
                    {
                        bool contact = ServerHelper.Contact(serverSettings.ServerName);
                        if (contact)
                            status = (int)ServerStatus.Contact;
                        else
                            status = (int)ServerStatus.NoContact;
                    }

                    // Update states according to status
                    if (status == (int)ServerStatus.Connected)
                        Invoke(new setConnectedDelegate(SetConnected));
                    else if (status == (int)ServerStatus.Available)
                        Invoke(new setAvailableDelegate(SetAvailable));
                    else if (status == (int)ServerStatus.Contact)
                        Invoke(new setContactDelegate(SetContact));
                    else if (status == (int)ServerStatus.NoContact)
                        Invoke(new setNoContactDelegate(SetNoContact));
                    if (status == (int)ServerStatus.Available || status == (int)ServerStatus.Connected)
                    {
                        if (!versionShowing)
                        {
                            try
                            {
                                String serverVersion = raasServiceClient.GetServerVersion(serverName);
                                if (serverVersion != null)
                                {
                                    Invoke(new setVersionDelegate(SetVersion), serverVersion);
                                }
                            }
                            catch { }
                        }
                    }
                    else if (versionShowing)
                        Invoke(new clearVersionDelegate(ClearVersion));
                }
                else
                {
                    RAASNotResponding();
                }
            }
            catch
            {
                RAASNotResponding();
            }
        }

        private void RAASNotResponding()
        {
            if (versionShowing)
                Invoke(new clearVersionDelegate(ClearVersion));
            Invoke(new setRAASNotRespondingDelegate(SetRAASNotResponding));
            ConnectRAAS();
        }

        private void ClearVersion()
        {
            serverVersionLabel.Visible = false;
            serverVersionText.Visible = false;
            serverVersionText.Text = "";
            versionShowing = false;
        }

        private void SetVersion(String version)
        {
            serverVersionLabel.Visible = true;
            serverVersionText.Text = version;
            serverVersionText.Visible = true;
            versionShowing = true;
        }

        private void SetConnected()
        {
            connectionText.Text = Properties.Resources.Connection_ConnectedLabel;
            disconnectButton.Enabled = true;
            connectButton.Enabled = true;
            connectButton.Text = Properties.Resources.Connection_ReconnectButton;
            connected = true;
        }

        private void SetAvailable()
        {
            connected = false;
            connectionText.Text = Properties.Resources.Connection_AvailableLabel;
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void SetContact()
        {
            connected = false;
            connectionText.Text = Properties.Resources.Connection_ContactLabel;
            disconnectButton.Enabled = false;
            connectButton.Enabled = false;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void SetNoMessage()
        {
            connected = false;
            connectionText.Text = "";
            disconnectButton.Enabled = false;
            connectButton.Enabled = false;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void SetNoServer()
        {
            connected = false;
            connectionText.Text = Properties.Resources.Connection_NoServerLabel;
            disconnectButton.Enabled = false;
            connectButton.Enabled = false;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void SetNoContact()
        {
            connected = false;
            connectionText.Text = Properties.Resources.Connection_NoContactLabel;
            disconnectButton.Enabled = false;
            connectButton.Enabled = false;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void SetRAASNotResponding()
        {
            connected = false;
            connectionText.Text = Properties.Resources.Connection_RAASClientNotRespondingLabel;
            disconnectButton.Enabled = false;
            connectButton.Enabled = false;
            connectButton.Text = Properties.Resources.Connection_ConnectButton;
        }

        private void ConnectRAAS()
        {
            try
            {
                raasServiceClient?.Abort();
            }
            catch { }
            try
            {
                raasServiceClient = new RAASClientServiceClient();
                raasServiceClient.Open();
            }
            catch { }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (serverSettingsAreDirty || visualizationsSettingsAreDirty || explorerSettingsAreDirty)
            {
                if (!Apply())
                {
                    return;
                }
                try
                {
                    raasServiceClient.Abort();
                }
                catch { }
            }
            Application.Exit();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (!Apply())
                return;
            applyButton.Enabled = false;
            serverSettingsAreDirty = false;
            visualizationsSettingsAreDirty = false;
            explorerSettingsAreDirty = false;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Clean up
            try
            {
                raasServiceClient.Abort();
            }
            catch { }

            // Exit
            Application.Exit();
        }

        private void EnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void EnableServerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetExplorerSettingsDirty();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                raasServiceClient.ConnectServer(serverName);
                reconnectRequiredOnConnected = false;
            }
            catch {  }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                raasServiceClient.DisconnectServer(serverName);
            }
            catch { }
        }

        private void MainColorButton_Click(object sender, EventArgs e)
        {
            colorPickerDialog.ShowDialog();
            mainColorTextBox.BackColor = colorPickerDialog.Color;
            SetVisualizationsSettingsDirty();
        }

        private void TextColorButton_Click(object sender, EventArgs e)
        {
            colorPickerDialog.ShowDialog();
            textColorTextBox.BackColor = colorPickerDialog.Color;
            SetVisualizationsSettingsDirty();
        }

        private void LineColorButton_Click(object sender, EventArgs e)
        {
            colorPickerDialog.ShowDialog();
            lineColorTextBox.BackColor = colorPickerDialog.Color;
            SetVisualizationsSettingsDirty();
        }

        private void VisualizationsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            reconnectRequiredOnConnected = true;
            SetVisualizationsSettingsDirty();
        }

        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void DomainTextBox_TextChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void AutostartProgramsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            autostartProgramsDirty = true;
        }

        private void ClipboardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            reconnectRequiredOnConnected = true;
        }

        private void UsbCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            reconnectRequiredOnConnected = true;
        }

        private void PrintersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            reconnectRequiredOnConnected = true;
        }

        private void CrosslinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetVisualizationsSettingsDirty();
        }

        private void FramesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetVisualizationsSettingsDirty();
        }

        private void OverlaysTrackBar_Scroll(object sender, EventArgs e)
        {
            SetVisualizationsSettingsDirty();
        }

        private void ConnectionBarTrackBar_Scroll(object sender, EventArgs e)
        {
            SetVisualizationsSettingsDirty();
        }

        private void ConnectionBarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (connectionBarCheckBox.Checked)
                connectionBarTrackBar.Enabled = true;
            else
                connectionBarTrackBar.Enabled = false;
            SetVisualizationsSettingsDirty();
        }

        private void AuthenticationLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            reconnectRequiredOnConnected = true;
        }

        private void DesktopRootCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }

        private void AliasTextBox_TextChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetExplorerSettingsDirty();
        }

        private void AliasTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            Regex disallowedCharacters = new Regex("[*\\[\\]\\(\\)\\?\\|\\<\\>\\\\\\/\\:\\\"]");
            if (!Char.IsControl(e.KeyChar) && aliasTextBox.Text.Length >= ALIAS_MAX_LENGTH)
            {
                ToolTip messageToolTip = new ToolTip();
                messageToolTip.Show(String.Format(Properties.Resources.Alias_MaximumCharactersToolTip, ALIAS_MAX_LENGTH.ToString()), aliasTextBox, ALIAS_TOOLTIP_DURATION);
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
            else if (disallowedCharacters.IsMatch(e.KeyChar.ToString()))
            {
                ToolTip messageToolTip = new ToolTip();
                messageToolTip.Show(Properties.Resources.Alias_AllowedCharactersToolTip, aliasTextBox, ALIAS_TOOLTIP_DURATION);
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private void ThreeDObjectsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void ContactsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void DesktopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void DocumentsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void DownloadsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void FavoritesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void LinksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void MusicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void PicturesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void SavedGamesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void SearchesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void VideosCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void DisketteDrivesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void HardDrivesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void CdDrivesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void ExplorerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void AutoReconnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void RemovableDrivesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetExplorerSettingsDirty();
        }

        private void KeepAliveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            keepAliveAgentDirty = true;
        }

        private void ServersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!serverSupplied && serversComboBox.SelectedIndex == 0)
                return;
            if (serversComboboxMonitorChange)
            {
                serverSupplied = true;
                String newServerName = serverIndexes.FirstOrDefault(x => x.Value == serversComboBox.SelectedIndex).Key;
                if (newServerName == serverName)
                    return;
                if (serverSettingsAreDirty || visualizationsSettingsAreDirty || explorerSettingsAreDirty)
                {
                    DialogResult dialogResult = MessageBox.Show(null, Properties.Resources.Settings_UnsavedChangesMessage, Properties.Resources.Settings_UnsavedChangesMessageTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (!Apply())
                            return;
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        serversComboboxMonitorChange = false;
                        serversComboBox.SelectedIndex = serverIndexes[serverName];
                        serversComboboxMonitorChange = true;
                        return;
                    }
                }
                InitializeServerConfig(newServerName);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            HostNameForm addServerDialog = new HostNameForm();
            addServerDialog.ShowDialog();
            if (addServerDialog.DialogButton == DialogResult.OK)
            {
                Dictionary<String, ServerSettings> currentServerSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
                String newServerName = addServerDialog.ServerName.ToUpperInvariant();
                if (currentServerSettings.Keys.Contains(newServerName))
                {
                    if (MessageBox.Show(String.Format(Properties.Resources.Settings_AlreadyConfiguredMessage, newServerName), Properties.Resources.Settings_AlreadyConfiguredMessageTitle, MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                }
                else
                    AddServer(newServerName);
                UpdateAvailableServers();
                serversComboBox.SelectedIndex = serverIndexes[newServerName];
                InitializeServerConfig(newServerName);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(String.Format(Properties.Resources.Server_ConfirmRemoveMessage, serverName), Properties.Resources.Server_ConfirmRemoveMessageTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    serverSettings = new ServerSettings(serverName);
                    serverSettings.RemoveServer = true;
                    serverSettings.RemoveShortcuts = true;
                    if (serverSettings.ServerEnabled && serverSettings.CreateShortcuts)
                        serverSettings.ShortcutsRemoved = false;
                    try
                    {
                        serverSettings.SaveServerSettings();
                    }
                    catch
                    {
                        MessageBox.Show(null, Properties.Resources.ServerSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (RAASClientFeatureHelper.ShortcutsInstalled())
                    {
                        Process shortcuts = RAASClientProgramHelper.StartShortcuts("-remove");
                        shortcuts.WaitForExit();
                    }
                    try
                    {
                        serverSettings.Remove();
                    }
                    catch
                    {
                        MessageBox.Show(null, Properties.Resources.ServerSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    String serverPath = RAASClientPathHelper.GetServerAppDataRAASClientPath(serverName);
                    if (Directory.Exists(serverPath))
                        try
                        {
                            Directory.Delete(serverPath, true);
                        }catch { }
                    serverSupplied = false;
                    InitializeServerConfig();
                    ServerHelper.UpdateNamespaceExtensionDeletedServer(serverSettings.Alias);
                    ServerHelper.UpdateNamespaceExtension();
                }
                catch { }
            }
        }

        private void EnableShortcutsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }

        private void ShortcutsAppendAliasCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }

        private void DesktopShortcutsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }

        private void StartMenuShortcutsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }

        private void NotificationsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void LocalizeShortcutsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
        }

        private void uwpApplicationShortcutsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetServerSettingsDirty();
            SetShortcutsSettingsDirty();
        }
    }
}
