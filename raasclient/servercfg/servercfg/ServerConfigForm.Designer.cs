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
﻿using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.ServerCfg
{
    partial class ServerConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConfigForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.accountLabel = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.aliasLabel = new System.Windows.Forms.Label();
            this.notificationsCheckBox = new System.Windows.Forms.CheckBox();
            this.printersCheckBox = new System.Windows.Forms.CheckBox();
            this.keepAliveCheckBox = new System.Windows.Forms.CheckBox();
            this.usbCheckBox = new System.Windows.Forms.CheckBox();
            this.clipboardCheckBox = new System.Windows.Forms.CheckBox();
            this.autoReconnectCheckBox = new System.Windows.Forms.CheckBox();
            this.domainLabel = new System.Windows.Forms.Label();
            this.autostartProgramsCheckBox = new System.Windows.Forms.CheckBox();
            this.aliasTextBox = new System.Windows.Forms.TextBox();
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.domainTextBox = new System.Windows.Forms.TextBox();
            this.authenticationLevelComboBox = new System.Windows.Forms.ComboBox();
            this.enableServerCheckBox = new System.Windows.Forms.CheckBox();
            this.serverConnectionGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.serverVersionLabel = new System.Windows.Forms.Label();
            this.serverVersionText = new System.Windows.Forms.Label();
            this.connectionText = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.remoteShortcutsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.enableShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.shortcutsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.startMenuShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.localizeShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.uwpApplicationShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.desktopShortcutsCheckBox = new System.Windows.Forms.CheckBox();
            this.shortcutsAppendAliasCheckBox = new System.Windows.Forms.CheckBox();
            this.desktopRootCheckBox = new System.Windows.Forms.CheckBox();
            this.visualizationsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.visualizationsCheckBox = new System.Windows.Forms.CheckBox();
            this.visualizationsSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.mainColorButton = new System.Windows.Forms.Button();
            this.connectionBarTrackBar = new System.Windows.Forms.TrackBar();
            this.connectionBarCheckBox = new System.Windows.Forms.CheckBox();
            this.connectionOpacityLabel = new System.Windows.Forms.Label();
            this.mainColorTextBox = new System.Windows.Forms.TextBox();
            this.textColorButton = new System.Windows.Forms.Button();
            this.textColorTextBox = new System.Windows.Forms.TextBox();
            this.lineColorButton = new System.Windows.Forms.Button();
            this.lineColorTextBox = new System.Windows.Forms.TextBox();
            this.framesCheckBox = new System.Windows.Forms.CheckBox();
            this.crosslinesCheckBox = new System.Windows.Forms.CheckBox();
            this.sharesTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.explorerCheckBox = new System.Windows.Forms.CheckBox();
            this.explorerGroupBox = new System.Windows.Forms.GroupBox();
            this.visibleFoldersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.desktopCheckBox = new System.Windows.Forms.CheckBox();
            this.documentsCheckBox = new System.Windows.Forms.CheckBox();
            this.downloadsCheckBox = new System.Windows.Forms.CheckBox();
            this.favoritesCheckBox = new System.Windows.Forms.CheckBox();
            this.linksCheckBox = new System.Windows.Forms.CheckBox();
            this.musicCheckBox = new System.Windows.Forms.CheckBox();
            this.picturesCheckBox = new System.Windows.Forms.CheckBox();
            this.savedGamesCheckBox = new System.Windows.Forms.CheckBox();
            this.videosCheckBox = new System.Windows.Forms.CheckBox();
            this.devicesAndDrivesGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.disketteDrivesCheckBox = new System.Windows.Forms.CheckBox();
            this.removableDrivesCheckBox = new System.Windows.Forms.CheckBox();
            this.hardDrivesCheckBox = new System.Windows.Forms.CheckBox();
            this.cdDrivesCheckBox = new System.Windows.Forms.CheckBox();
            this.colorPickerDialog = new System.Windows.Forms.ColorDialog();
            this.serverLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.serversComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.settingsGroupBox.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.serverConnectionGroupBox.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.remoteShortcutsTab.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.shortcutsGroupBox.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.visualizationsTab.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.visualizationsSettingsGroupBox.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBarTrackBar)).BeginInit();
            this.sharesTab.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.explorerGroupBox.SuspendLayout();
            this.visibleFoldersTableLayoutPanel.SuspendLayout();
            this.devicesAndDrivesGroupBox.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.AutoSize = true;
            this.cancelButton.Location = new System.Drawing.Point(374, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okButton.AutoSize = true;
            this.okButton.Location = new System.Drawing.Point(293, 8);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameTextBox.Location = new System.Drawing.Point(202, 32);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(276, 20);
            this.userNameTextBox.TabIndex = 2;
            this.userNameTextBox.TextChanged += new System.EventHandler(this.UserNameTextBox_TextChanged);
            // 
            // accountLabel
            // 
            this.accountLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.accountLabel.AutoSize = true;
            this.accountLabel.Location = new System.Drawing.Point(6, 35);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Size = new System.Drawing.Size(50, 13);
            this.accountLabel.TabIndex = 14;
            this.accountLabel.Text = "Account:";
            // 
            // applyButton
            // 
            this.applyButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.applyButton.AutoSize = true;
            this.applyButton.Location = new System.Drawing.Point(455, 8);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(70, 23);
            this.applyButton.TabIndex = 12;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(6, 61);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordLabel.TabIndex = 16;
            this.passwordLabel.Text = "Password:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.Location = new System.Drawing.Point(202, 58);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(276, 20);
            this.passwordTextBox.TabIndex = 3;
            this.passwordTextBox.TextChanged += new System.EventHandler(this.PasswordTextBox_TextChanged);
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.AutoSize = true;
            this.settingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsGroupBox.Controls.Add(this.tableLayoutPanel8);
            this.settingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsGroupBox.Location = new System.Drawing.Point(3, 26);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(504, 317);
            this.settingsGroupBox.TabIndex = 19;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel8.Controls.Add(this.aliasLabel, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.notificationsCheckBox, 1, 11);
            this.tableLayoutPanel8.Controls.Add(this.accountLabel, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.printersCheckBox, 1, 10);
            this.tableLayoutPanel8.Controls.Add(this.keepAliveCheckBox, 1, 7);
            this.tableLayoutPanel8.Controls.Add(this.usbCheckBox, 1, 9);
            this.tableLayoutPanel8.Controls.Add(this.passwordLabel, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.clipboardCheckBox, 1, 8);
            this.tableLayoutPanel8.Controls.Add(this.autoReconnectCheckBox, 1, 5);
            this.tableLayoutPanel8.Controls.Add(this.domainLabel, 0, 3);
            this.tableLayoutPanel8.Controls.Add(this.autostartProgramsCheckBox, 1, 6);
            this.tableLayoutPanel8.Controls.Add(this.aliasTextBox, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.authenticationLabel, 0, 4);
            this.tableLayoutPanel8.Controls.Add(this.userNameTextBox, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.passwordTextBox, 1, 2);
            this.tableLayoutPanel8.Controls.Add(this.domainTextBox, 1, 3);
            this.tableLayoutPanel8.Controls.Add(this.authenticationLevelComboBox, 1, 4);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel8.RowCount = 12;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(498, 298);
            this.tableLayoutPanel8.TabIndex = 39;
            // 
            // aliasLabel
            // 
            this.aliasLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aliasLabel.AutoSize = true;
            this.aliasLabel.Location = new System.Drawing.Point(6, 9);
            this.aliasLabel.Name = "aliasLabel";
            this.aliasLabel.Size = new System.Drawing.Size(32, 13);
            this.aliasLabel.TabIndex = 37;
            this.aliasLabel.Text = "Alias:";
            // 
            // notificationsCheckBox
            // 
            this.notificationsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.notificationsCheckBox.AutoSize = true;
            this.notificationsCheckBox.Location = new System.Drawing.Point(202, 275);
            this.notificationsCheckBox.Name = "notificationsCheckBox";
            this.notificationsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.notificationsCheckBox.TabIndex = 38;
            this.notificationsCheckBox.Text = "Show Notifications";
            this.notificationsCheckBox.UseVisualStyleBackColor = true;
            this.notificationsCheckBox.CheckedChanged += new System.EventHandler(this.NotificationsCheckBox_CheckedChanged);
            // 
            // printersCheckBox
            // 
            this.printersCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.printersCheckBox.AutoSize = true;
            this.printersCheckBox.Location = new System.Drawing.Point(202, 252);
            this.printersCheckBox.Name = "printersCheckBox";
            this.printersCheckBox.Size = new System.Drawing.Size(104, 17);
            this.printersCheckBox.TabIndex = 11;
            this.printersCheckBox.Text = "Redirect Printers";
            this.printersCheckBox.UseVisualStyleBackColor = true;
            this.printersCheckBox.CheckedChanged += new System.EventHandler(this.PrintersCheckBox_CheckedChanged);
            // 
            // keepAliveCheckBox
            // 
            this.keepAliveCheckBox.AutoSize = true;
            this.keepAliveCheckBox.Location = new System.Drawing.Point(202, 183);
            this.keepAliveCheckBox.Name = "keepAliveCheckBox";
            this.keepAliveCheckBox.Size = new System.Drawing.Size(108, 17);
            this.keepAliveCheckBox.TabIndex = 8;
            this.keepAliveCheckBox.Text = "Keep Alive Agent";
            this.keepAliveCheckBox.UseVisualStyleBackColor = true;
            this.keepAliveCheckBox.CheckedChanged += new System.EventHandler(this.KeepAliveCheckBox_CheckedChanged);
            // 
            // usbCheckBox
            // 
            this.usbCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.usbCheckBox.AutoSize = true;
            this.usbCheckBox.Location = new System.Drawing.Point(202, 229);
            this.usbCheckBox.Name = "usbCheckBox";
            this.usbCheckBox.Size = new System.Drawing.Size(108, 17);
            this.usbCheckBox.TabIndex = 10;
            this.usbCheckBox.Text = "Redirect Devices";
            this.usbCheckBox.UseVisualStyleBackColor = true;
            this.usbCheckBox.CheckedChanged += new System.EventHandler(this.UsbCheckBox_CheckedChanged);
            // 
            // clipboardCheckBox
            // 
            this.clipboardCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clipboardCheckBox.AutoSize = true;
            this.clipboardCheckBox.Location = new System.Drawing.Point(202, 206);
            this.clipboardCheckBox.Name = "clipboardCheckBox";
            this.clipboardCheckBox.Size = new System.Drawing.Size(113, 17);
            this.clipboardCheckBox.TabIndex = 9;
            this.clipboardCheckBox.Text = "Redirect Clipboard";
            this.clipboardCheckBox.UseVisualStyleBackColor = true;
            this.clipboardCheckBox.CheckedChanged += new System.EventHandler(this.ClipboardCheckBox_CheckedChanged);
            // 
            // autoReconnectCheckBox
            // 
            this.autoReconnectCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.autoReconnectCheckBox.AutoSize = true;
            this.autoReconnectCheckBox.Location = new System.Drawing.Point(202, 137);
            this.autoReconnectCheckBox.Name = "autoReconnectCheckBox";
            this.autoReconnectCheckBox.Size = new System.Drawing.Size(104, 17);
            this.autoReconnectCheckBox.TabIndex = 6;
            this.autoReconnectCheckBox.Text = "Auto Reconnect";
            this.autoReconnectCheckBox.UseVisualStyleBackColor = true;
            this.autoReconnectCheckBox.CheckedChanged += new System.EventHandler(this.AutoReconnectCheckBox_CheckedChanged);
            // 
            // domainLabel
            // 
            this.domainLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(6, 87);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(46, 13);
            this.domainLabel.TabIndex = 23;
            this.domainLabel.Text = "Domain:";
            // 
            // autostartProgramsCheckBox
            // 
            this.autostartProgramsCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.autostartProgramsCheckBox.AutoSize = true;
            this.autostartProgramsCheckBox.Location = new System.Drawing.Point(202, 160);
            this.autostartProgramsCheckBox.Name = "autostartProgramsCheckBox";
            this.autostartProgramsCheckBox.Size = new System.Drawing.Size(152, 17);
            this.autostartProgramsCheckBox.TabIndex = 7;
            this.autostartProgramsCheckBox.Text = "Autostart Startup Programs";
            this.autostartProgramsCheckBox.UseVisualStyleBackColor = true;
            this.autostartProgramsCheckBox.CheckedChanged += new System.EventHandler(this.AutostartProgramsCheckBox_CheckedChanged);
            // 
            // aliasTextBox
            // 
            this.aliasTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.aliasTextBox.Location = new System.Drawing.Point(202, 6);
            this.aliasTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.aliasTextBox.Name = "aliasTextBox";
            this.aliasTextBox.Size = new System.Drawing.Size(276, 20);
            this.aliasTextBox.TabIndex = 1;
            this.aliasTextBox.TextChanged += new System.EventHandler(this.AliasTextBox_TextChanged);
            this.aliasTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AliasTextBox_KeyPress);
            // 
            // authenticationLabel
            // 
            this.authenticationLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(6, 114);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(104, 13);
            this.authenticationLabel.TabIndex = 34;
            this.authenticationLabel.Text = "RDP Authentication:";
            // 
            // domainTextBox
            // 
            this.domainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.domainTextBox.HideSelection = false;
            this.domainTextBox.Location = new System.Drawing.Point(202, 84);
            this.domainTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.domainTextBox.Name = "domainTextBox";
            this.domainTextBox.Size = new System.Drawing.Size(276, 20);
            this.domainTextBox.TabIndex = 4;
            this.domainTextBox.TextChanged += new System.EventHandler(this.DomainTextBox_TextChanged);
            // 
            // authenticationLevelComboBox
            // 
            this.authenticationLevelComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticationLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationLevelComboBox.FormattingEnabled = true;
            this.authenticationLevelComboBox.Location = new System.Drawing.Point(202, 110);
            this.authenticationLevelComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.authenticationLevelComboBox.Name = "authenticationLevelComboBox";
            this.authenticationLevelComboBox.Size = new System.Drawing.Size(276, 21);
            this.authenticationLevelComboBox.TabIndex = 5;
            this.authenticationLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.AuthenticationLevelComboBox_SelectedIndexChanged);
            // 
            // enableServerCheckBox
            // 
            this.enableServerCheckBox.AutoSize = true;
            this.enableServerCheckBox.Location = new System.Drawing.Point(3, 3);
            this.enableServerCheckBox.Name = "enableServerCheckBox";
            this.enableServerCheckBox.Size = new System.Drawing.Size(93, 17);
            this.enableServerCheckBox.TabIndex = 0;
            this.enableServerCheckBox.Text = "Enable Server";
            this.enableServerCheckBox.UseVisualStyleBackColor = true;
            this.enableServerCheckBox.CheckedChanged += new System.EventHandler(this.EnableServerCheckBox_CheckedChanged);
            // 
            // serverConnectionGroupBox
            // 
            this.serverConnectionGroupBox.AutoSize = true;
            this.serverConnectionGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.serverConnectionGroupBox.Controls.Add(this.tableLayoutPanel9);
            this.serverConnectionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverConnectionGroupBox.Location = new System.Drawing.Point(3, 349);
            this.serverConnectionGroupBox.Name = "serverConnectionGroupBox";
            this.serverConnectionGroupBox.Size = new System.Drawing.Size(504, 96);
            this.serverConnectionGroupBox.TabIndex = 22;
            this.serverConnectionGroupBox.TabStop = false;
            this.serverConnectionGroupBox.Text = "RAAS Server Connection";
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.AutoSize = true;
            this.tableLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel11, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.tableLayoutPanel10, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel9.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel9.RowCount = 2;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(498, 77);
            this.tableLayoutPanel9.TabIndex = 18;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.AutoSize = true;
            this.tableLayoutPanel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel11.ColumnCount = 3;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel11.Controls.Add(this.connectButton, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.disconnectButton, 2, 0);
            this.tableLayoutPanel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel11.Location = new System.Drawing.Point(4, 44);
            this.tableLayoutPanel11.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel11.Size = new System.Drawing.Size(490, 29);
            this.tableLayoutPanel11.TabIndex = 19;
            // 
            // connectButton
            // 
            this.connectButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.connectButton.AutoSize = true;
            this.connectButton.Location = new System.Drawing.Point(331, 3);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 15;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.disconnectButton.AutoSize = true;
            this.disconnectButton.Location = new System.Drawing.Point(412, 3);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(75, 23);
            this.disconnectButton.TabIndex = 16;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.AutoSize = true;
            this.tableLayoutPanel10.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel10.Controls.Add(this.statusLabel, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.serverVersionLabel, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.serverVersionText, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.connectionText, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 2;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(490, 38);
            this.tableLayoutPanel10.TabIndex = 19;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(3, 3);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(3);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 13);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Status:";
            // 
            // serverVersionLabel
            // 
            this.serverVersionLabel.AutoSize = true;
            this.serverVersionLabel.Location = new System.Drawing.Point(3, 22);
            this.serverVersionLabel.Margin = new System.Windows.Forms.Padding(3);
            this.serverVersionLabel.Name = "serverVersionLabel";
            this.serverVersionLabel.Size = new System.Drawing.Size(111, 13);
            this.serverVersionLabel.TabIndex = 16;
            this.serverVersionLabel.Text = "RAAS Server Version:";
            // 
            // serverVersionText
            // 
            this.serverVersionText.AutoSize = true;
            this.serverVersionText.Location = new System.Drawing.Point(199, 22);
            this.serverVersionText.Margin = new System.Windows.Forms.Padding(3);
            this.serverVersionText.Name = "serverVersionText";
            this.serverVersionText.Size = new System.Drawing.Size(40, 13);
            this.serverVersionText.TabIndex = 17;
            this.serverVersionText.Text = "0.0.0.0";
            // 
            // connectionText
            // 
            this.connectionText.AutoSize = true;
            this.connectionText.Location = new System.Drawing.Point(199, 0);
            this.connectionText.Name = "connectionText";
            this.connectionText.Size = new System.Drawing.Size(0, 13);
            this.connectionText.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.generalTab);
            this.tabControl.Controls.Add(this.remoteShortcutsTab);
            this.tabControl.Controls.Add(this.visualizationsTab);
            this.tabControl.Controls.Add(this.sharesTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(5, 67);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(524, 487);
            this.tabControl.TabIndex = 23;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.tableLayoutPanel1);
            this.generalTab.Location = new System.Drawing.Point(4, 22);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(3);
            this.generalTab.Size = new System.Drawing.Size(516, 461);
            this.generalTab.TabIndex = 0;
            this.generalTab.Text = "General";
            this.generalTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.enableServerCheckBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.serverConnectionGroupBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.settingsGroupBox, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(510, 455);
            this.tableLayoutPanel1.TabIndex = 23;
            // 
            // remoteShortcutsTab
            // 
            this.remoteShortcutsTab.Controls.Add(this.tableLayoutPanel5);
            this.remoteShortcutsTab.Location = new System.Drawing.Point(4, 22);
            this.remoteShortcutsTab.Name = "remoteShortcutsTab";
            this.remoteShortcutsTab.Padding = new System.Windows.Forms.Padding(3);
            this.remoteShortcutsTab.Size = new System.Drawing.Size(516, 461);
            this.remoteShortcutsTab.TabIndex = 3;
            this.remoteShortcutsTab.Text = "Remote Shortcuts";
            this.remoteShortcutsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.enableShortcutsCheckBox, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.shortcutsGroupBox, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(510, 455);
            this.tableLayoutPanel5.TabIndex = 13;
            // 
            // enableShortcutsCheckBox
            // 
            this.enableShortcutsCheckBox.AutoSize = true;
            this.enableShortcutsCheckBox.Location = new System.Drawing.Point(3, 3);
            this.enableShortcutsCheckBox.Name = "enableShortcutsCheckBox";
            this.enableShortcutsCheckBox.Size = new System.Drawing.Size(147, 17);
            this.enableShortcutsCheckBox.TabIndex = 0;
            this.enableShortcutsCheckBox.Text = "Enable Remote Shortcuts";
            this.enableShortcutsCheckBox.UseVisualStyleBackColor = true;
            this.enableShortcutsCheckBox.CheckedChanged += new System.EventHandler(this.EnableShortcutsCheckBox_CheckedChanged);
            // 
            // shortcutsGroupBox
            // 
            this.shortcutsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shortcutsGroupBox.AutoSize = true;
            this.shortcutsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.shortcutsGroupBox.Controls.Add(this.tableLayoutPanel12);
            this.shortcutsGroupBox.Location = new System.Drawing.Point(3, 26);
            this.shortcutsGroupBox.Name = "shortcutsGroupBox";
            this.shortcutsGroupBox.Size = new System.Drawing.Size(504, 163);
            this.shortcutsGroupBox.TabIndex = 12;
            this.shortcutsGroupBox.TabStop = false;
            this.shortcutsGroupBox.Text = "Remote Shortcuts Settings";
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.AutoSize = true;
            this.tableLayoutPanel12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel12.ColumnCount = 1;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel12.Controls.Add(this.startMenuShortcutsCheckBox, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.localizeShortcutsCheckBox, 0, 4);
            this.tableLayoutPanel12.Controls.Add(this.uwpApplicationShortcutsCheckBox, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.desktopShortcutsCheckBox, 0, 2);
            this.tableLayoutPanel12.Controls.Add(this.shortcutsAppendAliasCheckBox, 0, 5);
            this.tableLayoutPanel12.Controls.Add(this.desktopRootCheckBox, 0, 3);
            this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel12.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel12.RowCount = 6;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel12.Size = new System.Drawing.Size(498, 144);
            this.tableLayoutPanel12.TabIndex = 5;
            // 
            // startMenuShortcutsCheckBox
            // 
            this.startMenuShortcutsCheckBox.AutoSize = true;
            this.startMenuShortcutsCheckBox.Location = new System.Drawing.Point(6, 6);
            this.startMenuShortcutsCheckBox.Name = "startMenuShortcutsCheckBox";
            this.startMenuShortcutsCheckBox.Size = new System.Drawing.Size(159, 17);
            this.startMenuShortcutsCheckBox.TabIndex = 1;
            this.startMenuShortcutsCheckBox.Text = "Create Start-menu Shortcuts";
            this.startMenuShortcutsCheckBox.UseVisualStyleBackColor = true;
            this.startMenuShortcutsCheckBox.CheckedChanged += new System.EventHandler(this.StartMenuShortcutsCheckBox_CheckedChanged);
            // 
            // localizeShortcutsCheckBox
            // 
            this.localizeShortcutsCheckBox.AutoSize = true;
            this.localizeShortcutsCheckBox.Location = new System.Drawing.Point(6, 98);
            this.localizeShortcutsCheckBox.Name = "localizeShortcutsCheckBox";
            this.localizeShortcutsCheckBox.Size = new System.Drawing.Size(202, 17);
            this.localizeShortcutsCheckBox.TabIndex = 5;
            this.localizeShortcutsCheckBox.Text = "Server Language for Shortcut Names";
            this.localizeShortcutsCheckBox.UseVisualStyleBackColor = true;
            this.localizeShortcutsCheckBox.CheckedChanged += new System.EventHandler(this.LocalizeShortcutsCheckBox_CheckedChanged);
            // 
            // uwpApplicationShortcutsCheckBox
            // 
            this.uwpApplicationShortcutsCheckBox.AutoSize = true;
            this.uwpApplicationShortcutsCheckBox.Location = new System.Drawing.Point(6, 29);
            this.uwpApplicationShortcutsCheckBox.Name = "uwpApplicationShortcutsCheckBox";
            this.uwpApplicationShortcutsCheckBox.Size = new System.Drawing.Size(174, 17);
            this.uwpApplicationShortcutsCheckBox.TabIndex = 2;
            this.uwpApplicationShortcutsCheckBox.Text = "Create Windows App Shortcuts";
            this.uwpApplicationShortcutsCheckBox.UseVisualStyleBackColor = true;
            this.uwpApplicationShortcutsCheckBox.CheckedChanged += new System.EventHandler(this.uwpApplicationShortcutsCheckBox_CheckedChanged);
            // 
            // desktopShortcutsCheckBox
            // 
            this.desktopShortcutsCheckBox.AutoSize = true;
            this.desktopShortcutsCheckBox.Location = new System.Drawing.Point(6, 52);
            this.desktopShortcutsCheckBox.Name = "desktopShortcutsCheckBox";
            this.desktopShortcutsCheckBox.Size = new System.Drawing.Size(148, 17);
            this.desktopShortcutsCheckBox.TabIndex = 3;
            this.desktopShortcutsCheckBox.Text = "Create Desktop Shortcuts";
            this.desktopShortcutsCheckBox.UseVisualStyleBackColor = true;
            this.desktopShortcutsCheckBox.CheckedChanged += new System.EventHandler(this.DesktopShortcutsCheckBox_CheckedChanged);
            // 
            // shortcutsAppendAliasCheckBox
            // 
            this.shortcutsAppendAliasCheckBox.AutoSize = true;
            this.shortcutsAppendAliasCheckBox.Location = new System.Drawing.Point(6, 121);
            this.shortcutsAppendAliasCheckBox.Name = "shortcutsAppendAliasCheckBox";
            this.shortcutsAppendAliasCheckBox.Size = new System.Drawing.Size(213, 17);
            this.shortcutsAppendAliasCheckBox.TabIndex = 6;
            this.shortcutsAppendAliasCheckBox.Text = "Append Server Alias to Shortcut Names";
            this.shortcutsAppendAliasCheckBox.UseVisualStyleBackColor = true;
            this.shortcutsAppendAliasCheckBox.CheckedChanged += new System.EventHandler(this.ShortcutsAppendAliasCheckBox_CheckedChanged);
            // 
            // desktopRootCheckBox
            // 
            this.desktopRootCheckBox.AutoSize = true;
            this.desktopRootCheckBox.Location = new System.Drawing.Point(6, 75);
            this.desktopRootCheckBox.Name = "desktopRootCheckBox";
            this.desktopRootCheckBox.Size = new System.Drawing.Size(228, 17);
            this.desktopRootCheckBox.TabIndex = 4;
            this.desktopRootCheckBox.Text = "Place Desktop Shortcuts on Desktop Root";
            this.desktopRootCheckBox.UseVisualStyleBackColor = true;
            this.desktopRootCheckBox.CheckedChanged += new System.EventHandler(this.DesktopRootCheckBox_CheckedChanged);
            // 
            // visualizationsTab
            // 
            this.visualizationsTab.Controls.Add(this.tableLayoutPanel6);
            this.visualizationsTab.Controls.Add(this.crosslinesCheckBox);
            this.visualizationsTab.Location = new System.Drawing.Point(4, 22);
            this.visualizationsTab.Name = "visualizationsTab";
            this.visualizationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.visualizationsTab.Size = new System.Drawing.Size(516, 461);
            this.visualizationsTab.TabIndex = 1;
            this.visualizationsTab.Text = "Visualizations";
            this.visualizationsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.visualizationsCheckBox, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.visualizationsSettingsGroupBox, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(510, 455);
            this.tableLayoutPanel6.TabIndex = 33;
            // 
            // visualizationsCheckBox
            // 
            this.visualizationsCheckBox.AutoSize = true;
            this.visualizationsCheckBox.Location = new System.Drawing.Point(3, 3);
            this.visualizationsCheckBox.Name = "visualizationsCheckBox";
            this.visualizationsCheckBox.Size = new System.Drawing.Size(125, 17);
            this.visualizationsCheckBox.TabIndex = 0;
            this.visualizationsCheckBox.Text = "Enable Visualizations";
            this.visualizationsCheckBox.UseVisualStyleBackColor = true;
            this.visualizationsCheckBox.CheckedChanged += new System.EventHandler(this.VisualizationsCheckBox_CheckedChanged);
            // 
            // visualizationsSettingsGroupBox
            // 
            this.visualizationsSettingsGroupBox.AutoSize = true;
            this.visualizationsSettingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.visualizationsSettingsGroupBox.Controls.Add(this.tableLayoutPanel14);
            this.visualizationsSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visualizationsSettingsGroupBox.Location = new System.Drawing.Point(3, 26);
            this.visualizationsSettingsGroupBox.Name = "visualizationsSettingsGroupBox";
            this.visualizationsSettingsGroupBox.Size = new System.Drawing.Size(504, 209);
            this.visualizationsSettingsGroupBox.TabIndex = 32;
            this.visualizationsSettingsGroupBox.TabStop = false;
            this.visualizationsSettingsGroupBox.Text = "Visualizations Settings";
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.AutoSize = true;
            this.tableLayoutPanel14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel14.Controls.Add(this.mainColorButton, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.connectionBarTrackBar, 1, 7);
            this.tableLayoutPanel14.Controls.Add(this.connectionBarCheckBox, 1, 6);
            this.tableLayoutPanel14.Controls.Add(this.connectionOpacityLabel, 0, 7);
            this.tableLayoutPanel14.Controls.Add(this.mainColorTextBox, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.textColorButton, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.textColorTextBox, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.lineColorButton, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.lineColorTextBox, 1, 2);
            this.tableLayoutPanel14.Controls.Add(this.framesCheckBox, 1, 3);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel14.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel14.RowCount = 8;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel14.Size = new System.Drawing.Size(498, 190);
            this.tableLayoutPanel14.TabIndex = 32;
            // 
            // mainColorButton
            // 
            this.mainColorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mainColorButton.Location = new System.Drawing.Point(110, 6);
            this.mainColorButton.Name = "mainColorButton";
            this.mainColorButton.Size = new System.Drawing.Size(86, 23);
            this.mainColorButton.TabIndex = 1;
            this.mainColorButton.Text = "Main Color";
            this.mainColorButton.UseVisualStyleBackColor = true;
            this.mainColorButton.Click += new System.EventHandler(this.MainColorButton_Click);
            // 
            // connectionBarTrackBar
            // 
            this.connectionBarTrackBar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionBarTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.connectionBarTrackBar.Location = new System.Drawing.Point(202, 139);
            this.connectionBarTrackBar.Name = "connectionBarTrackBar";
            this.connectionBarTrackBar.Size = new System.Drawing.Size(204, 45);
            this.connectionBarTrackBar.TabIndex = 8;
            this.connectionBarTrackBar.Scroll += new System.EventHandler(this.ConnectionBarTrackBar_Scroll);
            // 
            // connectionBarCheckBox
            // 
            this.connectionBarCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionBarCheckBox.AutoSize = true;
            this.connectionBarCheckBox.Location = new System.Drawing.Point(202, 116);
            this.connectionBarCheckBox.Name = "connectionBarCheckBox";
            this.connectionBarCheckBox.Size = new System.Drawing.Size(99, 17);
            this.connectionBarCheckBox.TabIndex = 7;
            this.connectionBarCheckBox.Text = "Connection Bar";
            this.connectionBarCheckBox.UseVisualStyleBackColor = true;
            this.connectionBarCheckBox.CheckedChanged += new System.EventHandler(this.ConnectionBarCheckBox_CheckedChanged);
            // 
            // connectionOpacityLabel
            // 
            this.connectionOpacityLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionOpacityLabel.AutoSize = true;
            this.connectionOpacityLabel.Location = new System.Drawing.Point(6, 155);
            this.connectionOpacityLabel.Name = "connectionOpacityLabel";
            this.connectionOpacityLabel.Size = new System.Drawing.Size(118, 13);
            this.connectionOpacityLabel.TabIndex = 31;
            this.connectionOpacityLabel.Text = "Connectionbar Opacity:";
            // 
            // mainColorTextBox
            // 
            this.mainColorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mainColorTextBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.mainColorTextBox.Location = new System.Drawing.Point(202, 7);
            this.mainColorTextBox.Name = "mainColorTextBox";
            this.mainColorTextBox.ReadOnly = true;
            this.mainColorTextBox.Size = new System.Drawing.Size(20, 20);
            this.mainColorTextBox.TabIndex = 23;
            this.mainColorTextBox.TabStop = false;
            // 
            // textColorButton
            // 
            this.textColorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textColorButton.Location = new System.Drawing.Point(110, 35);
            this.textColorButton.Name = "textColorButton";
            this.textColorButton.Size = new System.Drawing.Size(86, 23);
            this.textColorButton.TabIndex = 2;
            this.textColorButton.Text = "Text Color";
            this.textColorButton.UseVisualStyleBackColor = true;
            this.textColorButton.Click += new System.EventHandler(this.TextColorButton_Click);
            // 
            // textColorTextBox
            // 
            this.textColorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textColorTextBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.textColorTextBox.Location = new System.Drawing.Point(202, 36);
            this.textColorTextBox.Name = "textColorTextBox";
            this.textColorTextBox.ReadOnly = true;
            this.textColorTextBox.Size = new System.Drawing.Size(20, 20);
            this.textColorTextBox.TabIndex = 25;
            this.textColorTextBox.TabStop = false;
            // 
            // lineColorButton
            // 
            this.lineColorButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lineColorButton.Location = new System.Drawing.Point(110, 64);
            this.lineColorButton.Name = "lineColorButton";
            this.lineColorButton.Size = new System.Drawing.Size(86, 23);
            this.lineColorButton.TabIndex = 3;
            this.lineColorButton.Text = "Line Color";
            this.lineColorButton.UseVisualStyleBackColor = true;
            this.lineColorButton.Click += new System.EventHandler(this.LineColorButton_Click);
            // 
            // lineColorTextBox
            // 
            this.lineColorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lineColorTextBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lineColorTextBox.Location = new System.Drawing.Point(202, 65);
            this.lineColorTextBox.Name = "lineColorTextBox";
            this.lineColorTextBox.ReadOnly = true;
            this.lineColorTextBox.Size = new System.Drawing.Size(20, 20);
            this.lineColorTextBox.TabIndex = 27;
            this.lineColorTextBox.TabStop = false;
            // 
            // framesCheckBox
            // 
            this.framesCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.framesCheckBox.AutoSize = true;
            this.framesCheckBox.Location = new System.Drawing.Point(202, 93);
            this.framesCheckBox.Name = "framesCheckBox";
            this.framesCheckBox.Size = new System.Drawing.Size(60, 17);
            this.framesCheckBox.TabIndex = 4;
            this.framesCheckBox.Text = "Frames";
            this.framesCheckBox.UseVisualStyleBackColor = true;
            this.framesCheckBox.CheckedChanged += new System.EventHandler(this.FramesCheckBox_CheckedChanged);
            // 
            // crosslinesCheckBox
            // 
            this.crosslinesCheckBox.AutoSize = true;
            this.crosslinesCheckBox.Location = new System.Drawing.Point(135, 432);
            this.crosslinesCheckBox.Name = "crosslinesCheckBox";
            this.crosslinesCheckBox.Size = new System.Drawing.Size(73, 17);
            this.crosslinesCheckBox.TabIndex = 9;
            this.crosslinesCheckBox.TabStop = false;
            this.crosslinesCheckBox.Text = "Crosslines";
            this.crosslinesCheckBox.UseVisualStyleBackColor = true;
            this.crosslinesCheckBox.Visible = false;
            this.crosslinesCheckBox.CheckedChanged += new System.EventHandler(this.CrosslinesCheckBox_CheckedChanged);
            // 
            // sharesTab
            // 
            this.sharesTab.Controls.Add(this.tableLayoutPanel7);
            this.sharesTab.Location = new System.Drawing.Point(4, 22);
            this.sharesTab.Name = "sharesTab";
            this.sharesTab.Padding = new System.Windows.Forms.Padding(3);
            this.sharesTab.Size = new System.Drawing.Size(516, 461);
            this.sharesTab.TabIndex = 2;
            this.sharesTab.Text = "Explorer Extension";
            this.sharesTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.explorerCheckBox, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.explorerGroupBox, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.devicesAndDrivesGroupBox, 0, 2);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 4;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(510, 455);
            this.tableLayoutPanel7.TabIndex = 36;
            // 
            // explorerCheckBox
            // 
            this.explorerCheckBox.AutoSize = true;
            this.explorerCheckBox.Location = new System.Drawing.Point(3, 3);
            this.explorerCheckBox.Name = "explorerCheckBox";
            this.explorerCheckBox.Size = new System.Drawing.Size(160, 17);
            this.explorerCheckBox.TabIndex = 0;
            this.explorerCheckBox.Text = "Enable in Explorer Extension";
            this.explorerCheckBox.UseVisualStyleBackColor = true;
            this.explorerCheckBox.CheckedChanged += new System.EventHandler(this.ExplorerCheckBox_CheckedChanged);
            // 
            // explorerGroupBox
            // 
            this.explorerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.explorerGroupBox.AutoSize = true;
            this.explorerGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.explorerGroupBox.Controls.Add(this.visibleFoldersTableLayoutPanel);
            this.explorerGroupBox.Location = new System.Drawing.Point(3, 26);
            this.explorerGroupBox.Name = "explorerGroupBox";
            this.explorerGroupBox.Size = new System.Drawing.Size(504, 232);
            this.explorerGroupBox.TabIndex = 35;
            this.explorerGroupBox.TabStop = false;
            this.explorerGroupBox.Text = "Visible Folders";
            // 
            // visibleFoldersTableLayoutPanel
            // 
            this.visibleFoldersTableLayoutPanel.AutoSize = true;
            this.visibleFoldersTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.visibleFoldersTableLayoutPanel.ColumnCount = 1;
            this.visibleFoldersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.desktopCheckBox, 0, 0);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.documentsCheckBox, 0, 1);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.downloadsCheckBox, 0, 2);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.favoritesCheckBox, 0, 3);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.linksCheckBox, 0, 4);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.musicCheckBox, 0, 5);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.picturesCheckBox, 0, 6);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.savedGamesCheckBox, 0, 7);
            this.visibleFoldersTableLayoutPanel.Controls.Add(this.videosCheckBox, 0, 8);
            this.visibleFoldersTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visibleFoldersTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.visibleFoldersTableLayoutPanel.Margin = new System.Windows.Forms.Padding(1);
            this.visibleFoldersTableLayoutPanel.Name = "visibleFoldersTableLayoutPanel";
            this.visibleFoldersTableLayoutPanel.Padding = new System.Windows.Forms.Padding(3);
            this.visibleFoldersTableLayoutPanel.RowCount = 16;
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.visibleFoldersTableLayoutPanel.Size = new System.Drawing.Size(498, 213);
            this.visibleFoldersTableLayoutPanel.TabIndex = 17;
            // 
            // desktopCheckBox
            // 
            this.desktopCheckBox.AutoSize = true;
            this.desktopCheckBox.Location = new System.Drawing.Point(6, 6);
            this.desktopCheckBox.Name = "desktopCheckBox";
            this.desktopCheckBox.Size = new System.Drawing.Size(66, 17);
            this.desktopCheckBox.TabIndex = 3;
            this.desktopCheckBox.Text = "Desktop";
            this.desktopCheckBox.UseVisualStyleBackColor = true;
            this.desktopCheckBox.CheckedChanged += new System.EventHandler(this.DesktopCheckBox_CheckedChanged);
            // 
            // documentsCheckBox
            // 
            this.documentsCheckBox.AutoSize = true;
            this.documentsCheckBox.Location = new System.Drawing.Point(6, 29);
            this.documentsCheckBox.Name = "documentsCheckBox";
            this.documentsCheckBox.Size = new System.Drawing.Size(80, 17);
            this.documentsCheckBox.TabIndex = 4;
            this.documentsCheckBox.Text = "Documents";
            this.documentsCheckBox.UseVisualStyleBackColor = true;
            this.documentsCheckBox.CheckedChanged += new System.EventHandler(this.DocumentsCheckBox_CheckedChanged);
            // 
            // downloadsCheckBox
            // 
            this.downloadsCheckBox.AutoSize = true;
            this.downloadsCheckBox.Location = new System.Drawing.Point(6, 52);
            this.downloadsCheckBox.Name = "downloadsCheckBox";
            this.downloadsCheckBox.Size = new System.Drawing.Size(79, 17);
            this.downloadsCheckBox.TabIndex = 5;
            this.downloadsCheckBox.Text = "Downloads";
            this.downloadsCheckBox.UseVisualStyleBackColor = true;
            this.downloadsCheckBox.CheckedChanged += new System.EventHandler(this.DownloadsCheckBox_CheckedChanged);
            // 
            // favoritesCheckBox
            // 
            this.favoritesCheckBox.AutoSize = true;
            this.favoritesCheckBox.Location = new System.Drawing.Point(6, 75);
            this.favoritesCheckBox.Name = "favoritesCheckBox";
            this.favoritesCheckBox.Size = new System.Drawing.Size(69, 17);
            this.favoritesCheckBox.TabIndex = 6;
            this.favoritesCheckBox.Text = "Favorites";
            this.favoritesCheckBox.UseVisualStyleBackColor = true;
            this.favoritesCheckBox.CheckedChanged += new System.EventHandler(this.FavoritesCheckBox_CheckedChanged);
            // 
            // linksCheckBox
            // 
            this.linksCheckBox.AutoSize = true;
            this.linksCheckBox.Location = new System.Drawing.Point(6, 98);
            this.linksCheckBox.Name = "linksCheckBox";
            this.linksCheckBox.Size = new System.Drawing.Size(51, 17);
            this.linksCheckBox.TabIndex = 7;
            this.linksCheckBox.Text = "Links";
            this.linksCheckBox.UseVisualStyleBackColor = true;
            this.linksCheckBox.CheckedChanged += new System.EventHandler(this.LinksCheckBox_CheckedChanged);
            // 
            // musicCheckBox
            // 
            this.musicCheckBox.AutoSize = true;
            this.musicCheckBox.Location = new System.Drawing.Point(6, 121);
            this.musicCheckBox.Name = "musicCheckBox";
            this.musicCheckBox.Size = new System.Drawing.Size(54, 17);
            this.musicCheckBox.TabIndex = 8;
            this.musicCheckBox.Text = "Music";
            this.musicCheckBox.UseVisualStyleBackColor = true;
            this.musicCheckBox.CheckedChanged += new System.EventHandler(this.MusicCheckBox_CheckedChanged);
            // 
            // picturesCheckBox
            // 
            this.picturesCheckBox.AutoSize = true;
            this.picturesCheckBox.Location = new System.Drawing.Point(6, 144);
            this.picturesCheckBox.Name = "picturesCheckBox";
            this.picturesCheckBox.Size = new System.Drawing.Size(64, 17);
            this.picturesCheckBox.TabIndex = 9;
            this.picturesCheckBox.Text = "Pictures";
            this.picturesCheckBox.UseVisualStyleBackColor = true;
            this.picturesCheckBox.CheckedChanged += new System.EventHandler(this.PicturesCheckBox_CheckedChanged);
            // 
            // savedGamesCheckBox
            // 
            this.savedGamesCheckBox.AutoSize = true;
            this.savedGamesCheckBox.Location = new System.Drawing.Point(6, 167);
            this.savedGamesCheckBox.Name = "savedGamesCheckBox";
            this.savedGamesCheckBox.Size = new System.Drawing.Size(93, 17);
            this.savedGamesCheckBox.TabIndex = 10;
            this.savedGamesCheckBox.Text = "Saved Games";
            this.savedGamesCheckBox.UseVisualStyleBackColor = true;
            this.savedGamesCheckBox.CheckedChanged += new System.EventHandler(this.SavedGamesCheckBox_CheckedChanged);
            // 
            // videosCheckBox
            // 
            this.videosCheckBox.AutoSize = true;
            this.videosCheckBox.Location = new System.Drawing.Point(6, 190);
            this.videosCheckBox.Name = "videosCheckBox";
            this.videosCheckBox.Size = new System.Drawing.Size(58, 17);
            this.videosCheckBox.TabIndex = 12;
            this.videosCheckBox.Text = "Videos";
            this.videosCheckBox.UseVisualStyleBackColor = true;
            this.videosCheckBox.CheckedChanged += new System.EventHandler(this.VideosCheckBox_CheckedChanged);
            // 
            // devicesAndDrivesGroupBox
            // 
            this.devicesAndDrivesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.devicesAndDrivesGroupBox.AutoSize = true;
            this.devicesAndDrivesGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.devicesAndDrivesGroupBox.Controls.Add(this.tableLayoutPanel13);
            this.devicesAndDrivesGroupBox.Location = new System.Drawing.Point(3, 264);
            this.devicesAndDrivesGroupBox.Name = "devicesAndDrivesGroupBox";
            this.devicesAndDrivesGroupBox.Size = new System.Drawing.Size(504, 117);
            this.devicesAndDrivesGroupBox.TabIndex = 36;
            this.devicesAndDrivesGroupBox.TabStop = false;
            this.devicesAndDrivesGroupBox.Text = "Visible Devices and Drives";
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.AutoSize = true;
            this.tableLayoutPanel13.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel13.ColumnCount = 1;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tableLayoutPanel13.Controls.Add(this.disketteDrivesCheckBox, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.removableDrivesCheckBox, 0, 3);
            this.tableLayoutPanel13.Controls.Add(this.hardDrivesCheckBox, 0, 1);
            this.tableLayoutPanel13.Controls.Add(this.cdDrivesCheckBox, 0, 2);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel13.RowCount = 4;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(498, 98);
            this.tableLayoutPanel13.TabIndex = 0;
            // 
            // disketteDrivesCheckBox
            // 
            this.disketteDrivesCheckBox.AutoSize = true;
            this.disketteDrivesCheckBox.Location = new System.Drawing.Point(6, 6);
            this.disketteDrivesCheckBox.Name = "disketteDrivesCheckBox";
            this.disketteDrivesCheckBox.Size = new System.Drawing.Size(114, 17);
            this.disketteDrivesCheckBox.TabIndex = 13;
            this.disketteDrivesCheckBox.Text = "Floppy Disk Drives";
            this.disketteDrivesCheckBox.UseVisualStyleBackColor = true;
            this.disketteDrivesCheckBox.CheckedChanged += new System.EventHandler(this.DisketteDrivesCheckBox_CheckedChanged);
            // 
            // removableDrivesCheckBox
            // 
            this.removableDrivesCheckBox.AutoSize = true;
            this.removableDrivesCheckBox.Location = new System.Drawing.Point(6, 75);
            this.removableDrivesCheckBox.Name = "removableDrivesCheckBox";
            this.removableDrivesCheckBox.Size = new System.Drawing.Size(113, 17);
            this.removableDrivesCheckBox.TabIndex = 16;
            this.removableDrivesCheckBox.Text = "Removable Drives";
            this.removableDrivesCheckBox.UseVisualStyleBackColor = true;
            this.removableDrivesCheckBox.CheckedChanged += new System.EventHandler(this.RemovableDrivesCheckBox_CheckedChanged);
            // 
            // hardDrivesCheckBox
            // 
            this.hardDrivesCheckBox.AutoSize = true;
            this.hardDrivesCheckBox.Location = new System.Drawing.Point(6, 29);
            this.hardDrivesCheckBox.Name = "hardDrivesCheckBox";
            this.hardDrivesCheckBox.Size = new System.Drawing.Size(82, 17);
            this.hardDrivesCheckBox.TabIndex = 14;
            this.hardDrivesCheckBox.Text = "Hard Drives";
            this.hardDrivesCheckBox.UseVisualStyleBackColor = true;
            this.hardDrivesCheckBox.CheckedChanged += new System.EventHandler(this.HardDrivesCheckBox_CheckedChanged);
            // 
            // cdDrivesCheckBox
            // 
            this.cdDrivesCheckBox.AutoSize = true;
            this.cdDrivesCheckBox.Location = new System.Drawing.Point(6, 52);
            this.cdDrivesCheckBox.Name = "cdDrivesCheckBox";
            this.cdDrivesCheckBox.Size = new System.Drawing.Size(74, 17);
            this.cdDrivesCheckBox.TabIndex = 15;
            this.cdDrivesCheckBox.Text = "CD Drives";
            this.cdDrivesCheckBox.UseVisualStyleBackColor = true;
            this.cdDrivesCheckBox.CheckedChanged += new System.EventHandler(this.CdDrivesCheckBox_CheckedChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(3, 3);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 25;
            this.serverLabel.Text = "Server:";
            // 
            // removeButton
            // 
            this.removeButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.removeButton.AutoSize = true;
            this.removeButton.Location = new System.Drawing.Point(450, 3);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.AutoSize = true;
            this.addButton.Location = new System.Drawing.Point(369, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add...";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel4, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.tabControl, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 493F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(534, 601);
            this.tableLayoutPanel.TabIndex = 26;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.removeButton, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.addButton, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 34);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(528, 29);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.serversComboBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.serverLabel, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(528, 29);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // serversComboBox
            // 
            this.serversComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serversComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serversComboBox.FormattingEnabled = true;
            this.serversComboBox.Location = new System.Drawing.Point(46, 3);
            this.serversComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.serversComboBox.Name = "serversComboBox";
            this.serversComboBox.Size = new System.Drawing.Size(479, 21);
            this.serversComboBox.TabIndex = 0;
            this.serversComboBox.SelectedIndexChanged += new System.EventHandler(this.ServersComboBox_SelectedIndexChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.okButton, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.applyButton, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.cancelButton, 2, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 558);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(528, 40);
            this.tableLayoutPanel4.TabIndex = 24;
            // 
            // ServerConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(534, 601);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerConfigForm";
            this.Text = "RAAS Server Configuration";
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.serverConnectionGroupBox.ResumeLayout(false);
            this.serverConnectionGroupBox.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.generalTab.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.remoteShortcutsTab.ResumeLayout(false);
            this.remoteShortcutsTab.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.shortcutsGroupBox.ResumeLayout(false);
            this.shortcutsGroupBox.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            this.visualizationsTab.ResumeLayout(false);
            this.visualizationsTab.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.visualizationsSettingsGroupBox.ResumeLayout(false);
            this.visualizationsSettingsGroupBox.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectionBarTrackBar)).EndInit();
            this.sharesTab.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.explorerGroupBox.ResumeLayout(false);
            this.explorerGroupBox.PerformLayout();
            this.visibleFoldersTableLayoutPanel.ResumeLayout(false);
            this.visibleFoldersTableLayoutPanel.PerformLayout();
            this.devicesAndDrivesGroupBox.ResumeLayout(false);
            this.devicesAndDrivesGroupBox.PerformLayout();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel13.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label accountLabel;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.CheckBox autostartProgramsCheckBox;
        private System.Windows.Forms.TextBox domainTextBox;
        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.CheckBox printersCheckBox;
        private System.Windows.Forms.CheckBox usbCheckBox;
        private System.Windows.Forms.CheckBox clipboardCheckBox;
        private System.Windows.Forms.CheckBox enableServerCheckBox;
        private System.Windows.Forms.GroupBox serverConnectionGroupBox;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label connectionText;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage visualizationsTab;
        private System.Windows.Forms.TabPage generalTab;
        private System.Windows.Forms.CheckBox framesCheckBox;
        private System.Windows.Forms.CheckBox crosslinesCheckBox;
        private System.Windows.Forms.ColorDialog colorPickerDialog;
        private System.Windows.Forms.Button mainColorButton;
        private System.Windows.Forms.TextBox mainColorTextBox;
        private System.Windows.Forms.Button lineColorButton;
        private System.Windows.Forms.TextBox lineColorTextBox;
        private System.Windows.Forms.Button textColorButton;
        private System.Windows.Forms.TextBox textColorTextBox;
        private System.Windows.Forms.GroupBox visualizationsSettingsGroupBox;
        private System.Windows.Forms.TrackBar connectionBarTrackBar;
        private System.Windows.Forms.Label connectionOpacityLabel;
        private System.Windows.Forms.CheckBox connectionBarCheckBox;
        private System.Windows.Forms.ComboBox authenticationLevelComboBox;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.TextBox aliasTextBox;
        private System.Windows.Forms.Label aliasLabel;
        private System.Windows.Forms.CheckBox visualizationsCheckBox;
        private System.Windows.Forms.TabPage sharesTab;
        private System.Windows.Forms.GroupBox explorerGroupBox;
        private System.Windows.Forms.CheckBox cdDrivesCheckBox;
        private System.Windows.Forms.CheckBox hardDrivesCheckBox;
        private System.Windows.Forms.CheckBox picturesCheckBox;
        private System.Windows.Forms.CheckBox disketteDrivesCheckBox;
        private System.Windows.Forms.CheckBox videosCheckBox;
        private System.Windows.Forms.CheckBox savedGamesCheckBox;
        private System.Windows.Forms.CheckBox desktopCheckBox;
        private System.Windows.Forms.CheckBox musicCheckBox;
        private System.Windows.Forms.CheckBox linksCheckBox;
        private System.Windows.Forms.CheckBox favoritesCheckBox;
        private System.Windows.Forms.CheckBox documentsCheckBox;
        private System.Windows.Forms.CheckBox explorerCheckBox;
        private System.Windows.Forms.CheckBox downloadsCheckBox;
        private System.Windows.Forms.CheckBox autoReconnectCheckBox;
        private System.Windows.Forms.CheckBox removableDrivesCheckBox;
        private System.Windows.Forms.CheckBox keepAliveCheckBox;
        private System.Windows.Forms.Label serverVersionText;
        private System.Windows.Forms.Label serverVersionLabel;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private TabPage remoteShortcutsTab;
        private GroupBox shortcutsGroupBox;
        private CheckBox desktopRootCheckBox;
        private CheckBox enableShortcutsCheckBox;
        private CheckBox shortcutsAppendAliasCheckBox;
        private CheckBox desktopShortcutsCheckBox;
        private CheckBox startMenuShortcutsCheckBox;
        private CheckBox notificationsCheckBox;
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox serversComboBox;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel11;
        private TableLayoutPanel tableLayoutPanel10;
        private TableLayoutPanel tableLayoutPanel12;
        private TableLayoutPanel visibleFoldersTableLayoutPanel;
        private TableLayoutPanel tableLayoutPanel14;
        private CheckBox localizeShortcutsCheckBox;
        private GroupBox devicesAndDrivesGroupBox;
        private TableLayoutPanel tableLayoutPanel13;
        private CheckBox uwpApplicationShortcutsCheckBox;
    }
}

