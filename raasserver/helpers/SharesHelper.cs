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
﻿// Copyright (c) Elbitin
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Security.AccessControl;
using Elbitin.Applications.RAAS.Common.Models;
using Elbitin.Applications.RAAS.Common.Helpers;
using System.Linq;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    public static class SharesHelper
    {
        public static List<string> UpdateRAASServerShares()
        {
            // Share each drive
            List<String> shareNames = new List<string>();
            List<String> driveShareNames = new List<String>();
            foreach (DriveInfo drive in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    // Share name by naming convention
                    String shareName = "RAASServer_" + drive.Name.Substring(0, 1).ToUpper();

                    // Store share name
                    driveShareNames.Add(shareName);

                    // Get current shares
                    GetCurrentShares(ref shareNames, drive, shareName);

                    // Continue if already shared
                    if (shareNames.Contains(shareName))
                        continue;

                    AddShare(drive, shareName);
                }
                catch { }
            }

            return driveShareNames;
        }

        public static void RemoveUnusedRAASServerShares(List<string> driveShareNames)
        {
            // Get a list of existing shares
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            connectionOptions.Authentication = AuthenticationLevel.Packet;
            ManagementScope scope = new ManagementScope("root\\CIMV2", connectionOptions);
            scope.Connect();
            ManagementObjectSearcher worker = new ManagementObjectSearcher(scope, new ObjectQuery("select Name, Path from win32_share"));

            // Store existing shares
            Dictionary<String, String> existingShares = new Dictionary<String, String>();
            foreach (ManagementObject share in worker.Get())
            {
                existingShares[share["Name"].ToString()] = share["Path"].ToString();
            }

            // Remove all raas server shares and ignore others
            foreach (String existingShare in existingShares.Keys)
            {
                bool removeShare = true;

                // Dont remove shares to drives which still exist
                foreach (String driveShare in driveShareNames)
                    if (driveShare.ToLowerInvariant().Equals(existingShare.ToLowerInvariant()))
                        removeShare = false;

                // Only remove shares which are named according to naming convention
                if (!existingShare.StartsWith("RAASServer_"))
                    removeShare = false;

                if (removeShare)
                {
                    // Remove share
                    foreach (ManagementObject share in worker.Get())
                    {
                        if (share["Name"].ToString() == existingShare)
                            share.Delete();
                    }
                }
            }
        }

        public static void AddShare(DriveInfo drive, string shareName)
        {

            // User which can access share
            String userName = "Everyone";

            // Create a ManagementClass object
            ManagementClass managementClass = new ManagementClass("Win32_Share");

            // Create ManagementBaseObjects for in and out parameters
            ManagementBaseObject inParams = managementClass.GetMethodParameters("Create");
            ManagementBaseObject outParams;

            String Description = drive.Name;
            String FolderPath = drive.Name;

            // Set the input parameters
            inParams["Description"] = Description;
            inParams["Name"] = shareName;
            inParams["Path"] = FolderPath;
            inParams["Type"] = 0x0; // Disk Drive
                                    //Another Type:
                                    //        DISK_DRIVE = 0x0
                                    //        PRINT_QUEUE = 0x1
                                    //        DEVICE = 0x2
                                    //        IPC = 0x3
                                    //        DISK_DRIVE_ADMIN = 0x80000000
                                    //        PRINT_QUEUE_ADMIN = 0x80000001
                                    //        DEVICE_ADMIN = 0x80000002
                                    //        IPC_ADMIN = 0x8000003
            inParams["MaximumAllowed"] = null;
            inParams["Password"] = null;
            inParams["Access"] = null;

            // Invoke the create method on the ManagementClass object
            outParams = managementClass.InvokeMethod("Create", inParams, null);

            // Create domain context
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            try
            {
                if (System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain() != null)
                    context = new PrincipalContext(ContextType.Domain);
            }
            catch
            {
            }

            // User principal
            Principal userPrincipal = new UserPrincipal(context);
            userPrincipal.SamAccountName = userName;
            PrincipalSearcher searcherUser = new PrincipalSearcher(userPrincipal);
            userPrincipal = searcherUser.FindOne() as UserPrincipal;

            // User selection
            NTAccount ntAccount = new NTAccount(userName);
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));

            // SID
            SecurityIdentifier userSID = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
            byte[] utenteSIDArray = new byte[userSID.BinaryLength];
            userSID.GetBinaryForm(utenteSIDArray, 0);

            // Trustee
            ManagementObject userTrustee = new ManagementClass(new ManagementPath("Win32_Trustee"), null);
            userTrustee["Name"] = userName;
            userTrustee["SID"] = utenteSIDArray;

            // ACE
            ManagementObject userACE = new ManagementClass(new ManagementPath("Win32_Ace"), null);
            userACE["AccessMask"] = Win32Helper.AccessMaskEnum.OWNER;
            userACE["AceFlags"] = AceFlags.ObjectInherit | AceFlags.ContainerInherit;
            userACE["AceType"] = AceType.AccessAllowed;
            userACE["Trustee"] = userTrustee;

            // Security descriptor
            ManagementObject userSecurityDescriptor = new ManagementClass(new ManagementPath("Win32_SecurityDescriptor"), null);
            userSecurityDescriptor["ControlFlags"] = 4;
            userSecurityDescriptor["DACL"] = new object[] { userACE };

            // Share
            ManagementObject Share = new ManagementObject(managementClass.Path + ".Name='" + shareName + "'");
            Share.InvokeMethod("SetShareInfo", new object[] { Int32.MaxValue, Description, userSecurityDescriptor });
        }

        public static void GetCurrentShares(ref List<string> shareNames, DriveInfo drive, string shareName)
        {
            ConnectionOptions myConnectionOptions = new ConnectionOptions();
            myConnectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            myConnectionOptions.Authentication = AuthenticationLevel.Packet;
            ManagementScope myManagementScope = new ManagementScope(@"\\" + System.Environment.MachineName + @"\root\cimv2", myConnectionOptions);
            myManagementScope.Connect();
            if (!myManagementScope.IsConnected)
                Console.WriteLine("could not connect");
            else
            {
                ManagementObjectSearcher myObjectSearcher =
                    new ManagementObjectSearcher(myManagementScope.Path.ToString(), "SELECT * FROM Win32_Share WHERE Path='" + drive.Name.Replace("\\", "\\\\") + "'");
                foreach (ManagementObject share in myObjectSearcher.Get())
                {
                    if (shareName == null || share["Name"].ToString() == shareName)
                    {
                        shareNames.Add(share["Name"].ToString());
                    }
                }
            }
        }

        public static void RemoveAllRAASServerShares()
        {
            // Remove RAAS Server shares
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            connectionOptions.Authentication = AuthenticationLevel.Packet;
            ManagementScope scope = new ManagementScope("root\\CIMV2", connectionOptions);
            scope.Connect();
            ManagementObjectSearcher worker = new ManagementObjectSearcher(scope, new ObjectQuery("select Name, Path from win32_share"));
            Dictionary<String, String> existingShares = new Dictionary<String, String>();
            foreach (ManagementObject share in worker.Get())
            {
                existingShares[share["Name"].ToString()] = share["Path"].ToString();
            }
            foreach (String existingShare in existingShares.Keys)
            {
                // Remove share if it follows RAAS Server shares naming convention
                bool removeShare = true;
                if (!existingShare.StartsWith("RAASServer_"))
                    removeShare = false;
                if (removeShare)
                {
                    foreach (ManagementObject share in worker.Get())
                    {
                        if (share["Name"].ToString() == existingShare)
                            share.Delete();
                    }
                }
            }
        }

        public static string GetSharesXml(string userName)
        {
            Shares shares = new Shares();
            DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            shares.Drives = new List<SharesDrive>();

            // Add each drive share to shares object
            for (int i = 0; i < drives.Count(); i++)
            {
                DriveInfo drive = drives[i];

                // Share name by naming convention
                String shareName = "RAASServer_" + drive.Name.Substring(0, 1).ToUpper();

                // Get a list of shares by share name
                List<String> shareNames = new List<string>();
                SharesHelper.GetCurrentShares(ref shareNames, drive, shareName);

                // Add share to shares object if share exist
                if (shareNames.Contains(shareName))
                {
                    SharesDrive sharesDrive = new SharesDrive();
                    sharesDrive.Name = drive.Name;
                    sharesDrive.Type = Convert.ToString(drive.DriveType);
                    sharesDrive.Share = shareName;
                    shares.Drives.Add(sharesDrive);
                }
            }

            // Create profile path element
            String profilePathString = ProfileHelper.GetUserProfilePath(userName);
            shares.ProfilePath = profilePathString;

            // Return the entire XML document
            return shares.SerializeXml();
        }
    }
}
