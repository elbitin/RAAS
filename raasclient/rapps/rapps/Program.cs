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
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    public class Program
    {
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
        static void Main(string[] args)
        {
            // Kill all instances of current application if requested
            if (args.Contains("-kill"))
            {
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)))
                    if (p.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                        p.Kill();
                return;
            }

            Log.Logger = RAASClientLogHelper.CreateDefaultRAASClientLogger("rapps");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            // Start application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RemoteAppsForm clientForm;
            if (args.Count() > 0)
                clientForm = new RemoteAppsForm(args[0]);
            else
                return;
            Application.Run(clientForm);
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