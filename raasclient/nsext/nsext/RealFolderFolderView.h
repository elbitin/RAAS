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
#include "BaseFolderView.h"

class CRealFolderFolderView : public CBaseFolderView, public IStorage
{
public:
	CRealFolderFolderView();

	// IUnknown methods
	IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IShellFolder
	IFACEMETHODIMP ParseDisplayName(HWND hwnd, IBindCtx *pbc, PWSTR pszName,
		ULONG *pchEaten, PIDLIST_RELATIVE *ppidl, ULONG *pulAttributes);
	IFACEMETHODIMP EnumObjects(HWND hwnd, DWORD grfFlags, IEnumIDList **ppenumIDList);
	IFACEMETHODIMP BindToObject(PCUIDLIST_RELATIVE pidl, IBindCtx *pbc, REFIID riid, void **ppv);
	IFACEMETHODIMP BindToStorage(PCUIDLIST_RELATIVE pidl, IBindCtx *pbc, REFIID riid, void **ppv);
	IFACEMETHODIMP CompareIDs(LPARAM lParam, PCUIDLIST_RELATIVE pidl1, PCUIDLIST_RELATIVE pidl2);
	IFACEMETHODIMP CreateViewObject(HWND hwnd, REFIID riid, void **ppv);
	IFACEMETHODIMP GetAttributesOf(UINT cidl, PCUITEMID_CHILD_ARRAY apidl, ULONG *rgfInOut);
	IFACEMETHODIMP GetUIObjectOf(HWND hwnd, UINT cidl, LPCITEMIDLIST* ppidl, REFIID riid, UINT* prgfInOut, void** ppv);
	IFACEMETHODIMP GetDisplayNameOf(PCUITEMID_CHILD pidl, SHGDNF shgdnFlags, STRRET *pName);
	IFACEMETHODIMP SetNameOf(HWND hwnd, PCUITEMID_CHILD pidl, PCWSTR pszName, DWORD uFlags, PITEMID_CHILD * ppidlOut);

	// IShellFolder2
	IFACEMETHODIMP GetDefaultColumn(DWORD dwRes, ULONG *pSort, ULONG *pDisplay);
	IFACEMETHODIMP GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pbState);
	IFACEMETHODIMP GetDetailsEx(PCUITEMID_CHILD pidl, const PROPERTYKEY *pkey, VARIANT *pv);
	IFACEMETHODIMP GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS *pDetails);
	IFACEMETHODIMP MapColumnToSCID(UINT iColumn, PROPERTYKEY *pkey);

	// IPersistFolder
	IFACEMETHODIMP Initialize(PCIDLIST_ABSOLUTE pidl);

	// IStorage
	STDMETHOD(CreateStream)(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStream** ppstm);
	STDMETHOD(OpenStream)(LPCWSTR pwcsName, LPVOID reserved1, DWORD grfMode, DWORD reserved2, IStream** ppstm);
	STDMETHOD(CreateStorage)(LPCWSTR pwcsName, DWORD grfMode, DWORD reserved1, DWORD reserved2, IStorage** ppstg);
	STDMETHOD(OpenStorage)(LPCWSTR pwcsName, IStorage* pstgPriority, DWORD grfMode, SNB snbExclude, DWORD reserved, IStorage** ppstg);
	STDMETHOD(CopyTo)(DWORD ciidExclude, const IID* rgiidExclude, SNB snbExclude, IStorage* pstgDest);
	STDMETHOD(MoveElementTo)(LPCWSTR pwcsName, IStorage* pstgDest, LPCWSTR pwcsNewName, DWORD grfFlags);
	STDMETHOD(Commit)(DWORD grfCommitFlags);
	STDMETHOD(Revert)();
	STDMETHOD(EnumElements)(DWORD reserved1, LPVOID reserved2, DWORD reserved3, IEnumSTATSTG** ppenum);
	STDMETHOD(DestroyElement)(LPCWSTR pwcsName);
	STDMETHOD(RenameElement)(LPCWSTR pwcsOldName, LPCWSTR pwcsNewName);
	STDMETHOD(SetElementTimes)(LPCWSTR pwcsName, const FILETIME* pctime, const FILETIME* patime, const FILETIME* pmtime);
	STDMETHOD(SetClass)(REFCLSID clsid);
	STDMETHOD(SetStateBits)(DWORD grfStateBits, DWORD grfMask);
	STDMETHOD(Stat)(STATSTG* pstatstg, DWORD grfStatFlag);

protected:
	static LPITEMIDLIST _GetRealPidl(LPITEMIDLIST pidl);

private:
	~CRealFolderFolderView();

	void _SwitchColumns(const UINT &iColumn, int &iMappedColumn);
	CBaseFolderView* CreateChildFolderView();
	HRESULT CreateFolderViewCB(REFIID riid, void** ppv);

	const struct {
		const unsigned int n_cNAME = 0;
		const unsigned int n_cLATEST_CHANGE = 3;
		const unsigned int n_cTYPE = 2;
		const unsigned int n_cSIZE = 1;
	} DEFAULT_COLUMN_ORDER;

	PIDLIST_ABSOLUTE    m_pidlOriginal = NULL;
	IShellFolder2*      m_pFolder = NULL;
	IShellFolder*       m_pDesktop = NULL;
	IStorage*			m_pStorage = NULL;
	WCHAR               m_szRealFolderPath[MAX_PATH];
};
