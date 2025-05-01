// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "PatchDecompressor.h"
#include "PatchManager.h"
#include "Logger.h"

#include <fstream>
#include <iomanip>
#include <sstream>
#include <vector>
#include <zlib.h>
#include <string>
#include <filesystem>
#include <windows.h>

#define ZLIB_WINAPI

// Reads a .NET 7-bit encoded integer
static int32_t Read7BitEncodedInt(std::istream& stream)
{
    int32_t result = 0;
    int shift = 0;
    uint8_t byte;

    for (int i = 0; i < 5; ++i)
    {
        stream.read(reinterpret_cast<char*>(&byte), 1);
        if (!stream) return -1;

        result |= (byte & 0x7F) << shift;
        if ((byte & 0x80) == 0) return result;

        shift += 7;
    }

    return -1; // invalid encoding
}

// Reads a .NET UTF-8 Pascal-style string
static std::wstring ReadDotNetString(std::istream& stream)
{
    int32_t length = Read7BitEncodedInt(stream);
    if (length < 0 || length > 65536)
    {
        Logger::Log(L"[Decompressor] Invalid string length: " + std::to_wstring(length));
        stream.setstate(std::ios::failbit);
        return L"";
    }

    std::string utf8(length, '\0');
    stream.read(utf8.data(), length);
    if (!stream)
    {
        Logger::Log(L"[Decompressor] Failed to read string payload.");
        return L"";
    }

    int wideLen = MultiByteToWideChar(CP_UTF8, 0, utf8.data(), length, nullptr, 0);
    std::wstring wstr(wideLen, L'\0');
    MultiByteToWideChar(CP_UTF8, 0, utf8.data(), length, &wstr[0], wideLen);
    return wstr;
}

std::vector<FileEntry> PatchDecompressor::Decompress(const std::wstring& mruPath)
{
    std::vector<FileEntry> files;

    // Load full .mru file
    std::ifstream file(mruPath, std::ios::binary | std::ios::ate);
    if (!file.is_open())
    {
        Logger::Log(L"[Decompressor] Failed to open patch file: " + mruPath);
        return files;
    }

    std::streamsize size = file.tellg();
    file.seekg(0, std::ios::beg);
    std::vector<uint8_t> compressed(static_cast<size_t>(size));
    file.read(reinterpret_cast<char*>(compressed.data()), size);
    file.close();

    // Decompress with zlib
    std::vector<uint8_t> uncompressed;
    std::vector<uint8_t> buffer(8192);

    z_stream stream{};
    stream.next_in = compressed.data();
    stream.avail_in = static_cast<uInt>(compressed.size());

    if (inflateInit2(&stream, 15) != Z_OK)
    {
        Logger::Log(L"[Decompressor] inflateInit2 failed.");
        return files;
    }

    int result;
    do
    {
        if (PatchManager::gCancelled)
        {
            Logger::Log(L"[Decompressor] Cancelled during inflate.");
            inflateEnd(&stream);
            return files;
        }

        stream.next_out = buffer.data();
        stream.avail_out = static_cast<uInt>(buffer.size());

        result = inflate(&stream, Z_NO_FLUSH);
        if (result != Z_OK && result != Z_STREAM_END)
        {
            Logger::Log(L"[Decompressor] inflate failed. Code: " + std::to_wstring(result));
            inflateEnd(&stream);
            return files;
        }

        size_t bytesWritten = buffer.size() - stream.avail_out;
        uncompressed.insert(uncompressed.end(), buffer.begin(), buffer.begin() + bytesWritten);

    } while (result != Z_STREAM_END);

    inflateEnd(&stream);

    // Begin parsing uncompressed patch data
    std::istringstream ss(std::string(reinterpret_cast<const char*>(uncompressed.data()), uncompressed.size()), std::ios::binary);

    // Read magic string
    std::wstring magic = ReadDotNetString(ss);
    if (magic != L"Mids Reborn Patch Data")
    {
        Logger::Log(L"[Decompressor] Invalid patch header.");
        return files;
    }

    uint32_t count = 0;
    ss.read(reinterpret_cast<char*>(&count), sizeof(count));

    for (uint32_t i = 0; i < count; ++i)
    {
        if (PatchManager::gCancelled)
        {
            Logger::Log(L"[Decompressor] Cancelled during patch entry parsing.");
            break; // stop parsing
        }

        FileEntry entry;
        uint32_t dataLen = 0;

        ss.read(reinterpret_cast<char*>(&dataLen), sizeof(dataLen));
        if (!ss)
        {
            Logger::Log(L"[Decompressor] Failed to read data length at index " + std::to_wstring(i));
            break;
        }

        entry.FileName = ReadDotNetString(ss);
        if (!ss)
        {
            Logger::Log(L"[Decompressor] Failed to read FileName at index " + std::to_wstring(i));
            break;
        }

        entry.Directory = ReadDotNetString(ss);
        if (!ss)
        {
            Logger::Log(L"[Decompressor] Failed to read Directory at index " + std::to_wstring(i));
            break;
        }

        std::ranges::replace(entry.Directory, L'/', L'\\');
        while (!entry.Directory.empty() && entry.Directory.front() == L'\\')
            entry.Directory.erase(entry.Directory.begin());

        entry.Data.resize(dataLen);
        ss.read(reinterpret_cast<char*>(entry.Data.data()), dataLen);
        if (!ss)
        {
            Logger::Log(L"[Decompressor] Failed to read file data at index " + std::to_wstring(i));
            break;
        }

        files.push_back(entry);
    }

    return files;
}
