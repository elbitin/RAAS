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
#include <windows.h>
#include <shlobj.h>
#include "stdafx.h"
#include "ClassFactory.h"
#include "ShellExt.h"

CClassFactory::CClassFactory()
{
	m_ObjRefCount = 1;
	g_DllRefCount++;
}

CClassFactory::~CClassFactory()
{
	g_DllRefCount--;
}

STDMETHODIMP CClassFactory::QueryInterface( REFIID riid, LPVOID *ppReturn )
{
	*ppReturn = NULL;
	if( IsEqualIID(riid, IID_IUnknown) )
		*ppReturn = this;
	else 
		if( IsEqualIID(riid, IID_IClassFactory) )
			*ppReturn = (IClassFactory*)this;
	if( *ppReturn )
	{
		LPUNKNOWN pUnk = (LPUNKNOWN)(*ppReturn);
		pUnk->AddRef();
		return S_OK;
	}
	return E_NOINTERFACE;
}

STDMETHODIMP_(DWORD) CClassFactory::AddRef()
{
	return ++m_ObjRefCount;
}

STDMETHODIMP_(DWORD) CClassFactory::Release()
{
	if(--m_ObjRefCount == 0)
	{
		delete this;
		return 0;
	}
	return m_ObjRefCount;
}

STDMETHODIMP CClassFactory::CreateInstance( LPUNKNOWN pUnknown, REFIID riid, LPVOID *ppObject )
{
	*ppObject = NULL;
	if( pUnknown != NULL )
		return CLASS_E_NOAGGREGATION;
	CShellExt *pShellExt = new CShellExt();
	if( NULL==pShellExt ) 
		return E_OUTOFMEMORY;
	HRESULT hResult = pShellExt->QueryInterface(riid, ppObject);
	pShellExt->Release();
	return hResult;
}
