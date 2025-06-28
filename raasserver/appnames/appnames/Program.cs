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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elbitin.Applications.RAAS.RAASServer.Models;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Windows.Management.Deployment;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Security.Principal;
using System.Diagnostics;
using System.Windows.Forms;
using Serilog;
using System.IO;
using System.Xml.Linq;

namespace appnames
{
    internal static class Program
    {
        private static List<String> allowedExceptions = new List<string>
        {
        };

        [STAThread]
        static void Main()
        {
            Log.Logger = RAASServerLogHelper.CreateDefaultRAASServerLogger("appnames");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            
            // Register apps
            var pm = new PackageManager();
            var packages = pm.FindPackagesForUser(@"");
            List<AppListApp> appListApps = new List<AppListApp>();
            AppList appList = new AppList();
            foreach (var package in packages)
            {
                var asyncResult = package.GetAppListEntriesAsync();
                while (asyncResult.Status != Windows.Foundation.AsyncStatus.Completed)
                {
                    Thread.Sleep(10);
                }
                foreach (var app in asyncResult.GetResults())
                {
                    AppListApp appListApp = new AppListApp();
                    appListApp.AppUserModelId = app.AppUserModelId;
                    appListApp.DisplayName = app.DisplayInfo.DisplayName;
                    appListApps.Add(appListApp);
                }
            }

            // Register shortcuts
            List<AppListShortcut> appListShortcuts = new List<AppListShortcut>();
            String userStartMenuPath = RAASServerPathHelper.GetUserStartMenuPath(Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));
            String commonStartMenuPath = RAASServerPathHelper.GetCommonStartMenuPath();
            String userDesktopPath = RAASServerPathHelper.GetUserDesktopPath(Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));
            String commonDesktopPath = RAASServerPathHelper.GetPublicDesktopPath();
            appListShortcuts.AddRange(RegisterShortcuts(userStartMenuPath));
            appListShortcuts.AddRange(RegisterShortcuts(commonStartMenuPath));
            appListShortcuts.AddRange(RegisterShortcuts(userDesktopPath));
            appListShortcuts.AddRange(RegisterShortcuts(commonDesktopPath));

            // Write appnames XML
            appList.App = appListApps.ToArray();

            appList.Shortcut = appListShortcuts.ToArray();
            String appListXml = appList.SerializeXml();

            // Return if app list unchanged
            if (File.Exists(RAASServerPathHelper.GetCurrentAppNamesXMLPath()))
            {
                try
                {
                    XDocument appListXDocument = XDocument.Parse(appListXml);
                    XDocument existingAppListXml = XDocument.Load(RAASServerPathHelper.GetCurrentAppNamesXMLPath());
                    if (XDocument.DeepEquals(appListXDocument, existingAppListXml))
                        return;
                }catch { }
            }

            appList.SerializeXmlFile(RAASServerPathHelper.GetCurrentAppNamesXMLPath());
        }

        private static List<AppListShortcut> RegisterShortcuts(String path)
        {
            List<AppListShortcut> appListShortcuts = new List<AppListShortcut>();
            foreach (String file in Directory.GetFiles(path))
            {
                AppListShortcut appListShortcut = new AppListShortcut();
                appListShortcut.shortcutPath = file;
                appListShortcut.DisplayName = GetLocalizedName(file);
                appListShortcuts.Add(appListShortcut);
            }
            foreach (String dir in Directory.GetDirectories(path))
                appListShortcuts.AddRange(RegisterShortcuts(dir));
            return appListShortcuts;
        }


        private static String GetLocalizedName(String path)
        {
            String inDirectory = Path.GetDirectoryName(path);
            String pathFileName = Path.GetFileName(path);
            String desktopIniPath = Path.Combine(inDirectory, "desktop.ini");
            if (File.Exists(desktopIniPath))
            {
                String desktopIniText = File.ReadAllText(desktopIniPath);
                String[] sections = desktopIniText.Split('[');
                foreach (String section in sections)
                {
                    if (section.StartsWith("LocalizedFileNames]"))
                    {
                        String[] rows = section.Split('\n');
                        for (int rowNum = 1; rowNum < rows.Count(); rowNum++)
                        {
                            String[] equalSignParts = rows[rowNum].Split('=');
                            if (equalSignParts.Count() > 1)
                            {
                                String fileName = equalSignParts[0];
                                if (fileName.ToLowerInvariant() == pathFileName.ToLowerInvariant())
                                {
                                    String[] commaParts = equalSignParts[1].Split(',');
                                    if (commaParts.Count() > 1)
                                    {
                                        int index = -Convert.ToInt32(commaParts[1]);
                                        String[] atParts = commaParts[0].Split('@');
                                        if (atParts.Count() > 1)
                                        {
                                            String stringResourcePath = atParts[1];
                                            stringResourcePath = Environment.ExpandEnvironmentVariables(stringResourcePath);
                                            if (File.Exists(stringResourcePath))
                                            {
                                                String localizedName = StringHelper.ExtractStringFromLib(stringResourcePath, index);
                                                if (localizedName.Length > 0)
                                                    return localizedName;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return pathFileName.Split('.')[0];
        }

        private static void OnFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs eventArgs)
        {
            if (!allowedExceptions.Contains(eventArgs.Exception.GetType().ToString()))
                Log.Debug(eventArgs.Exception.ToString());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            Log.CloseAndFlush();
        }
    }
}
