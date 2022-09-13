using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.v2Controls
{
    public partial class PowerList : UserControl
    {
        #region Designer Properties (HIDDEN)

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Image BackgroundImage { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new BorderStyle BorderStyle { get; set; } = BorderStyle.None;

        #endregion

        #region Designer Properties (VISIBLE)

        [Description("The background color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color BackColor { get; set; } = Color.Transparent;
        
        [Description("The font to be used on the label of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font { get; set; } = new Font("Arial", 9.25f);

        [Description("The color of the text used for the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color ForeColor { get; set; } = Color.White;

        [Description("Determines the appearance of the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public PowerListOutline Outline { get; set; } = new PowerListOutline();

        [Description("The text to be displayed as the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text { get; set; }

        #endregion

        #region Designer Properties TypeConverters and Classes

        public class PowerListOutlineTypeConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(PowerListOutline));
            }
        }

        [TypeConverter(typeof(PowerListOutlineTypeConverter))]
        public class PowerListOutline
        {
            [Description("Enables the controls label text to be outlined.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public bool Enabled { get; set; } = true;

            [Description("Sets the color of outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color Color { get; set; } = Color.Black;

            [Description("Sets the width of the outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Width { get; set; } = 2;

            public override string ToString()
            {
                return $"{Enabled}, {Color}, {Width}";
            }
        }

        #endregion



        public PowerList()
        {
            InitializeComponent();
        }
    }
}
