using System;
using System.ComponentModel;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmExportColor : Form
    {
        public ExportConfig.ColorScheme myScheme;

        public frmExportColor(ref ExportConfig.ColorScheme iScheme)
        {
            Load += frmExportColor_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmExportColor));
            Icon = Resources.reborn;
            Name = nameof(frmExportColor);
            myScheme.Assign(iScheme);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void csHeading_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Heading;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Heading = cPicker.Color;
            updateColors();
        }

        private void csHO_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.HOColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.HOColor = cPicker.Color;
            updateColors();
        }

        private void csIO_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.IOColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.IOColor = cPicker.Color;
            updateColors();
        }

        private void csLevel_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Level;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Level = cPicker.Color;
            updateColors();
        }

        private void csPower_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Power;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Power = cPicker.Color;
            updateColors();
        }

        private void csSet_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.SetColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.SetColor = cPicker.Color;
            updateColors();
        }

        private void csSlots_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Slots;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Slots = cPicker.Color;
            updateColors();
        }

        private void csTitle_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Title;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Title = cPicker.Color;
            updateColors();
        }

        private void frmExportColor_Load(object sender, EventArgs e)
        {
            txtName.Text = myScheme.SchemeName;
            updateColors();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            myScheme.SchemeName = txtName.Text;
        }

        private void updateColors()
        {
            csTitle.BackColor = myScheme.Title;
            csHeading.BackColor = myScheme.Heading;
            csLevel.BackColor = myScheme.Level;
            csSlots.BackColor = myScheme.Slots;
            csIO.BackColor = myScheme.IOColor;
            csSet.BackColor = myScheme.SetColor;
            csHO.BackColor = myScheme.HOColor;
            csPower.BackColor = myScheme.Power;
        }
    }
}