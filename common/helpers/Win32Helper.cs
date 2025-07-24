/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS.
 *
 * RAAS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    public static class Win32Helper
    {
        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        };

        public enum ResourceType
        {
            RESOURCETYPE_ANY = 0,
            RESOURCETYPE_DISK = 1,
            RESOURCETYPE_PRINT = 2,
            RESOURCETYPE_RESERVED = 8,
            RESOURCETYPE_UNKNOWN = -1,
        };

        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        };

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        };

        public enum ResourceConnection
        {
            CONNECT_UPDATE_PROFILE = 1,
            CONNECT_UPDATE_RECENT = 2,
            CONNECT_TEMPORARY = 4,
            CONNECT_INTERACTIVE = 8,
            CONNECT_PROMPT = 0X10,
            CONNECT_REDIRECT = 0X80,
            CONNECT_CURRENT_MEDIA = 0X200,
            CONNECT_COMMAND_LINE = 0X800,
            CONNECT_CMD_SAVECRED = 0X1000,
            CONNECT_CRED_RESET = 0X2000

        };

        [Flags]
        public enum SHCNE : uint
        {
            SHCNE_RENAMEITEM = 0x00000001,
            SHCNE_CREATE = 0x00000002,
            SHCNE_DELETE = 0x00000004,
            SHCNE_MKDIR = 0x00000008,
            SHCNE_RMDIR = 0x00000010,
            SHCNE_MEDIAINSERTED = 0x00000020,
            SHCNE_MEDIAREMOVED = 0x00000040,
            SHCNE_DRIVEREMOVED = 0x00000080,
            SHCNE_DRIVEADD = 0x00000100,
            SHCNE_NETSHARE = 0x00000200,
            SHCNE_NETUNSHARE = 0x00000400,
            SHCNE_ATTRIBUTES = 0x00000800,
            SHCNE_UPDATEDIR = 0x00001000,
            SHCNE_UPDATEITEM = 0x00002000,
            SHCNE_SERVERDISCONNECT = 0x00004000,
            SHCNE_UPDATEIMAGE = 0x00008000,
            SHCNE_DRIVEADDGUI = 0x00010000,
            SHCNE_RENAMEFOLDER = 0x00020000,
            SHCNE_FREESPACE = 0x00040000,
            SHCNE_EXTENDED_EVENT = 0x04000000,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_DISKEVENTS = 0x0002381F,
            SHCNE_GLOBALEVENTS = 0x0C0581E0,
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_INTERRUPT = 0x80000000
        }

        public enum SHChangeNotifyFlags : uint
        {
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_DWORD = 0x0003,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_TYPE = 0x00FF,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000,
            SHCNF_PATH = SHCNF_PATHW,
            SHCNF_PRINTER = SHCNF_PRINTERW
        }

        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        public enum ESFGAO : uint
        {
            SFGAO_CANCOPY = 0x00000001,
            SFGAO_CANMOVE = 0x00000002,
            SFGAO_CANLINK = 0x00000004,
            SFGAO_LINK = 0x00010000,
            SFGAO_SHARE = 0x00020000,
            SFGAO_READONLY = 0x00040000,
            SFGAO_HIDDEN = 0x00080000,
            SFGAO_FOLDER = 0x20000000,
            SFGAO_FILESYSTEM = 0x40000000,
            SFGAO_HASSUBFOLDER = 0x80000000,
        }

        public enum ESHCONTF
        {
            SHCONTF_FOLDERS = 0x0020,
            SHCONTF_NONFOLDERS = 0x0040,
            SHCONTF_INCLUDEHIDDEN = 0x0080,
            SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
            SHCONTF_NETPRINTERSRCH = 0x0200,
            SHCONTF_SHAREABLE = 0x0400,
            SHCONTF_STORAGE = 0x0800
        }

        public enum ESHGDN
        {
            SHGDN_NORMAL = 0x0000,
            SHGDN_INFOLDER = 0x0001,
            SHGDN_FOREDITING = 0x1000,
            SHGDN_FORADDRESSBAR = 0x4000,
            SHGDN_FORPARSING = 0x8000,
        }

        public enum ESTRRET : int
        {
            eeRRET_WSTR = 0x0000,    // Use STRRET.pOleStr
            STRRET_OFFSET = 0x0001,    // Use STRRET.uOffset to Ansi
            STRRET_CSTR = 0x0002    // Use STRRET.cStr
        }

        [Flags()]
        public enum IExtractIconuFlags : uint
        {
            GIL_ASYNC = 0x0020,
            GIL_DEFAULTICON = 0x0040,
            GIL_FORSHELL = 0x0002,
            GIL_FORSHORTCUT = 0x0080,
            GIL_OPENICON = 0x0001,
            GIL_CHECKSHIELD = 0x0200
        }

        [Flags()]
        public enum IExtractIconpwFlags : uint
        {
            GIL_DONTCACHE = 0x0010,
            GIL_NOTFILENAME = 0x0008,
            GIL_PERCLASS = 0x0004,
            GIL_PERINSTANCE = 0x0002,
            GIL_SIMULATEDOC = 0x0001,
            GIL_SHIELD = 0x0200,//Windows Vista only
            GIL_FORCENOSHIELD = 0x0400//Windows Vista only
        }

        [Flags()]
        public enum SLR_FLAGS
        {
            /// <summary>
            /// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
            /// the high-order word of fFlags can be set to a time-out value that specifies the
            /// maximum amount of time to be spent resolving the link. The function returns if the
            /// link cannot be resolved within the time-out duration. If the high-order word is set
            /// to zero, the time-out duration will be set to the default value of 3,000 milliseconds
            /// (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
            /// duration, in milliseconds.
            /// </summary>
            SLR_NO_UI = 0x1,
            /// <summary>Obsolete and no longer used</summary>
            SLR_ANY_MATCH = 0x2,
            /// <summary>If the link object has changed, update its path and list of identifiers.
            /// If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
            /// whether or not the link object has changed.</summary>
            SLR_UPDATE = 0x4,
            /// <summary>Do not update the link information</summary>
            SLR_NOUPDATE = 0x8,
            /// <summary>Do not execute the search heuristics</summary>
            SLR_NOSEARCH = 0x10,
            /// <summary>Do not use distributed link tracking</summary>
            SLR_NOTRACK = 0x20,
            /// <summary>Disable distributed link tracking. By default, distributed link tracking tracks
            /// removable media across multiple devices based on the volume name. It also uses the
            /// Universal Naming Convention (UNC) path to track remote file systems whose drive letter
            /// has changed. Setting SLR_NOLINKINFO disables both types of tracking.</summary>
            SLR_NOLINKINFO = 0x40,
            /// <summary>Call the Microsoft Windows Installer</summary>
            SLR_INVOKE_MSI = 0x80
        }

        public struct Pair
        {
            public System.Drawing.Icon icon { get; set; }
            public IntPtr iconHandleToDestroy { set; get; }

        }

        public static bool DestroyIcon2(IntPtr hIcon)
        {
            return DestroyIcon(hIcon);
        }

        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        // this works too...from Unions.cs
        [StructLayout(LayoutKind.Explicit, Size = 520)]
        public struct STRRETinternal
        {
            [FieldOffset(0)]
            public IntPtr pOleStr;

            [FieldOffset(0)]
            public IntPtr pStr;  // LPSTR pStr;   NOT USED

            [FieldOffset(0)]
            public uint uOffset;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STRRET
        {
            public uint uType;
            public STRRETinternal data;
        }

        /// <summary>
        /// Defines an item identifier.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHITEMID
        {
            /// <summary>
            /// The size of identifier, in bytes, including cb itself.
            /// </summary>
            public ushort cb;
            /// <summary>
            /// A variable-length item identifier.
            /// </summary>
            public byte[] abID;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ITEMIDLIST
        {
            /// <summary>
            /// A list of item identifiers.
            /// </summary>
            [MarshalAs(UnmanagedType.Struct)]
            public SHITEMID mkid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };

        public enum GWLParameter
        {
            GWL_EXSTYLE = -20,
            GWL_HINSTANCE = -6,
            GWL_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4
        }

        [Flags]
        public enum WindowStyles : uint
        {
            // Window Styles
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
        }

        public enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        public enum ABEdge
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3,
        }

        public enum ABState
        {
            ABS_MANUAL = 0,
            ABS_AUTOHIDE = 1,
            ABS_ALWAYSONTOP = 2,
            ABS_AUTOHIDEANDONTOP = 3
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public enum GetAncestorFlags
        {
            GetParent = 1,
            GetRoot = 2,
            GetRootOwner = 3
        }

        public enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
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

        [Flags]
        public enum AccessMaskEnum
        {
            FILE_READ_DATA = 0x000001,
            FILE_LIST_DIRECTORY = 0x000001,
            FILE_WRITE_DATA = 0x000002,
            FILE_ADD_FILE = 0x000002,
            FILE_APPEND_DATA = 0x000004,
            FILE_ADD_SUBDIRECTORY = 0x000004,
            FILE_READ_EA = 0x000008,
            FILE_WRITE_EA = 0x000010,
            FILE_EXECUTE = 0x000020,
            FILE_TRAVERSE = 0x000020,
            FILE_DELETE_CHILD = 0x000040,
            FILE_READ_ATTRIBUTES = 0x000080,
            FILE_WRITE_ATTRIBUTES = 0x000100,
            DELETE = 0x010000,
            READ_CONTROL = 0x020000,
            WRITE_DAC = 0x040000,
            WRITE_OWNER = 0x080000,
            SYNCHRONIZE = 0x100000,
            OWNER = FILE_READ_DATA | FILE_LIST_DIRECTORY | FILE_WRITE_DATA |
                                    FILE_ADD_FILE | FILE_APPEND_DATA | FILE_ADD_SUBDIRECTORY |
                                    FILE_READ_EA | FILE_WRITE_EA | FILE_EXECUTE |
                                    FILE_TRAVERSE | FILE_DELETE_CHILD | FILE_READ_ATTRIBUTES |
                                    FILE_WRITE_ATTRIBUTES | DELETE | READ_CONTROL |
                                    WRITE_DAC | WRITE_OWNER | SYNCHRONIZE,
            READ_ONLY = FILE_READ_DATA | FILE_LIST_DIRECTORY | FILE_READ_EA |
                                    FILE_EXECUTE | FILE_TRAVERSE | FILE_READ_ATTRIBUTES |
                                    READ_CONTROL | SYNCHRONIZE,
            CONTRIBUTOR = OWNER & ~(FILE_DELETE_CHILD | WRITE_DAC | WRITE_OWNER)
        }

        public enum SIGDN
        {
            SIGDN_NORMALDISPLAY,
            SIGDN_PARENTRELATIVEPARSING,
            SIGDN_DESKTOPABSOLUTEPARSING,
            SIGDN_PARENTRELATIVEEDITING,
            SIGDN_DESKTOPABSOLUTEEDITING,
            SIGDN_FILESYSPATH,
            SIGDN_URL,
            SIGDN_PARENTRELATIVEFORADDRESSBAR,
            SIGDN_PARENTRELATIVE,
            SIGDN_PARENTRELATIVEFORUI
        };

        [Flags()]
        public enum SLGP_FLAGS
        {
            /// <summary>Retrieves the standard short (8.3 format) file name</summary>
            SLGP_SHORTPATH = 0x1,
            /// <summary>Retrieves the Universal Naming Convention (UNC) path name of the file</summary>
            SLGP_UNCPRIORITY = 0x2,
            /// <summary>Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded</summary>
            SLGP_RAWPATH = 0x4
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public Int32 SessionID;

            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;

            public WTS_CONNECTSTATE_CLASS State;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler)
                : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public Int32 cbSize;
            public IntPtr hWnd;
            public Int32 uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        [DllImport("shell32.dll")]
        public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, out IntPtr pidl, uint sfgaoIn, out uint psfgaoOut);
        public static class SWP
        {
            public static readonly uint
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        [ComImportAttribute()]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        //helpstring("Image List"),
        public interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr hbmImage,
                IntPtr hbmMask,
                ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                int iImage,
                int iOverlay);

            [PreserveSig]
            int Replace(
                int i,
                IntPtr hbmImage,
                IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
                IntPtr hbmImage,
                int crMask,
                ref int pi);

            [PreserveSig]
            int Draw(
                ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
            int i);

            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                int i,
                ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
                int iDst,
                IImageList punkSrc,
                int iSrc,
                int uFlags);

            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                int i,
                ref RECT prc);

            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);

            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);

            [PreserveSig]
            int GetImageCount(
            ref int pi);

            [PreserveSig]
            int SetImageCount(
                int uNewCount);

            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);

            [PreserveSig]
            int GetBkColor(
                ref int pclr);

            [PreserveSig]
            int BeginDrag(
                int iTrack,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                IntPtr hwndLock,
                int x,
                int y);

            [PreserveSig]
            int DragLeave(
                IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
                int x,
                int y);

            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int iDrag,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
                int fShow);

            [PreserveSig]
            int GetDragImage(
                ref POINT ppt,
                ref POINT pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
                int iOverlay,
                ref int piIndex);
        };


        [ComImport()]
        [Guid("000214fa-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //http://msdn.microsoft.com/en-us/library/windows/desktop/bb761852(v=vs.85).aspx
        public interface IExtractIcon
        {
            /// <summary>
            /// Gets the location and index of an icon.
            /// </summary>
            /// <param name="uFlags">One or more of the following values. This parameter can also be NULL.use GIL_ Consts</param>
            /// <param name="szIconFile">A pointer to a buffer that receives the icon location. The icon location is a null-terminated string that identifies the file that contains the icon.</param>
            /// <param name="cchMax">The size of the buffer, in characters, pointed to by pszIconFile.</param>
            /// <param name="piIndex">A pointer to an int that receives the index of the icon in the file pointed to by pszIconFile.</param>
            /// <param name="pwFlags">A pointer to a UINT value that receives zero or a combination of the following value</param>
            /// <returns></returns>
            ///
            [PreserveSig]
            int GetIconLocation(IExtractIconuFlags uFlags, [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] StringBuilder szIconFile, int cchMax, out int piIndex, out IExtractIconpwFlags pwFlags);

            /// <summary>
            /// Extracts an icon image from the specified location.
            /// </summary>
            /// <param name="pszFile">A pointer to a null-terminated string that specifies the icon location.</param>
            /// <param name="nIconIndex">The index of the icon in the file pointed to by pszFile.</param>
            /// <param name="phiconLarge">A pointer to an HICON value that receives the handle to the large icon. This parameter may be NULL.</param>
            /// <param name="phiconSmall">A pointer to an HICON value that receives the handle to the small icon. This parameter may be NULL.</param>
            /// <param name="nIconSize">The desired size of the icon, in pixels. The low word contains the size of the large icon, and the high word contains the size of the small icon. The size specified can be the width or height. The width of an icon always equals its height.</param>
            /// <returns>
            /// Returns S_OK if the function extracted the icon, or S_FALSE if the calling application should extract the icon.
            /// </returns>
            [PreserveSig]
            int Extract([MarshalAs(UnmanagedType.LPWStr)] string pszFile, uint nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, uint nIconSize);
        }

        /// <summary>
        ///  managed equivalent of IShellFolder interface
        ///  Pinvoke.net / Mod by Arik Poznanski - pooya parsa
        ///  Msdn:      http://msdn.microsoft.com/en-us/library/windows/desktop/bb775075(v=vs.85).aspx
        ///  Pinvoke:   http://pinvoke.net/default.aspx/Interfaces/IShellFolder.html
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        public interface IShellFolder
        {
            /// <summary>
            /// Translates a file object's or folder's display name into an item identifier list.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="hwnd">Optional window handle</param>
            /// <param name="pbc">Optional bind context that controls the parsing operation. This parameter is normally set to NULL. </param>
            /// <param name="pszDisplayName">Null-terminated UNICODE string with the display name</param>
            /// <param name="pchEaten">Pointer to a ULONG value that receives the number of characters of the display name that was parsed.</param>
            /// <param name="ppidl"> Pointer to an ITEMIDLIST pointer that receives the item identifier list for the object.</param>
            /// <param name="pdwAttributes">Optional parameter that can be used to query for file attributes.this can be values from the SFGAO enum</param>
            void ParseDisplayName(IntPtr hwnd, IntPtr pbc, String pszDisplayName, UInt32 pchEaten, out IntPtr ppidl, UInt32 pdwAttributes);

            /// <summary>
            ///Allows a client to determine the contents of a folder by creating an item identifier enumeration object and returning its IEnumIDList interface.
            ///Return value: error code, if any
            /// </summary>
            /// <param name="hwnd">If user input is required to perform the enumeration, this window handle should be used by the enumeration object as the parent window to take user input.</param>
            /// <param name="grfFlags">Flags indicating which items to include in the  enumeration. For a list of possible values, see the SHCONTF enum. </param>
            /// <param name="ppenumIDList">Address that receives a pointer to the IEnumIDList interface of the enumeration object created by this method. </param>
            void EnumObjects(IntPtr hwnd, ESHCONTF grfFlags, out IntPtr ppenumIDList);

            /// <summary>
            ///Retrieves an IShellFolder object for a subfolder.
            // Return value: error code, if any
            /// </summary>
            /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.</param>
            /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be used during this operation.</param>
            /// <param name="riid">Identifier of the interface to return. </param>
            /// <param name="ppv">Address that receives the interface pointer.</param>
            void BindToObject(IntPtr pidl, IntPtr pbc, [In]ref Guid riid, out IntPtr ppv);

            /// <summary>
            /// Requests a pointer to an object's storage interface.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="pidl">Address of an ITEMIDLIST structure that identifies the subfolder relative to its parent folder. </param>
            /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be  used during this operation.</param>
            /// <param name="riid">Interface identifier (IID) of the requested storage interface.</param>
            /// <param name="ppv"> Address that receives the interface pointer specified by riid.</param>
            void BindToStorage(IntPtr pidl, IntPtr pbc, [In]ref Guid riid, out IntPtr ppv);

            /// <summary>
            /// Determines the relative order of two file objects or folders, given
            /// their item identifier lists. Return value: If this method is
            /// successful, the CODE field of the HRESULT contains one of the
            /// following values (the code can be retrived using the helper function
            /// GetHResultCode): Negative A negative return value indicates that the first item should precede the second (pidl1 < pidl2).
            ////
            ///Positive A positive return value indicates that the first item should
            ///follow the second (pidl1 > pidl2).  Zero A return value of zero
            ///indicates that the two items are the same (pidl1 = pidl2).
            /// </summary>
            /// <param name="lParam">Value that specifies how the comparison  should be performed. The lower Sixteen bits of lParam define the sorting  rule.
            ///  The upper sixteen bits of lParam are used for flags that modify the sorting rule. values can be from  the SHCIDS enum
            /// </param>
            /// <param name="pidl1">Pointer to the first item's ITEMIDLIST structure.</param>
            /// <param name="pidl2"> Pointer to the second item's ITEMIDLIST structure.</param>
            /// <returns></returns>
            [PreserveSig] Int32 CompareIDs(Int32 lParam, IntPtr pidl1, IntPtr pidl2);

            /// <summary>
            /// Requests an object that can be used to obtain information from or interact
            /// with a folder object.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="hwndOwner">Handle to the owner window.</param>
            /// <param name="riid">Identifier of the requested interface.</param>
            /// <param name="ppv">Address of a pointer to the requested interface. </param>
            void CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

            /// <summary>
            /// Retrieves the attributes of one or more file objects or subfolders.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="cidl">Number of file objects from which to retrieve attributes. </param>
            /// <param name="apidl">Address of an array of pointers to ITEMIDLIST structures, each of which  uniquely identifies a file object relative to the parent folder.</param>
            /// <param name="rgfInOut">Address of a single ULONG value that, on entry contains the attributes that the caller is
            /// requesting. On exit, this value contains the requested attributes that are common to all of the specified objects. this value can be from the SFGAO enum
            /// </param>
            void GetAttributesOf(UInt32 cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]IntPtr[] apidl, ref ESFGAO rgfInOut);

            /// <summary>
            /// Retrieves an OLE interface that can be used to carry out actions on the
            /// specified file objects or folders. Return value: error code, if any
            /// </summary>
            /// <param name="hwndOwner">Handle to the owner window that the client should specify if it displays a dialog box or message box.</param>
            /// <param name="cidl">Number of file objects or subfolders specified in the apidl parameter. </param>
            /// <param name="apidl">Address of an array of pointers to ITEMIDLIST  structures, each of which  uniquely identifies a file object or subfolder relative to the parent folder.</param>
            /// <param name="riid">Identifier of the COM interface object to return.</param>
            /// <param name="rgfReserved"> Reserved. </param>
            /// <param name="ppv">Pointer to the requested interface.</param>
            void GetUIObjectOf(IntPtr hwndOwner, UInt32 cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]IntPtr[] apidl, [In] ref Guid riid, UInt32 rgfReserved, out IntPtr ppv);

            /// <summary>
            /// Retrieves the display name for the specified file object or subfolder.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL)  that uniquely identifies the file  object or subfolder relative to the parent  folder. </param>
            /// <param name="uFlags">Flags used to request the type of display name to return. For a list of possible values. </param>
            /// <param name="pName"> Address of a STRRET structure in which to return the display name.</param>
            void GetDisplayNameOf(IntPtr pidl, ESHGDN uFlags, out STRRET pName);

            /// <summary>
            /// Sets the display name of a file object or subfolder, changing the item
            /// identifier in the process.
            /// Return value: error code, if any
            /// </summary>
            /// <param name="hwnd"> Handle to the owner window of any dialog or message boxes that the client displays.</param>
            /// <param name="pidl"> Pointer to an ITEMIDLIST structure that uniquely identifies the file object or subfolder relative to the parent folder. </param>
            /// <param name="pszName"> Pointer to a null-terminated string that specifies the new display name.</param>
            /// <param name="uFlags">Flags indicating the type of name specified by  the lpszName parameter. For a list of possible values, see the description of the SHGNO enum.</param>
            /// <param name="ppidlOut"></param>
            void SetNameOf(IntPtr hwnd, IntPtr pidl, String pszName, ESHCONTF uFlags, out IntPtr ppidlOut);

        }

        [ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersist
        {
            [PreserveSig]
            void GetClassID(out Guid pClassID);
        }

        [Guid("00000001-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        public interface IClassFactory
        {
            void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
            void LockServer(bool fLock);
        }

        [ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistFile : IPersist
        {
            new void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();

            [PreserveSig]
            void Load([In, MarshalAs(UnmanagedType.LPWStr)]
                string pszFileName, uint dwMode);

            [PreserveSig]
            void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
                [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

            [PreserveSig]
            void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

            [PreserveSig]
            void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
        }

        /// <summary>The IShellLink interface allows Shell links to be created, modified, and resolved</summary>
        [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        public interface IShellLinkW
        {
            /// <summary>Retrieves the path and file name of a Shell link object</summary>
            void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SLGP_FLAGS fFlags);
            /// <summary>Retrieves the list of item identifiers for a Shell link object</summary>
            void GetIDList(out IntPtr ppidl);
            /// <summary>Sets the pointer to an item identifier list (PIDL) for a Shell link object.</summary>
            void SetIDList(IntPtr pidl);
            /// <summary>Retrieves the description string for a Shell link object</summary>
            void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            /// <summary>Sets the description for a Shell link object. The description can be any application-defined string</summary>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            /// <summary>Retrieves the name of the working directory for a Shell link object</summary>
            void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            /// <summary>Sets the name of the working directory for a Shell link object</summary>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            /// <summary>Retrieves the command-line arguments associated with a Shell link object</summary>
            void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            /// <summary>Sets the command-line arguments for a Shell link object</summary>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            /// <summary>Retrieves the hot key for a Shell link object</summary>
            void GetHotkey(out short pwHotkey);
            /// <summary>Sets a hot key for a Shell link object</summary>
            void SetHotkey(short wHotkey);
            /// <summary>Retrieves the show command for a Shell link object</summary>
            void GetShowCmd(out int piShowCmd);
            /// <summary>Sets the show command for a Shell link object. The show command sets the initial show state of the window.</summary>
            void SetShowCmd(int iShowCmd);
            /// <summary>Retrieves the location (path and index) of the icon for a Shell link object</summary>
            void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
                int cchIconPath, out int piIcon);
            /// <summary>Sets the location (path and index) of the icon for a Shell link object</summary>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            /// <summary>Sets the relative path to the Shell link object</summary>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            /// <summary>Attempts to find the target of a Shell link, even if it has been moved or renamed</summary>
            void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
            /// <summary>Sets the path and file name of a Shell link object</summary>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);

        }

        // CLSID_ShellLink from ShlGuid.h 
        [
            ComImport(),
            Guid("00021401-0000-0000-C000-000000000046")
        ]
        public class ShellLink
        {
        }

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageList(
            int iImageList,
            ref Guid riid,
            out IImageList ppv
            );

        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Int32 nFolder,
                 ref IntPtr ppidl);

        public delegate int DllGetClassObject(ref Guid clsid, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IClassFactory classFactory);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadString(IntPtr hInstance, int ID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, int lpName, int lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("user32.dll")]
        public static extern int LookupIconIdFromDirectoryEx(byte[] presbits, bool fIcon, int cxDesired, int cyDesired, uint Flags);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr ILCreateFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);

        [DllImport("shell32.dll")]
        public static extern int SHGetDesktopFolder(out IShellFolder ppshf);

        [DllImport("shell32.dll", ExactSpelling = true, PreserveSig = false)]
        public static extern void SHBindToParent(IntPtr pidl,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IntPtr ppv,
            out IntPtr ppidlLast);

        [DllImport("Mpr.dll")]
        public static extern int WNetUseConnection(
            IntPtr hwndOwner,
            NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            int dwFlags,
            string lpAccessName,
            string lpBufferSize,
            string lpResult
        );

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(
            int wEventId,
            int uFlags,
            IntPtr dwItem1,
            IntPtr dwItem2);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern void SHChangeNotify(
            long wEventId,
            uint uFlags,
            string dwItem1,
            IntPtr dwItem2);


        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern Boolean CloseHandle(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Shell32.dll")]
        public extern static int ExtractIconEx(string libName,
            int iconIndex,
            IntPtr[] largeIcon,
            IntPtr[] smallIcon,
            int nIcons);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr oldValue);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr oldValue);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("User32.Dll", EntryPoint = "PostMessageA", SetLastError = true)]
        public static extern bool PostMessage(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(
            IntPtr hWnd,
            out uint lpdwProcessId);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern long SystemParametersInfo(
            long uAction,
            int lpvParam,
            ref bool uParam,
            int fuWinIni);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr windowHandle, GWLParameter nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr windowHandle, GWLParameter nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(
            string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendNotifyMessage(
            int hWnd,
            int Msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
             int hWnd,
             int hWndInsertAfter,
             int X,
             int Y,
             int cx,
             int cy,
             uint uFlags);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("Wksprt.exe")]
        public static extern int GetWorkspaceNames([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = System.Runtime.InteropServices.VarEnum.VT_BSTR)] ref string[] columnTitles);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int wFlag);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        public extern static IntPtr SetActiveWindow(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockSetForegroundWindow(uint uLockCode);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern bool SystemParametersInfoSet(uint action, uint param, uint vparam, uint init);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int GetProcessId(IntPtr hProcess);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(
            IntPtr hIcon);

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractAssociatedIcon(
            IntPtr hInst,
            StringBuilder lpIconPath,
            out ushort lpiIcon);

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint SHExtractIconsW(
            string pszFileName,
            int nIconIndex,
            int cxIcon,
            int cyIcon,
            IntPtr[] phIcon,
            uint[] pIconId,
            uint nIcons,
            uint flags
        );

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbFileInfo,
            int uFlags);

        [DllImport("shell32.dll")]
        public static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr pszPath);

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(
            int uFlags,
            int dwReason);

        [DllImport("shlwapi.dll")]
        public static extern bool PathIsNetworkPath(string pszPath);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);

        [DllImport("shell32.dll")]
        public static extern int SHGetFolderPath(
            IntPtr hwndOwner,
            int nFolder,
            IntPtr hToken,
            uint dwFlags,
            [Out] StringBuilder pszPath);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

        public enum WtsInfoClass
        {
            WTSUserName = 5,
            WTSDomainName = 7,
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

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWLParameter nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        public static int SetWindowLong(IntPtr windowHandle, GWLParameter nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return (int)SetWindowLongPtr64(windowHandle, nIndex, new IntPtr(dwNewLong));
            }
            return SetWindowLong32(windowHandle, nIndex, dwNewLong);
        }

        public enum ShowWindowCommands : uint
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9
        }

        // Window handle constants
        public static IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        public static IntPtr HWND_TOP = new IntPtr(0);
        public static IntPtr HWND_BOTTOM = new IntPtr(1);
        public static IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // Window messages constants
        public const UInt32 WM_ACTIVATE = 0x0006;
        public const UInt32 WM_ACTIVATEAPP = 0x001C;
        public const UInt32 WM_AFXFIRST = 0x0360;
        public const UInt32 WM_AFXLAST = 0x037F;
        public const UInt32 WM_APP = 0x8000;
        public const UInt32 WM_ASKCBFORMATNAME = 0x030C;
        public const UInt32 WM_CANCELJOURNAL = 0x004B;
        public const UInt32 WM_CANCELMODE = 0x001F;
        public const UInt32 WM_CAPTURECHANGED = 0x0215;
        public const UInt32 WM_CHANGECBCHAIN = 0x030D;
        public const UInt32 WM_CHANGEUISTATE = 0x0127;
        public const UInt32 WM_CHAR = 0x0102;
        public const UInt32 WM_CHARTOITEM = 0x002F;
        public const UInt32 WM_CHILDACTIVATE = 0x0022;
        public const UInt32 WM_CLEAR = 0x0303;
        public const UInt32 WM_CLOSE = 0x0010;
        public const UInt32 WM_COMMAND = 0x0111;
        public const UInt32 WM_COMPACTING = 0x0041;
        public const UInt32 WM_COMPAREITEM = 0x0039;
        public const UInt32 WM_CONTEXTMENU = 0x007B;
        public const UInt32 WM_COPY = 0x0301;
        public const UInt32 WM_COPYDATA = 0x004A;
        public const UInt32 WM_CREATE = 0x0001;
        public const UInt32 WM_CTLCOLORBTN = 0x0135;
        public const UInt32 WM_CTLCOLORDLG = 0x0136;
        public const UInt32 WM_CTLCOLOREDIT = 0x0133;
        public const UInt32 WM_CTLCOLORLISTBOX = 0x0134;
        public const UInt32 WM_CTLCOLORMSGBOX = 0x0132;
        public const UInt32 WM_CTLCOLORSCROLLBAR = 0x0137;
        public const UInt32 WM_CTLCOLORSTATIC = 0x0138;
        public const UInt32 WM_CUT = 0x0300;
        public const UInt32 WM_DEADCHAR = 0x0103;
        public const UInt32 WM_DELETEITEM = 0x002D;
        public const UInt32 WM_DESTROY = 0x0002;
        public const UInt32 WM_DESTROYCLIPBOARD = 0x0307;
        public const UInt32 WM_DEVICECHANGE = 0x0219;
        public const UInt32 WM_DEVMODECHANGE = 0x001B;
        public const UInt32 WM_DISPLAYCHANGE = 0x007E;
        public const UInt32 WM_DRAWCLIPBOARD = 0x0308;
        public const UInt32 WM_DRAWITEM = 0x002B;
        public const UInt32 WM_DROPFILES = 0x0233;
        public const UInt32 WM_ENABLE = 0x000A;
        public const UInt32 WM_ENDSESSION = 0x0016;
        public const UInt32 WM_ENTERIDLE = 0x0121;
        public const UInt32 WM_ENTERMENULOOP = 0x0211;
        public const UInt32 WM_ENTERSIZEMOVE = 0x0231;
        public const UInt32 WM_ERASEBKGND = 0x0014;
        public const UInt32 WM_EXITMENULOOP = 0x0212;
        public const UInt32 WM_EXITSIZEMOVE = 0x0232;
        public const UInt32 WM_FONTCHANGE = 0x001D;
        public const UInt32 WM_GETDLGCODE = 0x0087;
        public const UInt32 WM_GETFONT = 0x0031;
        public const UInt32 WM_GETHOTKEY = 0x0033;
        public const UInt32 WM_GETICON = 0x007F;
        public const UInt32 WM_GETMINMAXINFO = 0x0024;
        public const UInt32 WM_GETOBJECT = 0x003D;
        public const UInt32 WM_GETTEXT = 0x000D;
        public const UInt32 WM_GETTEXTLENGTH = 0x000E;
        public const UInt32 WM_HANDHELDFIRST = 0x0358;
        public const UInt32 WM_HANDHELDLAST = 0x035F;
        public const UInt32 WM_HELP = 0x0053;
        public const UInt32 WM_HOTKEY = 0x0312;
        public const UInt32 WM_HSCROLL = 0x0114;
        public const UInt32 WM_HSCROLLCLIPBOARD = 0x030E;
        public const UInt32 WM_ICONERASEBKGND = 0x0027;
        public const UInt32 WM_IME_CHAR = 0x0286;
        public const UInt32 WM_IME_COMPOSITION = 0x010F;
        public const UInt32 WM_IME_COMPOSITIONFULL = 0x0284;
        public const UInt32 WM_IME_CONTROL = 0x0283;
        public const UInt32 WM_IME_ENDCOMPOSITION = 0x010E;
        public const UInt32 WM_IME_KEYDOWN = 0x0290;
        public const UInt32 WM_IME_KEYLAST = 0x010F;
        public const UInt32 WM_IME_KEYUP = 0x0291;
        public const UInt32 WM_IME_NOTIFY = 0x0282;
        public const UInt32 WM_IME_REQUEST = 0x0288;
        public const UInt32 WM_IME_SELECT = 0x0285;
        public const UInt32 WM_IME_SETCONTEXT = 0x0281;
        public const UInt32 WM_IME_STARTCOMPOSITION = 0x010D;
        public const UInt32 WM_INITDIALOG = 0x0110;
        public const UInt32 WM_INITMENU = 0x0116;
        public const UInt32 WM_INITMENUPOPUP = 0x0117;
        public const UInt32 WM_INPUTLANGCHANGE = 0x0051;
        public const UInt32 WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public const UInt32 WM_KEYDOWN = 0x0100;
        public const UInt32 WM_KEYFIRST = 0x0100;
        public const UInt32 WM_KEYLAST = 0x0108;
        public const UInt32 WM_KEYUP = 0x0101;
        public const UInt32 WM_KILLFOCUS = 0x0008;
        public const UInt32 WM_LBUTTONDBLCLK = 0x0203;
        public const UInt32 WM_LBUTTONDOWN = 0x0201;
        public const UInt32 WM_LBUTTONUP = 0x0202;
        public const UInt32 WM_MBUTTONDBLCLK = 0x0209;
        public const UInt32 WM_MBUTTONDOWN = 0x0207;
        public const UInt32 WM_MBUTTONUP = 0x0208;
        public const UInt32 WM_MDIACTIVATE = 0x0222;
        public const UInt32 WM_MDICASCADE = 0x0227;
        public const UInt32 WM_MDICREATE = 0x0220;
        public const UInt32 WM_MDIDESTROY = 0x0221;
        public const UInt32 WM_MDIGETACTIVE = 0x0229;
        public const UInt32 WM_MDIICONARRANGE = 0x0228;
        public const UInt32 WM_MDIMAXIMIZE = 0x0225;
        public const UInt32 WM_MDINEXT = 0x0224;
        public const UInt32 WM_MDIREFRESHMENU = 0x0234;
        public const UInt32 WM_MDIRESTORE = 0x0223;
        public const UInt32 WM_MDISETMENU = 0x0230;
        public const UInt32 WM_MDITILE = 0x0226;
        public const UInt32 WM_MEASUREITEM = 0x002C;
        public const UInt32 WM_MENUCHAR = 0x0120;
        public const UInt32 WM_MENUCOMMAND = 0x0126;
        public const UInt32 WM_MENUDRAG = 0x0123;
        public const UInt32 WM_MENUGETOBJECT = 0x0124;
        public const UInt32 WM_MENURBUTTONUP = 0x0122;
        public const UInt32 WM_MENUSELECT = 0x011F;
        public const UInt32 WM_MOUSEACTIVATE = 0x0021;
        public const UInt32 WM_MOUSEFIRST = 0x0200;
        public const UInt32 WM_MOUSEHOVER = 0x02A1;
        public const UInt32 WM_MOUSELAST = 0x020D;
        public const UInt32 WM_MOUSELEAVE = 0x02A3;
        public const UInt32 WM_MOUSEMOVE = 0x0200;
        public const UInt32 WM_MOUSEWHEEL = 0x020A;
        public const UInt32 WM_MOUSEHWHEEL = 0x020E;
        public const UInt32 WM_MOVE = 0x0003;
        public const UInt32 WM_MOVING = 0x0216;
        public const UInt32 WM_NCACTIVATE = 0x0086;
        public const UInt32 WM_NCCALCSIZE = 0x0083;
        public const UInt32 WM_NCCREATE = 0x0081;
        public const UInt32 WM_NCDESTROY = 0x0082;
        public const UInt32 WM_NCHITTEST = 0x0084;
        public const UInt32 WM_NCLBUTTONDBLCLK = 0x00A3;
        public const UInt32 WM_NCLBUTTONDOWN = 0x00A1;
        public const UInt32 WM_NCLBUTTONUP = 0x00A2;
        public const UInt32 WM_NCMBUTTONDBLCLK = 0x00A9;
        public const UInt32 WM_NCMBUTTONDOWN = 0x00A7;
        public const UInt32 WM_NCMBUTTONUP = 0x00A8;
        public const UInt32 WM_NCMOUSEHOVER = 0x02A0;
        public const UInt32 WM_NCMOUSELEAVE = 0x02A2;
        public const UInt32 WM_NCMOUSEMOVE = 0x00A0;
        public const UInt32 WM_NCPAINT = 0x0085;
        public const UInt32 WM_NCRBUTTONDBLCLK = 0x00A6;
        public const UInt32 WM_NCRBUTTONDOWN = 0x00A4;
        public const UInt32 WM_NCRBUTTONUP = 0x00A5;
        public const UInt32 WM_NCXBUTTONDBLCLK = 0x00AD;
        public const UInt32 WM_NCXBUTTONDOWN = 0x00AB;
        public const UInt32 WM_NCXBUTTONUP = 0x00AC;
        public const UInt32 WM_NCUAHDRAWCAPTION = 0x00AE;
        public const UInt32 WM_NCUAHDRAWFRAME = 0x00AF;
        public const UInt32 WM_NEXTDLGCTL = 0x0028;
        public const UInt32 WM_NEXTMENU = 0x0213;
        public const UInt32 WM_NOTIFY = 0x004E;
        public const UInt32 WM_NOTIFYFORMAT = 0x0055;
        public const UInt32 WM_NULL = 0x0000;
        public const UInt32 WM_PAINT = 0x000F;
        public const UInt32 WM_PAINTCLIPBOARD = 0x0309;
        public const UInt32 WM_PAINTICON = 0x0026;
        public const UInt32 WM_PALETTECHANGED = 0x0311;
        public const UInt32 WM_PALETTEISCHANGING = 0x0310;
        public const UInt32 WM_PARENTNOTIFY = 0x0210;
        public const UInt32 WM_PASTE = 0x0302;
        public const UInt32 WM_PENWINFIRST = 0x0380;
        public const UInt32 WM_PENWINLAST = 0x038F;
        public const UInt32 WM_POWER = 0x0048;
        public const UInt32 WM_POWERBROADCAST = 0x0218;
        public const UInt32 WM_PRINT = 0x0317;
        public const UInt32 WM_PRINTCLIENT = 0x0318;
        public const UInt32 WM_QUERYDRAGICON = 0x0037;
        public const UInt32 WM_QUERYENDSESSION = 0x0011;
        public const UInt32 WM_QUERYNEWPALETTE = 0x030F;
        public const UInt32 WM_QUERYOPEN = 0x0013;
        public const UInt32 WM_QUEUESYNC = 0x0023;
        public const UInt32 WM_QUIT = 0x0012;
        public const UInt32 WM_RBUTTONDBLCLK = 0x0206;
        public const UInt32 WM_RBUTTONDOWN = 0x0204;
        public const UInt32 WM_RBUTTONUP = 0x0205;
        public const UInt32 WM_RENDERALLFORMATS = 0x0306;
        public const UInt32 WM_RENDERFORMAT = 0x0305;
        public const UInt32 WM_SETCURSOR = 0x0020;
        public const UInt32 WM_SETFOCUS = 0x0007;
        public const UInt32 WM_SETFONT = 0x0030;
        public const UInt32 WM_SETHOTKEY = 0x0032;
        public const UInt32 WM_SETICON = 0x0080;
        public const UInt32 WM_SETREDRAW = 0x000B;
        public const UInt32 WM_SETTEXT = 0x000C;
        public const UInt32 WM_SETTINGCHANGE = 0x001A;
        public const UInt32 WM_SHOWWINDOW = 0x0018;
        public const UInt32 WM_SIZE = 0x0005;
        public const UInt32 WM_SIZECLIPBOARD = 0x030B;
        public const UInt32 WM_SIZING = 0x0214;
        public const UInt32 WM_SPOOLERSTATUS = 0x002A;
        public const UInt32 WM_STYLECHANGED = 0x007D;
        public const UInt32 WM_STYLECHANGING = 0x007C;
        public const UInt32 WM_SYNCPAINT = 0x0088;
        public const UInt32 WM_SYSCHAR = 0x0106;
        public const UInt32 WM_SYSCOLORCHANGE = 0x0015;
        public const UInt32 WM_SYSCOMMAND = 0x0112;
        public const UInt32 WM_SYSDEADCHAR = 0x0107;
        public const UInt32 WM_SYSKEYDOWN = 0x0104;
        public const UInt32 WM_SYSKEYUP = 0x0105;
        public const UInt32 WM_TCARD = 0x0052;
        public const UInt32 WM_TIMECHANGE = 0x001E;
        public const UInt32 WM_TIMER = 0x0113;
        public const UInt32 WM_UNDO = 0x0304;
        public const UInt32 WM_UNINITMENUPOPUP = 0x0125;
        public const UInt32 WM_USER = 0x0400;
        public const UInt32 WM_USERCHANGED = 0x0054;
        public const UInt32 WM_VKEYTOITEM = 0x002E;
        public const UInt32 WM_VSCROLL = 0x0115;
        public const UInt32 WM_VSCROLLCLIPBOARD = 0x030A;
        public const UInt32 WM_WINDOWPOSCHANGED = 0x0047;
        public const UInt32 WM_WINDOWPOSCHANGING = 0x0046;
        public const UInt32 WM_WININICHANGE = 0x001A;
        public const UInt32 WM_XBUTTONDBLCLK = 0x020D;
        public const UInt32 WM_XBUTTONDOWN = 0x020B;
        public const UInt32 WM_XBUTTONUP = 0x020C;

        public const uint STGM_READ = 0;
        public const int MAX_PATH = 260;

        // Extract icon constants
        public const int RT_GROUP_ICON = 14;
        public const int RT_ICON = 0x00000003;


        // Hook code constant
        public const int HC_ACTION = 0;

        // Get file info constants
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        public const uint SHGFI_ICONLOCATION = 0x100;

        // Image list constants
        public const int SHIL_JUMBO = 0x4;
        public const int SHIL_EXTRALARGE = 0x2;

        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        // Guids
        public static readonly Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        public static readonly Guid IID_IExtractIcon = new Guid("000214FA-0000-0000-C000-000000000046");
        public static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
        public static readonly Guid IID_IClassFactory = new Guid("00000001-0000-0000-c000-000000000046");
        public static readonly Guid IID_ImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}