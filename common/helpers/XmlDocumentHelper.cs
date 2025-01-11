/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS.
 *
 * RAAS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    public static class XmlDocumentHelper
    {
        private const int DEFAULT_RETRY_LOAD_XML_COUNT = 30;
        private const int DEFAULT_RETRY_LOAD_XML_INTERVAL_MS = 50;

        public static bool LoadWithRetries(this XmlDocument xmlDoc, String path)
        {
            return LoadWithRetries(xmlDoc, path, DEFAULT_RETRY_LOAD_XML_COUNT, DEFAULT_RETRY_LOAD_XML_INTERVAL_MS);
        }

        public static bool LoadWithRetries(this XmlDocument xmlDoc, String path, int retryCount, int retryIntervalMs)
        {
            bool xmlDocLoaded = false;
            for (int i = 0; i < retryCount; i++)
                try
                {
                    xmlDoc.Load(path);
                    xmlDocLoaded = true;
                    break;
                }
                catch
                {
                    Thread.Sleep(retryIntervalMs);
                }
            return xmlDocLoaded;
        }
    }
}
