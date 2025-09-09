using System.ComponentModel;

namespace Mids_Reborn.UI.Controls.Test
{
    [ToolboxItem(true)]
    public sealed class MidsTotalsGraph : Control
    {
        #region Constants
        private const TextFormatFlags LabelFlags =
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;

        private const TextFormatFlags ValueFlags =
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;

        // Logical pixels; scaled via ScalePx for DPI
        private const int MinBarWidthLogical = 40;
        #endregion

        #region Backing fields
        private readonly List<GraphItem> _items = new();
        private readonly List<Rectangle> _rowHitRects = new();
        private readonly ToolTip _tip = new();
        private int _hoverIndex = -1;

        // Layout
        private int _columns = 2;          // how many columns to render
        private int _rowsPerColumn = 5;    // default rows per column
        private bool _autoHeight;  // let the container size us

        // Geometry (logical px)
        private int _xPad = 4;
        private int _yPad = 4;
        private int _itemHeight = 14;
        private int _labelWidth = 88;
        private int _valueWidthMax = 48;   // cap; actual width is measured <= this

        // Appearance
        private bool _showValueText = true;
        private bool _showRowSeparators;
        private bool _showBorders = true;
        private bool _dualLabel;
        private SecondaryTextAlign _secondaryAlign = SecondaryTextAlign.Right;

        // Value formatting
        private string _valueFormat = "0.#";
        private bool _appendPercent = true;
        private DisplayValueKind _displayValue = DisplayValueKind.Enhanced;
        public Func<GraphItem, float>? ValueProvider { get; set; }

        // Colors
        private Color _baseColor = Color.FromArgb(233, 72, 201);
        private Color _enhColor = Color.FromArgb(223, 122, 255);
        private Color _overcapColor = Color.FromArgb(140, 140, 140);
        private Color _absorbedColor = Color.Gainsboro;
        private Color _borderColor = Color.Black;
        private Color _lineColor = Color.FromArgb(32, 32, 32);
        private Color _highlightFill = Color.FromArgb(32, 255, 255, 191);

        // Scale
        private float _forcedMax;   // 0 => auto
        private float _markerValue; // 0 => hidden

        // Designer sample
        [Category("Design"), DefaultValue(true)]
        public bool ShowDesignSample { get; set; } = true;
        #endregion

        #region Public API
        [Category("Layout"), DefaultValue(2)]
        public int Columns
        {
            get => _columns;
            set { _columns = Math.Max(1, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(5)]
        public int RowsPerColumn
        {
            get => _rowsPerColumn;
            set { _rowsPerColumn = Math.Max(1, value); Invalidate(); }
        }

        [Category("Behavior"), DefaultValue(false)]
        public bool AutoHeight
        {
            get => _autoHeight;
            set { _autoHeight = value; Invalidate(); }
        }

        [Category("Layout"), DefaultValue(4)]
        public int XPadding
        {
            get => _xPad;
            set { _xPad = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(4)]
        public int YPadding
        {
            get => _yPad;
            set { _yPad = Math.Max(0, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(14)]
        public int ItemHeight
        {
            get => _itemHeight;
            set { _itemHeight = Math.Max(8, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(88)]
        public int LabelWidth
        {
            get => _labelWidth;
            set { _labelWidth = Math.Max(24, value); Invalidate(); }
        }

        [Category("Layout"), DefaultValue(48)]
        [Description("Upper bound for the value column width; actual width is measured from content and capped by this.")]
        public int ValueWidth
        {
            get => _valueWidthMax;
            set { _valueWidthMax = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(true)]
        public bool ShowValueText
        {
            get => _showValueText;
            set { _showValueText = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool ShowRowSeparators
        {
            get => _showRowSeparators;
            set { _showRowSeparators = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(true)]
        public bool ShowBorders
        {
            get => _showBorders;
            set { _showBorders = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool DualLabel
        {
            get => _dualLabel;
            set { _dualLabel = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(SecondaryTextAlign.Right)]
        public SecondaryTextAlign SecondaryLabelAlign
        {
            get => _secondaryAlign;
            set { _secondaryAlign = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue("0.#")]
        public string ValueFormat
        {
            get => _valueFormat;
            set { _valueFormat = string.IsNullOrWhiteSpace(value) ? "0.#" : value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(true)]
        public bool AppendPercentSymbol
        {
            get => _appendPercent;
            set { _appendPercent = value; Invalidate(); }
        }

        [Category("Behavior"), DefaultValue(DisplayValueKind.Enhanced)]
        public DisplayValueKind DisplayValue
        {
            get => _displayValue;
            set { _displayValue = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BaseColor { get => _baseColor; set { _baseColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color EnhColor { get => _enhColor; set { _enhColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color OvercapColor { get => _overcapColor; set { _overcapColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color AbsorbedColor { get => _absorbedColor; set { _absorbedColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color LineColor { get => _lineColor; set { _lineColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color HighlightColor { get => _highlightFill; set { _highlightFill = value; Invalidate(); } }

        [Category("Behavior"), DefaultValue(0f)]
        public float ForcedMax { get => _forcedMax; set { _forcedMax = Math.Max(0, value); Invalidate(); } }

        [Category("Behavior"), DefaultValue(0f)]
        public float MarkerValue { get => _markerValue; set { _markerValue = Math.Max(0, value); Invalidate(); } }

        [Browsable(false)]
        public int ItemCount => _items.Count;

        public void Clear()
        {
            _items.Clear();
            _tip.SetToolTip(this, string.Empty);
            Invalidate();
        }

        public void AddItem(string name, float baseVal, float enhVal, string? tip = null)
            => _items.Add(new GraphItem(name, null, baseVal, enhVal, enhVal, 0, tip ?? string.Empty));

        public void AddItemPair(string name, string name2, float baseVal, float enhVal, string? tip = null)
            => _items.Add(new GraphItem(name, name2, baseVal, enhVal, enhVal, 0, tip ?? string.Empty));

        public void AddItemOvercap(string name, string? name2, float baseVal, float enhVal, float overcap, float absorbed, string? tip = null)
            => _items.Add(new GraphItem(name, name2, baseVal, enhVal, overcap, absorbed, tip ?? string.Empty));
        #endregion

        #region Boilerplate
        public MidsTotalsGraph()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            TabStop = false;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            EnsureDesignSample();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_autoHeight && !DesignMode)
                Height = ComputeContentHeight();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                _tip.SetToolTip(this, string.Empty);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = HitTestRow(e.Location);
            if (idx != _hoverIndex)
            {
                _hoverIndex = idx;
                _tip.SetToolTip(this, idx >= 0 ? _items[idx].Tip : string.Empty);
                Invalidate();
            }
        }
        #endregion

        #region Paint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_items.Count == 0) EnsureDesignSample();

            var g = e.Graphics;
            g.Clear(BackColor);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _rowHitRects.Clear();

            if (_items.Count == 0)
            {
                if (_autoHeight) Height = Padding.Vertical + ScalePx(_itemHeight + _yPad);
                return;
            }

            // 1) Layout box
            int innerL = Padding.Left + ScalePx(_xPad);
            int innerT = Padding.Top + ScalePx(_yPad);
            int innerR = Width - Padding.Right - ScalePx(_xPad);
            int innerB = Height - Padding.Bottom - ScalePx(_yPad);
            if (innerR <= innerL || innerB <= innerT) return;

            // 2) Columns & rows
            int rowsPerCol = Math.Max(1, _rowsPerColumn);
            int cols = Math.Max(_columns, (int)Math.Ceiling(_items.Count / (double)rowsPerCol)); // ensure all items show
            int colGap = ScalePx(12);
            int colW = (innerR - innerL - (cols - 1) * colGap) / cols;
            if (colW <= 0) return;

            int rowH = ScalePx(_itemHeight + _yPad);

            // 3) Fixed widths and equal gaps
            int labelW = ScalePx(_labelWidth);
            int gapLv = _showValueText ? ScalePx(_xPad) : 0; // Label → Value
            int gapVb = _showValueText ? ScalePx(_xPad) : 0; // Value → Bar

            // 4) Measure a single value width across ALL items (keeps both columns aligned)
            int valueW = 0;
            if (_showValueText)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    float v = ValueProvider != null
                        ? ValueProvider(_items[i])
                        : _displayValue switch
                        {
                            DisplayValueKind.Base => _items[i].ValueBase,
                            DisplayValueKind.Overcap => _items[i].ValueOvercap,
                            _ => _items[i].ValueEnh
                        };

                    string s = v.ToString(_valueFormat);
                    if (_appendPercent) s += "%";
                    var sz = TextRenderer.MeasureText(g, s, Font, new Size(int.MaxValue, int.MaxValue), ValueFlags);
                    valueW = Math.Max(valueW, sz.Width);
                }
                valueW = Math.Min(valueW, ScalePx(_valueWidthMax));
            }

            // 5) Bar width (shrink value first if space is tight)
            int barW = colW - labelW - valueW - gapLv - gapVb;
            int minBarW = ScalePx(MinBarWidthLogical);
            if (barW < minBarW)
            {
                int deficit = minBarW - barW;
                valueW = Math.Max(0, valueW - deficit);
                barW = colW - labelW - valueW - gapLv - gapVb;
            }
            if (barW <= 0) return;

            // 6) Scale max
            float max = _forcedMax > 0
                ? _forcedMax
                : _items.Select(i => Math.Max(Math.Max(i.ValueBase, i.ValueEnh), i.ValueOvercap))
                        .DefaultIfEmpty(100f).Max();

            using var baseBrush = new SolidBrush(_baseColor);
            using var enhBrush = new SolidBrush(_enhColor);
            using var overBrush = new SolidBrush(_overcapColor);
            using var absBrush = new SolidBrush(Color.FromArgb(180, _absorbedColor));
            using var sepPen = new Pen(_lineColor, 1f);
            using var borderPen = new Pen(_borderColor, 1f);
            using var marker2 = new Pen(Color.FromArgb(192, ForeColor), 2f);
            using var marker1 = new Pen(ForeColor, 1f);

            // 7) Rows
            for (int i = 0; i < _items.Count; i++)
            {
                int col = i / rowsPerCol;
                int row = i % rowsPerCol;

                int cx = innerL + col * (colW + colGap);
                int cy = innerT + row * rowH;

                var labelRect = new Rectangle(cx, cy, labelW, ScalePx(_itemHeight));
                var valueRect = new Rectangle(cx + labelW + gapLv, cy, valueW, ScalePx(_itemHeight));
                var barRect = new Rectangle(cx + labelW + gapLv + valueW + gapVb, cy, barW, ScalePx(_itemHeight));

                // Hover highlight
                if (i == _hoverIndex)
                {
                    using var h = new SolidBrush(_highlightFill);
                    g.FillRectangle(h, new Rectangle(cx, cy - ScalePx(_yPad / 2), colW, ScalePx(_itemHeight + _yPad)));
                }

                // Label (single or dual)
                if (_dualLabel && !string.IsNullOrEmpty(_items[i].Name2))
                {
                    int half = labelRect.Height / 2;
                    var top = new Rectangle(labelRect.X, labelRect.Y, labelRect.Width, half);
                    var bot = new Rectangle(labelRect.X, labelRect.Y + half, labelRect.Width, labelRect.Height - half);

                    TextRenderer.DrawText(g, _items[i].Name, Font, top, ForeColor, LabelFlags);

                    var align = _secondaryAlign == SecondaryTextAlign.Left ? TextFormatFlags.Left : TextFormatFlags.Right;
                    TextRenderer.DrawText(g, _items[i].Name2!, Font, bot, ForeColor, (LabelFlags & ~TextFormatFlags.Left) | align);
                }
                else
                {
                    TextRenderer.DrawText(g, _items[i].Name, Font, labelRect, ForeColor, LabelFlags);
                }

                // Value text
                if (_showValueText && valueW > 0)
                {
                    float v = ValueProvider != null
                        ? ValueProvider(_items[i])
                        : _displayValue switch
                        {
                            DisplayValueKind.Base => _items[i].ValueBase,
                            DisplayValueKind.Overcap => _items[i].ValueOvercap,
                            _ => _items[i].ValueEnh
                        };

                    string vs = v.ToString(_valueFormat);
                    if (_appendPercent) vs += "%";
                    TextRenderer.DrawText(g, vs, Font, valueRect, ForeColor, ValueFlags);
                }

                // Bars: base, then deltas (enh, overcap); absorbed overlays from the start
                float b = Math.Max(0f, _items[i].ValueBase);
                float eh = Math.Max(b, _items[i].ValueEnh);
                float oc = Math.Max(eh, _items[i].ValueOvercap);
                float ab = Math.Max(0f, _items[i].ValueAbsorbed);

                int wBase = max > 0 ? (int)Math.Round(barW * (b / max)) : 0;
                int wEnh = max > 0 ? (int)Math.Round(barW * ((eh - b) / max)) : 0;
                int wOver = max > 0 ? (int)Math.Round(barW * ((oc - eh) / max)) : 0;
                int wAbs = max > 0 ? (int)Math.Round(barW * (ab / max)) : 0;

                if (wBase > 0) g.FillRectangle(baseBrush, barRect.X, barRect.Y, wBase, barRect.Height);
                if (wEnh > 0) g.FillRectangle(enhBrush, barRect.X + wBase, barRect.Y, wEnh, barRect.Height);
                if (wOver > 0) g.FillRectangle(overBrush, barRect.X + wBase + wEnh, barRect.Y, wOver, barRect.Height);
                if (wAbs > 0) g.FillRectangle(absBrush, barRect.X, barRect.Y, Math.Min(wAbs, wBase + wEnh + wOver), barRect.Height);

                if (_showRowSeparators)
                    g.DrawLine(sepPen, cx, cy + barRect.Height + ScalePx(_yPad / 2), cx + colW, cy + barRect.Height + ScalePx(_yPad / 2));

                _rowHitRects.Add(new Rectangle(cx, cy - ScalePx(_yPad / 2), colW, ScalePx(_itemHeight + _yPad)));
            }

            // 8) Marker rail (optional)
            if (_markerValue > 0 && max > 0)
            {
                for (int col = 0; col < cols; col++)
                {
                    int cx = innerL + col * (colW + colGap);
                    int x = cx + labelW + gapLv + valueW + gapVb
                           + (int)Math.Round(barW * (_markerValue / max));

                    int rowsInCol = Math.Min(rowsPerCol, Math.Max(0, _items.Count - col * rowsPerCol));
                    if (rowsInCol <= 0) continue;

                    int top = innerT - ScalePx(_yPad / 2);
                    int bot = innerT + rowsInCol * rowH - ScalePx(_yPad / 2);

                    g.DrawLine(marker2, x, top, x, bot);
                    g.DrawLine(marker1, x, top, x, bot);
                }
            }

            if (_showBorders)
                g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
        }
        #endregion

        #region Helpers & model
        private int ScalePx(int logicalPx) => (int)Math.Round(logicalPx * (DeviceDpi / 96f));

        private int HitTestRow(Point p)
        {
            for (int i = 0; i < _rowHitRects.Count; i++)
                if (_rowHitRects[i].Contains(p)) return i;
            return -1;
        }

        private int ComputeContentHeight()
        {
            // Height that would fit exactly RowsPerColumn rows (for AutoHeight scenarios)
            return Padding.Vertical + ScalePx(_yPad) + ScalePx((_itemHeight + _yPad) * _rowsPerColumn);
        }

        private void EnsureDesignSample()
        {
            if (!ShowDesignSample || !DesignMode || _items.Count > 0) return;

            // Two columns of sample content
            AddItem("Smashing", 75f, 90f, "Smashing");
            AddItem("Lethal", 75f, 90f, "Lethal");
            AddItem("Energy", 65f, 85f, "Energy");
            AddItem("Negative", 65f, 85f, "Negative");
            AddItem("Toxic", 50f, 70f, "Toxic");
            AddItem("Psionic", 45f, 65f, "Psionic");

            AddItem("Fire", 50f, 56.4f, "Fire");
            AddItem("Cold", 50f, 56.4f, "Cold");
            AddItem("Melee", 65f, 74.9f, "Melee");
            AddItem("Ranged", 55f, 69.9f, "Ranged");
            AddItem("AoE", 72f, 80.5f, "AoE");
        }

        public enum SecondaryTextAlign { Left, Right }
        public enum DisplayValueKind { Enhanced, Base, Overcap }

        public sealed class GraphItem
        {
            public GraphItem(string name, string? name2, float baseVal, float enhVal, float overcapVal, float absorbedVal, string tip)
            {
                Name = name;
                Name2 = name2;
                ValueBase = baseVal;
                ValueEnh = enhVal;
                ValueOvercap = overcapVal;
                ValueAbsorbed = absorbedVal;
                Tip = tip;
            }
            public string Name { get; }
            public string? Name2 { get; }
            public float ValueBase { get; }
            public float ValueEnh { get; }
            public float ValueOvercap { get; }
            public float ValueAbsorbed { get; }
            public string Tip { get; }
        }
        #endregion
    }
}
