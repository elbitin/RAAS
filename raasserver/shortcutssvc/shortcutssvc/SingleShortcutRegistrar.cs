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
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    internal class SingleShortcutRegistrar
    {
        public String ShortcutPath { get; set; }
        public String ShortcutLocalizedName { get; set; }
        public String ShortcutDefaultName { get; set; }
        public String ShortcutSection { get; set; }
        public Bitmap ShortcutIcon { get; set; }
        public String IconsPath { get; set; }
        public String Arguments { get; set; }
        public ShortcutType Type { get; set; }
        public void RegisterShortcut(ref Shortcuts shortcuts) {
            ShortcutsFile newFile = new ShortcutsFile();
            newFile.Path = "\\" + Path.GetFileNameWithoutExtension(ShortcutDefaultName) + ".lnk";
            newFile.LocalizedName = Path.GetFileNameWithoutExtension(ShortcutLocalizedName) + ".lnk";
            newFile.DefaultName = Path.GetFileNameWithoutExtension(ShortcutDefaultName) + ".lnk";
            newFile.Section = ShortcutSection;
            newFile.Shortcut = ShortcutPath;
            newFile.Arguments = Arguments;
            if (ShortcutIcon != null)
            {
                ShortcutIcon shortcutIcon = new ShortcutIcon(ShortcutIcon);
                String iconHash = IconHelper.GetBitmapHash(shortcutIcon.IconBitmap);
                newFile.MD5 = iconHash;
                String fileName = Path.GetFileName(ShortcutPath);
                String fileIcon = Path.Combine(IconsPath, IconHelper.GetIconFileName(iconHash, newFile.DefaultName));
                if (!System.IO.File.Exists(fileIcon))
                    try
                    {
                        shortcutIcon.SaveAsIcon(fileIcon);
                    }
                    catch { }
            }
            if (Type == ShortcutType.Desktop)
                shortcuts.Desktop.File.Add(newFile);
            else if (Type == ShortcutType.StartMenu)
                shortcuts.StartMenu.File.Add(newFile);
            else if (Type == ShortcutType.UWP)
                shortcuts.UWP.File.Add(newFile);
        }
    }
}
