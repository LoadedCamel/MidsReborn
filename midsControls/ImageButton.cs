using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Base.Display;
using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    // Token: 0x0200001C RID: 28
    [DefaultEvent("ButtonClicked")]
    [DesignerGenerated]
    public class ImageButton : UserControl
    {
        // Token: 0x0200001D RID: 29
        // (Invoke) Token: 0x060001C6 RID: 454
        public delegate void ButtonClickedEventHandler();

        // Token: 0x040000CC RID: 204
        private ExtendedBitmap bxAltImage;

        // Token: 0x040000CB RID: 203
        private ExtendedBitmap bxImage;

        // Token: 0x040000CD RID: 205
        private ExtendedBitmap bxOut;

        // Token: 0x040000D4 RID: 212
        private PictureBox Knockout;

        // Token: 0x040000D5 RID: 213
        private Point KnockoutLocation;

        // Token: 0x040000D3 RID: 211
        private ImageAttributes myIA;

        // Token: 0x040000D1 RID: 209
        private bool pAltState;

        // Token: 0x040000CE RID: 206
        private string pTextOff;

        // Token: 0x040000CF RID: 207
        private string pTextOn;

        // Token: 0x040000D0 RID: 208
        private bool pToggle;

        // Token: 0x040000D2 RID: 210
        private bool pTogState;

        // Token: 0x060001B7 RID: 439 RVA: 0x0001079C File Offset: 0x0000E99C
        public ImageButton()
        {
            MouseDown += ImageButton_MouseDown;
            MouseLeave += ImageButton_MouseLeave;
            MouseUp += ImageButton_MouseUp;
            Paint += ImageButton_Paint;
            SizeChanged += ImageButton_SizeChanged;
            BackColorChanged += ImageButton_BackColorChanged;
            FontChanged += ImageButton_FontChanged;
            bxImage = null;
            bxAltImage = null;
            bxOut = null;
            pTextOff = "Button Text";
            pTextOn = "Alt Text";
            pToggle = false;
            pAltState = false;
            pTogState = false;
            myIA = null;
            Knockout = null;
            KnockoutLocation = new Point(0, 0);
            InitializeComponent();
        }

        // Token: 0x17000059 RID: 89
        // (get) Token: 0x060001A9 RID: 425 RVA: 0x0001059C File Offset: 0x0000E79C
        // (set) Token: 0x060001AA RID: 426 RVA: 0x000105B4 File Offset: 0x0000E7B4
        public string TextOff
        {
            get => pTextOff;
            set
            {
                pTextOff = value;
                Redraw();
            }
        }

        // Token: 0x1700005A RID: 90
        // (get) Token: 0x060001AB RID: 427 RVA: 0x000105C8 File Offset: 0x0000E7C8
        // (set) Token: 0x060001AC RID: 428 RVA: 0x000105E0 File Offset: 0x0000E7E0
        public string TextOn
        {
            get => pTextOn;
            set
            {
                pTextOn = value;
                Redraw();
            }
        }

        // Token: 0x1700005B RID: 91
        // (get) Token: 0x060001AD RID: 429 RVA: 0x000105F4 File Offset: 0x0000E7F4
        // (set) Token: 0x060001AE RID: 430 RVA: 0x0001060C File Offset: 0x0000E80C
        public bool Toggle
        {
            get => pToggle;
            set
            {
                pToggle = value;
                Redraw();
            }
        }

        // Token: 0x1700005C RID: 92
        // (get) Token: 0x060001AF RID: 431 RVA: 0x00010620 File Offset: 0x0000E820
        // (set) Token: 0x060001B0 RID: 432 RVA: 0x00010638 File Offset: 0x0000E838
        public bool Checked
        {
            get => pAltState;
            set
            {
                pAltState = value;
                Redraw();
            }
        }

        // Token: 0x1700005D RID: 93
        // (set) Token: 0x060001B1 RID: 433 RVA: 0x00010649 File Offset: 0x0000E849
        public ImageAttributes IA
        {
            set => myIA = value;
        }

        // Token: 0x1700005E RID: 94
        // (set) Token: 0x060001B2 RID: 434 RVA: 0x00010654 File Offset: 0x0000E854
        public Bitmap ImageOff
        {
            set
            {
                bxImage = new ExtendedBitmap(Width, Height);
                bxImage.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                bxImage.Graphics.DrawImage(value, 0, 0, bxImage.Size.Width, bxImage.Size.Height);
                Redraw();
            }
        }

        // Token: 0x1700005F RID: 95
        // (set) Token: 0x060001B3 RID: 435 RVA: 0x000106CC File Offset: 0x0000E8CC
        public Bitmap ImageOn
        {
            set
            {
                bxAltImage = new ExtendedBitmap(Width, Height);
                bxImage.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                bxAltImage.Graphics.DrawImage(value, 0, 0, bxAltImage.Size.Width, bxAltImage.Size.Height);
                Redraw();
            }
        }

        // Token: 0x17000060 RID: 96
        // (set) Token: 0x060001B4 RID: 436 RVA: 0x00010744 File Offset: 0x0000E944
        public PictureBox KnockoutPB
        {
            set
            {
                Knockout = value;
                Redraw();
            }
        }

        // Token: 0x17000061 RID: 97
        // (get) Token: 0x060001B5 RID: 437 RVA: 0x00010758 File Offset: 0x0000E958
        // (set) Token: 0x060001B6 RID: 438 RVA: 0x00010770 File Offset: 0x0000E970
        public Point KnockoutLocationPoint
        {
            get => KnockoutLocation;
            set
            {
                KnockoutLocation = value;
                if (Knockout != null) Redraw();
            }
        }

        // Token: 0x040000CA RID: 202
        private IContainer Components { get; }

        // Token: 0x14000008 RID: 8
        // (add) Token: 0x060001A7 RID: 423 RVA: 0x00010566 File Offset: 0x0000E766
        // (remove) Token: 0x060001A8 RID: 424 RVA: 0x00010580 File Offset: 0x0000E780
        public event ButtonClickedEventHandler ButtonClicked;

        // Token: 0x060001B8 RID: 440 RVA: 0x0001089C File Offset: 0x0000EA9C
        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing) Components?.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Token: 0x060001B9 RID: 441 RVA: 0x000108EC File Offset: 0x0000EAEC
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            SuspendLayout();
            var autoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleDimensions = autoScaleDimensions;
            AutoScaleMode = AutoScaleMode.Font;
            Name = "ImageButton";
            ResumeLayout(false);
        }

        // Token: 0x060001BA RID: 442 RVA: 0x00010938 File Offset: 0x0000EB38
        private void Redraw()
        {
            Draw();
            if (bxOut.Bitmap != null) CreateGraphics().DrawImage(bxOut.Bitmap, 0, 0);
        }

        // Token: 0x060001BB RID: 443 RVA: 0x0001097A File Offset: 0x0000EB7A
        public void SetImages(ImageAttributes ia, Bitmap imageOff, Bitmap imageOn)
        {
            IA = ia;
            ImageOff = imageOff;
            ImageOn = imageOn;
            Redraw();
        }

        // Token: 0x060001BC RID: 444 RVA: 0x0001099C File Offset: 0x0000EB9C
        private void ImageButton_BackColorChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        // Token: 0x060001BD RID: 445 RVA: 0x000109A6 File Offset: 0x0000EBA6
        private void ImageButton_FontChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        // Token: 0x060001BE RID: 446 RVA: 0x000109B0 File Offset: 0x0000EBB0
        private void Draw()
        {
            var stringFormat = new StringFormat();
            if (bxOut == null)
            {
                if ((Width == 0) | (Height == 0))
                {
                    bxOut = null;
                    return;
                }

                bxOut = new ExtendedBitmap(Width, Height);
            }

            bxOut.Graphics.TextContrast = 0;
            bxOut.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            bxOut.Graphics.Clear(BackColor);
            if (Knockout != null && (KnockoutLocation.X > -1) & (KnockoutLocation.Y > -1))
            {
                var srcRect = new Rectangle(KnockoutLocation, Size);
                var graphics = bxOut.Graphics;
                var image = Knockout.Image;
                var location = new Point(0, 0);
                var bounds = new Rectangle(location, bxOut.Size);
                graphics.DrawImage(image, bounds, srcRect, GraphicsUnit.Pixel);
            }

            checked
            {
                if (!((bxImage == null) | (bxAltImage == null)))
                {
                    if (pAltState | (pToggle & pTogState))
                    {
                        bxOut.Graphics.DrawImageUnscaled(bxAltImage.Bitmap, 0, 0);
                    }
                    else if (myIA != null)
                    {
                        var graphics2 = bxOut.Graphics;
                        Image bitmap = bxImage.Bitmap;
                        var location = new Point(0, 0);
                        var bounds = new Rectangle(location, bxOut.Size);
                        graphics2.DrawImage(bitmap, bounds, 0, 0, bxOut.Size.Width, bxOut.Size.Height,
                            GraphicsUnit.Pixel, myIA);
                    }
                    else
                    {
                        var graphics3 = bxOut.Graphics;
                        Image bitmap2 = bxImage.Bitmap;
                        var location = new Point(0, 0);
                        var bounds = new Rectangle(location, bxOut.Size);
                        graphics3.DrawImage(bitmap2, bounds, 0, 0, bxOut.Size.Width, bxOut.Size.Height,
                            GraphicsUnit.Pixel);
                    }
                }
                else if (pAltState | (pToggle & pTogState))
                {
                    bxOut.Graphics.Clear(Color.LightCoral);
                    var graphics4 = bxOut.Graphics;
                    var red = Pens.Red;
                    var x = 0;
                    var y = 0;
                    var bounds = Bounds;
                    var bounds2 = new Rectangle(x, y, bounds.Width - 1, Bounds.Height - 1);
                    graphics4.DrawRectangle(red, bounds2);
                }
                else
                {
                    bxOut.Graphics.Clear(Color.LavenderBlush);
                    var graphics5 = bxOut.Graphics;
                    var red2 = Pens.Red;
                    var x2 = 0;
                    var y2 = 0;
                    var bounds2 = Bounds;
                    var bounds = new Rectangle(x2, y2, bounds2.Width - 1, Bounds.Height - 1);
                    graphics5.DrawRectangle(red2, bounds);
                }
            }

            var num = Font.GetHeight(bxOut.Graphics) + 2f;
            var rectangleF = new RectangleF(0f, (Height - num) / 2f, Width, num);
            string text;
            if (!pAltState | !pToggle)
                text = pTextOff;
            else
                text = pTextOn;
            var iStr = text;
            var bounds3 = rectangleF;
            var whiteSmoke = Color.WhiteSmoke;
            var outline = Color.FromArgb(192, 0, 0, 0);
            var font = Font;
            var outlineSpace = 1f;
            var graphics6 = bxOut.Graphics;
            graphics6.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            DrawOutlineText(iStr, bounds3, whiteSmoke, outline, font, outlineSpace, ref graphics6);
        }

        // Token: 0x060001BF RID: 447 RVA: 0x00010E10 File Offset: 0x0000F010
        private void DrawOutlineText(string iStr, RectangleF bounds, Color text, Color outline, Font bFont,
            float outlineSpace, ref Graphics target, bool smallMode = false, bool leftAlign = false)
        {
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Near,
                Alignment = leftAlign ? StringAlignment.Near : StringAlignment.Center
            };
            var brush = new SolidBrush(outline);
            var layoutRectangle = bounds;
            var layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width,
                bFont.GetHeight(target));
            layoutRectangle2.X -= outlineSpace;
            if (!smallMode) target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y -= outlineSpace;
            target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X += outlineSpace;
            if (!smallMode) target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X += outlineSpace;
            target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y += outlineSpace;
            if (!smallMode) target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y += outlineSpace;
            target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X -= outlineSpace;
            if (!smallMode) target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X -= outlineSpace;
            target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y -= outlineSpace;
            if (!smallMode) target.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            brush = new SolidBrush(text);
            target.DrawString(iStr, bFont, brush, layoutRectangle, stringFormat);
        }

        // Token: 0x060001C0 RID: 448 RVA: 0x00010FEC File Offset: 0x0000F1EC
        private void ImageButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (!pToggle)
            {
                pAltState = true;
                Redraw();
            }
            else
            {
                pTogState = true;
                Redraw();
            }
        }

        // Token: 0x060001C1 RID: 449 RVA: 0x00011028 File Offset: 0x0000F228
        private void ImageButton_MouseLeave(object sender, EventArgs e)
        {
            if (!pToggle)
            {
                pAltState = false;
                Redraw();
            }
            else
            {
                pTogState = false;
                Redraw();
            }
        }

        // Token: 0x060001C2 RID: 450 RVA: 0x00011064 File Offset: 0x0000F264
        private void ImageButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (!pToggle)
            {
                pAltState = false;
                Redraw();
                if ((e.X >= 0) & (e.Y >= 0) & (e.X <= Width) & (e.Y <= Height)) ButtonClicked?.Invoke();
            }
            else if ((e.X >= 0) & (e.Y >= 0) & (e.X <= Width) & (e.Y <= Height))
            {
                pAltState = !pAltState;
                pTogState = false;
                Redraw();
                ButtonClicked?.Invoke();
            }
        }

        // Token: 0x060001C3 RID: 451 RVA: 0x00011168 File Offset: 0x0000F368
        private void ImageButton_Paint(object sender, PaintEventArgs e)
        {
            if (bxOut == null) Draw();
            if (bxOut != null) e.Graphics.DrawImage(bxOut.Bitmap, 0, 0);
        }

        // Token: 0x060001C4 RID: 452 RVA: 0x000111BC File Offset: 0x0000F3BC
        private void ImageButton_SizeChanged(object sender, EventArgs e)
        {
            if ((Size.Width != 105) | (Size.Height != 22))
            {
                var size = new Size(105, 22);
                Size = size;
            }

            bxOut = new ExtendedBitmap(Width, Height);
            Redraw();
        }
    }
}