
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Base.Display;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEditPowerset : Form
    {
        private bool Loading;
        public readonly IPowerset myPS;


        public frmEditPowerset(ref IPowerset iSet)
        {
            Load += frmEditPowerset_Load;
            Loading = true;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmEditPowerset));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon", CultureInfo.InvariantCulture);
            Name = nameof(frmEditPowerset);
            myPS = new Powerset(iSet);
        }

        private void AddListItem(int Index)
        {
            lvPowers.Items.Add(new ListViewItem(new[]
            {
                Convert.ToString(DatabaseAPI.Database.Power[myPS.Power[Index]].Level,CultureInfo.InvariantCulture),
                DatabaseAPI.Database.Power[myPS.Power[Index]].DisplayName,
                DatabaseAPI.Database.Power[myPS.Power[Index]].DescShort
            }));
            lvPowers.Items[Index].Selected = true;
            lvPowers.Items[Index].EnsureVisible();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnClearIcon_Click(object sender, EventArgs e)
        {
            myPS.ImageName = "";
            DisplayIcon();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            IPowerset ps = myPS;
            lblNameFull.Text = ps.GroupName + "." + ps.SetName;
            if (ps.GroupName == "" | ps.SetName == "")
            {
                int num1 = (int)Interaction.MsgBox("Powerset name '" + ps.FullName + " is invalid.", MsgBoxStyle.Exclamation, "No Can Do");
            }
            else if (!PowersetFullNameIsUnique(Convert.ToString(ps.nID)))
            {
                int num2 = (int)Interaction.MsgBox("Powerset name '" + ps.FullName + " already exists, please enter a unique name.", MsgBoxStyle.Exclamation, "No Can Do");
            }
            else
            {
                myPS.IsModified = true;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        void btnIcon_Click(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetPowersetsPath();
            ImagePicker.FileName = myPS.ImageName;
            if (ImagePicker.ShowDialog() != DialogResult.OK)
                return;
            string str = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(FileIO.AddSlash(ImagePicker.InitialDirectory) + str))
            {
                int num = (int)Interaction.MsgBox("You must select an image from the " + I9Gfx.GetPowersetsPath() + " folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it.", MsgBoxStyle.Information, "Ah...");
            }
            else
            {
                myPS.ImageName = str;
                DisplayIcon();
            }
        }

        string BuildFullName()
        {
            string str = cbNameGroup.Text + "." + txtNameSet.Text;
            lblNameFull.Text = str;
            myPS.FullName = str;
            myPS.SetName = txtNameSet.Text;
            return str;
        }

        void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            if (cbAT.SelectedIndex > -1)
            {
                myPS.nArchetype = cbAT.SelectedIndex - 1;
                myPS.ATClass = DatabaseAPI.UidFromNidClass(cbAT.SelectedIndex - 1);
            }
            else
                myPS.nArchetype = -1;
        }

        void cbLinkGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            FillLinkSetCombo();
        }

        void cbLinkSet_SelectedIndexChanged(object sender, EventArgs e)
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
                string uidPowerset = cbLinkGroup.Text + "." + cbLinkSet.Text;
                int num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                myPS.UIDLinkSecondary = uidPowerset;
                myPS.nIDLinkSecondary = num;
            }
        }

        void cbMutexGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ListMutexSets();
        }

        void cbNameGroup_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayNameData();
        }

        void cbNameGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }

        void cbNameGroup_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }

        void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
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

        void cbTrunkGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            FillTrunkSetCombo();
        }

        void cbTrunkSet_SelectedIndexChanged(object sender, EventArgs e)
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
                string uidPowerset = cbTrunkGroup.Text + "." + cbTrunkSet.Text;
                int num = DatabaseAPI.NidFromUidPowerset(uidPowerset);
                myPS.UIDTrunkSet = uidPowerset;
                myPS.nIDTrunkSet = num;
            }
        }

        void chkNoLink_CheckedChanged(object sender, EventArgs e)
        {
            cbLinkSet_SelectedIndexChanged(this, new EventArgs());
        }

        void chkNoTrunk_CheckedChanged(object sender, EventArgs e)
        {
            cbTrunkSet_SelectedIndexChanged(this, new EventArgs());
        }

        private void DisplayIcon()
        {
            if (!string.IsNullOrEmpty(myPS.ImageName))
            {
                using ExtendedBitmap extendedBitmap = new ExtendedBitmap(I9Gfx.GetPowersetsPath() + myPS.ImageName);
                picIcon.Image = new Bitmap(extendedBitmap.Bitmap);
                btnIcon.Text = myPS.ImageName;
            }
            else
            {
                using ExtendedBitmap extendedBitmap = new ExtendedBitmap(30,30);
                picIcon.Image = new Bitmap(extendedBitmap.Bitmap);
                btnIcon.Text = "Select Icon";
            }
        }

        void DisplayNameData()
        {
            IPowerset ps = myPS;
            lblNameFull.Text = BuildFullName();
            if (string.IsNullOrEmpty(ps.GroupName) | string.IsNullOrEmpty(ps.SetName))
            {
                lblNameUnique.Text = "This name is invalid.";
            }
            else if (PowersetFullNameIsUnique(Convert.ToString(ps.nID, CultureInfo.InvariantCulture)))
            {
                lblNameUnique.Text = "This name is unique.";
            }
            else
            {
                lblNameUnique.Text = "This name is NOT unique.";
            }
        }

        void FillLinkGroupCombo()
        {
            cbLinkGroup.BeginUpdate();
            cbLinkGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbLinkGroup.Items.Add(key);
            cbLinkGroup.EndUpdate();
            if (myPS.UIDLinkSecondary == "")
                return;
            int index = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
            if (index > -1)
                cbLinkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        void FillLinkSetCombo()
        {
            cbLinkSet.BeginUpdate();
            cbLinkSet.Items.Clear();
            if (cbLinkGroup.SelectedIndex > -1)
            {
                int index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDLinkSecondary);
                int[] indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbLinkGroup.SelectedText);
                int num = indexesByGroupName.Length - 1;
                for (int index2 = 0; index2 <= num; ++index2)
                {
                    cbLinkSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName);
                    if (index1 > -1 && DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName == DatabaseAPI.Database.Powersets[index1].SetName)
                        index1 = index2;
                }
                cbLinkSet.SelectedIndex = index1;
            }
            cbLinkSet.EndUpdate();
        }

        void FillTrunkGroupCombo()
        {
            cbTrunkGroup.BeginUpdate();
            cbTrunkGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbTrunkGroup.Items.Add(key);
            cbTrunkGroup.EndUpdate();
            if (myPS.UIDTrunkSet == "")
                return;
            int index = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
            if (index > -1)
                cbTrunkGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        void FillTrunkSetCombo()
        {
            cbTrunkSet.BeginUpdate();
            cbTrunkSet.Items.Clear();
            if (cbTrunkGroup.SelectedIndex > -1)
            {
                int index1 = DatabaseAPI.NidFromUidPowerset(myPS.UIDTrunkSet);
                int[] indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(cbTrunkGroup.SelectedText);
                int num = indexesByGroupName.Length - 1;
                for (int index2 = 0; index2 <= num; ++index2)
                {
                    cbTrunkSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName);
                    if (index1 > -1 && DatabaseAPI.Database.Powersets[indexesByGroupName[index2]].SetName == DatabaseAPI.Database.Powersets[index1].SetName)
                        index1 = index2;
                }
                cbTrunkSet.SelectedIndex = index1;
            }
            cbTrunkSet.EndUpdate();
        }

        void frmEditPowerset_Load(object sender, EventArgs e)
        {
            Enums.ePowerSetType ePowerSetType = Enums.ePowerSetType.None;
            ListPowers();
            txtName.Text = myPS.DisplayName;
            cbNameGroup.BeginUpdate();
            cbNameGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
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
            int num = DatabaseAPI.Database.Classes.Length - 1;
            for (int index = 0; index <= num; ++index)
                cbAT.Items.Add(DatabaseAPI.Database.Classes[index].DisplayName);
            cbAT.EndUpdate();
            cbAT.SelectedIndex = myPS.nArchetype + 1;
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(Enum.GetNames(ePowerSetType.GetType()));
            cbSetType.EndUpdate();
            cbSetType.SelectedIndex = (int)myPS.SetType;
            ListMutexGroups();
            ListMutexSets();
            Loading = false;
            DisplayNameData();
        }

        void ListMutexGroups()
        {
            cbMutexGroup.BeginUpdate();
            cbMutexGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbMutexGroup.Items.Add(key);
            cbMutexGroup.EndUpdate();
            if (myPS.nIDMutexSets.Length <= 0)
                return;
            int index = DatabaseAPI.NidFromUidPowerset(myPS.UIDMutexSets[0]);
            if (index > -1)
                cbMutexGroup.SelectedValue = DatabaseAPI.Database.Powersets[index].GroupName;
        }

        void ListMutexSets()
        {
            lvMutexSets.BeginUpdate();
            lvMutexSets.Items.Clear();
            if (cbMutexGroup.SelectedIndex > -1)
            {
                int[] numArray = DatabaseAPI.NidSets(cbMutexGroup.SelectedText, Convert.ToString(-1), Enums.ePowerSetType.None);
                int num1 = numArray.Length - 1;
                for (int index1 = 0; index1 <= num1; ++index1)
                {
                    lvMutexSets.Items.Add(DatabaseAPI.Database.Powersets[numArray[index1]].FullName);
                    int num2 = myPS.nIDMutexSets.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
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
            int num = myPS.Power.Length - 1;
            for (int Index = 0; Index <= num; ++Index)
                AddListItem(Index);
            if (lvPowers.Items.Count > 0)
            {
                lvPowers.Items[0].Selected = true;
                lvPowers.Items[0].EnsureVisible();
            }
            lvPowers.EndUpdate();
        }

        void lvMutexSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbMutexGroup.SelectedIndex < 0)
                return;
            IPowerset ps = myPS;
            ps.UIDMutexSets = new string[lvMutexSets.SelectedIndices.Count - 1 + 1];
            ps.nIDMutexSets = new int[lvMutexSets.SelectedIndices.Count - 1 + 1];
            int[] numArray = DatabaseAPI.NidSets(cbMutexGroup.SelectedText, Convert.ToString(-1), Enums.ePowerSetType.None);
            int num = lvMutexSets.SelectedIndices.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                ps.UIDMutexSets[index] = DatabaseAPI.Database.Powersets[numArray[lvMutexSets.SelectedIndices[index]]].FullName;
                ps.nIDMutexSets[index] = DatabaseAPI.NidFromUidPowerset(ps.UIDMutexSets[index]);
            }
        }

        static bool PowersetFullNameIsUnique(string iFullName, int skipId = -1)
        {
            if (string.IsNullOrEmpty(iFullName))
                return true;
            int num = DatabaseAPI.Database.Powersets.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (index != skipId && string.Equals(DatabaseAPI.Database.Powersets[index].FullName, iFullName, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myPS.Description = txtDesc.Text;
        }

        void txtName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myPS.DisplayName = txtName.Text;
        }

        void txtNameSet_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayNameData();
        }

        void txtNameSet_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            BuildFullName();
        }
    }
}