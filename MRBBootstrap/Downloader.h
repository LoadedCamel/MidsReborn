// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once
#include <windows.h>
#include <winhttp.h> 
#include <string>

class Downloader
{
public:
    Downloader();
    ~Downloader();

    static bool GetLocalFileSize(const std::wstring& path, DWORD& outFileSize);
    static bool GetRemoteFileSize(const std::wstring& url, DWORD& outFileSize);
    bool ConnectToServer(const std::wstring& url);
    bool DownloadFile(const std::wstring& url, const std::wstring& localPath,
        const std::wstring& statusText = L"Downloading...", bool showProgress = true);
    void Cleanup();

private:
    HINTERNET _hSession = nullptr;
    HINTERNET _hConnect = nullptr;
    std::wstring _currentServer; // Host of last connection
};
