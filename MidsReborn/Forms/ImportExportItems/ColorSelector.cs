using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ColorSelector : Form
    {
        public Color SelectedColor;
        private Color InitialColor;

        public ColorSelector()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            InitialColor = Color.FromArgb(0x80, 0x80, 0x80);
        }

        public ColorSelector(Color initialColor)
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            InitialColor = initialColor;
        }

        private void ColorSelector_Load(object sender, EventArgs e)
        {
            btnOk.UseAlt = MidsContext.Character?.IsVillain == true;
            btnCancel.UseAlt = MidsContext.Character?.IsVillain == true;

            SelectedColor = InitialColor;
        }

        private void colorWheel1_SelectionChanged(object sender, ColorWheel.SelectedValues selected)
        {
            SelectedColor = selected.Color;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
