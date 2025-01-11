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
#include <strsafe.h>
#include <shellapi.h>
#include <strsafe.h>
#include "BaseFolderViewEnumIDList.h"

class CBaseFolderViewCB : public IShellFolderViewCB,
	public IFolderViewSettings
{
public:
	CBaseFolderViewCB(LPITEMIDLIST pidl);

	// IUnknown
	virtual IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv);
	virtual IFACEMETHODIMP_(ULONG) AddRef();
	virtual IFACEMETHODIMP_(ULONG) Release();

	// IShellFolderViewCB
	virtual IFACEMETHODIMP MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam);

	// IFolderViewSettings
	virtual IFACEMETHODIMP GetColumnPropertyList(REFIID /* riid */, void** ppv);
	virtual IFACEMETHODIMP GetGroupByProperty(PROPERTYKEY* /* pkey */, BOOL* /* pfGroupAscending */);
	virtual IFACEMETHODIMP GetViewMode(FOLDERLOGICALVIEWMODE* plvm);
	virtual IFACEMETHODIMP GetIconSize(UINT* /* puIconSize */);
	virtual IFACEMETHODIMP GetFolderFlags(FOLDERFLAGS* pfolderMask, FOLDERFLAGS* pfolderFlags);
	virtual IFACEMETHODIMP GetSortColumns(SORTCOLUMN* /* rgSortColumns */, UINT /* cColumnsIn */, UINT* /* pcColumnsOut */);
	virtual IFACEMETHODIMP GetGroupSubsetCount(UINT* /* pcVisibleRows */);

protected:
	~CBaseFolderViewCB();

	long m_cRef;
	LPITEMIDLIST m_pidl = NULL;
};