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
/*
	Copyright (c) Elbitin, modifications by Elbitin

	This file includes code from Googles google-drive-shell-extension
	but have been modified by Elbitin, modifications are copyrighted by Elbitin
	but included is	copyright notices from the google source project.

	google-drive-shell-extension source code can be found at:
	https://github.com/google/google-drive-shell-extension

*/

/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#pragma once
#include <string>
#include <Guiddef.h>
#include <Windows.h>

struct LogType
{
  enum eType
  {
    None = 0,
    Information = 1<<1,
    Warning = 1<<2,
    Error = 1<<3,
    Debug = 1<<4,
    Test = 1<<5,
    All = Information | Warning | Error | Debug,
  };
};

class Log
{
public:
  Log();
  ~Log(void);

public:
  static void Debug(const std::wstring& message);
  static void Information(const std::wstring& message);
  static void Warning(const std::wstring& message);
  static void Error(const std::wstring& message);
  static void WriteOutput(LogType::eType logType, const wchar_t* szFormat, ...);

  static HRESULT HRFromWin32(BOOL didNotFail, LPTSTR lpszFunction);

private:
  static LogType::eType _level;
  static std::wstring _lastError;
  static bool _shouldLogToFile;
  static std::wstring _logFile;
};
