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
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    static class SerializationHelper
    {
        private const int DEFAULT_RETRY_LOAD_XML_COUNT = 30;
        private const int DEFAULT_RETRY_SAVE_XML_COUNT = 30;
        private const int DEFAULT_RETRY_LOAD_XML_INTERVAL_MS = 50;
        private const int DEFAULT_RETRY_SAVE_XML_INTERVAL_MS = 50;

        public static bool LoadWithRetries(this XmlDocument xmlDoc, String path)
        {
            return LoadWithRetries(xmlDoc, path, DEFAULT_RETRY_LOAD_XML_COUNT, DEFAULT_RETRY_LOAD_XML_INTERVAL_MS);
        }

        public static bool LoadWithRetries(this XmlDocument xmlDoc, String path, int retryCount, int retryIntervalMs)
        {
            bool xmlDocLoaded = false;
            for (int i = 0; i < retryCount; i++)
                try
                {
                    xmlDoc.Load(path);
                    xmlDocLoaded = true;
                    break;
                }
                catch
                {
                    Thread.Sleep(retryIntervalMs);
                }
            return xmlDocLoaded;
        }

        public static bool SaveWithRetries(this XmlDocument xmlDoc, String path, int retryCount, int retryIntervalMs)
        {
            bool xmlDocLoaded = false;
            for (int i = 0; i < retryCount; i++)
                try
                {
                    xmlDoc.Load(path);
                    xmlDocLoaded = true;
                    break;
                }
                catch
                {
                    Thread.Sleep(retryIntervalMs);
                }
            return xmlDocLoaded;
        }

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

        public static object DeserializeXmlFileWithRetries(String xmlPath, Type type)
        {
            for (int i = 0; i < DEFAULT_RETRY_LOAD_XML_COUNT; i++)
            {
                try
                {
                    return DeserializeXmlFile(xmlPath, type);
                }
                catch
                {
                    Thread.Sleep(DEFAULT_RETRY_LOAD_XML_INTERVAL_MS);
                }
            }
            return null;
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

        public static bool SerializeXmlFileWithRetries(String xmlPath, Type type, object obj)
        {
            for (int i = 0; i < DEFAULT_RETRY_SAVE_XML_COUNT; i++)
            {
                try
                {
                    return SerializeXmlFile(xmlPath, type, obj);
                }
                catch
                {
                    Thread.Sleep(DEFAULT_RETRY_SAVE_XML_INTERVAL_MS);
                }
            }
            return false;
        }
    }
}
