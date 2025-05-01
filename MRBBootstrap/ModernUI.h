// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once
#include <windows.h>
#include <windowsx.h>
#include <objidl.h> 
#include <gdiplus.h>
#include <optional>
#include <string>
#include "ModernUIMessages.h"


class ModernUI
{
public:
    static HWND hWnd;

    static void Show(HINSTANCE hInstance);
    static void SetStatus(const std::wstring& text);
    static void SetVersionInfo(const std::wstring& versionText);
    static void SetProgress(float percent);
    static void Shutdown();
    static void TriggerUpdateFailure();
    static void TriggerRollback();
    static void SetShowProgressBar(bool visible);

    static void PostUpdateStatus(const std::wstring& text);
    static void PostUpdateVersionInfo(const std::wstring& versionText);
    static void PostShowProgressBar(bool visible);
    static void PostUpdateProgress(float percent);
    static void PostTriggerUpdateFailure();
    static void PostTriggerRollback();
    static void PostTriggerCancel();
    static void PostStartCleanup();
    static void PostCloseUI();
    static void Delay(UINT milliseconds);

private:
    static std::wstring currentStatus;
    static std::wstring versionInfo;
    static float currentProgress;
    static ULONG_PTR gdiplusToken;

    static int glowAlpha;
    static bool glowIncreasing;
    static int glowPulseSpeed;

    static constexpr UINT_PTR TIMER_ID = 1;
    static constexpr UINT_PTR SHUTDOWN_TIMER_ID = 2;

    static bool cancelHover;
    static RECT cancelButtonRect;

    static bool simulateDownload;
    static int simulationStage;
    static float simulationProgress;
    static bool showProgressBar;
    static bool updateFailed;
    static bool rollbackInProgress;
    static bool fadingOut;
    static BYTE fadeOpacity;
    static float plasmaScrollOffset;

    static Gdiplus::Bitmap* logoBitmap;
    static Gdiplus::Bitmap* plasmaBitmap;

    static float sparkleAngle; // Sparkle rotation

    struct CleanupContext
    {
        std::wstring mruPath;
        std::wstring hashPath;
        std::optional<std::wstring> stagingPath;
        std::optional<std::wstring> backupPath;
    };
    static CleanupContext cleanupContext;

    static LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
    static void DrawUI(HDC hdc);
};
