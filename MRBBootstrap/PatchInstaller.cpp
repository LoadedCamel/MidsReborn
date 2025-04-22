#include "PatchInstaller.h"
#include "Logger.h"
#include "BootstrapperUI.h"

#include <filesystem>
#include <fstream>
#include <unordered_set>
#include <windows.h>

namespace fs = std::filesystem;

static bool BackupFile(const fs::path& target, const fs::path& backup)
{
    try {
        if (exists(target))
        {
            create_directories(backup.parent_path());
            copy_file(target, backup, fs::copy_options::overwrite_existing);
        }
        return true;
    }
    catch (...)
    {
        Logger::Log(L"[Installer] Failed to backup: " + target.wstring());
        return false;
    }
}

static bool RestoreBackup(const fs::path& backupDir, const fs::path& installDir)
{
    try {
        for (auto& entry : fs::recursive_directory_iterator(backupDir))
        {
            if (entry.is_regular_file())
            {
                fs::path relative = fs::relative(entry.path(), backupDir);
                fs::path dest = installDir / relative;
                create_directories(dest.parent_path());
                copy_file(entry.path(), dest, fs::copy_options::overwrite_existing);
            }
        }
        return true;
    }
    catch (...)
    {
        Logger::Log(L"[Installer] Rollback failed.");
        return false;
    }
}

bool PatchInstaller::IsApplicationPatch(const std::vector<FileEntry>& files)
{
    // This can be enhanced in the future (e.g., check manifest metadata)
    return std::ranges::any_of(files, [](const FileEntry& f) {
        return _wcsicmp(f.FileName.c_str(), L"MidsReborn.exe") == 0;
        });
}

bool PatchInstaller::WriteStagingFiles(const std::vector<FileEntry>& files, const std::wstring& stagingPath)
{
    const fs::path staging = stagingPath;

    try {
        create_directories(staging);

        BootstrapperUI::SetProgressLabel(L"Staging files...");
        BootstrapperUI::SetProgress(0, static_cast<int>(files.size()));

        for (const auto& [Directory, FileName, Data] : files)
        {
            fs::path staged = staging / Directory / FileName;
            create_directories(staged.parent_path());

            // Write file to staging
            std::ofstream out(staged, std::ios::binary);
            out.write(reinterpret_cast<const char*>(Data.data()), Data.size());
            out.close();

            // Verify file size after write
            const std::uintmax_t actual_size = fs::exists(staged) ? fs::file_size(staged) : 0;

            // Optional: compare in-memory and on-disk size
            if (actual_size != Data.size())
            {
                Logger::Log(L"[Installer] WARNING: File size mismatch for " + staged.wstring());
            }

            static int index = 0;
            BootstrapperUI::UpdateFileName(FileName);
            BootstrapperUI::SetProgress(++index, static_cast<int>(files.size()));
        }

        return true;
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"[Installer] Exception while writing staged files: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        return false;
    }
}

void PatchInstaller::CleanStaleFiles(const std::wstring& installPath, const std::vector<FileEntry>& patchFiles)
{
    namespace fs = std::filesystem;

    static const std::vector<std::wstring> excluded_paths = {
        L"Data",
        L"Logs",
        L"UpdateStaging",
        L"UpdateBackup",
        L"appSettings.json"
    };

    std::unordered_set<std::wstring> validPaths;

    // Normalize and store all valid paths from the patch
    for (const auto& file : patchFiles)
    {
        fs::path full = fs::path(installPath) / file.Directory / file.FileName;
        validPaths.insert(fs::weakly_canonical(full).wstring());
    }

    // Walk the install dir looking for unknown files
    for (const auto& entry : fs::recursive_directory_iterator(installPath))
    {
        if (!entry.is_regular_file())
            continue;

        std::wstring relativePath = fs::relative(entry.path(), installPath).wstring();

        // Check if path is excluded
        bool isExcluded = std::ranges::any_of(excluded_paths, [&](const std::wstring& p) {
            return relativePath.starts_with(p) || relativePath == p;
            });

        if (isExcluded)
            continue;

        // Canonicalize for comparison
        std::wstring canonicalPath = fs::weakly_canonical(entry.path()).wstring();
        if (!validPaths.contains(canonicalPath))
        {
            Logger::Log(L"[Installer] Removing stale file: " + canonicalPath);
            std::error_code ec;
            fs::remove(entry.path(), ec);
            if (ec)
                Logger::Log(L"[Installer] Failed to remove: " + canonicalPath + L" - " + std::wstring(ec.message().begin(), ec.message().end()));
        }
    }
}

bool PatchInstaller::ApplyStagedFiles(const std::vector<FileEntry>& files, const std::wstring& installPath, const std::wstring& stagingPath)
{
    const fs::path staging = stagingPath;
    const fs::path backup = installPath + L"\\UpdateBackup";

    try {
        create_directories(backup);

        BootstrapperUI::SetProgressLabel(L"Installing patch...");
        BootstrapperUI::SetProgress(0, static_cast<int>(files.size()));

        for (const auto& file : files)
        {
            std::filesystem::path target = std::filesystem::path(installPath) / file.Directory / file.FileName;
            std::filesystem::path backupFile = backup / file.Directory / file.FileName;

            if (!BackupFile(target, backupFile))
            {
                RestoreBackup(backup, installPath);
                return false;
            }
        }

        if (IsApplicationPatch(files)) {
            Logger::Log(L"[Installer] Detected application patch. Cleaning stale files...");
        	CleanStaleFiles(installPath, files);
        }

        int index = 0;
        for (const auto& file : files)
        {
            std::filesystem::path target = std::filesystem::path(installPath) / file.Directory / file.FileName;
            std::filesystem::path staged = staging / file.Directory / file.FileName;

            create_directories(target.parent_path());

            if (!MoveFileExW(staged.c_str(), target.c_str(), MOVEFILE_REPLACE_EXISTING))
            {
                Logger::Log(L"[Installer] Failed to replace: " + target.wstring());
                RestoreBackup(backup, installPath);
                return false;
            }

            BootstrapperUI::UpdateFileName(file.FileName);
            BootstrapperUI::SetProgress(++index, static_cast<int>(files.size()));
        }

        remove_all(staging);
        remove_all(backup);

        return true;
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"[Installer] Exception during install: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        RestoreBackup(backup, installPath);
        return false;
    }
}
