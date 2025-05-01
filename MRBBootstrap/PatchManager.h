// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once

#include <optional>
#include <string>
#include <vector>

struct FileEntry;

namespace PatchManager
{
    //
    // ======= Public API =======
    //

    int LogAndFail(const std::wstring& message, bool showToUser = false);
    int RunSinglePatchFlow();
    int RunJsonDrivenPatchFlow(const std::wstring& jsonPath);

    void CleanupPatchFiles(
        const std::wstring& mruPath,
        const std::wstring& hashPath,
        const std::optional<std::wstring>& stagingPath = std::nullopt,
        const std::optional<std::wstring>& backupPath = std::nullopt
    );

    bool CreateBackup(
        const std::wstring& installPath,
        const std::wstring& backupPath,
        const std::wstring& patchType,
        const std::wstring& dbName = L""
    );
    bool RestoreBackup(
        const std::wstring& installPath,
        const std::wstring& backupPath,
        const std::wstring& patchType,
        const std::wstring& dbName = L""
    );
    bool PerformRollback(
        const std::wstring& installPath,
        const std::wstring& backupPath,
        const std::wstring& patchType,
        const std::wstring& dbName = L""
    );

    bool IsApplicationPatch(const std::vector<FileEntry>& files);
    bool PreInstallCleanup(const std::wstring& installPath, const std::wstring& patchType, const std::wstring& dbName = L"");

    //
    // ======= Internal Helpers =======
    //

    std::wstring ExpandEnv(const std::string& path);
    std::wstring GetExecutableDir();
    std::wstring GetHashFileNameFromPatch(const std::wstring& patchName);

    //
    // ======= Global Cancel Handling =======
    //

    extern volatile bool gCancelled;
    extern volatile bool gInstalling;
    extern volatile bool gPreInstall;

    int HandleUpdateTermination(const std::wstring& failureMessage);
}
