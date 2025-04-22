#pragma once

#include "framework.h"
#include <string>

namespace HashValidator
{
    // Validates the .mru patch by comparing extracted .upd files against the .hash file.
    // Returns true if all match, false if any mismatch or error.
    bool Validate(const std::wstring& mruPath, const std::wstring& hashPath);
}
