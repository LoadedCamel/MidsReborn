// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "PatchManager.h"
#include "Downloader.h"
#include "PatchDecompressor.h"
#include "PatchInstaller.h"
#include "HashValidator.h"
#include "ModernUI.h"
#include "PatchInfo.h"
#include "Logger.h"

#include <filesystem>
#include <fstream>
#include <shellapi.h>
#include <tlhelp32.h>
#include <nlohmann/json.hpp>

extern bool gSilent;
extern bool gNoRestart;
extern std::wstring gPatchFile;
extern std::wstring gPatchType;
extern std::wstring gHashBaseUrl;
extern bool gUseJsonPayload;
extern std::wstring gJsonPayloadPath;
extern HINSTANCE hInst;

namespace PatchManager
{
    volatile bool gCancelled = false;
    volatile bool gInstalling = false;
    volatile bool gPreInstall = false;
    std::wstring gBackupPath;

    // --- Struct ---

    struct UpdateEntry
    {
        std::string Type;
        std::string Name;
        std::string Version;
        std::string File;
        std::string TargetPath;
        std::wstring LocalPath;
    };

    // --- Utility functions ---

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
        return url.starts_with(L"http://") || url.starts_with(L"https://");
    }

    int LogAndFail(const std::wstring& message, bool showToUser)
    {
        Logger::Log(message);
        if (showToUser)
        {
            ModernUI::PostUpdateStatus(message);
            ModernUI::PostTriggerUpdateFailure();
            ModernUI::Delay(500);
            ModernUI::PostCloseUI(); // optional depending on flow
        }
        return -1;
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

    void CleanupPatchFiles(const std::wstring& mruPath, const std::wstring& hashPath, const std::optional<std::wstring>& stagingPath, const std::optional<std::wstring>& backupPath)
    {
        try
        {
            namespace fs = std::filesystem;

            if (!mruPath.empty() && fs::exists(mruPath))
                fs::remove(mruPath);

            if (!hashPath.empty() && fs::exists(hashPath))
                fs::remove(hashPath);

            if (stagingPath && fs::exists(*stagingPath))
                fs::remove_all(*stagingPath);

            if (backupPath && fs::exists(*backupPath))
                fs::remove_all(*backupPath);

            // --- New: Delete any "mids_patch*.json" files in temp directory ---
            wchar_t tempPath[MAX_PATH];
            if (GetTempPathW(MAX_PATH, tempPath))
            {
                for (const auto& entry : fs::directory_iterator(tempPath))
                {
                    const std::wstring fileName = entry.path().filename().wstring();
                    if (entry.is_regular_file() &&
                        fileName.ends_with(L".json") &&
                        fileName.find(L"mids_patch") != std::wstring::npos)
                    {
                        std::error_code ec;
                        fs::remove(entry.path(), ec);
                        if (ec)
                        {
                            Logger::Log(L"[Cleanup] Failed to remove temp file: " + entry.path().wstring());
                        }
                        else
                        {
                            Logger::Log(L"[Cleanup] Removed temp file: " + entry.path().wstring());
                        }
                    }
                }
            }

            Logger::Log(L"[Installer] Cleanup complete.");
        }
        catch (const std::exception& ex)
        {
            Logger::Log(L"[Installer] Cleanup error: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        }
    }

    bool CreateBackup(const std::wstring& installPath, const std::wstring& backupPath, const std::wstring& patchType, const std::wstring& dbName)
    {
        try
        {
            namespace fs = std::filesystem;
            fs::create_directories(backupPath);

            std::vector<std::pair<fs::path, fs::path>> filesToCopy;

            if (patchType == L"Application")
            {
                // Collect files to copy (excluding filtered paths)
                for (const auto& entry : fs::recursive_directory_iterator(installPath))
                {
                    fs::path relativePath = fs::relative(entry.path(), installPath);
                    std::wstring relStr = relativePath.wstring();

                    if (
                        relStr == L"appSettings.json" ||
                        relStr == L"MRBBootstrap.exe" ||
                        relStr.starts_with(L"Logs") ||
                        relStr.starts_with(L"UpdateStaging") ||
                        relStr.starts_with(L"UpdateBackup") ||
                        relStr.starts_with(L"Data") ||
                        relStr.starts_with(L"Patches") ||
                        relStr.starts_with(L"MidsReborn.exe.WebView2")
                        )
                        continue;

                    if (entry.is_regular_file())
                        filesToCopy.emplace_back(entry.path(), backupPath / relativePath);
                }
            }
            else if (patchType == L"Database")
            {
                if (dbName.empty())
                    throw std::runtime_error("Database name is required for db patch.");

                fs::path sourceDb = fs::path(installPath) / L"Data" / dbName;
                fs::path destDb = fs::path(backupPath) / L"Data" / dbName;

                if (!fs::exists(sourceDb))
                    throw std::runtime_error("Source DB folder does not exist: " + sourceDb.string());

                for (const auto& entry : fs::recursive_directory_iterator(sourceDb))
                {
                    if (entry.is_regular_file())
                    {
                        fs::path relativePath = fs::relative(entry.path(), sourceDb);
                        filesToCopy.emplace_back(entry.path(), destDb / relativePath);
                    }
                }
            }

            // UI: Show progress
            ModernUI::PostUpdateStatus(L"Creating snapshot...");
            ModernUI::PostShowProgressBar(true);
            ModernUI::PostUpdateProgress(0.0f);

            int current = 0;
            int total = static_cast<int>(filesToCopy.size());

            for (const auto& [source, dest] : filesToCopy)
            {
                fs::create_directories(dest.parent_path());
                fs::copy_file(source, dest, fs::copy_options::overwrite_existing);

                ++current;
                ModernUI::PostUpdateProgress(static_cast<float>(current) / total);
            }

            return true;
        }
        catch (const std::exception& ex)
        {
            Logger::Log(L"[PatchManager] CreateBackup failed: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
            return false;
        }
    }

    bool RestoreBackup(const std::wstring& installPath, const std::wstring& backupPath, const std::wstring& patchType, const std::wstring& dbName)
    {
        try
        {
            namespace fs = std::filesystem;

            ModernUI::PostUpdateStatus(L"Rolling back...");
            ModernUI::PostShowProgressBar(true);
            ModernUI::PostUpdateProgress(0.0f);

            std::vector<std::pair<fs::path, fs::path>> filesToCopy;

            if (patchType == L"Application")
            {
                // Step 1: Clean installPath (exclude important files/folders)
                for (const auto& entry : fs::recursive_directory_iterator(installPath, fs::directory_options::skip_permission_denied))
                {
                    fs::path relativePath = fs::relative(entry.path(), installPath);
                    std::wstring relStr = relativePath.wstring();

                    if (
                        relStr == L"appSettings.json" ||
                        relStr == L"MRBBootstrap.exe" ||
                        relStr.starts_with(L"Logs") ||
                        relStr.starts_with(L"UpdateStaging") ||
                        relStr.starts_with(L"UpdateBackup") ||
                        relStr.starts_with(L"Data") ||
                        relStr.starts_with(L"Patches")
                        )
                        continue;

                    std::error_code ec;
                    fs::remove_all(entry.path(), ec); // Non-fatal
                }

                // Step 2: Scan files from backup to restore
                for (const auto& entry : fs::recursive_directory_iterator(backupPath))
                {
                    if (entry.is_regular_file())
                    {
                        fs::path relativePath = fs::relative(entry.path(), backupPath);
                        fs::path dest = fs::path(installPath) / relativePath;
                        filesToCopy.emplace_back(entry.path(), dest);
                    }
                }
            }
            else if (patchType == L"Database")
            {
                if (dbName.empty())
                    throw std::runtime_error("Database name is required for db patch restore.");

                fs::path targetDbDir = fs::path(installPath) / L"Data" / dbName;
                fs::path backupDbDir = fs::path(backupPath) / L"Data" / dbName;

                if (!fs::exists(backupDbDir))
                    throw std::runtime_error("Backup DB folder not found: " + backupDbDir.string());

                // Step 1: Clean DB target
                std::error_code ec;
                fs::remove_all(targetDbDir, ec);

                // Step 2: Scan files from DB backup
                for (const auto& entry : fs::recursive_directory_iterator(backupDbDir))
                {
                    if (entry.is_regular_file())
                    {
                        fs::path relativePath = fs::relative(entry.path(), backupDbDir);
                        fs::path dest = targetDbDir / relativePath;
                        filesToCopy.emplace_back(entry.path(), dest);
                    }
                }
            }

            // Step 3: Perform file copy with UI progress
            int current = 0;
            int total = static_cast<int>(filesToCopy.size());

            for (const auto& [source, dest] : filesToCopy)
            {
                fs::create_directories(dest.parent_path());
                fs::copy_file(source, dest, fs::copy_options::overwrite_existing);
                ++current;
                ModernUI::PostUpdateProgress(static_cast<float>(current) / total);
            }

            return true;
        }
        catch (const std::exception& ex)
        {
            Logger::Log(L"[PatchManager] RestoreBackup failed: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
            return false;
        }
    }

    bool PerformRollback(const std::wstring& installPath, const std::wstring& backupPath, const std::wstring& patchType, const std::wstring& dbName)
    {
        if (!gInstalling)
        {
            Logger::Log(L"[PatchManager] Rollback skipped: install phase not started.");
            return false;
        }

        Logger::Log(L"[PatchManager] Performing rollback...");

        ModernUI::PostTriggerUpdateFailure();
        ModernUI::PostTriggerRollback();

        if (!RestoreBackup(installPath, backupPath, patchType, dbName))
        {
            Logger::Log(L"[PatchManager] Rollback failed.");
            ModernUI::PostUpdateStatus(L"Rollback failed.");
            return false;
        }

        ModernUI::PostUpdateStatus(L"Rollback Complete.");
        ModernUI::PostShowProgressBar(false);
        ModernUI::Delay(1500);
        return true;
    }

    int HandleUpdateTermination(const std::wstring& failureMessage)
    {
        std::wstring installPath = GetExecutableDir();

        if (gCancelled)
        {
            Logger::Log(L"[PatchManager] Update cancelled by user.");
            ModernUI::PostUpdateStatus(L"Update Cancelled.");
        }
        else
        {
            Logger::Log(L"[PatchManager] Update failed: " + failureMessage);
            ModernUI::PostUpdateStatus(L"Update Failed.");
        }

        ModernUI::PostShowProgressBar(false);
        ModernUI::PostUpdateProgress(0.0f);
        ModernUI::Delay(1500);

        if (gInstalling)
        {
            Logger::Log(L"[PatchManager] Installation was in progress. Performing rollback...");
            PerformRollback(
                installPath,
                gBackupPath,
                gPatchType,
                (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L"")
            );
        }

        if (gPreInstall)
        {
            if (gInstalling)
            {
                Logger::Log(L"[PatchManager] Pre-Installation cleanup was in progress. Performing rollback...");
                PerformRollback(
                    installPath,
                    gBackupPath,
                    gPatchType,
                    (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L"")
                );
            }
        }

        // Cleanup
        std::wstring mruPath = installPath + L"\\" + gPatchFile;
        std::wstring hashPath = installPath + L"\\" + GetHashFileNameFromPatch(gPatchFile);
        std::wstring stagingPath = installPath + L"\\UpdateStaging";
        std::wstring backupPath = installPath + L"\\UpdateBackup";

        CleanupPatchFiles(mruPath, hashPath, stagingPath, backupPath);

        ModernUI::Delay(1500);
        ModernUI::PostCloseUI();

        return -1;
    }

    bool PreInstallCleanup(const std::wstring& installPath, const std::wstring& patchType, const std::wstring& dbName)
    {
        namespace fs = std::filesystem;

        Logger::Log(L"[PatchManager] Starting pre-install cleanup...");
        Logger::Log(L"[PatchManager] installPath: " + installPath);
        Logger::Log(L"[PatchManager] patchType: " + patchType);
        Logger::Log(L"[PatchManager] dbName: " + dbName);

        try
        {
            std::vector<fs::path> toDelete;

            if (patchType == L"Application")
            {
                const std::vector<std::wstring> excludedPaths = {
                    L"appSettings.json",
                    L"MRBBootstrap.exe",
                    L"Logs",
                    L"Data",
                    L"UpdateStaging",
                    L"UpdateBackup",
                    L"Patches"
                };

                for (const auto& entry : fs::recursive_directory_iterator(installPath, fs::directory_options::skip_permission_denied))
                {
                    fs::path relativePath = fs::relative(entry.path(), installPath);
                    std::wstring relStr = relativePath.wstring();

                    Logger::Log(L"[PreInstallCleanup] Evaluating: " + relStr);

                    if (std::ranges::any_of(excludedPaths, [&](const std::wstring& p) {
                        return relStr == p || relStr.starts_with(p + L"\\");
                        }))
                    {
                        Logger::Log(L"[PreInstallCleanup] Skipped (excluded path): " + relStr);
                        continue;
                    }

                    toDelete.push_back(entry.path());
                }
            }
            else if (patchType == L"Database")
            {
                fs::path dbPath = fs::path(installPath) / L"Data" / dbName;
                Logger::Log(L"[PreInstallCleanup] Target DB directory: " + dbPath.wstring());

                if (!fs::exists(dbPath))
                {
                    Logger::Log(L"[PreInstallCleanup] DB directory not found, skipping cleanup.");
                    return true;
                }

                for (const auto& entry : fs::recursive_directory_iterator(dbPath, fs::directory_options::skip_permission_denied))
                {
                    fs::path relativePath = fs::relative(entry.path(), installPath);
                    std::wstring relStr = relativePath.wstring();

                    Logger::Log(L"[PreInstallCleanup] Evaluating: " + relStr);
                    toDelete.push_back(entry.path());
                }
            }

            // Reverse order deletion
            for (auto it = toDelete.rbegin(); it != toDelete.rend(); ++it)
            {
                std::error_code ec;
                fs::remove_all(*it, ec);

                std::wstring relStr = fs::relative(*it, installPath).wstring();

                if (ec)
                {
                    Logger::Log(L"[PreInstallCleanup] Failed to delete: " + relStr + L" - " +
                        std::wstring(ec.message().begin(), ec.message().end()));
                }
                else
                {
                    Logger::Log(L"[PreInstallCleanup] Deleted: " + relStr);
                }
            }

            Logger::Log(L"[PatchManager] Pre-install cleanup complete.");
            return true;
        }
        catch (const std::exception& ex)
        {
            Logger::Log(L"[PreInstallCleanup] Exception: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
            return false;
        }
    }

    bool IsApplicationPatch(const std::vector<FileEntry>& files)
    {
        // This can be enhanced in the future (e.g., check manifest metadata)
        return std::ranges::any_of(files, [](const FileEntry& f) {
            return _wcsicmp(f.FileName.c_str(), L"MidsReborn.exe") == 0;
            });
    }

    // --- RunSinglePatchFlow ---
    int RunSinglePatchFlow()
    {
        Downloader downloader;

        std::wstring workingDir = GetExecutableDir();
        std::wstring mruPath = workingDir + L"\\" + gPatchFile;
        std::wstring hashFileName = GetHashFileNameFromPatch(gPatchFile);
        std::wstring hashPath = workingDir + L"\\" + hashFileName;

        bool isAppPatch = (gPatchType == L"Application");

        std::wstring mruUrl = isAppPatch
            ? L"https://updates.midsreborn.com/" + gPatchFile
            : gHashBaseUrl + gPatchFile;

        std::wstring hashUrl = isAppPatch
            ? L"https://updates.midsreborn.com/" + hashFileName
            : gHashBaseUrl + hashFileName;

        // Step 1: Query and set version info
        DWORD fileSize = 0;
        if (std::filesystem::exists(mruPath))
        {
            if (Downloader::GetLocalFileSize(mruPath, fileSize))
                ModernUI::PostUpdateVersionInfo(FormatPatchInfoFromFields(gPatchType, L"", fileSize));
            else
                ModernUI::PostUpdateVersionInfo(gPatchType);
        }
        else
        {
            if (Downloader::GetRemoteFileSize(mruUrl, fileSize))
                ModernUI::PostUpdateVersionInfo(FormatPatchInfoFromFields(gPatchType, L"", fileSize));
            else
                ModernUI::PostUpdateVersionInfo(gPatchType);
        }

        // Step 2: Download patch and hash if needed
        if (!std::filesystem::exists(mruPath))
        {
            ModernUI::PostUpdateStatus(L"Downloading update...");
            ModernUI::PostShowProgressBar(true);

            if (!downloader.DownloadFile(mruUrl, mruPath, L"Downloading update...", true))
            {
                return HandleUpdateTermination(L"Failed to download patch.");
            }
        }

        ModernUI::Delay(1500);

        if (!std::filesystem::exists(hashPath))
        {
            ModernUI::PostUpdateStatus(L"Obtaining hashes...");
            ModernUI::PostShowProgressBar(true);

            if (!downloader.DownloadFile(hashUrl, hashPath, L"Obtaining hashes...", true))
            {
                return HandleUpdateTermination(L"Failed to download hash file.");
            }
        }

        ModernUI::Delay(1500);

        // Step 3: Decompress patch
        ModernUI::PostUpdateStatus(L"Preparing files...");
        ModernUI::PostShowProgressBar(false);

        auto files = PatchDecompressor::Decompress(mruPath);
        if (files.empty())
        {
            return HandleUpdateTermination(L"Failure occurred during decompression.");
        }

        ModernUI::Delay(1500);

        // Step 4: Create staging path
        std::wstring stagingBase = workingDir + L"\\UpdateStaging\\";
        std::filesystem::create_directories(stagingBase);

        std::wstring simpleName = GetSimplePatchDisplayNameVersion(gPatchFile);
        std::wstring stagingPath = stagingBase + simpleName;

        if (!PatchInstaller::WriteStagingFiles(files, stagingPath))
        {
            return HandleUpdateTermination(L"Failed to stage patch files.");
        }

        ModernUI::Delay(1500);

        // Step 5: Validate staged files
        ModernUI::PostUpdateStatus(L"Validating update...");
        ModernUI::PostShowProgressBar(true);

        if (!HashValidator::Validate(stagingPath, hashPath))
        {
            return HandleUpdateTermination(L"Failure occurred during patch validation.");
        }

        ModernUI::Delay(1500);

        // Step 6: Backup existing install
        std::wstring backupBase = workingDir + L"\\UpdateBackup";
        gBackupPath = backupBase + simpleName;

        if (!CreateBackup(workingDir, gBackupPath, gPatchType, (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L"")))
        {
            return HandleUpdateTermination(L"Failed to create backup snapshot.");
        }

        ModernUI::Delay(1500);

        // Step 7: Pre-Install Cleanup
        PreInstallCleanup(workingDir, gPatchType, (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L""));

        // Step 8: Install staged files
        ModernUI::PostUpdateStatus(L"Installing update...");
        ModernUI::PostShowProgressBar(true);

        KillMidsReborn();
        ModernUI::Delay(500);

        if (!PatchInstaller::ApplyStagedFiles(files, workingDir, stagingPath))
        {
            return HandleUpdateTermination(L"Failure occurred during patch installation.");
        }

        // Step 9: Cleanup
        ModernUI::PostUpdateStatus(L"Cleaning up...");
        ModernUI::PostShowProgressBar(false);

        CleanupPatchFiles(mruPath, hashPath, stagingBase, backupBase);

        ModernUI::Delay(1500);

        // Step 10: Finalize
        ModernUI::PostUpdateStatus(L"Update Complete.");
        ModernUI::PostShowProgressBar(false);
        ModernUI::Delay(1500);

        if (!gNoRestart)
        {
            ShellExecuteW(nullptr, L"open", L"midsreborn.exe", nullptr, workingDir.c_str(), SW_SHOW);
        }

        return 0;
    }


    // --- RunJsonDrivenPatchFlow ---
    int RunJsonDrivenPatchFlow(const std::wstring& jsonPath)
    {
        Downloader downloader;
        std::vector<UpdateEntry> updates;

        try
        {
            std::ifstream in(jsonPath);
            if (!in.is_open())
                return LogAndFail(L"Failed to open JSON payload.", true);

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
            return LogAndFail(L"Failed to open JSON payload.", true);
        }

        if (updates.empty())
        {
            return LogAndFail(L"Failed to open JSON payload.", true);
        }

        std::wstring workingDir = GetExecutableDir();
        std::filesystem::create_directories(workingDir);

        for (auto& update : updates)
        {
            ModernUI::SetShowProgressBar(false);
            gPatchType = ToW(update.Type);
            gPatchFile = ToW(update.File);

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

            // Query Version Info
            DWORD remoteFileSize = 0;
            if (Downloader::GetRemoteFileSize(mruUrl, remoteFileSize))
            {
                ModernUI::PostUpdateVersionInfo(FormatPatchInfoFromFields(ToW(update.Type), ToW(update.Version), remoteFileSize));
            }
            else
            {
                ModernUI::PostUpdateVersionInfo(ToW(update.Name) + L" v" + ToW(update.Version));
            }

            // Step 1: Download MRU
            ModernUI::PostUpdateStatus(L"Downloading update...");
            ModernUI::PostShowProgressBar(true);

            if (!downloader.DownloadFile(mruUrl, mruLocal, L"Downloading update...", true))
            {
                return HandleUpdateTermination(L"Failed to download patch.");
            }

            // Step 2: Download HASH
            ModernUI::PostUpdateStatus(L"Obtaining hashes...");

            if (!downloader.DownloadFile(hashUrl, hashLocal, L"Obtaining hashes...", true))
            {
                return HandleUpdateTermination(L"Failed to download hash file.");
            }

            update.LocalPath = mruLocal;

            KillMidsReborn();
            ModernUI::Delay(500);

            // Step 3: Decompress
            ModernUI::PostUpdateStatus(L"Preparing files...");
            ModernUI::PostShowProgressBar(false);

            auto files = PatchDecompressor::Decompress(update.LocalPath);
            if (files.empty())
            {
                return HandleUpdateTermination(L"Failure occurred during decompression.");
            }

            std::wstring stagingBase = workingDir + L"\\UpdateStaging\\";
            std::filesystem::create_directories(stagingBase);

            std::wstring simpleName = GetSimplePatchDisplayNameVersion(gPatchFile);
            std::wstring stagingPath = stagingBase + simpleName;

            // Step 4: Stage files
            if (!PatchInstaller::WriteStagingFiles(files, stagingPath))
            {
                return HandleUpdateTermination(L"Failed to stage patch files.");
            }

            ModernUI::Delay(1500);

            // Step 5: Validate
            ModernUI::PostUpdateStatus(L"Validating update...");
            ModernUI::PostShowProgressBar(true);

            std::wstring hashPath = update.LocalPath.substr(0, update.LocalPath.find_last_of(L'.')) + L".hash";

            if (!HashValidator::Validate(stagingPath, hashPath))
            {
                return HandleUpdateTermination(L"Failure occurred during patch validation.");
            }

            ModernUI::Delay(1500);

            // Step 6: Create Backup
            std::wstring installPath = ExpandEnv(update.TargetPath);
            std::wstring backupBase = workingDir + L"\\UpdateBackup\\";
            gBackupPath = backupBase + simpleName;

            if (!CreateBackup(installPath, gBackupPath, gPatchType, (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L"")))
            {
                return HandleUpdateTermination(L"Failed to create backup snapshot.");
            }

            // Step 7: Pre-Install Cleanup
            Logger::Log(L"[PatchManager] Initiating Pre-Install Cleanup.");
            if (!PreInstallCleanup(installPath, gPatchType, (gPatchType == L"db" ? GetSimplePatchDisplayName(gPatchFile) : L"")))
            {
                return HandleUpdateTermination(L"Pre-install cleanup failed.");
            }

            ModernUI::Delay(1500);

            // Step 8: Install
            ModernUI::PostUpdateStatus(L"Installing update...");
            ModernUI::PostShowProgressBar(true);

            if (!PatchInstaller::ApplyStagedFiles(files, installPath, stagingPath))
            {
                return HandleUpdateTermination(L"Failure occurred during patch installation.");
            }

            ModernUI::Delay(1500);

            // Step 9: Cleanup staging
            ModernUI::PostUpdateStatus(L"Cleaning up...");
            ModernUI::PostShowProgressBar(false);

            CleanupPatchFiles(mruLocal, hashLocal, stagingBase, backupBase);

            ModernUI::Delay(1500);
        }

        // Final step
        ModernUI::PostUpdateStatus(L"All updates complete.");
        ModernUI::PostShowProgressBar(false);
        
        try
        {
            if (std::filesystem::exists(jsonPath))
                std::filesystem::remove(jsonPath);
        }
        catch (...) {}

        ModernUI::Delay(1500);

        if (!gNoRestart)
            ShellExecuteW(nullptr, L"open", L"midsreborn.exe", nullptr, workingDir.c_str(), SW_SHOW);

        return 0;
    }
}
