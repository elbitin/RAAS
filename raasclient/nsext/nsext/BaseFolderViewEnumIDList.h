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
#include <windows.h>
#include <shlobj.h>
#include <memory>
#include <vector>
#include "BaseFolderViewChildHelper.h"
#include "RAASClientXmlUtils.h"

using namespace std;

class CBaseFolderViewEnumIDList : public IEnumIDList
{
public:
	CBaseFolderViewEnumIDList(DWORD grfFlags);

	// IUnknown methods
	IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IEnumIDList
	IFACEMETHODIMP Next(ULONG celt, PITEMID_CHILD *rgelt, ULONG *pceltFetched);
	IFACEMETHODIMP Skip(DWORD celt);
	IFACEMETHODIMP Reset();
	IFACEMETHODIMP Clone(IEnumIDList **ppenum);

	virtual HRESULT Initialize();

protected:
	~CBaseFolderViewEnumIDList();

	void _GetServerModuleNames();
	HRESULT _GetServerStrings();
	HRESULT _GetServerVisibility(const wchar_t* szServerName, NETWORK_SHARES_VISIBILITY* NetworkSharesVisibility);
	HRESULT _GetShares(const wchar_t* szServerName, NETWORK_SHARES &networkShares);

	const wchar_t* sz_cPROGRAM_NAME = L"RAAS Client";
	const wchar_t* sz_cVENDOR_NAME = L"Elbitin";
	const wchar_t* sz_cSERVERS_XML_FILE_NAME = L"servers.xml";
	const wchar_t* sz_cSHARE_XML_FILE_NAME = L"share.xml";
	const wchar_t* sz_cNSEXT_VISIBILITY_XML_FILE_NAME = L"nsextvisibility.xml";
	static const int n_cMYOBJID = CBaseFolderViewChildHelper::n_cMYOBJID;

	vector<std::shared_ptr<ITEMDATA>> m_vData;
	vector<wstring> m_vServerNames;
	vector<wstring> m_vServerAliases;
	vector<wstring> m_vServerModuleNames;
	unsigned int m_cServers = 0;
private:
	long m_cRef;
	DWORD m_grfFlags;
	unsigned int m_nItem = 0;
};