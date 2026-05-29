using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Diagnostics;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    public sealed class MidsListViewItemHoverEventArgs : EventArgs
    {
        public MidsListViewItem? Item { get; }
        public Point Location { get; }          // Mouse location in control (Client) coordinates
        public Rectangle ItemBounds { get; }    // Drawn bounds of the hovered item (Client coords)
        public int ItemIndex { get; }           // -1 when hovering nothing

        public MidsListViewItemHoverEventArgs(MidsListViewItem? item, Point location, Rectangle itemBounds, int itemIndex)
        {
            Item = item;
            Location = location;
            ItemBounds = itemBounds;
            ItemIndex = itemIndex;
        }
    }

    public sealed class MidsListViewItemClickEventArgs : EventArgs
    {
        public MidsListViewItem? Item { get; }
        public MouseButtons Button { get; }
        public MidsListViewItemClickEventArgs(MidsListViewItem? item, MouseButtons button) { Item = item; Button = button; }
    }


    public sealed partial class MidsListView : UserControl
    {
        #region Private Fields

        private List<MidsListViewItem> _items = [];

        // State Management
        private int _hoveredItemIndex = -1;
        private int _scrollOffset;
        private bool _isDraggingScrollbar;

        // Configuration
        private Color[] _stateColors;
        private int _actualLineHeight;
        private int _lineSpacing = -2;
        private int _designerItemCount = 10;
        private bool _designerHideHeadings;
        private bool _decorateHeadings = true;
        private int _scrollBarWidth = 10;
        private WordwrapMode _textWrapMode = WordwrapMode.New;
        private int _paddingX = 2;
        private int _paddingY;
        private bool _scrollable = true;
        private bool _layoutDirty = true;
        private int _layoutCacheKey = int.MinValue;

        // Rectangles for hit testing
        private Rectangle _textArea;
        private Rectangle _scrollUpButtonRect;
        private Rectangle _scrollDownButtonRect;
        private Rectangle _scrollThumbRect;

#if DEBUG
        private int _debugRecalculateLayoutCount;
#endif

        #endregion

        #region Delegates and Events

        public event EventHandler<MidsListViewItemClickEventArgs>? ItemClicked;
        public event EventHandler<MidsListViewItemHoverEventArgs>? ItemHovered;

        #endregion

        #region Public Properties

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<MidsListViewItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Design")]
        [Description("If true, hides heading items in the designer preview.")]
        [DefaultValue(false)]
        public bool DesignerHideHeadings
        {
            get => _designerHideHeadings;
            set
            {
                _designerHideHeadings = value;
                if (DesignMode)
                {
                    PopulateWithSampleData();
                    MarkLayoutDirty();
                    RecalculateLayout();
                    Invalidate();
                }
            }
        }

        [Category("Design")]
        [Description("Controls how many sample items are shown in the designer view.")]
        [DefaultValue(10)]
        public int DesignerItemCount
        {
            get => _designerItemCount;
            set
            {
                _designerItemCount = value;
                // If we are in the designer, immediately update the preview.
                if (DesignMode)
                {
                    PopulateWithSampleData();
                    MarkLayoutDirty();
                    RecalculateLayout();
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("The background color of the control.")]
        public override Color BackColor { get; set; } = Color.Transparent;

        [Category("Appearance")]
        public Color HoverColor { get; set; } = Color.FromArgb(45, 85, 135);

        [Category("Appearance")]
        public int ScrollBarWidth
        {
            get => _scrollBarWidth;
            set
            {
                int normalized = Math.Max(0, value);
                if (_scrollBarWidth == normalized) return;
                _scrollBarWidth = normalized;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Appearance")]
        public WordwrapMode TextWrapMode
        {
            get => _textWrapMode;
            set
            {
                if (_textWrapMode == value) return;
                _textWrapMode = value;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DecorateHeadings
        {
            get => _decorateHeadings;
            set
            {
                _decorateHeadings = value;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Layout")]
        public int PaddingX
        {
            get => _paddingX;
            set
            {
                int normalized = Math.Max(0, value);
                if (_paddingX == normalized) return;
                _paddingX = normalized;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Layout")]
        public int PaddingY
        {
            get => _paddingY;
            set
            {
                int normalized = Math.Max(0, value);
                if (_paddingY == normalized) return;
                _paddingY = normalized;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Behavior")]
        public bool Scrollable
        {
            get => _scrollable;
            set
            {
                if (_scrollable == value) return;
                _scrollable = value;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        [Category("Layout")]
        [Description("Adds or removes vertical pixels between items. Can be negative to tighten spacing.")]
        public int LineSpacing
        {
            get => _lineSpacing;
            set
            {
                _lineSpacing = value;
                MarkLayoutDirty();
                RecalculateLayout();
                Invalidate();
            }
        }

        // Calculated read-only properties
        [Browsable(false)]
        public int TotalContentHeight
        {
            get
            {
                if (_items.Count == 0) return 0;

                int itemHeight = _items.Sum(item => item.CalculatedHeight);
                int spacingHeight = DpiScale(_lineSpacing) * (_items.Count - 1);
                return Math.Max(0, itemHeight + spacingHeight);
            }
        }

        [Browsable(false)]
        public bool ScrollVisible { get; private set; }

        #endregion

        #region Private Properties

        private ListViewTheme CurrentTheme
        {
            get
            {
                if (DesignMode)
                {
                    return ThemeManager.DesignTime.ListView;
                }
                return ThemeManager.CurrentTheme?.ListView ?? ThemeManager.DesignTime.ListView;
            }
        }

        #endregion

        public MidsListView()
        {
            InitializeComponent();

            // Set optimized drawing styles
            DoubleBuffered = true;
            ResizeRedraw = true; // Automatically invalidates on resize

            // Initialize state colors (matches original logic)
            _stateColors =
            [
                Color.LightBlue, Color.LightGreen, Color.LightGray, Color.DarkGreen, Color.Red, Color.Orange
            ];

            if (!DesignMode) ThemeManager.ThemeChanged += Invalidate;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Ideal height based on all items + top/bottom padding
            int desiredHeight = TotalContentHeight + DpiScale(PaddingY * 2);

            // If scrolling is enabled and the desired height is too large,
            // return only the available height so scrolling kicks in.
            if (Scrollable && proposedSize.Height > 0 && desiredHeight > proposedSize.Height)
            {
                desiredHeight = proposedSize.Height;
            }

            return proposedSize with { Height = desiredHeight };
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
            {
                PopulateWithSampleData();
            }

            MarkLayoutDirty();
            RecalculateLayout();
        }

        public void SetStateColor(MidsItemState state, Color color)
        {
            _stateColors[(int)state] = color;
            Invalidate();
        }

        public void AddItem(MidsListViewItem item)
        {
            _items.Add(item);
            MarkLayoutDirty();
            RecalculateLayout();
            Invalidate();
        }

        public void ClearItems()
        {
            _items.Clear();
            _scrollOffset = 0;
            UpdateHoverTarget(-1, Point.Empty);
            MarkLayoutDirty();
            RecalculateLayout();
            Invalidate();
        }

        internal void ApplyUiMetrics(int scrollBarWidth, int paddingX, int paddingY, int lineSpacing)
        {
            int normalizedScrollBarWidth = Math.Max(0, scrollBarWidth);
            int normalizedPaddingX = Math.Max(0, paddingX);
            int normalizedPaddingY = Math.Max(0, paddingY);

            if (_scrollBarWidth == normalizedScrollBarWidth &&
                _paddingX == normalizedPaddingX &&
                _paddingY == normalizedPaddingY &&
                _lineSpacing == lineSpacing)
            {
                return;
            }

            _scrollBarWidth = normalizedScrollBarWidth;
            _paddingX = normalizedPaddingX;
            _paddingY = normalizedPaddingY;
            _lineSpacing = lineSpacing;
            MarkLayoutDirty();
            RecalculateLayout();
            Invalidate();
        }

        private void MarkLayoutDirty()
        {
            _layoutDirty = true;
        }

        private int DpiScale(int value)
        {
            // The base DPI is 96. DeviceDpi gives the current DPI.
            // This calculates the correct scaling factor for pixel values.
            float scaleFactor = DeviceDpi / 96.0f;
            return (int)(value * scaleFactor);
        }

        private void PopulateWithSampleData()
        {
            var allSampleItems = new List<MidsListViewItem>
            {
                new("Header Item", MidsItemState.Heading, MidsItemFontStyles.Bold, MidsItemAlign.Center),
                new("Enabled", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Disabled Item", MidsItemState.Disabled, MidsItemFontStyles.Bold),
                new("Selected Item", MidsItemState.Selected,
                    MidsItemFontStyles.Bold | MidsItemFontStyles.Italic),
                new("Automatic multiline Item display", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("SD Item", MidsItemState.SelectedDisabled, MidsItemFontStyles.Bold),
                new("Invalid Item", MidsItemState.Invalid, MidsItemFontStyles.Bold),
                new("Scrollable", MidsItemState.Heading, MidsItemFontStyles.Bold, MidsItemAlign.Center),
                new("Item 1", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 2", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 3", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 4", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 5", MidsItemState.Disabled, MidsItemFontStyles.Bold),
                new("Item 6", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 7", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 8", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 9", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 10", MidsItemState.Disabled, MidsItemFontStyles.Bold),
                new("Item 11", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 12", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 13", MidsItemState.Invalid, MidsItemFontStyles.Bold),
                new("Item 14", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 15", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 16", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 17", MidsItemState.Selected, MidsItemFontStyles.Bold),
                new("Item 18", MidsItemState.Invalid, MidsItemFontStyles.Bold),
                new("Item 19", MidsItemState.Enabled, MidsItemFontStyles.Bold),
                new("Item 20", MidsItemState.Enabled, MidsItemFontStyles.Bold)
            };

            // Start with the full list as a queryable source.
            IEnumerable<MidsListViewItem> query = allSampleItems;

            // Apply the heading filter if the property is true.
            if (_designerHideHeadings)
            {
                query = query.Where(item => item.State != MidsItemState.Heading);
            }

            // Apply the count limit and create the final list.
            _items = query.Take(_designerItemCount).ToList();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            MarkLayoutDirty();
            RecalculateLayout();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            MarkLayoutDirty();
            RecalculateLayout();
            Invalidate();
        }

        private void RecalculateLayout()
        {
            if (!IsHandleCreated) return;

            int padX = DpiScale(PaddingX);
            int padY = DpiScale(PaddingY);

            int scaledBarWidth = DpiScale(ScrollBarWidth);
            int scaledLineSpacing = DpiScale(_lineSpacing);
            int fullTextWidth = Math.Max(0, Width - (padX * 2)); 
            int availableHeight = Math.Max(0, Height - padY - padY);

            int layoutKey = ComputeLayoutKey(padX, padY, scaledBarWidth, scaledLineSpacing, fullTextWidth, availableHeight);
            if (!_layoutDirty && layoutKey == _layoutCacheKey)
            {
                return;
            }

            _layoutDirty = false;
            _layoutCacheKey = layoutKey;

#if DEBUG
            _debugRecalculateLayoutCount++;
            Debug.WriteLine($"[MidsListView:{Name}] RecalculateLayout #{_debugRecalculateLayoutCount} width={Width} height={Height} items={_items.Count}");
#endif

            var fontCache = new Dictionary<FontStyle, Font>();
            try
            {
                Font GetOrCreateFont(MidsItemFontStyles itemStyle)
                {
                    var style = (FontStyle)itemStyle;
                    if (!fontCache.TryGetValue(style, out var font))
                    {
                        font = new Font(Font, style);
                        fontCache[style] = font;
                    }

                    return font;
                }

                // ---------- PASS 1: Probe without reserving space for the scrollbar ----------
                // We measure how tall everything would be if there were no scrollbar.
                int totalHeightNoScroll = 0;
                if (_items.Count > 0)
                {
                    foreach (var item in _items)
                    {
                        var font = GetOrCreateFont(item.FontStyle);
                        int contentIndent = GetItemContentIndent(item);
                        int textWidth = Math.Max(1, fullTextWidth - contentIndent);

                        string tempWrapped = WrapText(item.Text, item.State, item.FontStyle, textWidth);

                        Size proposed = new Size(textWidth, int.MaxValue);
                        TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;
                        int measured = TextRenderer.MeasureText(tempWrapped, font, proposed, flags).Height;
                        measured = Math.Max(measured, GetLeadingImageSize(item));

                        totalHeightNoScroll += measured + (padY * 2);
                    }

                    totalHeightNoScroll = Math.Max(0, totalHeightNoScroll + scaledLineSpacing * (_items.Count - 1));
                }

                bool needsScrollbar = Scrollable && totalHeightNoScroll > availableHeight;
                ScrollVisible = needsScrollbar;

                int finalTextWidth = Math.Max(0, fullTextWidth - (needsScrollbar ? scaledBarWidth : 0));
                _textArea = new Rectangle(padX, padY, finalTextWidth, availableHeight);

                if (_items.Count == 0)
                {
                    _actualLineHeight = DpiScale(Font.Height + PaddingY * 2);
                    return;
                }

                int sumHeights = 0;
                foreach (var item in _items)
                {
                    int contentIndent = GetItemContentIndent(item);
                    int textWidth = Math.Max(1, _textArea.Width - contentIndent);

                    item.WrappedText = WrapText(item.Text, item.State, item.FontStyle, textWidth);

                    var font = GetOrCreateFont(item.FontStyle);
                    Size proposed = new Size(textWidth, int.MaxValue);
                    TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;
                    int measured = TextRenderer.MeasureText(item.WrappedText, font, proposed, flags).Height;
                    measured = Math.Max(measured, GetLeadingImageSize(item));

                    item.CalculatedHeight = measured + (padY * 2);
                    sumHeights += item.CalculatedHeight;
                }

                int maxScroll = Math.Max(0, TotalContentHeight - _textArea.Height);
                _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);

                _actualLineHeight = _items.Count > 0
                    ? Math.Max(1, (sumHeights + scaledLineSpacing * (_items.Count - 1)) / _items.Count)
                    : DpiScale(Font.Height + PaddingY * 2);
            }
            finally
            {
                foreach (var font in fontCache.Values)
                {
                    font.Dispose();
                }
            }
        }

        private int ComputeLayoutKey(int padX, int padY, int scaledBarWidth, int scaledLineSpacing, int fullTextWidth, int availableHeight)
        {
            var hash = new HashCode();
            hash.Add(Width);
            hash.Add(Height);
            hash.Add(DeviceDpi);
            hash.Add(padX);
            hash.Add(padY);
            hash.Add(scaledBarWidth);
            hash.Add(scaledLineSpacing);
            hash.Add(fullTextWidth);
            hash.Add(availableHeight);
            hash.Add(_decorateHeadings);
            hash.Add(_textWrapMode);
            hash.Add(_scrollable);
            if (Font != null)
            {
                hash.Add(Font.FontFamily.Name);
                hash.Add(Font.Size);
                hash.Add((int)Font.Style);
            }

            hash.Add(ComputeItemsLayoutSignature());
            return hash.ToHashCode();
        }

        private int ComputeItemsLayoutSignature()
        {
            var hash = new HashCode();
            hash.Add(_items.Count);
            foreach (var item in _items)
            {
                hash.Add(item.Text);
                hash.Add((int)item.State);
                hash.Add((int)item.FontStyle);
                hash.Add((int)item.Alignment);
                if (item.LeadingImage != null)
                {
                    hash.Add(item.LeadingImage.Width);
                    hash.Add(item.LeadingImage.Height);
                }
            }

            return hash.ToHashCode();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsHandleCreated) return;

            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 1. Draw Background
            using (var backBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(backBrush, ClientRectangle);
            }

            // 2. Draw Items
            DrawItems(g);

            // 3. Draw Scrollbar (if needed)
            if (Scrollable && TotalContentHeight > _textArea.Height)
            {
                DrawScrollBar(g);
            }
        }

        private void DrawItems(Graphics g)
        {
            if (_items.Count == 0) return;

            int currentY = -(_scrollOffset);
            var fontCache = new Dictionary<FontStyle, Font>();
            try
            {
                Font GetOrCreateFont(MidsItemFontStyles itemStyle)
                {
                    var style = (FontStyle)itemStyle;
                    if (!fontCache.TryGetValue(style, out var font))
                    {
                        font = new Font(Font, style);
                        fontCache[style] = font;
                    }

                    return font;
                }

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (currentY > Height) break; // Optimization

                    var itemRect = new Rectangle(DpiScale(PaddingX), currentY, _textArea.Width, item.CalculatedHeight);

                    if (itemRect.Bottom > 0)
                    {
                        if (i == _hoveredItemIndex)
                        {
                            using var hoverBrush = new SolidBrush(HoverColor);
                            g.FillRectangle(hoverBrush, itemRect);
                        }

                        TextFormatFlags itemFlags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;
                        switch (item.Alignment)
                        {
                            case MidsItemAlign.Center:
                                itemFlags |= TextFormatFlags.HorizontalCenter;
                                break;
                            case MidsItemAlign.Right:
                                itemFlags |= TextFormatFlags.Right;
                                break;
                            default:
                                itemFlags |= TextFormatFlags.Left;
                                break;
                        }

                        var font = GetOrCreateFont(item.FontStyle);
                        DrawLeadingImage(g, item, itemRect);
                        var textRect = GetTextRect(itemRect, item);

                        TextRenderer.DrawText(
                            g,
                            item.WrappedText,
                            font,
                            textRect,
                            _stateColors[(int)item.State],
                            itemFlags
                        );
                    }
                    currentY += item.CalculatedHeight + DpiScale(_lineSpacing);
                }
            }
            finally
            {
                foreach (var font in fontCache.Values)
                {
                    font.Dispose();
                }
            }
        }

        private int GetLeadingImageSize(MidsListViewItem item)
        {
            return item.LeadingImage == null
                ? 0
                : Math.Max(DpiScale(18), Font.Height + DpiScale(2));
        }

        private int GetLeadingImageGap(MidsListViewItem item)
        {
            return item.LeadingImage == null ? 0 : DpiScale(6);
        }

        private int GetItemContentIndent(MidsListViewItem item)
        {
            return GetLeadingImageSize(item) + GetLeadingImageGap(item);
        }

        private Rectangle GetLeadingImageBounds(Rectangle itemRect, MidsListViewItem item)
        {
            int iconSize = GetLeadingImageSize(item);
            if (iconSize <= 0)
            {
                return Rectangle.Empty;
            }

            int iconX = itemRect.Left;
            int iconY = itemRect.Top + Math.Max(0, (itemRect.Height - iconSize) / 2);
            return new Rectangle(iconX, iconY, iconSize, iconSize);
        }

        private Rectangle GetTextRect(Rectangle itemRect, MidsListViewItem item)
        {
            int indent = GetItemContentIndent(item);
            return new Rectangle(
                itemRect.Left + indent,
                itemRect.Top,
                Math.Max(0, itemRect.Width - indent),
                itemRect.Height);
        }

        private void DrawLeadingImage(Graphics g, MidsListViewItem item, Rectangle itemRect)
        {
            if (item.LeadingImage == null)
            {
                return;
            }

            var imageBounds = GetLeadingImageBounds(itemRect, item);
            if (imageBounds == Rectangle.Empty)
            {
                return;
            }

            g.DrawImage(item.LeadingImage, imageBounds);
        }

        private void DrawScrollBar(Graphics g)
        {
            int scaledBarWidth = DpiScale(ScrollBarWidth);
            int scrollBarX = Width - scaledBarWidth;
            int centerX = scrollBarX + scaledBarWidth / 2;

            // A proportional margin to make the arrows scale with the scrollbar's width.
            // You can adjust this value (e.g., from 0.3f to 0.25f) to fine-tune the arrow size.
            float margin = scaledBarWidth * 0.08f;

            // --- Draw Up Arrow Button ---
            _scrollUpButtonRect = new Rectangle(scrollBarX, 0, scaledBarWidth, scaledBarWidth);
            using (var brush = new SolidBrush(CurrentTheme.ScrollButton))
            {
                var upTriangle = new PointF[]
                {
                    new(_scrollUpButtonRect.Left + scaledBarWidth / 2f, _scrollUpButtonRect.Top + margin),
                    new(_scrollUpButtonRect.Left + margin, _scrollUpButtonRect.Bottom - margin),
                    new(_scrollUpButtonRect.Right - margin, _scrollUpButtonRect.Bottom - margin)
                };
                g.FillPolygon(brush, upTriangle);
            }

            // --- Draw Down Arrow Button ---
            _scrollDownButtonRect = new Rectangle(scrollBarX, Height - scaledBarWidth, scaledBarWidth, scaledBarWidth);
            using (var brush = new SolidBrush(CurrentTheme.ScrollButton))
            {
                var downTriangle = new PointF[]
                {
                    new(_scrollDownButtonRect.Left + scaledBarWidth / 2f, _scrollDownButtonRect.Bottom - margin),
                    new(_scrollDownButtonRect.Left + margin, _scrollDownButtonRect.Top + margin),
                    new(_scrollDownButtonRect.Right - margin, _scrollDownButtonRect.Top + margin)
                };
                g.FillPolygon(brush, downTriangle);
            }

            // --- Draw Track Rail ---
            int trackTop = _scrollUpButtonRect.Bottom + DpiScale(4);
            int trackBottom = _scrollDownButtonRect.Top - DpiScale(4);
            using (var pen = new Pen(CurrentTheme.ScrollBar, DpiScale(2)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, centerX, trackTop, centerX, trackBottom);
            }

            // --- Draw Thumb ---
            int trackHeight = trackBottom - trackTop;
            if (TotalContentHeight <= _textArea.Height || trackHeight <= 0) return;

            float contentRatio = (float)_textArea.Height / TotalContentHeight;
            int thumbHeight = Math.Max(DpiScale(10), (int)(trackHeight * contentRatio));

            int scrollablePixels = TotalContentHeight - _textArea.Height;
            float scrollPercent = scrollablePixels > 0 ? (float)_scrollOffset / scrollablePixels : 0;
            int thumbY = trackTop + (int)((trackHeight - thumbHeight) * scrollPercent);

            _scrollThumbRect = new Rectangle(scrollBarX, thumbY, scaledBarWidth, thumbHeight);
            using (var thumbBrush = new SolidBrush(CurrentTheme.ScrollButton))
            {
                g.FillRectangle(thumbBrush, _scrollThumbRect);
            }
        }

        private string WrapText(string originalText, MidsItemState state, MidsItemFontStyles style, int maxWidth)
        {
            if (string.IsNullOrEmpty(originalText) || maxWidth <= 0) return "";

            bool isHeading = state == MidsItemState.Heading;
            bool decorateHeading = isHeading && DecorateHeadings;

            // For headings, the text is wrapped in tildes, which must be part of the calculation.
            if (decorateHeading)
            {
                originalText = $"~ {originalText.Trim()} ~";
            }

            using var font = new Font(Font, (FontStyle)style);
            // For text that won't be wrapped (or can't be), return it as is.
            // The drawing method will handle trimming with an ellipsis if needed.
            if (TextWrapMode == WordwrapMode.UseEllipsis || !originalText.Contains(' '))
            {
                return originalText;
            }

            var lines = new List<string>();
            var currentLine = "";
            string[] words = originalText.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            TextFormatFlags flags = TextFormatFlags.NoPadding;

            foreach (var word in words)
            {
                // Test if the next word fits on the current line
                var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                var width = TextRenderer.MeasureText(testLine, font, new Size(int.MaxValue, int.MaxValue), flags).Width;

                if (width > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    // The line is full. Add it to our list and start a new line.
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    // The word fits. Add it to the current line.
                    currentLine = testLine;
                }
            }
            // Add the last line to the list
            lines.Add(currentLine);

            // Join the wrapped lines with a newline character.
            return string.Join("\n", lines);
        }

        private int GetItemIndexAt(Point location)
        {
            if (!_textArea.Contains(location)) return -1;

            int currentY = -_scrollOffset;
            for (int i = 0; i < _items.Count; i++)
            {
                var itemRect = new Rectangle(_textArea.X, currentY, _textArea.Width, _items[i].CalculatedHeight);
                if (itemRect.Contains(location))
                {
                    return i;
                }
                currentY += _items[i].CalculatedHeight + DpiScale(_lineSpacing);
            }
            return -1;
        }

        private Rectangle GetItemBounds(int index)
        {
            if (index < 0 || index >= _items.Count) return Rectangle.Empty;

            int y = -_scrollOffset;
            for (int i = 0; i < index; i++)
                y += _items[i].CalculatedHeight + DpiScale(_lineSpacing);

            return new Rectangle(_textArea.X, y, _textArea.Width, _items[index].CalculatedHeight);
        }

        private void UpdateHoverTarget(int newIndex, Point location)
        {
            if (newIndex == _hoveredItemIndex) return;

            _hoveredItemIndex = newIndex;

            var item = (newIndex >= 0 && newIndex < _items.Count) ? _items[newIndex] : null;
            var bounds = GetItemBounds(newIndex);

            ItemHovered?.Invoke(this, new MidsListViewItemHoverEventArgs(item, location, bounds, newIndex));
            Invalidate(); // update highlight
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDraggingScrollbar)
            {
                Cursor = Cursors.SizeNS;

                // Use the text area's height for an accurate scroll range.
                int scrollableHeight = TotalContentHeight - _textArea.Height;
                if (scrollableHeight <= 0)
                {
                    // If content isn't scrollable, stop dragging.
                    _isDraggingScrollbar = false;
                    return;
                }

                // The draggable area of the track.
                int trackHeight = _textArea.Height - _scrollThumbRect.Height;
                if (trackHeight <= 0) return;

                // Calculate the new scroll offset based on the mouse's vertical position.
                float dragRatio = (float)(e.Y - _scrollThumbRect.Height / 2) / trackHeight;
                _scrollOffset = (int)(Math.Clamp(dragRatio, 0f, 1f) * scrollableHeight);

                Invalidate();
                return;
            }

            // Treat entire scrollbar column as non-item area
            int scaledBarWidth = DpiScale(ScrollBarWidth);
            var scrollBarRect = new Rectangle(Width - scaledBarWidth, 0, scaledBarWidth, Height);

            bool overScrollUi =
                Scrollable && TotalContentHeight > _textArea.Height &&
                (_scrollUpButtonRect.Contains(e.Location)
                 || _scrollDownButtonRect.Contains(e.Location)
                 || _scrollThumbRect.Contains(e.Location)
                 || scrollBarRect.Contains(e.Location));

            if (overScrollUi)
            {
                Cursor = Cursors.Hand;
                UpdateHoverTarget(-1, e.Location);
            }
            else
            {
                Cursor = Cursors.Default;
                UpdateHoverTarget(GetItemIndexAt(e.Location), e.Location);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // --- Handle new Arrow Button clicks ---
            if (_scrollUpButtonRect.Contains(e.Location) && Scrollable)
            {
                _scrollOffset = Math.Max(0, _scrollOffset - _actualLineHeight);
                Invalidate();
                return;
            }

            if (_scrollDownButtonRect.Contains(e.Location) && Scrollable)
            {
                int maxScroll = Math.Max(0, TotalContentHeight - _textArea.Height);
                _scrollOffset = Math.Min(maxScroll, _scrollOffset + _actualLineHeight);
                Invalidate();
                return;
            }

            if (_scrollThumbRect.Contains(e.Location) && Scrollable)
            {
                _isDraggingScrollbar = true;
                UpdateHoverTarget(-1, e.Location);
                return;
            }

            int clickedIndex = GetItemIndexAt(e.Location);
            if (clickedIndex != -1)
            {
                ItemClicked?.Invoke(this, new MidsListViewItemClickEventArgs(_items[clickedIndex], e.Button));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDraggingScrollbar = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateHoverTarget(-1, Point.Empty);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!Scrollable || TotalContentHeight <= _textArea.Height) return;

            int scrollAmount = _actualLineHeight * SystemInformation.MouseWheelScrollLines;

            if (e.Delta < 0)
            {
                _scrollOffset += scrollAmount;
            }
            else
            {
                _scrollOffset -= scrollAmount;
            }

            int maxScroll = Math.Max(0, TotalContentHeight - _textArea.Height);
            _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);

            Invalidate();
        }
    }
}
