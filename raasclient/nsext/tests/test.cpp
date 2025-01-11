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
#include "pch.h"
#include <strsafe.h>
#include <shlwapi.h>
#include <windows.h>
#include "BaseFolderViewChildHelper.h"

class PidlTest : public ::testing::Test {
protected:
    PidlTest() {

        pidl = NULL;
        initItemData(&itemData);
        pidlResult = CBaseFolderViewChildHelper::CreateChildID(itemData, &pidl);
        if (SUCCEEDED(pidlResult))
        {
            initItemData(&itemDataWithPidlOriginal);
            itemDataWithPidlOriginal.fIsRealPath = TRUE;
            itemDataWithPidlOriginal.pidlOriginal = pidl;
            pidlResult = CBaseFolderViewChildHelper::CreateChildID(itemDataWithPidlOriginal, &pidlWithPidlOriginal);
        }
    }

    virtual void initItemData(ITEMDATA* itemData) {
        itemData->fIsFolder = TRUE;
        itemData->fIsRealPath = FALSE;
        itemData->dwIconIndex = 10;
        itemData->ulAttributes = 256;
        itemData->ulGroupOrder = 12;
        itemData->pidlOriginal = NULL;
        StringCchCopyW(itemData->szModuleName, MAX_PATH, L"TestModuleName");
        StringCchCopyW(itemData->szGroupName, MAX_PATH, L"TestGroupName");
        StringCchCopyW(itemData->szIconPath, MAX_PATH, L"TestIconPath");
        StringCchCopyW(itemData->szParseName, MAX_PATH, L"TestParseName");
        StringCchCopyW(itemData->szDisplayName, MAX_PATH, L"TestDisplayName");
        return;
    }

    ~PidlTest() override {
        ILFree(pidlWithPidlOriginal);
        ILFree(pidl);
    }

protected:
    PITEMID_CHILD pidl;
    PITEMID_CHILD pidlWithPidlOriginal;
    ITEMDATA itemData;
    ITEMDATA itemDataWithPidlOriginal;
    HRESULT pidlResult;
};


TEST_F(PidlTest, TestEqualValuesForPidlAndItemData) {
    ASSERT_EQ(pidlResult, S_OK);
    HRESULT hr;
    WCHAR szModuleName[MAX_PATH];
    hr = CBaseFolderViewChildHelper::GetModuleName(pidl, szModuleName, MAX_PATH);
    EXPECT_EQ(hr, S_OK) << "GetModuleName returned error";
    EXPECT_EQ(StrCmpW(itemData.szModuleName, szModuleName), 0) << L"szModuleName differs";
    WCHAR szGroupName[MAX_PATH];
    hr = CBaseFolderViewChildHelper::GetGroupName(pidl, szGroupName, MAX_PATH);
    EXPECT_EQ(hr, S_OK) << "GetGroupName returned error";
    EXPECT_EQ(StrCmpW(itemData.szGroupName, szGroupName), 0) << L"szGroupName differs";
    WCHAR szIconPath[MAX_PATH];
    hr = CBaseFolderViewChildHelper::GetIconPath(pidl, szIconPath, MAX_PATH);
    EXPECT_EQ(hr, S_OK) << "GetIconPath returned error";
    EXPECT_EQ(StrCmpW(itemData.szIconPath, szIconPath), 0) << L"szIconPath differs";
    WCHAR szParseName[MAX_PATH];
    hr = CBaseFolderViewChildHelper::GetParseName(pidl, szParseName, MAX_PATH);
    EXPECT_EQ(hr, S_OK) << "GetParseName returned error";
    EXPECT_EQ(StrCmpW(itemData.szParseName, szParseName), 0) << L"szParseName differs";
    WCHAR szDisplayName[MAX_PATH];
    hr = CBaseFolderViewChildHelper::GetDisplayName(pidl, szDisplayName, MAX_PATH);
    EXPECT_EQ(hr, S_OK) << "GetDisplayName returned error";
    EXPECT_EQ(StrCmpW(itemData.szDisplayName, szDisplayName), 0) << L"szDisplayName differs";
    BOOL fIsFolder;
    hr = CBaseFolderViewChildHelper::GetFolderness(pidl, &fIsFolder);
    PITEMID_CHILD pidlOriginal;
    pidlOriginal = CBaseFolderViewChildHelper::ClonePidlOriginal(pidl);
    EXPECT_EQ(pidlOriginal, (PITEMID_CHILD)NULL) << "pidlOriginal should be NULL for pidl";
    PITEMID_CHILD pidlOriginalNotNull;
    pidlOriginalNotNull = CBaseFolderViewChildHelper::ClonePidlOriginal(pidlWithPidlOriginal);
    EXPECT_TRUE(pidlOriginalNotNull != NULL) << "True pidlOriginal should not be NULL for pidlWithPidlOriginal";
    EXPECT_TRUE(ILIsEqual(pidlOriginalNotNull, itemDataWithPidlOriginal.pidlOriginal)) << "True pidlOriginal be equal to itemDataWithPidlOriginal.pidlOriginal for pidlWithPidlOriginal";
    EXPECT_EQ(hr, S_OK) << "GetFolderness returned error";
    EXPECT_EQ(fIsFolder, itemData.fIsFolder) << "fIsFolder from GetFolderness inEqual";
    EXPECT_EQ(itemData.fIsFolder, ((FVITEMID*)pidl)->fIsFolder) << "fIsFolder inEqual";
    EXPECT_EQ(itemData.ulAttributes, ((FVITEMID*)pidl)->ulAttributes) << "ulAttributes inEqual";
    EXPECT_EQ(itemData.ulGroupOrder, ((FVITEMID*)pidl)->ulGroupOrder) << "ulGroupOrder inEqual";
    EXPECT_EQ(itemData.dwIconIndex, ((FVITEMID*)pidl)->dwIconIndex) << "dwIconIndex inEqual";
    EXPECT_EQ(itemData.fIsRealPath, ((FVITEMID*)pidl)->fIsRealPath) << "fIsRealPath inEqual";
    EXPECT_TRUE(CBaseFolderViewChildHelper::IsValid(pidl)) << "isValid is not true for valid pidl";
    EXPECT_FALSE(CBaseFolderViewChildHelper::IsValid(NULL)) << "isValid is true for inValid pidl";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 1)], 0) << "pidl last byte not 0";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 2)], 0) << "pidl second last byte not 0";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 3)], 0) << "pidl third last byte not 0";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 4)], 0) << "pidl forth last byte not 0";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 5)], 0) << "pidl fifth last byte not 0";
    EXPECT_EQ(((unsigned char*)pidl)[(ILGetSize(pidl) - 6)], 0) << "pidl sixth last byte not 0";
    EXPECT_EQ(ILGetSize(pidl), pidl->mkid.cb + sizeof(USHORT)) << "pidl size does not match";
}


TEST_F(PidlTest, CheckIfNeedToChangeObjId) {
    ASSERT_EQ(pidlResult, S_OK);
    BYTE hash[4];
    hash[0] = 0;
    hash[1] = 0;
    hash[2] = 0;
    hash[3] = 0;
    for (unsigned int i = 0; i < ((FVITEMID*)pidl)->cb; i++)
        hash[i % 4] ^= ((BYTE*)(FVITEMID*)pidl)[i];
    EXPECT_EQ(hash[0], 172) << "hash[0] error add 1 to n_cMYOBJID and replace test to equal registered values";
    EXPECT_EQ(hash[1], 1) << "hash[1] error add 1 to n_cMYOBJID and replace test to equal registered values";
    EXPECT_EQ(hash[2], 101) << "hash[2] error add 1 to n_cMYOBJID and replace test to equal registered values";
    EXPECT_EQ(hash[3], 72) << "hash[3] error add 1 to n_cMYOBJID and replace test to equal registered values";
}