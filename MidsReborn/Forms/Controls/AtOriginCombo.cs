using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Controls
{
    public partial class AtOriginCombo : ComboBox
    {
        #region Enums
        public enum ComboBoxType
        {
            Archetype,
            Origin,
        }
        #endregion

        [Description("Type of the ComboBox")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ComboBoxType ComboType { get; set; }

        [Description("The font to be used on the label of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font { get; set; } = new("Microsoft Sans Serif", 9.75f, FontStyle.Bold);

        [Description("Highlight color for selected item")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "Dodger Blue")]
        public Color HighlightColor { get; set; }

        public AtOriginCombo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                Image comboImage = null;
                List<string> images;
                string selectedImage;
                switch (ComboType)
                {
                    case ComboBoxType.Archetype:
                        images = I9Gfx.LoadArchetypes().GetAwaiter().GetResult();
                        var selectedArchetype = (Archetype)Items[e.Index];
                        selectedImage = images.FirstOrDefault(i => i.Contains(selectedArchetype.ClassName));
                        if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                        break;
                    case ComboBoxType.Origin:
                        images = I9Gfx.LoadOrigins().GetAwaiter().GetResult();
                        var selectedOrigin = (string)Items[e.Index];
                        selectedImage = images.FirstOrDefault(i => i.Contains(selectedOrigin));
                        if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                        break;
                }

                if (comboImage != null)
                {
                    e.Graphics.DrawImage(comboImage, e.Bounds.Left, e.Bounds.Top);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), Font, new SolidBrush(e.ForeColor), e.Bounds.Left + comboImage.Width + 5, e.Bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), Font, new SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top);
                }
            }

            base.OnDrawItem(e);
        }
    }
}
