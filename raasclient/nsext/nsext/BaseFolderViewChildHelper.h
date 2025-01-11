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
#pragma once
#include <windows.h>
#include <shlobj.h>

typedef struct
{
	USHORT  cb;
	WORD    wMyObjID;
	ULONG	ulGroupOrder;
	DWORD   dwIconIndex;
	BOOL    fIsRealPath;
	BOOL    fIsFolder;
	ULONG	ulAttributes;
	UINT16  cchModuleName;
	UINT16  cchIconPath;
	UINT16  cchGroupName;
	UINT16  cchDisplayName;
	UINT16  cchParseName;
	BYTE    data[1];
} FVITEMID;

typedef struct
{
	ULONG ulGroupOrder;
	BOOL fIsFolder;
	BOOL fIsRealPath;
	DWORD dwIconIndex;
	ULONG ulAttributes;
	WCHAR szParseName[MAX_PATH];
	WCHAR szGroupName[MAX_PATH];
	WCHAR szDisplayName[MAX_PATH];
	WCHAR szModuleName[MAX_PATH];
	WCHAR szRealPath[MAX_PATH];
	WCHAR szIconPath[MAX_PATH];
	LPITEMIDLIST pidlOriginal;
} ITEMDATA;

typedef UNALIGNED FVITEMID* PFVITEMID;
typedef const UNALIGNED FVITEMID* PCFVITEMID;

class CBaseFolderViewChildHelper
{
public:
	__declspec(dllexport) static HRESULT CreateChildID(ITEMDATA itemData, PITEMID_CHILD* ppidl);
	__declspec(dllexport) static PCFVITEMID IsValid(PCUIDLIST_RELATIVE pidl);
	__declspec(dllexport) static HRESULT GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax);
	__declspec(dllexport) static HRESULT GetModuleName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz);
	__declspec(dllexport) static HRESULT GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax);
	__declspec(dllexport) static HRESULT GetIconPath(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz);
	__declspec(dllexport) static HRESULT GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax);
	__declspec(dllexport) static HRESULT GetGroupName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz);
	__declspec(dllexport) static HRESULT GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax);
	__declspec(dllexport) static HRESULT GetDisplayName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz);
	__declspec(dllexport) static HRESULT GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR pszName, int cchMax);
	__declspec(dllexport) static HRESULT GetParseName(PCUIDLIST_RELATIVE pidl, PWSTR* ppsz);
	__declspec(dllexport) static HRESULT GetFolderness(PCUIDLIST_RELATIVE pidl, BOOL* pbIsFolder);
	__declspec(dllexport) static PIDLIST_ABSOLUTE ClonePidlOriginal(PCUIDLIST_RELATIVE pidl);
	static const int n_cMYOBJID = 0x4832;
private:
	static HRESULT _GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR pszName, int cchMax);
	static HRESULT _GetPidlString(PCUIDLIST_RELATIVE pidl, int cbDataOffset, PWSTR* ppsz, int cchMax);
	static HRESULT _GetModuleNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
	static HRESULT _GetIconPathOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
	static HRESULT _GetGroupNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
	static HRESULT _GetDisplayNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
	static HRESULT _GetParseNameOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
	static HRESULT _GetPidlOriginalOffset(PCUIDLIST_RELATIVE pidl, UINT32* pcbOffset);
};