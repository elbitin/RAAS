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
// Copyright (c) Elbitin

#include "stdafx.h"
#include <windows.h>
#include <wingdi.h>
#include <tchar.h>
#include <wchar.h>
#include <shlobj.h>
#include <strsafe.h>
#include <Strsafe.h>
#include <objbase.h>
#include <comutil.h>
#include <string>
#include <Shellapi.h>
#include <Winnetwk.h>
#include <Shlwapi.h>
#include <wingdi.h>
#include "ShellExt.h"
#include "Utils.h"
#include "resource.h"

extern HINSTANCE  g_hInst;

#define IDM_OPEN_REMOTE                 0
#define IDM_REMOTE_SHORTCUT             1 

using namespace std;

CShellExt::CShellExt()
{
	m_cRef = 1;
}

CShellExt::~CShellExt()
{

}

STDMETHODIMP_(DWORD) CShellExt::Release(void)
{
	if(--m_cRef == 0)
	{
		delete this;
		return 0;
	}
	return m_cRef;
}

STDMETHODIMP_(DWORD) CShellExt::AddRef(void)
{
	return ++m_cRef;
}

BOOL CShellExt::_GetServerNameFromPath(wstring szPath, wchar_t* buffer, int bufSize)
{
	if (szPath.substr(0, 2).compare(L"\\\\") != 0)
		return FALSE;
	else
	{
		wcscpy_s(buffer, (rsize_t)bufSize, (const wchar_t*)(szPath.substr(2, szPath.find(std::wstring(L"\\"), 2) - 2).c_str()));
		return TRUE;
	}
}

HRESULT CShellExt::_GetRAASClientInstallDir(wchar_t* szDest, DWORD cchDest)
{
	HRESULT hr;
	HKEY hKey;
	LONG lRes = RegOpenKeyExW(HKEY_LOCAL_MACHINE, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
	bool bSuccess(lRes == ERROR_SUCCESS);
	if (!bSuccess)
	{
		lRes = RegOpenKeyExW(HKEY_CURRENT_USER, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
		bool bSuccess(lRes == ERROR_SUCCESS);
		if (!bSuccess)
			return E_FAIL;
	}
	ULONG nError;
	nError = RegQueryValueExW(hKey, sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR, 0, NULL, (LPBYTE)szDest, &cchDest);
	hr = (ERROR_SUCCESS == nError ? S_OK : E_FAIL);
	return hr;
}

BOOL CShellExt::_ServerExists(wchar_t* szServerName, wchar_t* szServersFile)
{
	BOOL fExists = FALSE;
	HRESULT hr = S_OK;
	IXMLDOMDocument *pXMLDom = NULL;
	IXMLDOMNodeList *pNodes = NULL;
	IXMLDOMNodeList *pSubNodes = NULL;
	IXMLDOMNode *pNode = NULL;
	IXMLDOMNode *pNodeText = NULL;
	IXMLDOMNode *pNodeEnabled = NULL;
	IXMLDOMNode *pServerNode = NULL;
	IXMLDOMNode *pSubNode = NULL;
	BSTR bstrQuery1 = NULL;
	BSTR bstrQuery2 = NULL;
	BSTR bstrNodeName = NULL;
	BSTR bstrNodeValue = NULL;
	VARIANT_BOOL varStatus;
	VARIANT varFileName;
	VARIANT varNodeValue;
	VariantInit(&varFileName);
	VariantInit(&varNodeValue);
	CHK_HR(CreateAndInitDOM(&pXMLDom));
	CHK_HR(VariantFromString(_bstr_t(szServersFile), varFileName));
	CHK_HR(pXMLDom->load(varFileName, &varStatus));
	if (varStatus != VARIANT_TRUE)
	{
		CHK_HR(ReportParseError(pXMLDom, "Failed to load DOM from servers.xml."));
	}
	bstrQuery1 = SysAllocString(_bstr_t(sz_cSERVER_NAME_QUERY));
	CHK_ALLOC(bstrQuery1);
	CHK_HR(pXMLDom->selectNodes(bstrQuery1, &pNodes));
	long length;
	if (pNodes)
	{
		CHK_HR(pNodes->get_length(&length));
		for (long i = 0; i < length; i++)
		{
			CHK_HR(pNodes->get_item(i, &pNode));
			CHK_HR(pNode->get_nodeName(&bstrNodeName));
			pNode->get_parentNode(&pServerNode);
			pNode->get_firstChild(&pNodeText);
			CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
			wchar_t nodeValue[MAX_PATH];
			wcscpy_s(nodeValue, wcslen(_bstr_t(varNodeValue)) + 1, _bstr_t(varNodeValue));
			_wcslwr_s(nodeValue, MAX_PATH);
			wchar_t serverNameCopy[MAX_PATH];
			wcscpy_s(serverNameCopy, wcslen(szServerName) + 1, szServerName);
			_wcslwr_s(serverNameCopy, MAX_PATH);
			if (pNodeText)
				if (_wcsicmp(_bstr_t(varNodeValue), szServerName) == 0)
				{
					pServerNode->selectSingleNode(_bstr_t(sz_cSERVER_ENABLED), &pNodeEnabled);
					if (pNodeEnabled)
					{
						VariantClear(&varNodeValue);
						pNodeEnabled->get_firstChild(&pNodeText);
						if (pNodeText)
						{
							CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
							if (_wcsicmp(_bstr_t(varNodeValue), L"1") == 0)
								fExists = TRUE;
							VariantClear(&varNodeValue);
						}
					}
				}
		}
	}
CleanUp:
	SAFE_RELEASE(pXMLDom);
	SAFE_RELEASE(pNodes);
	SAFE_RELEASE(pNode);
	SAFE_RELEASE(pNodeText);
	SAFE_RELEASE(pServerNode);
	SysFreeString(bstrQuery1);
	SysFreeString(bstrQuery2);
	SysFreeString(bstrNodeName);
	SysFreeString(bstrNodeValue);
	VariantClear(&varFileName);
	VariantClear(&varNodeValue);
	return fExists;
}

BOOL CShellExt::_ShortcutsInstalled()
{
	HKEY hKey;
	LONG lRes = RegOpenKeyExW(HKEY_LOCAL_MACHINE, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
	bool bSuccess(lRes == ERROR_SUCCESS);
	if (!bSuccess)
	{
		lRes = RegOpenKeyExW(HKEY_CURRENT_USER, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
		bool bSuccess(lRes == ERROR_SUCCESS);
		if (!bSuccess)
			return FALSE;
	}
	wchar_t* installDir = NULL;
	WCHAR szBuffer[MAX_PATH];
	DWORD dwBufferSize = sizeof(szBuffer);
	ULONG nError;
	nError = RegQueryValueExW(hKey, sz_cRAAS_CLIENT_REGISTRY_SHORTCUTS, 0, NULL, (LPBYTE)szBuffer, &dwBufferSize);
	if (ERROR_SUCCESS != nError)
	{
		LONG lRes = RegOpenKeyExW(HKEY_LOCAL_MACHINE, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
		bool bExistsAndSuccess(lRes == ERROR_SUCCESS);
		bool bDoesNotExistsSpecifically(lRes == ERROR_FILE_NOT_FOUND);
		ULONG nError;
		nError = RegQueryValueExW(hKey, sz_cRAAS_CLIENT_REGISTRY_SHORTCUTS, 0, NULL, (LPBYTE)szBuffer, &dwBufferSize);
		if (ERROR_SUCCESS != nError)
		{
			return FALSE;
		}
	}
	return ((wcscmp((wchar_t*)szBuffer, sz_cRAAS_CLIENT_REGISTRY_INSTALLED_VALUE) == 0) ? TRUE : FALSE);
}

void CShellExt::_CreateRemoteShortcut()
{
	wchar_t szPath[MAX_PATH];
	wchar_t szRemoteScPath[MAX_PATH];
	wchar_t* szInstallDir = new wchar_t[MAX_PATH];
	HRESULT hr = _GetRAASClientInstallDir(szInstallDir, MAX_PATH);
	if (SUCCEEDED(hr))
	{
		if (SUCCEEDED(StringCchPrintf(szPath, ARRAYSIZE(szPath),
			L"\"%s\"", this->m_szFile)) &&
			SUCCEEDED(StringCchPrintf(szRemoteScPath, ARRAYSIZE(szRemoteScPath),
				L"\"%s%s\"", szInstallDir, sz_cREMOTE_SHORTCUT_PROGRAM_NAME)))
		{
			ShellExecute(NULL, L"open", szRemoteScPath, szPath, NULL, SW_SHOW);
		}
		delete[] szInstallDir;
	}
}

void CShellExt::_OpenRemote()
{
	wchar_t szPath[MAX_PATH];
	wchar_t szRemoteScPath[MAX_PATH];
	wchar_t* szInstallDir = new wchar_t[MAX_PATH];
	HRESULT hr = _GetRAASClientInstallDir(szInstallDir, MAX_PATH);
	if (SUCCEEDED(hr))
	{
		if (SUCCEEDED(StringCchPrintf(szPath, ARRAYSIZE(szPath),
			L"\"%s\"", this->m_szFile)) &&
			SUCCEEDED(StringCchPrintf(szRemoteScPath, ARRAYSIZE(szRemoteScPath),
				L"\"%s%s\"", szInstallDir, sz_cOPEN_REMOTE_PROGRAM_NAME)))
		{
			ShellExecute(NULL, L"open", szRemoteScPath, szPath, NULL, SW_SHOW);
		}
		delete[] szInstallDir;
	}
}

STDMETHODIMP CShellExt::QueryInterface(REFIID riid, LPVOID *ppReturn )
{
	*ppReturn = NULL;
	if( IsEqualIID(riid, IID_IUnknown) )
		*ppReturn = this;
	else if( IsEqualIID(riid, IID_IClassFactory) )
		*ppReturn = (IClassFactory*)this;
	else if ( IsEqualIID(riid, IID_IShellExtInit))
		*ppReturn = (IShellExtInit*)this;
	else if ( IsEqualIID(riid, IID_IContextMenu))
		*ppReturn = (IContextMenu*)this;
	if( *ppReturn )
    {
        LPUNKNOWN pUnk = (LPUNKNOWN)(*ppReturn);
		pUnk->AddRef();
		return S_OK;
    }
    return E_NOINTERFACE;
}

STDMETHODIMP CShellExt::Initialize (
	LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hkeyProgID )
{
	FORMATETC fmt = { CF_HDROP, NULL, DVASPECT_CONTENT, -1, TYMED_HGLOBAL };
	STGMEDIUM stg = { TYMED_HGLOBAL };
	HDROP     hDrop;
	if ( FAILED( pDataObj->GetData ( &fmt, &stg ) ))
	{
		return E_INVALIDARG;
	}
	hDrop = (HDROP) GlobalLock ( stg.hGlobal );
	if ( NULL == hDrop )
		return E_INVALIDARG;
	UINT uNumFiles = DragQueryFile ( hDrop, 0xFFFFFFFF, NULL, 0 );
	HRESULT hr = S_OK;
	if ( 0 == uNumFiles )
	{
		GlobalUnlock ( stg.hGlobal );
		ReleaseStgMedium ( &stg );
		return E_INVALIDARG;
	}
	if ( 0 == DragQueryFile ( hDrop, 0, m_szFile, MAX_PATH ) )
		hr = E_INVALIDARG;
	GlobalUnlock ( stg.hGlobal );
	ReleaseStgMedium ( &stg );
	return hr;
}

STDMETHODIMP CShellExt::QueryContextMenu (
	HMENU hmenu, UINT indexMenu, UINT idCmdFirst,
	UINT idCmdLast, UINT uFlags )
{
	if (uFlags & CMF_DEFAULTONLY || uFlags &  CMF_VERBSONLY)
		return MAKE_HRESULT ( SEVERITY_SUCCESS, FACILITY_NULL, 0 );
	wchar_t szAppDataPath[MAX_PATH];
	wchar_t szAppDataRAASServersXmlPath[MAX_PATH];
	wchar_t szServerName[MAX_PATH];
	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
		SUCCEEDED(StringCchPrintf(szAppDataRAASServersXmlPath, ARRAYSIZE(szAppDataRAASServersXmlPath),
		L"%s\\%s\\%s", szAppDataPath, sz_cRAAS_CLIENT_APPDATA_SUB_DIR, sz_cSERVERS_XML_FILE_NAME)))
	{
		if(!_GetServerNameFromPath(std::wstring(this->m_szFile), szServerName, MAX_PATH))
			return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
		if (!_ServerExists(szServerName, szAppDataRAASServersXmlPath))
			return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	}
	else
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	if (PathIsDirectory(this->m_szFile))
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	int lastCmd = 0;
	if (!Win11orLater()) {
		WCHAR szOpenRemoteMenuItem[80];
		LoadString(g_hInst, IDS_OPENREMOTE, szOpenRemoteMenuItem, ARRAYSIZE(szOpenRemoteMenuItem));
		UINT nCmdID = idCmdFirst + IDM_OPEN_REMOTE;
		MENUITEMINFO mii;
		ZeroMemory(&mii, sizeof(MENUITEMINFO));
		mii.cbSize = sizeof(MENUITEMINFO);
		mii.fMask = MIIM_FTYPE | MIIM_STRING | MIIM_ID | MIIM_BITMAP;
		mii.wID = nCmdID;
		mii.dwTypeData = szOpenRemoteMenuItem;
		HDC screen = GetDC(NULL);
		int hSize = GetDeviceCaps(screen, HORZSIZE);
		int hRes = GetDeviceCaps(screen, HORZRES);
		int scale = (int)((float(hRes) * 25.4 / hSize) + 64) / 128;
		mii.hbmpItem = (HBITMAP)LoadImage(g_hInst, MAKEINTRESOURCE(IDB_RAASCLIENT), IMAGE_BITMAP, 16 * scale, 16 * scale, LR_DEFAULTSIZE | LR_LOADTRANSPARENT);
		if (!InsertMenuItem(hmenu, indexMenu++, TRUE, &mii))
			return E_FAIL;
		lastCmd++;
	}
	if (_ShortcutsInstalled())
	{
		WCHAR szCreateRemoteShortcutMenuItem[80];
		LoadString(g_hInst, IDS_REMOTESHORTCUT, szCreateRemoteShortcutMenuItem, ARRAYSIZE(szCreateRemoteShortcutMenuItem));
		InsertMenu(hmenu, indexMenu++, MF_BYPOSITION, idCmdFirst + IDM_REMOTE_SHORTCUT, szCreateRemoteShortcutMenuItem);
		lastCmd++;
	}
	return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, lastCmd + 1);
}

STDMETHODIMP CShellExt::GetCommandString (
	UINT_PTR idCmd, UINT uFlags, UINT* pwRes, LPSTR pszName, UINT cchMax )
{
	if ( 0 != idCmd )
		return E_INVALIDARG;
	if ( uFlags & GCS_HELPTEXT )
	{
		wchar_t* szText = _T("RAAS context menu commands");
		return StringCchCopy(reinterpret_cast<PWSTR>(pszName), cchMax, szText);
	}
	return E_INVALIDARG;
}

STDMETHODIMP CShellExt::InvokeCommand ( LPCMINVOKECOMMANDINFO pCmdInfo )
{
	switch ( LOWORD( pCmdInfo->lpVerb) )
	{
	case IDM_OPEN_REMOTE:
		_OpenRemote();
		return S_OK;
		break;
	case IDM_REMOTE_SHORTCUT:
		_CreateRemoteShortcut();
		return S_OK;
		break;
	default:
		return E_INVALIDARG;
		break;
	}
}