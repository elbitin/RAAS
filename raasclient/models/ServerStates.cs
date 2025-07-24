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
﻿namespace Elbitin.Applications.RAAS.RAASClient.Models
{
    public enum ServerStatus
    {
        NoContact,
        Contact,
        Available,
        Connected
    };

    public class ServerStates
    {
        public bool RAASServerServiceSubscribed { get; set; } = false;
        public bool RAASServerSharesConnected { get; set; } = false;
        public bool RAASServerSharesShouldReload { get; set; } = false;
        public bool RAASServerCanLogOff { get; set; } = false;
        public bool RAASServerCanReboot { get; set; } = false;
        public bool RAASServerContact { get; set; } = false;
        public bool RemoteApplicationsServiceConnected { get; set; } = false;
        public bool RemoteApplicationsProcessRunning { get; set; } = false;
        public bool RemoteApplicationsConnected { get; set; } = false;
        public bool RemoteApplicationsConnectedOnLock { get; set; } = false;
        public bool RemoteApplicationsConnectedSinceUnlock { get; set; } = false;
        public bool RemoteApplicationsShouldClose { get; set; } = false;
        public bool DisconnectedOnNextExit { get; set; } = false;
        public bool FailedOnNextExit { get; set; } = false;
        public bool HasConnectedSinceUnlock { get; set; } = false;
        public bool HasFailedSinceUnlock { get; set; } = false;
        public bool UserInitiatedLastDisconnect { get; set; } = false;
        public bool AutoConnectStarted { get; set; } = false;
        public bool SessionLocked { get; set; } = false;
        public bool RAASServerServiceSubscribing { get; set; } = false;
        private int failCount = 0;
        private const int DISCONNECTED_AFTER_FAILCOUNT = 3;

        public ServerStatus GetServerStatus()
        {
            if (RemoteApplicationsConnected)
                return ServerStatus.Connected;
            else if (RAASServerServiceSubscribed)
                return ServerStatus.Available;
            else if (RAASServerContact)
                return ServerStatus.Contact;
            else
                return ServerStatus.NoContact;
        }

        public bool ShouldRestartTimers()
        {
            return !SessionLocked;
        }

        public bool ShouldUpdateRemoteApplicationsStatus()
        {
            return !RemoteApplicationsConnected && 
                RemoteApplicationsServiceConnected &&
                RemoteApplicationsProcessRunning &&
                !RemoteApplicationsShouldClose;
        }

        public bool ShouldFailOnTimeOut(ServerSettings serverSettings)
        {
            return !HasFailedSinceUnlock &&
                !UserInitiatedLastDisconnect &&
                !RemoteApplicationsConnectedSinceUnlock &&
                !RemoteApplicationsProcessRunning &&
                (serverSettings.AutoConnect || (serverSettings.AutoReconnect && RemoteApplicationsConnectedOnLock));
        }

        public bool ShouldConnectBeforeTimeout(ServerSettings serverSettings)
        {
            return !HasFailedSinceUnlock &&
                !AutoConnectStarted &&
                RAASServerServiceSubscribed &&
                !RemoteApplicationsConnectedSinceUnlock &&
                (serverSettings.AutoConnect || (serverSettings.AutoReconnect && RemoteApplicationsConnectedOnLock) && !UserInitiatedLastDisconnect) &&
                !RemoteApplicationsProcessRunning &&
                !SessionLocked;
        }

        public void SessionUnlockStartedUpdate()
        {
            SessionLocked = false;
            RAASServerServiceSubscribed = false;
        }

        public void SessionUnlockDoneUpdate(ServerSettings serverSettings)
        {
            HasFailedSinceUnlock = false;
            RemoteApplicationsConnectedSinceUnlock = false;
            RAASServerSharesConnected = false;
            AutoConnectStarted = false;
        }

        public void SessionLockStartedUpdate()
        {
            SessionLocked = true;

            // Do not show failed messages for unconnected raas clients
            FailedOnNextExit = false;
            DisconnectedOnNextExit = false;

            // Remember what states raas client have
            if (RemoteApplicationsConnected)
                RemoteApplicationsConnectedOnLock = true;
            else
                RemoteApplicationsConnectedOnLock = false;
        }

        public void SessionLockDoneUpdate()
        {

        }

        public void ConnectedUpdate()
        {
            RemoteApplicationsConnected = true;
            HasConnectedSinceUnlock = true;
            RemoteApplicationsConnectedSinceUnlock = true;
            DisconnectedOnNextExit = true;
            HasFailedSinceUnlock = false;
            FailedOnNextExit = false;
        }

        public void ConnectStartedUpdate()
        {
            UserInitiatedLastDisconnect = false;
            DisconnectedOnNextExit = false;
            FailedOnNextExit = false;
        }

        public void RemoteApplicationsStartedUpdate()
        {
            RemoteApplicationsProcessRunning = true;
            FailedOnNextExit = true;
            DisconnectedOnNextExit = false;
            HasConnectedSinceUnlock = false;
        }

        public void ResetRemoteApplicationsUpdate()
        {
            HasConnectedSinceUnlock = false;
            RemoteApplicationsServiceConnected = false;
        }

        public void ContactUpdate(bool contact)
        {
            if (!contact && failCount < DISCONNECTED_AFTER_FAILCOUNT)
            {
                failCount++;
                return;
            }
            failCount = 0;
            if (contact)
                RAASServerContact = true;
            else
                RAASServerContact = false;
        }

        public void RemoteApplicationsProcessNotRunning()
        {
            RemoteApplicationsProcessRunning = false;
            RemoteApplicationsConnected = false;
        }
    }
}