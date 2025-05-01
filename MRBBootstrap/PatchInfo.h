// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once
#include <string>

#include "Downloader.h"

std::wstring FormatPatchInfo(const std::wstring& filePath, const std::wstring& patchType);
std::wstring FormatPatchInfoFromFields(const std::wstring& name, const std::wstring& version, DWORD fileSizeBytes);
std::wstring GetSimplePatchDisplayNameVersion(const std::wstring& filePath);
std::wstring GetSimplePatchDisplayName(const std::wstring& filePath);
