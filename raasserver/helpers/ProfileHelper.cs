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
﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Management;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    static class ProfileHelper
    {
        private const String USERS_FOLDER_GUID = "{0762D272-C50A-4BB0-A382-697DCD729B80}";

        public static string GetUserProfilePath(String userName)
        {
            NTAccount account = new NTAccount(userName);
            SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
            return GetUserProfilePath(sid);
        }

        public static string GetUserProfilePath(System.Security.Principal.SecurityIdentifier sid)
        {
            string path = string.Empty;
            ManagementObjectCollection profiles = GetProfiles();
            foreach (ManagementBaseObject profile in profiles)
            {
                if (Convert.ToString(profile["SID"]) == sid.ToString())
                {
                    path = Convert.ToString(profile["LocalPath"]);

                    // Comment this if condition if you want to get profiles of builtin system accounts as well
                    if (path.ToLowerInvariant().Contains(":\\users\\"))
                    {
                        return path;
                    }
                }
            }
            throw new Exception("No matching user profile path");
        }

        public static string GetUserFromProfilePath(String path)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Machine);
            IntPtr folderString;
            Win32Helper.SHGetKnownFolderPath(Guid.Parse(USERS_FOLDER_GUID), (uint)0, IntPtr.Zero, out folderString);
            String usersFolder = Marshal.PtrToStringUni(folderString);
            if (!path.ToLowerInvariant().StartsWith(usersFolder.ToLowerInvariant()))
                throw new Exception("Path not in users folder");
            System.Management.ManagementClass wmi = new ManagementClass("Win32_UserProfile");
            ManagementObjectCollection profiles = wmi.GetInstances();
            foreach (ManagementBaseObject profile in profiles)
            {
                if (path.ToLowerInvariant().StartsWith(Convert.ToString(profile["LocalPath"]).ToLowerInvariant()))
                {
                    String sid = Convert.ToString(profile["SID"]);
                    System.Management.ManagementClass wmiUsers = new ManagementClass("Win32_User");
                    SelectQuery query = new SelectQuery("Win32_UserAccount");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                    foreach (ManagementObject envVar in searcher.Get())
                        if (envVar["SID"].ToString() == profile["SID"].ToString())
                            return envVar["Name"].ToString();
                    UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.Sid, sid);
                    return up.Name;
                }
            }
            throw new Exception("No matching user profile");
        }

        public static List<string> GetAllUserProfilePaths()
        {
            List<string> profilePaths = new List<string>();
            string path = string.Empty;
            ManagementObjectCollection profiles = GetProfiles();
            IntPtr folderString;
            Win32Helper.SHGetKnownFolderPath(Guid.Parse(USERS_FOLDER_GUID), (uint)0, IntPtr.Zero, out folderString);
            String usersFolder = Marshal.PtrToStringUni(folderString);
            List<String> SIDs = new List<string>();
            SelectQuery query = new SelectQuery("Win32_UserAccount");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject mo in searcher.Get())
            {
                SIDs.Add(mo["SID"].ToString());
            }
            foreach (ManagementBaseObject profile in profiles)
            {
                path = Convert.ToString(profile["LocalPath"]);

                // Comment this if condition if you want to get profiles of builtin system accounts as well
                if (SIDs.Contains(Convert.ToString(profile["SID"])) && path.ToLowerInvariant().StartsWith(usersFolder.ToLowerInvariant()))
                {
                    profilePaths.Add(path);
                }
            }
            return profilePaths;
        }

        public static String GetUsersFolderPath()
        {
            IntPtr folderString;
            Win32Helper.SHGetKnownFolderPath(Guid.Parse(USERS_FOLDER_GUID), (uint)0, IntPtr.Zero, out folderString);
            String usersFolder = Marshal.PtrToStringUni(folderString);
            return usersFolder;
        }

        private static ManagementObjectCollection GetProfiles()
        {
            ManagementClass wmi = new ManagementClass("Win32_UserProfile");
            ManagementObjectCollection profiles = wmi.GetInstances();
            return profiles;
        }
    }
}
