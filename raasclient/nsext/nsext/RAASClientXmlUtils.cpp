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
#include <msxml6.h>
#include <comutil.h>
#include <shlwapi.h>
#include <new>  // std::nothrow
#include <memory>
#include <vector>
#include <algorithm>
#include "ComUtils.h"
#include "Log.h"
#include "RAASClientXmlUtils.h"

using namespace std;

// Macro that calls a COM method returning HRESULT value.
#define CHK_HR(stmt)        do { hr=(stmt); if (FAILED(hr)) goto CleanUp; } while(0)

// Macro to verify memory allcation.
#define CHK_ALLOC(p)        do { if (!(p)) { hr = E_OUTOFMEMORY; goto CleanUp; } } while(0)

// Helper function to create a VT_BSTR variant from a null terminated string. 
HRESULT VariantFromString(PCWSTR szValue, VARIANT &Variant)
{
	HRESULT hr = S_OK;
	BSTR bstr = SysAllocString(szValue);
	CHK_ALLOC(bstr);
	V_VT(&Variant) = VT_BSTR;
	V_BSTR(&Variant) = bstr;
CleanUp:
	return hr;
}

// Helper function to create a DOM instance. 
HRESULT CreateAndInitDOM(IXMLDOMDocument **ppDoc)
{
	HRESULT hr = CoCreateInstance(__uuidof(DOMDocument60), NULL, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(ppDoc));
	if (SUCCEEDED(hr))
	{
		// these methods should not fail so don't inspect result
		(*ppDoc)->put_async(VARIANT_FALSE);
		(*ppDoc)->put_validateOnParse(VARIANT_FALSE);
		(*ppDoc)->put_resolveExternals(VARIANT_FALSE);
		(*ppDoc)->put_preserveWhiteSpace(VARIANT_TRUE);
	}
	return hr;
}

// Helper function to display parse error.
// It returns error code of the parse error.
HRESULT ReportParseError(IXMLDOMDocument *pDoc, char* /* szDesc */)
{
	HRESULT hr = S_OK;
	HRESULT hrRet = E_FAIL; // Default error code if failed to get from parse error.
	IXMLDOMParseError *pXMLErr = NULL;
	BSTR bstrReason = NULL;
	CHK_HR(pDoc->get_parseError(&pXMLErr));
	CHK_HR(pXMLErr->get_errorCode(&hrRet));
	CHK_HR(pXMLErr->get_reason(&bstrReason));
CleanUp:
	SAFE_RELEASE(pXMLErr);
	SysFreeString(bstrReason);
	return hrRet;
}

HRESULT GetServerStringsFromXmlFile(wchar_t* szServersFile, vector<wstring> &szServerNames, vector<wstring> &szServerAliases, unsigned int *pnServerCount)
{
	Log::Debug(L"GetServerStringsFromXmlFile(wchar_t* szServersFile, vector<wstring> &szServerNames, vector<wstring> &szServerAliases, unsigned int *pnServerCount)");
	HRESULT hr;
	int cServers = 0;
	IXMLDOMDocument *pXMLDom = NULL;
	IXMLDOMNodeList *pNodes = NULL;
	IXMLDOMNode *pNode = NULL;
	IXMLDOMNode *pParentNode = NULL;
	IXMLDOMNode *pEnabledNode = NULL;
	IXMLDOMNode *pAliasNode = NULL;
	IXMLDOMNode *pNodeText = NULL;
	IXMLDOMNode *pServerNode = NULL;
	BSTR bstrQuery1 = NULL;
	BSTR bstrQuery2 = NULL;
	BSTR bstrNodeName = NULL;
	BSTR bstrNodeValue = NULL;
	VARIANT_BOOL varStatus;
	VARIANT varFileName;
	VARIANT varNodeValue;
	VariantInit(&varFileName);
	VariantInit(&varNodeValue);
	CHK_HR(CreateAndInitDOM(&pXMLDom));
	CHK_HR(VariantFromString(_bstr_t(szServersFile), varFileName));
	CHK_HR(pXMLDom->load(varFileName, &varStatus));
	if (varStatus != VARIANT_TRUE)
	{
		CHK_HR(ReportParseError(pXMLDom, "Failed to load DOM from servers.xml."));
	}
	bstrQuery1 = SysAllocString(_bstr_t(L"//Servers/Server/Name"));
	CHK_ALLOC(bstrQuery1);
	CHK_HR(pXMLDom->selectNodes(bstrQuery1, &pNodes));
	long length;
	if (pNodes)
	{
		CHK_HR(pNodes->get_length(&length));
		for (long i = 0; i < length; i++)
		{
			CHK_HR(pNodes->get_item(i, &pNode));
			CHK_HR(pNode->get_parentNode(&pParentNode));
			CHK_HR(pParentNode->selectSingleNode(L"Enabled", &pEnabledNode));
			CHK_HR(pEnabledNode->get_firstChild(&pNodeText));
			VariantClear(&varNodeValue);
			CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
			if (StrCmpW(_bstr_t(varNodeValue), L"1") == 0) {
				SAFE_RELEASE(pNodeText);
				CHK_HR(pNode->get_nodeName(&bstrNodeName));
				CHK_HR(pNode->get_firstChild(&pNodeText));
				SAFE_RELEASE(pNode);
				VariantClear(&varNodeValue);
				CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
				wstring strServerName;
				wstring strServerAlias;
				if (pNodeText)
				{
					strServerName = _bstr_t(varNodeValue);
					strServerAlias = _bstr_t(varNodeValue);
					SAFE_RELEASE(pNodeText);
				}
				try
				{
					szServerNames.push_back(strServerName);
				}
				catch (...)
				{
					CHK_HR(S_FALSE);
				}
				CHK_HR(pParentNode->selectSingleNode(L"Alias", &pAliasNode));
				CHK_HR(pAliasNode->get_firstChild(&pNodeText));
				if (pNodeText)
				{
					VariantClear(&varNodeValue);
					CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
					strServerAlias = _bstr_t(varNodeValue);
					SAFE_RELEASE(pNodeText);
				}
				SAFE_RELEASE(pAliasNode);
				try
				{
					szServerAliases.push_back(strServerAlias);
				}
				catch (...)
				{
					CHK_HR(S_FALSE);
				}
				cServers++;
			}
			SAFE_RELEASE(pNodeText);
			SAFE_RELEASE(pServerNode);
			SAFE_RELEASE(pParentNode);
			SAFE_RELEASE(pEnabledNode);
		}
	}
	else {
		return S_FALSE;
	}
CleanUp:
	SAFE_RELEASE(pXMLDom);
	SAFE_RELEASE(pNodes);
	SAFE_RELEASE(pNode);
	SAFE_RELEASE(pNodeText);
	SAFE_RELEASE(pServerNode);
	SAFE_RELEASE(pParentNode);
	SAFE_RELEASE(pEnabledNode);
	SysFreeString(bstrQuery1);
	SysFreeString(bstrQuery2);
	SysFreeString(bstrNodeName);
	SysFreeString(bstrNodeValue);
	VariantClear(&varFileName);
	VariantClear(&varNodeValue);
	*pnServerCount = cServers;
	return hr;
}

HRESULT GetSharesFromXmlFile(wchar_t* szShareFile, NETWORK_SHARES* networkShares)
{
	Log::Debug(L"GetSharesFromXmlFile(wchar_t* szShareFile, NETWORK_SHARES* networkShares)");
	HRESULT hr;
	IXMLDOMDocument *pXMLDom = NULL;
	IXMLDOMNodeList *pNodes = NULL;
	IXMLDOMNodeList *pDriveNodes = NULL;
	IXMLDOMNode *pNode = NULL;
	IXMLDOMNode *pNodeText = NULL;
	IXMLDOMNode *pServerNode = NULL;
	IXMLDOMNode *pDriveNode = NULL;
	IXMLDOMNode *pSubNode = NULL;
	BSTR bstrQuery1 = NULL;
	BSTR bstrQuery2 = NULL;
	BSTR bstrNodeName = NULL;
	BSTR bstrNodeValue = NULL;
	VARIANT_BOOL varStatus;
	VARIANT varFileName;
	VARIANT varNodeValue;
	wchar_t szProfilePathOriginal[MAX_PATH];
	networkShares->strProfilePath[0] = NULL;
	VariantInit(&varFileName);
	VariantInit(&varNodeValue);
	CHK_HR(CreateAndInitDOM(&pXMLDom));
	CHK_HR(VariantFromString(_bstr_t(szShareFile), varFileName));
	CHK_HR(pXMLDom->load(varFileName, &varStatus));
	if (varStatus != VARIANT_TRUE)
	{
		CHK_HR(ReportParseError(pXMLDom, "Failed to load DOM from share.xml."));
	}
	bstrQuery1 = SysAllocString(_bstr_t(L"//Shares/ProfilePath"));
	CHK_ALLOC(bstrQuery1);
	CHK_HR(pXMLDom->selectNodes(bstrQuery1, &pNodes));
	long length;
	long driveCount;
	if (pNodes)
	{
		CHK_HR(pNodes->get_length(&length));
		int i = 0;
		hr = (length != 0) ? S_OK : S_FALSE;
		if (SUCCEEDED(hr))
		{
			CHK_HR(pNodes->get_item(i, &pNode));
			CHK_HR(pNode->get_nodeName(&bstrNodeName));
			CHK_HR(pNode->get_parentNode(&pServerNode));
			CHK_HR(pNode->get_firstChild(&pNodeText));
			if (pNodeText)
			{
				VariantClear(&varNodeValue);
				CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
				if (wcslen(_bstr_t(varNodeValue)) <= MAX_PATH)
					wcscpy_s(szProfilePathOriginal, MAX_PATH, _bstr_t(varNodeValue));
				else
					return S_FALSE;
				SAFE_RELEASE(pNodeText);
			}
			else
				return S_FALSE;
			_wcsupr_s(szProfilePathOriginal, MAX_PATH);
			CHK_HR(pServerNode->selectNodes(L"Drive", &pDriveNodes));
			CHK_HR(pDriveNodes->get_length(&driveCount));
			for (int j = 0; j < driveCount && j < n_cMAX_DRIVE_SHARES; j++)
			{
				std::shared_ptr<DRIVE_SHARE> driveShare(new DRIVE_SHARE());

				// Get name
				CHK_HR(pDriveNodes->get_item(j, &pDriveNode));
				CHK_HR(pDriveNode->selectSingleNode(L"Name", &pSubNode));
				CHK_HR(pSubNode->get_firstChild(&pNodeText));
				if (pNodeText)
				{
					VariantClear(&varNodeValue);
					CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
					if (wcslen(_bstr_t(varNodeValue)) <= MAX_PATH)
						driveShare->strName = _bstr_t(varNodeValue);
					else
						return S_FALSE;
					driveShare->strName[2] = NULL;
					SAFE_RELEASE(pNodeText);
				}
				SAFE_RELEASE(pSubNode);

				// Get type
				CHK_HR(pDriveNode->selectSingleNode(L"Type", &pSubNode));
				CHK_HR(pSubNode->get_firstChild(&pNodeText));
				if (pNodeText)
				{
					VariantClear(&varNodeValue);
					CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
					if (wcslen(_bstr_t(varNodeValue)) <= MAX_PATH)
						driveShare->strType = _bstr_t(varNodeValue);
					else
						return S_FALSE;
					SAFE_RELEASE(pNodeText);
				}
				SAFE_RELEASE(pSubNode);

				// Get path
				CHK_HR(pDriveNode->selectSingleNode(L"Share", &pSubNode));
				CHK_HR(pSubNode->get_firstChild(&pNodeText));
				if (pNodeText)
				{
					VariantClear(&varNodeValue);
					CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
					if (wcslen(_bstr_t(varNodeValue)) <= MAX_PATH)
						driveShare->strPath = _bstr_t(varNodeValue);
					else
						return S_FALSE;
					SAFE_RELEASE(pNodeText);
				}
				std::transform(driveShare->strName.begin(), driveShare->strName.end(), driveShare->strName.begin(), [](wchar_t c) {return static_cast<wchar_t>(::toupper(c)); });
				SAFE_RELEASE(pSubNode);
				SAFE_RELEASE(pDriveNode);

				// Determine profile path if share is the same drive
				if (driveShare->strName[0] == szProfilePathOriginal[0])
					networkShares->strProfilePath = driveShare->strPath + L"\\" + &szProfilePathOriginal[3];

				// Add share
				try
				{
					networkShares->vDriveShares.push_back(driveShare);
				}
				catch (...)
				{
					CHK_HR(S_FALSE);
				}
			}
			CHK_HR((networkShares->strProfilePath[0] != NULL) ? S_OK : S_FALSE);
		}
	}
CleanUp:
	SAFE_RELEASE(pXMLDom);
	SAFE_RELEASE(pNodes);
	SAFE_RELEASE(pNode);
	SAFE_RELEASE(pSubNode);
	SAFE_RELEASE(pDriveNode);
	SAFE_RELEASE(pDriveNodes);
	SAFE_RELEASE(pNodeText);
	SAFE_RELEASE(pServerNode);
	SysFreeString(bstrQuery1);
	SysFreeString(bstrQuery2);
	SysFreeString(bstrNodeName);
	SysFreeString(bstrNodeValue);
	VariantClear(&varFileName);
	VariantClear(&varNodeValue);
	return hr;
}



HRESULT GetServerVisibilityFromXmlFile(wchar_t* szShareFile, NETWORK_SHARES_VISIBILITY* networkShareVisibility)
{
	Log::Debug(L"GetServerVisibilityFromXmlFile(wchar_t* szShareFile, NETWORK_SHARES_VISIBILITY* networkShareVisibility)");
	HRESULT hr;
	IXMLDOMDocument *pXMLDom = NULL;
	IXMLDOMNode *pTopNode = NULL;
	IXMLDOMNode *pNode = NULL;
	IXMLDOMNode *pNodeText = NULL;
	IXMLDOMNode *pServerNode = NULL;
	IXMLDOMNode *pSubNode = NULL;
	BSTR bstrQuery1 = NULL;
	BSTR bstrQuery2 = NULL;
	BSTR bstrNodeName = NULL;
	BSTR bstrNodeValue = NULL;
	VARIANT_BOOL varStatus;
	VARIANT varFileName;
	VARIANT varNodeValue;
	VariantInit(&varNodeValue);
	VariantInit(&varFileName);
	CHK_HR(CreateAndInitDOM(&pXMLDom));
	CHK_HR(VariantFromString(_bstr_t(szShareFile), varFileName));
	CHK_HR(pXMLDom->load(varFileName, &varStatus));
	if (varStatus != VARIANT_TRUE)
	{
		CHK_HR(ReportParseError(pXMLDom, "Failed to load DOM from nsextvisibility.xml."));
	}

	// Get root visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Active", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bActive = true;
		else
			networkShareVisibility->bActive = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get 3DObjects visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/ThreeDObjects", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bThreeDObjects = true;
		else
			networkShareVisibility->bThreeDObjects = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Contacts visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Contacts", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bContacts = true;
		else
			networkShareVisibility->bContacts = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Desktop visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Desktop", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bDesktop = true;
		else
			networkShareVisibility->bDesktop = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Documents visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Documents", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bDocuments = true;
		else
			networkShareVisibility->bDocuments = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Downloads visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Downloads", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bDownloads = true;
		else
			networkShareVisibility->bDownloads = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Favorites visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Favorites", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bFavorites = true;
		else
			networkShareVisibility->bFavorites = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Links visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Links", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bLinks = true;
		else
			networkShareVisibility->bLinks = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Music visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Music", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bMusic = true;
		else
			networkShareVisibility->bMusic = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Pictures visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Pictures", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bPictures = true;
		else
			networkShareVisibility->bPictures = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Saved Games visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/SavedGames", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bSavedGames = true;
		else
			networkShareVisibility->bSavedGames = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Searches visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Searches", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bSearches = true;
		else
			networkShareVisibility->bSearches = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Videos visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/Videos", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bVideos = true;
		else
			networkShareVisibility->bVideos = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Diskette Drives visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/DisketteDrives", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bDisketteDrives = true;
		else
			networkShareVisibility->bDisketteDrives = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Hard Drives visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/HardDrives", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bFixedHardDrives = true;
		else
			networkShareVisibility->bFixedHardDrives = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get CD Drives visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/CDDrives", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bCDDrives = true;
		else
			networkShareVisibility->bCDDrives = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

	// Get Removable Drives visibility
	CHK_HR(pXMLDom->selectSingleNode(L"//ExplorerExtension/RemovableDrives", &pSubNode));
	CHK_HR(pSubNode->get_firstChild(&pNodeText));
	if (pNodeText)
	{
		CHK_HR(pNodeText->get_nodeValue(&varNodeValue));
		if (wcscmp(_bstr_t(varNodeValue), L"1") == 0)
			networkShareVisibility->bRemovableDrives = true;
		else
			networkShareVisibility->bRemovableDrives = false;
		SAFE_RELEASE(pNodeText);
	}
	SAFE_RELEASE(pSubNode);

CleanUp:
	SAFE_RELEASE(pXMLDom);
	SAFE_RELEASE(pTopNode);
	SAFE_RELEASE(pNode);
	SAFE_RELEASE(pSubNode);
	SAFE_RELEASE(pNodeText);
	SAFE_RELEASE(pServerNode);
	SysFreeString(bstrQuery1);
	SysFreeString(bstrQuery2);
	SysFreeString(bstrNodeName);
	SysFreeString(bstrNodeValue);
	VariantClear(&varFileName);
	VariantClear(&varNodeValue);
	return hr;
}