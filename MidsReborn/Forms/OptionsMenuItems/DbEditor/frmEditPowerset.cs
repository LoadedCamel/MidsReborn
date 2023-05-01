using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public readonly IPowerset? myPS;
        private bool Loading;


        public frmEditPowerset(ref IPowerset? iSet)
        {
            Load += frmEditPowerset_Load;
            Loading = true;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEditPowerset));
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmEditPowerset);
            myPS = new Powerset(iSet);
        }

        private void AddListItem(int Index)
        {
            lvPowers.Items.Add(new ListViewItem(new[]
            {
                Convert.ToString(DatabaseAPI.Database.Power[myPS.Power[Index]].Level, CultureInfo.InvariantCulture),
                DatabaseAPI.Database.Power[myPS.Power[Index]].DisplayName,
                DatabaseAPI.Database.Power[myPS.Power[Index]].DescShort
            }));
            lvPowers.Items[Index].Selected = true;
            lvPowers.Items[Index].EnsureVisible();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnClearIcon_Click(object sender, EventArgs e)
        {
            myPS.ImageName = "";
            DisplayIcon();
        }

        private void frmEditPowerset_CancelClose(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FormClosing += frmEditPowerset_CancelClose;
            lblNameFull.Text = $"{myPS.GroupName}.{myPS.SetName}";
            if (myPS.GroupName == "" | myPS.SetName == "")
            {
                MessageBox.Show($"Powerset name '{myPS.FullName}' is invalid.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (!PowersetFullNameIsUnique(Convert.ToString(myPS.nID)))
            {
                MessageBox.Show($"Powerset name '{myPS.FullName}' already exists, please enter a unique name.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                FormClosing -= frmEditPowerset_CancelClose;
                myPS.IsModified = true;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetDbPowerSetsPath();
            ImagePicker.FileName = myPS.ImageName;
            if (ImagePicker.ShowDialog(this) != DialogResult.OK) return;

            var imageFile = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(Path.Combine(I9Gfx.GetDbPowerSetsPath(), imageFile)))
            {
                MessageBox.Show($@"You must select an image from the {I9Gfx.GetDbPowerSetsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it.", @"Select Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                myPS.ImageName = imageFile;
                DisplayIcon();
            }
        }

        private string BuildFullName()
        {
            var str = $"{cbNameGroup.Text}.{txtNameSet.Text}";
            lblNameFull.Text = str;
            myPS.FullName = str;
            myPS.SetName = txtNameSet.Text;
            Text = $"Edit Powerset ({str})";

            return str;
        }

        private void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            if (cbAT.SelectedIndex > -1)
            {
                myPS.nArchetype = cbAT.SelectedIndex - 1;
                myPS.ATClass = DatabaseAPI.UidFromNidClass(cbAT.SelectedIndex - 1);
            }
            else
            {
                myPS.nArchetype = -1;
            }
        }

        private void cbLinkGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            FillLinkSetCombo();
        }

        private void cbLinkSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            if (chkNoLink.Checked)
            {
                myPS.UIDLinkSecondary = "";
                myPS.nIDLinkSecondary = -1;
            }
            else if (cbLinkSet.SelectedIndex > -1)
            {
                var uidPowerset = cbLinkGroup.Text + "." + cbLinkSet.Text;
                var num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                myPS.UIDLinkSecondary = uidPowerset;
                myPS.nIDLinkSecondary = num;
            }
        }

        private void cbMutexGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ListMutexSets();
        }

        private void cbNameGroup_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayNameData();
        }

        private void cbNameGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }

        private void cbNameGroup_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }

        private void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            if (cbSetType.SelectedIndex > -1)
                myPS.SetType = (Enums.ePowerSetType)cbSetType.SelectedIndex;
            if (myPS.SetType == Enums.ePowerSetType.Primary)
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
            if (Loading)
                return;
            FillTrunkSetCombo();
        }

        private void cbTrunkSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            if (chkNoTrunk.Checked)
            {
                myPS.UIDTrunkSet = "";
                myPS.nIDTrunkSet = -1;
            }
            else if (cbTrunkSet.SelectedIndex > -1)
            {
                var uidPowerset = cbTrunkGroup.Text + "." + cbTrunkSet.Text;
                var num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                myPS.UIDTrunkSet = uidPowerset;
                myPS.nIDTrunkSet = num;
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
            if (!string.IsNullOrEmpty(myPS.ImageName))
            {
                using var extendedBitmap = new ExtendedBitmap(I9Gfx.GetPowersetsPath() + myPS.ImageName);
                picIcon.Image = new Bitmap(extendedBitmap.Bitmap);
                btnIcon.Text = myPS.ImageName;
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
            lblNameUnique.Text = string.IsNullOrEmpty(myPS.GroupName) | string.IsNullOrEmpty(myPS.SetName)
                ? "This name is invalid."
                : PowersetFullNameIsUnique(Convert.ToString(myPS.nID, CultureInfo.InvariantCulture))
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
            if (myPS.UIDLinkSecondary == "")
            {
                return;
            }

            var index = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
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
                var index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
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
            if (myPS.UIDTrunkSet == "")
            {
                return;
            }

            var index = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
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
                var index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
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

        private void frmEditPowerset_Load(object sender, EventArgs e)
        {
            var ePowerSetType = Enums.ePowerSetType.None;
            ListPowers();
            txtName.Text = myPS.DisplayName;
            cbNameGroup.BeginUpdate();
            cbNameGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbNameGroup.Items.Add(key);
            }

            cbNameGroup.EndUpdate();
            cbNameGroup.Text = myPS.GroupName;
            txtNameSet.Text = myPS.SetName;
            txtDesc.Text = myPS.Description;
            FillTrunkGroupCombo();
            FillTrunkSetCombo();
            chkNoTrunk.Checked = myPS.UIDTrunkSet == "";
            FillLinkGroupCombo();
            FillLinkSetCombo();
            chkNoLink.Checked = myPS.UIDLinkSecondary == "";
            if (myPS.SetType == Enums.ePowerSetType.Primary)
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
            cbAT.SelectedIndex = myPS.nArchetype + 1;
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(Enum.GetNames(ePowerSetType.GetType()));
            cbSetType.EndUpdate();
            cbSetType.SelectedIndex = (int)myPS.SetType;
            ListMutexSets();
            Loading = false;
            DisplayNameData();
        }

        private void ListMutexSets()
        {
            lvMutexSets.BeginUpdate();
            lvMutexSets.Items.Clear();
            foreach (var m in myPS.UIDMutexSets)
            {
                lvMutexSets.Items.Add(m);
            }

            lvMutexSets.EndUpdate();
        }

        private void ListPowers()
        {
            lvPowers.BeginUpdate();
            lvPowers.Items.Clear();
            for (var index = 0; index < myPS.Power.Length; index++)
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

        private void lvMutexSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (Loading || cbMutexGroup.SelectedIndex < 0)
            {
                return;
            }

            myPS.UIDMutexSets = new string[lvMutexSets.SelectedIndices.Count];
            myPS.nIDMutexSets = new int[lvMutexSets.SelectedIndices.Count];
            var numArray = DatabaseAPI.NidSets(cbMutexGroup.SelectedText, "-1", Enums.ePowerSetType.None);
            for (var index = 0; index < lvMutexSets.SelectedIndices.Count; index++)
            {
                myPS.UIDMutexSets[index] = DatabaseAPI.Database.Powersets[numArray[lvMutexSets.SelectedIndices[index]]].FullName;
                myPS.nIDMutexSets[index] = DatabaseAPI.NidFromUidPowerset(myPS.UIDMutexSets[index]);
            }*/
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
            if (Loading)
            {
                return;
            }

            myPS.Description = txtDesc.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            myPS.DisplayName = txtName.Text;
        }

        private void txtNameSet_Leave(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            DisplayNameData();
        }

        private void txtNameSet_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            BuildFullName();
        }

        private void btnAddMutexSet_Click(object sender, EventArgs e)
        {
            using var psSelector = new frmPowersetSelector();

            var ret = psSelector.ShowDialog();
            if (ret != DialogResult.OK)
            {
                return;
            }

            var uidMutexList = myPS.UIDMutexSets.ToList();
            var nidMutexList = myPS.nIDMutexSets.ToList();

            uidMutexList.Add(psSelector.UidSet);
            nidMutexList.Add(psSelector.NidSet);

            myPS.UIDMutexSets = uidMutexList.ToArray();
            myPS.nIDMutexSets = nidMutexList.ToArray();

            ListMutexSets();
        }

        private void btnRemoveMutexSet_Click(object sender, EventArgs e)
        {
            var uidMutexList = new List<string>();
            var nidMutexList = new List<int>();

            /*
            myPS.UIDMutexSets = new string[lvMutexSets.SelectedIndices.Count];
            myPS.nIDMutexSets = new int[lvMutexSets.SelectedIndices.Count];
            var numArray = DatabaseAPI.NidSets(cbMutexGroup.SelectedText, "-1", Enums.ePowerSetType.None);
            for (var index = 0; index < lvMutexSets.SelectedIndices.Count; index++)
            {
                myPS.UIDMutexSets[index] = DatabaseAPI.Database.Powersets[numArray[lvMutexSets.SelectedIndices[index]]].FullName;
                myPS.nIDMutexSets[index] = DatabaseAPI.NidFromUidPowerset(myPS.UIDMutexSets[index]);
            }
            */

            if (uidMutexList.Count <= 0)
            {
                return;
            }

            if (lvMutexSets.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedItem = lvMutexSets.SelectedIndices[0];

            for (var i = 0; i < myPS.UIDMutexSets.Length; i++)
            {
                if (i == selectedItem)
                {
                    continue;
                }

                uidMutexList.Add(myPS.UIDMutexSets[i]);
                nidMutexList.Add(myPS.nIDMutexSets[i]);
            }

            myPS.UIDMutexSets = uidMutexList.ToArray();
            myPS.nIDMutexSets = nidMutexList.ToArray();

            ListMutexSets();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Join(", ", myPS.UIDMutexSets));
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            {
                return;
            }

            var items = Clipboard.GetText().Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var uidMutexList = new List<string>();
            var nidMutexList = new List<int>();

            if (myPS.UIDMutexSets.Length > 0)
            {
                var overwrite = MessageBox.Show(
                    "Powerset already contains mutex entries.\r\nDo you want to overwrite the existing ones?",
                    "Append mutex entries", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (overwrite)
                {
                    case DialogResult.Yes:
                        foreach (var item in items)
                        {
                            if (!DatabaseAPI.Database.Powersets.Any(e => e?.FullName == item))
                            {
                                continue;
                            }

                            uidMutexList.Add(item);
                            nidMutexList.Add(DatabaseAPI.NidFromUidPowerset(item));
                        }

                        break;
                    
                    case DialogResult.No:
                        foreach (var m in myPS.UIDMutexSets)
                        {
                            uidMutexList.Add(m);
                            nidMutexList.Add(DatabaseAPI.NidFromUidPowerset(m));
                        }

                        foreach (var item in items)
                        {
                            if (!DatabaseAPI.Database.Powersets.Any(e => e?.FullName == item))
                            {
                                continue;
                            }

                            if (uidMutexList.Contains(item))
                            {
                                continue;
                            }

                            uidMutexList.Add(item);
                            nidMutexList.Add(DatabaseAPI.NidFromUidPowerset(item));
                        }

                        break;

                    default:
                        return;
                }
            }
            else
            {
                foreach (var item in items)
                {
                    if (!DatabaseAPI.Database.Powersets.Any(e => e?.FullName == item))
                    {
                        continue;
                    }

                    uidMutexList.Add(item);
                    nidMutexList.Add(DatabaseAPI.NidFromUidPowerset(item));
                }
            }

            myPS.UIDMutexSets = uidMutexList.ToArray();
            myPS.nIDMutexSets = nidMutexList.ToArray();

            ListMutexSets();
        }
    }
}