using SkiaSharp;
using System;
using System.Drawing;

namespace Mids_Reborn.Controls.Skia
{
    public class SkListItem : IDisposable
    {
        public string Text { get; set; } = string.Empty;
        public string? WrappedText { get; set; } = string.Empty;

        public EItemState ItemState { get; set; } = EItemState.Enabled;
        public EFontFlags FontFlags { get; set; } = EFontFlags.Normal;
        public ETextAlign TextAlign { get; set; } = ETextAlign.Left;

        public int Index { get; set; } = -1;
        public int LineCount { get; set; } = 1;
        public int ItemHeight { get; set; } = 1;

        public int NIdSet { get; set; } = -1;
        public int NIdPower { get; set; } = -1;
        public int IdxPower { get; set; } = -1;
        public string Tag { get; set; } = string.Empty;

        private SKFont? _cachedFont;
        private FontCacheKey? _cachedKey;
        private bool _disposed;

        public bool Bold
        {
            get => FontFlags.HasFlag(EFontFlags.Bold);
            set => FontFlags = value ? FontFlags | EFontFlags.Bold : FontFlags & ~EFontFlags.Bold;
        }

        public bool Italic
        {
            get => FontFlags.HasFlag(EFontFlags.Italic);
            set => FontFlags = value ? FontFlags | EFontFlags.Italic : FontFlags & ~EFontFlags.Italic;
        }

        public bool Underline
        {
            get => FontFlags.HasFlag(EFontFlags.Underline);
            set => FontFlags = value ? FontFlags | EFontFlags.Underline : FontFlags & ~EFontFlags.Underline;
        }

        public bool Strikethrough
        {
            get => FontFlags.HasFlag(EFontFlags.Strikethrough);
            set => FontFlags = value ? FontFlags | EFontFlags.Strikethrough : FontFlags & ~EFontFlags.Strikethrough;
        }

        public SkListItem() { }

        public SkListItem(string text, EItemState state, int nidSet = -1, int idxPower = -1, int nidPower = -1, string tag = "", EFontFlags fontFlags = EFontFlags.Normal, ETextAlign align = ETextAlign.Left)
        {
            Text = text;
            WrappedText = "";
            ItemState = state;
            FontFlags = fontFlags;
            TextAlign = align;
            NIdSet = nidSet;
            IdxPower = idxPower;
            NIdPower = nidPower;
            Tag = tag;
            LineCount = 1;
            ItemHeight = 1;
            Index = -1;
        }

        public SkListItem(SkListItem other)
        {
            Text = other.Text;
            WrappedText = other.WrappedText;
            ItemState = other.ItemState;
            FontFlags = other.FontFlags;
            TextAlign = other.TextAlign;
            LineCount = other.LineCount;
            ItemHeight = other.ItemHeight;
            Index = other.Index;
            NIdSet = other.NIdSet;
            IdxPower = other.IdxPower;
            NIdPower = other.NIdPower;
            Tag = other.Tag;
        }

        public SKFont GetOrCreateFont(Font baseFont)
        {
            const float dpi = 96f;
            const float pointsPerInch = 72f;

            var key = new FontCacheKey(baseFont.FontFamily.Name, baseFont.Size, FontFlags);

            if (_cachedFont != null && _cachedKey == key)
                return _cachedFont;

            _cachedFont?.Dispose();

            var weight = FontFlags.HasFlag(EFontFlags.Bold) ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            var slant = FontFlags.HasFlag(EFontFlags.Italic) ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;
            var width = SKFontStyleWidth.Normal;

            var style = new SKFontStyle(weight, width, slant);
            var typeface = SKTypeface.FromFamilyName(baseFont.FontFamily.Name, style);

            float sizePx = (baseFont.Size / pointsPerInch) * dpi;
            _cachedFont = new SKFont(typeface, sizePx);
            _cachedKey = key;

            return _cachedFont;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _cachedFont?.Dispose();
            _cachedFont = null;
            _cachedKey = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~SkListItem()
        {
            Dispose();
        }
    }

    internal sealed record FontCacheKey(string FontFamily, float Size, EFontFlags Flags);

    [Flags]
    public enum EFontFlags
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikethrough = 8
    }

    public enum EItemState
    {
        Enabled,
        Selected,
        Disabled,
        SelectedDisabled,
        Invalid,
        Heading
    }

    public enum ETextAlign
    {
        Left,
        Center,
        Right
    }
}

