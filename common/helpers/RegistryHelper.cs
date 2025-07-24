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
using System.Security.Principal;
using Microsoft.Win32;
using static Elbitin.Applications.RAAS.Common.Helpers.LinkHelper;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    public class InstallPathNotFoundException : Exception
    {
        public InstallPathNotFoundException()
        {
        }

        public InstallPathNotFoundException(string message)
            : base(message)
        {
        }

        public InstallPathNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class RegistryValueNotFoundException : Exception
    {
        public RegistryValueNotFoundException()
        {
        }

        public RegistryValueNotFoundException(string message)
            : base(message)
        {
        }

        public RegistryValueNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class RegistryHelper
    {
        private const string CLASSES_SUBKEY = @"SOFTWARE\Classes";
        public static Dictionary<String, String> GetDefaultApplications(String keyPath, String userName)
        {
            Dictionary<String,String> extensions = new Dictionary<String,String>();
            RegistryKey lmKey = Registry.LocalMachine;
            var lmKeySubKeys = lmKey.GetSubKeyNames();
            foreach (var subKey in lmKeySubKeys)
            {
                try
                {
                    String appId = lmKey.OpenSubKey(keyPath)?.OpenSubKey(subKey)?.OpenSubKey("UserChoice")?.GetValue("ProgId").ToString();
                    if (appId != null)
                        extensions[subKey] = lmKey.OpenSubKey(keyPath)?.OpenSubKey(subKey)?.OpenSubKey("UserChoice")?.GetValue("ProgId").ToString();
                }
                catch { }
            }
            String sidString = GetUserSid(userName);
            Microsoft.Win32.RegistryKey usersKey = Microsoft.Win32.Registry.Users.OpenSubKey(sidString);
            var usersKeySubKeys = usersKey.OpenSubKey(keyPath).GetSubKeyNames();
            foreach (var subKey in usersKeySubKeys)
            {
                try
                {
                    String appId = usersKey.OpenSubKey(keyPath)?.OpenSubKey(subKey)?.OpenSubKey("UserChoice")?.GetValue("ProgId").ToString();
                    if (appId != null)
                        extensions[subKey] = usersKey.OpenSubKey(keyPath)?.OpenSubKey(subKey)?.OpenSubKey("UserChoice")?.GetValue("ProgId").ToString();
                }
                catch { }
            }
            return extensions;
        }

        public static bool GetKeyFileIconDetails(String classKey, ref LinkHelper.LinkDetails linkDetails)
        {
            Object obj = null;
            obj = GetClassesRootValue(classKey + @"\shellex\IconHandler", null, linkDetails.UserName);
            if (obj != null)
            {
                linkDetails.IconHandlerClsid = obj.ToString();
                obj = GetClassesRootValue("CLSID\\" + linkDetails.IconHandlerClsid + "\\InprocServer32", null, linkDetails.UserName);
                if (obj != null)
                    linkDetails.IconHandlerLibraryPath = Environment.ExpandEnvironmentVariables(obj.ToString());
            }
            obj = GetClassesRootValue(classKey + @"\DefaultIcon", null, linkDetails.UserName);
            if (obj != null)
            {
                linkDetails.IconLocation = obj.ToString().Split(',')[0].Replace("%1", linkDetails.TargetPath);
                if (linkDetails.IconLocation.StartsWith("\""))
                    linkDetails.IconLocation = linkDetails.IconLocation.Split('"')[1];
                if (obj.ToString().Split(',').Length > 1)
                    linkDetails.IconIndex = Convert.ToInt32(obj.ToString().Split(',')[1]);
                return true;
            }
            if (linkDetails.IconLocation.Length == 0 && linkDetails.IconHandlerLibraryPath.Length > 0)
            {
                linkDetails.IconLocation = linkDetails.TargetPath;
                linkDetails.IconIndex = 0;
                return true;
            }
            else
                return false;
        }

        public static String GetKeyAppIconString(String classKey, String userName)
        {
            Object obj = null;
            obj = GetClassesRootValue(classKey + @"\DefaultIcon", null, userName);
            if (obj != null)
            {
                return obj.ToString();
            }
            else
                return null;
        }

        private static String GetUserSid(String userName)
        {
            // Get user sid
            NTAccount f = new NTAccount(userName);
            SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
            String sidString = s.ToString();
            return sidString;
        }

        private static object GetClassesKeyValue(RegistryKey rootKey, String keyPath, String valueName)
        {
            object obj = null;
            RegistryKey key = rootKey?.OpenSubKey(CLASSES_SUBKEY + @"\" + keyPath);
            if (key != null)
            {
                obj = key.GetValue(valueName);
            }
            return obj;
        }
        public static object GetClassesRootValue(String keyPath, String valueName, String userName)
        {
            object obj = null;
            String sidString = GetUserSid(userName);
            Microsoft.Win32.RegistryKey userKey = Microsoft.Win32.Registry.Users.OpenSubKey(sidString);
            if (userKey != null)
            {
                obj = GetClassesKeyValue(userKey, keyPath, valueName);
                if (obj != null)
                    return obj;
            }
            RegistryKey lmKey = Registry.LocalMachine;
            obj = GetClassesKeyValue(lmKey, keyPath, valueName);
            return obj;
        }

        public static bool GetCommandStringIconDetails(object keyValue, ref LinkHelper.LinkDetails linkDetails)
        {
            if (keyValue != null)
            {
                linkDetails.IconLocation = keyValue.ToString().Split('"')[1];
                linkDetails.IconIndex = 0;
                return true;
            }
            else
                return false;
        }

        public static void GetShortcutIconDetails(ref LinkHelper.LinkDetails linkDetails)
        {
            // Initialize variables
            string extension = "." + linkDetails.TargetPath.ToLowerInvariant().Split('.').Last().ToLowerInvariant();

            // Get UserChoice file icon if it exists
            if (GetUserChoiceFileIcon(ref linkDetails, extension))
                return;

            // Get associated file icon if it exists
            if (GetAssociatedFileIcon(ref linkDetails, extension))
                return;
        }

        public static bool GetAssociatedFileIcon(ref LinkHelper.LinkDetails linkDetails, string extension)
        {
            Object obj;
            if (extension != null)
            {
                obj = GetClassesRootValue(extension, null, linkDetails.UserName);
                if (obj != null)
                {
                    String className = obj.ToString();
                    if (GetKeyFileIconDetails(className, ref linkDetails))
                        return true;
                }
            }
            return false;
        }

        private static bool GetUserChoiceFileIcon(ref LinkHelper.LinkDetails linkDetails, string extension)
        {
            Object obj = null;
            String sidString = GetUserSid(linkDetails.UserName);
            Microsoft.Win32.RegistryKey usersKey = Microsoft.Win32.Registry.Users;
            if (extension != null)
            {
                RegistryKey explorerExtKey = usersKey.OpenSubKey(sidString + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension + @"\UserChoice", true);
                if (explorerExtKey != null)
                {
                    obj = explorerExtKey.GetValue("ProgID");
                    if (obj != null)
                    {
                        String className = obj.ToString();
                        if (GetKeyFileIconDetails(className, ref linkDetails))
                            return true;
                    }
                }
            }
            return false;
        }

        public static String GetInstallPath(String companyName, String programName)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + companyName + "\\" + programName);
            String installPath;
            if (registryKey != null)
                installPath = registryKey.GetValue("InstallDir").ToString();
            else
            {
                registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + companyName + "\\" + programName);
                if (registryKey != null)
                    installPath = registryKey.GetValue("InstallDir").ToString();
                else
                    throw new InstallPathNotFoundException();
            }
            return installPath;
        }


        public static bool GetCanReboot(String companyName, String programName)
        {
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + companyName + "\\" + programName);
                if (registryKey != null)
                    return Convert.ToBoolean(Convert.ToInt64(registryKey.GetValue("CanReboot").ToString()));
                else
                {
                    return false;
                }
            }catch { }
            return false;
        }

        public static String GetProgramRootValue(String companyName, String programName, String valueName)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + companyName + "\\" + programName);
            String installPath;
            if (registryKey != null)
                installPath = registryKey.GetValue(valueName).ToString();
            else
            {
                registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + companyName + "\\" + programName);
                if (registryKey != null)
                    installPath = registryKey.GetValue(valueName).ToString();
                else
                    throw new InstallPathNotFoundException();
            }
            return installPath;
        }

        public static string[] GetAutostartEntries()
        {
            // Search registry for programs which are autostarted on startup
            List<string> autostartEntries = new List<string>();
            RegistryKey registryKey;
            List<String> registryKeyNames = new List<string>();
            registryKeyNames.Add(@"Software\Microsoft\Windows\CurrentVersion\RunOnce");
            registryKeyNames.Add(@"Software\Microsoft\Windows\CurrentVersion\Run");
            registryKeyNames.Add(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run");
            registryKeyNames.Add(@"Software\Microsoft\Windows NT\CurrentVersion\Windows\load");
            registryKeyNames.Add(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce");
            registryKeyNames.Add(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run");
            registryKeyNames.Add(@"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run");
            registryKeyNames.Add(@"Software\Wow6432Node\Microsoft\Windows NT\CurrentVersion\Windows\load");
            foreach (string registryKeyName in registryKeyNames)
            {
                registryKey = Registry.CurrentUser.OpenSubKey(registryKeyName);
                if (registryKey != null)
                    foreach (String valueName in registryKey.GetValueNames())
                        autostartEntries.Add(registryKey.GetValue(valueName).ToString());
                registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName);
                if (registryKey != null)
                    foreach (String valueName in registryKey.GetValueNames())
                        autostartEntries.Add(registryKey.GetValue(valueName).ToString());
            }

            // TODO: Autostart by folders
            return autostartEntries.ToArray();
        }

        public static String RegistryValueToString(String registryKey, String registryValue)
        {
            using (RegistryKey lmKey = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (lmKey != null)
                {
                    object o = lmKey.GetValue(registryValue);
                    if (o != null)
                    {
                        return o.ToString();
                    }
                }
            }
            return null;
        }

        public static String RegistryValueToString(String registryKey, String registryValue, String userName)
        {
            String sidString = GetUserSid(userName);
            Microsoft.Win32.RegistryKey usersKey = Microsoft.Win32.Registry.Users;
            using (RegistryKey userKey = usersKey?.OpenSubKey(sidString)?.OpenSubKey(registryKey))
            {
                if (userKey != null)
                {
                    object o = userKey?.GetValue(registryValue);
                    if (o != null)
                    {
                        return o.ToString();
                    }
                }
            }
            return RegistryValueToString(registryKey, registryValue);
        }
    }
}