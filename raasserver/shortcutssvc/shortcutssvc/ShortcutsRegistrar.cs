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
﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Elbitin.Applications.RAAS.RAASServer.Models;
using Serilog;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    class ShortcutsRegistrar
    {
        public String AppNamesXmlPath { get; set; }
        public String IconsDirPath { get; set; }
        public String RootDirPath { get; set; }
        public String Section { get; set; }
        public String ShortcutsXmlFilePath { get; set; }
        public ShortcutType Type { get; set; }
        private AppList appNames = null;
        private const int SHORTCUTS_XML_RETRY_COUNT = 200;
        private const int SHORTCUTS_XML_RETRY_INTERVAL_MS = 200;
        private const int APPNAMES_XML_RETRY_COUNT = 200;
        private const int APPNAMES_XML_RETRY_INTERVAL_MS = 200;
        private Shortcuts shortcuts = null;
        private const String RUN_PATH = @"\System Tools\Run.lnk";
        private const String IMMERSIVE_CONTROL_PANEL_PATH = @"\Immersive Control Panel.lnk";
        private List<String> startMenuPathFilter = new List<string>() { RUN_PATH.ToLowerInvariant(), IMMERSIVE_CONTROL_PANEL_PATH.ToLowerInvariant() };

        public void RegisterShortcutsPath(ref Shortcuts shortcuts, String targetPath)
        {
            this.shortcuts = shortcuts;
            RegisterShortcutsPath(targetPath);
            shortcuts = this.shortcuts;
        }

        public void RegisterShortcutsPath(String targetPath)
        {
            ParseAppNames();
            int count = 0;
            bool exception = false;
            do
            {
                try
                {
                    if (this.shortcuts == null)
                    {
                        if (File.Exists(ShortcutsXmlFilePath))
                        {
                            shortcuts = Shortcuts.DeserializeXmlFile(ShortcutsXmlFilePath);
                        }
                        else
                        {
                            shortcuts = new Shortcuts();
                        }
                    }
                    if (System.IO.File.Exists(targetPath) || targetPath.EndsWith(".lnk"))
                    {
                        UpdateShortcut(targetPath);
                    }
                    else
                    {
                        UpdateShortcutsDir(targetPath);
                    }
                    shortcuts.SerializeXmlFile(ShortcutsXmlFilePath);
                    exception = false;
                }
                catch
                {
                    Thread.Sleep(SHORTCUTS_XML_RETRY_INTERVAL_MS);
                    count++;
                    exception = true; 
                }
            } while (exception && count < SHORTCUTS_XML_RETRY_COUNT);
        }

        public bool UpdateShortcut(String file)
        {
            // Validate file
            if (!file.ToLowerInvariant().StartsWith(RootDirPath.ToLowerInvariant()))
                return false;

            // Get user name
            String userName = ProfileHelper.GetUserFromProfilePath(IconsDirPath);


            // Remove file from shortcuts
            FileInfo fi;
            fi = new System.IO.FileInfo(file);
            RemoveExistingFile(fi);
            String subPath = fi.FullName.Substring(RootDirPath.Length);
            if (Type == ShortcutType.StartMenu && startMenuPathFilter.Contains(subPath.ToLowerInvariant()))
                return false;

            // Return if file does not exist
            if (!System.IO.File.Exists(file))
                return true;

            // Return if file is not shortcut
            if (!fi.Name.EndsWith(".lnk"))
                return true;

            // Disable file system redirection
            IntPtr oldValue = IntPtr.Zero;
            Win32Helper.Wow64DisableWow64FsRedirection(ref oldValue);

            // Create icon if available and allowed
            Bitmap shortcutIcon;
            Bitmap existingAssociationsIconPath = AssociationsIconHelper.GetExistingAssociationIcon(fi.FullName, RAASServerPathHelper.GetUserRAASServerIconsAssociationsPath(IconsDirPath));
            if (existingAssociationsIconPath != null)
                shortcutIcon = new Bitmap(existingAssociationsIconPath);
            else
                shortcutIcon = IconHelper.GetShortcutIconBitmap(file, userName);


            // Set file system redirection to its original value
            Win32Helper.Wow64RevertWow64FsRedirection(oldValue);

            // Get localized name
            String localizedName = "";
            try
            {
                localizedName = appNames.Shortcut.Where(x => (x.shortcutPath.ToLowerInvariant() == file.ToLowerInvariant())).FirstOrDefault().DisplayName + ".lnk";
            }catch
            {
                localizedName = Path.GetFileName(fi.FullName);
            }

            // Register shortcuts for the file
            SingleShortcutRegistrar singleShortcutRegistrar = new SingleShortcutRegistrar();
            singleShortcutRegistrar.IconsPath = IconsDirPath;
            singleShortcutRegistrar.ShortcutPath = fi.FullName;
            singleShortcutRegistrar.ShortcutDefaultName = Path.GetFileName(fi.FullName);
            singleShortcutRegistrar.ShortcutIcon = shortcutIcon;
            singleShortcutRegistrar.ShortcutLocalizedName = localizedName;
            singleShortcutRegistrar.ShortcutSection = Section;
            singleShortcutRegistrar.Type = Type;
            singleShortcutRegistrar.RegisterShortcut(ref shortcuts);

            return true;
        }

        public void ParseAppNames()
        {
            int count = 0;
            bool exception = false;
            do
            {
                try
                {
                    appNames = AppListHelper.ParseAppNames(AppNamesXmlPath);
                    exception = false;
                }
                catch
                {
                    Thread.Sleep(APPNAMES_XML_RETRY_INTERVAL_MS);
                    count++;
                    exception = true;
                }
            } while (exception && count < APPNAMES_XML_RETRY_COUNT);
            if (exception)
                throw new Exception("App names could not be parsed");
        }

        public bool UpdateShortcutsDir(String targetDir)
        {
            // Validate the directory
            if (!targetDir.ToLowerInvariant().StartsWith(RootDirPath.ToLowerInvariant()))
                return false;

            // Get shortcuts dirs
            List<ShortcutsDir> dirs;
            if (Type == ShortcutType.Desktop)
                dirs = shortcuts.Desktop.Dir;
            else if (Type == ShortcutType.StartMenu)
                dirs = shortcuts.StartMenu.Dir;
            else
                return false;

            // Remove removed directories from shortcuts
            if (!Directory.Exists(targetDir))
            {
                foreach (ShortcutsDir dir in dirs.ToArray())
                {
                    if (dir.Path.Equals(targetDir.Substring(RootDirPath.Length)))
                    {
                        dirs.Remove(dir);
                        break;
                    }
                }
                return true;
            }

            // Get contents of directory
            String[] currentFiles = System.IO.Directory.GetFiles(targetDir);
            String[] currentDirs = System.IO.Directory.GetDirectories(targetDir);

            // Determine if directory already exist in shortcuts
            bool dirExist = false;
            foreach (ShortcutsDir dir in dirs.ToArray())
                if (dir.Path == targetDir.Substring(RootDirPath.Length))
                    dirExist = true;

            // Add directory to list of directories if it does not exist
            if (!dirExist && targetDir.Substring(RootDirPath.Length).Length > 0)
            {
                ShortcutsDir newDir = new ShortcutsDir();
                newDir.Path = targetDir.Substring(RootDirPath.Length);
                newDir.Section = Section;
                dirs.Add(newDir);
            }

            // Register each containing file
            foreach (String file in currentFiles)
            {
                UpdateShortcut(file);
            }

            // Register files and folders for all contained directories
            foreach (String dir in currentDirs)
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);
                UpdateShortcutsDir(dir);
            }

            return true;
        }

        private void RemoveExistingFile(System.IO.FileInfo fi)
        {
            List<ShortcutsFile> files;
            if (Type == ShortcutType.Desktop)
                files = shortcuts.Desktop.File;
            else if (Type == ShortcutType.StartMenu)
                files = shortcuts.StartMenu.File;
            else
                return;
            foreach (ShortcutsFile file in files.ToArray())
            {
                if (file.Shortcut.ToLowerInvariant() == fi.FullName.ToLowerInvariant())
                {
                    files.Remove(file);
                }
            }
        }
    }
}
