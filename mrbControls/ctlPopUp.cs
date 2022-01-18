using Microsoft.VisualBasic.CompilerServices;
using mrbBase.Base.Display;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace mrbControls
{
    [DesignerGenerated]
    public class ctlPopUp : UserControl
    {
        private IContainer components;

        public int eIDX;

        public int hIDX;

        public float lHeight;

        private ExtendedBitmap myBX;

        private int pBXHeight;

        private float pColumnPosition;

        public PopUp.PopupData pData;

        public int pIDX;

        private int pInternalPadding;

        private bool pRightAlignColumn;

        private float pScroll;

        private int pSectionPadding;

        public int psIDX;

        private Font pFont;

        public ctlPopUp()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            BackColorChanged += ctlPopUp_BackColorChanged;
            FontChanged += ctlPopUp_FontChanged;
            ForeColorChanged += ctlPopUp_ForeColorChanged;
            Paint += ctlPopUp_Paint;
            Load += ctlPopUp_Load;
            SizeChanged += ctlPopUp_SizeChanged;
            pSectionPadding = 8;
            pInternalPadding = 3;
            pScroll = 0f;
            lHeight = 0f;
            pBXHeight = 675;
            pColumnPosition = 0.5f;
            pRightAlignColumn = false;
            hIDX = -1;
            pIDX = -1;
            eIDX = -1;
            psIDX = -1;
            pFont = new Font("Arial", 12.25f, FontStyle.Bold, GraphicsUnit.Pixel);
            InitializeComponent();
        }

        public int BXHeight
        {
            get => pBXHeight;
            set
            {
                pBXHeight = value;
                NewBX();
            }
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

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    components?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
            Name = "ctlPopUp";
            var size = new Size(167, 104);
            Size = size;
            ResumeLayout(false);
        }

        private void ctlPopUp_BackColorChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        private void ctlPopUp_FontChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        private void ctlPopUp_ForeColorChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        private void ctlPopUp_Load(object sender, EventArgs e)
        {
            NewBX();
            pData = default;
            pData.Init();
            Draw();
        }

        private void NewBX()
        {
            if (pBXHeight < 300)
            {
                pBXHeight = 300;
            }

            myBX = new ExtendedBitmap(Size.Width, pBXHeight);
            myBX.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            myBX.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            myBX.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            myBX.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            myBX.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public void SetPopup(PopUp.PopupData iPopup)
        {
            pData = iPopup;
            Draw();
        }

        private void Draw()
        {
            if (myBX == null)
            {
                NewBX();
            }

            myBX.Graphics.Clear(BackColor);
            DrawBorder();
            DrawStrings();
            CreateGraphics().DrawImageUnscaled(myBX.Bitmap, 0, 0);
        }

        private void DrawStrings()
        {
            var num = 0f;
            checked
            {
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
                var num3 = 0;
                var num4 = pData.Sections.Length - 1;
                for (var i = num3; i <= num4; i++)
                {
                    if (pData.Sections[i].Content == null)
                    {
                        continue;
                    }

                    var num5 = 0;
                    var num6 = pData.Sections[i].Content.Length - 1;
                    for (var j = num5; j <= num6; j++)
                    {
                        unchecked
                        {
                            var layoutRectangle = new RectangleF(pInternalPadding + pData.Sections[i].Content[j].tIndent * Font.Size, num + pInternalPadding, Width - (checked(pInternalPadding * 2) + pData.Sections[i].Content[j].tIndent * Font.Size), myBX.Size.Height);
                            if (pData.Sections[i].Content[j].HasColumn)
                            {
                                stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                            }

                            var sizeF = myBX.Graphics.MeasureString(Operators.CompareString(pData.Sections[i].Content[j].Text, "", false) == 0
                                    ? "Null String"
                                    : pData.Sections[i].Content[j].Text, pFont, layoutRectangle.Size, stringFormat);
                            var brush = new SolidBrush(pData.Sections[i].Content[j].tColor);
                            layoutRectangle.Height = sizeF.Height + 1f;
                            layoutRectangle = new RectangleF(layoutRectangle.X, layoutRectangle.Y - pScroll, layoutRectangle.Width, layoutRectangle.Height);
                            myBX.Graphics.DrawString(pData.Sections[i].Content[j].Text, pFont, brush, layoutRectangle, stringFormat);
                            if (pData.Sections[i].Content[j].HasColumn)
                            {
                                if (pRightAlignColumn)
                                {
                                    stringFormat.Alignment = StringAlignment.Far;
                                }

                                var stringSize = TextRenderer.MeasureText(myBX.Graphics, pData.Sections[i].Content[j].TextColumn.Trim(), pFont);
                                layoutRectangle.X = checked(pInternalPadding * 6) + checked(Width - stringSize.Width * 2) * pColumnPosition;
                                //layoutRectangle.X = pInternalPadding + checked(Width - pInternalPadding * 2) * pColumnPosition - pInternalPadding;
                                layoutRectangle.Width = Width - (pInternalPadding + layoutRectangle.X);
                                brush = new SolidBrush(pData.Sections[i].Content[j].tColorColumn);
                                myBX.Graphics.DrawString(pData.Sections[i].Content[j].TextColumn, pFont, brush, layoutRectangle, stringFormat);
                                stringFormat.FormatFlags = StringFormatFlags.NoClip;
                            }

                            stringFormat.Alignment = StringAlignment.Near;
                            num += sizeF.Height + 1f;
                        }
                    }

                    num += pSectionPadding;
                }

                Height = (int)Math.Round(num);
                lHeight = num;
                pColumnPosition = num2;
                pRightAlignColumn = flag;
            }
        }

        private void DrawBorder()
        {
            var pen = new Pen(ForeColor);
            Rectangle rect = default;
            rect.X = 0;
            rect.Y = 0;
            checked
            {
                rect.Height = Height - 1;
                rect.Width = Width - 1;
                myBX.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void ctlPopUp_Paint(object sender, PaintEventArgs e)
        {
            if (myBX != null)
            {
                e.Graphics.DrawImageUnscaled(myBX.Bitmap, 0, 0);
            }
        }

        private void ctlPopUp_SizeChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }
    }
}