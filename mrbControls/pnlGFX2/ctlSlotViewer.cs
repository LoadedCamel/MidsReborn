using System;
using System.Windows.Forms;
using mrbBase;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls.pnlGFX2
{
    public partial class ctlSlotViewer : SKControl
    {
        public enum BorderType
        {
            Auto = 0,
            Special = 1,
            Io = 2,
            Catalyzed = 3
        }

        public ctlBuildViewer ParentBv;
        public ctlPowerViewer ParentPv;
        private clsDrawX _drawX;
        private SlotEntry _slot;
        private bool _useAlternate = false;
        private SKImageInfo _SKImageInfo;
        private SKSurface _SKSurface;
        private SKCanvas _SKCanvas;
        private BorderType _BorderTypeOverride;
        private bool _MouseOver;

        public BorderType BorderTypeOverride
        {
            get => _BorderTypeOverride;
            set => _BorderTypeOverride = value;
        }

        public bool MouseOver
        {
            get => _MouseOver;
            set => _MouseOver = value;
        }

        public ctlSlotViewer(clsDrawX drawX)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            InitializeComponent();
            _SKImageInfo = new SKImageInfo(30, 30, SKColorType.Rgba8888);
            _SKSurface = SKSurface.Create(_SKImageInfo);
            _SKCanvas = _SKSurface.Canvas;
            _SKCanvas.Clear();
            _BorderTypeOverride = BorderType.Auto;
            _MouseOver = false;
            _drawX = drawX;
        }

        // https://social.msdn.microsoft.com/Forums/en-US/f43ed61e-6e15-41f8-801d-1e908810eed2/transparent-picturebox?forum=winforms
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= 0x20; // WS_EX_TRANSPARENT
                
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            RecreateHandle();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            RecreateHandle();
        }

        private SKColorFilter GrayscaleFilter()
        {
            return SKColorFilter.CreateColorMatrix(new []
            {
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0,     0,     0,     1, 0
            });
        }

        private void Draw()
        {
            if (_MouseOver)
            {
                _SKCanvas.Clear();
                _SKCanvas.DrawBitmap(SkiaHelper.ToSKBitmap(_drawX.bxNewSlot.Bitmap), new SKPoint(0, 0));

                return;
            }
            
            var enhIdx = (_useAlternate ? _slot.FlippedEnhancement : _slot.Enhancement).Enh;
            var imageIdx = enhIdx == -1
                ? -1
                : DatabaseAPI.Database.Enhancements[enhIdx].ImageIdx;

            if (imageIdx == -1)
            {
                //var grayscaleEmptySlot = new Bitmap(Path.Combine(I9Gfx.ImagePath(), "Newslot.png"));
                //_bxBuffer.Graphics.DrawImage(grayscaleEmptySlot, new Rectangle(0, 0, grayscaleEmptySlot.Width, grayscaleEmptySlot.Height),
                //    0, 0,
                //    grayscaleEmptySlot.Width, grayscaleEmptySlot.Height,
                //    GraphicsUnit.Pixel,
                //    GrayscaleIa());

                return;
            }

            // _bxBuffer.Graphics.DrawImage(I9Gfx.Enhancements[imageIdx], new PointF(0, 0));
        }

        public void Flip()
        {

        }

        private void ctlSlotViewer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // Draw
        }
    }
}