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

/**************************************************************************
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

(c) Microsoft Corporation. All Rights Reserved.
**************************************************************************/

#include "Log.h"
#include "ContextMenuUtils.h"

const ICIVERBTOIDMAP* CmdIDToMapServer(UINT_PTR idCmd, BOOL fUnicode, const ICIVERBTOIDMAP* pmap)
{
	Log::Debug(L"CmdIDToMapServer(UINT_PTR idCmd, BOOL fUnicode, const ICIVERBTOIDMAP* pmap)");
	const ICIVERBTOIDMAP* pmapRet = NULL;
	if (IS_INTRESOURCE(idCmd))
	{
		UINT idVerb = (UINT)idCmd;
		while (!pmapRet && -1 != pmap->idCmd)
		{
			if (pmap->idCmd == idVerb)
			{
				pmapRet = pmap;
			}
			pmap++;
		}
	}
	else if (fUnicode)
	{
		LPCWSTR pszVerb = (LPCWSTR)idCmd;
		while (!pmapRet && -1 != pmap->idCmd)
		{
			if (pmap->pszCmd && 0 == StrCmpIC(pszVerb, pmap->pszCmd))
			{
				pmapRet = pmap;
			}
			pmap++;
		}
	}
	else
	{
		LPCSTR pszVerbA = (LPCSTR)idCmd;
		while (!pmapRet && -1 != pmap->idCmd)
		{
			if (pmap->pszCmdA && 0 == StrCmpICA(pszVerbA, pmap->pszCmdA))
			{
				pmapRet = pmap;
			}
			pmap++;
		}
	}
	return pmapRet;
}

HRESULT MapICIVerbToCmdIDServer(LPCMINVOKECOMMANDINFO pici, const ICIVERBTOIDMAP* pmap, UINT* pid)
{
	Log::Debug(L"MapICIVerbToCmdIDServer(LPCMINVOKECOMMANDINFO pici, const ICIVERBTOIDMAP* pmap, UINT* pid)");
	HRESULT hr = E_FAIL;
	if (!IS_INTRESOURCE(pici->lpVerb))
	{
		UINT_PTR idCmd;
		BOOL fUnicode;
		if (IS_UNICODE_ICI(pici) && ((LPCMINVOKECOMMANDINFOEX)pici)->lpVerbW)
		{
			fUnicode = TRUE;
			idCmd = (UINT_PTR)(((LPCMINVOKECOMMANDINFOEX)pici)->lpVerbW);
		}
		else
		{
			fUnicode = FALSE;
			idCmd = (UINT_PTR)(pici->lpVerb);
		}
		pmap = CmdIDToMapServer(idCmd, fUnicode, pmap);
		if (pmap)
		{
			*pid = pmap->idCmd;
			hr = S_OK;
		}
	}
	else
	{
		*pid = LOWORD((UINT_PTR)pici->lpVerb);
		hr = S_OK;
	}
	return hr;
}