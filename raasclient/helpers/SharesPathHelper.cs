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
using Elbitin.Applications.RAAS.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    class SharesPathHelper
    {
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // Look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = Win32Helper.WNetGetConnection(originalPath.Substring(0, 2), sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);
                        string path = Path.GetFullPath(originalPath).Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            return originalPath;
        }

        public static String GetServerNameFromPath(string uncPath)
        {
            String serverName = uncPath.Split('\\')[2];
            return serverName;
        }

        public static String GetServerPath(string uncPath, string serverName)
        {
            String path = uncPath;

            // Get share from unc path
            String share = uncPath.Split('\\')[3];

            String sharesFilePath = RAASClientPathHelper.GetSharesFilePath(serverName);

            // Find server local path for share
            Shares shares = Shares.DeserializeXmlFileWithRetries(sharesFilePath);
            IEnumerable<SharesDrive> matchedDrive =
                from drive in shares.Drives
                where drive.Share.ToLowerInvariant() == share.ToLowerInvariant()
                select drive;
            if (matchedDrive.Count() != 0)
                path = matchedDrive.First().Name + uncPath.Substring(serverName.Length + share.Length + 4, uncPath.Length - (serverName.Length + share.Length + 4));
            return path;
        }

        public static String GetLocalPath(string serverPath, string serverName)
        {
            String path = serverPath;

            // Get share from unc path
            String serverDrive = serverPath.Split('\\')[0];

            String sharesFilePath = RAASClientPathHelper.GetSharesFilePath(serverName);

            // Find server local path for share
            Shares shares = Shares.DeserializeXmlFileWithRetries(sharesFilePath);
            IEnumerable<SharesDrive> matchedDrive =
                from drive in shares.Drives
                where drive.Name.ToLowerInvariant().StartsWith(serverDrive.ToLowerInvariant())
                select drive;
            if (matchedDrive.Count() != 0)
                path = "\\\\" + serverName + "\\" + matchedDrive.First().Share + serverPath.Substring(serverDrive.Length, serverPath.Length - (serverDrive.Length));
            return path;
        }
    }
}
