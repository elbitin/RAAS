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
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.Common.Models
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Shares
    {
        [System.Xml.Serialization.XmlElementAttribute("Drive")]
        public List<SharesDrive> Drives { get; set; }
        public string ProfilePath { get; set; }

        public static Shares DeserializeXml(String xmlData)
        {
            return (Shares)SerializationHelper.DeserializeXml(xmlData, typeof(Shares));
        }

        public static Shares DeserializeXmlFileWithRetries(String xmlPath)
        {
            return (Shares)SerializationHelper.DeserializeXmlFileWithRetries(xmlPath,typeof(Shares));
        }

        public string SerializeXml()
        {
            return SerializationHelper.SerializeXml(typeof(Shares), this);
        }

        public bool SerializeXmlFileWithRetries(string shareXmlPath)
        {
            return SerializationHelper.SerializeXmlFileWithRetries(shareXmlPath, typeof(Shares), this);
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SharesDrive
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Share { get; set; }
    }
}
