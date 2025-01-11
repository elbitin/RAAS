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
#include "BaseFolderViewCB.h"

class CRealFolderFolderViewCB : public CBaseFolderViewCB
{
public:
	CRealFolderFolderViewCB(LPITEMIDLIST pidl, LPITEMIDLIST pidlVirtual);

	// IShellFolderViewCB
	virtual IFACEMETHODIMP MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam);

	IFACEMETHODIMP GetViewMode(FOLDERLOGICALVIEWMODE* plvm);
	IFACEMETHODIMP GetGroupByProperty(PROPERTYKEY* /* pkey */, BOOL* /* pfGroupAscending */);

private:
	~CRealFolderFolderViewCB();

	LPITEMIDLIST m_pidlVirtual;
};