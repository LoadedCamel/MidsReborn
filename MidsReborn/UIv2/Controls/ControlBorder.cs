using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Controls.Converters;

namespace Mids_Reborn.UIv2.Controls
{
    [TypeConverter(typeof(GenericTypeConverter<ControlBorder>))]
    public class ControlBorder : INotifyPropertyChanged
    {
        private ControlBorderSide _left = new();
        private ControlBorderSide _right = new();
        private ControlBorderSide _top = new();
        private ControlBorderSide _bottom = new();

        [Category("Appearance")]
        [Description("Settings for the left border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))] // This makes it expandable in the Property Grid
        public ControlBorderSide Left
        {
            get => _left;
            set => SetField(ref _left, value);
        }

        [Category("Appearance")]
        [Description("Settings for the right border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))] // This makes it expandable in the Property Grid
        public ControlBorderSide Right
        {
            get => _right;
            set => SetField(ref _right, value);
        }

        [Category("Appearance")]
        [Description("Settings for the top border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))] // This makes it expandable in the Property Grid
        public ControlBorderSide Top
        {
            get => _top;
            set => SetField(ref _top, value);
        }

        [Category("Appearance")]
        [Description("Settings for the bottom border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))] // This makes it expandable in the Property Grid
        public ControlBorderSide Bottom
        {
            get => _bottom;
            set => SetField(ref _bottom, value);
        }

        public override string ToString()
        {
            // Assuming ControlBorderSide class has a Style property and a way to check if it's set to None
            var leftSet = _left.Style != ButtonBorderStyle.None;
            var rightSet = _right.Style != ButtonBorderStyle.None;
            var topSet = _top.Style != ButtonBorderStyle.None;
            var bottomSet = _bottom.Style != ButtonBorderStyle.None;

            // Check if all sides are set
            if (leftSet && rightSet && topSet && bottomSet)
            {
                return "All";
            }
    
            // Build a list of sides that are set
            var setSides = new List<string>();
            if (leftSet) setSides.Add("Left");
            if (rightSet) setSides.Add("Right");
            if (topSet) setSides.Add("Top");
            if (bottomSet) setSides.Add("Bottom");

            // Join the set sides names with commas or return "None" if none are set
            return setSides.Count > 0 ? string.Join(", ", setSides) : "None";
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
