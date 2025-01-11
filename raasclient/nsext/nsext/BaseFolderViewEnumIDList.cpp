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
#include "BaseFolderViewEnumIDList.h"
#include "BaseFolderViewChildHelper.h"
#include "Log.h"
#include "BaseFolderView.h"

CBaseFolderViewEnumIDList::CBaseFolderViewEnumIDList(DWORD grfFlags) :
	m_cRef(1), m_grfFlags(grfFlags)
{
	Log::Debug(L"CBaseFolderViewEnumIDList(DWORD grfFlags) : m_cRef(1), m_grfFlags(grfFlags)");
}

CBaseFolderViewEnumIDList::~CBaseFolderViewEnumIDList()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::~CBaseFolderViewEnumIDList()");
	for (std::vector<std::shared_ptr<ITEMDATA>>::iterator itemData = m_vData.begin(); itemData != m_vData.end(); ++itemData) {
		ILFree((*itemData)->pidlOriginal);
	}
}

HRESULT CBaseFolderViewEnumIDList::QueryInterface(REFIID riid, void **ppv)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::QueryInterface(REFIID riid, void **ppv)");
	static const QITAB qit[] = {
		QITABENT(CBaseFolderViewEnumIDList, IEnumIDList),
	{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

ULONG CBaseFolderViewEnumIDList::AddRef()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::AddRef()");
	return InterlockedIncrement(&m_cRef);
}

ULONG CBaseFolderViewEnumIDList::Release()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::Release()");
	long cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}
	return cRef;
}

HRESULT CBaseFolderViewEnumIDList::Skip(DWORD celt)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::Skip(DWORD celt)");
	m_nItem += celt;
	return S_OK;
}

HRESULT CBaseFolderViewEnumIDList::Reset()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::Reset()");
	m_nItem = 0;
	return S_OK;
}

HRESULT CBaseFolderViewEnumIDList::Clone(IEnumIDList **ppenum)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::Clone(IEnumIDList **ppenum)");
	// this method is rarely used and it's acceptable to not implement it.
	*ppenum = NULL;
	return E_NOTIMPL;
}


// This initializes the enumerator.  In this case we set up a default array of items, this represents our
// data source.  In a real-world implementation the array would be replaced and would be backed by some
// other data source that you traverse and convert into IShellFolder item IDs.
HRESULT CBaseFolderViewEnumIDList::Initialize()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::Initialize()");
	try
	{
		HRESULT hr;
		m_cServers = 0;
		hr = _GetServerStrings();
		if (FAILED(hr))
			return hr;
		_GetServerModuleNames();
		return S_OK;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderViewEnumIDList::Initialize()");
		return E_FAIL;
	}
}

void CBaseFolderViewEnumIDList::_GetServerModuleNames()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::_GetServerModuleNames()");
	for (unsigned int i = 0; i < m_cServers; i++)
	{
		wstring strServerModuleName = sz_cPROGRAM_NAME;
		strServerModuleName += L"\\";
		strServerModuleName += m_vServerAliases[i];
		m_vServerModuleNames.push_back(strServerModuleName);
	}
}

HRESULT CBaseFolderViewEnumIDList::_GetShares(const wchar_t* szServerName, NETWORK_SHARES &networkShares)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::_GetShares(const wchar_t* szServerName, NETWORK_SHARES &networkShares)");
	HRESULT hr;
	wchar_t szSharesPath[MAX_PATH];
	wchar_t szAppDataPath[MAX_PATH];
	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
		SUCCEEDED(StringCchPrintf(szSharesPath, ARRAYSIZE(szSharesPath),
			L"%s\\%s\\%s\\[%s]\\%s", szAppDataPath, sz_cVENDOR_NAME, sz_cPROGRAM_NAME, szServerName, sz_cSHARE_XML_FILE_NAME)) &&
		PathFileExists(szSharesPath))
	{
		hr = GetSharesFromXmlFile(szSharesPath, &networkShares);
	}
	else
		hr = E_FAIL;
	return hr;
}

HRESULT CBaseFolderViewEnumIDList::_GetServerVisibility(const wchar_t* szServerName, NETWORK_SHARES_VISIBILITY* NetworkSharesVisibility)
{
	Log::Debug(L"CBaseFolderViewEnumIDList::_GetServerVisibility(const wchar_t* szServerName, NETWORK_SHARES_VISIBILITY &NetworkSharesVisibility)");
	HRESULT hr;
	wchar_t szAppDataPath[MAX_PATH];
	wchar_t szServerVisibilityPath[MAX_PATH];
	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
		SUCCEEDED(StringCchPrintf(szServerVisibilityPath, ARRAYSIZE(szServerVisibilityPath),
			L"%s\\%s\\%s\\[%s]\\%s", szAppDataPath, sz_cVENDOR_NAME, sz_cPROGRAM_NAME, szServerName, sz_cNSEXT_VISIBILITY_XML_FILE_NAME)) &&
		PathFileExists(szServerVisibilityPath))
	{
		hr = GetServerVisibilityFromXmlFile(szServerVisibilityPath, NetworkSharesVisibility);
	}
	else
		hr = E_FAIL;
	return hr;
}

HRESULT CBaseFolderViewEnumIDList::_GetServerStrings()
{
	Log::Debug(L"CBaseFolderViewEnumIDList::_GetServerStrings()");
	HRESULT hr;
	wchar_t szAppDataPath[MAX_PATH];
	wchar_t szServersXmlPath[MAX_PATH];
	if (SUCCEEDED(SHGetFolderPath(NULL, CSIDL_APPDATA, NULL, 0, szAppDataPath)) &&
		SUCCEEDED(StringCchPrintf(szServersXmlPath, ARRAYSIZE(szServersXmlPath),
			L"%s\\%s\\%s\\%s", szAppDataPath, sz_cVENDOR_NAME, sz_cPROGRAM_NAME, sz_cSERVERS_XML_FILE_NAME)) &&
		PathFileExists(szServersXmlPath))
	{
		hr = GetServerStringsFromXmlFile(szServersXmlPath, m_vServerNames, m_vServerAliases, &m_cServers);
	}
	else
		hr = E_FAIL;
	return hr;
}

// Retrieves the specified number of item identifiers in
// the enumeration sequence and advances the current position
// by the number of items retrieved.
HRESULT CBaseFolderViewEnumIDList::Next(ULONG celt, PITEMID_CHILD *rgelt, ULONG *pceltFetched)
{
	try
	{
		Log::Debug(L"CBaseFolderViewEnumIDList::Next(ULONG celt, PITEMID_CHILD *rgelt, ULONG *pceltFetched)");
		ULONG celtFetched = 0;
		HRESULT hr = (pceltFetched || celt <= 1) ? S_OK : E_INVALIDARG;
		if (SUCCEEDED(hr))
		{
			ULONG i = 0;
			while (SUCCEEDED(hr) && i < celt && m_nItem < m_vData.size())
			{
				BOOL fSkip = FALSE;
				if (!(m_grfFlags & SHCONTF_STORAGE))
				{
					if (m_vData[m_nItem]->fIsFolder)
					{
						if (!(m_grfFlags & SHCONTF_FOLDERS))
						{
							// this is a folder, but caller doesnt want folders
							fSkip = TRUE;
						}
					}
					else
					{
						if (!(m_grfFlags & SHCONTF_NONFOLDERS))
						{
							// this is a file, but caller doesnt want files
							fSkip = TRUE;
						}
					}
				}
				if (!fSkip)
				{
					hr = CBaseFolderViewChildHelper::CreateChildID(*m_vData[m_nItem], &rgelt[i]);
					if (SUCCEEDED(hr))
					{
						celtFetched++;
						i++;
					}
				}
				m_nItem++;
			}
		}
		if (pceltFetched)
		{
			*pceltFetched = celtFetched;
		}
		return (celtFetched == celt) ? S_OK : S_FALSE;
	}
	catch (...)
	{
		Log::Error(L"CBaseFolderViewEnumIDList::Next(ULONG celt, PITEMID_CHILD *rgelt, ULONG *pceltFetched)");
		return E_FAIL;
	}
}