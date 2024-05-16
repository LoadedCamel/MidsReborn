using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.Controls
{
    public class ControlBorderSide
    {
        private int _thickness = 1;
        private readonly Color _defaultColor = Color.FromArgb(12, 56, 100);
        private Color? _color;
        private ButtonBorderStyle _style = ButtonBorderStyle.None;

        [Description("Determines the thickness of the components border.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Thickness
        {
            get => _thickness;
            set => SetField(ref _thickness, value);
        }

        [Description("The border color of the component.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Color
        {
            get => _color ??= _defaultColor;
            set => SetField(ref _color, value);
        }

        [Description("Defines the style of the components border.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ButtonBorderStyle Style
        {
            get => _style;
            set => SetField(ref _style, value);
        }

        public override string ToString()
        {
            return $"{Color.Name}, {Thickness}pt, style={Style}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
