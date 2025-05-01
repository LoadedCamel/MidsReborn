// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include <windows.h>
#include <windowsx.h>
#include <filesystem>

#include "MRBBootstrap.h"
#include "PatchManager.h"
#include "ModernUI.h"
#include "Logger.h"

#include <string>

// Global flags and paths
HINSTANCE hInst;
bool gSilent = false;
bool gNoRestart = false;
std::wstring gPatchFile;
std::wstring gPatchType;
std::wstring gHashBaseUrl;
bool gUseJsonPayload = false;
std::wstring gJsonPayloadPath;

// Forward declarations
DWORD WINAPI UIPumpThread(LPVOID lpParam);
DWORD WINAPI WorkerThread(LPVOID lpParam);

// Simple utility
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

// Main WinMain
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

    Logger::Init(PatchManager::GetExecutableDir() + L"\\Logs\\update.log");

    if (gSilent)
    {
        return gUseJsonPayload
            ? PatchManager::RunJsonDrivenPatchFlow(gJsonPayloadPath)
            : PatchManager::RunSinglePatchFlow();
    }

    // Start UI thread
    HANDLE hUIThread = CreateThread(nullptr, 0, UIPumpThread, nullptr, 0, nullptr);
    if (!hUIThread)
        return FailWithError(L"Failed to start UI thread.");

    // Start Worker thread
    HANDLE hWorkerThread = CreateThread(nullptr, 0, WorkerThread, nullptr, 0, nullptr);
    if (!hWorkerThread)
        return FailWithError(L"Failed to start worker thread.");

    // Wait for worker to finish
    WaitForSingleObject(hWorkerThread, INFINITE);

    // Tell UI to close
    PostMessage(ModernUI::hWnd, WM_CLOSE, 0, 0);

    // Wait for UI to exit
    WaitForSingleObject(hUIThread, INFINITE);

    return 0;
}

// UI Thread
DWORD WINAPI UIPumpThread(LPVOID lpParam)
{
    ModernUI::Show(hInst);

    MSG msg = {};
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return 0;
}

// Worker Thread
DWORD WINAPI WorkerThread(LPVOID lpParam)
{
    if (gUseJsonPayload)
        return PatchManager::RunJsonDrivenPatchFlow(gJsonPayloadPath);
    return PatchManager::RunSinglePatchFlow();
}
