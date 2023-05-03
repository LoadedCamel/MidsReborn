using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditPowerset : Form
    {
        public readonly IPowerset MyPowerSet;
        private bool _loading;
        private List<string>? _mutexUidSets;
        private List<int>? _mutexNidSets;


        public frmEditPowerset(ref IPowerset iSet)
        {
            Load += frmEditPowerset_Load;
            _loading = true;
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmEditPowerset);
            MyPowerSet = new Powerset(iSet);
            _mutexUidSets = MyPowerSet.UIDMutexSets.ToList();
            _mutexNidSets = MyPowerSet.nIDMutexSets.ToList();
        }

        private void AddListItem(int index)
        {
            lvPowers.Items.Add(new ListViewItem(new[]
            {
                Convert.ToString(DatabaseAPI.Database.Power[MyPowerSet.Power[index]]?.Level,
                    CultureInfo.InvariantCulture),
                DatabaseAPI.Database.Power[MyPowerSet.Power[index]]?.DisplayName,
                DatabaseAPI.Database.Power[MyPowerSet.Power[index]]?.DescShort
            }));
            lvPowers.Items[index].Selected = true;
            lvPowers.Items[index].EnsureVisible();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnClearIcon_Click(object sender, EventArgs e)
        {
            MyPowerSet.ImageName = "";
            DisplayIcon();
        }

        private void frmEditPowerset_CancelClose(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FormClosing += frmEditPowerset_CancelClose;
            lblNameFull.Text = $"{MyPowerSet.GroupName}.{MyPowerSet.SetName}";
            if (MyPowerSet.GroupName == "" | MyPowerSet.SetName == "")
            {
                MessageBox.Show($"Powerset name '{MyPowerSet.FullName}' is invalid.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (!PowersetFullNameIsUnique(Convert.ToString(MyPowerSet.nID)))
            {
                MessageBox.Show($"Powerset name '{MyPowerSet.FullName}' already exists, please enter a unique name.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                FormClosing -= frmEditPowerset_CancelClose;
                MyPowerSet.IsModified = true;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            if (_loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetDbPowerSetsPath();
            ImagePicker.FileName = MyPowerSet.ImageName;
            if (ImagePicker.ShowDialog(this) != DialogResult.OK) return;

            var imageFile = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(Path.Combine(I9Gfx.GetDbPowerSetsPath(), imageFile)))
            {
                MessageBox.Show($@"You must select an image from the {I9Gfx.GetDbPowerSetsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it.", @"Select Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MyPowerSet.ImageName = imageFile;
                DisplayIcon();
            }
        }

        private string BuildFullName()
        {
            var str = $"{cbNameGroup.Text}.{txtNameSet.Text}";
            lblNameFull.Text = str;
            MyPowerSet.FullName = str;
            MyPowerSet.SetName = txtNameSet.Text;
            Text = $"Edit Powerset ({str})";

            return str;
        }

        private void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            if (cbAT.SelectedIndex > -1)
            {
                MyPowerSet.nArchetype = cbAT.SelectedIndex - 1;
                MyPowerSet.ATClass = DatabaseAPI.UidFromNidClass(cbAT.SelectedIndex - 1);
            }
            else
            {
                MyPowerSet.nArchetype = -1;
            }
        }

        private void cbLinkGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            FillLinkSetCombo();
        }

        private void cbLinkSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            if (chkNoLink.Checked)
            {
                MyPowerSet.UIDLinkSecondary = "";
                MyPowerSet.nIDLinkSecondary = -1;
            }
            else if (cbLinkSet.SelectedIndex > -1)
            {
                var uidPowerset = cbLinkGroup.Text + "." + cbLinkSet.Text;
                var num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                MyPowerSet.UIDLinkSecondary = uidPowerset;
                MyPowerSet.nIDLinkSecondary = num;
            }
        }

        private void cbNameGroup_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            DisplayNameData();
        }

        private void cbNameGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            BuildFullName();
        }

        private void cbNameGroup_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            BuildFullName();
        }

        private void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            if (cbSetType.SelectedIndex > -1)
                MyPowerSet.SetType = (Enums.ePowerSetType)cbSetType.SelectedIndex;
            if (MyPowerSet.SetType == Enums.ePowerSetType.Primary)
            {
                gbLink.Enabled = true;
            }
            else
            {
                gbLink.Enabled = false;
                cbLinkSet.SelectedIndex = -1;
                cbLinkGroup.SelectedIndex = -1;
                chkNoLink.Checked = true;
            }
        }

        private void cbTrunkGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            FillTrunkSetCombo();
        }

        private void cbTrunkSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            if (chkNoTrunk.Checked)
            {
                MyPowerSet.UIDTrunkSet = "";
                MyPowerSet.nIDTrunkSet = -1;
            }
            else if (cbTrunkSet.SelectedIndex > -1)
            {
                var uidPowerset = cbTrunkGroup.Text + "." + cbTrunkSet.Text;
                var num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                MyPowerSet.UIDTrunkSet = uidPowerset;
                MyPowerSet.nIDTrunkSet = num;
            }
        }

        private void chkNoLink_CheckedChanged(object sender, EventArgs e)
        {
            cbLinkSet_SelectedIndexChanged(this, new EventArgs());
        }

        private void chkNoTrunk_CheckedChanged(object sender, EventArgs e)
        {
            cbTrunkSet_SelectedIndexChanged(this, new EventArgs());
        }

        private void DisplayIcon()
        {
            if (!string.IsNullOrEmpty(MyPowerSet.ImageName))
            {
                using var extendedBitmap = new ExtendedBitmap(I9Gfx.GetPowersetsPath() + MyPowerSet.ImageName);
                picIcon.Image = new Bitmap(extendedBitmap.Bitmap);
                btnIcon.Text = MyPowerSet.ImageName;
            }
            else
            {
                using var extendedBitmap = new ExtendedBitmap(30, 30);
                picIcon.Image = new Bitmap(extendedBitmap.Bitmap);
                btnIcon.Text = "Select Icon";
            }
        }

        private void DisplayNameData()
        {
            lblNameFull.Text = BuildFullName();
            lblNameUnique.Text = string.IsNullOrEmpty(MyPowerSet.GroupName) | string.IsNullOrEmpty(MyPowerSet.SetName)
                ? "This name is invalid."
                : PowersetFullNameIsUnique(Convert.ToString(MyPowerSet.nID, CultureInfo.InvariantCulture))
                    ? "This name is unique."
                    : "This name is NOT unique.";
        }

        private void FillLinkGroupCombo()
        {
            cbLinkGroup.BeginUpdate();
            cbLinkGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbLinkGroup.Items.Add(key);
            }

            cbLinkGroup.EndUpdate();
            if (MyPowerSet.UIDLinkSecondary == "")
            {
                return;
            }

            var index = DatabaseAPI.NidFromUidPowerset(MyPowerSet.UIDLinkSecondary);
            if (index > -1)
            {
                cbLinkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
            }
        }

        private void FillLinkSetCombo()
        {
            cbLinkSet.BeginUpdate();
            cbLinkSet.Items.Clear();
            if (cbLinkGroup.SelectedIndex > -1)
            {
                var index1 = DatabaseAPI.NidFromUidPowerset(MyPowerSet.UIDLinkSecondary);
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbLinkGroup.SelectedText);
                for (var index2 = 0; index2 < indexesByGroupName.Length; index2++)
                {
                    cbLinkSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName);
                    if (index1 > -1 && DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName ==
                        DatabaseAPI.Database.Powersets[index1].SetName)
                        index1 = index2;
                }

                cbLinkSet.SelectedIndex = index1;
            }

            cbLinkSet.EndUpdate();
        }

        private void FillTrunkGroupCombo()
        {
            cbTrunkGroup.BeginUpdate();
            cbTrunkGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbTrunkGroup.Items.Add(key);
            }

            cbTrunkGroup.EndUpdate();
            if (MyPowerSet.UIDTrunkSet == "")
            {
                return;
            }

            var index = DatabaseAPI.NidFromUidPowerset(MyPowerSet.UIDTrunkSet);
            if (index > -1)
            {
                cbTrunkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
            }
        }

        private void FillTrunkSetCombo()
        {
            cbTrunkSet.BeginUpdate();
            cbTrunkSet.Items.Clear();
            if (cbTrunkGroup.SelectedIndex > -1)
            {
                var index1 = DatabaseAPI.NidFromUidPowerset(MyPowerSet.UIDTrunkSet);
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbTrunkGroup.SelectedText);
                for (var index2 = 0; index2 < indexesByGroupName.Length; index2++)
                {
                    cbTrunkSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName);
                    if (index1 > -1 && DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName ==
                        DatabaseAPI.Database.Powersets[index1].SetName)
                        index1 = index2;
                }

                cbTrunkSet.SelectedIndex = index1;
            }

            cbTrunkSet.EndUpdate();
        }

        private void frmEditPowerset_Load(object? sender, EventArgs e)
        {
            var ePowerSetType = Enums.ePowerSetType.None;
            ListPowers();
            txtName.Text = MyPowerSet.DisplayName;
            cbNameGroup.BeginUpdate();
            cbNameGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbNameGroup.Items.Add(key);
            }

            cbNameGroup.EndUpdate();
            cbNameGroup.Text = MyPowerSet.GroupName;
            txtNameSet.Text = MyPowerSet.SetName;
            txtDesc.Text = MyPowerSet.Description;
            FillTrunkGroupCombo();
            FillTrunkSetCombo();
            chkNoTrunk.Checked = MyPowerSet.UIDTrunkSet == "";
            FillLinkGroupCombo();
            FillLinkSetCombo();
            chkNoLink.Checked = MyPowerSet.UIDLinkSecondary == "";
            if (MyPowerSet.SetType == Enums.ePowerSetType.Primary)
            {
                gbLink.Enabled = true;
            }
            else
            {
                gbLink.Enabled = false;
                cbLinkSet.SelectedIndex = -1;
                cbLinkGroup.SelectedIndex = -1;
                chkNoLink.Checked = true;
            }

            DisplayIcon();
            cbAT.BeginUpdate();
            cbAT.Items.Clear();
            cbAT.Items.Add("All / None");
            foreach (var c in DatabaseAPI.Database.Classes)
            {
                cbAT.Items.Add(c.DisplayName);
            }

            cbAT.EndUpdate();
            cbAT.SelectedIndex = MyPowerSet.nArchetype + 1;
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(Enum.GetNames(ePowerSetType.GetType()));
            cbSetType.EndUpdate();
            cbSetType.SelectedIndex = (int)MyPowerSet.SetType;
            ListAvailableMutexSets();
            ListCurrentMutexSets();
            _loading = false;
            DisplayNameData();
        }

        private void ListAvailableMutexSets()
        {
            var mutexSets = MyPowerSet.UIDMutexSets.ToList();
            var usedSets = mutexSets.Select(DatabaseAPI.GetPowersetByFullname).Where(set => set != null).Where(set => set != null).ToList();
            IEnumerable<IPowerset?>? powerSets;
            List<IPowerset?>? availableSets;
            BindingSource? bindingSource;
            switch (MyPowerSet.SetType)
            {
                case Enums.ePowerSetType.Primary:
                    powerSets = DatabaseAPI.Database.Powersets.Where(x => x?.ATClass == MyPowerSet.ATClass && x.SetType == Enums.ePowerSetType.Secondary);
                    availableSets = powerSets.Except(usedSets).ToList();
                    bindingSource = new BindingSource
                    {
                        DataSource = availableSets
                    };
                    lbAvailbleSets.DataSource = null;
                    lbAvailbleSets.DisplayMember = "DisplayName";
                    lbAvailbleSets.ValueMember = null;
                    lbAvailbleSets.DataSource = bindingSource;
                    lbAvailbleSets.Invalidate();
                    break;
                case Enums.ePowerSetType.Secondary:
                    powerSets = DatabaseAPI.Database.Powersets.Where(x => x?.ATClass == MyPowerSet.ATClass && x.SetType == Enums.ePowerSetType.Primary);
                    availableSets = powerSets.Except(usedSets).ToList();
                    bindingSource = new BindingSource
                    {
                        DataSource = availableSets
                    };
                    lbAvailbleSets.DataSource = null;
                    lbAvailbleSets.DisplayMember = "DisplayName";
                    lbAvailbleSets.ValueMember = null;
                    lbAvailbleSets.DataSource = bindingSource;
                    lbAvailbleSets.Invalidate();
                    break;
            }

        }

        private void lbAvailableSets_DoubleClick(object sender, EventArgs e)
        {
            if (lbAvailbleSets.SelectedItem == null) return;
            var set = (IPowerset)lbAvailbleSets.SelectedItem;
            if (_mutexUidSets != null && _mutexNidSets != null)
            {
                _mutexUidSets.Add(set.FullName);
                _mutexNidSets.Add(set.nID);
                MyPowerSet.UIDMutexSets = _mutexUidSets.ToArray();
                MyPowerSet.nIDMutexSets = _mutexNidSets.ToArray();
            }

            ListAvailableMutexSets();
            ListCurrentMutexSets();
        }

        private void ListCurrentMutexSets()
        {
            var mutexSets = MyPowerSet.UIDMutexSets.ToList();
            var usedSets = mutexSets.Select(DatabaseAPI.GetPowersetByFullname).Where(set => set != null).Where(set => set != null).ToList();
            var bindingSource = new BindingSource
            {
                DataSource = usedSets
            };
            lbAssignedSets.DataSource = null;
            lbAssignedSets.DisplayMember = "DisplayName";
            lbAssignedSets.ValueMember = null;
            lbAssignedSets.DataSource = bindingSource;
            lbAssignedSets.Invalidate();
        }

        private void lbAssignedSets_DoubleClick(object sender, EventArgs e)
        {
            if (lbAssignedSets.SelectedItem == null) return;
            var set = (IPowerset)lbAssignedSets.SelectedItem;
            if (_mutexUidSets != null && _mutexNidSets != null)
            {
                _mutexUidSets.Remove(set.FullName);
                _mutexNidSets.Remove(set.nID);
                MyPowerSet.UIDMutexSets = _mutexUidSets.ToArray();
                MyPowerSet.nIDMutexSets = _mutexNidSets.ToArray();
            }

            ListAvailableMutexSets();
            ListCurrentMutexSets();
        }

        private void ListPowers()
        {
            lvPowers.BeginUpdate();
            lvPowers.Items.Clear();
            for (var index = 0; index < MyPowerSet.Power.Length; index++)
            {
                AddListItem(index);
            }

            if (lvPowers.Items.Count > 0)
            {
                lvPowers.Items[0].Selected = true;
                lvPowers.Items[0].EnsureVisible();
            }

            lvPowers.EndUpdate();
        }

        private static bool PowersetFullNameIsUnique(string iFullName, int skipId = -1)
        {
            if (string.IsNullOrEmpty(iFullName))
            {
                return true;
            }

            return !DatabaseAPI.Database.Powersets
                .Where((t, index) => index != skipId && string.Equals(t.FullName, iFullName, StringComparison.OrdinalIgnoreCase))
                .Any();
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            MyPowerSet.Description = txtDesc.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            MyPowerSet.DisplayName = txtName.Text;
        }

        private void txtNameSet_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            DisplayNameData();
        }

        private void txtNameSet_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            BuildFullName();
        }
    }
}