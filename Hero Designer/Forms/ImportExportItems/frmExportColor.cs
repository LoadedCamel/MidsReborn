using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Hero_Designer
{
    public partial class frmExportColor : Form
    {

        public ExportConfig.ColorScheme myScheme;

        public frmExportColor(ref ExportConfig.ColorScheme iScheme)
        {
            Load += frmExportColor_Load;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmExportColor));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmExportColor);
            myScheme.Assign(iScheme);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        void csHeading_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Heading;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Heading = cPicker.Color;
            updateColors();
        }

        void csHO_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.HOColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.HOColor = cPicker.Color;
            updateColors();
        }

        void csIO_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.IOColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.IOColor = cPicker.Color;
            updateColors();
        }

        void csLevel_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Level;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Level = cPicker.Color;
            updateColors();
        }

        void csPower_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Power;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Power = cPicker.Color;
            updateColors();
        }

        void csSet_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.SetColor;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.SetColor = cPicker.Color;
            updateColors();
        }

        void csSlots_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Slots;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Slots = cPicker.Color;
            updateColors();
        }

        void csTitle_Click(object sender, EventArgs e)
        {
            cPicker.Color = myScheme.Title;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myScheme.Title = cPicker.Color;
            updateColors();
        }

        void frmExportColor_Load(object sender, EventArgs e)
        {
            txtName.Text = myScheme.SchemeName;
            updateColors();
        }

        void txtName_TextChanged(object sender, EventArgs e)
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