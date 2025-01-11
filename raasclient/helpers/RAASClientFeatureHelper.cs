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
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    static class RAASClientFeatureHelper
    {
        private const String RAAS_CLIENT_REGISTRY_KEY = "Software\\Elbitin\\RAAS Client";
        private const String FEATURE_INSTALLED_VALUE = "Installed";
        private const String RAAS_CLIENT_SHORTCUTS_FEATURE_NAME = "Shortcuts";
        private const String RAAS_CLIENT_VISUAL_AIDS_FEATURE_NAME = "Visualizations";
        private const String RAAS_CLIENT_NSEXT_FEATURE_NAME = "NSExt";

        private static bool RAASClientFeatureInstalled(String feature)
        {
            try
            {
                if(RegistryHelper.RegistryValueToString(RAAS_CLIENT_REGISTRY_KEY, feature) == FEATURE_INSTALLED_VALUE)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ShortcutsInstalled()
        {
            return RAASClientFeatureInstalled(RAAS_CLIENT_SHORTCUTS_FEATURE_NAME);
        }

        public static bool VisualizationsInstalled()
        {
            return RAASClientFeatureInstalled(RAAS_CLIENT_VISUAL_AIDS_FEATURE_NAME);
        }

        public static bool NSExtInstalled()
        {
            return RAASClientFeatureInstalled(RAAS_CLIENT_NSEXT_FEATURE_NAME);
        }
    }
}
