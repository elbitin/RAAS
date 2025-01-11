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
#pragma once
#include "BaseFolderViewEnumIDList.h"

class CServerFolderViewEnumIDList : public CBaseFolderViewEnumIDList
{
public:
	CServerFolderViewEnumIDList(DWORD grfFlags, wchar_t* szModuleName);
	HRESULT Initialize();
private:
	~CServerFolderViewEnumIDList();

	const struct {
		const wchar_t* sz_c3D_OBJECTS = L"3D Objects";
		const wchar_t* sz_cCONTACTS = L"Contacts";
		const wchar_t* sz_cDESKTOP = L"Desktop";
		const wchar_t* sz_cDOCUMENTS = L"Documents";
		const wchar_t* sz_cDOWNLOADS = L"Downloads";
		const wchar_t* sz_cFAVORITES = L"Favorites";
		const wchar_t* sz_cLINKS = L"Links";
		const wchar_t* sz_cMUSIC = L"Music";
		const wchar_t* sz_cPICTURES = L"Pictures";
		const wchar_t* sz_cSAVED_GAMES = L"Saved Games";
		const wchar_t* sz_cSEARCHES = L"Searches";
		const wchar_t* sz_cVIDEOS = L"Videos";
	} COMMON_FOLDER_NAMES;
	const struct {
		const unsigned int n_c3DOBJECTS_ICON_INDEX = 187;
		const unsigned int n_cCONTACTS_ICON_INDEX = 172;
		const unsigned int n_cDESCTOP_ICON_INDEX = 174;
		const unsigned int n_cDOCUMENTS_ICON_INDEX = 107;
		const unsigned int n_cDOWNLOADS_ICON_INDEX = 175;
		const unsigned int n_cFAVORITES_ICON_INDEX = 110;
		const unsigned int n_cLINKS_ICON_INDEX = 176;
		const unsigned int n_cMUSIC_ICON_INDEX = 103;
		const unsigned int n_cPICTURES_ICON_INDEX = 108;
		const unsigned int n_cSAVEDGAMES_ICON_INDEX = 177;
		const unsigned int n_cSEARCHES_ICON_INDEX = 13;
		const unsigned int n_cVIDEOS_ICON_INDEX = 178;
	} COMMON_FOLDER_ICON_INDICES;
	const struct {
		const unsigned int n_cDISKETTE_DRIVE_ICON_INDEX = 6;
		const unsigned int n_cREMOVABLE_DRIVE_ICON_INDEX = 7;
		const unsigned int n_cCDROM_DRIVE_ICON_INDEX = 11;
		const unsigned int n_cFIXED_DRIVE_ICON_INDEX = 8;
	} DRIVE_ICON_INDICES;
	const enum DRIVE_TYPE {
		n_cNONE = 0,
		n_cDISKETTE_DRIVE,
		n_cREMOVABLE_DRIVE,
		n_cCDROM_DRIVE,
		n_cFIXED_DRIVE,
	};
	const unsigned int n_cCOMMON_FOLDER_COUNT = 12;
	const wchar_t* sz_cCOMMON_FOLDERS_ICON_DLL = L"imageres.dll";
	const wchar_t* sz_cSHARES_ICON_DLL = L"shell32.dll";

	HRESULT _AddDrive(unsigned int  driveIndex);
	bool _ShouldShowDrive(unsigned int driveIndex);
	int _GetDriveIconIndex(DRIVE_TYPE nDriveType);
	DRIVE_TYPE _GetDriveType(wchar_t* szDriveType, wchar_t* szDriveName);
	HRESULT _AddUserFolder(const wchar_t* szFolderName, unsigned int uiIconIndex);
	bool _DriveAccess(int driveIndex);

	wstring m_strModuleName;
	wchar_t* m_szServerName;
	wchar_t* m_szServerAlias;
	NETWORK_SHARES m_networkShares;
	NETWORK_SHARES_VISIBILITY m_networkSharesVisibility;
};