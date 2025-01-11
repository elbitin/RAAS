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
using System;
using System.Drawing;
using System.Linq;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    static class AssociationsIconHelper
    {
        public static Bitmap GetExistingAssociationIcon(String filePath, String userPath)
        {
            if (!filePath.ToLowerInvariant().EndsWith(".exe") && !filePath.ToLowerInvariant().EndsWith(".dll") && !filePath.ToLowerInvariant().EndsWith(".ico") && !filePath.ToLowerInvariant().EndsWith(".cpl"))
            {
                String extension = filePath.Split('.').Last().ToLowerInvariant();
                String associationIcon = RAASServerPathHelper.GetAssociationIconPath(extension, userPath);
                if (associationIcon != null && associationIcon.Length > 0)
                    return new Bitmap(associationIcon);

            }
            return null;
        }
    }
}