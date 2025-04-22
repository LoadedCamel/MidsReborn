#include "Downloader.h"
#include "BootstrapperUI.h"
#include "Logger.h"

#include <wininet.h>
#include <fstream>

#pragma comment(lib, "wininet.lib")

bool Downloader::DownloadFile(const std::wstring& url, const std::wstring& localPath)
{
    const int BUFFER_SIZE = 8192;
    BYTE buffer[BUFFER_SIZE];

    HINTERNET hInternet = InternetOpenW(L"MidsBootstrapper", INTERNET_OPEN_TYPE_PRECONFIG, nullptr, nullptr, 0);
    if (!hInternet)
        return false;

    HINTERNET hFile = InternetOpenUrlW(hInternet, url.c_str(), nullptr, 0, INTERNET_FLAG_RELOAD, 0);
    if (!hFile)
    {
        InternetCloseHandle(hInternet);
        return false;
    }

    // Get total size for progress
    DWORD fileSize = 0;
    DWORD sizeSize = sizeof(fileSize);
    HttpQueryInfo(hFile, HTTP_QUERY_CONTENT_LENGTH | HTTP_QUERY_FLAG_NUMBER, &fileSize, &sizeSize, nullptr);

    std::ofstream outFile(localPath, std::ios::binary);
    if (!outFile.is_open())
    {
        InternetCloseHandle(hFile);
        InternetCloseHandle(hInternet);
        return false;
    }

    BootstrapperUI::SetProgressLabel(L"Downloading...");
    BootstrapperUI::SetProgress(0, fileSize);

    DWORD bytesRead = 0;
    DWORD totalRead = 0;
    while (InternetReadFile(hFile, buffer, BUFFER_SIZE, &bytesRead) && bytesRead > 0)
    {
        outFile.write(reinterpret_cast<char*>(buffer), bytesRead);
        totalRead += bytesRead;

        BootstrapperUI::SetProgress(static_cast<int>(totalRead), static_cast<int>(fileSize));
    }

    outFile.close();
    InternetCloseHandle(hFile);
    InternetCloseHandle(hInternet);

    return true;
}
