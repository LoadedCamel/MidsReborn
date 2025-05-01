// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once
#include <string>

class Logger
{
public:
    static void Init(const std::wstring& logFilePath);
    static void Log(const std::wstring& message); // General fallback
    static void Log(const std::wstring& category, const std::wstring& message);
};
