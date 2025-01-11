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
#include <propkey.h>
#include "Log.h"
#include "BaseFolderViewChildHelper.h"
#include "ServerFolderViewEnumIDList.h"
#include "ComUtils.h"
#include "RealFolderFolderView.h"
#include "ServerFolderView.h"
#include "BaseFolderViewCB.h"

//  IPersistFolder method
HRESULT CServerFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)
{
	Log::Debug(L"CServerFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
	HRESULT hr = CBaseFolderView::Initialize(pidl);
	if (FAILED(hr))
		return hr;
	PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(ILFindLastID(pidl));
	hr = pMyObj ? S_OK : E_INVALIDARG;
	if (FAILED(hr))
		return hr;
	WCHAR szModuleName[MAX_PATH];
	hr = CBaseFolderViewChildHelper::GetModuleName(ILFindLastID(pidl), szModuleName, MAX_PATH);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(m_szModuleName, MAX_PATH, szModuleName);
	if (FAILED(hr))
		return hr;
	m_ulAttributes = pMyObj->ulAttributes;
	return hr;
}

HRESULT CServerFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,
	REFIID riid, UINT *prgfInOut, void **ppv)
{
	Log::Debug(L"CServerFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT * prgfInOut, void** ppv)");
	*ppv = NULL;
	HRESULT hr = S_OK;
	BOOL fIsFolder;
	PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(*ppidl);
	hr = pMyObj ? S_OK : E_INVALIDARG;
	if (FAILED(hr))
		return hr;
	fIsFolder = pMyObj->fIsFolder;
	if (riid == IID_IDataObject)
	{
		IShellFolder* pParent;
		hr = GetParentRealFolder(*ppidl, &pParent);
		if (SUCCEEDED(hr))
		{
			LPITEMIDLIST pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(*ppidl);
			hr = pidlOriginal ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				LPITEMIDLIST pidlLastID = ILFindLastID(pidlOriginal);
				hr = pidlLastID ? S_OK : E_OUTOFMEMORY;
				if (SUCCEEDED(hr))
				{
					hr = SHCreateDataObject(m_pidl, cidl, ppidl, NULL, riid, ppv);
				}
				ILFree(pidlOriginal);
			}
			pParent->Release();
		}
		return hr;
	}
	else if (riid == IID_IDropTarget || riid == IID_IQueryAssociations)
	{
		IShellFolder* pParent;
		hr = GetParentRealFolder(*ppidl, &pParent);
		if (SUCCEEDED(hr))
		{
			LPITEMIDLIST pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(*ppidl);
			hr = pidlOriginal ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				LPITEMIDLIST pidlLastID = ILFindLastID(pidlOriginal);
				hr = pidlLastID ? S_OK : E_OUTOFMEMORY;
				if (SUCCEEDED(hr))
				{
					hr = pParent->GetUIObjectOf(hwnd, 1, const_cast<LPCITEMIDLIST*>(&pidlLastID),
						riid, prgfInOut, reinterpret_cast<void**>(ppv));
				}
				ILFree(pidlOriginal);
			}
			pParent->Release();
		}
	}
	else
	{
		hr = CBaseFolderView::GetUIObjectOf(hwnd, cidl, ppidl, riid, prgfInOut, ppv);
	}
	return hr;
}

//  Factory for handlers for the specified item.
HRESULT CServerFolderView::BindToObject(PCUIDLIST_RELATIVE pidl,
	IBindCtx *pbc, REFIID riid, void **ppv)
{
	Log::Debug(L"CServerFolderView::BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
	HRESULT hr = E_FAIL;
	*ppv = NULL;
	PCFVITEMID pMyObj =  CBaseFolderViewChildHelper::IsValid(pidl);
	hr = pMyObj ? S_OK : E_INVALIDARG;
	if (FAILED(hr))
		return hr;
	if (riid == IID_IStream /* || riid == IID_ITransferMediumItem || riid == IID_IViewStateIdentityItem || riid == IID_IDisplayItem  || riid == IID_IIdentityName */ || riid == IID_IPropertyStoreFactory|| riid == IID_IPersistFile || riid == IID_IPropertyStore)
	{
		if (riid == IID_IPropertyStoreFactory || riid == IID_IPropertyStore)
			return E_NOTIMPL;
		if (ILFindLastID(pidl) == pidl)
		{
			IShellFolder* pDesktop;
			hr = SHGetDesktopFolder(&pDesktop);
			if (SUCCEEDED(hr))
			{
				if (pMyObj->fIsRealPath == TRUE)
				{
					LPITEMIDLIST pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(pidl);
					hr = pidlOriginal ? S_OK : E_OUTOFMEMORY;
					if (SUCCEEDED(hr))
					{
						hr = pDesktop->BindToObject(pidlOriginal, pbc, riid, ppv);
						ILFree(pidlOriginal);
					}
				}
				else
					hr = E_NOTIMPL;
				pDesktop->Release();
			}
			return hr;
		}
	}
	return CBaseFolderView::BindToObject(pidl, pbc, riid, ppv);
}

//  Called to determine the equivalence and/or sort order of two idlists.
HRESULT CServerFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)
{
	Log::Debug(L"CServerFolderView::CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2)");
	PCFVITEMID pMyObj1 = CBaseFolderViewChildHelper::IsValid(pidl1);
	PCFVITEMID pMyObj2 = CBaseFolderViewChildHelper::IsValid(pidl2);
	if (pMyObj1 == NULL || pMyObj2 == NULL)
	{
		return E_INVALIDARG;
	}
	if ((lParam & SHCIDS_ALLFIELDS) != 0 || (lParam & SHCIDS_COLUMNMASK) == 0)
	{
		if (pMyObj1->ulGroupOrder < pMyObj2->ulGroupOrder)
			return ResultFromShort(-1);
		else if (pMyObj1->ulGroupOrder > pMyObj2->ulGroupOrder)
			return ResultFromShort(1);
		else
		{
			HRESULT hr;
			WCHAR szName1[MAX_PATH];
			hr = CBaseFolderViewChildHelper::GetParseName(pidl1, szName1, MAX_PATH);
			if (SUCCEEDED(hr))
			{
				WCHAR szName2[MAX_PATH];
				hr = CBaseFolderViewChildHelper::GetParseName(pidl2, szName2, MAX_PATH);
				if (SUCCEEDED(hr))
				{
					if (StrCmpW(szName1, szName2))
						hr = ResultFromShort(StrCmpW(szName1, szName2));
					else
						hr = ILCompareRelIDs(this, pidl1, pidl2, lParam);
				}
			}
			return hr;
		}
	}
	else if ((lParam & SHCIDS_COLUMNMASK) == 1)
	{
		if (pMyObj1->ulGroupOrder < pMyObj2->ulGroupOrder)
			return ResultFromShort(-1);
		else if (pMyObj1->ulGroupOrder > pMyObj2->ulGroupOrder)
			return ResultFromShort(1);
		else
			return ILCompareRelIDs(this, pidl1, pidl2, lParam);
	}
	else
	{
		return E_INVALIDARG;
	}
}

HRESULT CServerFolderView::QueryInterface(REFIID riid, void** ppv)
{
	Log::Debug(L"CServerFolderView::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] =
	{
		QITABENT(CServerFolderView, IShellFolder),
		QITABENT(CServerFolderView, IShellFolder2),
		QITABENT(CServerFolderView, IPersist),
		QITABENT(CServerFolderView, IPersistFolder),
		QITABENT(CServerFolderView, IPersistFolder2),
		QITABENT(CServerFolderView, IExplorerPaneVisibility),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

ULONG CServerFolderView::AddRef()
{
	Log::Debug(L"CServerFolderView::AddRef()");
	return InterlockedIncrement(&m_cRef);
}

ULONG CServerFolderView::Release()
{
	Log::Debug(L"CServerFolderView::Release()");
	long cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}
	return cRef;
}

CBaseFolderViewEnumIDList* CServerFolderView::CreateEnumIDList(DWORD grfFlags)
{
	Log::Debug(L"CServerFolderView::CreateEnumIDList(DWORD grfFlags)");
	return new (std::nothrow) CServerFolderViewEnumIDList(grfFlags, m_szModuleName);
}

CBaseFolderView* CServerFolderView::CreateChildFolderView()
{
	Log::Debug(L"CServerFolderView::CreateChildFolderView()");
	return new (std::nothrow) CRealFolderFolderView();
}

CServerFolderView::CServerFolderView() : CBaseFolderView(), m_szModuleName(L"")
{
	Log::Debug(L"CServerFolderView::CServerFolderView() : CBaseFolderView()");
}

CServerFolderView::~CServerFolderView()
{
	Log::Debug(L"CServerFolderView::~CServerFolderView()");
}