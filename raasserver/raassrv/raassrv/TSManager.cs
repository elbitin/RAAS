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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Elbitin.Applications.RAAS.RAASServer.RAASSvr
{
    public class TSManager
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        public enum WTS_CONNECTSTATE_CLASS
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

        public enum WTS_INFO_CLASS
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
        static extern Int32 WTSEnumerateSessions(IntPtr hServer, [MarshalAs(UnmanagedType.U4)] Int32 Reserved, [MarshalAs(UnmanagedType.U4)] Int32 Version, ref IntPtr ppSessionInfo, [MarshalAs(UnmanagedType.U4)] ref Int32 pCount);

        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(IntPtr pMemory);

        public static List<int> GetSessionIDs(IntPtr server)
        {
            List<int> sessionIds = new List<int>();
            IntPtr buffer = IntPtr.Zero;
            int count = 0;
            int retval = WTSEnumerateSessions(server, 0, 1, ref buffer, ref count);
            int dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int64 current = (int)buffer;

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

        public static bool LogOffUser(string userName)
        {
            IntPtr server = IntPtr.Zero;
            try
            {
                userName = userName.Trim().ToUpper();
                List<int> sessions = GetSessions();
                Dictionary<string, int> userSessionDictionary = GetUserSessionDictionary(server, sessions);
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

        public static bool UserIsLoggedIn(string userName)
        {
            IntPtr server = IntPtr.Zero;
            try
            {
                userName = userName.Trim().ToUpper();
                List<int> sessions = GetSessions();
                Dictionary<string, int> userSessionDictionary = GetUserSessionDictionary(server, sessions);
                if (userSessionDictionary.ContainsKey(userName))
                    return true;
                else
                    return false;
            }
            finally
            {
                CloseServer(server);
            }
        }

        public static string GetUserName(int sessionId, IntPtr server)
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

        public static Dictionary<string, int> GetUserSessionDictionary(IntPtr server, List<int> sessions)
        {
            Dictionary<string, int> userSession = new Dictionary<string, int>();

            foreach (var sessionId in sessions)
            {
                string uName = GetUserName(sessionId, server);
                if (!string.IsNullOrWhiteSpace(uName))
                    userSession.Add(uName, sessionId);
            }
            return userSession;
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

        public static List<String> LogOffSessions()
        {
            IntPtr server = IntPtr.Zero;
            List<String> ret = new List<string>();
            server = IntPtr.Zero;
            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                IntPtr currentSession = ppSessionInfo;
                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Win32Helper.WTS_SESSION_INFO si = (Win32Helper.WTS_SESSION_INFO)Marshal.PtrToStructure(currentSession, typeof(Win32Helper.WTS_SESSION_INFO));
                        currentSession += dataSize;
                        WTSLogoffSession(IntPtr.Zero, si.SessionID, true);
                    }
                    WTSFreeMemory(ppSessionInfo);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseServer(server);
            }
            return ret;
        }


        public static List<int> GetSessions()
        {
            IntPtr server = IntPtr.Zero;
            List<int> ret = new List<int>();
            server = IntPtr.Zero;
            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                IntPtr currentSession = ppSessionInfo;
                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Win32Helper.WTS_SESSION_INFO si = (Win32Helper.WTS_SESSION_INFO)Marshal.PtrToStructure(currentSession, typeof(Win32Helper.WTS_SESSION_INFO));
                        currentSession += dataSize;
                        ret.Add(si.SessionID);
                    }
                    WTSFreeMemory(ppSessionInfo);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseServer(server);
            }
            return ret;
        }

        public static bool SessionActive()
        {
            IntPtr server = IntPtr.Zero;
            List<String> ret = new List<string>();
            server = IntPtr.Zero;
            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                IntPtr currentSession = ppSessionInfo;
                bool active = false;
                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Win32Helper.WTS_SESSION_INFO si = (Win32Helper.WTS_SESSION_INFO)Marshal.PtrToStructure(currentSession, typeof(Win32Helper.WTS_SESSION_INFO));
                        currentSession += dataSize;
                        if (si.State == Win32Helper.WTS_CONNECTSTATE_CLASS.WTSActive || si.State == Win32Helper.WTS_CONNECTSTATE_CLASS.WTSDisconnected)
                            active = true;
                    }
                    WTSFreeMemory(ppSessionInfo);
                }
                return active;
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseServer(server);
            }
        }

        public static int GetSessionCount()
        {
            IntPtr server = IntPtr.Zero;
            List<String> ret = new List<string>();
            server = IntPtr.Zero;
            try
            {
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                Int64 current = (int)ppSessionInfo;
                WTSFreeMemory(ppSessionInfo);
                return count;
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseServer(server);
            }
        }
    }
}