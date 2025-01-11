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
using System.ServiceModel;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        AddressFilterMode = AddressFilterMode.Any)]
    public class RemoteAppsService : IRemoteAppsService
    {
        public IRemoteAppsChange ServiceCallback { get; set; }
        public bool Connected { get; set; } = false;
        public bool HasReportedConnection { get; set; } = false;
        public bool ClientHasSubscribed { get; set; } = false;
        public delegate void RemoteAppEventHandler(String application, String arguments);
        public static event RemoteAppEventHandler RemoteApplicationEvent;
        public delegate void ConnectEventHandler();
        public static event ConnectEventHandler ConnectEvent;
        public delegate void DisconnectEventHandler();
        public static event DisconnectEventHandler DisconnectEvent;

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public void Subscribe()
        {
            try
            {
                // Store callback
                ServiceCallback = OperationContext.Current.GetCallbackChannel<IRemoteAppsChange>();

                // Indicate that a subscription has occured
                ClientHasSubscribed = true;

                // Report connection to service client if needed
                if (Connected && !HasReportedConnection)
                {
                    try
                    {
                        ServiceCallback.Connected();
                        HasReportedConnection = true;
                    }
                    catch { }
                }
            }
            catch
            {
                throw new FaultException("Subscribe exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Unsubscribe()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    // Remove callback
                    ServiceCallback = null;
                }
            }
            catch
            {
                throw new FaultException("Unsubscribe exception");
            }
        }

        public void StartRemoteApplication(string application, string arguments)
        {
            try
            {
                RemoteApplicationEvent(application, arguments);
            }
            catch
            {
                throw new FaultException("StartRemoteApplication exception");
            }
        }

        public void Connect()
        {
            try
            {
                ConnectEvent();
            }
            catch
            {
                throw new FaultException("Connect exception");
            }
        }

        public int GetStatus()
        {
            if (Connected)
                return 1;
            else
                return 0;
        }

        public void Disconnect()
        {
            try
            {
                DisconnectEvent();
            }
            catch
            {
                throw new FaultException("Disconnect exception");
            }
        }
    }
}
