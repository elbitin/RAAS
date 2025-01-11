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
#include "Log.h"
#include "BaseFolderViewChildHelper.h"
#include "RootFolderViewEnumIDList.h"
#include "ComUtils.h"
#include "ServerFolderView.h"
#include "RootFolderView.h"
#include "BaseFolderViewCB.h"

//  IPersistFolder method
HRESULT CRootFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)
{
	try
	{
		Log::Debug(L"CRootFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		return CBaseFolderView::Initialize(pidl);
	}
	catch (...)
	{
		Log::Error(L"CRootFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		return E_FAIL;
	}
}

HRESULT CRootFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,
	REFIID riid, UINT *prgfInOut, void **ppv)
{
	try
	{
		Log::Debug(L"CRootFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT * prgfInOut, void** ppv)");
		*ppv = NULL;
		HRESULT hr;
		if (riid == IID_IQueryAssociations)
		{
			ASSOCIATIONELEMENT const rgAssocFolder[] =
			{
			{ ASSOCCLASS_PROGID_STR, NULL, L"RAASServer" },
			{ ASSOCCLASS_FOLDER, NULL, NULL },
			};
			hr = AssocCreateForClasses(rgAssocFolder, ARRAYSIZE(rgAssocFolder), riid, ppv);
		}
		else {
			hr = CBaseFolderView::GetUIObjectOf(hwnd, cidl, ppidl, riid, prgfInOut, ppv);
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CRootFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT * prgfInOut, void** ppv)");
		return E_FAIL;
	}
}

//  Called to determine the equivalence and/or sort order of two idlists.
HRESULT CRootFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)
{
	try
	{
		Log::Debug(L"CRootFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)");
		HRESULT hr;
		PCFVITEMID pMyObj1 = CBaseFolderViewChildHelper::IsValid(pidl1);
		PCFVITEMID pMyObj2 = CBaseFolderViewChildHelper::IsValid(pidl2);
		if (pMyObj1 == NULL || pMyObj2 == NULL)
		{
			return E_INVALIDARG;
		}
		WCHAR szParseName1[MAX_PATH];
		WCHAR szParseName2[MAX_PATH];
		hr = CBaseFolderViewChildHelper::GetParseName(pidl1, szParseName1, MAX_PATH);
		if (FAILED(hr))
			return hr;
		hr = CBaseFolderViewChildHelper::GetParseName(pidl2, szParseName2, MAX_PATH);
		if (FAILED(hr))
			return hr;
		if (StrCmpW(szParseName1, szParseName2))
			return ResultFromShort(StrCmpW(szParseName1, szParseName2));
		else
			return ILCompareRelIDs(this, pidl1, pidl2, lParam);
	}
	catch (...)
	{
		Log::Error(L"CRootFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)");
		return E_FAIL;
	}
}

HRESULT CRootFolderView::QueryInterface(REFIID riid, void** ppv)
{
	Log::Debug(L"CRootFolderView::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] =
	{
		QITABENT(CRootFolderView, IShellFolder),
		QITABENT(CRootFolderView, IShellFolder2),
		QITABENT(CRootFolderView, IPersist),
		QITABENT(CRootFolderView, IPersistFolder),
		QITABENT(CRootFolderView, IPersistFolder2),
		QITABENT(CRootFolderView, IExplorerPaneVisibility),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

ULONG CRootFolderView::AddRef()
{
	Log::Debug(L"CRootFolderView::AddRef()");
	return InterlockedIncrement(&m_cRef);
}

ULONG CRootFolderView::Release()
{
	Log::Debug(L"CRootFolderView::Release()");
	long cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}
	return cRef;
}

CBaseFolderViewEnumIDList* CRootFolderView::CreateEnumIDList(DWORD grfFlags)
{
	Log::Debug(L"CRootFolderView::CreateEnumIDList(DWORD grfFlags)");
	return new (std::nothrow) CRootFolderViewEnumIDList(grfFlags);
}

CBaseFolderView* CRootFolderView::CreateChildFolderView()
{
	Log::Debug(L"CRootFolderView::CreateChildFolderView()");
	return new (std::nothrow) CServerFolderView();
}

CRootFolderView::CRootFolderView() : CBaseFolderView()
{

}

CRootFolderView::~CRootFolderView()
{

}