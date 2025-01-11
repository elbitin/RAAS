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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    class ServerSettingsHelper
    {
        public static Dictionary<String, ServerSettings> GetServerSettingsFromConfig()
        {
            Dictionary<String, ServerSettings> serverSettings = new Dictionary<String, ServerSettings>();
            String serversFile = RAASClientPathHelper.GetServersConfigFilePath();
            if (File.Exists(serversFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (!xmlDoc.LoadWithRetries(serversFile))
                    throw new CouldNotLoadServerSettingsException();
                foreach (XmlNode node in xmlDoc.SelectNodes("Servers/Server/Name"))
                {
                    serverSettings.Add(node.InnerText, new ServerSettings(node.InnerText, xmlDoc));
                }
            }
            return serverSettings;
        }
    }
}
