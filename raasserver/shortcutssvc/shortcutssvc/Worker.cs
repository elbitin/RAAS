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
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    public class Worker : BackgroundService, IHostedService, IDisposable
    {
        private FileSystemWatcher newUserWatcher;
        private static readonly object userChangeLock = new System.Object();
        private Dictionary<String, UserShortcutsManager> userShortcutsManagers = new Dictionary<string, UserShortcutsManager>();
        private static List<String> allowedExceptions = new List<string>
        {
        };

        public Worker()
        {
            Log.Logger = RAASServerLogHelper.CreateDefaultRAASServerLogger("shortcutssvc");
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Set file system watchers on each folder which contains shortcuts
            configureUserChangeFileSystemWatchers();

            // Start with rebuilding shortcuts and configuring watchers for all users
            foreach (string userPath in ProfileHelper.GetAllUserProfilePaths())
            {
                try
                {
                    UserShortcutsManager userShortcutsManager = new UserShortcutsManager(userPath);
                    userShortcutsManager.RegisterAllUserShortcuts();
                    userShortcutsManagers[userPath] = userShortcutsManager;
                }
                catch { }
            }

            // Keep program alive
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(200);
            }

            await Task.FromResult(0);
        }

        private static void OnFirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (!allowedExceptions.Contains(e.Exception.GetType().ToString()))
                Log.Debug(e.Exception.ToString() + ":" + e.Exception.Source + ":" + e.Exception.StackTrace);
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

        private void userFolderChange(String updatedUserPath)
        {
            if (Directory.Exists(updatedUserPath) && ProfileHelper.GetAllUserProfilePaths().Contains(updatedUserPath))
            {
                bool alreadyExist = false;
                foreach (String userPath in userShortcutsManagers.Keys)
                {
                    if (updatedUserPath.ToLowerInvariant().StartsWith(userPath.ToLowerInvariant()))
                        alreadyExist = true;
                }
                if (!alreadyExist)
                {
                    UserShortcutsManager userShortcutsManager = new UserShortcutsManager(updatedUserPath);
                    userShortcutsManager.RegisterAllUserShortcuts();
                    userShortcutsManagers[updatedUserPath] = userShortcutsManager;
                }
            }
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

    }
}