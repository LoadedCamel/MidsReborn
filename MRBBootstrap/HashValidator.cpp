// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "HashValidator.h"
#include "PatchDecompressor.h"
#include "Logger.h"
#include "PatchManager.h"

#include <fstream>
#include <nlohmann/json.hpp>  
#include <windows.h>
#include <wincrypt.h>
#include <iomanip>
#include <sstream>
#include <vector>
#include <filesystem>
#include <string>
#include <iostream>

#include "ModernUI.h"


#pragma comment(lib, "advapi32.lib")

using json = nlohmann::json;

struct HashEntry
{
    std::wstring FileName;
    std::wstring Directory;
    std::wstring Hash;
};

// Converts a byte array to lowercase hex string
static std::wstring ToHex(const BYTE* hash, DWORD length)
{
    std::wstringstream ss;
    for (DWORD i = 0; i < length; ++i)
    {
        ss << std::hex << std::setw(2) << std::setfill(L'0') << static_cast<int>(hash[i]);
    }
    return ss.str();
}

// Calculates SHA256 hash of a file
static std::wstring ComputeSHA256(const std::wstring& path)
{
    HCRYPTPROV h_prov = 0;
    HCRYPTHASH h_hash = 0;
    DWORD cb_hash = 32;
    std::wstring result;

    if (!CryptAcquireContextW(&h_prov, nullptr, nullptr, PROV_RSA_AES, CRYPT_VERIFYCONTEXT))
    {
        Logger::Log(L"[HashValidator] CryptAcquireContextW failed.");
        return result;
    }

    if (!CryptCreateHash(h_prov, CALG_SHA_256, 0, 0, &h_hash))
    {
        Logger::Log(L"[HashValidator] CryptCreateHash failed.");
        CryptReleaseContext(h_prov, 0);
        return result;
    }

    std::ifstream file(path, std::ios::binary);
    if (!file)
    {
        Logger::Log(L"[HashValidator] Failed to open file for hashing: " + path);
        CryptDestroyHash(h_hash);
        CryptReleaseContext(h_prov, 0);
        return result;
    }

    std::vector<char> buffer(8192);
    while (file.good())
    {
        file.read(buffer.data(), buffer.size());
        if (!CryptHashData(h_hash, reinterpret_cast<BYTE*>(buffer.data()), static_cast<DWORD>(file.gcount()), 0))
        {
            Logger::Log(L"[HashValidator] CryptHashData failed while reading: " + path);
            CryptDestroyHash(h_hash);
            CryptReleaseContext(h_prov, 0);
            return result;
        }
    }

    if (BYTE rgbHash[32]; CryptGetHashParam(h_hash, HP_HASHVAL, rgbHash, &cb_hash, 0))
    {
        result = ToHex(rgbHash, cb_hash);
    }
    else
    {
        Logger::Log(L"[HashValidator] CryptGetHashParam failed for file: " + path);
    }

    CryptDestroyHash(h_hash);
    CryptReleaseContext(h_prov, 0);
    return result;
}


bool HashValidator::Validate(const std::wstring& stagingPath, const std::wstring& hashPath)
{
    std::wstring stagingDir = stagingPath;

    std::ifstream hashFile(hashPath);
    if (!hashFile.is_open())
    {
        Logger::Log(L"Failed to open hash file: " + hashPath);
        return false;
    }

    json j;
    try {
        hashFile >> j;
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"Failed to parse JSON: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        return false;
    }

    const size_t totalFiles = j.size();
    size_t currentIndex = 0;

    ModernUI::PostUpdateStatus(L"Validating...");
    ModernUI::PostShowProgressBar(true);
    ModernUI::PostUpdateProgress(0.0f);

    for (const auto& item : j)
    {
        if (PatchManager::gCancelled)
        {
            Logger::Log(L"[HashValidator] Cancelled during validation phase.");
            ModernUI::PostUpdateStatus(L"Validation Cancelled.");
            ModernUI::PostShowProgressBar(false);
            return false;
        }

        HashEntry entry;

        std::string dirStr = item.value("Directory", "");
        std::string fileStr = item.value("FileName", "");
        std::string hashStr = item.value("Hash", "");

        entry.Directory = std::wstring(dirStr.begin(), dirStr.end());
        entry.FileName = std::wstring(fileStr.begin(), fileStr.end());
        entry.Hash = std::wstring(hashStr.begin(), hashStr.end());

        namespace fs = std::filesystem;

        fs::path stagedPath = fs::path(stagingDir) / entry.Directory / entry.FileName;
        std::wstring actualHash = ComputeSHA256(stagedPath);

        ++currentIndex;
        ModernUI::SetProgress(static_cast<float>(currentIndex) / static_cast<float>(totalFiles));

        if (_wcsicmp(actualHash.c_str(), entry.Hash.c_str()) != 0)
        {
            Logger::Log(L"Hash mismatch for: " + entry.FileName);
            ModernUI::PostUpdateStatus(L"Validation Failed.");
            ModernUI::PostShowProgressBar(false);
            Sleep(1500);
            return false;
        }
    }

    return true;
}
