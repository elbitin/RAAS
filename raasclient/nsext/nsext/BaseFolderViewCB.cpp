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
#include "resource.h"
#include "Log.h"
#include "BaseFolderViewCB.h"
#include "BaseFolderView.h"

CBaseFolderViewCB::CBaseFolderViewCB(LPITEMIDLIST pidl) : m_cRef(1), m_pidl(NULL)
{
	Log::Debug(L"CBaseFolderViewCB::CBaseFolderViewCB(LPITEMIDLIST pidl) : m_cRef(1), m_pidl(NULL), m_pidlOriginal(NULL)");
	m_pidl = ILCloneFull(pidl);
}

CBaseFolderViewCB::~CBaseFolderViewCB()
{
	Log::Debug(L"CBaseFolderViewCB::~CBaseFolderViewCB()");
	ILFree(m_pidl);
};

// IUnknown
IFACEMETHODIMP CBaseFolderViewCB::QueryInterface(REFIID riid, void** ppv)
{
	Log::Debug(L"CBaseFolderViewCB::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] =
	{
		QITABENT(CBaseFolderViewCB, IShellFolderViewCB),
		QITABENT(CBaseFolderViewCB, IFolderViewSettings),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CBaseFolderViewCB::AddRef()
{
	Log::Debug(L"CBaseFolderViewCB::AddRef()");
	return InterlockedIncrement(&m_cRef);
}

IFACEMETHODIMP_(ULONG) CBaseFolderViewCB::Release()
{
	Log::Debug(L"CBaseFolderViewCB::Release()");
	long cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}
	return cRef;
}

// IShellFolderViewCB
IFACEMETHODIMP CBaseFolderViewCB::MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	Log::Debug(L"CBaseFolderViewCB::MessageSFVCB(UINT uMsg, WPARAM wParam, LPARAM lParam)");
	if (uMsg == SFVM_DEFVIEWMODE)
	{
		*((FOLDERVIEWMODE*)lParam) = FVM_AUTO;
		return S_OK;
	}
	if (uMsg == SFVM_QUERYFSNOTIFY)
	{
		if (m_pidl != NULL)
		{
			SHChangeNotifyEntry notifyEntry;
			notifyEntry.pidl = m_pidl;
			notifyEntry.fRecursive = TRUE;
			*((SHChangeNotifyEntry*)lParam) = notifyEntry;
			return S_OK;
		}
		return S_OK;
	}
	if (uMsg == SFVM_GETNOTIFY)
	{
		if (m_pidl != NULL)
		{
			*((LPITEMIDLIST*)wParam) = m_pidl;
			*((LONG*)lParam) = SHCNE_RENAMEITEM | SHCNE_RENAMEFOLDER | SHCNE_CREATE | SHCNE_DELETE | SHCNE_MKDIR | SHCNE_RMDIR | SHCNE_UPDATEITEM | SHCNE_ASSOCCHANGED | SHCNE_DISKEVENTS | SHCNE_UPDATEDIR;
			return S_OK;
		}
	}
	if (uMsg == SFVM_FSNOTIFY)
	{
		return S_OK;
	}
	return E_NOTIMPL;
}

// IFolderViewSettings
IFACEMETHODIMP CBaseFolderViewCB::GetColumnPropertyList(REFIID /* riid */, void** ppv)
{
	Log::Debug(L"CBaseFolderViewCB::GetColumnPropertyList(REFIID /* riid */, void **ppv)");
	*ppv = NULL;
	return E_NOTIMPL;
}

IFACEMETHODIMP CBaseFolderViewCB::GetViewMode(FOLDERLOGICALVIEWMODE* plvm)
{
	Log::Debug(L"CBaseFolderViewCB::GetViewMode(FOLDERLOGICALVIEWMODE* plvm)");
	*plvm = FLVM_TILES;
	return S_OK;
}

IFACEMETHODIMP CBaseFolderViewCB::GetGroupByProperty(PROPERTYKEY* pkey, BOOL* pfGroupAscending)
{
	Log::Debug(L"CBaseFolderViewCB::GetGroupByProperty(PROPERTYKEY* pkey, BOOL* pfGroupAscending)");
	*pkey = CBaseFolderView::pkey_cItemGroup;
	wchar_t szDevicesAndDrives[MAX_PATH];
	wchar_t szFolders[MAX_PATH];
	HMODULE lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	LoadString(lib, IDS_SHELL32_DEVICESANDDRIVES, szDevicesAndDrives, MAX_PATH);
	LoadString(lib, IDS_SHELL32_FOLDERS, szFolders, MAX_PATH);
	FreeLibrary(lib);
	if (StrCmpW(szDevicesAndDrives, szFolders) < 0)
		*pfGroupAscending = FALSE;
	else
		*pfGroupAscending = TRUE;
	return S_OK;
}
IFACEMETHODIMP CBaseFolderViewCB::GetIconSize(UINT* /* puIconSize */)
{
	Log::Debug(L"CBaseFolderViewCB::GetIconSize(UINT * /* puIconSize */)");
	return E_NOTIMPL;
}

IFACEMETHODIMP CBaseFolderViewCB::GetSortColumns(SORTCOLUMN* /* rgSortColumns */, UINT /* cColumnsIn */, UINT* /* pcColumnsOut */)
{
	Log::Debug(L"CBaseFolderViewCB::GetSortColumns(SORTCOLUMN * /* rgSortColumns */, UINT /* cColumnsIn */, UINT * /* pcColumnsOut */)");
	return E_NOTIMPL;
}

IFACEMETHODIMP CBaseFolderViewCB::GetGroupSubsetCount(UINT* /* pcVisibleRows */)
{
	Log::Debug(L"CBaseFolderViewCB::GetGroupSubsetCount(UINT * /* pcVisibleRows */)");
	return E_NOTIMPL;
}

IFACEMETHODIMP CBaseFolderViewCB::GetFolderFlags(FOLDERFLAGS* /* pfolderMask */, FOLDERFLAGS* pfolderFlags)
{
	Log::Debug(L"CBaseFolderViewCB::GetFolderFlags(FOLDERFLAGS *pfolderMask, FOLDERFLAGS *pfolderFlags)");
	if (pfolderFlags)
	{
		*pfolderFlags = FWF_AUTOARRANGE;
	}
	return S_OK;
}