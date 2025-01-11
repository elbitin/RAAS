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
#include "Log.h"
#include "BaseFolderViewChildHelper.h"

// Item idlists passed to folder methods are guaranteed to have accessible memory as specified
// by the cbSize in the itemid.  However they may be loaded from a persisted form (for example
// shortcuts on disk) where they could be corrupted.  It is the shell folder's responsibility
// to make sure it's safe against corrupted or malicious itemids.
PCFVITEMID CBaseFolderViewChildHelper::IsValid(PCUIDLIST_RELATIVE pidl)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::IsValid(PCUIDLIST_RELATIVE pidl)");
	PCFVITEMID pidmine = NULL;
	if (pidl)
	{
		pidmine = reinterpret_cast<PCFVITEMID>(pidl);
		if (!(pidmine->cb && n_cMYOBJID == pidmine->wMyObjID))
		{
			pidmine = NULL;
		}
	}
	return pidmine;
}

HRESULT CBaseFolderViewChildHelper::_GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR pszName, int cchMax");
	PCFVITEMID pMyObj = IsValid(pidl);
	HRESULT hr = pMyObj ? S_OK : E_INVALIDARG;
	if (SUCCEEDED(hr))
	{
		// StringCchCopy requires aligned strings, and itemids are not necessarily aligned.
		int i = 0;
		for (; i < cchMax; i++)
		{
			pszName[i] = reinterpret_cast<WCHAR*>((BYTE*)&pMyObj->data + cbDataOffset)[i];
			if (0 == pszName[i])
			{
				break;
			}
		}

		// Make sure the string is null-terminated.
		if (i == cchMax)
		{
			pszName[cchMax - 1] = 0;
		}
	}
	return hr;
}

HRESULT CBaseFolderViewChildHelper::_GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR* ppsz, int cch)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR* ppsz, int cch)");
	*ppsz = 0;
	PCFVITEMID pMyObj = IsValid(pidl);
	HRESULT hr = pMyObj ? S_OK : E_INVALIDARG;
	if (SUCCEEDED(hr))
	{
		*ppsz = (PWSTR)CoTaskMemAlloc(cch * sizeof(**ppsz));
		hr = *ppsz ? S_OK : E_OUTOFMEMORY;
		if (SUCCEEDED(hr))
		{
			hr = _GetPidlString(pidl, cbDataOffset, *ppsz, cch);
		}
	}
	return hr;
}

HRESULT CBaseFolderViewChildHelper::_GetModuleNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetModuleNameOffset(PCUIDLIST_RELATIVE pidl");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = 0;
	return S_OK;
}

HRESULT CBaseFolderViewChildHelper::GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)");
	HRESULT hr;
	UINT32 cbDataOffset;
	hr = _GetModuleNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, pszName, cchMax);
}

HRESULT CBaseFolderViewChildHelper::GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	UINT32 cbDataOffset;
	hr = _GetModuleNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, ppsz, pMyObj->cchModuleName);
}

HRESULT CBaseFolderViewChildHelper::_GetIconPathOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetIconPathOffset(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = (pMyObj->cchModuleName) * sizeof(WCHAR);
	return S_OK;
}

HRESULT CBaseFolderViewChildHelper::GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)");
	HRESULT hr;
	UINT32 cbDataOffset;
	hr = _GetIconPathOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, pszName, cchMax);
}

HRESULT CBaseFolderViewChildHelper::GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	UINT32 cbDataOffset;
	hr = _GetIconPathOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, ppsz, pMyObj->cchIconPath);
}

HRESULT CBaseFolderViewChildHelper::_GetGroupNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetGroupNameOffset(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = (pMyObj->cchModuleName + pMyObj->cchIconPath) * sizeof(WCHAR);
	return S_OK;
}

HRESULT CBaseFolderViewChildHelper::GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)");
	HRESULT hr;
	UINT32 cbDataOffset;
	hr = _GetGroupNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, pszName, cchMax);
}

HRESULT CBaseFolderViewChildHelper::GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	UINT32 cbDataOffset;
	hr = _GetGroupNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, ppsz, pMyObj->cchGroupName);
}

HRESULT CBaseFolderViewChildHelper::_GetDisplayNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetDisplayNameOffset(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = (pMyObj->cchModuleName + pMyObj->cchIconPath + pMyObj->cchGroupName) * sizeof(WCHAR);
	return S_OK;
}

HRESULT CBaseFolderViewChildHelper::GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)");
	HRESULT hr;
	UINT32 cbDataOffset;
	hr = _GetDisplayNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, pszName, cchMax);
}

HRESULT CBaseFolderViewChildHelper::GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	UINT32 cbDataOffset;
	hr = _GetDisplayNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, ppsz, pMyObj->cchDisplayName);
}

HRESULT CBaseFolderViewChildHelper::_GetParseNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetParseNameOffset(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = (pMyObj->cchModuleName + pMyObj->cchIconPath + pMyObj->cchGroupName + pMyObj->cchDisplayName) * sizeof(WCHAR);
	return S_OK;
}

HRESULT CBaseFolderViewChildHelper::GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax)");
	HRESULT hr;
	UINT32 cbDataOffset;
	hr = _GetParseNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, pszName, cchMax);
}

HRESULT CBaseFolderViewChildHelper::GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	UINT32 cbDataOffset;
	hr = _GetParseNameOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return hr;
	return _GetPidlString(pidl, cbDataOffset, ppsz, pMyObj->cchParseName);
}

HRESULT CBaseFolderViewChildHelper::_GetPidlOriginalOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetParseNameOffset(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return hr;
	*pcbOffset = (pMyObj->cchModuleName + pMyObj->cchIconPath + pMyObj->cchGroupName + pMyObj->cchDisplayName + pMyObj->cchParseName) * sizeof(WCHAR);
	return S_OK;
}

PIDLIST_ABSOLUTE CBaseFolderViewChildHelper::ClonePidlOriginal(PCUIDLIST_RELATIVE pidl)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_ClonePidlOriginal(PCUIDLIST_RELATIVE pidl)");
	HRESULT hr;
	PCFVITEMID pMyObj = IsValid(pidl);
	hr = pMyObj ? S_OK : E_FAIL;
	if (FAILED(hr))
		return NULL;
	UINT32 cbDataOffset;
	hr = _GetPidlOriginalOffset(pidl, &cbDataOffset);
	if (FAILED(hr))
		return NULL;
	if (pMyObj->fIsRealPath != TRUE)
		return NULL;
	return ILClone(reinterpret_cast<PCUIDLIST_RELATIVE>((BYTE*)&pMyObj->data + cbDataOffset));
}

HRESULT CBaseFolderViewChildHelper::GetFolderness(PCUIDLIST_RELATIVE pidl, BOOL* pbIsFolder)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_GetFolderness(PCUIDLIST_RELATIVE pidl, BOOL *pbIsFolder)");
	PCFVITEMID pMyObj = IsValid(pidl);
	HRESULT hr = pMyObj ? S_OK : E_INVALIDARG;
	if (SUCCEEDED(hr))
	{
		*pbIsFolder = pMyObj->fIsFolder;
	}
	return hr;
}


HRESULT CBaseFolderViewChildHelper::CreateChildID(ITEMDATA itemData, PITEMID_CHILD* ppidl)
{
	Log::Debug(L"CBaseFolderViewPidlHelper::_CreateChildID(ITEMDATA itemData, PITEMID_CHILD *ppidl)");
	if (itemData.pidlOriginal == NULL && itemData.fIsRealPath == TRUE)
		return E_FAIL;
	UINT16 cchModuleName = (UINT16)(lstrlen(itemData.szModuleName) + 1);
	UINT16 cchIconPath = (UINT16)(lstrlen(itemData.szIconPath) + 1);
	UINT16 cchGroupName = (UINT16)(lstrlen(itemData.szGroupName) + 1);
	UINT16 cchDisplayName = (UINT16)(lstrlen(itemData.szDisplayName) + 1);
	UINT16 cchParseName = (UINT16)(lstrlen(itemData.szParseName) + 1);
	UINT nIDSize;

	// Set size to size of object + size of strings + size of next cb
	nIDSize = sizeof(FVITEMID) + ILGetSize(itemData.pidlOriginal) + (cchModuleName + cchIconPath + cchGroupName + cchDisplayName + cchParseName) * sizeof(WCHAR) + sizeof(USHORT);

	// Allocate memory.
	FVITEMID* lpMyObj = (FVITEMID*)CoTaskMemAlloc(nIDSize);

	HRESULT hr = lpMyObj ? S_OK : E_OUTOFMEMORY;
	if (SUCCEEDED(hr))
	{
		ZeroMemory(lpMyObj, nIDSize);
		lpMyObj->cb = static_cast<short>(nIDSize - sizeof(lpMyObj->cb));
		lpMyObj->wMyObjID = n_cMYOBJID;
		lpMyObj->fIsFolder = (BOOL)itemData.fIsFolder;
		lpMyObj->fIsRealPath = itemData.fIsRealPath;
		lpMyObj->dwIconIndex = itemData.dwIconIndex;
		lpMyObj->ulAttributes = itemData.ulAttributes;
		lpMyObj->ulGroupOrder = itemData.ulGroupOrder;
		lpMyObj->cchModuleName = cchModuleName;
		lpMyObj->cchIconPath = cchIconPath;
		lpMyObj->cchGroupName = cchGroupName;
		lpMyObj->cchDisplayName = cchDisplayName;
		lpMyObj->cchParseName = cchParseName;
		void* pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)), itemData.szModuleName, cchModuleName * sizeof(WCHAR));
		hr = pResult ? S_OK : E_FAIL;
		if (SUCCEEDED(hr))
		{
			pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)) + cchModuleName * sizeof(WCHAR), itemData.szIconPath, cchIconPath * sizeof(WCHAR));
			hr = pResult ? S_OK : E_FAIL;
			if (SUCCEEDED(hr))
			{
				pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)) + (cchModuleName + cchIconPath) * sizeof(WCHAR), itemData.szGroupName, cchGroupName * sizeof(WCHAR));
				hr = pResult ? S_OK : E_FAIL;
				if (SUCCEEDED(hr))
				{
					pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)) + (cchModuleName + cchIconPath + cchGroupName) * sizeof(WCHAR), itemData.szDisplayName, cchDisplayName * sizeof(WCHAR));
					hr = pResult ? S_OK : E_FAIL;
					if (SUCCEEDED(hr))
					{
						pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)) + (cchModuleName + cchIconPath + cchGroupName + cchDisplayName) * sizeof(WCHAR), itemData.szParseName, cchParseName * sizeof(WCHAR));
						hr = pResult ? S_OK : E_FAIL;
						if (SUCCEEDED(hr))
						{
							if (itemData.pidlOriginal != NULL)
							{
								pResult = memcpy(reinterpret_cast<BYTE*>(&(lpMyObj->data)) + (cchModuleName + cchIconPath + cchGroupName + cchDisplayName + cchParseName) * sizeof(WCHAR), itemData.pidlOriginal, ILGetSize(itemData.pidlOriginal));
								hr = pResult ? S_OK : E_FAIL;
							}
						}
					}
				}
			}
		}
		if (SUCCEEDED(hr))
		{
			*ppidl = (PITEMID_CHILD)lpMyObj;
		}
		else
		{
			CoTaskMemFree(lpMyObj);
		}
	}
	return hr;
}
