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
#include <ShlObj.h>
#include <Shobjidl.h>
#include <string>
#include <shidfact.h>
#include <propsys.h>
#include <propkey.h>
#include <stdlib.h>
#include <vector>
#include <Strsafe.h>
#include <stack>
#include <shidfact.h>
#include <propkeydef.h>
#include <propkey.h>
#include <mutex>

#include "Log.h"
#include "PathInfo.h"
#include <string>
#include <time.h>
#include <sstream>
#include <fstream>

using namespace std;

namespace
{
  const auto fallbackLogLevel = LogType::Information | LogType::Warning | LogType::Error;

  std::wstring LogPath()
  {
	std::wstring logPath = L"Elbitin\\RAAS Client\\Logs\\nsextlog.dat";
	return logPath;
  }

  std::wstring BuildLogFileName()
  {
    PWSTR appDataPath = NULL;
    if( SHGetKnownFolderPath(FOLDERID_RoamingAppData, 0, NULL, &appDataPath) == S_OK )
    {
      std::wstring logFile = PathInfo::CombinePath(appDataPath, LogPath());
      CoTaskMemFree(appDataPath);
      return logFile;
    }
    else
    {
      return L"C:\\nsextlog.dat";
    }
  }

  LogType::eType GetLogLevel()
  {
    DWORD logLevel = fallbackLogLevel;
    HKEY nsextKey;
    if (RegOpenKeyEx(
            HKEY_LOCAL_MACHINE,
            L"Software\\Elbitin\\RAAS Client\\NSExt",
            0,
            KEY_QUERY_VALUE,
            &nsextKey) != ERROR_SUCCESS)
	  if (RegOpenKeyEx(
			  HKEY_CURRENT_USER,
		      L"Software\\Elbitin\\RAAS Client\\NSExt",
			  0,
			  KEY_QUERY_VALUE,
			  &nsextKey) != ERROR_SUCCESS)
        return LogType::eType(logLevel);

    DWORD sizeLogLevel = sizeof(logLevel);
    RegGetValue(
        nsextKey,
        NULL,
        L"logLevel",
        RRF_RT_REG_DWORD,
        NULL,
        &logLevel,
        &sizeLogLevel);

    return LogType::eType(logLevel);
  }
}

LogType::eType Log::_level = GetLogLevel();
std::wstring Log::_lastError = L"";
bool Log::_shouldLogToFile = true;
std::wstring Log::_logFile = BuildLogFileName();

void Log::Debug(const std::wstring& message)
{
  WriteOutput(LogType::Debug, L"%s", message.c_str());
}

void Log::Information(const std::wstring& message)
{
  WriteOutput(LogType::Information, L"%s", message.c_str());
}

void Log::Warning(const std::wstring& message)
{
  WriteOutput(LogType::Warning, L"%s", message.c_str());
}

void Log::Error(const std::wstring& message)
{
  WriteOutput(LogType::Error, L"%s", message.c_str());
}

void Log::WriteOutput(LogType::eType logType, const wchar_t* szFormat, ...)
{
  if ((_level & logType) == 0)
  {
    return;
  }

  std::wstringstream message;

  wstring strLogType;
  switch (logType)
  {
	case LogType::Information:
	  strLogType = L"Information";
	  break;
	case LogType::Warning:
	  strLogType = L"Warning";
	  break;
	case LogType::Error:
	  strLogType = L"Error";
	  break;
	case LogType::Debug:
	  strLogType = L"Debug";
	  break;
	default:
		return;
  }

  message << L"(" << GetCurrentThreadId() << L") - ";

  time_t timeRaw;
  time(&timeRaw);
  TCHAR timeBuffer[26] = {};
  _wctime_s( timeBuffer,sizeof(timeBuffer)/sizeof(timeBuffer[0]),&timeRaw);
  std::wstring timeValue(timeBuffer);

  timeValue.erase(timeValue.size() - 1); // remove trailing newline

  message << timeValue << L" - " << strLogType;

  message << L" - ";

  wchar_t szBuff[4096] = {0};
  va_list arg = NULL;
  va_start(arg, szFormat);
  _vsnwprintf_s(szBuff, _TRUNCATE, szFormat, arg);
  va_end(arg);

  std::wstring buffer = (LPCWSTR)&szBuff;

  message << buffer;
  message << L"\n";

  std::wstring output = message.str();

#ifdef DEBUG
  OutputDebugString(output.c_str());
#endif

  if (logType == LogType::Error)
  {
    _lastError = output;
  }

  if (_shouldLogToFile && _logFile.length() > 0)
  {
	static mutex mutex;
	mutex.lock();

    PathInfo::CreatePath(_logFile);
    std::wofstream myfile(_logFile, std::ofstream::app);
    myfile.write(output.c_str(), output.length());

	mutex.unlock();
  }
}

HRESULT Log::HRFromWin32(BOOL didNotFail, LPTSTR lpszFunction)
{
  HRESULT hr = S_OK;

  if (didNotFail == false)
  {
    DWORD dw = GetLastError();
    hr = HRESULT_FROM_WIN32(dw);

    LPVOID lpMsgBuf;
    LPVOID lpDisplayBuf;

    auto charCount = FormatMessage(
      FORMAT_MESSAGE_ALLOCATE_BUFFER |
      FORMAT_MESSAGE_FROM_SYSTEM |
      FORMAT_MESSAGE_IGNORE_INSERTS,
      NULL,
      dw,
      MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
      (LPTSTR)&lpMsgBuf,
      0, NULL);

    // Display the error message and exit the process
    if (charCount != 0)
    {
      lpDisplayBuf = (LPVOID)LocalAlloc(LMEM_ZEROINIT,
        (lstrlen((LPCTSTR)lpMsgBuf) + lstrlen((LPCTSTR)lpszFunction) + 40) * sizeof(TCHAR));

      if (lpDisplayBuf != NULL)
      {
        StringCchPrintf((LPTSTR)lpDisplayBuf,
          LocalSize(lpDisplayBuf) / sizeof(TCHAR),
          TEXT("%s failed with error %d: %s"),
          lpszFunction, dw, lpMsgBuf);

        Log::WriteOutput(LogType::Error, L"%s", lpDisplayBuf);

        LocalFree(lpMsgBuf);
        LocalFree(lpDisplayBuf);
      }
    }
  }

  return hr;
}
