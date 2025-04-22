#pragma once

#include "framework.h"
#include <string>
#include <vector>

struct FileEntry
{
    std::wstring Directory;
    std::wstring FileName;
    std::vector<uint8_t> Data;
};

namespace PatchDecompressor
{
    std::vector<FileEntry> Decompress(const std::wstring& mruPath);
}
