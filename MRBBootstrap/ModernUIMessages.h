// Copyright (c) 2025 Jason Thompson
// All rights reserved.
//
// This file is part of a proprietary software package.
// Unauthorized copying, modification, or distribution is strictly prohibited.
// For license information, see the LICENSE.txt file or contact jason@metalios.dev.


#pragma once

constexpr UINT WM_UI_UPDATE_STATUS = WM_APP + 1;
constexpr UINT WM_UI_UPDATE_VERSION = WM_APP + 2;
constexpr UINT WM_UI_UPDATE_PROGRESS = WM_APP + 3;
constexpr UINT WM_UI_SHOW_PROGRESS_BAR = WM_APP + 4;
constexpr UINT WM_UI_TRIGGER_FAILURE = WM_APP + 5;
constexpr UINT WM_UI_TRIGGER_ROLLBACK = WM_APP + 6;
constexpr UINT WM_UI_START_CLEANUP = WM_APP + 7;
constexpr UINT WM_UI_CLOSE_WINDOW = WM_APP + 8;
constexpr UINT WM_UI_TRIGGER_CANCEL = WM_APP + 9;