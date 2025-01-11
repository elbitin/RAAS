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
﻿using Elbitin.Applications.RAAS.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Elbitin.Applications.RAAS.Common.Helpers;
using Elbitin.Applications.RAAS.RAASServer.Helpers;
using Elbitin.Applications.RAAS.RAASServer.Models;
using Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc;
using System.Resources;
using System.Globalization;
using PriFormat;
using System.Collections.ObjectModel;
using System.Drawing;
using Serilog;
using System.Diagnostics.Metrics;
using static Elbitin.Applications.RAAS.RAASServer.Helpers.AppListHelper;
using static Elbitin.Applications.RAAS.Common.Helpers.LinkHelper;
using System.Linq.Expressions;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    internal class UWPAppsRegistrar
    {
        public String IconsDirPath { get; set; }
        public String AssociationsDirPath { get; set; }
        public String Section { get; set; }
        public String ShortcutsXmlFilePath { get; set; }
        public String AppNamesXmlPath { get; set; }
        public String AssociationsXmlPath { get; set; }
        public ShortcutType Type { get; set; }
        private Runspace runspace { get; set; }
        private const int SHORTCUTS_XML_RETRY_COUNT = 200;
        private const int SHORTCUTS_XML_RETRY_INTERVAL_MS = 200;
        private const int ASSOCIATIONS_XML_RETRY_COUNT = 200;
        private const int ASSOCIATIONS_XML_RETRY_INTERVAL_MS = 200;
        private const int APPNAMES_XML_RETRY_COUNT = 200;
        private const int APPNAMES_XML_RETRY_INTERVAL_MS = 200;
        private const int ICON_SIZE = 48;
        private List<AppxApplication> appxApplicationList = new List<AppxApplication>();
        private List<AppxProperties> appxPropertyList = new List<AppxProperties>();
        private String priFileNameStart = "resources";
        private String languageStart = "language-";
        private AppList appNames = null;

        public List<String> GetEnglishCultureKeyStarts()
        {
            List<String> cultureKeyStarts = new List<String>();
            cultureKeyStarts.Add("en.");
            cultureKeyStarts.Add("en-US");
            cultureKeyStarts.Add("en-GB");
            cultureKeyStarts.Add("en");
            return cultureKeyStarts;
        }

        public List<String> GetCultureKeyStarts(String cultureKey)
        {
            List<String> cultureKeyStarts = new List<String>();

            // Match for cultures
            cultureKeyStarts.Add(cultureKey);
            if (cultureKey.StartsWith("en"))
            {
                cultureKeyStarts.AddRange(GetEnglishCultureKeyStarts());
            }
            else
            {
                cultureKeyStarts.Add(cultureKey.Split('-')[0] + ".");
                cultureKeyStarts.Add(cultureKey.Split('-')[0] + "-");
                cultureKeyStarts.Add(cultureKey.Split('-')[0]);
                cultureKeyStarts.AddRange(GetEnglishCultureKeyStarts());
            }
            return cultureKeyStarts;
        }

        public String MatchPriFileCulture(List<String> priFiles, List<String> cultureKeyStarts)
        {
            foreach (String cultureKeyStart in cultureKeyStarts)
            {
                foreach (String file in priFiles)
                {
                    if (Path.GetFileName(file).ToLowerInvariant().StartsWith(priFileNameStart + "." + cultureKeyStart.ToLowerInvariant()))
                        return file;
                    if (Path.GetFileName(file).ToLowerInvariant().StartsWith(priFileNameStart + "." + languageStart + cultureKeyStart.ToLowerInvariant()))
                        return file;
                }
            }
            return null;
        }

        public String GetLocalizedPriFilePath(String installLocation, String cultureKey)
        {
            List<String> cultureKeyStarts = new List<String>();

            // Get for cultures
            cultureKeyStarts = GetCultureKeyStarts(cultureKey);

            // Match pri files in install location
            List<String> priFiles = new List<string>();
            foreach (String file in Directory.GetFiles(installLocation))
            {
                if (Path.GetFileName(file).ToLowerInvariant().StartsWith(priFileNameStart))
                {
                    priFiles.Add(file);
                }
            }
            String result = MatchPriFileCulture(priFiles, cultureKeyStarts);
            if (result != null)
                return result;

            // Match pri files in install location pri sub folder
            priFiles.Clear();
            String prisSubDir = Path.Combine(installLocation, "pris");
            if (Directory.Exists(prisSubDir))
                foreach (String file in Directory.GetFiles(prisSubDir))
                {
                    if (Path.GetFileName(file).ToLowerInvariant().StartsWith(priFileNameStart))
                    {
                        priFiles.Add(file);
                    }
                }
            result = MatchPriFileCulture(priFiles, cultureKeyStarts);
            if (result != null)
                return result;

            // Return default pri file
            return Path.Combine(installLocation, priFileNameStart + ".pri");
        }

        public void RegisterUWPApps()
        {
            int count = 0;
            bool exception = false;
            Shortcuts shortcuts;
            do
            {
                try
                {
                    shortcuts = Shortcuts.DeserializeXmlFile(ShortcutsXmlFilePath);
                    RegisterUWPApps(ref shortcuts);
                    exception = false;
                }
                catch
                {
                    Thread.Sleep(SHORTCUTS_XML_RETRY_INTERVAL_MS);
                    count++;
                    exception = true;
                }
            } while (exception && count < SHORTCUTS_XML_RETRY_COUNT);
        }

        public void ParseAppNames()
        {
            int count = 0;
            bool exception = false;
            do
            {
                try
                {
                    appNames = AppListHelper.ParseAppNames(AppNamesXmlPath);
                    exception = false;
                }
                catch
                {
                    Thread.Sleep(APPNAMES_XML_RETRY_INTERVAL_MS);
                    count++;
                    exception = true;
                }
            } while (exception && count < APPNAMES_XML_RETRY_COUNT);
            if (exception)
                throw new Exception("App names could not be parsed");
        }

        public void RegisterUWPApps(ref Shortcuts shortcuts)
        {
            PowerShell invoker = PowerShell.Create();
            string script1 = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted -force;";
            invoker.AddScript(script1);
            invoker.Invoke();
            InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.ImportPSModule("WindowsCompatibility");
            runspace = RunspaceFactory.CreateRunspace(initialSessionState);
            runspace.Open();
            invoker = PowerShell.Create(runspace);
            invoker.AddScript("Get-Command Import-Module -ParameterName UseWindowsPowershell");
            var result = invoker.Invoke();
            bool success = success = !invoker.HadErrors;
            if (!success)
                throw new Exception("Get-Command Import-Module -ParameterName UseWindowsPowershell error");
            if (result.Count() > 0)
            {
                invoker = PowerShell.Create(runspace);
                invoker.AddScript("Import-Module appx -UseWindowsPowershell");
                result = invoker.Invoke();
                success = !invoker.HadErrors;
                if (!success)
                    throw new Exception("Import-Module appx -UseWindowsPowershell error");
            }
            else
            {
                invoker = PowerShell.Create(runspace);
                invoker.AddScript("Import-Module appx");
                result = invoker.Invoke();
                success = !invoker.HadErrors;
                if (!success)
                    throw new Exception("Import-Module appx error");
            }
            ParseAppNames();
            //if (!(appNames.App.Count() > 0))
            //    throw new Exception("UWP appNames.App empty");
            invoker = PowerShell.Create(runspace);
            var appxList = invoker.AddCommand("Get-AppxPackage").AddParameter("AllUsers", true).Invoke();
            success = !invoker.HadErrors;
            if (!success)
                throw new Exception("Get-AppxPackage -AllUsers error");
            ParseAppxProperties(appxList);
            ParseAppxApplications();
            if (appxList.Count == 0)
                throw new Exception("Get-AppxPackage -AllUsers error appxList.Count()=0");
            RegisterAppxApplications(ref shortcuts);
            RegisterDefaultAssociations();
            shortcuts.FilterDuplicates();
            shortcuts.SerializeXmlFile(ShortcutsXmlFilePath);
        }

        void ParseAppxProperties(System.Collections.ObjectModel.Collection<PSObject> appxList)
        {
            appxPropertyList.Clear();
            foreach (var appxObject in appxList)
            {
                AppxProperties appxProperties = new AppxProperties();
                foreach (var psProperty in appxObject.Properties)
                {
                    switch (psProperty.Name)
                    {
                        case "InstallLocation":
                            if (psProperty.Value != null)
                            {
                                appxProperties.InstallLocation = psProperty.Value.ToString();
                            }
                            break;
                        case "Name":
                            if (psProperty.Value != null)
                            {
                                appxProperties.Name = psProperty.Value.ToString();
                            }
                            break;
                        case "PackageFamilyName":
                            if (psProperty.Value != null)
                            {
                                appxProperties.PackageFamilyName = psProperty.Value.ToString();
                            }
                            break;
                        case "PackageFullName":
                            if (psProperty.Value != null)
                            {
                                appxProperties.PackageFullName = psProperty.Value.ToString();
                            }
                            break;
                        default: break;
                    }
                }
                appxPropertyList.Add(appxProperties);
            }
        }

        void RegisterDefaultAssociations()
        {
            try
            {
                List<String> existingIconFiles = new List<string>();
                String userName = ProfileHelper.GetUserFromProfilePath(IconsDirPath);
                Dictionary<String, String> extensions = RegistryHelper.GetDefaultApplications(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts", userName);
                // Update icon files
                foreach (var key in extensions.Keys)
                {
                    try
                    {
                        String appIconString = RegistryHelper.GetKeyAppIconString(extensions[key], userName);
                        if (appIconString == null)
                        {
                            String intermediate = RegistryHelper.RegistryValueToString("Software\\Classes\\" + extensions[key] + "\\CurVer", null, userName);
                            appIconString = RegistryHelper.GetKeyAppIconString(intermediate, userName);
                        }
                        if (appIconString != null && appIconString.StartsWith("@{"))
                        {
                            String appString = appIconString.Substring(2, appIconString.Length - 3);
                            foreach (var appx in appxPropertyList)
                            {
                                if (appx.PackageFullName != null && appx.PackageFullName == appString.Split('?')[0])
                                {
                                    String priFileName = "resources.pri";
                                    String defaultPriPath = (File.Exists(Path.Combine(appx.InstallLocation, priFileName)) ? Path.Combine(appx.InstallLocation, priFileName) : null);
                                    if (appString.Split('?')[1].StartsWith("ms-resource:"))
                                    {
                                        String iconString = appString.Split('?')[1].Split("ms-resource:")[1].Split('}')[0];
                                        String displayName = iconString.Substring(appx.Name.Length + 3);
                                        ShortcutIcon shortcutIcon = new ShortcutIcon(GetBitmap(defaultPriPath, appx.InstallLocation, displayName));
                                        String iconFilePath = Path.Combine(RAASServerPathHelper.GetUserRAASServerIconsAssociationsPath(IconsDirPath), key.ToLowerInvariant() + "." + IconHelper.GetBitmapHash(shortcutIcon.IconBitmap) + ".ico");
                                        if (!File.Exists(iconFilePath))
                                            shortcutIcon.SaveAsIcon(iconFilePath);
                                        existingIconFiles.Add(iconFilePath);
                                    }
                                }
                            }
                        }
                        else
                        {
                            LinkDetails linkDetails = new LinkDetails();
                            linkDetails.UserName = userName;
                            RegistryHelper.GetKeyFileIconDetails(extensions[key], ref linkDetails);
                            if (linkDetails.IconLocation.ToLowerInvariant().EndsWith(".exe") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".dll") || linkDetails.IconLocation.ToLowerInvariant().EndsWith(".cpl"))
                                try
                                {
                                    Bitmap bitmap = IconHelper.GetShellShortcutIconBitmap(linkDetails.IconLocation, linkDetails.IconIndex);
                                    ShortcutIcon icon = new ShortcutIcon(bitmap);
                                    String iconFilePath = Path.Combine(RAASServerPathHelper.GetUserRAASServerIconsAssociationsPath(IconsDirPath), key.ToLowerInvariant() + "." + IconHelper.GetBitmapHash(icon.IconBitmap) + ".ico");
                                    if (!File.Exists(iconFilePath))
                                        icon.SaveAsIcon(iconFilePath);
                                    existingIconFiles.Add(iconFilePath);
                                }
                                catch { }
                        }
                    }
                    catch { }
                }

                // Delete not used icon files
                foreach (String file in Directory.GetFiles(RAASServerPathHelper.GetUserRAASServerIconsAssociationsPath(IconsDirPath)))
                {
                    if (!existingIconFiles.Contains(file))
                        File.Delete(file);
                }
            }catch { }
        }

        void ParseAppxApplications()
        {
            appxApplicationList.Clear();
            foreach (var appxProperties in appxPropertyList)
            {
                if (appxProperties.InstallLocation != null)
                {
                    try
                    {
                        String appxManiefstXML = File.ReadAllText(Path.Combine(appxProperties.InstallLocation, "appxmanifest.xml"));
                        Package appxManifest = Package.DeserializeXml(appxManiefstXML);
                        if (appxManifest.Applications != null)
                        {
                            foreach (var appxApplicationManifest in appxManifest.Applications)
                            {
                                if (appxApplicationManifest.Id != null && appxApplicationManifest.Executable != null && appxApplicationManifest.VisualElements != null && appxApplicationManifest.VisualElements.AppListEntry != "none")
                                {
                                    AppxApplication appxApplication = new AppxApplication();
                                    String priFileName = "resources.pri";
                                    String defaultPriPath = (File.Exists(Path.Combine(appxProperties.InstallLocation, priFileName)) ? Path.Combine(appxProperties.InstallLocation, priFileName) : null);
                                    if (appxApplicationManifest.VisualElements.DisplayName.StartsWith("ms-resource:"))
                                    {
                                        String displayName = (appxApplicationManifest.VisualElements.DisplayName.Split("ms-resource:")[1].StartsWith("//" + appxManifest.Identity.Name) ? appxApplicationManifest.VisualElements.DisplayName.Split("ms-resource:")[1].Substring(appxManifest.Identity.Name.Length + 3) : appxApplicationManifest.VisualElements.DisplayName.Split("ms-resource:")[1]);
                                        appxApplication.DefaultPriFilePath = defaultPriPath;
                                        appxApplication.EnglishPriFilePath = GetLocalizedPriFilePath(appxProperties.InstallLocation, "en-US");
                                        appxApplication.DefaultDisplayName = GetEntry(appxApplication.EnglishPriFilePath, displayName, "en-US");

                                    }
                                    else
                                    {
                                        appxApplication.DefaultDisplayName = appxApplicationManifest.VisualElements.DisplayName;
                                    }
                                    try
                                    {
                                        appxApplication.LocalizedDisplayName = appNames.App.Where(x => (x.AppUserModelId.Contains("!") && x.AppUserModelId.StartsWith(appxProperties.PackageFamilyName) && x.AppUserModelId.Split('!')[1] == appxApplicationManifest.Id) || (!x.AppUserModelId.Contains("!") && x.AppUserModelId.StartsWith(appxProperties.PackageFamilyName))).FirstOrDefault().DisplayName;
                                    }
                                    catch
                                    {
                                        appxApplication.LocalizedDisplayName = appxApplication.DefaultDisplayName;
                                    }
                                    appxApplication.InstallLocation = appxProperties.InstallLocation;
                                    appxApplication.IconIdentifier = (appxApplicationManifest.VisualElements.Square44x44Logo != null ? appxApplicationManifest.VisualElements.Square44x44Logo : appxApplicationManifest.VisualElements.Square150x150Logo);
                                    if (defaultPriPath != null)
                                        appxApplication.Icon = GetBitmap(defaultPriPath, appxProperties.InstallLocation, appxApplication.IconIdentifier);
                                    else
                                        appxApplication.Icon = new Bitmap(Path.Combine(appxProperties.InstallLocation, (appxApplicationManifest.VisualElements.Square150x150Logo != null ? appxApplicationManifest.VisualElements.Square150x150Logo : appxApplicationManifest.VisualElements.Square44x44Logo).Replace('/', '\\')));
                                    appxApplication.Executable = appxApplicationManifest.Executable;
                                    appxApplication.AppId = appxApplicationManifest.Id;
                                    appxApplication.PackageFamilyName = appxProperties.PackageFamilyName;
                                    appxApplicationList.Add(appxApplication);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private String GetEntry(String priFilePath, String itemName, String cultureKey)
        {
            List<String> cultureKeyStarts = GetCultureKeyStarts(cultureKey); ;
            using (FileStream stream = File.OpenRead(priFilePath))
            {
                PriFile priFile = PriFile.Parse(stream);
                foreach (var resourceMapSectionRef in priFile.PriDescriptorSection.ResourceMapSections)
                {
                    ResourceMapSection resourceMapSection = priFile.GetSectionByRef(resourceMapSectionRef);
                    if (resourceMapSection.HierarchicalSchemaReference != null)
                        continue;
                    DecisionInfoSection decisionInfoSection = priFile.GetSectionByRef(resourceMapSection.DecisionInfoSection);

                    foreach (var candidateSet in resourceMapSection.CandidateSets.Values)
                    {
                        ResourceMapItem item = priFile.GetResourceMapItemByRef(candidateSet.ResourceMapItem);
                        String itemFullName = (itemName.Contains("/") ? "\\" + itemName.Replace('/', '\\') : "\\" + itemName);
                        String resourcesItemFullName = "\\Resources" + itemFullName;
                        if (item.FullName.Contains("AppName"))
                            Console.WriteLine(itemName);
                        if (item.FullName.ToLowerInvariant() == itemFullName.ToLowerInvariant() || item.FullName.ToLowerInvariant() == resourcesItemFullName.ToLowerInvariant())
                        {
                            foreach (String cultureKeyStart in cultureKeyStarts)
                            {
                                foreach (var candidate in candidateSet.Candidates)
                                {
                                    QualifierSet qualifierSet = decisionInfoSection.QualifierSets[candidate.QualifierSet];
                                    bool cultureMatch = false;
                                    foreach (Qualifier q in qualifierSet.Qualifiers)
                                        if (q.Type.ToString() == "Language")
                                            if (q.Value.ToLowerInvariant().StartsWith(cultureKeyStart.ToLowerInvariant()))
                                                cultureMatch = true;
                                    if (!cultureMatch)
                                        continue;
                                    string value = null;
                                    if (candidate.SourceFile != null)
                                        value = string.Format("<external in {0}>", priFile.GetReferencedFileByRef(candidate.SourceFile.Value).FullName);
                                    else
                                    {
                                        ByteSpan byteSpan;

                                        if (candidate.DataItem != null)
                                            byteSpan = priFile.GetDataItemByRef(candidate.DataItem.Value);
                                        else
                                            byteSpan = candidate.Data.Value;

                                        stream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                        byte[] data;

                                        using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, true))
                                            data = binaryReader.ReadBytes((int)byteSpan.Length);
                                        switch (candidate.Type)
                                        {
                                            case ResourceValueType.AsciiPath:
                                            case ResourceValueType.AsciiString:
                                                value = Encoding.ASCII.GetString(data).TrimEnd('\0');
                                                break;
                                            case ResourceValueType.Utf8Path:
                                            case ResourceValueType.Utf8String:
                                                value = Encoding.UTF8.GetString(data).TrimEnd('\0');
                                                break;
                                            case ResourceValueType.Path:
                                            case ResourceValueType.String:
                                                value = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                                break;
                                            case ResourceValueType.EmbeddedData:
                                                value = string.Format("<{0} bytes>", data.Length);
                                                break;
                                        }
                                    }
                                    return value;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        private Bitmap GetBitmap(String priFilePath, String installLocation, String itemName, bool maxSize = false)
        {
            using (FileStream stream = File.OpenRead(priFilePath))
            {
                PriFile priFile = PriFile.Parse(stream);
                foreach (var resourceMapSectionRef in priFile.PriDescriptorSection.ResourceMapSections)
                {
                    ResourceMapSection resourceMapSection = priFile.GetSectionByRef(resourceMapSectionRef);
                    if (resourceMapSection.HierarchicalSchemaReference != null)
                        continue;
                    DecisionInfoSection decisionInfoSection = priFile.GetSectionByRef(resourceMapSection.DecisionInfoSection);

                    foreach (var candidateSet in resourceMapSection.CandidateSets.Values)
                    {
                        ResourceMapItem item = priFile.GetResourceMapItemByRef(candidateSet.ResourceMapItem);
                        String itemFullName = (itemName.Contains("/") ? "\\" + itemName.Replace('/', '\\') : "\\" + itemName);
                        String resourcesItemFullName = "\\Files" + itemFullName;
                        if (item.FullName.ToLowerInvariant() == itemFullName.ToLowerInvariant() || item.FullName.ToLowerInvariant() == resourcesItemFullName.ToLowerInvariant())
                        {
                            int maxTargetSize = 0;
                            foreach (var candidate in candidateSet.Candidates)
                            {
                                QualifierSet qualifierSet = decisionInfoSection.QualifierSets[candidate.QualifierSet];
                                if (maxSize)
                                    foreach (Qualifier q in qualifierSet.Qualifiers)
                                        if (q.Type.ToString() == "TargetSize")
                                            if (maxTargetSize < Convert.ToInt32(q.Value.Split("x")[0]))
                                                maxTargetSize = Convert.ToInt32(q.Value.Split("x")[0]);

                                bool skip = false;
                                foreach (Qualifier q in qualifierSet.Qualifiers)
                                {
                                    if (q.Type.ToString() == "Contrast")
                                        skip = true;

                                    if (maxSize)
                                    {
                                        if (q.Type.ToString() == "TargetSize")
                                            if (maxTargetSize > Convert.ToInt32(q.Value.Split("x")[0]))
                                                skip = true;
                                    }
                                    else
                                    {
                                        if (q.Type.ToString() == "TargetSize")
                                            if (ICON_SIZE != Convert.ToInt32(q.Value.Split("x")[0]))
                                                skip = true;
                                    }
                                }
                                if (skip)
                                    continue;
                                string value = null;
                                if (candidate.SourceFile != null)
                                    value = string.Format("<external in {0}>", priFile.GetReferencedFileByRef(candidate.SourceFile.Value).FullName);
                                else
                                {
                                    ByteSpan byteSpan;

                                    if (candidate.DataItem != null)
                                        byteSpan = priFile.GetDataItemByRef(candidate.DataItem.Value);
                                    else
                                        byteSpan = candidate.Data.Value;

                                    stream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                    byte[] data;

                                    using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, true))
                                        data = binaryReader.ReadBytes((int)byteSpan.Length);
                                    string rootPath;
                                    byte[] byteData = null;
                                    string externalFilePath = null;
                                    rootPath = installLocation;
                                    switch (candidate.Type)
                                    {
                                        case ResourceValueType.Path:
                                            externalFilePath = Path.Combine(rootPath, Encoding.Unicode.GetString(data).TrimEnd('\0'));
                                            break;
                                        case ResourceValueType.AsciiPath:
                                            externalFilePath = Path.Combine(rootPath, Encoding.ASCII.GetString(data).TrimEnd('\0'));
                                            break;
                                        case ResourceValueType.Utf8Path:
                                            externalFilePath = Path.Combine(rootPath, Encoding.UTF8.GetString(data).TrimEnd('\0'));
                                            break;
                                        case ResourceValueType.Utf8String:
                                            value = Encoding.UTF8.GetString(data).TrimEnd('\0');
                                            break;
                                        case ResourceValueType.String:
                                            value = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                            break;
                                        case ResourceValueType.EmbeddedData:

                                            break;
                                    }
                                    if (File.Exists(externalFilePath))
                                    {
                                        byteData = File.ReadAllBytes(externalFilePath);
                                        Bitmap bitmap = new Bitmap(new MemoryStream(byteData, false));
                                        return bitmap;
                                    }
                                    else
                                        return null;

                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Shortcuts RegisterAppxApplications(ref Shortcuts shortcuts)
        {
            shortcuts.UWP.File.Clear();
            foreach (var appxApplication in appxApplicationList)
            {
                SingleShortcutRegistrar uwpShortcutRegistrar = new SingleShortcutRegistrar();
                uwpShortcutRegistrar.IconsPath = IconsDirPath;
                uwpShortcutRegistrar.ShortcutPath = Environment.ExpandEnvironmentVariables("%WINDIR%\\explorer.exe");
                uwpShortcutRegistrar.Arguments = "shell:AppsFolder\\" + appxApplication.PackageFamilyName + "!" + appxApplication.AppId;
                uwpShortcutRegistrar.ShortcutSection = Section;
                uwpShortcutRegistrar.ShortcutLocalizedName = appxApplication.AppId;
                uwpShortcutRegistrar.ShortcutDefaultName = appxApplication.AppId;
                uwpShortcutRegistrar.ShortcutIcon = appxApplication.Icon;
                uwpShortcutRegistrar.ShortcutDefaultName = (appxApplication.DefaultDisplayName != null ? appxApplication.DefaultDisplayName : appxApplication.LocalizedDisplayName);
                uwpShortcutRegistrar.ShortcutLocalizedName = (appxApplication.LocalizedDisplayName != null ? appxApplication.LocalizedDisplayName : appxApplication.DefaultDisplayName);
                uwpShortcutRegistrar.Type = ShortcutType.UWP;
                uwpShortcutRegistrar.RegisterShortcut(ref shortcuts);
            }
            return shortcuts;
        }
    }
}
