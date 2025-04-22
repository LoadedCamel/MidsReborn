#include "MRBBootstrap.h"
#include "Downloader.h"
#include "PatchDecompressor.h"
#include "PatchInstaller.h"
#include "HashValidator.h"
#include "BootstrapperUI.h"
#include "Logger.h"

#include <filesystem>
#include <fstream>
#include <windows.h>
#include <shellapi.h>
#include <tlhelp32.h>
#include <nlohmann/json.hpp>

HINSTANCE hInst;
bool gSilent = false;
bool gNoRestart = false;
std::wstring gPatchFile;
std::wstring gPatchType;
std::wstring gHashBaseUrl;

bool gUseJsonPayload = false;
std::wstring gJsonPayloadPath;

struct UpdateEntry
{
    std::string Type;
    std::string Name;
    std::string Version;
    std::string File;
    std::string TargetPath;
    std::wstring LocalPath;
};

std::wstring ToW(const std::string& s)
{
    return std::wstring(s.begin(), s.end());
}

std::wstring ExpandEnv(const std::string& path)
{
    wchar_t buffer[MAX_PATH];
    ExpandEnvironmentStringsW(ToW(path).c_str(), buffer, MAX_PATH);
    return buffer;
}

std::wstring GetExecutableDir()
{
    wchar_t path[MAX_PATH];
    GetModuleFileNameW(nullptr, path, MAX_PATH);
    return std::filesystem::path(path).parent_path().wstring();
}

std::wstring GetHashFileNameFromPatch(const std::wstring& patchName)
{
    return std::filesystem::path(patchName).stem().wstring() + L".hash";
}

inline bool IsAbsoluteUrl(const std::wstring& url)
{
    return url.rfind(L"http://", 0) == 0 || url.rfind(L"https://", 0) == 0;
}

std::wstring Sanitize(const std::wstring& name)
{
    std::wstring result = name;
    for (auto& ch : result)
    {
        if (ch == L'\\' || ch == L'/' || ch == L':' || ch == L'*' || ch == L'?' || ch == L'"' || ch == L'<' || ch == L'>' || ch == L'|')
            ch = L'_';
    }
    return result;
}

int FailWithError(const std::wstring& message)
{
    Logger::Log(message);
    if (!gSilent)
        MessageBoxW(nullptr, message.c_str(), L"Bootstrapper Error", MB_OK | MB_ICONERROR);
    return -1;
}

bool ParseCommandLine(LPSTR lpCmdLine)
{
    int argc = 0;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLineW(), &argc);
    if (!argv || argc < 2)
        return false;

    std::wstring firstArg = argv[1];

    if (firstArg.ends_with(L".json") && std::filesystem::exists(firstArg))
    {
        gUseJsonPayload = true;
        gJsonPayloadPath = firstArg;

        for (int i = 2; i < argc; ++i)
        {
            std::wstring arg = argv[i];
            if (arg == L"--silent") gSilent = true;
            else if (arg == L"--no-restart") gNoRestart = true;
        }

        return true;
    }

    for (int i = 1; i < argc; ++i)
    {
        std::wstring arg = argv[i];
        if (arg == L"--type" && i + 1 < argc)
            gPatchType = argv[++i];
        else if (arg == L"--file" && i + 1 < argc)
            gPatchFile = argv[++i];
        else if (arg == L"--hashbase" && i + 1 < argc)
            gHashBaseUrl = argv[++i];
        else if (arg == L"--silent")
            gSilent = true;
        else if (arg == L"--no-restart")
            gNoRestart = true;
    }

    return !gPatchFile.empty();
}

static void KillMidsReborn()
{
    Logger::Log(L"[Bootstrapper] Attempting to terminate MidsReborn.exe...");

    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (snapshot == INVALID_HANDLE_VALUE)
        return;

    PROCESSENTRY32W entry{};
    entry.dwSize = sizeof(PROCESSENTRY32W);

    if (Process32FirstW(snapshot, &entry))
    {
        do {
            if (_wcsicmp(entry.szExeFile, L"MidsReborn.exe") == 0)
            {
                HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, FALSE, entry.th32ProcessID);
                if (hProcess)
                {
                    TerminateProcess(hProcess, 0);
                    CloseHandle(hProcess);
                    Logger::Log(L"[Bootstrapper] Terminated process ID: " + std::to_wstring(entry.th32ProcessID));
                }
                break;
            }
        } while (Process32NextW(snapshot, &entry));
    }

    CloseHandle(snapshot);
}

int RunSinglePatchFlow()
{
    std::wstring workingDir = GetExecutableDir();
    std::wstring mruPath = workingDir + L"\\" + gPatchFile;
    std::wstring hashFileName = GetHashFileNameFromPatch(gPatchFile);
    std::wstring hashPath = workingDir + L"\\" + hashFileName;

    BootstrapperUI::SetProgressLabel(L"Downloading patch...");
    if (!std::filesystem::exists(mruPath))
    {
        std::wstring url = (gPatchType == L"app")
            ? L"https://updates.midsreborn.com/" + gPatchFile
            : gHashBaseUrl + gPatchFile;

        if (!Downloader::DownloadFile(url, mruPath))
            return FailWithError(L"Failed to download patch file.");
    }

    BootstrapperUI::SetProgressLabel(L"Downloading hash...");
    if (!std::filesystem::exists(hashPath))
    {
        std::wstring url = (gPatchType == L"app")
            ? L"https://updates.midsreborn.com/" + hashFileName
            : gHashBaseUrl + hashFileName;

        if (!Downloader::DownloadFile(url, hashPath))
            return FailWithError(L"Failed to download hash file.");
    }

    BootstrapperUI::SetProgressLabel(L"Extracting patch...");
    auto files = PatchDecompressor::Decompress(mruPath);
    if (files.empty())
        return FailWithError(L"Failed to decompress patch.");

    std::wstring stagingDir = GetExecutableDir() + L"\\UpdateStaging";
    std::filesystem::create_directories(stagingDir);

    if (!PatchInstaller::WriteStagingFiles(files, stagingDir))
        return FailWithError(L"Failed to write staged files.");

    KillMidsReborn();

    if (!HashValidator::Validate(mruPath, hashPath))
        return FailWithError(L"Patch validation failed.");

    if (!PatchInstaller::ApplyStagedFiles(files, workingDir,  stagingDir))
        return FailWithError(L"Install failed. Rollback applied.");

    BootstrapperUI::SetProgressLabel(L"Update complete.");

    try {
        if (std::filesystem::exists(mruPath))
            std::filesystem::remove(mruPath);

        if (std::filesystem::exists(hashPath))
            std::filesystem::remove(hashPath);
    }
    catch (const std::exception& ex) {
        Logger::Log(L"Warning: Failed to clean up patch files: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
    }

    if (!gNoRestart)
        ShellExecuteW(nullptr, L"open", L"midsreborn.exe", nullptr, workingDir.c_str(), SW_SHOW);

    return 0;
}

int RunJsonDrivenPatchFlow(const std::wstring& jsonPath)
{
    std::vector<UpdateEntry> updates;

    try {
        std::ifstream in(jsonPath);
        if (!in.is_open()) return FailWithError(L"Failed to open JSON payload.");
        nlohmann::json j;
        in >> j;

        for (const auto& entry : j)
        {
            UpdateEntry u;
            u.Type = entry.value("Type", "");
            u.Name = entry.value("Name", "");
            u.Version = entry.value("Version", "");
            u.File = entry.value("File", "");
            u.TargetPath = entry.value("TargetPath", "");
            updates.push_back(u);
        }
    }
    catch (...)
    {
        return FailWithError(L"Failed to parse JSON update list.");
    }

    if (updates.empty())
        return FailWithError(L"No updates defined in payload.");

    std::wstring workingDir = GetExecutableDir();
    std::filesystem::create_directories(workingDir);

    for (auto& update : updates)
    {
        std::wstring mruName = std::filesystem::path(update.File).filename();
        std::wstring hashName = GetHashFileNameFromPatch(mruName);
        std::wstring mruLocal = workingDir + L"\\" + mruName;
        std::wstring hashLocal = workingDir + L"\\" + hashName;

        bool isAbsolute = IsAbsoluteUrl(ToW(update.File));

        std::wstring mruUrl = isAbsolute
            ? ToW(update.File)
            : L"https://updates.midsreborn.com/" + ToW(update.File);

        std::wstring hashUrl = isAbsolute
            ? std::filesystem::path(mruUrl).parent_path().wstring() + L"/" + hashName
            : L"https://updates.midsreborn.com/" + hashName;

        BootstrapperUI::SetProgressLabel(L"Downloading: " + ToW(update.Name));
        if (!Downloader::DownloadFile(mruUrl, mruLocal))
            return FailWithError(L"Download failed: " + mruUrl);
        if (!Downloader::DownloadFile(hashUrl, hashLocal))
            return FailWithError(L"Download failed: " + hashUrl);

        update.LocalPath = mruLocal;
    }

    for (const auto& update : updates)
    {
        BootstrapperUI::SetProgressLabel(L"Processing: " + ToW(update.Name));

        auto files = PatchDecompressor::Decompress(update.LocalPath);
        if (files.empty())
            return FailWithError(L"Decompression failed: " + ToW(update.File));

        std::wstring stagingPath = GetExecutableDir() + L"\\UpdateStaging\\";
        std::filesystem::create_directories(stagingPath);

        if (!PatchInstaller::WriteStagingFiles(files, stagingPath))
            return FailWithError(L"Failed to stage files for: " + ToW(update.File));

        std::wstring installPath = ExpandEnv(update.TargetPath);
        std::wstring hashPath = update.LocalPath.substr(0, update.LocalPath.find_last_of(L'.')) + L".hash";

        KillMidsReborn();

        if (!HashValidator::Validate(update.LocalPath, hashPath))
            return FailWithError(L"Hash check failed: " + ToW(update.File));

        if (!PatchInstaller::ApplyStagedFiles(files, installPath, stagingPath))
            return FailWithError(L"Install failed: " + ToW(update.File));
    }

    BootstrapperUI::SetProgressLabel(L"All updates complete.");

    try {
        for (const auto& update : updates)
        {
            if (std::filesystem::exists(update.LocalPath))
                std::filesystem::remove(update.LocalPath);

            const std::wstring hashPath = update.LocalPath.substr(0, update.LocalPath.find_last_of(L'.')) + L".hash";
            if (std::filesystem::exists(hashPath))
                std::filesystem::remove(hashPath);
        }

        if (std::filesystem::exists(jsonPath))
            std::filesystem::remove(jsonPath);
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"Warning: Cleanup error: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
    }

    if (!gNoRestart)
        ShellExecuteW(nullptr, L"open", L"midsreborn.exe", nullptr, workingDir.c_str(), SW_SHOW);

    return 0;
}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR lpCmdLine, int nCmdShow)
{
    hInst = hInstance;

    if (!ParseCommandLine(lpCmdLine))
    {
        return FailWithError(
            L"Invalid arguments.\n\n"
            L"Usage:\n"
            L"  mids-bootstrap.exe --type app|db --file <filename> [--hashbase <url>] [--silent] [--no-restart]\n"
            L"  mids-bootstrap.exe <update.json> [--silent] [--no-restart]"
        );
    }

    Logger::Init(GetExecutableDir() + L"\\Logs\\update.log");

    if (!gSilent)
        BootstrapperUI::Show(hInst);

    return gUseJsonPayload
        ? RunJsonDrivenPatchFlow(gJsonPayloadPath)
        : RunSinglePatchFlow();
}
