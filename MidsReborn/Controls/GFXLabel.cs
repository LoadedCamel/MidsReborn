using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    // Token: 0x02000014 RID: 20
    public class GFXLabel : UserControl
    {
        // Token: 0x04000092 RID: 146
        private ExtendedBitmap? bxBuffer;

        // Token: 0x04000094 RID: 148
        private ContentAlignment myAlign;

        // Token: 0x04000093 RID: 147
        private Graphics myGFX;

        // Token: 0x04000091 RID: 145
        private string myText;

        // Token: 0x06000134 RID: 308 RVA: 0x0000AFFC File Offset: 0x000091FC
        public GFXLabel()
        {
            BackColorChanged += GFXLabel_BackColorChanged;
            FontChanged += GFXLabel_FontChanged;
            SizeChanged += GFXLabel_SizeChanged;
            Resize += GFXLabel_Resize;
            Load += GFXLabel_Load;
            Paint += GFXlabel_Paint;
            InitializeComponent();
        }

        public override Font Font => new(Fonts.Family("Noto Sans"), base.Font.Size, base.Font.Style, GraphicsUnit.Pixel);

        // Token: 0x17000050 RID: 80
        // (get) Token: 0x0600012E RID: 302 RVA: 0x0000AF38 File Offset: 0x00009138
        // (set) Token: 0x0600012F RID: 303 RVA: 0x0000AF50 File Offset: 0x00009150
        public override string Text
        {
            get => myText;
            set
            {
                if (Operators.CompareString(value, myText, false) == 0)
                    return;
                myText = value;
                Draw();
            }
        }

        // Token: 0x17000051 RID: 81
        // (get) Token: 0x06000130 RID: 304 RVA: 0x0000AF84 File Offset: 0x00009184
        // (set) Token: 0x06000131 RID: 305 RVA: 0x0000AF9C File Offset: 0x0000919C
        public string InitialText
        {
            get => myText;
            set
            {
                if (Operators.CompareString(value, myText, false) == 0)
                    return;
                myText = value;
                Draw();
            }
        }

        // Token: 0x17000052 RID: 82
        // (get) Token: 0x06000132 RID: 306 RVA: 0x0000AFD0 File Offset: 0x000091D0
        // (set) Token: 0x06000133 RID: 307 RVA: 0x0000AFE8 File Offset: 0x000091E8
        public ContentAlignment TextAlign
        {
            get => myAlign;
            set
            {
                myAlign = value;
                Draw();
            }
        }

        // Token: 0x04000090 RID: 144
        private IContainer Components { get; }

        // Token: 0x06000135 RID: 309 RVA: 0x0000B08C File Offset: 0x0000928C
        protected override void Dispose(bool disposing)
        {
            if (disposing) Components?.Dispose();

            base.Dispose(disposing);
        }

        // Token: 0x06000136 RID: 310 RVA: 0x0000B0C4 File Offset: 0x000092C4
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            Name = "GFXLabel";
            var size = new Size(324, 72);
            Size = size;
        }

        // Token: 0x06000137 RID: 311 RVA: 0x0000B0F5 File Offset: 0x000092F5
        private void GFXLabel_Load(object sender, EventArgs e)
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        // Token: 0x06000138 RID: 312 RVA: 0x0000B124 File Offset: 0x00009324
        private void Draw()
        {
            if (bxBuffer == null)
                return;
            var rect = new Rectangle(0, 0, Width, Height);
            var layoutRectangle = new RectangleF(0f, 0f, 0f, 0f);
            var stringFormat = new StringFormat();
            layoutRectangle.X = 0f;
            layoutRectangle.Width = Width;
            layoutRectangle.Height = Font.GetHeight();
            bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Brush brush = new SolidBrush(BackColor);
            bxBuffer.Graphics.FillRectangle(brush, rect);
            brush = new SolidBrush(ForeColor);
            var num = 1f;
            var num2 = checked((int) Math.Round(bxBuffer.Graphics
                .MeasureString(myText, Font, (int) Math.Round(layoutRectangle.Width * 10f), stringFormat).Width));
            if (num2 > checked(bxBuffer.Size.Width - 10)) num = (bxBuffer.Size.Width - 10f) / num2;

            float num3;
            if (num < 0.6)
            {
                num = 0.6f;
                num3 = 2f;
            }
            else
            {
                num3 = 1f;
            }

            var font = new Font(Font.Name, Font.Size * num, Font.Style, GraphicsUnit.Point, 0);
            //var font = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel, 1);
            layoutRectangle.Height = font.GetHeight() * num3;
            var contentAlignment = myAlign;
            switch (contentAlignment)
            {
                case ContentAlignment.BottomCenter:
                    stringFormat.Alignment = StringAlignment.Center;
                    layoutRectangle.Y = Height - layoutRectangle.Height;
                    break;
                case ContentAlignment.BottomLeft:
                    stringFormat.Alignment = StringAlignment.Near;
                    layoutRectangle.Y = Height - layoutRectangle.Height;
                    break;
                case ContentAlignment.BottomRight:
                    stringFormat.Alignment = StringAlignment.Far;
                    layoutRectangle.Y = Height - layoutRectangle.Height;
                    break;
                case ContentAlignment.MiddleCenter:
                    stringFormat.Alignment = StringAlignment.Center;
                    layoutRectangle.Y = (Height - layoutRectangle.Height) / 2f;
                    break;
                case ContentAlignment.MiddleLeft:
                    stringFormat.Alignment = StringAlignment.Near;
                    layoutRectangle.Y = (Height - layoutRectangle.Height) / 2f;
                    break;
                case ContentAlignment.MiddleRight:
                    stringFormat.Alignment = StringAlignment.Far;
                    layoutRectangle.Y = (Height - layoutRectangle.Height) / 2f;
                    break;
                case ContentAlignment.TopCenter:
                    stringFormat.Alignment = StringAlignment.Center;
                    layoutRectangle.Y = 0f;
                    break;
                case ContentAlignment.TopLeft:
                    stringFormat.Alignment = StringAlignment.Near;
                    layoutRectangle.Y = 0f;
                    break;
                case ContentAlignment.TopRight:
                    stringFormat.Alignment = StringAlignment.Far;
                    layoutRectangle.Y = 0f;
                    break;
            }

            bxBuffer.Graphics.DrawString(myText, font, brush, layoutRectangle, stringFormat);
            myGFX.DrawImageUnscaled(bxBuffer.Bitmap, 0, 0);
        }

        // Token: 0x06000139 RID: 313 RVA: 0x0000B4F8 File Offset: 0x000096F8
        private void GFXlabel_Paint(object sender, PaintEventArgs e)
        {
            if (bxBuffer != null)
                myGFX.DrawImage(bxBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        // Token: 0x0600013A RID: 314 RVA: 0x0000B53D File Offset: 0x0000973D
        protected override void OnResize(EventArgs e)
        {
            Draw();
        }

        // Token: 0x0600013B RID: 315 RVA: 0x0000B547 File Offset: 0x00009747
        protected override void OnFontChanged(EventArgs e)
        {
            Draw();
        }

        // Token: 0x0600013C RID: 316 RVA: 0x0000B551 File Offset: 0x00009751
        protected override void OnForeColorChanged(EventArgs e)
        {
            Draw();
        }

        // Token: 0x0600013D RID: 317 RVA: 0x0000B55B File Offset: 0x0000975B
        private void GFXLabel_BackColorChanged(object sender, EventArgs e)
        {
            Draw();
        }

        // Token: 0x0600013E RID: 318 RVA: 0x0000B565 File Offset: 0x00009765
        private void GFXLabel_FontChanged(object sender, EventArgs e)
        {
            Draw();
        }

        // Token: 0x0600013F RID: 319 RVA: 0x0000B56F File Offset: 0x0000976F
        private void GFXLabel_SizeChanged(object sender, EventArgs e)
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }

        // Token: 0x06000140 RID: 320 RVA: 0x0000B59C File Offset: 0x0000979C
        private void GFXLabel_Resize(object sender, EventArgs e)
        {
            myGFX = CreateGraphics();
            bxBuffer = new ExtendedBitmap(Width, Height);
            Draw();
        }
    }
}