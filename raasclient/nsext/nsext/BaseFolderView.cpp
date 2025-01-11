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
/*
Copyright (c) Elbitin, modifications by Elbitin

This file includes code from Microsofts ExplorerDataProvider and
Googles google-drive-shell-extension but have been modified by
Elbitin, modifications are copyrighted by Elbitin but included is
copyright notices from the other source projects.

ExplorerDataProvider source code can be found at:
https://github.com/Microsoft/Windows-classic-samples

google-drive-shell-extension source code can be found at:
https://github.com/google/google-drive-shell-extension

*/

/**************************************************************************
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

(c) Microsoft Corporation. All Rights Reserved.
**************************************************************************/

/*
Copyright 2014 Google Inc
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#include <shlwapi.h>
#include <strsafe.h>
#include <shlwapi.h>
#include <shellapi.h>
#include <strsafe.h>
#include <shlobj.h>
#include <propkey.h>
#include "resource.h"
#include "Guid.h"
#include "Dll.h"
#include "ComUtils.h"
#include "BaseFolderView.h"
#include "BaseFolderViewChildHelper.h"
#include "Log.h"

const PROPERTYKEY CBaseFolderView::pkey_cItemGroup = { GUID({ 0x2e8792b4, 0x31a1, 0x47e8, { 0xaf, 0x8d, 0xea, 0x77, 0xd8, 0xdc, 0x45, 0x76 } }), 2 };

CBaseFolderView::CBaseFolderView() : m_cRef(1), m_pidl(NULL), m_ulAttributes(0)
{
	Log::Debug(L"CBaseFolderView::CBaseFolderView()");
	DllAddRef();
}

CBaseFolderView::~CBaseFolderView()
{
	Log::Debug(L"CBaseFolderView::~CBaseFolderView()");
	ILFree(m_pidl);
	DllRelease();
}

HRESULT CBaseFolderView::QueryInterface(REFIID riid, void **ppv)
{
	Log::Debug(L"CBaseFolderView::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] =
	{
		QITABENT(CBaseFolderView, IShellFolder),
		QITABENT(CBaseFolderView, IShellFolder2),
		QITABENT(CBaseFolderView, IPersist),
		QITABENT(CBaseFolderView, IPersistFolder),
		QITABENT(CBaseFolderView, IPersistFolder2),
		QITABENT(CBaseFolderView, IExplorerPaneVisibility),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

//  Translates a display name into an item identifier list.
HRESULT CBaseFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName,
	ULONG *pchEaten, PIDLIST_RELATIVE *ppidl, ULONG *pulAttributes)
{
	try
	{
		Log::Debug(L"CBaseFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName, ULONG * pchEaten, PIDLIST_RELATIVE * ppidl, ULONG * pulAttributes)");
		HRESULT hr;
		hr = pszName ? S_OK : E_INVALIDARG;
		if (FAILED(hr))
			return hr;
		WCHAR szNameComponent[MAX_PATH] = {};

		// extract first component of the display name
		PWSTR pszNext = PathFindNextComponent(pszName);
		if (pszNext && *pszNext)
		{
			hr = StringCchCopyN(szNameComponent, ARRAYSIZE(szNameComponent), pszName, lstrlen(pszName) - lstrlen(pszNext));
		}
		else
		{
			hr = StringCchCopy(szNameComponent, ARRAYSIZE(szNameComponent), pszName);
		}

		if (SUCCEEDED(hr))
		{
			hr =GetPidlByName(ppidl, szNameComponent);
			if (SUCCEEDED(hr))
			{
				PIDLIST_RELATIVE pidlCurrent = *ppidl;

				// If there are more components to parse, delegate to the child folder to handle the rest.
				if (pszNext && *pszNext)
				{
					// Bind to current item
					IShellFolder* psf;
					hr = BindToObject(pidlCurrent, pbc, IID_PPV_ARGS(&psf));
					if (SUCCEEDED(hr))
					{
						PIDLIST_RELATIVE pidlNext = NULL;
						hr = psf->ParseDisplayName(hwnd, pbc, pszNext, pchEaten, &pidlNext, pulAttributes);
						if (SUCCEEDED(hr))
						{
							*ppidl = ILCombine(pidlCurrent, pidlNext);
							ILFree(pidlNext);
						}
						psf->Release();
					}
					ILFree(pidlCurrent);
				}
			}
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName, ULONG * pchEaten, PIDLIST_RELATIVE * ppidl, ULONG * pulAttributes)");
		return E_FAIL;
	}
}

HRESULT CBaseFolderView::GetPidlByName(LPITEMIDLIST* ppidl, WCHAR* szNameComponent)
{
	Log::Debug(L"CBaseFolderView::GetPidlByName(LPITEMIDLIST* ppidl, WCHAR* szNameComponent)");
	HRESULT hr;
	*ppidl = NULL;
	PathRemoveBackslash(szNameComponent);
	IEnumIDList* idList;
	hr = EnumObjects(NULL, SHCONTF_FOLDERS, &idList);
	if (FAILED(hr))
		return hr;
	LPITEMIDLIST pidlOut = NULL;
	DWORD fetched;
	STRRET pName;
	WCHAR* strName;
	while (idList->Next(1, &pidlOut, &fetched) == S_OK)
	{
		GetDisplayNameOf(pidlOut, SHGDN_INFOLDER | SHGDN_FORPARSING, &pName);
		hr = StrRetToStrW(&pName, pidlOut, &strName);
		if (SUCCEEDED(hr))
		{
			int iCmpResult = StrCmpW(szNameComponent, strName);
			CoTaskMemFree(strName);
			if (iCmpResult == 0)
			{
				*ppidl = pidlOut;
				break;
			}
		}
		ILFree(pidlOut);
	}
	idList->Release();
	hr = *ppidl ? S_OK : E_FAIL;
	return hr;
}

//  Allows a client to determine the contents of a folder by
//  creating an item identifier enumeration object and returning
//  its IEnumIDList interface. The methods supported by that
//  interface can then be used to enumerate the folder's contents.
HRESULT CBaseFolderView::EnumObjects(HWND /* hwnd */, DWORD grfFlags, IEnumIDList **ppenumIDList)
{
	try
	{
		Log::Debug(L"CBaseFolderView::EnumObjects(HWND /* hwnd */, DWORD grfFlags, IEnumIDList **ppenumIDList)");
		HRESULT hr;
		CBaseFolderViewEnumIDList* penum = CreateEnumIDList(grfFlags);
		hr = penum ? S_OK : E_FAIL;
		if (SUCCEEDED(hr))
		{
			hr = penum->Initialize();
			if (SUCCEEDED(hr))
			{
				hr = penum->QueryInterface(IID_PPV_ARGS(ppenumIDList));
			}
			penum->Release();
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::EnumObjects(HWND /* hwnd */, DWORD grfFlags, IEnumIDList **ppenumIDList)");
		return E_FAIL;
	}
}


//  Factory for handlers for the specified item.
HRESULT CBaseFolderView::BindToObject(PCUIDLIST_RELATIVE pidl,
	IBindCtx *pbc, REFIID riid, void **ppv)
{
	try
	{
		Log::Debug(L"CBaseFolderView::BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		HRESULT hr;
		CBaseFolderView* pCFolderView = CreateChildFolderView();
		hr = pCFolderView ? S_OK : E_OUTOFMEMORY;
		if (SUCCEEDED(hr))
		{
			// Initialize it.
			PITEMID_CHILD pidlFirst = ILCloneFirst(pidl);
			hr = pidlFirst ? S_OK : E_OUTOFMEMORY;
			if (SUCCEEDED(hr))
			{
				PIDLIST_ABSOLUTE pidlBind = ILCombine(m_pidl, pidlFirst);
				hr = pidlBind ? S_OK : E_OUTOFMEMORY;
				if (SUCCEEDED(hr))
				{
					hr = pCFolderView->Initialize(pidlBind);
					if (SUCCEEDED(hr))
					{
						PCUIDLIST_RELATIVE pidlNext = ILNext(pidl);
						if (ILIsEmpty(pidlNext))
						{
							// If we're reached the end of the idlist, return the interfaces we support for this item.
							// Other potential handlers to return include IPropertyStore, IStream, IStorage, etc.
							hr = pCFolderView->QueryInterface(riid, ppv);
						}
						else
						{
							// Otherwise we delegate to our child folder to let it bind to the next level.
							hr = pCFolderView->BindToObject(pidlNext, pbc, riid, ppv);
						}
					}
					CoTaskMemFree(pidlBind);
				}
				ILFree(pidlFirst);
			}
			pCFolderView->Release();
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return E_FAIL;
	}
}

HRESULT CBaseFolderView::BindToStorage(PCUIDLIST_RELATIVE /* pidl */,
	IBindCtx* /* pbc */, REFIID /* riid */, void** /* ppv */)
{
	try
	{
		Log::Debug(L"CBaseFolderView::BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return E_NOTIMPL;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx * pbc, REFIID riid, void** ppv)");
		return E_FAIL;
	}
}

//  Helper function to help compare relative IDs.
HRESULT CBaseFolderView::ILCompareRelIDs(IShellFolder *psfParent, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2,
	LPARAM lParam)
{
	Log::Debug(L"CBaseFolderView::ILCompareRelIDs(IShellFolder *psfParent, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2, LPARAM lParam)");
	HRESULT hr;
	PCUIDLIST_RELATIVE pidlRel1 = ILNext(pidl1);
	PCUIDLIST_RELATIVE pidlRel2 = ILNext(pidl2);
	if (ILIsEmpty(pidlRel1))
	{
		if (ILIsEmpty(pidlRel2))
		{
			hr = ResultFromShort(0);  // Both empty
		}
		else
		{
			hr = ResultFromShort(-1);   // 1 is empty, 2 is not.
		}
	}
	else
	{
		if (ILIsEmpty(pidlRel2))
		{
			hr = ResultFromShort(1);  // 2 is empty, 1 is not
		}
		else
		{
			// pidlRel1 and pidlRel2 point to something, so:
			//  (1) Bind to the next level of the IShellFolder
			//  (2) Call its CompareIDs to let it compare the rest of IDs.
			PIDLIST_RELATIVE pidlNext = ILCloneFirst(pidl1);    // pidl2 would work as well
			hr = pidlNext ? S_OK : E_OUTOFMEMORY;
			if (pidlNext)
			{
				IShellFolder *psfNext;
				hr = psfParent->BindToObject(pidlNext, NULL, IID_PPV_ARGS(&psfNext));
				if (SUCCEEDED(hr))
				{
					// We do not want to pass the lParam is IShellFolder2 isn't supported.
					// Although it isn't important for this example it shoud be considered
					// if you are implementing this for other situations.
					IShellFolder2 *psf2;
					if (SUCCEEDED(psfNext->QueryInterface(&psf2)))
					{
						psf2->Release();  // We can use the lParam
					}
					else
					{
						lParam = 0;       // We can't use the lParam
					}

					// Also, the column mask will not be relevant and should never be passed.
					hr = psfNext->CompareIDs((lParam & ~SHCIDS_COLUMNMASK), pidlRel1, pidlRel2);
					psfNext->Release();
				}
				CoTaskMemFree(pidlNext);
			}
		}
	}
	return hr;
}

//  Called by the Shell to create the View Object and return it.
HRESULT CBaseFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)
{
	try
	{
		Log::Debug(L"CBaseFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)");
		HRESULT hr = E_NOINTERFACE;
		if (riid == IID_IShellView)
		{
			SFV_CREATE sfvc = { sizeof(sfvc), 0 };
			hr = QueryInterface(IID_PPV_ARGS(&sfvc.pshf));
			if (SUCCEEDED(hr))
			{
				// Add our callback to the SFV_CREATE.  This is optional.  We
				// are adding it so we can enable searching within our
				// namespace.
				hr = CreateFolderViewCB(IID_PPV_ARGS(&sfvc.psfvcb));
				if (SUCCEEDED(hr))
				{
					hr = SHCreateShellFolderView(&sfvc, (IShellView * *)ppv);
					sfvc.psfvcb->Release();
				}
				sfvc.pshf->Release();
			}
		}
		else if (riid == IID_IContextMenu)
		{
			DEFCONTEXTMENU const dcm = { hwnd, NULL, NULL, static_cast<IShellFolder2*>(this),
				0, NULL, NULL, 0, NULL };
			hr = SHCreateDefaultContextMenu(&dcm, riid, ppv);
		}
		else
		{
			hr = E_NOTIMPL;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::CreateViewObject(HWND hwnd, REFIID riid, void **ppv)");
		return E_FAIL;
	}
}

//  Retrieves the attributes of one or more file objects or subfolders.
HRESULT CBaseFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)");
		HRESULT hr;
		PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(*apidl);
		hr = pMyObj ? S_OK : E_INVALIDARG;
		if (FAILED(hr))
			return hr;
		if (pMyObj->fIsRealPath == TRUE)
		{
			*rgfInOut &= pMyObj->ulAttributes;
			for (unsigned int i = 0; i < cidl; i++)
			{
				pMyObj = CBaseFolderViewChildHelper::IsValid(apidl[i]);
				if (pMyObj != NULL) {
					*rgfInOut &= pMyObj->ulAttributes;
				}
			}
			return S_OK;
		}
		DWORD dwAttribs = 0;
		dwAttribs |= SFGAO_FOLDER | SFGAO_BROWSABLE | SFGAO_FILESYSANCESTOR;
		dwAttribs |= SFGAO_HASSUBFOLDER;
		*rgfInOut &= dwAttribs;
		return S_OK;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut)");
		return E_FAIL;
	}
}

//  Retrieves an OLE interface that can be used to carry out
//  actions on the specified file objects or folders.
HRESULT CBaseFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,
	REFIID riid, UINT * /* prgfInOut */, void **ppv)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,	REFIID riid, UINT * prgfInOut, void** ppv)");
		if (ILIsEmpty(*ppidl))
		{
			return E_INVALIDARG;
		}
		*ppv = NULL;
		HRESULT hr;
		PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(*ppidl);
		hr = pMyObj ? S_OK : E_INVALIDARG;
		if (FAILED(hr))
			return hr;
		if (riid == IID_IDataObject)
		{
			hr = SHCreateDataObject(m_pidl, cidl, ppidl, NULL, riid, ppv);
		}
		else if ((riid == IID_IContextMenu || riid == IID_IContextMenu2 || riid == IID_IContextMenu3))
		{
			DEFCONTEXTMENU const dcm = { hwnd, NULL, m_pidl, static_cast<IShellFolder2*>(this),
				cidl, ppidl, NULL, 0, NULL };
			hr = SHCreateDefaultContextMenu(&dcm, riid, ppv);
		}
		else if (riid == IID_IExtractIcon)
		{
			IDefaultExtractIconInit* pdxi;
			hr = SHCreateDefaultExtractIcon(IID_PPV_ARGS(&pdxi));
			if (SUCCEEDED(hr))
			{
				PWSTR pszIconPath;
				hr = CBaseFolderViewChildHelper::GetIconPath(*ppidl, &pszIconPath);
				if (SUCCEEDED(hr))
				{
					// This refers to icon indices in shell32.  You can also supply custom icons or
					// register IExtractImage to support general images.
					hr = pdxi->SetNormalIcon(pszIconPath, pMyObj->dwIconIndex);
					if (SUCCEEDED(hr))
					{
						hr = pdxi->QueryInterface(riid, ppv);
					}
					pdxi->Release();
					CoTaskMemFree(pszIconPath);
				}
			}
		}
		else if (riid == IID_IDataObject)
		{
			hr = SHCreateDataObject(m_pidl, cidl, ppidl, NULL, riid, ppv);
		}
		else
		{
			hr = E_NOINTERFACE;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl,	REFIID riid, UINT * prgfInOut, void** ppv)");
		return E_FAIL;
	}
}

//  Retrieves the display name for the specified file object or subfolder.
HRESULT CBaseFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)");
		HRESULT hr;
		PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(pidl);
		hr = pMyObj ? S_OK : E_INVALIDARG;
		if (FAILED(hr))
			return hr;
		if (pMyObj->fIsRealPath == TRUE)
		{
			if (shgdnFlags & SHGDN_FORPARSING)
			{
				IShellFolder* pParentFolder;
				hr = GetParentRealFolder(pidl, &pParentFolder);
				if (SUCCEEDED(hr))
				{
					PUIDLIST_RELATIVE pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(pidl);
					hr = pidlOriginal ? S_OK : E_FAIL;
					if (SUCCEEDED(hr))
					{
						hr = pParentFolder->GetDisplayNameOf(ILFindLastID(pidlOriginal), shgdnFlags, pName);
						ILFree(pidlOriginal);
					}
					pParentFolder->Release();
				}
				return hr;
			}
			else
			{
				PWSTR pszDisplayName;
				hr = CBaseFolderViewChildHelper::GetDisplayName(pidl, &pszDisplayName);
				if (SUCCEEDED(hr))
				{
					StringToStrRet(pszDisplayName, pName);
					CoTaskMemFree(pszDisplayName);
				}
			}
			return S_OK;
		}
		if (shgdnFlags & SHGDN_FORPARSING)
		{
			WCHAR szParseName[MAX_PATH];
			if (shgdnFlags & SHGDN_INFOLDER)
			{
				// This form of the display name needs to be handled by ParseDisplayName.
				hr = CBaseFolderViewChildHelper::GetParseName(pidl, szParseName, ARRAYSIZE(szParseName));
			}
			else
			{
				PWSTR pszThisFolder;
				hr = SHGetNameFromIDList(m_pidl, (shgdnFlags & SHGDN_FORADDRESSBAR) ? SIGDN_DESKTOPABSOLUTEEDITING : SIGDN_DESKTOPABSOLUTEPARSING, &pszThisFolder);
				if (SUCCEEDED(hr))
				{
					hr = StringCchCopy(szParseName, ARRAYSIZE(szParseName), pszThisFolder);
					CoTaskMemFree(pszThisFolder);
					if (FAILED(hr))
						return hr;
					hr = StringCchCat(szParseName, ARRAYSIZE(szParseName), L"\\");
					if (FAILED(hr))
						return hr;
					WCHAR strName[MAX_PATH];
					hr = CBaseFolderViewChildHelper::GetParseName(pidl, strName, ARRAYSIZE(strName));
					if (SUCCEEDED(hr))
					{
						hr = StringCchCat(szParseName, ARRAYSIZE(szParseName), strName);
					}
				}
			}
			if (SUCCEEDED(hr))
			{
				hr = StringToStrRet(szParseName, pName);
			}
		}
		else
		{
			PWSTR pszDisplayName;
			hr = CBaseFolderViewChildHelper::GetDisplayName(pidl, &pszDisplayName);
			if (SUCCEEDED(hr))
			{
				hr = StringToStrRet(pszDisplayName, pName);
				CoTaskMemFree(pszDisplayName);
			}
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName)");
		return E_FAIL;
	}
}


HRESULT CBaseFolderView::GetParentRealFolder(LPCITEMIDLIST pidl, IShellFolder** ppFolder)
{
	Log::Debug(L"CServerFolderView::GetParentRealFolder(LPCITEMIDLIST pidl, IShellFolder** ppFolder)");
	HRESULT hr;
	PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	hr = pMyObj->fIsRealPath == TRUE ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	IShellFolder* pDesktop;
	hr = SHGetDesktopFolder(&pDesktop);
	if (SUCCEEDED(hr))
	{
		LPITEMIDLIST pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(pidl);
		hr = pidlOriginal ? S_OK : E_OUTOFMEMORY;
		if (SUCCEEDED(hr))
		{
			if (ILRemoveLastID(pidlOriginal))
			{
				hr = pDesktop->BindToObject(pidlOriginal, NULL, IID_IShellFolder, reinterpret_cast<void**>(ppFolder));
			}
			ILFree(pidlOriginal);
		}
		pDesktop->Release();
	}
	return hr;
}


//  Sets the display name of a file object or subfolder, changing
//  the item identifier in the process.
HRESULT CBaseFolderView::SetNameOf(HWND /* hwnd */, PCUITEMID_CHILD /* pidl */,
	PCWSTR /* pszName */, DWORD /* uFlags */, PITEMID_CHILD* ppidlOut)
{
	try
	{
		Log::Debug(L"CBaseFolderView::SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut)");
		HRESULT hr = E_NOTIMPL;
		*ppidlOut = NULL;
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut)");
		return E_FAIL;
	}
}

//  IPersist method
HRESULT CBaseFolderView::GetClassID(CLSID *pClassID)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetClassID(CLSID *pClassID)");
		*pClassID = CLSID_FolderViewImpl;
		return S_OK;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetClassID(CLSID *pClassID)");
		return E_FAIL;
	}
}

//  IPersistFolder method
HRESULT CBaseFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)
{
	try
	{
		Log::Debug(L"CBaseFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		m_pidl = ILCloneFull(pidl);
		return m_pidl ? S_OK : E_OUTOFMEMORY;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::Initialize(PCIDLIST_ABSOLUTE pidl)");
		return E_FAIL;
	}
}

//  IShellFolder2 methods
HRESULT CBaseFolderView::EnumSearches(IEnumExtraSearch **ppEnum)
{
	try
	{
		Log::Debug(L"CBaseFolderView::EnumSearches(IEnumExtraSearch **ppEnum)");
		*ppEnum = NULL;
		return E_NOINTERFACE;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::EnumSearches(IEnumExtraSearch **ppEnum)");
		return E_FAIL;
	}
}

//  Retrieves the default sorting and display column (indices from GetDetailsOf).
HRESULT CBaseFolderView::GetDefaultColumn(DWORD /* dwRes */, ULONG *pSort, ULONG *pDisplay)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDefaultColumn(DWORD dwRes, ULONG *pSort, ULONG *pDisplay)");
		*pSort = 0;
		*pDisplay = 0;
		return S_OK;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDefaultColumn(DWORD dwRes, ULONG *pSort, ULONG *pDisplay)");
		return E_FAIL;
	}
}

//  Retrieves the default state for a specified column.
HRESULT CBaseFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)");
		HRESULT hr;
		switch (iColumn)
		{
		case 0:
			*pcsFlags = SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE_TYPE_STR;
			hr = S_OK;
			break;
		case 1:
			*pcsFlags = SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE_TYPE_STR;
			hr = S_OK;
			break;
		default:
			hr = E_INVALIDARG;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags)");
		return E_FAIL;
	}
}

//  Requests the GUID of the default search object for the folder.
HRESULT CBaseFolderView::GetDefaultSearchGUID(GUID * /* pguid */)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDefaultSearchGUID(GUID * /* pguid */)");
		return E_NOTIMPL;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDefaultSearchGUID(GUID * /* pguid */)");
		return E_FAIL;
	}
}

// IExplorerPaneVisibility methods
HRESULT CBaseFolderView::GetPaneState(
	REFEXPLORERPANE ep,
	EXPLORERPANESTATE* peps
)
{
	if (ep == EP_Ribbon)
	{
		*peps = EPS_DEFAULT_ON;
	}
	else
	{
		*peps = EPS_DONTCARE;
	}
	return S_OK;
}

HRESULT CBaseFolderView::CreateFolderViewCB(REFIID riid, void** ppv)
{
	Log::Debug(L"CBaseFolderView::CreateFolderViewCB(REFIID riid, void** ppv)");
	*ppv = NULL;
	HRESULT hr = E_OUTOFMEMORY;
	CBaseFolderViewCB* pfvcb = new (std::nothrow) CBaseFolderViewCB(m_pidl);
	if (pfvcb)
	{
		hr = pfvcb->QueryInterface(riid, ppv);
		pfvcb->Release();
	}
	return hr;
}

//  Helper function for getting the display name for a column.
//  IMPORTANT: If cch is set to 0 the value is returned in the VARIANT.
HRESULT CBaseFolderView::GetColumnDisplayName(PCUITEMID_CHILD pidl, const PROPERTYKEY* pkey, VARIANT* pv, WCHAR* pszRet, UINT cch)
{
	Log::Debug(L"CBaseFolderView::GetColumnDisplayName(PCUITEMID_CHILD pidl, const PROPERTYKEY* pkey, VARIANT* pv, WCHAR* pszRet, UINT cch)");
	HRESULT hr;
	if (IsEqualPropertyKey(*pkey, PKEY_ItemNameDisplay))
	{
		PWSTR pszName;
		hr = CBaseFolderViewChildHelper::GetParseName(pidl, &pszName);
		if (SUCCEEDED(hr))
		{
			if (pv != NULL)
			{
				pv->vt = VT_BSTR;
				pv->bstrVal = SysAllocString(pszName);
				hr = pv->bstrVal ? S_OK : E_OUTOFMEMORY;
			}
			else
			{
				hr = StringCchCopy(pszRet, cch, pszName);
			}
			CoTaskMemFree(pszName);
		}
	}
	else if (IsEqualPropertyKey(*pkey, pkey_cItemGroup))
	{
		PCFVITEMID pMyObj = CBaseFolderViewChildHelper::IsValid(pidl);
		hr = pMyObj ? S_OK : E_FAIL;
		if (SUCCEEDED(hr))
		{
			PWSTR pszGroupName;
			hr = CBaseFolderViewChildHelper::GetGroupName(pidl, &pszGroupName);
			if (SUCCEEDED(hr))
			{
				if (pv != NULL)
				{
					pv->vt = VT_BSTR;
					pv->bstrVal = SysAllocString(pszGroupName);
					hr = pv->bstrVal ? S_OK : E_OUTOFMEMORY;
				}
				else
				{
					hr = StringCchCopy(pszRet, cch, pszGroupName);
				}
				CoTaskMemFree(pszGroupName);
			}
		}
	}
	else
	{
		if (pv)
		{
			VariantInit(pv);
		}
		if (pszRet)
		{
			*pszRet = '\0';
		}
		hr = S_OK;
	}
	return hr;
}

//  Retrieves detailed information, identified by a
//  property set ID (FMTID) and property ID (PID),
//  on an item in a Shell folder.
HRESULT CBaseFolderView::GetDetailsEx(PCUITEMID_CHILD pidl,
	const PROPERTYKEY *pkey,
	VARIANT *pv)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY * pkey, VARIANT * pv)");
		BOOL fIsFolder = FALSE;
		HRESULT hr = CBaseFolderViewChildHelper::GetFolderness(pidl, &fIsFolder);
		if (SUCCEEDED(hr))
		{
			if (!fIsFolder && IsEqualPropertyKey(*pkey, PKEY_PropList_PreviewDetails))
			{
				// This proplist indicates what properties are shown in the details pane at the bottom of the explorer browser.
				pv->vt = VT_BSTR;
				pv->bstrVal = SysAllocString(L"prop:");
				hr = pv->bstrVal ? S_OK : E_OUTOFMEMORY;
			}
			else
			{
				hr = GetColumnDisplayName(pidl, pkey, pv, NULL, 0);
			}
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY * pkey, VARIANT * pv)");
		return E_FAIL;
	}
}

HRESULT CBaseFolderView::GetDetailsOf(__in IPropertyDescription *ppropdesc, __out SHELLDETAILS *psd)
{
	Log::Debug(L"CBaseFolderView::GetDetailsOf(__in IPropertyDescription *ppropdesc, __out SHELLDETAILS *psd)");
	PROPDESC_VIEW_FLAGS dvf;
	HRESULT hr = ppropdesc->GetViewFlags(&dvf);
	if (SUCCEEDED(hr))
	{
		int fmt = LVCFMT_LEFT; // default
		if (dvf & PDVF_RIGHTALIGN)
		{
			fmt = LVCFMT_RIGHT;
		}
		else if (dvf & PDVF_CENTERALIGN)
		{
			fmt = LVCFMT_CENTER;
		}
		static const struct PROPVIEWFLAGSTOLVCFMT
		{
			PROPDESC_VIEW_FLAGS dvf;
			int fmt;
		}
		s_mapFlags[] =
		{
			{ PDVF_BEGINNEWGROUP, LVCFMT_LINE_BREAK },
		{ PDVF_FILLAREA, LVCFMT_FILL },
		{ PDVF_HIDELABEL, LVCFMT_NO_TITLE },
		{ PDVF_CANWRAP, LVCFMT_WRAP },
		};
		for (int i = 0; i < ARRAYSIZE(s_mapFlags); i++)
		{
			if (dvf & s_mapFlags[i].dvf)
			{
				fmt |= s_mapFlags[i].fmt;
			}
		}
		psd->fmt = fmt;
		hr = ppropdesc->GetDefaultColumnWidth((UINT *)&psd->cxChar);
		if (SUCCEEDED(hr))
		{
			PROPDESC_TYPE_FLAGS dtf;
			hr = ppropdesc->GetTypeFlags(PDTF_ISVIEWABLE, &dtf);
			if (SUCCEEDED(hr))
			{
				WCHAR* spszDisplayName = NULL;
				hr = ppropdesc->GetDisplayName(&spszDisplayName);
				if (SUCCEEDED(hr))
				{
					psd->str.uType = STRRET_WSTR;
					psd->str.pOleStr = spszDisplayName;
				}
			}
		}
	}
	return hr;
}

//  Retrieves detailed information, identified by a
//  column index, on an item in a Shell folder.
HRESULT CBaseFolderView::GetDetailsOf(PCUITEMID_CHILD pidl,
	UINT iColumn,
	SHELLDETAILS *pDetails)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS * pDetails)");
		PROPERTYKEY key;
		HRESULT hr = MapColumnToSCID(iColumn, &key);
		WCHAR szRet[80];
		if (!pidl)
		{
			// No item means we're returning information about the column itself.
			HMODULE lib = LoadLibrary(L"shell32.dll");
			if (lib == 0)
				return E_FAIL;
			switch (iColumn)
			{
			case 0:
				LoadString(lib, IDS_SHELL32_NAME, szRet, ARRAYSIZE(szRet));
				pDetails->fmt = LVCFMT_LEFT;
				pDetails->cxChar = cx_cCHAR_NAME;
				break;
			case 1:
				LoadString(lib, IDS_SHELL32_TYPE, szRet, ARRAYSIZE(szRet));
				pDetails->fmt = LVCFMT_LEFT;
				pDetails->cxChar = cx_cCHAR_GROUP;
				break;
			default:
				// GetDetailsOf is called with increasing column indices until failure.
				hr = E_FAIL;
				break;
			}
			FreeLibrary(lib);
		}
		else if (SUCCEEDED(hr))
		{
			hr = GetColumnDisplayName(pidl, &key, NULL, szRet, ARRAYSIZE(szRet));
		}
		if (SUCCEEDED(hr))
		{
			hr = StringToStrRet(szRet, &pDetails->str);
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS * pDetails)");
		return E_FAIL;
	}
}

//  Converts a column name to the appropriate
//  property set ID (FMTID) and property ID (PID).
HRESULT CBaseFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)
{
	try
	{
		Log::Debug(L"CBaseFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)");
		// The property keys returned here are used by the categorizer.
		HRESULT hr;
		switch (iColumn)
		{
		case 0:
			*pkey = PKEY_ItemNameDisplay;
			hr = S_OK;
			break;
		case 1:
			*pkey = pkey_cItemGroup;
			hr = S_OK;
			break;
		default:
			hr = E_FAIL;
			break;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey)");
		return E_FAIL;
	}
}

//IPersistFolder2 methods
//  Retrieves the PIDLIST_ABSOLUTE for the folder object.
HRESULT CBaseFolderView::GetCurFolder(PIDLIST_ABSOLUTE *ppidl)
{
	try
	{
		Log::Debug(L"CBaseFolderView::GetCurFolder(PIDLIST_ABSOLUTE *ppidl)");
		*ppidl = NULL;
		HRESULT hr = m_pidl ? S_OK : E_FAIL;
		if (SUCCEEDED(hr))
		{
			*ppidl = ILCloneFull(m_pidl);
			hr = *ppidl ? S_OK : E_OUTOFMEMORY;
		}
		return hr;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderView::GetCurFolder(PIDLIST_ABSOLUTE *ppidl)");
		return E_FAIL;
	}
}

CBaseFolderViewEnumIDList* CBaseFolderView::CreateEnumIDList(DWORD /* grfFlags */)
{
	Log::Debug(L"CBaseFolderView::CreateEnumIDList(DWORD grfFlags)");
	return NULL; // Do not force implementation
}