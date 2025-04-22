#pragma once

#include "framework.h"
#include <string>

namespace Logger
{
    // Initializes the log file.
    void Init(const std::wstring& logFilePath);

    // Appends a timestamped message to the log.
    void Log(const std::wstring& message);
}
