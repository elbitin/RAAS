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
using System.IO;
using System.Runtime.InteropServices;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.Common.Models
{
    class ShortcutIcon
    {
        public Bitmap IconBitmap { get; set; }

        public ShortcutIcon(String iconFile, int iconIndex)
        {
            LoadIcon(iconFile, iconIndex);
        }

        public ShortcutIcon(byte[] iconBytes)
        {
            LoadIcon(iconBytes);
        }

        public ShortcutIcon(Bitmap bitmap)
        {
            IconBitmap = bitmap;
        }

        public void LoadIcon(String iconFile, int iconIndex)
        {
            IconBitmap = IconHelper.LoadIcon(iconFile, iconIndex);
        }

        public void LoadIcon(byte[] iconBytes)
        {
            IconBitmap = IconHelper.BytesToIcon(iconBytes);
        }

        public void AddIconOverlay(ShortcutIcon shortcutIcon)
        {
            IconBitmap = IconHelper.AddIconOverlay(IconBitmap, shortcutIcon.IconBitmap);
        }

        public void AddIconOverlay(Bitmap bitmap)
        {
            IconBitmap = IconHelper.AddIconOverlay(IconBitmap, bitmap);
        }

        public void SaveAsIcon(String targetIconLocation)
        {
            IconHelper.SaveAsIcon(IconBitmap, targetIconLocation);
        }

        public void SaveAsMultiSizeIcon(String targetIconLocation)
        {
            IconHelper.SaveAsMultiSizeIcon(IconBitmap, targetIconLocation);
        }

        public byte[] IconToBytes()
        {
            IntPtr hIcon = IconBitmap.GetHicon();
            Icon ico = Icon.FromHandle(hIcon);
            return IconHelper.IconToBytes(ico);
        }

        public static System.Drawing.Icon GetIconFromGroup(string file, int groupId, int size)
        {
            return IconHelper.GetIconFromGroup(file, groupId, size);
        }

        public static ShortcutIcon CreateRemoteAppIcon()
        {
            // Remote app icon
            String remoteAppIconLocation = Path.Combine(Environment.SystemDirectory, "mstscax.dll");
            int remoteAppIconGroup = 13417;
            return new ShortcutIcon(GetIconFromGroup(Path.Combine(Environment.SystemDirectory, remoteAppIconLocation), remoteAppIconGroup, IconHelper.ICON_SIZE).ToBitmap());
        }

        public static ShortcutIcon CreateDefaultIcon()
        {
            // Default icon
            String defaultIconLocation = Path.Combine(Environment.SystemDirectory, "shell32.dll");
            int defaultIconIndex = 0;
            ShortcutIcon defaultIcon = new ShortcutIcon(defaultIconLocation, defaultIconIndex);

            // Add remote app icon as overlay for default icon
            defaultIcon.AddIconOverlay(CreateRemoteAppIcon());

            return defaultIcon;
        }
    }
}
