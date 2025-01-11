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
#include <sys/types.h>
#include <sys/stat.h>
#include <algorithm>
#include "resource.h"
#include "shlobj_core.h"
#include "Log.h"
#include "ServerFolderViewEnumIDList.h"

CServerFolderViewEnumIDList::CServerFolderViewEnumIDList(DWORD grfFlags, wchar_t* szModuleName) : CBaseFolderViewEnumIDList(grfFlags), m_strModuleName(szModuleName), m_szServerAlias(L""), m_szServerName(L""), m_networkSharesVisibility({}), m_networkShares({})
{
	Log::Debug(L"CServerFolderViewEnumIDList::CServerFolderViewEnumIDList(DWORD grfFlags, wchar_t* szModuleName) : CBaseFolderViewEnumIDList(grfFlags), m_strModuleName(szModuleName), m_szServerAlias(L""), m_szServerName(L""), m_networkSharesVisibility({}), m_networkShares({})");
}

CServerFolderViewEnumIDList::~CServerFolderViewEnumIDList()
{
	Log::Debug(L"CServerFolderViewEnumIDList::~CServerFolderViewEnumIDList()");
}

HRESULT CServerFolderViewEnumIDList::Initialize()
{
	try
	{
		Log::Debug(L"CServerFolderViewEnumIDList::Initialize()");
		HRESULT hr = CBaseFolderViewEnumIDList::Initialize();
		if (FAILED(hr))
			return hr;
		for (unsigned int i = 0; i < m_cServers; i++)
		{
			if (StrCmpW(m_strModuleName.c_str(), m_vServerModuleNames[i].c_str()) == 0)
			{
				m_szServerName = const_cast<wchar_t*>(m_vServerNames[i].c_str());
				m_szServerAlias = const_cast<wchar_t*>(m_vServerAliases[i].c_str());
				hr = _GetShares(m_szServerName, m_networkShares);
				if (FAILED(hr))
					continue;
				hr = _GetServerVisibility(m_szServerName, &m_networkSharesVisibility);
				if (FAILED(hr))
					continue;
				if (m_networkSharesVisibility.bThreeDObjects)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_c3D_OBJECTS, COMMON_FOLDER_ICON_INDICES.n_c3DOBJECTS_ICON_INDEX);
				if (m_networkSharesVisibility.bContacts)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cCONTACTS, COMMON_FOLDER_ICON_INDICES.n_cCONTACTS_ICON_INDEX);
				if (m_networkSharesVisibility.bDesktop)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cDESKTOP, COMMON_FOLDER_ICON_INDICES.n_cDESCTOP_ICON_INDEX);
				if (m_networkSharesVisibility.bDocuments)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cDOCUMENTS, COMMON_FOLDER_ICON_INDICES.n_cDOCUMENTS_ICON_INDEX);
				if (m_networkSharesVisibility.bDownloads)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cDOWNLOADS, COMMON_FOLDER_ICON_INDICES.n_cDOWNLOADS_ICON_INDEX);
				if (m_networkSharesVisibility.bFavorites)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cFAVORITES, COMMON_FOLDER_ICON_INDICES.n_cFAVORITES_ICON_INDEX);
				if (m_networkSharesVisibility.bLinks)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cLINKS, COMMON_FOLDER_ICON_INDICES.n_cLINKS_ICON_INDEX);
				if (m_networkSharesVisibility.bMusic)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cMUSIC, COMMON_FOLDER_ICON_INDICES.n_cMUSIC_ICON_INDEX);
				if (m_networkSharesVisibility.bPictures)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cPICTURES, COMMON_FOLDER_ICON_INDICES.n_cPICTURES_ICON_INDEX);
				if (m_networkSharesVisibility.bSavedGames)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cSAVED_GAMES, COMMON_FOLDER_ICON_INDICES.n_cSAVEDGAMES_ICON_INDEX);
				if (m_networkSharesVisibility.bSearches)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cSEARCHES, COMMON_FOLDER_ICON_INDICES.n_cSEARCHES_ICON_INDEX);
				if (m_networkSharesVisibility.bVideos)
					_AddUserFolder(COMMON_FOLDER_NAMES.sz_cVIDEOS, COMMON_FOLDER_ICON_INDICES.n_cVIDEOS_ICON_INDEX);
				bool bDriveAccessed = false;
				if (m_vData.empty())
				{
					// Test if any drives are accessible and add all that should be added if they are
					for (unsigned int j = 0; j < m_networkShares.vDriveShares.size(); j++)
					{
						if (_DriveAccess(j))
							bDriveAccessed = true;
					}
				}
				if (!m_vData.empty() || bDriveAccessed)
				{
					for (unsigned int j = 0; j < m_networkShares.vDriveShares.size(); j++)
					{
						if (_ShouldShowDrive(j))
							_AddDrive(j);
					}
				}
			}
		}
		if (m_vData.empty())
		{
			return E_FAIL;
		}
		else
		{
			return S_OK;
		}
	}
	catch (...)
	{
		Log::Error(L"CServerFolderViewEnumIDList::Initialize()");
		return E_FAIL;
	}
}

bool CServerFolderViewEnumIDList::_DriveAccess(int driveIndex)
{
	WCHAR szRealPath[MAX_PATH];
	HRESULT hr = StringCchPrintf(szRealPath, MAX_PATH, L"\\\\%s\\%s", m_szServerName, m_networkShares.vDriveShares.at(driveIndex)->strPath.c_str());
	if (FAILED(hr))
		return false;
	struct stat info;
	char output[MAX_PATH];
	sprintf_s(output, MAX_PATH, "%ws", szRealPath);
	if (stat(output, &info) == 0)
		return true;
	return false;
}

HRESULT CServerFolderViewEnumIDList::_AddDrive(unsigned int driveIndex)
{
	Log::Debug(L"CServerFolderViewEnumIDList::_AddDrive(unsigned int driveIndex)");
	HRESULT hr;
	std::shared_ptr<ITEMDATA> itemData(new ITEMDATA());
	itemData->pidlOriginal = NULL;
	hr = itemData ? S_OK : E_OUTOFMEMORY;
	if (FAILED(hr))
		return hr;
	hr = StringCchPrintf(itemData->szRealPath, MAX_PATH, L"\\\\%s\\%s", m_szServerName, m_networkShares.vDriveShares.at(driveIndex)->strPath.c_str());
	if (FAILED(hr))
		return hr;
	hr = StringCchPrintf(itemData->szModuleName, MAX_PATH, L"%s\\%s\\%s", sz_cPROGRAM_NAME, m_szServerAlias, m_networkShares.vDriveShares.at(driveIndex)->strPath.c_str());
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szIconPath, MAX_PATH, sz_cSHARES_ICON_DLL);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szParseName, MAX_PATH, m_networkShares.vDriveShares.at(driveIndex)->strName.c_str());
	if (FAILED(hr))
		return hr;
	wchar_t szDevicesAndDrives[MAX_PATH];
	HMODULE lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	LoadString(lib, IDS_SHELL32_DEVICESANDDRIVES, szDevicesAndDrives, MAX_PATH);
	FreeLibrary(lib);
	hr = StringCchCopyW(itemData->szGroupName, MAX_PATH, szDevicesAndDrives);
	if (FAILED(hr))
		return hr;
	itemData->fIsFolder = TRUE;
	itemData->fIsRealPath = TRUE;
	if (FAILED(hr))
		return hr;
	DRIVE_TYPE nDriveType = _GetDriveType(const_cast<wchar_t*>(m_networkShares.vDriveShares.at(driveIndex)->strType.c_str()), const_cast<wchar_t*>(m_networkShares.vDriveShares.at(driveIndex)->strName.c_str()));
	itemData->dwIconIndex = _GetDriveIconIndex(nDriveType);
	itemData->ulGroupOrder = 1;
	WCHAR szDisplayNameDescription[MAX_PATH];
	lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	switch (nDriveType)
	{
	case n_cDISKETTE_DRIVE:
		LoadString(lib, IDS_SHELL32_DISKETTEDRIVE, szDisplayNameDescription, MAX_PATH);
		break;
	case n_cREMOVABLE_DRIVE:
		LoadString(lib, IDS_SHELL32_REMOVABLEDRIVE, szDisplayNameDescription, MAX_PATH);
		break;
	case n_cCDROM_DRIVE:
		LoadString(lib, IDS_SHELL32_CDROMDRIVE, szDisplayNameDescription, MAX_PATH);
		break;
	case n_cFIXED_DRIVE:
		LoadString(lib, IDS_SHELL32_FIXEDDRIVE, szDisplayNameDescription, MAX_PATH);
		break;
	}
	FreeLibrary(lib);
	if (FAILED(hr))
		return hr;
	hr = StringCchPrintf(itemData->szDisplayName, MAX_PATH, L"%s (%s)", szDisplayNameDescription, m_networkShares.vDriveShares.at(driveIndex)->strName.c_str());
	if (FAILED(hr))
		return hr;
	LPITEMIDLIST fullPidlFolder;
	IShellFolder* pDesktop;
	hr = SHGetDesktopFolder(&pDesktop);
	if (SUCCEEDED(hr))
	{
		PWSTR szParentRealPath = new (std::nothrow) wchar_t[MAX_PATH];
		hr = szParentRealPath ? S_OK : E_OUTOFMEMORY;
		if (SUCCEEDED(hr))
		{
			hr = StringCchPrintf(szParentRealPath, MAX_PATH, L"\\\\%s", m_szServerName);
			if (SUCCEEDED(hr))
			{
				LPITEMIDLIST pidlParent;
				hr = pDesktop->ParseDisplayName(NULL, NULL, szParentRealPath, NULL, &pidlParent, NULL);
				if (SUCCEEDED(hr))
				{
					IShellFolder* pParent;
					hr = pDesktop->BindToObject(pidlParent, NULL, IID_IShellFolder, (void**)&pParent);
					if (SUCCEEDED(hr))
					{
						hr = pDesktop->ParseDisplayName(NULL, NULL, itemData->szRealPath, NULL, &fullPidlFolder, NULL);
						if (SUCCEEDED(hr))
						{
							LPITEMIDLIST pidlFolder = ILFindLastID(fullPidlFolder);
							itemData->ulAttributes = 0xF1CFC17F;
							hr = pParent->GetAttributesOf(1, const_cast<LPCITEMIDLIST*>(&pidlFolder), &itemData->ulAttributes);
							itemData->ulAttributes &= ~(SFGAO_CANMOVE | SFGAO_CANRENAME | SFGAO_CANDELETE);
							if (SUCCEEDED(hr))
							{
								itemData->pidlOriginal = ILClone(fullPidlFolder);
								hr = itemData->pidlOriginal ? S_OK : E_FAIL;
								if (SUCCEEDED(hr))
								{
									try
									{
										m_vData.push_back(itemData);
									}
									catch (...)
									{
										ILFree(itemData->pidlOriginal);
									}
								}
							}
							ILFree(fullPidlFolder);
						}
						pParent->Release();
					}
					ILFree(pidlParent);
				}
			}
			delete[] szParentRealPath;
		}
		pDesktop->Release();
	}
	return hr;
}

bool CServerFolderViewEnumIDList::_ShouldShowDrive(unsigned int driveIndex)
{
	Log::Debug(L"CServerFolderViewEnumIDList::_ShouldShowDrive(unsigned int driveIndex)");
	std::transform(m_networkShares.vDriveShares.at(driveIndex)->strType.begin(), m_networkShares.vDriveShares.at(driveIndex)->strType.end(), m_networkShares.vDriveShares.at(driveIndex)->strType.begin(), [](wchar_t c) {return static_cast<wchar_t>(::toupper(c)); });
	if (wcscmp(const_cast<wchar_t*>(m_networkShares.vDriveShares.at(driveIndex)->strType.c_str()), L"REMOVABLE") == 0)
	{
		if (_wcsupr_s(const_cast<wchar_t*>(m_networkShares.vDriveShares.at(driveIndex)->strName.c_str()), MAX_PATH) != 0)
			return false;
		if (wcscmp(m_networkShares.vDriveShares.at(driveIndex)->strName.c_str(), L"A:") == 0 || wcscmp(m_networkShares.vDriveShares.at(driveIndex)->strName.c_str(), L"B:") == 0)
		{
			if (m_networkSharesVisibility.bDisketteDrives)
				return true;
		}
		else
		{
			if (m_networkSharesVisibility.bRemovableDrives)
				return true;
		}
	}
	if (wcscmp(m_networkShares.vDriveShares.at(driveIndex)->strType.c_str(), L"CDROM") == 0)
	{
		if (m_networkSharesVisibility.bCDDrives)
			return true;
	}
	if (wcscmp(m_networkShares.vDriveShares.at(driveIndex)->strType.c_str(), L"FIXED") == 0)
	{
		if (m_networkSharesVisibility.bFixedHardDrives)
			return true;
	}
	return false;
}

int CServerFolderViewEnumIDList::_GetDriveIconIndex(DRIVE_TYPE nDriveType)
{
	Log::Debug(L"CServerFolderViewEnumIDList::_GetDriveIconIndex(DRIVE_TYPE nDriveType)");
	if (nDriveType == n_cREMOVABLE_DRIVE)
		return DRIVE_ICON_INDICES.n_cREMOVABLE_DRIVE_ICON_INDEX;
	if (nDriveType == n_cDISKETTE_DRIVE)
		return DRIVE_ICON_INDICES.n_cDISKETTE_DRIVE_ICON_INDEX;;
	if (nDriveType == n_cCDROM_DRIVE)
		return DRIVE_ICON_INDICES.n_cCDROM_DRIVE_ICON_INDEX;
	if (nDriveType == n_cFIXED_DRIVE)
		return DRIVE_ICON_INDICES.n_cFIXED_DRIVE_ICON_INDEX;
	return 0;
}

CServerFolderViewEnumIDList::DRIVE_TYPE CServerFolderViewEnumIDList::_GetDriveType(wchar_t* szDriveType, wchar_t* szDriveName)
{
	Log::Debug(L"CServerFolderViewEnumIDList::_GetDriveType(wchar_t* szDriveType, wchar_t* szDriveName)");
	_wcsupr_s(szDriveType, MAX_PATH);
	if (wcscmp(szDriveType, L"REMOVABLE") == 0)
	{
		if (_wcsupr_s(szDriveName, MAX_PATH) != 0)
			return n_cNONE;
		if (wcscmp(szDriveName, L"A:") == 0 || wcscmp(szDriveName, L"B:") == 0)
		{
			return n_cDISKETTE_DRIVE;
		}
		else
		{
			return n_cREMOVABLE_DRIVE;
		}
	}
	if (wcscmp(szDriveType, L"CDROM") == 0)
	{
		return n_cCDROM_DRIVE;
	}
	if (wcscmp(szDriveType, L"FIXED") == 0)
	{
		return n_cFIXED_DRIVE;
	}
	return n_cNONE;
}

HRESULT CServerFolderViewEnumIDList::_AddUserFolder(const wchar_t* szFolderName, unsigned int uiIconIndex)
{
	Log::Debug(L"CServerFolderViewEnumIDList::_AddUserFolder(const wchar_t* szFolderName, unsigned int uiIconIndex, unsigned long ulGroupOrder)");
	HRESULT hr;
	std::shared_ptr<ITEMDATA> itemData = std::shared_ptr<ITEMDATA>(new ITEMDATA());
	hr = StringCchPrintf(itemData->szRealPath, MAX_PATH, L"\\\\%s\\%s\\%s", m_szServerName, m_networkShares.strProfilePath.c_str(), szFolderName);
	if (FAILED(hr))
		return hr;
	hr = StringCchPrintf(itemData->szModuleName, MAX_PATH, L"%s\\%s\\%s", sz_cPROGRAM_NAME, m_szServerAlias, szFolderName);
	if (FAILED(hr))
		return hr;
	hr = StringCchCopyW(itemData->szIconPath, MAX_PATH, sz_cCOMMON_FOLDERS_ICON_DLL);
	if (FAILED(hr))
		return hr;
	itemData->dwIconIndex = uiIconIndex;
	wchar_t szFolders[MAX_PATH];
	HMODULE lib = LoadLibrary(L"shell32.dll");
	if (lib == 0)
		return E_FAIL;
	LoadString(lib, IDS_SHELL32_FOLDERS, szFolders, MAX_PATH);
	FreeLibrary(lib);
	hr = StringCchCopyW(itemData->szGroupName, MAX_PATH, szFolders);
	if (FAILED(hr))
		return hr;
	LPITEMIDLIST fullPidlFolder;
	IShellFolder* pDesktop;
	hr = SHGetDesktopFolder(&pDesktop);
	if (SUCCEEDED(hr))
	{
		PWSTR szParentRealPath = new (std::nothrow) wchar_t[MAX_PATH];
		hr = szParentRealPath ? S_OK : E_OUTOFMEMORY;
		if (SUCCEEDED(hr))
		{
			hr = StringCchPrintf(szParentRealPath, MAX_PATH, L"\\\\%s\\%s", m_szServerName, m_networkShares.strProfilePath.c_str());
			if (SUCCEEDED(hr))
			{
				LPITEMIDLIST pidlParent;
				hr = pDesktop->ParseDisplayName(NULL, NULL, szParentRealPath, NULL, &pidlParent, NULL);
				if (SUCCEEDED(hr))
				{
					IShellFolder* pParent;
					hr = pDesktop->BindToObject(pidlParent, NULL, IID_IShellFolder, reinterpret_cast<void**>(&pParent));
					if (SUCCEEDED(hr))
					{
						hr = pDesktop->ParseDisplayName(NULL, NULL, itemData->szRealPath, NULL, &fullPidlFolder, NULL);
						if (SUCCEEDED(hr))
						{
							STRRET pDisplayName;
							hr = pParent->GetDisplayNameOf(ILFindLastID(fullPidlFolder), SHGDN_NORMAL, &pDisplayName);
							if (SUCCEEDED(hr))
							{
								wchar_t* szDisplayName = new wchar_t[MAX_PATH];
								hr = StrRetToStrW(&pDisplayName, fullPidlFolder, &szDisplayName);
								if (SUCCEEDED(hr))
								{
									hr = StringCchCopyW(itemData->szParseName, MAX_PATH, szDisplayName);
									if (SUCCEEDED(hr))
									{
										hr = StringCchCopyW(itemData->szDisplayName, MAX_PATH, szDisplayName);
										if (SUCCEEDED(hr))
										{
											if (SUCCEEDED(hr))
											{
												LPITEMIDLIST pidlFolder = ILFindLastID(fullPidlFolder);
												itemData->ulAttributes = 0xF1CFC17F;
												hr = pParent->GetAttributesOf(1, const_cast<LPCITEMIDLIST*>(&pidlFolder), &itemData->ulAttributes);
												itemData->ulAttributes &= ~(SFGAO_CANMOVE | SFGAO_CANRENAME | SFGAO_CANDELETE);
												if (SUCCEEDED(hr))
												{
													itemData->pidlOriginal = ILClone(fullPidlFolder);
													hr = itemData->pidlOriginal ? S_OK : E_FAIL;
													if (SUCCEEDED(hr))
													{
														itemData->fIsFolder = TRUE;
														itemData->fIsRealPath = TRUE;
														itemData->ulGroupOrder = 0;
														try
														{
															m_vData.push_back(itemData);
														}
														catch (...)
														{
															ILFree(itemData->pidlOriginal);
														}
													}
												}
											}
										}
									}
									delete[] szDisplayName;
								}
							}
							ILFree(fullPidlFolder);
						}
						pParent->Release();
					}
					ILFree(pidlParent);
				}
			}
			delete[] szParentRealPath;
		}
		pDesktop->Release();
	}
	return hr;
}