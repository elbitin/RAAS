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
﻿using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Elbitin.Applications.RAAS.RAASClient.Shortcuts.RAASServerServiceRef;
using System.Security.Cryptography;
using System.Timers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Elbitin.Applications.RAAS.RAASClient.Shortcuts
{
    class ShortcutsManager : IDisposable
    {
        public static ShortcutIcon RemoteAppIcon { get; set; }
        public static ShortcutIcon DefaultIcon { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public RAASServerServiceClient RaasServerServiceClient { get; set; } = null;
        public bool ServerSubscribed { get; set; }
        public String ServerShortcutsXml { get; set; }
        private System.Timers.Timer updateTimer = new System.Timers.Timer();
        private bool removeShortcuts;
        private bool running = true;
        private bool clientShortcutsChanged = false;
        private Object shortcutsXmlLock = new Object();
        private const int UPDATETIMER_INTERVAL_MS = 2000;
        private bool deleteAll;
        private bool desktopRoot;
        private bool desktopShortcutsUpdateRequired = false;
        private bool startMenuShortcutsUpdateRequired = false;
        private String userDesktopDirPath;
        private String userStartMenuDirPath;
        private String userServerStartMenuDirPath;
        private String raasIconsDirPath;
        private String raasShortcutsDirPath;
        private String raasClientShortcutsXMLFilePath;
        private String raasServerShortcutsXMLFilePath;
        private Common.Models.Shortcuts clientShortcuts;
        private Common.Models.Shortcuts serverShortcuts = new Common.Models.Shortcuts();
        private SessionSwitchEventHandler sessionSwitchEventHandler;
        private delegate void refreshShortcutsDelegate();
        private refreshShortcutsDelegate refreshShortcutsDel;
        private static readonly object shortcutsChangeLock = new System.Object();
        static System.Threading.Mutex shortcutsChange = new Mutex();
        private const int MAX_SERVER_ENTITIES = 5000;
        private const int MAX_SERVER_ICONS = 5000;
        private const int MAX_ICON_SIZE = 500000;
        private const int MAX_SHORTCUTS_XML_SIZE = 5000000;
        private const int PING_TIMEOUT = 2000;

        public ShortcutsManager(ServerSettings serverSettings, bool removeShortcuts = false)
        {
            ServerSettings = serverSettings;
            UpdateFlags(serverSettings);
            UpdatePaths(serverSettings);
            ReadClientShortcuts();
            ReadServerShortcuts();
            try
            {
                RemoteAppIcon = ShortcutIcon.CreateRemoteAppIcon();
                DefaultIcon = ShortcutIcon.CreateDefaultIcon();
            }
            catch { }
            AddRAASServerServiceClient();
            this.removeShortcuts = removeShortcuts;
            RAASClientPathHelper.CreateMissingServerAppDataRAASDirectories(ServerSettings.ServerName);
            InitializeUpdateTimer();
            try
            {
                UpdateShortcuts(false);
            }
            catch { }
            RegisterSessionSwitchEventHandler();
        }

        public static bool Contact(String serverName)
        {
            try
            {
                // Ping server
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(serverName, PING_TIMEOUT);
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void RegisterSessionSwitchEventHandler()
        {
            sessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += sessionSwitchEventHandler;
        }

        private void InitializeUpdateTimer()
        {
            updateTimer.Interval = UPDATETIMER_INTERVAL_MS;
            updateTimer.AutoReset = false;
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Enabled = true;
            updateTimer.Start();
        }

        public void Dispose()
        {
            running = false;
            updateTimer.Stop();
            if (RaasServerServiceClient != null)
                try
                {
                    RaasServerServiceClient.Abort();
                }catch { }
            RaasServerServiceClient = null;
            SystemEvents.SessionSwitch -= sessionSwitchEventHandler;
        }

        private void UpdateFlags(ServerSettings serverSettings)
        {
            desktopShortcutsUpdateRequired = false;
            startMenuShortcutsUpdateRequired = false;
            if (!serverSettings.CreateShortcuts || !serverSettings.ServerEnabled || ServerSettings.RemoveShortcuts || removeShortcuts)
                deleteAll = true;
            else
                deleteAll = false;
            if (serverSettings.DesktopRoot && serverSettings.ServerEnabled)
                desktopRoot = true;
            else
                desktopRoot = false;
        }

        private void UpdatePaths(ServerSettings serverSettings)
        {
            raasClientShortcutsXMLFilePath = RAASClientPathHelper.GetClientShortcutsFilePath(serverSettings.ServerName);
            raasServerShortcutsXMLFilePath = RAASClientPathHelper.GetServerShortcutsFilePath(serverSettings.ServerName);
            raasIconsDirPath = RAASClientPathHelper.GetServerIconsDirectoryPath(serverSettings.ServerName);
            raasShortcutsDirPath = RAASClientPathHelper.GetServerShortcutsDirectory(serverSettings.ServerName);
            userDesktopDirPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            userStartMenuDirPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            userServerStartMenuDirPath = Path.Combine(userStartMenuDirPath, "[" + serverSettings.Alias + "]");
        }

        private bool ReadClientShortcuts()
        {
            // Load user shortcuts XML file or create new
            if (System.IO.File.Exists(raasClientShortcutsXMLFilePath))
            {
                try
                {
                    clientShortcuts = Common.Models.Shortcuts.DeserializeXmlFileWithRetries(raasClientShortcutsXMLFilePath);
                }
                catch
                {
                    return false;
                }
            }
            else
                clientShortcuts = new Common.Models.Shortcuts();

            return true;
        }

        private bool ReadServerShortcuts()
        {
            // Load server shortcuts XML file or create new
            if (System.IO.File.Exists(raasServerShortcutsXMLFilePath))
            {
                try
                {
                    ServerShortcutsXml = File.ReadAllText(raasServerShortcutsXMLFilePath);
                    serverShortcuts = Common.Models.Shortcuts.DeserializeXml(ServerShortcutsXml);
                }
                catch
                {
                    return false;
                }
            }
            else
                clientShortcuts = new Common.Models.Shortcuts();

            return true;
        }

        private void CreateMissingServerStartMenuDirs()
        {
            if (!Directory.Exists(userServerStartMenuDirPath) && !deleteAll)
                System.IO.Directory.CreateDirectory(userServerStartMenuDirPath);
        }

        public void RemoveShortcuts()
        {
            try
            {
                removeShortcuts = true;
                UpdateShortcuts(false);
            }
            catch { }
        }

        public void UpdateShortcuts()
        {
            UpdateShortcuts(true);
        }

        public void UpdateShortcuts(bool fetchNewIcons)
        {
            // Only one instance running at a time
            shortcutsChange.WaitOne();

            try
            {
                ServerSettings = ServerSettingsHelper.GetServerSettingsFromConfig()[ServerSettings.ServerName];
                ReadClientShortcuts();
                clientShortcutsChanged = false;
                UpdateFlags(ServerSettings);
                UpdatePaths(ServerSettings);
                CreateMissingServerStartMenuDirs();
                String currentDesktopDir = userDesktopDirPath;
                if (deleteAll && !System.IO.File.Exists(raasClientShortcutsXMLFilePath))
                    return;
                else if (ServerSettings.ShortcutsRemoved)
                {
                    ServerSettings.ShortcutsRemoved = false;
                    ServerSettings.SaveServerSettings();
                }
                if (!removeShortcuts && !deleteAll)
                {
                    // Delete old desktop root if changed
                    try { DeleteOldDesktopRootOnChange(currentDesktopDir, userStartMenuDirPath); } catch { }

                    if (!desktopRoot)
                    {
                        // Delete files which dont belong on desktop root
                        try { DeleteDesktopRootShortcuts(currentDesktopDir); } catch { }

                        // Delete directories which dont belong on desktop root
                        try { DeleteDesktopRootShortcutDirectories(currentDesktopDir); } catch { }
                    }
                    else
                    {
                        // Delete the old server folder on desktop root if it exists
                        try { DeleteOldDesktopServerFolder(currentDesktopDir); } catch { }

                        // Delete the new server folder on desktop root if it exists
                        try { DeleteDesktopServerFolder(currentDesktopDir); } catch { }

                        // Remove all instances of client path which exist
                        try { RemoveAllClientPaths(currentDesktopDir); } catch { }
                    }
                }
                if (!deleteAll)
                {
                    try { UpdateAlias(ref currentDesktopDir); } catch { }
                }

                // Delete shortcut files and directories which are no longer referenced
                try { DeleteNotReferencedShortcuts(ShortcutType.Desktop); } catch { }
                try { DeleteNotReferencedShortcutDirectories(currentDesktopDir, ShortcutType.Desktop); } catch { }
                try { DeleteNotReferencedShortcuts(ShortcutType.StartMenu); } catch { }
                try { DeleteNotReferencedShortcutDirectories(userServerStartMenuDirPath, ShortcutType.StartMenu); } catch { }
                try { DeleteNotReferencedShortcuts(ShortcutType.UWP); } catch { }
                try { DeleteNotReferencedShortcutDirectories(userServerStartMenuDirPath, ShortcutType.UWP); } catch { }

                if (deleteAll)
                {
                    try { File.Delete(raasClientShortcutsXMLFilePath); } catch { }
                    try { File.Delete(raasClientShortcutsXMLFilePath); } catch { }
                    try { CleanServerStartMenuDirectories(raasClientShortcutsXMLFilePath, userServerStartMenuDirPath); } catch { }
                    try { CleanServerDesktopDirectories(raasClientShortcutsXMLFilePath, userServerStartMenuDirPath); } catch { }
                    return;
                }
                if (!ServerSettings.CreateStartMenuShortcuts && !ServerSettings.CreateUWPApplicationShortcuts)
                    try { CleanServerStartMenuDirectories(raasClientShortcutsXMLFilePath, userServerStartMenuDirPath); } catch { }
                if (!ServerSettings.CreateDesktopShortcuts)
                    try { CleanServerDesktopDirectories(raasClientShortcutsXMLFilePath, userServerStartMenuDirPath); } catch { }

                // Create shortcut files and directories which are referenced
                if (ServerSettings.CreateDesktopShortcuts)
                {
                    try { CreateShortcutDirectories(currentDesktopDir, ShortcutType.Desktop); } catch { }
                    try { CreateShortcutFiles(currentDesktopDir, ShortcutType.Desktop, fetchNewIcons); } catch { }
                }
                if (ServerSettings.CreateStartMenuShortcuts)
                {
                    try { CreateShortcutDirectories(userServerStartMenuDirPath, ShortcutType.StartMenu); } catch { }
                    try { CreateShortcutFiles(userServerStartMenuDirPath, ShortcutType.StartMenu, fetchNewIcons); } catch { }
                }
                if (ServerSettings.CreateUWPApplicationShortcuts)
                {
                    try { CreateShortcutDirectories(userServerStartMenuDirPath, ShortcutType.UWP); } catch { }
                    try { CreateShortcutFiles(userServerStartMenuDirPath, ShortcutType.UWP, fetchNewIcons); } catch { }
                }

                // Remove unused icons
                try { RemoveNotReferencedIcons(); } catch { }
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (!removeShortcuts && clientShortcutsChanged)
                        // Save XML document
                        try
                        {
                            clientShortcuts.SerializeXmlFileWithRetries(raasClientShortcutsXMLFilePath);
                        }catch { }
                }
                catch { }
                try
                {
                    // Update desktop if necessary
                    if (startMenuShortcutsUpdateRequired)
                    {
                        Win32Helper.SHChangeNotify(0x08000000, (int)Win32Helper.SHChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
                        startMenuShortcutsUpdateRequired = false;
                    }
                    if (desktopShortcutsUpdateRequired)
                    {
                        Win32Helper.SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
                        desktopShortcutsUpdateRequired = false;
                    }
                }
                catch { }
                shortcutsChange.ReleaseMutex();
            }
        }

        private void RemoveNotReferencedIcons()
        {
            List<String> serverIcons = new List<string>();
            List<String> clientIcons = new List<string>();
            List<ShortcutsFile> serverFiles = new List<ShortcutsFile>();
            serverFiles.AddRange(serverShortcuts.Desktop.File);
            serverFiles.AddRange(serverShortcuts.StartMenu.File);
            serverFiles.AddRange(serverShortcuts.UWP.File);
            foreach (ShortcutsFile serverFile in serverFiles.ToArray())
            {
                String iconName = IconHelper.GetIconFileName(serverFile.MD5, Path.GetFileName(serverFile.Path));
                String clientIconFile = Path.Combine(raasIconsDirPath, iconName);
                serverIcons.Add(clientIconFile.ToLowerInvariant());
            }
            foreach (String fileIcon in Directory.GetFiles(raasIconsDirPath))
                clientIcons.Add(fileIcon.ToLowerInvariant());
            List<String> iconsToRemove = new List<string>();
            iconsToRemove = clientIcons.Except(serverIcons).ToList();
            foreach (String removeIcon in iconsToRemove)
                File.Delete(removeIcon);
        }

        private void RemoveAllClientPaths(string raasShortcutsDesktopPath)
        {
            foreach (ShortcutsFile clientFile in clientShortcuts.Desktop.File.ToArray())
            {
                if (clientFile.ClientPath.StartsWith(Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]")))
                {
                    clientShortcuts.Desktop.File.Remove(clientFile);
                    clientShortcutsChanged = true;
                }
            }
        }

        private void DeleteOldDesktopRootOnChange(string raasShortcutsDesktopPath, string clientStartMenuPath)
        {
            if (clientShortcuts.NodeRoot != null && clientShortcuts.NodeRoot.Alias != null)
            {
                String storedServerDisplayName = clientShortcuts.NodeRoot.Alias;
                if (storedServerDisplayName != ServerSettings.Alias)
                {
                    if (Directory.Exists(Path.Combine(raasShortcutsDesktopPath, "[" + storedServerDisplayName + "]")))
                    {
                        try
                        {
                            Directory.Delete(Path.Combine(raasShortcutsDesktopPath, "[" + storedServerDisplayName + "]"), true);
                        }
                        catch { }
                        desktopShortcutsUpdateRequired = true;
                        startMenuShortcutsUpdateRequired = true;
                    }
                    if (Directory.Exists(Path.Combine(clientStartMenuPath, "[" + storedServerDisplayName + "]")))
                    {
                        try
                        {
                            Directory.Delete(Path.Combine(clientStartMenuPath, "[" + storedServerDisplayName + "]"), true);
                        }
                        catch { }
                        desktopShortcutsUpdateRequired = true;
                        startMenuShortcutsUpdateRequired = true;
                    }
                    clientShortcutsChanged = true;
                }
            }
        }

        private void DeleteDesktopServerFolder(string raasShortcutsDesktopPath)
        {
            if (Directory.Exists(Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]")))
            {
                try
                {
                    Directory.Delete(Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]"), true);
                }
                catch { }
                desktopShortcutsUpdateRequired = true;
            }
        }

        private void DeleteOldDesktopServerFolder(string raasShortcutsDesktopPath)
        {
            if (clientShortcuts.NodeRoot != null && clientShortcuts.NodeRoot.Alias != null)
            {
                String storedServerDisplayName = clientShortcuts.NodeRoot.Alias;
                if (Directory.Exists(Path.Combine(raasShortcutsDesktopPath, "[" + storedServerDisplayName + "]")))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(raasShortcutsDesktopPath, "[" + storedServerDisplayName + "]"), true);
                    }
                    catch { }
                    desktopShortcutsUpdateRequired = true;
                }
            }
        }

        private bool GetServerShortcutsChangesFromServer()
        {
            String shortcutsXml = RaasServerServiceClient.GetShortcutsXml();
            if (shortcutsXml.Length > MAX_SHORTCUTS_XML_SIZE)
                return false;
            Common.Models.Shortcuts newServerShortcuts = Common.Models.Shortcuts.DeserializeXml(shortcutsXml);
            if (ValidateEntityCount(newServerShortcuts))
            {
                serverShortcuts = newServerShortcuts;
                if (shortcutsXml != ServerShortcutsXml)
                {
                    System.IO.File.WriteAllText(raasServerShortcutsXMLFilePath, shortcutsXml);
                    ServerShortcutsXml = shortcutsXml;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                (new System.Threading.Thread(() => {
                    MessageBox.Show(String.Format(Properties.Resources.Entities_ExceededMessage, ServerSettings.Alias), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                })).Start();
                return false;
            }
        }

        private bool ValidateEntityCount(Common.Models.Shortcuts shortcuts)
        {
            if (shortcuts.Desktop.File.Count() > MAX_SERVER_ENTITIES || shortcuts.Desktop.Dir.Count() > MAX_SERVER_ENTITIES || shortcuts.StartMenu.File.Count() > MAX_SERVER_ENTITIES || shortcuts.StartMenu.Dir.Count() > MAX_SERVER_ENTITIES)
                return false;
            else
                return true;
        }

        private bool ValidateIconCount()
        {
            int count = Directory.GetFiles(raasIconsDirPath, "*", SearchOption.AllDirectories).Length;
            if (count >= MAX_SERVER_ICONS)
                return false;
            else
                return true;
        }

        private void DeleteDesktopRootShortcutDirectories(string raasShortcutsDesktopPath)
        {
            List<String> deleteRootDirectoryDesktopPaths = new List<String>();
            foreach (ShortcutsDir clientDir in clientShortcuts.Desktop.Dir.ToArray())
            {
                bool deleteNode = false;
                if (!clientDir.ClientPath.ToLowerInvariant().StartsWith(Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]").ToLowerInvariant()))
                    deleteNode = true;
                if (deleteNode)
                {
                    clientShortcuts.Desktop.Dir.Remove(clientDir);
                    clientShortcutsChanged = true;
                    deleteRootDirectoryDesktopPaths.Add(clientDir.ClientPath);
                }
            }
            System.Linq.IOrderedEnumerable<String> sortedDeletedDirectoryRootDesktopPaths =
                from s
                in deleteRootDirectoryDesktopPaths
                orderby s.Length descending
                select s;
            foreach (String directoryPath in sortedDeletedDirectoryRootDesktopPaths)
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        String[] dirs = System.IO.Directory.GetDirectories(directoryPath);
                        String[] files = System.IO.Directory.GetFiles(directoryPath);
                        if (dirs.Length == 0 && files.Length == 0)
                        {
                            try
                            {
                                Directory.Delete(directoryPath);
                            }
                            catch { }
                            desktopShortcutsUpdateRequired = true;
                        }
                    }
                }
                catch { }
            }
        }

        private void DeleteDesktopRootShortcuts(string raasShortcutsDesktopPath)
        {
            foreach (ShortcutsFile clientFile in clientShortcuts.Desktop.File.ToArray())
            {
                String fileName = clientFile.Path;
                String filePath = clientFile.ClientPath;
                StringBuilder hash = new StringBuilder();
                bool deleteNode = false;
                if (!filePath.StartsWith(Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]")))
                    deleteNode = true;
                if (deleteNode)
                {
                    clientShortcuts.Desktop.File.Remove(clientFile);
                    clientShortcutsChanged = true;
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        desktopShortcutsUpdateRequired = true;
                    }
                }
            }
        }

        private void UpdateAlias(ref string raasShortcutsDesktopPath)
        {
            if (!desktopRoot)
            {
                raasShortcutsDesktopPath = Path.Combine(raasShortcutsDesktopPath, "[" + ServerSettings.Alias + "]");
                if (!Directory.Exists(raasShortcutsDesktopPath))
                {
                    System.IO.Directory.CreateDirectory(raasShortcutsDesktopPath);
                    desktopShortcutsUpdateRequired = true;
                }
            }
            if (clientShortcuts.NodeRoot.Alias != ServerSettings.Alias)
                clientShortcutsChanged = true;
            clientShortcuts.NodeRoot.Alias = ServerSettings.Alias;
        }

        private void DeleteNotReferencedShortcuts(ShortcutType shortcutType)
        {
            List<ShortcutsFile> serverFiles;
            List<ShortcutsFile> localFiles;
            if (shortcutType == ShortcutType.Desktop)
            {
                serverFiles = serverShortcuts.Desktop.File;
                localFiles = clientShortcuts.Desktop.File;
            }
            else if (shortcutType == ShortcutType.StartMenu)
            {
                serverFiles = serverShortcuts.StartMenu.File;
                localFiles = clientShortcuts.StartMenu.File;
            }
            else
            {
                serverFiles = serverShortcuts.UWP.File;
                localFiles = clientShortcuts.UWP.File;
            }
            foreach (ShortcutsFile clientFile in localFiles.ToArray())
            {
                String fileName = clientFile.Path;
                String filePath = clientFile.ClientPath;
                bool stillExist = false;
                StringBuilder hash = new StringBuilder();
                foreach (ShortcutsFile serverFile in serverFiles.ToArray())
                    if (clientFile.Path == serverFile.Path)
                    {
                        String serverAlias;
                        if (ServerSettings.ShortcutsAppendAlias)
                            serverAlias = ServerSettings.Alias;
                        else
                            serverAlias = "";
                        if (serverFile.MD5 == clientFile.MD5 && clientFile.Alias == serverAlias && clientFile.Section == serverFile.Section)
                        {
                            if ((ServerSettings.LocalizeShortcuts && clientFile.IsLocalized && clientFile.LocalizedName.ToLowerInvariant() == serverFile.LocalizedName.ToLowerInvariant())
                                || (!ServerSettings.LocalizeShortcuts && !clientFile.IsLocalized))
                                stillExist = true;
                        }
                    }
                if (stillExist && !deleteAll && !(!ServerSettings.CreateDesktopShortcuts && shortcutType == ShortcutType.Desktop) && !(!ServerSettings.CreateStartMenuShortcuts && shortcutType == ShortcutType.StartMenu) && !(!ServerSettings.CreateUWPApplicationShortcuts && shortcutType == ShortcutType.UWP))
                    continue;
                if (System.IO.File.Exists(filePath))
                    using (MD5 md5 = MD5.Create())
                    {
                        using (Stream stream = System.IO.File.OpenRead(filePath))
                        {
                            foreach (byte b in md5.ComputeHash(stream))
                                hash.Append(b.ToString("X2"));
                        }
                    }
                localFiles.Remove(clientFile);
                clientShortcutsChanged = true;
                if (clientFile.ClientMD5 == hash.ToString())
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        if (shortcutType == ShortcutType.Desktop)
                            desktopShortcutsUpdateRequired = true;
                        else
                            startMenuShortcutsUpdateRequired = true;
                    }
                }
            }
        }

        private void DeleteNotReferencedShortcutDirectories(string raasShortcutsPath, ShortcutType shortcutType)
        {
            List<ShortcutsDir> serverDirs;
            List<ShortcutsDir> localDirs;
            if (shortcutType == ShortcutType.Desktop)
            {
                serverDirs = serverShortcuts.Desktop.Dir;
                localDirs = clientShortcuts.Desktop.Dir;
            }
            else if (shortcutType == ShortcutType.StartMenu)
            {
                serverDirs = serverShortcuts.StartMenu.Dir;
                localDirs = clientShortcuts.StartMenu.Dir;
            }
            else if (shortcutType == ShortcutType.UWP)
            {
                serverDirs = serverShortcuts.UWP.Dir;
                localDirs = clientShortcuts.UWP.Dir;
            }
            else
                return;
            List<String> deleteDirectoryDesktopPaths = new List<String>();
            foreach (ShortcutsDir clientDir in localDirs.ToArray())
            {
                bool stillExist = false;
                foreach (ShortcutsDir serverDir in serverDirs)
                    if (clientDir.Path == serverDir.Path && clientDir.Alias == ServerSettings.Alias)
                    {
                        stillExist = true;
                        break;
                    }
                if (stillExist && !deleteAll && !(!ServerSettings.CreateDesktopShortcuts && shortcutType == ShortcutType.Desktop) && !(!ServerSettings.CreateStartMenuShortcuts && shortcutType == ShortcutType.StartMenu) && !(!ServerSettings.CreateUWPApplicationShortcuts && shortcutType == ShortcutType.UWP))
                    continue;
                localDirs.Remove(clientDir);
                clientShortcutsChanged = true;
                deleteDirectoryDesktopPaths.Add(clientDir.ClientPath);
            }
            System.Linq.IOrderedEnumerable<String> sortedDeletedDirectoryDesktopPaths =
                from s in deleteDirectoryDesktopPaths
                orderby s.Length descending
                select s;
            foreach (String directoryPath in sortedDeletedDirectoryDesktopPaths)
            {
                if (Directory.Exists(directoryPath))
                {
                    String[] dirs = System.IO.Directory.GetDirectories(directoryPath);
                    String[] files = System.IO.Directory.GetFiles(directoryPath);
                    if (dirs.Length == 0 && files.Length == 0)
                    {
                        try
                        {
                            Directory.Delete(directoryPath);
                        }
                        catch { }
                        if (shortcutType == ShortcutType.Desktop)
                            desktopShortcutsUpdateRequired = true;
                        else
                            startMenuShortcutsUpdateRequired = true;
                    }
                }
            }
        }

        private void CleanServerStartMenuDirectories(string userShortcutsPath, string raasShortcutsStartMenuPath)
        {
            if (Directory.Exists(raasShortcutsStartMenuPath))
                try
                {
                    Directory.Delete(raasShortcutsStartMenuPath, true);
                    startMenuShortcutsUpdateRequired = true;
                }
                catch { }
        }

        private void CleanServerDesktopDirectories(string userShortcutsPath, string raasShortcutsStartMenuPath)
        {
            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "[" + ServerSettings.Alias + "]")))
                try
                {
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "[" + ServerSettings.Alias + "]"), true);
                }
                catch { }
        }

        private void CreateShortcutFiles(string shortcutsPath, ShortcutType shortcutType, bool fetchNew)
        {
            List<ShortcutsFile> serverFiles;
            List<ShortcutsFile> localFiles;
            if (shortcutType == ShortcutType.Desktop)
            {
                serverFiles = serverShortcuts.Desktop.File;
                localFiles = clientShortcuts.Desktop.File;
            }
            else if(shortcutType == ShortcutType.StartMenu)
            {
                serverFiles = serverShortcuts.StartMenu.File;
                localFiles = clientShortcuts.StartMenu.File;
            }
            else
            {
                serverFiles = serverShortcuts.UWP.File;
                localFiles = clientShortcuts.UWP.File;
            }
            foreach (ShortcutsFile serverFile in serverFiles.ToArray())
            {
                bool alreadyExist = false;
                String serverAlias;
                if (ServerSettings.ShortcutsAppendAlias)
                    serverAlias = ServerSettings.Alias;
                else
                    serverAlias = "";
                foreach (ShortcutsFile clientFile in localFiles.ToArray())
                {
                    if (clientFile.Alias == serverAlias &&
                        clientFile.Path == serverFile.Path &&
                        clientFile.Section == serverFile.Section &&
                        clientFile.MD5 == serverFile.MD5)
                    {
                        if ((ServerSettings.LocalizeShortcuts && clientFile.IsLocalized && clientFile.LocalizedName == serverFile.LocalizedName) 
                            || (!ServerSettings.LocalizeShortcuts && !clientFile.IsLocalized))
                            alreadyExist = true;
                    }
                }
                if (alreadyExist)
                    continue;

                // Do not support sub directory shortcuts on desktop
                if (shortcutType == ShortcutType.Desktop && serverFile.Path.Split('\\').Count() >= 3)
                    continue;

                // Prioritize user shortcuts
                if (serverFile.Section == "public")
                {
                    Boolean userShortcutExists = false;
                    foreach (ShortcutsFile innerServerFile in serverFiles.ToArray())
                        if (innerServerFile.Path == serverFile.Path && innerServerFile.Section == "user")
                            userShortcutExists = true;
                    if (userShortcutExists)
                        continue;
                }

                // Remove prior occurances of shortcut
                foreach (ShortcutsFile clientFile in localFiles.ToArray())
                    if (clientFile.Path == serverFile.Path)
                    {
                        localFiles.Remove(clientFile);
                        clientShortcutsChanged = true;
                    }

                String originalFileSubPath = serverFile.Path.Substring(1);
                String hash = serverFile.MD5;
                String fileName;
                if (!ServerSettings.LocalizeShortcuts)
                    fileName = Path.GetFileName(originalFileSubPath);
                else
                    fileName = serverFile.LocalizedName;
                String iconName = IconHelper.GetIconFileName(hash, Path.GetFileName(originalFileSubPath));
                String clientIconFile = Path.Combine(raasIconsDirPath, iconName);
                if (!File.Exists(clientIconFile))
                {
                    if (hash != IconHelper.GetDefaultIconHash())
                    {
                        if (ValidateIconCount())
                        {
                            byte[] iconBytes;
                            try
                            {
                                if (fetchNew && RaasServerServiceClient.State == CommunicationState.Opened)
                                    iconBytes = RaasServerServiceClient.GetIcon(iconName);
                                else
                                    continue;
                            }
                            catch
                            {
                                continue;
                            }
                            if (iconBytes.Length > MAX_ICON_SIZE)
                                continue;
                            ShortcutIcon targetIcon = new ShortcutIcon(iconBytes);
                            targetIcon.AddIconOverlay(RemoteAppIcon);
                            targetIcon.SaveAsMultiSizeIcon(clientIconFile);
                        }
                        else
                            (new System.Threading.Thread(() => {
                                MessageBox.Show(String.Format(Properties.Resources.Icons_ExceededMessage, serverAlias), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            })).Start();
                    }
                    else
                        DefaultIcon.SaveAsIcon(clientIconFile);
                }
                String targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "openremote.exe");
                String shortcutPath;
                shortcutPath = System.IO.Path.Combine(Path.GetDirectoryName(Path.Combine(shortcutsPath, originalFileSubPath)), fileName);
                if (ServerSettings.ShortcutsAppendAlias)
                    shortcutPath = shortcutPath.Substring(0, shortcutPath.Length - 4) + " [" + ServerSettings.Alias + "].lnk";
                else
                    shortcutPath = shortcutPath.Substring(0, shortcutPath.Length - 4) + ".lnk";
                if (!Directory.Exists(Path.GetDirectoryName(shortcutPath)))
                    continue;
                if (System.IO.File.Exists(shortcutPath))
                {
                    String baseShortcutsPath = shortcutPath;
                    int i = 2;
                    do
                    {
                        shortcutPath = baseShortcutsPath.Substring(0, baseShortcutsPath.Length - 4) + " (" + i.ToString() + ").lnk";
                        i++;
                    } while (System.IO.File.Exists(shortcutPath));
                }
                String arguments = "\"" + serverFile.Shortcut.Replace("\"", "") + "\" -remote -server " + ServerSettings.ServerName + (serverFile.Arguments != null ? " -args \"" + serverFile.Arguments?.Replace("\"","") + "\"": "");
                LinkHelper.CreateLink(clientIconFile, shortcutPath, arguments);
                if (shortcutType == ShortcutType.Desktop)
                    desktopShortcutsUpdateRequired = true;
                else
                    startMenuShortcutsUpdateRequired = true;
                StringBuilder clientHash = new StringBuilder();
                if (System.IO.File.Exists(shortcutPath))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        using (var stream = System.IO.File.OpenRead(shortcutPath))
                        {
                            foreach (byte b in md5.ComputeHash(stream))
                                clientHash.Append(b.ToString("X2"));
                        }
                    }
                }
                ShortcutsFile newFile = new ShortcutsFile();
                newFile.Arguments = serverFile.Arguments;
                newFile.Path = serverFile.Path;
                newFile.MD5 = serverFile.MD5;
                newFile.Section = serverFile.Section;
                newFile.ClientPath = shortcutPath;
                newFile.ClientMD5 = clientHash.ToString();
                newFile.LocalizedName = serverFile.LocalizedName;
                newFile.IsLocalized = ServerSettings.LocalizeShortcuts;
                if (ServerSettings.ShortcutsAppendAlias)
                    newFile.Alias = ServerSettings.Alias;
                else
                    newFile.Alias = "";
                localFiles.Add(newFile);
                clientShortcutsChanged = true;
            }
        }

        private void CreateShortcutDirectories(string raasShortcutsPath, ShortcutType shortcutType)
        {
            List<ShortcutsDir> serverDirs;
            List<ShortcutsDir> localDirs;
            if (shortcutType == ShortcutType.Desktop)
            {
                serverDirs = serverShortcuts.Desktop.Dir;
                localDirs = clientShortcuts.Desktop.Dir;
            }
            else if (shortcutType == ShortcutType.StartMenu)
            {
                serverDirs = serverShortcuts.StartMenu.Dir;
                localDirs = clientShortcuts.StartMenu.Dir;
            }
            else
            {
                serverDirs = serverShortcuts.StartMenu.Dir;
                localDirs = clientShortcuts.StartMenu.Dir;
            }
            foreach (ShortcutsDir serverDir in serverDirs.ToArray())
            {
                bool alreadyExist = false;
                foreach (ShortcutsDir clientDir in localDirs.ToArray())
                    if (clientDir.Path == serverDir.Path)
                    {
                        alreadyExist = true;
                        break;
                    }
                if (alreadyExist)
                    continue;

                // Do not support sub directories on desktop
                if (shortcutType == ShortcutType.Desktop)
                    continue;

                String dirPath = serverDir.Path;
                String dir = raasShortcutsPath;
                int directoryLevel = 0;
                foreach (String folderName in dirPath.Split('\\'))
                {
                    if (folderName.Length > 0)
                    {
                        dir = Path.Combine(dir, folderName);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            if (shortcutType == ShortcutType.Desktop)
                                desktopShortcutsUpdateRequired = true;
                            else
                                startMenuShortcutsUpdateRequired = true;
                        }
                        directoryLevel++;
                    }
                }
                ShortcutsDir newDir = new ShortcutsDir();
                newDir.Path = dirPath;
                newDir.Alias = ServerSettings.Alias;
                newDir.ClientPath = dir;
                localDirs.Add(newDir);
                clientShortcutsChanged = true;
            }
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateTimer.Stop();
            if (running)
            {
                try
                {
                    if ((RaasServerServiceClient.State != CommunicationState.Opened && RaasServerServiceClient.State != CommunicationState.Opening))
                        ReconnectServer();
                    if (RaasServerServiceClient.State == CommunicationState.Opened)
                        SubscribeServer();
                }
                catch { }
                updateTimer.Start();
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                updateTimer.Stop();
            }
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                updateTimer.Start();
            }
        }

        private void SubscribeServer()
        {
            lock (shortcutsChangeLock)
            {
                try
                {
                    if (!ServerSubscribed)
                    {
                        // Subscribe and ensure that shortcuts have been collected afterwards
                        RaasServerServiceClient.Subscribe();
                        UpdateServer();
                        ServerSubscribed = true;
                    }
                }
                catch
                {
                    IndicateShortcutFailure();
                }
            }
        }

        public void ReconnectServer()
        {
            try
            {
                if (Contact(ServerSettings.ServerName))
                {
                    // Remove the old raas server service client
                    ServerSubscribed = false;
                    if (RaasServerServiceClient != null && RaasServerServiceClient.State == CommunicationState.Opened || RaasServerServiceClient.State == CommunicationState.Opening)
                        RaasServerServiceClient.Abort();

                    AddRAASServerServiceClient();

                    // Update shortcuts if needed
                    RaasServerServiceClient.Subscribe();
                    if (GetServerShortcutsChangesFromServer())
                        UpdateShortcuts();
                    ServerSubscribed = true;
                }
                else
                {
                    IndicateShortcutFailure();
                }
            }
            catch
            {
                IndicateShortcutFailure();
            }
        }

        private void IndicateShortcutFailure()
        {
            ServerShortcutsXml = null;
            ServerSubscribed = false;
        }

        private void UpdateServer()
        {
            // Create shortcuts for the specified server if enabled
            if (ServerSettings.ServerEnabled)
            {
                if (GetServerShortcutsChangesFromServer())
                    UpdateShortcuts();
            }
        }

        private void ShortcutsXmlChange()
        {
            refreshShortcutsDel = new refreshShortcutsDelegate(RefreshShortcuts);
            refreshShortcutsDel.BeginInvoke(null, null);
        }

        private void RefreshShortcuts()
        {
            lock (shortcutsChangeLock)
            {
                try
                {
                    ServerSubscribed = false;
                    UpdateServer();
                    ServerSubscribed = true;
                }
                catch
                {
                    IndicateShortcutFailure();
                }
            }
        }

        private void AddRAASServerServiceClient()
        {
            try
            {
                if (RaasServerServiceClient?.State == CommunicationState.Opened || RaasServerServiceClient?.State == CommunicationState.Opening)
                    return;
                else if (RaasServerServiceClient?.State == CommunicationState.Created)
                    RaasServerServiceClient?.Close();
                else if (RaasServerServiceClient?.State == CommunicationState.Faulted)
                    RaasServerServiceClient?.Abort();
            }
            catch { }

            // Indicate that server has not been subscribed to from the new raas server service client
            ServerSubscribed = false;

            // Create new raas server service client
            RAASServerServiceCallback clientServiceCallback = new RAASServerServiceCallback();
            clientServiceCallback.ShortcutsXmlChangeEvent += ShortcutsXmlChange;
            InstanceContext context = new InstanceContext(clientServiceCallback);
            RaasServerServiceClient = new RAASServerServiceClient(context);
            EndpointAddress endpoint = new EndpointAddress(new Uri(@"net.tcp://" + ServerSettings.ServerName + @":43000/RAASServerService"), new DnsEndpointIdentity("localhost"), new AddressHeaderCollection());
            RaasServerServiceClient.Endpoint.Address = endpoint;
            RaasServerServiceClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            RaasServerServiceClient.ClientCredentials.Windows.ClientCredential.UserName = ServerSettings.UserName;
            RaasServerServiceClient.ClientCredentials.Windows.ClientCredential.Password = ServerSettings.Password;
            RaasServerServiceClient.ClientCredentials.Windows.ClientCredential.Domain = ServerSettings.Domain;
            RaasServerServiceClient.ChannelFactory.Faulted += new EventHandler(RAASServerServiceClientOffline);
            RaasServerServiceClient.ChannelFactory.Closed += new EventHandler(RAASServerServiceClientOffline);
        }

        private void RAASServerServiceClientOffline(object sender, EventArgs e)
        {
            ServerSubscribed = false;
        }
    }
}
