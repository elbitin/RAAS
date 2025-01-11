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
#include <memory>
#include <vector>
#include <string>

using namespace std;

const int n_cMAX_DRIVE_SHARES = 25; // A-Z

struct DRIVE_SHARE {
	wstring strName;
	wstring strType;
	wstring strPath;
};

struct NETWORK_SHARES {
	wstring strProfilePath;
	vector<std::shared_ptr<DRIVE_SHARE>> vDriveShares;
};

struct NETWORK_SHARES_VISIBILITY {
	bool bActive;
	bool bThreeDObjects;
	bool bContacts;
	bool bDesktop;
	bool bDocuments;
	bool bDownloads;
	bool bFavorites;
	bool bLinks;
	bool bMusic;
	bool bPictures;
	bool bSavedGames;
	bool bSearches;
	bool bVideos;
	bool bDisketteDrives;
	bool bFixedHardDrives;
	bool bCDDrives;
	bool bRemovableDrives;
};

HRESULT GetServerStringsFromXmlFile(wchar_t* szServersFile, vector<wstring> &vServerNames, vector<wstring> &vServerAliases, unsigned int *pnServerCount);
HRESULT GetServerVisibilityFromXmlFile(wchar_t* shareFile, NETWORK_SHARES_VISIBILITY* networkShareVisibility);
HRESULT GetSharesFromXmlFile(wchar_t* shareFile, NETWORK_SHARES* networkShares);