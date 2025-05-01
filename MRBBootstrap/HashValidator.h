// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once

#include <string>

namespace HashValidator
{
    // Validates the .mru patch by comparing extracted .upd files against the .hash file.
    // Returns true if all match, false if any mismatch or error.
    bool Validate(const std::wstring& stagingPath, const std::wstring& hashPath);
}
