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
#include <windows.h>
#include <shlobj.h>
#include <shlwapi.h>

class CServerContextMenu : public IContextMenu
	, public IShellExtInit
	, public IObjectWithSite
{
public:
	CServerContextMenu();

	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();


	// IContextMenu
	IFACEMETHODIMP QueryContextMenu(HMENU hmenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
	IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO lpici);
	IFACEMETHODIMP GetCommandString(UINT_PTR idCmd, UINT uType, UINT *pRes, LPSTR pszName, UINT cchMax);

	// IShellExtInit
	IFACEMETHODIMP Initialize(PCIDLIST_ABSOLUTE pidlFolder, IDataObject *pDataObj, HKEY hkeyProgID);

	// IObjectWithSite
	IFACEMETHODIMP SetSite(IUnknown *punkSite);
	IFACEMETHODIMP GetSite(REFIID riid, void **ppvSite);

private:
	~CServerContextMenu();

	long    m_cRef;
	IDataObject *m_pDataObj;
	IUnknown *m_punkSite;
};