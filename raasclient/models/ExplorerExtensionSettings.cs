/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Client.
 *
 * RAAS Client is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Client is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Client. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using System;
using System.Xml;
using System.IO;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASClient.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.Models
{
    public class CouldNotLoadExplorerExtensionSettingsException : Exception
    {
        public CouldNotLoadExplorerExtensionSettingsException()
        {
        }

        public CouldNotLoadExplorerExtensionSettingsException(string message)
            : base(message)
        {
        }

        public CouldNotLoadExplorerExtensionSettingsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    class ExplorerExtensionSettings
    {
        public string ServerName { get; set; }
        public bool ExplorerExtensionActive { get; set; }
        public bool ThreeDObjectsFolder { get; set; }
        public bool ContactsFolder { get; set; }
        public bool DesktopFolder { get; set; }
        public bool DocumentsFolder { get; set; }
        public bool DownloadsFolder { get; set; }
        public bool FavoritesFolder { get; set; }
        public bool LinksFolder { get; set; }
        public bool MusicFolder { get; set; }
        public bool PicturesFolder { get; set; }
        public bool SavedGamesFolder { get; set; }
        public bool SearchesFolder { get; set; }
        public bool VideosFolder { get; set; }
        public bool DisketteDrives { get; set; }
        public bool HardDrives { get; set; }
        public bool CDDrives { get; set; }
        public bool RemovableDrives { get; set; }

        public ExplorerExtensionSettings(String serverName)
        {
            ServerName = serverName;
            if (!ReadExplorerExtensionSettings())
                throw new CouldNotLoadExplorerExtensionSettingsException();
        }

        public bool ReadExplorerExtensionSettings()
        {
            String explorerFile = RAASClientPathHelper.GetExplorerConfigFilePath(ServerName);
            if (File.Exists(explorerFile))
            {
                // Update explorer extension settings according to config
                XmlDocument xmlDoc = new XmlDocument();
                if (!xmlDoc.LoadWithRetries(explorerFile))
                    return false;
                bool active = (xmlDoc.SelectSingleNode("ExplorerExtension/Active").InnerText == "1") ? true : false;
                ExplorerExtensionActive = active;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/ThreeDObjects").InnerText == "1")
                    ThreeDObjectsFolder = true;
                else
                    ThreeDObjectsFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Contacts").InnerText == "1")
                    ContactsFolder = true;
                else
                    ContactsFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Desktop").InnerText == "1")
                    DesktopFolder = true;
                else
                    DesktopFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Documents").InnerText == "1")
                    DocumentsFolder = true;
                else
                    DocumentsFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Downloads").InnerText == "1")
                    DownloadsFolder = true;
                else
                    DownloadsFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Favorites").InnerText == "1")
                    FavoritesFolder = true;
                else
                    FavoritesFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Links").InnerText == "1")
                    LinksFolder = true;
                else
                    LinksFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Music").InnerText == "1")
                    MusicFolder = true;
                else
                    MusicFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Pictures").InnerText == "1")
                    PicturesFolder = true;
                else
                    PicturesFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/SavedGames").InnerText == "1")
                    SavedGamesFolder = true;
                else
                    SavedGamesFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Searches").InnerText == "1")
                    SearchesFolder = true;
                else
                    SearchesFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/Videos").InnerText == "1")
                    VideosFolder = true;
                else
                    VideosFolder = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/DisketteDrives").InnerText == "1")
                    DisketteDrives = true;
                else
                    DisketteDrives = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/HardDrives").InnerText == "1")
                    HardDrives = true;
                else
                    HardDrives = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/CDDrives").InnerText == "1")
                    CDDrives = true;
                else
                    CDDrives = false;
                if (xmlDoc.SelectSingleNode("ExplorerExtension/RemovableDrives").InnerText == "1")
                    RemovableDrives = true;
                else
                    RemovableDrives = false;

                return true;
            }
            return false;
        }

        public void SaveExplorerExtensionSettings()
        {
            String explorerFile = RAASClientPathHelper.GetAppDataExplorerConfigFilePath(ServerName);

            // Create new XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;

            // Create root XML element
            XmlNode rootNode = xmlDoc.CreateElement("ExplorerExtension");
            xmlDoc.AppendChild(rootNode);

            // Store settings in XML
            XmlNode active = xmlDoc.CreateElement("Active");
            if (ExplorerExtensionActive)
                active.InnerText = "1";
            else
                active.InnerText = "0";
            rootNode.AppendChild(active);
            XmlNode threeDObjects = xmlDoc.CreateElement("ThreeDObjects");
            if (ThreeDObjectsFolder)
                threeDObjects.InnerText = "1";
            else
                threeDObjects.InnerText = "0";
            rootNode.AppendChild(threeDObjects);
            XmlNode contacts = xmlDoc.CreateElement("Contacts");
            if (ContactsFolder)
                contacts.InnerText = "1";
            else
                contacts.InnerText = "0";
            rootNode.AppendChild(contacts);
            XmlNode desktop = xmlDoc.CreateElement("Desktop");
            if (DesktopFolder)
                desktop.InnerText = "1";
            else
                desktop.InnerText = "0";
            rootNode.AppendChild(desktop);
            XmlNode documents = xmlDoc.CreateElement("Documents");
            if (DocumentsFolder)
                documents.InnerText = "1";
            else
                documents.InnerText = "0";
            rootNode.AppendChild(documents);
            XmlNode downloads = xmlDoc.CreateElement("Downloads");
            if (DownloadsFolder)
                downloads.InnerText = "1";
            else
                downloads.InnerText = "0";
            rootNode.AppendChild(downloads);
            XmlNode favorites = xmlDoc.CreateElement("Favorites");
            if (FavoritesFolder)
                favorites.InnerText = "1";
            else
                favorites.InnerText = "0";
            rootNode.AppendChild(favorites);
            XmlNode links = xmlDoc.CreateElement("Links");
            if (LinksFolder)
                links.InnerText = "1";
            else
                links.InnerText = "0";
            rootNode.AppendChild(links);
            XmlNode music = xmlDoc.CreateElement("Music");
            if (MusicFolder)
                music.InnerText = "1";
            else
                music.InnerText = "0";
            rootNode.AppendChild(music);
            XmlNode pictures = xmlDoc.CreateElement("Pictures");
            if (PicturesFolder)
                pictures.InnerText = "1";
            else
                pictures.InnerText = "0";
            rootNode.AppendChild(pictures);
            XmlNode savedGames = xmlDoc.CreateElement("SavedGames");
            if (SavedGamesFolder)
                savedGames.InnerText = "1";
            else
                savedGames.InnerText = "0";
            rootNode.AppendChild(savedGames);
            XmlNode searches = xmlDoc.CreateElement("Searches");
            if (SearchesFolder)
                searches.InnerText = "1";
            else
                searches.InnerText = "0";
            rootNode.AppendChild(searches);
            XmlNode videos = xmlDoc.CreateElement("Videos");
            if (VideosFolder)
                videos.InnerText = "1";
            else
                videos.InnerText = "0";
            rootNode.AppendChild(videos);
            XmlNode disketteDrives = xmlDoc.CreateElement("DisketteDrives");
            if (DisketteDrives)
                disketteDrives.InnerText = "1";
            else
                disketteDrives.InnerText = "0";
            rootNode.AppendChild(disketteDrives);
            XmlNode hardDrives = xmlDoc.CreateElement("HardDrives");
            if (HardDrives)
                hardDrives.InnerText = "1";
            else
                hardDrives.InnerText = "0";
            rootNode.AppendChild(hardDrives);
            XmlNode cdDrives = xmlDoc.CreateElement("CDDrives");
            if (CDDrives)
                cdDrives.InnerText = "1";
            else
                cdDrives.InnerText = "0";
            rootNode.AppendChild(cdDrives);
            XmlNode removableDrives = xmlDoc.CreateElement("RemovableDrives");
            if (RemovableDrives)
                removableDrives.InnerText = "1";
            else
                removableDrives.InnerText = "0";
            rootNode.AppendChild(removableDrives);
            xmlDoc.Save(explorerFile);
        }
    }
}
