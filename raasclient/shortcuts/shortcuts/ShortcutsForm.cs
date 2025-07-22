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
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Models;
using System.Drawing;
using System.Threading;

namespace Elbitin.Applications.RAAS.RAASClient.Shortcuts
{
    public class ShortcutsForm : Form
    {
        private bool uninstall = false;
        private bool remove = false;
        private bool update = false;
        private Dictionary<String, ShortcutsManager> shortcutsManagers = new Dictionary<string, ShortcutsManager>();
        private static FileSystemWatcher serversXmlWatcher;
        private static object managersLock = new object();
        static Dictionary<String, ServerSettings> serverSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
        static System.Threading.Mutex serversChange = new Mutex();

        public ShortcutsForm(bool uninstall, bool remove, bool update)
        {
            this.uninstall = uninstall;
            this.remove = remove;
            this.update = update;
            RAASClientPathHelper.CreateMissingAppDataRAASClientDirectories();
            SetFormProperties();
            InitializeShortcuts();

            // Exit if uninstall, remove or update is requested, no more work to be done
            if (uninstall || remove || update)
            {
                Load += (s, e) => Close();
            }
        }

        private void SetFormProperties()
        {
            Opacity = 0;
            Visible = false;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Size = new Size(0, 0);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams Params = base.CreateParams;
                Params.ExStyle |= (int)Win32Helper.WindowStyles.WS_EX_TOOLWINDOW;
                return Params;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (uninstall)
                this.Close();
        }

        private static void UpdateShortcutsManagersFromConfig(ref Dictionary<String, ShortcutsManager> shortcutsManagers, bool uninstall = false, bool remove = false)
        {
            serversChange.WaitOne();
            try
            {
                // Fetch current server settings
                Dictionary<String, ServerSettings> newServerSettings = ServerSettingsHelper.GetServerSettingsFromConfig();

                // Return if server settings unchanged
                if (serverSettings.SequenceEqual(newServerSettings))
                    return;

                // Update server settings
                serverSettings = newServerSettings;

                // Remove all server managers
                foreach (String serverName in shortcutsManagers.Keys.ToArray())
                    if (!serverSettings.Keys.Contains(serverName))
                    {
                        shortcutsManagers[serverName].Dispose();
                        shortcutsManagers.Remove(serverName);
                    }
                shortcutsManagers.Clear();

                // Repopulate server managers
                foreach (String serverName in serverSettings.Keys.ToArray())
                {
                    shortcutsManagers[serverName] = new ShortcutsManager(serverSettings[serverName], false);
                }
            }
            catch (CouldNotLoadServerSettingsException)
            {
                (new System.Threading.Thread(() => {
                    MessageBox.Show(Properties.Resources.Settings_LoadFailedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK);
                })).Start();
                return;
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
            finally
            {
                serversChange.ReleaseMutex();
            }
        }

        private static void RemoveServerShortcuts(Dictionary<string, ShortcutsManager> shortcutsManagers, string serverName)
        {
            shortcutsManagers[serverName].Dispose();
            shortcutsManagers.Remove(serverName);
            SetShortcutsRemoved(serverName);
        }

        private static void SetShortcutsRemoved(string serverName)
        {
            try
            {
                // TODO: lock file to prevent it from being edited by RAAS Server Configuration (will probably never be needed)
                Dictionary<String, ServerSettings> currentServerSettings = ServerSettingsHelper.GetServerSettingsFromConfig();
                currentServerSettings[serverName].ShortcutsRemoved = true;
                currentServerSettings[serverName].SaveServerSettings();
            }
            catch { }
        }

        private void FileSystemWatcher_OnChangeServersXml(object sender, FileSystemEventArgs e)
        {
            UpdateShortcutsManagersFromConfig(ref shortcutsManagers, uninstall, remove);
        }

        private void FileSystemWatcher_OnErrorServersXml(object source, ErrorEventArgs e)
        {
            serversXmlWatcher.EnableRaisingEvents = false;
            serversXmlWatcher.EnableRaisingEvents = true;
        }

        private void InitializeShortcuts()
        {
            // Prepare directories
            String raasClientPath = RAASClientPathHelper.GetAppDataRAASClientPath();

            // Watch for server settings changes
            serversXmlWatcher = new FileSystemWatcher(raasClientPath, "servers.xml");
            serversXmlWatcher.IncludeSubdirectories = false;
            serversXmlWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            serversXmlWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnChangeServersXml);
            serversXmlWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnChangeServersXml);
            serversXmlWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnChangeServersXml);
            serversXmlWatcher.Error += new ErrorEventHandler(FileSystemWatcher_OnErrorServersXml);
            serversXmlWatcher.EnableRaisingEvents = true;

            UpdateShortcutsManagersFromConfig(ref shortcutsManagers, uninstall, remove);
        }
    }
}
