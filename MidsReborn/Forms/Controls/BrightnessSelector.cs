using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class BrightnessSelector : UserControl
    {
        public delegate void PositionChangedHandler(object? sender, float percentage);
        public event PositionChangedHandler? PositionChanged;

        public int CursorWidth { get; set; } = 8;
        public Color CursorColor { get; set; } = Color.FromArgb(85, 170, 255);
        public float Brightness { get; private set; }

        private Rectangle _gradientRectangle;
        private Rectangle _selectorRectangle;

        private readonly Bitmap _gradientBar = MRBResourceLib.Resources.gradientBar;
        private readonly Bitmap _selector = MRBResourceLib.Resources.selector;

        private int _cursorPosition;

        private int CursorPosition
        {
            get => _cursorPosition;
            set
            {
                _cursorPosition = value;
                PositionChanged?.Invoke(this, PointToPercentage(value, _gradientRectangle.Height - 10));
            }
        }
        private bool DragEnabled { get; set; }

        public BrightnessSelector()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Load += OnLoad;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            MouseClick += OnMouseClick;
            PositionChanged += OnPositionChanged;
            InitializeComponent();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            _gradientRectangle = new Rectangle(10, 0, 20, 300);
            _selectorRectangle = new Rectangle(0, 0, 40, 10)
            {
                Y = PercentageToPoint(Brightness, _gradientRectangle.Height - 10)
            };
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && (e.X < _selectorRectangle.Left || e.X > _selectorRectangle.Right || e.Y < _selectorRectangle.Top || e.Y > _selectorRectangle.Bottom)) return;
            if (e.Y > _gradientRectangle.Bottom - 10 || e.Y < _gradientRectangle.Top) return;
            CursorPosition = e.Y;
        }

        private void OnPositionChanged(object? sender, float percentage)
        {
            Brightness = percentage;
            Invalidate();
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            DragEnabled = false;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && (e.X < _selectorRectangle.Left || e.X > _selectorRectangle.Right || e.Y < _selectorRectangle.Top || e.Y > _selectorRectangle.Bottom)) return;
            DragEnabled = true;
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && (e.X < _selectorRectangle.Left || e.X > _selectorRectangle.Right || e.Y < _selectorRectangle.Top || e.Y > _selectorRectangle.Bottom)) return;
            if (!DragEnabled) return;
            if (e.Y > _gradientRectangle.Bottom - 10 || e.Y < _gradientRectangle.Top) return;
            CursorPosition = e.Y;
        }

        private static float PointToPercentage(int y, int height)
        {
            return (float)y / height * 100f;
        }

        private static int PercentageToPoint(float percentage, int height)
        {
            return (int)(percentage * height / 100.0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            // Set graphics quality
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Draw gradient
            e.Graphics.DrawImage(_gradientBar, _gradientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Set graphics quality
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Draw selector
            _selectorRectangle.Y = CursorPosition;
            e.Graphics.DrawImage(_selector, _selectorRectangle);
        }
    }
}
