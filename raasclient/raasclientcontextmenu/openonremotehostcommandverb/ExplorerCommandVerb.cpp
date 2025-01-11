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
// Copyright (c) modifications by Elbitin

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

// ExplorerCommand handlers are an inproc verb implementation method that can provide
// dynamic behavior including computing the name of the command, its icon and its visibility state.
// only use this verb implemetnation method if you are implementing a command handler on
// the commands module and need the same functionality on a context menu.
//
// each ExplorerCommand handler needs to have a unique COM object, run uuidgen to
// create new CLSID values for your handler. a handler can implement multiple
// different verbs using the information provided via IInitializeCommand (the verb name).
// your code can switch off those different verb names or the properties provided
// in the property bag

#include "Dll.h"
#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include <wchar.h>
#include <shlobj.h>
#include <strsafe.h>
#include <Strsafe.h>
#include <objbase.h>
#include <comdef.h>
#include "Utils.h"
#include <string>
#include <vector>
#include "resource.h"
#include "ExplorerCommandVerb.h"

extern HINSTANCE  g_hInst;

const wchar_t* sz_cVERB_DISPLAY_NAME = L"RAAS Client";
const wchar_t* sz_cVERB_NAME = L"RAASClient.OpenOnRemoteHost";
const wchar_t* sz_cRAAS_CLIENT_APPDATA_PROGRAM_SUB_DIR = L"Elbitin\\RAAS Client";

using namespace std;

namespace CommandID
{
	enum
	{
		None,

		OpenOnRemoteHost,
		RAASClientRemoteDesktop,
		RAASServerConfig
	};
}

CExplorerCommandVerb::CExplorerCommandVerb() : m_cRef(1), m_punkSite(NULL), m_hwnd(NULL), m_pstmShellItemArray(NULL), m_szFile(L"")
{
	DllAddRef();
}

// IUnknown
IFACEMETHODIMP CExplorerCommandVerb::QueryInterface(REFIID riid, void** ppv)
{
	static const QITAB qit[] =
	{
		QITABENT(CExplorerCommandVerb, IExplorerCommand),       // required
		QITABENT(CExplorerCommandVerb, IInitializeCommand),     // optional
		QITABENT(CExplorerCommandVerb, IObjectWithSite),        // optional
		{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

IFACEMETHODIMP_(ULONG) CExplorerCommandVerb::AddRef()
{
	return InterlockedIncrement(&m_cRef);
}

IFACEMETHODIMP_(ULONG) CExplorerCommandVerb::Release()
{
	long cRef = InterlockedDecrement(&m_cRef);
	if (!cRef)
	{
		delete this;
	}
	return cRef;
}

// IExplorerCommand
IFACEMETHODIMP CExplorerCommandVerb::GetTitle(IShellItemArray* /* psiItemArray */, LPWSTR* ppszName)
{
	// the verb name can be computed here, in this example it is static
	WCHAR szOpenRemoteMenuItem[80];
	LoadString(g_hInst, IDS_OPENREMOTE, szOpenRemoteMenuItem, ARRAYSIZE(szOpenRemoteMenuItem));
	return SHStrDup(szOpenRemoteMenuItem, ppszName);
}

IFACEMETHODIMP CExplorerCommandVerb::GetIcon(IShellItemArray* /* psiItemArray */, LPWSTR* ppszIcon)
{
	*ppszIcon = NULL;
	wchar_t* szRAASClientIcon = new wchar_t[MAX_PATH];
	wchar_t* szInstallDir = new wchar_t[MAX_PATH];
	HRESULT hr = GetRAASClientInstallDir(szInstallDir, MAX_PATH);
	if (SUCCEEDED(hr))
	{
		if (SUCCEEDED(StringCchPrintf(szRAASClientIcon, MAX_PATH,
			L"%s%s,-0", szInstallDir, sz_cRAASCLIENT_PROGRAM_NAME)))
		{
			SHStrDupW(szRAASClientIcon, ppszIcon);
			delete[] szRAASClientIcon;
		}
		delete[] szInstallDir;
	}
	return S_OK;
}

IFACEMETHODIMP CExplorerCommandVerb::GetToolTip(IShellItemArray* /* psiItemArray */, LPWSTR* ppszInfotip)
{
	*ppszInfotip = NULL;
	return E_NOTIMPL;
}

IFACEMETHODIMP CExplorerCommandVerb::GetCanonicalName(GUID* pguidCommandName)
{
	*pguidCommandName = __uuidof(this);
	return S_OK;
}

IFACEMETHODIMP CExplorerCommandVerb::GetState(IShellItemArray* psiItemArray, BOOL /* fOkToBeSlow */, EXPCMDSTATE* pCmdState)
{
	DWORD count;
	psiItemArray->GetCount(&count);

	IShellItem2* psi;
	HRESULT hr = GetItemAt(psiItemArray, 0, IID_PPV_ARGS(&psi));
	if (SUCCEEDED(hr))
	{
		PWSTR pszName;
		hr = psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &pszName);
		if (SUCCEEDED(hr))
		{
			if (SUCCEEDED(StringCchPrintf(m_szFile, ARRAYSIZE(m_szFile), pszName)))
			{
				wchar_t szAppDataPath[MAX_PATH];
				wchar_t szAppDataRAASServersXmlPath[MAX_PATH];
				wchar_t szServerName[MAX_PATH];
				if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
					SUCCEEDED(StringCchPrintf(szAppDataRAASServersXmlPath, ARRAYSIZE(szAppDataRAASServersXmlPath),
						L"%s\\%s\\%s", szAppDataPath, sz_cRAAS_CLIENT_APPDATA_SUB_DIR, sz_cSERVERS_XML_FILE_NAME)))
				{
					if (!GetServerNameFromPath(std::wstring(this->m_szFile), szServerName, MAX_PATH))
					{
						*pCmdState = ECS_HIDDEN;
					}
					else if (!ServerExists(szServerName, szAppDataRAASServersXmlPath))
					{
						*pCmdState = ECS_HIDDEN;
					}
					else {
						*pCmdState = ECS_ENABLED;
					}
				}
			}
			CoTaskMemFree(pszName);
		}
		psi->Release();
	}
	return hr;
}


IFACEMETHODIMP CExplorerCommandVerb::GetFlags(EXPCMDFLAGS* pFlags)
{
	*pFlags = ECF_DEFAULT;
	return S_OK;
}

IFACEMETHODIMP CExplorerCommandVerb::EnumSubCommands(IEnumExplorerCommand** ppEnum)
{
	*ppEnum = NULL;
	return E_NOTIMPL;
}

// IInitializeCommand

IFACEMETHODIMP CExplorerCommandVerb::Initialize(PCWSTR /* pszCommandName */, IPropertyBag* /* ppb */)
{
	return S_OK;
}

// IObjectWithSite

IFACEMETHODIMP CExplorerCommandVerb::SetSite(IUnknown* punkSite)
{
	SetInterface(&m_punkSite, punkSite);
	return S_OK;
}

IFACEMETHODIMP CExplorerCommandVerb::GetSite(REFIID riid, void** ppv)
{
	*ppv = NULL;
	return m_punkSite ? m_punkSite->QueryInterface(riid, ppv) : E_FAIL;
}

CExplorerCommandVerb::~CExplorerCommandVerb()
{
	SafeRelease(&m_punkSite);
	SafeRelease(&m_pstmShellItemArray);
	DllRelease();
}

BOOL CExplorerCommandVerb::ServerExists(wchar_t* szServerName, wchar_t* szServersFile)
{
	BOOL fExists = FALSE;
	HRESULT hr = S_OK;
	IXMLDOMDocument* pXMLDom = NULL;
	IXMLDOMNodeList* pNodes = NULL;
	IXMLDOMNode* pNode = NULL;
	IXMLDOMNode* pNodeText = NULL;
	IXMLDOMNode* pNodeEnabled = NULL;
	IXMLDOMNode* pServerNode = NULL;
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
		return false;
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


HRESULT CExplorerCommandVerb::GetRAASClientInstallDir(wchar_t* szDest, DWORD cchDest)
{
	HRESULT hr;
	HKEY hKey;
	LONG lRes = RegOpenKeyExW(HKEY_LOCAL_MACHINE, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
	if (lRes != ERROR_SUCCESS)
	{
		lRes = RegOpenKeyExW(HKEY_CURRENT_USER, sz_cRAAS_CLIENT_REGISTRY_KEY, 0, KEY_READ, &hKey);
		if (lRes != ERROR_SUCCESS)
			return E_FAIL;
	}
	ULONG nError;
	nError = RegQueryValueExW(hKey, sz_cRAAS_CLIENT_REGISTRY_INSTALL_DIR, 0, NULL, (LPBYTE)szDest, &cchDest);
	hr = (ERROR_SUCCESS == nError ? S_OK : E_FAIL);
	return hr;
}

void CExplorerCommandVerb::OpenRemote()
{
	wchar_t szPath[MAX_PATH];
	wchar_t szRemoteScPath[MAX_PATH];
	wchar_t* szInstallDir = new wchar_t[MAX_PATH];
	HRESULT hr = GetRAASClientInstallDir(szInstallDir, MAX_PATH);

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

BOOL CExplorerCommandVerb::GetServerNameFromPath(wstring szPath, wchar_t* buffer, int bufSize)
{
	if (szPath.substr(0, 2).compare(L"\\\\") != 0)
		return FALSE;
	else
	{
		wcscpy_s(buffer, (rsize_t)bufSize, (const wchar_t*)(szPath.substr(2, szPath.find(std::wstring(L"\\"), 2) - 2).c_str()));
		return TRUE;
	}
}


DWORD CExplorerCommandVerb::ThreadProc()
{
	IShellItemArray* psia;
	HRESULT hr = CoGetInterfaceAndReleaseStream(m_pstmShellItemArray, IID_PPV_ARGS(&psia));
	m_pstmShellItemArray = NULL;
	if (SUCCEEDED(hr))
	{
		DWORD count;
		psia->GetCount(&count);
		IShellItem2* psi;
		hr = GetItemAt(psia, 0, IID_PPV_ARGS(&psi));
		if (SUCCEEDED(hr))
		{
			PWSTR pszName;
			hr = psi->GetDisplayName(SIGDN_DESKTOPABSOLUTEPARSING, &pszName);
			if (SUCCEEDED(hr))
			{
				if (SUCCEEDED(StringCchPrintf(m_szFile, ARRAYSIZE(m_szFile), pszName)))
				{
					OpenRemote();
				}

				CoTaskMemFree(pszName);
			}

			psi->Release();
		}
		psia->Release();
	}
	return 0;
}

IFACEMETHODIMP CExplorerCommandVerb::Invoke(IShellItemArray* psia, IBindCtx* /* pbc */)
{
	IUnknown_GetWindow(m_punkSite, &m_hwnd);

	HRESULT hr = CoMarshalInterThreadInterfaceInStream(__uuidof(psia), psia, &m_pstmShellItemArray);
	if (SUCCEEDED(hr))
	{
		AddRef();
		if (!SHCreateThread(s_ThreadProc, this, CTF_COINIT_STA | CTF_PROCESS_REF, NULL))
		{
			Release();
		}
	}
	return S_OK;
}

static WCHAR const c_szPROG_ID[] = L"*";

HRESULT CExplorerCommandVerb_RegisterUnRegister(bool fRegister)
{
	CRegisterExtension re(__uuidof(CExplorerCommandVerb));

	HRESULT hr;
	if (fRegister)
	{
		hr = re.RegisterInProcServer(sz_cVERB_DISPLAY_NAME, L"Apartment");
		if (SUCCEEDED(hr))
		{
			hr = re.RegisterExplorerCommandVerb(c_szPROG_ID, sz_cVERB_NAME, sz_cVERB_DISPLAY_NAME);
			if (SUCCEEDED(hr))
			{
				hr = re.RegisterVerbAttribute(c_szPROG_ID, sz_cVERB_NAME, L"NeverDefault");
			}
		}
	}
	else
	{
		hr = re.UnRegisterVerb(c_szPROG_ID, sz_cVERB_NAME);
		hr = re.UnRegisterObject();
	}
	return hr;
}

HRESULT CExplorerCommandVerb_CreateInstance(REFIID riid, void** ppv)
{
	*ppv = NULL;
	CExplorerCommandVerb* pVerb = new (std::nothrow) CExplorerCommandVerb();
	HRESULT hr = pVerb ? S_OK : E_OUTOFMEMORY;
	if (SUCCEEDED(hr))
	{
		pVerb->QueryInterface(riid, ppv);
		pVerb->Release();
	}
	return hr;
}