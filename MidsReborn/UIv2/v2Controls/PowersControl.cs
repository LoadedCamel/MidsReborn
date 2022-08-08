using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.UIv2.v2Controls
{
    public partial class PowersControl : PictureBox
    {
        [Description("The color of the Power text.")]
        [Category("PowerSettings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor { get; set; }

        [Description("The font to be used for the Power text")]
        [Category("PowerSettings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont { get; set; }

        private Collection<ButtonImages> field = new Collection<ButtonImages>();

        [Description("A collection of images for the power buttons.")]
        [Category("PowerSettings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<ButtonImages> ButtonImages => field;

        [Description("Sets the number of columns to be used.")]
        [Category("PowerSettings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Columns { get; set; }

        [Description("Sets the number of rows to be used.")]
        [Category("PowerSettings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Rows { get; set; }

        private List<PowerItem> Items { get; set; }
        public PowersControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            Items = new List<PowerItem>();
            InitializeComponent();
        }

        public void AddPower(int pos, IPower power, Image button)
        {
            var item = new PowerItem
            {
                Position = pos,
                Power = power,
                Font = TextFont,
                Color = TextColor,
                ButtonImage = button
            };
            Items.Add(item);
        }

        public void RemovePower(int pos)
        {
            var match = Items.FindIndex(p => p.Position == pos);
            Items.RemoveAt(match);
        }

        private void DrawPowers()
        {

        }
    }

    public class ButtonImages
    {
        public Image EmptyPower { get; set; }
        public Image HeroPower { get; set; }
        public Image VillainPower { get; set; }
    }

    public class PowerItem
    {
        public int Position { get; set; }
        public IPower Power { get; set; }
        public Font Font { get; set; }
        public Color Color { get; set; }
        public Image ButtonImage { get; set; }
    }
}
