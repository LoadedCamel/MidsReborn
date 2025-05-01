// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "PatchInfo.h"
#include <filesystem>
#include <sstream>
#include <iomanip>

std::wstring CapitalizeFirst(const std::wstring& input)
{
    if (input.empty()) return input;
    std::wstring result = input;
    result[0] = towupper(result[0]);
    return result;
}

std::wstring FormatPatchInfo(const std::wstring& filePath, const std::wstring& patchType)
{
    namespace fs = std::filesystem;

    std::wstring appDisplayName = L"Mids Reborn";
    std::wstring dbName = L"Unknown";
    std::wstring version = L"Unknown";

    fs::path path(filePath);
    std::wstring filename = path.stem().wstring();

    size_t firstDash = filename.find(L'-');
    size_t secondDash = filename.find(L'-', firstDash + 1);

    if (firstDash != std::wstring::npos && secondDash != std::wstring::npos)
    {
        dbName = filename.substr(0, firstDash);
        version = filename.substr(firstDash + 1, secondDash - firstDash - 1);
    }

    uintmax_t fileSizeBytes = 0;
    try
    {
        fileSizeBytes = fs::file_size(filePath);
    }
    catch (...)
    {
        // ignore errors
    }

    std::wstringstream sizeStream;

    if (fileSizeBytes >= 1024 * 1024) // ≥ 1MB
    {
        double fileSizeMB = static_cast<double>(fileSizeBytes) / (1024.0 * 1024.0);
        if (std::fmod(fileSizeMB, 1.0) < 0.01)
            sizeStream << static_cast<int>(fileSizeMB) << L" MB";
        else
            sizeStream << std::fixed << std::setprecision(1) << fileSizeMB << L" MB";
    }
    else
    {
        double fileSizeKB = static_cast<double>(fileSizeBytes) / 1024.0;
        sizeStream << static_cast<int>(fileSizeKB) << L" KB";
    }

    std::wstringstream ss;
    if (patchType == L"db")
    {
        ss << CapitalizeFirst(dbName) << L" DB v" << version << L" (" << sizeStream.str() << L")";
    }
    else
    {
        ss << appDisplayName << L" v" << version << L" (" << sizeStream.str() << L")";
    }

    return ss.str();
}

std::wstring FormatPatchInfoFromFields(const std::wstring& name, const std::wstring& version, DWORD fileSizeBytes)
{
    std::wstringstream sizeStream;

    if (fileSizeBytes >= 1024 * 1024)
    {
        double fileSizeMB = static_cast<double>(fileSizeBytes) / (1024.0 * 1024.0);
        if (std::fmod(fileSizeMB, 1.0) < 0.01)
            sizeStream << static_cast<int>(fileSizeMB) << L" MB";
        else
            sizeStream << std::fixed << std::setprecision(1) << fileSizeMB << L" MB";
    }
    else
    {
        double fileSizeKB = static_cast<double>(fileSizeBytes) / 1024.0;
        sizeStream << static_cast<int>(fileSizeKB) << L" KB";
    }

    std::wstringstream ss;
    if (name == L"db")
        ss << CapitalizeFirst(name) << L" DB v" << version << L" (" << sizeStream.str() << L")";
    else
        ss << L"Mids Reborn v" << version << L" (" << sizeStream.str() << L")";

    return ss.str();
}

std::wstring GetSimplePatchDisplayNameVersion(const std::wstring& filePath)
{
    namespace fs = std::filesystem;

    fs::path path(filePath);
    std::wstring filename = path.stem().wstring();

    size_t firstDash = filename.find(L'-');
    size_t secondDash = filename.find(L'-', firstDash + 1);

    std::wstring name = L"Unknown";
    std::wstring version = L"Unknown";

    if (firstDash != std::wstring::npos && secondDash != std::wstring::npos)
    {
        name = filename.substr(0, firstDash);
        version = filename.substr(firstDash + 1, secondDash - firstDash - 1);
    }

    if (name == L"midsreborn")
        return L"Mids Reborn " + version;

    return CapitalizeFirst(name) + L" " + version;
}

std::wstring GetSimplePatchDisplayName(const std::wstring& filePath)
{
    namespace fs = std::filesystem;

    fs::path path(filePath);
    std::wstring filename = path.stem().wstring();

    size_t firstDash = filename.find(L'-');

    std::wstring name = L"Unknown";

    if (firstDash != std::wstring::npos)
    {
        name = filename.substr(0, firstDash);
    }

    if (name == L"midsreborn")
        return L"Mids Reborn";

    return CapitalizeFirst(name);
}

