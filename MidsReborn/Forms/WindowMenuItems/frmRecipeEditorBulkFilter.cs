using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmRecipeEditorBulkFilter : Form
    {
        public enum FlagAction
        {
            Enable = 0,
            Disable = 1,
            Ignore = 2
        }

        public enum FlagConditionType
        {
            None = -1,
            Is = 0,
            Contains = 1,
            StartsWith = 2,
            DoesNotContain = 3,
            DoesNotStartWith = 4
        }

        public class BulkFilterResult
        {
            public FlagAction G;
            public FlagAction H;
            public FlagAction V;
            public bool UseFilter;
            public int Column;
            public FlagConditionType FlagConditionType;
            public string ConditionText;
        }

        public BulkFilterResult FilterResult;
        
        public frmRecipeEditorBulkFilter()
        {
            InitializeComponent();
        }

        private void frmRecipeEditorBulkFilter_Load(object sender, EventArgs e)
        {
            cbColumn.SelectedIndex = 0;
            cbConditionRel.SelectedIndex = 2;
            tbConditionText.Text = "";
            rbRange2.Checked = true;
            rbGIgnore.Checked = true;
            rbVIgnore.Checked = true;
            rbHIgnore.Checked = true;
            tbConditionText.Focus();
        }

        private void rbRange_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox) sender;
            var filterEnabled = cb.Name == "rbRange2" & cb.Checked || cb.Name == "rbRange1" & !cb.Checked;

            cbColumn.Enabled = filterEnabled;
            cbConditionRel.Enabled = filterEnabled;
            tbConditionText.Enabled = filterEnabled;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            FilterResult = new BulkFilterResult();
            if (rbGEnabled.Checked)
            {
                FilterResult.G = FlagAction.Enable;
            }
            else if (rbGDisabled.Checked)
            {
                FilterResult.G = FlagAction.Disable;
            }
            else
            {
                FilterResult.G = FlagAction.Ignore;
            }

            if (rbVEnabled.Checked)
            {
                FilterResult.V = FlagAction.Enable;
            }
            else if (rbVDisabled.Checked)
            {
                FilterResult.V = FlagAction.Disable;
            }
            else
            {
                FilterResult.V = FlagAction.Ignore;
            }

            if (rbHEnabled.Checked)
            {
                FilterResult.H = FlagAction.Enable;
            }
            else if (rbHDisabled.Checked)
            {
                FilterResult.H = FlagAction.Disable;
            }
            else
            {
                FilterResult.H = FlagAction.Ignore;
            }

            FilterResult.UseFilter = rbRange2.Checked;
            FilterResult.Column = cbColumn.SelectedIndex;
            FilterResult.FlagConditionType = (FlagConditionType) cbConditionRel.SelectedIndex;
            FilterResult.ConditionText = tbConditionText.Text;

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