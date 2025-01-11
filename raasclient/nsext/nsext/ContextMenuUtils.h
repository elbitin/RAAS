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

#pragma once
#include <windows.h>
#include <shlobj.h>
#include <shlwapi.h>

#define IS_UNICODE_ICI(pici) (((pici)->fMask & CMIC_MASK_UNICODE) == CMIC_MASK_UNICODE)

typedef struct
{
	LPCWSTR pszCmd;         // verbW
	LPCSTR  pszCmdA;        // verbA
	UINT    idCmd;          // hmenu id
	UINT    idsHelpText;    // id of help text
} ICIVERBTOIDMAP;

const ICIVERBTOIDMAP* CmdIDToMapServer(UINT_PTR idCmd, BOOL fUnicode, const ICIVERBTOIDMAP* pmap);

HRESULT MapICIVerbToCmdIDServer(LPCMINVOKECOMMANDINFO pici, const ICIVERBTOIDMAP* pmap, UINT* pid);