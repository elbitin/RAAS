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
// Copyright (c) Elbitin

#include "stdafx.h"
#include <windows.h>
#include <guiddef.h>
#include "ClassFactory.h"

// Define guid
// {1D5F87BE-0BC0-4842-A82C-12ECF6087056}
const CLSID CLSID_cmenuh = { 0x1d5f87be, 0xbc0, 0x4842, { 0xa8, 0x2c, 0x12, 0xec, 0xf6, 0x8, 0x70, 0x56 } };

HINSTANCE   g_hInst = NULL;
UINT        g_DllRefCount = 0;

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		g_hInst = hModule;
		DisableThreadLibraryCalls(hModule);
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

STDAPI DllCanUnloadNow()
{
	return (g_DllRefCount > 0) ? S_FALSE : S_OK;
}

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID *ppReturn)
{
	*ppReturn = NULL;
	if (!IsEqualCLSID(rclsid, CLSID_cmenuh))
		return CLASS_E_CLASSNOTAVAILABLE;
	CClassFactory *pClassFactory = new CClassFactory();
	if (pClassFactory == NULL)
		return E_OUTOFMEMORY;
	HRESULT hResult = pClassFactory->QueryInterface(riid, ppReturn);
	pClassFactory->Release();
	return hResult;
}