using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UI.Controls
{
    [ToolboxItem(false)]
    public sealed class PopUpDisplay : UserControl
    {
        #region Constants

        private const int DefaultBxHeight = 675;
        private const int MinBxHeight = 300;
        private const float DefaultColumnPosition = 0.5f;

        #endregion

        #region Static Fields

        private new static readonly Font DefaultFont = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Point);

        #endregion

        #region Instance Fields

        private BufferedGraphics? _buffer;
        private BufferedGraphicsContext? _bufferContext;
        private readonly IContainer? _components;
        private Font _font;
        private I9Picker.EnhUniqueStatus? _enhUniqueStatus;

        private int _bxHeight;
        private float _columnPosition;
        private bool _columnRight;
        private int _internalPadding;
        private int _sectionPadding;
        private float _scrollY;

        public PopUp.PopupData PopupData;

        public int EIdx = -1;
        public int HIdx = -1;
        public int PIdx = -1;
        public int PsIdx = -1;
        public float LayoutHeight;

        #endregion

        #region Properties

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BxHeight
        {
            get => _bxHeight;
            set
            {
                _bxHeight = value < MinBxHeight ? MinBxHeight : value;
                CreateBuffer();
            }
        }

        [Browsable(true)]
        public float ColumnPosition
        {
            get => _columnPosition;
            set
            {
                _columnPosition = value;
                Redraw();
            }
        }

        [Browsable(true)]
        public bool ColumnRight
        {
            get => _columnRight;
            set
            {
                _columnRight = value;
                Redraw();
            }
        }

        [Browsable(true)]
        public int SectionPadding
        {
            get => _sectionPadding;
            set
            {
                _sectionPadding = value;
                Redraw();
            }
        }

        [Browsable(true)]
        public int InternalPadding
        {
            get => _internalPadding;
            set
            {
                _internalPadding = value;
                Redraw();
            }
        }

        [Browsable(false)]
        public float ScrollY
        {
            get => _scrollY;
            set
            {
                if (Math.Abs(_scrollY - value) > float.Epsilon)
                {
                    _scrollY = value;
                    Redraw();
                }
            }
        }

        #endregion

        #region Constructors

        public PopUpDisplay()
        {
            if (!DesignMode)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor |
                         ControlStyles.UserPaint, true);
            }

            _bxHeight = DefaultBxHeight;
            _columnPosition = DefaultColumnPosition;
            _columnRight = false;
            _internalPadding = 3;
            _sectionPadding = 8;
            _scrollY = 0f;

            _font = Fonts.Family("Noto Sans") is { } fam
                ? new Font(fam, Font.Size, FontStyle.Bold, GraphicsUnit.Point)
                : DefaultFont;

            BackColor = Color.LightYellow;
            ForeColor = Color.Black;

            InitializeComponent();

            if (!DesignMode)
            {
                Load += OnLoad;
                Paint += OnPaint;
                SizeChanged += OnSizeChanged;
                BackColorChanged += (_, _) => Redraw();
                ForeColorChanged += (_, _) => Redraw();
                FontChanged += (_, _) => Redraw();
            }
        }

        #endregion

        #region Public Methods

        public void SetPopup(PopUp.PopupData data, I9Picker.EnhUniqueStatus? status = null)
        {
            PopupData = data;
            _enhUniqueStatus = status;
            Redraw();
        }

        public void ShowAt(Control anchor, PopUp.PopupData data, I9Picker.EnhUniqueStatus? status = null)
        {
            SetPopup(data, status);
            var location = anchor.PointToScreen(Point.Empty);
            location = FindForm()?.PointToClient(location) ?? Point.Empty;
            Location = new Point(location.X + anchor.Width, location.Y);
            Visible = true;
        }

        public void HidePopup()
        {
            Visible = false;
        }

        #endregion

        #region Protected/Internal Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _components?.Dispose();
                _buffer?.Dispose();
                _bufferContext?.Dispose();
                _font.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            SuspendLayout();
            Font = _font;
            Name = nameof(PopUpDisplay);
            Size = new Size(167, 104);
            ResumeLayout(false);
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CreateBuffer();
            PopupData = default;
            PopupData.Init();
            Redraw();
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            CreateBuffer();
            Redraw();
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_buffer?.Graphics != null)
                _buffer.Render(e.Graphics);
        }

        private void CreateBuffer()
        {
            if (!IsHandleCreated || Width <= 0 || _bxHeight <= 0)
                return;

            _bufferContext ??= BufferedGraphicsManager.Current;

            _buffer?.Dispose();
            _buffer = _bufferContext.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, _bxHeight));

            var g = _buffer.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private void Redraw()
        {
            if (_buffer == null)
                return;

            Graphics g = _buffer.Graphics;
            g.Clear(BackColor);

            DrawBorder(g);
            DrawContent(g);

            Invalidate();
        }

        private void DrawBorder(Graphics g)
        {
            using var pen = new Pen(ForeColor);
            g.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        private void DrawContent(Graphics g)
        {
            if (PopupData.Sections is null) return;

            float y = 0;
            var originalColPos = _columnPosition;
            var originalColRight = _columnRight;

            if (PopupData.CustomSet)
            {
                _columnPosition = PopupData.ColPos;
                _columnRight = PopupData.ColRight;
            }

            StringFormat fmt = new(StringFormatFlags.NoClip)
            {
                LineAlignment = StringAlignment.Near,
                Alignment = StringAlignment.Near,
                Trimming = StringTrimming.None
            };

            int maxWidth = -1;

            foreach (var section in PopupData.Sections)
            {
                if (section.Content is null)
                    continue;

                foreach (var line in section.Content)
                {
                    RectangleF layout = new(_internalPadding + line.Indent * Font.Size, y + _internalPadding, Width - (2 * _internalPadding + line.Indent * Font.Size), _bxHeight);

                    if (line.HasColumn)
                        fmt.FormatFlags |= StringFormatFlags.NoWrap;

                    SizeF size = g.MeasureString(string.IsNullOrWhiteSpace(line.Text) ? "Null String" : line.Text, _font, layout.Size, fmt);
                    Size textSize = TextRenderer.MeasureText(g, line.Text, _font);
                    maxWidth = maxWidth == -1 ? textSize.Width : Math.Max(maxWidth, textSize.Width);

                    layout.Height = size.Height + 1;
                    layout.Y -= _scrollY;

                    using SolidBrush brush = new(line.Color);
                    g.DrawString(line.Text, _font, brush, layout, fmt);

                    if (line.HasColumn)
                    {
                        fmt.Alignment = _columnRight ? StringAlignment.Far : StringAlignment.Near;

                        layout.X = _internalPadding + (Width - 2 * _internalPadding) * _columnPosition;
                        layout.Width = Width - layout.X - _internalPadding;

                        using SolidBrush brush2 = new(line.ColorColumn);
                        g.DrawString(line.TextColumn, _font, brush2, layout, fmt);

                        fmt.FormatFlags = StringFormatFlags.NoClip;
                        fmt.Alignment = StringAlignment.Near;
                    }

                    y += size.Height + 1;
                }

                y += _sectionPadding;
            }

            LayoutHeight = y;
            Height = (int)Math.Round(y);

            _columnPosition = originalColPos;
            _columnRight = originalColRight;

            if (_enhUniqueStatus is { } status && (status.InMain || status.InAlternate))
            {
                string msg = status.InMain ? "[Used]" : "[In Alternate]";
                Color color = status.InMain ? Color.Cyan : Color.MediumPurple;

                SizeF sz = g.MeasureString(msg, _font);
                using SolidBrush brush = new(color);
                g.DrawString(msg, _font, brush, new PointF(Width - _internalPadding - sz.Width, _internalPadding), fmt);
            }
        }

        #endregion
    }
}