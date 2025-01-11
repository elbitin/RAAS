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
using System.Linq;
using Elbitin.Applications.RAAS.RAASClient.RemoteSc.RAASServerServiceRef;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Collections.Generic;
using Serilog;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteSc
{
    public class Program : Object
    {
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);
        private const int MAX_ICON_SIZE = 500000;
        private static List<String> allowedExceptions = new List<string> {
                "System.ServiceModel.EndpointNotFoundException",
                "System.Net.Sockets.SocketException",
                "System.Reflection.TargetInvocationException",
                "System.Net.NetworkInformation.PingException",
                "System.ServiceModel.CommunicationException",
                "System.IO.IOException",
                "System.IO.PipeException",
                "System.ServiceModel.CommunicationObjectFaultedException",
                "System.ServiceModel.CommunicationObjectAbortedException",
            };

        [CallbackBehaviorAttribute(UseSynchronizationContext = false)]
        public class ServiceCallback : IRAASServerServiceCallback
        {
            public void ShareXmlChange()
            {
            }

            public void ShortcutsXmlChange()
            {
            }

            public void LoggedInChange()
            {
            }
        }

        [STAThread]
        static void Main(String[] args)
        {
            Log.Logger = RAASClientLogHelper.CreateDefaultRAASClientLogger("remotesc");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            if (args.Count() > 0)
            {
                // Initialize variables from arguments
                string uncPath = SharesPathHelper.GetUNCPath(args[0]);
                String serverName = "";
                bool serverSupplied = false;
                for (int i = 0; i < args.Count() - 1; i++)
                {
                    if (args[i].StartsWith("-server"))
                    {
                        serverName = args[++i];
                        serverSupplied = true;
                    }
                }

                // Prepare paths
                String path = uncPath;
                if (!serverSupplied)
                {
                    serverName = SharesPathHelper.GetServerNameFromPath(uncPath);
                    path = SharesPathHelper.GetServerPath(uncPath, serverName);
                }
                String serversFile = RAASClientPathHelper.GetServersConfigFilePath();

                // Try to create shortcut with shortcut image
                ServerSettings serverSettings = new ServerSettings(serverName);
                if (serverSettings.ServerEnabled)
                {
                    RAASServerServiceClient raasServerServiceClient = InitRAASServerServiceClient(serverSettings);

                    // Determine path of shortcut
                    String fileName = Path.GetFileName(path) + ".lnk";
                    String shortcutFilePath = Path.Combine(Path.GetDirectoryName(path), fileName);
                    if (args.Count() > 1)
                        shortcutFilePath = Path.Combine(args[1], fileName);
                    else
                        shortcutFilePath = Path.Combine(Path.GetDirectoryName(uncPath), fileName);

                    try
                    {
                        // Try to get shortcut icon from server
                        raasServerServiceClient.Open();
                        byte[] iconBytes;
                        try
                        {
                            iconBytes = raasServerServiceClient.GetShortcutIcon(path);
                            if (iconBytes.Length >= MAX_ICON_SIZE)
                                throw new Exception("Too large icon file size");
                            SaveWithTargetIcon(shortcutFilePath, serverName, path, iconBytes);
                        }
                        catch
                        {
                            try
                            {
                                SaveWithDefaultIcon(shortcutFilePath, serverName, path);
                            }
                            catch (UnauthorizedAccessException /* e */)
                            {
                                MessageBox.Show(new Form() { TopMost = true }, Properties.Resources.Shortcut_UnauthorizedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            catch
                            {
                                MessageBox.Show(new Form() { TopMost = true }, Properties.Resources.Shortcut_FailGeneralMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.HResult == E_ACCESSDENIED)
                            MessageBox.Show(new Form() { TopMost = true }, Properties.Resources.Shortcut_AccessDeniedMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.Shortcut_ExceptionMessage, e.Message), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                else
                {
                    MessageBox.Show(new Form() { TopMost = true }, String.Format(Properties.Resources.Shortcut_NotEnabledMessage, "RAAS Server"), Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private static void SaveWithDefaultExeIcon(string shortcutFilePath, string serverName, string path)
        {
            String iconLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "imageres.dll");
            int iconIndex = 11;
            ShortcutIcon defaultExeIcon = new ShortcutIcon(IconHelper.GetShellShortcutIconBitmap(iconLocation, iconIndex));
            byte[] iconBytes = defaultExeIcon.IconToBytes();
            SaveWithTargetIcon(shortcutFilePath, serverName, path, iconBytes);
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

        private static RAASServerServiceClient InitRAASServerServiceClient(ServerSettings serverSettings)
        {
            // Get server settings from XML file
            String serverName = serverSettings.ServerName;
            String userName = serverSettings.UserName;
            String password = serverSettings.Password;

            // Start raas server service client
            String domain = serverSettings.Domain;
            ServiceCallback clientServiceCallback = new ServiceCallback();
            InstanceContext context = new InstanceContext(clientServiceCallback);
            RAASServerServiceClient raasServerServiceClient = new RAASServerServiceClient(context);
            EndpointAddress endpoint = new EndpointAddress(new Uri(@"net.tcp://" + serverName + @":43000/RAASServerService"), new DnsEndpointIdentity("localhost"), new AddressHeaderCollection());
            raasServerServiceClient.Endpoint.Address = endpoint;
            raasServerServiceClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            raasServerServiceClient.ClientCredentials.Windows.ClientCredential.UserName = userName;
            raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Password = password;
            raasServerServiceClient.ClientCredentials.Windows.ClientCredential.Domain = domain;
            return raasServerServiceClient;
        }

        private static void SaveWithTargetIcon(string shortcutFilePath, string serverName, string path, byte[] iconBytes)
        {
            String addedIcons = RAASClientPathHelper.GetAddedIconsDirectoryPath();

            // Create hash for icon
            String hash = IconHelper.GetIconHashString(iconBytes);
            String iconFile = Path.Combine(addedIcons, IconHelper.GetIconFileName(hash, Path.GetFileName(path)));

            // Initialize shortcut icon
            ShortcutIcon shortcutIcon = new ShortcutIcon(iconBytes);

            // RemoteApp icon
            ShortcutIcon remoteAppIcon = ShortcutIcon.CreateRemoteAppIcon();

            // Add RemoteApp overlay to icon
            shortcutIcon.AddIconOverlay(remoteAppIcon);

            // Save icon
            shortcutIcon.SaveAsMultiSizeIcon(iconFile);

            // Create link
            String arguments = "\"" + path + "\" -remote -server " + serverName;
            LinkHelper.CreateLink(iconFile, shortcutFilePath, arguments);
        }

        private static void SaveWithDefaultIcon(string shortcutFilePath, string serverName, string path)
        {
            // Initialize paths
            String raasPath = RAASClientPathHelper.GetAppDataRAASClientPath();
            String defaultIconPath = Path.Combine(raasPath, "Default.ico");

            // Create default icon
            ShortcutIcon defaultIcon = ShortcutIcon.CreateDefaultIcon();
            defaultIcon.SaveAsMultiSizeIcon(Path.Combine(raasPath, "Default.ico"));

            // Create link
            String arguments = "\"" + path + "\" -remote -server " + serverName;
            LinkHelper.CreateLink(defaultIconPath, shortcutFilePath, arguments);
        }
    }
}
