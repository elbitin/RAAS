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
// Copyright (c) Elbitin

#include <Shobjidl.h>
#include <string>

using namespace std;

#if !defined(AFX_SHELLEXT_H__38EFB68B_5591_485A_9E50_409E8CDE10E2__INCLUDED_)
#define AFX_SHELLEXT_H__38EFB68B_5591_485A_9E50_409E8CDE10E2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CShellExt : public IShellExtInit, IContextMenu
{
public:
	CShellExt();

	// IUnknown methods
	STDMETHOD (QueryInterface) (REFIID riid, LPVOID * ppvObj);
	STDMETHOD_ (ULONG, AddRef) (void);
	STDMETHOD_ (ULONG, Release) (void);
	
    // IShellExtInit
    STDMETHODIMP Initialize(LPCITEMIDLIST, LPDATAOBJECT, HKEY);

    // IContextMenu
    STDMETHODIMP GetCommandString(UINT_PTR, UINT, UINT*, LPSTR, UINT);
    STDMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO);
    STDMETHODIMP QueryContextMenu(HMENU, UINT, UINT, UINT, UINT);

private:
	~CShellExt();

	void _CreateRemoteShortcut();
	void _OpenRemote();
	BOOL _GetServerNameFromPath(wstring szPath, wchar_t* buffer, int bufSize);
	HRESULT _GetRAASClientInstallDir(wchar_t* szDest, DWORD cchDest);
	BOOL _ServerExists(wchar_t* serverName, wchar_t* serversFile);
	BOOL _ShortcutsInstalled();

	const wchar_t* sz_cREMOTE_SHORTCUT_PROGRAM_NAME = L"remotesc.exe";
	const wchar_t* sz_cOPEN_REMOTE_PROGRAM_NAME = L"openremote.exe";
	const wchar_t* sz_cCONFIGURE_SERVER_PROGRAM_NAME = L"servercfg.exe";
	const wchar_t* sz_cRAAS_CLIENT_APPDATA_SUB_DIR = L"Elbitin\\RAAS Client";
	const wchar_t* sz_cSERVERS_XML_FILE_NAME = L"servers.xml";
	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_KEY = L"SOFTWARE\\Elbitin\\RAAS Client";
	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR = L"InstallDir";
	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_SHORTCUTS = L"Shortcuts";
	const wchar_t* sz_cRAAS_CLIENT_REGISTRY_INSTALLED_VALUE = L"Installed";
	const wchar_t* sz_cSERVER_NAME_QUERY = L"//Servers/Server/Name";
	const wchar_t* sz_cSERVER_ENABLED = L"Enabled";

	DWORD m_cRef;
    TCHAR m_szFile [MAX_PATH];
};

#endif // !defined(AFX_SHELLEXT_H__38EFB68B_5591_485A_9E50_409E8CDE10E2__INCLUDED_)
