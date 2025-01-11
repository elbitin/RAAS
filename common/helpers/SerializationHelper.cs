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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    static class SerializationHelper
    {
        public static object DeserializeXml(String xmlData, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            TextReader reader = new StringReader(xmlData);
            object deseralizedXmlFile = serializer.Deserialize(reader);
            reader.Close();
            return deseralizedXmlFile;
        }

        public static object DeserializeXmlFile(String xmlPath, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            FileStream fileStream = new FileStream(xmlPath, FileMode.Open);
            object deseralizedXmlFile = serializer.Deserialize(fileStream);
            fileStream.Close();
            return deseralizedXmlFile;
        }

        public static string SerializeXml(Type type, object obj)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            String xml = "";
            StringWriter stringWriter = new StringWriter();
            XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings);
            xmlSerializer.Serialize(xmlWriter, obj);
            xml = stringWriter.ToString();
            xmlWriter.Close();
            stringWriter.Close();
            return xml;
        }

        public static bool SerializeXmlFile(string xmlPath, Type type, object obj)
        {
            try
            {
                string xml = SerializeXml(type, obj);
                File.WriteAllText(xmlPath, xml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
