using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class PowerFlowPanel : FlowLayoutPanel
    {
        protected override CreateParams CreateParams
        {
            get 
            { 
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public event EventHandler? BorderColorChanged;

        private Color _borderColor = Color.DodgerBlue;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle { get; set; } = BorderStyle.None;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool WrapContents
        {
            get => base.WrapContents;
            set => base.WrapContents = value;
        }

        [DefaultValue(typeof(AutoSizeMode), "GrowAndShrink")]
        public override AutoSizeMode AutoSizeMode
        {
            get => base.AutoSizeMode;
            set => base.AutoSizeMode = value;
        }

        [Description("The border color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                BorderColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public PowerFlowPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BorderColorChanged += OnBorderColorChanged;
        }

        private void OnBorderColorChanged(object? sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            using var brush = new SolidBrush(BorderColor);
            e.Graphics.DrawRectangle(new Pen(brush, 2), ClientRectangle);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SuspendLayout();
            switch (FlowDirection)
            {
                case FlowDirection.TopDown:
                    foreach (Control control in Controls)
                    {
                        control.Width = ClientSize.Width - control.Margin.Left - control.Margin.Right;
                    }
                    break;
                case FlowDirection.LeftToRight:
                    break;
                case FlowDirection.RightToLeft:
                    break;
                case FlowDirection.BottomUp:
                    break;
            }
            ResumeLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            var control = levent.AffectedControl;
            if (control == null) return;
            switch (FlowDirection)
            {
                case FlowDirection.LeftToRight:
                    break;
                case FlowDirection.TopDown:
                    control.Width = ClientSize.Width - control.Margin.Left - control.Margin.Right;
                    break;
                case FlowDirection.RightToLeft:
                    break;
                case FlowDirection.BottomUp:
                    break;
            }
        }
    }
}
