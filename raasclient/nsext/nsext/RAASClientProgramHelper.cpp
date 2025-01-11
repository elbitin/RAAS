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

	This file includes code from Microsofts ExplorerDataProvider but have
	been modified by Elbitin, modifications are copyrighted by Elbitin but
	included is copyright notices from ExplorerDataProvider project.

	ExplorerDataProvider source code can be found at:
	https://github.com/Microsoft/Windows-classic-samples

	*/

/**************************************************************************
    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
   ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
   PARTICULAR PURPOSE.

   (c) Microsoft Corporation. All Rights Reserved.
**************************************************************************/

#include <windows.h>
#include <msxml6.h>
#include <shlobj.h>
#include <Strsafe.h>
#include "RAASClientProgramHelper.h"
#include "shlwapi.h"
#include <comutil.h>
#include <vector>
#include "Log.h"
#include "RAASClientXmlUtils.h"

using namespace std;

const wchar_t* CRAASClientProgramHelper::sz_cCONNECT_SHARES_PROGRAM_NAME = L"connectshares.exe";
const wchar_t* CRAASClientProgramHelper::sz_cCONNECT_SHARES_ADMIN_PROGRAM_NAME = L"connectsharesadmin.exe";
const wchar_t* CRAASClientProgramHelper::sz_cREMOTE_DESKTOP_PROGRAM_NAME = L"rdesktop.exe";
const wchar_t* CRAASClientProgramHelper::sz_cCONFIGURE_SERVER_PROGRAM_NAME = L"servercfg.exe";
const wchar_t* CRAASClientProgramHelper::sz_cRAAS_CLIENT_APPDATA_PROGRAM_SUB_DIR = L"Elbitin\\RAAS Client";
const wchar_t* CRAASClientProgramHelper::sz_cSERVERS_XML_FILE_NAME = L"servers.xml";
const wchar_t* CRAASClientProgramHelper::sz_cRAAS_CLIENT_REGISTRY_KEY = L"SOFTWARE\\Elbitin\\RAAS Client";
const wchar_t* CRAASClientProgramHelper::sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR = L"InstallDir";

const HRESULT CRAASClientProgramHelper::GetRAASInstallDir(wchar_t* szDest, DWORD cchDest)
{
	Log::Debug(L"CRAASProgramHelper::GetRAASInstallDir(wchar_t* szDest, DWORD cchDest)");
	HRESULT hr;
	HKEY hKey;
	LONG lRes = RegOpenKeyExW(HKEY_LOCAL_MACHINE, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
	bool bLMSuccess(lRes == ERROR_SUCCESS);
	if (!bLMSuccess)
	{
		lRes = RegOpenKeyExW(HKEY_CURRENT_USER, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
		bool bUserSuccess(lRes == ERROR_SUCCESS);
		if (!bUserSuccess)
			return E_FAIL;
	}
	ULONG nError;
	nError = RegQueryValueExW(hKey, sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR, 0, NULL, (LPBYTE)szDest, &cchDest);
	hr = (ERROR_SUCCESS == nError ? S_OK : E_FAIL);
	return hr;
}

const void CRAASClientProgramHelper::StartConnectSharesAdmin()
{
	Log::Debug(L"CRAASProgramHelper::StartConnectSharesAdmin()");
	StartRAASClientProgram(sz_cCONNECT_SHARES_ADMIN_PROGRAM_NAME, NULL);
}

const void CRAASClientProgramHelper::StartConnectShares()
{
	Log::Debug(L"CRAASProgramHelper::StartConnectShares()");
	StartRAASClientProgram(sz_cCONNECT_SHARES_PROGRAM_NAME, NULL);
}

const HRESULT CRAASClientProgramHelper::ConfigureServer(IShellItemArray *psia)
{
	Log::Debug(L"CRAASProgramHelper::ConfigureServer(IShellItemArray *psia)");
	return StartRAASServerProgram(sz_cCONFIGURE_SERVER_PROGRAM_NAME, psia);
}

const HRESULT CRAASClientProgramHelper::RemoteDesktop(IShellItemArray *psia)
{
	Log::Debug(L"CRAASProgramHelper::RemoteDesktop(IShellItemArray *psia)");
	return StartRAASServerProgram(sz_cREMOTE_DESKTOP_PROGRAM_NAME, psia);
}

const HRESULT CRAASClientProgramHelper::StartRAASServerProgram(const wchar_t* szProgramName, IShellItemArray *psia)
{
	Log::Debug(L"CRAASProgramHelper::StartRAASServerProgram(const wchar_t* szProgramName, IShellItemArray *psia)");
	IShellItem *psi;
	HRESULT hr = psia->GetItemAt(0, &psi);
	if (SUCCEEDED(hr))
	{
		PWSTR szDisplayName;
		hr = psi->GetDisplayName(SIGDN_NORMALDISPLAY, &szDisplayName);
		if (SUCCEEDED(hr))
		{
			wchar_t* szServerName = new (std::nothrow) wchar_t[MAX_PATH];
			hr = szServerName ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				hr = GetServerNameFromAlias(szDisplayName, &szServerName, MAX_PATH);
				if (SUCCEEDED(hr))
				{
					StartRAASClientProgram(szProgramName, szServerName);
					CoTaskMemFree(szDisplayName);
				}
				delete[] szServerName;
			}
		}
		psi->Release();
	}
	return hr;
}

const HRESULT CRAASClientProgramHelper::GetServerNameFromAlias(PWSTR szServerAlias, wchar_t** pszServerName, DWORD cchServerName)
{
	Log::Debug(L"CRAASProgramHelper::GetServerNameFromAlias(PWSTR szServerAlias, wchar_t** pszServerName, DWORD cchServerName)");
	HRESULT hr;
	unsigned int cServers = 0;
	BOOL fFound = FALSE;
	vector<wstring> vServerNames;
	vector<wstring> vServerAliases;
	wchar_t szAppDataPath[MAX_PATH];
	wchar_t szServersXmlPath[MAX_PATH];
	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
		SUCCEEDED(StringCchPrintf(szServersXmlPath, ARRAYSIZE(szServersXmlPath),
			L"%s\\%s\\%s", szAppDataPath, sz_cRAAS_CLIENT_APPDATA_PROGRAM_SUB_DIR, sz_cSERVERS_XML_FILE_NAME)) &&
		PathFileExists(szServersXmlPath))
	{
		hr = GetServerStringsFromXmlFile(szServersXmlPath, vServerNames, vServerAliases, &cServers);
		if (SUCCEEDED(hr))
		{
			for (unsigned int i = 0; i < cServers; i++)
			{
				if (StrCmpW(szServerAlias, vServerAliases[i].c_str()) == 0)
				{
					wcscpy_s(*pszServerName, cchServerName, vServerNames[i].c_str());
					fFound = TRUE;
				}
			}
			hr = (fFound ? S_OK : E_FAIL);
		}
	}
	else
		hr = E_FAIL;
	return hr;
}

const void CRAASClientProgramHelper::StartRAASClientProgram(const wchar_t* szProgramName, const wchar_t* szArguments)
{
	Log::Debug(L"CRAASProgramHelper::StartRAASProgram(const wchar_t* szProgramName, const wchar_t* szArguments)");
	wchar_t szProgramPath[MAX_PATH];
	wchar_t* szInstallDir = new (std::nothrow) wchar_t[MAX_PATH];
	HRESULT hr = szInstallDir ? S_OK : E_OUTOFMEMORY;
	if (SUCCEEDED(hr))
	{
		hr = GetRAASInstallDir(szInstallDir, MAX_PATH);
		if (SUCCEEDED(hr))
		{
			if (SUCCEEDED(StringCchPrintf(szProgramPath, ARRAYSIZE(szProgramPath),
				L"\"%s%s\"", szInstallDir, szProgramName)))
			{
				ShellExecute(NULL, L"open", szProgramPath, szArguments, NULL, SW_SHOW);
			}
			delete[] szInstallDir;
		}
	}
}