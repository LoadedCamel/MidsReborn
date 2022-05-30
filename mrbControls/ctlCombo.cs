using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mrbBase;

namespace mrbControls
{
    public partial class ctlCombo : ComboBox
    {
        #region Enums
        public enum ComboBoxType
        {
            Archetype,
            Origin,
            Primary,
            Secondary,
            Pool,
            Ancillary
        }
        #endregion

        [Description("Type of the ComboBox")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ComboBoxType ComboType { get; set; }

        [Description("Highlight color for selected item")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "Dodger Blue")]
        public Color HighlightColor { get; set; }

        public ctlCombo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                Image comboImage = null;
                string imagesFolder;
                List<string> images;
                switch (ComboType)
                {
                    case ComboBoxType.Archetype:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Archetypes";
                        //var archetype = (Archetype) Items[e.Index];
                        //comboImage = archetype.Image(Directory.GetFiles(imagesFolder).ToList());
                        break;
                    case ComboBoxType.Origin:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Origins";
                        //images = Directory.GetFiles(imagesFolder).ToList();
                        //comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(Items[e.Index].ToString())) ?? throw new InvalidOperationException());
                        break;
                    case ComboBoxType.Primary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var primary = (IPowerset) Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(primary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case ComboBoxType.Secondary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var secondary = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(secondary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case ComboBoxType.Pool:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var pool = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(pool.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case ComboBoxType.Ancillary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var ancillary = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(ancillary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                }

                if (comboImage != null)
                {
                    e.Graphics.DrawImage(comboImage, e.Bounds.Left, e.Bounds.Top);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + comboImage.Width + 5, e.Bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top);
                }
            }

            base.OnDrawItem(e);
        }
    }
}
