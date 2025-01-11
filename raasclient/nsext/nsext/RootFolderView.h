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
#include "BaseFolderView.h"

class CRootFolderView : public CBaseFolderView
{
public:
	CRootFolderView();

	// IUnknown methods
	IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IShellFolder
	IFACEMETHODIMP GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT* prgfInOut, void** ppv);
	IFACEMETHODIMP CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2);

	// IPersistFolder
	IFACEMETHODIMP Initialize(PCIDLIST_ABSOLUTE pidl);

private:
	~CRootFolderView();

	CBaseFolderViewEnumIDList * CreateEnumIDList(DWORD grfFlags);
	CBaseFolderView* CreateChildFolderView();
};
