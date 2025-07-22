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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Elbitin.Applications.RAAS.Common.Helpers;
using Serilog;

namespace Elbitin.Applications.RAAS.Common.Models
{
    public enum ShortcutType
    {
        Desktop,
        StartMenu,
        UWP
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Shortcuts
    {
        public ShortcutsDesktop Desktop { get; set; }
        public ShortcutsStartMenu StartMenu { get; set; }
        public ShortcutsUWP UWP { get; set; }
        public ShortcutsNodeRoot NodeRoot { get; set; }

        public Shortcuts()
        {
            Desktop = new ShortcutsDesktop();
            StartMenu = new ShortcutsStartMenu();
            NodeRoot = new ShortcutsNodeRoot();
            UWP = new ShortcutsUWP();
        }

        public static Shortcuts DeserializeXml(String xmlData)
        {
            return (Shortcuts)SerializationHelper.DeserializeXml(xmlData, typeof(Shortcuts));
        }

        public static Shortcuts DeserializeXmlFileWithRetries(String xmlPath)
        {
            return (Shortcuts)SerializationHelper.DeserializeXmlFileWithRetries(xmlPath, typeof(Shortcuts));
        }

        public string SerializeXml()
        {
            return SerializationHelper.SerializeXml(typeof(Shortcuts), this);
        }

        public bool SerializeXmlFileWithRetries(string shortcutsXmlPath)
        {
            return SerializationHelper.SerializeXmlFileWithRetries(shortcutsXmlPath, typeof(Shortcuts), this);
        }

        public void FilterDuplicates()
        {
            UWP.File = UWP.File.Except(UWP.File.GroupBy(node => node.Path).SelectMany(node => node.Skip(1)).ToList()).ToList();
            UWP.File = UWP.File.Except(UWP.File.GroupBy(node => node.LocalizedName).SelectMany(node => node.Skip(1)).ToList()).ToList();
            UWP.File = UWP.File.Except(UWP.File.GroupBy(node => node.DefaultName).SelectMany(node => node.Skip(1)).ToList()).ToList();
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsNodeRoot
    {
        public string Alias { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsDesktop
    {
        public ShortcutsDesktop()
        {
            Dir = new List<ShortcutsDir>();
            File = new List<ShortcutsFile>();
        }

        [System.Xml.Serialization.XmlElementAttribute("Dir")]
        public List<ShortcutsDir> Dir { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("File")]
        public List<ShortcutsFile> File { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsStartMenu
    {
        public ShortcutsStartMenu()
        {
            Dir = new List<ShortcutsDir>();
            File = new List<ShortcutsFile>();
        }

        [System.Xml.Serialization.XmlElementAttribute("Dir")]
        public List<ShortcutsDir> Dir { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("File")]
        public List<ShortcutsFile> File { get; set; }
    }


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsUWP
    {
        public ShortcutsUWP()
        {
            Dir = new List<ShortcutsDir>();
            File = new List<ShortcutsFile>();
        }

        [System.Xml.Serialization.XmlElementAttribute("Dir")]
        public List<ShortcutsDir> Dir { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("File")]
        public List<ShortcutsFile> File { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsDir
    {
        public string Path { get; set; }
        public string Section { get; set; }
        public string Alias { get; set; }
        public string ClientPath { get; set; }
    }
    
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShortcutsFile
    {
        public string Path { get; set; }
        public string ClientPath { get; set; }
        public string Section { get; set; }
        public string Shortcut { get; set; }
        public string MD5 { get; set; }
        public string ClientMD5 { get; set; }
        public string DefaultName { get; set; }
        public string LocalizedName { get; set; }
        public bool IsLocalized { get; set; }
        public string Alias { get; set; }
        public string Arguments { get; set; }
    }
}
