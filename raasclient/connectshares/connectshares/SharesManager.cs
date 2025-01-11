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
﻿using Microsoft.Win32;
using Elbitin.Applications.RAAS.RAASClient.Models;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using System;
using System.Timers;

namespace Elbitin.Applications.RAAS.RAASClient.ConnectShares
{
    class SharesManager : IDisposable
    {
        public ServerSettings ServerSettings { get; set; }
        private System.Timers.Timer updateSharesTimer = new System.Timers.Timer();
        private bool serverSharesValidAttempt = false;
        private bool running = true;
        private SessionSwitchEventHandler sessionSwitchEventHandler;
        private const int UPDATESHARESTIMER_INTERVAL_MS = 1000;

        public SharesManager(ServerSettings serverSettings)
        {
            ServerSettings = serverSettings;
            RegisterSessionSwitchEventHandler();
            PrepareUpdateSharesTimer();
            try
            {
                ConnectShares();
            }catch { }
        }

        private void RegisterSessionSwitchEventHandler()
        {
            sessionSwitchEventHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            SystemEvents.SessionSwitch += sessionSwitchEventHandler;
        }

        private void PrepareUpdateSharesTimer()
        {
            updateSharesTimer.Interval = UPDATESHARESTIMER_INTERVAL_MS;
            updateSharesTimer.AutoReset = false;
            updateSharesTimer.Elapsed += UpdateSharesTimer_Elapsed;
            updateSharesTimer.Enabled = true;
            updateSharesTimer.Start();
        }

        public void Dispose()
        {
            running = false;
            SystemEvents.SessionSwitch -= sessionSwitchEventHandler;
            updateSharesTimer.Stop();
        }

        private void UpdateSharesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateSharesTimer.Stop();
            if (running)
            {
                try
                {
                    if (!serverSharesValidAttempt)
                        ConnectShares();
                }
                catch { }
                updateSharesTimer.Start();
            }
        }


        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                serverSharesValidAttempt = false;
            }
        }

        private void ConnectShares()
        {
            // Ping server
            bool contact = ServerHelper.Contact(ServerSettings.ServerName);

            // Try to get disk access to server shares if contact successful
            if (contact)
            {
                serverSharesValidAttempt = ServerHelper.ConnectShares(ServerSettings);
            }
        }
    }
}
