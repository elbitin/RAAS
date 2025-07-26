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
using System.Text;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using IWshRuntimeLibrary;
using System.ComponentModel;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.Common.Models;
using static System.Net.Mime.MediaTypeNames;
using static Elbitin.Applications.RAAS.Common.Helpers.Win32Helper;
using static Elbitin.Applications.RAAS.Common.Helpers.LinkHelper;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    static public class IconHelper
    {
        public const int ICON_SIZE = 48;
        private const string DEFAULT_ICON_HASH = "00000000000000000000000000000000";
        private const string DEFAULT_ICON_DLL = "shell32.dll";
        private const string SETTINGS_SHORTCUT_ICON_DLL = "shell32.dll";
        private const int SETTINGS_SHORTCUT_ICON_INDEX = 16826;
        private const int DEFAULT_ICON_INDEX = 0;
        private const string DEFAULT_EXE_ICON_DLL = "imageres.dll";
        private const int DEFAULT_EXE_ICON_INDEX = 11;
        public static string GetIconFileName(string iconHash, string fileName)
        {
            return iconHash + "." + fileName + ".ico";
        }

        public static Bitmap BytesToIcon(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return new Bitmap(ms);
            }
        }

        public static byte[] IconToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        public static void SaveAsIcon(Bitmap inputBitmap, String targetIconLocation)
        {
            int width, height;
            width = inputBitmap.Width;
            height = inputBitmap.Height;
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(inputBitmap, new System.Drawing.Size(width, height));
            System.IO.MemoryStream memData = new System.IO.MemoryStream();
            newBitmap.Save(memData, System.Drawing.Imaging.ImageFormat.Png);
            FileStream outputStream = new FileStream(targetIconLocation, FileMode.OpenOrCreate);
            System.IO.BinaryWriter iconWriter = new System.IO.BinaryWriter(outputStream);
            iconWriter.Write((byte)0);
            iconWriter.Write((byte)0);
            iconWriter.Write((short)1);
            iconWriter.Write((short)1);
            iconWriter.Write((byte)width);
            iconWriter.Write((byte)height);
            iconWriter.Write((byte)0);
            iconWriter.Write((byte)0);
            iconWriter.Write((short)0);
            iconWriter.Write((short)32);
            iconWriter.Write((int)memData.Length);
            iconWriter.Write((int)(6 + 16));
            iconWriter.Write(memData.ToArray());
            iconWriter.Flush();
            outputStream.Close();
            return;
        }

        public static void SaveAsMultiSizeIcon(Bitmap inputBitmap, String targetIconLocation)
        {
            FileStream outputStream = new FileStream(targetIconLocation, FileMode.OpenOrCreate);
            ImagingHelper.ConvertToIcon(inputBitmap, outputStream);
            outputStream.Close();
            return;
        }

        public static Bitmap AddIconOverlay(Bitmap originalIcon, Bitmap overlay)
        {
            System.Drawing.Image a = originalIcon;
            System.Drawing.Image b = overlay;
            Bitmap bitmap = new Bitmap(ICON_SIZE, ICON_SIZE);
            Graphics canvas = Graphics.FromImage(bitmap);
            canvas.DrawImage(a, 0, 0, ICON_SIZE, ICON_SIZE);
            canvas.DrawImage(b, ICON_SIZE / 2, 0, ICON_SIZE / 2, ICON_SIZE / 2);
            canvas.Save();
            return bitmap;
        }

        public static byte[] ExtractAssociatedIconBytes(String filePath)
        {
            byte[] iconBytes;
            ushort uicon;
            StringBuilder strB = new StringBuilder(filePath);
            IntPtr handle = Win32Helper.ExtractAssociatedIcon(IntPtr.Zero, strB, out uicon);
            Icon localIcon = Icon.FromHandle(handle);
            iconBytes = IconHelper.IconToBytes(localIcon);
            Win32Helper.DestroyIcon((IntPtr)uicon);
            return iconBytes;
        }

        public static Bitmap LoadIcon(String iconFile, int iconIndex)
        {
            int numIcons = 1;
            IntPtr[] largeIcons = new IntPtr[numIcons];
            IntPtr[] smallIcons = new IntPtr[numIcons];
            Win32Helper.ExtractIconEx(iconFile, iconIndex, largeIcons, smallIcons, numIcons);
            Icon largeIico = Icon.FromHandle(largeIcons[0]);
            Bitmap IconBitmap = largeIico.ToBitmap();

            // Destoy references
            foreach (IntPtr hIcon in largeIcons)
                Win32Helper.DestroyIcon(hIcon);
            foreach (IntPtr hIcon in smallIcons)
                Win32Helper.DestroyIcon(hIcon);

            return IconBitmap;
        }

        public static Bitmap GetShortcutIconBitmap(String filePath, String userName)
        {
            if (AccessRightsHelper.AllowedRead(filePath, userName))
            {
                // Initialize default values
                LinkHelper.LinkDetails linkDetails;

                // Determine if the supplied path is a shortcut and update icon information accordingly
                if (filePath.ToLowerInvariant().EndsWith(".lnk"))
                {
                    LinkHelper.GetLinkDetails(filePath, out linkDetails);
                }
                else
                {
                    linkDetails = new LinkHelper.LinkDetails();
                    linkDetails.TargetPath = filePath;
                }

                linkDetails.UserName = userName;

                // Get file info from registry if no icon location was provided in link
                if (linkDetails.IconLocation.Length == 0)
                    RegistryHelper.GetShortcutIconDetails(ref linkDetails);

                // Return default icon if no associated icon could be found
                if (linkDetails.IconLocation.Length == 0)
                    return IconHelper.GetFileIconFromSHGetFileInfo(filePath).ToBitmap();

                if (AccessRightsHelper.AllowedRead(linkDetails.IconLocation, userName))
                {
                    try
                    {
                        if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".ico"))
                        {
                            return new System.Drawing.Icon(linkDetails.IconLocation, ICON_SIZE, ICON_SIZE).ToBitmap();
                        }
                        if (linkDetails.IconHandlerLibraryPath.Length != 0)
                        {
                            if (AccessRightsHelper.AllowedRead(linkDetails.IconHandlerLibraryPath, userName))
                                return GetLibraryIcon(linkDetails).ToBitmap();
                        }
                        if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".exe") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".dll") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".cpl"))
                            try
                            {
                                return GetShellShortcutIconBitmap(linkDetails.IconLocation, linkDetails.IconIndex);
                            }
                            catch
                            {
                                if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".exe"))
                                    return GetDefaultExeIcon();
                            }
                    }
                    catch { }
                    return IconHelper.GetFileIconFromSHGetFileInfo(filePath).ToBitmap();
                }
            }
            throw new Exception("getShortcutIcon exception: Insufficient access rights");
        }

        public static Bitmap GetSettingsIcon()
        {
            String file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), SETTINGS_SHORTCUT_ICON_DLL);
            int iconIndex = SETTINGS_SHORTCUT_ICON_INDEX;
            return GetIconFromGroup(file, iconIndex, ICON_SIZE).ToBitmap();
        }

        public static Bitmap GetDefaultIcon()
        {
            String iconLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), DEFAULT_ICON_DLL);
            int iconIndex = DEFAULT_ICON_INDEX;
            return GetShellShortcutIconBitmap(iconLocation, iconIndex);
        }

        public static Bitmap GetDefaultExeIcon()
        {
            String iconLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), DEFAULT_EXE_ICON_DLL);
            int iconIndex = DEFAULT_EXE_ICON_INDEX;
            return GetShellShortcutIconBitmap(iconLocation, iconIndex);
        }

        public static Bitmap GetShellShortcutIconBitmap(String iconLocation, int iconIndex)
        {
            // Initiate variables
            Bitmap IconBitmap = null;
            bool iconBitmapCreated = false;
            int numIcons = 1;
            IntPtr[] largeIcons = new IntPtr[numIcons];
            IntPtr[] smallIcons = new IntPtr[numIcons];

            // Extract icon from the icon location
            Icon ico;
            uint nIcons = 1;
            uint[] IconIDs = new uint[nIcons];
            uint iconsExtracted = Win32Helper.SHExtractIconsW(
                iconLocation,
                iconIndex,
                ICON_SIZE, ICON_SIZE,
                largeIcons,
                IconIDs,
                nIcons,
                0
            );
            if (largeIcons[0] == IntPtr.Zero)
            {
                if (iconIndex < 0)
                    iconIndex = 0;
                iconsExtracted = Win32Helper.SHExtractIconsW(
                    iconLocation,
                    iconIndex,
                    ICON_SIZE, ICON_SIZE,
                    largeIcons,
                    IconIDs,
                    nIcons,
                    0
                );
            }
            if (largeIcons[0] != IntPtr.Zero)
            {
                ico = Icon.FromHandle(largeIcons[0]);
                IconBitmap = ico.ToBitmap();
                iconBitmapCreated = true;
            }

            // Destoy references
            foreach (IntPtr hIcon in largeIcons)
                Win32Helper.DestroyIcon(hIcon);
            foreach (IntPtr hIcon in smallIcons)
                Win32Helper.DestroyIcon(hIcon);

            // Throw exception if no icon bitmap could be found
            if (!iconBitmapCreated)
                throw new Exception("LoadShellIconBitmap could not load icon bitmap");

            return IconBitmap;

        }

        public static String GetIconHashString(byte[] iconBytes)
        {
            StringBuilder hash = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                foreach (byte b in md5.ComputeHash(iconBytes))
                    hash.Append(b.ToString("X2"));
            }

            return hash.ToString();
        }

        public static String GetDefaultIconHash()
        {
            return DEFAULT_ICON_HASH;
        }

        public static String GetBitmapHash(Bitmap bmp)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] bytes = stream.ToArray();
                using (MD5 md5 = MD5.Create())
                {
                    StringBuilder hash = new StringBuilder();
                    foreach (byte b in md5.ComputeHash(bytes))
                        hash.Append(b.ToString("X2"));
                    stream.Close();
                    return hash.ToString();
                }
            }
        }

        public static System.Drawing.Icon GetIconFromGroup(string file, int groupId, int size)
        {
            IntPtr hExe = Win32Helper.LoadLibrary(file);
            if (hExe != IntPtr.Zero)
            {
                IntPtr hResource = Win32Helper.FindResource(hExe, groupId, Win32Helper.RT_GROUP_ICON);
                IntPtr hMem = Win32Helper.LoadResource(hExe, hResource);
                IntPtr lpResourcePtr = Win32Helper.LockResource(hMem);
                uint sz = Win32Helper.SizeofResource(hExe, hResource);
                byte[] lpResource = new byte[sz];
                Marshal.Copy(lpResourcePtr, lpResource, 0, (int)sz);
                int nID = Win32Helper.LookupIconIdFromDirectoryEx(lpResource, true, size, size, 0x0000);
                hResource = Win32Helper.FindResource(hExe, nID, Win32Helper.RT_ICON);
                hMem = Win32Helper.LoadResource(hExe, hResource);
                lpResourcePtr = Win32Helper.LockResource(hMem);
                sz = Win32Helper.SizeofResource(hExe, hResource);
                lpResource = new byte[sz];
                Marshal.Copy(lpResourcePtr, lpResource, 0, (int)sz);
                IntPtr hIcon = Win32Helper.CreateIconFromResourceEx(lpResource, sz, true, 0x00030000, size, size, 0);
                System.Drawing.Icon icon = (Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
                Win32Helper.DestroyIcon(hIcon);
                return icon;
            }
            return null;
        }

        public static byte[] IconBitmapToBytes(Bitmap inputBit)
        {
            System.IO.MemoryStream memData = new System.IO.MemoryStream();
            inputBit.Save(memData, System.Drawing.Imaging.ImageFormat.Png);
            return memData.ToArray();
        }

        public static Icon GetFolderIcon(string dir)
        {
            Win32Helper.SHFILEINFO shfi = new Win32Helper.SHFILEINFO();
            IntPtr res = Win32Helper.SHGetFileInfo((string)dir, (uint)0, out shfi, (uint)Marshal.SizeOf(shfi), (int)(0x000000100 | 0x000000008));
            if (shfi.hIcon != IntPtr.Zero)
            {
                Icon icon;
                icon = (Icon)System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
                Win32Helper.DestroyIcon(shfi.hIcon);
                return icon;
            }
            throw new Exception("GetFolderIcon exception");
        }

        public static byte[] GetShortcutIconBitmapBytes(String filePath, String userName)
        {
            try
            {
                // Extract icon from file path if file exist
                if (System.IO.File.Exists(filePath))
                {
                    Bitmap iconBitmap;
                    iconBitmap = GetShortcutIconBitmap(filePath, userName);
                    return IconBitmapToBytes(iconBitmap);
                }
                else
                    throw new Exception("GetShortcutIcon file do not exist: " + filePath);
            }
            catch 
            {
                throw;
            }
            
        }
        public static System.Drawing.Icon GetFileIconFromSHGetFileInfo(string filePath)
        {
            // Get icon index from Shell
            Win32Helper.SHFILEINFO shfi = new Win32Helper.SHFILEINFO();
            uint flags = Win32Helper.SHGFI_USEFILEATTRIBUTES | Win32Helper.SHGFI_ICONLOCATION;
            IntPtr res = Win32Helper.SHGetFileInfo(filePath,
                Win32Helper.FILE_ATTRIBUTE_NORMAL,
                out shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                (int)flags);
            if (res == IntPtr.Zero)
                throw (new System.IO.FileNotFoundException());
            int iconIndex = shfi.iIcon;
            Win32Helper.DestroyIcon(shfi.hIcon);

            // Get the System IImageList object from the Shell:
            Guid iidImageList = Win32Helper.IID_ImageList;
            Win32Helper.IImageList iml;
            int size = (int)Win32Helper.SHIL_EXTRALARGE;
            int hres = Win32Helper.SHGetImageList(size, ref iidImageList, out iml);
            if (hres != 0)
                throw (new System.Exception("Error SHGetImageList"));

            // Get icon from image list
            IntPtr hIcon = IntPtr.Zero;
            hres = iml.GetIcon(iconIndex, 0, ref hIcon);
            if (hres != 0)
                throw (new System.Exception("Error iml.GetIcon"));

            // Return icon
            System.Drawing.Icon icon = (Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
            Win32Helper.DestroyIcon(hIcon);
            return icon;
        }

        public static Win32Helper.IExtractIcon GetDefaultLinkExtractIcon(string filename)
        {
            Win32Helper.ShellLink link = new Win32Helper.ShellLink();
            ((Win32Helper.IPersistFile)link).Load(filename, Win32Helper.STGM_READ);
            IntPtr ppidl = IntPtr.Zero;
            ((Win32Helper.IShellLinkW)link).GetIDList(out ppidl);
            Win32Helper.IExtractIcon extractIcon = GetExtractIcon(ppidl);
            return extractIcon;
        }

        private static Win32Helper.IExtractIcon GetExtractIcon(IntPtr ppidl)
        {
            Win32Helper.SHBindToParent(ppidl, Win32Helper.IID_IShellFolder, out IntPtr pFolder, out IntPtr childPidl);
            Win32Helper.IShellFolder shellFolder = (Win32Helper.IShellFolder)Marshal.GetTypedObjectForIUnknown(pFolder, typeof(Win32Helper.IShellFolder));
            IntPtr[] intPtrs = new IntPtr[1];
            intPtrs[0] = childPidl;
            shellFolder.GetUIObjectOf(IntPtr.Zero, 1, intPtrs, Win32Helper.IID_IExtractIcon, 0, out IntPtr ppv);
            Win32Helper.IExtractIcon extractIcon = (Win32Helper.IExtractIcon)Marshal.GetTypedObjectForIUnknown(ppv, typeof(Win32Helper.IExtractIcon));
            return extractIcon;
        }

        public static Icon GetDefaultLinkIDListIcon(string filename)
        {
            StringBuilder sb = new StringBuilder(Win32Helper.MAX_PATH);
            Win32Helper.IExtractIcon extractIcon = GetDefaultLinkExtractIcon(filename);
            extractIcon.GetIconLocation(Win32Helper.IExtractIconuFlags.GIL_FORSHORTCUT, sb, Win32Helper.MAX_PATH, out int piIndex, out Win32Helper.IExtractIconpwFlags flags);
            extractIcon.Extract(sb.ToString(), (uint)piIndex, out IntPtr hIconLarge, out IntPtr hIconSmall, ICON_SIZE);
            System.Drawing.Icon icon = (Icon)System.Drawing.Icon.FromHandle(hIconLarge).Clone();
            Win32Helper.DestroyIcon(hIconLarge);
            return icon;
        }

        public static Icon GetDefaultFileIcon(string filename)
        {
            StringBuilder sb = new StringBuilder(Win32Helper.MAX_PATH);
            Win32Helper.SHGetDesktopFolder(out Win32Helper.IShellFolder desktopFolder);
            desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, filename.ToString(), 0, out IntPtr ppFilePidl, 0);
            Win32Helper.IExtractIcon extractIcon = GetExtractIcon(ppFilePidl);
            extractIcon.GetIconLocation(Win32Helper.IExtractIconuFlags.GIL_FORSHORTCUT, sb, Win32Helper.MAX_PATH, out int piIndex, out Win32Helper.IExtractIconpwFlags flags);
            extractIcon.Extract(sb.ToString(), (uint)piIndex, out IntPtr hIconLarge, out IntPtr hIconSmall, ICON_SIZE);
            System.Drawing.Icon icon = (Icon)System.Drawing.Icon.FromHandle(hIconLarge).Clone();
            Win32Helper.DestroyIcon(hIconLarge);
            return icon;
        }

        public static Icon GetFileIconByIconHandler(LinkDetails linkDetails)
        {
            String libraryPath = linkDetails.IconHandlerLibraryPath;
            IntPtr module = Win32Helper.LoadLibrary(libraryPath);
            if (module == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, "Unable to load library: " + libraryPath);
            }
            IntPtr dllGetClassObjectPtr = Win32Helper.GetProcAddress(module, "DllGetClassObject");
            if (dllGetClassObjectPtr == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                string message = string.Format("Unable to find DllGetClassObject in file: " + libraryPath);
                throw new Win32Exception(error, message);
            }
            Win32Helper.DllGetClassObject dllGetClassObject = (Win32Helper.DllGetClassObject)Marshal.GetDelegateForFunctionPointer(dllGetClassObjectPtr, typeof(Win32Helper.DllGetClassObject));
            Guid classFactoryGuid = Win32Helper.IID_IClassFactory;
            Win32Helper.IClassFactory classFactory;
            Guid guidClsid = new Guid(linkDetails.IconHandlerClsid);
            int hResult = dllGetClassObject(ref guidClsid, ref classFactoryGuid, out classFactory);
            if (hResult != 0)
                throw new Win32Exception(hResult, "Cannot create class factory for file: " + libraryPath);
            Guid iidIUnknown = Win32Helper.IID_IUnknown;
            object pfObj;
            classFactory.CreateInstance(null, Win32Helper.IID_IPersistFile, out pfObj);
            IPersistFile ip = (IPersistFile)pfObj;
            ip.Load(linkDetails.IconLocation, 0x00000000);
            object eiObj;
            classFactory.CreateInstance(null, Win32Helper.IID_IExtractIcon, out eiObj);
            Win32Helper.IExtractIcon iExtractIcon = (Win32Helper.IExtractIcon)eiObj;
            StringBuilder sb = new StringBuilder(256);
            int pil;
            Win32Helper.IExtractIconpwFlags extractIconpwFlags;
            hResult = iExtractIcon.GetIconLocation(Win32Helper.IExtractIconuFlags.GIL_FORSHELL, sb, 256, out pil, out extractIconpwFlags);//.Extract(@"c:\temp\test.sln", (uint)0, out IntPtr hIconLarge, out IntPtr phIconSmall, 32);
            IntPtr hIconSmall;
            IntPtr hIconLarge;
            if ((extractIconpwFlags & IExtractIconpwFlags.GIL_NOTFILENAME) == IExtractIconpwFlags.GIL_NOTFILENAME)
                hResult = iExtractIcon.Extract(linkDetails.IconLocation, (uint)0, out hIconLarge, out hIconSmall, ICON_SIZE);
            else
                hResult = iExtractIcon.Extract(sb.ToString(), (uint)pil, out hIconLarge, out hIconSmall, ICON_SIZE);
            Icon icon = (Icon)Icon.FromHandle(hIconLarge).Clone();
            if (hResult != 0)
                throw new Win32Exception(hResult, "No icon returned from IExtractIcon for file: " + libraryPath);
            Win32Helper.FreeLibrary(module);
            Win32Helper.DestroyIcon(hIconLarge);
            return icon;
        }

        public static Bitmap GetLinkDetialsIcon(LinkHelper.LinkDetails linkDetails)
        {
            if (AccessRightsHelper.AllowedRead(linkDetails.IconLocation, linkDetails.UserName))
            {
                try
                {
                    if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".ico"))
                    {
                        return new System.Drawing.Icon(linkDetails.IconLocation, ICON_SIZE, ICON_SIZE).ToBitmap();
                    }
                    if (linkDetails.IconHandlerLibraryPath.Length != 0)
                    {
                        if (AccessRightsHelper.AllowedRead(linkDetails.IconHandlerLibraryPath, linkDetails.UserName))
                            return IconHelper.GetLibraryIcon(linkDetails).ToBitmap();
                    }
                    if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".exe") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".dll") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".cpl") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".icl"))
                        try
                        {
                            return IconHelper.GetShellShortcutIconBitmap(linkDetails.IconLocation, linkDetails.IconIndex);
                        }
                        catch
                        {
                            if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".exe"))
                                return IconHelper.GetDefaultExeIcon();
                        }
                }
                catch { }
            }
            return IconHelper.GetFileIconFromSHGetFileInfo(linkDetails.IconLocation).ToBitmap();
        }

        public static Icon GetLibraryIcon(LinkHelper.LinkDetails linkDetails)
        {
            String libraryPath = linkDetails.IconHandlerLibraryPath;
            IntPtr module = Win32Helper.LoadLibrary(libraryPath);
            if (module == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, "Unable to load library: " + libraryPath);
            }
            IntPtr dllGetClassObjectPtr = Win32Helper.GetProcAddress(module, "DllGetClassObject");
            if (dllGetClassObjectPtr == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                string message = string.Format("Unable to find DllGetClassObject in file: " + libraryPath);
                throw new Win32Exception(error, message);
            }
            Win32Helper.DllGetClassObject dllGetClassObject = (Win32Helper.DllGetClassObject)Marshal.GetDelegateForFunctionPointer(dllGetClassObjectPtr, typeof(Win32Helper.DllGetClassObject));
            Guid classFactoryGuid = Win32Helper.IID_IClassFactory;
            Win32Helper.IClassFactory classFactory;
            Guid clsid = new Guid(linkDetails.IconHandlerClsid);
            int hResult = dllGetClassObject(ref clsid, ref classFactoryGuid, out classFactory);
            if (hResult != 0)
                throw new Win32Exception(hResult, "Cannot create class factory for file: " + libraryPath);
            Guid iidIUnknown = Win32Helper.IID_IUnknown;
            object obj;
            classFactory.CreateInstance(null, ref iidIUnknown, out obj);
            Win32Helper.IExtractIcon iExtractIcon = (Win32Helper.IExtractIcon)obj;
            hResult = iExtractIcon.Extract(linkDetails.IconLocation, (uint)linkDetails.IconIndex, out IntPtr hIconLarge, out IntPtr phIconSmall, ICON_SIZE);
            if (hResult != 0)
                throw new Win32Exception(hResult, "No icon returned from IExtractIcon for file: " + libraryPath);
            Win32Helper.FreeLibrary(module);
            System.Drawing.Icon icon = (Icon)System.Drawing.Icon.FromHandle(hIconLarge).Clone();
            Win32Helper.DestroyIcon(hIconLarge);
            return icon;
        }
    }
}
