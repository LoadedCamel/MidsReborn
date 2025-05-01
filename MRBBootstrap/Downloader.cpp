// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include <windows.h>
#include <winhttp.h>
#include "Downloader.h"

#include <filesystem>

#include "Logger.h"
#include <fstream>

#include "ModernUI.h"
#include "PatchManager.h"

#pragma comment(lib, "winhttp.lib")

Downloader::Downloader()
    : _hSession(nullptr), _hConnect(nullptr)
{
}

Downloader::~Downloader()
{
    Cleanup();
}

void Downloader::Cleanup()
{
    if (_hConnect)
    {
        WinHttpCloseHandle(_hConnect);
        _hConnect = nullptr;
    }
    if (_hSession)
    {
        WinHttpCloseHandle(_hSession);
        _hSession = nullptr;
    }
    _currentServer.clear();
}

bool Downloader::GetLocalFileSize(const std::wstring& path, DWORD& outFileSize)
{
    outFileSize = 0;

    try
    {
        if (std::filesystem::exists(path))
        {
            auto size = std::filesystem::file_size(path);
            if (size > 0)
            {
                outFileSize = static_cast<DWORD>(size);
                return true;
            }
        }
    }
    catch (...)
    {
    }

    return false;
}

bool Downloader::GetRemoteFileSize(const std::wstring& url, DWORD& outFileSize)
{
    outFileSize = 0;

    URL_COMPONENTS urlComp{};
    urlComp.dwStructSize = sizeof(urlComp);

    wchar_t hostName[256];
    wchar_t urlPath[2048];

    urlComp.lpszHostName = hostName;
    urlComp.dwHostNameLength = ARRAYSIZE(hostName);
    urlComp.lpszUrlPath = urlPath;
    urlComp.dwUrlPathLength = ARRAYSIZE(urlPath);

    if (!WinHttpCrackUrl(url.c_str(), 0, 0, &urlComp))
    {
        return false;
    }

    HINTERNET hSession = WinHttpOpen(L"MidsBootstrapper/1.0",
        WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
        WINHTTP_NO_PROXY_NAME,
        WINHTTP_NO_PROXY_BYPASS, 0);

    if (!hSession)
        return false;

    HINTERNET hConnect = WinHttpConnect(hSession, urlComp.lpszHostName, urlComp.nPort, 0);
    if (!hConnect)
    {
        WinHttpCloseHandle(hSession);
        return false;
    }

    DWORD flags = WINHTTP_FLAG_REFRESH;
    if (urlComp.nScheme == INTERNET_SCHEME_HTTPS)
        flags |= WINHTTP_FLAG_SECURE;

    HINTERNET hRequest = WinHttpOpenRequest(hConnect, L"HEAD", urlComp.lpszUrlPath,
        nullptr, WINHTTP_NO_REFERER, WINHTTP_DEFAULT_ACCEPT_TYPES, flags);

    if (!hRequest)
    {
        WinHttpCloseHandle(hConnect);
        WinHttpCloseHandle(hSession);
        return false;
    }

    bool result = false;
    if (WinHttpSendRequest(hRequest,
        WINHTTP_NO_ADDITIONAL_HEADERS, 0,
        WINHTTP_NO_REQUEST_DATA, 0,
        0, 0) &&
        WinHttpReceiveResponse(hRequest, nullptr))
    {
        DWORD size = sizeof(outFileSize);
        if (WinHttpQueryHeaders(hRequest,
            WINHTTP_QUERY_CONTENT_LENGTH | WINHTTP_QUERY_FLAG_NUMBER,
            WINHTTP_HEADER_NAME_BY_INDEX,
            &outFileSize, &size, WINHTTP_NO_HEADER_INDEX))
        {
            result = true;
        }
    }

    WinHttpCloseHandle(hRequest);
    WinHttpCloseHandle(hConnect);
    WinHttpCloseHandle(hSession);

    return result;
}

bool Downloader::ConnectToServer(const std::wstring& url)
{
    Cleanup(); // Always clean up before reconnect

    // Parse the host name
    URL_COMPONENTS urlComp{};
    urlComp.dwStructSize = sizeof(urlComp);

    wchar_t hostName[256];
    urlComp.lpszHostName = hostName;
    urlComp.dwHostNameLength = ARRAYSIZE(hostName);

    if (!WinHttpCrackUrl(url.c_str(), 0, 0, &urlComp))
    {
        Logger::Log(L"[Downloader] Failed to parse URL for server connect.");
        return false;
    }

    const std::wstring newServer(hostName);

    const int MAX_RETRIES = 3;
    const int CONNECT_TIMEOUT = 10000; // 10s
    const int RECEIVE_TIMEOUT = 10000; // 10s

    for (int attempt = 1; attempt <= MAX_RETRIES; ++attempt)
    {
        // Friendly dynamic status text
        if (newServer == L"updates.midsreborn.com")
        {
            ModernUI::PostUpdateStatus(L"Connecting to update server...");
        }
        else
        {
            ModernUI::PostUpdateStatus(L"Connecting to partner server...");
        }
        ModernUI::PostShowProgressBar(false);

        _hSession = WinHttpOpen(L"MidsBootstrapper/1.0",
            WINHTTP_ACCESS_TYPE_DEFAULT_PROXY,
            WINHTTP_NO_PROXY_NAME,
            WINHTTP_NO_PROXY_BYPASS, 0);

        if (!_hSession)
            continue;

        WinHttpSetTimeouts(_hSession, CONNECT_TIMEOUT, CONNECT_TIMEOUT, RECEIVE_TIMEOUT, RECEIVE_TIMEOUT);

        _hConnect = WinHttpConnect(_hSession, newServer.c_str(), urlComp.nPort, 0);

        if (_hConnect)
        {
            _currentServer = newServer; // Record the successful server
            return true;
        }

        WinHttpCloseHandle(_hSession);
        _hSession = nullptr;
        Sleep(1000 * attempt);
    }

    return false;
}

bool Downloader::DownloadFile(const std::wstring& url, const std::wstring& localPath, const std::wstring& statusText, bool showProgress)
{
    // Parse host name
    URL_COMPONENTS urlComp{};
    urlComp.dwStructSize = sizeof(urlComp);

    wchar_t hostName[256];
    wchar_t urlPath[2048];
    urlComp.lpszHostName = hostName;
    urlComp.dwHostNameLength = ARRAYSIZE(hostName);
    urlComp.lpszUrlPath = urlPath;
    urlComp.dwUrlPathLength = ARRAYSIZE(urlPath);

    if (!WinHttpCrackUrl(url.c_str(), 0, 0, &urlComp))
    {
        Logger::Log(L"[Downloader] Failed to parse URL in DownloadFile.");
        return false;
    }

    std::wstring serverHost(hostName);

    // Only connect if server changed
    if (_currentServer.empty() || serverHost != _currentServer)
    {
        if (!ConnectToServer(url))
        {
            Logger::Log(L"[Downloader] Failed to connect to server: " + serverHost);
            return false;
        }
    }

    DWORD flags = WINHTTP_FLAG_REFRESH;
    if (urlComp.nScheme == INTERNET_SCHEME_HTTPS)
        flags |= WINHTTP_FLAG_SECURE;

    HINTERNET hRequest = WinHttpOpenRequest(_hConnect, L"GET", urlComp.lpszUrlPath,
        nullptr, WINHTTP_NO_REFERER, WINHTTP_DEFAULT_ACCEPT_TYPES, flags);

    if (!hRequest)
    {
        Logger::Log(L"[Downloader] Failed to open HTTP request.");
        return false;
    }

    ModernUI::PostUpdateStatus(statusText);
    ModernUI::PostShowProgressBar(showProgress);
    ModernUI::PostUpdateProgress(0.0f);

    bool result = false;

    if (WinHttpSendRequest(hRequest,
        WINHTTP_NO_ADDITIONAL_HEADERS, 0,
        WINHTTP_NO_REQUEST_DATA, 0,
        0, 0) &&
        WinHttpReceiveResponse(hRequest, nullptr))
    {
        DWORD fileSize = 0;
        DWORD sizeSize = sizeof(fileSize);
        WinHttpQueryHeaders(hRequest,
            WINHTTP_QUERY_CONTENT_LENGTH | WINHTTP_QUERY_FLAG_NUMBER,
            WINHTTP_HEADER_NAME_BY_INDEX, &fileSize, &sizeSize, WINHTTP_NO_HEADER_INDEX);

        std::ofstream outFile(localPath, std::ios::binary);
        if (outFile.is_open())
        {
            DWORD bytesAvailable = 0;
            DWORD totalRead = 0;
            BYTE buffer[8192];
            bool success = true;

            while (WinHttpQueryDataAvailable(hRequest, &bytesAvailable) && bytesAvailable > 0)
            {
                if (PatchManager::gCancelled)
                {
                    outFile.close();
                    std::filesystem::remove(localPath); // Clean up partial file
                    WinHttpCloseHandle(hRequest);
                    return false;
                }

                DWORD bytesRead = 0;
                if (!WinHttpReadData(hRequest, buffer, sizeof(buffer), &bytesRead) || bytesRead == 0)
                {
                    success = false;
                    break;
                }

                outFile.write(reinterpret_cast<const char*>(buffer), bytesRead);
                totalRead += bytesRead;

                if (showProgress && fileSize > 0)
                {
                    ModernUI::SetProgress(static_cast<float>(totalRead) / static_cast<float>(fileSize));
                }
            }

            outFile.close();
            result = success && (totalRead > 0);
        }
    }

    WinHttpCloseHandle(hRequest);
    return result;
}
