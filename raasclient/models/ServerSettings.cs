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
using System.Text;
using System.Xml;
using System.IO;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.Models
{
    public class AliasAlreadyExistException : Exception
    {
        public AliasAlreadyExistException()
        {
        }

        public AliasAlreadyExistException(string message)
            : base(message)
        {
        }

        public AliasAlreadyExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class CouldNotLoadServerSettingsException : Exception
    {
        public CouldNotLoadServerSettingsException()
        {
        }

        public CouldNotLoadServerSettingsException(string message)
            : base(message)
        {
        }

        public CouldNotLoadServerSettingsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ServerSettings
    {
        public String ServerName { get; set; } = "";
        public String UserName { get; set; } = "";
        public String Alias { get; set; } = "";
        public String Password { get; set; } = "";
        public String Domain { get; set; } = "";
        public int AuthenticationLevel { get; set; } = 1;
        public bool AutoConnect { get; set; } = false;
        public bool AutoReconnect { get; set; } = false;
        public bool AutoStartPrograms { get; set; } = false;
        public bool KeepAliveAgent { get; set; } = false;
        public bool CreateShortcuts { get; set; } = false;
        public bool CreateDesktopShortcuts { get; set; } = false;
        public bool CreateStartMenuShortcuts { get; set; } = false;
        public bool CreateUWPApplicationShortcuts { get; set; } = false;
        public bool DesktopRoot { get; set; } = false;
        public bool RedirectClipboard { get; set; } = false;
        public bool RedirectUsb { get; set; } = false;
        public bool RedirectPrinters { get; set; } = false;
        public bool ServerEnabled { get; set; } = false;
        public bool ShortcutsAppendAlias { get; set; }
        public bool RemoveShortcuts { get; set; } = false;
        public bool ShortcutsRemoved { get; set; } = false;
        public bool RemoveServer { get; set; } = false;
        public bool ShowNotifications { get; set; } = false;
        public bool LocalizeShortcuts { get; set; } = false;

        public ServerSettings(String serverName)
        {
            ServerName = serverName;
            if (!ReadServerSettings())
                throw new CouldNotLoadServerSettingsException();
        }

        public ServerSettings(String serverName, XmlDocument sourceXmlDoc)
        {
            ServerName = serverName;
            if (!ReadServerSettings(sourceXmlDoc))
                throw new CouldNotLoadServerSettingsException();
        }

        public bool ReadServerSettings()
        {
            String serversFile = RAASClientPathHelper.GetServersConfigFilePath();
            if (File.Exists(serversFile))
            {
                XmlDocument sourceXmlDoc = new XmlDocument();
                if (XmlDocumentHelper.LoadWithRetries(sourceXmlDoc, serversFile))
                {
                    if (ReadServerSettings(sourceXmlDoc))
                        return true;
                }
            }
            return ReadDefaultServerSettings();
        }

        public bool ReadDefaultServerSettings()
        {
            XmlDocument defaultXmlDoc = new XmlDocument();
            if (XmlDocumentHelper.LoadWithRetries(defaultXmlDoc, RAASClientPathHelper.GetDefaultServersConfigFilePath()))
               foreach (XmlNode node in defaultXmlDoc.SelectNodes("Servers/Server"))
               {
                   ReadServerSettingsFromNode(node);
                   return true;
               }
            return false;
        }

        public bool ReadServerSettings(XmlDocument sourceXmlDoc)
        {
            String serversFile = RAASClientPathHelper.GetServersConfigFilePath();
            if (File.Exists(serversFile))
            {
                foreach (XmlNode node in sourceXmlDoc.SelectNodes("Servers/Server/Name"))
                {
                    if (node.InnerText.ToUpperInvariant() == ServerName.ToUpperInvariant())
                    {
                        ReadServerSettingsFromNode(node.ParentNode);
                        return true;
                    }
                }
            }
            return false;
        }

        private void ReadServerSettingsFromNode(XmlNode node)
        {
            // Update server settings according to config
            XmlNode authenticationLevel = node.SelectSingleNode("AuthenticationLevel");
            if (authenticationLevel != null)
                AuthenticationLevel = Convert.ToInt32(authenticationLevel.InnerText);
            XmlNode userName = node.SelectSingleNode("UserName");
            if (userName != null)
                UserName = userName.InnerText;
            XmlNode alias = node.SelectSingleNode("Alias");
            if (alias != null)
                Alias = alias.InnerText;
            XmlNode password = node.SelectSingleNode("Password");
            if (password != null)
            {
                List<byte> bytes = new List<byte>();
                for (int i = 0; i < password.InnerText.Length; i += 2)
                    bytes.Add(Convert.ToByte(int.Parse(password.InnerText.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)));
                Password = Encoding.Unicode.GetString(System.Security.Cryptography.ProtectedData.Unprotect(bytes.ToArray(), null, System.Security.Cryptography.DataProtectionScope.CurrentUser));
            }
            XmlNode domain = node.SelectSingleNode("Domain");
            if (domain != null)
                Domain = domain.InnerText;
            XmlNode autoConnect = node.SelectSingleNode("AutoConnect");
            if (autoConnect != null && autoConnect.InnerText == "1")
                AutoConnect = true;
            else
                AutoConnect = false;
            XmlNode autoReconnect = node.SelectSingleNode("AutoReconnect");
            if (autoReconnect != null && autoReconnect.InnerText == "1")
                AutoReconnect = true;
            else
                AutoReconnect = false;
            XmlNode autostartPrograms = node.SelectSingleNode("AutoStartPrograms");
            if (autostartPrograms != null && autostartPrograms.InnerText == "1")
                AutoStartPrograms = true;
            else
                AutoStartPrograms = false;
            XmlNode keepAliveAgent = node.SelectSingleNode("KeepAliveAgent");
            if (keepAliveAgent != null && keepAliveAgent.InnerText == "1")
                KeepAliveAgent = true;
            else
                KeepAliveAgent = false;
            XmlNode shortcuts = node.SelectSingleNode("CreateShortcuts");
            if (shortcuts != null && shortcuts.InnerText == "1")
                CreateShortcuts = true;
            else
                CreateShortcuts = false;
            XmlNode startMenuShortcuts = node.SelectSingleNode("CreateStartMenuShortcuts");
            if (startMenuShortcuts != null && startMenuShortcuts.InnerText == "1")
                CreateStartMenuShortcuts = true;
            else
                CreateStartMenuShortcuts = false;
            XmlNode uwpApplicationShortcuts = node.SelectSingleNode("CreateUWPApplicationShortcuts");
            if (uwpApplicationShortcuts != null && uwpApplicationShortcuts.InnerText == "1")
                CreateUWPApplicationShortcuts = true;
            else
                CreateUWPApplicationShortcuts = false;
            XmlNode desktopShortcuts = node.SelectSingleNode("CreateDesktopShortcuts");
            if (desktopShortcuts != null && desktopShortcuts.InnerText == "1")
                CreateDesktopShortcuts = true;
            else
                CreateDesktopShortcuts = false;
            XmlNode desktopRoot = node.SelectSingleNode("DesktopRoot");
            if (desktopRoot != null && desktopRoot.InnerText == "1")
                DesktopRoot = true;
            else
                DesktopRoot = false;
            XmlNode clipboard = node.SelectSingleNode("RedirectClipboard");
            if (clipboard != null && clipboard.InnerText == "1")
                RedirectClipboard = true;
            else
                RedirectClipboard = false;
            XmlNode usb = node.SelectSingleNode("RedirectDevices");
            if (usb != null && usb.InnerText == "1")
                RedirectUsb = true;
            else
                RedirectUsb = false;
            XmlNode printers = node.SelectSingleNode("RedirectPrinters");
            if (printers != null && printers.InnerText == "1")
                RedirectPrinters = true;
            else
                RedirectPrinters = false;
            XmlNode serverEnabled = node.SelectSingleNode("Enabled");
            if (serverEnabled != null && serverEnabled.InnerText == "1")
                ServerEnabled = true;
            else
                ServerEnabled = false;
            XmlNode ahortcutsAppendAlias = node.SelectSingleNode("ShortcutsAppendAlias");
            if (ahortcutsAppendAlias != null && ahortcutsAppendAlias.InnerText == "1")
                ShortcutsAppendAlias = true;
            else
                ShortcutsAppendAlias = false;
            XmlNode removeShortcuts = node.SelectSingleNode("RemoveShortcuts");
            if (removeShortcuts != null && removeShortcuts.InnerText == "1")
                RemoveShortcuts = true;
            else
                RemoveShortcuts = false;
            XmlNode shortcutsRemoved = node.SelectSingleNode("ShortcutsRemoved");
            if (shortcutsRemoved != null && shortcutsRemoved.InnerText == "1")
                ShortcutsRemoved = true;
            else
                ShortcutsRemoved = false;
            XmlNode removeServer = node.SelectSingleNode("RemoveServer");
            if (removeServer != null && removeServer.InnerText == "1")
                RemoveServer = true;
            else
                RemoveServer = false;
            XmlNode showNotifications = node.SelectSingleNode("ShowNotifications");
            if (showNotifications != null && showNotifications.InnerText == "1")
                ShowNotifications = true;
            else
                ShowNotifications = false;
            XmlNode localizeShortcuts = node.SelectSingleNode("LocalizeShortcuts");
            if (localizeShortcuts != null && localizeShortcuts.InnerText == "1")
                LocalizeShortcuts = true;
            else
                LocalizeShortcuts = false;
        }

        public void Remove()
        {
            String serversFile = RAASClientPathHelper.GetServersConfigFilePath();
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            if (!File.Exists(serversFile))
            {
                // Create new root node
                rootNode = xmlDoc.CreateElement("Servers");
                xmlDoc.AppendChild(rootNode);
            }
            else
            {
                // Load existing XML document
                if (!XmlDocumentHelper.LoadWithRetries(xmlDoc, serversFile))
                    throw new CouldNotLoadServerSettingsException();
            }

            // Remove any existing settings node for server
            foreach (XmlNode node in xmlDoc.SelectNodes("Servers/Server/Name"))
            {
                if (node.InnerText.ToUpperInvariant() == ServerName.ToUpperInvariant())
                {
                    node.ParentNode.ParentNode.RemoveChild(node.ParentNode);
                    break;
                }
            }

            XmlTextWriter serversFileWriter = new XmlTextWriter(serversFile, null);
            serversFileWriter.Formatting = Formatting.None;
            xmlDoc.Save(serversFileWriter);
            serversFileWriter.Close();
        }

        public void SaveServerSettings()
        {
            String serversFile = RAASClientPathHelper.GetServersConfigFilePath();
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;
            if (!File.Exists(serversFile))
            {
                // Create new root node
                rootNode = xmlDoc.CreateElement("Servers");
                xmlDoc.AppendChild(rootNode);
            }
            else
            {
                // Load existing XML document
                if (!XmlDocumentHelper.LoadWithRetries(xmlDoc, serversFile))
                    throw new CouldNotLoadServerSettingsException();
            }

            // Check if alias is already taken
            bool aliasAlreadyExist = false;
            foreach (XmlNode innerNode in xmlDoc.SelectNodes("Servers/Server"))
            {
                if (Alias.Length >0 &&
                    innerNode.SelectSingleNode("Alias").InnerText.ToUpperInvariant() == Alias.ToUpperInvariant() &&
                    innerNode.SelectSingleNode("Name").InnerText.ToUpperInvariant() != ServerName.ToUpperInvariant())
                    aliasAlreadyExist = true;
            }
            if (aliasAlreadyExist)
            {
                throw new AliasAlreadyExistException();
            }

            // Remove any existing settings node for server
            foreach (XmlNode node in xmlDoc.SelectNodes("Servers/Server/Name"))
            {
                if (node.InnerText.ToUpperInvariant() == ServerName.ToUpperInvariant())
                {
                    node.ParentNode.ParentNode.RemoveChild(node.ParentNode);
                    break;
                }
            }

            // Store settings in XML
            rootNode = xmlDoc.SelectSingleNode("Servers");
            XmlNode serverNode = xmlDoc.CreateElement("Server");
            rootNode.AppendChild(serverNode);
            XmlNode server = xmlDoc.CreateElement("Name");
            server.InnerText = ServerName;
            serverNode.AppendChild(server);
            XmlNode authenticationLevel = xmlDoc.CreateElement("AuthenticationLevel");
            authenticationLevel.InnerText = AuthenticationLevel.ToString();
            serverNode.AppendChild(authenticationLevel);
            XmlNode password = xmlDoc.CreateElement("Password");
            String numPassword = "";
            foreach (byte b in System.Security.Cryptography.ProtectedData.Protect(Encoding.Unicode.GetBytes(Password), null, System.Security.Cryptography.DataProtectionScope.CurrentUser))
                numPassword += Convert.ToInt16(b).ToString("X2");
            password.InnerText = numPassword;
            serverNode.AppendChild(password);
            XmlNode autoStartPrograms = xmlDoc.CreateElement("AutoStartPrograms");
            if (AutoStartPrograms)
                autoStartPrograms.InnerText = "1";
            else
                autoStartPrograms.InnerText = "0";
            serverNode.AppendChild(autoStartPrograms);
            XmlNode keepAliveAgent = xmlDoc.CreateElement("KeepAliveAgent");
            if (KeepAliveAgent)
                keepAliveAgent.InnerText = "1";
            else
                keepAliveAgent.InnerText = "0";
            serverNode.AppendChild(keepAliveAgent);
            XmlNode clipboard = xmlDoc.CreateElement("RedirectClipboard");
            if (RedirectClipboard)
                clipboard.InnerText = "1";
            else
                clipboard.InnerText = "0";
            serverNode.AppendChild(clipboard);
            XmlNode usb = xmlDoc.CreateElement("RedirectDevices");
            if (RedirectUsb)
                usb.InnerText = "1";
            else
                usb.InnerText = "0";
            serverNode.AppendChild(usb);
            XmlNode printers = xmlDoc.CreateElement("RedirectPrinters");
            if (RedirectPrinters)
                printers.InnerText = "1";
            else
                printers.InnerText = "0";
            serverNode.AppendChild(printers);
            XmlNode autoConnect = xmlDoc.CreateElement("AutoConnect");
            if (AutoConnect)
                autoConnect.InnerText = "1";
            else
                autoConnect.InnerText = "0";
            serverNode.AppendChild(autoConnect);
            XmlNode autoReconnect = xmlDoc.CreateElement("AutoReconnect");
            if (AutoReconnect)
                autoReconnect.InnerText = "1";
            else
                autoReconnect.InnerText = "0";
            serverNode.AppendChild(autoReconnect);
            XmlNode remoteApplications = xmlDoc.CreateElement("RemoteApplications");
            remoteApplications.InnerText = "1";
            serverNode.AppendChild(remoteApplications);
            XmlNode shortcuts = xmlDoc.CreateElement("CreateShortcuts");
            if (CreateShortcuts)
                shortcuts.InnerText = "1";
            else
                shortcuts.InnerText = "0";
            serverNode.AppendChild(shortcuts);
            XmlNode startMenuShortcuts = xmlDoc.CreateElement("CreateStartMenuShortcuts");
            if (CreateStartMenuShortcuts)
                startMenuShortcuts.InnerText = "1";
            else
                startMenuShortcuts.InnerText = "0";
            serverNode.AppendChild(startMenuShortcuts);
            XmlNode uwpApplicationShortcuts = xmlDoc.CreateElement("CreateUWPApplicationShortcuts");
            if (CreateUWPApplicationShortcuts)
                uwpApplicationShortcuts.InnerText = "1";
            else
                uwpApplicationShortcuts.InnerText = "0";
            serverNode.AppendChild(uwpApplicationShortcuts);
            XmlNode desktopShortcuts = xmlDoc.CreateElement("CreateDesktopShortcuts");
            if (CreateDesktopShortcuts)
                desktopShortcuts.InnerText = "1";
            else
                desktopShortcuts.InnerText = "0";
            serverNode.AppendChild(desktopShortcuts);
            XmlNode desktopRoot = xmlDoc.CreateElement("DesktopRoot");
            if (DesktopRoot)
                desktopRoot.InnerText = "1";
            else
                desktopRoot.InnerText = "0";
            serverNode.AppendChild(desktopRoot);
            XmlNode serverEnabled = xmlDoc.CreateElement("Enabled");
            if (ServerEnabled)
                serverEnabled.InnerText = "1";
            else
                serverEnabled.InnerText = "0";
            serverNode.AppendChild(serverEnabled);
            XmlNode shortcutsAppendAlias = xmlDoc.CreateElement("ShortcutsAppendAlias");
            if (ShortcutsAppendAlias)
                shortcutsAppendAlias.InnerText = "1";
            else
                shortcutsAppendAlias.InnerText = "0";
            serverNode.AppendChild(shortcutsAppendAlias);
            XmlNode removeShortcuts = xmlDoc.CreateElement("RemoveShortcuts");
            if (RemoveShortcuts)
                removeShortcuts.InnerText = "1";
            else
                removeShortcuts.InnerText = "0";
            serverNode.AppendChild(removeShortcuts);
            XmlNode shortcutsRemoved = xmlDoc.CreateElement("ShortcutsRemoved");
            if (ShortcutsRemoved)
                shortcutsRemoved.InnerText = "1";
            else
                shortcutsRemoved.InnerText = "0";
            serverNode.AppendChild(shortcutsRemoved);
            XmlNode removeServer = xmlDoc.CreateElement("RemoveServer");
            if (RemoveServer)
                removeServer.InnerText = "1";
            else
                removeServer.InnerText = "0";
            serverNode.AppendChild(removeServer);
            XmlNode showNotifications = xmlDoc.CreateElement("ShowNotifications");
            if (ShowNotifications)
                showNotifications.InnerText = "1";
            else
                showNotifications.InnerText = "0";
            serverNode.AppendChild(showNotifications);
            XmlNode localizeShortcuts = xmlDoc.CreateElement("LocalizeShortcuts");
            if (LocalizeShortcuts)
                localizeShortcuts.InnerText = "1";
            else
                localizeShortcuts.InnerText = "0";
            serverNode.AppendChild(localizeShortcuts);
            XmlNode userName = userName = xmlDoc.CreateElement("UserName");
            userName.InnerText = UserName;
            serverNode.AppendChild(userName);
            XmlNode alias = xmlDoc.CreateElement("Alias");
            alias.InnerText = Alias;
            serverNode.AppendChild(alias);
            XmlNode domain = domain = xmlDoc.CreateElement("Domain");
            domain.InnerText = Domain;
            serverNode.AppendChild(domain);
            XmlTextWriter serversFileWriter = new XmlTextWriter(serversFile, null);
            serversFileWriter.Formatting = Formatting.None;
            xmlDoc.Save(serversFileWriter);
            serversFileWriter.Close();
        }
    }
}
