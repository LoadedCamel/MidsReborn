#pragma once
#include <vector>
#include <string>
#include "PatchDecompressor.h"

namespace PatchInstaller
{
    bool WriteStagingFiles(const std::vector<FileEntry>& files, const std::wstring& installPath);
    bool ApplyStagedFiles(const std::vector<FileEntry>& files, const std::wstring& installPath, const std::wstring& stagingPath);
    static bool IsApplicationPatch(const std::vector<FileEntry>& files);
    static void CleanStaleFiles(const std::wstring& installPath, const std::vector<FileEntry>& patchFiles);
}
