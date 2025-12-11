using System;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmExprSR : Form
    {
        public string SearchText;
        public string ReplaceText;
        public bool IncludeConditionals;

        public frmExprSR()
        {
            InitializeComponent();
            SearchText = "";
            ReplaceText = "";
            IncludeConditionals = false;
        }

        private void frmExprSR_Load(object sender, EventArgs e)
        {
            searchPattern.Select();
        }

        private void searchPattern_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = !string.IsNullOrEmpty(searchPattern.Text) & !string.IsNullOrEmpty(replacePattern.Text);
            SearchText = searchPattern.Text;
        }

        private void replacePattern_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = !string.IsNullOrEmpty(searchPattern.Text) & !string.IsNullOrEmpty(replacePattern.Text);
            ReplaceText = replacePattern.Text;
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

        private void chkIncludeConditionals_CheckedChanged(object sender, EventArgs e)
        {
            IncludeConditionals = chkIncludeConditionals.Checked;
        }
    }
}
