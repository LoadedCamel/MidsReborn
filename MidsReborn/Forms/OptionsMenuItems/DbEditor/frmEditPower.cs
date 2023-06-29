using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Import;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;
using Newtonsoft.Json;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditPower : Form
    {
        private readonly Requirement backup_Requires;
        private readonly int enhAcross;
        private readonly int enhPadding;
        private ExtendedBitmap bxEnhPicked;
        private ExtendedBitmap bxEnhPicker;
        private ExtendedBitmap bxSet;
        private ExtendedBitmap bxSetList;
        private bool cancelClose;
        public IPower? myPower;
        private bool ReqChanging;
        private bool Updating;
        private bool EditMode;
        private int OrigStaticIndex;

        public frmEditPower(IPower? iPower, bool editMode = false)
        {
            Load += frmEditPower_Load;
            enhPadding = 6;
            enhAcross = 8;
            Updating = true;
            ReqChanging = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEditPower));
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmEditPower);
            myPower = new Power(iPower);
            backup_Requires = new Requirement(myPower.Requires);
            EditMode = editMode;
            OrigStaticIndex = EditMode ? myPower.StaticIndex : -1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnCSVImport_Click(object sender, EventArgs e)
        {
            var str = Clipboard.GetDataObject()?.GetData("System.String", true).ToString();
            if (!string.IsNullOrWhiteSpace(str))
                return;
            if (new PowerData(str.Replace("\t", ",")).IsValid)
            {
                MessageBox.Show("Import successful.");
                RefreshPowerData();
            }
            else
            {
                MessageBox.Show("Import failed. No changes made.");
            }
        }

        private void btnFullCopy_Click(object sender, EventArgs e)
        {
            var data = JsonConvert.SerializeObject(myPower, Formatting.None, Serializer.SerializerSettings);
            Clipboard.SetData(@"MidsPowerData", data);
        }

        private void btnFullPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsData(@"MidsPowerData"))
            {
                MessageBox.Show(@"The clipboard does not contain any power data.", @"Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var powerData = JsonConvert.DeserializeObject<Power>((string)Clipboard.GetData(@"MidsPowerData"), Serializer.SerializerSettings) ?? throw new InvalidOperationException();
            if (powerData.StaticIndex != myPower.StaticIndex)
            {
                var result = MessageBox.Show(@"Do you want to overwrite the static index with the imported one?", @"Static Index Is Different!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    powerData.StaticIndex = myPower.StaticIndex;
                    lblStaticIndex.Text = powerData.StaticIndex.ToString();
                    lblStaticIndex.ForeColor = CheckStaticIndex() ? SystemColors.ControlText : Color.DarkRed;
                }
            }
            myPower = powerData;
            SetFullName();
            RefreshPowerData();
        }

        private void btnFXAdd_Click(object sender, EventArgs e)
        {
            IEffect iFX = new Effect
            {
                Scale = 1,
                nMagnitude = 1
            };
            iFX.SetPower(myPower);
            using var frmPowerEffect = new frmPowerEffect(iFX, myPower, myPower.Effects.Length);
            cbCoDFormat.Checked = MidsContext.Config.CoDEffectFormat;
            if (frmPowerEffect.ShowDialog() != DialogResult.OK) return;

            var effectList = myPower.Effects.ToList();
            effectList.Add((IEffect)frmPowerEffect.MyFx.Clone());
            myPower.Effects = effectList.ToArray();
            RefreshFXData();
            lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        private void btnFXEdit_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0) return;

            var selectedIndex = lvFX.SelectedIndices[0];
            using var frmPowerEffect = new frmPowerEffect(myPower.Effects[selectedIndex], myPower, selectedIndex);
            cbCoDFormat.Checked = MidsContext.Config.CoDEffectFormat;
            if (frmPowerEffect.ShowDialog(this) != DialogResult.OK) return;

            myPower.Effects[selectedIndex] = (IEffect)frmPowerEffect.MyFx.Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex;
        }

        private void btnFXDown_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvFX.SelectedIndices[0];
            if (selectedIndex > myPower.Effects.Length - 2)
                return;
            IEffect[] effectArray =
            {
                (IEffect) myPower.Effects[selectedIndex].Clone(),
                (IEffect) myPower.Effects[selectedIndex + 1].Clone()
            };
            myPower.Effects[selectedIndex] = (IEffect)effectArray[1].Clone();
            myPower.Effects[selectedIndex + 1] = (IEffect)effectArray[0].Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex + 1;
        }

        private void btnFXDuplicate_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;

            var selectedEffect = (IEffect)myPower.Effects[lvFX.SelectedIndices[0]].Clone();
            using var frmPowerEffect = new frmPowerEffect(selectedEffect, myPower, myPower.Effects.Length);
            cbCoDFormat.Checked = MidsContext.Config.CoDEffectFormat;
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;

            var effectList = myPower.Effects.ToList();
            effectList.Add(frmPowerEffect.MyFx);
            myPower.Effects = effectList.ToArray();
            RefreshFXData();
            lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        private void btnFXRemove_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndex < 0)
                return;
            var effectArray = new IEffect[myPower.Effects.Length];
            var selectedIndex = lvFX.SelectedIndex;
            var num1 = effectArray.Length - 1;
            for (var index = 0; index <= num1; ++index)
                effectArray[index] = (IEffect)myPower.Effects[index].Clone();
            myPower.Effects = new IEffect[myPower.Effects.Length - 1];
            var index1 = 0;
            var num2 = effectArray.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
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

        private void btnFXUp_Click(object sender, EventArgs e)
        {
            if (lvFX.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvFX.SelectedIndices[0];
            var effectArray = new IEffect[2];
            if (selectedIndex < 1)
                return;
            effectArray[0] = (IEffect)myPower.Effects[selectedIndex].Clone();
            effectArray[1] = (IEffect)myPower.Effects[selectedIndex - 1].Clone();
            myPower.Effects[selectedIndex] = (IEffect)effectArray[1].Clone();
            myPower.Effects[selectedIndex - 1] = (IEffect)effectArray[0].Clone();
            RefreshFXData();
            lvFX.SelectedIndex = selectedIndex - 1;
        }

        private static void inputBox_StaticIndexValidating(object sender, InputBoxValidatingArgs e)
        {
            var isNumeric = int.TryParse(e.Text.Trim(), out var staticIndexResult);
            if (isNumeric && staticIndexResult >= 0)
            {
                e.Message = staticIndexResult.ToString();
                e.Cancel = false;
            }
            else
            {
                e.Message = "Value must be a number and must also be greater or equal to 0";
                e.Cancel = true;
            }
        }

        private static void inputBox_MutexValidating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                e.Message = "Required";
            }
        }

        private void btnMutexAdd_Click(object sender, EventArgs e)
        {
            var result = InputBox.Show("Enter a name for the new group.", "Add Mutex Group", false, "New Group", InputBox.InputBoxIcon.Info, inputBox_MutexValidating);
            if (!result.OK) return;

            var b = result.Text.Replace(" ", "_");
            var count = clbMutex.Items.Count;
            var index = 0;
            if (index > count) return;

            if (string.Equals(clbMutex.Items[index].ToString(), b, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"'{b}' is not unique!", "Unable to add", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                clbMutex.Items.Add(b, true);
                clbMutex.SelectedIndex = clbMutex.Items.Count - 1;
            }
        }

        private void frmEditPower_CancelClose(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            FormClosing += frmEditPower_CancelClose;
            lblNameFull.Text = $@"{myPower.GroupName}.{myPower.SetName}.{myPower.PowerName}";
            if (string.IsNullOrWhiteSpace(myPower.GroupName) | string.IsNullOrWhiteSpace(myPower.SetName) |
                string.IsNullOrWhiteSpace(myPower.PowerName))
            {
                MessageBox.Show(@$"Power name ({myPower.FullName}) is invalid.", @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (!PowerFullNameIsUnique(myPower.FullName, EditMode ? DatabaseAPI.NidFromUidPower(myPower.FullName) : -1))
            {
                MessageBox.Show(@$"Power name ({myPower.FullName}) already exists. Please enter a unique name.",
                    @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (myPower.StaticIndex < 0)
            {
                MessageBox.Show(@$"Invalid static index. Please enter a positive integer.", @"No Can Do",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            var siCheck = DatabaseAPI.Database.Power.Where(p => p.StaticIndex == myPower.StaticIndex & p.FullName != myPower.FullName).ToList();
            var firstAvailableIndex = 0;
            switch (siCheck.Count)
            {
                case 1:
                    firstAvailableIndex = DatabaseAPI.Database.Power.Max(p => p.StaticIndex) + 1;
                    MessageBox.Show(
                        $"Another power with the same static index ({myPower.StaticIndex}) already exists. Please enter a unique static index.\r\nStatic index {myPower.StaticIndex} is used by:\r\n{siCheck[0].FullName}\r\n\r\nFirst available index: {firstAvailableIndex}",
                        @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                case > 1:
                    {
                        var pList = string.Join(",\r\n", siCheck.Select(p => p.FullName));
                        firstAvailableIndex = DatabaseAPI.Database.Power.Max(p => p.StaticIndex) + 1;
                        MessageBox.Show(
                            $"Other powers with the same static index ({myPower.StaticIndex}) already exists. Please fix them first.\r\nStatic index {myPower.StaticIndex} is used by:\r\n{pList}\r\n\r\nFirst available index: {firstAvailableIndex}",
                            @"No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
            }

            if (chkSubInclude.Checked)
            {
                var ret = int.TryParse(txtVisualLocation.Text, out var inherentGridIndex);
                if (ret)
                {
                    var ps = myPower.GetPowerSet()?.FullName ?? "---";
                    var inherentSlotPowers1 = DatabaseAPI.Database.Power
                        .Where(e => e is { IncludeFlag: true, InherentType: not Enums.eGridType.None } && e.DisplayLocation == myPower.DisplayLocation && e.StaticIndex != myPower.StaticIndex && e.GetPowerSet()?.FullName == ps)
                        .OrderBy(e => e?.DisplayName)
                        .Select(e => $"- {e?.DisplayName}")
                        .ToList();


                    var inherentSlotPowers2 = DatabaseAPI.Database.Power
                        .Where(e => e is { IncludeFlag: true, InherentType: not Enums.eGridType.None } && e.DisplayLocation == myPower.DisplayLocation && e.StaticIndex != myPower.StaticIndex && e.GetPowerSet()?.FullName != ps)
                        .OrderBy(e => e?.DisplayName)
                        .Select(e => $"- {e?.DisplayName} ({e?.GetPowerSet()?.FullName ?? "No powerset"})")
                        .ToList();

                    if (inherentSlotPowers1.Count > 0 | inherentSlotPowers2.Count > 0)
                    {
                        var msg = MessageBox.Show(
                            $"Warning: you have selected inherent slot {inherentGridIndex} for this power.\r\nIt may conflict with these powers{(inherentSlotPowers1.Count + inherentSlotPowers2.Count > 8 ? $" (and {inherentSlotPowers1.Count + inherentSlotPowers2.Count - 8} others)" : "")}:\r\n\r\nIn this powerset:\r\n{(inherentSlotPowers1.Count <= 0 ? "(None)" : string.Join("\r\n", inherentSlotPowers1.Take(Math.Min(8, inherentSlotPowers1.Count))))}\r\n\r\nOther powersets:\r\n{(inherentSlotPowers2.Count <= 0 ? "(None)" : string.Join("\r\n", inherentSlotPowers2.Take(Math.Min(8, inherentSlotPowers2.Count))))}\r\n\r\nConfirm selection ?",
                            "Possible inherent grid index conflict", MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning);

                        if (msg != DialogResult.OK)
                        {
                            return;
                        }
                    }

                    myPower.DisplayLocation = inherentGridIndex;
                }
            }


            FormClosing -= frmEditPower_CancelClose;

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

                if (!myPower.VariableOverride)
                {
                    /*if ((myPower.MaxTargets > 1) & (myPower.MaxTargets != myPower.VariableMax))
                    {
                        myPower.VariableMax = myPower.MaxTargets;
                    }*/
                }
                else
                {
                    myPower.VariableMax = (int)Math.Round(udScaleMax.Value);
                }
            }

            myPower.GroupMembership = new string[clbMutex.CheckedItems.Count];
            myPower.NGroupMembership = new int[clbMutex.CheckedItems.Count];
            for (var index = 0; index < clbMutex.CheckedItems.Count; index++)
            {
                myPower.GroupMembership[index] = $"{clbMutex.CheckedItems[index]}";
                myPower.NGroupMembership[index] = clbMutex.CheckedIndices[index];
            }

            myPower.BoostsAllowed = myPower.Enhancements
                .Select(k => DatabaseAPI.Database.EnhancementClasses.First(c => c.ID == k).ClassID)
                .ToArray();

            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnPrDown_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            var num = Convert.ToInt32(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag));
            //var num = Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            var flag = lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1;
            var index1 = num;
            var index2 = index1 + 1;
            var strArray1 = new string[2][];
            var strArray2 = new string[2];
            strArray1[0] = strArray2;
            var strArray3 = new string[2];
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

        private void btnPrReset_Click(object sender, EventArgs e)
        {
            myPower.Requires = new Requirement(backup_Requires);
            FillTab_Req();
        }

        private void btnPrSetNone_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            if (rbPrPowerA.Checked)
            {
                if (!string.IsNullOrWhiteSpace(myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1]))
                {
                    myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][0] =
                        myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1];
                    myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1] = "";
                }
                else
                {
                    rbPrRemove_Click(this, new EventArgs());
                }
            }
            else
            {
                myPower.Requires.PowerID[lvPrListing.SelectedIndices[0]][1] = "";
            }

            FillTab_Req();
        }

        private void btnPrUp_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            var num = Convert.ToInt32(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag));
            //var num = (int) Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            var flag = lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1;
            if (num < 1)
                return;
            var index1 = num;
            var index2 = index1 - 1;
            var strArray1 = new string[2][];
            var strArray2 = new string[2];
            strArray1[0] = strArray2;
            var strArray3 = new string[2];
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

        private void btnSPAdd_Click(object sender, EventArgs e)
        {
            if (lvSPPower.SelectedItems.Count < 1)
                return;
            var b = Convert.ToString(lvSPPower.SelectedItems[0].Tag, CultureInfo.InvariantCulture);
            var num = myPower.UIDSubPower.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (string.Equals(myPower.UIDSubPower[index], b, StringComparison.OrdinalIgnoreCase))
                    return;
            var power = myPower;
            var strArray = Array.Empty<string>();
            Array.Copy(power.UIDSubPower, strArray, power.UIDSubPower.Length + 1);
            //var strArray = (string[]) Utils.CopyArray(power.UIDSubPower, new string[myPower.UIDSubPower.Length + 1]);
            power.UIDSubPower = strArray;
            myPower.UIDSubPower[^1] = b;
            SPFillList();
            lvSPSelected.Items[^1].Selected = true;
            lvSPSelected.Items[^1].EnsureVisible();
        }

        private void btnSPRemove_Click(object sender, EventArgs e)
        {
            if (lvSPSelected.SelectedItems.Count < 1)
                return;
            var text = lvSPSelected.SelectedItems[0].Text;
            var strArray = new string[myPower.UIDSubPower.Length - 1];
            var index1 = 0;
            var subPowerCount = myPower.UIDSubPower.Length - 1;
            for (var index2 = 0; index2 <= subPowerCount; ++index2)
            {
                if (string.Equals(myPower.UIDSubPower[index2], text, StringComparison.OrdinalIgnoreCase))
                    continue;
                strArray[index1] = myPower.UIDSubPower[index2];
                ++index1;
            }

            myPower.UIDSubPower = new string[strArray.Length];
            Array.Copy(strArray, myPower.UIDSubPower, strArray.Length);
            SPFillList();
            var num2 = index1 - 1;
            if (num2 > lvSPSelected.Items.Count - 1)
            {
                var num3 = lvSPSelected.Items.Count - 1;
            }
            else if (num2 >= 0)
            {
            }

            SPFillList();
            if (lvSPSelected.Items.Count <= 0)
                return;
            lvSPSelected.Items[^1].Selected = true;
            lvSPSelected.Items[^1].EnsureVisible();
        }

        private void cbEffectArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.EffectArea = (Enums.eEffectArea)cbEffectArea.SelectedIndex;
        }

        private void cbForcedClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            var index = cbForcedClass.SelectedIndex - 1;
            myPower.ForcedClass = !((index < 0) | (index > DatabaseAPI.Database.Classes.Length - 1))
                ? DatabaseAPI.Database.Classes[index].ClassName
                : "";
        }

        private void cbInherentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.InherentType = (Enums.eGridType)cbInherentType.SelectedIndex;
        }

        private void cbNameGroup_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.GroupName = cbNameGroup.Text;
            DisplayNameData();
            FillComboNameSet();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void cbNameGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.GroupName = cbNameGroup.Text;
            SetFullName();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void cbNameGroup_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.GroupName = cbNameGroup.Text;
            SetFullName();
        }

        private void cbNameSet_Leave(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SetName = cbNameSet.Text;
            DisplayNameData();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void cbNameSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SetName = cbNameSet.Text;
            SetFullName();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void cbNameSet_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SetName = cbNameSet.Text;
            SetFullName();
        }

        private void cbNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AIReport = (Enums.eNotify)cbNotify.SelectedIndex;
        }

        private void cbPowerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            myPower.PowerType = (Enums.ePowerType)cbPowerType.SelectedIndex;
            if ((myPower.ActivatePeriod > 0) & (myPower.PowerType == Enums.ePowerType.Toggle))
            {
                lblEndCost.Text = $"({myPower.EndCost / myPower.ActivatePeriod:##0.##}/s)";
            }
            else
            {
                lblEndCost.Text = "";
            }
        }

        private bool CheckStaticIndex()
        {
            var t = int.TryParse(lblStaticIndex.Text, out var pIndex);

            if (!t) return false;

            var hasDuplicate = DatabaseAPI.Database.Power.Any(p => p.StaticIndex == pIndex && p.FullName != myPower.FullName);

            return !hasDuplicate;
        }

        private void CheckScaleValues()
        {
            var ret = float.TryParse(udScaleStart.Text, out var scaleStart);
            if (!ret) return;

            ret = float.TryParse(udScaleMin.Text, out var scaleMin);
            if (!ret) return;

            ret = float.TryParse(udScaleMax.Text, out var scaleMax);
            if (!ret) return;

            // Sync variables after a change
            scaleStart = myPower.VariableStart;
            scaleMin = myPower.VariableMin;
            scaleMax = myPower.VariableMax;

            if (scaleMin >= scaleMax & chkScale.Checked)
            {
                udScaleMin.BackColor = Color.Coral;
                udScaleMin.ForeColor = Color.Black;
                udScaleMax.BackColor = Color.Coral;
                udScaleMax.ForeColor = Color.Black;
            }
            else
            {
                udScaleMin.BackColor = SystemColors.Window;
                udScaleMin.ForeColor = SystemColors.WindowText;
                udScaleMax.BackColor = SystemColors.Window;
                udScaleMax.ForeColor = SystemColors.WindowText;
            }

            if (chkScale.Checked && scaleStart < scaleMin | scaleStart > scaleMax)
            {
                udScaleStart.BackColor = Color.Coral;
                udScaleStart.ForeColor = Color.Black;
            }
            else
            {
                udScaleStart.BackColor = SystemColors.Window;
                udScaleStart.ForeColor = SystemColors.WindowText;
            }
        }

        private void chkAltSub_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            //myPower.SubIsAltColor = chkAltSub.Checked;
        }

        private void chkAlwaysToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AlwaysToggle = chkAlwaysToggle.Checked;
        }

        private void chkBoostBoostable_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.BoostBoostable = chkPRFrontLoad.Checked;
        }

        private void chkBoostUsePlayerLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.BoostUsePlayerLevel = chkPRFrontLoad.Checked;
        }

        private void chkBuffCycle_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.ClickBuff = chkBuffCycle.Checked;
        }

        private void chkGraphFix_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SkipMax = chkGraphFix.Checked;
        }

        private void chkHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.HiddenPower = chkHidden.Checked;
        }

        private void chkIgnoreStrength_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.IgnoreStrength = chkIgnoreStrength.Checked;
        }

        private void chkLos_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.TargetLoS = chkLos.Checked;
        }

        private void chkMutexAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.MutexAuto = chkMutexAuto.Checked;
        }

        private void chkMutexSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.MutexIgnore = chkMutexSkip.Checked;
        }

        private void chkNoAUReq_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.NeverAutoUpdateRequirements = chkNoAUReq.Checked;
        }

        private void chkNoAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.NeverAutoUpdate = chkNoAutoUpdate.Checked;
        }

        private void chkPRFrontLoad_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AllowFrontLoading = chkPRFrontLoad.Checked;
        }

        private void chkScale_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (!myPower.VariableEnabled)
            {
                udScaleMin.Value = new decimal(0);
                udScaleMax.Value = myPower.MaxTargets <= 1 ? new decimal(3) : new decimal(myPower.MaxTargets);
            }

            myPower.VariableEnabled = chkScale.Checked;
            udScaleStart.Enabled = myPower.VariableEnabled;
            udScaleMax.Enabled = myPower.VariableEnabled;
            udScaleMin.Enabled = myPower.VariableEnabled;
            overideScale.Enabled = myPower.VariableEnabled;
            txtScaleName.Enabled = myPower.VariableEnabled;
        }

        private void overideScale_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.VariableOverride = overideScale.Checked;
        }

        private void chkSortOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.SortOverride = chkSortOverride.Checked;
        }

        private void chkSubInclude_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            cbInherentType.Enabled = chkSubInclude.CheckState == CheckState.Checked;
            myPower.IncludeFlag = chkSubInclude.Checked;
        }

        private void chkSummonDisplayEntity_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.ShowSummonAnyway = chkSummonDisplayEntity.Checked;
        }

        private void chkSummonStealAttributes_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AbsorbSummonAttributes = chkSummonStealAttributes.Checked;
        }

        private void chkSummonStealEffects_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.AbsorbSummonEffects = chkSummonStealEffects.Checked;
        }

        private void clbFlags_ItemCheck(object sender, ItemCheckEventArgs e)
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
                var numArray = new int[28];
                var num1 = 0;
                var num2 = 1;
                var num3 = numArray.Length - 1;
                for (var index = 1; index <= num3; ++index)
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
                if ((e.CurrentValue == CheckState.Unchecked) & (e.NewValue == CheckState.Checked))
                    num1 += numArray[e.Index];
                else if ((e.CurrentValue == CheckState.Checked) & (e.NewValue == CheckState.Unchecked))
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

        private bool CheckPowersetExists(string psFullName)
        {
            return DatabaseAPI.Database.Powersets.Count(e => e.FullName == psFullName) == 1;
        }

        private void DisplayNameData()
        {
            lblNameFull.Text = $"{myPower.GroupName}.{myPower.SetName}.{myPower.PowerName}";
            if (string.IsNullOrWhiteSpace(myPower.GroupName) | string.IsNullOrWhiteSpace(myPower.SetName) |
                string.IsNullOrWhiteSpace(myPower.PowerName))
            {
                lblNameUnique.Text = "This name is invalid.";
            }
            else if (!CheckPowersetExists($"{myPower.GroupName}.{myPower.SetName}"))
            {
                lblNameUnique.Text = "This powerset does not exist.";
            }
            else if (PowerFullNameIsUnique(Convert.ToString(myPower.PowerIndex, CultureInfo.InvariantCulture)))
            {
                lblNameUnique.Text = "This name is unique.";
            }
            else
            {
                lblNameUnique.Text = "This name is NOT unique.";
            }
        }

        private void DrawAcceptedSets()
        {
            bxSet = new ExtendedBitmap(pbInvSetUsed.Width, pbInvSetUsed.Height);
            var enhPadding1 = enhPadding;
            var enhPadding2 = enhPadding;
            using var font = new Font(Font, FontStyle.Bold);
            using var format = new StringFormat(StringFormatFlags.NoWrap);
            using var solidBrush1 = new SolidBrush(Color.Black);
            using var solidBrush2 = new SolidBrush(Color.FromArgb(0, byte.MaxValue, 0));
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            using var solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxSet.Graphics.FillRectangle(solidBrush3, bxSet.ClipRect);
            var num = myPower.SetTypes.Count - 1;
            for (var index = 0; index <= num; ++index)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxSet.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, destRect, I9Gfx.GetImageRect(myPower.SetTypes[index]), GraphicsUnit.Pixel);
                /*var s = myPower.SetTypes[index] switch
                {
                    Enums.eSetType.MeleeST => "M\r\nST",
                    Enums.eSetType.RangedST => "R\r\nST",
                    Enums.eSetType.RangedAoE => "R\r\nAoE",
                    Enums.eSetType.MeleeAoE => "M\r\nAoE",
                    Enums.eSetType.Snipe => "S",
                    _ => ""
                };
                var layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
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
                bxSet.Graphics.DrawString(s, font, solidBrush2, layoutRectangle, format);*/
                enhPadding2 += 30 + enhPadding;
            }

            pbInvSetUsed.CreateGraphics().DrawImageUnscaled(bxSet.Bitmap, 0, 0);
        }

        private void DrawSetList()
        {
            //var eSetType = Enums.eSetType.Untyped;
            bxSetList = new ExtendedBitmap(pbInvSetList.Width, pbInvSetList.Height);
            var enhPadding1 = enhPadding;
            var enhPadding2 = enhPadding;
            var num1 = 0;
            using var font = new Font(Font, FontStyle.Bold);
            using var format = new StringFormat(StringFormatFlags.NoWrap);
            using var solidBrush1 = new SolidBrush(Color.Black);
            using var solidBrush2 = new SolidBrush(Color.FromArgb(0, byte.MaxValue, 0));
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            //var names = Enum.GetNames(eSetType.GetType());
            var setTypes = DatabaseAPI.Database.SetTypes;
            using var solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxSetList.Graphics.FillRectangle(solidBrush3, bxSetList.ClipRect);
            //var num2 = names.Length - 1;
            for (var index = 0; index <= setTypes.Count - 1; ++index)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxSetList.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, destRect, I9Gfx.GetImageRect(index), GraphicsUnit.Pixel);
                /*var s = (Enums.eSetType) index switch
                {
                    Enums.eSetType.MeleeST => "M\r\nST",
                    Enums.eSetType.RangedST => "R\r\nST",
                    Enums.eSetType.RangedAoE => "R\r\nAoE",
                    Enums.eSetType.MeleeAoE => "M\r\nAoE",
                    Enums.eSetType.Snipe => "S",
                    Enums.eSetType.UniversalDamage => "Dmg",
                    _ => ""
                };
                var layoutRectangle = new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height);
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
                bxSetList.Graphics.DrawString(s, font, solidBrush2, layoutRectangle, format);*/
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

        private void FillAdvAtrList()
        {
            var num1 = 0;
            var type = myPower.EntitiesAutoHit.GetType();
            var flag = true;
            var updating = Updating;
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
                var names = Enum.GetNames(type);
                var values = (int[])Enum.GetValues(type);
                var num2 = values.Length - 1;
                for (var index = 0; index <= num2; ++index)
                {
                    var isChecked = (values[index] & num1) != 0;
                    clbFlags.Items.Add(names[index], isChecked);
                }
            }

            clbFlags.EndUpdate();
            Updating = updating;
        }

        private void FillCombo_Attribs()
        {
            var ePowerType = Enums.ePowerType.Click;
            var updating = Updating;
            Updating = true;
            cbEffectArea.BeginUpdate();
            cbNotify.BeginUpdate();
            cbPowerType.BeginUpdate();
            cbEffectArea.Items.Clear();
            cbNotify.Items.Clear();
            cbPowerType.Items.Clear();
            cbEffectArea.Items.AddRange(Enum.GetNames(myPower.EffectArea.GetType()));
            cbNotify.Items.AddRange(Enum.GetNames(myPower.AIReport.GetType()));
            var names = Enum.GetNames(ePowerType.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                names[index] = names[index].Replace("_", "");
            cbPowerType.Items.AddRange(names);
            cbEffectArea.EndUpdate();
            cbNotify.EndUpdate();
            cbPowerType.EndUpdate();
            Updating = updating;
        }

        private void FillCombo_Basic()
        {
            var updating = Updating;
            Updating = true;
            cbNameGroup.BeginUpdate();
            cbNameGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                cbNameGroup.Items.Add(key);
            cbNameGroup.EndUpdate();
            FillComboNameSet();
            cbForcedClass.BeginUpdate();
            cbForcedClass.Items.Clear();
            cbForcedClass.Items.Add("None");
            var num2 = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num2; ++index)
                cbForcedClass.Items.Add(DatabaseAPI.Database.Classes[index].ClassName);
            cbForcedClass.EndUpdate();
            Updating = updating;
        }

        private void FillComboBoxes()
        {
            var eEnhance = Enums.eEnhance.X_RechargeTime;
            lvDisablePass1.BeginUpdate();
            lvDisablePass1.Items.Clear();
            lvDisablePass1.Items.AddRange(Enum.GetNames(eEnhance.GetType()));
            lvDisablePass1.EndUpdate();
            lvDisablePass4.BeginUpdate();
            lvDisablePass4.Items.Clear();
            lvDisablePass4.Items.AddRange(Enum.GetNames(eEnhance.GetType()));
            lvDisablePass4.EndUpdate();
        }

        private void FillComboNameSet()
        {
            cbNameSet.BeginUpdate();
            cbNameSet.Items.Clear();
            var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(myPower.GroupName);
            var num1 = indexesByGroupName.Length - 1;
            for (var index = 0; index <= num1; ++index)
                cbNameSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
            cbNameSet.EndUpdate();
        }

        private void FillComboInherent()
        {
            var eGridType = Enums.eGridType.None;
            cbInherentType.BeginUpdate();
            cbInherentType.Items.Clear();
            cbInherentType.Items.AddRange(Enum.GetNames(eGridType.GetType()));
            cbInherentType.EndUpdate();
        }

        private void FillTab_Attribs()
        {
            var power = myPower;
            // var Style = "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###";
            // txtLevel.Text = Convert.ToString(power.Level, CultureInfo.InvariantCulture);
            // txtAcc.Text = Strings.Format(power.Accuracy, Style);
            // txtInterrupt.Text = Strings.Format(power.InterruptTime, Style);
            // txtCastTime.Text = Strings.Format(power.CastTimeReal, Style);
            // txtRechargeTime.Text = Strings.Format(power.RechargeTime, Style);
            // txtActivate.Text = Strings.Format(power.ActivatePeriod, Style);
            // txtEndCost.Text = Strings.Format(power.EndCost, Style);
            // txtRange.Text = Strings.Format(power.Range, Style);
            // txtRangeSec.Text = Strings.Format(power.RangeSecondary, Style);

            txtLevel.Text = Convert.ToString(power.Level, CultureInfo.InvariantCulture);
            txtAcc.Text = $@"{Convert.ToDecimal(power.Accuracy):##0.###}";
            txtInterrupt.Text = $@"{Convert.ToDecimal(power.InterruptTime):##0.###}";
            txtCastTime.Text = $@"{Convert.ToDecimal(power.CastTimeReal):##0.###}";
            txtRechargeTime.Text = $@"{Convert.ToDecimal(power.RechargeTime):##0.###}";
            txtActivate.Text = $@"{Convert.ToDecimal(power.ActivatePeriod):##0.###}";
            txtEndCost.Text = $@"{Convert.ToDecimal(power.EndCost):##0.###}";
            txtRange.Text = $@"{Convert.ToDecimal(power.Range):##0.###}";
            txtRangeSec.Text = $@"{Convert.ToDecimal(power.RangeSecondary):##0.###}";


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

        private void FillTab_Basic()
        {
            var power = myPower;
            txtNameDisplay.Text = power.DisplayName;
            txtNamePower.Text = power.PowerName;
            cbNameGroup.Text = power.GroupName;
            cbNameSet.Text = power.SetName;
            DisplayNameData();
            txtDescLong.Text = power.DescLong;
            txtDescShort.Text = power.DescShort;
            udScaleStart.Value = new decimal(power.VariableStart);
            udScaleMin.Value = new decimal(power.VariableMin);
            udScaleMax.Value = new decimal(power.VariableMax);
            txtScaleName.Text = power.VariableName;
            chkScale.Checked = power.VariableEnabled;
            overideScale.Checked = power.VariableOverride;
            chkBuffCycle.Checked = power.ClickBuff;
            chkAlwaysToggle.Checked = power.AlwaysToggle;
            chkGraphFix.Checked = myPower.SkipMax;
            cbInherentType.SelectedIndex = (int)myPower.InherentType;
            //chkAltSub.Checked = power.SubIsAltColor;
            chkSubInclude.Checked = power.IncludeFlag;
            chkSortOverride.Checked = power.SortOverride;
            txtVisualLocation.Text = $"{power.DisplayLocation}";
            chkSummonStealEffects.Checked = power.AbsorbSummonEffects;
            chkSummonStealAttributes.Checked = power.AbsorbSummonAttributes;
            chkSummonDisplayEntity.Checked = power.ShowSummonAnyway;
            chkNoAUReq.Checked = myPower.NeverAutoUpdateRequirements;
            cbForcedClass.SelectedIndex = DatabaseAPI.NidFromUidClass(power.ForcedClass) + 1;
            chkNoAutoUpdate.Checked = myPower.NeverAutoUpdate;
            chkHidden.Visible = MidsContext.Config.MasterMode;
            chkHidden.Checked = myPower.HiddenPower;
        }

        private void FillTab_Disabling()
        {
            foreach (var pie in myPower.IgnoreEnh)
            {
                if (pie < (Enums.eEnhance)lvDisablePass1.Items.Count)
                {
                    lvDisablePass1.SetSelected((int)pie, true);
                }
            }

            foreach (var pib in myPower.Ignore_Buff)
            {
                if (pib < (Enums.eEnhance)lvDisablePass4.Items.Count)
                {
                    lvDisablePass4.SetSelected((int)pib, true);
                }
            }
        }

        private void FillTab_Effects()
        {
            RefreshFXData();
        }

        private void FillTab_Enhancements()
        {
            RedrawEnhList();
            chkPRFrontLoad.Checked = myPower.AllowFrontLoading;
            chkBoostBoostable.Checked = myPower.BoostBoostable;
            chkBoostUsePlayerLevel.Checked = myPower.BoostUsePlayerLevel;
        }

        private void FillTab_Mutex()
        {
            chkMutexAuto.Checked = myPower.MutexAuto;
            chkMutexSkip.Checked = myPower.MutexIgnore;
            clbMutex.BeginUpdate();
            clbMutex.Items.Clear();
            var strArray = DatabaseAPI.UidMutexAll();
            foreach (var m in strArray)
            {
                var isChecked = myPower.GroupMembership.Any(t => string.Equals(m, t, StringComparison.OrdinalIgnoreCase));
                clbMutex.Items.Add(m, isChecked);
            }

            clbMutex.EndUpdate();
        }

        private void FillTab_Req()
        {
            ReqChanging = true;
            lvPrListing.BeginUpdate();
            lvPrListing.Items.Clear();
            for (var index = 0; index < myPower.Requires.PowerID.Length; index++)
            {
                var items = new string[3];
                if (myPower.Requires.PowerID[index].Length <= 0)
                {
                    continue;
                }

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

            for (var index = 0; index < myPower.Requires.PowerIDNot.Length; index++)
            {
                var items = new string[3];
                if (myPower.Requires.PowerIDNot[index].Length <= 0)
                {
                    continue;
                }

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
            {
                return;
            }

            lvPrListing.Items[0].Selected = true;
        }

        private void Filltab_ReqClasses()
        {
            clbClassReq.BeginUpdate();
            clbClassReq.Items.Clear();
            foreach (var c in DatabaseAPI.Database.Classes)
            {
                var isChecked = false;
                foreach (var rc in myPower.Requires.ClassName)
                {
                    if (string.Equals(c.ClassName, rc, StringComparison.OrdinalIgnoreCase))
                    {
                        isChecked = true;
                    }
                }

                clbClassReq.Items.Add(c.ClassName, isChecked);
            }

            clbClassReq.EndUpdate();
            clbClassExclude.BeginUpdate();
            clbClassExclude.Items.Clear();
            foreach (var c in DatabaseAPI.Database.Classes)
            {
                var isChecked = false;
                foreach (var rcn in myPower.Requires.ClassNameNot)
                {
                    if (string.Equals(c.ClassName, rcn, StringComparison.OrdinalIgnoreCase))
                    {
                        isChecked = true;
                    }
                }

                clbClassExclude.Items.Add(c.ClassName, isChecked);
            }

            clbClassExclude.EndUpdate();
        }

        private void FillTab_Sets()
        {
            DrawAcceptedSets();
        }

        private void FillTab_SubPowers()
        {
            var reqChanging = ReqChanging;
            ReqChanging = true;
            SP_GroupList();
            if (lvSPGroup.Items.Count > 0)
            {
                lvSPGroup.Items[0].Selected = true;
            }

            SP_SetList();
            if (lvSPSet.Items.Count > 0)
            {
                lvSPSet.Items[0].Selected = true;
            }

            SP_PowerList();
            ReqChanging = reqChanging;
            SPFillList();
        }

        private void frmEditPower_Load(object sender, EventArgs e)
        {
            RedrawEnhPicker();
            FillComboBoxes();
            FillComboInherent();
            DrawSetList();
            Req_GroupList();
            FillTab_SubPowers();
            RefreshPowerData();
            CheckScaleValues();
            Updating = false;
            if (chkSubInclude.CheckState == CheckState.Checked)
            {
                cbInherentType.Enabled = true;
            }

            /*foreach (var boost in myPower.BoostsAllowed)
            {
                Debug.WriteLine(boost);
            }*/
            cbCoDFormat.Checked = MidsContext.Config.CoDEffectFormat;
        }

        private static int GetClassByID(int iID)
        {
            var num = DatabaseAPI.Database.EnhancementClasses.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (DatabaseAPI.Database.EnhancementClasses[index].ID == iID)
                    return index;
            return 0;
        }

        private int GetInvSetIndex(Point e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = 0;
            do
            {
                if ((e.X > (enhPadding + 30) * num3) & (e.X < (enhPadding + 30) * (num3 + 1)))
                    num1 = num3;
                ++num3;
            } while (num3 <= 9);

            var num4 = 0;
            do
            {
                if ((e.Y > (enhPadding + 30) * num4) & (e.Y < (enhPadding + 30) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            return num1 + num2 * 10;
        }

        private int GetInvSetListIndex(Point e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = enhAcross - 1;
            for (var index = 0; index <= num3; ++index)
                if ((e.X > (enhPadding + 30) * index) & (e.X < (enhPadding + 30) * (index + 1)))
                    num1 = index;
            var num4 = 0;
            do
            {
                if ((e.Y > (enhPadding + 30) * num4) & (e.Y < (enhPadding + 30) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            return num1 + num2 * enhAcross;
        }

        private void lblStaticIndex_Click(object sender, EventArgs e)
        {
            var result = InputBox.Show("Enter a new static index for this power.", "Add Static Index", false, $"{myPower.StaticIndex}", InputBox.InputBoxIcon.Info, inputBox_StaticIndexValidating);
            if (!result.OK) return;

            lblStaticIndex.Text = result.Text;
            var t = int.TryParse(result.Text, out var pIndex);
            if (!t) return;

            myPower.StaticIndex = pIndex;
            lblStaticIndex.ForeColor = CheckStaticIndex() ? SystemColors.ControlText : Color.DarkRed;
        }

        private void lvDisablePass1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.IgnoreEnh = new Enums.eEnhance[lvDisablePass1.SelectedIndices.Count];
            var num = lvDisablePass1.SelectedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
                myPower.IgnoreEnh[index] = (Enums.eEnhance)lvDisablePass1.SelectedIndices[index];
        }

        private void lvDisablePass4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            myPower.Ignore_Buff = new Enums.eEnhance[lvDisablePass4.SelectedIndices.Count];
            var num = lvDisablePass4.SelectedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
                myPower.Ignore_Buff[index] = (Enums.eEnhance)lvDisablePass4.SelectedIndices[index];
        }

        private void lvFX_DoubleClick(object sender, EventArgs e)
        {
            btnFXEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void lvPrGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvPrGroup.SelectedItems.Count <= 0)
                return;
            Req_SetList();
            if (lvPrSet.Items.Count > 0)
                lvPrSet.Items[0].Selected = true;
        }

        private void lvPrListing_SelectedIndexChanged(object sender, EventArgs e)
        {
            Req_Listing_IndexChanged();
        }

        private void lvPrPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging)
                return;
            Req_UpdateItem();
        }

        private void lvPrSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvPrSet.SelectedItems.Count <= 0)
                return;
            Req_PowerList();
        }

        private void lvSPGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvSPGroup.SelectedItems.Count <= 0)
                return;
            SP_SetList();
            if (lvSPSet.Items.Count > 0)
                lvSPSet.Items[0].Selected = true;
        }

        private void lvSPSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReqChanging || lvSPSet.SelectedItems.Count <= 0)
                return;
            SP_PowerList();
        }

        private void pbEnhancementList_Hover(object sender, MouseEventArgs e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = enhAcross - 1;
            for (var index = 0; index <= num3; ++index)
                if ((e.X > (enhPadding + 30) * index) & (e.X < (enhPadding + 30) * (index + 1)))
                    num1 = index;
            var num4 = 0;
            do
            {
                if ((e.Y > (enhPadding + 30) * num4) & (e.Y < (enhPadding + 30) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            var index1 = num1 + num2 * enhAcross;
            if ((index1 < DatabaseAPI.Database.EnhancementClasses.Length) & (num1 > -1) & (num2 > -1))
                lblEnhName.Text = DatabaseAPI.Database.EnhancementClasses[index1].Name;
            else
                lblEnhName.Text = "";
        }

        private void pbEnhancementList_MouseDown(object sender, MouseEventArgs e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = enhAcross - 1;
            for (var index = 0; index < enhAcross; index++)
            {
                if (!((e.X > (enhPadding + 30) * index) & (e.X < (enhPadding + 30) * (index + 1)))) continue;

                num1 = index;
                break;
            }

            var num4 = 0;
            do
            {
                if ((e.Y > (enhPadding + 30) * num4) & (e.Y < (enhPadding + 30) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            var index1 = num1 + num2 * enhAcross;
            if (!((index1 <= DatabaseAPI.Database.EnhancementClasses.Length - 1) & (num1 > -1) & (num2 > -1)))
                return;
            var flag = false;
            foreach (var pEnh in myPower.Enhancements)
            {
                if (pEnh == DatabaseAPI.Database.EnhancementClasses[index1].ID)
                {
                    flag = true;
                    break;
                }
            }

            if (flag) return;

            var enhList = myPower.Enhancements.ToList();
            enhList.Add(DatabaseAPI.Database.EnhancementClasses[index1].ID);
            myPower.Enhancements = enhList.ToArray();
            Array.Sort(myPower.Enhancements);
            RedrawEnhList();
        }

        private void pbEnhancementList_Paint(object sender, PaintEventArgs e)
        {
            if (bxEnhPicker == null)
                return;
            e.Graphics.DrawImageUnscaled(bxEnhPicker.Bitmap, 0, 0);
        }

        private void pbEnhancements_Hover(object sender, MouseEventArgs e)
        {
            var num = -1;
            var length = myPower.Enhancements.Length;
            for (var index = 0; index <= length; ++index)
                if ((e.X > (enhPadding + 30) * index) & (e.X < (enhPadding + 30) * (index + 1)))
                    num = index;
            var index1 = num;
            if ((index1 < myPower.Enhancements.Length) & (num > -1))
                lblEnhName.Text = DatabaseAPI.Database.EnhancementClasses[GetClassByID(myPower.Enhancements[index1])]
                    .Name;
            else
                lblEnhName.Text = "";
        }

        private void pbEnhancements_MouseDown(object sender, MouseEventArgs e)
        {
            var enhIdx = -1;
            for (var index = 0; index < myPower.Enhancements.Length; index++)
            {
                if (!((e.X > (enhPadding + 30) * index) & (e.X < (enhPadding + 30) * (index + 1)))) continue;

                enhIdx = index;
                break;
            }

            if (!((enhIdx < myPower.Enhancements.Length) & (enhIdx > -1))) return;

            var numArray = new int[myPower.Enhancements.Length];
            for (var index = 0; index < myPower.Enhancements.Length; index++)
            {
                numArray[index] = myPower.Enhancements[index];
            }

            var index1 = 0;
            myPower.Enhancements = new int[myPower.Enhancements.Length - 1];
            var num4 = numArray.Length - 1;
            for (var index2 = 0; index2 < numArray.Length; index2++)
            {
                if (index2 == enhIdx)
                    continue;
                myPower.Enhancements[index1] = numArray[index2];
                index1++;
            }

            Array.Sort(myPower.Enhancements);
            RedrawEnhList();
        }

        private void pbEnhancements_Paint(object sender, PaintEventArgs e)
        {
            if (bxEnhPicked == null)
                return;
            e.Graphics.DrawImageUnscaled(bxEnhPicked.Bitmap, 0, 0);
        }

        private void pbInvSetList_MouseDown(object sender, MouseEventArgs e)
        {
            var invSetListIndex = GetInvSetListIndex(new Point(e.X, e.Y));
            var setTypes = DatabaseAPI.Database.SetTypes;

            if (!((invSetListIndex < setTypes.Count) & (invSetListIndex > -1))) return;

            var flag = false;

            for (var index = 0; index <= myPower.SetTypes.Count - 1; ++index)
            {
                if (myPower.SetTypes[index] == invSetListIndex)
                {
                    flag = true;
                }
            }

            if (flag | (myPower.SetTypes.Count > 15))
            {
                return;
            }

            var eSetTypeList = myPower.SetTypes;
            eSetTypeList.Add(invSetListIndex);
            myPower.SetTypes = eSetTypeList;
            DrawAcceptedSets();
        }

        private void pbInvSetList_MouseMove(object sender, MouseEventArgs e)
        {
            var invSetListIndex = GetInvSetListIndex(new Point(e.X, e.Y));
            var setTypes = DatabaseAPI.Database.SetTypes;

            if ((invSetListIndex < setTypes.Count) & (invSetListIndex > -1))
            {
                lblInvSet.Text = DatabaseAPI.GetSetTypeByIndex(invSetListIndex).Name;
            }
            else
            {
                lblInvSet.Text = "";
            }
        }

        private void pbInvSetList_Paint(object sender, PaintEventArgs e)
        {
            if (bxSetList == null)
                return;
            e.Graphics.DrawImageUnscaled(bxSetList.Bitmap, 0, 0);
        }

        private void pbInvSetUsed_MouseDown(object sender, MouseEventArgs e)
        {
            var invSetIndex = GetInvSetIndex(new Point(e.X, e.Y));
            if (!((invSetIndex < myPower.SetTypes.Count) & (invSetIndex > -1)))
            {
                return;
            }

            myPower.SetTypes.Remove(myPower.SetTypes[invSetIndex]);

            DrawAcceptedSets();
        }

        private void pbInvSetUsed_MouseMove(object sender, MouseEventArgs e)
        {
            var invSetIndex = GetInvSetIndex(new Point(e.X, e.Y));

            if (invSetIndex < myPower.SetTypes.Count & invSetIndex > -1)
            {
                lblInvSet.Text = DatabaseAPI.GetSetTypeByIndex(myPower.SetTypes[invSetIndex]).Name;
            }
            else
            {
                lblInvSet.Text = "";
            }
        }

        private void pbInvSetUsed_Paint(object sender, PaintEventArgs e)
        {
            if (bxSet == null)
                return;
            e.Graphics.DrawImageUnscaled(bxSet.Bitmap, 0, 0);
        }

        private static bool PowerFullNameIsUnique(string iFullName, int skipId = -1)
        {
            if (string.IsNullOrEmpty(iFullName))
            {
                return true;
            }

            return !DatabaseAPI.Database.Power
                .Where((t, index) => index != skipId && string.Equals(t.FullName, iFullName, StringComparison.OrdinalIgnoreCase))
                .Any();
        }

        private void rbFlagX_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            FillAdvAtrList();
        }

        private string[][] AddEmptyToJagged(string[][] originalArray)
        {
            var jaggedList = originalArray.Select(x => x.ToList()).ToList();
            jaggedList.Add(new List<string> { "Empty", "" });
            string[][] output = jaggedList.Select(a => a.ToArray()).ToArray();
            return output;
        }

        private void rbPrAdd_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("If this power is required to be present, click 'Yes'.\r\nIf this power is NOT required to be present, click 'No'.", "Query", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                // myPower.Requires.PowerIDNot = (string[][]) Utils.CopyArray(myPower.Requires.PowerIDNot, new string[myPower.Requires.PowerIDNot.Length + 1][]);
                // myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1] = new string[2];
                // myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1][0] = "Empty";
                // myPower.Requires.PowerIDNot[myPower.Requires.PowerIDNot.Length - 1][1] = "";
                myPower.Requires.PowerIDNot = AddEmptyToJagged(myPower.Requires.PowerIDNot);
                FillTab_Req();
                lvPrListing.Items[^1].Selected = true;
                lvPrListing.Items[^1].EnsureVisible();
            }
            else
            {
                // myPower.Requires.PowerID = (string[][]) Utils.CopyArray(myPower.Requires.PowerID, new string[myPower.Requires.PowerID.Length + 1][]);
                // myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1] = new string[2];
                // myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1][0] = "Empty";
                // myPower.Requires.PowerID[myPower.Requires.PowerID.Length - 1][1] = "";
                myPower.Requires.PowerID = AddEmptyToJagged(myPower.Requires.PowerID);
                FillTab_Req();
                lvPrListing.Items[myPower.Requires.PowerID.Length - 1].Selected = true;
                lvPrListing.Items[myPower.Requires.PowerID.Length - 1].EnsureVisible();
            }
        }

        private void rbPrPowerX_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == rbPrPowerB.GetType() && ((Control)sender).Text == "Power B")
                return;
            btnPrSetNone.Text = rbPrPowerA.Checked ? "Set Power A to None" : "Set Power B to None";
            Req_Listing_IndexChanged();
        }

        private void rbPrRemove_Click(object sender, EventArgs e)
        {
            if (lvPrListing.SelectedItems.Count < 1)
                return;
            var num1 = (int)Math.Round(Convert.ToDouble(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            if (lvPrListing.SelectedIndices[0] > myPower.Requires.PowerID.Length - 1)
            {
                var strArray1 = new string[myPower.Requires.PowerIDNot.Length][];
                var num2 = num1;
                var num3 = strArray1.Length - 1;
                for (var index = 0; index <= num3; ++index)
                {
                    var strArray2 = new string[2];
                    strArray1[index] = strArray2;
                    strArray1[index][0] = myPower.Requires.PowerIDNot[index][0];
                    strArray1[index][1] = myPower.Requires.PowerIDNot[index][1];
                }

                myPower.Requires.PowerIDNot = new string[myPower.Requires.PowerIDNot.Length - 1][];
                var index1 = 0;
                var num4 = strArray1.Length - 1;
                for (var index2 = 0; index2 <= num4; ++index2)
                {
                    if (index2 == num2)
                        continue;
                    var strArray2 = new string[2];
                    myPower.Requires.PowerIDNot[index1] = strArray2;
                    myPower.Requires.PowerIDNot[index1][0] = strArray1[index2][0];
                    myPower.Requires.PowerIDNot[index1][1] = strArray1[index2][1];
                    ++index1;
                }
            }
            else
            {
                var strArray1 = new string[myPower.Requires.PowerID.Length][];
                var num2 = num1;
                var num3 = strArray1.Length - 1;
                for (var index = 0; index <= num3; ++index)
                {
                    var strArray2 = new string[2];
                    strArray1[index] = strArray2;
                    strArray1[index][0] = myPower.Requires.PowerID[index][0];
                    strArray1[index][1] = myPower.Requires.PowerID[index][1];
                }

                myPower.Requires.PowerID = new string[myPower.Requires.PowerID.Length - 1][];
                var index1 = 0;
                var num4 = strArray1.Length - 1;
                for (var index2 = 0; index2 <= num4; ++index2)
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

        private void RedrawEnhList()
        {
            bxEnhPicked = new ExtendedBitmap(pbEnhancements.Width, pbEnhancements.Height);
            var enhPadding1 = enhPadding;
            var enhPadding2 = enhPadding;
            using var solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxEnhPicked.Graphics.FillRectangle(solidBrush, bxEnhPicked.ClipRect);
            var graphics = bxEnhPicked.Graphics;
            using var pen = new Pen(Color.FromArgb(0, 0, byte.MaxValue), 1f);
            var size = bxEnhPicked.Size;
            var width = size.Width - 1;
            size = bxEnhPicked.Size;
            var height = size.Height - 1;
            graphics.DrawRectangle(pen, 0, 0, width, height);
            var num = myPower.Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxEnhPicked.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(GetClassByID(myPower.Enhancements[index])), GraphicsUnit.Pixel);
                enhPadding2 += 30 + enhPadding;
            }

            pbEnhancements.CreateGraphics().DrawImageUnscaled(bxEnhPicked.Bitmap, 0, 0);
        }

        private void RedrawEnhPicker()
        {
            pbEnhancementList.Width = (enhPadding + 30) * enhAcross + enhPadding;
            pbEnhancementList.Height = (enhPadding + 30) * 6 + enhPadding;
            bxEnhPicker = new ExtendedBitmap(pbEnhancementList.Width, pbEnhancementList.Height);
            var enhPadding1 = enhPadding;
            var enhPadding2 = enhPadding;
            var num1 = 0;
            using var solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxEnhPicker.Graphics.FillRectangle(solidBrush, bxEnhPicker.ClipRect);
            var graphics = bxEnhPicker.Graphics;
            using var pen = new Pen(Color.FromArgb(0, 0, byte.MaxValue), 1f);
            var size = bxEnhPicker.Size;
            var width = size.Width - 1;
            size = bxEnhPicker.Size;
            var height = size.Height - 1;
            graphics.DrawRectangle(pen, 0, 0, width, height);
            var num2 = DatabaseAPI.Database.EnhancementClasses.Length - 1;
            for (var ecIndex = 0; ecIndex <= num2; ++ecIndex)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxEnhPicker.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(ecIndex), GraphicsUnit.Pixel);
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

        private void RefreshPowerData()
        {
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.FullName})";
            lblStaticIndex.Text = Convert.ToString(myPower.StaticIndex, CultureInfo.InvariantCulture);
            lblStaticIndex.ForeColor = CheckStaticIndex() ? SystemColors.ControlText : Color.DarkRed;
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

        private void RefreshFXData(int Index = 0)
        {
            var power = myPower;
            lvFX.BeginUpdate();
            lvFX.Items.Clear();
            var num = power.Effects.Length;
            for (var index = 0; index < num; index++)
            {
                lvFX.Items.Add(power.Effects[index].BuildEffectString(false, "", false, false, true, false, true).Replace("\r\n", " - "));
            }

            lvFX.EndUpdate();
            if (lvFX.Items.Count > Index)
                lvFX.SelectedIndex = Index;
            else
                lvFX.SelectedIndex = lvFX.Items.Count - 1;
        }

        private void Req_GroupList()
        {
            lvPrGroup.BeginUpdate();
            lvPrGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvPrGroup.Items.Add(key);
            lvPrGroup.EndUpdate();
        }

        private void Req_Listing_IndexChanged()
        {
            if (lvPrListing.SelectedIndices.Count < 1)
                return;
            var index = (int)Math.Round(Convert.ToDouble(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
            ReqDisplayPower(lvPrListing.SelectedIndices[0] <= myPower.Requires.PowerID.Length - 1
                ? !rbPrPowerA.Checked ? myPower.Requires.PowerID[index][1] : myPower.Requires.PowerID[index][0]
                : !rbPrPowerA.Checked
                    ? myPower.Requires.PowerIDNot[index][1]
                    : myPower.Requires.PowerIDNot[index][0]);
        }

        private void Req_PowerList()
        {
            lvPrPower.BeginUpdate();
            lvPrPower.Items.Clear();
            if (lvPrSet.SelectedIndices.Count < 1)
            {
                lvPrPower.EndUpdate();
            }
            else
            {
                var index1 =
                    DatabaseAPI.NidFromUidPowerset(Convert.ToString(lvPrSet.SelectedItems[0].Tag,
                        CultureInfo.InvariantCulture));
                if (index1 > -1)
                {
                    var num = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                    for (var index2 = 0; index2 <= num; ++index2)
                        if (!DatabaseAPI.Database.Powersets[index1].Powers[index2].HiddenPower)
                            lvPrPower.Items.Add(DatabaseAPI.Database.Powersets[index1].Powers[index2].PowerName);
                }

                lvPrPower.EndUpdate();
            }
        }

        private void Req_SetList()
        {
            lvPrSet.BeginUpdate();
            lvPrSet.Items.Clear();
            if (lvPrGroup.SelectedIndices.Count < 1)
            {
                lvPrSet.EndUpdate();
            }
            else
            {
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvPrGroup.SelectedItems[0].Text);
                var num = indexesByGroupName.Length - 1;
                for (var index = 0; index <= num; ++index)
                {
                    lvPrSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvPrSet.Items[^1].Tag =
                        DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }

                lvPrSet.EndUpdate();
            }
        }

        private void Req_UpdateItem()
        {
            if ((lvPrListing.SelectedIndices.Count < 1) | (lvPrGroup.SelectedIndices.Count < 1) |
                (lvPrSet.SelectedIndices.Count < 1) | (lvPrPower.SelectedIndices.Count < 1))
                return;
            var str = lvPrGroup.SelectedItems[0].Text + "." + lvPrSet.SelectedItems[0].Text + "." +
                      lvPrPower.SelectedItems[0].Text;
            var index = (int)Math.Round(Convert.ToDouble(RuntimeHelpers.GetObjectValue(lvPrListing.SelectedItems[0].Tag)));
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

        private void ReqDisplayPower(string iPower)
        {
            ReqChanging = true;
            var strArray = iPower.Split(".".ToCharArray());
            if (strArray.Length > 0)
            {
                var num = lvPrGroup.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
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
                var num = lvPrSet.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
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
                var num = lvPrPower.Items.Count - 1;
                for (var index = 0; index <= num; ++index)
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

        private void SetDynamics()
        {
            var power = myPower;
            chkBuffCycle.Enabled = power.PowerType == Enums.ePowerType.Click;
            chkAlwaysToggle.Enabled = power.PowerType == Enums.ePowerType.Toggle;
            if ((power.ActivatePeriod > 0.0) & (power.PowerType == Enums.ePowerType.Toggle))
            {
                lblEndCost.Text = $@"{Convert.ToDecimal(power.EndCost / power.ActivatePeriod):##0.##}/s";
                //lblEndCost.Text = "(" + Strings.Format((float) (power.EndCost / (double) power.ActivatePeriod), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "/s)";
            }
            else
            {
                lblEndCost.Text = "";
            }

            lblAcc.Text = $@"({Convert.ToDecimal(power.Accuracy * MidsContext.Config.ScalingToHit * 100.0):##0.##}%)";
            //lblAcc.Text = "(" + Strings.Format((float) (power.Accuracy * (double) DatabaseAPI.ServerData.BaseToHit * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "#") + "%)";
        }

        private void SetFullName()
        {
            var power = myPower;
            power.FullName = power.GroupName + "." + power.SetName + "." + power.PowerName;
        }

        private void SP_GroupList()
        {
            lvSPGroup.BeginUpdate();
            lvSPGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
                lvSPGroup.Items.Add(key);
            lvSPGroup.EndUpdate();
        }

        private void SP_PowerList()
        {
            lvSPPower.BeginUpdate();
            lvSPPower.Items.Clear();
            if (lvSPSet.SelectedIndices.Count < 1)
            {
                lvSPPower.EndUpdate();
            }
            else
            {
                var index1 =
                    DatabaseAPI.NidFromUidPowerset(Convert.ToString(lvSPSet.SelectedItems[0].Tag,
                        CultureInfo.InvariantCulture));
                if (index1 > -1)
                {
                    var num = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                    for (var index2 = 0; index2 <= num; ++index2)
                    {
                        if (DatabaseAPI.Database.Powersets[index1].Powers[index2].HiddenPower)
                            continue;
                        lvSPPower.Items.Add(DatabaseAPI.Database.Powersets[index1].Powers[index2].PowerName);
                        lvSPPower.Items[^1].Tag =
                            DatabaseAPI.Database.Powersets[index1].Powers[index2].FullName;
                    }
                }

                lvSPPower.EndUpdate();
            }
        }

        private void SP_SetList()
        {
            lvSPSet.BeginUpdate();
            lvSPSet.Items.Clear();
            if (lvSPGroup.SelectedIndices.Count < 1)
            {
                lvSPSet.EndUpdate();
            }
            else
            {
                var indexesByGroupName = DatabaseAPI.GetPowersetIndexesByGroupName(lvSPGroup.SelectedItems[0].Text);
                var num = indexesByGroupName.Length - 1;
                for (var index = 0; index <= num; ++index)
                {
                    lvSPSet.Items.Add(DatabaseAPI.Database.Powersets[indexesByGroupName[index]].SetName);
                    lvSPSet.Items[^1].Tag =
                        DatabaseAPI.Database.Powersets[indexesByGroupName[index]].FullName;
                }

                lvSPSet.EndUpdate();
            }
        }

        private void SPFillList()
        {
            lvSPSelected.BeginUpdate();
            lvSPSelected.Items.Clear();
            var num = myPower.UIDSubPower.Length - 1;
            for (var index = 0; index <= num; ++index)
                lvSPSelected.Items.Add(myPower.UIDSubPower[index]);
            lvSPSelected.EndUpdate();
        }

        private void Store_Req_Classes()
        {
            myPower.Requires.ClassName = new string[clbClassReq.CheckedIndices.Count];
            var num1 = clbClassReq.CheckedIndices.Count - 1;
            for (var index = 0; index <= num1; ++index)
                myPower.Requires.ClassName[index] =
                    DatabaseAPI.Database.Classes[clbClassReq.CheckedIndices[index]].ClassName;
            myPower.Requires.ClassNameNot = new string[clbClassExclude.CheckedIndices.Count];
            var num2 = clbClassExclude.CheckedIndices.Count - 1;
            for (var index = 0; index <= num2; ++index)
                myPower.Requires.ClassNameNot[index] =
                    DatabaseAPI.Database.Classes[clbClassExclude.CheckedIndices[index]].ClassName;
        }

        private void txtAcc_Leave(object sender, EventArgs e)
        {
            txtAcc_TextChanged(null, EventArgs.Empty);
            if (Updating)
            {
                return;
            }

            txtAcc.Text = Convert.ToString(myPower.Accuracy, CultureInfo.InvariantCulture);
        }

        private void txtAcc_TextChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            var res = float.TryParse(txtAcc.Text, out var num);
            if (!res) return;

            if (num is < 0 or > 100)
            {
                num = Math.Max(0, Math.Min(100, num));
                txtAcc.Text = Convert.ToString(num, CultureInfo.InvariantCulture);
            }

            myPower.Accuracy = num;
            lblAcc.Text = $@"({myPower.Accuracy * MidsContext.Config.ScalingToHit * 100:##0.##}%)";
        }

        private void txtActivate_Leave(object sender, EventArgs e)
        {
            txtActivate_TextChanged(null, EventArgs.Empty);

            if (Updating)
            {
                return;
            }

            txtActivate.Text = Convert.ToString(myPower.ActivatePeriod, CultureInfo.InvariantCulture);
        }

        private void txtActivate_TextChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            var power = myPower;
            var ret = float.TryParse(txtActivate.Text, out var num);
            if (!ret) return;

            if (num < 0)
            {
                num = 0;
                txtActivate.Text = Convert.ToString(num, CultureInfo.InvariantCulture);
            }
            else if (num > 2147483904)
            {
                num = 2147483904;
                txtActivate.Text = Convert.ToString(num, CultureInfo.InvariantCulture);
            }

            power.ActivatePeriod = num;

            if ((power.ActivatePeriod > 0) & (power.PowerType == Enums.ePowerType.Toggle))
            {
                lblEndCost.Text = $"{power.EndCost / power.ActivatePeriod:##0.##}/s";
            }
            else
            {
                lblEndCost.Text = "";
            }
        }

        private void txtArc_Leave(object sender, EventArgs e)
        {
            txtArc_TextChanged(null, EventArgs.Empty);

            if (Updating)
            {
                return;
            }

            txtArc.Text = Convert.ToString(myPower.Arc, CultureInfo.InvariantCulture);
        }

        private void txtArc_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtArc.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and < 360)
            {
                myPower.Arc = num;
            }
        }

        private void txtCastTime_Leave(object sender, EventArgs e)
        {
            txtCastTime_TextChanged(null, EventArgs.Empty);

            if (Updating)
            {
                return;
            }

            txtCastTime.Text = Convert.ToString(myPower.CastTimeReal, CultureInfo.InvariantCulture);
        }

        private void txtCastTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtCastTime.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0 & num <= 100)
            {
                myPower.CastTimeReal = num;
            }
        }

        private void txtDescLong_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.DescLong = txtDescLong.Text;
        }

        private void txtDescShort_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.DescShort = txtDescShort.Text;
        }

        private void txtEndCost_Leave(object sender, EventArgs e)
        {
            txtEndCost_TextChanged(null, EventArgs.Empty);

            if (Updating)
            {
                return;
            }

            txtEndCost.Text = Convert.ToString(myPower.EndCost, CultureInfo.InvariantCulture);
        }

        private void txtEndCost_TextChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            var res = float.TryParse(txtEndCost.Text, out var num);
            if (!res) return;

            if (num is < 0 or > 2147483904)
            {
                num = Math.Max(0, Math.Min(2147483904, num));
                txtEndCost.Text = Convert.ToString(num, CultureInfo.InvariantCulture);
            }

            myPower.EndCost = num;
            if (myPower.ActivatePeriod > 0 & myPower.PowerType == Enums.ePowerType.Toggle)
            {
                lblEndCost.Text = $"({myPower.EndCost / myPower.ActivatePeriod:##0.##}/s)";
            }
            else
            {
                lblEndCost.Text = "";
            }
        }

        private void txtInterrupt_Leave(object sender, EventArgs e)
        {
            txtInterrupt_TextChanged(null, EventArgs.Empty);
            
            if (Updating)
            {
                return;
            }

            txtInterrupt.Text = Convert.ToString(myPower.InterruptTime, CultureInfo.InvariantCulture);
        }

        private void txtInterrupt_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtInterrupt.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0 & num <= 100)
            {
                myPower.InterruptTime = num;
            }
        }

        private void txtLevel_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtLevel_TextChanged(null, EventArgs.Empty);
            txtLevel.Text = Convert.ToString(myPower.Level, CultureInfo.InvariantCulture);
        }

        private void txtLevel_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtLevel.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and < 51)
            {
                myPower.Level = num;
            }
        }

        private void txtLifeTimeGame_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtLifeTimeGame_TextChanged(null, EventArgs.Empty);
            txtLifeTimeGame.Text = Convert.ToString(myPower.LifeTimeInGame, CultureInfo.InvariantCulture);
        }

        private void txtLifeTimeGame_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtLifeTimeGame.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.LifeTimeInGame = num;
            }
        }

        private void txtLifeTimeReal_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtLifeTimeReal_TextChanged(null, EventArgs.Empty);
            txtLifeTimeReal.Text = Convert.ToString(myPower.LifeTime, CultureInfo.InvariantCulture);
        }

        private void txtLifeTimeReal_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtLifeTimeReal.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.LifeTime = num;
            }
        }

        private void txtMaxTargets_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtMaxTargets_TextChanged(null, EventArgs.Empty);
            txtMaxTargets.Text = Convert.ToString(myPower.MaxTargets, CultureInfo.InvariantCulture);
        }

        private void txtMaxTargets_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtMaxTargets.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.MaxTargets = (int)Math.Round(num);
            }
        }

        private void txtNamePower_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.PowerName = txtNamePower.Text;
            DisplayNameData();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void txtNamePower_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.PowerName = txtNamePower.Text;
            SetFullName();
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void txtNumCharges_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtNumCharges_TextChanged(null, EventArgs.Empty);
            txtNumCharges.Text = Convert.ToString(myPower.NumCharges, CultureInfo.InvariantCulture);
        }

        private void txtNumCharges_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtNumCharges.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.NumCharges = num;
            }
        }

        private void txtPowerName_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.DisplayName = txtNameDisplay.Text;
        }

        private void txtRadius_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtRadius_TextChanged(null, EventArgs.Empty);
            txtRadius.Text = Convert.ToString(myPower.Radius, CultureInfo.InvariantCulture);
        }

        private void txtRadius_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtRadius.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.Radius = num;
            }
        }

        private void txtRange_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtRange_TextChanged(null, EventArgs.Empty);
            txtRange.Text = Convert.ToString(myPower.Range, CultureInfo.InvariantCulture);
        }

        private void txtRange_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtRange.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and < 2147483904)
            {
                myPower.Range = num;
            }
        }

        private void txtRangeSec_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtRangeSec_TextChanged(null, EventArgs.Empty);
            txtRangeSec.Text = Convert.ToString(myPower.RangeSecondary, CultureInfo.InvariantCulture);
        }

        private void txtRangeSec_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtRangeSec.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0 & num < 2147483904)
            {
                myPower.RangeSecondary = num;
            }
        }

        private void txtRechargeTime_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtRechargeTime_TextChanged(null, EventArgs.Empty);
            txtRechargeTime.Text = Convert.ToString(myPower.RechargeTime, CultureInfo.InvariantCulture);
        }

        private void txtRechargeTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = float.TryParse(txtRechargeTime.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and <= 2147483904)
            {
                myPower.RechargeTime = num;
                myPower.BaseRechargeTime = num;
            }
        }

        private void txtScaleName_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            myPower.VariableName = txtScaleName.Text;
        }

        private void txtUseageTime_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtUseageTime_TextChanged(null, EventArgs.Empty);
            txtUseageTime.Text = Convert.ToString(myPower.UsageTime, CultureInfo.InvariantCulture);
        }

        private void txtUseageTime_TextChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            var ret = int.TryParse(txtUseageTime.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.UsageTime = num;
            }
        }

        private void txtVisualLocation_Leave(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            txtVisualLocation_TextChanged(null, EventArgs.Empty);
            txtVisualLocation.Text = Convert.ToString(myPower.DisplayLocation, CultureInfo.InvariantCulture);
        }

        private void txtVisualLocation_TextChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine($"Updating: {Updating}, cbInherentType: {cbInherentType.SelectedIndex}");

            if (Updating || cbInherentType.SelectedIndex == 0)
            {
                return;
            }

            var ret = int.TryParse(txtVisualLocation.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num >= 0)
            {
                myPower.DisplayLocation = num;
            }
        }

        private void udScaleMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckScaleValues();
        }

        private void udScaleMax_Leave(object sender, EventArgs e)
        {
            var ret = int.TryParse(udScaleMax.Text, out var num);
            if (!ret)
            {
                return;
            }

            myPower.VariableMax = num;
            CheckScaleValues();
        }

        private void udScaleMax_ValueChanged(object sender, EventArgs e)
        {
            myPower.VariableMax = (int)Math.Round(udScaleMax.Value);
            CheckScaleValues();
        }

        private void udScaleMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckScaleValues();
        }

        private void udScaleMin_Leave(object sender, EventArgs e)
        {
            var ret = int.TryParse(udScaleMin.Text, out var num);
            if (!ret)
            {
                return;
            }

            myPower.VariableMin = num;
            CheckScaleValues();
        }

        private void udScaleMin_ValueChanged(object sender, EventArgs e)
        {
            myPower.VariableMin = (int)Math.Round(udScaleMin.Value);
            CheckScaleValues();
        }

        private void udScaleStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckScaleValues();
        }

        private void udScaleStart_Leave(object sender, EventArgs e)
        {
            var ret = int.TryParse(udScaleStart.Text, out var num);
            if (!ret)
            {
                return;
            }

            myPower.VariableStart = num;
            CheckScaleValues();
        }

        private void udScaleStart_ValueChanged(object sender, EventArgs e)
        {
            myPower.VariableStart = (int)Math.Round(udScaleStart.Value);
            CheckScaleValues();
        }

        private void txtNameDisplay_Leave(object sender, EventArgs e)
        {
            Text = $"Edit {(EditMode ? "" : "New ")}Power ({myPower.GroupName}.{myPower.SetName}.{myPower.PowerName})";
        }

        private void cbCoDFormat_CheckedChanged(object sender, EventArgs e)
        {
            MidsContext.Config.CoDEffectFormat = cbCoDFormat.Checked;
            RefreshFXData(lvFX.SelectedIndices.Count <= 0 ? 0 : lvFX.SelectedIndices[0]);
        }

        private void btnJsonExport_Click(object sender, EventArgs e)
        {
            var dlgSave = new SaveFileDialog
            {
                CheckPathExists = true,
                DefaultExt = "json",
                FileName = $"{(string.IsNullOrWhiteSpace(myPower.DisplayName) ? "power" : myPower.DisplayName)}-{myPower.StaticIndex}.json",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (dlgSave.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var targetFile = dlgSave.FileName;
            var objStr = JsonConvert.SerializeObject(myPower, Formatting.Indented);
            File.WriteAllText(targetFile, objStr);
        }

        private void btnJsonImport_Click(object sender, EventArgs e)
        {
            var dlgJsonOpen = new frmJsonImportOptions();
            if (dlgJsonOpen.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var jsonStr = File.ReadAllText(dlgJsonOpen.FileName);

            try
            {
                JsonConverter[] converters =
                {
                    new PowerConverter<IEffect, Effect>(),
                    new PowerConverter<IEnhancement, Enhancement>()
                };

                var obj = JsonConvert.DeserializeObject<Power>(jsonStr, new JsonSerializerSettings { Converters = converters });
                if (obj == null)
                {
                    MessageBox.Show("Could not deserialize JSON to object.");

                    return;
                }

                var displayName = myPower.DisplayName;
                var groupName = myPower.GroupName;
                var setName = myPower.SetName;
                var internalName = myPower.PowerName;
                var staticIndex = myPower.StaticIndex;
                myPower = obj.Clone();
                if (!dlgJsonOpen.OverrideStaticIndex)
                {
                    myPower.StaticIndex = staticIndex;
                }

                if (!dlgJsonOpen.OverrideBasicData & !string.IsNullOrWhiteSpace(groupName) &
                    !string.IsNullOrWhiteSpace(setName) & !string.IsNullOrWhiteSpace(internalName))
                {
                    myPower.DisplayName = displayName;
                    myPower.GroupName = groupName;
                    myPower.SetName = setName;
                    myPower.PowerName = internalName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not deserialize JSON to object.\r\n{ex.GetType()}: {ex.Message}");

                return;
            }

            Updating = true;
            RedrawEnhPicker();
            FillComboBoxes();
            FillComboInherent();
            DrawSetList();
            Req_GroupList();
            FillTab_SubPowers();
            RefreshPowerData();
            CheckScaleValues();
            Updating = false;

            if (chkSubInclude.CheckState == CheckState.Checked)
            {
                cbInherentType.Enabled = true;
            }
        }
    }

    public class PowerConverter<TFromType, TToType> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TFromType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TToType>(reader);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}