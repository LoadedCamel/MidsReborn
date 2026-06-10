using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

public sealed record SpecialPowerFlyoutRequest(
    SpecialPowerCategory Category,
    string Title,
    Rectangle AnchorBounds,
    IReadOnlyList<IPower> Powers,
    IReadOnlyList<SpecialPowerChipOption>? ChipOptions = null,
    string? SelectedChipKey = null);

public sealed class SpecialPowerSelectionEventArgs : EventArgs
{
    public SpecialPowerSelectionEventArgs(IPower power, MouseButtons button, MidsItemState itemState)
    {
        Power = power;
        Button = button;
        ItemState = itemState;
    }

    public IPower Power { get; }
    public MouseButtons Button { get; }
    public MidsItemState ItemState { get; }
}

public sealed class SpecialPowerHoverEventArgs : EventArgs
{
    public SpecialPowerHoverEventArgs(IPower? power, Rectangle screenBounds)
    {
        Power = power;
        ScreenBounds = screenBounds;
    }

    public IPower? Power { get; }
    public Rectangle ScreenBounds { get; }
}

public sealed class SpecialPowerChipChangedEventArgs : EventArgs
{
    public SpecialPowerChipChangedEventArgs(SpecialPowerChipOption chip)
    {
        Chip = chip;
    }

    public SpecialPowerChipOption Chip { get; }
    public string Key => Chip.Key;
    public string Label => Chip.Label;
}

public sealed class SpecialPowerFlyout : UserControl
{
    private static readonly Size BaseFlyoutSize = new(440, 420);
    private static readonly Size BaseMinimumSize = new(320, 280);
    private static readonly Size BaseHostSize = new(1500, 900);
    private const float MinUiScale = 0.9f;
    private const float MaxUiScale = 1.25f;
    private const int BasePaddingSize = 8;
    private const int BaseHeaderHeight = 34;
    private const int BaseCloseButtonWidth = 70;
    private const int BaseCloseButtonHeight = 26;
    private const int BaseCloseButtonTop = 4;
    private const int BaseChipHeight = 24;
    private const int BaseChipMinWidth = 72;
    private const int BaseChipHorizontalPadding = 22;
    private const int BaseChipGap = 6;
    private const int BaseChipPanelMinimumHeight = 30;
    private const int BaseChipBottomPadding = 4;
    private const int BasePowerListPaddingX = 6;
    private const int BasePowerListPaddingY = 4;
    private const float BaseTitleFontSize = 11f;
    private const float BaseActionFontSize = 8.5f;
    private const float BaseListFontSize = 9f;

    private readonly Panel _headerPanel;
    private readonly Label _titleLabel;
    private readonly MidsVectorButton _closeButton;
    private readonly Panel _chipPanel;
    private readonly MidsListView _powerList;
    private readonly Dictionary<string, MidsVectorButton> _chipButtons = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Control, Font> _ownedFonts = new();

    private SpecialPowerFlyoutRequest? _request;
    private float _uiScale = 1f;

    public event EventHandler<SpecialPowerSelectionEventArgs>? PowerClicked;
    public event EventHandler<SpecialPowerHoverEventArgs>? PowerHovered;
    public event EventHandler<SpecialPowerChipChangedEventArgs>? ChipChanged;
    public event EventHandler? Closed;

    public SpecialPowerFlyoutRequest? Request => _request;
    public bool IsOpen => Visible;

    public SpecialPowerFlyout()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.UserPaint,
            true);

        BackColor = Color.Black;
        Padding = new Padding(BasePaddingSize);
        MinimumSize = BaseMinimumSize;
        Size = BaseFlyoutSize;
        Visible = false;
        TabStop = true;

        _headerPanel = new Panel
        {
            Height = BaseHeaderHeight,
            BackColor = Color.Transparent
        };

        _closeButton = new MidsVectorButton
        {
            ButtonType = MidsVectorButton.ButtonTypes.Normal,
            Text = "Close",
            Size = new Size(BaseCloseButtonWidth, BaseCloseButtonHeight),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(Width - BaseCloseButtonWidth - (BasePaddingSize * 2), BaseCloseButtonTop)
        };
        _closeButton.Click += (_, _) => CloseFlyout();

        _titleLabel = new Label
        {
            AutoEllipsis = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.Transparent
        };

        _headerPanel.Controls.Add(_titleLabel);
        _headerPanel.Controls.Add(_closeButton);

        _chipPanel = new Panel
        {
            Height = 0,
            Margin = new Padding(0, 0, 0, 6),
            Padding = new Padding(0, 0, 0, BaseChipBottomPadding),
            Visible = false
        };

        _powerList = new MidsListView
        {
            PaddingX = BasePowerListPaddingX,
            PaddingY = BasePowerListPaddingY,
            LineSpacing = -1,
            Scrollable = true
        };
        _powerList.ItemClicked += PowerList_ItemClicked;
        _powerList.ItemHovered += PowerList_ItemHovered;

        Controls.Add(_powerList);
        Controls.Add(_chipPanel);
        Controls.Add(_headerPanel);

        UpdateResponsiveLayout(BaseHostSize);
        ApplyTheme();
        if (!DesignMode)
        {
            ThemeManager.ThemeChanged += ApplyTheme;
        }
    }

    public void Open(SpecialPowerFlyoutRequest request)
    {
        SuspendLayout();

        _request = request;
        Visible = true;
        _titleLabel.Text = request.Title;

        ConfigureChipPanel(request);
        ConfigureListItems(request);
        ApplyTheme();
        UpdateContentLayout();
        ResumeLayout(performLayout: true);
        BringToFront();
        Select();
    }

    public void UpdateResponsiveLayout(Size hostClientSize)
    {
        if (hostClientSize.Width <= 0 || hostClientSize.Height <= 0)
        {
            return;
        }

        _uiScale = ComputeScale(hostClientSize);

        MinimumSize = new Size(ScalePx(BaseMinimumSize.Width), ScalePx(BaseMinimumSize.Height));

        var maximumWidth = Math.Max(MinimumSize.Width, (int)Math.Round(hostClientSize.Width * 0.46f));
        var maximumHeight = Math.Max(MinimumSize.Height, (int)Math.Round(hostClientSize.Height * 0.68f));
        Size = new Size(
            Math.Clamp(ScalePx(BaseFlyoutSize.Width), MinimumSize.Width, maximumWidth),
            Math.Clamp(ScalePx(BaseFlyoutSize.Height), MinimumSize.Height, maximumHeight));

        Padding = new Padding(ScalePx(BasePaddingSize));
        _headerPanel.Height = ScalePx(BaseHeaderHeight);
        _closeButton.Size = new Size(ScalePx(BaseCloseButtonWidth), ScalePx(BaseCloseButtonHeight));
        _chipPanel.Margin = new Padding(0, 0, 0, ScalePx(BaseChipGap));
        _chipPanel.Padding = new Padding(0, 0, 0, ScalePx(BaseChipBottomPadding));
        _chipPanel.MinimumSize = new Size(0, ScalePx(BaseChipPanelMinimumHeight));
        _powerList.PaddingX = ScalePx(BasePowerListPaddingX);
        _powerList.PaddingY = ScalePx(BasePowerListPaddingY);

        ApplyScaledFont(_titleLabel, BaseTitleFontSize, FontStyle.Bold);
        ApplyScaledFont(_closeButton, BaseActionFontSize, FontStyle.Bold);
        ApplyScaledFont(_powerList, BaseListFontSize, FontStyle.Bold);
        UpdateContentLayout();
        Invalidate();
    }

    public void CloseFlyout()
    {
        if (!Visible)
        {
            return;
        }

        Visible = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !DesignMode)
        {
            ThemeManager.ThemeChanged -= ApplyTheme;
        }

        if (disposing)
        {
            DisposeOwnedFonts();
        }

        base.Dispose(disposing);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_closeButton is null)
        {
            return;
        }

        _closeButton.Location = new Point(
            Math.Max(0, _headerPanel.Width - _closeButton.Width),
            ScalePx(BaseCloseButtonTop));
        UpdateContentLayout();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var theme = ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        using var surfaceBrush = new SolidBrush(theme.Card);
        using var borderPen = new Pen(Blend(theme.Border, theme.TabActiveBottom, 0.55f), 1.5f);

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.FillRectangle(surfaceBrush, ClientRectangle);
        e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
    }

    private void ApplyTheme()
    {
        var theme = ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
        var config = MidsContext.Config;
        var alignment = MidsContext.Character?.Alignment ?? Enums.Alignment.Hero;
        var isHero = AccoladeSideRules.IsHeroSideAlignment(alignment);
        var enabledColor = config?.RtFont.ColorPowerAvailable ?? theme.Text;
        var disabledColor = config?.RtFont.ColorPowerDisabled ?? theme.Muted;
        var selectedColor = isHero
            ? config?.RtFont.ColorPowerTakenHero ?? theme.Accent
            : config?.RtFont.ColorPowerTakenVillain ?? theme.Accent;
        var selectedDisabledColor = isHero
            ? config?.RtFont.ColorPowerTakenDarkHero ?? theme.Muted
            : config?.RtFont.ColorPowerTakenDarkVillain ?? theme.Muted;

        BackColor = theme.Card;
        _headerPanel.BackColor = Color.Transparent;
        _chipPanel.BackColor = theme.Card;
        _titleLabel.ForeColor = theme.Text;
        _powerList.BackColor = theme.Background;
        _powerList.HoverColor = theme.ChipActive;
        _powerList.SetStateColor(MidsItemState.Enabled, enabledColor);
        _powerList.SetStateColor(MidsItemState.Selected, selectedColor);
        _powerList.SetStateColor(MidsItemState.Disabled, disabledColor);
        _powerList.SetStateColor(MidsItemState.SelectedDisabled, selectedDisabledColor);
        _powerList.SetStateColor(MidsItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
        _powerList.SetStateColor(MidsItemState.Heading, theme.Text);

        Invalidate();
    }

    private void ConfigureChipPanel(SpecialPowerFlyoutRequest request)
    {
        DisposeChipButtons();
        _chipButtons.Clear();
        _chipPanel.Controls.Clear();

        var chipOptions = request.ChipOptions ?? [];
        if (chipOptions.Count == 0)
        {
            _chipPanel.Visible = false;
            _chipPanel.Height = 0;
            return;
        }

        foreach (var chipOption in chipOptions)
        {
            var chip = new MidsVectorButton
            {
                ButtonType = MidsVectorButton.ButtonTypes.Toggle,
                ToggleState = string.Equals(chipOption.Key, request.SelectedChipKey, StringComparison.OrdinalIgnoreCase)
                    ? MidsVectorButton.States.ToggledOn
                    : MidsVectorButton.States.ToggledOff,
                ToggleText =
                {
                    ToggledOff = chipOption.Label,
                    ToggledOn = chipOption.Label,
                    Indeterminate = chipOption.Label
                },
                Text = chipOption.Label,
                CornerRadius = 6,
                Tag = chipOption
            };

            chip.Click += Chip_Click;
            _chipButtons[chipOption.Key] = chip;
            _chipPanel.Controls.Add(chip);
        }

        _chipPanel.Visible = true;
        UpdateContentLayout();
    }

    private void ConfigureListItems(SpecialPowerFlyoutRequest request)
    {
        var pairedBold = MidsContext.Config?.RtFont.PairedBold ?? false;
        var toon = MidsContext.Character as Toon ?? MainModule.MidsController.Toon;

        var items = new List<MidsListViewItem>(request.Powers.Count);
        foreach (var power in request.Powers)
        {
            var state = ResolvePowerState(toon, power);
            var item = new PowerListViewItem(
                power.DisplayName,
                state,
                nidSet: power.PowerSetID,
                idxPower: power.PowerSetIndex,
                nidPower: power.PowerIndex,
                tag: power,
                style: MidsItemFontStyles.Bold)
            {
                Bold = pairedBold,
                Italic = state == MidsItemState.Invalid
            };
            item.LeadingImage = ResolveLeadingImage(power);

            items.Add(item);
        }

        _powerList.Items = items;
    }

    private static Image? ResolveLeadingImage(IPower power)
    {
        var icon = AssetManager.GetPowerImage(power);
        if (icon == AssetManager.UnknownIcon)
        {
            icon = AssetManager.GetPowersetImage(power);
        }

        return icon == AssetManager.UnknownIcon
            ? null
            : icon?.Bitmap;
    }

    private static MidsItemState ResolvePowerState(Toon? toon, IPower power)
    {
        if (toon == null)
        {
            return MidsItemState.Enabled;
        }

        var matchingEntry = SpecialPowerBuildIdentityResolver.FindMatchingEntry(toon.CurrentBuild, power);
        var message = string.Empty;
        if (matchingEntry?.Power != null)
        {
            var selectedState = toon.PowerState(matchingEntry.Power.PowerIndex, ref message);
            return selectedState == MidsItemState.SelectedDisabled
                ? MidsItemState.SelectedDisabled
                : MidsItemState.Selected;
        }

        return toon.PowerState(power.PowerIndex, ref message);
    }

    private void Chip_Click(object? sender, EventArgs e)
    {
        if (sender is not MidsVectorButton chip || chip.Tag is not SpecialPowerChipOption chipOption)
        {
            return;
        }

        if (_request is { SelectedChipKey: not null } &&
            string.Equals(_request.SelectedChipKey, chipOption.Key, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        ChipChanged?.Invoke(this, new SpecialPowerChipChangedEventArgs(chipOption));
    }

    private void PowerList_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
    {
        if (e.Item is not PowerListViewItem item || item.Tag is not IPower power)
        {
            return;
        }

        PowerClicked?.Invoke(this, new SpecialPowerSelectionEventArgs(power, e.Button, item.State));
    }

    private void PowerList_ItemHovered(object? sender, MidsListViewItemHoverEventArgs e)
    {
        if (e.Item is not PowerListViewItem item || item.Tag is not IPower power || e.ItemIndex < 0)
        {
            PowerHovered?.Invoke(this, new SpecialPowerHoverEventArgs(null, Rectangle.Empty));
            return;
        }

        var screenRect = new Rectangle(_powerList.PointToScreen(e.ItemBounds.Location), e.ItemBounds.Size);
        PowerHovered?.Invoke(this, new SpecialPowerHoverEventArgs(power, screenRect));
    }

    private void UpdateContentLayout()
    {
        var contentLeft = Padding.Left;
        var contentTop = Padding.Top;
        var contentWidth = Math.Max(0, ClientSize.Width - Padding.Horizontal);

        _headerPanel.Bounds = new Rectangle(contentLeft, contentTop, contentWidth, ScalePx(BaseHeaderHeight));
        _closeButton.Location = new Point(
            Math.Max(0, _headerPanel.Width - _closeButton.Width),
            ScalePx(BaseCloseButtonTop));

        var listTop = _headerPanel.Bottom;
        if (_chipPanel.Visible)
        {
            _chipPanel.Location = new Point(contentLeft, _headerPanel.Bottom + ScalePx(BaseChipGap));
            _chipPanel.Width = contentWidth;
            _chipPanel.Height = Math.Max(_chipPanel.MinimumSize.Height, LayoutChipButtons());
            listTop = _chipPanel.Bottom;
        }
        else
        {
            _chipPanel.Bounds = new Rectangle(contentLeft, _headerPanel.Bottom, contentWidth, 0);
        }

        _powerList.Bounds = new Rectangle(
            contentLeft,
            listTop + ScalePx(BaseChipGap),
            contentWidth,
            Math.Max(0, ClientSize.Height - Padding.Bottom - listTop - ScalePx(BaseChipGap)));
    }

    private int LayoutChipButtons()
    {
        if (!_chipPanel.Visible)
        {
            return 0;
        }

        var gap = ScalePx(BaseChipGap);
        var availableWidth = Math.Max(
            ScalePx(BaseChipMinWidth),
            _chipPanel.ClientSize.Width > 0
                ? _chipPanel.ClientSize.Width
                : Width - Padding.Horizontal);

        var x = 0;
        var y = 0;
        var rowHeight = 0;

        foreach (var pair in _chipButtons)
        {
            var chip = pair.Value;
            ApplyScaledFont(chip, BaseActionFontSize, FontStyle.Bold);
            chip.Height = ScalePx(BaseChipHeight);
            chip.Width = Math.Max(
                ScalePx(BaseChipMinWidth),
                TextRenderer.MeasureText(chip.Text, chip.Font).Width + ScalePx(BaseChipHorizontalPadding));

            if (x > 0 && x + chip.Width > availableWidth)
            {
                x = 0;
                y += rowHeight + gap;
                rowHeight = 0;
            }

            chip.Location = new Point(x, y);
            x += chip.Width + gap;
            rowHeight = Math.Max(rowHeight, chip.Height);
        }

        return y + rowHeight + _chipPanel.Padding.Bottom;
    }

    private void DisposeChipButtons()
    {
        var existingControls = _chipPanel.Controls.Cast<Control>().ToArray();
        foreach (var control in existingControls)
        {
            ReleaseOwnedFont(control);
            control.Dispose();
        }
    }

    private void ApplyScaledFont(Control control, float baseSize, FontStyle style)
    {
        var scaledSize = Math.Max(7.5f, (float)Math.Round(baseSize * _uiScale, 1));
        if (_ownedFonts.TryGetValue(control, out var existingFont) &&
            Math.Abs(existingFont.SizeInPoints - scaledSize) < 0.05f &&
            existingFont.Style == style)
        {
            return;
        }

        var font = new Font("Noto Sans SemiBold", scaledSize, style, GraphicsUnit.Point);
        control.Font = font;

        ReleaseOwnedFont(control);
        _ownedFonts[control] = font;
    }

    private void ReleaseOwnedFont(Control control)
    {
        if (_ownedFonts.Remove(control, out var font))
        {
            font.Dispose();
        }
    }

    private void DisposeOwnedFonts()
    {
        foreach (var font in _ownedFonts.Values)
        {
            font.Dispose();
        }

        _ownedFonts.Clear();
    }

    private static float ComputeScale(Size hostClientSize)
    {
        var widthScale = hostClientSize.Width / (float)BaseHostSize.Width;
        var heightScale = hostClientSize.Height / (float)BaseHostSize.Height;
        return Math.Clamp(Math.Min(widthScale, heightScale), MinUiScale, MaxUiScale);
    }

    private int ScalePx(int value)
    {
        return Math.Max(1, (int)Math.Round(value * _uiScale));
    }

    private static Color Blend(Color first, Color second, float amountSecond)
    {
        amountSecond = Math.Clamp(amountSecond, 0f, 1f);
        var amountFirst = 1f - amountSecond;

        return Color.FromArgb(
            255,
            (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
            (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
            (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
    }

}
