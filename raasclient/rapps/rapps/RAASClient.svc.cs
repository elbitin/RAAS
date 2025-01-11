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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RAAS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    public class RAASClient : IRAASClient
    {
        public delegate void RemoteAppEventHandler(String application, String arguments);
        public static event RemoteAppEventHandler remoteAppEvent;

        public delegate void ConnectEventHandler();
        public static event ConnectEventHandler connectEvent;

        public delegate void DisconnectEventHandler();
        public static event DisconnectEventHandler disconnectEvent;

        public List<RDPClientInstance> rdpList;
        public void startRemoteApplication(String application, String arguments)
        {
            remoteAppEvent(application, arguments);
            return;
        }

        public void connect()
        {
            connectEvent();
            return;
        }

        public int status()
        {
            foreach (RDPClientInstance rdpInstance in rdpList)
            {
                lock (rdpInstance)
                {
                    try
                    {
                        return rdpInstance.rdpClient.Connected;
                    }
                    catch { }
                }
            }
            return 0;
        }

        public void disconnect()
        {
            disconnectEvent();
            return;
        }
    }
}
