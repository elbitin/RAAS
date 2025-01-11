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
using System.Text;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    internal class StringHelper
    {
        public static string ExtractStringFromLib(string file, int number)
        {
            IntPtr lib = Win32Helper.LoadLibrary(file);
            StringBuilder result = new StringBuilder(2048);
            Win32Helper.LoadString(lib, number, result, result.Capacity);
            Win32Helper.FreeLibrary(lib);
            return result.ToString();
        }
    }
}
