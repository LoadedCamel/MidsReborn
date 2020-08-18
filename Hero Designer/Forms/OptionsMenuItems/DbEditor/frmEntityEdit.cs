using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEntityEdit : Form
    {
        public readonly SummonedEntity myEntity;
        private bool loading;
        private bool Updating;

        public frmEntityEdit(SummonedEntity iEntity)
        {
            Load += frmEntityEdit_Load;
            Updating = false;
            loading = true;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEntityEdit));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            Label4.Text = componentResourceManager.GetString("Label4.Text");
            Name = nameof(frmEntityEdit);
            myEntity = new SummonedEntity(iEntity);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var num1 = DatabaseAPI.Database.Entities.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                if (!((DatabaseAPI.Database.Entities[index].UID.ToLower() == myEntity.UID.ToLower()) &
                      (index != myEntity.GetNId())))
                    continue;
                Interaction.MsgBox(myEntity.UID + " is not unique. Please enter a unique name.",
                    MsgBoxStyle.Information, "Invalid Name");
                return;
            }

            myEntity.UpdateNClassID(DatabaseAPI.NidFromUidClass);
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnPAdd_Click(object sender, EventArgs e)
        {
            myEntity.PAdd();
            PS_FillList();
            lvPower.Items[lvPower.Items.Count - 1].Selected = true;
            lvPower.Items[lvPower.Items.Count - 1].EnsureVisible();
        }

        private void btnPDelete_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedItems.Count < 1)
                return;
            var strArray = new string[myEntity.PowersetFullName.Length];
            var selectedIndex = lvPower.SelectedIndices[0];
            var num1 = strArray.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                strArray = new string[index + 1];
                strArray[index] = myEntity.PowersetFullName[index];
                strArray[index] = myEntity.PowersetFullName[index];
            }

            myEntity.PDelete(selectedIndex);
            PS_FillList();
        }

        private void btnPDown_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedItems.Count < 1 || lvPower.SelectedIndices[0] > lvPower.Items.Count - 2)
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            var index = selectedIndex + 1;
            var strArray2 = new[]
            {
                myEntity.PowersetFullName[selectedIndex],
                myEntity.PowersetFullName[index]
            };
            myEntity.PowersetFullName[selectedIndex] = strArray2[1];
            myEntity.PowersetFullName[index] = strArray2[0];
            PS_FillList();
            lvPower.Items[index].Selected = true;
            lvPower.Items[index].EnsureVisible();
        }

        private void btnPUp_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedItems.Count < 1 || lvPower.SelectedIndices[0] < 1)
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            var index = selectedIndex - 1;
            string[] strArray2 =
            {
                myEntity.PowersetFullName[selectedIndex],
                myEntity.PowersetFullName[index]
            };
            myEntity.PowersetFullName[selectedIndex] = strArray2[1];
            myEntity.PowersetFullName[index] = strArray2[0];
            PS_FillList();
            lvPower.Items[index].Selected = true;
            lvPower.Items[index].EnsureVisible();
        }

        private void btnUGAdd_Click(object sender, EventArgs e)
        {
            myEntity.UGAdd();
            UG_FillList();
            lvUpgrade.Items[lvUpgrade.Items.Count - 1].Selected = true;
            lvUpgrade.Items[lvUpgrade.Items.Count - 1].EnsureVisible();
        }

        private void btnUGDelete_Click(object sender, EventArgs e)

        {
            if (lvUpgrade.SelectedItems.Count < 1)
                return;
            var strArray = new string[myEntity.UpgradePowerFullName.Length - 1 + 1];
            var selectedIndex = lvUpgrade.SelectedIndices[0];
            var num1 = strArray.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                strArray = new string[index + 1];
                strArray[index] = myEntity.UpgradePowerFullName[index];
                strArray[index] = myEntity.UpgradePowerFullName[index];
            }

            myEntity.UGDelete(selectedIndex);
            UG_FillList();
        }

        private void btnUGDown_Click(object sender, EventArgs e)
        {
            if (lvUpgrade.SelectedItems.Count < 1 || lvUpgrade.SelectedIndices[0] > lvUpgrade.Items.Count - 2)
                return;
            var selectedIndex = lvUpgrade.SelectedIndices[0];
            var index = selectedIndex + 1;
            string[] strArray2 =
            {
                myEntity.UpgradePowerFullName[selectedIndex],
                myEntity.UpgradePowerFullName[index]
            };
            myEntity.UpgradePowerFullName[selectedIndex] = strArray2[1];
            myEntity.UpgradePowerFullName[index] = strArray2[0];
            UG_FillList();
            lvUpgrade.Items[index].Selected = true;
            lvUpgrade.Items[index].EnsureVisible();
        }

        private void btnUGUp_Click(object sender, EventArgs e)
        {
            if (lvUpgrade.SelectedItems.Count < 1 || lvUpgrade.SelectedIndices[0] < 1)
                return;
            var selectedIndex = lvUpgrade.SelectedIndices[0];
            var index = selectedIndex - 1;
            string[] strArray2 =
            {
                myEntity.UpgradePowerFullName[selectedIndex],
                myEntity.UpgradePowerFullName[index]
            };
            myEntity.UpgradePowerFullName[selectedIndex] = strArray2[1];
            myEntity.UpgradePowerFullName[index] = strArray2[0];
            UG_FillList();
            lvUpgrade.Items[index].Selected = true;
            lvUpgrade.Items[index].EnsureVisible();
        }

        private void cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            myEntity.ClassName = cbClass.SelectedItem.ToString();
        }

        private void cbEntType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            myEntity.EntityType = (Enums.eSummonEntity) cbEntType.SelectedIndex;
        }

        private void DisplayInfo()
        {
            PS_FillList();
            UG_FillList();
            txtDisplayName.Text = myEntity.DisplayName;
            txtEntName.Text = myEntity.UID;
            cbEntType.SelectedIndex = (int) myEntity.EntityType;
            cbClass.SelectedIndex = myEntity.GetNClassId();
        }

        private void frmEntityEdit_Load(object sender, EventArgs e)
        {
            Text = "Editing Entity: " + myEntity.UID;
            cbEntType.BeginUpdate();
            cbEntType.Items.Clear();
            cbEntType.Items.AddRange(Enum.GetNames(myEntity.EntityType.GetType()));
            cbEntType.EndUpdate();
            cbClass.BeginUpdate();
            cbClass.Items.Clear();
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
                cbClass.Items.Add(DatabaseAPI.Database.Classes[index].ClassName);
            cbClass.EndUpdate();
            UG_GroupList();
            PS_GroupList();
            DisplayInfo();
            loading = false;
        }

        private void lvPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count < 1)
                return;
            PS_DisplaySet(Convert.ToString(myEntity.PowersetFullName[lvPower.SelectedIndices[0]][0]));
        }

        private void lvPSGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating || lvPSGroup.SelectedItems.Count <= 0)
                return;
            PS_SetList();
            if (lvPSSet.Items.Count > 0)
                lvPSSet.Items[0].Selected = true;
        }

        private void lvPSSet_Click(object sender, EventArgs e)
        {
            if (Updating)
                return;
            PS_UpdateItem();
        }

        private void lvPSSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            PS_UpdateItem();
        }

        private void lvUGGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating || lvUGGroup.SelectedItems.Count <= 0)
                return;
            UG_SetList();
            if (lvUGSet.Items.Count > 0)
                lvUGSet.Items[0].Selected = true;
        }

        private void lvUGPower_Click(object sender, EventArgs e)
        {
            if (Updating)
                return;
            PS_UpdateItem();
        }

        private void lvUGPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            UG_UpdateItem();
        }

        private void lvUGSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating || lvUGSet.SelectedItems.Count <= 0)
                return;
            UG_PowerList();
        }

        private void lvUpgrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUpgrade.SelectedIndices.Count < 1)
                return;
            UG_DisplayPower(Convert.ToString(myEntity.UpgradePowerFullName[lvUpgrade.SelectedIndices[0]][0]));
        }

        private void PS_DisplaySet(string iPower)
        {
            Updating = true;
            var strArray = iPower.Split(".".ToCharArray());
            if (strArray.Length > 0)
            {
                var num = lvPSGroup.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvPSGroup.Items[index].Text, strArray[0], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvPSGroup.Items[index].Selected = true;
                    lvPSGroup.Items[index].EnsureVisible();
                    break;
                }
            }

            UG_SetList();
            if (strArray.Length > 1)
            {
                var num = lvPSSet.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvPSSet.Items[index].Text, strArray[1], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvPSSet.Items[index].Selected = true;
                    lvPSSet.Items[index].EnsureVisible();
                    break;
                }
            }

            Updating = false;
        }

        private void PS_FillList()
        {
            Updating = true;
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            var num = myEntity.PowersetFullName.Length - 1;
            for (var index = 0; index <= num; ++index)
                lvPower.Items.Add(myEntity.PowersetFullName[index]);
            lvPower.EndUpdate();
            Updating = false;
            if (lvPower.Items.Count <= 0)
                return;
            lvPower.Items[0].Selected = true;
        }

        private void PS_GroupList()
        {
            lvPSGroup.BeginUpdate();
            lvPSGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvPSGroup.Items.Add(key);
            lvPSGroup.EndUpdate();
        }

        private void PS_SetList()
        {
            lvPSSet.BeginUpdate();
            lvPSSet.Items.Clear();
            if (lvPSGroup.SelectedIndices.Count < 1)
            {
                lvPSSet.EndUpdate();
            }
            else
            {
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvPSGroup.SelectedItems[0].Text);
                var num = indexesByGroupName.Length - 1;
                for (var index = 0; index <= num; ++index)
                {
                    lvPSSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvPSSet.Items[lvPSSet.Items.Count - 1].Tag =
                        DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }

                lvPSSet.EndUpdate();
            }
        }

        private void PS_UpdateItem()
        {
            if ((lvPower.SelectedIndices.Count < 1) | (lvPSGroup.SelectedIndices.Count < 1) |
                (lvPSSet.SelectedIndices.Count < 1))
                return;
            var str = lvPSGroup.SelectedItems[0].Text + "." + lvPSSet.SelectedItems[0].Text;
            myEntity.PowersetFullName[lvPower.SelectedIndices[0]] = str;
            lvPower.SelectedItems[0].SubItems[0].Text = str;
        }

        private void txtDisplayName_TextChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            myEntity.DisplayName = txtDisplayName.Text;
        }

        private void txtEntName_TextChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            myEntity.UID = txtEntName.Text;
        }

        private void UG_DisplayPower(string iPower)
        {
            Updating = true;
            var strArray = iPower.Split(".".ToCharArray());
            if (strArray.Length > 0)
            {
                var num = lvUGGroup.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvUGGroup.Items[index].Text, strArray[0], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvUGGroup.Items[index].Selected = true;
                    lvUGGroup.Items[index].EnsureVisible();
                    break;
                }
            }

            UG_SetList();
            if (strArray.Length > 1)
            {
                var num = lvUGSet.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvUGSet.Items[index].Text, strArray[1], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvUGSet.Items[index].Selected = true;
                    lvUGSet.Items[index].EnsureVisible();
                    break;
                }
            }

            UG_PowerList();
            if (strArray.Length > 2)
            {
                var num = lvUGPower.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvUGPower.Items[index].Text, strArray[2], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvUGPower.Items[index].Selected = true;
                    lvUGPower.Items[index].EnsureVisible();
                    break;
                }
            }

            Updating = false;
        }

        private void UG_FillList()
        {
            Updating = true;
            lvUpgrade.BeginUpdate();
            lvUpgrade.Items.Clear();
            var num = myEntity.UpgradePowerFullName.Length - 1;
            for (var index = 0; index <= num; ++index)
                lvUpgrade.Items.Add(myEntity.UpgradePowerFullName[index]);
            lvUpgrade.EndUpdate();
            Updating = false;
            if (lvUpgrade.Items.Count <= 0)
                return;
            lvUpgrade.Items[0].Selected = true;
        }

        private void UG_GroupList()
        {
            lvUGGroup.BeginUpdate();
            lvUGGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvUGGroup.Items.Add(key);
            lvUGGroup.EndUpdate();
        }

        private void UG_PowerList()
        {
            lvUGPower.BeginUpdate();
            lvUGPower.Items.Clear();
            if (lvUGSet.SelectedIndices.Count < 1)
            {
                lvUGPower.EndUpdate();
            }
            else
            {
                var index1 = DatabaseAPI.NidFromUidPowerset(Convert.ToString(lvUGSet.SelectedItems[0].Tag));
                if (index1 > -1)
                {
                    var num = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                    for (var index2 = 0; index2 <= num; ++index2)
                        lvUGPower.Items.Add(DatabaseAPI.Database.Powersets[index1].Powers[index2].PowerName);
                }

                lvUGPower.EndUpdate();
            }
        }

        private void UG_SetList()
        {
            lvUGSet.BeginUpdate();
            lvUGSet.Items.Clear();
            if (lvUGGroup.SelectedIndices.Count < 1)
            {
                lvUGSet.EndUpdate();
            }
            else
            {
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvUGGroup.SelectedItems[0].Text);
                var num = indexesByGroupName.Length - 1;
                for (var index = 0; index <= num; ++index)
                {
                    lvUGSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvUGSet.Items[lvUGSet.Items.Count - 1].Tag =
                        DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }

                lvUGSet.EndUpdate();
            }
        }

        private void UG_UpdateItem()
        {
            if ((lvUpgrade.SelectedIndices.Count < 1) | (lvUGGroup.SelectedIndices.Count < 1) |
                (lvUGSet.SelectedIndices.Count < 1) | (lvUGPower.SelectedIndices.Count < 1))
                return;
            var str = lvUGGroup.SelectedItems[0].Text + "." + lvUGSet.SelectedItems[0].Text + "." +
                      lvUGPower.SelectedItems[0].Text;
            myEntity.UpgradePowerFullName[lvUpgrade.SelectedIndices[0]] = str;
            lvUpgrade.SelectedItems[0].SubItems[0].Text = str;
        }
    }
}