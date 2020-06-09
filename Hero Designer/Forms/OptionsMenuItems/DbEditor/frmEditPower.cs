
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Import;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEditPower : Form
    {
        readonly Requirement backup_Requires;
        ExtendedBitmap bxEnhPicked;
        ExtendedBitmap bxEnhPicker;
        ExtendedBitmap bxSet;
        ExtendedBitmap bxSetList;
        readonly int enhAcross;
        readonly int enhPadding;
        public IPower myPower;
        bool ReqChanging;
        bool Updating;
        bool cancelClose;

        public frmEditPower(IPower iPower)
        {
            Load += frmEditPower_Load;
            enhPadding = 6;
            enhAcross = 8;
            Updating = true;
            ReqChanging = false;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmEditPower));
            Icon = (Icon)componentResourceManager.GetObject("$this.icon", CultureInfo.InvariantCulture);
            Name = nameof(frmEditPower);
            myPower = new Power(iPower);
            backup_Requires = new Requirement(myPower.Requires);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnCSVImport_Click(object sender, EventArgs e)
        {
            string str = Clipboard.GetDataObject()?.GetData("System.String", true).ToString();
            if (!string.IsNullOrWhiteSpace(str))
                return;
            if (new PowerData(str.Replace("\t", ",")).IsValid)
            {
                Interaction.MsgBox("Import successful.");
                refresh_PowerData();
            }
            else
            {
                Interaction.MsgBox("Import failed. No changes made.");
            }
        }

        void btnFullCopy_Click(object sender, EventArgs e)
        {
            DataFormats.Format format = DataFormats.GetFormat("mhdPowerBIN");
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            myPower.StoreTo(ref writer);
            writer.Close();
            Clipboard.SetDataObject(new DataObject(format.Name, memoryStream.GetBuffer()));
            memoryStream.Close();
        }

        void btnFullPaste_Click(object sender, EventArgs e)
        {
            DataFormats.Format format = DataFormats.GetFormat("mhdPowerBIN");
            string groupName = myPower.GroupName;
            string setName = myPower.SetName;
            if (!Clipboard.ContainsData(format.Name))
            {
                Interaction.MsgBox("No power data on the clipboard!", MsgBoxStyle.Information, "Unable to Paste");
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream((byte[])Clipboard.GetDataObject()?.GetData(format.Name) ?? throw new InvalidOperationException());
                BinaryReader reader = new BinaryReader(memoryStream);
                myPower = new Power(reader) {GroupName = groupName, SetName = setName};
                SetFullName();
                refresh_PowerData();
                reader.Close();
                memoryStream.Close();
            }
        }

        void btnFXAdd_Click(object sender, EventArgs e)
        {
            IEffect iFX = new Effect();
            using frmPowerEffect frmPowerEffect = new frmPowerEffect(iFX);
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;
            IPower power1 = myPower;
            IPower power2 = power1;
            IEffect[] effectArray = (IEffect[])Utils.CopyArray(power2.Effects, new IEffect[power1.Effects.Length + 1]);
            power2.Effects = effectArray;
            power1.Effects[power1.Effects.Length - 1] = (IEffect)frmPowerEffect.myFX.Clone();
            RefreshFXData();
            lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        void btnFXDown_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvFX.SelectedIndices[0];
            if (selectedIndex > myPower.Effects.Length - 2)
                return;
            IEffect[] effectArray = {
                (IEffect) myPower.Effects[selectedIndex].Clone(),
                (IEffect) myPower.Effects[selectedIndex + 1].Clone()
            };
            myPower.Effects[selectedIndex] = (IEffect)effectArray[1].Clone();
            myPower.Effects[selectedIndex + 1] = (IEffect)effectArray[0].Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex + 1;
        }

        void btnFXDuplicate_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            IEffect iFX = (IEffect)myPower.Effects[lvFX.SelectedIndices[0]].Clone();
            using frmPowerEffect frmPowerEffect = new frmPowerEffect(iFX);
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;
            IPower power1 = myPower;
            IPower power2 = power1;
            IEffect[] effectArray = (IEffect[])Utils.CopyArray(power2.Effects, new IEffect[power1.Effects.Length + 1]);
            power2.Effects = effectArray;
            power1.Effects[power1.Effects.Length - 1] = (IEffect)frmPowerEffect.myFX.Clone();
            RefreshFXData();
            lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        void btnFXEdit_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvFX.SelectedIndices[0];
            IEffect iFX = (IEffect)myPower.Effects[selectedIndex].Clone();
            using frmPowerEffect frmPowerEffect = new frmPowerEffect(iFX);
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;
            myPower.Effects[selectedIndex] = (IEffect)frmPowerEffect.myFX.Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex;
        }

        void btnFXRemove_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndex < 0)
                return;
            IEffect[] effectArray = new IEffect[myPower.Effects.Length - 1 + 1];
            int selectedIndex = lvFX.SelectedIndex;
            int num1 = effectArray.Length - 1;
            for (int index = 0; index <= num1; ++index)
                effectArray[index] = (IEffect)myPower.Effects[index].Clone();
            myPower.Effects = new IEffect[myPower.Effects.Length - 2 + 1];
            int index1 = 0;
            int num2 = effectArray.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                myPower.Effects[index1] = effectArray[index2];
                ++index1;
            }
            RefreshFXData();
            if (lvFX.Items.Count > selectedIndex)
                lvFX.SelectedIndex = selectedIndex;
            else if (lvFX.Items.Count > selectedIndex - 1)
                lvFX.SelectedIndex = selectedIndex - 1;
        }

        void btnFXUp_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvFX.SelectedIndices[0];
            IEffect[] effectArray = new IEffect[2];
            if (selectedIndex < 1)
                return;
            effectArray[0] = (IEffect)myPower.Effects[selectedIndex].Clone();
            effectArray[1] = (IEffect)myPower.Effects[selectedIndex - 1].Clone();
            myPower.Effects[selectedIndex] = (IEffect)effectArray[1].Clone();
            myPower.Effects[selectedIndex - 1] = (IEffect)effectArray[0].Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex - 1;
        }

        void btnMutexAdd_Click(object sender, EventArgs e)
        {
            string b = Interaction.InputBox("Please enter a new group name. It must be different to all the others", "Add Mutex Group", "New_Group").Replace(" ", "_");
            int count = clbMutex.Items.Count;
            int index = 0;
            if (index > count)
                return;
            if (string.Equals(clbMutex.Items[index].ToString(), b, StringComparison.OrdinalIgnoreCase))
            {
                Interaction.MsgBox(("'" + b + "' is not unique!"), MsgBoxStyle.Information, "Unable to add");
            }
            else
            {
                clbMutex.Items.Add(b, true);
                clbMutex.SelectedIndex = clbMutex.Items.Count - 1;
            }
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            IPower power = myPower;
            lblNameFull.Text = $@"{power.GroupName}.{power.SetName}.{power.PowerName}";
            if (string.IsNullOrWhiteSpace(power.GroupName) | string.IsNullOrWhiteSpace(power.SetName) | string.IsNullOrWhiteSpace(power.PowerName))
            {
                _ = MessageBox.Show(@$"Power name ({power.FullName}) is invalid.", @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (!PowerFullNameIsUnique(Convert.ToString(power.PowerIndex, CultureInfo.InvariantCulture)))
            {
                _ = MessageBox.Show(@$"Power name ({power.FullName}) already exists. Please enter a unique name.", @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Array.Sort(myPower.UIDSubPower);
                Store_Req_Classes();
                myPower.IsModified = true;
                if (myPower.VariableEnabled)
                {
                    if (myPower.VariableMin >= myPower.VariableMax)
                    {
                        myPower.VariableMin = 0;
                        if (myPower.VariableMax == 0)
                            myPower.VariableMax = 1;
                    }
                    if (myPower.MaxTargets > 1 & myPower.MaxTargets != myPower.VariableMax)
                        myPower.VariableMax = myPower.MaxTargets;
                }
                myPower.GroupMembership = new string[clbMutex.CheckedItems.Count - 1 + 1];
                myPower.NGroupMembership = new int[clbMutex.CheckedItems.Count - 1 + 1];
                int checkedMutexCount = clbMutex.CheckedItems.Count - 1;
                for (int index = 0; index <= checkedMutexCount; ++index)
                {
                    myPower.GroupMembership[index] = Convert.ToString(clbMutex.CheckedItems[index], CultureInfo.InvariantCulture);
                    myPower.NGroupMembership[index] = clbMutex.CheckedIndices[index];
                }
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        void btnPrDown_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            int num = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            bool flag = lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1;
            int index1 = num;
            int index2 = index1 + 1;
            string[][] strArray1 = new string[2][];
            string[] strArray2 = new string[2];
            strArray1[0] = strArray2;
            string[] strArray3 = new string[2];
            strArray1[1] = strArray3;
            if (flag)
            {
                if (num > myPower.Requires.PowerIDNot.Length - 2)
                    return;
                strArray1[0][0] = myPower.Requires.PowerIDNot[index1][0];
                strArray1[0][1] = myPower.Requires.PowerIDNot[index1][1];
                strArray1[1][0] = myPower.Requires.PowerIDNot[index2][0];
                strArray1[1][1] = myPower.Requires.PowerIDNot[index2][1];
                myPower.Requires.PowerIDNot[index1][0] = strArray1[1][0];
                myPower.Requires.PowerIDNot[index1][1] = strArray1[1][1];
                myPower.Requires.PowerIDNot[index2][0] = strArray1[0][0];
                myPower.Requires.PowerIDNot[index2][1] = strArray1[0][1];
                index2 = lvPrListing.SelectedIndices[0] + 1;
            }
            else
            {
                if (num > myPower.Requires.PowerID.Length - 2)
                    return;
                strArray1[0][0] = myPower.Requires.PowerID[index1][0];
                strArray1[0][1] = myPower.Requires.PowerID[index1][1];
                strArray1[1][0] = myPower.Requires.PowerID[index2][0];
                strArray1[1][1] = myPower.Requires.PowerID[index2][1];
                myPower.Requires.PowerID[index1][0] = strArray1[1][0];
                myPower.Requires.PowerID[index1][1] = strArray1[1][1];
                myPower.Requires.PowerID[index2][0] = strArray1[0][0];
                myPower.Requires.PowerID[index2][1] = strArray1[0][1];
            }
            FillTab_Req();
            lvPrListing.Items[index2].Selected = true;
            lvPrListing.Items[index2].EnsureVisible();
        }

        void btnPrReset_Click(object sender, EventArgs e)
        {
            myPower.Requires = new Requirement(backup_Requires);
            FillTab_Req();
        }

        void btnPrSetNone_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            if (rbPrPowerA.Checked)
            {
                if (!string.IsNullOrWhiteSpace(myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1]))
                {
                    myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][0] = myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1];
                    myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1] = "";
                }
                else
                    rbPrRemove_Click(this, new EventArgs());
            }
            else
                myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1] = "";
            FillTab_Req();
        }

        void btnPrUp_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            int num = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            bool flag = lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1;
            if (num < 1)
                return;
            int index1 = num;
            int index2 = index1 - 1;
            string[][] strArray1 = new string[2][];
            string[] strArray2 = new string[2];
            strArray1[0] = strArray2;
            string[] strArray3 = new string[2];
            strArray1[1] = strArray3;
            if (flag)
            {
                strArray1[0][0] = myPower.Requires.PowerIDNot[index1][0];
                strArray1[0][1] = myPower.Requires.PowerIDNot[index1][1];
                strArray1[1][0] = myPower.Requires.PowerIDNot[index2][0];
                strArray1[1][1] = myPower.Requires.PowerIDNot[index2][1];
                myPower.Requires.PowerIDNot[index1][0] = strArray1[1][0];
                myPower.Requires.PowerIDNot[index1][1] = strArray1[1][1];
                myPower.Requires.PowerIDNot[index2][0] = strArray1[0][0];
                myPower.Requires.PowerIDNot[index2][1] = strArray1[0][1];
                index2 = lvPrListing.SelectedIndices[0] - 1;
            }
            else
            {
                strArray1[0][0] = myPower.Requires.PowerID[index1][0];
                strArray1[0][1] = myPower.Requires.PowerID[index1][1];
                strArray1[1][0] = myPower.Requires.PowerID[index2][0];
                strArray1[1][1] = myPower.Requires.PowerID[index2][1];
                myPower.Requires.PowerID[index1][0] = strArray1[1][0];
                myPower.Requires.PowerID[index1][1] = strArray1[1][1];
                myPower.Requires.PowerID[index2][0] = strArray1[0][0];
                myPower.Requires.PowerID[index2][1] = strArray1[0][1];
            }
            FillTab_Req();
            lvPrListing.Items[index2].Selected = true;
            lvPrListing.Items[index2].EnsureVisible();
        }

        void btnSPAdd_Click(object sender, EventArgs e)
        {
            if (lvSPPower.SelectedItems.Count < 1)
                return;
            string b = Convert.ToString(lvSPPower.SelectedItems[0].Tag, CultureInfo.InvariantCulture);
            int num = myPower.UIDSubPower.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (string.Equals(myPower.UIDSubPower[index], b, StringComparison.OrdinalIgnoreCase))
                    return;
            }
            IPower power = myPower;
            string[] strArray = (string[])Utils.CopyArray(power.UIDSubPower, new string[myPower.UIDSubPower.Length + 1]);
            power.UIDSubPower = strArray;
            myPower.UIDSubPower[myPower.UIDSubPower.Length - 1] = b;
            SPFillList();
            lvSPSelected.Items[lvSPSelected.Items.Count - 1].Selected = true;
            lvSPSelected.Items[lvSPSelected.Items.Count - 1].EnsureVisible();
        }

        void btnSPRemove_Click(object sender, EventArgs e)
        {
            if (lvSPSelected.SelectedItems.Count < 1)
                return;
            string text = lvSPSelected.SelectedItems[0].Text;
            string[] strArray = new string[myPower.UIDSubPower.Length - 2 + 1];
            int index1 = 0;
            int subPowerCount = myPower.UIDSubPower.Length - 1;
            for (int index2 = 0; index2 <= subPowerCount; ++index2)
            {
                if (string.Equals(myPower.UIDSubPower[index2], text, StringComparison.OrdinalIgnoreCase))
                    continue;
                strArray[index1] = myPower.UIDSubPower[index2];
                ++index1;
            }
            myPower.UIDSubPower = new string[strArray.Length - 1 + 1];
            Array.Copy(strArray, myPower.UIDSubPower, strArray.Length);
            SPFillList();
            int num2 = index1 - 1;
            if (num2 > lvSPSelected.Items.Count - 1)
            {
                int num3 = lvSPSelected.Items.Count - 1;
            }
            else if (num2 >= 0)
            {
            }

            SPFillList();
            if (lvSPSelected.Items.Count <= 0)
                return;
            lvSPSelected.Items[lvSPSelected.Items.Count - 1].Selected = true;
            lvSPSelected.Items[lvSPSelected.Items.Count - 1].EnsureVisible();
        }

        void cbEffectArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.EffectArea = (Enums.eEffectArea)cbEffectArea.SelectedIndex;
        }

        void cbForcedClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            int index = cbForcedClass.SelectedIndex - 1;
            myPower.ForcedClass = !(index < 0 | index > DatabaseAPI.Database.Classes.Length - 1) ? DatabaseAPI.Database.Classes[index].ClassName : "";
        }

        void cbNameGroup_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            DisplayNameData();
            FillComboNameSet();
        }

        void cbNameGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.GroupName = cbNameGroup.Text;
            SetFullName();
        }

        void cbNameGroup_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.GroupName = cbNameGroup.Text;
            SetFullName();
        }

        void cbNameSet_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            DisplayNameData();
        }

        void cbNameSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SetName = cbNameSet.Text;
            SetFullName();
        }

        void cbNameSet_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SetName = cbNameSet.Text;
            SetFullName();
        }

        void cbNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AIReport = (Enums.eNotify)cbNotify.SelectedIndex;
        }

        void cbPowerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.PowerType = (Enums.ePowerType)cbPowerType.SelectedIndex;
        }

        private void CheckScaleValues()
        {
            if (Conversion.Val(udScaleMin.Text) >= Conversion.Val(udScaleMax.Text))
            {
                udScaleMin.BackColor = Color.Coral;
                udScaleMax.BackColor = Color.Coral;
            }
            else
            {
                udScaleMin.BackColor = SystemColors.Window;
                udScaleMax.BackColor = SystemColors.Window;
            }
        }

        void chkAltSub_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SubIsAltColour = chkAltSub.Checked;
        }

        void chkAlwaysToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AlwaysToggle = chkAlwaysToggle.Checked;
        }

        void chkBoostBoostable_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.BoostBoostable = chkPRFrontLoad.Checked;
        }

        void chkBoostUsePlayerLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.BoostUsePlayerLevel = chkPRFrontLoad.Checked;
        }

        void chkBuffCycle_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.ClickBuff = chkBuffCycle.Checked;
        }

        void chkGraphFix_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SkipMax = chkGraphFix.Checked;
        }

        void chkHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.HiddenPower = chkHidden.Checked;
        }

        void chkIgnoreStrength_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.IgnoreStrength = chkIgnoreStrength.Checked;
        }

        void chkLos_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.TargetLoS = chkLos.Checked;
        }

        void chkMutexAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.MutexAuto = chkMutexAuto.Checked;
        }

        void chkMutexSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.MutexIgnore = chkMutexSkip.Checked;
        }

        void chkNoAUReq_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.NeverAutoUpdateRequirements = chkNoAUReq.Checked;
        }

        void chkNoAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.NeverAutoUpdate = chkNoAutoUpdate.Checked;
        }

        void chkPRFrontLoad_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AllowFrontLoading = chkPRFrontLoad.Checked;
        }

        void chkScale_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (!myPower.VariableEnabled)
            {
                udScaleMin.Value = new decimal(0);
                udScaleMax.Value = myPower.MaxTargets <= 1 ? new decimal(3) : new decimal(myPower.MaxTargets);
            }
            myPower.VariableEnabled = chkScale.Checked;
            udScaleMax.Enabled = myPower.VariableEnabled;
            udScaleMin.Enabled = myPower.VariableEnabled;
            txtScaleName.Enabled = myPower.VariableEnabled;
        }

        void chkSortOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SortOverride = chkSortOverride.Checked;
        }

        void chkSubInclude_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.IncludeFlag = chkSubInclude.Checked;
        }

        void chkSummonDisplayEntity_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.ShowSummonAnyway = chkSummonDisplayEntity.Checked;
        }

        void chkSummonStealAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AbsorbSummonAttributes = chkSummonStealAttributes.Checked;
        }

        void chkSummonStealEffects_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AbsorbSummonEffects = chkSummonStealEffects.Checked;
        }

        void clbFlags_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (Updating)
                return;
            if (rbFlagCastThrough.Checked)
            {
                if (e.Index == 0)
                    myPower.CastThroughHold = e.NewValue > CheckState.Unchecked;
            }
            else
            {
                int[] numArray = new int[28];
                int num1 = 0;
                int num2 = 1;
                int num3 = numArray.Length - 1;
                for (int index = 1; index <= num3; ++index)
                {
                    numArray[index] = num2;
                    num2 *= 2;
                }
                if (rbFlagAutoHit.Checked)
                    num1 = (int)myPower.EntitiesAutoHit;
                else if (rbFlagAffected.Checked)
                    num1 = (int)myPower.EntitiesAffected;
                else if (rbFlagTargets.Checked)
                    num1 = (int)myPower.Target;
                else if (rbFlagTargetsSec.Checked)
                    num1 = (int)myPower.TargetSecondary;
                else if (rbFlagCast.Checked)
                    num1 = (int)myPower.CastFlags;
                else if (rbFlagVector.Checked)
                    num1 = (int)myPower.AttackTypes;
                else if (rbFlagRequired.Checked)
                    num1 = (int)myPower.ModesRequired;
                else if (rbFlagDisallow.Checked)
                    num1 = (int)myPower.ModesDisallowed;
                if (e.CurrentValue == CheckState.Unchecked & e.NewValue == CheckState.Checked)
                    num1 += numArray[e.Index];
                else if (e.CurrentValue == CheckState.Checked & e.NewValue == CheckState.Unchecked)
                    num1 -= numArray[e.Index];
                if (rbFlagAutoHit.Checked)
                    myPower.EntitiesAutoHit = (Enums.eEntity)num1;
                else if (rbFlagAffected.Checked)
                    myPower.EntitiesAffected = (Enums.eEntity)num1;
                else if (rbFlagTargets.Checked)
                    myPower.Target = (Enums.eEntity)num1;
                else if (rbFlagTargetsSec.Checked)
                    myPower.TargetSecondary = (Enums.eEntity)num1;
                else if (rbFlagCast.Checked)
                    myPower.CastFlags = (Enums.eCastFlags)num1;
                else if (rbFlagVector.Checked)
                    myPower.AttackTypes = (Enums.eVector)num1;
                else if (rbFlagRequired.Checked)
                    myPower.ModesRequired = (Enums.eModeFlags)num1;
                else if (rbFlagDisallow.Checked)
                    myPower.ModesDisallowed = (Enums.eModeFlags)num1;
            }
        }

        void DisplayNameData()
        {
            IPower power = myPower;
            lblNameFull.Text = $@"{power.GroupName}.{power.SetName}.{power.PowerName}";
            if (!string.IsNullOrWhiteSpace(power.GroupName) | !string.IsNullOrWhiteSpace(power.SetName) | !string.IsNullOrWhiteSpace(power.PowerName))
                lblNameUnique.Text = @"This name is invalid.";
            else if (PowerFullNameIsUnique(Convert.ToString(power.PowerIndex, CultureInfo.InvariantCulture)))
                lblNameUnique.Text = @"This name is unique.";
            else
                lblNameUnique.Text = @"This name is NOT unique.";
        }

        void DrawAcceptedSets()
        {
            bxSet = new ExtendedBitmap(pbInvSetUsed.Width, pbInvSetUsed.Height);
            int enhPadding1 = enhPadding;
            int enhPadding2 = enhPadding;
            using Font font = new Font(Font, FontStyle.Bold);
            using StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
            using SolidBrush solidBrush1 = new SolidBrush(Color.Black);
            using SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(0, byte.MaxValue, 0));
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            using SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxSet.Graphics.FillRectangle(solidBrush3, bxSet.ClipRect);
            int num = myPower.SetTypes.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxSet.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, destRect, I9Gfx.GetImageRect((int)myPower.SetTypes[index]), GraphicsUnit.Pixel);
                var s = myPower.SetTypes[index] switch
                {
                    Enums.eSetType.MeleeST => "M\r\nST",
                    Enums.eSetType.RangedST => "R\r\nST",
                    Enums.eSetType.RangedAoE => "R\r\nAoE",
                    Enums.eSetType.MeleeAoE => "M\r\nAoE",
                    Enums.eSetType.Snipe => "S",
                    _ => ""
                };
                RectangleF layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                --layoutRectangle.X;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.Y;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.X;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.X;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.Y;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.Y;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.X;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.X;
                bxSet.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                bxSet.Graphics.DrawString(s, font, solidBrush2, layoutRectangle, format);
                enhPadding2 += 30 + enhPadding;
            }
            pbInvSetUsed.CreateGraphics().DrawImageUnscaled(bxSet.Bitmap, 0, 0);
        }

        void DrawSetList()
        {
            Enums.eSetType eSetType = Enums.eSetType.Untyped;
            bxSetList = new ExtendedBitmap(pbInvSetList.Width, pbInvSetList.Height);
            int enhPadding1 = enhPadding;
            int enhPadding2 = enhPadding;
            int num1 = 0;
            using Font font = new Font(Font, FontStyle.Bold);
            using StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
            using SolidBrush solidBrush1 = new SolidBrush(Color.Black);
            using SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(0, byte.MaxValue, 0));
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            string[] names = Enum.GetNames(eSetType.GetType());
            using SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(0,0,0));
            bxSetList.Graphics.FillRectangle(solidBrush3, bxSetList.ClipRect);
            int num2 = names.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxSetList.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, destRect, I9Gfx.GetImageRect(index), GraphicsUnit.Pixel);
                var s = (Enums.eSetType) index switch
                {
                    Enums.eSetType.MeleeST => "M\r\nST",
                    Enums.eSetType.RangedST => "R\r\nST",
                    Enums.eSetType.RangedAoE => "R\r\nAoE",
                    Enums.eSetType.MeleeAoE => "M\r\nAoE",
                    Enums.eSetType.Snipe => "S",
                    Enums.eSetType.UniversalDamage => "Dmg",
                    _ => ""
                };
                RectangleF layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                --layoutRectangle.X;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.Y;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.X;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.X;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.Y;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                ++layoutRectangle.Y;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.X;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                --layoutRectangle.X;
                bxSetList.Graphics.DrawString(s, font, solidBrush1, layoutRectangle, format);
                layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
                bxSetList.Graphics.DrawString(s, font, solidBrush2, layoutRectangle, format);
                enhPadding2 += 30 + enhPadding;
                ++num1;
                if (num1 != enhAcross)
                    continue;
                num1 = 0;
                enhPadding2 = enhPadding;
                enhPadding1 += 30 + enhPadding;
            }
            pbInvSetList.CreateGraphics().DrawImageUnscaled(bxSetList.Bitmap, 0, 0);
        }

        void FillAdvAtrList()
        {
            int num1 = 0;
            Type type = myPower.EntitiesAutoHit.GetType();
            bool flag = true;
            bool updating = Updating;
            Updating = true;
            clbFlags.BeginUpdate();
            clbFlags.Items.Clear();
            if (rbFlagAutoHit.Checked)
            {
                type = myPower.EntitiesAutoHit.GetType();
                num1 = (int)myPower.EntitiesAutoHit;
            }
            else if (rbFlagAffected.Checked)
            {
                type = myPower.EntitiesAffected.GetType();
                num1 = (int)myPower.EntitiesAffected;
            }
            else if (rbFlagTargets.Checked)
            {
                type = myPower.Target.GetType();
                num1 = (int)myPower.Target;
            }
            else if (rbFlagTargetsSec.Checked)
            {
                type = myPower.TargetSecondary.GetType();
                num1 = (int)myPower.TargetSecondary;
            }
            else if (rbFlagCast.Checked)
            {
                type = myPower.CastFlags.GetType();
                num1 = (int)myPower.CastFlags;
            }
            else if (rbFlagVector.Checked)
            {
                type = myPower.AttackTypes.GetType();
                num1 = (int)myPower.AttackTypes;
            }
            else if (rbFlagRequired.Checked)
            {
                type = myPower.ModesRequired.GetType();
                num1 = (int)myPower.ModesRequired;
            }
            else if (rbFlagDisallow.Checked)
            {
                type = myPower.ModesDisallowed.GetType();
                num1 = (int)myPower.ModesDisallowed;
            }
            else if (rbFlagCastThrough.Checked)
            {
                flag = false;
                clbFlags.Items.Add("Mez", myPower.CastThroughHold);
            }
            if (flag)
            {
                string[] names = Enum.GetNames(type);
                int[] values = (int[])Enum.GetValues(type);
                int num2 = values.Length - 1;
                for (int index = 0; index <= num2; ++index)
                {
                    bool isChecked = (values[index] & num1) != 0;
                    clbFlags.Items.Add(names[index], isChecked);
                }
            }
            clbFlags.EndUpdate();
            Updating = updating;
        }

        void FillCombo_Attribs()
        {
            Enums.ePowerType ePowerType = Enums.ePowerType.Click;
            bool updating = Updating;
            Updating = true;
            cbEffectArea.BeginUpdate();
            cbNotify.BeginUpdate();
            cbPowerType.BeginUpdate();
            cbEffectArea.Items.Clear();
            cbNotify.Items.Clear();
            cbPowerType.Items.Clear();
            cbEffectArea.Items.AddRange(Enum.GetNames(myPower.EffectArea.GetType()));
            cbNotify.Items.AddRange(Enum.GetNames(myPower.AIReport.GetType()));
            string[] names = Enum.GetNames(ePowerType.GetType());
            int num = names.Length - 1;
            for (int index = 0; index <= num; ++index)
                names[index] = names[index].Replace("_", "");
            cbPowerType.Items.AddRange(names);
            cbEffectArea.EndUpdate();
            cbNotify.EndUpdate();
            cbPowerType.EndUpdate();
            Updating = updating;
        }

        void FillCombo_Basic()
        {
            bool updating = Updating;
            Updating = true;
            cbNameGroup.BeginUpdate();
            cbNameGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbNameGroup.Items.Add(key);
            cbNameGroup.EndUpdate();
            FillComboNameSet();
            cbForcedClass.BeginUpdate();
            cbForcedClass.Items.Clear();
            cbForcedClass.Items.Add("None");
            int num2 = DatabaseAPI.Database.Classes.Length - 1;
            for (int index = 0; index <= num2; ++index)
                cbForcedClass.Items.Add(DatabaseAPI.Database.Classes[index].ClassName);
            cbForcedClass.EndUpdate();
            Updating = updating;
        }

        void FillComboBoxes()
        {
            Enums.eEnhance eEnhance = Enums.eEnhance.X_RechargeTime;
            lvDisablePass1.BeginUpdate();
            lvDisablePass1.Items.Clear();
            lvDisablePass1.Items.AddRange(Enum.GetNames(eEnhance.GetType()));
            lvDisablePass1.EndUpdate();
            lvDisablePass4.BeginUpdate();
            lvDisablePass4.Items.Clear();
            lvDisablePass4.Items.AddRange(Enum.GetNames(eEnhance.GetType()));
            lvDisablePass4.EndUpdate();
        }

        void FillComboNameSet()
        {
            cbNameSet.BeginUpdate();
            cbNameSet.Items.Clear();
            int[] indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(myPower.GroupName);
            int num1 = indexesByGroupName.Length - 1;
            for (int index = 0; index <= num1; ++index)
                cbNameSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
            cbNameSet.EndUpdate();
        }

        void FillTab_Attribs()
        {
            IPower power = myPower;
            string Style = "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###";
            txtLevel.Text = Convert.ToString(power.Level, CultureInfo.InvariantCulture);
            txtAcc.Text = Strings.Format(power.Accuracy, Style);
            txtInterrupt.Text = Strings.Format(power.InterruptTime, Style);
            txtCastTime.Text = Strings.Format(power.CastTimeReal, Style);
            txtRechargeTime.Text = Strings.Format(power.RechargeTime, Style);
            txtActivate.Text = Strings.Format(power.ActivatePeriod, Style);
            txtEndCost.Text = Strings.Format(power.EndCost, Style);
            txtRange.Text = Strings.Format(power.Range, Style);
            txtRangeSec.Text = Strings.Format(power.RangeSecondary, Style);
            txtRadius.Text = Convert.ToString(power.Radius, CultureInfo.InvariantCulture);
            txtArc.Text = Convert.ToString(power.Arc, CultureInfo.InvariantCulture);
            txtMaxTargets.Text = Convert.ToString(power.MaxTargets, CultureInfo.InvariantCulture);
            cbPowerType.SelectedIndex = (int)power.PowerType;
            cbEffectArea.SelectedIndex = (int)power.EffectArea;
            cbNotify.SelectedIndex = (int)power.AIReport;
            chkLos.Checked = power.TargetLoS;
            chkIgnoreStrength.Checked = power.IgnoreStrength;
            txtNumCharges.Text = Convert.ToString(power.NumCharges, CultureInfo.InvariantCulture);
            txtUseageTime.Text = Convert.ToString(power.UsageTime, CultureInfo.InvariantCulture);
            txtLifeTimeGame.Text = Convert.ToString(power.LifeTimeInGame, CultureInfo.InvariantCulture);
            txtLifeTimeReal.Text = Convert.ToString(power.LifeTime, CultureInfo.InvariantCulture);
            rbFlagAutoHit.Checked = true;
            FillAdvAtrList();
        }

        void FillTab_Basic()
        {
            IPower power = myPower;
            txtNameDisplay.Text = power.DisplayName;
            txtNamePower.Text = power.PowerName;
            cbNameGroup.Text = power.GroupName;
            cbNameSet.Text = power.SetName;
            DisplayNameData();
            txtDescLong.Text = power.DescLong;
            txtDescShort.Text = power.DescShort;
            udScaleMin.Value = new decimal(power.VariableMin);
            udScaleMax.Value = new decimal(power.VariableMax);
            txtScaleName.Text = power.VariableName;
            chkScale.Checked = power.VariableEnabled;
            chkBuffCycle.Checked = power.ClickBuff;
            chkAlwaysToggle.Checked = power.AlwaysToggle;
            chkGraphFix.Checked = myPower.SkipMax;
            chkAltSub.Checked = power.SubIsAltColour;
            chkSubInclude.Checked = power.IncludeFlag;
            chkSortOverride.Checked = power.SortOverride;
            txtVisualLocation.Text = Convert.ToString(power.DisplayLocation, CultureInfo.InvariantCulture);
            chkSummonStealEffects.Checked = power.AbsorbSummonEffects;
            chkSummonStealAttributes.Checked = power.AbsorbSummonAttributes;
            chkSummonDisplayEntity.Checked = power.ShowSummonAnyway;
            chkNoAUReq.Checked = myPower.NeverAutoUpdateRequirements;
            cbForcedClass.SelectedIndex = DatabaseAPI.NidFromUidClass(power.ForcedClass) + 1;
            chkNoAutoUpdate.Checked = myPower.NeverAutoUpdate;
            chkHidden.Visible = MidsContext.Config.MasterMode;
            chkHidden.Checked = myPower.HiddenPower;
        }

        void FillTab_Disabling()
        {
            int num1 = myPower.IgnoreEnh.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if (myPower.IgnoreEnh[index] <= (Enums.eEnhance)(lvDisablePass1.Items.Count - 1))
                    lvDisablePass1.SetSelected((int)myPower.IgnoreEnh[index], true);
            }
            int num2 = myPower.Ignore_Buff.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (myPower.Ignore_Buff[index] <= (Enums.eEnhance)(lvDisablePass4.Items.Count - 1))
                    lvDisablePass4.SetSelected((int)myPower.Ignore_Buff[index], true);
            }
        }

        void FillTab_Effects()
        {
            RefreshFXData();
        }

        void FillTab_Enhancements()
        {
            RedrawEnhList();
            chkPRFrontLoad.Checked = myPower.AllowFrontLoading;
            chkBoostBoostable.Checked = myPower.BoostBoostable;
            chkBoostUsePlayerLevel.Checked = myPower.BoostUsePlayerLevel;
        }

        void FillTab_Mutex()
        {
            chkMutexAuto.Checked = myPower.MutexAuto;
            chkMutexSkip.Checked = myPower.MutexIgnore;
            clbMutex.BeginUpdate();
            clbMutex.Items.Clear();
            string[] strArray = DatabaseAPI.UidMutexAll();
            int num1 = strArray.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                bool isChecked = false;
                int num2 = myPower.GroupMembership.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (!string.Equals(strArray[index1], myPower.GroupMembership[index2], StringComparison.OrdinalIgnoreCase))
                        continue;
                    isChecked = true;
                    break;
                }
                clbMutex.Items.Add(strArray[index1], isChecked);
            }
            clbMutex.EndUpdate();
        }

        void FillTab_Req()
        {
            ReqChanging = true;
            lvPrListing.BeginUpdate();
            lvPrListing.Items.Clear();
            int num1 = myPower.Requires.PowerID.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                string[] items = new string[3];
                if (myPower.Requires.PowerID[index].Length <= 0)
                    continue;
                items[0] = myPower.Requires.PowerID[index][0];
                if (!string.IsNullOrWhiteSpace(myPower.Requires.PowerID[index][1]))
                {
                    items[1] = "AND";
                    items[2] = myPower.Requires.PowerID[index][1];
                }
                lvPrListing.Items.Add(new ListViewItem(items)
                {
                    Tag = index
                });
            }
            int num2 = myPower.Requires.PowerIDNot.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                string[] items = new string[3];
                if (myPower.Requires.PowerIDNot[index].Length <= 0)
                    continue;
                items[0] = "NOT " + myPower.Requires.PowerIDNot[index][0];
                if (!string.IsNullOrWhiteSpace(myPower.Requires.PowerIDNot[index][1]))
                {
                    items[1] = "AND";
                    items[2] = "NOT " + myPower.Requires.PowerIDNot[index][1];
                }
                lvPrListing.Items.Add(new ListViewItem(items)
                {
                    Tag = index
                });
            }
            lvPrListing.EndUpdate();
            ReqChanging = false;
            if (lvPrListing.Items.Count <= 0)
                return;
            lvPrListing.Items[0].Selected = true;
        }

        void Filltab_ReqClasses()
        {
            clbClassReq.BeginUpdate();
            clbClassReq.Items.Clear();
            int num1 = DatabaseAPI.Database.Classes.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                bool isChecked = false;
                int num2 = myPower.Requires.ClassName.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (string.Equals(DatabaseAPI.Database.Classes[index1].ClassName, myPower.Requires.ClassName[index2], StringComparison.OrdinalIgnoreCase))
                        isChecked = true;
                }
                clbClassReq.Items.Add(DatabaseAPI.Database.Classes[index1].ClassName, isChecked);
            }
            clbClassReq.EndUpdate();
            clbClassExclude.BeginUpdate();
            clbClassExclude.Items.Clear();
            int num3 = DatabaseAPI.Database.Classes.Length - 1;
            for (int index1 = 0; index1 <= num3; ++index1)
            {
                bool isChecked = false;
                int num2 = myPower.Requires.ClassNameNot.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (string.Equals(DatabaseAPI.Database.Classes[index1].ClassName, myPower.Requires.ClassNameNot[index2], StringComparison.OrdinalIgnoreCase))
                        isChecked = true;
                }
                clbClassExclude.Items.Add(DatabaseAPI.Database.Classes[index1].ClassName, isChecked);
            }
            clbClassExclude.EndUpdate();
        }

        void FillTab_Sets()
        {
            DrawAcceptedSets();
        }

        void FillTab_SubPowers()
        {
            bool reqChanging = ReqChanging;
            ReqChanging = true;
            SP_GroupList();
            if (lvSPGroup.Items.Count > 0)
                lvSPGroup.Items[0].Selected = true;
            SP_SetList();
            if (lvSPSet.Items.Count > 0)
                lvSPSet.Items[0].Selected = true;
            SP_PowerList();
            ReqChanging = reqChanging;
            SPFillList();
        }

        void frmEditPower_Load(object sender, EventArgs e)
        {
            RedrawEnhPicker();
            FillComboBoxes();
            DrawSetList();
            Req_GroupList();
            FillTab_SubPowers();
            refresh_PowerData();
            Updating = false;
        }

        static int GetClassByID(int iID)
        {
            int num = DatabaseAPI.Database.EnhancementClasses.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (DatabaseAPI.Database.EnhancementClasses[index].ID == iID)
                    return index;
            }
            return 0;
        }

        int GetInvSetIndex(Point e)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = 0;
            do
            {
                if (e.X > (enhPadding + 30) * num3 & e.X < (enhPadding + 30) * (num3 + 1))
                    num1 = num3;
                ++num3;
            }
            while (num3 <= 9);
            int num4 = 0;
            do
            {
                if (e.Y > (enhPadding + 30) * num4 & e.Y < (enhPadding + 30) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            return num1 + num2 * 10;
        }

        int GetInvSetListIndex(Point e)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = enhAcross - 1;
            for (int index = 0; index <= num3; ++index)
            {
                if (e.X > (enhPadding + 30) * index & e.X < (enhPadding + 30) * (index + 1))
                    num1 = index;
            }
            int num4 = 0;
            do
            {
                if (e.Y > (enhPadding + 30) * num4 & e.Y < (enhPadding + 30) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            return num1 + num2 * enhAcross;
        }

        void lblStaticIndex_Click(object sender, EventArgs e)
        {
            string s = Interaction.InputBox("Insert new static index for this power.", "", Convert.ToString(myPower.StaticIndex, CultureInfo.InvariantCulture));
            try
            {   
                var num1 = 1;
                if (s != null)
                {
                    num1 = int.Parse(s, CultureInfo.InvariantCulture);
                }

                if (num1 < 0)
                {
                    Interaction.MsgBox("The static index cannot be a negative number.", MsgBoxStyle.Exclamation);
                }
                else
                {
                    lblStaticIndex.Text = Convert.ToString(num1, CultureInfo.InvariantCulture);
                    myPower.StaticIndex = num1;
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Interaction.MsgBox(ex.Message);
                ProjectData.ClearProjectError();
            }
        }

        void lvDisablePass1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            myPower.IgnoreEnh = new Enums.eEnhance[lvDisablePass1.SelectedIndices.Count - 1 + 1];
            int num = lvDisablePass1.SelectedIndices.Count - 1;
            for (int index = 0; index <= num; ++index)
                myPower.IgnoreEnh[index] = (Enums.eEnhance)lvDisablePass1.SelectedIndices[index];
        }

        void lvDisablePass4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            myPower.Ignore_Buff = new Enums.eEnhance[lvDisablePass4.SelectedIndices.Count - 1 + 1];
            int num = lvDisablePass4.SelectedIndices.Count - 1;
            for (int index = 0; index <= num; ++index)
                myPower.Ignore_Buff[index] = (Enums.eEnhance)lvDisablePass4.SelectedIndices[index];
        }

        void lvFX_DoubleClick(object sender, EventArgs e)
        {
            btnFXEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        void lvPrGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvPrGroup.SelectedItems.Count <= 0)
                return;
            Req_SetList();
            if (lvPrSet.Items.Count > 0)
                lvPrSet.Items[0].Selected = true;
        }

        void lvPrListing_SelectedIndexChanged(object sender, EventArgs e)
        {
            Req_Listing_IndexChanged();
        }

        void lvPrPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging)
                return;
            Req_UpdateItem();
        }

        void lvPrSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvPrSet.SelectedItems.Count <= 0)
                return;
            Req_PowerList();
        }

        void lvSPGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvSPGroup.SelectedItems.Count <= 0)
                return;
            SP_SetList();
            if (lvSPSet.Items.Count > 0)
                lvSPSet.Items[0].Selected = true;
        }

        void lvSPSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvSPSet.SelectedItems.Count <= 0)
                return;
            SP_PowerList();
        }

        void pbEnhancementList_Hover(object sender, MouseEventArgs e)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = enhAcross - 1;
            for (int index = 0; index <= num3; ++index)
            {
                if (e.X > (enhPadding + 30) * index & e.X < (enhPadding + 30) * (index + 1))
                    num1 = index;
            }
            int num4 = 0;
            do
            {
                if (e.Y > (enhPadding + 30) * num4 & e.Y < (enhPadding + 30) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            int index1 = num1 + num2 * enhAcross;
            if (index1 < DatabaseAPI.Database.EnhancementClasses.Length & num1 > -1 & num2 > -1)
                lblEnhName.Text = DatabaseAPI.Database.EnhancementClasses[index1].Name;
            else
                lblEnhName.Text = "";
        }

        void pbEnhancementList_MouseDown(object sender, MouseEventArgs e)
        {
            int num1 = -1;
            int num2 = -1;
            int num3 = enhAcross - 1;
            for (int index = 0; index <= num3; ++index)
            {
                if (e.X > (enhPadding + 30) * index & e.X < (enhPadding + 30) * (index + 1))
                    num1 = index;
            }
            int num4 = 0;
            do
            {
                if (e.Y > (enhPadding + 30) * num4 & e.Y < (enhPadding + 30) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            int index1 = num1 + num2 * enhAcross;
            if (!(index1 <= DatabaseAPI.Database.EnhancementClasses.Length - 1 & num1 > -1 & num2 > -1))
                return;
            bool flag = false;
            int num5 = myPower.Enhancements.Length - 1;
            for (int index2 = 0; index2 <= num5; ++index2)
            {
                if (myPower.Enhancements[index2] == DatabaseAPI.Database.EnhancementClasses[index1].ID)
                    flag = true;
            }

            if (flag)
                return;
            IPower power = myPower;
            int[] numArray = (int[])Utils.CopyArray(power.Enhancements, new int[myPower.Enhancements.Length + 1]);
            power.Enhancements = numArray;
            myPower.Enhancements[myPower.Enhancements.Length - 1] = DatabaseAPI.Database.EnhancementClasses[index1].ID;
            Array.Sort(myPower.Enhancements);
            RedrawEnhList();
        }

        void pbEnhancementList_Paint(object sender, PaintEventArgs e)
        {
            if (bxEnhPicker == null)
                return;
            e.Graphics.DrawImageUnscaled(bxEnhPicker.Bitmap, 0, 0);
        }

        void pbEnhancements_Hover(object sender, MouseEventArgs e)
        {
            int num = -1;
            int length = myPower.Enhancements.Length;
            for (int index = 0; index <= length; ++index)
            {
                if (e.X > (enhPadding + 30) * index & e.X < (enhPadding + 30) * (index + 1))
                    num = index;
            }
            int index1 = num;
            if (index1 < myPower.Enhancements.Length & num > -1)
                lblEnhName.Text = DatabaseAPI.Database.EnhancementClasses[GetClassByID(myPower.Enhancements[index1])].Name;
            else
                lblEnhName.Text = "";
        }

        void pbEnhancements_MouseDown(object sender, MouseEventArgs e)
        {
            int num1 = -1;
            int length = myPower.Enhancements.Length;
            for (int index = 0; index <= length; ++index)
            {
                if (e.X > (enhPadding + 30) * index & e.X < (enhPadding + 30) * (index + 1))
                    num1 = index;
            }
            int num2 = num1;
            if (!(num2 < myPower.Enhancements.Length & num1 > -1))
                return;
            IPower power = myPower;
            int[] numArray = new int[power.Enhancements.Length - 1 + 1];
            int num3 = power.Enhancements.Length - 1;
            for (int index = 0; index <= num3; ++index)
                numArray[index] = power.Enhancements[index];
            int index1 = 0;
            power.Enhancements = new int[power.Enhancements.Length - 2 + 1];
            int num4 = numArray.Length - 1;
            for (int index2 = 0; index2 <= num4; ++index2)
            {
                if (index2 == num2)
                    continue;
                power.Enhancements[index1] = numArray[index2];
                ++index1;
            }
            Array.Sort(power.Enhancements);
            RedrawEnhList();
        }

        void pbEnhancements_Paint(object sender, PaintEventArgs e)
        {
            if (bxEnhPicked == null)
                return;
            e.Graphics.DrawImageUnscaled(bxEnhPicked.Bitmap, 0, 0);
        }

        void pbInvSetList_MouseDown(object sender, MouseEventArgs e)
        {
            Enums.eSetType eSetType = Enums.eSetType.Untyped;
            int invSetListIndex = GetInvSetListIndex(new Point(e.X, e.Y));
            string[] names = Enum.GetNames(eSetType.GetType());
            if (!(invSetListIndex < names.Length & invSetListIndex > -1))
                return;
            bool flag = false;
            int num = myPower.SetTypes.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (myPower.SetTypes[index] == (Enums.eSetType)invSetListIndex)
                    flag = true;
            }

            if (flag | myPower.SetTypes.Length > 10)
                return;
            IPower power = myPower;
            Enums.eSetType[] eSetTypeArray = (Enums.eSetType[])Utils.CopyArray(power.SetTypes, new Enums.eSetType[myPower.SetTypes.Length + 1]);
            power.SetTypes = eSetTypeArray;
            myPower.SetTypes[myPower.SetTypes.Length - 1] = (Enums.eSetType)invSetListIndex;
            Array.Sort(myPower.SetTypes);
            DrawAcceptedSets();
        }

        void pbInvSetList_MouseMove(object sender, MouseEventArgs e)
        {
            Enums.eSetType eSetType = Enums.eSetType.Untyped;
            int invSetListIndex = GetInvSetListIndex(new Point(e.X, e.Y));
            string[] names = Enum.GetNames(eSetType.GetType());
            if (invSetListIndex < names.Length & invSetListIndex > -1)
                lblInvSet.Text = names[invSetListIndex];
            else
                lblInvSet.Text = "";
        }

        void pbInvSetList_Paint(object sender, PaintEventArgs e)
        {
            if (bxSetList == null)
                return;
            e.Graphics.DrawImageUnscaled(bxSetList.Bitmap, 0, 0);
        }

        void pbInvSetUsed_MouseDown(object sender, MouseEventArgs e)
        {
            int invSetIndex = GetInvSetIndex(new Point(e.X, e.Y));
            if (!(invSetIndex < myPower.SetTypes.Length & invSetIndex > -1))
                return;
            int[] numArray = new int[myPower.SetTypes.Length - 1 + 1];
            int num1 = myPower.SetTypes.Length - 1;
            for (int index = 0; index <= num1; ++index)
                numArray[index] = (int)myPower.SetTypes[index];
            int index1 = 0;
            myPower.SetTypes = new Enums.eSetType[myPower.SetTypes.Length - 2 + 1];
            int num2 = numArray.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
            {
                if (index2 == invSetIndex)
                    continue;
                myPower.SetTypes[index1] = (Enums.eSetType)numArray[index2];
                ++index1;
            }
            Array.Sort(myPower.SetTypes);
            DrawAcceptedSets();
        }

        void pbInvSetUsed_MouseMove(object sender, MouseEventArgs e)
        {
            Enums.eSetType eSetType = Enums.eSetType.Untyped;
            int invSetIndex = GetInvSetIndex(new Point(e.X, e.Y));
            string[] names = Enum.GetNames(eSetType.GetType());
            if (invSetIndex < myPower.SetTypes.Length & invSetIndex > -1)
                lblInvSet.Text = names[(int)myPower.SetTypes[invSetIndex]];
            else
                lblInvSet.Text = "";
        }

        void pbInvSetUsed_Paint(object sender, PaintEventArgs e)
        {
            if (bxSet == null)
                return;
            e.Graphics.DrawImageUnscaled(bxSet.Bitmap, 0, 0);
        }

        static bool PowerFullNameIsUnique(string iFullName, int skipId = -1)
        {
            if (string.IsNullOrEmpty(iFullName))
                return true;
            int num = DatabaseAPI.Database.Power.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (index != skipId && string.Equals(DatabaseAPI.Database.Power[index].FullName, iFullName, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        void rbFlagX_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            FillAdvAtrList();
        }

        void rbPrAdd_Click(object sender, EventArgs e)
        {
            if (Interaction.MsgBox("If this power is required to be present, click 'Yes'.\r\nIf this power must NOT be present, click 'No'.", MsgBoxStyle.YesNo, "Query") == MsgBoxResult.No)
            {
                myPower.Requires.PowerIDNot = (string[][])Utils.CopyArray(myPower.Requires.PowerIDNot, new string[myPower.Requires.PowerIDNot.Length + 1][]);
                myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1] = new string[2];
                myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1][0] = "Empty";
                myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1][1] = "";
                FillTab_Req();
                lvPrListing.Items[lvPrListing.Items.Count - 1].Selected = true;
                lvPrListing.Items[lvPrListing.Items.Count - 1].EnsureVisible();
            }
            else
            {
                myPower.Requires.PowerID = (string[][])Utils.CopyArray(myPower.Requires.PowerID, new string[myPower.Requires.PowerID.Length + 1][]);
                myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1] = new string[2];
                myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1][0] = "Empty";
                myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1][1] = "";
                FillTab_Req();
                lvPrListing.Items[myPower.Requires.PowerID.Length - 1].Selected = true;
                lvPrListing.Items[myPower.Requires.PowerID.Length - 1].EnsureVisible();
            }
        }

        void rbPrPowerX_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == rbPrPowerB.GetType() && ((Control)sender).Text == "Power B")
                return;
            btnPrSetNone.Text = rbPrPowerA.Checked ? "Set Power A to None" : "Set Power B to None";
            Req_Listing_IndexChanged();
        }

        void rbPrRemove_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            int num1 = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            if (lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1)
            {
                string[][] strArray1 = new string[myPower.Requires.PowerIDNot.Length - 1 + 1][];
                int num2 = num1;
                int num3 = strArray1.Length - 1;
                for (int index = 0; index <= num3; ++index)
                {
                    string[] strArray2 = new string[2];
                    strArray1[index] = strArray2;
                    strArray1[index][0] = myPower.Requires.PowerIDNot[index][0];
                    strArray1[index][1] = myPower.Requires.PowerIDNot[index][1];
                }
                myPower.Requires.PowerIDNot = new string[myPower.Requires.PowerIDNot.Length - 2 + 1][];
                int index1 = 0;
                int num4 = strArray1.Length - 1;
                for (int index2 = 0; index2 <= num4; ++index2)
                {
                    if (index2 == num2)
                        continue;
                    string[] strArray2 = new string[2];
                    myPower.Requires.PowerIDNot[index1] = strArray2;
                    myPower.Requires.PowerIDNot[index1][0] = strArray1[index2][0];
                    myPower.Requires.PowerIDNot[index1][1] = strArray1[index2][1];
                    ++index1;
                }
            }
            else
            {
                string[][] strArray1 = new string[myPower.Requires.PowerID.Length - 1 + 1][];
                int num2 = num1;
                int num3 = strArray1.Length - 1;
                for (int index = 0; index <= num3; ++index)
                {
                    string[] strArray2 = new string[2];
                    strArray1[index] = strArray2;
                    strArray1[index][0] = myPower.Requires.PowerID[index][0];
                    strArray1[index][1] = myPower.Requires.PowerID[index][1];
                }
                myPower.Requires.PowerID = new string[myPower.Requires.PowerID.Length - 2 + 1][];
                int index1 = 0;
                int num4 = strArray1.Length - 1;
                for (int index2 = 0; index2 <= num4; ++index2)
                {
                    if (index2 == num2)
                        continue;
                    myPower.Requires.PowerID[index1] = new string[2];
                    myPower.Requires.PowerID[index1][0] = strArray1[index2][0];
                    myPower.Requires.PowerID[index1][1] = strArray1[index2][1];
                    ++index1;
                }
            }
            FillTab_Req();
        }

        void RedrawEnhList()
        {
            bxEnhPicked = new ExtendedBitmap(pbEnhancements.Width, pbEnhancements.Height);
            int enhPadding1 = enhPadding;
            int enhPadding2 = enhPadding;
            using SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0,0,0));
            bxEnhPicked.Graphics.FillRectangle(solidBrush, bxEnhPicked.ClipRect);
            Graphics graphics = bxEnhPicked.Graphics;
            using Pen pen = new Pen(Color.FromArgb(0, 0, byte.MaxValue), 1f);
            Size size = bxEnhPicked.Size;
            int width = size.Width - 1;
            size = bxEnhPicked.Size;
            int height = size.Height - 1;
            graphics.DrawRectangle(pen, 0, 0, width, height);
            int num = myPower.Enhancements.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxEnhPicked.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(GetClassByID(myPower.Enhancements[index])), GraphicsUnit.Pixel);
                enhPadding2 += 30 + enhPadding;
            }
            pbEnhancements.CreateGraphics().DrawImageUnscaled(bxEnhPicked.Bitmap, 0, 0);
        }

        void RedrawEnhPicker()
        {
            pbEnhancementList.Width = (enhPadding + 30) * enhAcross + enhPadding;
            pbEnhancementList.Height = (enhPadding + 30) * 6 + enhPadding;
            bxEnhPicker = new ExtendedBitmap(pbEnhancementList.Width, pbEnhancementList.Height);
            int enhPadding1 = enhPadding;
            int enhPadding2 = enhPadding;
            int num1 = 0;
            using SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0,0,0));
            bxEnhPicker.Graphics.FillRectangle(solidBrush, bxEnhPicker.ClipRect);
            Graphics graphics = bxEnhPicker.Graphics;
            using Pen pen = new Pen(Color.FromArgb(0, 0, byte.MaxValue), 1f);
            Size size = bxEnhPicker.Size;
            int width = size.Width - 1;
            size = bxEnhPicker.Size;
            int height = size.Height - 1;
            graphics.DrawRectangle(pen, 0, 0, width, height);
            int num2 = DatabaseAPI.Database.EnhancementClasses.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxEnhPicker.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(index), GraphicsUnit.Pixel);
                enhPadding2 += 30 + enhPadding;
                ++num1;
                if (num1 != enhAcross)
                    continue;
                num1 = 0;
                enhPadding2 = enhPadding;
                enhPadding1 += 30 + enhPadding;
            }
            pbEnhancementList.CreateGraphics().DrawImageUnscaled(bxEnhPicker.Bitmap, 0, 0);
        }

        void refresh_PowerData()
        {
            Text = "Edit Power (" + myPower.FullName + ")";
            lblStaticIndex.Text = Convert.ToString(myPower.StaticIndex, CultureInfo.InvariantCulture);
            FillCombo_Basic();
            FillTab_Basic();
            FillCombo_Attribs();
            FillTab_Attribs();
            FillTab_Effects();
            FillTab_Enhancements();
            FillTab_Sets();
            FillTab_Req();
            Filltab_ReqClasses();
            FillTab_Disabling();
            FillTab_Mutex();
            SetDynamics();
        }

        void RefreshFXData(int Index = 0)
        {
            IPower power = myPower;
            lvFX.BeginUpdate();
            lvFX.Items.Clear();
            int num = power.Effects.Length - 1;
            for (int index = 0; index <= num; ++index)
                lvFX.Items.Add(power.Effects[index].BuildEffectString(false, "", false, false, true).Replace("\r\n", " - "));
            lvFX.EndUpdate();
            if (lvFX.Items.Count > Index)
                lvFX.SelectedIndex = Index;
            else
                lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        void Req_GroupList()
        {
            lvPrGroup.BeginUpdate();
            lvPrGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvPrGroup.Items.Add(key);
            lvPrGroup.EndUpdate();
        }

        void Req_Listing_IndexChanged()
        {
            if (lvPrListing.SelectedIndices.Count < 1)
                return;
            int index = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            ReqDisplayPower(lvPrListing.SelectedIndices[0] <= myPower.Requires.PowerID.Length - 1 ? (!rbPrPowerA.Checked ? myPower.Requires.PowerID[index][1] : myPower.Requires.PowerID[index][0]) : (!rbPrPowerA.Checked ? myPower.Requires.PowerIDNot[index][1] : myPower.Requires.PowerIDNot[index][0]));
        }

        void Req_PowerList()
        {
            lvPrPower.BeginUpdate();
            lvPrPower.Items.Clear();
            if (lvPrSet.SelectedIndices.Count < 1)
            {
                lvPrPower.EndUpdate();
            }
            else
            {
                int index1 = DatabaseAPI.NidFromUidPowerset(Convert.ToString(lvPrSet.SelectedItems[0].Tag, CultureInfo.InvariantCulture));
                if (index1 > -1)
                {
                    int num = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
                    {
                        if (!DatabaseAPI.Database.Powersets[index1].Powers[index2].HiddenPower)
                            lvPrPower.Items.Add(DatabaseAPI.Database.Powersets[index1].Powers[index2].PowerName);
                    }
                }
                lvPrPower.EndUpdate();
            }
        }

        void Req_SetList()
        {
            lvPrSet.BeginUpdate();
            lvPrSet.Items.Clear();
            if (lvPrGroup.SelectedIndices.Count < 1)
            {
                lvPrSet.EndUpdate();
            }
            else
            {
                int[] indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvPrGroup.SelectedItems[0].Text);
                int num = indexesByGroupName.Length - 1;
                for (int index = 0; index <= num; ++index)
                {
                    lvPrSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvPrSet.Items[lvPrSet.Items.Count - 1].Tag = DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }
                lvPrSet.EndUpdate();
            }
        }

        void Req_UpdateItem()
        {
            if (lvPrListing.SelectedIndices.Count < 1 | lvPrGroup.SelectedIndices.Count < 1 | lvPrSet.SelectedIndices.Count < 1 | lvPrPower.SelectedIndices.Count < 1)
                return;
            string str = lvPrGroup.SelectedItems[0].Text + "." + lvPrSet.SelectedItems[0].Text + "." + lvPrPower.SelectedItems[0].Text;
            int index = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            if (lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1)
            {
                if (rbPrPowerA.Checked)
                {
                    myPower.Requires.PowerIDNot[index][0] = str;
                    lvPrListing.SelectedItems[0].SubItems[0].Text = "NOT " + str;
                }
                else
                {
                    myPower.Requires.PowerIDNot[index][1] = str;
                    lvPrListing.SelectedItems[0].SubItems[2].Text = "NOT " + str;
                }
            }
            else if (rbPrPowerA.Checked)
            {
                myPower.Requires.PowerID[index][0] = str;
                lvPrListing.SelectedItems[0].SubItems[0].Text = str;
            }
            else
            {
                myPower.Requires.PowerID[index][1] = str;
                lvPrListing.SelectedItems[0].SubItems[2].Text = str;
            }
        }

        void ReqDisplayPower(string iPower)
        {
            ReqChanging = true;
            string[] strArray = iPower.Split(".".ToCharArray());
            if (strArray.Length > 0)
            {
                int num = lvPrGroup.Items.Count - 1;
                for (int index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvPrGroup.Items[index].Text, strArray[0], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvPrGroup.Items[index].Selected = true;
                    lvPrGroup.Items[index].EnsureVisible();
                    break;
                }
            }
            Req_SetList();
            if (strArray.Length > 1)
            {
                int num = lvPrSet.Items.Count - 1;
                for (int index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvPrSet.Items[index].Text, strArray[1], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvPrSet.Items[index].Selected = true;
                    lvPrSet.Items[index].EnsureVisible();
                    break;
                }
            }
            Req_PowerList();
            if (strArray.Length > 2)
            {
                int num = lvPrPower.Items.Count - 1;
                for (int index = 0; index <= num; ++index)
                {
                    if (!string.Equals(lvPrPower.Items[index].Text, strArray[2], StringComparison.OrdinalIgnoreCase))
                        continue;
                    lvPrPower.Items[index].Selected = true;
                    lvPrPower.Items[index].EnsureVisible();
                    break;
                }
            }
            ReqChanging = false;
        }

        void SetDynamics()
        {
            IPower power = myPower;
            chkBuffCycle.Enabled = power.PowerType == Enums.ePowerType.Click;
            chkAlwaysToggle.Enabled = power.PowerType == Enums.ePowerType.Toggle;
            if (power.ActivatePeriod > 0.0 & power.PowerType == Enums.ePowerType.Toggle)
                lblEndCost.Text = "(" + Strings.Format((float)(power.EndCost / (double)power.ActivatePeriod), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "/s)";
            else
                lblEndCost.Text = "";
            lblAcc.Text = "(" + Strings.Format((float)(power.Accuracy * (double)MidsContext.Config.BaseAcc * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "#") + "%)";
        }

        void SetFullName()
        {
            IPower power = myPower;
            power.FullName = power.GroupName + "." + power.SetName + "." + power.PowerName;
        }

        void SP_GroupList()
        {
            lvSPGroup.BeginUpdate();
            lvSPGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvSPGroup.Items.Add(key);
            lvSPGroup.EndUpdate();
        }

        void SP_PowerList()
        {
            lvSPPower.BeginUpdate();
            lvSPPower.Items.Clear();
            if (lvSPSet.SelectedIndices.Count < 1)
            {
                lvSPPower.EndUpdate();
            }
            else
            {
                int index1 = DatabaseAPI.NidFromUidPowerset(Convert.ToString(lvSPSet.SelectedItems[0].Tag, CultureInfo.InvariantCulture));
                if (index1 > -1)
                {
                    int num = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
                    {
                        if (DatabaseAPI.Database.Powersets[index1].Powers[index2].HiddenPower)
                            continue;
                        lvSPPower.Items.Add(DatabaseAPI.Database.Powersets[index1].Powers[index2].PowerName);
                        lvSPPower.Items[lvSPPower.Items.Count - 1].Tag = DatabaseAPI.Database.Powersets[index1].Powers[index2].FullName;
                    }
                }
                lvSPPower.EndUpdate();
            }
        }

        void SP_SetList()
        {
            lvSPSet.BeginUpdate();
            lvSPSet.Items.Clear();
            if (lvSPGroup.SelectedIndices.Count < 1)
            {
                lvSPSet.EndUpdate();
            }
            else
            {
                int[] indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvSPGroup.SelectedItems[0].Text);
                int num = indexesByGroupName.Length - 1;
                for (int index = 0; index <= num; ++index)
                {
                    lvSPSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvSPSet.Items[lvSPSet.Items.Count - 1].Tag = DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }
                lvSPSet.EndUpdate();
            }
        }

        void SPFillList()
        {
            lvSPSelected.BeginUpdate();
            lvSPSelected.Items.Clear();
            int num = myPower.UIDSubPower.Length - 1;
            for (int index = 0; index <= num; ++index)
                lvSPSelected.Items.Add(myPower.UIDSubPower[index]);
            lvSPSelected.EndUpdate();
        }

        void Store_Req_Classes()
        {
            myPower.Requires.ClassName = new string[clbClassReq.CheckedIndices.Count - 1 + 1];
            int num1 = clbClassReq.CheckedIndices.Count - 1;
            for (int index = 0; index <= num1; ++index)
                myPower.Requires.ClassName[index] = DatabaseAPI.Database.Classes[clbClassReq.CheckedIndices[index]].ClassName;
            myPower.Requires.ClassNameNot = new string[clbClassExclude.CheckedIndices.Count - 1 + 1];
            int num2 = clbClassExclude.CheckedIndices.Count - 1;
            for (int index = 0; index <= num2; ++index)
                myPower.Requires.ClassNameNot[index] = DatabaseAPI.Database.Classes[clbClassExclude.CheckedIndices[index]].ClassName;
        }

        void txtAcc_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtAcc.Text = Convert.ToString(myPower.Accuracy, CultureInfo.InvariantCulture);
        }

        void txtAcc_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtAcc.Text);
            if (num >= 0.0 & num <= 100.0)
                power.Accuracy = num;
        }

        void txtActivate_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtActivate.Text = Convert.ToString(myPower.ActivatePeriod, CultureInfo.InvariantCulture);
        }

        void txtActivate_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtActivate.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.ActivatePeriod = num;
        }

        void txtArc_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtArc.Text = Convert.ToString(myPower.Arc, CultureInfo.InvariantCulture);
        }

        void txtArc_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtArc.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.Arc = (int)Math.Round(num);
        }

        void txtCastTime_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtCastTime.Text = Convert.ToString(myPower.CastTimeReal, CultureInfo.InvariantCulture);
        }

        void txtCastTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtCastTime.Text);
            if (num >= 0.0 & num <= 100.0)
                power.CastTimeReal = num;
        }

        void txtDescLong_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.DescLong = txtDescLong.Text;
        }

        void txtDescShort_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.DescShort = txtDescShort.Text;
        }

        void txtEndCost_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtEndCost.Text = Convert.ToString(myPower.EndCost, CultureInfo.InvariantCulture);
        }

        void txtEndCost_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtEndCost.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.EndCost = num;
        }

        void txtInterrupt_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtInterrupt.Text = Convert.ToString(myPower.InterruptTime, CultureInfo.InvariantCulture);
        }

        void txtInterrupt_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtInterrupt.Text);
            if (num >= 0.0 & num <= 100.0)
                power.InterruptTime = num;
        }

        void txtLevel_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtLevel.Text = Convert.ToString(myPower.Level, CultureInfo.InvariantCulture);
        }

        void txtLevel_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            int num = (int)Math.Round(Conversion.Val(txtLevel.Text));
            if (num >= 0 & num < 51)
                power.Level = num;
        }

        void txtLifeTimeGame_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtLifeTimeGame.Text = Convert.ToString(myPower.LifeTimeInGame, CultureInfo.InvariantCulture);
        }

        void txtLifeTimeGame_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtLifeTimeGame.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.LifeTimeInGame = (int)Math.Round(num);
        }

        void txtLifeTimeReal_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtLifeTimeReal.Text = Convert.ToString(myPower.LifeTime, CultureInfo.InvariantCulture);
        }

        void txtLifeTimeReal_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtLifeTimeReal.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.LifeTime = (int)Math.Round(num);
        }

        void txtMaxTargets_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtMaxTargets.Text = Convert.ToString(myPower.MaxTargets, CultureInfo.InvariantCulture);
        }

        void txtMaxTargets_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtMaxTargets.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.MaxTargets = (int)Math.Round(num);
        }

        void txtNamePower_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            DisplayNameData();
        }

        void txtNamePower_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.PowerName = txtNamePower.Text;
            SetFullName();
        }

        void txtNumCharges_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtNumCharges.Text = Convert.ToString(myPower.NumCharges, CultureInfo.InvariantCulture);
        }

        void txtNumCharges_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtNumCharges.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.NumCharges = (int)Math.Round(num);
        }

        void txtPowerName_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.DisplayName = txtNameDisplay.Text;
        }

        void txtRadius_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtRadius.Text = Convert.ToString(myPower.Radius, CultureInfo.InvariantCulture);
        }

        void txtRadius_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtRadius.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.Radius = (int)Math.Round(num);
        }

        void txtRange_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtRange.Text = Convert.ToString(myPower.Range, CultureInfo.InvariantCulture);
        }

        void txtRange_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtRange.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.Range = num;
        }

        void txtRangeSec_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtRangeSec.Text = Convert.ToString(myPower.RangeSecondary, CultureInfo.InvariantCulture);
        }

        void txtRangeSec_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtRangeSec.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.RangeSecondary = num;
        }

        void txtRechargeTime_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtRechargeTime.Text = Convert.ToString(myPower.RechargeTime, CultureInfo.InvariantCulture);
        }

        void txtRechargeTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtRechargeTime.Text);
            if (num >= 0.0 & num <= 2147483904.0)
            {
                power.RechargeTime = num;
                power.BaseRechargeTime = num;
            }
        }

        void txtScaleName_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.VariableName = txtScaleName.Text;
        }

        void txtUseageTime_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtUseageTime.Text = Convert.ToString(myPower.UsageTime, CultureInfo.InvariantCulture);
        }

        void txtUseageTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtUseageTime.Text);
            if (num >= 0.0 & num < 2147483904.0)
                power.UsageTime = (int)Math.Round(num);
        }

        void txtVisualLocation_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            txtVisualLocation.Text = Convert.ToString(myPower.DisplayLocation, CultureInfo.InvariantCulture);
        }

        void txtVisualLocation_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            IPower power = myPower;
            float num = (float)Conversion.Val(txtVisualLocation.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                power.DisplayLocation = (int)Math.Round(num);
        }

        void udScaleMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckScaleValues();
        }

        void udScaleMax_Leave(object sender, EventArgs e)
        {
            myPower.VariableMax = (int)Math.Round(Conversion.Val(udScaleMax.Text));
            CheckScaleValues();
        }

        void udScaleMax_ValueChanged(object sender, EventArgs e)
        {
            myPower.VariableMax = Convert.ToInt32(udScaleMax.Value);
            CheckScaleValues();
        }

        void udScaleMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckScaleValues();
        }

        void udScaleMin_Leave(object sender, EventArgs e)
        {
            myPower.VariableMin = (int)Math.Round(Conversion.Val(udScaleMin.Text));
            CheckScaleValues();
        }

        void udScaleMin_ValueChanged(object sender, EventArgs e)
        {
            myPower.VariableMin = Convert.ToInt32(udScaleMin.Value);
            CheckScaleValues();
        }
    }
}