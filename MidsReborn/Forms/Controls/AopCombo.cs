using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public partial class AopCombo : ComboBox
    {
        #region Enums
        public enum ComboBoxType
        {
            Archetype,
            Origin,
            Powerset,
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
        public new Font Font { get; set; } = DefaultFont;

        [Description("Highlight color for selected item")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "Dodger Blue")]
        public Color HighlightColor { get; set; }

        public AopCombo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override async void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                Image? comboImage = null;
                List<string?> images;
                string? selectedImage;
                switch (ComboType)
                {
                    case ComboBoxType.Archetype:
                        images = await I9Gfx.LoadArchetypes();
                        selectedImage = images.FirstOrDefault(i => Items[e.Index] is Archetype selectedArchetype && i != null && i.Contains(selectedArchetype.ClassName));
                        if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                        break;
                    case ComboBoxType.Origin:
                        images = await I9Gfx.LoadOrigins();
                        selectedImage = images.FirstOrDefault(i => Items[e.Index] is string selectedOrigin && i != null && i.Contains(selectedOrigin));
                        if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                        break;
                    case ComboBoxType.Powerset:
                        images = await I9Gfx.LoadPowerSets();
                        selectedImage = images.FirstOrDefault(i => Items[e.Index] is Powerset powerSet && i != null && i.Contains(powerSet.ImageName));
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
