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
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    static class AccessRightsHelper
    {
        public static bool ContainsRight(System.Security.AccessControl.FileSystemRights right, System.Security.AccessControl.FileSystemAccessRule rule)
        {
            return (((int)right & (int)rule.FileSystemRights) == (int)right);
        }

        public static void GetAccessRightsForFile(String path, String userName, ref bool allowGroupRead, ref bool allowUserRead, ref bool denyGroupRead, ref bool denyUserRead)
        {
            System.IO.FileInfo targetFile = new System.IO.FileInfo(path);
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            AuthorizationRuleCollection acl = targetFile.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
            Principal userPrincipal = new UserPrincipal(context);
            userPrincipal.SamAccountName = userName;
            PrincipalSearcher searcherUser = new PrincipalSearcher(userPrincipal);
            userPrincipal = searcherUser.FindOne() as UserPrincipal;
            for (int i = 0; i < acl.Count; i++)
            {
                System.Security.AccessControl.FileSystemAccessRule rule = (System.Security.AccessControl.FileSystemAccessRule)acl[i];

                if (userPrincipal.Sid.Equals(rule.IdentityReference))
                {
                    if (System.Security.AccessControl.AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if (ContainsRight(FileSystemRights.Read, rule)) allowUserRead = true;
                    }
                    if (System.Security.AccessControl.AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if (ContainsRight(FileSystemRights.Read, rule)) denyUserRead = true;
                    }
                }
            }
            PrincipalSearchResult<Principal> groupPrincipals = userPrincipal.GetGroups();
            foreach (Principal groupPrincipal in groupPrincipals)
            {
                for (int i = 0; i < acl.Count; i++)
                {
                    System.Security.AccessControl.FileSystemAccessRule rule =
                        (System.Security.AccessControl.FileSystemAccessRule)acl[i];
                    if (groupPrincipal.Sid.Equals(rule.IdentityReference))
                    {
                        if (System.Security.AccessControl.AccessControlType.Allow.Equals(rule.AccessControlType))
                        {
                            if (ContainsRight(FileSystemRights.Read, rule)) allowGroupRead = true;
                        }
                        if (System.Security.AccessControl.AccessControlType.Deny.Equals(rule.AccessControlType))
                        {
                            if (ContainsRight(FileSystemRights.Read, rule)) denyGroupRead = true;
                        }
                    }
                }
            }
        }

        public static bool AllowedRead(String path, String userName)
        {
            // Default not allowed to access file
            bool allowGroupRead = false;
            bool allowUserRead = false;
            bool denyGroupRead = false;
            bool denyUserRead = false;

            // Check that the user has access rights to file
            GetAccessRightsForFile(path, userName, ref allowGroupRead, ref allowUserRead, ref denyGroupRead, ref denyUserRead);
            return ((allowGroupRead || allowUserRead) && !denyGroupRead && !denyUserRead);
        }
    }
}
