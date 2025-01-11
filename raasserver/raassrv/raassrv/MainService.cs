/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Server.
 *
 * RAAS Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Server. If not, see <https://www.gnu.org/licenses/>.
 */
﻿// Copyright (c) Elbitin
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using System.Management;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using System.Threading;
using System;
using Serilog;
using System.IO;

namespace Elbitin.Applications.RAAS.RAASServer.RAASSvr
{
    class MainService : ServiceBase
    {
        private bool running = false;
        private ServiceHost host;
        private Thread mainThread;
        private FileSystemWatcher newUserWatcher;
        private static readonly object userChangeLock = new System.Object();
        private static List<String> allowedExceptions = new List<string> {
            "System.ServiceModel.CommunicationObjectFaultedException",
            "System.ServiceModel.CommunicationObjectAbortedException",
            "System.Net.Sockets.SocketException",
            "System.ServiceModel.CommunicationException",
        };

        static void Main(string[] args)
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] { new MainService() };
            ServiceBase.Run(servicesToRun);
        }

        public MainService()
        {
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            Log.Logger = RAASServerLogHelper.CreateDefaultRAASServerLogger("raassrv");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            running = true;
            host = new ServiceHost(typeof(RAASServerService));
            host.Open();
            ThreadStart mainThreadStart = new ThreadStart(this.ServiceMain);
            mainThread = new Thread(mainThreadStart);
            mainThread.Start();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            AppDomain.CurrentDomain.FirstChanceException -= OnFirstChanceException;
            Log.CloseAndFlush();
            running = false;
            host.Close();
            base.OnStop();
        }

        private static void OnFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs eventArgs)
        {
            if (!allowedExceptions.Contains(eventArgs.Exception.GetType().ToString()))
                Log.Debug(eventArgs.Exception.ToString());
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    RAASServerService.SessionChange();
                    break;

                case SessionChangeReason.SessionLogoff:
                    RAASServerService.SessionChange();
                    break;
            }
        }

        private void VolumeChangeEventArrived(object sender, EventArrivedEventArgs e)
        {
            UpdateShares();
            CreateSharesXml();
            RAASServerService.ShareChange();
        }

        private static void UpdateShares()
        {
            List<string> driveShareNames = SharesHelper.UpdateRAASServerShares();
            SharesHelper.RemoveUnusedRAASServerShares(driveShareNames);
        }

        private void configureUserChangeFileSystemWatchers()
        {
            String usersFolder = ProfileHelper.GetUsersFolderPath();
            newUserWatcher = new FileSystemWatcher(usersFolder);
            newUserWatcher.IncludeSubdirectories = false;
            newUserWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            newUserWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_OnNewUser);
            newUserWatcher.Deleted += new FileSystemEventHandler(fileSystemWatcher_OnNewUser);
            newUserWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_OnNewUser);
            newUserWatcher.Renamed += new RenamedEventHandler(fileSystemWatcher_OnRenamedUser);
            newUserWatcher.EnableRaisingEvents = true;
        }

        private void fileSystemWatcher_OnRenamedUser(object sender, RenamedEventArgs e)
        {
            new Thread(delegate () {
                onRenamedUser(sender, e);
            }).Start();
        }

        private void onRenamedUser(object sender, RenamedEventArgs e)
        {
            lock (userChangeLock)
            {
                try
                {
                    userFolderChange(e.FullPath);
                }
                catch { }
            }
        }

        private void fileSystemWatcher_OnNewUser(object sender, FileSystemEventArgs e)
        {
            new Thread(delegate () {
                onNewUser(sender, e);
            }).Start();
        }

        private void onNewUser(object sender, FileSystemEventArgs e)
        {
            lock (userChangeLock)
            {
                try
                {
                    userFolderChange(e.FullPath);
                }
                catch { }
            }
        }

        private void userFolderChange(String updatedUserPath)
        {
            CreateSharesXml();
        }

        private void CreateSharesXml()
        {
            try
            {
                foreach (string userPath in ProfileHelper.GetAllUserProfilePaths())
                {
                    String userName = ProfileHelper.GetUserFromProfilePath(userPath);
                    String shareXml = SharesHelper.GetSharesXml(userName);
                    System.IO.File.WriteAllText(RAASServerPathHelper.GetUserRAASServerSharesXMLPath(userPath), shareXml);
                }
            }
            catch { }
        }

        protected void ServiceMain()
        {
            if (RAASServerFeatureHelper.SharesInstalled())
            {
                // Initialize shares
                UpdateShares();
                CreateSharesXml();

                // Set file system watchers on each folder which should be updated with share xml
                configureUserChangeFileSystemWatchers();

                // Watch for changes in volumes
                while (running)
                {
                    try
                    {
                        ManagementEventWatcher watcher = new ManagementEventWatcher();
                        WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");
                        watcher.EventArrived += new EventArrivedEventHandler(new EventArrivedEventHandler(VolumeChangeEventArrived));
                        watcher.Query = query;
                        watcher.Start();
                        watcher.WaitForNextEvent();
                    }
                    catch { }
                }
            }
            else
            {
                // Keep program alive
                while (running)
                {
                    Thread.Sleep(200);
                }
            }
        }
    }
}
