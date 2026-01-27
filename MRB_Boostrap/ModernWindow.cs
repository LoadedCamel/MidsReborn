using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static MRB_Boostrap.Interop.Win32;

namespace MRB_Boostrap
{
    public sealed class ModernWindow : NativeWindow, IDisposable
    {
        private static ModernWindow? _instance;
        public static ModernWindow? Instance => _instance;

        private const int WsPopup = unchecked((int)0x80000000);
        private const int WsExLayered = 0x00080000;
        private const int LwaAlpha = 0x2;

        private const int WmPaint = 0x000F;
        private const int WmDestroy = 0x0002;
        private const int WmTimer = 0x0113;
        private const int WmMouseMove = 0x0200;
        private const int WmLButtonDown = 0x0201;
        private const int TimerId = 1;

        private const int WmUiUpdateStatus = 0x0400 + 1;
        private const int WmUiUpdateVersion = 0x0400 + 2;
        private const int WmUiUpdateProgress = 0x0400 + 3;
        private const int WmUiShowProgressBar = 0x0400 + 4;
        private const int WmUiTriggerFailure = 0x0400 + 5;
        private const int WmUiTriggerCancel = 0x0400 + 6;
        private const int WmUiTriggerRollback = 0x0400 + 7;
        private const int WmUiStartCleanup = 0x0400 + 8;
        private const int WmUiCloseWindow = 0x0400 + 9;

        private readonly IntPtr _hWnd;

        private readonly Bitmap _logoBitmap;
        private readonly Bitmap? _plasmaBitmap;
        private bool _cancelEnabled = true;
        private bool _cancelHover;

        private Rectangle _cancelRect = new(140, 330, 120, 40);
        private string _currentStatus = "Initializing...";
        private string _versionInfo = "v1.0.0";
        private float _currentProgress;
        private bool _showProgressBar = true;

        private System.Windows.Forms.Timer? _refreshTimer;

        public ModernWindow()
        {
            _instance = this;

            var cp = new CreateParams
            {
                Caption = string.Empty,
                Style = WsPopup,
                ExStyle = WsExLayered,
                Width = 400,
                Height = 400,
                X = (Screen.PrimaryScreen!.WorkingArea.Width - 400) / 2,
                Y = (Screen.PrimaryScreen.WorkingArea.Height - 400) / 2
            };

            CreateHandle(cp);
            _hWnd = Handle;

            var margins = new Margins()
            {
                cxLeftWidth = 1,
                cxRightWidth = 1,
                cyTopHeight = 1,
                cyBottomHeight = 1
            };
            DwmExtendFrameIntoClientArea(_hWnd, ref margins);

            SetLayeredWindowAttributes(_hWnd, 0, 255, LwaAlpha);
            var region = CreateRoundRectRgn(0, 0, 400, 400, 20, 20);
            SetWindowRgn(_hWnd, region, true);

            _logoBitmap = LoadEmbeddedBitmap("mids_logo.png");
            _plasmaBitmap = BoostBrightness(LoadEmbeddedBitmap("plasma.png"), 1.2f);

            // Start plasma animation
            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 16 // ~60 FPS
            };
            _refreshTimer.Tick += (_, _) =>
            {
                InvalidateRect(_hWnd, IntPtr.Zero, false);
            };
            _refreshTimer.Start();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WmPaint:
                    PaintWindow();
                    return;

                case WmTimer:
                    if ((int)m.WParam == TimerId)
                    {
                        InvalidateRect(_hWnd, IntPtr.Zero, false);
                    }
                    return;

                case WmMouseMove:
                    var pt = new Point((int)(m.LParam.ToInt64() & 0xFFFF), (int)((m.LParam.ToInt64() >> 16) & 0xFFFF));
                    var nowHover = _cancelRect.Contains(pt);
                    if (_cancelEnabled && nowHover != _cancelHover)
                    {
                        _cancelHover = nowHover;
                        InvalidateRect(_hWnd, IntPtr.Zero, false);
                    }
                    return;

                case WmLButtonDown:
                    var click = new Point((int)(m.LParam.ToInt64() & 0xFFFF), (int)((m.LParam.ToInt64() >> 16) & 0xFFFF));
                    if (_cancelRect.Contains(click) && _cancelEnabled)
                    {
                        _cancelEnabled = false;
                        _cancelHover = false;
                        _currentStatus = "Cancelling update...";
                        InvalidateRect(_hWnd, IntPtr.Zero, true);
                    }
                    else
                    {
                        ReleaseCapture();
                        SendMessage(_hWnd, 0xA1, new IntPtr(2), IntPtr.Zero); // HTCAPTION
                    }
                    return;

                case WmDestroy:
                    KillTimer(_hWnd, TimerId);
                    PostQuitMessage(0);
                    return;

                case WmUiUpdateStatus:
                    MarshalPtrToStringFree(m.LParam, out _currentStatus);
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    return;

                case WmUiUpdateVersion:
                    MarshalPtrToStringFree(m.LParam, out _versionInfo);
                    InvalidateRect(_hWnd, IntPtr.Zero, false);
                    return;

                case WmUiUpdateProgress:
                    if (m.WParam != IntPtr.Zero)
                    {
                        _currentProgress = Math.Clamp(Marshal.PtrToStructure<float>(m.WParam), 0f, 1f);
                        Marshal.FreeHGlobal(m.WParam);
                        InvalidateRect(_hWnd, IntPtr.Zero, false);
                    }
                    return;

                case WmUiShowProgressBar:
                    _showProgressBar = m.WParam != IntPtr.Zero;
                    InvalidateRect(_hWnd, IntPtr.Zero, false);
                    return;

                case WmUiTriggerFailure:
                    _currentStatus = "Update Failed.";
                    _showProgressBar = false;
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    return;

                case WmUiTriggerCancel:
                    _currentStatus = "Cancelling update...";
                    _showProgressBar = false;
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    return;

                case WmUiTriggerRollback:
                    _currentStatus = "Rolling Back...";
                    _currentProgress = 0f;
                    _showProgressBar = true;
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    return;

                case WmUiStartCleanup:
                    _currentStatus = "Cleaning up...";
                    _showProgressBar = false;
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    UpdateWindow(_hWnd);
                    // Add cleanup logic here
                    _currentStatus = "Cleanup complete.";
                    InvalidateRect(_hWnd, IntPtr.Zero, true);
                    return;

                case WmUiCloseWindow:
                    DestroyHandle();
                    return;
            }

            base.WndProc(ref m);
        }

        private void MarshalPtrToStringFree(IntPtr ptr, out string output)
        {
            output = Marshal.PtrToStringUni(ptr) ?? "";
            Marshal.FreeHGlobal(ptr);
        }

        private void PaintWindow()
        {
            using var g = Graphics.FromHwnd(_hWnd);
            using var buffer = new Bitmap(400, 400);
            using var gBuffer = Graphics.FromImage(buffer);
            gBuffer.SmoothingMode = SmoothingMode.HighQuality;
            gBuffer.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gBuffer.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gBuffer.CompositingQuality = CompositingQuality.HighQuality;

            // === Layout Constants ===
            const int width = 400;
            const int height = 400;

            const int logoWidthTarget = 220;
            const int logoTop = 5;

            const int statusTop = 100;
            const int donutCenterY = 215;

            const int versionTop = 305;
            const int cancelTop = 340;

            // === Background ===
            using (var bgBrush = new LinearGradientBrush(
                new Rectangle(0, 0, width, height),
                Color.FromArgb(5, 15, 30),
                Color.Black,
                LinearGradientMode.Vertical))
            {
                gBuffer.FillRectangle(bgBrush, new Rectangle(0, 0, width, height));
            }

            // === Border ===
            using (var borderPen = new Pen(Color.FromArgb(150, 0, 150, 255), 2))
            using (var borderPath = new GraphicsPath())
            {
                borderPath.StartFigure();
                borderPath.AddArc(2, 2, 20, 20, 180, 90);
                borderPath.AddLine(12, 2, 388, 2);
                borderPath.AddArc(378, 2, 20, 20, 270, 90);
                borderPath.AddLine(398, 12, 398, 388);
                borderPath.AddArc(378, 378, 20, 20, 0, 90);
                borderPath.AddLine(388, 398, 12, 398);
                borderPath.AddArc(2, 378, 20, 20, 90, 90);
                borderPath.AddLine(2, 388, 2, 12);
                borderPath.CloseFigure();
                gBuffer.DrawPath(borderPen, borderPath);
            }

            // === Logo ===
            {
                float scale = logoWidthTarget / (float)_logoBitmap.Width;
                int drawW = (int)(_logoBitmap.Width * scale);
                int drawH = (int)(_logoBitmap.Height * scale);
                int logoLeft = (width - drawW) / 2;
                gBuffer.DrawImage(_logoBitmap, logoLeft, logoTop, drawW, drawH);
            }

            // === Status Text ===
            using (var statusFont = new Font("Segoe UI", 16))
            using (var statusBrush = new SolidBrush(Color.FromArgb(220, 210, 230, 255)))
            {
                gBuffer.DrawString(_currentStatus, statusFont, statusBrush,
                    new RectangleF(0, statusTop, width, 30),
                    new StringFormat { Alignment = StringAlignment.Center });
            }

            // === Progress Donut ===
            if (_showProgressBar && _plasmaBitmap is { Width: > 0, Height: > 0 })
            {
                float cx = width / 2f;
                float cy = donutCenterY;
                float outer = 70f;
                float inner = 55f;
                float sweep = 360f * _currentProgress;

                // === Background Ring
                using var bgPen = new Pen(Color.FromArgb(60, 100, 100, 120), outer - inner);
                bgPen.StartCap = LineCap.Round;
                bgPen.EndCap = LineCap.Round;
                gBuffer.DrawArc(bgPen, cx - outer, cy - outer, outer * 2, outer * 2, 0, 360);

                // === Sinusoidal plasma translation
                float t = Environment.TickCount / 1000f;
                float x = MathF.Sin(t * 1.5f) * 30f;
                float y = MathF.Cos(t * 0.75f) * 20f;

                using var brush = new TextureBrush(_plasmaBitmap, WrapMode.TileFlipXY);
                brush.ScaleTransform(0.35f, 0.35f);
                brush.TranslateTransform(x, y);

                using var pen = new Pen(brush, outer - inner);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                gBuffer.DrawArc(pen, cx - outer, cy - outer, outer * 2, outer * 2, -90, sweep);

                // === Centered % text
                using var pFont = new Font("Segoe UI", 18, FontStyle.Bold);
                using var pBrush = new SolidBrush(Color.FromArgb(0, 220, 255));
                gBuffer.DrawString($"{_currentProgress * 100:0}%",
                    pFont, pBrush, new RectangleF(0, cy - 20, width, 40),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // === Version Info ===
            using (var vFont = new Font("Segoe UI", 14))
            using (var vBrush = new SolidBrush(Color.FromArgb(200, 210, 240, 255)))
            {
                gBuffer.DrawString(_versionInfo, vFont, vBrush,
                    new RectangleF(0, versionTop, width, 20),
                    new StringFormat { Alignment = StringAlignment.Center });
            }

            // === Cancel Button ===
            _cancelRect = new Rectangle(140, cancelTop, 120, 40);
            using var path = new GraphicsPath();
            path.AddArc(_cancelRect.X, _cancelRect.Y, 20, 40, 90, 180);
            path.AddArc(_cancelRect.Right - 20, _cancelRect.Y, 20, 40, 270, 180);
            path.CloseFigure();

            var btnColor = _cancelEnabled
                ? (_cancelHover ? Color.FromArgb(30, 130, 255) : Color.FromArgb(40, 70, 180))
                : Color.FromArgb(100, 100, 100);

            using var btnBrush = new LinearGradientBrush(_cancelRect, btnColor, Color.FromArgb(30, 60, 120), LinearGradientMode.Vertical);
            gBuffer.FillPath(btnBrush, path);

            using var btnPen = new Pen(Color.FromArgb(0, 180, 255), 2);
            gBuffer.DrawPath(btnPen, path);

            using var bFont = new Font("Segoe UI", 14, FontStyle.Bold);
            using var bText = new SolidBrush(Color.White);
            gBuffer.DrawString("Cancel", bFont, bText, _cancelRect,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // === Blit Final ===
            g.DrawImageUnscaled(buffer, 0, 0);
        }

        private static Bitmap LoadEmbeddedBitmap(string partialName, bool applyBrightness = false)
        {
            var asm = typeof(ModernWindow).Assembly;

            string? resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith(partialName, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                throw new InvalidOperationException($"Resource '{partialName}' not found.");
            }

            using Stream? stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new InvalidOperationException($"Failed to load resource stream: {resourceName}");

            using MemoryStream ms = new();
            stream.CopyTo(ms);
            ms.Position = 0;

            // Load original bitmap
            using Bitmap original = new Bitmap(ms);

            // Convert to 32bpp ARGB
            Bitmap converted = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            using Graphics g = Graphics.FromImage(converted);
            if (applyBrightness)
            {
                ColorMatrix cm = new ColorMatrix(new float[][]
                {
                    [2.5f, 0, 0, 0, 0],
                    [0, 2.5f, 0, 0, 0],
                    [0, 0, 2.5f, 0, 0],
                    [0, 0, 0, 1f, 0],
                    [0, 0, 0, 0, 1f]
                });

                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix(cm);

                g.DrawImage(original, new Rectangle(0, 0, converted.Width, converted.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attr);
            }
            else
            {
                g.DrawImage(original, 0, 0, converted.Width, converted.Height);
            }

            return converted;
        }

        private static Bitmap BoostBrightness(Bitmap original, float multiplier)
        {
            var cm = new ColorMatrix(new float[][]
            {
                [multiplier, 0, 0, 0, 0],
                [0, multiplier, 0, 0, 0],
                [0, 0, multiplier, 0, 0],
                [0, 0, 0, 1, 0],
                [0, 0, 0, 0, 1]
            });

            var imgAttr = new ImageAttributes();
            imgAttr.SetColorMatrix(cm);

            Bitmap bright = new(original.Width, original.Height);
            using var g = Graphics.FromImage(bright);
            g.DrawImage(original, new Rectangle(0, 0, bright.Width, bright.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imgAttr);
            return bright;
        }


        public void Show()
        {
            ShowWindow(_hWnd, 5); // SW_SHOW
            UpdateWindow(_hWnd);

            while (GetMessage(out var msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            Environment.Exit(0);
        }

        public void Dispose()
        {
            KillTimer(_hWnd, TimerId);
            if (Handle != IntPtr.Zero)
                DestroyHandle();
        }

        #region Public Static Messaging API

        public void PostUpdateStatus(string status)
        {
            PostString(WmUiUpdateStatus, status);
        }

        public void PostUpdateVersion(string version)
        {
            PostString(WmUiUpdateVersion, version);
        }

        public void PostUpdateProgress(float percent)
        {
            var ptr = Marshal.AllocHGlobal(sizeof(float));
            Marshal.StructureToPtr(percent, ptr, false);
            PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiUpdateProgress, ptr, IntPtr.Zero);
        }

        public void PostShowProgressBar(bool show)
        {
            PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiShowProgressBar, show ? new IntPtr(1) : IntPtr.Zero, IntPtr.Zero);
        }

        public void PostTriggerFailure() => PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiTriggerFailure, IntPtr.Zero, IntPtr.Zero);
        public void PostTriggerCancel() => PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiTriggerCancel, IntPtr.Zero, IntPtr.Zero);
        public void PostTriggerRollback() => PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiTriggerRollback, IntPtr.Zero, IntPtr.Zero);
        public void PostStartCleanup() => PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiStartCleanup, IntPtr.Zero, IntPtr.Zero);
        public void PostCloseWindow() => PostMessage(_instance?._hWnd ?? IntPtr.Zero, WmUiCloseWindow, IntPtr.Zero, IntPtr.Zero);

        private static void PostString(int msg, string value)
        {
            if (_instance == null) return;
            var ptr = Marshal.StringToHGlobalUni(value);
            PostMessage(_instance._hWnd, msg, IntPtr.Zero, ptr);
        }

        #endregion
    }
}
