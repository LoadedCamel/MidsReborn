#include "BootstrapperUI.h"
#include "resource.h"

#include <windows.h>
#include <CommCtrl.h>
#include <string>
#include <gdiplus.h>

#pragma comment(lib, "Comctl32.lib")
#pragma comment(lib, "gdiplus.lib")

using namespace Gdiplus;

// Global UI handles
HWND hWndMain = nullptr;
HWND hWndStatus = nullptr;
HWND hWndTaskLabel = nullptr;
HWND hWndProgress = nullptr;
HWND hWndFileName = nullptr;
HFONT hFont = nullptr;
HBRUSH hBackgroundBrush = nullptr;
ULONG_PTR gdiplusToken = 0;
Bitmap* gLogo = nullptr;

// Forward declarations
LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);

void InitGDIPlus()
{
    GdiplusStartupInput gdiplusStartupInput;
    GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, nullptr);
}

void ShutdownGDIPlus()
{
    if (gLogo)
    {
        delete gLogo;
        gLogo = nullptr;
    }
    GdiplusShutdown(gdiplusToken);
}

void LoadLogoFromResource(HINSTANCE hInstance)
{
    HRSRC hRes = FindResource(hInstance, MAKEINTRESOURCE(IDB_MRB_LOGO), L"PNG");
    if (!hRes) return;

    DWORD size = SizeofResource(hInstance, hRes);
    HGLOBAL hMem = LoadResource(hInstance, hRes);
    void* pImage = LockResource(hMem);

    IStream* pStream = nullptr;
    CreateStreamOnHGlobal(nullptr, TRUE, &pStream);
    pStream->Write(pImage, size, nullptr);
    pStream->Seek({ 0 }, STREAM_SEEK_SET, nullptr);

    gLogo = new Bitmap(pStream);
    pStream->Release();
}

void BootstrapperUI::Show(HINSTANCE hInstance)
{
    InitCommonControls();
    InitGDIPlus();
    LoadLogoFromResource(hInstance);

    const wchar_t CLASS_NAME[] = L"BootstrapperWindow";

    hBackgroundBrush = CreateSolidBrush(RGB(35, 35, 38));
    hFont = CreateFont(18, 0, 0, 0, FW_NORMAL, FALSE, FALSE, FALSE,
        ANSI_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, DEFAULT_QUALITY,
        DEFAULT_PITCH | FF_SWISS, L"Segoe UI");

    WNDCLASS wc = {};
    wc.lpfnWndProc = WndProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = CLASS_NAME;
    wc.hbrBackground = hBackgroundBrush;
    wc.hCursor = LoadCursor(nullptr, IDC_ARROW);
    wc.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MRBBOOTSTRAP));
    RegisterClass(&wc);

    hWndMain = CreateWindowEx(
        0, CLASS_NAME, L"Mids Reborn Updater",
        WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU,
        CW_USEDEFAULT, CW_USEDEFAULT, 520, 220,
        nullptr, nullptr, hInstance, nullptr
    );

    if (!hWndMain) return;

    hWndStatus = CreateWindow(L"STATIC", L"Mids will automatically restart when update is complete.",
        WS_VISIBLE | WS_CHILD | SS_CENTER,
        80, 10, 400, 20,
        hWndMain, nullptr, hInstance, nullptr);
    SendMessage(hWndStatus, WM_SETFONT, (WPARAM)hFont, TRUE);

    hWndTaskLabel = CreateWindow(L"STATIC", L"Preparing...",
        WS_VISIBLE | WS_CHILD,
        80, 40, 400, 20,
        hWndMain, nullptr, hInstance, nullptr);
    SendMessage(hWndTaskLabel, WM_SETFONT, (WPARAM)hFont, TRUE);

    hWndProgress = CreateWindowEx(0, PROGRESS_CLASS, nullptr,
        WS_VISIBLE | WS_CHILD,
        80, 60, 400, 20,
        hWndMain, nullptr, hInstance, nullptr);
    SendMessage(hWndProgress, PBM_SETRANGE, 0, MAKELPARAM(0, 100));

    hWndFileName = CreateWindow(L"STATIC", L"",
        WS_VISIBLE | WS_CHILD | SS_CENTER,
        80, 100, 400, 20,
        hWndMain, nullptr, hInstance, nullptr);
    SendMessage(hWndFileName, WM_SETFONT, (WPARAM)hFont, TRUE);

    ShowWindow(hWndMain, SW_SHOW);
    UpdateWindow(hWndMain);
}

void BootstrapperUI::UpdateStatus(const std::wstring& status)
{
    if (hWndStatus)
        SetWindowTextW(hWndStatus, status.c_str());
}

void BootstrapperUI::UpdateFileName(const std::wstring& filename)
{
    if (hWndFileName)
        SetWindowTextW(hWndFileName, filename.c_str());
}

void BootstrapperUI::SetProgressLabel(const std::wstring& text)
{
    if (hWndTaskLabel)
        SetWindowTextW(hWndTaskLabel, text.c_str());
}

void BootstrapperUI::SetProgress(int current, int total)
{
    if (hWndProgress && total > 0)
    {
        int percent = static_cast<int>((100.0 * current) / total);
        SendMessage(hWndProgress, PBM_SETPOS, percent, 0);
    }
}

void BootstrapperUI::SetProgressPercent(int percent)
{
    if (hWndProgress)
        SendMessage(hWndProgress, PBM_SETPOS, percent, 0);
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_CTLCOLORSTATIC:
    {
        HDC hdcStatic = (HDC)wParam;
        SetTextColor(hdcStatic, RGB(220, 220, 220));
        SetBkColor(hdcStatic, RGB(35, 35, 38));
        return (INT_PTR)hBackgroundBrush;
    }
    case WM_PAINT:
    {
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hwnd, &ps);
        Graphics g(hdc);
        if (gLogo)
            g.DrawImage(gLogo, 20, 40, 48, 48);
        EndPaint(hwnd, &ps);
        return 0;
    }
    case WM_DESTROY:
        ShutdownGDIPlus();
        DeleteObject(hBackgroundBrush);
        DeleteObject(hFont);
        PostQuitMessage(0);
        return 0;
    default:
        return DefWindowProc(hwnd, msg, wParam, lParam);
    }
}
