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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.RDesktop
{
    public partial class RemoteDesktopSettingsForm : Form
    {
        private bool serverSupplied = false;
        private bool serversComboboxMonitorChange = false;
        private bool remoteDesktopSettingsAreDirty = false;
        private String serverName;
        private ServerSettings serverSettings;
        private RemoteDesktopSettings remoteDesktopSettings;
        private Dictionary<String, int> serverIndexes;
        private ErrorProvider widthErrorProvider = new ErrorProvider();
        private ErrorProvider heightErrorProvider = new ErrorProvider();

        private class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public RemoteDesktopSettingsForm()
        {
            serverSupplied = false;
            InitializeComponent();
            InitializeRemoteDesktopSettings();
            serversComboBox.Focus();
            this.ActiveControl = serversComboBox;
            connectButton.Enabled = false;
        }

        public RemoteDesktopSettingsForm(String serverName)
        {
            serverSupplied = true;
            InitializeComponent();
            InitializeRemoteDesktopSettings(serverName);
            connectButton.Enabled = true;
            connectButton.Focus();
            this.ActiveControl = connectButton;
        }

        private void InitializeLocalizedStrings()
        {
            ComboboxItem cbi1 = new ComboboxItem();
            cbi1.Text = Properties.Resources.Settings_AuthenticationRequiredOption;
            cbi1.Value = 1;
            ComboboxItem cbi2 = new ComboboxItem();
            cbi2.Text = Properties.Resources.Settings_AuthenticationWarningOption;
            cbi2.Value = 0;
            authenticationLevelComboBox.Items.Clear();
            authenticationLevelComboBox.Items.Add(cbi1);
            authenticationLevelComboBox.Items.Add(cbi2);
            this.serverLabel.Text = Properties.Resources.MainForm_ServerLabel;
            this.settingsGroupBox.Text = Properties.Resources.MainForm_SettingsGroupBox;
            this.applyButton.Text = Properties.Resources.MainForm_ApplyButton;
            this.cancelButton.Text = Properties.Resources.MainForm_CancelButton;
            this.connectButton.Text = Properties.Resources.MainForm_ConnectButton;
            this.authenticationLabel.Text = Properties.Resources.General_RDPAuthenticationLabel;
            this.printersCheckBox.Text = Properties.Resources.Settings_RedirectPrintersCheckBox;
            this.usbCheckBox.Text = Properties.Resources.Settings_RedirectDevicesCheckBox;
            this.clipboardCheckBox.Text = Properties.Resources.Settings_RedirectClipboardCheckBox;
            this.fullscreenCheckBox.Text = Properties.Resources.Settings_FullscreenCheckBox;
            this.connectionBarCheckBox.Text = Properties.Resources.Settings_ConnectionBarCheckbox;
            this.pinConnectionBarCheckBox.Text = Properties.Resources.Settings_PinConnectionBarCheckbox;
            this.widthLabel.Text = Properties.Resources.Settings_WidthLabel;
            this.heightLabel.Text = Properties.Resources.Settings_HeightLabel;
            this.Text = Properties.Resources.MainForm_MainFormTitle;
        }

        private void InitializeRemoteDesktopSettings()
        {
            InitializeLocalizedStrings();
            UpdateAvailableServers();
            if (!serverSupplied)
            {
                settingsGroupBox.Visible = false;
                settingsGroupBox.TabStop = false;
            }
            else
            {
                settingsGroupBox.TabStop = true;
                settingsGroupBox.Visible = true;
            }
            if (serverSupplied)
                UpdateServerComponents();
            NothingDirty();
            UpdateEnabledStates();
        }

        private void InitializeRemoteDesktopSettings(String serverName)
        {
            this.serverName = serverName;

            // Read settings
            try
            {
                serverSettings = new ServerSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.ServerSettings_ServerLoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                remoteDesktopSettings = new RemoteDesktopSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.RemoteDesktopSettings_ServerLoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Initialize
            InitializeRemoteDesktopSettings();
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
            IEnumerable<String> sortedServerNames = servers.Keys.Where(server => servers[server].ServerEnabled).OrderBy(server => servers[server].ServerName);
            serverIndexes = new Dictionary<string, int>();
            int serverIndex = 0;
            if (serverSupplied)
            {
                bool serverExist = servers.Keys.Contains(serverName);
                if (!serverExist)
                {
                    MessageBox.Show(String.Format(Properties.Resources.Server_RemovedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    serverSupplied = false;
                    InitializeRemoteDesktopSettings();
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

        private void UpdateServerComponents()
        {
            // Update form according to settings
            authenticationLevelComboBox.SelectedIndex = Convert.ToInt32(remoteDesktopSettings.AuthenticationLevel) - 1;
            clipboardCheckBox.Checked = remoteDesktopSettings.RedirectClipboard;
            usbCheckBox.Checked = remoteDesktopSettings.RedirectUsb;
            printersCheckBox.Checked = remoteDesktopSettings.RedirectPrinters;
            fullscreenCheckBox.Checked = remoteDesktopSettings.Fullscreen;
            connectionBarCheckBox.Checked = remoteDesktopSettings.ConnectionBar;
            pinConnectionBarCheckBox.Checked = remoteDesktopSettings.PinConnectionBar;
            widthTextBox.Text = Convert.ToString(remoteDesktopSettings.Width);
            heightTextBox.Text = Convert.ToString(remoteDesktopSettings.Height);
        }

        private void UpdateEnabledStates()
        {
            settingsGroupBox.Enabled = true;
            if (remoteDesktopSettingsAreDirty)
                applyButton.Enabled = true;
            else
                applyButton.Enabled = false;
        }

        private void SetRemoteDesktopSettingsDirty()
        {
            remoteDesktopSettingsAreDirty = true;
            applyButton.Enabled = true;
        }

        private void NothingDirty()
        {
            // Set application states
            remoteDesktopSettingsAreDirty = false;
        }

        private void serversComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectButton.Enabled = true;
            connectButton.Focus();
            this.ActiveControl = connectButton;
            if (!serverSupplied && serversComboBox.SelectedIndex == 0)
                return;
            if (serversComboboxMonitorChange)
            {
                serverSupplied = true;
                String newServerName = serverIndexes.FirstOrDefault(x => x.Value == serversComboBox.SelectedIndex).Key;
                if (newServerName == serverName)
                    return;
                if (remoteDesktopSettingsAreDirty)
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
                InitializeRemoteDesktopSettings(newServerName);
            }
        }

        private bool Apply()
        {
            RAASClientPathHelper.CreateMissingServerAppDataRAASDirectories(serverName);

            // Try to save all setting which have changed
            if (remoteDesktopSettingsAreDirty)
            {
                if (!ValidateForm())
                    return false;
                if (!TryUpdateRemoteDesktopSettings())
                    return false;
            }
            return true;
        }

        private bool TryUpdateRemoteDesktopSettings()
        {
            try
            {
                UpdateRemoteDesktopSettings();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.Settings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool ValidateForm()
        {
            bool widthCorrect = Int32.TryParse(widthTextBox.Text, out int widthNumber);
            bool heightCorrect = Int32.TryParse(heightTextBox.Text, out int heightNumber);
            if (!widthCorrect || !heightCorrect)
            {
                if (!widthCorrect)
                    widthErrorProvider.SetError(widthTextBox, Properties.Resources.Settings_WidthErrorInfo);
                else
                    widthErrorProvider.Clear();
                if (!heightCorrect)
                    heightErrorProvider.SetError(heightTextBox, Properties.Resources.Settings_HeightErrorInfo);
                else
                    heightErrorProvider.Clear();
                MessageBox.Show(Properties.Resources.Input_WrongSizeMessage, Properties.Resources.Input_WrongSizeMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                widthErrorProvider.Clear();
                heightErrorProvider.Clear();
            }
            return true;
        }

        private void UpdateRemoteDesktopSettings()
        {
            // Initialize remote desktop settings from config
            remoteDesktopSettings = new RemoteDesktopSettings(serverName);

            // Store form states in settings
            remoteDesktopSettings.AuthenticationLevel = (authenticationLevelComboBox.SelectedIndex + 1);
            remoteDesktopSettings.RedirectClipboard = clipboardCheckBox.Checked;
            remoteDesktopSettings.RedirectUsb = usbCheckBox.Checked;
            remoteDesktopSettings.RedirectPrinters = printersCheckBox.Checked;
            remoteDesktopSettings.Fullscreen = fullscreenCheckBox.Checked;
            remoteDesktopSettings.ConnectionBar = connectionBarCheckBox.Checked;
            remoteDesktopSettings.PinConnectionBar = pinConnectionBarCheckBox.Checked;
            remoteDesktopSettings.Width = Convert.ToInt32(widthTextBox.Text);
            remoteDesktopSettings.Height = Convert.ToInt32(heightTextBox.Text);

            try
            {
                remoteDesktopSettings.SaveRemoteDesktopSettings();
            }
            catch
            {
                MessageBox.Show(null, Properties.Resources.RemoteDesktopSettings_UpdateFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void authenticationLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void clipboardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void usbCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void printersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void fullscreenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void allMonitorsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void widthTextBox_TextChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void heightTextBox_TextChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!Apply())
                return;
            applyButton.Enabled = false;
            remoteDesktopSettingsAreDirty = false;
            try
            {
                serverSettings = new ServerSettings(serverName);
            }
            catch
            {
                MessageBox.Show(String.Format(Properties.Resources.ServerSettings_ServerLoadFailedMessage, serverName), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Visible = false;
            this.ShowInTaskbar = false;
            RemoteDesktopForm remoteDesktopForm = new RemoteDesktopForm(serverSettings, remoteDesktopSettings);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Exit
            Application.Exit();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (!Apply())
                return;
            applyButton.Enabled = false;
            remoteDesktopSettingsAreDirty = false;
        }

        private void connectionBarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }

        private void pinConnectionBarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetRemoteDesktopSettingsDirty();
        }
    }
}
