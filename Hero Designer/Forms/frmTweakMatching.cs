using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmTweakMatching : Form
    {
        private bool Loaded;

        public frmTweakMatching()
        {
            Load += frmTweakMatching_Load;
            Loaded = false;
            InitializeComponent();
            Name = nameof(frmTweakMatching);
            var componentResourceManager = new ComponentResourceManager(typeof(frmTweakMatching));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
        }

        private void btnAdd_Click(object sender, EventArgs e)

        {
            var num1 = -1;
            var num2 = MidsContext.Config.CompOverride.Length - 1;
            for (var index1 = 0; index1 <= num2; ++index1)
            {
                var compOverride = MidsContext.Config.CompOverride;
                var index2 = index1;
                if ((compOverride[index2].Power == cbPower.SelectedItem.ToString()) &
                    (compOverride[index2].Powerset == cbSet1.SelectedItem.ToString()))
                    num1 = index1;
            }

            if (num1 > -1)
            {
                var num3 = (int) Interaction.MsgBox("An override for that powerset/power already exists!",
                    MsgBoxStyle.Information, "Can't have duplicates!");
                lstTweaks.SelectedIndex = num1;
            }
            else
            {
                if ((txtAddOvr.Text != txtAddActual.Text) & (txtAddOvr.Text != ""))
                {
                    MidsContext.Config.CompOverride = (Enums.CompOverride[]) Utils.CopyArray(
                        MidsContext.Config.CompOverride,
                        new Enums.CompOverride[MidsContext.Config.CompOverride.Length + 1]);
                    var compOverride = MidsContext.Config.CompOverride;
                    var index = MidsContext.Config.CompOverride.Length - 1;
                    compOverride[index].Power = Convert.ToString(cbPower.SelectedItem);
                    compOverride[index].Powerset = Convert.ToString(cbSet1.SelectedItem);
                    compOverride[index].Override = txtAddOvr.Text;
                }

                listOverrides();
                lstTweaks.SelectedIndex = lstTweaks.Items.Count - 1;
            }
        }

        private void btnDel_Click(object sender, EventArgs e)

        {
            if (lstTweaks.SelectedIndex < 0)
                return;
            var compOverrideArray = new Enums.CompOverride[MidsContext.Config.CompOverride.Length - 2 + 1];
            var selectedIndex = lstTweaks.SelectedIndex;
            var index1 = 0;
            var num1 = MidsContext.Config.CompOverride.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                var compOverride = MidsContext.Config.CompOverride;
                var index3 = index2;
                compOverrideArray[index1].Override = compOverride[index3].Override;
                compOverrideArray[index1].Power = compOverride[index3].Power;
                compOverrideArray[index1].Powerset = compOverride[index3].Powerset;
                ++index1;
            }

            MidsContext.Config.CompOverride = new Enums.CompOverride[compOverrideArray.Length - 1 + 1];
            var num2 = MidsContext.Config.CompOverride.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
            {
                var compOverride = MidsContext.Config.CompOverride;
                var index3 = index2;
                compOverride[index3].Override = compOverrideArray[index2].Override;
                compOverride[index3].Power = compOverrideArray[index2].Power;
                compOverride[index3].Powerset = compOverrideArray[index2].Powerset;
            }

            listOverrides();
        }

        private void Button1_Click(object sender, EventArgs e)

        {
            Hide();
        }

        private void Button2_Click(object sender, EventArgs e)

        {
            if (lstTweaks.SelectedIndex < 0)
                return;
            MidsContext.Config.CompOverride[lstTweaks.SelectedIndex].Override = txtOvr.Text;
            var selectedIndex = lstTweaks.SelectedIndex;
            listOverrides();
            lstTweaks.SelectedIndex = selectedIndex;
        }

        private void cbAT1_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            List_Sets();
        }

        private void cbPower_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (cbPower.SelectedIndex < 0)
                return;
            txtAddActual.Text = DatabaseAPI.Database.Powersets[getSetIndex()].Powers[cbPower.SelectedIndex].DescShort;
            txtAddOvr.Text = txtAddActual.Text;
        }

        private void cbSet1_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            GetPowers();
        }

        private void cbType1_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            List_Sets();
        }

        private void frmTweakMatching_Load(object sender, EventArgs e)

        {
            list_AT();
            list_Type();
            List_Sets();
            GetPowers();
            listOverrides();
            Loaded = true;
        }

        private int getAT()
        {
            return cbAT1.SelectedIndex;
        }

        private void GetPowers()
        {
            var index1 = 0;
            var numArray = new int[2];
            cbPower.BeginUpdate();
            cbPower.Items.Clear();
            numArray[0] = getSetIndex();
            var num = DatabaseAPI.Database.Powersets[numArray[index1]].Powers.Length - 1;
            for (var index2 = 0; index2 <= num; ++index2)
                cbPower.Items.Add(DatabaseAPI.Database.Powersets[numArray[index1]].Powers[index2].DisplayName);
            cbPower.SelectedIndex = 0;
            cbPower.EndUpdate();
        }

        private int getSetIndex()
        {
            return DatabaseAPI.GetPowersetIndexes(getAT(), getSetType())[cbSet1.SelectedIndex].nID;
        }

        private Enums.ePowerSetType getSetType()
        {
            var ePowerSetType = cbType1.SelectedIndex switch
            {
                0 => Enums.ePowerSetType.Primary,
                1 => Enums.ePowerSetType.Secondary,
                2 => Enums.ePowerSetType.Ancillary,
                _ => Enums.ePowerSetType.Primary
            };
            return ePowerSetType;
        }

        [DebuggerStepThrough]
        private void list_AT()
        {
            cbAT1.BeginUpdate();
            cbAT1.Items.Clear();
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
                cbAT1.Items.Add(DatabaseAPI.Database.Classes[index].DisplayName);
            cbAT1.SelectedIndex = 0;
            cbAT1.EndUpdate();
        }

        private void List_Sets()
        {
            var iSet = Enums.ePowerSetType.None;
            var cbSet1 = this.cbSet1;
            var cbType1 = this.cbType1;
            var selectedIndex = cbAT1.SelectedIndex;
            iSet = cbType1.SelectedIndex switch
            {
                0 => Enums.ePowerSetType.Primary,
                1 => Enums.ePowerSetType.Secondary,
                2 => Enums.ePowerSetType.Ancillary,
                _ => iSet
            };
            cbSet1.BeginUpdate();
            cbSet1.Items.Clear();
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(selectedIndex, iSet);
            var num = powersetIndexes.Length - 1;
            for (var index = 0; index <= num; ++index)
                cbSet1.Items.Add(powersetIndexes[index].DisplayName);
            if (cbSet1.Items.Count > 0)
                cbSet1.SelectedIndex = 0;
            cbSet1.EndUpdate();
        }

        private void list_Type()
        {
            cbType1.BeginUpdate();
            cbType1.Items.Clear();
            cbType1.Items.Add("Primary");
            cbType1.Items.Add("Secondary");
            cbType1.Items.Add("Ancillary");
            cbType1.SelectedIndex = 0;
            cbType1.EndUpdate();
        }

        private void listOverrides()
        {
            lstTweaks.BeginUpdate();
            lstTweaks.Items.Clear();
            var num = MidsContext.Config.CompOverride.Length - 1;
            for (var index = 0; index <= num; ++index)
                lstTweaks.Items.Add(MidsContext.Config.CompOverride[index].Powerset + "." +
                                    MidsContext.Config.CompOverride[index].Power);
            if (lstTweaks.Items.Count > 0)
                lstTweaks.SelectedIndex = 0;
            lstTweaks.EndUpdate();
        }

        private void lstTweaks_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (lstTweaks.SelectedIndex < 0)
                return;
            txtOvr.Text = MidsContext.Config.CompOverride[lstTweaks.SelectedIndex].Override;
        }
    }
}