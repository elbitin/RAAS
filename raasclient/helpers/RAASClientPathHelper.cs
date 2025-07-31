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
﻿using System;
using System.Collections.Generic;
using System.IO;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    static class RAASClientPathHelper
    {
        private const String SERVER_SETTINGS_CONFIG_FILE_NAME = "servers.xml";
        private const String EXPLORER_CONFIG_FILE_NAME = "nsextvisibility.xml";
        private const String VISUALIZATIONS_CONFIG_FILE_NAME = "visualizations.xml";
        private const String REMOTE_DESKTOP_CONFIG_FILE_NAME = "rdesktop.xml";
        private const String CLIENT_SHORTCUTS_FILE_NAME = "clientshortcuts.xml";
        private const String SERVER_SHORTCUTS_FILE_NAME = "servershortcuts.xml";
        private const String SHARE_FILE_NAME = "share.xml";
        private const String HELP_PDF_FILE_NAME = "RAAS Client.pdf";
        private const String RAAS_CLIENT_APPLICATION_DIR_NAME = "RAAS Client";
        private const String SERVER_ICONS_DIR_NAME = "Icons";
        private const String SERVER_SHORTCUTS_DIR_NAME = "Shortcuts";
        private const String RAAS_CLIENT_APPLICATION_NAME = "RAAS Client";
        private const String SETUP_DIR_NAME = "Setup";
        private const String HELP_DIR_NAME = "Help";
        private const String SHORTCUTS_PROGRAM_FILE_NAME = "shortcuts.exe";
        private const String SERVER_CONFIG_PROGRAM_FILE_NAME = "servercfg.exe";
        private const String REMOTE_APPS_PROGRAM_FILE_NAME = "rapps.exe";
        private const String RDESKTOP_PROGRAM_FILE_NAME = "rdesktop.exe";
        private const String SETUP_DIRECTORY_NAME = "Setup";
        private const String ICONS_DIRECTORY_NAME = "Icons";
        private const String REMOTE_DESKTOP_TEMPLATE_FILE_NAME = "rdesktop.tpl";
        private const String RAAS_CLIENT_ICON_FILE_NAME = "RAASClientIcon.ico";
        private const String REMOTE_DESKTOP_FILE_NAME = "rdesktop.rdp";
        private const String RAAS_CLIENT_COMPANY_NAME = "Elbitin";

        public static String GetShortcutsProgramPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, SHORTCUTS_PROGRAM_FILE_NAME);
        }

        public static String GetServerConfigProgramPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, SERVER_CONFIG_PROGRAM_FILE_NAME);
        }

        public static String GetRemoteAppsProgramPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, REMOTE_APPS_PROGRAM_FILE_NAME);
        }

        public static String GetRDesktopProgramPath()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, RDESKTOP_PROGRAM_FILE_NAME);
        }

        public static String GetSetupDirectory()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME,RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, SETUP_DIRECTORY_NAME);
        }

        public static String GetIconsDirectory()
        {
            String installPath = RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
            return Path.Combine(installPath, ICONS_DIRECTORY_NAME);
        }

        public static String GetRDPTemplatePath()
        {
            return Path.Combine(GetSetupDirectory(), REMOTE_DESKTOP_TEMPLATE_FILE_NAME);
        }

        public static String GetRAASClientIconPath()
        {
            return Path.Combine(GetIconsDirectory(), RAAS_CLIENT_ICON_FILE_NAME);
        }

        public static String GetAppDataRDPFilePath()
        {
            return GetAppDataRAASClientCombinedPath(REMOTE_DESKTOP_FILE_NAME);
        }

        public static void CreateMissingAppDataRAASClientDirectories()
        {
            String companyAppDataPath = GetAppDataCompanyPath();
            CreateDirectoryIfMissing(companyAppDataPath);
            String programAppDataDirectory = GetAppDataRAASClientPath();
            CreateDirectoryIfMissing(programAppDataDirectory);
            String addedIconsDirectory = GetAddedIconsDirectoryPath();
            CreateDirectoryIfMissing(addedIconsDirectory);
        }

        public static void CreateMissingServerAppDataRAASDirectories(String serverName)
        {
            String serverDirectory = GetServerAppDataRAASClientPath(serverName);
            CreateDirectoryIfMissing(serverDirectory);
            String serverIconsDirectory = GetServerIconsDirectoryPath(serverName);
            CreateDirectoryIfMissing(serverIconsDirectory);
            String serverShortcutsDirectory = GetServerShortcutsDirectory(serverName);
            CreateDirectoryIfMissing(serverShortcutsDirectory);
        }

        private static void CreateDirectoryIfMissing(String directoryName)
        {
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }

        public static String GetHelpPdfPath()
        {
            String raasClientInstallPath = GetRAASClientInstallPath();
            String[] pdfFileNameParts = HELP_PDF_FILE_NAME.Split('.');
            String pdfFullLanguagePdfFileName = pdfFileNameParts[0] + "." + System.Globalization.CultureInfo.CurrentCulture.Name + ".pdf";
            String pdfMajorLanguagePdfFileName = pdfFileNameParts[0] + "." + System.Globalization.CultureInfo.CurrentCulture.Name.Split('-')[0] + ".pdf";
            String pdfStartsWithMajorLamguagePdfFileName = pdfFileNameParts[0] + "-";
            if (File.Exists(Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME), pdfFullLanguagePdfFileName)))
                return Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME), pdfFullLanguagePdfFileName);
            else if (File.Exists(Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME), pdfMajorLanguagePdfFileName)))
                return Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME), pdfMajorLanguagePdfFileName);
            foreach (String fileName in Directory.GetFiles(Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME)))) {
                if (fileName.ToLowerInvariant().StartsWith(pdfStartsWithMajorLamguagePdfFileName.ToLowerInvariant()))
                    return fileName;
            }
            return Path.Combine(Path.Combine(raasClientInstallPath, HELP_DIR_NAME), HELP_PDF_FILE_NAME);
        }

        public static String GetServersConfigFilePath()
        {
            return GetAppDataRAASClientCombinedPath(SERVER_SETTINGS_CONFIG_FILE_NAME);
        }

        public static String GetDefaultRemoteDesktopConfigFilePath()
        {
            String raasClientInstallPath = GetRAASClientInstallPath();
            return Path.Combine(Path.Combine(raasClientInstallPath, SETUP_DIR_NAME), REMOTE_DESKTOP_CONFIG_FILE_NAME);
        }

        public static String GetDefaultServersConfigFilePath()
        {
            String raasClientInstallPath = GetRAASClientInstallPath();
            return Path.Combine(Path.Combine(raasClientInstallPath, SETUP_DIR_NAME), SERVER_SETTINGS_CONFIG_FILE_NAME);
        }

        public static String GetDefaultVisualizationsPath()
        {
            String raasClientInstallPath = GetRAASClientInstallPath();
            return Path.Combine(Path.Combine(raasClientInstallPath, SETUP_DIR_NAME), VISUALIZATIONS_CONFIG_FILE_NAME);
        }

        public static String GetExplorerConfigFilePath(String serverName)
        {
            String explorerFile = GetServerAppDataRAASClientCombinedPath(serverName, EXPLORER_CONFIG_FILE_NAME);
            if (!File.Exists(explorerFile))
            {
                explorerFile = GetDefaultExplorerConfigPath();
            }
            return explorerFile;
        }

        public static string GetDefaultExplorerConfigPath()
        {
            string explorerFile;
            String raasClientInstallPath = GetRAASClientInstallPath();
            explorerFile = Path.Combine(Path.Combine(raasClientInstallPath, SETUP_DIR_NAME), EXPLORER_CONFIG_FILE_NAME);
            return explorerFile;
        }

        public static String GetAppDataExplorerConfigFilePath(String serverName)
        {
            String explorerFile = GetServerAppDataRAASClientCombinedPath(serverName, EXPLORER_CONFIG_FILE_NAME);
            return explorerFile;
        }

        public static String GetVisualizationsConfigFilePath(String serverName)
        {
            String visualizationsFile = GetServerAppDataRAASClientCombinedPath(serverName, VISUALIZATIONS_CONFIG_FILE_NAME);
            if (!File.Exists(visualizationsFile))
            {
                return GetDefaultVisualizationsPath();
            }
            return visualizationsFile;
        }

        public static String GetAppDataVisualizationsConfigFilePath(String serverName)
        {
            String visualizationsFile = GetServerAppDataRAASClientCombinedPath(serverName, VISUALIZATIONS_CONFIG_FILE_NAME);
            return visualizationsFile;
        }

        public static String GetRemoteDesktopConfigFilePath(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, REMOTE_DESKTOP_CONFIG_FILE_NAME);
        }

        public static String GetClientShortcutsFilePath(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, CLIENT_SHORTCUTS_FILE_NAME);
        }

        public static String GetServerShortcutsFilePath(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, SERVER_SHORTCUTS_FILE_NAME);
        }

        public static String GetSharesFilePath(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, SHARE_FILE_NAME);
        }

        private const String addedIconsDirectoryName = "AddedIcons";
        public static String GetAddedIconsDirectoryPath()
        {
            return GetAppDataRAASClientCombinedPath(addedIconsDirectoryName);
        }

        public static String GetServerIconsDirectoryPath(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, SERVER_ICONS_DIR_NAME);
        }

        public static String GetServerShortcutsDirectory(String serverName)
        {
            return GetServerAppDataRAASClientCombinedPath(serverName, SERVER_SHORTCUTS_DIR_NAME);
        }

        public static String GetRAASClientInstallPath()
        {
            return RegistryHelper.GetInstallPath(RAAS_CLIENT_COMPANY_NAME, RAAS_CLIENT_APPLICATION_NAME);
        }

        public static String GetServerAppDataRAASClientCombinedPath(String serverName, String fileName)
        {
            String appDataRaasClientServer = GetServerAppDataRAASClientPath(serverName);
            String appDataFile = Path.Combine(appDataRaasClientServer, fileName);
            return appDataFile;
        }

        public static String GetAppDataRAASClientLogsPath()
        {
            return Path.Combine(GetAppDataRAASClientPath(), "Logs");
        }

        public static String GetAppDataRAASClientCombinedPath(String combinePath)
        {
            String appDataRaasClient = GetAppDataRAASClientPath();
            String appDataFile = Path.Combine(appDataRaasClient, combinePath);
            return appDataFile;
        }

        public static String GetServerAppDataRAASClientPath(String serverName)
        {
            string appDataRaasClient = GetAppDataRAASClientPath();
            return Path.Combine(appDataRaasClient, "[" + serverName + "]");
        }

        public static String GetAppDataRAASClientPath()
        {
            return Path.Combine(GetAppDataCompanyPath(), RAAS_CLIENT_APPLICATION_DIR_NAME);
        }

        public static String GetAppDataCompanyPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RAAS_CLIENT_COMPANY_NAME);
        }
    }
}
