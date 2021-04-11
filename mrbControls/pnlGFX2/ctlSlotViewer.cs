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
            Regular = 2,
            Io = 3,
            Attuned = 4
        }

        public ctlBuildViewer ParentBv;
        public ctlPowerViewer ParentPv;
        private clsDrawX _drawX;
        private SlotEntry _slot;
        private bool _useAlternate = false;
        private SKImageInfo _SKImageInfo;
        private SKSurface _SKSurface;
        private SKCanvas _SKCanvas;

        public BorderType BorderTypeOverride { get; set; }

        public bool MouseOver { get; set; }

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
            BorderTypeOverride = BorderType.Auto;
            MouseOver = false;
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

        private SKColorFilter GrayscaleFilter() =>
            SKColorFilter.CreateColorMatrix(new []
            {
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0.21f, 0.72f, 0.07f, 0, 0,
                0,     0,     0,     1, 0
            });

        private void Draw()
        {
            _SKCanvas.Clear();

            var enhIdx = (_useAlternate ? _slot.FlippedEnhancement : _slot.Enhancement).Enh;
            var enhGrade = (_useAlternate ? _slot.FlippedEnhancement : _slot.Enhancement).Grade;
            var imageIdx = enhIdx == -1
                ? -1
                : DatabaseAPI.Database.Enhancements[enhIdx].ImageIdx;

            if (imageIdx == -1)
            {
                if (MouseOver)
                {
                    _SKCanvas.DrawBitmap(SkiaHelper.ToSKBitmap(_drawX.bxNewSlot.Bitmap), new SKPoint(0, 0));
                }
                else
                {
                    using var paint = new SKPaint {ColorFilter = GrayscaleFilter()};
                    _SKCanvas.DrawBitmap(SkiaHelper.ToSKBitmap(_drawX.bxNewSlot.Bitmap), _SKImageInfo.Rect, paint);
                }

                return;
            }

            // Border
            // Draw first because overlays are opaque
            var borderType = BorderTypeOverride switch
            {
                BorderType.Regular => I9Gfx.ToGfxGrade(Enums.eType.Normal, enhGrade),
                BorderType.Special => I9Gfx.ToGfxGrade(Enums.eType.SpecialO),
                BorderType.Io => I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[enhIdx].nIDSet >= 0
                    ? Enums.eType.SetO
                    : Enums.eType.InventO),
                BorderType.Attuned => I9Gfx.ToGfxGrade(Enums.eType.SetO),
                _ => I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[enhIdx].TypeID, enhGrade)
            };
            var isAttuned = BorderTypeOverride == BorderType.Attuned || DatabaseAPI.EnhIsNaturallyAttuned(enhIdx);
            var isSuperior = DatabaseAPI.EnhIsSuperior(enhIdx);
            
            // Attuned enhancement border is never loaded, and is not part of the enums.
            // Consider for now enhancement image contains its own border.
            if (!isAttuned)
            {
                _SKCanvas.DrawBitmap(SkiaHelper.ToSKBitmap(I9Gfx.Borders.Bitmap),
                    SkiaHelper.ToSKRect(I9Gfx.GetOverlayRect(borderType)), _SKImageInfo.Rect);
            }

            // Enhancement
            _SKCanvas.DrawBitmap(SkiaHelper.ToSKBitmap(I9Gfx.Enhancements[imageIdx]), new SKPoint(0, 0));
        }

        public void Flip()
        {

        }

        private void ctlSlotViewer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Draw();
        }

        private void ctlSlotViewer_MouseEnter(object sender, EventArgs e)
        {
            MouseOver = true;
            var enhIdx = (_useAlternate ? _slot.FlippedEnhancement : _slot.Enhancement).Enh;

            if (enhIdx == -1) Invalidate();
        }

        private void ctlSlotViewer_MouseLeave(object sender, EventArgs e)
        {
            MouseOver = false;
            var enhIdx = (_useAlternate ? _slot.FlippedEnhancement : _slot.Enhancement).Enh;

            if (enhIdx == -1) Invalidate();
        }
    }
}