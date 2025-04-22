#pragma once
#include <string>

#include "windows.h"

namespace BootstrapperUI
{
    void Show(HINSTANCE hInstance);
    void UpdateStatus(const std::wstring& status);
    void UpdateFileName(const std::wstring& filename);
    void SetProgressLabel(const std::wstring& text);
    void SetProgress(int current, int total);
    void SetProgressPercent(int percent);
}
