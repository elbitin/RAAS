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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.IO;
using System.Security.Principal;
using System.Drawing;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using System.Threading;

namespace Elbitin.Applications.RAAS.RAASServer.RAASSvr
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerSession)]
    public class RAASServerService : IRAASServerService, IDisposable
    {
        private IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        private IRAASServerChange serviceCallback;
        private FileSystemWatcher xmlServersWatcher;
        private static Dictionary<String, FileSystemWatcher> fileSystemWatchers = new Dictionary<String, FileSystemWatcher>();
        private static event loggedInDelegate loggedInEvent;
        private static event shareChangeDelegate shareChangeEvent;
        private static event shortcutsChangeDelegate shortcutsXMLChangeEvent;
        private loggedInDelegate loggedIn;
        private shareChangeDelegate shareChange;
        private shortcutsChangeDelegate shortcutsChange;
        private bool loggedInEventSubscribed = false;
        private bool shareChangeEventSubscribed = false;
        private bool shortcutsChangeEventSubscribed = false;
        private delegate void shortcutsXmlChangeEventHandler(object sender, EventArgs e);
        private const int LOGGEDINTIMER_INTERVAL_MS = 250;
        private const int SHARESTIMER_INTERVAL_MS = 1000;
        private String lastShortcutsXml = "";
        private String RAAS_SHORTCUTS_SERVICE_NAME = "RAASShortcutsService";
        private const String COMPANY_NAME = "Elbitin";
        private const String PROGRAM_NAME = "RAAS Server";
        private const String VERSION_REGISTRY_STRING = "Version";

        public static void SessionChange()
        {
            try
            {
                loggedInEvent.Invoke();
            }
            catch { }
        }

        public static void ShareChange()
        {
            try
            {
                shareChangeEvent.Invoke();
            }
            catch { }
        }

        public void Dispose()
        {
            if (loggedInEventSubscribed)
            {
                loggedInEvent -= loggedIn;
            }
            if (shareChangeEventSubscribed)
            {
                shareChangeEvent -= shareChange;
            }
            if (shortcutsChangeEventSubscribed)
            {
                shortcutsXMLChangeEvent -= shortcutsChange;
            }
            // TODO: filesystemWatchers keeps all users paths, need to contain only active users paths
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public void Subscribe()
        {
            try
            {
                // Create and add event subscribers if not added
                if (!loggedInEventSubscribed)
                {
                    loggedIn = new loggedInDelegate(OnLoggedIn);
                    loggedInEvent += loggedIn;
                    loggedInEventSubscribed = true;
                }
                if (!shareChangeEventSubscribed)
                {
                    shareChange = new shareChangeDelegate(onShareChange);
                    shareChangeEvent += shareChange;
                    shareChangeEventSubscribed = true;
                }
                if (!shortcutsChangeEventSubscribed)
                {
                    shortcutsChange = new shortcutsChangeDelegate(onShortcutsChange);
                    shortcutsXMLChangeEvent += shortcutsChange;
                    shortcutsChangeEventSubscribed = true;
                }

                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    // Set service callback
                    serviceCallback = OperationContext.Current.GetCallbackChannel<IRAASServerChange>();

                    // Set paths
                    String shortcutsXmlPath = RAASServerPathHelper.GetCurrentShortcutsXMLPath();

                    // Start filesystemwatchers for user shortcuts file if it does not exist
                    if (!fileSystemWatchers.Keys.Contains(WindowsIdentity.GetCurrent().Name))
                    {
                        lastShortcutsXml = "";
                        if (File.Exists(shortcutsXmlPath))
                        {
                            try
                            {
                                // Save current shorcuts Xml
                                TextReader reader = System.IO.File.OpenText(shortcutsXmlPath);
                                lastShortcutsXml = reader.ReadToEnd();
                                reader.Close();
                            }
                            catch { }
                        }

                        // Prepare File System Watcher
                        String shortcutsXmlDir = Path.GetDirectoryName(shortcutsXmlPath);
                        if (!Directory.Exists(shortcutsXmlDir))
                            Directory.CreateDirectory(shortcutsXmlDir);
                        xmlServersWatcher = new FileSystemWatcher(shortcutsXmlDir);
                        xmlServersWatcher.Filter = Path.GetFileName(shortcutsXmlPath);
                        xmlServersWatcher.IncludeSubdirectories = false;
                        xmlServersWatcher.NotifyFilter = NotifyFilters.LastWrite;
                        xmlServersWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsXMLChange);
                        xmlServersWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsXMLChange);
                        xmlServersWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_OnShortcutsXMLChange);
                        xmlServersWatcher.EnableRaisingEvents = true;
                        fileSystemWatchers.Add(WindowsIdentity.GetCurrent().Name, xmlServersWatcher);
                    }
                }
            }
            catch
            {
                throw new FaultException("Subscribe exception");
            }
        }

        private delegate void loggedInDelegate();
        private void OnLoggedIn()
        {
            try
            {
                serviceCallback.LoggedInChange();
            }
            catch { }
        }

        public delegate void shareChangeDelegate();
        public void onShareChange()
        {
            try
            {
                serviceCallback.ShareXmlChange();
            }
            catch { }
        }

        public delegate void shortcutsChangeDelegate();
        public void onShortcutsChange()
        {
            try
            {
                serviceCallback.ShortcutsXmlChange();
            }
            catch { }
        }
        
        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Unsubscribe()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    fileSystemWatchers[WindowsIdentity.GetCurrent().Name].Dispose();
                    fileSystemWatchers.Remove(WindowsIdentity.GetCurrent().Name);
                }
            }
            catch
            {
                throw new FaultException("Unsubscribe exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string GetShortcutsXml()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    // Read shortcuts XML document
                    String shortcutsXmlPath = RAASServerPathHelper.GetCurrentShortcutsXMLPath();
                    TextReader reader = System.IO.File.OpenText(shortcutsXmlPath);
                    String shortcutsXml = reader.ReadToEnd();
                    reader.Close();

                    // Return contents of shortcuts xml document
                    return shortcutsXml;
                }
            }
            catch
            {
                throw new FaultException("GetShortcutsXML exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public bool GetLoggedInState()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    return TSManager.SessionActive();
                }
            }
            catch
            {
                throw new FaultException("GetLoggedInState exception");
            }
        }


        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetVersion()
        {
            try
            {
                return RegistryHelper.GetProgramRootValue(COMPANY_NAME, PROGRAM_NAME, VERSION_REGISTRY_STRING);
            }
            catch
            {
                throw new FaultException("GetVersion exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public bool LogOff()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    TSManager.LogOffSessions();
                    return true;
                }
            }
            catch
            {
                throw new FaultException("Logoff exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public static string GetUserProfilePath(String userName)
        {
            try
            {
                return ProfileHelper.GetUserProfilePath(userName);
            }
            catch
            {
                throw new FaultException("GetUserProfilePath exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public String GetShareXml()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    // Read shares XML document
                    String sharesXmlPath = RAASServerPathHelper.GetCurrentSharesXMLPath();
                    TextReader reader = System.IO.File.OpenText(sharesXmlPath);
                    String sharesXml = reader.ReadToEnd();
                    reader.Close();

                    // Return contents of shares xml document
                    return sharesXml;
                }
            }
            catch
            {
                throw new FaultException("GetShareXml exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public byte[] GetIcon(string iconName)
        {
            try {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    // Determine the location for the icon
                    String fullPath = RAASServerPathHelper.GetCurrentIconFilePath(Path.GetFileName(iconName));

                    // Return icon file contents if file exist
                    if (System.IO.File.Exists(fullPath))
                        return System.IO.File.ReadAllBytes(fullPath);
                    else
                        throw new FaultException("GetIcon exception");
                }
            }
            catch
            {
                throw new FaultException("GetIcon exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public string[] GetAutostartEntries()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    return RegistryHelper.GetAutostartEntries();
                }
            }
            catch
            {
                throw new FaultException("GetAutostartEntries exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetShortcutsServerPath()
        {
            try
            {
                return RAASServerPathHelper.GetShortcutsServerPath();
            }
            catch
            {
                throw new FaultException("GetShortcutsServerPath exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetAutostartPath()
        {
            try
            {
                return RAASServerPathHelper.GetAutostartPath();
            }
            catch
            {
                throw new FaultException("GetAutostartPath exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetKeepAliveAgentPath()
        {
            try
            {
                return RAASServerPathHelper.GetKeepAlivePath();
            }
            catch
            {
                throw new FaultException("GetKeepAliveAgentPath exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]

        public string GetAppNamesPath()
        {
            try
            {
                return RAASServerPathHelper.GetAppNamesPath();
            }
            catch
            {
                throw new FaultException("GetAppNamesPath exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public byte[] GetShortcutIcon(string path)
        {
            try
            {
                String userName;
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    Bitmap existingAssociationsIconPath = AssociationsIconHelper.GetExistingAssociationIcon(path, RAASServerPathHelper.GetCurrectAssociationsPath());
                    if (existingAssociationsIconPath != null)
                        return IconHelper.IconBitmapToBytes(existingAssociationsIconPath);
                    IntPtr oldValue = IntPtr.Zero;
                    Win32Helper.Wow64DisableWow64FsRedirection(ref oldValue);
                    byte[] result = IconHelper.GetShortcutIconBitmapBytes(path, userName);
                    Win32Helper.Wow64RevertWow64FsRedirection(oldValue);
                    return result;
                }

            }
            catch
            {
                throw new FaultException("GetShortcutIcon exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Reboot()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    PowerHelper.Reboot();
                    return;
                }
            }
            catch
            {
                throw new FaultException("Reboot exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public void UpdateShortcuts()
        {
            try
            {
                new Thread(delegate () {
                    ServiceHelper.RestartWindowsService(RAAS_SHORTCUTS_SERVICE_NAME, TimeSpan.FromDays(10));
                }).Start();
                return;
            }
            catch
            {
                throw new FaultException("UpdateShortcuts exception");
            }
        }

        private void FileSystemWatcher_OnShortcutsXMLChange(object sender, FileSystemEventArgs e)
        {
            try
            {
                String shortcutsXmlPath = e.FullPath;
                TextReader reader = System.IO.File.OpenText(shortcutsXmlPath);
                String shortcutsXml = reader.ReadToEnd();
                reader.Close();

                if (shortcutsXml != lastShortcutsXml)
                {
                    lastShortcutsXml = shortcutsXml;
                    shortcutsXMLChangeEvent.Invoke();
                }
            }
            catch { }
        }
    }
}