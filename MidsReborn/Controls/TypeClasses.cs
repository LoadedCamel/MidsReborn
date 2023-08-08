using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MetaControls.Converters;

namespace MetaControls
{
    #region FormPages

    [TypeConverter(typeof(GenericTypeConverter<BorderProps>))]
    public class BorderProps : INotifyPropertyChanged
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BorderToDraw Which { get; set; } = BorderToDraw.All;

        [Description("Determines the thickness of the components border.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Thickness { get; set; } = 1;

        [Description("The border color of the component.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Color { get; set; } = Color.FromArgb(12, 56, 100);

        [Description("Defines the style of the components border.")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
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