using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.Controls
{
    internal class BorderFlow : FlowLayoutPanel
    {
        [Description("Sets the border for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowBorder Border { get; set; } = new();
        
        public BorderFlow()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            switch (Border.Which)
            {
                case FlowBorder.BorderToDraw.All:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Border.Color, Border.Thickness, Border.Style,
                        Border.Color, Border.Thickness, Border.Style,
                        Border.Color, Border.Thickness, Border.Style,
                        Border.Color, Border.Thickness, Border.Style);
                    break;
                case FlowBorder.BorderToDraw.Left:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Border.Color, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case FlowBorder.BorderToDraw.Top:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Border.Color, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case FlowBorder.BorderToDraw.Right:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Border.Color, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case FlowBorder.BorderToDraw.Bottom:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Border.Color, Border.Thickness, Border.Style);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #region Border Properties

        [TypeConverter(typeof(MrbTypeConverter<FlowBorder>))]
        internal class FlowBorder : INotifyPropertyChanged
        {
            public enum BorderToDraw
            {
                All,
                Left,
                Top,
                Right,
                Bottom
            }

            [Description("Determines which of the components borders to draw.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public BorderToDraw Which { get; set; } = BorderToDraw.All;

            [Description("Determines the thickness of the components border.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public int Thickness { get; set; } = 1;

            [Description("The border color of the component.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color Color { get; set; } = Color.FromArgb(12, 56, 100);

            [Description("Defines the style of the components border.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public ButtonBorderStyle Style { get; set; } = ButtonBorderStyle.Solid;

            public override string ToString()
            {
                return $"{Which}, {Thickness}, {Color}, {Style}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion
    }
}
