using System.Diagnostics;
using System.Text.Json;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls.Test;

namespace Mids_Reborn.UI.Theming;

public static class ThemeManager
{
    private static readonly Dictionary<string, ApplicationTheme> BuiltInThemes = CreateBuiltInThemes();
    private static Dictionary<string, ApplicationTheme>? _runtimeThemes;

    public readonly record struct ThemeEntry(string Name, bool IsUser);

    public static string DesignTimeThemeName { get; set; } = "Hero";

    public static ApplicationTheme DesignTime
        => BuiltInThemes.TryGetValue(DesignTimeThemeName, out var theme) ? theme : BuiltInThemes.Values.First();

    // public static IReadOnlyCollection<string> AvailableThemeNames
    //     => _runtimeThemes?.Keys ?? (IReadOnlyCollection<string>)BuiltInThemes.Keys;

    public static bool IsUserTheme(string name)
        => (_runtimeThemes?.ContainsKey(name) ?? false) && !BuiltInThemes.ContainsKey(name);

    public static IReadOnlyCollection<ThemeEntry> AvailableThemes
    {
        get
        {
            // If runtime map exists, it contains built-ins + user themes; else use built-ins only.
            var dict = _runtimeThemes ?? BuiltInThemes;
            // For each key, user theme = exists in runtime but NOT in built-ins.
            var list = new List<ThemeEntry>(capacity: dict.Count);
            foreach (var name in dict.Keys)
                list.Add(new ThemeEntry(name, IsUserTheme(name)));
            return list;
        }
    }

    public static event Action? ThemeChanged;

    public static ApplicationTheme? CurrentTheme { get; private set; }
    private static ApplicationTheme? BackupTheme;

    public static void SaveTheme()
    {
        BackupTheme = CurrentTheme.Clone();
    }

    public static void RestoreTheme()
    {
        if (BackupTheme == null)
        {
            return;
        }

        CurrentTheme = BackupTheme.Clone();
        ThemeChanged?.Invoke();
    }

    public static void ApplyThemeDirect(ApplicationTheme theme)
    {
        CurrentTheme = theme;
        ThemeChanged?.Invoke();
    }

    public static void Initialize()
    {
        _runtimeThemes = new Dictionary<string, ApplicationTheme>(BuiltInThemes);

        var themesFolderPath = Path.Combine(AppContext.BaseDirectory, "Themes");
        if (!Directory.Exists(themesFolderPath))
        {
            Directory.CreateDirectory(themesFolderPath);
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        foreach (var filePath in Directory.EnumerateFiles(themesFolderPath, "*.json"))
        {
            try
            {
                var jsonContent = File.ReadAllText(filePath);
                var customTheme = JsonSerializer.Deserialize<ApplicationTheme>(jsonContent, options);

                if (customTheme != null && !string.IsNullOrWhiteSpace(customTheme.Name))
                {
                    customTheme = NormalizeTheme(customTheme);
                    if (!_runtimeThemes.TryAdd(customTheme.Name, customTheme))
                    {
                        Debug.WriteLine($"Custom theme '{customTheme.Name}' conflicts with a built-in theme and was ignored.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load custom theme from '{Path.GetFileName(filePath)}': {ex.Message}");
            }
        }
    }

    public static void SetTheme(string themeName)
    {
        if (_runtimeThemes != null && _runtimeThemes.TryGetValue(themeName, out var theme))
        {
            if (CurrentTheme?.Name == themeName) return;

            CurrentTheme = theme;

            var cfg = MidsContext.Config;
            if (cfg != null)
                cfg.SelectedTheme = themeName;

            ThemeChanged?.Invoke();
        }
    }

    public static void ReloadCustomThemes()
    {
        // Ensure runtime map exists and is seeded with built-ins
        _runtimeThemes ??= new Dictionary<string, ApplicationTheme>(BuiltInThemes);

        var themesFolderPath = Path.Combine(AppContext.BaseDirectory, "Themes");
        if (!Directory.Exists(themesFolderPath))
            Directory.CreateDirectory(themesFolderPath);

        // Load current on-disk user themes
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var files = Directory.EnumerateFiles(themesFolderPath, "*.json", SearchOption.TopDirectoryOnly).ToArray();

        var seenUserNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var filePath in files)
        {
            try
            {
                var jsonContent = File.ReadAllText(filePath);
                var customTheme = JsonSerializer.Deserialize<ApplicationTheme>(jsonContent, options);

                if (customTheme is null || string.IsNullOrWhiteSpace(customTheme.Name))
                    continue;

                // Do not allow overriding built-ins
                if (BuiltInThemes.ContainsKey(customTheme.Name))
                {
                    Debug.WriteLine($"Custom theme '{customTheme.Name}' conflicts with a built-in theme and was ignored.");
                    continue;
                }

                customTheme = NormalizeTheme(customTheme);
                seenUserNames.Add(customTheme.Name);
                _runtimeThemes[customTheme.Name] = customTheme; // add or update
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load custom theme from '{Path.GetFileName(filePath)}': {ex.Message}");
            }
        }

        // Remove user themes that no longer exist on disk
        var toRemove = _runtimeThemes.Keys
            .Where(name => !BuiltInThemes.ContainsKey(name) && !seenUserNames.Contains(name))
            .ToList();

        foreach (var name in toRemove)
            _runtimeThemes.Remove(name);

        // If the current theme vanished, fall back to first built-in and fire ThemeChanged
        if (CurrentTheme is not null && !_runtimeThemes.ContainsKey(CurrentTheme.Name))
        {
            var previous = CurrentTheme.Name;
            CurrentTheme = BuiltInThemes.Values.FirstOrDefault();
            if (CurrentTheme is not null && !string.Equals(previous, CurrentTheme.Name, StringComparison.OrdinalIgnoreCase))
                ThemeChanged?.Invoke();
        }
    }

    private static ApplicationTheme NormalizeTheme(ApplicationTheme theme)
    {
        theme.PowerSlot = NormalizePowerSlotTheme(theme, theme.PowerSlot);
        theme.SegmentedToggle = NormalizeSegmentedToggleTheme(theme, theme.SegmentedToggle);
        return theme;
    }

    private static PowerSlotTheme NormalizePowerSlotTheme(ApplicationTheme theme, PowerSlotTheme? powerSlotTheme)
    {
        PowerSlotTheme derived = CreateDerivedPowerSlotTheme(theme);
        powerSlotTheme ??= new PowerSlotTheme();

        powerSlotTheme.Border = ResolveThemeColor(powerSlotTheme.Border, derived.Border);
        powerSlotTheme.GradientTop = ResolveThemeColor(powerSlotTheme.GradientTop, derived.GradientTop);
        powerSlotTheme.GradientBottom = ResolveThemeColor(powerSlotTheme.GradientBottom, derived.GradientBottom);
        powerSlotTheme.HoverGradientTop = ResolveThemeColor(powerSlotTheme.HoverGradientTop, derived.HoverGradientTop);
        powerSlotTheme.HoverGradientBottom = ResolveThemeColor(powerSlotTheme.HoverGradientBottom, derived.HoverGradientBottom);
        powerSlotTheme.OpenBorder = ResolveThemeColor(powerSlotTheme.OpenBorder, derived.OpenBorder);
        powerSlotTheme.EmptyFill = ResolveThemeColor(powerSlotTheme.EmptyFill, derived.EmptyFill);
        powerSlotTheme.DisabledFill = ResolveThemeColor(powerSlotTheme.DisabledFill, derived.DisabledFill);
        powerSlotTheme.ForeColor = ResolveThemeColor(powerSlotTheme.ForeColor, derived.ForeColor);
        return powerSlotTheme;
    }

    private static PowerSlotTheme CreateDerivedPowerSlotTheme(ApplicationTheme theme)
    {
        Color accent = ResolveThemeColor(theme.DataView.Accent, Color.FromArgb(32, 88, 182));
        Color accentLight = ResolveThemeColor(theme.MenuStrip.AccentLightColor, ResolveThemeColor(theme.DropDownList.HoverBorder, Color.FromArgb(126, 207, 255)));
        Color buttonTop = ResolveThemeColor(theme.Button.GradientTop, accent);
        Color buttonBottom = ResolveThemeColor(theme.Button.GradientBottom, Blend(buttonTop, Color.Black, 0.42f));
        Color headerDark = ResolveThemeColor(theme.Header.HeaderDark, buttonBottom);
        Color dataCard = ResolveThemeColor(theme.DataView.Card, headerDark);
        Color border = Blend(ResolveThemeColor(theme.Button.Border, accent), accent, 0.36f);
        Color gradientTop = Blend(buttonTop, accentLight, 0.14f);
        Color gradientBottom = Blend(buttonBottom, headerDark, 0.26f);
        Color openBorder = Blend(accentLight, accent, 0.18f);
        Color emptyFill = Blend(dataCard, headerDark, 0.34f);
        Color disabledFill = Blend(emptyFill, Color.Black, 0.24f);
        Color foreColor = ResolveThemeColor(theme.DataView.Text, ResolveThemeColor(theme.Button.ForeColor, Color.WhiteSmoke));

        return new PowerSlotTheme
        {
            Border = border,
            GradientTop = gradientTop,
            GradientBottom = gradientBottom,
            HoverGradientTop = Blend(gradientTop, Color.White, 0.10f),
            HoverGradientBottom = Blend(gradientBottom, Color.White, 0.06f),
            OpenBorder = openBorder,
            EmptyFill = emptyFill,
            DisabledFill = disabledFill,
            ForeColor = foreColor
        };
    }

    private static SegmentedToggleTheme NormalizeSegmentedToggleTheme(ApplicationTheme theme, SegmentedToggleTheme? segmentedToggleTheme)
    {
        var derived = CreateDerivedSegmentedToggleTheme(theme);
        segmentedToggleTheme ??= new SegmentedToggleTheme();

        segmentedToggleTheme.WellTop = ResolveThemeColor(segmentedToggleTheme.WellTop, derived.WellTop);
        segmentedToggleTheme.WellBottom = ResolveThemeColor(segmentedToggleTheme.WellBottom, derived.WellBottom);
        segmentedToggleTheme.Divider = ResolveThemeColor(segmentedToggleTheme.Divider, derived.Divider);
        segmentedToggleTheme.SelectedTop = ResolveThemeColor(segmentedToggleTheme.SelectedTop, derived.SelectedTop);
        segmentedToggleTheme.SelectedBottom = ResolveThemeColor(segmentedToggleTheme.SelectedBottom, derived.SelectedBottom);
        segmentedToggleTheme.SelectedBorder = ResolveThemeColor(segmentedToggleTheme.SelectedBorder, derived.SelectedBorder);
        segmentedToggleTheme.SelectedText = ResolveThemeColor(segmentedToggleTheme.SelectedText, derived.SelectedText);
        segmentedToggleTheme.SelectedTextOutline = ResolveThemeColor(segmentedToggleTheme.SelectedTextOutline, derived.SelectedTextOutline);
        segmentedToggleTheme.UnselectedText = ResolveThemeColor(segmentedToggleTheme.UnselectedText, derived.UnselectedText);
        segmentedToggleTheme.UnselectedTextOutline = ResolveThemeColor(segmentedToggleTheme.UnselectedTextOutline, derived.UnselectedTextOutline);
        return segmentedToggleTheme;
    }

    public static SegmentedToggleTheme CreateDerivedSegmentedToggleTheme(ApplicationTheme theme)
    {
        Color buttonTop = ResolveThemeColor(theme.Button.GradientTop, Color.FromArgb(70, 120, 180));
        Color buttonBottom = ResolveThemeColor(theme.Button.GradientBottom, Blend(buttonTop, Color.Black, 0.45f));
        Color toggledTop = ResolveThemeColor(theme.Button.ToggledGradientTop, buttonTop);
        Color toggledBottom = ResolveThemeColor(theme.Button.ToggledGradientBottom, buttonBottom);
        Color headerDark = ResolveThemeColor(theme.Header.HeaderDark, buttonBottom);
        Color dataCard = ResolveThemeColor(theme.DataView.Card, headerDark);
        Color wellTop = Blend(headerDark, dataCard, 0.14f);
        Color wellBottom = Blend(headerDark, Color.Black, 0.10f);
        Color divider = Blend(ResolveThemeColor(theme.Button.Border, buttonTop), wellBottom, 0.72f);
        Color selectedTop = toggledTop;
        Color selectedBottom = toggledBottom;

        return new SegmentedToggleTheme
        {
            WellTop = wellTop,
            WellBottom = wellBottom,
            Divider = divider,
            SelectedTop = selectedTop,
            SelectedBottom = selectedBottom,
            SelectedBorder = ResolveThemeColor(theme.Button.ToggledBorderColor, ResolveThemeColor(theme.MenuStrip.AccentColor, Color.Gold)),
            SelectedText = ResolveThemeColor(theme.Button.ToggledTextColor, ResolveThemeColor(theme.Button.ForeColor, Color.WhiteSmoke)),
            SelectedTextOutline = ResolveThemeColor(theme.Button.ToggledTextOutlineColor, ResolveThemeColor(theme.Button.TextOutlineColor, Color.Black)),
            UnselectedText = ResolveThemeColor(theme.Button.ForeColor, Color.WhiteSmoke),
            UnselectedTextOutline = ResolveThemeColor(theme.Button.TextOutlineColor, Color.Black)
        };
    }

    public static FooterTheme CreateDefaultFooterTheme() => new FooterTheme { Background = Color.FromArgb(6, 17, 35), SummaryText = Color.WhiteSmoke, TotalSlotsText = Color.WhiteSmoke, SlotsLeftText = Color.FromArgb(115, 255, 110) };

    private static Color ResolveThemeColor(Color candidate, Color fallback)
    {
        return candidate.IsEmpty ? fallback : candidate;
    }

    private static Color Blend(Color first, Color second, float amountSecond)
    {
        amountSecond = Math.Clamp(amountSecond, 0f, 1f);
        float amountFirst = 1f - amountSecond;
        return Color.FromArgb(
            (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
            (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
            (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
            (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
    }

    private static Dictionary<string, ApplicationTheme> CreateBuiltInThemes()
    {
        var themes = new Dictionary<string, ApplicationTheme>();

        #region Hero Theme
        var hero = new ApplicationTheme
        {
            Name = "Hero",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(70, 120, 180),
                GradientBottom = Color.FromArgb(25, 60, 110),
                HoverGradientTop = Color.FromArgb(100, 150, 210),
                HoverGradientBottom = Color.FromArgb(45, 85, 135),
                PressedGradientTop = Color.FromArgb(25, 60, 110),
                PressedGradientBottom = Color.FromArgb(70, 120, 180),
                Border = Color.FromArgb(0, 64, 128),
                ForeColor = Color.WhiteSmoke,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(70, 120, 180),
                ToggledGradientBottom = Color.FromArgb(25, 60, 110),
                ToggledBorderColor = Color.FromArgb(255, 192, 0),
                ToggledTextColor = Color.White,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(2, 8, 15),
                Card = Color.FromArgb(10, 19, 29),
                Border = Color.FromArgb(38, 44, 58),
                Accent = Color.FromArgb(228, 177, 63),

                // Header tabs
                HeaderTop = Color.FromArgb(20, 54, 87),
                HeaderBottom = Color.FromArgb(28, 80, 128),
                TabActiveTop = Color.FromArgb(56, 118, 205),
                TabActiveBottom = Color.FromArgb(35, 86, 154),
                TabInactiveTop = Color.FromArgb(28, 28, 36),
                TabInactiveBottom = Color.FromArgb(20, 20, 28),
                TabBorder = Color.FromArgb(10, 10, 14),
                Text = Color.FromArgb(248, 241, 212),
                ValueText = Color.FromArgb(240, 240, 240),
                Muted = Color.FromArgb(225, 217, 192),

                // Controls
                Chip = Color.FromArgb(32, 40, 56),
                ChipActive = Color.FromArgb(52, 78, 124),
                Rail = Color.FromArgb(28, 32, 44),
                RailFill = Color.FromArgb(60, 112, 196),
                Thumb = Color.Gold,
                ThumbBorder = Color.FromArgb(24, 28, 36),

                // Grid
                GridHeaderTop = Color.FromArgb(30, 35, 48),
                GridHeaderBottom = Color.FromArgb(22, 26, 36),
                GridHeaderBorder = Color.FromArgb(12, 14, 20),
                GridRowEven = Color.FromArgb(2, 7, 14),
                GridRowOdd = Color.FromArgb(6, 23, 46),
                GridRowLine = Color.FromArgb(24, 28, 36),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(230, 86, 86),
                GridBandMid = Color.FromArgb(144, 173, 28),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(70, 120, 180),
                GradientBottom = Color.FromArgb(25, 60, 110),
                HoverGradientTop = Color.FromArgb(100, 150, 210),
                HoverGradientBottom = Color.FromArgb(45, 85, 135),
                Border = Color.FromArgb(0, 64, 128),
                HoverBorder = Color.FromArgb(255, 173, 35),
                DropDownBackColor = Color.FromArgb(4, 34, 64),
                DropDownSelectionBackColor = Color.FromArgb(0, 80, 160),
                DropDownSelectionForeColor = Color.White,
                LockColor = Color.FromArgb(151, 185, 212),
                ForeColor = Color.WhiteSmoke,
                Arrow = Color.White,
                HoverArrow = Color.Gold,
                FocusBorder = Color.Gold
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(42, 64, 99),
                HeaderMid = Color.FromArgb(9, 39, 70),
                HeaderDark = Color.FromArgb(6, 33, 57),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 100, 150, 210),
                WindowIconPressed = Color.FromArgb(96, 45, 85, 135),
                WindowIconCloseHover = Color.FromArgb(220, 20, 60),
                WindowIconClosePressed = Color.FromArgb(139, 0, 0),
                LogoTargetColor = Color.FromArgb(70, 120, 180)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(40, 80, 130),
                ScrollButton = Color.FromArgb(70, 120, 180),
                Enabled = Color.FromArgb(173, 216, 230),
                Selected = Color.FromArgb(80, 137, 205),
                Disabled = Color.FromArgb(120, 120, 120),
                SelectedDisabled = Color.FromArgb(45, 77, 116),
                Invalid = Color.Red,
                Heading = Color.FromArgb(255, 165, 0)
            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(3, 111, 160),
                AccentColor = Color.Gold,
                AccentLightColor = Color.WhiteSmoke
            },
            SegmentedToggle = new SegmentedToggleTheme
            {
                WellTop = Color.FromArgb(7, 31, 53),
                WellBottom = Color.FromArgb(5, 30, 51),
                Divider = Color.FromArgb(4, 40, 73),
                SelectedTop = Color.FromArgb(70, 120, 180),
                SelectedBottom = Color.FromArgb(25, 60, 110),
                SelectedBorder = Color.FromArgb(255, 192, 0),
                SelectedText = Color.White,
                SelectedTextOutline = Color.Black,
                UnselectedText = Color.White,
                UnselectedTextOutline = Color.Black
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(32, 88, 182),
                GradientTop = Color.FromArgb(100, 165, 238),
                GradientBottom = Color.FromArgb(11, 65, 149),
                OpenBorder = Color.FromArgb(126, 207, 255),
                EmptyFill = Color.FromArgb(34, 48, 68),
                DisabledFill = Color.FromArgb(24, 34, 50),
                ForeColor = Color.FromArgb(245, 247, 252)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(40, 80, 130),
                Bar = Color.FromArgb(70, 120, 180),
                Hover = Color.FromArgb(100, 150, 210)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(6, 17, 35),
                SummaryText = Color.WhiteSmoke,
                TotalSlotsText = Color.WhiteSmoke,
                SlotsLeftText = Color.FromArgb(115, 255, 110)
            }
        };
        themes.Add(hero.Name, hero);
        #endregion

        #region Villain Theme
        var villain = new ApplicationTheme
        {
            Name = "Villain",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(190, 90, 90),
                GradientBottom = Color.FromArgb(101, 28, 28),
                HoverGradientTop = Color.FromArgb(220, 120, 120),
                HoverGradientBottom = Color.FromArgb(131, 58, 58),
                PressedGradientTop = Color.FromArgb(101, 28, 28),
                PressedGradientBottom = Color.FromArgb(190, 90, 90),
                Border = Color.FromArgb(128, 0, 0),
                ForeColor = Color.White,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(190, 90, 90),
                ToggledGradientBottom = Color.FromArgb(101, 28, 28),
                ToggledBorderColor = Color.FromArgb(255, 80, 0),
                ToggledTextColor = Color.White,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(8, 6, 8),
                Card = Color.FromArgb(20, 14, 18),
                Border = Color.FromArgb(54, 32, 40),
                Accent = Color.FromArgb(212, 64, 72),

                // Header tabs
                HeaderTop = Color.FromArgb(68, 12, 20),
                HeaderBottom = Color.FromArgb(110, 22, 30),
                TabActiveTop = Color.FromArgb(180, 60, 70),
                TabActiveBottom = Color.FromArgb(122, 34, 40),
                TabInactiveTop = Color.FromArgb(34, 24, 28),
                TabInactiveBottom = Color.FromArgb(24, 16, 20),
                TabBorder = Color.FromArgb(12, 8, 10),
                Text = Color.FromArgb(235, 235, 240),
                ValueText = Color.FromArgb(235, 235, 240),
                Muted = Color.FromArgb(185, 190, 200),

                // Controls
                Chip = Color.FromArgb(40, 28, 34),
                ChipActive = Color.FromArgb(96, 44, 56),
                Rail = Color.FromArgb(34, 24, 28),
                RailFill = Color.FromArgb(190, 90, 90),
                Thumb = Color.FromArgb(230, 220, 224),
                ThumbBorder = Color.FromArgb(32, 24, 28),

                // Grid
                GridHeaderTop = Color.FromArgb(36, 24, 28),
                GridHeaderBottom = Color.FromArgb(26, 18, 22),
                GridHeaderBorder = Color.FromArgb(16, 10, 14),
                GridRowEven = Color.FromArgb(18, 12, 16),
                GridRowOdd = Color.FromArgb(14, 10, 14),
                GridRowLine = Color.FromArgb(32, 20, 24),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(236, 90, 90),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(190, 90, 90),
                GradientBottom = Color.FromArgb(101, 28, 28),
                HoverGradientTop = Color.FromArgb(220, 120, 120),
                HoverGradientBottom = Color.FromArgb(131, 58, 58),
                Border = Color.FromArgb(128, 0, 0),
                HoverBorder = Color.FromArgb(255, 80, 0),
                DropDownBackColor = Color.FromArgb(64, 4, 4),
                DropDownSelectionBackColor = Color.FromArgb(128, 0, 0),
                DropDownSelectionForeColor = Color.White,
                LockColor = Color.FromArgb(212, 151, 151),
                ForeColor = Color.White,
                Arrow = Color.White,
                HoverArrow = Color.Gold,
                FocusBorder = Color.Gold
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(99, 42, 42),
                HeaderMid = Color.FromArgb(70, 9, 9),
                HeaderDark = Color.FromArgb(57, 6, 6),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 220, 120, 120),
                WindowIconPressed = Color.FromArgb(96, 131, 58, 58),
                WindowIconCloseHover = Color.FromArgb(231, 76, 60),
                WindowIconClosePressed = Color.FromArgb(192, 57, 43),
                LogoTargetColor = Color.FromArgb(190, 90, 90)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(140, 60, 60),
                ScrollButton = Color.FromArgb(190, 90, 90),
                Enabled = Color.FromArgb(120, 52, 52),
                Selected = Color.FromArgb(219, 58, 71),
                Disabled = Color.FromArgb(120, 120, 120),
                SelectedDisabled = Color.FromArgb(116, 50, 50),
                Invalid = Color.Red,
                Heading = Color.WhiteSmoke
            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(140, 50, 50),
                AccentColor = Color.FromArgb(255, 80, 0),
                AccentLightColor = Color.WhiteSmoke
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(138, 30, 40),
                GradientTop = Color.FromArgb(214, 100, 112),
                GradientBottom = Color.FromArgb(109, 17, 27),
                OpenBorder = Color.FromArgb(255, 141, 92),
                EmptyFill = Color.FromArgb(72, 40, 44),
                DisabledFill = Color.FromArgb(48, 28, 32),
                ForeColor = Color.FromArgb(248, 243, 244)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(140, 60, 60),
                Bar = Color.FromArgb(190, 90, 90),
                Hover = Color.FromArgb(220, 120, 120)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(20, 2, 2),
                SummaryText = Color.WhiteSmoke,
                TotalSlotsText = Color.WhiteSmoke,
                SlotsLeftText = Color.FromArgb(231, 109, 109)
            }
        };
        themes.Add(villain.Name, villain);
        #endregion

        #region Loyalist Theme
        var loyalist = new ApplicationTheme
        {
            Name = "Loyalist",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(220, 190, 60),
                GradientBottom = Color.FromArgb(120, 100, 20),
                HoverGradientTop = Color.FromArgb(250, 215, 85),
                HoverGradientBottom = Color.FromArgb(145, 120, 40),
                PressedGradientTop = Color.FromArgb(120, 100, 20),
                PressedGradientBottom = Color.FromArgb(220, 190, 60),
                Border = Color.FromArgb(160, 120, 0),
                ForeColor = Color.WhiteSmoke,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(220, 190, 60),
                ToggledGradientBottom = Color.FromArgb(120, 100, 20),
                ToggledBorderColor = Color.FromArgb(240, 240, 230),
                ToggledTextColor = Color.WhiteSmoke,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(8, 8, 6),
                Card = Color.FromArgb(20, 20, 14),
                Border = Color.FromArgb(58, 54, 38),
                Accent = Color.FromArgb(232, 196, 72),

                // Header tabs (steel + imperial gold)
                HeaderTop = Color.FromArgb(48, 56, 68),
                HeaderBottom = Color.FromArgb(71, 84, 102),
                TabActiveTop = Color.FromArgb(196, 160, 60),
                TabActiveBottom = Color.FromArgb(120, 100, 32),
                TabInactiveTop = Color.FromArgb(32, 34, 40),
                TabInactiveBottom = Color.FromArgb(24, 26, 32),
                TabBorder = Color.FromArgb(12, 12, 10),
                Text = Color.FromArgb(235, 235, 240),
                ValueText = Color.FromArgb(235, 235, 240),
                Muted = Color.FromArgb(185, 190, 200),

                // Controls
                Chip = Color.FromArgb(34, 32, 24),
                ChipActive = Color.FromArgb(78, 64, 36),
                Rail = Color.FromArgb(30, 32, 24),
                RailFill = Color.FromArgb(200, 170, 60),
                Thumb = Color.FromArgb(240, 236, 220),
                ThumbBorder = Color.FromArgb(36, 34, 26),

                // Grid
                GridHeaderTop = Color.FromArgb(36, 38, 46),
                GridHeaderBottom = Color.FromArgb(28, 30, 38),
                GridHeaderBorder = Color.FromArgb(18, 18, 16),
                GridRowEven = Color.FromArgb(20, 22, 28),
                GridRowOdd = Color.FromArgb(16, 18, 24),
                GridRowLine = Color.FromArgb(28, 28, 24),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(230, 86, 86),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(220, 190, 60),
                GradientBottom = Color.FromArgb(120, 100, 20),
                HoverGradientTop = Color.FromArgb(250, 215, 85),
                HoverGradientBottom = Color.FromArgb(145, 120, 40),
                Border = Color.FromArgb(160, 120, 0),
                HoverBorder = Color.FromArgb(255, 255, 128),
                DropDownBackColor = Color.FromArgb(60, 50, 10),
                DropDownSelectionBackColor = Color.FromArgb(185, 145, 0),
                DropDownSelectionForeColor = Color.Black,
                LockColor = Color.FromArgb(200, 180, 120),
                ForeColor = Color.WhiteSmoke,
                Arrow = Color.White,
                HoverArrow = Color.Black,
                FocusBorder = Color.FromArgb(255, 255, 128)
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(99, 86, 42),
                HeaderMid = Color.FromArgb(70, 60, 9),
                HeaderDark = Color.FromArgb(57, 49, 6),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 250, 215, 85),
                WindowIconPressed = Color.FromArgb(96, 145, 120, 40),
                WindowIconCloseHover = Color.FromArgb(231, 76, 60),
                WindowIconClosePressed = Color.FromArgb(192, 57, 43),
                LogoTargetColor = Color.FromArgb(220, 190, 60)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(170, 140, 40),
                ScrollButton = Color.FromArgb(220, 190, 60),
                Enabled = Color.FromArgb(144, 136, 107),
                Selected = Color.FromArgb(255, 204, 35),
                Disabled = Color.FromArgb(79, 79, 71),
                SelectedDisabled = Color.FromArgb(142, 117, 33),
                Invalid = Color.Red,
                Heading = Color.FromArgb(255, 165, 0)
            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(145, 120, 40),
                AccentColor = Color.FromArgb(220, 190, 60),
                AccentLightColor = Color.FromArgb(200, 180, 120)
            },
            SegmentedToggle = new SegmentedToggleTheme
            {
                WellTop = Color.FromArgb(52, 45, 7),
                WellBottom = Color.FromArgb(51, 44, 5),
                Divider = Color.FromArgb(82, 65, 4),
                SelectedTop = Color.FromArgb(220, 190, 60),
                SelectedBottom = Color.FromArgb(120, 100, 20),
                SelectedBorder = Color.FromArgb(240, 240, 230),
                SelectedText = Color.FromArgb(245, 245, 245),
                SelectedTextOutline = Color.Black,
                UnselectedText = Color.FromArgb(245, 245, 245),
                UnselectedTextOutline = Color.Black
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(150, 114, 24),
                GradientTop = Color.FromArgb(230, 196, 76),
                GradientBottom = Color.FromArgb(124, 93, 18),
                OpenBorder = Color.FromArgb(255, 233, 148),
                EmptyFill = Color.FromArgb(70, 60, 30),
                DisabledFill = Color.FromArgb(46, 38, 20),
                ForeColor = Color.FromArgb(248, 244, 231)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(170, 140, 40),
                Bar = Color.FromArgb(220, 190, 60),
                Hover = Color.FromArgb(250, 215, 85)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(26, 27, 20),
                SummaryText = Color.FromArgb(245, 245, 245),
                TotalSlotsText = Color.FromArgb(245, 245, 245),
                SlotsLeftText = Color.FromArgb(226, 186, 53)
            }
        };
        themes.Add(loyalist.Name, loyalist);
        #endregion

        #region Resistance Theme
        var resistance = new ApplicationTheme
        {
            Name = "Resistance",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(60, 220, 220),
                GradientBottom = Color.FromArgb(20, 110, 110),
                HoverGradientTop = Color.FromArgb(100, 255, 255),
                HoverGradientBottom = Color.FromArgb(40, 140, 140),
                PressedGradientTop = Color.FromArgb(20, 110, 110),
                PressedGradientBottom = Color.FromArgb(60, 220, 220),
                Border = Color.FromArgb(0, 128, 128),
                ForeColor = Color.White,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(60, 220, 220),
                ToggledGradientBottom = Color.FromArgb(20, 110, 110),
                ToggledBorderColor = Color.FromArgb(245, 245, 245),
                ToggledTextColor = Color.White,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(6, 8, 8),
                Card = Color.FromArgb(14, 20, 22),
                Border = Color.FromArgb(32, 54, 58),
                Accent = Color.FromArgb(255, 128, 0),

                // Header tabs (teal/cyan)
                HeaderTop = Color.FromArgb(20, 68, 78),
                HeaderBottom = Color.FromArgb(28, 96, 108),
                TabActiveTop = Color.FromArgb(64, 152, 164),
                TabActiveBottom = Color.FromArgb(32, 112, 122),
                TabInactiveTop = Color.FromArgb(22, 26, 28),
                TabInactiveBottom = Color.FromArgb(16, 20, 22),
                TabBorder = Color.FromArgb(10, 12, 12),
                Text = Color.FromArgb(235, 235, 240),
                ValueText = Color.FromArgb(235, 235, 240),
                Muted = Color.FromArgb(185, 190, 200),

                // Controls
                Chip = Color.FromArgb(26, 34, 36),
                ChipActive = Color.FromArgb(40, 96, 104),
                Rail = Color.FromArgb(24, 30, 32),
                RailFill = Color.FromArgb(60, 160, 172),
                Thumb = Color.FromArgb(220, 236, 240),
                ThumbBorder = Color.FromArgb(22, 30, 32),

                // Grid
                GridHeaderTop = Color.FromArgb(26, 34, 36),
                GridHeaderBottom = Color.FromArgb(18, 24, 26),
                GridHeaderBorder = Color.FromArgb(12, 16, 18),
                GridRowEven = Color.FromArgb(14, 20, 22),
                GridRowOdd = Color.FromArgb(12, 18, 20),
                GridRowLine = Color.FromArgb(22, 28, 30),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(230, 86, 86),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(60, 220, 220),
                GradientBottom = Color.FromArgb(20, 110, 110),
                HoverGradientTop = Color.FromArgb(100, 255, 255),
                HoverGradientBottom = Color.FromArgb(40, 140, 140),
                Border = Color.FromArgb(0, 128, 128),
                HoverBorder = Color.FromArgb(128, 255, 255),
                DropDownBackColor = Color.FromArgb(10, 40, 40),
                DropDownSelectionBackColor = Color.FromArgb(0, 160, 160),
                DropDownSelectionForeColor = Color.Black,
                LockColor = Color.FromArgb(120, 200, 200),
                ForeColor = Color.White,
                Arrow = Color.White,
                HoverArrow = Color.Black,
                FocusBorder = Color.FromArgb(128, 255, 255)
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(42, 99, 99),
                HeaderMid = Color.FromArgb(9, 70, 70),
                HeaderDark = Color.FromArgb(6, 57, 57),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 100, 255, 255),
                WindowIconPressed = Color.FromArgb(96, 40, 140, 140),
                WindowIconCloseHover = Color.FromArgb(220, 20, 60),
                WindowIconClosePressed = Color.FromArgb(139, 0, 0),
                LogoTargetColor = Color.FromArgb(60, 220, 220)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(40, 170, 170),
                ScrollButton = Color.FromArgb(60, 220, 220),
                Enabled = Color.FromArgb(173, 216, 230),
                Selected = Color.FromArgb(56, 206, 206),
                Disabled = Color.FromArgb(140, 140, 140),
                SelectedDisabled = Color.FromArgb(31, 114, 114),
                Invalid = Color.Red,
                Heading = Color.White
            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(40, 140, 140),
                AccentColor = Color.FromArgb(56, 195, 195),
                AccentLightColor = Color.WhiteSmoke
            },
            SegmentedToggle = new SegmentedToggleTheme
            {
                WellTop = Color.FromArgb(7, 52, 52),
                WellBottom = Color.FromArgb(5, 51, 51),
                Divider = Color.FromArgb(4, 73, 73),
                SelectedTop = Color.FromArgb(60, 220, 220),
                SelectedBottom = Color.FromArgb(20, 110, 110),
                SelectedBorder = Color.WhiteSmoke,
                SelectedText = Color.White,
                SelectedTextOutline = Color.Black,
                UnselectedText = Color.White,
                UnselectedTextOutline = Color.Black
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(0, 120, 126),
                GradientTop = Color.FromArgb(92, 226, 226),
                GradientBottom = Color.FromArgb(14, 118, 126),
                OpenBorder = Color.FromArgb(164, 255, 241),
                EmptyFill = Color.FromArgb(26, 68, 70),
                DisabledFill = Color.FromArgb(18, 44, 46),
                ForeColor = Color.FromArgb(239, 248, 248)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(40, 170, 170),
                Bar = Color.FromArgb(60, 220, 220),
                Hover = Color.FromArgb(100, 255, 255)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(14, 18, 20),
                SummaryText = Color.WhiteSmoke,
                TotalSlotsText = Color.WhiteSmoke,
                SlotsLeftText = Color.FromArgb(67, 244, 244)
            }
        };
        themes.Add(resistance.Name, resistance);
        #endregion

        #region Rogue Theme
        var rogue = new ApplicationTheme
        {
            Name = "Rogue",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(192, 192, 192),
                GradientBottom = Color.FromArgb(80, 80, 80),
                HoverGradientTop = Color.FromArgb(220, 220, 220),
                HoverGradientBottom = Color.FromArgb(110, 110, 110),
                PressedGradientTop = Color.FromArgb(80, 80, 80),
                PressedGradientBottom = Color.FromArgb(192, 192, 192),
                Border = Color.FromArgb(220, 20, 60),
                ForeColor = Color.WhiteSmoke,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(192, 192, 192),
                ToggledGradientBottom = Color.FromArgb(80, 80, 80),
                ToggledBorderColor = Color.FromArgb(255, 0, 0),
                ToggledTextColor = Color.WhiteSmoke,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(6, 8, 6),
                Card = Color.FromArgb(16, 22, 18),
                Border = Color.FromArgb(34, 54, 38),
                Accent = Color.FromArgb(96, 176, 112),

                // Header tabs (hunter green)
                HeaderTop = Color.FromArgb(18, 64, 46),
                HeaderBottom = Color.FromArgb(26, 96, 70),
                TabActiveTop = Color.FromArgb(40, 130, 90),
                TabActiveBottom = Color.FromArgb(24, 96, 68),
                TabInactiveTop = Color.FromArgb(24, 28, 26),
                TabInactiveBottom = Color.FromArgb(18, 20, 18),
                TabBorder = Color.FromArgb(10, 12, 10),
                Text = Color.FromArgb(235, 235, 240),
                ValueText = Color.FromArgb(235, 235, 240),
                Muted = Color.FromArgb(185, 190, 200),

                // Controls
                Chip = Color.FromArgb(28, 40, 32),
                ChipActive = Color.FromArgb(52, 100, 84),
                Rail = Color.FromArgb(24, 32, 26),
                RailFill = Color.FromArgb(60, 140, 110),
                Thumb = Color.FromArgb(220, 236, 224),
                ThumbBorder = Color.FromArgb(24, 30, 26),

                // Grid
                GridHeaderTop = Color.FromArgb(28, 36, 30),
                GridHeaderBottom = Color.FromArgb(20, 28, 22),
                GridHeaderBorder = Color.FromArgb(14, 18, 14),
                GridRowEven = Color.FromArgb(16, 22, 18),
                GridRowOdd = Color.FromArgb(14, 20, 16),
                GridRowLine = Color.FromArgb(22, 28, 24),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(230, 86, 86),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(192, 192, 192),
                GradientBottom = Color.FromArgb(80, 80, 80),
                HoverGradientTop = Color.FromArgb(220, 220, 220),
                HoverGradientBottom = Color.FromArgb(110, 110, 110),
                Border = Color.FromArgb(110, 110, 110),
                HoverBorder = Color.FromArgb(255, 0, 0),
                DropDownBackColor = Color.FromArgb(130, 130, 130),
                DropDownSelectionBackColor = Color.FromArgb(100, 100, 100),
                DropDownSelectionForeColor = Color.White,
                LockColor = Color.FromArgb(120, 120, 120),
                ForeColor = Color.Black,
                Arrow = Color.Black,
                HoverArrow = Color.FromArgb(255, 0, 0),
                FocusBorder = Color.FromArgb(255, 0, 0)
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(80, 80, 80),
                HeaderMid = Color.FromArgb(50, 50, 50),
                HeaderDark = Color.FromArgb(35, 35, 35),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 160, 160, 160),
                WindowIconPressed = Color.FromArgb(96, 90, 90, 90),
                WindowIconCloseHover = Color.FromArgb(220, 20, 60),
                WindowIconClosePressed = Color.FromArgb(139, 0, 0),
                LogoTargetColor = Color.FromArgb(201, 24, 24)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(140, 140, 140),
                ScrollButton = Color.FromArgb(192, 192, 192),
                Enabled = Color.FromArgb(200, 150, 150),
                Selected = Color.FromArgb(229, 56, 86),
                Disabled = Color.FromArgb(120, 120, 120),
                SelectedDisabled = Color.FromArgb(132, 51, 59),
                Invalid = Color.Red,
                Heading = Color.White

            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(90, 90, 90),
                AccentColor = Color.FromArgb(255, 0, 0), // Red
                AccentLightColor = Color.WhiteSmoke
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(150, 38, 56),
                GradientTop = Color.FromArgb(205, 205, 212),
                GradientBottom = Color.FromArgb(90, 90, 98),
                OpenBorder = Color.FromArgb(244, 96, 110),
                EmptyFill = Color.FromArgb(58, 58, 64),
                DisabledFill = Color.FromArgb(36, 36, 40),
                ForeColor = Color.FromArgb(245, 245, 248)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(140, 140, 140),
                Bar = Color.FromArgb(192, 192, 192),
                Hover = Color.FromArgb(220, 220, 220)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(18, 18, 18),
                SummaryText = Color.WhiteSmoke,
                TotalSlotsText = Color.WhiteSmoke,
                SlotsLeftText = Color.FromArgb(244, 119, 130)
            }
        };
        themes.Add(rogue.Name, rogue);
        #endregion

        #region Vigilante Theme
        var vigilante = new ApplicationTheme
        {
            Name = "Vigilante",
            Button = new ButtonTheme
            {
                GradientTop = Color.FromArgb(180, 120, 60),
                GradientBottom = Color.FromArgb(90, 60, 30),
                HoverGradientTop = Color.FromArgb(210, 145, 85),
                HoverGradientBottom = Color.FromArgb(110, 75, 40),
                PressedGradientTop = Color.FromArgb(90, 60, 30),
                PressedGradientBottom = Color.FromArgb(180, 120, 60),
                Border = Color.FromArgb(100, 65, 30),
                ForeColor = Color.WhiteSmoke,
                TextOutlineColor = Color.Black,
                ToggledGradientTop = Color.FromArgb(180, 120, 60),
                ToggledGradientBottom = Color.FromArgb(90, 60, 30),
                ToggledBorderColor = Color.FromArgb(255, 192, 0),
                ToggledTextColor = Color.WhiteSmoke,
                ToggledTextOutlineColor = Color.Black,
            },
            DataView = new DataViewTheme
            {
                Background = Color.FromArgb(10, 8, 6),
                Card = Color.FromArgb(22, 18, 14),
                Border = Color.FromArgb(58, 44, 32),
                Accent = Color.FromArgb(220, 130, 40),

                // Header tabs (amber/bronze)
                HeaderTop = Color.FromArgb(64, 44, 18),
                HeaderBottom = Color.FromArgb(94, 68, 28),
                TabActiveTop = Color.FromArgb(200, 120, 50),
                TabActiveBottom = Color.FromArgb(140, 80, 32),
                TabInactiveTop = Color.FromArgb(34, 28, 24),
                TabInactiveBottom = Color.FromArgb(26, 20, 16),
                TabBorder = Color.FromArgb(14, 10, 8),
                Text = Color.FromArgb(235, 235, 240),
                ValueText = Color.FromArgb(235, 235, 240),
                Muted = Color.FromArgb(185, 190, 200),

                // Controls
                Chip = Color.FromArgb(44, 34, 26),
                ChipActive = Color.FromArgb(96, 70, 40),
                Rail = Color.FromArgb(36, 28, 22),
                RailFill = Color.FromArgb(180, 120, 60),
                Thumb = Color.FromArgb(240, 232, 220),
                ThumbBorder = Color.FromArgb(36, 28, 22),

                // Grid
                GridHeaderTop = Color.FromArgb(40, 34, 26),
                GridHeaderBottom = Color.FromArgb(30, 26, 20),
                GridHeaderBorder = Color.FromArgb(18, 14, 10),
                GridRowEven = Color.FromArgb(22, 18, 14),
                GridRowOdd = Color.FromArgb(18, 14, 12),
                GridRowLine = Color.FromArgb(34, 26, 20),
                GridBandLow = Color.FromArgb(96, 212, 120),
                GridBandHigh = Color.FromArgb(230, 86, 86),
                GridNeutral = Color.FromArgb(185, 190, 200)
            },
            DropDownList = new DropDownListTheme
            {
                GradientTop = Color.FromArgb(180, 120, 60),
                GradientBottom = Color.FromArgb(90, 60, 30),
                HoverGradientTop = Color.FromArgb(210, 145, 85),
                HoverGradientBottom = Color.FromArgb(110, 75, 40),
                Border = Color.FromArgb(100, 65, 30),
                HoverBorder = Color.Gold,
                DropDownBackColor = Color.FromArgb(40, 25, 10),
                DropDownSelectionBackColor = Color.FromArgb(130, 90, 45),
                DropDownSelectionForeColor = Color.White,
                LockColor = Color.FromArgb(170, 130, 90),
                ForeColor = Color.WhiteSmoke,
                Arrow = Color.White,
                HoverArrow = Color.Gold,
                FocusBorder = Color.Gold
            },
            Header = new HeaderTheme
            {
                HeaderLight = Color.FromArgb(99, 64, 42),
                HeaderMid = Color.FromArgb(70, 39, 9),
                HeaderDark = Color.FromArgb(57, 33, 6),
                WindowIcon = Color.WhiteSmoke,
                WindowIconHover = Color.FromArgb(64, 210, 145, 85),
                WindowIconPressed = Color.FromArgb(96, 110, 75, 40),
                WindowIconCloseHover = Color.FromArgb(231, 76, 60),
                WindowIconClosePressed = Color.FromArgb(192, 57, 43),
                LogoTargetColor = Color.FromArgb(205, 127, 50)
            },
            ListView = new ListViewTheme
            {
                ScrollBar = Color.FromArgb(160, 100, 40),
                ScrollButton = Color.FromArgb(205, 127, 50),
                Enabled = Color.FromArgb(131, 82, 33),
                Selected = Color.FromArgb(255, 158, 62),
                Disabled = Color.FromArgb(125, 125, 125),
                SelectedDisabled = Color.FromArgb(99, 62, 25),
                Invalid = Color.Red,
                Heading = Color.FromArgb(255, 182, 47)
            },
            MenuStrip = new MenuStripTheme
            {
                ItemSelectedColor = Color.FromArgb(110, 75, 40),
                AccentColor = Color.Gold,
                AccentLightColor = Color.WhiteSmoke
            },
            PowerSlot = new PowerSlotTheme
            {
                Border = Color.FromArgb(128, 82, 28),
                GradientTop = Color.FromArgb(204, 143, 76),
                GradientBottom = Color.FromArgb(87, 51, 24),
                OpenBorder = Color.FromArgb(255, 204, 124),
                EmptyFill = Color.FromArgb(72, 50, 30),
                DisabledFill = Color.FromArgb(48, 32, 20),
                ForeColor = Color.FromArgb(248, 239, 229)
            },
            ScrollPanel = new ScrollPanelTheme
            {
                Track = Color.FromArgb(160, 100, 40),
                Bar = Color.FromArgb(205, 127, 50),
                Hover = Color.FromArgb(220, 150, 60)
            },
            Footer = new FooterTheme
            {
                Background = Color.FromArgb(18, 14, 12),
                SummaryText = Color.WhiteSmoke,
                TotalSlotsText = Color.WhiteSmoke,
                SlotsLeftText = Color.FromArgb(255, 147, 73)
            }
        };
        themes.Add(vigilante.Name, vigilante);
        #endregion

        foreach (var themeName in themes.Keys.ToArray())
        {
            themes[themeName] = NormalizeTheme(themes[themeName]);
        }

        return themes;
    }

    // Names must match those in CreateBuiltInThemes().
    public static string[] BuiltInThemeNames => ["Hero", "Villain", "Loyalist", "Resistance", "Rogue", "Vigilante"];
}
