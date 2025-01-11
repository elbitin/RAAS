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
﻿
using Elbitin.Applications.RAAS.RAASClient.RDesktop.Properties;

namespace Elbitin.Applications.RAAS.RAASClient.RDesktop
{
    partial class RemoteDesktopSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteDesktopSettingsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.serverLabel = new System.Windows.Forms.Label();
            this.serversComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.connectButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.authenticationLabel = new System.Windows.Forms.Label();
            this.authenticationLevelComboBox = new System.Windows.Forms.ComboBox();
            this.clipboardCheckBox = new System.Windows.Forms.CheckBox();
            this.usbCheckBox = new System.Windows.Forms.CheckBox();
            this.printersCheckBox = new System.Windows.Forms.CheckBox();
            this.fullscreenCheckBox = new System.Windows.Forms.CheckBox();
            this.connectionBarCheckBox = new System.Windows.Forms.CheckBox();
            this.pinConnectionBarCheckBox = new System.Windows.Forms.CheckBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.widthLabel = new System.Windows.Forms.Label();
            this.heightLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.settingsGroupBox.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.serverLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.serversComboBox, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(528, 32);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // serverLabel
            // 
            this.serverLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(3, 9);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 26;
            this.serverLabel.Text = "Server:";
            // 
            // serversComboBox
            // 
            this.serversComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.serversComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.serversComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serversComboBox.FormattingEnabled = true;
            this.serversComboBox.Location = new System.Drawing.Point(46, 5);
            this.serversComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.serversComboBox.Name = "serversComboBox";
            this.serversComboBox.Size = new System.Drawing.Size(479, 21);
            this.serversComboBox.TabIndex = 4;
            this.serversComboBox.SelectedIndexChanged += new System.EventHandler(this.serversComboBox_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.settingsGroupBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(534, 339);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.connectButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.applyButton, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.cancelButton, 2, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 303);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(528, 33);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // connectButton
            // 
            this.connectButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.connectButton.AutoSize = true;
            this.connectButton.Location = new System.Drawing.Point(288, 3);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(80, 27);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.applyButton.AutoSize = true;
            this.applyButton.Location = new System.Drawing.Point(455, 3);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(70, 27);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.AutoSize = true;
            this.cancelButton.Location = new System.Drawing.Point(374, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 27);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsGroupBox.AutoSize = true;
            this.settingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settingsGroupBox.Controls.Add(this.tableLayoutPanel4);
            this.settingsGroupBox.Location = new System.Drawing.Point(5, 39);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(524, 236);
            this.settingsGroupBox.TabIndex = 1;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel4.Controls.Add(this.authenticationLabel, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.authenticationLevelComboBox, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.clipboardCheckBox, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.usbCheckBox, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.printersCheckBox, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.fullscreenCheckBox, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.connectionBarCheckBox, 1, 5);
            this.tableLayoutPanel4.Controls.Add(this.pinConnectionBarCheckBox, 1, 6);
            this.tableLayoutPanel4.Controls.Add(this.widthTextBox, 1, 7);
            this.tableLayoutPanel4.Controls.Add(this.heightTextBox, 1, 8);
            this.tableLayoutPanel4.Controls.Add(this.widthLabel, 0, 7);
            this.tableLayoutPanel4.Controls.Add(this.heightLabel, 0, 8);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 9;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(518, 217);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // authenticationLabel
            // 
            this.authenticationLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.authenticationLabel.AutoSize = true;
            this.authenticationLabel.Location = new System.Drawing.Point(3, 7);
            this.authenticationLabel.Name = "authenticationLabel";
            this.authenticationLabel.Size = new System.Drawing.Size(104, 13);
            this.authenticationLabel.TabIndex = 35;
            this.authenticationLabel.Text = "RDP Authentication:";
            // 
            // authenticationLevelComboBox
            // 
            this.authenticationLevelComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.authenticationLevelComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authenticationLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.authenticationLevelComboBox.FormattingEnabled = true;
            this.authenticationLevelComboBox.Location = new System.Drawing.Point(210, 3);
            this.authenticationLevelComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.authenticationLevelComboBox.Name = "authenticationLevelComboBox";
            this.authenticationLevelComboBox.Size = new System.Drawing.Size(291, 21);
            this.authenticationLevelComboBox.TabIndex = 5;
            this.authenticationLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.authenticationLevelComboBox_SelectedIndexChanged);
            // 
            // clipboardCheckBox
            // 
            this.clipboardCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clipboardCheckBox.AutoSize = true;
            this.clipboardCheckBox.Location = new System.Drawing.Point(210, 30);
            this.clipboardCheckBox.Name = "clipboardCheckBox";
            this.clipboardCheckBox.Size = new System.Drawing.Size(113, 17);
            this.clipboardCheckBox.TabIndex = 6;
            this.clipboardCheckBox.Text = "Redirect Clipboard";
            this.clipboardCheckBox.UseVisualStyleBackColor = true;
            this.clipboardCheckBox.CheckedChanged += new System.EventHandler(this.clipboardCheckBox_CheckedChanged);
            // 
            // usbCheckBox
            // 
            this.usbCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.usbCheckBox.AutoSize = true;
            this.usbCheckBox.Location = new System.Drawing.Point(210, 53);
            this.usbCheckBox.Name = "usbCheckBox";
            this.usbCheckBox.Size = new System.Drawing.Size(108, 17);
            this.usbCheckBox.TabIndex = 7;
            this.usbCheckBox.Text = "Redirect Devices";
            this.usbCheckBox.UseVisualStyleBackColor = true;
            this.usbCheckBox.CheckedChanged += new System.EventHandler(this.usbCheckBox_CheckedChanged);
            // 
            // printersCheckBox
            // 
            this.printersCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.printersCheckBox.AutoSize = true;
            this.printersCheckBox.Location = new System.Drawing.Point(210, 76);
            this.printersCheckBox.Name = "printersCheckBox";
            this.printersCheckBox.Size = new System.Drawing.Size(104, 17);
            this.printersCheckBox.TabIndex = 8;
            this.printersCheckBox.Text = "Redirect Printers";
            this.printersCheckBox.UseVisualStyleBackColor = true;
            this.printersCheckBox.CheckedChanged += new System.EventHandler(this.printersCheckBox_CheckedChanged);
            // 
            // fullscreenCheckBox
            // 
            this.fullscreenCheckBox.AutoSize = true;
            this.fullscreenCheckBox.Location = new System.Drawing.Point(210, 99);
            this.fullscreenCheckBox.Name = "fullscreenCheckBox";
            this.fullscreenCheckBox.Size = new System.Drawing.Size(74, 17);
            this.fullscreenCheckBox.TabIndex = 9;
            this.fullscreenCheckBox.Text = "Fullscreen";
            this.fullscreenCheckBox.UseVisualStyleBackColor = true;
            this.fullscreenCheckBox.CheckedChanged += new System.EventHandler(this.fullscreenCheckBox_CheckedChanged);
            // 
            // connectionBarCheckBox
            // 
            this.connectionBarCheckBox.AutoSize = true;
            this.connectionBarCheckBox.Location = new System.Drawing.Point(210, 122);
            this.connectionBarCheckBox.Name = "connectionBarCheckBox";
            this.connectionBarCheckBox.Size = new System.Drawing.Size(98, 17);
            this.connectionBarCheckBox.TabIndex = 10;
            this.connectionBarCheckBox.Text = "Connection bar";
            this.connectionBarCheckBox.UseVisualStyleBackColor = true;
            this.connectionBarCheckBox.CheckedChanged += new System.EventHandler(this.connectionBarCheckBox_CheckedChanged);
            // 
            // pinConnectionBarCheckBox
            // 
            this.pinConnectionBarCheckBox.AutoSize = true;
            this.pinConnectionBarCheckBox.Location = new System.Drawing.Point(210, 145);
            this.pinConnectionBarCheckBox.Name = "pinConnectionBarCheckBox";
            this.pinConnectionBarCheckBox.Size = new System.Drawing.Size(115, 17);
            this.pinConnectionBarCheckBox.TabIndex = 11;
            this.pinConnectionBarCheckBox.Text = "Pin connection bar";
            this.pinConnectionBarCheckBox.UseVisualStyleBackColor = true;
            this.pinConnectionBarCheckBox.CheckedChanged += new System.EventHandler(this.pinConnectionBarCheckBox_CheckedChanged);
            // 
            // widthTextBox
            // 
            this.widthTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.widthTextBox.Location = new System.Drawing.Point(210, 168);
            this.widthTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(291, 20);
            this.widthTextBox.TabIndex = 12;
            this.widthTextBox.TextChanged += new System.EventHandler(this.widthTextBox_TextChanged);
            // 
            // heightTextBox
            // 
            this.heightTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.heightTextBox.Location = new System.Drawing.Point(210, 194);
            this.heightTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 17, 3);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(291, 20);
            this.heightTextBox.TabIndex = 13;
            this.heightTextBox.TextChanged += new System.EventHandler(this.heightTextBox_TextChanged);
            // 
            // widthLabel
            // 
            this.widthLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(3, 171);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(38, 13);
            this.widthLabel.TabIndex = 42;
            this.widthLabel.Text = "Width:";
            // 
            // heightLabel
            // 
            this.heightLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(3, 197);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(38, 13);
            this.heightLabel.TabIndex = 43;
            this.heightLabel.Text = "Height";
            // 
            // RemoteDesktopSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 339);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteDesktopSettingsForm";
            this.Text = "Remote Desktop";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.ComboBox serversComboBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label authenticationLabel;
        private System.Windows.Forms.ComboBox authenticationLevelComboBox;
        private System.Windows.Forms.CheckBox clipboardCheckBox;
        private System.Windows.Forms.CheckBox usbCheckBox;
        private System.Windows.Forms.CheckBox printersCheckBox;
        private System.Windows.Forms.CheckBox fullscreenCheckBox;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.CheckBox connectionBarCheckBox;
        private System.Windows.Forms.CheckBox pinConnectionBarCheckBox;
    }
}