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
using System.Drawing;
using System.Xml;
using System.IO;
using System.Globalization;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.Models
{
    public class CouldNotLoadVisualizationsSettingsException : Exception
    {
        public CouldNotLoadVisualizationsSettingsException()
        {
        }

        public CouldNotLoadVisualizationsSettingsException(string message)
            : base(message)
        {
        }

        public CouldNotLoadVisualizationsSettingsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    class VisualizationsSettings
    {
        public String ServerName { get; set; }
        public bool VisualizationsActive { get; set; }
        public Color MainColor { get; set; }
        public Color TextColor { get; set; }
        public Color LineColor { get; set; }
        public bool Frames { get; set; }
        public bool ConnectionBar { get; set; }
        public double ConnectionBarOpacity { get; set; }

        public VisualizationsSettings(String serverName)
        {
            ServerName = serverName;
            if (!ReadVisualizationsSettings())
                throw new CouldNotLoadVisualizationsSettingsException();
        }

        public bool ReadVisualizationsSettings()
        {
            String visualizationsFile = RAASClientPathHelper.GetVisualizationsConfigFilePath(ServerName);
            if (File.Exists(visualizationsFile))
            {
                // Update visual aids settings according to config
                XmlDocument xmlDoc = new XmlDocument();
                if (!xmlDoc.LoadWithRetries(visualizationsFile))
                    return false;
                bool active = (xmlDoc.SelectSingleNode("Visualizations/Active").InnerText == "1") ? true : false;
                VisualizationsActive = active;
                MainColor = Color.FromArgb(Convert.ToInt32(xmlDoc.SelectSingleNode("Visualizations/MainColor").InnerText, 16));
                TextColor = Color.FromArgb(Convert.ToInt32(xmlDoc.SelectSingleNode("Visualizations/TextColor").InnerText, 16));
                LineColor = Color.FromArgb(Convert.ToInt32(xmlDoc.SelectSingleNode("Visualizations/LineColor").InnerText, 16));
                if (xmlDoc.SelectNodes("/Visualizations/Frames").Count > 0)
                    Frames = true;
                else
                    Frames = false;
                if (xmlDoc.SelectNodes("/Visualizations/ConnectionBar").Count > 0)
                {
                    ConnectionBar = true;
                    ConnectionBarOpacity = Double.Parse(xmlDoc.SelectSingleNode("Visualizations/ConnectionBar/Opacity").InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                else
                {
                    ConnectionBar = false;
                }
                return true;
            }
            return false;
        }

        private static String HexConverter(System.Drawing.Color c)
        {
            return "0x" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public void SaveVisualizationsSettings()
        {
            String visualizationsFile = RAASClientPathHelper.GetAppDataVisualizationsConfigFilePath(ServerName);

            // Create new XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;

            // Store settings in XML
            XmlNode rootNode = xmlDoc.CreateElement("Visualizations");
            xmlDoc.AppendChild(rootNode);
            XmlNode active = xmlDoc.CreateElement("Active");
            if (VisualizationsActive)
                active.InnerText = "1";
            else
                active.InnerText = "0";
            rootNode.AppendChild(active);
            XmlNode mainColor = xmlDoc.CreateElement("MainColor");
            mainColor.InnerText = HexConverter(MainColor);
            rootNode.AppendChild(mainColor);
            XmlNode textColor = xmlDoc.CreateElement("TextColor");
            textColor.InnerText = HexConverter(TextColor);
            rootNode.AppendChild(textColor);
            XmlNode lineColor = xmlDoc.CreateElement("LineColor");
            lineColor.InnerText = HexConverter(LineColor);
            rootNode.AppendChild(lineColor);
            if (ConnectionBar)
            {
                XmlNode connectionBar = xmlDoc.CreateElement("ConnectionBar");
                rootNode.AppendChild(connectionBar);
                XmlNode connectionBarOpacity = xmlDoc.CreateElement("Opacity");
                connectionBarOpacity.InnerText = ConnectionBarOpacity.ToString(CultureInfo.InvariantCulture);
                connectionBar.AppendChild(connectionBarOpacity);
            }
            XmlNode windowOverlays = xmlDoc.CreateElement("WindowOverlays");
            rootNode.AppendChild(windowOverlays);
            XmlNode windowOverlayColor = xmlDoc.CreateElement("Color");
            windowOverlayColor.InnerText = HexConverter(MainColor);
            windowOverlays.AppendChild(windowOverlayColor);
            if (Frames)
            {
                XmlNode frames = xmlDoc.CreateElement("Frames");
                rootNode.AppendChild(frames);
            }
            xmlDoc.Save(visualizationsFile);
        }
    }
}
