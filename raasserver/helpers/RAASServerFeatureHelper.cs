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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    class RAASServerFeatureHelper
    {
        private const String RAAS_REGISTRY_KEY = "Software\\Elbitin\\RAAS Server";
        private const String FEATURE_INSTALLED_VALUE = "Installed";
        private const String RAASSERVER_SHARES_FEATURE_NAME = "Shares";

        private static bool RAASServerFeatureInstalled(String feature)
        {
            try
            {
                if (RegistryHelper.RegistryValueToString(RAAS_REGISTRY_KEY, feature) == FEATURE_INSTALLED_VALUE)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool SharesInstalled()
        {
            return RAASServerFeatureInstalled(RAASSERVER_SHARES_FEATURE_NAME);
        }
    }
}
