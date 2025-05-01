// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once
#include <vector>
#include <string>
#include "PatchDecompressor.h"

namespace PatchInstaller
{
    bool WriteStagingFiles(const std::vector<FileEntry>& files, const std::wstring& installPath);
    bool ApplyStagedFiles(const std::vector<FileEntry>& files, const std::wstring& installPath, const std::wstring& stagingPath);
}
