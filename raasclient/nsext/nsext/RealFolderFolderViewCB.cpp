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
#include "Log.h"
#include "RealFolderFolderViewCB.h"

CRealFolderFolderViewCB::CRealFolderFolderViewCB(LPITEMIDLIST pidl, LPITEMIDLIST pidlVirtual) : CBaseFolderViewCB::CBaseFolderViewCB(pidl)
{
	Log::Debug(L"CRealFolderFolderViewCB::CRealFolderFolderViewCB(LPITEMIDLIST pidl) : CBaseFolderViewCB::CBaseFolderViewCB(pidl)");
	m_pidlVirtual = ILCloneFull(pidlVirtual);
}

CRealFolderFolderViewCB::~CRealFolderFolderViewCB()
{
	Log::Debug(L"CRealFolderFolderViewCB::~CRealFolderFolderViewCB");
}

IFACEMETHODIMP CRealFolderFolderViewCB::GetViewMode(FOLDERLOGICALVIEWMODE* plvm)
{
	Log::Debug(L"CRealFolderFolderViewCB::GetViewMode(FOLDERLOGICALVIEWMODE* plvm)");
	*plvm = FLVM_DETAILS;
	return S_OK;
}

IFACEMETHODIMP CRealFolderFolderViewCB::GetGroupByProperty(PROPERTYKEY* /* pkey */, BOOL* /* pfGroupAscending */)
{
	Log::Debug(L"CRealFolderFolderViewCB::GetGroupByProperty(PROPERTYKEY* /* pkey */, BOOL* /* pfGroupAscending */)");
	return E_NOTIMPL;
}

// IShellFolderViewCB
IFACEMETHODIMP CRealFolderFolderViewCB::MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	Log::Debug(L"CBaseFolderViewCB::MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam)");
	HRESULT hr;
	hr = CBaseFolderViewCB::MessageSFVCB(uMsg, wParam, lParam);
	if (uMsg == SFVM_FSNOTIFY)
	{
		SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_IDLIST, m_pidlVirtual, NULL);
		SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_IDLIST, m_pidlVirtual, NULL);
		if (lParam != SHCNE_UPDATEDIR)
		{
			SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_IDLIST, m_pidl, NULL);
		}
	}
	return hr;
}