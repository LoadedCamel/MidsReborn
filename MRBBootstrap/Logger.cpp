#include "Logger.h"
#include <fstream>
#include <chrono>
#include <iomanip>
#include <sstream>

static std::wstring gLogPath;

void Logger::Init(const std::wstring& logFilePath)
{
    gLogPath = logFilePath;

    // Ensure Logs directory exists
    size_t pos = gLogPath.find_last_of(L"\\/");
    if (pos != std::wstring::npos)
    {
        std::wstring dir = gLogPath.substr(0, pos);
        CreateDirectoryW(dir.c_str(), nullptr);
    }

    // Overwrite log file at startup
    std::wofstream file(gLogPath, std::ios::out | std::ios::trunc);
    if (file.is_open())
    {
        auto now = std::chrono::system_clock::now();
        std::time_t now_c = std::chrono::system_clock::to_time_t(now);
        std::tm localTime;
        localtime_s(&localTime, &now_c);

        file << L"------------------------------\n";
        file << L"Log started at "
            << std::put_time(&localTime, L"%Y-%m-%d %H:%M:%S") << L"\n";
        file << L"------------------------------\n";
        file.close();
    }
}

void Logger::Log(const std::wstring& message)
{
    std::wofstream file(gLogPath, std::ios::out | std::ios::app);
    if (!file.is_open())
        return;

    // Timestamp
    auto now = std::chrono::system_clock::now();
    std::time_t now_c = std::chrono::system_clock::to_time_t(now);
    std::tm localTime;
    localtime_s(&localTime, &now_c);

    std::wstringstream timestamp;
    timestamp << std::put_time(&localTime, L"[%Y-%m-%d %H:%M:%S] ");

    file << timestamp.str() << message << L"\n";
    file.close();
}
