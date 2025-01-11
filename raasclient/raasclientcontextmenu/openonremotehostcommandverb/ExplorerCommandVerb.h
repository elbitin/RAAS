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
#pragma once

#include "Dll.h"
#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include <wchar.h>
#include <shlobj.h>
#include <strsafe.h>
#include <objbase.h>
#include <comdef.h>
#include "Utils.h"
#include <string>
#include <vector>
#include "resource.h"
#include "ExplorerCommandVerb.h"

using namespace std;

class CExplorerCommandVerb : public IExplorerCommand,
	public IInitializeCommand,
	public IObjectWithSite
{
public:
	CExplorerCommandVerb();

	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IExplorerCommand
	IFACEMETHODIMP GetTitle(IShellItemArray* /* psiItemArray */, LPWSTR* ppszName);
	IFACEMETHODIMP GetIcon(IShellItemArray* /* psiItemArray */, LPWSTR* ppszIcon);
	IFACEMETHODIMP GetToolTip(IShellItemArray* /* psiItemArray */, LPWSTR* ppszInfotip);
	IFACEMETHODIMP GetCanonicalName(GUID* pguidCommandName);
	IFACEMETHODIMP GetState(IShellItemArray* psiItemArray, BOOL fOkToBeSlow, EXPCMDSTATE* pCmdState);
	IFACEMETHODIMP Invoke(IShellItemArray* psiItemArray, IBindCtx* pbc);
	IFACEMETHODIMP GetFlags(EXPCMDFLAGS* pFlags);
	IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** ppEnum);

	// IInitializeCommand
	IFACEMETHODIMP Initialize(PCWSTR /* pszCommandName */, IPropertyBag* /* ppb */);

	// IObjectWithSite
	IFACEMETHODIMP SetSite(IUnknown* punkSite);
	IFACEMETHODIMP GetSite(REFIID riid, void** ppv);

	~CExplorerCommandVerb();

	DWORD ThreadProc();
	HRESULT GetRAASClientInstallDir(wchar_t* szDest, DWORD cchDest);
	void OpenRemote();
	BOOL ServerExists(wchar_t* szServerName, wchar_t* szServersFile);
	BOOL GetServerNameFromPath(wstring szPath, wchar_t* buffer, int bufSize);
private:

	static DWORD __stdcall CExplorerCommandVerb::s_ThreadProc(void* pv)
	{
		CExplorerCommandVerb* pecv = (CExplorerCommandVerb*)pv;
		const DWORD ret = pecv->ThreadProc();
		pecv->Release();
		return ret;
	}

	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR = L"InstallDir";
	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_KEY = L"SOFTWARE\\Elbitin\\RAAS Client";
	const wchar_t* sz_cSERVER_NAME_QUERY = L"//Servers/Server/Name";
	const wchar_t* sz_cSERVER_ENABLED = L"Enabled";
	const wchar_t* sz_cRAAS_CLIENT_APPDATA_SUB_DIR = L"Elbitin\\RAAS Client";
	const wchar_t* sz_cSERVERS_XML_FILE_NAME = L"servers.xml";
	const wchar_t* sz_cOPEN_REMOTE_PROGRAM_NAME = L"openremote.exe";
	const wchar_t* sz_cRAASCLIENT_PROGRAM_NAME = L"raasclient.exe";
	long m_cRef;
	IUnknown* m_punkSite;
	HWND m_hwnd;
	IStream* m_pstmShellItemArray;
	TCHAR m_szFile[MAX_PATH];
};

HRESULT CExplorerCommandVerb_RegisterUnRegister(bool fRegister);
HRESULT CExplorerCommandVerb_CreateInstance(REFIID riid, void** ppv);