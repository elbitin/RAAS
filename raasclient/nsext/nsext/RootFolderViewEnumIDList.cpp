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
#include "resource.h"
#include "Log.h"
#include "RootFolderViewEnumIDList.h"

CRootFolderViewEnumIDList::~CRootFolderViewEnumIDList()
{
}

// This initializes the enumerator.  In this case we set up a default array of items, this represents our
// data source.  In a real-world implementation the array would be replaced and would be backed by some
// other data source that you traverse and convert into IShellFolder item IDs.
HRESULT CRootFolderViewEnumIDList::Initialize()
{
	try
	{
		Log::Debug(L"CRootFolderViewEnumIDList::Initialize()");
		HRESULT hr = CBaseFolderViewEnumIDList::Initialize();
		if (FAILED(hr))
			return hr;
		for (unsigned int i = 0; i < m_cServers; i++)
		{
			NETWORK_SHARES_VISIBILITY NetworkSharesVisibility;
			hr = _GetServerVisibility(m_vServerNames[i].c_str(), &NetworkSharesVisibility);
			if (FAILED(hr))
				continue;
			if (NetworkSharesVisibility.bActive)
				_AddServer(m_vServerModuleNames[i].c_str(), m_vServerAliases[i].c_str(), i);
		}
		return S_OK;
	}
	catch (...)
	{
		Log::Error(L"CRootFolderViewEnumIDList::Initialize()");
		return E_FAIL;
	}
}

HRESULT CRootFolderViewEnumIDList::_AddServer(const wchar_t*  szServerModuleName, const wchar_t* szServerAlias, int i)
{
	Log::Debug(L"CRootFolderViewEnumIDList::_AddServer(const wchar_t*  szServerModuleName, const wchar_t* szServerAlias, int i)");
	HRESULT hr;
	std::shared_ptr<ITEMDATA> itemData(new ITEMDATA());
	hr = itemData ? S_OK : E_OUTOFMEMORY;
	itemData->pidlOriginal = NULL;
	itemData->fIsFolder = TRUE;
	itemData->fIsRealPath = FALSE;
	hr = StringCchCopyW(itemData->szModuleName, MAX_PATH, szServerModuleName);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szIconPath, MAX_PATH, L"shell32.dll");
	if (FAILED(hr))
		return hr;
	itemData->ulGroupOrder = i;
	itemData->dwIconIndex = 15;
	hr = StringCchCopyW(itemData->szParseName, MAX_PATH, szServerAlias);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szDisplayName, MAX_PATH, szServerAlias);
	if (FAILED(hr))
		return hr;
	wchar_t szComputer[MAX_PATH];
	HMODULE lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	LoadString(lib, IDS_SHELL32_COMPUTER, szComputer, MAX_PATH);
	FreeLibrary(lib);
	hr = StringCchCopyW(itemData->szGroupName, MAX_PATH, szComputer);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szRealPath, MAX_PATH, L"");
	if (FAILED(hr))
		return hr;
	if (SUCCEEDED(hr))
	{
		m_vData.push_back(itemData);
	}
	return hr;
}