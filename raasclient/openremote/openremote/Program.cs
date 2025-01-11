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
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Linq;
using Elbitin.Applications.RAAS.RAASClient.OpenRemote.RAASClientServiceRef;
using Serilog;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;
using System.Globalization;

namespace Elbitin.Applications.RAAS.RAASClient.OpenRemote
{

    public class Program : Object
    {
        private static String[] argsStored;
        private Form fparent = new Form();
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
            Log.Logger = RAASClientLogHelper.CreateDefaultRAASClientLogger("openremote");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            argsStored = args;
            BackgroundWorker bw = StartBackgroundWorker();
            KeepAliveWhileBusy(bw);
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

        private static void KeepAliveWhileBusy(BackgroundWorker bw)
        {
            do
            {
                Thread.Sleep(50);
            } while (bw.IsBusy);
        }

        private static BackgroundWorker StartBackgroundWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(DoWork);
            bw.RunWorkerAsync();
            return bw;
        }

        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            String[] args = argsStored;
            if (args.Count() > 0)
            {
                // Initialize variables from arguments
                string uncPath = SharesPathHelper.GetUNCPath(args[0]);
                List<String> argsList = new List<string>();
                String suppliedArguments = "";
                String serverName = "";
                bool serverSupplied = false;
                for (int i = 0; i < args.Count() - 1; i++)
                {
                    if (args[i].StartsWith("-server"))
                    {
                        serverName = args[++i];
                        serverSupplied = true;
                    }
                    else if (args[i].StartsWith("-args"))
                    {
                        suppliedArguments = args[++i];
                    }
                    else
                        argsList.Add(args[i]);
                }
                String path = uncPath;
                if (!serverSupplied)
                {
                    serverName = SharesPathHelper.GetServerNameFromPath(uncPath);
                    path = SharesPathHelper.GetServerPath(uncPath, serverName);
                }

                // Try to start remote application
                try
                {
                    // Create raas service client

                    RAASClientServiceClient raasServiceClient = new RAASClientServiceClient();
                    raasServiceClient.Open();

                    // Start remote application
                    raasServiceClient.StartRemoteApplication(path, suppliedArguments, serverName);
                }
                catch
                {
                    MessageBox.Show(new Form() { TopMost = true }, Properties.Resources.RAASClientService_NoCommunicationMessage, Properties.Resources.Program_ErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
        }
    }
}
