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
using static Elbitin.Applications.RAAS.Common.Helpers.Win32Helper;
using System.Runtime.InteropServices;
using System.Management;
using Serilog.Core;
using Serilog;
using System.ServiceModel.Channels;
using System.Text;

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

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int LoadString(IntPtr hInstance, int ID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);

        private enum WtsInfoClass
        {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        private static string GetUsername(int sessionId, bool prependDomain = true)
        {
            IntPtr buffer;
            int strLen;
            string username = "SYSTEM";
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (prependDomain)
                {
                    if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1)
                    {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        internal enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        internal enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo
        }

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);

        [DllImport("Wtsapi32.dll")]
        static extern bool WTSQuerySessionInformation(
            System.IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern int WTSEnumerateSessions(
                        System.IntPtr hServer,
                        int Reserved,
                        int Version,
                        ref System.IntPtr ppSessionInfo,
                        ref int pCount);
        internal static List<int> GetSessionIDs(IntPtr server)
        {
            List<int> sessionIds = new List<int>();
            IntPtr buffer = IntPtr.Zero;
            int count = 0;
            int retval = WTSEnumerateSessions(server, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int32 current = (int)buffer;

            if (retval != 0)
            {
                for (int i = 0; i < count; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += dataSize;
                    sessionIds.Add(si.SessionID);
                }
                WTSFreeMemory(buffer);
            }
            return sessionIds;
        }

        public static IntPtr OpenServer(String Name)
        {
            IntPtr server = WTSOpenServer(Name);
            return server;
        }
        public static void CloseServer(IntPtr ServerHandle)
        {
            WTSCloseServer(ServerHandle);
        }

        public static List<Int32> ListSessions(String ServerName)
        {
            IntPtr server = IntPtr.Zero;
            List<Int32> ret = new List<Int32>();
            server = OpenServer(ServerName);

            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;

                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));

                Int32 current = (int)ppSessionInfo;

                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                        current += dataSize;

                        ret.Add(si.SessionID);
                    }

                    WTSFreeMemory(ppSessionInfo);
                }
            }
            finally
            {
                CloseServer(server);
            }

            return ret;
        }

        internal static bool LogOffUser(string userName, String serverName)
        {
            IntPtr server = WTSOpenServer(serverName);
            try
            {
                userName = userName.Trim().ToUpper();
                List<Int32> sessions = ListSessions(serverName);
                Dictionary<String, Int32> userSessionDictionary = GetUserSessionDictionary(serverName, sessions);
                if (userSessionDictionary.ContainsKey(userName))
                    return WTSLogoffSession(server, userSessionDictionary[userName], true);
                else
                    return false;
            }
            finally
            {
                CloseServer(server);
            }
        }

        private static Dictionary<string, Int32> GetUserSessionDictionary(String serverName, List<Int32> sessions)
        {
            Dictionary<string, Int32> userSession = new Dictionary<string, Int32>();
            IntPtr server = WTSOpenServer(serverName);
            try
            {
                foreach (Int32 sessionId in sessions)
                {
                    string uName = GetUserName(sessionId, server);
                    if (!string.IsNullOrWhiteSpace(uName))
                        userSession.Add(uName, sessionId);
                }
            }
            finally
            {
                CloseServer(server);
            }
            return userSession;
        }

        internal static string GetUserName(Int32 sessionId, IntPtr server)
        {
            IntPtr buffer = IntPtr.Zero;
            uint count = 0;
            string userName = string.Empty;
            try
            {
                WTSQuerySessionInformation(server, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out count);
                userName = Marshal.PtrToStringAnsi(buffer).ToUpper().Trim();
            }
            finally
            {
                WTSFreeMemory(buffer);
            }
            return userName;
        }

        bool UserLoggedIn(String userName)
        {
            userName = userName.ToLowerInvariant();
            NTAccount f = new NTAccount(userName);
            SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
            String sidString = s.ToString();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile WHERE Loaded = True");
            foreach (ManagementObject mo in searcher.Get())
            {
                List<string> SIDs = new List<string>();
                foreach (var prop in mo.Properties)
                    if (prop.Name == "SID")
                    {
                        String propUserName = (string)(prop?.Value?.ToString())?.Split('\\')?.Last();
                        if (propUserName != null)
                            SIDs.Add(propUserName);
                    }
                foreach (String propSID in SIDs)
                    if (propSID.ToLowerInvariant() == sidString?.Split('\\')?.Last()?.ToLowerInvariant())
                        return true;
            }
            return false;
        }

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

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public bool GetLoggedInState()
        {
            try
            {
                String userName;
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last().ToLowerInvariant();
                }
                return UserLoggedIn(userName);
            }
            catch (Exception e)
            {
                throw new FaultException("GetLoggedInState exception: " + e.Message + ": " + e.StackTrace);
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
                String userName;
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    userName = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name?.ToLowerInvariant()?.Split('\\')?.Last();
                    if (userName != null)
                        return LogOffUser(userName, Environment.MachineName);
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                throw new FaultException("Logoff exception: " + e.Message + ":" + e.StackTrace);
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
        public bool GetCanReboot()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    return RegistryHelper.GetCanReboot(COMPANY_NAME, PROGRAM_NAME);
                }
            }
            catch
            {
                throw new FaultException("GetCanReboot exception");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Reboot()
        {
            try
            {
                using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                {
                    if (RegistryHelper.GetCanReboot(COMPANY_NAME, PROGRAM_NAME))
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