using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mids_Reborn.Core;

namespace Mids_Reborn.UI.Controls
{
    public sealed partial class PairedListEx : UserControl
    {
        #region Appearance

        [Category("Appearance")]
        public Color ItemColor { get; set; } = Color.Silver;

        [Category("Appearance")]
        public Color ValueColor { get; set; } = Color.WhiteSmoke;

        [Category("Appearance")]
        public Color ValueAlternateColor { get; set; } = Color.Chartreuse;

        [Category("Appearance")]
        public Color ValueConditionColor { get; set; } = Color.Firebrick;

        [Category("Appearance")]
        public Color ValueSpecialColor { get; set; } = Color.SlateBlue;

        [Category("Appearance")]
        public bool UseHighlighting { get; set; }

        [Category("Appearance")]
        public Color HighlightColor { get; set; } = Color.CornflowerBlue;

        [Category("Appearance")]
        public Color HighlightTextColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public bool SetItemsBold
        {
            get => _setItemsBold;
            set { _setItemsBold = value; Invalidate(); }
        }

        #endregion

        #region Layout (DPI-aware)

        [Category("Layout"), DefaultValue(2)]
        public int Columns
        {
            get => _columns;
            set
            {
                int v = Math.Max(1, value);
                if (_columns == v) return;
                _columns = v;
                EnsureAlignmentCapacity();
                RegenerateSamples();
                Invalidate();
            }
        }

        [Category("Layout"), DefaultValue(true)]
        [Description("If true, row height is computed from text; otherwise fixed by FixedRowHeight.")]
        public bool AutoSizeLineHeight
        {
            get => _autoSizeLineHeight;
            set { _autoSizeLineHeight = value; Invalidate(); }
        }

        [Category("Layout"), DefaultValue(22)]
        [Description("Fixed row height in logical (96-DPI) pixels when AutoSizeLineHeight=false.")]
        public int FixedRowHeight
        {
            get => _fixedRowHeightLogical;
            set { _fixedRowHeightLogical = Math.Max(1, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(6)]
        public int RowSpacing
        {
            get => _rowSpacingLogical;
            set { _rowSpacingLogical = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(6)]
        public int PaddingH
        {
            get => _paddingHLogical;
            set { _paddingHLogical = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(6)]
        public int PaddingV
        {
            get => _paddingVLogical;
            set { _paddingVLogical = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(4)]
        [Description("Horizontal gap between label and value when inline.")]
        public int LabelValueGap
        {
            get => _labelValueGapLogical;
            set { _labelValueGapLogical = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(2)]
        [Description("Vertical spacing before a wrapped value under its label.")]
        public int ValueBelowSpacing
        {
            get => _valueBelowSpacingLogical;
            set { _valueBelowSpacingLogical = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(true)]
        [Description("When the value wraps (and alignment is Left), indent wrapped lines to just past the first label character.")]
        public bool WrapIndent
        {
            get => _wrapIndent;
            set { _wrapIndent = value; Invalidate(); }
        }

        #endregion

        #region Samples

        [Category("Samples"), DefaultValue(true)]
        [Description("Show curated dummy items at design-time when the list is empty.")]
        public bool ShowDesignTimeSamples
        {
            get => _showDesignTimeSamples;
            set { if (_showDesignTimeSamples != value) { _showDesignTimeSamples = value; RegenerateSamples(); Invalidate(); } }
        }

        [Category("Samples"), DefaultValue(false)]
        [Description("Show curated dummy items at runtime when the list is empty.")]
        public bool ShowRuntimeSamples
        {
            get => _showRuntimeSamples;
            set { if (_showRuntimeSamples != value) { _showRuntimeSamples = value; RegenerateSamples(); Invalidate(); } }
        }

        [Category("Samples"), DefaultValue(4)]
        [Description("Max number of demo rows per column when samples are shown.")]
        public int SampleRowsPerColumn
        {
            get => _sampleRowsPerColumn;
            set { int v = Math.Max(1, value); if (_sampleRowsPerColumn != v) { _sampleRowsPerColumn = v; RegenerateSamples(); Invalidate(); } }
        }

        #endregion

        #region Per-column alignment

        private readonly List<HorizontalAlignment> _labelAlignments = new();
        private readonly List<HorizontalAlignment> _valueAlignments = new();

        [Browsable(false)]
        public HorizontalAlignment GetLabelAlignment(int column)
            => (column >= 0 && column < _labelAlignments.Count) ? _labelAlignments[column] : HorizontalAlignment.Left;

        [Browsable(false)]
        public HorizontalAlignment GetValueAlignment(int column)
            => (column >= 0 && column < _valueAlignments.Count) ? _valueAlignments[column] : HorizontalAlignment.Left;

        public void SetColumnAlignment(int column, HorizontalAlignment labelAlign, HorizontalAlignment valueAlign)
        {
            if (column < 0) return;
            while (_labelAlignments.Count <= column) _labelAlignments.Add(HorizontalAlignment.Left);
            while (_valueAlignments.Count <= column) _valueAlignments.Add(HorizontalAlignment.Left);
            _labelAlignments[column] = labelAlign;
            _valueAlignments[column] = valueAlign;
            Invalidate();
        }

        private void EnsureAlignmentCapacity()
        {
            while (_labelAlignments.Count < _columns) _labelAlignments.Add(HorizontalAlignment.Left);
            while (_valueAlignments.Count < _columns) _valueAlignments.Add(HorizontalAlignment.Left);
        }

        #endregion

        #region Backing fields

        private readonly List<Item> _items = new();
        private List<Item> _samples = new();

        private int _hoverIndex = -1;

        private int _columns = 2;
        private bool _setItemsBold;
        private bool _autoSizeLineHeight = true;

        private int _fixedRowHeightLogical = 22;
        private int _rowSpacingLogical = 6;
        private int _paddingHLogical = 6;
        private int _paddingVLogical = 6;
        private int _labelValueGapLogical = 4;
        private int _valueBelowSpacingLogical = 2;
        private bool _wrapIndent = true;

        private bool _showDesignTimeSamples = true;
        private bool _showRuntimeSamples;
        private int _sampleRowsPerColumn = 4;

        private float DpiScale => DeviceDpi / 96f;
        private int ScalePx(int logical) => (int)Math.Round(logical * DpiScale);

        #endregion

        #region Public API

        public int ItemCount => _items.Count;

        public void AddItem(Item? item)
        {
            if (item == null) return;
            _items.Add(item);
            Invalidate();
        }

        public void Clear(bool redraw = false)
        {
            _items.Clear();
            if (redraw) Invalidate();
        }

        public void Redraw() => Invalidate();

        public void SetUnique()
        {
            if (_items.Count > 0) _items[^1].UseUniqueColor = true;
        }

        public bool IsSpecialColor() => _items.Count > 0 && _items[^1].UseSpecialColor;

        #endregion

        #region Construction & samples

        public PairedListEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.FromArgb(45, 45, 48);

            FontChanged += (_, __) => Invalidate();
            ForeColorChanged += (_, __) => Invalidate();
            Resize += (_, __) => Invalidate();

            MouseMove += OnMouseMoveInternal;
            MouseLeave += OnMouseLeaveInternal;
            MouseClick += OnMouseClickInternal;

            EnsureAlignmentCapacity();
            RegenerateSamples();

            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegenerateSamples();
        }

        private void RegenerateSamples()
        {
            var list = new List<Item>();

            // How many sample rows we need to fully populate the grid
            int perCol = Math.Max(1, _sampleRowsPerColumn);
            int columns = Math.Max(1, _columns);
            int max = perCol * columns;

            // Larger pool of sample names so raising perCol still looks varied
            string[] names =
            {
        "Accuracy", "Recharge", "Damage", "Endurance Disc.", "Defense (All)", "Resist (S/L)",
        "Knockback Prot", "To-Hit", "Defense (Melee)", "Defense (Ranged)", "Defense (AoE)",
        "Resist (Fire)", "Resist (Cold)", "Resist (Energy)", "Resist (Negative)", "Resist (Toxic)", "Psionic Res.",
        "Recovery", "Endurance", "Heal", "Absorb", "Range", "Global Chance", "Global Recharge",
        "Speed Running", "Speed Flying", "Speed Jumping", "Jump Height", "Perception", "Taunt", "Placate",
        "Max Run", "Max Jump", "Max Fly", "Resist (All)"
    };

            // Short values to rotate through, plus an occasional long one to demo wrapping
            string longVal = " 10000% (Self)";
            string[] shortVals = { "+25%", "+33%", "+12.5%", "+15%", "+18.75%", "+22.0%", "+10", "+7.5%", "+5%", "+45%" };

            // Helper to add an item
            void AddPair(string name, string value, bool alt = false, bool special = false, bool unique = false, string tip = "")
                => list.Add(new Item(name, value, alt, special, unique, tip));

            // Always produce at least the exact number of rows the layout expects
            for (int i = 0; i < max; i++)
            {
                string n = names[i % names.Length];
                string val = (i % 6 == 3) ? longVal : shortVals[i % shortVals.Length];

                bool alt = (i % 2) == 1;   // zebra
                bool special = (i % 5) == 2;   // occasional “special” color demo
                bool unique = (i % 9) == 0;   // occasional “unique” color demo

                string tip = $"{n} sample #{i + 1}";

                AddPair(n, val, alt, special, unique, tip);
            }

            // If someone asks for fewer than our pool produced, trim down (keeps behavior deterministic)
            if (list.Count > max)
                list = list.Take(max).ToList();

            _samples = list;
            Invalidate(); // make changes visible immediately at design-time/runtime
        }

        #endregion

        #region Measure / AutoSize

        public override Size GetPreferredSize(Size proposedSize)
        {
            var source = EffectiveItems();
            if (source.Count == 0) return base.GetPreferredSize(proposedSize);

            int width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(1, Width);
            int height = MeasureHeightForWidth(width, source);
            return new Size(width, height);
        }

        private int MeasureHeightForWidth(int totalWidth, List<Item> source)
        {
            using var bmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(bmp);
            using var baseFont = SetItemsBold ? new Font(Font, FontStyle.Bold) : new Font(Font, FontStyle.Regular);

            int padH = ScalePx(_paddingHLogical);
            int padV = ScalePx(_paddingVLogical);
            int rowGap = ScalePx(_rowSpacingLogical);

            int innerWidth = Math.Max(1, totalWidth - padH * 2);
            int columns = Math.Max(1, _columns);
            int colWidth = Math.Max(1, innerWidth / columns);

            // each column advances independently
            int[] yPerCol = new int[columns];
            for (int c = 0; c < columns; c++) yPerCol[c] = padV;

            for (int i = 0; i < source.Count; i++)
            {
                int c = i % columns;
                int h = MeasureItemHeight(g, baseFont, source[i], colWidth, GetLabelAlignment(c), GetValueAlignment(c));
                if (!_autoSizeLineHeight) h = Math.Max(h, ScalePx(_fixedRowHeightLogical));
                yPerCol[c] += h + rowGap;
            }

            int maxY = yPerCol[0];
            for (int c = 1; c < columns; c++) if (yPerCol[c] > maxY) maxY = yPerCol[c];

            int height = maxY + padV - rowGap;
            return Math.Max(height, padV * 2 + 1);
        }

        private int MeasureItemHeight(Graphics g, Font font, Item it, int colWidth,
                              HorizontalAlignment labelAlign, HorizontalAlignment valueAlign)
        {
            string name = (it.Name ?? string.Empty).Trim();
            if (name.Length > 0 && !name.EndsWith(":")) name += ":";

            string value = (it.Value ?? string.Empty).Trim();

            Size nameSz = TextRenderer.MeasureText(g, string.IsNullOrEmpty(name) ? " " : name, font,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

            int hvGap = ScalePx(_labelValueGapLogical);
            int belowGap = ScalePx(_valueBelowSpacingLogical);

            if (string.IsNullOrEmpty(value))
                return nameSz.Height;

            if (valueAlign == HorizontalAlignment.Left)
            {
                // Space available to the RIGHT of the label (inline)
                int labelX = AlignedX(colWidth, nameSz.Width, labelAlign);
                int inlineAvail = Math.Max(0, colWidth - (labelX + nameSz.Width + hvGap));

                // Whole value fits inline?
                Size valueSingle = TextRenderer.MeasureText(g, value, font,
                    new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                if (valueSingle.Width <= inlineAvail)
                    return Math.Max(nameSz.Height, valueSingle.Height);

                // Keep the largest whole-word prefix inline; wrap remainder under VALUE start
                var words = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstLine = string.Empty, probe = string.Empty;
                for (int w = 0; w < words.Length; w++)
                {
                    string next = string.IsNullOrEmpty(probe) ? words[w] : probe + " " + words[w];
                    Size nextSz = TextRenderer.MeasureText(g, next, font,
                        new Size(int.MaxValue, int.MaxValue),
                        TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                    if (nextSz.Width <= inlineAvail) { probe = next; firstLine = probe; }
                    else break;
                }

                // Remainder = substring AFTER the inline prefix (found by index, not length alone)
                string remainder;
                if (!string.IsNullOrEmpty(firstLine))
                {
                    int start = value.IndexOf(firstLine, StringComparison.Ordinal);
                    remainder = (start >= 0) ? value.Substring(start + firstLine.Length).TrimStart() : value;
                }
                else remainder = value;

                // Wrap rect begins at VALUE start (label + gap), in column-local coords
                int indentLocal = Math.Min(colWidth - 1, labelX + nameSz.Width + hvGap + ScalePx(8));
                int wrapWidth = Math.Max(1, colWidth - indentLocal);

                if (string.IsNullOrEmpty(remainder))
                    return nameSz.Height; // everything fit into firstLine

                Size remSz = TextRenderer.MeasureText(g, remainder, font,
                    new Size(wrapWidth, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.WordBreak);

                return nameSz.Height + belowGap + remSz.Height;
            }
            else
            {
                // Center/Right: wrap inside the value area to the right of the label
                int labelX = AlignedX(colWidth, nameSz.Width, labelAlign);
                int vx = labelX + nameSz.Width + hvGap;
                int vwidth = Math.Max(1, colWidth - vx);

                Size valueMultiline = TextRenderer.MeasureText(g, value, font,
                    new Size(vwidth, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.WordBreak);

                return Math.Max(nameSz.Height, valueMultiline.Height);
            }
        }

        private static int AlignedX(int containerWidth, int textWidth, HorizontalAlignment align)
        {
            return align switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => (containerWidth - textWidth) / 2,
                HorizontalAlignment.Right => containerWidth - textWidth,
                _ => 0
            };
        }

        private static int MeasureFirstCharWidth(Graphics g, Font font, string name)
        {
            // If we have a label, measure its first glyph; else use a representative wide glyph.
            string s = string.IsNullOrEmpty(name) ? "W" : name.Substring(0, 1);
            Size sz = TextRenderer.MeasureText(g, s, font,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            return Math.Max(1, sz.Width);
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            g.Clear(BackColor);
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var source = EffectiveItems();
            if (source.Count == 0) return;

            using var baseFont = SetItemsBold ? new Font(Font, FontStyle.Bold) : new Font(Font, FontStyle.Regular);

            int padH = ScalePx(_paddingHLogical);
            int padV = ScalePx(_paddingVLogical);
            int rowGap = ScalePx(_rowSpacingLogical);

            int innerLeft = padH;
            int innerTop = padV;
            int innerWidth = Math.Max(1, ClientSize.Width - padH * 2);
            int columns = Math.Max(1, Columns);
            int colWidth = Math.Max(1, innerWidth / columns);

            // per-column running Y
            int[] yPerCol = new int[columns];
            for (int c = 0; c < columns; c++) yPerCol[c] = innerTop;

            for (int i = 0; i < source.Count; i++)
            {
                int c = i % columns;
                int colX = innerLeft + c * colWidth;

                int h = MeasureItemHeight(g, baseFont, source[i], colWidth, GetLabelAlignment(c), GetValueAlignment(c));
                if (!_autoSizeLineHeight) h = Math.Max(h, ScalePx(_fixedRowHeightLogical));

                var colRect = new Rectangle(colX, yPerCol[c], colWidth, h);

                Region? oldClip = g.Clip?.Clone();
                g.SetClip(colRect);

                DrawItem(g, baseFont, source[i], colRect, GetLabelAlignment(c), GetValueAlignment(c));
                source[i].SetBounds(colRect);

                g.SetClip(oldClip, CombineMode.Replace);
                oldClip?.Dispose();

                yPerCol[c] += h + rowGap;
            }
        }

        private static TextFormatFlags FlagsFor(HorizontalAlignment align, bool wordBreak = false)
        {
            var flags = TextFormatFlags.NoPadding;
            if (wordBreak) flags |= TextFormatFlags.WordBreak;
            flags |= align switch
            {
                HorizontalAlignment.Right => TextFormatFlags.Right,
                HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
                _ => TextFormatFlags.Left
            };
            return flags;
        }

        private void DrawItem(Graphics g, Font font, Item it, Rectangle colRect,
                      HorizontalAlignment labelAlign, HorizontalAlignment valueAlign)
        {
            string name = (it.Name ?? string.Empty).Trim();
            if (name.Length > 0 && !name.EndsWith(":")) name += ":";

            string value = (it.Value ?? string.Empty).Trim();

            bool highlighted = UseHighlighting && it.IsHighlightable;
            if (highlighted)
            {
                using var hl = new SolidBrush(HighlightColor);
                g.FillRectangle(hl, colRect);
            }

            Color labelColor = highlighted ? HighlightTextColor : ItemColor;
            Color valueColor =
                highlighted ? HighlightTextColor :
                it.UseAlternateColor ? ValueAlternateColor :
                it.UseSpecialColor ? ValueSpecialColor :
                it.UseUniqueColor ? ValueConditionColor :
                ValueColor;

            int hvGap = ScalePx(_labelValueGapLogical);
            int belowGap = ScalePx(_valueBelowSpacingLogical);

            // Label (single line), column-local X for alignment
            Size nameSz = TextRenderer.MeasureText(g, string.IsNullOrEmpty(name) ? " " : name, font,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
            int localLabelX = AlignedX(colRect.Width, nameSz.Width, labelAlign);
            var labelPt = new Point(colRect.X + localLabelX, colRect.Y);

            TextRenderer.DrawText(g, name, font, labelPt, labelColor,
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

            if (string.IsNullOrEmpty(value)) return;

            if (valueAlign == HorizontalAlignment.Left)
            {
                // Space available inline (to the right of label)
                int inlineAvail = Math.Max(0, colRect.Width - (localLabelX + nameSz.Width + hvGap));
                Size valueSingle = TextRenderer.MeasureText(g, value, font,
                    new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

                int valueStartXAbs = labelPt.X + nameSz.Width + hvGap;            // absolute
                int valueStartXRel = localLabelX + nameSz.Width + hvGap + ScalePx(8);          // column-local

                if (valueSingle.Width <= inlineAvail)
                {
                    TextRenderer.DrawText(g, value, font,
                        new Point(valueStartXAbs, colRect.Y), valueColor,
                        TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                }
                else
                {
                    // Largest whole-word prefix inline
                    var words = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string firstLine = string.Empty, probe = string.Empty;
                    for (int w = 0; w < words.Length; w++)
                    {
                        string next = string.IsNullOrEmpty(probe) ? words[w] : probe + " " + words[w];
                        Size nextSz = TextRenderer.MeasureText(g, next, font,
                            new Size(int.MaxValue, int.MaxValue),
                            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                        if (nextSz.Width <= inlineAvail) { probe = next; firstLine = probe; }
                        else break;
                    }

                    if (!string.IsNullOrEmpty(firstLine))
                    {
                        TextRenderer.DrawText(g, firstLine, font,
                            new Point(valueStartXAbs, colRect.Y), valueColor,
                            TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                    }

                    // Remainder computed by index so we don't cut a character on spacing
                    string remainder;
                    if (!string.IsNullOrEmpty(firstLine))
                    {
                        int start = value.IndexOf(firstLine, StringComparison.Ordinal);
                        remainder = (start >= 0) ? value.Substring(start + firstLine.Length).TrimStart() : value;
                    }
                    else remainder = value;

                    if (!string.IsNullOrEmpty(remainder))
                    {
                        // WRAP RECT: column-local indent (value start relative to colRect)
                        int indentLocal = Math.Min(colRect.Width - 1, valueStartXRel);
                        var wrappedRect = new Rectangle(
                            colRect.X + indentLocal,
                            colRect.Y + nameSz.Height + belowGap,
                            Math.Max(1, colRect.Width - indentLocal),
                            Math.Max(1, colRect.Height - nameSz.Height - belowGap));

                        TextRenderer.DrawText(g, remainder, font, wrappedRect, valueColor,
                            FlagsFor(HorizontalAlignment.Left, wordBreak: true));
                    }
                }
            }
            else
            {
                // Center/Right: wrap inside the value area to the right of the label
                int vx = labelPt.X + nameSz.Width + hvGap;
                var valueRect = new Rectangle(
                    vx,
                    colRect.Y,
                    Math.Max(1, colRect.Right - vx),
                    colRect.Height);

                TextRenderer.DrawText(g, value, font, valueRect, valueColor,
                    FlagsFor(valueAlign, wordBreak: true));
            }
        }

        private List<Item> EffectiveItems()
        {
            if (_items.Count > 0) return _items;
            bool wantSamples = (DesignMode && _showDesignTimeSamples) || (!DesignMode && _showRuntimeSamples);
            return wantSamples ? _samples : _empty;
        }

        private static readonly List<Item> _empty = new();

        #endregion

        #region Mouse / Events

        private void OnMouseClickInternal(object? sender, MouseEventArgs e)
        {
            var source = EffectiveItems();
            if (source.Count == 0) return;

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Bounds.Contains(e.Location))
                {
                    ItemClick?.Invoke(this, source[i], e);
                    return;
                }
            }
        }

        private void OnMouseMoveInternal(object? sender, MouseEventArgs e)
        {
            var source = EffectiveItems();
            if (source.Count == 0) return;

            int newHover = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Bounds.Contains(e.Location))
                {
                    newHover = i;
                    break;
                }
            }

            if (newHover == _hoverIndex) return;

            if (UseHighlighting)
            {
                for (int i = 0; i < source.Count; i++)
                    source[i].IsHighlightable = (i == newHover);
            }

            _hoverIndex = newHover;
            Invalidate();

            if (_hoverIndex == -1)
                ItemOut?.Invoke(this);
            else
            {
                var it = source[_hoverIndex];
                if (!string.IsNullOrEmpty(it.ToolTip))
                    ItemHover?.Invoke(this, _hoverIndex, it.TagId, it.ToolTip);
            }
        }

        private void OnMouseLeaveInternal(object? sender, EventArgs e)
        {
            var source = EffectiveItems();
            if (!UseHighlighting)
            {
                if (_hoverIndex != -1) { _hoverIndex = -1; Invalidate(); }
                ItemOut?.Invoke(this);
                return;
            }

            bool changed = false;
            foreach (var it in source)
            {
                if (it.IsHighlightable) { it.IsHighlightable = false; changed = true; }
            }

            _hoverIndex = -1;
            if (changed) Invalidate();
            ItemOut?.Invoke(this);
        }

        public delegate void ItemClickEventHandler(object? sender, Item item, MouseEventArgs e);
        public delegate void ItemHoverEventHandler(object? sender, int index, Enums.ShortFX tagId, string? tooltip = "");
        public delegate void ItemOutEventHandler(object? sender);

        public event ItemClickEventHandler? ItemClick;
        public event ItemHoverEventHandler? ItemHover;
        public event ItemOutEventHandler? ItemOut;

        #endregion

        #region Item model

        public sealed class Item
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
            public Enums.ShortFX TagId { get; init; } = default!;
            public SummonedEntity? EntTag { get; set; }
            public string? ToolTip { get; set; }

            public bool UseAlternateColor { get; set; }
            public bool UseSpecialColor { get; set; }
            public bool UseUniqueColor { get; set; }

            public bool IsHighlightable { get; set; }

            public Rectangle Bounds { get; private set; }
            public void SetBounds(Rectangle rect) => Bounds = rect;

            public Item() { }
            public Item(string name, string value) { Name = name; Value = value; }
            public Item(string name, string value, bool alternate)
            {
                Name = name; Value = value; UseAlternateColor = alternate;
            }
            public Item(string name, string value, bool alternate, bool isSpecial, bool isUnique, string tip)
            {
                Name = name; Value = value;
                TagId.Add(-1, 0f);
                UseAlternateColor = alternate;
                UseSpecialColor = isSpecial;
                UseUniqueColor = isUnique;
                ToolTip = tip;
            }
            public Item(string name, string value, bool useAlternate, bool useSpecial, bool useUnique, string tip, SummonedEntity entTag)
            {
                Name = name;
                Value = value;
                TagId.Add(-1, 0f);
                ToolTip = tip;
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
                EntTag = entTag;
            }
            public Item(string name, string value, bool useAlternate = false, bool useSpecial = false, bool useUnique = false, int idValue = -1)
            {
                Name = name;
                Value = value;
                TagId.Add(idValue, 0f);
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
            }
            public Item(string name, string value, bool useAlternate, bool useSpecial, bool useUnique, Enums.ShortFX fxId)
            {
                Name = name;
                Value = value;
                TagId.Assign(fxId);
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
            }
            public Item(Item other)
            {
                Name = other.Name; Value = other.Value; ToolTip = other.ToolTip;
                TagId.Assign(other.TagId);
                UseAlternateColor = other.UseAlternateColor;
                UseSpecialColor = other.UseSpecialColor;
                UseUniqueColor = other.UseUniqueColor;
            }
        }

        #endregion
    }
}
