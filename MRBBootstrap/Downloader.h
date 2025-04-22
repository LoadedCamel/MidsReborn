#pragma once

#include "framework.h"
#include <string>

namespace Downloader
{
    // Downloads a file from a URL to the specified local file path.
    // Returns true on success, false on failure.
    bool DownloadFile(const std::wstring& url, const std::wstring& localPath);
}
