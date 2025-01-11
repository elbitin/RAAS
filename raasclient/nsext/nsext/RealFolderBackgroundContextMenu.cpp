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

	This file includes code from Microsofts ExplorerDataProvider but have
	been modified by Elbitin, modifications are copyrighted by Elbitin but
	included is copyright notices from ExplorerDataProvider project.

	ExplorerDataProvider source code can be found at:
	https://github.com/Microsoft/Windows-classic-samples

	*/

/**************************************************************************
    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
   ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
   PARTICULAR PURPOSE.

   (c) Microsoft Corporation. All Rights Reserved.
**************************************************************************/

#include <windows.h>
#include <shlobj.h>
#include "resource.h"
#include "ComUtils.h"
#include "Dll.h"
#include "ContextMenuUtils.h"
#include "RealFolderBackgroundContextMenu.h"
#include <new>  // std::nothrow
#include "Log.h"
#include <strsafe.h>
#define MENUVERB_PROPERTIES     0
LPCWSTR pszPropertiesVerb = L"Properties";
LPCSTR pszPropertiesVerbA = "Properties";

const ICIVERBTOIDMAP a_cREAL_FOLDER_BACKGROUND_CONTEXT_MENU_ID_MAP[] =
{
    { pszPropertiesVerb,    pszPropertiesVerbA,   MENUVERB_PROPERTIES, 0, },
    { NULL,          NULL,        (UINT)-1,         0, }
};

CRealFolderBackgroundContextMenu::CRealFolderBackgroundContextMenu() : m_cRef(1), m_punkSite(NULL), m_pDataObj(NULL), m_pidl(NULL)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::CRealFolderBackgroundContextMenu() : m_cRef(1), m_punkSite(NULL), m_pDataObj(NULL), m_pidl(NULL)");
    DllAddRef();
}


CRealFolderBackgroundContextMenu::~CRealFolderBackgroundContextMenu()
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::~CRealFolderBackgroundContextMenu()");
	// _punkSite should be NULL due to SetSite(NULL).
	SAFE_RELEASE(m_pDataObj);
	ILFree((LPITEMIDLIST)m_pidl);
	DllRelease();
}

// IUnknown
IFACEMETHODIMP CRealFolderBackgroundContextMenu::QueryInterface(REFIID riid, void **ppv)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::QueryInterface(REFIID riid, void **ppv)");
    static const QITAB qit[] = {
        QITABENT(CRealFolderBackgroundContextMenu, IContextMenu),
        QITABENT(CRealFolderBackgroundContextMenu, IShellExtInit),
        QITABENT(CRealFolderBackgroundContextMenu, IObjectWithSite),
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CRealFolderBackgroundContextMenu::AddRef()
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::AddRef()");
    return InterlockedIncrement(&m_cRef);
}

IFACEMETHODIMP_(ULONG) CRealFolderBackgroundContextMenu::Release()
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::Release()");
    long cRef = InterlockedDecrement(&m_cRef);
    if (!cRef)
    {
        delete this;
    }
    return cRef;
}

HRESULT CRealFolderBackgroundContextMenu_CreateInstance(REFIID riid, void **ppv)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu_CreateInstance(REFIID riid, void **ppv)");
    *ppv = NULL;
    CRealFolderBackgroundContextMenu* pServerContextMenu = new (std::nothrow) CRealFolderBackgroundContextMenu();
    HRESULT hr = pServerContextMenu ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = pServerContextMenu->QueryInterface(riid, ppv);
        pServerContextMenu->Release();
    }
    return hr;
}

HRESULT CRealFolderBackgroundContextMenu::QueryContextMenu(HMENU hmenu, UINT /* indexMenu */, UINT idCmdFirst, UINT idCmdLast, UINT /* uFlags */)
{
    WCHAR szProperties[MAX_PATH];
	HMODULE lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	LoadString(lib, IDS_SHELL32_PROPERTIES, szProperties, MAX_PATH);
	FreeLibrary(lib);
	InsertMenu(hmenu, idCmdLast, MF_BYPOSITION, idCmdFirst + MENUVERB_PROPERTIES, szProperties);

    // other verbs could go here...

    // indicate that we added one verb.
    return MAKE_HRESULT(SEVERITY_SUCCESS, 0, (USHORT)(1));
}



HRESULT CRealFolderBackgroundContextMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pici)");
    HRESULT hr = E_INVALIDARG;
    UINT uID;

    // Is this command for us?
	if (SUCCEEDED(MapICIVerbToCmdIDServer(pici, a_cREAL_FOLDER_BACKGROUND_CONTEXT_MENU_ID_MAP, &uID)) && uID == MENUVERB_PROPERTIES)
    {
		IShellFolder* pDesktop;
		hr = SHGetDesktopFolder(&pDesktop);
		if (SUCCEEDED(hr))
		{
			IDataObject* dataObject;
			pDesktop->GetUIObjectOf(NULL, 1, &m_pidl, IID_IDataObject, NULL, reinterpret_cast<void**>(&dataObject));
			SHMultiFileProperties(dataObject, 0);
			pDesktop->Release();
		}
    }
    return hr;
}

HRESULT CRealFolderBackgroundContextMenu::GetCommandString(UINT_PTR /* idCmd */, UINT /* uType */, UINT * /* pRes */, LPSTR pszName, UINT cchName)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::GetCommandString(UINT_PTR idCmd, UINT uType, UINT * pRes, LPSTR pszName, UINT cchName)");
	return StringCchCopyW(reinterpret_cast<PWSTR>(pszName),
		cchName,
		L"Properties");
}

HRESULT CRealFolderBackgroundContextMenu::Initialize(PCIDLIST_ABSOLUTE pidlFolder, IDataObject *pDataObj, HKEY /* hkeyProgID */)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::Initialize(PCIDLIST_ABSOLUTE pidlFolder, IDataObject *pDataObj, HKEY /* hkeyProgID */)");
    if (m_pDataObj)
    {
        m_pDataObj->Release();
        m_pDataObj = NULL;
    }
	m_pidl = ILClone(pidlFolder);
    m_pDataObj = pDataObj;
    if (pDataObj)
    {
        pDataObj->AddRef();
    }
    return S_OK;
}

HRESULT CRealFolderBackgroundContextMenu::SetSite(IUnknown *punkSite)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::SetSite(IUnknown *punkSite)");
    if (m_punkSite)
    {
        m_punkSite->Release();
        m_punkSite = NULL;
    }
    m_punkSite = punkSite;
    if (punkSite)
    {
        punkSite->AddRef();
    }
    return S_OK;
}

HRESULT CRealFolderBackgroundContextMenu::GetSite(REFIID riid, void **ppvSite)
{
	Log::Debug(L"CRealFolderBackgroundContextMenu::GetSite(REFIID riid, void **ppvSite)");
    return m_punkSite ? m_punkSite->QueryInterface(riid, ppvSite) : E_FAIL;
}
