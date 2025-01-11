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
#include <shlwapi.h>
#include <strsafe.h>
#include <shlwapi.h>
#include <shellapi.h>
#include <strsafe.h>
#include <shlobj.h>
#include "resource.h"
#include "Log.h"
#include "ComUtils.h"
#include "BaseFolderViewChildHelper.h"
#include "RealFolderFolderView.h"
#include "RealFolderFolderViewCB.h"

//  IPersistFolder method
HRESULT CRealFolderFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		HRESULT hr = CBaseFolderView::Initialize(pidl);
		if (FAILED(hr))
			return hr;
		hr = SHGetDesktopFolder(&m_pDesktop);
		if (FAILED(hr))
			return hr;
		SFGAOF attributes = 0xffffffff;
		m_pidlOriginal = _GetRealPidl(const_cast<LPITEMIDLIST>(pidl));
		hr = m_pidlOriginal ? S_OK : E_OUTOFMEMORY;
		if (FAILED(hr))
			return hr;
		hr = m_pDesktop->GetAttributesOf(1, const_cast<LPCITEMIDLIST*>(&m_pidlOriginal), &attributes);
		if (FAILED(hr))
			return hr;
		hr = ((attributes & SFGAO_FOLDER) != 0) ? S_OK : E_NOINTERFACE;
		if (FAILED(hr))
			return hr;
		LPWSTR szFileName;
		STRRET fullPath;
		hr = m_pDesktop->GetDisplayNameOf(m_pidlOriginal, SHGDN_FORPARSING, &fullPath);
		if (SUCCEEDED(hr))
		{
			hr = StrRetToStr(&fullPath, m_pidlOriginal, &szFileName);
			if (SUCCEEDED(hr))
			{
				hr = StringCchCopyW(m_szRealFolderPath, MAX_PATH, szFileName);
				CoTaskMemFree(szFileName);
			}
		}
		if (FAILED(hr))
			return hr;

		// m_pDesktop here, m_pFolder with child pidl have caused problems
		hr = m_pDesktop->BindToObject(m_pidlOriginal, NULL, IID_IShellFolder2, reinterpret_cast<void**>(&m_pFolder));
		if (FAILED(hr))
			return hr;
		hr = m_pDesktop->BindToObject(m_pidlOriginal, NULL, IID_IStorage, reinterpret_cast<void**>(&m_pStorage));
		if (FAILED(hr))
			return hr;

		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		return E_FAIL;
	}
}

HRESULT CRealFolderFolderView::QueryInterface(REFIID riid, void **ppv)
{
	Log::Debug(L"CRealFolderFolderView::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] =
	{
		QITABENT(CRealFolderFolderView, IShellFolder),
		QITABENT(CRealFolderFolderView, IShellFolder2),
		QITABENT(CRealFolderFolderView, IPersist),
		QITABENT(CRealFolderFolderView, IPersistFolder),
		QITABENT(CRealFolderFolderView, IPersistFolder2),
		QITABENT(CRealFolderFolderView, IStorage),
		QITABENT(CRealFolderFolderView, IExplorerPaneVisibility),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

ULONG CRealFolderFolderView::AddRef()
{
	Log::Debug(L"CRealFolderFolderView::AddRef()");
	return InterlockedIncrement(&m_cRef);
}

ULONG CRealFolderFolderView::Release()
{
	Log::Debug(L"CRealFolderFolderView::Release()");
	long cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}
	return cRef;
}

//  Translates a display name into an item identifier list.
HRESULT CRealFolderFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName,
	ULONG *pchEaten, PIDLIST_RELATIVE *ppidl, ULONG *pulAttributes)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName, ULONG * pchEaten, PIDLIST_RELATIVE * ppidl, ULONG * pulAttributes)");
		return m_pFolder->ParseDisplayName(hwnd, pbc, pszName, pchEaten, ppidl, pulAttributes);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName, ULONG * pchEaten, PIDLIST_RELATIVE * ppidl, ULONG * pulAttributes)");
		return E_FAIL;
	}
}

//  Allows a client to determine the contents of a folder by
//  creating an item identifier enumeration object and returning
//  its IEnumIDList interface. The methods supported by that
//  interface can then be used to enumerate the folder's contents.
HRESULT CRealFolderFolderView::EnumObjects(HWND hwnd, DWORD grfFlags, IEnumIDList **ppenumIDList)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::EnumObjects(HWND hwnd, DWORD grfFlags, IEnumIDList **ppenumIDList)");
		return m_pFolder->EnumObjects(hwnd, grfFlags, ppenumIDList);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::EnumObjects(HWND hwnd, DWORD grfFlags, IEnumIDList **ppenumIDList)");
		return E_FAIL;
	}
}

//  Factory for handlers for the specified item.
HRESULT CRealFolderFolderView::BindToObject(PCUIDLIST_RELATIVE pidl,
	IBindCtx *pbc, REFIID riid, void **ppv)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		HRESULT hr;
		*ppv = NULL;
		if (riid == IID_IStream || riid == IID_IPropertyStoreFactory|| riid == IID_IPersistFile || riid == IID_IPropertyStore)
		{
			if (ILFindLastID(pidl) == pidl)
			{
				hr = m_pFolder->BindToObject(pidl, pbc, riid, ppv);
				return hr;
			}
		}
		return CBaseFolderView::BindToObject(pidl, pbc, riid, ppv);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return E_FAIL;
	}
}

HRESULT CRealFolderFolderView::BindToStorage(PCUIDLIST_RELATIVE pidl,
	IBindCtx *pbc, REFIID riid, void **ppv)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return m_pFolder->BindToStorage(pidl, pbc, riid, ppv);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return E_FAIL;
	}
}

HRESULT CRealFolderFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)");
		uint16_t iColumn = (uint16_t)(lParam & 0xFFFFUL);
		int iMappedColumn = 0;
		_SwitchColumns(iColumn, iMappedColumn);
		int lParamSwitched = (lParam & (0xFFFFUL << 16)) | iMappedColumn;
		return m_pFolder->CompareIDs(lParamSwitched, pidl1, pidl2);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)");
		return E_FAIL;
	}
}

//  Called by the Shell to create the View Object and return it.
HRESULT CRealFolderFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)");
		HRESULT hr;
		if (riid == IID_ICategoryProvider || riid == IID_IDropTarget || riid == IID_IShellDetails || riid == IID_IIdentityName || riid == IID_IShellItemResources || riid == IID_ICategoryProvider)
		{
			hr = m_pFolder->CreateViewObject(hwnd, riid, ppv);
		}
		else if (riid == IID_IContextMenu)
		{
			// This is the background context menu for the folder itself, not the context menu on items within it.
			HKEY aKeys[2];
			HKEY hkDirectoryBackgroundResult;
			hr = RegOpenKeyEx(HKEY_CLASSES_ROOT, L"Directory\\Background", NULL, KEY_QUERY_VALUE, &hkDirectoryBackgroundResult);
			if (SUCCEEDED(hr))
			{
				aKeys[0] = hkDirectoryBackgroundResult;
				HKEY hkRealFolderBackgroundResult;
				hr = RegOpenKeyEx(HKEY_CLASSES_ROOT, L"RAASClientRealFolder\\Background", NULL, KEY_QUERY_VALUE, &hkRealFolderBackgroundResult);
				if (SUCCEEDED(hr))
				{
					aKeys[1] = hkRealFolderBackgroundResult;
					DEFCONTEXTMENU const dcm = { hwnd, NULL, m_pidl, static_cast<IShellFolder2*>(this),
						0, NULL, NULL, 2, aKeys };
					hr = SHCreateDefaultContextMenu(&dcm, riid, ppv);
				}
			}
		}
		else
		{
			hr = CBaseFolderView::CreateViewObject(hwnd, riid, ppv);
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)");
		return E_FAIL;
	}
}

//  Retrieves the attributes of one or more file objects or subfolders.
HRESULT CRealFolderFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)");
		return m_pFolder->GetAttributesOf(cidl, apidl, rgfInOut);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)");
		return E_FAIL;
	}
}

//  Retrieves an OLE interface that can be used to carry out
//  actions on the specified file objects or folders.
HRESULT CRealFolderFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,
	REFIID riid, UINT *prgfInOut, void **ppv)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT * prgfInOut, void** ppv)");
		HRESULT hr;
		BOOL fIsFolder;
		ULONG ulAttributes = 0xffffffff;
		m_pFolder->GetAttributesOf(cidl, ppidl, &ulAttributes);
		if ((ulAttributes & SFGAO_FOLDER) != 0)
		{
			fIsFolder = TRUE;
		}
		else
		{
			fIsFolder = FALSE;
		}
		if (riid == IID_IDataObject)
		{
			hr = SHCreateDataObject(m_pidl, cidl, ppidl, NULL, riid, ppv);
		}
		else if (riid == IID_IContextMenu || riid == IID_IContextMenu2 || riid == IID_IContextMenu3)
		{
			DEFCONTEXTMENU const dcm = { hwnd, NULL, m_pidl, static_cast<IShellFolder2*>(this),
				cidl, ppidl, NULL, 0, NULL };
			hr = SHCreateDefaultContextMenu(&dcm, riid, ppv);
		}
		else
		{
			hr = m_pFolder->GetUIObjectOf(hwnd, cidl, ppidl,
				riid, prgfInOut, ppv);
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT * prgfInOut, void** ppv)");
		return E_FAIL;
	}
}

//  Retrieves the display name for the specified file object or subfolder.
HRESULT CRealFolderFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)");
		return m_pFolder->GetDisplayNameOf(pidl, shgdnFlags, pName);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)");
		return E_FAIL;
	}
}

//  Sets the display name of a file object or subfolder, changing
//  the item identifier in the process.
HRESULT CRealFolderFolderView::SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl,
	PCWSTR pszName, DWORD uFlags, PITEMID_CHILD *ppidlOut)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut)");
		HRESULT hr;
		hr = m_pFolder->SetNameOf(hwnd,
			pidl,
			pszName,
			uFlags,
			ppidlOut);
		if (SUCCEEDED(hr))
		{
			PCUIDLIST_ABSOLUTE pidlBefore = ILCombine(m_pidl, pidl);
			hr = pidlBefore ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				PCUIDLIST_ABSOLUTE pidlAfter = ILCombine(m_pidl, *ppidlOut);
				hr = pidlBefore ? S_OK : E_OUTOFMEMORY;
				if (SUCCEEDED(hr))
				{
					::SHChangeNotify(SHCNE_RENAMEITEM, SHCNF_IDLIST, pidlBefore, pidlAfter);
					::SHChangeNotify(SHCNE_RENAMEFOLDER, SHCNF_IDLIST | SHCNF_FLUSH, pidlBefore, pidlAfter);
					ILFree((LPITEMIDLIST)pidlAfter);
				}
				ILFree((LPITEMIDLIST)pidlBefore);
			}
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut)");
		return E_FAIL;
	}
}

//  Retrieves the default sorting and display column (indices from GetDetailsOf).
HRESULT CRealFolderFolderView::GetDefaultColumn(DWORD dwRes,
	ULONG *pSort,
	ULONG *pDisplay)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetDefaultColumn(DWORD dwRes, ULONG * pSort, 	ULONG * pDisplay)");
		HRESULT hr;
		hr = m_pFolder->GetDefaultColumn(dwRes,
			pSort,
			pDisplay);
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetDefaultColumn(DWORD dwRes, ULONG * pSort, 	ULONG * pDisplay)");
		return E_FAIL;
	}
}

//  Retrieves the default state for a specified column.
HRESULT CRealFolderFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)");
		HRESULT hr;
		int iMappedColumn = 0;
		_SwitchColumns(iColumn, iMappedColumn);
		hr = m_pFolder->GetDefaultColumnState(iMappedColumn,
			pcsFlags);
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)");
		return E_FAIL;
	}
}

//  Retrieves detailed information, identified by a
//  property set ID (FMTID) and property ID (PID),
//  on an item in a Shell folder.
HRESULT CRealFolderFolderView::GetDetailsEx(PCUITEMID_CHILD pidl,
	const PROPERTYKEY *pkey,
	VARIANT *pv)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY * pkey, VARIANT * pv)");
		return m_pFolder->GetDetailsEx(pidl, pkey, pv);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY * pkey, VARIANT * pv)");
		return E_FAIL;
	}
}

//  Retrieves detailed information, identified by a
//  column index, on an item in a Shell folder.
HRESULT CRealFolderFolderView::GetDetailsOf(PCUITEMID_CHILD pidl,
	UINT iColumn,
	SHELLDETAILS *pDetails)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS * pDetails)");
		HRESULT hr;
		if (pidl == NULL)
		{
			PROPERTYKEY key;
			hr = MapColumnToSCID(iColumn, &key);
			IPropertyDescription* sppropdesc;
			hr = PSGetPropertyDescription(key, IID_PPV_ARGS(&sppropdesc));
			if (!SUCCEEDED(hr))
				return E_FAIL;
			hr = CBaseFolderView::GetDetailsOf(sppropdesc, pDetails);
			if (iColumn == 0)
				pDetails->cxChar = cx_cCHAR_NAME;
			if (iColumn == 2)
			{
				HMODULE lib = LoadLibrary(L"shell32.dll");
				if (lib == 0)
					return E_FAIL;
				WCHAR szType[80];
				LoadString(lib, IDS_SHELL32_TYPE, szType, ARRAYSIZE(szType));
				FreeLibrary(lib);
				hr = StringToStrRet(szType, &pDetails->str);
			}
			return hr;
		}
		hr = m_pFolder->GetDetailsOf(pidl,
			iColumn,
			pDetails);
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS * pDetails)");
		return E_FAIL;
	}
}

//  Converts a column name to the appropriate
//  property set ID (FMTID) and property ID (PID).
HRESULT CRealFolderFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)");
		HRESULT hr;

		// Switch default column order
		int iMappedColumn = 0;
		_SwitchColumns(iColumn, iMappedColumn);

		hr = m_pFolder->MapColumnToSCID(iMappedColumn, pkey);
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)");
		return E_FAIL;
	}
}

// IStorage

STDMETHODIMP CRealFolderFolderView::CreateStream(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStream** ppstm)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::CreateStream(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStream** ppstm)");
		BOOL fTest = FALSE;
		WCHAR szFileName[MAX_PATH];
		wsprintfW(szFileName, L"%s\\%s", m_szRealFolderPath, pwcsName);
		HANDLE hFile = CreateFile(szFileName, GENERIC_WRITE, FILE_SHARE_WRITE,
			NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		fTest = CloseHandle(hFile);
		if (hFile == INVALID_HANDLE_VALUE)
		{
			DWORD dwLastError = GetLastError();
			return  HRESULT_FROM_WIN32(dwLastError);
		}
		DeleteFile(szFileName);
		return m_pStorage->CreateStream(pwcsName, grfMode, reserved1, reserved2, ppstm);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::CreateStream(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStream** ppstm)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::OpenStream(LPCWSTR pwcsName, LPVOID reserved1, DWORD grfMode, DWORD reserved2, IStream** ppstm)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::OpenStream(LPCWSTR pwcsName, LPVOID reserved1, DWORD grfMode, DWORD reserved2, IStream** ppstm)");
		return m_pStorage->OpenStream(pwcsName, reserved1, grfMode, reserved2, ppstm);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::OpenStream(LPCWSTR pwcsName, LPVOID reserved1, DWORD grfMode, DWORD reserved2, IStream** ppstm)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::CreateStorage(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStorage** ppstg)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::CreateStorage(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStorage** ppstg)");
		return m_pStorage->CreateStorage(pwcsName, grfMode, reserved1, reserved2, ppstg);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::CreateStorage(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStorage** ppstg)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::OpenStorage(LPCWSTR pwcsName, IStorage* pstgPriority, DWORD grfMode, SNB snbExclude, DWORD reserved, IStorage** ppstg)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::OpenStorage(LPCWSTR pwcsName, IStorage* pstgPriority, DWORD grfMode, SNB snbExclude, DWORD reserved, IStorage** ppstg)");
		return m_pStorage->OpenStorage(pwcsName, pstgPriority, grfMode, snbExclude, reserved, ppstg);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::OpenStorage(LPCWSTR pwcsName, IStorage* pstgPriority, DWORD grfMode, SNB snbExclude, DWORD reserved, IStorage** ppstg)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::CopyTo(DWORD ciidExclude, const IID* rgiidExclude, SNB snbExclude, IStorage* pstgDest)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::CopyTo(DWORD ciidExclude, const IID* rgiidExclude, SNB snbExclude, IStorage* pstgDest)");
		return m_pStorage->CopyTo(ciidExclude, rgiidExclude, snbExclude, pstgDest);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::CopyTo(DWORD ciidExclude, const IID* rgiidExclude, SNB snbExclude, IStorage* pstgDest)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::MoveElementTo(LPCWSTR pwcsName, IStorage* pstgDest, LPCWSTR pwcsNewName, DWORD grfFlags)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::MoveElementTo(LPCWSTR pwcsName, IStorage* pstgDest, LPCWSTR pwcsNewName, DWORD grfFlags)");
		return m_pStorage->MoveElementTo(pwcsName, pstgDest, pwcsNewName, grfFlags);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::MoveElementTo(LPCWSTR pwcsName, IStorage* pstgDest, LPCWSTR pwcsNewName, DWORD grfFlags)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::Commit(DWORD grfCommitFlags)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::Commit(DWORD grfCommitFlags)");
		return m_pStorage->Commit(grfCommitFlags);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::Commit(DWORD grfCommitFlags)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::Revert()
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::Revert()");
		return m_pStorage->Revert();
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::Revert()");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::EnumElements(DWORD reserved1, LPVOID reserved2, DWORD reserved3, IEnumSTATSTG** ppenum)
{
	try
	{
		HRESULT hr;
		Log::Debug(L"CRealFolderFolderView::EnumElements(DWORD reserved1, LPVOID reserved2, DWORD reserved3, IEnumSTATSTG** ppenum)");
		hr = m_pStorage->EnumElements(reserved1, reserved2, reserved3, ppenum);
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::EnumElements(DWORD reserved1, LPVOID reserved2, DWORD reserved3, IEnumSTATSTG** ppenum)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::DestroyElement(LPCWSTR pwcsName)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::DestroyElement(LPCWSTR pwcsName)");
		HRESULT hr;
		wchar_t szFullPath[MAX_PATH];
		hr = StringCchPrintf(szFullPath, MAX_PATH, L"%s\\%s", m_szRealFolderPath, pwcsName);
		if (PathFileExists(szFullPath) || PathIsDirectory(szFullPath))
		{
			hr = m_pStorage->DestroyElement(pwcsName);
			if (hr != S_OK)
			{
				BOOL bSuccess;
				if (PathFileExists(szFullPath))
					bSuccess = DeleteFile(szFullPath);
				else
					bSuccess = RemoveDirectory(szFullPath);
				if (!bSuccess)
				{
					DWORD dwLastError = GetLastError();
					return  HRESULT_FROM_WIN32(dwLastError);
				}
			}
		}
		else
		{
			hr = S_OK;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::DestroyElement(LPCWSTR pwcsName)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::RenameElement(LPCWSTR pwcsOldName, LPCWSTR pwcsNewName)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::RenameElement(LPCWSTR pwcsOldName, LPCWSTR pwcsNewName)");
		return m_pStorage->RenameElement(pwcsOldName, pwcsNewName);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::RenameElement(LPCWSTR pwcsOldName, LPCWSTR pwcsNewName)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::SetElementTimes(LPCWSTR pwcsName, const FILETIME* pctime, const FILETIME* patime, const FILETIME* pmtime)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::SetElementTimes(LPCWSTR pwcsName, const FILETIME* pctime, const FILETIME* patime, const FILETIME* pmtime)");
		return m_pStorage->SetElementTimes(pwcsName, pctime, patime, pmtime);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::SetElementTimes(LPCWSTR pwcsName, const FILETIME* pctime, const FILETIME* patime, const FILETIME* pmtime)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::SetClass(REFCLSID clsid)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::SetClass(REFCLSID clsid)");
		return m_pStorage->SetClass(clsid);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::SetClass(REFCLSID clsid)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::SetStateBits(DWORD grfStateBits, DWORD grfMask)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::SetStateBits(DWORD grfStateBits, DWORD grfMask)");
		return m_pStorage->SetStateBits(grfStateBits, grfMask);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::SetStateBits(DWORD grfStateBits, DWORD grfMask)");
		return E_FAIL;
	}
}

STDMETHODIMP CRealFolderFolderView::Stat(STATSTG* pStatstg, DWORD grfStatFlag)
{
	try
	{
		Log::Debug(L"CRealFolderFolderView::Stat(STATSTG* pStatstg, DWORD grfStatFlag)");
		return m_pStorage->Stat(pStatstg, grfStatFlag);
	}
	catch (...)
	{
		Log::Error(L"CRealFolderFolderView::Stat(STATSTG* pStatstg, DWORD grfStatFlag)");
		return E_FAIL;
	}
}

CBaseFolderView* CRealFolderFolderView::CreateChildFolderView()
{
	Log::Debug(L"CRealFolderFolderView::CreateChildFolderView()");
	return new (std::nothrow) CRealFolderFolderView();
}

void CRealFolderFolderView::_SwitchColumns(const UINT &iColumn, int &iMappedColumn)
{
	Log::Debug(L"CRealFolderFolderView::_SwitchColumns(const UINT &iColumn, int &iMappedColumn)");
	// Switch column order
	switch (iColumn)
	{
	case 0:
		iMappedColumn = DEFAULT_COLUMN_ORDER.n_cNAME;
		break;
	case 1:
		iMappedColumn = DEFAULT_COLUMN_ORDER.n_cLATEST_CHANGE;
		break;
	case 2:
		iMappedColumn = DEFAULT_COLUMN_ORDER.n_cTYPE;
		break;
	case 3:
		iMappedColumn = DEFAULT_COLUMN_ORDER.n_cSIZE;
		break;
	default:
		iMappedColumn = iColumn;
		break;
	}
}

HRESULT CRealFolderFolderView::CreateFolderViewCB(REFIID riid, void** ppv)
{
	Log::Debug(L"CRealFolderFolderView::CreateFolderViewCB(REFIID riid, void** ppv)");
	*ppv = NULL;
	HRESULT hr = E_OUTOFMEMORY;
	CRealFolderFolderViewCB* pfvcb = new (std::nothrow) CRealFolderFolderViewCB(m_pidlOriginal, m_pidl);
	if (pfvcb)
	{
		hr = pfvcb->QueryInterface(riid, ppv);
		pfvcb->Release();
	}
	return hr;
}

LPITEMIDLIST CRealFolderFolderView::_GetRealPidl(LPITEMIDLIST pidl)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::_GetRealPidl(LPITEMIDLIST pidl)");
	LPITEMIDLIST pidlOriginal = NULL;
	PCUIDLIST_RELATIVE nextPidl = pidl;
	do
	{
		nextPidl = ILNext(nextPidl);
		if (!ILIsEmpty(nextPidl))
		{
			PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(nextPidl);
			if (pMyObj != NULL && pMyObj->fIsRealPath == TRUE)
			{
				pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(nextPidl);
				nextPidl = ILNext(nextPidl);
				if (!ILIsEmpty(nextPidl))
				{
					LPITEMIDLIST pidlOriginalOld = pidlOriginal;
					pidlOriginal = ILCombine(pidlOriginalOld, nextPidl);
					ILFree(pidlOriginalOld);
				}
				break;
			}
		}
	} while (!ILIsEmpty(nextPidl));
	return pidlOriginal;
}

CRealFolderFolderView::CRealFolderFolderView() : CBaseFolderView(), m_pDesktop(NULL), m_pFolder(NULL), m_pStorage(NULL), m_pidlOriginal(NULL), m_szRealFolderPath(L"")
{
	Log::Debug(L"CRealFolderFolderView::CRealFolderFolderView() : CBaseFolderView(), m_pDesktop(NULL), m_pFolder(NULL), m_pStorage(NULL), m_pidlOriginal(NULL)");
}

CRealFolderFolderView::~CRealFolderFolderView()
{
	Log::Debug(L"CRealFolderFolderView::~CRealFolderFolderView()");
	SAFE_RELEASE(m_pFolder);
	SAFE_RELEASE(m_pDesktop);
	SAFE_RELEASE(m_pStorage);
	ILFree(m_pidlOriginal);
}