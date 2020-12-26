using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using mrbBase.Base.Display;

namespace mrbControls
{
    // Token: 0x02000013 RID: 19
    [DesignerGenerated]
    public class ctlPopUp : UserControl
    {
        // Token: 0x04000082 RID: 130
        private IContainer components;

        // Token: 0x0400008E RID: 142
        public int eIDX;

        // Token: 0x0400008C RID: 140
        public int hIDX;

        // Token: 0x04000088 RID: 136
        public float lHeight;

        // Token: 0x04000083 RID: 131
        private ExtendedBitmap myBX;

        // Token: 0x04000089 RID: 137
        private int pBXHeight;

        // Token: 0x0400008A RID: 138
        private float pColumnPosition;

        // Token: 0x04000084 RID: 132
        public PopUp.PopupData pData;

        // Token: 0x0400008D RID: 141
        public int pIDX;

        // Token: 0x04000086 RID: 134
        private int pInternalPadding;

        // Token: 0x0400008B RID: 139
        private bool pRightAlignColumn;

        // Token: 0x04000087 RID: 135
        private float pScroll;

        // Token: 0x04000085 RID: 133
        private int pSectionPadding;

        // Token: 0x0400008F RID: 143
        public int psIDX;

        // Token: 0x0600012D RID: 301 RVA: 0x0000AE4C File Offset: 0x0000904C
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
            pBXHeight = 600;
            pColumnPosition = 0.5f;
            pRightAlignColumn = false;
            hIDX = -1;
            pIDX = -1;
            eIDX = -1;
            psIDX = -1;
            InitializeComponent();
        }

        // Token: 0x1700004A RID: 74
        // (get) Token: 0x06000114 RID: 276 RVA: 0x0000A5A4 File Offset: 0x000087A4
        // (set) Token: 0x06000115 RID: 277 RVA: 0x0000A5BC File Offset: 0x000087BC
        public int BXHeight
        {
            get => pBXHeight;
            set
            {
                pBXHeight = value;
                NewBX();
            }
        }

        // Token: 0x1700004B RID: 75
        // (get) Token: 0x06000116 RID: 278 RVA: 0x0000A5D0 File Offset: 0x000087D0
        // (set) Token: 0x06000117 RID: 279 RVA: 0x0000A5E8 File Offset: 0x000087E8
        public float ColumnPosition
        {
            get => pColumnPosition;
            set
            {
                pColumnPosition = value;
                Draw();
            }
        }

        // Token: 0x1700004C RID: 76
        // (get) Token: 0x06000118 RID: 280 RVA: 0x0000A5FC File Offset: 0x000087FC
        // (set) Token: 0x06000119 RID: 281 RVA: 0x0000A614 File Offset: 0x00008814
        public bool ColumnRight
        {
            get => pRightAlignColumn;
            set
            {
                pRightAlignColumn = value;
                Draw();
            }
        }

        // Token: 0x1700004D RID: 77
        // (get) Token: 0x0600011A RID: 282 RVA: 0x0000A628 File Offset: 0x00008828
        // (set) Token: 0x0600011B RID: 283 RVA: 0x0000A640 File Offset: 0x00008840
        public int SectionPadding
        {
            get => pSectionPadding;
            set
            {
                pSectionPadding = value;
                Draw();
            }
        }

        // Token: 0x1700004E RID: 78
        // (get) Token: 0x0600011C RID: 284 RVA: 0x0000A654 File Offset: 0x00008854
        // (set) Token: 0x0600011D RID: 285 RVA: 0x0000A66C File Offset: 0x0000886C
        public int InternalPadding
        {
            get => pInternalPadding;
            set
            {
                pInternalPadding = value;
                Draw();
            }
        }

        // Token: 0x1700004F RID: 79
        // (get) Token: 0x0600011E RID: 286 RVA: 0x0000A680 File Offset: 0x00008880
        // (set) Token: 0x0600011F RID: 287 RVA: 0x0000A698 File Offset: 0x00008898
        public float ScrollY
        {
            get => pScroll;
            set
            {
                if (!(Math.Abs(pScroll - value) > float.Epsilon))
                    return;
                pScroll = value;
                Draw();
            }
        }

        // Token: 0x06000120 RID: 288 RVA: 0x0000A6C4 File Offset: 0x000088C4
        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing) components?.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Token: 0x06000121 RID: 289 RVA: 0x0000A714 File Offset: 0x00008914
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = new Font("Arial", 11.5f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
            Name = "ctlPopUp";
            var size = new Size(167, 104);
            Size = size;
            ResumeLayout(false);
        }

        // Token: 0x06000122 RID: 290 RVA: 0x0000A775 File Offset: 0x00008975
        private void ctlPopUp_BackColorChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        // Token: 0x06000123 RID: 291 RVA: 0x0000A786 File Offset: 0x00008986
        private void ctlPopUp_FontChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        // Token: 0x06000124 RID: 292 RVA: 0x0000A797 File Offset: 0x00008997
        private void ctlPopUp_ForeColorChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }

        // Token: 0x06000125 RID: 293 RVA: 0x0000A7A8 File Offset: 0x000089A8
        private void ctlPopUp_Load(object sender, EventArgs e)
        {
            NewBX();
            pData = default;
            pData.Init();
            Draw();
        }

        // Token: 0x06000126 RID: 294 RVA: 0x0000A7D4 File Offset: 0x000089D4
        private void NewBX()
        {
            if (pBXHeight < 300) pBXHeight = 300;

            myBX = new ExtendedBitmap(Size.Width, pBXHeight);
            myBX.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            myBX.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            myBX.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            myBX.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        // Token: 0x06000127 RID: 295 RVA: 0x0000A834 File Offset: 0x00008A34
        public void SetPopup(PopUp.PopupData iPopup)
        {
            pData = iPopup;
            Draw();
        }

        // Token: 0x06000128 RID: 296 RVA: 0x0000A848 File Offset: 0x00008A48
        private void Draw()
        {
            if (myBX == null) NewBX();

            myBX.Graphics.Clear(BackColor);
            DrawBorder();
            DrawStrings();
            CreateGraphics().DrawImageUnscaled(myBX.Bitmap, 0, 0);
        }

        // Token: 0x06000129 RID: 297 RVA: 0x0000A8B0 File Offset: 0x00008AB0
        private void DrawStrings()
        {
            var num = 0f;
            checked
            {
                if (pData.Sections == null)
                    return;
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
                        continue;
                    var num5 = 0;
                    var num6 = pData.Sections[i].Content.Length - 1;
                    for (var j = num5; j <= num6; j++)
                        unchecked
                        {
                            /*Font font = new Font(Font.FontFamily, Font.Size * pData.Sections[i].Content[j].tSize,
                                pData.Sections[i].Content[j].tFormat, Font.Unit);*/
                            var font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel, 1);
                            var layoutRectangle = new RectangleF(
                                pInternalPadding + pData.Sections[i].Content[j].tIndent * Font.Size,
                                num + pInternalPadding,
                                Width - (checked(pInternalPadding * 2) +
                                         pData.Sections[i].Content[j].tIndent * Font.Size), myBX.Size.Height);
                            if (pData.Sections[i].Content[j].HasColumn)
                                stringFormat.FormatFlags |= StringFormatFlags.NoWrap;

                            var sizeF = myBX.Graphics.MeasureString(
                                Operators.CompareString(pData.Sections[i].Content[j].Text, "", false) == 0
                                    ? "Null String"
                                    : pData.Sections[i].Content[j].Text, font, layoutRectangle.Size, stringFormat);
                            var brush = new SolidBrush(pData.Sections[i].Content[j].tColor);
                            layoutRectangle.Height = sizeF.Height + 1f;
                            layoutRectangle = new RectangleF(layoutRectangle.X, layoutRectangle.Y - pScroll,
                                layoutRectangle.Width,
                                layoutRectangle.Height);
                            myBX.Graphics.DrawString(pData.Sections[i].Content[j].Text, font, brush, layoutRectangle,
                                stringFormat);
                            if (pData.Sections[i].Content[j].HasColumn)
                            {
                                if (pRightAlignColumn) stringFormat.Alignment = StringAlignment.Far;

                                layoutRectangle.X = pInternalPadding +
                                                    checked(Width - pInternalPadding * 2) * pColumnPosition;
                                layoutRectangle.Width = Width - (pInternalPadding + layoutRectangle.X);
                                brush = new SolidBrush(pData.Sections[i].Content[j].tColorColumn);
                                myBX.Graphics.DrawString(pData.Sections[i].Content[j].TextColumn, font, brush,
                                    layoutRectangle, stringFormat);
                                stringFormat.FormatFlags = StringFormatFlags.NoClip;
                            }

                            stringFormat.Alignment = StringAlignment.Near;
                            num += sizeF.Height + 1f;
                        }

                    num += pSectionPadding;
                }

                Height = (int) Math.Round(num);
                lHeight = num;
                pColumnPosition = num2;
                pRightAlignColumn = flag;
            }
        }

        // Token: 0x0600012A RID: 298 RVA: 0x0000AD94 File Offset: 0x00008F94
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

        // Token: 0x0600012B RID: 299 RVA: 0x0000AE00 File Offset: 0x00009000
        private void ctlPopUp_Paint(object sender, PaintEventArgs e)
        {
            if (myBX != null) e.Graphics.DrawImageUnscaled(myBX.Bitmap, 0, 0);
        }

        // Token: 0x0600012C RID: 300 RVA: 0x0000AE3A File Offset: 0x0000903A
        private void ctlPopUp_SizeChanged(object sender, EventArgs e)
        {
            NewBX();
            Draw();
        }
    }
}