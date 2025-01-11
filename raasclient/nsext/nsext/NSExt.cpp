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
/*
	Copyright (c) Elbitin, modifications by Elbitin

	This file includes code from Microsofts ExplorerDataProvider but have
	been modified by Elbitin, modifications are copyrighted by Elbitin but
	included is copyright notices from ExplorerDataProvider project.

	ExplorerDataProvider source code can be found at:
	https://github.com/Microsoft/Windows-classic-samples

	*/

#include "Log.h"
#include "ElevationUtils.h"
#include "RAASClientProgramHelper.h"
#include "RootFolderView.h"

using namespace std;

static bool bConnectSharesStartedOnce = false;
static bool bConnectSharesAdminStartedOnce = false;

HRESULT NSExt_Initialize(REFIID riid, void **ppv)
{
	Log::Debug(L"NSExt_Initialize(REFIID riid, void **ppv)");
	if(!bConnectSharesStartedOnce)
		if (!IsElevated())
		{
			CRAASClientProgramHelper::StartConnectShares();
			bConnectSharesStartedOnce = true;
		}
	if (!bConnectSharesAdminStartedOnce)
		if (IsElevated())
		{
			CRAASClientProgramHelper::StartConnectSharesAdmin();
			bConnectSharesAdminStartedOnce = true;
		}
    *ppv = NULL;
    CBaseFolderView* pRootFolderViewShellFolder = new (std::nothrow) CRootFolderView();
    HRESULT hr = pRootFolderViewShellFolder ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = pRootFolderViewShellFolder->QueryInterface(riid, ppv);
        pRootFolderViewShellFolder->Release();
    }
    return hr;
}

