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
using System;
using System.IO;
using System.Threading;
using Elbitin.Applications.RAAS.RAASServer.Models;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    static public class AppListHelper
    {
        private const int APPNAMES_XML_RETRY_COUNT = 200;
        private const int APPNAMES_XML_RETRY_INTERVAL_MS = 200;
        static public AppList ParseAppNames(String appNamesXmlPath)
        {
            AppList appNames = new AppList();
            int count = 0;
            bool exception = false;
            do
            {
                try
                {
                    if (File.Exists(appNamesXmlPath))
                        appNames = AppList.DeserializeXmlFile(appNamesXmlPath);
                    exception = false;
                }
                catch
                {
                    Thread.Sleep(APPNAMES_XML_RETRY_INTERVAL_MS);
                    count++;
                    exception = true;
                }
            } while (exception && count < APPNAMES_XML_RETRY_COUNT);
            return appNames;
        }
    }
}