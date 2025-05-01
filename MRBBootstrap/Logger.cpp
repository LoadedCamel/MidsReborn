// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma message(">>> Compiling Logger.cpp (LoggerV2)")

#include "Logger.h"
#include <fstream>
#include <chrono>
#include <iomanip>
#include <mutex>
#include <windows.h> // For GetCurrentProcessId()

static std::wstring gLogPath;
static std::mutex gLogMutex;

void Logger::Init(const std::wstring& logFilePath)
{
    std::lock_guard<std::mutex> lock(gLogMutex);
    gLogPath = logFilePath;

    // Ensure Logs directory exists
    size_t pos = gLogPath.find_last_of(L"\\/");
    if (pos != std::wstring::npos)
    {
        std::wstring dir = gLogPath.substr(0, pos);
        CreateDirectoryW(dir.c_str(), nullptr);
    }

    std::wofstream file(gLogPath, std::ios::out | std::ios::trunc);
    if (file.is_open())
    {
        auto now = std::chrono::system_clock::now();
        std::time_t now_c = std::chrono::system_clock::to_time_t(now);
        std::tm localTime{};
        if (localtime_s(&localTime, &now_c) == 0)
        {
            file << L"------------------------------\n";
            file << L"Log started at " << std::put_time(&localTime, L"%Y-%m-%d %H:%M:%S") << L"\n";
        }
        else
        {
            file << L"------------------------------\n";
            file << L"Log started at <failed to retrieve local time>\n";
        }

        file << L"Session PID: " << GetCurrentProcessId() << L"\n";
        file << L"------------------------------\n";
        file.close();
    }
}

void Logger::Log(const std::wstring& message)
{
    Log(L"General", message);
}

void Logger::Log(const std::wstring& category, const std::wstring& message)
{
    std::lock_guard<std::mutex> lock(gLogMutex);

    std::wofstream file(gLogPath, std::ios::out | std::ios::app);
    if (!file.is_open())
        return;

    auto now = std::chrono::system_clock::now();
    std::time_t now_c = std::chrono::system_clock::to_time_t(now);
    std::tm localTime{};
    if (localtime_s(&localTime, &now_c) == 0)
    {
        file << L"[" << std::put_time(&localTime, L"%Y-%m-%d %H:%M:%S") << L"] ";
    }
    else
    {
        file << L"[<invalid time>] ";
    }

    file << L"[" << category << L"] " << message << L"\n";
    file.close();
}
