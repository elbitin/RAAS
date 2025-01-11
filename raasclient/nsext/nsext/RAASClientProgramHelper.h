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

#pragma once
#include <shobjidl_core.h>

class CRAASClientProgramHelper
{
public:
	static const HRESULT GetRAASInstallDir(wchar_t* szDest, DWORD cchDest);
	static const HRESULT ConfigureServer(IShellItemArray *psia);
	static const void StartConnectShares();
	static const void StartConnectSharesAdmin();
	static const HRESULT StartRAASServerProgram(const wchar_t* szProgramName, IShellItemArray *psia);
	static const void StartRAASClientProgram(const wchar_t* szProgramName, const wchar_t* szArguments);
	static const HRESULT GetServerNameFromAlias(PWSTR szServerAlias, wchar_t** pszServerName, DWORD cchServerName);
	static const HRESULT RemoteDesktop(IShellItemArray *psia);
private:
	static const wchar_t* sz_cCONNECT_SHARES_PROGRAM_NAME;
	static const wchar_t* sz_cCONNECT_SHARES_ADMIN_PROGRAM_NAME;
	static const wchar_t* sz_cREMOTE_DESKTOP_PROGRAM_NAME;
	static const wchar_t* sz_cCONFIGURE_SERVER_PROGRAM_NAME;
	static const wchar_t* sz_cRAAS_CLIENT_APPDATA_PROGRAM_SUB_DIR;
	static const wchar_t* sz_cSERVERS_XML_FILE_NAME;
	static const wchar_t* sz_cRAAS_CLIENT_REGISTRY_KEY;
	static const wchar_t* sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR;
};