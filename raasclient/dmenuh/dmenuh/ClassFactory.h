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
#include <Unknwn.h>

#if !defined(AFX_CLASSFACTORY_H__3B4B8B30_5B16_4B69_ACC8_25C7259A9F74__INCLUDED_)
#define AFX_CLASSFACTORY_H__3B4B8B30_5B16_4B69_ACC8_25C7259A9F74__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

extern HINSTANCE  g_hInst;
extern UINT       g_DllRefCount;

class CClassFactory : public IClassFactory  
{
protected:
	DWORD m_ObjRefCount;
public:
	CClassFactory();
	virtual ~CClassFactory();

   //IUnknown methods
   STDMETHODIMP QueryInterface(REFIID, LPVOID*);
   STDMETHODIMP_(DWORD) AddRef();
   STDMETHODIMP_(DWORD) Release();

   //IClassFactory methods
   STDMETHODIMP CreateInstance(LPUNKNOWN, REFIID, LPVOID*);
   STDMETHODIMP LockServer(BOOL) { return E_NOTIMPL; };
};

#endif // !defined(AFX_CLASSFACTORY_H__3B4B8B30_5B16_4B69_ACC8_25C7259A9F74__INCLUDED_)
