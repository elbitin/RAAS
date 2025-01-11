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
#include <shlwapi.h>
#include <new>  // std::nothrow
#include "Log.h"
#include "RAASClientProgramHelper.h"
#include "Dll.h"
#include "ComUtils.h"
#include "resource.h"
#include "ContextMenuUtils.h"
#include "ServerContextMenu.h"

#define MENUVERB_REMOTEDESKTOP				0
#define MENUVERB_RAASSERVERCONFIGURATION    1

const ICIVERBTOIDMAP a_cSERVER_CONTEXT_MENU_ID_MAP[] =
{
	{ L"Remote Desktop",    "Remote Desktop",   MENUVERB_REMOTEDESKTOP, 0, },
    { L"RAAS Server Configuration",    "RAAS Server Configuration",   MENUVERB_RAASSERVERCONFIGURATION, 0, },
    { NULL,          NULL,        (UINT)-1,         0, }
};

CServerContextMenu::CServerContextMenu() : m_cRef(1), m_punkSite(NULL), m_pDataObj(NULL)
{
	Log::Debug(L"CServerContextMenu::CServerContextMenu() : m_cRef(1), m_punkSite(NULL), m_pDataObj(NULL)");
    DllAddRef();
}

CServerContextMenu::~CServerContextMenu()
{
	Log::Debug(L"CServerContextMenu::~CServerContextMenu()");
	SAFE_RELEASE(m_pDataObj);
	DllRelease();
}

// IUnknown
IFACEMETHODIMP CServerContextMenu::QueryInterface(REFIID riid, void **ppv)
{
	Log::Debug(L"CServerContextMenu::QueryInterface(REFIID riid, void **ppv)");
    static const QITAB qit[] = {
        QITABENT(CServerContextMenu, IContextMenu),
        QITABENT(CServerContextMenu, IShellExtInit),
        QITABENT(CServerContextMenu, IObjectWithSite),
        { 0 },
    };
    return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CServerContextMenu::AddRef()
{
	Log::Debug(L"CServerContextMenu::AddRef()");
    return InterlockedIncrement(&m_cRef);
}

IFACEMETHODIMP_(ULONG) CServerContextMenu::Release()
{
	Log::Debug(L"CServerContextMenu::Release()");
    long cRef = InterlockedDecrement(&m_cRef);
    if (!cRef)
    {
        delete this;
    }
    return cRef;
}

HRESULT CServerContextMenu_CreateInstance(REFIID riid, void **ppv)
{
	Log::Debug(L"CServerContextMenu_CreateInstance(REFIID riid, void **ppv)");
    *ppv = NULL;
    CServerContextMenu* pServerContextMenu = new (std::nothrow) CServerContextMenu();
    HRESULT hr = pServerContextMenu ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = pServerContextMenu->QueryInterface(riid, ppv);
        pServerContextMenu->Release();
    }
    return hr;
}

HRESULT CServerContextMenu::QueryContextMenu(HMENU hmenu, UINT indexMenu, UINT idCmdFirst, UINT /* idCmdLast */, UINT /* uFlags */)
{
    HDC screen = GetDC(NULL);
    int hSize = GetDeviceCaps(screen, HORZSIZE);
    int hRes = GetDeviceCaps(screen, HORZRES);
    int scale = (int)((float(hRes) * 25.4 / hSize) + 64) / 128;
	Log::Debug(L"CServerContextMenu::QueryContextMenu(HMENU hmenu, UINT indexMenu, UINT idCmdFirst, UINT /* idCmdLast */, UINT /* uFlags */)");
	WCHAR szRemoteDesktopMenuItem[80];
	LoadString(g_hInst, IDS_REMOTEDESKTOP, szRemoteDesktopMenuItem, ARRAYSIZE(szRemoteDesktopMenuItem));
    UINT nCmdID = idCmdFirst + MENUVERB_REMOTEDESKTOP;
    MENUITEMINFO mii;
    ZeroMemory(&mii, sizeof(MENUITEMINFO));
    mii.cbSize = sizeof(MENUITEMINFO);
    mii.fMask = MIIM_FTYPE | MIIM_STRING | MIIM_ID | MIIM_BITMAP;
    mii.wID = nCmdID;
    mii.dwTypeData = szRemoteDesktopMenuItem;
    mii.hbmpItem = (HBITMAP)LoadImage(g_hInst, MAKEINTRESOURCE(IDB_RDESKTOP), IMAGE_BITMAP, 16 * scale, 16 * scale, LR_DEFAULTSIZE | LR_LOADTRANSPARENT);
    if (!InsertMenuItem(hmenu, indexMenu++, TRUE, &mii))
        return E_FAIL;
	WCHAR szServerConfigMenuItem[80];
	LoadString(g_hInst, IDS_SERVERCONFIG, szServerConfigMenuItem, ARRAYSIZE(szServerConfigMenuItem));
    nCmdID = idCmdFirst + MENUVERB_RAASSERVERCONFIGURATION;
    ZeroMemory(&mii, sizeof(MENUITEMINFO));
    mii.cbSize = sizeof(MENUITEMINFO);
    mii.fMask = MIIM_FTYPE | MIIM_STRING | MIIM_ID | MIIM_BITMAP;
    mii.wID = nCmdID;
    mii.dwTypeData = szServerConfigMenuItem;
    mii.hbmpItem = (HBITMAP)LoadImage(g_hInst, MAKEINTRESOURCE(IDB_SERVERCONFIG), IMAGE_BITMAP, 16 * scale, 16 * scale, LR_DEFAULTSIZE | LR_LOADTRANSPARENT);
    if (!InsertMenuItem(hmenu, indexMenu++, TRUE, &mii))
        return E_FAIL;

    // other verbs could go here...

    // indicate that we added two verbs.
    return MAKE_HRESULT(SEVERITY_SUCCESS, 0, (USHORT)(2));
}

HRESULT CServerContextMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
	Log::Debug(L"CServerContextMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pici)");
    HRESULT hr = E_INVALIDARG;
    UINT uID;

    // Is this command for us?
    if (SUCCEEDED(MapICIVerbToCmdIDServer(pici, a_cSERVER_CONTEXT_MENU_ID_MAP, &uID)) && uID == MENUVERB_RAASSERVERCONFIGURATION && m_pDataObj)
    {
        IShellItemArray *psia;
        hr = SHCreateShellItemArrayFromDataObject(m_pDataObj, IID_PPV_ARGS(&psia));
        if (SUCCEEDED(hr))
        {
            hr = CRAASClientProgramHelper::ConfigureServer(psia);
            psia->Release();
        }
    }
	if (SUCCEEDED(MapICIVerbToCmdIDServer(pici, a_cSERVER_CONTEXT_MENU_ID_MAP, &uID)) && uID == MENUVERB_REMOTEDESKTOP && m_pDataObj)
	{
		IShellItemArray *psia;
		hr = SHCreateShellItemArrayFromDataObject(m_pDataObj, IID_PPV_ARGS(&psia));
		if (SUCCEEDED(hr))
		{
			hr = CRAASClientProgramHelper::RemoteDesktop(psia);
			psia->Release();
		}
	}
    return hr;
}

HRESULT CServerContextMenu::GetCommandString(UINT_PTR /* idCmd */, UINT /* uType */, UINT * /* pRes */, LPSTR /* pszName */, UINT /* cchMax */)
{
	Log::Debug(L"CServerContextMenu::GetCommandString(UINT_PTR /* idCmd */, UINT /* uType */, UINT * /* pRes */, LPSTR /* pszName */, UINT /* cchMax */)");
    return E_NOTIMPL;
}

HRESULT CServerContextMenu::Initialize(PCIDLIST_ABSOLUTE /* pidlFolder */, IDataObject *pDataObj, HKEY /* hkeyProgID */)
{
	Log::Debug(L"CServerContextMenu::Initialize(PCIDLIST_ABSOLUTE /* pidlFolder */, IDataObject *pDataObj, HKEY /* hkeyProgID */)");
    if (m_pDataObj)
    {
        m_pDataObj->Release();
        m_pDataObj = NULL;
    }
    m_pDataObj = pDataObj;
    if (pDataObj)
    {
        pDataObj->AddRef();
    }
    return S_OK;
}

HRESULT CServerContextMenu::SetSite(IUnknown *punkSite)
{
	Log::Debug(L"CServerContextMenu::SetSite(IUnknown *punkSite)");
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

HRESULT CServerContextMenu::GetSite(REFIID riid, void **ppvSite)
{
	Log::Debug(L"CServerContextMenu::GetSite(REFIID riid, void **ppvSite)");
    return m_punkSite ? m_punkSite->QueryInterface(riid, ppvSite) : E_FAIL;
}
