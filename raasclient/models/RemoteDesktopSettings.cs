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

    public class CouldNotLoadRemoteDesktopSettingsException : Exception
    {
        public CouldNotLoadRemoteDesktopSettingsException()
        {
        }

        public CouldNotLoadRemoteDesktopSettingsException(string message)
            : base(message)
        {
        }

        public CouldNotLoadRemoteDesktopSettingsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class RemoteDesktopSettings
    {
        public String ServerName { get; set; } = "";
        public int AuthenticationLevel { get; set; } = 1;
        public bool RedirectClipboard { get; set; } = false;
        public bool RedirectUsb { get; set; } = false;
        public bool RedirectPrinters { get; set; } = false;
        public bool ConnectionBar { get; set; } = false;
        public bool PinConnectionBar { get; set; } = false;
        public bool Fullscreen { get; set; } = false;
        public bool AllMonitors { get; set; } = false;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        const int DEFAULT_WIDTH = 800;
        const int DEFAULT_HEIGHT = 600;
        public RemoteDesktopSettings(String serverName)
        {
            ServerName = serverName;
            if (!ReadRemoteDesktopServerSettings(serverName))
            {
                if (!ReadDefaultRemoteDesktopSettings())
                    throw new CouldNotLoadRemoteDesktopSettingsException();
            }
        }

        public RemoteDesktopSettings(String serverName, XmlDocument sourceXmlDoc)
        {
            ServerName = serverName;
            if (!ReadRemoteDesktopSettings(sourceXmlDoc))
                throw new CouldNotLoadRemoteDesktopSettingsException();
        }

        public bool ReadDefaultRemoteDesktopSettings()
        {
            String remoteDesktopSettingsFile = RAASClientPathHelper.GetDefaultRemoteDesktopConfigFilePath();
            if (File.Exists(remoteDesktopSettingsFile))
            {
                XmlDocument sourceXmlDoc = new XmlDocument();
                if (sourceXmlDoc.LoadWithRetries(remoteDesktopSettingsFile))
                {
                    if (ReadRemoteDesktopSettings(sourceXmlDoc))
                        return true;
                }
            }
            return false;
        }

        public bool ReadRemoteDesktopServerSettings(String serverName)
        {
            String remoteDesktopSettingsFile = RAASClientPathHelper.GetRemoteDesktopConfigFilePath(serverName);
            if (File.Exists(remoteDesktopSettingsFile))
            {
                XmlDocument sourceXmlDoc = new XmlDocument();
                if (sourceXmlDoc.LoadWithRetries(remoteDesktopSettingsFile))
                {
                    if (ReadRemoteDesktopSettings(sourceXmlDoc))
                        return true;
                }
            }
            return ReadDefaultRemoteDesktopSettings();
        }

        public bool ReadRemoteDesktopSettings(XmlDocument sourceXmlDoc)
        {
            XmlNode node = sourceXmlDoc.SelectSingleNode("RemoteDesktopSettings");
            ReadRemoteDesktopSettingsFromNode(node);
            return true;
        }

        private void ReadRemoteDesktopSettingsFromNode(XmlNode node)
        {
            // Update server settings according to config
            XmlNode authenticationLevel = node.SelectSingleNode("AuthenticationLevel");
            if (authenticationLevel != null)
                AuthenticationLevel = Convert.ToInt32(authenticationLevel.InnerText);
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
            XmlNode fullscreen = node.SelectSingleNode("Fullscreen");
            if (fullscreen != null && fullscreen.InnerText == "1")
                Fullscreen = true;
            else
                Fullscreen = false;
            XmlNode allMonitors = node.SelectSingleNode("AllMonitors");
            if (allMonitors != null && allMonitors.InnerText == "1")
                AllMonitors = true;
            else
                AllMonitors = false;
            XmlNode connectionBar = node.SelectSingleNode("ConnectionBar");
            if (connectionBar != null && connectionBar.InnerText == "1")
                ConnectionBar = true;
            else
                ConnectionBar = false;
            XmlNode pinConnectionBar = node.SelectSingleNode("PinConnectionBar");
            if (pinConnectionBar != null && pinConnectionBar.InnerText == "1")
                PinConnectionBar = true;
            else
                PinConnectionBar = false;
            XmlNode width = node.SelectSingleNode("Width");
            if (width != null)
                Width = Convert.ToInt32(width.InnerText);
            else
                Width = DEFAULT_WIDTH;
            XmlNode height = node.SelectSingleNode("Height");
            if (height != null)
                Height = Convert.ToInt32(height.InnerText);
            else
                Height = DEFAULT_HEIGHT;
        }

        public void SaveRemoteDesktopSettings()
        {
            String remoteDesktopSettingsFile = RAASClientPathHelper.GetRemoteDesktopConfigFilePath(ServerName);
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode;

            // Store settings in XML
            rootNode = xmlDoc.CreateElement("RemoteDesktopSettings");
            xmlDoc.AppendChild(rootNode);
            XmlNode authenticationLevel = xmlDoc.CreateElement("AuthenticationLevel");
            authenticationLevel.InnerText = AuthenticationLevel.ToString();
            rootNode.AppendChild(authenticationLevel);
            XmlNode clipboard = xmlDoc.CreateElement("RedirectClipboard");
            if (RedirectClipboard)
                clipboard.InnerText = "1";
            else
                clipboard.InnerText = "0";
            rootNode.AppendChild(clipboard);
            XmlNode usb = xmlDoc.CreateElement("RedirectDevices");
            if (RedirectUsb)
                usb.InnerText = "1";
            else
                usb.InnerText = "0";
            rootNode.AppendChild(usb);
            XmlNode printers = xmlDoc.CreateElement("RedirectPrinters");
            if (RedirectPrinters)
                printers.InnerText = "1";
            else
                printers.InnerText = "0";
            rootNode.AppendChild(printers);
            XmlNode fullscreen = xmlDoc.CreateElement("Fullscreen");
            if (Fullscreen)
                fullscreen.InnerText = "1";
            else
                fullscreen.InnerText = "0";
            rootNode.AppendChild(fullscreen);
            XmlNode allMonitors = xmlDoc.CreateElement("AllMonitors");
            if (AllMonitors)
                allMonitors.InnerText = "1";
            else
                allMonitors.InnerText = "0";
            rootNode.AppendChild(allMonitors);
            XmlNode connectionBar = xmlDoc.CreateElement("ConnectionBar");
            if (ConnectionBar)
                connectionBar.InnerText = "1";
            else
                connectionBar.InnerText = "0";
            rootNode.AppendChild(connectionBar);
            XmlNode pinConnectionBar = xmlDoc.CreateElement("PinConnectionBar");
            if (PinConnectionBar)
                pinConnectionBar.InnerText = "1";
            else
                pinConnectionBar.InnerText = "0";
            rootNode.AppendChild(pinConnectionBar);
            XmlNode width = xmlDoc.CreateElement("Width");
            if (Width != 0)
                width.InnerText = Convert.ToString(Width);
            else
                width.InnerText = Convert.ToString(DEFAULT_WIDTH);
            rootNode.AppendChild(width);
            XmlNode height = xmlDoc.CreateElement("Height");
            if (Height != 0)
                height.InnerText = Convert.ToString(Height);
            else
                height.InnerText = Convert.ToString(DEFAULT_WIDTH);
            rootNode.AppendChild(height);
            XmlTextWriter remoteDesktopFileWriter = new XmlTextWriter(remoteDesktopSettingsFile, null);
            remoteDesktopFileWriter.Formatting = Formatting.None;
            xmlDoc.Save(remoteDesktopFileWriter);
            remoteDesktopFileWriter.Close();
        }
    }
}
