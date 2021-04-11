#nullable enable
using System.Collections.Generic;
using System.Windows.Forms;
using mrbBase;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls.pnlGFX2
{
    public partial class ctlPowerViewer : SKControl
    {
        public enum PowerState
        {
            Empty = 0,
            Placed = 1
        }

        public ctlBuildViewer ParentBv;
        public PowerState State = PowerState.Empty;
        public bool Active = false;
        private List<ctlSlotViewer> _slotsGfx;
        private PowerEntry _powerEntry;

        public ctlPowerViewer(PowerEntry? powerEntry, ctlBuildViewer parentBv)
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            ParentBv = parentBv;
            _powerEntry = powerEntry ?? new PowerEntry();
            _slotsGfx = new List<ctlSlotViewer>();

            InitializeComponent();
        }
    }
}