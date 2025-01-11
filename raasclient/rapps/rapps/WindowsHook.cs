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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Linq;

namespace Elbitin.Applications.RAAS.RAASClient.RemoteApps
{
    partial class VisualizationsForm
    {
        private class WindowsHook : IDisposable
        {
            private bool disposed = false;
            private IntPtr hook;
            private VisualizationsForm vsForm;
            private Win32Helper.HookProc del;

            public WindowsHook(VisualizationsForm form, int threadId, string serverName)
            {
                del = new Win32Helper.HookProc(ProcessMessage);
                vsForm = form;
                hook = Win32Helper.SetWindowsHookEx(Win32Helper.HookType.WH_CALLWNDPROC, del, IntPtr.Zero, (uint)threadId);
            }

            ~WindowsHook()
            {
                Dispose(true);
            }

            public void UnHook()
            {
                if (hook != IntPtr.Zero)
                {
                    Win32Helper.UnhookWindowsHookEx(hook);
                    hook = IntPtr.Zero;
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        if (hook != IntPtr.Zero)
                        {
                            UnHook();
                        }
                    }
                    disposed = true;
                }
            }

            public void Dispose()
            {
                UnHook();
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private IntPtr ProcessMessage(int hookcode, IntPtr wparam, IntPtr lparam)
            {
                if (Win32Helper.HC_ACTION == hookcode)
                {
                    Win32Helper.CWPSTRUCT eStruct = (Win32Helper.CWPSTRUCT)Marshal.PtrToStructure(lparam, typeof(Win32Helper.CWPSTRUCT));

                    // Handle standard window messages
                    switch ((uint)eStruct.message)
                    {
                        case Win32Helper.WM_MOUSEACTIVATE:
                            if(vsForm.hWnds.Contains((IntPtr)eStruct.wparam.ToInt32()))
                                vsForm.gotFocusEvent.Invoke();
                            break;
                        case Win32Helper.WM_ACTIVATE:
                            if (vsForm.noOverlayHWnds.Contains(eStruct.hwnd))
                                break;
                            if (vsForm.hWnds.Contains(eStruct.hwnd))
                            {
                                if ((eStruct.wparam.ToInt32() == 1 || eStruct.wparam.ToInt32() == 2))
                                    vsForm.gotFocusEvent.Invoke();
                            }
                            break;
                        case Win32Helper.WM_ACTIVATEAPP:
                            if (vsForm.noOverlayHWnds.Contains(eStruct.hwnd))
                                break;
                            if (vsForm.hWnds.Contains(eStruct.hwnd))
                                if (eStruct.wparam != IntPtr.Zero)
                                    vsForm.gotFocusEvent.Invoke();
                            break;
                        case Win32Helper.WM_NCACTIVATE:
                            if (eStruct.wparam.ToInt32() == 0)
                                break;
                            if (vsForm.noOverlayHWnds.Contains(eStruct.hwnd))
                                break;
                            if (vsForm.hWnds.Contains(eStruct.hwnd))
                                vsForm.gotFocusEvent.Invoke();
                            break;
                        case Win32Helper.WM_SETFOCUS:
                            if (vsForm.noOverlayHWnds.Contains(eStruct.hwnd))
                                break;
                            long windowStyleExActivate = Win32Helper.GetWindowLong(eStruct.hwnd, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
                            if (vsForm.hWnds.Contains(eStruct.hwnd) && (windowStyleExActivate & (int)Win32Helper.WindowStyles.WS_EX_NOACTIVATE) == 0)
                                vsForm.gotFocusEvent.Invoke();
                            break;
                        case Win32Helper.WM_KILLFOCUS:
                            if (vsForm.noOverlayHWnds.Contains(eStruct.hwnd))
                                break;
                            if (vsForm.hWnds.Contains(eStruct.hwnd))
                                if (!vsForm.hWnds.Contains((IntPtr)eStruct.wparam.ToInt32()))
                                    vsForm.lostFocusEvent.Invoke();
                            break;
                        case Win32Helper.WM_SIZE:
                            if ((vsForm.visualizationsEnabled) && !vsForm.noOverlayHWnds.Contains((IntPtr)eStruct.hwnd) && Win32Helper.IsWindowVisible((IntPtr)eStruct.hwnd) && Win32Helper.IsWindow((IntPtr)eStruct.hwnd))
                            {
                                System.Int64 windowStyle = Win32Helper.GetWindowLong((IntPtr)eStruct.hwnd, (int)Win32Helper.GWLParameter.GWL_STYLE);
                                System.Int64 windowStyleEx = Win32Helper.GetWindowLong((IntPtr)eStruct.hwnd, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
                                if ((vsForm.visualizationsEnabled && vsForm.visualizationsActive) && (windowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) != 0 && (windowStyleEx & (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT) == 0)
                                {
                                    vsForm.windowPosChangedEvent.Invoke((IntPtr)eStruct.hwnd, (uint)windowStyle);
                                }
                            }
                            break;
                        case Win32Helper.WM_STYLECHANGED:
                        case Win32Helper.WM_WINDOWPOSCHANGED:
                            if ((vsForm.visualizationsEnabled)&& !vsForm.noOverlayHWnds.Contains((IntPtr)eStruct.hwnd) && Win32Helper.IsWindowVisible((IntPtr)eStruct.hwnd) && Win32Helper.IsWindow((IntPtr)eStruct.hwnd))
                            {
                                System.Int64 windowStyle = Win32Helper.GetWindowLong((IntPtr)eStruct.hwnd, (int)Win32Helper.GWLParameter.GWL_STYLE);
                                System.Int64 windowStyleEx = Win32Helper.GetWindowLong((IntPtr)eStruct.hwnd, (int)Win32Helper.GWLParameter.GWL_EXSTYLE);
                                long lWindowStyle = windowStyle;
                                if ((windowStyle & (int)Win32Helper.WindowStyles.WS_VISIBLE) != 0 && (windowStyleEx & (int)Win32Helper.WindowStyles.WS_EX_TRANSPARENT) == 0 && !vsForm.noOverlayHWnds.Contains((IntPtr)eStruct.hwnd))
                                {
                                    if (!vsForm.hWnds.Contains((IntPtr)eStruct.hwnd))
                                    {
                                        lock (vsForm.hWnds)
                                            vsForm.hWnds.Add((IntPtr)eStruct.hwnd);
                                    }
                                    vsForm.updateOverlayEvent.Invoke((IntPtr)eStruct.hwnd);
                                }

                                // Force connection bar on top of topmost windows
                                if ((windowStyleEx & (int)Win32Helper.WindowStyles.WS_EX_TOPMOST) != 0)
                                    if (VisualizationsForm.connectionbarActive)
                                        vsForm.showConnectionBarsEvent.Invoke(true, true);
                            }
                            else if ((vsForm.visualizationsActive && vsForm.visualizationsEnabled) && !vsForm.noOverlayHWnds.Contains((IntPtr)eStruct.hwnd) && Win32Helper.IsWindow((IntPtr)eStruct.hwnd))
                            {
                                vsForm.setInvisibleWindowEvent.Invoke((IntPtr)eStruct.hwnd);
                            }
                            break;
                        case Win32Helper.WM_DESTROY:
                            vsForm.forgetHwndEvent.Invoke((IntPtr)eStruct.hwnd);
                            break;
                    }
                }
                return Win32Helper.CallNextHookEx(hook, hookcode, wparam, lparam);
            }
        }
    }
}