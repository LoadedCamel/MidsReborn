using System;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls
{
    public class ctlPopUp : UserControl
    {

        public int eIDX;
        public int hIDX;
        public float lHeight;
        
        public PopUp.PopupData pData;
        public int pIDX;
        public int psIDX;

        private BufferedGraphics? _buffer;
        private BufferedGraphicsContext? _bufferContext;
        private bool _disableRedraw;

        private int pBXHeight;
        private float pColumnPosition;
        private int pInternalPadding;
        private bool pRightAlignColumn;
        private float pScroll;
        private int pSectionPadding;
        private Font? pFont;
        private I9Picker.EnhUniqueStatus? _enhUniqueStatus;

        private bool IsDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            Site is { DesignMode: true };

        public ctlPopUp()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            pSectionPadding = 8;
            pInternalPadding = 3;
            pScroll = 0;
            lHeight = 0;
            pBXHeight = 675;
            pColumnPosition = 0.5f;
            pRightAlignColumn = false;
            hIDX = -1;
            pIDX = -1;
            eIDX = -1;
            psIDX = -1;
            InitializeComponent();
        }

        private void InitBuffer()
        {
            if (_disableRedraw || Width <= 0 || Height <= 0)
            {
                return;
            }

            _buffer?.Dispose();
            _bufferContext = BufferedGraphicsManager.Current;

            _buffer = _bufferContext.Allocate(CreateGraphics(), ClientRectangle);
            _buffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _buffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _buffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _buffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!IsDesignMode)
            {
                SafeInitialize();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _buffer?.Dispose();
            _buffer = null;
            base.OnHandleDestroyed(e);
        }

        private void SafeInitialize()
        {
            BackColorChanged += ctlPopUp_BackColorChanged;
            FontChanged += ctlPopUp_FontChanged;
            ForeColorChanged += ctlPopUp_ForeColorChanged;
            Load += ctlPopUp_Load;
            SizeChanged += ctlPopUp_SizeChanged;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsDesignMode)
            {
                DrawDesignTimePlaceholder(e.Graphics);

                return;
            }

            if (_buffer is null)
            {
                Draw();
            }

            _buffer?.Render(e.Graphics);
        }

        private void DrawDesignTimePlaceholder(Graphics g)
        {
            using var bgBrush = new SolidBrush(Color.FromArgb(80, 0, 120, 215));
            g.FillRectangle(bgBrush, ClientRectangle);

            using var borderPen = new Pen(Color.Gray, 1);
            borderPen.DashStyle = DashStyle.Dash;
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            var label = string.IsNullOrWhiteSpace(Name)
                ? $"{GetType().Name} (Design Time)"
                : $"{Name}";

            using var font = new Font("Segoe UI", 9f, FontStyle.Italic);
            using var textBrush = new SolidBrush(Color.White);

            var textSize = g.MeasureString(label, font);
            var center = new PointF((Width - textSize.Width) / 2f, (Height - textSize.Height) / 2f);
            g.DrawString(label, font, textBrush, center);
        }

        public float ColumnPosition
        {
            get => pColumnPosition;
            set
            {
                pColumnPosition = value;
                Draw();
            }
        }

        public bool ColumnRight
        {
            get => pRightAlignColumn;
            set
            {
                pRightAlignColumn = value;
                Draw();
            }
        }

        public int SectionPadding
        {
            get => pSectionPadding;
            set
            {
                pSectionPadding = value;
                Draw();
            }
        }

        public int InternalPadding
        {
            get => pInternalPadding;
            set
            {
                pInternalPadding = value;
                Draw();
            }
        }

        public float ScrollY
        {
            get => pScroll;
            set
            {
                if (!(Math.Abs(pScroll - value) > float.Epsilon))
                {
                    return;
                }

                pScroll = value;
                Draw();
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Name = "ctlPopUp";
            Size = new Size(167, 104);
            ResumeLayout(false);
        }

        private void ctlPopUp_BackColorChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlPopUp_FontChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlPopUp_ForeColorChanged(object? sender, EventArgs e)
        {
            Draw();
        }

        private void ctlPopUp_Load(object? sender, EventArgs e)
        {
            _disableRedraw = true;
            InitBuffer();
            pData = default;
            pData.Init();
            _disableRedraw = false;
        }

        public void SetPopup(PopUp.PopupData iPopup, I9Picker.EnhUniqueStatus? enhUniqueStatus = null)
        {
            pData = iPopup;
            _enhUniqueStatus = enhUniqueStatus;
            Draw();
        }

        private void Draw()
        {
            if (IsDisposed || _disableRedraw || Width <= 0 || Height <= 0)
            {

                return;
            }

            InitBuffer();

            if (_buffer is null)
            {
                return;
            }
           
            if (pFont == null)
            {
                try
                {
                    pFont = new Font(Fonts.Family("Noto Sans"), 12f, FontStyle.Bold, GraphicsUnit.Pixel);
                    Font = new Font(Fonts.Family("Noto Sans"), 11f, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                catch (Exception)
                {
                    pFont = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
                    Font = new Font("Microsoft Sans Serif", 11f, FontStyle.Regular, GraphicsUnit.Pixel);
                }
            }

            _buffer.Graphics.Clear(BackColor);
            DrawBorder();
            DrawStrings();

            using var screenGraphics = CreateGraphics();
            _buffer.Render(screenGraphics);
        }

        private void DrawStrings()
        {
            var num = 0f;
            
            if (pData.Sections == null)
            {
                return;
            }

            var stringFormat = new StringFormat(StringFormatFlags.NoClip);
            var num2 = pColumnPosition;
            var flag = pRightAlignColumn;
            if (pData.CustomSet)
            {
                pColumnPosition = pData.ColPos;
                pRightAlignColumn = pData.ColRight;
            }

            stringFormat.LineAlignment = StringAlignment.Near;
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.Trimming = StringTrimming.None;

            var maxPos = -1;
            foreach (var section in pData.Sections)
            {
                if (section.Content == null)
                {
                    continue;
                }

                for (var j = 0; j < section.Content.Length; j++)
                {
                    var layoutRectangle = new RectangleF(pInternalPadding + section.Content[j].tIndent * pFont.Size, num + pInternalPadding, Width - (checked(pInternalPadding * 2) + section.Content[j].tIndent * pFont.Size), Height); // myBX.Size.Height
                    if (section.Content[j].HasColumn)
                    {
                        stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                    }

                    var sizeF = _buffer?.Graphics.MeasureString(string.IsNullOrWhiteSpace(section.Content[j].Text)
                        ? "Null String"
                        : section.Content[j].Text, pFont, layoutRectangle.Size, stringFormat);

                    var contentTextSize = TextRenderer.MeasureText(_buffer?.Graphics, section.Content[j].Text, pFont);
                    maxPos = maxPos == -1
                        ? contentTextSize.Width
                        : Math.Max(maxPos, contentTextSize.Width);
                    var brush = new SolidBrush(section.Content[j].tColor);
                    layoutRectangle.Height = sizeF.Value.Height + 1;
                    layoutRectangle = layoutRectangle with {Y = layoutRectangle.Y - pScroll};
                    _buffer.Graphics.DrawString(section.Content[j].Text, pFont, brush, layoutRectangle, stringFormat);
                    if (section.Content[j].HasColumn)
                    {
                        if (pRightAlignColumn)
                        {
                            stringFormat.Alignment = StringAlignment.Far;
                        }

                        //var columnStringSize = TextRenderer.MeasureText(myBX.Graphics, pData.Sections[i].Content[j].TextColumn, pFont);
                        //layoutRectangle.X = (maxPos/2 - columnStringSize.Width) + checked(Width - columnStringSize.Width * 2);
                        layoutRectangle.X = pInternalPadding + checked(Width - pInternalPadding * 2) * pColumnPosition;
                        layoutRectangle.Width = Width - (pInternalPadding + layoutRectangle.X);
                        brush = new SolidBrush(section.Content[j].tColorColumn);
                        _buffer.Graphics.DrawString(section.Content[j].TextColumn, pFont, brush, layoutRectangle, stringFormat);
                        stringFormat.FormatFlags = StringFormatFlags.NoClip;
                    }

                    stringFormat.Alignment = StringAlignment.Near;
                    num += sizeF.Value.Height + 1;
                }

                num += pSectionPadding;
            }

            Height = (int)Math.Round(num);
            lHeight = num;
            pColumnPosition = num2;
            pRightAlignColumn = flag;

            if (_enhUniqueStatus != null && _enhUniqueStatus.Value.InMain | _enhUniqueStatus.Value.InAlternate)
            {
                var brush = new SolidBrush(_enhUniqueStatus.Value.InMain ? Color.Cyan : Color.MediumPurple);
                var enhUsedText = _enhUniqueStatus.Value.InMain ? "[Used]" : "[In Alternate]";
                var enhUsedSize = _buffer?.Graphics.MeasureString(enhUsedText, pFont);
                _buffer?.Graphics.DrawString(enhUsedText, pFont, brush, new PointF(Width - pInternalPadding - enhUsedSize.Value.Width, pInternalPadding), new StringFormat(StringFormatFlags.NoClip));
            }
        }

        private void DrawBorder()
        {
            using var pen = new Pen(ForeColor);
            _buffer?.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
        }

        private void ctlPopUp_SizeChanged(object? sender, EventArgs e)
        {
            Draw();
        }
    }
}