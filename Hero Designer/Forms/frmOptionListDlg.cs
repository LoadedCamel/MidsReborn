using System;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmOptionListDlg : Form
    {
        private Button Cancel_Button;
        private CheckBox chkRemember;
        private ComboBox cmbAction;
        private Label lblDescript;

        private Button OK_Button;
        private TableLayoutPanel TableLayoutPanel1;

        private frmOptionListDlg()
        {
            InitializeComponent();
            Name = nameof(frmOptionListDlg);
        }

        private bool? remember => chkRemember?.Checked;

        private void Cancel_Button_Click(object sender, EventArgs e)

        {
            chkRemember.Checked = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OK_Button_Click(object sender, EventArgs e)

        {
            DialogResult = (DialogResult) (cmbAction.SelectedIndex + 2);
            Close();
        }

        public static (DialogResult, bool? remember) ShowWithOptions(
            bool AllowRemember,
            int DefaultOption,
            string descript,
            params string[] OptionList)
        {
            var frm = new frmOptionListDlg
            {
                chkRemember = {Enabled = AllowRemember, Visible = AllowRemember, Checked = false},
                lblDescript = {Text = descript}
            };
            frm.cmbAction.Items.Clear();
            frm.cmbAction.Items.AddRange(OptionList);
            frm.cmbAction.SelectedIndex = DefaultOption < frm.cmbAction.Items.Count - 1 ? DefaultOption : 0;
            var result = frm.ShowDialog();
            return (result, frm.remember);
        }
    }
}