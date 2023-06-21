using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Mids_Reborn.Core.Base.IO_Classes;
using Mids_Reborn.Forms.Controls;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms
{
    public sealed partial class frmInitializing : PerPixelAlpha, IMessager
    {
        public event EventHandler? LoadingStarted;
        private const int BorderRadius = 20;
        private const int BorderSize = 2;
        private readonly Color _borderColor = Color.Black;
        private bool _webViewReady;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= 0x20000;
                return cp;
            }
        }

        public frmInitializing()
        {
            Load += FrmInitializing_Load;
            Closing += FrmInitializing_Closing;
            InitializeComponent();
            Padding = new Padding(BorderSize);
            BackColor = _borderColor;
            panel1.Paint += Panel1_Paint;
            Activated += FrmInitializing_Activated;
            webView21.NavigationCompleted += WebView21OnNavigationCompleted;
        }
        
        private void WebView21OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;
            _webViewReady = true;
            LoadingStarted?.Invoke(this, e);
        }

        private void FrmInitializing_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            AnimateWindow(Handle, 500, AnimationFlags.Activate | AnimationFlags.Blend | AnimationFlags.Hide);
        }

        private async void FrmInitializing_Load(object? sender, EventArgs e)
        {
            SetTopMost(false);
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets.mrb", $"{Path.Combine(AppContext.BaseDirectory)}", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.NavigateToString(Html);
        }

        private const string Html = @"<!DOCTYPE html>
<html>
<head>
<style type=""text/css"">
body, html {
    width: 100%;
    height: 100%;
    overflow: hidden;
    background-color: #000;
}

body {
    background-image: url(""http://appassets.mrb/images/MRBLoading.gif"");
    background-repeat: no-repeat;
    background-size: auto 100%;
    background-position: center;
}

div#message {
    margin: 12px auto auto auto;
    text-align: center;
    color: Gainsboro;
    font-family: Sans-Serif;
    font-size: 11pt;
    font-weight: bold;
    text-shadow: 0 0 3px #000, 0 0 1px #000, 0 0 1px #000;
    transition: all 0.5s ease-out;
}
</style>
</head>
<body>
    <div id=""message""></div>
</body>
</html>";

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public bool LoadingComplete { get; set; }

        public void SetMessage(string text)
        {
            if (Label1.InvokeRequired)
            {
                void Action()
                {
                    SetMessage(text);
                }

                Invoke(Action);
            }
            else
            {
                if (Label1.Text == text)
                {
                    return;
                }

                Label1.Text = text;
                Label1.Refresh();
                if (!_webViewReady)
                {
                    return;
                }

                webView21.CoreWebView2.ExecuteScriptAsync($@"var messageDiv = document.querySelector(""div#message"");
messageDiv.style.opacity = 0;
setTimeout(() => {{ messageDiv.textContent = ""{text.Replace("\"", "\\\"")}""; }}, 100);
setTimeout(() => {{ messageDiv.style.opacity = 1; }}, 100);");
                //Refresh();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            //-> SMOOTH OUTER BORDER
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var rectForm = ClientRectangle;
            var mWidth = rectForm.Width / 2;
            var mHeight = rectForm.Height / 2;
            var fbColors = GetFormBoundsColors();

            //Top Left
            DrawPath(rectForm, e.Graphics, fbColors.TopLeftColor);

            //Top Right
            var rectTopRight = new Rectangle(mWidth, rectForm.Y, mWidth, mHeight);
            DrawPath(rectTopRight, e.Graphics, fbColors.TopRightColor);

            //Bottom Left
            var rectBottomLeft = new Rectangle(rectForm.X, rectForm.X + mHeight, mWidth, mHeight);
            DrawPath(rectBottomLeft, e.Graphics, fbColors.BottomLeftColor);

            //Bottom Right
            var rectBottomRight = new Rectangle(mWidth, rectForm.Y + mHeight, mWidth, mHeight);
            DrawPath(rectBottomRight, e.Graphics, fbColors.BottomRightColor);

            //-> SET ROUNDED REGION AND BORDER
            FormRegionAndBorder(this, BorderRadius, e.Graphics, _borderColor, BorderSize);
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            var path = new GraphicsPath();
            var curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColor, float borderSize)
        {
            if (WindowState == FormWindowState.Minimized) return;
            using var roundPath = GetRoundedPath(form.ClientRectangle, radius);
            using var penBorder = new Pen(borderColor, borderSize);
            using var transform = new Matrix();
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            form.Region = new Region(roundPath);
            if (!(borderSize >= 1)) return;
            var rect = form.ClientRectangle;
            var scaleX = 1.0F - ((borderSize + 1) / rect.Width);
            var scaleY = 1.0F - ((borderSize + 1) / rect.Height);
            transform.Scale(scaleX, scaleY);
            transform.Translate(borderSize / 1.6F, borderSize / 1.6F);
            graph.Transform = transform;
            graph.DrawPath(penBorder, roundPath);
        }

        private static void ControlRegionAndBorder(Control control, float radius, Graphics graph, Color borderColor)
        {
            using var roundPath = GetRoundedPath(control.ClientRectangle, radius);
            using var penBorder = new Pen(borderColor, 1);
            graph.SmoothingMode = SmoothingMode.HighQuality;
            control.Region = new Region(roundPath);
            graph.DrawPath(penBorder, roundPath);
        }

        private static void DrawPath(Rectangle rect, Graphics graph, Color color)
        {
            using var roundPath = GetRoundedPath(rect, BorderRadius);
            using var penBorder = new Pen(color, 3);
            graph.DrawPath(penBorder, roundPath);
        }

        private struct FormBoundsColors
        {
            public Color TopLeftColor;
            public Color TopRightColor;
            public Color BottomLeftColor;
            public Color BottomRightColor;
        }
        private FormBoundsColors GetFormBoundsColors()
        {
            var fbColor = new FormBoundsColors();
            using var bmp = new Bitmap(1, 1);
            using var graph = Graphics.FromImage(bmp);
            var rectBmp = new Rectangle(0, 0, 1, 1)
            {
                //Top Left
                X = Bounds.X - 1,
                Y = Bounds.Y
            };

            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.TopLeftColor = bmp.GetPixel(0, 0);

            //Top Right
            rectBmp.X = Bounds.Right;
            rectBmp.Y = Bounds.Y;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.TopRightColor = bmp.GetPixel(0, 0);

            //Bottom Left
            rectBmp.X = Bounds.X;
            rectBmp.Y = Bounds.Bottom;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.BottomLeftColor = bmp.GetPixel(0, 0);

            //Bottom Right
            rectBmp.X = Bounds.Right;
            rectBmp.Y = Bounds.Bottom;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.BottomRightColor = bmp.GetPixel(0, 0);
            return fbColor;
        }

        private void Panel1_Paint(object? sender, PaintEventArgs e)
        {
            OnPaint(e);
            ControlRegionAndBorder(panel1, BorderRadius - BorderSize / 2f, e.Graphics, _borderColor);
        }

        private void FrmInitializing_Activated(object? sender, EventArgs e)
        {
            Invalidate();
        }
    }

    public sealed class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Paint background with underlying graphics from other controls
            base.OnPaintBackground(e);
            var g = e.Graphics;

            if (Parent == null) return;

            // Take each control in turn
            var index = Parent.Controls.GetChildIndex(this);
            for (var i = Parent.Controls.Count - 1; i > index; i--)
            {
                var c = Parent.Controls[i];

                // Check it's visible and overlaps this control
                if (!c.Bounds.IntersectsWith(Bounds) || !c.Visible) continue;

                // Load appearance of underlying control and redraw it on this background
                var bmp = new Bitmap(c.Width, c.Height, g);
                c.DrawToBitmap(bmp, c.ClientRectangle);
                g.TranslateTransform(c.Left - Left, c.Top - Top);
                g.DrawImageUnscaled(bmp, Point.Empty);
                g.TranslateTransform(Left - c.Left, Top - c.Top);
                bmp.Dispose();
            }
        }
    }
}