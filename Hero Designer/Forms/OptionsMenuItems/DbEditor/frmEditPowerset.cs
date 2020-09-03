using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Base.Display;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditPowerset : Form
    {
        public readonly IPowerset myPS;
        private bool Loading;


        public frmEditPowerset(ref IPowerset iSet)
        {
            Load += frmEditPowerset_Load;
            Loading = true;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEditPowerset));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon", CultureInfo.InvariantCulture);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            var ps = myPS;
            lblNameFull.Text = ps.GroupName + "." + ps.SetName;
            if ((ps.GroupName == "") | (ps.SetName == ""))
            {
                MessageBox.Show($"Powerset name '{ps.FullName}' is invalid.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (!PowersetFullNameIsUnique(Convert.ToString(ps.nID)))
            {
                MessageBox.Show($"Powerset name '{ps.FullName}' already exists, please enter a unique name.", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                myPS.IsModified = true;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        private void btnIcon_Click(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetPowersetsPath();
            ImagePicker.FileName = myPS.ImageName;
            if (ImagePicker.ShowDialog() != DialogResult.OK)
                return;
            var str = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(FileIO.AddSlash(ImagePicker.InitialDirectory) + str))
            {
                MessageBox.Show(
                    $"You must select an image from the {I9Gfx.GetPowersetsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it the folder and then select it.",
                    "Ah...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                myPS.ImageName = str;
                DisplayIcon();
            }
        }

        private string BuildFullName()
        {
            var str = cbNameGroup.Text + "." + txtNameSet.Text;
            lblNameFull.Text = str;
            myPS.FullName = str;
            myPS.SetName = txtNameSet.Text;
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
                myPS.SetType = (Enums.ePowerSetType) cbSetType.SelectedIndex;
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
            var ps = myPS;
            lblNameFull.Text = BuildFullName();
            if (string.IsNullOrEmpty(ps.GroupName) | string.IsNullOrEmpty(ps.SetName))
                lblNameUnique.Text = "This name is invalid.";
            else if (PowersetFullNameIsUnique(Convert.ToString(ps.nID, CultureInfo.InvariantCulture)))
                lblNameUnique.Text = "This name is unique.";
            else
                lblNameUnique.Text = "This name is NOT unique.";
        }

        private void FillLinkGroupCombo()
        {
            cbLinkGroup.BeginUpdate();
            cbLinkGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbLinkGroup.Items.Add(key);
            cbLinkGroup.EndUpdate();
            if (myPS.UIDLinkSecondary == "")
                return;
            var index = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
            if (index > -1)
                cbLinkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        private void FillLinkSetCombo()
        {
            cbLinkSet.BeginUpdate();
            cbLinkSet.Items.Clear();
            if (cbLinkGroup.SelectedIndex > -1)
            {
                var index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbLinkGroup.SelectedText);
                var num = indexesByGroupName.Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
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
                cbTrunkGroup.Items.Add(key);
            cbTrunkGroup.EndUpdate();
            if (myPS.UIDTrunkSet == "")
                return;
            var index = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
            if (index > -1)
                cbTrunkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        private void FillTrunkSetCombo()
        {
            cbTrunkSet.BeginUpdate();
            cbTrunkSet.Items.Clear();
            if (cbTrunkGroup.SelectedIndex > -1)
            {
                var index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbTrunkGroup.SelectedText);
                var num = indexesByGroupName.Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
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
                cbNameGroup.Items.Add(key);
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
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
                cbAT.Items.Add(DatabaseAPI.Database.Classes[index].DisplayName);
            cbAT.EndUpdate();
            cbAT.SelectedIndex = myPS.nArchetype + 1;
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(Enum.GetNames(ePowerSetType.GetType()));
            cbSetType.EndUpdate();
            cbSetType.SelectedIndex = (int) myPS.SetType;
            ListMutexGroups();
            ListMutexSets();
            Loading = false;
            DisplayNameData();
        }

        private void ListMutexGroups()
        {
            cbMutexGroup.BeginUpdate();
            cbMutexGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbMutexGroup.Items.Add(key);
            cbMutexGroup.EndUpdate();
            if (myPS.nIDMutexSets.Length <= 0)
                return;
            var index = DatabaseAPI.NidFromUidPowerset(myPS.UIDMutexSets[0]);
            if (index > -1)
                cbMutexGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        private void ListMutexSets()
        {
            lvMutexSets.BeginUpdate();
            lvMutexSets.Items.Clear();
            if (cbMutexGroup.SelectedIndex > -1)
            {
                var numArray = DatabaseAPI.NidSets(cbMutexGroup.SelectedText, Convert.ToString(-1),
                    Enums.ePowerSetType.None);
                var num1 = numArray.Length - 1;
                for (var index1 = 0; index1 <= num1; ++index1)
                {
                    lvMutexSets.Items.Add(DatabaseAPI.Database.Powersets[numArray[index1]].FullName);
                    var num2 = myPS.nIDMutexSets.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                    {
                        if (numArray[index1] != myPS.nIDMutexSets[index2])
                            continue;
                        lvMutexSets.SetSelected(index1, true);
                        break;
                    }
                }
            }

            lvMutexSets.EndUpdate();
        }

        private void ListPowers()
        {
            lvPowers.BeginUpdate();
            lvPowers.Items.Clear();
            var num = myPS.Power.Length - 1;
            for (var Index = 0; Index <= num; ++Index)
                AddListItem(Index);
            if (lvPowers.Items.Count > 0)
            {
                lvPowers.Items[0].Selected = true;
                lvPowers.Items[0].EnsureVisible();
            }

            lvPowers.EndUpdate();
        }

        private void lvMutexSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbMutexGroup.SelectedIndex < 0)
                return;
            var ps = myPS;
            ps.UIDMutexSets = new string[lvMutexSets.SelectedIndices.Count - 1 + 1];
            ps.nIDMutexSets = new int[lvMutexSets.SelectedIndices.Count - 1 + 1];
            var numArray =
                DatabaseAPI.NidSets(cbMutexGroup.SelectedText, Convert.ToString(-1), Enums.ePowerSetType.None);
            var num = lvMutexSets.SelectedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
            {
                ps.UIDMutexSets[index] = DatabaseAPI.Database.Powersets[numArray[lvMutexSets.SelectedIndices[index]]]
                    .FullName;
                ps.nIDMutexSets[index] = DatabaseAPI.NidFromUidPowerset(ps.UIDMutexSets[index]);
            }
        }

        private static bool PowersetFullNameIsUnique(string iFullName, int skipId = -1)
        {
            if (string.IsNullOrEmpty(iFullName))
                return true;
            var num = DatabaseAPI.Database.Powersets.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (index != skipId && string.Equals(DatabaseAPI.Database.Powersets[index].FullName, iFullName,
                    StringComparison.OrdinalIgnoreCase))
                    return false;
            return true;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myPS.Description = txtDesc.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myPS.DisplayName = txtName.Text;
        }

        private void txtNameSet_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayNameData();
        }

        private void txtNameSet_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }
    }
}