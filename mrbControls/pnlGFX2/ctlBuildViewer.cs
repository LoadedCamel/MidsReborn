using System;
using System.Collections.Generic;
using System.Windows.Forms;
using mrbBase;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace mrbControls.pnlGFX2
{
    public class PowerChangedEventArgs : EventArgs
    {
        public enum PowerChange
        {
            Removed = 0,
            Added = 1,
            Moved = 2
        }

        public int PowerIndex;
        public PowerEntry Power;
        public PowerChange PowerStatus;
    }

    public class SlotChangedEventArgs : EventArgs
    {
        public enum SlotChange
        {
            Removed = 0,
            Added = 1,
            Flipped = 2,
            Hovered = 3,
            Mouseout = 4
        }

        public int PowerIndex;
        public int SlotIndex;
        public SlotEntry Slot;
        public SlotChange SlotStatus;
    }
    
    public partial class ctlBuildViewer : SKControl
    {
        public int Columns;
        public int InherentsDirection;

        private List<PowerEntry> _mainPowers;
        private List<PowerEntry> _inherents;
        private List<int> _powerLevels; // From NLevel.mhd
        private float _scale;

        private List<ctlPowerViewer> _powersGfxList;

        public event ScrollEventHandler ScrollEvent;
        public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);
        
        public event PowerClickEventHandler PowerClickEvent;
        public delegate void PowerClickEventHandler(object sender, MouseEventArgs e);
        public event PowerHoverEventHandler PowerHoverEvent;
        public delegate void PowerHoverEventHandler(object sender, MouseEventArgs e);
        public event PowerMouseoutEventHandler PowerMouseoutEvent;
        public delegate void PowerMouseoutEventHandler(object sender, MouseEventArgs e);
        
        public event SlotClickEventHandler SlotClickEvent;
        public delegate void SlotClickEventHandler(object sender, MouseEventArgs e);
        public event SlotHoverEventHandler SlotHoverEvent;
        public delegate void SlotHoverEventHandler(object sender, MouseEventArgs e);
        public event SlotMouseoutEventHandler SlotMouseoutEvent;
        public delegate void SlotMouseoutEventHandler(object sender, MouseEventArgs e);

        public event PowerChangedEventHandler PowerChanged;
        public delegate void PowerChangedEventHandler(object sender, PowerChangedEventArgs e);
        public event SlotChangedEventHandler SlotChanged;
        public delegate void SlotChangedEventHandler(object sender, SlotChangedEventArgs e);

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                // ResizeToScale(_scale);
            }
        }

        public ctlBuildViewer()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            InitializeComponent();
        }

        #region Event from sub-controls retransmitters
        // Slots
        private void SlotClickEvent_Emit(object sender, MouseEventArgs e)
        {
            SlotClickEvent?.Invoke(sender, e);
        }

        private void SlotHoverEvent_Emit(object sender, MouseEventArgs e)
        {
            SlotHoverEvent?.Invoke(sender, e);
        }

        private void SlotMouseoutEvent_Emit(object sender, MouseEventArgs e)
        {
            SlotMouseoutEvent?.Invoke(sender, e);
        }

        // Powers
        private void PowerClickEvent_Emit(object sender, MouseEventArgs e)
        {
            PowerClickEvent?.Invoke(sender, e);
        }

        private void PowerHoverEvent_Emit(object sender, MouseEventArgs e)
        {
            PowerHoverEvent?.Invoke(sender, e);
        }

        private void PowerMouseoutEvent_Emit(object sender, MouseEventArgs e)
        {
            PowerMouseoutEvent?.Invoke(sender, e);
        }
        #endregion
    }
}