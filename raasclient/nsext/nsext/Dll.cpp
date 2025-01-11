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

   This file includes code from Microsofts ExplorerDataProvider but
   have been modified by Elbitin, modifications are copyrighted by
   Elbitin but included is copyright notices from ExplorerDataProvider
   project.

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
#include <shlwapi.h>
#include <objbase.h>
#include <Shlobj.h>
#include <olectl.h>
#include <strsafe.h>
#include <new>  // std::nothrow

// The GUID for the FolderViewImpl
#include "GUID.h"

#pragma comment(lib, "propsys.lib")
#pragma comment(lib, "shlwapi.lib")

HRESULT NSExt_Initialize(REFIID riid, void **ppv);
HRESULT CServerContextMenu_CreateInstance(REFIID riid, void **ppv);
HRESULT CRealFolderBackgroundContextMenu_CreateInstance(REFIID riid, void **ppv);

typedef HRESULT(*PFNCREATEINSTANCE)(REFIID riid, void **ppvObject);
struct CLASS_OBJECT_INIT
{
	const CLSID *pClsid;
	PFNCREATEINSTANCE pfnCreate;
};

// add classes supported by this module here
const CLASS_OBJECT_INIT c_rgClassObjectInit[] =
{
	{ &CLSID_FolderViewImpl, NSExt_Initialize },
	{ &CLSID_ServerContextMenu, CServerContextMenu_CreateInstance },
    { &CLSID_FolderViewContextMenuRealFolderBackground, CRealFolderBackgroundContextMenu_CreateInstance }
};

long g_cRefModule = 0;

// Handle the the DLL's module
HINSTANCE g_hInst = NULL;

// Standard DLL functions
STDAPI_(BOOL) DllMain(HINSTANCE hInstance, DWORD dwReason, void *)
{
	if (dwReason == DLL_PROCESS_ATTACH)
	{
		g_hInst = hInstance;
		DisableThreadLibraryCalls(hInstance);
	}
	return TRUE;
}

STDAPI DllCanUnloadNow()
{
	// Only allow the DLL to be unloaded after all outstanding references have been released
	return (g_cRefModule == 0) ? S_OK : S_FALSE;
}

void DllAddRef()
{
	InterlockedIncrement(&g_cRefModule);
}

void DllRelease()
{
	InterlockedDecrement(&g_cRefModule);
}

class CClassFactory : public IClassFactory
{
public:
	static HRESULT CreateInstance(REFCLSID clsid, const CLASS_OBJECT_INIT *pClassObjectInits, size_t cClassObjectInits, REFIID riid, void **ppv)
	{
		*ppv = NULL;
		HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;
		for (size_t i = 0; i < cClassObjectInits; i++)
		{
			if (clsid == *pClassObjectInits[i].pClsid)
			{
				IClassFactory *pClassFactory = new (std::nothrow) CClassFactory(pClassObjectInits[i].pfnCreate);
				hr = pClassFactory ? S_OK : E_OUTOFMEMORY;
				if (SUCCEEDED(hr))
				{
					hr = pClassFactory->QueryInterface(riid, ppv);
					pClassFactory->Release();
				}
				break; // match found
			}
		}
		return hr;
	}

	CClassFactory(PFNCREATEINSTANCE pfnCreate) : m_cRef(1), m_pfnCreate(pfnCreate)
	{
		DllAddRef();
	}

	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void ** ppv)
	{
		static const QITAB qit[] =
		{
			QITABENT(CClassFactory, IClassFactory),
			{ 0 }
		};
		return QISearch(this, qit, riid, ppv);
	}

	IFACEMETHODIMP_(ULONG) AddRef()
	{
		return InterlockedIncrement(&m_cRef);
	}

	IFACEMETHODIMP_(ULONG) Release()
	{
		long cRef = InterlockedDecrement(&m_cRef);
		if (cRef == 0)
		{
			delete this;
		}
		return cRef;
	}

	// IClassFactory
	IFACEMETHODIMP CreateInstance(IUnknown *punkOuter, REFIID riid, void **ppv)
	{
		return punkOuter ? CLASS_E_NOAGGREGATION : m_pfnCreate(riid, ppv);
	}

	IFACEMETHODIMP LockServer(BOOL fLock)
	{
		if (fLock)
		{
			DllAddRef();
		}
		else
		{
			DllRelease();
		}
		return S_OK;
	}

private:
	~CClassFactory()
	{
		DllRelease();
	}

	long m_cRef;
	PFNCREATEINSTANCE m_pfnCreate;
};


STDAPI DllGetClassObject(REFCLSID clsid, REFIID riid, void **ppv)
{
	return CClassFactory::CreateInstance(clsid, c_rgClassObjectInit, ARRAYSIZE(c_rgClassObjectInit), riid, ppv);
}


#define MYCOMPUTER_NAMESPACE_GUID L"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Desktop\\NameSpace\\%s"

const WCHAR g_szExtTitle[] = L"RAAS Client";

void RefreshFolderViews(UINT csidl)
{
	PIDLIST_ABSOLUTE pidl;
	if (SUCCEEDED(SHGetSpecialFolderLocation(NULL, csidl, &pidl)))
	{
		SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_IDLIST, pidl, 0);
		CoTaskMemFree(pidl);
	}
}

// Structure to hold data for individual keys to register.
typedef struct
{
	HKEY  hRootKey;
	PCWSTR pszSubKey;
	PCWSTR pszClassID;
	PCWSTR pszValueName;
	BYTE *pszData;
	DWORD dwType;
} REGSTRUCT;

//  1. The classID must be created under HKCR\CLSID.
//     a. DefaultIcon must be set to <Path and Module>,0.
//     b. InprocServer32 set to path and module.
//        i. Threading model specified as Apartment.
//     c. ShellFolder attributes must be set.
//  2. If the extension in non-rooted, its GUID is entered at the desired folder.
//  3. It must be registered as approved for Windows NT or XP.
STDAPI DllRegisterServer()
{
	return S_OK;
}

// Registry keys are removed here.
STDAPI DllUnregisterServer()
{
	return S_OK;
}
