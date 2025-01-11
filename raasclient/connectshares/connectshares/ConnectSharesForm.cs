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
﻿using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.ConnectShares
{
    public class ConnectSharesForm : Form
    {
        private static FileSystemWatcher xmlServersWatcher;
        private static Dictionary<String, SharesManager> sharesManagers = new Dictionary<string, SharesManager>();
        private const int OFFSCREEN = 100000;

        public ConnectSharesForm()
        {
            SetFormProperties();
            InitializeComponent();
            Hide();
            WatchServersConfig();
            UpdateSharesManagers();
        }

        private static void UpdateSharesManagers()
        {
            Dictionary<String, ServerSettings> serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
            UpdateSharesManagersFromConfig(ref sharesManagers);
        }

        private static void WatchServersConfig()
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
            WindowState = FormWindowState.Minimized;
        }

        private static void UpdateSharesManagersFromConfig(ref Dictionary<String, SharesManager> sharesManagers)
        {
            // Update server managers with current settings
            Dictionary<String, ServerSettings> serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
            foreach (String serverName in serverSettings.Keys.ToArray())
            {
                if (serverSettings[serverName].ServerEnabled)
                {
                    if (sharesManagers.Keys.Contains(serverName))
                        sharesManagers[serverName].ServerSettings = serverSettings[serverName];
                    else
                        sharesManagers[serverName] = new SharesManager(serverSettings[serverName]);
                }
            }

            // Remove server managers for servers which do not occur in settings
            foreach (String serverName in sharesManagers.Keys.ToArray())
                if (!serverSettings.Keys.Contains(serverName) || !serverSettings[serverName].ServerEnabled)
                {
                    sharesManagers[serverName].Dispose();
                    sharesManagers.Remove(serverName);
                }
        }

        private static void FileSystemWatcher_OnChangeXmlServers(object sender, FileSystemEventArgs e)
        {
            try
            {
                Dictionary<String, ServerSettings> serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
                UpdateSharesManagersFromConfig(ref sharesManagers);
            }
            catch { }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectSharesForm));
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Connect Shares Form";
            this.ResumeLayout(false);
        }
    }
}
