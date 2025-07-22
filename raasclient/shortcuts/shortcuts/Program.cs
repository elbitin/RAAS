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
using System.Windows.Forms;
using System.Linq;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Collections.Generic;
using Serilog;

namespace Elbitin.Applications.RAAS.RAASClient.Shortcuts
{
    class Program
    {
        static System.Threading.Mutex shortcutsMutex;
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
        static void Main(String[] args)
        {
            Log.Logger = RAASClientLogHelper.CreateDefaultRAASClientLogger("shortcuts");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            // Create RAAS configuration folder if it does not exist
            String raasClientPath = RAASClientPathHelper.GetAppDataRAASClientPath();

            // Determine if an uninstall should take place
            bool uninstall = false;
            if (args.Contains("-uninstall"))
                uninstall = true;

            // Determine if operation is to remove shortcuts for desired servers
            bool remove = false;
            if (args.Contains("-remove"))
                remove = true;

            // Determine if operation is to update shortcuts for desired servers
            bool update = false;
            if (args.Contains("-update"))
                update = true;

            // Return if another instance is running and operation is not remove or uninstall
            if (!uninstall && !remove)
            {
                bool result = false;
                shortcutsMutex = new System.Threading.Mutex(true, "raasclientshortcuts", out result);
                if (!result)
                    return;
            }

            // Kill other instances on uninstall
            if (uninstall)
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)))
                    if (p.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                    {
                        ProcessHelper.CloseProcess(p);
                        p.WaitForExit();
                    }

            // Initialize application states
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create main form for application
            Application.Run(new ShortcutsForm(uninstall, remove, update));
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