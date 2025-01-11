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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    class RAASServerProgramHelper
    {
        public static void StartStartupPrograms()
        {
            String[] autostartEntries = RegistryHelper.GetAutostartEntries();
            foreach (String autostartEntry in autostartEntries)
            {
                try
                {
                    String applicationPath, applicationArguments;
                    GetApplicationParameters(autostartEntry, out applicationPath, out applicationArguments);

                    // Start process
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo(applicationPath, applicationArguments);
                    process.StartInfo.WorkingDirectory = Path.GetDirectoryName(applicationPath);
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
                catch { }
            }
        }

        private static void GetApplicationParameters(String applicationString, out String applicationPath, out String applicationArguments)
        {
            // Parse to separate path from arguments
            if (applicationString.Split('"').Count() > 1 && applicationString.Split('"')[0].TrimStart() == "")
            {
                applicationPath = applicationString.Split('"')[1];
                applicationArguments = applicationString.Substring(applicationPath.Length + 2).TrimStart();
            }
            else if (applicationString.Split('\'').Count() > 1 && applicationString.Split('\'')[0].TrimStart() == "")
            {
                applicationPath = applicationString.Split('\'')[1];
                applicationArguments = applicationString.TrimStart().Substring(applicationPath.Length + 2).TrimStart();
            }
            else
            {
                int skippedChars = 0;
                String[] doubleQuotesSplit = applicationString.Split('"');
                String programStripped = doubleQuotesSplit[0];
                String fullFileName;
                for (int i = 1; i < doubleQuotesSplit.Count() - 1; i++)
                {
                    if (i % 2 == 1)
                    {
                        if (doubleQuotesSplit[i + 1].StartsWith("\\"))
                        {
                            programStripped += doubleQuotesSplit[i];
                            skippedChars += 2;
                        }
                        else if (doubleQuotesSplit[i - 1].EndsWith("\\"))
                            fullFileName = "\"" + doubleQuotesSplit[i] + "\"";
                    }
                    else
                        programStripped += doubleQuotesSplit[i];
                }
                String[] singleQuoteSplit = programStripped.Split('\'');
                skippedChars += singleQuoteSplit.Count() - 1;
                programStripped = singleQuoteSplit[0];
                for (int i = 1; i < singleQuoteSplit.Count() - 1; i++)
                {
                    if (i % 2 == 1)
                    {
                        if (singleQuoteSplit[i + 1].StartsWith("\\"))
                        {
                            programStripped += singleQuoteSplit[i];
                            skippedChars += 2;
                        }
                        else if (singleQuoteSplit[i - 1].EndsWith("\\"))
                            programStripped =  "\"" + singleQuoteSplit[i] + "\"";
                    }
                    else
                        programStripped += singleQuoteSplit[i];
                }
                String directoryName = "";
                String[] directoryParts = programStripped.Split('\\');
                for (int i = 0; i < directoryParts.Count() - 1; i++)
                    directoryName += directoryParts[i] + "\\";
                fullFileName = programStripped.Split('\\').Last().Split(' ').First();
                fullFileName.Replace("\"", "");
                applicationPath = Path.Combine(Path.GetDirectoryName(directoryName), fullFileName);
                applicationArguments = applicationString.Substring(applicationPath.Length + skippedChars).TrimStart();
            }
        }
    }
}
