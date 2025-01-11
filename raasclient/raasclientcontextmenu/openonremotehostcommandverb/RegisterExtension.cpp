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
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "RegisterExtension.h"
#include <shlobj.h>
#include <shlwapi.h>
#include <strsafe.h>
#include <shobjidl.h>

#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "shlwapi.lib")     // link to this

__inline HRESULT ResultFromKnownLastError() { const DWORD err = GetLastError(); return err == ERROR_SUCCESS ? E_FAIL : HRESULT_FROM_WIN32(err); }

// retrieve the HINSTANCE for the current DLL or EXE using this symbol that
// the linker provides for every module, avoids the need for a global HINSTANCE variable
// and provides access to this value for static libraries
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
__inline HINSTANCE GetModuleHINSTANCE() { return (HINSTANCE)&__ImageBase; }

CRegisterExtension::CRegisterExtension(REFCLSID clsid /* = CLSID_NULL */, HKEY hkeyRoot /* = HKEY_CURRENT_USER */) : m_hkeyRoot(hkeyRoot), m_fAssocChanged(false)
{
    SetHandlerCLSID(clsid);
    GetModuleFileName(GetModuleHINSTANCE(), m_szModule, ARRAYSIZE(m_szModule));
}

CRegisterExtension::~CRegisterExtension()
{
    if (m_fAssocChanged)
    {
        // inform Explorer, et al that file association data has changed
        SHChangeNotify(SHCNE_ASSOCCHANGED, 0, 0, 0);
    }
}

void CRegisterExtension::SetHandlerCLSID(REFCLSID clsid)
{
    m_clsid = clsid;
    StringFromGUID2(m_clsid, m_szCLSID, ARRAYSIZE(m_szCLSID));
}

void CRegisterExtension::SetInstallScope(HKEY hkeyRoot)
{
    // must be HKEY_CURRENT_USER or HKEY_LOCAL_MACHINE
    m_hkeyRoot = hkeyRoot;
}

HRESULT CRegisterExtension::SetModule(PCWSTR pszModule)
{
    return StringCchCopy(m_szModule, ARRAYSIZE(m_szModule), pszModule);
}

HRESULT CRegisterExtension::SetModule(HINSTANCE hinst)
{
    return GetModuleFileName(hinst, m_szModule, ARRAYSIZE(m_szModule)) ? S_OK : E_FAIL;
}

HRESULT CRegisterExtension::_EnsureModule() const
{
    return m_szModule[0] ? S_OK : E_FAIL;
}

// this sample registers its objects in the per user registry to avoid having
// to elevate

// register the COM local server for the current running module
// this is for self registering applications

HRESULT CRegisterExtension::RegisterAppAsLocalServer(PCWSTR pszFriendlyName, PCWSTR pszCmdLine) const
{
    HRESULT hr = _EnsureModule();
    if (SUCCEEDED(hr))
    {
        WCHAR szCmdLine[MAX_PATH + 20];
        if (pszCmdLine)
        {
            StringCchPrintf(szCmdLine, ARRAYSIZE(szCmdLine), L"%s %s", m_szModule, pszCmdLine);
        }
        else
        {
            StringCchCopy(szCmdLine, ARRAYSIZE(szCmdLine), m_szModule);
        }

        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s\\LocalServer32", L"", szCmdLine, m_szCLSID);
        if (SUCCEEDED(hr))
        {
            hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s", L"AppId", m_szCLSID, m_szCLSID);
            if (SUCCEEDED(hr))
            {
                hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s", L"", pszFriendlyName, m_szCLSID);
                if (SUCCEEDED(hr))
                {
                    // hr = RegSetKeyValuePrintf(_hkeyRoot, L"Software\\Classes\\AppID\\%s", L"RunAs", L"Interactive User", _szCLSID);
                    hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\AppID\\%s", L"", pszFriendlyName, m_szCLSID);
                }
            }
        }
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterElevatableLocalServer(PCWSTR pszFriendlyName, UINT idLocalizeString, UINT idIconRef) const
{
    HRESULT hr = _EnsureModule();
    if (SUCCEEDED(hr))
    {
        hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s", L"", pszFriendlyName, m_szCLSID);
        if (SUCCEEDED(hr))
        {
            WCHAR szRes[MAX_PATH + 20];
            StringCchPrintf(szRes, ARRAYSIZE(szRes), L"@%s,-%d", m_szModule, idLocalizeString);
            hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s", L"LocalizedString", szRes, m_szCLSID);
            if (SUCCEEDED(hr))
            {
                hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\LocalServer32", L"", m_szModule, m_szCLSID);
                if (SUCCEEDED(hr))
                {
                    hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\Elevation", L"Enabled", 1, m_szCLSID);
                    if (SUCCEEDED(hr) && idIconRef)
                    {
                        StringCchPrintf(szRes, ARRAYSIZE(szRes), L"@%s,-%d", m_szModule, idIconRef);
                        hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\Elevation", L"IconReference", szRes, m_szCLSID);
                    }
                }
            }
        }
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterElevatableInProcServer(PCWSTR pszFriendlyName, UINT idLocalizeString, UINT idIconRef) const
{
    HRESULT hr = _EnsureModule();
    if (SUCCEEDED(hr))
    {
        hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\AppId\\%s", L"", pszFriendlyName, m_szCLSID);
        if (SUCCEEDED(hr))
        {
            const unsigned char c_rgAccessPermission[] =
                {0x01,0x00,0x04,0x80,0x60,0x00,0x00,0x00,0x70,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x14,
                 0x00,0x00,0x00,0x02,0x00,0x4c,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x14,0x00,0x03,0x00,
                 0x00,0x00,0x01,0x01,0x00,0x00,0x00,0x00,0x00,0x05,0x12,0x00,0x00,0x00,0x00,0x00,0x14,
                 0x00,0x07,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x00,0x00,0x00,0x05,0x0a,0x00,0x00,0x00,
                 0x00,0x00,0x14,0x00,0x03,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x00,0x00,0x00,0x05,0x04,
                 0x00,0x00,0x00,0xcd,0xcd,0xcd,0xcd,0xcd,0xcd,0xcd,0xcd,0x01,0x02,0x00,0x00,0x00,0x00,
                 0x00,0x05,0x20,0x00,0x00,0x00,0x20,0x02,0x00,0x00,0x01,0x02,0x00,0x00,0x00,0x00,0x00,
                 0x05,0x20,0x00,0x00,0x00,0x20,0x02,0x00,0x00};
            // shell32\shell32.man uses this for InProcServer32 cases
            // 010004805800000068000000000000001400000002004400030000000000140003000000010100000000000504000000000014000700000001010000000000050a00000000001400030000000101000000000005120000000102000000000005200000002002000001020000000000052000000020020000
            hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\AppId\\%s", L"AccessPermission", c_rgAccessPermission, sizeof(c_rgAccessPermission), m_szCLSID);

            const unsigned char c_rgLaunchPermission[] =
                {0x01,0x00,0x04,0x80,0x78,0x00,0x00,0x00,0x88,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x14,
                 0x00,0x00,0x00,0x02,0x00,0x64,0x00,0x04,0x00,0x00,0x00,0x00,0x00,0x14,0x00,0x1f,0x00,
                 0x00,0x00,0x01,0x01,0x00,0x00,0x00,0x00,0x00,0x05,0x12,0x00,0x00,0x00,0x00,0x00,0x18,
                 0x00,0x1f,0x00,0x00,0x00,0x01,0x02,0x00,0x00,0x00,0x00,0x00,0x05,0x20,0x00,0x00,0x00,
                 0x20,0x02,0x00,0x00,0x00,0x00,0x14,0x00,0x1f,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x00,
                 0x00,0x00,0x05,0x04,0x00,0x00,0x00,0x00,0x00,0x14,0x00,0x0b,0x00,0x00,0x00,0x01,0x01,
                 0x00,0x00,0x00,0x00,0x00,0x05,0x12,0x00,0x00,0x00,0xcd,0xcd,0xcd,0xcd,0xcd,0xcd,0xcd,
                 0xcd,0x01,0x02,0x00,0x00,0x00,0x00,0x00,0x05,0x20,0x00,0x00,0x00,0x20,0x02,0x00,0x00,
                 0x01,0x02,0x00,0x00,0x00,0x00,0x00,0x05,0x20,0x00,0x00,0x00,0x20,0x02,0x00,0x00};
            hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\AppId\\%s", L"LaunchPermission", c_rgLaunchPermission, sizeof(c_rgLaunchPermission), m_szCLSID);

            hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s", L"", pszFriendlyName, m_szCLSID);
            if (SUCCEEDED(hr))
            {
                hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s", L"AppId", m_szCLSID, m_szCLSID);
                if (SUCCEEDED(hr))
                {
                    WCHAR szRes[MAX_PATH + 20];
                    StringCchPrintf(szRes, ARRAYSIZE(szRes), L"@%s,-%d", m_szModule, idLocalizeString);
                    hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s", L"LocalizedString", szRes, m_szCLSID);
                    if (SUCCEEDED(hr))
                    {
                        hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\InProcServer32", L"", m_szModule, m_szCLSID);
                        if (SUCCEEDED(hr))
                        {
                            hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\Elevation", L"Enabled", 1, m_szCLSID);
                            if (SUCCEEDED(hr) && idIconRef)
                            {
                                StringCchPrintf(szRes, ARRAYSIZE(szRes), L"@%s,-%d", m_szModule, idIconRef);
                                hr = RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Classes\\CLSID\\%s\\Elevation", L"IconReference", szRes, m_szCLSID);
                            }
                        }
                    }
                }
            }
        }
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterInProcServer(PCWSTR pszFriendlyName, PCWSTR pszThreadingModel) const
{
    HRESULT hr = _EnsureModule();
    if (SUCCEEDED(hr))
    {
        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s", L"", pszFriendlyName, m_szCLSID);
        if (SUCCEEDED(hr))
        {
            hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s\\InProcServer32", L"", m_szModule, m_szCLSID);
            if (SUCCEEDED(hr))
            {
                hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s\\InProcServer32", L"ThreadingModel", pszThreadingModel, m_szCLSID);
            }
        }
    }
    return hr;
}

// use for
// ManualSafeSave = REG_DWORD:<1>
// EnableShareDenyNone = REG_DWORD:<1>
// EnableShareDenyWrite = REG_DWORD:<1>

HRESULT CRegisterExtension::RegisterInProcServerAttribute(PCWSTR pszAttribute, DWORD dwValue) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s", pszAttribute, dwValue, m_szCLSID);
}

HRESULT CRegisterExtension::UnRegisterObject() const
{
    // might have an AppID value, try that
    HRESULT hr = RegDeleteKeyPrintf(m_hkeyRoot, L"Software\\Classes\\AppID\\%s", m_szCLSID);
    if (SUCCEEDED(hr))
    {
        hr = RegDeleteKeyPrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s", m_szCLSID);
    }
    return hr;
}

//
// pszProtocol values:
// "*" - all
// "http"
// "ftp"
// "shellstream" - NYI in Win7

HRESULT CRegisterExtension::RegisterHandlerSupportedProtocols(PCWSTR pszProtocol) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s\\SupportedProtocols", pszProtocol, L"", m_szCLSID);
}

// this enables drag drop directly onto the .exe, useful if you have a
// shortcut to the exe somewhere (or the .exe is accessable via the send to menu)

HRESULT CRegisterExtension::RegisterAppDropTarget() const
{
    HRESULT hr = _EnsureModule();
    if (SUCCEEDED(hr))
    {
        // Windows7 supports per user App Paths, downlevel requires HKLM
        hr = RegSetKeyValuePrintf(m_hkeyRoot,
            L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\%s",
            L"DropTarget", m_szCLSID, PathFindFileName(m_szModule));
    }
    return hr;
}

// work around the missing "NeverDefault" feature for verbs on downlevel platforms
// these ProgID values should need special treatment to keep the verbs registered there
// from becoming default

bool CRegisterExtension::_IsBaseClassProgID(PCWSTR pszProgID) const
{
    return !StrCmpIC(pszProgID, L"AllFileSystemObjects") ||
           !StrCmpIC(pszProgID, L"Directory") ||
           !StrCmpIC(pszProgID, L"*") ||
           StrStrI(pszProgID, L"SystemFileAssociations\\Directory.");   // SystemFileAssociations\Directory.* values
}

HRESULT CRegisterExtension::_EnsureBaseProgIDVerbIsNone(PCWSTR pszProgID) const
{
    // putting the value of "none" that does not match any of the verbs under this key
    // avoids those verbs from becoming the default.
    return _IsBaseClassProgID(pszProgID) ?
        RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell", L"", L"none", pszProgID) :
        S_OK;
}

HRESULT CRegisterExtension::RegisterCreateProcessVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszCmdLine, PCWSTR pszVerbDisplayName) const
{
    UnRegisterVerb(pszProgID, pszVerb); // make sure no existing registration exists, ignore failure

    HRESULT hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shell\\%s\\command", L"", pszCmdLine, pszProgID, pszVerb);
    if (SUCCEEDED(hr))
    {
        hr = _EnsureBaseProgIDVerbIsNone(pszProgID);

        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shell\\%s", L"", pszVerbDisplayName, pszProgID, pszVerb);
    }
    return hr;
}

// create registry entries for drop target based static verb.
// the specified clsid will be

HRESULT CRegisterExtension::RegisterDropTargetVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const
{
    UnRegisterVerb(pszProgID, pszVerb); // make sure no existing registration exists, ignore failure

    HRESULT hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s\\DropTarget",
        L"CLSID", m_szCLSID, pszProgID, pszVerb);
    if (SUCCEEDED(hr))
    {
        hr = _EnsureBaseProgIDVerbIsNone(pszProgID);

        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s",
            L"", pszVerbDisplayName, pszProgID, pszVerb);
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterExecuteCommandVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const
{
    UnRegisterVerb(pszProgID, pszVerb); // make sure no existing registration exists, ignore failure

    HRESULT hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s\\command",
        L"DelegateExecute", m_szCLSID, pszProgID, pszVerb);
    if (SUCCEEDED(hr))
    {
        hr = _EnsureBaseProgIDVerbIsNone(pszProgID);

        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s",
            L"", pszVerbDisplayName, pszProgID, pszVerb);
    }
    return hr;
}

// must be an inproc handler registered here

HRESULT CRegisterExtension::RegisterExplorerCommandVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const
{
    UnRegisterVerb(pszProgID, pszVerb); // make sure no existing registration exists, ignore failure

    HRESULT hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s",
        L"ExplorerCommandHandler", m_szCLSID, pszProgID, pszVerb);
    if (SUCCEEDED(hr))
    {
        hr = _EnsureBaseProgIDVerbIsNone(pszProgID);

        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s",
            L"", pszVerbDisplayName, pszProgID, pszVerb);
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterExplorerCommandStateHandler(PCWSTR pszProgID, PCWSTR pszVerb) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s",
        L"CommandStateHandler", m_szCLSID, pszProgID, pszVerb);
}

HRESULT CRegisterExtension::UnRegisterVerb(PCWSTR pszProgID, PCWSTR pszVerb) const
{
    return RegDeleteKeyPrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell\\%s", pszProgID, pszVerb);
}

HRESULT CRegisterExtension::UnRegisterVerbs(PCWSTR const rgpszAssociation[], UINT countAssociation, PCWSTR pszVerb) const
{
    HRESULT hr = S_OK;
    for (UINT i = 0; SUCCEEDED(hr) && (i < countAssociation); i++)
    {
        hr = UnRegisterVerb(rgpszAssociation[i], pszVerb);
    }

    if (SUCCEEDED(hr) && HasClassID())
    {
        hr = UnRegisterObject();
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterThumbnailHandler(PCWSTR pszExtension) const
{
    // IThumbnailHandler
    // HKEY_CLASSES_ROOT\.wma\ShellEx\{e357fccd-a995-4576-b01f-234630154e96}={9DBD2C50-62AD-11D0-B806-00C04FD706EC}

    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\ShellEx\\{e357fccd-a995-4576-b01f-234630154e96}",
        L"", m_szCLSID, pszExtension);
}

// in process context menu handler for right drag context menu
// need to create new method that allows out of proc handling of this

// pszProgID "Folder" or "Directory"
HRESULT CRegisterExtension::RegisterRightDragContextMenuHandler(PCWSTR pszProgID, PCWSTR pszDescription) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shellex\\DragDropHandlers\\%s",
        L"", pszDescription, pszProgID, m_szCLSID);
}

// in process context menu handler

HRESULT CRegisterExtension::RegisterContextMenuHandler(PCWSTR pszProgID, PCWSTR pszDescription) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shellex\\ContextMenuHandlers\\%s",
        L"", pszDescription, pszProgID, m_szCLSID);
}

HRESULT CRegisterExtension::RegisterPropertyHandler(PCWSTR pszExtension) const
{
    // IPropertyHandler
    // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers\.docx={993BE281-6695-4BA5-8A2A-7AACBFAAB69E}

    return RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\PropertySystem\\PropertyHandlers\\%s",
        L"", m_szCLSID, pszExtension);
}

HRESULT CRegisterExtension::UnRegisterPropertyHandler(PCWSTR pszExtension) const
{
    return RegDeleteKeyPrintf(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\PropertySystem\\PropertyHandlers\\%s", pszExtension);
}

// IResolveShellLink handler, used for custom link resolution behavior
HRESULT CRegisterExtension::RegisterLinkHandler(PCWSTR pszProgID) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\ShellEx\\LinkHandler", L"", m_szCLSID, pszProgID);
}

// HKCR\<ProgID> = <Type Name>
//      DefaultIcon=<icon ref>
//      <icon ref>=<module path>,<res_id>

HRESULT CRegisterExtension::RegisterProgID(PCWSTR pszProgID, PCWSTR pszTypeName, UINT idIcon) const
{
    HRESULT hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s", L"", pszTypeName, pszProgID);
    if (SUCCEEDED(hr))
    {
        if (idIcon)
        {
            WCHAR szIconRef[MAX_PATH];
            StringCchPrintf(szIconRef, ARRAYSIZE(szIconRef), L"\"%s\",-%d", m_szModule, idIcon);
            // HKCR\<ProgID>\DefaultIcon
            hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\DefaultIcon", L"", szIconRef, pszProgID);
        }
    }
    return hr;
}

HRESULT CRegisterExtension::UnRegisterProgID(PCWSTR pszProgID, PCWSTR pszFileExtension) const
{
    HRESULT hr = RegDeleteKeyPrintf(m_hkeyRoot, L"Software\\Classes\\%s", pszProgID);
    if (SUCCEEDED(hr) && pszFileExtension)
    {
        hr = RegDeleteKeyPrintf(m_hkeyRoot, L"Software\\Classes\\%s\\%s", pszFileExtension, pszProgID);
    }
    return hr;
}

// value names that do not require a value
// HKCR\<ProgID>
//      NoOpen - display the "No Open" dialog for this file to disable double click
//      IsShortcut - report SFGAO_LINK for this item type, should have a IShellLink handler
//      NeverShowExt - never show the file extension
//      AlwaysShowExt - always show the file extension
//      NoPreviousVersions - don't display the "Previous Versions" verb for this file type

HRESULT CRegisterExtension::RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s", pszValueName, L"", pszProgID);
}

// value names that require a string value
// HKCR\<ProgID>
//      NoOpen - display the "No Open" dialog for this file to disable double click, display this message
//      FriendlyTypeName - localized resource
//      ConflictPrompt
//      FullDetails
//      InfoTip
//      QuickTip
//      PreviewDetails
//      PreviewTitle
//      TileInfo
//      ExtendedTileInfo
//      SetDefaultsFor - right click.new will populate the file with these properties, example: "prop:System.Author;System.Document.DateCreated"

HRESULT CRegisterExtension::RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName, PCWSTR pszValue) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s", pszValueName, pszValue, pszProgID);
}

// value names that require a DWORD value
// HKCR\<ProgID>
//      EditFlags
//      ThumbnailCutoff

HRESULT CRegisterExtension::RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName, DWORD dwValue) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s", pszValueName, dwValue, pszProgID);
}

// NeverDefault
// LegacyDisable
// Extended
// OnlyInBrowserWindow
// ProgrammaticAccessOnly
// SeparatorBefore
// SeparatorAfter
// CheckSupportedTypes, used SupportedTypes that is a file type filter registered under AppPaths (I think)
HRESULT CRegisterExtension::RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shell\\%s", pszValueName, L"", pszProgID, pszVerb);
}

// MUIVerb=@dll,-resid
// MultiSelectModel=Single|Player|Document
// Position=Bottom|Top
// DefaultAppliesTo=System.ItemName:"foo"
// HasLUAShield=System.ItemName:"bar"
// AppliesTo=System.ItemName:"foo"
HRESULT CRegisterExtension::RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName, PCWSTR pszValue) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shell\\%s", pszValueName, pszValue, pszProgID, pszVerb);
}

// BrowserFlags
// ExplorerFlags
// AttributeMask
// AttributeValue
// ImpliedSelectionModel
// SuppressionPolicy
HRESULT CRegisterExtension::RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName, DWORD dwValue) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\shell\\%s", pszValueName, dwValue, pszProgID, pszVerb);
}

// "open explorer" is an example
HRESULT CRegisterExtension::RegisterVerbDefaultAndOrder(PCWSTR pszProgID, PCWSTR pszVerbOrderFirstIsDefault) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\Shell", L"", pszVerbOrderFirstIsDefault, pszProgID);
}

// register a verb on an array of ProgIDs

HRESULT CRegisterExtension::RegisterPlayerVerbs(PCWSTR const rgpszAssociation[], UINT countAssociation,
                                                PCWSTR pszVerb, PCWSTR pszTitle) const
{
    HRESULT hr = RegisterAppAsLocalServer(pszTitle, NULL);
    if (SUCCEEDED(hr))
    {
        // enable this handler to work with OpenSearch results, avoiding the downlaod
        // and open behavior by indicating that we can accept all URL forms
        hr = RegisterHandlerSupportedProtocols(L"*");

        for (UINT i = 0; SUCCEEDED(hr) && (i < countAssociation); i++)
        {
            hr = RegisterExecuteCommandVerb(rgpszAssociation[i], pszVerb, pszTitle);
            if (SUCCEEDED(hr))
            {
                hr = RegisterVerbAttribute(rgpszAssociation[i], pszVerb, L"NeverDefault");
                if (SUCCEEDED(hr))
                {
                    hr = RegisterVerbAttribute(rgpszAssociation[i], pszVerb, L"MultiSelectModel", L"Player");
                }
            }
        }
    }
    return hr;
}

// this is where the file assocation is being taken over

HRESULT CRegisterExtension::RegisterExtensionWithProgID(PCWSTR pszFileExtension, PCWSTR pszProgID) const
{
    // HKCR\<.ext>=<ProgID>
    // "Content Type"
    // "PerceivedType"

    // TODO: to be polite if there is an existing mapping of extension to ProgID make sure it is
    // added to the OpenWith list so that users can get back to the old app using OpenWith
    // TODO: verify that HKLM/HKCU settings do not already exist as if they do they will
    // get in the way of the setting being made here
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s", L"", pszProgID, pszFileExtension);
}

// adds the ProgID to a file extension assuming that this ProgID will have
// the "open" verb under it that will be used in Open With

HRESULT CRegisterExtension::RegisterOpenWith(PCWSTR pszFileExtension, PCWSTR pszProgID) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\OpenWithProgIds", pszProgID, L"", pszFileExtension);
}

HRESULT CRegisterExtension::RegisterNewMenuNullFile(PCWSTR pszFileExtension, PCWSTR pszProgID) const
{
    // there are 2 forms of this
    // HKCR\<.ext>\ShellNew
    // HKCR\<.ext>\ShellNew\<ProgID> - only
    // ItemName
    // NullFile
    // Data - REG_BINARY:<binary data>
    // File
    // command
    // iconpath

    // another way that this works
    // HKEY_CLASSES_ROOT\.doc\Word.Document.8\ShellNew
    HRESULT hr;
    if (pszProgID)
    {
        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\%s\\ShellNew", L"NullFile", L"", pszFileExtension, pszProgID);
    }
    else
    {
        hr = RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\%s\\ShellNew", L"NullFile", L"", pszFileExtension);
    }
    return hr;
}

HRESULT CRegisterExtension::RegisterNewMenuData(PCWSTR pszFileExtension, PCWSTR pszProgID, PCSTR pszBase64) const
{
    HRESULT hr;
    if (pszProgID)
    {
        hr = RegSetKeyValueBinaryPrintf(m_hkeyRoot, L"Software\\Classes\\%s\\%s\\ShellNew", L"Data", pszBase64, pszFileExtension, pszProgID);
    }
    else
    {
        hr = RegSetKeyValueBinaryPrintf(m_hkeyRoot, L"Software\\Classes\\%s\\ShellNew", L"Data", pszBase64, pszFileExtension);
    }
    return hr;
}

// define the kind of a file extension. this is a multi-value property, see
// HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\explorer\KindMap
HRESULT CRegisterExtension::RegisterKind(PCWSTR pszFileExtension, PCWSTR pszKindValue) const
{
    return RegSetKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\KindMap", pszFileExtension, pszKindValue);
}

HRESULT CRegisterExtension::UnRegisterKind(PCWSTR pszFileExtension) const
{
    return RegDeleteKeyValuePrintf(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\KindMap", pszFileExtension);
}

// when indexing it is possible to override some of the file system property values, that includes the following
// use this registration helper to set the override flag for each
//
// System.ItemNameDisplay
// System.SFGAOFlags
// System.Kind
// System.FileName
// System.ItemPathDisplay
// System.ItemPathDisplayNarrow
// System.ItemFolderNameDisplay
// System.ItemFolderPathDisplay
// System.ItemFolderPathDisplayNarrow

HRESULT CRegisterExtension::RegisterPropertyHandlerOverride(PCWSTR pszProperty) const
{
    return RegSetKeyValuePrintf(m_hkeyRoot, L"Software\\Classes\\CLSID\\%s\\OverrideFileSystemProperties", pszProperty, 1, m_szCLSID);
}

HRESULT CRegisterExtension::RegisterAppShortcutInSendTo() const
{
    WCHAR szPath[MAX_PATH];
    HRESULT hr = GetModuleFileName(NULL, szPath, ARRAYSIZE(szPath)) ? S_OK : ResultFromKnownLastError();
    if (SUCCEEDED(hr))
    {
        //  Set the shortcut target
        IShellLink *psl;
        hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&psl));
        if (SUCCEEDED(hr))
        {
            hr = psl->SetPath(szPath);
            if (SUCCEEDED(hr))
            {
                WCHAR szName[MAX_PATH];
                StringCchCopy(szName, ARRAYSIZE(szName), PathFindFileName(szPath));
                PathRenameExtension(szName, L".lnk");

                hr = SHGetFolderPath(NULL, CSIDL_SENDTO, NULL, 0, szPath);
                if (SUCCEEDED(hr))
                {
                    hr = PathAppend(szPath, szName) ? S_OK : E_FAIL;
                    if (SUCCEEDED(hr))
                    {
                        IPersistFile *ppf;
                        hr = psl->QueryInterface(IID_PPV_ARGS(&ppf));
                        if (SUCCEEDED(hr))
                        {
                            hr = ppf->Save(szPath, TRUE);
                            ppf->Release();
                        }
                    }
                }
            }
            psl->Release();
        }
    }
    return hr;
}

HRESULT CRegisterExtension::RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, PCWSTR pszValue, ...) const
{
    va_list argList;
    va_start(argList, pszValue);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegSetKeyValueW(hkey, szKeyName, pszValueName, REG_SZ, pszValue,
            lstrlen(pszValue) * sizeof(*pszValue)));
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return hr;
}

HRESULT CRegisterExtension::RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, DWORD dwValue, ...) const
{
    va_list argList;
    va_start(argList, dwValue);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegSetKeyValueW(hkey, szKeyName, pszValueName, REG_DWORD, &dwValue, sizeof(dwValue)));
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return hr;
}

HRESULT CRegisterExtension::RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, const unsigned char pc[], DWORD dwSize, ...) const
{
    va_list argList;
    va_start(argList, pc);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegSetKeyValueW(hkey, szKeyName, pszValueName, REG_BINARY, pc, dwSize));
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return hr;
}

HRESULT CRegisterExtension::RegSetKeyValueBinaryPrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, PCSTR pszBase64, ...) const
{
    va_list argList;
    va_start(argList, pszBase64);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        DWORD dwDecodedImageSize, dwSkipChars, dwActualFormat;
        hr = CryptStringToBinaryA(pszBase64, NULL, CRYPT_STRING_BASE64, NULL,
            &dwDecodedImageSize, &dwSkipChars, &dwActualFormat) ? S_OK : E_FAIL;
        if (SUCCEEDED(hr))
        {
            BYTE *pbDecodedImage = (BYTE*)LocalAlloc(LPTR, dwDecodedImageSize);
            hr = pbDecodedImage ? S_OK : E_OUTOFMEMORY;
            if (SUCCEEDED(hr))
            {
                hr = CryptStringToBinaryA(pszBase64, lstrlenA(pszBase64), CRYPT_STRING_BASE64,
                    pbDecodedImage, &dwDecodedImageSize, &dwSkipChars, &dwActualFormat) ? S_OK : E_FAIL;
                if (SUCCEEDED(hr))
                {
                    hr = HRESULT_FROM_WIN32(RegSetKeyValueW(hkey, szKeyName, pszValueName, REG_BINARY, pbDecodedImage, dwDecodedImageSize));
                }
            }
        }
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return hr;
}

__inline HRESULT MapNotFoundToSuccess(HRESULT hr)
{
    return HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND) == hr ? S_OK : hr;
}

HRESULT CRegisterExtension::RegDeleteKeyPrintf(HKEY hkey, PCWSTR pszKeyFormatString, ...) const
{
    va_list argList;
    va_start(argList, pszKeyFormatString);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteTree(hkey, szKeyName));
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return MapNotFoundToSuccess(hr);
}

HRESULT CRegisterExtension::RegDeleteKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValue, ...) const
{
    va_list argList;
    va_start(argList, pszKeyFormatString);

    WCHAR szKeyName[512];
    HRESULT hr = StringCchVPrintf(szKeyName, ARRAYSIZE(szKeyName), pszKeyFormatString, argList);
    if (SUCCEEDED(hr))
    {
        hr = HRESULT_FROM_WIN32(RegDeleteKeyValueW(hkey, szKeyName, pszValue));
    }

    va_end(argList);

    _UpdateAssocChanged(hr, pszKeyFormatString);
    return MapNotFoundToSuccess(hr);
}

void CRegisterExtension::_UpdateAssocChanged(HRESULT hr, PCWSTR pszKeyFormatString) const
{
    static const WCHAR c_szProgIDPrefix[] = L"Software\\Classes\\%s";
    if (SUCCEEDED(hr) && !m_fAssocChanged &&
        (StrCmpNIC(pszKeyFormatString, c_szProgIDPrefix, ARRAYSIZE(c_szProgIDPrefix) - 1) == 0 ||
         StrStrI(pszKeyFormatString, L"PropertyHandlers") ||
         StrStrI(pszKeyFormatString, L"KindMap")))
    {
        const_cast<CRegisterExtension*>(this)->m_fAssocChanged = true;
    }
}
