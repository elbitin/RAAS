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
        public static IntPtr OpenServer(String Name)
        {
            IntPtr server = Win32Helper.WTSOpenServer(Name);
            return server;
        }

        public static void CloseServer(IntPtr ServerHandle)
        {
            Win32Helper.WTSCloseServer(ServerHandle);
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
                Int32 retval = Win32Helper.WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                IntPtr currentSession = ppSessionInfo;
                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Win32Helper.WTS_SESSION_INFO si = (Win32Helper.WTS_SESSION_INFO)Marshal.PtrToStructure(currentSession, typeof(Win32Helper.WTS_SESSION_INFO));
                        currentSession += dataSize;
                        Win32Helper.WTSLogoffSession(IntPtr.Zero, si.SessionID, true);
                    }
                    Win32Helper.WTSFreeMemory(ppSessionInfo);
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
                Int32 retval = Win32Helper.WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
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
                    Win32Helper.WTSFreeMemory(ppSessionInfo);
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
                Int32 retval = Win32Helper.WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(Win32Helper.WTS_SESSION_INFO));
                Int64 current = (int)ppSessionInfo;
                Win32Helper.WTSFreeMemory(ppSessionInfo);
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