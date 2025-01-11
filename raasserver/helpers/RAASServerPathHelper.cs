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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    class RAASServerPathHelper
    {
        private const String PROGRAM_SHORTCUTS_SUB_DIR = "Shortcuts";
        private const String USER_START_MENU_SUB_DIR = "AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs";
        private const String PROGRAM_LOGS_SUB_DIR = "Elbitin\\RAAS Server\\Logs";
        private const String USER_DESKTOP_SUB_DIR = "Desktop";
        private const String ICONS_SUB_DIR = "Icons";
        private const String ASSOCIATIONS_SUB_DIR = "Associations";
        private const String SHORTCUTS_SUB_DIR = "Shortcuts";
        private const String ASSOCITATIONS_SUB_DIR = "Associations";
        private const String RAAS_SERVER_SUB_DIR = "RAAS Server";
        private const String SHORTCUTS_XML_FILE_NAME = "shortcuts.xml";
        private const String SETTINGS_SHORTCUT_FILE_NAME = "Settings.lnk";
        private const String ASSOCIATIONS_FILE_NAME = "associations.xml";
        private const String SHARES_XML_FILE_NAME = "shares.xml";
        private const String USER_APPDATA_SUB_DIR = "\\AppData\\Roaming";
        private const String START_MENU_SUB_DIR = "Microsoft\\Windows\\Start Menu\\Programs";
        private const String AUTOSTART_FILE_NAME = "autostart.exe";
        private const String KEEPALIVE_FILE_NAME = "keepalive.exe";
        private const String APPNAMES_FILE_NAME = "appnames.exe";
        private const String APPNAMES_XML_FILE_NAME = "appnames.xml";
        private const String SHORTCUTS_SERVER_FILE_NAME = "shortcutssrv.exe";
        private const String RAAS_SERVER_COMPANY_NAME = "Elbitin";
        private const String RAAS_SERVER_APPLICATION_NAME = "RAAS Server";

        public static void CreateMissingUserDirs(String userPath)
        {
            // Configure directories to be used
            String userRAASServerPath = GetUserRAASServerPath(userPath);
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
            }
            catch { }
        }

        public static String GetAutostartPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_SERVER_COMPANY_NAME, RAAS_SERVER_APPLICATION_NAME);
            return Path.Combine(installPath, AUTOSTART_FILE_NAME);
        }

        public static String GetKeepAlivePath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_SERVER_COMPANY_NAME, RAAS_SERVER_APPLICATION_NAME);
            return Path.Combine(installPath, KEEPALIVE_FILE_NAME);
        }

        public static String GetAppNamesPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_SERVER_COMPANY_NAME, RAAS_SERVER_APPLICATION_NAME);
            return Path.Combine(installPath, APPNAMES_FILE_NAME);
        }

        public static String GetShortcutsServerPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_SERVER_COMPANY_NAME, RAAS_SERVER_APPLICATION_NAME);
            return Path.Combine(installPath, SHORTCUTS_SERVER_FILE_NAME);
        }

        public static String GetProgramSettingsShortcutPath()
        {
            return Path.Combine(Path.Combine(RegistryHelper.GetInstallPath(RAAS_SERVER_COMPANY_NAME, RAAS_SERVER_APPLICATION_NAME), PROGRAM_SHORTCUTS_SUB_DIR), SETTINGS_SHORTCUT_FILE_NAME);
        }
        public static String GetCommonLogsPath()
        {
            String commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(commonAppData, PROGRAM_LOGS_SUB_DIR);
        }

        public static String GetUserStartMenuPath(String userPath)
        {
            return Path.Combine(userPath, USER_START_MENU_SUB_DIR);
        }

        public static String GetUserDesktopPath(String userPath)
        {
            return Path.Combine(userPath, USER_DESKTOP_SUB_DIR);
        }

        public static String GetUserRAASServerIconsPath(String userPath)
        {
            string userRAASServerShortcutsPath = GetUserRAASServerShortcutsPath(userPath);
            return Path.Combine(userRAASServerShortcutsPath, ICONS_SUB_DIR);
        }

        public static String GetUsersFolderPath()
        {
            String commonData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return (new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))).Parent.ToString();
        }

        public static String GetCurrentRAASServerPath()
        {
            String localAppDataCompany = GetAppDataCompanyPath();
            return Path.Combine(localAppDataCompany, RAAS_SERVER_SUB_DIR);
        }

        public static String GetCurrentSharesXMLPath()
        {
            return Path.Combine(GetCurrentRAASServerPath(), SHARES_XML_FILE_NAME);
        }

        public static String GetCurrentAssociationsXMLPath()
        {
            return Path.Combine(GetCurrentRAASServerPath(), ASSOCIATIONS_FILE_NAME);
        }

        public static String GetUserRAASServerAssociationsXMLPath(String userPath)
        {
            String userRAASServerPath = GetUserRAASServerPath(userPath);
            return Path.Combine(userRAASServerPath, ASSOCIATIONS_FILE_NAME);
        }

        public static String GetUserRAASServerIconsAssociationsPath(String iconsPath)
        {
            return Path.Combine(iconsPath, ASSOCIATIONS_SUB_DIR);
        }

        public static String GetCurrentAppNamesXMLPath()
        {
            return Path.Combine(GetCurrentRAASServerPath(), APPNAMES_XML_FILE_NAME);
        }

        public static String GetUserRAASServerSharesXMLPath(String userPath)
        {
            String userRAASServerPath = GetUserRAASServerPath(userPath);
            return Path.Combine(userRAASServerPath, SHARES_XML_FILE_NAME);
        }

        public static String GetCurrentShortcutsXMLPath()
        {
            return Path.Combine(GetCurrentRAASServerPath(), SHORTCUTS_XML_FILE_NAME);
        }

        public static String GetCurrectShortcutsPath()
        {
            return Path.Combine(GetCurrentRAASServerPath(), SHORTCUTS_SUB_DIR);
        }

        public static String GetCurrectAssociationsPath()
        {
            return Path.Combine(Path.Combine(Path.Combine(GetCurrentRAASServerPath(), SHORTCUTS_SUB_DIR), ICONS_SUB_DIR), ASSOCITATIONS_SUB_DIR);
        }

        public static String GetCurrectIconsPath()
        {
            return Path.Combine(GetCurrectShortcutsPath(), ICONS_SUB_DIR);
        }

        public static String GetCurrentAssociationIconsPath()
        {
            return Path.Combine(GetCurrectIconsPath(), ASSOCIATIONS_SUB_DIR);
        }

        public static String GetCurrentIconFilePath(String iconFileName)
        {
            return Path.Combine(GetCurrectIconsPath(), iconFileName);
        }

        public static String GetUserRAASServerShortcutsPath(String userPath)
        {
            String userRAASServerPath = GetUserRAASServerPath(userPath);
            return Path.Combine(userRAASServerPath, SHORTCUTS_SUB_DIR);
        }
        public static String GetUserRAASServerAssociationsPath(String userPath)
        {
            return Path.Combine(Path.Combine(Path.Combine(GetUserRAASServerPath(userPath), SHORTCUTS_SUB_DIR), ICONS_SUB_DIR), ASSOCITATIONS_SUB_DIR);
        }

        public static String GetUserRAASServerShortcutsXMLPath(String userPath)
        {
            String userRAASServerPath = GetUserRAASServerPath(userPath);
            return Path.Combine(userRAASServerPath, SHORTCUTS_XML_FILE_NAME);
        }

        public static String GetUserRAASServerAppNamesXMLPath(String userPath)
        {
            String userRAASServerPath = GetUserRAASServerPath(userPath);
            return Path.Combine(userRAASServerPath, APPNAMES_XML_FILE_NAME);
        }

        public static String GetUserRAASServerPath(String userPath)
        {
            String userAppDataCompanyPath = GetUserAppDataCompanyPath(userPath);
            return Path.Combine(userAppDataCompanyPath, RAAS_SERVER_SUB_DIR);
        }

        public static String GetUserAppDataCompanyPath(String userPath)
        {
            String userAppDataPath = GetUserAppdataPath(userPath);
            return Path.Combine(userAppDataPath, RAAS_SERVER_COMPANY_NAME);
        }

        public static String GetUserAppdataPath(String userPath)
        {
            return userPath + USER_APPDATA_SUB_DIR;
        }

        public static String GetPublicDesktopPath()
        {
            return System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        }

        public static String GetCommonStartMenuPath()
        {
            string commonAppData = GetCommonAppDataPath();
            return Path.Combine(commonAppData, START_MENU_SUB_DIR);
        }

        public static String GetCommonAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        public static String GetCurrentAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static String GetAppDataCompanyPath()
        {
            return Path.Combine(GetCurrentAppDataPath(), RAAS_SERVER_COMPANY_NAME);
        }

        public static String GetAssociationIconPath(String extension, String associationsDirPath)
        {
            foreach (String file in Directory.GetFiles(associationsDirPath))
            {
                string[] fileParts = file.Split('\\');
                if (fileParts[fileParts.Length - 1].StartsWith("." + extension))
                    return file;
            }
            return null;
        }
    }
}
