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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Elbitin.Applications.RAAS.RAASClient.Models;

namespace Elbitin.Applications.RAAS.RAASClient.RAASClient
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    public class RAASClientService : IRAASClientService
    {
        public Dictionary<String, ServerManager> ServerManagers { get; set; }
        public delegate void RemoteAppEventHandler(String application, String arguments, String server);
        public static event RemoteAppEventHandler RemoteApplicationEvent;
        public delegate void AutostartEventHandler(String arguments, String server);
        public static event AutostartEventHandler AutostartApplicationEvent;
        public delegate void KeepAliveEventHandler(String arguments, String server);
        public static event KeepAliveEventHandler KeepAliveApplicationEvent;
        public delegate void ShortcutsServerEventHandler(String arguments, String server);
        public static event ShortcutsServerEventHandler ShortcutsServerApplicationEvent;
        public delegate void ConnectServerEventHandler(String server);
        public static event ConnectServerEventHandler ConnectServerEvent;
        public delegate void DisconnectServerEventHandler(String server);
        public static event DisconnectServerEventHandler DisconnectServerEvent;

        public RAASClientService(ref Dictionary<String, ServerManager> serverManagers)
        {
            ServerManagers = serverManagers;
        }

        public void StartRemoteApplication(string application, string arguments, string serverName)
        {
            try
            {
                RemoteApplicationEvent.BeginInvoke(application, arguments, serverName, null, null);
            }
            catch
            {
                throw new FaultException("StartRemoteApplication exception");
            }
        }

        public void ConnectServer(string serverName)
        {
            try
            {
                ConnectServerEvent.BeginInvoke(serverName, null, null);
            }
            catch
            {
                throw new FaultException("ConnectServer exception");
            }
        }

        public int GetServerStatus(string serverName)
        {
            try
            {
                if (ServerManagers.Keys.Contains(serverName))
                    return (int)ServerManagers[serverName].ServerStates.GetServerStatus();
                else
                    return (int)ServerStatus.NoContact;
            }
            catch
            {
                throw new FaultException("GetServerStatus exception");
            }
        }

        public string GetServerVersion(string serverName)
        {
            try
            {
                return ServerManagers[serverName].RAASServerVersion;
            }
            catch
            {
                throw new FaultException("GetServerVersion exception");
            }
        }

        public void DisconnectServer(string serverName)
        {
            try
            {
                DisconnectServerEvent.BeginInvoke(serverName, null, null);
            }
            catch
            {
                throw new FaultException("DisconnectServer exception");
            }
        }

        public void StartAutostart(string arguments, string serverName)
        {
            try
            {
                AutostartApplicationEvent.BeginInvoke(arguments, serverName, null, null);
            }
            catch
            {
                throw new FaultException("StartAutostart exception");
            }
        }

        public void StartShortcutsServer(string arguments, string serverName)
        {
            try
            {
                ShortcutsServerApplicationEvent.BeginInvoke(arguments, serverName, null, null);
            }
            catch
            {
                throw new FaultException("StartShortcutsServer exception");
            }
        }

        public void StartKeepAlive(string arguments, string serverName)
        {
            try
            {
                KeepAliveApplicationEvent.BeginInvoke(arguments, serverName, null, null);
            }
            catch
            {
                throw new FaultException("StartKeepAlive exception");
            }
        }
    }
}
