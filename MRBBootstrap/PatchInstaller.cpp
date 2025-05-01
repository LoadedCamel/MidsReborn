// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "PatchInstaller.h"
#include "PatchManager.h"
#include "Logger.h"

#include <filesystem>
#include <fstream>
#include <windows.h>

#include "ModernUI.h"

namespace fs = std::filesystem;

bool PatchInstaller::WriteStagingFiles(const std::vector<FileEntry>& files, const std::wstring& stagingPath)
{
    const fs::path staging = stagingPath;

    try {
        create_directories(staging);

        for (const auto& [Directory, FileName, Data] : files)
        {
            if (PatchManager::gCancelled)
            {
                Logger::Log(L"[Installer] Cancelled while writing staged files.");
                return false;
            }

            fs::path staged = staging / Directory / FileName;
            create_directories(staged.parent_path());

            // Write file to staging
            std::ofstream out(staged, std::ios::binary);
            if (!out.is_open())
            {
                Logger::Log(L"[Installer] Failed to open file for writing: " + staged.wstring());
                return false;
            }

            out.write(reinterpret_cast<const char*>(Data.data()), Data.size());
            out.close();

            // Verify file size after write
            const std::uintmax_t actual_size = fs::exists(staged) ? fs::file_size(staged) : 0;

            // Optional: compare in-memory and on-disk size
            if (actual_size != Data.size())
            {
                Logger::Log(L"[Installer] WARNING: File size mismatch for " + staged.wstring());
            }
        }

        return true;
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"[Installer] Exception while writing staged files: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        return false;
    }
}

bool PatchInstaller::ApplyStagedFiles(const std::vector<FileEntry>& files, const std::wstring& installPath, const std::wstring& stagingPath)
{
    PatchManager::gInstalling = true; // Signal: installation in progress
    const fs::path staging = stagingPath;

    try {
        ModernUI::PostUpdateStatus(L"Installing...");
        ModernUI::PostShowProgressBar(true);
        ModernUI::PostUpdateProgress(0.0f);

        int index = 0;
        const int total = static_cast<int>(files.size());

        // Install staged files
        for (const auto& file : files)
        {
            if (PatchManager::gCancelled)
            {
                Logger::Log(L"[Installer] Cancelled during file install phase.");
                return false;
            }

            fs::path target = fs::path(installPath) / file.Directory / file.FileName;
            fs::path staged = staging / file.Directory / file.FileName;

            if (file.Directory == L"Logs")
            {
                continue;
            }

            create_directories(target.parent_path());

            if (!MoveFileExW(staged.c_str(), target.c_str(), MOVEFILE_REPLACE_EXISTING))
            {
                DWORD lastError = GetLastError();

                Logger::Log(L"[Installer] Failed to replace: " + staged.wstring() + L" -> " + target.wstring());
                Logger::Log(L"[Installer] MoveFileExW error code: " + std::to_wstring(lastError));
                PatchManager::gInstalling = false;
                return false;
            }

            ModernUI::PostUpdateProgress(static_cast<float>(++index) / static_cast<float>(total));
        }

        PatchManager::gInstalling = false;
        return true;
    }
    catch (const std::exception& ex)
    {
        Logger::Log(L"[Installer] Exception during install: " + std::wstring(ex.what(), ex.what() + strlen(ex.what())));
        PatchManager::gInstalling = false;
        return false;
    }
}
