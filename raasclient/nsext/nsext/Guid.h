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

// This file contains the CLSID and Property Keys used in this sample.
#define INITGUID
#include <guiddef.h>
#include <propkeydef.h>



// {7223EA74-0FAC-4ED0-B347-C01FD2D0FE1A}
DEFINE_GUID(CLSID_FolderViewImpl, 0x7223ea74, 0xfac, 0x4ed0, 0xb3, 0x47, 0xc0, 0x1f, 0xd2, 0xd0, 0xfe, 0x1a);

// {55277D42-0F81-4F67-B0CA-11FC97062AE8}
DEFINE_GUID(CLSID_ServerContextMenu, 0x55277d42, 0xf81, 0x4f67, 0xb0, 0xca, 0x11, 0xfc, 0x97, 0x6, 0x2a, 0xe8);

// {0C106E1A-5D44-41D4-8234-064AC3FE103B}
DEFINE_GUID(CLSID_FolderViewContextMenuRealFolderBackground, 0xc106e1a, 0x5d44, 0x41d4, 0x82, 0x34, 0x6, 0x4a, 0xc3, 0xfe, 0x10, 0x3b);
