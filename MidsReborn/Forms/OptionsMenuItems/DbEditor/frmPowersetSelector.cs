using System;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowersetSelector : Form
    {
        public string UidSet { get; private set; }
        public int NidSet { get; private set; }

        public frmPowersetSelector()
        {
            InitializeComponent();
        }

        private void frmPowersetSelector_Load(object sender, EventArgs e)
        {
            rbSelectionType1.Checked = true;
            PopulateGroups();
            PopulateGroupPowersets();
        }

        private void PopulateGroups()
        {
            cbPsGroup.BeginUpdate();
            cbPsGroup.Items.Clear();
            cbPsGroup.Items.Add("--");
            foreach (var g in DatabaseAPI.Database.PowersetGroups)
            {
                cbPsGroup.Items.Add(g.Value.Name);
            }
            cbPsGroup.EndUpdate();
            cbPsGroup.SelectedIndex = 0;
        }

        private void PopulateGroupPowersets()
        {
            cbPowerset.BeginUpdate();
            cbPowerset.Items.Clear();
            cbPowerset.Items.Add("--");
            if (cbPsGroup.SelectedIndex > 0)
            {
                var psList = DatabaseAPI.Database.Powersets
                    .Where(e => e != null && e.FullName.StartsWith(cbPsGroup.Items[cbPsGroup.SelectedIndex].ToString()))
                    .ToList();

                foreach (var ps in psList)
                {
                    cbPowerset.Items.Add($"{ps.DisplayName} [{ps.FullName.Replace($"{ps.GroupName}.", "")}]");
                }
            }
            cbPowerset.EndUpdate();
            cbPowerset.SelectedIndex = 0;
        }

        private void rbSelectionType1_CheckedChanged(object sender, EventArgs e)
        {
            rbSelectionType2.Checked = !rbSelectionType1.Checked;
            tbPsName.Enabled = !rbSelectionType1.Checked;
            cbPsGroup.Enabled = rbSelectionType1.Checked;
            cbPowerset.Enabled = rbSelectionType1.Checked;
        }

        private void rbSelectionType2_CheckedChanged(object sender, EventArgs e)
        {
            rbSelectionType1.Checked = !rbSelectionType2.Checked;
            tbPsName.Enabled = rbSelectionType2.Checked;
            cbPsGroup.Enabled = !rbSelectionType2.Checked;
            cbPowerset.Enabled = !rbSelectionType2.Checked;
        }

        private void cbPsGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGroupPowersets();
            tbPsName.Text = cbPsGroup.SelectedIndex <= 0 | cbPowerset.SelectedIndex <= 0
                ? ""
                : $"{cbPsGroup.Items[cbPsGroup.SelectedIndex]}.{cbPowerset.Items[cbPowerset.SelectedIndex]}";
        }

        private void cbPowerset_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbPsName.Text = cbPsGroup.SelectedIndex <= 0 | cbPowerset.SelectedIndex <= 0
                ? ""
                : $"{cbPsGroup.Items[cbPsGroup.SelectedIndex]}.{cbPowerset.Items[cbPowerset.SelectedIndex]}";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var psName = tbPsName.Text.Trim();
            if (string.IsNullOrWhiteSpace(psName))
            {
                MessageBox.Show($"Powerset name is empty.", "Bloop", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var psTest = DatabaseAPI.Database.Powersets
                .Any(e => e?.FullName == psName);

            if (!psTest)
            {
                MessageBox.Show($"Name '{psName}' doesn't match any powerset.", "Bloop", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var psData = DatabaseAPI.Database.Powersets
                .First(e => e?.FullName == psName);

            UidSet = psData.FullName;
            NidSet = DatabaseAPI.NidFromUidPowerset(psData.FullName);

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
