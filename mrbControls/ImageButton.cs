using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using mrbBase.Base.Display;

namespace mrbControls
{
    [DefaultEvent("ButtonClicked")]
    [DesignerGenerated]
    public class ImageButton : UserControl
    {
        public delegate void ButtonClickedEventHandler();

        private ExtendedBitmap bxAltImage;

        private ExtendedBitmap bxImage;

        private ExtendedBitmap bxOut;

        private PictureBox Knockout;

        private Point KnockoutLocation;

        private ImageAttributes myIA;

        private bool pAltState;

        private string pTextOff;

        private string pTextOn;

        private bool pToggle;

        private bool pTogState;

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

        public string TextOff
        {
            get => pTextOff;
            set
            {
                pTextOff = value;
                Redraw();
            }
        }

        public string TextOn
        {
            get => pTextOn;
            set
            {
                pTextOn = value;
                Redraw();
            }
        }

        public bool Toggle
        {
            get => pToggle;
            set
            {
                pToggle = value;
                Redraw();
            }
        }

        public bool Checked
        {
            get => pAltState;
            set
            {
                pAltState = value;
                Redraw();
            }
        }

        public ImageAttributes IA
        {
            set => myIA = value;
        }

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

        
        public PictureBox KnockoutPB
        {
            set
            {
                Knockout = value;
                Redraw();
            }
        }

        
        public Point KnockoutLocationPoint
        {
            get => KnockoutLocation;
            set
            {
                KnockoutLocation = value;
                if (Knockout != null) Redraw();
            }
        }

        
        private IContainer Components { get; }

        
        public event ButtonClickedEventHandler ButtonClicked;

        
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

        private void Redraw()
        {
            if (IsDisposed) return;

            Draw();
            if (bxOut.Bitmap != null) CreateGraphics().DrawImage(bxOut.Bitmap, 0, 0);
        }

        public void SetImages(ImageAttributes ia, Bitmap imageOff, Bitmap imageOn)
        {
            IA = ia;
            ImageOff = imageOff;
            ImageOn = imageOn;
            Redraw();
        }

        private void ImageButton_BackColorChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void ImageButton_FontChanged(object sender, EventArgs e)
        {
            Redraw();
        }

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
                        graphics2.DrawImage(bitmap, bounds, 0, 0, bxOut.Size.Width, bxOut.Size.Height, GraphicsUnit.Pixel, myIA);
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

        private void DrawOutlineText(string iStr, RectangleF bounds, Color text, Color outline, Font bFont, float outlineSpace, ref Graphics target, bool smallMode = false, bool leftAlign = false)
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

        private void ImageButton_Paint(object sender, PaintEventArgs e)
        {
            if (bxOut == null) Draw();
            if (bxOut != null) e.Graphics.DrawImage(bxOut.Bitmap, 0, 0);
        }

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