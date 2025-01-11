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

#pragma once

#include <windows.h>

class CRegisterExtension
{
public:
    CRegisterExtension(REFCLSID clsid = CLSID_NULL, HKEY hkeyRoot = HKEY_CURRENT_USER);
    ~CRegisterExtension();
    void SetHandlerCLSID(REFCLSID clsid);
    void SetInstallScope(HKEY hkeyRoot);
    HRESULT SetModule(PCWSTR pszModule);
    HRESULT SetModule(HINSTANCE hinst);
    HRESULT RegisterInProcServer(PCWSTR pszFriendlyName, PCWSTR pszThreadingModel) const;
    HRESULT RegisterInProcServerAttribute(PCWSTR pszAttribute, DWORD dwValue) const;
    HRESULT RegisterAppAsLocalServer(PCWSTR pszFriendlyName, PCWSTR pszCmdLine = NULL) const;
    HRESULT RegisterElevatableLocalServer(PCWSTR pszFriendlyName, UINT idLocalizeString, UINT idIconRef) const;
    HRESULT RegisterElevatableInProcServer(PCWSTR pszFriendlyName, UINT idLocalizeString, UINT idIconRef) const;
    HRESULT UnRegisterObject() const;
    HRESULT RegisterAppDropTarget() const;
    HRESULT RegisterCreateProcessVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszCmdLine, PCWSTR pszVerbDisplayName) const;
    HRESULT RegisterDropTargetVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const;
    HRESULT RegisterExecuteCommandVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const;
    HRESULT RegisterExplorerCommandVerb(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszVerbDisplayName) const;
    HRESULT RegisterExplorerCommandStateHandler(PCWSTR pszProgID, PCWSTR pszVerb) const;
    HRESULT RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName) const;
    HRESULT RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName, PCWSTR pszValue) const;
    HRESULT RegisterVerbAttribute(PCWSTR pszProgID, PCWSTR pszVerb, PCWSTR pszValueName, DWORD dwValue) const;
    HRESULT RegisterVerbDefaultAndOrder(PCWSTR pszProgID, PCWSTR pszVerbOrderFirstIsDefault) const;
    HRESULT RegisterPlayerVerbs(PCWSTR const rgpszAssociation[], UINT countAssociation, PCWSTR pszVerb, PCWSTR pszTitle) const;
    HRESULT UnRegisterVerb(PCWSTR pszProgID, PCWSTR pszVerb) const;
    HRESULT UnRegisterVerbs(PCWSTR const rgpszAssociation[], UINT countAssociation, PCWSTR pszVerb) const;
    HRESULT RegisterContextMenuHandler(PCWSTR pszProgID, PCWSTR pszDescription) const;
    HRESULT RegisterRightDragContextMenuHandler(PCWSTR pszProgID, PCWSTR pszDescription) const;
    HRESULT RegisterAppShortcutInSendTo() const;
    HRESULT RegisterThumbnailHandler(PCWSTR pszExtension) const;
    HRESULT RegisterPropertyHandler(PCWSTR pszExtension) const;
    HRESULT UnRegisterPropertyHandler(PCWSTR pszExtension) const;
    HRESULT RegisterLinkHandler(PCWSTR pszProgID) const;
    HRESULT RegisterProgID(PCWSTR pszProgID, PCWSTR pszTypeName, UINT idIcon) const;
    HRESULT UnRegisterProgID(PCWSTR pszProgID, PCWSTR pszFileExtension) const;
    HRESULT RegisterExtensionWithProgID(PCWSTR pszFileExtension, PCWSTR pszProgID) const;
    HRESULT RegisterOpenWith(PCWSTR pszFileExtension, PCWSTR pszProgID) const;
    HRESULT RegisterNewMenuNullFile(PCWSTR pszFileExtension, PCWSTR pszProgID) const;
    HRESULT RegisterNewMenuData(PCWSTR pszFileExtension, PCWSTR pszProgID, PCSTR pszBase64) const;
    HRESULT RegisterKind(PCWSTR pszFileExtension, PCWSTR pszKindValue) const;
    HRESULT UnRegisterKind(PCWSTR pszFileExtension) const;
    HRESULT RegisterPropertyHandlerOverride(PCWSTR pszProperty) const;
    HRESULT RegisterHandlerSupportedProtocols(PCWSTR pszProtocol) const;
    HRESULT RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName) const;
    HRESULT RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName, PCWSTR pszValue) const;
    HRESULT RegisterProgIDValue(PCWSTR pszProgID, PCWSTR pszValueName, DWORD dwValue) const;
    HRESULT RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, PCWSTR pszValue, ...) const;
    HRESULT RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, DWORD dwValue, ...) const;
    HRESULT RegSetKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, const unsigned char pc[], DWORD dwSize, ...) const;
    HRESULT RegSetKeyValueBinaryPrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValueName, PCSTR pszBase64, ...) const;
    HRESULT RegDeleteKeyPrintf(HKEY hkey, PCWSTR pszKeyFormatString, ...) const;
    HRESULT RegDeleteKeyValuePrintf(HKEY hkey, PCWSTR pszKeyFormatString, PCWSTR pszValue, ...) const;

    PCWSTR GetCLSIDString() const { return m_szCLSID; };
    bool HasClassID() const { return m_clsid != CLSID_NULL ? true : false; };

private:

    HRESULT _EnsureModule() const;
    bool _IsBaseClassProgID(PCWSTR pszProgID)  const;
    HRESULT _EnsureBaseProgIDVerbIsNone(PCWSTR pszProgID) const;
    void _UpdateAssocChanged(HRESULT hr, PCWSTR pszKeyFormatString) const;

    CLSID m_clsid;
    HKEY m_hkeyRoot;
    WCHAR m_szCLSID[39];
    WCHAR m_szModule[MAX_PATH];
    bool m_fAssocChanged;
};
