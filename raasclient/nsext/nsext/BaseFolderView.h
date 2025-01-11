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
#include "BaseFolderViewEnumIDList.h"
#include "BaseFolderViewCB.h"

class CBaseFolderView : public IShellFolder2,
	public IPersistFolder2,
	public IExplorerPaneVisibility
{
public:
	CBaseFolderView();

	// IUnknown methods
	virtual IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv) = NULL;
	virtual IFACEMETHODIMP_(ULONG) AddRef() = NULL;
	virtual IFACEMETHODIMP_(ULONG) Release() = NULL;

	// IShellFolder
	virtual IFACEMETHODIMP ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName,
		ULONG *pchEaten, PIDLIST_RELATIVE *ppidl, ULONG *pulAttributes);
	virtual IFACEMETHODIMP EnumObjects(HWND hwnd, DWORD grfFlags, IEnumIDList **ppenumIDList);
	virtual IFACEMETHODIMP BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx *pbc, REFIID riid, void **ppv);
	virtual IFACEMETHODIMP BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx *pbc, REFIID riid, void **ppv);
	virtual IFACEMETHODIMP CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2) = NULL;
	virtual IFACEMETHODIMP CreateViewObject(HWND hwnd, REFIID riid, void **ppv);
	virtual IFACEMETHODIMP GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut);
	virtual IFACEMETHODIMP GetUIObjectOf(HWND hwnd, UINT cidl, PCUITEMID_CHILD_ARRAY apidl,
		REFIID riid, UINT* prgfInOut, void **ppv);
	virtual IFACEMETHODIMP GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName);
	virtual IFACEMETHODIMP SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut);

	// IShellFolder2
	virtual IFACEMETHODIMP GetDefaultSearchGUID(GUID *pGuid);
	virtual IFACEMETHODIMP EnumSearches(IEnumExtraSearch **ppenum);
	virtual IFACEMETHODIMP GetDefaultColumn(DWORD dwRes, ULONG *pSort, ULONG *pDisplay);
	virtual IFACEMETHODIMP GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pbState);
	virtual IFACEMETHODIMP GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY *pkey, VARIANT *pv);
	virtual IFACEMETHODIMP GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS *pDetails);
	virtual IFACEMETHODIMP MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey);

	// IPersist
	IFACEMETHODIMP GetClassID(CLSID *pClassID);

	// IPersistFolder
	virtual IFACEMETHODIMP Initialize(PCIDLIST_ABSOLUTE pidl);

	// IPersistFolder2
	IFACEMETHODIMP GetCurFolder(PIDLIST_ABSOLUTE *ppidl);

	static const PROPERTYKEY pkey_cItemGroup;

	// IExplorerPaneVisibility
	IFACEMETHODIMP GetPaneState(REFEXPLORERPANE ep, EXPLORERPANESTATE* peps);
protected:
	~CBaseFolderView();

	virtual CBaseFolderViewEnumIDList* CreateEnumIDList(DWORD grfFlags);
	virtual CBaseFolderView* CreateChildFolderView() = NULL;
	virtual HRESULT CreateFolderViewCB(REFIID riid, void** ppv);
	virtual HRESULT GetColumnDisplayName(PCUITEMID_CHILD pidl, const PROPERTYKEY* pkey, VARIANT* pv, WCHAR* pszRet, UINT cch);
	HRESULT GetPidlByName(LPITEMIDLIST * ppidl, WCHAR* szNameComponent);
	HRESULT GetDetailsOf(__in IPropertyDescription *ppropdesc, __out SHELLDETAILS *psd);
	HRESULT GetParentRealFolder(LPCITEMIDLIST pidl, IShellFolder** ppFolder);
	HRESULT CBaseFolderView::ILCompareRelIDs(IShellFolder *psfParent, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2, LPARAM lParam);

	const int           cx_cCHAR_NAME = 34;
	const int           cx_cCHAR_GROUP = 20;
	PIDLIST_ABSOLUTE    m_pidl = NULL;
	ULONG               m_ulAttributes;
	long                m_cRef;
};
