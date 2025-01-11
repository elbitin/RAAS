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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    internal class AppxApplication
    {
        public string DefaultDisplayName { get; set; }
        public string LocalizedDisplayName { get; set; }
        public string AppId { get; set; }
        public string Executable { get; set; }
        public string IconIdentifier { get; set; }
        public string PackageFamilyName { get; set; }
        public string InstallLocation { get; set; }
        public string DefaultPriFilePath { get; set; }
        public string LocalizedPriFilePath { get; set; }
        public string EnglishPriFilePath { get; set; }
        public Bitmap Icon { get; set; } 
    }
}
