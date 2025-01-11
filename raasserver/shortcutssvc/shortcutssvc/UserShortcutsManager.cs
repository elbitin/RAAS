/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Server.
 *
 * RAAS Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Server. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Microsoft.Extensions.FileSystemGlobbing;
using Serilog;
using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using static System.Collections.Specialized.BitVector32;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    public class UserShortcutsManager
    {
        private String shortcutsXMLPath { get; set; }
        private String appNamesXMLPath { get; set; }
        private String userDesktopPath { get; set; }
        private String userStartMenuPath { get; set; }
        private String iconsPath { get; set; }
        private String associationsPath { get; set; }
        private String associationsXMLPath { get; set; }
        private String publicDesktopPath { get; set; }
        private String commonStartMenuPath { get; set; }
        private UWPAppsRegistrar uwpAppsRegistrar;
        private ShortcutsRegistrar userDesktopRegistrar;
        private ShortcutsRegistrar publicDesktopRegistrar;
        private ShortcutsRegistrar userStartMenuRegistrar;
        private ShortcutsRegistrar commonStartMenuRegistrar;
        private FileSystemWatcher desktopWatcher;
        private FileSystemWatcher startMenuWatcher;
        private FileSystemWatcher publicDesktopwatcher;
        private FileSystemWatcher commonStartMenuWatcher;
        private FileSystemWatcher appNamesWatcher;
        private FileSystemWatcher appsFolderWatcher;
        static System.Threading.Mutex shortcutsChange = new Mutex();

        public UserShortcutsManager(String userPath)
        {
            CreateMissingUserDirs(userPath);
            InitializePaths(userPath);
            InitializeWatchers();
            CreateShortcutsRegistrars();
        }

        private void InitializePaths(string userPath)
        {
            userDesktopPath = RAASServerPathHelper.GetUserDesktopPath(userPath);
            userStartMenuPath = RAASServerPathHelper.GetUserStartMenuPath(userPath);
            iconsPath = RAASServerPathHelper.GetUserRAASServerIconsPath(userPath);
            associationsXMLPath = RAASServerPathHelper.GetUserRAASServerAssociationsXMLPath(userPath);
            associationsPath = RAASServerPathHelper.GetUserRAASServerAssociationsPath(userPath);
            shortcutsXMLPath = RAASServerPathHelper.GetUserRAASServerShortcutsXMLPath(userPath);
            appNamesXMLPath = RAASServerPathHelper.GetUserRAASServerAppNamesXMLPath(userPath);
            commonStartMenuPath = RAASServerPathHelper.GetCommonStartMenuPath();
            publicDesktopPath = RAASServerPathHelper.GetPublicDesktopPath();
        }


        private void CreateShortcutsRegistrars()
        {
            SetUserDesktopRegistrar();
            SetPublicDesktopRegistrar();
            SetUserStartMenuRegistrar();
            SetCommonStartMenuRegistrar();
            SetUWPAppsRegistrar();
        }

        private void SetUWPAppsRegistrar()
        {
            uwpAppsRegistrar = new UWPAppsRegistrar();
            uwpAppsRegistrar.IconsDirPath = iconsPath;
            uwpAppsRegistrar.AssociationsXmlPath = associationsXMLPath;
            uwpAppsRegistrar.AssociationsDirPath = associationsPath;
            uwpAppsRegistrar.ShortcutsXmlFilePath = shortcutsXMLPath;
            uwpAppsRegistrar.AppNamesXmlPath = appNamesXMLPath;
            uwpAppsRegistrar.Section = "public";
            uwpAppsRegistrar.Type = ShortcutType.UWP;
            uwpAppsRegistrar.ParseAppNames();
        }

        private void SetCommonStartMenuRegistrar()
        {
            commonStartMenuRegistrar = new ShortcutsRegistrar();
            commonStartMenuRegistrar.AppNamesXmlPath = appNamesXMLPath;
            commonStartMenuRegistrar.IconsDirPath = iconsPath;
            commonStartMenuRegistrar.ShortcutsXmlFilePath = shortcutsXMLPath;
            commonStartMenuRegistrar.RootDirPath = commonStartMenuPath;
            commonStartMenuRegistrar.Section = "public";
            commonStartMenuRegistrar.Type = ShortcutType.StartMenu;
            commonStartMenuRegistrar.ParseAppNames();
        }

        private void SetUserStartMenuRegistrar()
        {
            userStartMenuRegistrar = new ShortcutsRegistrar();
            userStartMenuRegistrar.AppNamesXmlPath = appNamesXMLPath;
            userStartMenuRegistrar.IconsDirPath = iconsPath;
            userStartMenuRegistrar.ShortcutsXmlFilePath = shortcutsXMLPath;
            userStartMenuRegistrar.RootDirPath = userStartMenuPath;
            userStartMenuRegistrar.Section = "user";
            userStartMenuRegistrar.Type = ShortcutType.StartMenu;
            userStartMenuRegistrar.ParseAppNames();
        }

        private void SetPublicDesktopRegistrar()
        {
            publicDesktopRegistrar = new ShortcutsRegistrar();
            publicDesktopRegistrar.AppNamesXmlPath = appNamesXMLPath;
            publicDesktopRegistrar.IconsDirPath = iconsPath;
            publicDesktopRegistrar.ShortcutsXmlFilePath = shortcutsXMLPath;
            publicDesktopRegistrar.RootDirPath = publicDesktopPath;
            publicDesktopRegistrar.Section = "public";
            publicDesktopRegistrar.Type = ShortcutType.Desktop;
            publicDesktopRegistrar.ParseAppNames();
        }

        private void SetUserDesktopRegistrar()
        {
            userDesktopRegistrar = new ShortcutsRegistrar();
            userDesktopRegistrar.AppNamesXmlPath = appNamesXMLPath;
            userDesktopRegistrar.IconsDirPath = iconsPath;
            userDesktopRegistrar.ShortcutsXmlFilePath = shortcutsXMLPath;
            userDesktopRegistrar.RootDirPath = userDesktopPath;
            userDesktopRegistrar.Section = "user";
            userDesktopRegistrar.Type = ShortcutType.Desktop;
            userDesktopRegistrar.ParseAppNames();
        }

        private void InitializeWatchers()
        {
            // Set file system watchers for shortcut directories to monitor
            try 
            { 
                SetDesktopWatcher();
            } catch { }
            try
            {
                SetStartMenuWatcher();
            } catch { }
            try
            {
                SetPublicDesktopWatcher();
            } catch { }
            try
            {
                SetCommonStartMenuWatcher();
            }
            catch { }
            try
            {
                SetAppNamesXmlWatcher();
            } catch { }
        }


        private void SetAppNamesXmlWatcher()
        {
            appNamesWatcher = new FileSystemWatcher(Path.GetDirectoryName(appNamesXMLPath));
            appNamesWatcher.Filter = Path.GetFileName(appNamesXMLPath);
            appNamesWatcher.IncludeSubdirectories = false;
            appNamesWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            appNamesWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnAppNamesChange);
            appNamesWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnAppNamesChange);
            appNamesWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnAppNamesChange);
            appNamesWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcher_OnAppNamesChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                shortcutsChange.WaitOne();
                uwpAppsRegistrar.ParseAppNames();
                userDesktopRegistrar.ParseAppNames();
                userStartMenuRegistrar.ParseAppNames();
                publicDesktopRegistrar.ParseAppNames();
                commonStartMenuRegistrar.ParseAppNames();
                RegisterAllUserShortcuts();
            }
            catch { }
            finally
            {
                shortcutsChange.ReleaseMutex();
            }
        }

        private void SetCommonStartMenuWatcher()
        {
            commonStartMenuWatcher = new FileSystemWatcher(commonStartMenuPath);
            commonStartMenuWatcher.IncludeSubdirectories = true;
            commonStartMenuWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            commonStartMenuWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            commonStartMenuWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            commonStartMenuWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            commonStartMenuWatcher.Renamed += new RenamedEventHandler(FileSystemWatcher_OnRenamedShortcuts);
            commonStartMenuWatcher.EnableRaisingEvents = true;
        }

        private void SetAppsFolderWatcher()
        {
            String appsFolderPath = Environment.ExpandEnvironmentVariables("%PROGRAMFILES%\\WindowsApps");
            appsFolderWatcher = new FileSystemWatcher(appsFolderPath);
            appsFolderWatcher.IncludeSubdirectories = true;
            appsFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            appsFolderWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnAppsFolderChange);
            appsFolderWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnAppsFolderChange);
            publicDesktopwatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnAppsFolderChange);
            appsFolderWatcher.EnableRaisingEvents = true;
        }

        private void SetPublicDesktopWatcher()
        {
            publicDesktopwatcher = new FileSystemWatcher(publicDesktopPath);
            publicDesktopwatcher.IncludeSubdirectories = true;
            publicDesktopwatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            publicDesktopwatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            publicDesktopwatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            publicDesktopwatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            publicDesktopwatcher.Renamed += new RenamedEventHandler(FileSystemWatcher_OnRenamedShortcuts);
            publicDesktopwatcher.EnableRaisingEvents = true;
        }

        private void SetStartMenuWatcher()
        {
            startMenuWatcher = new FileSystemWatcher(userStartMenuPath);
            startMenuWatcher.IncludeSubdirectories = true;
            startMenuWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            startMenuWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            startMenuWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            startMenuWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            startMenuWatcher.Renamed += new RenamedEventHandler(FileSystemWatcher_OnRenamedShortcuts);
            startMenuWatcher.EnableRaisingEvents = true;
        }

        private void SetDesktopWatcher()
        {
            desktopWatcher = new FileSystemWatcher(userDesktopPath);
            desktopWatcher.IncludeSubdirectories = true;
            desktopWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            desktopWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            desktopWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            desktopWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsChange);
            desktopWatcher.Renamed += new RenamedEventHandler(FileSystemWatcher_OnRenamedShortcuts);
            desktopWatcher.EnableRaisingEvents = true;
        }

        private void CreateMissingUserDirs(String userPath)
        {
            RAASServerPathHelper.CreateMissingUserDirs(userPath);
            String userRAASServerPath = RAASServerPathHelper.GetUserRAASServerPath(userPath);
            String userRAASServerShortcutsPath;
            String userRAASServerIconsPath;
            String userRAASServerAssociationsPath;
            try
            {
                if (!Directory.Exists(userRAASServerPath))
                    System.IO.Directory.CreateDirectory(userRAASServerPath);
                userRAASServerShortcutsPath = RAASServerPathHelper.GetUserRAASServerShortcutsPath(userPath);
                if (!Directory.Exists(userRAASServerShortcutsPath))
                    System.IO.Directory.CreateDirectory(userRAASServerShortcutsPath);
                userRAASServerIconsPath = RAASServerPathHelper.GetUserRAASServerIconsPath(userPath);
                if (!Directory.Exists(userRAASServerIconsPath))
                    System.IO.Directory.CreateDirectory(userRAASServerIconsPath);
                userRAASServerAssociationsPath = RAASServerPathHelper.GetUserRAASServerAssociationsPath(userPath);
                if (!Directory.Exists(userRAASServerAssociationsPath))
                    System.IO.Directory.CreateDirectory(userRAASServerAssociationsPath);
            }
            catch { }
        }

        public void RegisterAllUserShortcuts()
        {
            shortcutsChange.WaitOne();
            try
            {
                Shortcuts shortcuts;
                if (File.Exists(shortcutsXMLPath))
                    shortcuts = Common.Models.Shortcuts.DeserializeXmlFile(shortcutsXMLPath);
                else
                    shortcuts = new Shortcuts();
                uwpAppsRegistrar.RegisterUWPApps(ref shortcuts);
                userDesktopRegistrar.RegisterShortcutsPath(ref shortcuts, userDesktopPath);
                userStartMenuRegistrar.RegisterShortcutsPath(ref shortcuts, userStartMenuPath);
                publicDesktopRegistrar.RegisterShortcutsPath(ref shortcuts, publicDesktopPath);
                commonStartMenuRegistrar.RegisterShortcutsPath(ref shortcuts, commonStartMenuPath);
                shortcuts.FilterDuplicates();
                shortcuts.SerializeXmlFile(shortcutsXMLPath);
            }
            catch { }
            shortcutsChange.ReleaseMutex();
        }

        private void FileSystemWatcher_OnShortcutsChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                new Thread(delegate () {
                    OnShortcutsChange(sender, e);
                }).Start();
            }
            catch { }
        }

        private void OnProgramInstalled()
        {
            shortcutsChange.WaitOne();

            // Register change in shortcuts
            try
            {
                RegisterAllUserShortcuts();
            }
            catch { }

            shortcutsChange.ReleaseMutex();
        }

        private void OnShortcutsChange(object sender, FileSystemEventArgs e)
        {
            shortcutsChange.WaitOne();

            // Register change in shortcuts
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    FileHelper.WaitTimeWhileFileLocked(e.FullPath);
                }
                if (e.FullPath.ToLowerInvariant().StartsWith(publicDesktopPath.ToLowerInvariant()))
                    publicDesktopRegistrar.RegisterShortcutsPath(e.FullPath);
                if (e.FullPath.ToLowerInvariant().StartsWith(commonStartMenuPath.ToLowerInvariant()))
                    commonStartMenuRegistrar.RegisterShortcutsPath(e.FullPath);
                if (e.FullPath.ToLowerInvariant().StartsWith(userDesktopPath.ToLowerInvariant()))
                    userDesktopRegistrar.RegisterShortcutsPath(e.FullPath);
                if (e.FullPath.ToLowerInvariant().StartsWith(userStartMenuPath.ToLowerInvariant()))
                    userStartMenuRegistrar.RegisterShortcutsPath(e.FullPath);
            }
            catch { }

            shortcutsChange.ReleaseMutex();
        }

        private void FileSystemWatcher_OnAppsFolderChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                new Thread(delegate () {
                    OnAppsFolderChange(sender, e);
                }).Start();
            }
            catch { }
        }

        private void FileSystemWatcher_OnRenamedShortcuts(object sender, RenamedEventArgs e)
        {
            try
            {
                new Thread(delegate () {
                    OnRenamedShortcuts(sender, e);
                }).Start();
            }
            catch { }
        }
        private void OnAppsFolderChange(object sender, FileSystemEventArgs e)
        {
            bool hasMutex = false;

            if (e.FullPath.ToLowerInvariant().Split("\\").Count() >= 4)
            {
                using (Mutex mutex = new Mutex(true, e.FullPath.ToLowerInvariant().Split("\\")[3], out hasMutex))
                {
                    if (hasMutex)
                    {
                        // Register all UWP apps
                        uwpAppsRegistrar.RegisterUWPApps();
                    }
                }
            }
        }

        private void OnRenamedShortcuts(object sender, RenamedEventArgs e)
        {
            shortcutsChange.WaitOne();

            // Register new name in shortcuts
            try
            {
                if (e.FullPath.ToLowerInvariant().StartsWith(publicDesktopPath.ToLowerInvariant()))
                {
                    publicDesktopRegistrar.RegisterShortcutsPath(e.FullPath);
                    publicDesktopRegistrar.RegisterShortcutsPath(e.OldFullPath);
                }
                if (e.FullPath.ToLowerInvariant().StartsWith(commonStartMenuPath.ToLowerInvariant()))
                {
                    commonStartMenuRegistrar.RegisterShortcutsPath(e.FullPath);
                    commonStartMenuRegistrar.RegisterShortcutsPath(e.OldFullPath);
                }
                if (e.FullPath.ToLowerInvariant().StartsWith(userDesktopPath.ToLowerInvariant()))
                {
                    userDesktopRegistrar.RegisterShortcutsPath(e.FullPath);
                    userDesktopRegistrar.RegisterShortcutsPath(e.OldFullPath);
                }
                if (e.FullPath.ToLowerInvariant().StartsWith(userStartMenuPath.ToLowerInvariant()))
                {
                    userStartMenuRegistrar.RegisterShortcutsPath(e.FullPath);
                    userStartMenuRegistrar.RegisterShortcutsPath(e.OldFullPath);
                }
            }
            catch { }

            shortcutsChange.ReleaseMutex();
        }
    }
}

