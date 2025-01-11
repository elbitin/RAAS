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
using System.IO;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Models;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    public static class ServerHelper
    {
        private const int PING_TIMEOUT = 2000;
        private const String RAAS_CLIENT_NAMESPACE_EXTENSION_PATH = "::{7223EA74-0FAC-4ED0-B347-C01FD2D0FE1A}";
        const int NO_ERROR = 0;
        const int ERROR_ACCESS_DENIED = 5;
        const int ERROR_ALREADY_ASSIGNED = 85;
        const int ERROR_BAD_DEVICE = 1200;
        const int ERROR_BAD_NET_NAME = 67;
        const int ERROR_BAD_PROVIDER = 1204;
        const int ERROR_CANCELLED = 1223;
        const int ERROR_EXTENDED_ERROR = 1208;
        const int ERROR_INVALID_ADDRESS = 487;
        const int ERROR_INVALID_PARAMETER = 87;
        const int ERROR_INVALID_PASSWORD = 1216;
        const int ERROR_MORE_DATA = 234;
        const int ERROR_NO_MORE_ITEMS = 259;
        const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        const int ERROR_NO_NETWORK = 1222;
        const int ERROR_BAD_PROFILE = 1206;
        const int ERROR_CANNOT_OPEN_PROFILE = 1205;
        const int ERROR_DEVICE_IN_USE = 2404;
        const int ERROR_NOT_CONNECTED = 2250;
        const int ERROR_OPEN_FILES = 2401;

        public static bool Contact(String serverName)
        {
            try
            {
                // Ping server
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(serverName, PING_TIMEOUT);
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ConnectShares(ServerSettings serverSettings)
        {
            // Set variables
            String serverName = serverSettings.ServerName;
            String userName = serverSettings.UserName;
            String domain = serverSettings.Domain;
            if (domain == "")
                domain = serverName;
            String password = serverSettings.Password;

            // Disk Access
            Win32Helper.NETRESOURCE nr = new Win32Helper.NETRESOURCE();
            nr.dwType = Win32Helper.ResourceType.RESOURCETYPE_DISK;
            nr.lpRemoteName = "\\\\" + serverName;
            int netReturn;
            netReturn = Win32Helper.WNetUseConnection(IntPtr.Zero, nr, password, userName, (int)(Win32Helper.ResourceConnection.CONNECT_UPDATE_PROFILE | Win32Helper.ResourceConnection.CONNECT_CMD_SAVECRED), null, null, null);
            if (netReturn == NO_ERROR || netReturn == ERROR_ACCESS_DENIED || netReturn == ERROR_INVALID_PASSWORD)
                return true;
            else
                return false;
        }

        public static void UpdateNamespaceExtensionDeletedServer(String serverDisplayName)
        {
            UpdateNamespaceExtension();
            Win32Helper.SHChangeNotify((int)Win32Helper.SHCNE.SHCNE_RMDIR, (int)Win32Helper.SHChangeNotifyFlags.SHCNF_PATHW | (int)Win32Helper.SHChangeNotifyFlags.SHCNF_FLUSHNOWAIT, Path.Combine(RAAS_CLIENT_NAMESPACE_EXTENSION_PATH, serverDisplayName), IntPtr.Zero);
        }

        public static void UpdateNamespaceExtension()
        {
            Win32Helper.SHChangeNotify((int)Win32Helper.SHCNE.SHCNE_UPDATEITEM, (int)Win32Helper.SHChangeNotifyFlags.SHCNF_PATHW, RAAS_CLIENT_NAMESPACE_EXTENSION_PATH, IntPtr.Zero);
        }

        public static void UpdateNamespaceExtension(String serverDisplayName)
        {
            UpdateNamespaceExtension();
            Win32Helper.SHChangeNotify((int)Win32Helper.SHCNE.SHCNE_UPDATEDIR, (int)Win32Helper.SHChangeNotifyFlags.SHCNF_PATHW, Path.Combine(RAAS_CLIENT_NAMESPACE_EXTENSION_PATH, serverDisplayName), IntPtr.Zero);
        }
    }
}