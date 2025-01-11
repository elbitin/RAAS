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
using Elbitin.Applications.RAAS.RAASClient.Models;
using MSTSCLib;
using AxMSTSCLib;
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
    public partial class RemoteDesktopForm : Form
    {
        private const int TS_PERF_ENABLE_ENHANCED_GRAPHICS = 0x00000010;
        private const int TS_PERF_ENABLE_FONT_SMOOTHING = 0x00000080;
        private const int WINDOW_WIDTH = 1366;
        private const int WINDOW_HEIGHT = 768;
        private const int REMOTE_DESKTOP_ICON_GROUP = 13400;
        private ProgressForm progressForm;
        private ServerSettings serverSettings;
        private RemoteDesktopSettings remoteDesktopSettings;
        private bool currentlyFullscreen = false;
        private bool connectedOnce = false;

        public RemoteDesktopForm(ServerSettings serverSettings, RemoteDesktopSettings remoteDesktopSettings)
        {
            this.serverSettings = serverSettings;
            this.remoteDesktopSettings = remoteDesktopSettings;
            InitializeComponent();
            InitializeLocalizedStrings();
            progressForm = new ProgressForm();
            if (remoteDesktopSettings.Fullscreen)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                this.Opacity = 0;
                progressForm.Show();
            }
            else
                this.Show();
            InitializeRdpClient();
            rdpClient.Connect();
            FormClosed += RemoteDesktopForm_FormClosed;
            Resize += RemoteDesktopForm_Resize;
            Activated += RemoteDesktopForm_Activated;
        }

        private void RemoteDesktopForm_Activated(object sender, EventArgs e)
        {
            if (currentlyFullscreen)
            {
                rdpClient.FullScreen = false;
                rdpClient.FullScreen = true;
            }
        }

        private void SetInvisible()
        {
            this.Opacity = 0;
        }

        private void SetVisible()
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.Opacity = 1;
        }

        private void RemoteDesktopForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Screen currentScreen = Screen.FromControl(this);
                if (currentScreen.DeviceName == Screen.PrimaryScreen.DeviceName)
                {
                    rdpClient.FullScreen = true;
                }
            }
        }

        private void RemoteDesktopForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


        private void InitializeLocalizedStrings()
        {
            this.Text = String.Format(Properties.Resources.RemoteDesktopForm_RemoteDesktopFormTitle, serverSettings.ServerName + " (" + serverSettings.Alias + ")");
        }

        private void InitializeRdpClient()
        {
            // Register event handlers
            rdpClient.OnLogonError += RdpClient_OnLogonError;
            rdpClient.OnFatalError += RdpClient_OnFatalError;
            rdpClient.OnDisconnected += RdpClient_OnDisconnected;
            rdpClient.OnEnterFullScreenMode += RdpClient_OnEnterFullScreenMode;
            rdpClient.OnLeaveFullScreenMode += RdpClient_OnLeaveFullScreenMode;
            rdpClient.OnConnecting += RdpClient_OnConnecting;
            rdpClient.OnConnected += RdpClient_OnConnected;
            InitializeLifetimeService();
            InitLayout();
            rdpClient.CreateControl();
            rdpClient.CreateGraphics();

            // Set settings for RDP client
            IMsRdpClientNonScriptable5 UseMultiSetting = (IMsRdpClientNonScriptable5)(rdpClient.GetOcx());
            UseMultiSetting.UseMultimon = remoteDesktopSettings.AllMonitors;
            rdpClient.AdvancedSettings.ContainerHandledFullScreen = 0;
            rdpClient.Server = serverSettings.ServerName;
            rdpClient.UserName = serverSettings.UserName;
            rdpClient.Domain = serverSettings.Domain;
            rdpClient.AdvancedSettings7.PerformanceFlags = TS_PERF_ENABLE_ENHANCED_GRAPHICS | TS_PERF_ENABLE_FONT_SMOOTHING;
            rdpClient.AdvancedSettings7.ClearTextPassword = serverSettings.Password;
            rdpClient.AdvancedSettings7.RedirectDrives = false;
            rdpClient.AdvancedSettings7.RedirectPrinters = remoteDesktopSettings.RedirectPrinters;
            rdpClient.AdvancedSettings7.RedirectSmartCards = false;
            rdpClient.AdvancedSettings7.RedirectClipboard = remoteDesktopSettings.RedirectClipboard;
            rdpClient.AdvancedSettings7.RedirectDevices = remoteDesktopSettings.RedirectUsb;
            rdpClient.AdvancedSettings7.RedirectDrives = false;
            rdpClient.AdvancedSettings7.MinutesToIdleTimeout = 0;
            rdpClient.AdvancedSettings7.overallConnectionTimeout = 180;
            rdpClient.AdvancedSettings7.AuthenticationLevel = (uint)remoteDesktopSettings.AuthenticationLevel;
            rdpClient.AdvancedSettings7.DisableRdpdr = 0;
            rdpClient.AdvancedSettings7.DisplayConnectionBar = remoteDesktopSettings.ConnectionBar;
            rdpClient.AdvancedSettings7.PinConnectionBar = remoteDesktopSettings.PinConnectionBar;
            rdpClient.AdvancedSettings7.PublicMode = false;
            rdpClient.AdvancedSettings7.SmartSizing = true;
            rdpClient.AdvancedSettings7.EnableAutoReconnect = true;
            rdpClient.AdvancedSettings7.MaxReconnectAttempts = 3;
            rdpClient.RemoteProgram.RemoteProgramMode = false;
            rdpClient.AdvancedSettings7.keepAliveInterval = 1000;
            rdpClient.DesktopHeight = remoteDesktopSettings.Height;
            rdpClient.DesktopWidth = remoteDesktopSettings.Width;
            rdpClient.FullScreen = remoteDesktopSettings.Fullscreen;
            currentlyFullscreen = remoteDesktopSettings.Fullscreen;
            rdpClient.Width = WINDOW_WIDTH;
            rdpClient.Height = WINDOW_HEIGHT;
            rdpClient.Visible = true;
            rdpClient.Dock = DockStyle.Fill;
        }

        private void RdpClient_OnConnected(object sender, EventArgs e)
        {
            connectedOnce = true;
        }

        private void RdpClient_OnConnecting(object sender, EventArgs e)
        {
            progressForm.SetProgress50Percent();
        }

        private void RdpClient_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RemoteDesktop_FatalErrorMessage, ServerIdString()), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private void RdpClient_OnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
        {
            //MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RemoteDesktop_LoginFailedMessage, ServerIdString()), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private String ServerIdString()
        {
            return serverSettings.ServerName + " (" + serverSettings.Alias + ")";
        }

        private void RdpClient_OnEnterFullScreenMode(object sender, EventArgs e)
        {
            SetInvisible();
            currentlyFullscreen = true;
            progressForm.Hide();
        }

        private void RdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            if (!connectedOnce)
                MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.RemoteDesktop_ConnectionFailedMessage, ServerIdString()), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private void RdpClient_OnLeaveFullScreenMode(object sender, EventArgs e)
        {
            currentlyFullscreen = false;
            this.WindowState = FormWindowState.Normal;
            SetVisible();
        }
    }
}
