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
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.Common.Models;
using System;
using System.Collections.Generic;

namespace Elbitin.Applications.RAAS.RAASServer.Models
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AppList
    {
        public static AppList DeserializeXml(String xmlData)
        {
            return (AppList)SerializationHelper.DeserializeXml(xmlData, typeof(AppList));
        }

        public static AppList DeserializeXmlFileWithRetries(String xmlPath)
        {
            return (AppList)SerializationHelper.DeserializeXmlFileWithRetries(xmlPath, typeof(AppList));
        }

        public string SerializeXml()
        {
            return SerializationHelper.SerializeXml(typeof(AppList), this);
        }

        public bool SerializeXmlFileWithRetries(string appListXmlPath)
        {
            return SerializationHelper.SerializeXmlFileWithRetries(appListXmlPath, typeof(AppList), this);
        }

        private AppListApp[] appField;
        private AppListShortcut[] shorcutField; 

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("App")]
        public AppListApp[] App
        {
            get
            {
                return this.appField;
            }
            set
            {
                this.appField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Shortcut")]
        public AppListShortcut[] Shortcut
        {
            get
            {
                return this.shorcutField;
            }
            set
            {
                this.shorcutField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AppListShortcut
    {

        private string shortcutPathField;

        private string displayNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string shortcutPath
        {
            get
            {
                return this.shortcutPathField;
            }
            set
            {
                this.shortcutPathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }
    }
    public partial class AppListApp
    {

        private string appUserModelIdField;

        private string displayNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AppUserModelId
        {
            get
            {
                return this.appUserModelIdField;
            }
            set
            {
                this.appUserModelIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }
    }
}