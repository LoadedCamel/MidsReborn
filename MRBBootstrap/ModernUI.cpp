// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#include "ModernUI.h"
#include "PatchInstaller.h"

#include <algorithm>
#include <cmath>

#include "PatchManager.h"
#include "resource.h"

#pragma comment(lib, "Msimg32.lib")

using namespace Gdiplus;

// Static Members
HWND ModernUI::hWnd = nullptr;
std::wstring ModernUI::currentStatus = L"Initializing...";
std::wstring ModernUI::versionInfo = L"";
float ModernUI::currentProgress = 0.0f;
ULONG_PTR ModernUI::gdiplusToken = 0;

bool ModernUI::cancelHover = false;
RECT ModernUI::cancelButtonRect = { 150, 300, 250, 340 };

bool ModernUI::simulateDownload = true;
int ModernUI::simulationStage = 0;
float ModernUI::simulationProgress = 0.0f;

bool ModernUI::showProgressBar = false;
bool ModernUI::updateFailed = false;
bool ModernUI::rollbackInProgress = false;
bool ModernUI::fadingOut = false;
BYTE ModernUI::fadeOpacity = 255;

Bitmap* ModernUI::logoBitmap = nullptr;
Bitmap* ModernUI::plasmaBitmap = nullptr;
float ModernUI::plasmaScrollOffset = 0.0f;
float ModernUI::sparkleAngle = 0.0f;
ModernUI::CleanupContext ModernUI::cleanupContext;
static bool cancelButtonEnabled = true;

void ModernUI::Show(HINSTANCE hInstance)
{
    GdiplusStartupInput input;
    GdiplusStartup(&gdiplusToken, &input, nullptr);

    WNDCLASS wc = {};
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"ModernBootstrapper";
    wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
    wc.hCursor = LoadCursor(nullptr, IDC_ARROW);
    RegisterClass(&wc);

    hWnd = CreateWindowEx(
        WS_EX_LAYERED,
        wc.lpszClassName, L"",
        WS_POPUP,
        CW_USEDEFAULT, CW_USEDEFAULT, 400, 400,
        nullptr, nullptr, hInstance, nullptr);

    if (!hWnd)
        return;

    SetLayeredWindowAttributes(hWnd, 0, 255, LWA_ALPHA);

    HRGN hrgn = CreateRoundRectRgn(0, 0, 400, 400, 20, 20);
    SetWindowRgn(hWnd, hrgn, TRUE);

    RECT rcWork;
    SystemParametersInfo(SPI_GETWORKAREA, 0, &rcWork, 0);
    int x = rcWork.left + (rcWork.right - rcWork.left - 400) / 2;
    int y = rcWork.top + (rcWork.bottom - rcWork.top - 400) / 2;
    SetWindowPos(hWnd, nullptr, x, y, 400, 400, SWP_NOZORDER | SWP_NOSIZE);

    // Load embedded logo
    HRSRC hResource = FindResource(nullptr, MAKEINTRESOURCE(IDB_MIDS_LOGO), L"PNG");
    if (hResource)
    {
        HGLOBAL hLoaded = LoadResource(nullptr, hResource);
        if (hLoaded)
        {
            void* pResourceData = LockResource(hLoaded);
            DWORD size = SizeofResource(nullptr, hResource);
            if (pResourceData && size)
            {
                HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE, size);
                if (hGlobal)
                {
                    void* pBuffer = GlobalLock(hGlobal);
                    memcpy(pBuffer, pResourceData, size);
                    IStream* pStream = nullptr;
                    if (CreateStreamOnHGlobal(hGlobal, TRUE, &pStream) == S_OK)
                    {
                        logoBitmap = Bitmap::FromStream(pStream);
                        pStream->Release();
                    }
                }
            }
        }
    }

    // Load embedded plasma texture
    HRSRC hPlasma = FindResource(nullptr, MAKEINTRESOURCE(IDB_PLASMA_TEXTURE), L"PNG");
    if (hResource)
    {
        HGLOBAL hLoaded = LoadResource(nullptr, hPlasma);
        if (hLoaded)
        {
            void* pResourceData = LockResource(hLoaded);
            DWORD size = SizeofResource(nullptr, hPlasma);
            if (pResourceData && size)
            {
                HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE, size);
                if (hGlobal)
                {
                    void* pBuffer = GlobalLock(hGlobal);
                    memcpy(pBuffer, pResourceData, size);
                    IStream* pStream = nullptr;
                    if (CreateStreamOnHGlobal(hGlobal, TRUE, &pStream) == S_OK)
                    {
                        plasmaBitmap = Bitmap::FromStream(pStream);
                        pStream->Release();
                    }
                }
            }
        }
    }

    // --- Apply STRONG brightness boost immediately ---
    if (plasmaBitmap)
    {
        Bitmap* brightPlasma = new Bitmap(plasmaBitmap->GetWidth(), plasmaBitmap->GetHeight(), plasmaBitmap->GetPixelFormat());
        Graphics g(brightPlasma);
        ColorMatrix cm = {
    2.5f, 0, 0, 0, 0,  // Red * 2.5
    0, 2.5f, 0, 0, 0,  // Green * 2.5
    0, 0, 2.5f, 0, 0,  // Blue * 2.5
    0, 0, 0, 1.0f, 0,  // Alpha unchanged
    0, 0, 0, 0, 1.0f
        };
        ImageAttributes attr;
        attr.SetColorMatrix(&cm);
        g.DrawImage(plasmaBitmap, Rect(0, 0, plasmaBitmap->GetWidth(), plasmaBitmap->GetHeight()), 0, 0, plasmaBitmap->GetWidth(), plasmaBitmap->GetHeight(), UnitPixel, &attr);

        delete plasmaBitmap;
        plasmaBitmap = brightPlasma;
    }

    ShowWindow(hWnd, SW_SHOW);
    UpdateWindow(hWnd);

    SetTimer(hWnd, TIMER_ID, 33, nullptr);
}

void ModernUI::SetStatus(const std::wstring& text)
{
    currentStatus = text;
    InvalidateRect(hWnd, nullptr, TRUE);
    UpdateWindow(hWnd);
}

void ModernUI::SetVersionInfo(const std::wstring& versionText)
{
    versionInfo = versionText;
    InvalidateRect(hWnd, nullptr, TRUE);
}

void ModernUI::SetProgress(float percent)
{
    currentProgress = max(0.0f, min(1.0f, percent));
    InvalidateRect(hWnd, nullptr, FALSE);
}

void ModernUI::Shutdown()
{
    if (hWnd)
    {
        DestroyWindow(hWnd);
        hWnd = nullptr;
    }

    if (logoBitmap)
    {
        delete logoBitmap;
        logoBitmap = nullptr;
    }

    GdiplusShutdown(gdiplusToken);
}

void ModernUI::TriggerUpdateFailure()
{
    simulateDownload = false;
    updateFailed = true;
    showProgressBar = false;
    SetStatus(L"Update Failed.");
    SetTimer(hWnd, SHUTDOWN_TIMER_ID, 2000, nullptr);
}

void ModernUI::TriggerRollback()
{
    KillTimer(hWnd, SHUTDOWN_TIMER_ID);
    showProgressBar = true;
    SetProgress(0.0f);
    SetStatus(L"Rolling Back...");
}

void ModernUI::SetShowProgressBar(bool visible)
{
    showProgressBar = visible;
    InvalidateRect(hWnd, nullptr, FALSE);
}

void ModernUI::PostUpdateStatus(const std::wstring& text)
{
    PostMessage(hWnd, WM_UI_UPDATE_STATUS, 0, reinterpret_cast<LPARAM>(new std::wstring(text)));
}

void ModernUI::PostUpdateVersionInfo(const std::wstring& versionText)
{
    PostMessage(hWnd, WM_UI_UPDATE_VERSION, 0, reinterpret_cast<LPARAM>(new std::wstring(versionText)));
}

void ModernUI::PostShowProgressBar(bool visible)
{
    if (hWnd)
    {
        PostMessageW(hWnd, WM_UI_SHOW_PROGRESS_BAR, visible, 0);
    }
}

void ModernUI::PostUpdateProgress(float percent)
{
    float* copy = new float(percent);
    PostMessage(hWnd, WM_UI_UPDATE_PROGRESS, reinterpret_cast<WPARAM>(copy), 0);
}

void ModernUI::PostTriggerUpdateFailure()
{
    PostMessage(hWnd, WM_UI_TRIGGER_FAILURE, 0, 0);
}

void ModernUI::PostTriggerRollback()
{
    PostMessage(hWnd, WM_UI_TRIGGER_ROLLBACK, 0, 0);
}

void ModernUI::PostTriggerCancel()
{
    PostMessage(hWnd, WM_UI_TRIGGER_CANCEL, 0, 0);
}

void ModernUI::PostStartCleanup()
{
    PostMessage(hWnd, WM_UI_START_CLEANUP, 0, 0);
}

void ModernUI::PostCloseUI()
{
    PostMessage(hWnd, WM_UI_CLOSE_WINDOW, 0, 0);
}

void ModernUI::Delay(UINT milliseconds)
{
    MSG msg;
    DWORD endTime = GetTickCount() + milliseconds;

    while (GetTickCount() < endTime)
    {
        while (PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
        Sleep(1);
    }
}

LRESULT CALLBACK ModernUI::WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch (msg)
	{
	case WM_PAINT:
		{
			PAINTSTRUCT ps;
			HDC hdc = BeginPaint(hwnd, &ps);
			DrawUI(hdc);
			EndPaint(hwnd, &ps);
			return 0;
		}
	case WM_TIMER:
		{
			if (wParam == TIMER_ID)
			{
				plasmaScrollOffset += 5.0f; // Plasma speed
                InvalidateRect(hwnd, nullptr, FALSE);
			}
			return 0;
		}
    case WM_MOUSEMOVE:
    {
        int x = GET_X_LPARAM(lParam);
        int y = GET_Y_LPARAM(lParam);

        bool inside = (x >= cancelButtonRect.left && x <= cancelButtonRect.right &&
            y >= cancelButtonRect.top && y <= cancelButtonRect.bottom);

        if (cancelButtonEnabled && inside != cancelHover)
        {
            cancelHover = inside;
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        return 0;
    }
    case WM_LBUTTONDOWN:
    {
        int x = GET_X_LPARAM(lParam);
        int y = GET_Y_LPARAM(lParam);

        if (x >= cancelButtonRect.left && x <= cancelButtonRect.right &&
            y >= cancelButtonRect.top && y <= cancelButtonRect.bottom)
        {
            if (cancelButtonEnabled) // Only allow if still enabled
            {
                cancelButtonEnabled = false;      // <-- Disable it
                cancelHover = false;               // <-- Turn off hover
                InvalidateRect(hwnd, nullptr, TRUE); // Redraw the UI to grey it

                PatchManager::gCancelled = true;
                ModernUI::PostTriggerCancel();
            }
        }
        else
        {
            // Simulate dragging the window
            ReleaseCapture();
            SendMessage(hwnd, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
        return 0;
    }
	case WM_DESTROY:
		{
			PostQuitMessage(0);
			return 0;
		}
    case WM_UI_UPDATE_STATUS:
    {
        auto* text = reinterpret_cast<std::wstring*>(lParam);
        if (text)
        {
            currentStatus = *text;
            delete text;
            InvalidateRect(hwnd, nullptr, TRUE);
        }
        return 0;
    }
    case WM_UI_UPDATE_VERSION:
    {
        auto* text = reinterpret_cast<std::wstring*>(lParam);
        if (text)
        {
            versionInfo = *text;
            delete text;
            InvalidateRect(hwnd, nullptr, TRUE);
        }
        return 0;
    }
    case WM_UI_UPDATE_PROGRESS:
    {
        auto* percent = reinterpret_cast<float*>(wParam);
        if (percent)
        {
            currentProgress = std::clamp(*percent, 0.0f, 1.0f);
            delete percent;
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        return 0;
    }
    case WM_UI_SHOW_PROGRESS_BAR:
    {
        showProgressBar = static_cast<bool>(wParam);
        InvalidateRect(hwnd, nullptr, FALSE);
        return 0;
    }
    case WM_UI_TRIGGER_FAILURE:
    {
        simulateDownload = false;
        updateFailed = true;
        showProgressBar = false;
        currentStatus = L"Update Failed.";
        InvalidateRect(hwnd, nullptr, TRUE);
        SetTimer(hwnd, SHUTDOWN_TIMER_ID, 2000, nullptr);
        return 0;
    }
    case WM_UI_TRIGGER_ROLLBACK:
    {
        KillTimer(hwnd, SHUTDOWN_TIMER_ID);
        showProgressBar = true;
        currentProgress = 0.0f;
        currentStatus = L"Rolling Back...";
        InvalidateRect(hwnd, nullptr, TRUE);
        return 0;
    }
    case WM_UI_TRIGGER_CANCEL:
    {
        KillTimer(hwnd, SHUTDOWN_TIMER_ID);
        simulateDownload = false;
        rollbackInProgress = false;
        updateFailed = false;
        showProgressBar = false;
        currentProgress = 0.0f;
        currentStatus = L"Cancelling update...";
        InvalidateRect(hwnd, nullptr, TRUE);
        return 0;
    }
    case WM_UI_START_CLEANUP:
		{
        // Step 1: Show "Cleaning up..."
        currentStatus = L"Cleaning up...";
        showProgressBar = false;
        InvalidateRect(hWnd, nullptr, TRUE);
        UpdateWindow(hWnd);

        // Step 2: Do the actual cleanup
        PatchManager::CleanupPatchFiles(
            cleanupContext.mruPath,
            cleanupContext.hashPath,
            cleanupContext.stagingPath,
            cleanupContext.backupPath
        );

        // Step 3: Update UI to show "Cleanup complete."
        currentStatus = L"Cleanup Complete.";
        showProgressBar = false;
        InvalidateRect(hWnd, nullptr, TRUE);
        UpdateWindow(hWnd);

        return 0;
    }
    case WM_UI_CLOSE_WINDOW:
    {
        DestroyWindow(hwnd);
        return 0;
    }
	default:
		return DefWindowProc(hwnd, msg, wParam, lParam);
	}
}

void ModernUI::DrawUI(HDC hdc)
{
    Bitmap buffer(400, 400);
    Graphics gBuffer(&buffer);
    gBuffer.SetSmoothingMode(SmoothingModeHighQuality);

    // 1. Background Gradient
    LinearGradientBrush bgBrush(
        Point(0, 0), Point(0, 400),
        Color(255, 5, 15, 30),   // Dark navy blue
        Color(255, 0, 0, 0)      // Deeper black
    );
    gBuffer.FillRectangle(&bgBrush, 0, 0, 400, 400);

    // 2. Neon Border
    Pen borderPen(Color(150, 0, 150, 255), 2);

    GraphicsPath borderPath;
    borderPath.StartFigure();

    borderPath.AddArc(2, 2, 20, 20, 180, 90); // Top-left
    borderPath.AddLine(12, 2, 388, 2);         // Top
    borderPath.AddArc(378, 2, 20, 20, 270, 90); // Top-right
    borderPath.AddLine(398, 12, 398, 388);     // Right
    borderPath.AddArc(378, 378, 20, 20, 0, 90); // Bottom-right
    borderPath.AddLine(388, 398, 12, 398);     // Bottom
    borderPath.AddArc(2, 378, 20, 20, 90, 90);  // Bottom-left
    borderPath.AddLine(2, 388, 2, 12);          // Left

    borderPath.CloseFigure();
    gBuffer.DrawPath(&borderPen, &borderPath);

    // 3. Logo Image (scaled to 220px width, centered at top)
    if (logoBitmap)
    {
        int logoWidth = logoBitmap->GetWidth();
        int logoHeight = logoBitmap->GetHeight();
        float scale = 220.0f / logoWidth;

        int drawWidth = static_cast<int>(logoWidth * scale);
        int drawHeight = static_cast<int>(logoHeight * scale);

        int x = (400 - drawWidth) / 2;
        int y = 5;

        gBuffer.DrawImage(logoBitmap, x, y, drawWidth, drawHeight);
    }

    // 4. Status Text (e.g., "Checking for updates...")
    FontFamily fontFamily(L"Segoe UI");
    Font statusFont(&fontFamily, 16, FontStyleRegular, UnitPixel);
    SolidBrush statusBrush(Color(220, 210, 230, 255)); // Light gray

    StringFormat centerFormat;
    centerFormat.SetAlignment(StringAlignmentCenter);
    centerFormat.SetLineAlignment(StringAlignmentNear);

    gBuffer.DrawString(
        currentStatus.c_str(), -1, &statusFont,
        RectF(0, 100, 400, 30), &centerFormat, &statusBrush
    );

    // 5. Progress Donut (perfect plasma ring)
    if (showProgressBar && plasmaBitmap)
    {
        float cx = 200.0f, cy = 200.0f;
        float outerRadius = 70.0f;
        float innerRadius = 55.0f; // Thin ring: 10px

        float startAngle = -90.0f;
        float sweepAngle = 360.0f * currentProgress;

        gBuffer.SetSmoothingMode(SmoothingModeAntiAlias);

        // --- 1. Draw Background Grey Ring ---
        Pen backgroundPen(Color(60, 100, 100, 120), outerRadius - innerRadius); // 60 alpha darker-grey
        backgroundPen.SetStartCap(LineCapRound);
        backgroundPen.SetEndCap(LineCapRound);

        gBuffer.DrawArc(&backgroundPen,
            cx - outerRadius, cy - outerRadius,
            outerRadius * 2.0f, outerRadius * 2.0f,
            0.0f, 360.0f); // Full circle

        // --- 2. Plasma TextureBrush ---
        float trueOffset = fmod(plasmaScrollOffset, plasmaBitmap->GetWidth() * 3.0f);

        TextureBrush plasmaBrush(plasmaBitmap, WrapModeTile);
        plasmaBrush.ScaleTransform(0.35f, 0.35f); // More detailed plasma
        plasmaBrush.TranslateTransform(-trueOffset, -trueOffset * 0.75f); // Diagonal drift

        Pen plasmaPen(&plasmaBrush, outerRadius - innerRadius);
        plasmaPen.SetStartCap(LineCapRound);
        plasmaPen.SetEndCap(LineCapRound);

        gBuffer.DrawArc(&plasmaPen,
            cx - outerRadius, cy - outerRadius,
            outerRadius * 2.0f, outerRadius * 2.0f,
            startAngle, sweepAngle);

        // --- 3. Centered Percent Text ---
        FontFamily fontFamily(L"Segoe UI");
        Font percentFont(&fontFamily, 24, FontStyleBold, UnitPixel);
        SolidBrush percentBrush(Color(255, 0, 220, 255)); // Bright cyan

        wchar_t percentText[32];
        swprintf_s(percentText, L"%.0f%%", currentProgress * 100.0f);

        StringFormat centerFormat;
        centerFormat.SetAlignment(StringAlignmentCenter);
        centerFormat.SetLineAlignment(StringAlignmentCenter);

        gBuffer.DrawString(
            percentText, -1, &percentFont,
            RectF(0, cy - 20, 400, 40), &centerFormat, &percentBrush
        );
    }

    // 6. Version Info (small at bottom)
    Font versionFont(&fontFamily, 14, FontStyleRegular, UnitPixel);
    SolidBrush versionBrush(Color(200, 210, 240, 255));

    gBuffer.DrawString(
        versionInfo.c_str(), -1, &versionFont,
        RectF(0, 290, 400, 20), &centerFormat, &versionBrush
    );

    // 7. Cancel Button (centered at bottom)
    GraphicsPath buttonPath;
    buttonPath.AddArc(140, 330, 20, 40, 90, 180);
    buttonPath.AddArc(240, 330, 20, 40, 270, 180);
    buttonPath.CloseFigure();

    Color buttonColor;

    if (!cancelButtonEnabled)
    {
        // Greyed out when disabled
        buttonColor = Color(255, 100, 100, 100);
    }
    else
    {
        buttonColor = cancelHover ? Color(255, 30, 130, 255) : Color(255, 40, 70, 180);
    }

    //Color buttonColor = cancelHover ? Color(255, 20, 80, 180) : Color(255, 40, 70, 140);
    LinearGradientBrush buttonBrush(
        Point(140, 330), Point(260, 370),
        buttonColor,
        Color(255, 30, 60, 120)
    );
    gBuffer.FillPath(&buttonBrush, &buttonPath);

    Pen buttonBorderPen(Color(255, 0, 180, 255), 2);
    gBuffer.DrawPath(&buttonBorderPen, &buttonPath);

    Font buttonFont(&fontFamily, 14, FontStyleBold, UnitPixel);
    SolidBrush buttonTextBrush(Color(255, 255, 255, 255));

    StringFormat buttonFormat;
    buttonFormat.SetAlignment(StringAlignmentCenter);
    buttonFormat.SetLineAlignment(StringAlignmentCenter);

    gBuffer.DrawString(
        L"Cancel", -1, &buttonFont,
        RectF(140, 330, 120, 40), &buttonFormat, &buttonTextBrush
    );

    cancelButtonRect = { 140, 330, 140 + 120, 330 + 40 };

    // Finally blit buffer to window
    Graphics gWindow(hdc);
    gWindow.DrawImage(&buffer, 0, 0);
}
