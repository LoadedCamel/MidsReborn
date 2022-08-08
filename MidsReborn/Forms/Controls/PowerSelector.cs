using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public partial class PowerSelector : Form
    {
        public IPower SelectedPower { get; set; }
        
        public PowerSelector()
        {
            Load += OnLoad;
            InitializeComponent();
        }

        private async void OnLoad(object sender, EventArgs e)
        {
            await FillGroups();
        }

        private async Task FillGroups()
        {
            var groups = (from dbGroup in DatabaseAPI.Database.PowersetGroups where dbGroup.Key != "Boosts" && dbGroup.Key != "Incarnate" && dbGroup.Key != "Set_Bonus" select new KeyValue<string, PowersetGroup>(dbGroup.Key, dbGroup.Value)).ToList();
            cbGroup.BeginUpdate();
            cbGroup.DataSource = new BindingSource(groups, null);
            cbGroup.DisplayMember = "Key";
            cbGroup.ValueMember = "Value";
            cbGroup.SelectedIndex = 0;
            cbGroup.EndUpdate();
            CbGroupOnSelectedIndexChanged(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        private void CbGroupOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGroup.SelectedIndex < 0) return;

            if (cbGroup.SelectedValue is not PowersetGroup selectedGroup) return;
            var powerSets = selectedGroup.Powersets.Values.ToList();
            cbPowerset.BeginUpdate();
            cbPowerset.DataSource = new BindingSource(powerSets, null);
            cbPowerset.DisplayMember = "DisplayName";
            cbPowerset.ValueMember = null;
            cbPowerset.SelectedIndex = 0;
            cbPowerset.EndUpdate();

        }

        private void CbPowersetOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPowerset.SelectedIndex < 0) return;

            if (cbPowerset.SelectedValue is not IPowerset selectedPowerSet) return;
            var powers = selectedPowerSet.Powers.ToList();
            cbPower.BeginUpdate();
            cbPower.DataSource = new BindingSource(powers, null);
            cbPower.DisplayMember = "DisplayName";
            cbPower.ValueMember = null;
            cbPower.SelectedIndex = 0;
            cbPower.EndUpdate();
        }

        private void CbPowerOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPower.SelectedIndex < 0) return;
            if (cbPower.SelectedValue is not IPower selectedPower) return;
            SelectedPower = selectedPower;
        }

        private void btnOkay_Click(object sender, EventArgs e)
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
