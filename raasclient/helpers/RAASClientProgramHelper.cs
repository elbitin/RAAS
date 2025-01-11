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
﻿using System;
using System.Diagnostics;
using System.IO;
using Elbitin.Applications.RAAS.Common.Helpers;

namespace Elbitin.Applications.RAAS.RAASClient.Helpers
{
    public static class RAASClientProgramHelper
    {
        public static Process StartShortcuts()
        {
            return StartRAASClientProgram(RAASClientPathHelper.GetShortcutsProgramPath());
        }

        public static Process StartShortcuts(String arguments)
        {
            return StartProgram(RAASClientPathHelper.GetShortcutsProgramPath(), arguments);
        }

        public static Process StartServerConfig()
        {
            return StartRAASClientProgram(RAASClientPathHelper.GetServerConfigProgramPath());
        }

        public static Process StartServerConfig(String serverName)
        {
            return StartProgram(RAASClientPathHelper.GetServerConfigProgramPath(), serverName);
        }

        public static Process StartRemoteApps(String serverName, EventHandler exitHandler)
        {
            Process process = CreateProgramProcess(RAASClientPathHelper.GetRemoteAppsProgramPath(), serverName);
            process.EnableRaisingEvents = true;
            process.Exited += exitHandler;
            process.Start();
            return process;
        }

        public static Process StartRDesktop(String serverName)
        {
            Process process = CreateProgramProcess(RAASClientPathHelper.GetRDesktopProgramPath(), serverName);
            process.EnableRaisingEvents = true;
            process.Start();
            return process;
        }


        public static void StartRDesktop()
        {
            String rDesktopPath = RAASClientPathHelper.GetRDesktopProgramPath();

            // Start remote desktop
            System.Diagnostics.Process.Start(rDesktopPath);
        }

        public static void StartRemoteDesktop(String serverName, String userName)
        {
            // Generate rdp file based on template
            String remoteDesktopTemplatePath = RAASClientPathHelper.GetRDPTemplatePath();
            String remoteDesktopRdpPath = RAASClientPathHelper.GetAppDataRDPFilePath();
            File.Copy(remoteDesktopTemplatePath, remoteDesktopRdpPath, true);
            File.AppendAllText(remoteDesktopRdpPath, "\n\r" + "full address:s:" + serverName + "\n\r" + "username:s:" + serverName + "\\" + userName);

            // Start remote desktop
            System.Diagnostics.Process.Start(remoteDesktopRdpPath);
        }

        private static Process StartRAASClientProgram(String programPath)
        {
            return StartProgram(programPath, "");
        }

        private static Process StartProgram(String programPath, String arguments)
        {
            Process process = CreateProgramProcess(programPath, arguments);
            process.Start();
            return process;
        }

        private static Process CreateProgramProcess(String programPath, String arguments)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(programPath, arguments);
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(programPath);
            process.StartInfo.UseShellExecute = true;
            return process;
        }
    }
}