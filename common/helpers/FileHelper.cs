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
using System.IO;
using System.Text;
using System.Threading;

namespace Elbitin.Applications.RAAS.Common.Helpers
{
    class FileHelper
    {
        private const int DEFAULT_WAIT_WHILE_LOCKED_COUNT = 50;
        private const int DEFAULT_WAIT_WHILE_LOCKED_INTERVAL_MS = 200;

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void WaitTimeWhileFileLocked(String fileFullPath)
        {
            WaitTimeWhileFileLocked(fileFullPath, DEFAULT_WAIT_WHILE_LOCKED_COUNT, DEFAULT_WAIT_WHILE_LOCKED_INTERVAL_MS);
        }

        public static void WaitTimeWhileFileLocked(String fileFullPath, int retryCount, int retryIntervalMs)
        {
            FileInfo fi = new FileInfo(fileFullPath);
            if (fi.Exists && fi.FullName.ToLowerInvariant().EndsWith(".lnk"))
                for (int i = 0; i < retryCount && IsFileLocked(fi); i++)
                    Thread.Sleep(retryIntervalMs);
        }
    }
}
