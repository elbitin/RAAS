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
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elbitin.Applications.RAAS.RAASServer.Helpers
{
    class RAASServerLogHelper
    {
        private const String COMPANY_NAME = "Elbitin";
        private const String PROGRAM_NAME = "RAAS Server";
        private const String MINIMUM_LOGLEVEL_REGISTRY_STRING = "minimumLogLevel";
        private enum MinimumLogLevel
        {
            VERBOSE = 0,
            DEBUG,
            INFORMATION,
            WARNING,
            ERROR,
            FATAL,
        }

        public static Serilog.ILogger CreateDefaultRAASServerLogger(String programName)
        {
            LoggerConfiguration logger = new LoggerConfiguration();
            try
            {
                MinimumLogLevel minimumLogLevel = (MinimumLogLevel)Convert.ToInt32(RegistryHelper.GetProgramRootValue(COMPANY_NAME, PROGRAM_NAME, MINIMUM_LOGLEVEL_REGISTRY_STRING));
                switch (minimumLogLevel)
                {
                    case MinimumLogLevel.VERBOSE:
                        logger.MinimumLevel.Verbose();
                        break;
                    case MinimumLogLevel.DEBUG:
                        logger.MinimumLevel.Debug();
                        break;
                    case MinimumLogLevel.INFORMATION:
                        logger.MinimumLevel.Information();
                        break;
                    case MinimumLogLevel.WARNING:
                        logger.MinimumLevel.Warning();
                        break;
                    case MinimumLogLevel.ERROR:
                        logger.MinimumLevel.Error();
                        break;
                    case MinimumLogLevel.FATAL:
                        logger.MinimumLevel.Error();
                        break;
                    default:
                        logger.MinimumLevel.Information();
                        break;
                }
            }
            catch
            {
                logger.MinimumLevel.Information();
            }
            String logPath = Path.Combine(RAASServerPathHelper.GetCommonLogsPath(), programName + "-.dat");
            logger.WriteTo.File(logPath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1048576, retainedFileCountLimit: 31);
            return logger.CreateLogger();
        }
    }
}
