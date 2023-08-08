using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Extensions;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ShareMenu : Form
    {
        public ShareMenu()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
        }

        private void frmForumExport_Load(object sender, EventArgs e)
        {
            // Restyle utility windows - experimental
            StylizeWindow(Handle, Color.Silver, Color.Black, Color.WhiteSmoke);
        }

        // Previous HTML generator code
        private void btnExport_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialogExt(this, "Theme Color Selector");
            colorDialog.ShowDialog(this);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lbColorTheme_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rbLightThemes_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbDarkThemes_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbAllThemes_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panelColorTitle_Click(object sender, EventArgs e)
        {

        }

        private void panelColorHeadings_Click(object sender, EventArgs e)
        {

        }

        private void panelColorLevels_Click(object sender, EventArgs e)
        {

        }

        private void panelColorSlots_Click(object sender, EventArgs e)
        {

        }

        private void chkCustomThemeDark_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabStrip1_TabClick(int index)
        {
            formPages1.SelectedIndex = index;
        }

        private void cbHtmlIncludeAcc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbHtmlIncludeInc_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbHtmlIncludeExtras_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ibExExportHtml_Click(object sender, EventArgs e)
        {

        }
    }
}
