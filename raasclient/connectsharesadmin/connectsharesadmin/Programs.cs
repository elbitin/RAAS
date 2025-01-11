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
using System.Collections.Generic;
using System.Windows.Forms;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Serilog;

namespace Elbitin.Applications.RAAS.RAASClient.ConnectShares
{
    static class Program
    {
        static System.Threading.Mutex connectSharesAdimMutex;
        private static List<String> allowedExceptions = new List<string> {
                "System.ServiceModel.EndpointNotFoundException",
                "System.Net.Sockets.SocketException",
                "System.Reflection.TargetInvocationException",
                "System.Net.NetworkInformation.PingException",
                "System.ServiceModel.CommunicationException",
                "System.IO.IOException",
                "System.IO.PipeException",
                "System.ServiceModel.CommunicationObjectFaultedException",
                "System.ServiceModel.CommunicationObjectAbortedException",
            };

        [STAThread]
        static void Main()
        {
            bool result;
            connectSharesAdimMutex = new System.Threading.Mutex(true, "raasclientconnectsharesadmin", out result);
            if (!result)
                return;
            if (!AdminRightsHelper.IsAdministrator())
                return;
            Log.Logger = RAASClientLogHelper.CreateDefaultRAASClientLogger("connectsharesadmin");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConnectSharesForm());
        }

        private static void OnFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs eventArgs)
        {
            if (!allowedExceptions.Contains(eventArgs.Exception.GetType().ToString()))
                Log.Debug(eventArgs.Exception.ToString());
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            Log.CloseAndFlush();
        }
    }
}
