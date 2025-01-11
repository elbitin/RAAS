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
using System.IO;
using System.Text;
using IWshRuntimeLibrary;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    public static class LinkHelper
    {
        public class LinkDetails
        {
            public String UserName { get; set; } = "";
            public String LinkFilePath { get; set; } = "";
            public String TargetPath { get; set; } = "";
            public String IconLocation { get; set; } = "";
            public int IconIndex { get; set; } = 0;
            public String IconHandlerLibraryPath { get; set; } = "";
            public String IconHandlerClsid { get; set; } = "";
        }

        public static void CreateLink(string iconPath, string filePath, string arguments)
        {
            WshShell shell = new WshShell();
            String targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "openremote.exe");
            String shortcutPath = filePath;
            IWshShortcut targetLink = shell.CreateShortcut(shortcutPath) as IWshShortcut;
            targetLink.TargetPath = targetPath;
            targetLink.IconLocation = iconPath;
            if (System.IO.File.Exists(iconPath))
                targetLink.IconLocation = iconPath;
            targetLink.Arguments = arguments;
            targetLink.Save();
        }
        
        public static void GetLinkDetails(string path, out LinkDetails linkDetails)
        {
            WshShell shell = new WshShell();
            IWshShortcut sourceLink = shell.CreateShortcut(path) as IWshShortcut;
            linkDetails = new LinkDetails();
            linkDetails.LinkFilePath = path;
            linkDetails.TargetPath = Environment.ExpandEnvironmentVariables(sourceLink.TargetPath);
            String iconLocation = Environment.ExpandEnvironmentVariables(sourceLink.IconLocation.Split(',')[0]);
            if (iconLocation.ToLowerInvariant().EndsWith(".exe") || iconLocation.ToLowerInvariant().EndsWith(".dll") || iconLocation.ToLowerInvariant().EndsWith(".ico") || iconLocation.ToLowerInvariant().EndsWith(".cpl"))
                linkDetails.IconLocation = iconLocation;
            linkDetails.IconIndex = 0;

            // Fix common link details problems
            if (sourceLink.IconLocation.Split(',').Length > 1)
                linkDetails.IconIndex = Convert.ToInt32(sourceLink.IconLocation.Split(',')[1]);
            if (linkDetails.TargetPath.Length == 0 && linkDetails.IconLocation.Length == 0)
                GetShellIDListLinkDetails(ref linkDetails);
            if (linkDetails.IconLocation.ToLowerInvariant().StartsWith("file://"))
                linkDetails.IconLocation = linkDetails.IconLocation.Replace("file://", "");
            if (linkDetails.TargetPath.ToLowerInvariant().StartsWith("file://"))
                linkDetails.TargetPath = linkDetails.TargetPath.Replace("file://", "");
            if (!((System.IO.File.Exists(linkDetails.IconLocation) || System.IO.Directory.Exists(linkDetails.IconLocation))))
                linkDetails.IconLocation = linkDetails.IconLocation.ToLowerInvariant().Replace(":\\program files (x86)", ":\\program files");
            if (!((System.IO.File.Exists(linkDetails.TargetPath) || System.IO.Directory.Exists(linkDetails.TargetPath))))
                linkDetails.TargetPath = linkDetails.TargetPath.ToLowerInvariant().Replace(":\\program files (x86)", ":\\program files");
        }

        public static void GetShellIDListLinkDetails(ref LinkHelper.LinkDetails linkDetails)
        {
            StringBuilder sb = new StringBuilder(Win32Helper.MAX_PATH);
            Win32Helper.IExtractIcon extractIcon = IconHelper.GetDefaultLinkExtractIcon(linkDetails.LinkFilePath);
            extractIcon.GetIconLocation(Win32Helper.IExtractIconuFlags.GIL_FORSHORTCUT, sb, Win32Helper.MAX_PATH, out int piIndex, out Win32Helper.IExtractIconpwFlags flags);
            linkDetails.IconLocation = sb.ToString();
            linkDetails.IconIndex = piIndex;
        }
    }
}
