using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls;

[DesignerCategory("Code")]
internal sealed class PetActorBonusesView : Panel
{
    private const float MinimumResponsiveUiScale = 0.90f;
    private readonly SummaryCard _uniqueSourcesCard;
    private readonly SummaryCard _totalBonusesCard;
    private readonly ToolTip _toolTip = new();
    private readonly List<BonusEntryCard> _cards = [];
    private IReadOnlyList<PetAppliedBonusEntry> _entries = Array.Empty<PetAppliedBonusEntry>();
    private float _uiScale = 1f;

    public PetActorBonusesView()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);

        AutoScroll = false;
        BackColor = Color.Transparent;
        Margin = Padding.Empty;
        Padding = Padding.Empty;

        _uniqueSourcesCard = new SummaryCard
        {
            Title = "Unique Sources",
            AccentColor = Color.FromArgb(145, 200, 255)
        };
        _totalBonusesCard = new SummaryCard
        {
            Title = "Total Bonuses",
            AccentColor = Color.FromArgb(241, 194, 80)
        };

        Controls.Add(_uniqueSourcesCard);
        Controls.Add(_totalBonusesCard);
    }

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, MinimumResponsiveUiScale, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f)
            {
                return;
            }

            _uiScale = clamped;
            _uniqueSourcesCard.UiScale = clamped;
            _totalBonusesCard.UiScale = clamped;
            foreach (var card in _cards)
            {
                card.UiScale = clamped;
            }

            PerformLayout();
            Invalidate();
        }
    }

    public void Clear()
    {
        SetEntries(Array.Empty<PetAppliedBonusEntry>());
    }

    public void SetEntries(IReadOnlyList<PetAppliedBonusEntry> entries)
    {
        _entries = entries ?? Array.Empty<PetAppliedBonusEntry>();

        while (_cards.Count > _entries.Count)
        {
            var lastIndex = _cards.Count - 1;
            var card = _cards[lastIndex];
            Controls.Remove(card);
            card.Dispose();
            _cards.RemoveAt(lastIndex);
        }

        for (var index = 0; index < _entries.Count; index++)
        {
            if (index >= _cards.Count)
            {
                var card = new BonusEntryCard();
                card.UiScale = _uiScale;
                _cards.Add(card);
                Controls.Add(card);
            }

            _cards[index].UiScale = _uiScale;
            _cards[index].Entry = _entries[index];
            _toolTip.SetToolTip(_cards[index], _entries[index].Tooltip);
        }

        _uniqueSourcesCard.ValueText = _entries.Count.ToString();
        _totalBonusesCard.ValueText = _entries.Sum(entry => entry.StatDeltas.Count).ToString();

        PerformLayout();
        Invalidate();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(360, Width);
        return new Size(width, MeasureHeight(width));
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);

        if (_uniqueSourcesCard == null || _totalBonusesCard == null)
        {
            return;
        }

        var inset = ScalePx(4);
        var gap = ScalePx(8);
        var summaryGap = ScalePx(8);
        var availableWidth = Math.Max(200, ClientSize.Width - inset * 2);
        var summaryWidth = Math.Max(ScalePx(120), (availableWidth - summaryGap) / 2);
        var summaryHeight = _uniqueSourcesCard.GetPreferredSize(new Size(summaryWidth, 0)).Height;

        _uniqueSourcesCard.Bounds = new Rectangle(inset, inset, summaryWidth, summaryHeight);
        _totalBonusesCard.Bounds = new Rectangle(_uniqueSourcesCard.Right + summaryGap, inset, summaryWidth, summaryHeight);

        var y = _uniqueSourcesCard.Bottom + gap;
        var cardWidth = availableWidth;
        foreach (var card in _cards)
        {
            var preferred = card.GetPreferredSize(new Size(cardWidth, 0));
            card.Bounds = new Rectangle(inset, y, cardWidth, preferred.Height);
            y = card.Bottom + gap;
        }

        Height = Math.Max(0, y - gap + inset);
    }

    private int MeasureHeight(int width)
    {
        if (_uniqueSourcesCard == null)
        {
            return 0;
        }

        var inset = ScalePx(4);
        var gap = ScalePx(8);
        var summaryGap = ScalePx(8);
        var availableWidth = Math.Max(200, width - inset * 2);
        var summaryWidth = Math.Max(ScalePx(120), (availableWidth - summaryGap) / 2);
        var summaryHeight = _uniqueSourcesCard.GetPreferredSize(new Size(summaryWidth, 0)).Height;
        var totalHeight = inset + summaryHeight + gap + inset;

        foreach (var card in _cards)
        {
            totalHeight += card.GetPreferredSize(new Size(availableWidth, 0)).Height + gap;
        }

        return Math.Max(totalHeight, summaryHeight + inset * 2);
    }

    private int ScalePx(int value)
        => Math.Max(1, (int)Math.Round(value * _uiScale));

    private sealed class SummaryCard : Control
    {
        private float _uiScale = 1f;

        public string Title { get; set; } = string.Empty;
        public string ValueText { get; set; } = "0";
        public Color AccentColor { get; set; } = Color.FromArgb(145, 200, 255);

        public SummaryCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Margin = Padding.Empty;
        }

        public float UiScale
        {
            get => _uiScale;
            set
            {
                var clamped = Math.Clamp(value, MinimumResponsiveUiScale, 1.25f);
                if (Math.Abs(_uiScale - clamped) < 0.01f)
                {
                    return;
                }

                _uiScale = clamped;
                Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var width = proposedSize.Width > 0 ? proposedSize.Width : 160;
            return new Size(width, ScalePx(62));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var theme = CurrentTheme;
            var bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var path = RoundedRect(bounds, ScalePx(9));
            using var fillBrush = new LinearGradientBrush(
                bounds,
                Blend(theme.Card, theme.Background, 0.04f),
                Blend(theme.Background, theme.Card, 0.06f),
                LinearGradientMode.Vertical);
            using var borderPen = new Pen(Color.FromArgb(132, theme.GridHeaderBorder));
            e.Graphics.FillPath(fillBrush, path);
            e.Graphics.DrawPath(borderPen, path);

            var titleBounds = new Rectangle(bounds.X + ScalePx(12), bounds.Y + ScalePx(9), bounds.Width - ScalePx(24), ScalePx(16));
            var valueBounds = new Rectangle(bounds.X + ScalePx(12), bounds.Y + ScalePx(24), bounds.Width - ScalePx(24), bounds.Height - ScalePx(30));

            using var titleFont = new Font("Segoe UI", Math.Max(6f, 8f * _uiScale), FontStyle.Bold);
            using var valueFont = new Font("Segoe UI", Math.Max(10f, 16f * _uiScale), FontStyle.Bold);

            TextRenderer.DrawText(
                e.Graphics,
                Title.ToUpperInvariant(),
                titleFont,
                titleBounds,
                Blend(theme.Muted, Color.White, 0.18f),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            TextRenderer.DrawText(
                e.Graphics,
                ValueText,
                valueFont,
                valueBounds,
                AccentColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        private DataViewTheme CurrentTheme => DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

        private int ScalePx(int value)
            => Math.Max(1, (int)Math.Round(value * _uiScale));
    }

    private sealed class BonusEntryCard : Control
    {
        private const int HorizontalInset = 12;
        private const int VerticalInset = 10;
        private const int ChipHeight = 22;
        private const int ChipGap = 6;

        private PetAppliedBonusEntry? _entry;
        private float _uiScale = 1f;

        public PetAppliedBonusEntry? Entry
        {
            get => _entry;
            set
            {
                _entry = value;
                Invalidate();
            }
        }

        public float UiScale
        {
            get => _uiScale;
            set
            {
                var clamped = Math.Clamp(value, MinimumResponsiveUiScale, 1.25f);
                if (Math.Abs(_uiScale - clamped) < 0.01f)
                {
                    return;
                }

                _uiScale = clamped;
                Invalidate();
            }
        }

        public BonusEntryCard()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Margin = Padding.Empty;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            var width = proposedSize.Width > 0 ? proposedSize.Width : 360;
            return new Size(width, MeasureHeight(width));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var theme = CurrentTheme;
            var entry = _entry;
            var bounds = Rectangle.Inflate(ClientRectangle, -1, -1);
            if (entry == null || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var path = RoundedRect(bounds, ScalePx(9));
            using var fillBrush = new LinearGradientBrush(
                bounds,
                Blend(theme.Card, theme.Background, 0.05f),
                Blend(theme.Background, theme.Card, 0.02f),
                LinearGradientMode.Vertical);
            using var borderPen = new Pen(Color.FromArgb(142, theme.GridHeaderBorder));
            e.Graphics.FillPath(fillBrush, path);
            e.Graphics.DrawPath(borderPen, path);

            using var titleFont = new Font("Segoe UI", Math.Max(7.5f, 9.75f * _uiScale), FontStyle.Bold);
            using var badgeFont = new Font("Segoe UI", Math.Max(6.25f, 7.5f * _uiScale), FontStyle.Bold);
            using var summaryFont = new Font("Segoe UI", Math.Max(6.75f, 8.5f * _uiScale), FontStyle.Regular);

            var horizontalInset = ScalePx(HorizontalInset);
            var verticalInset = ScalePx(VerticalInset);
            var chipHeight = ScalePx(ChipHeight);
            var chipGap = ScalePx(ChipGap);
            var availableWidth = Math.Max(1, bounds.Width - horizontalInset * 2);
            var badgeText = FormatSourceType(entry.SourceType);
            var badgeSize = TextRenderer.MeasureText(
                e.Graphics,
                badgeText,
                badgeFont,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding);
            var badgeBounds = new Rectangle(
                bounds.Right - horizontalInset - Math.Max(ScalePx(60), badgeSize.Width + ScalePx(18)),
                bounds.Y + verticalInset - ScalePx(1),
                Math.Max(ScalePx(60), badgeSize.Width + ScalePx(18)),
                ScalePx(20));

            var titleBounds = new Rectangle(
                bounds.X + horizontalInset,
                bounds.Y + verticalInset,
                Math.Max(20, availableWidth - badgeBounds.Width - 10),
                ScalePx(20));
            TextRenderer.DrawText(
                e.Graphics,
                entry.SourceName,
                titleFont,
                titleBounds,
                theme.ValueText,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            DrawBadge(e.Graphics, badgeBounds, badgeFont, badgeText, theme, GetSourceAccent(entry.SourceType));

            var summaryTop = titleBounds.Bottom + ScalePx(8);
            var summaryWidth = Math.Max(20, availableWidth);
            var summaryText = string.IsNullOrWhiteSpace(entry.Summary)
                ? "No visible delta summary."
                : entry.Summary;
            var summarySize = TextRenderer.MeasureText(
                e.Graphics,
                summaryText,
                summaryFont,
                new Size(summaryWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            var summaryBounds = new Rectangle(
                bounds.X + horizontalInset,
                summaryTop,
                summaryWidth,
                summarySize.Height);
            TextRenderer.DrawText(
                e.Graphics,
                summaryText,
                summaryFont,
                summaryBounds,
                theme.Text,
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

            var chipY = summaryBounds.Bottom + ScalePx(10);
            var chipX = bounds.X + horizontalInset;
            var maxX = bounds.Right - horizontalInset;
            using var chipFont = new Font("Segoe UI", Math.Max(6.25f, 7.75f * _uiScale), FontStyle.Bold);

            foreach (var delta in entry.StatDeltas)
            {
                var chipText = delta.ToDisplayString();
                var chipWidth = Math.Max(ScalePx(84), TextRenderer.MeasureText(
                    e.Graphics,
                    chipText,
                    chipFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding).Width + ScalePx(18));

                if (chipX + chipWidth > maxX)
                {
                    chipX = bounds.X + horizontalInset;
                    chipY += chipHeight + chipGap;
                }

                var chipBounds = new Rectangle(chipX, chipY, chipWidth, chipHeight);
                DrawChip(e.Graphics, chipBounds, chipFont, chipText, delta.Delta >= 0f);
                chipX += chipWidth + chipGap;
            }
        }

        private int MeasureHeight(int width)
        {
            var entry = _entry;
            if (entry == null)
            {
                return 0;
            }

            using var graphics = CreateGraphics();
            using var summaryFont = new Font("Segoe UI", Math.Max(6.75f, 8.5f * _uiScale), FontStyle.Regular);
            using var chipFont = new Font("Segoe UI", Math.Max(6.25f, 7.75f * _uiScale), FontStyle.Bold);

            var contentWidth = Math.Max(ScalePx(140), width - ScalePx(HorizontalInset) * 2 - 2);
            var summarySize = TextRenderer.MeasureText(
                graphics,
                string.IsNullOrWhiteSpace(entry.Summary) ? "No visible delta summary." : entry.Summary,
                summaryFont,
                new Size(contentWidth, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);

            var x = 0;
            var rows = 1;
            foreach (var delta in entry.StatDeltas)
            {
                var chipWidth = Math.Max(ScalePx(84), TextRenderer.MeasureText(
                    graphics,
                    delta.ToDisplayString(),
                    chipFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding).Width + ScalePx(18));

                if (x > 0 && x + chipWidth > contentWidth)
                {
                    rows++;
                    x = 0;
                }

                x += chipWidth + ScalePx(ChipGap);
            }

            var chipHeight = ScalePx(ChipHeight);
            var chipGap = ScalePx(ChipGap);
            var chipsHeight = entry.StatDeltas.Count == 0 ? 0 : rows * chipHeight + (rows - 1) * chipGap;
            return ScalePx(VerticalInset) + ScalePx(20) + ScalePx(8) + summarySize.Height + (chipsHeight > 0 ? ScalePx(10) + chipsHeight : 0) + ScalePx(VerticalInset);
        }

        private void DrawChip(Graphics graphics, Rectangle bounds, Font font, string text, bool positive)
        {
            var fillTop = positive ? Color.FromArgb(36, 115, 76) : Color.FromArgb(122, 60, 44);
            var fillBottom = positive ? Color.FromArgb(22, 72, 48) : Color.FromArgb(78, 38, 28);
            var border = positive ? Color.FromArgb(110, 168, 120) : Color.FromArgb(168, 112, 96);

            using var chipPath = RoundedRect(bounds, bounds.Height / 2);
            using var fillBrush = new LinearGradientBrush(bounds, fillTop, fillBottom, LinearGradientMode.Vertical);
            using var borderPen = new Pen(border);
            graphics.FillPath(fillBrush, chipPath);
            graphics.DrawPath(borderPen, chipPath);

            TextRenderer.DrawText(
                graphics,
                text,
                font,
                bounds,
                Color.WhiteSmoke,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        private static void DrawBadge(Graphics graphics, Rectangle bounds, Font font, string text, DataViewTheme theme, Color accent)
        {
            using var badgePath = RoundedRect(bounds, bounds.Height / 2);
            using var fillBrush = new LinearGradientBrush(
                bounds,
                Color.FromArgb(26, accent),
                Color.FromArgb(10, accent),
                LinearGradientMode.Vertical);
            using var borderPen = new Pen(Color.FromArgb(118, accent));
            graphics.FillPath(fillBrush, badgePath);
            graphics.DrawPath(borderPen, badgePath);

            TextRenderer.DrawText(
                graphics,
                text,
                font,
                bounds,
                theme.ValueText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
        }

        private static string FormatSourceType(PetAppliedBonusSourceType sourceType)
        {
            return sourceType switch
            {
                PetAppliedBonusSourceType.UpgradeOverlay => "Upgrade",
                PetAppliedBonusSourceType.SetBonus => "Set Bonus",
                PetAppliedBonusSourceType.OwnerAuraOrBuff => "Owner Buff",
                PetAppliedBonusSourceType.PetAutoToggle => "Auto / Toggle",
                PetAppliedBonusSourceType.PetClickBuff => "Click Buff",
                _ => "Bonus"
            };
        }

        private static Color GetSourceAccent(PetAppliedBonusSourceType sourceType)
        {
            return sourceType switch
            {
                PetAppliedBonusSourceType.UpgradeOverlay => Color.FromArgb(127, 203, 255),
                PetAppliedBonusSourceType.SetBonus => Color.FromArgb(243, 195, 92),
                PetAppliedBonusSourceType.OwnerAuraOrBuff => Color.FromArgb(143, 232, 168),
                PetAppliedBonusSourceType.PetAutoToggle => Color.FromArgb(199, 164, 255),
                PetAppliedBonusSourceType.PetClickBuff => Color.FromArgb(255, 156, 112),
                _ => Color.FromArgb(210, 210, 210)
            };
        }

        private DataViewTheme CurrentTheme => DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

        private int ScalePx(int value)
            => Math.Max(1, (int)Math.Round(value * _uiScale));
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        radius = Math.Max(2, radius);
        var diameter = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static Color Blend(Color a, Color b, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var inverse = 1f - amount;
        return Color.FromArgb(
            (int)Math.Round(a.A * inverse + b.A * amount),
            (int)Math.Round(a.R * inverse + b.R * amount),
            (int)Math.Round(a.G * inverse + b.G * amount),
            (int)Math.Round(a.B * inverse + b.B * amount));
    }
}
