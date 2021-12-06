#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Jace;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerEffect : Form
    {
        private bool Loading;

        public IEffect myFX;

        private IPower myPower { get; set; }
        private readonly List<string> ConditionalTypes;
        private readonly List<string> ConditionalOps;
        private readonly int EffectIndex;
        private bool ValidExpression;

        public frmPowerEffect(IEffect iFX, IPower fxPower, int fxIndex = 0)
        {
            Loading = true;
            myPower = fxPower;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            //var componentResourceManager = new ComponentResourceManager(typeof(frmPowerEffect));
            Icon = Resources.reborn;
            ConditionalTypes = new List<string> { "Power Active", "Power Taken", "Stacks", "Team Members" };
            ConditionalOps = new List<string> { "Equal To", "Greater Than", "Less Than" };
            if (iFX != null) myFX = (IEffect) iFX.Clone();
            EffectIndex = fxIndex;
        }

        public frmPowerEffect(IEffect iFX, int fxIndex = 0)
        {
            Loading = true;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            //var componentResourceManager = new ComponentResourceManager(typeof(frmPowerEffect));
            Icon = Resources.reborn;
            ConditionalTypes = new List<string> { "Power Active", "Power Taken", "Stacks", "Team Members" };
            ConditionalOps = new List<string> { "Equal To", "Greater Than", "Less Than" };
            if (iFX != null) myFX = (IEffect) iFX.Clone();
            EffectIndex = fxIndex;
        }

        private void frmPowerEffect_Load(object sender, EventArgs e)
        {
            FillComboBoxes();
            DisplayEffectData();
            if (myFX.GetPower() is { } power)
            {                
                Text = $"Edit Effect {EffectIndex} for: {power.FullName}";
            }
            else if (myFX.Enhancement != null)
            {
                Text = $"Edit Effect for: {myFX.Enhancement.UID}";
            }
            else
            {
                Text = "Edit Effect";
            }

            UpdateConditionalTypes();
            InitSelectedItems();
            Loading = false;
            UpdateFXText();
            CheckMagExpression();
            cbCoDFormat.Checked = MidsContext.Config.CoDEffectFormat;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            FullCopy();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateFXText();
                StoreSuppression();
                DialogResult = DialogResult.OK;
                Hide();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            FullPaste();
        }

        private void cbAffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAffects.SelectedIndex < 0)
                return;
            myFX.ToWho = (Enums.eToWho)cbAffects.SelectedIndex;
            lblAffectsCaster.Text = "";
            var power = myFX.GetPower();
            if (power != null && (power.EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
                lblAffectsCaster.Text = "Power also affects Self";
            UpdateFXText();
        }

        private void cbAspect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAspect.SelectedIndex < 0)
                return;
            myFX.Aspect = (Enums.eAspect)cbAspect.SelectedIndex;
            UpdateFXText();
        }

        private void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAttribute.SelectedIndex < 0)
                return;
            myFX.AttribType = (Enums.eAttribType)cbAttribute.SelectedIndex;
            label7.Visible = myFX.AttribType == Enums.eAttribType.Expression;
            txtMagExpression.Visible = myFX.AttribType == Enums.eAttribType.Expression;
            UpdateFXText();
        }

        private void cbFXClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.EffectClass = (Enums.eEffectClass)cbFXClass.SelectedIndex;
            UpdateFXText();
        }

        private void cbFXSpecialCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            if (myFX.ActiveConditionals.Count > 0)
            {
                MessageBox.Show(@"You cannot use Special Cases when using Conditionals.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                myFX.SpecialCase = Enums.eSpecialCase.None;
            }
            else if (cbFXSpecialCase.SelectedIndex > 0 && myFX.ActiveConditionals.Count == 0)
            {
                myFX.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                Conditionals(false);
            }
            else if (cbFXSpecialCase.SelectedIndex == 0 && myFX.ActiveConditionals.Count == 0)
            {
                myFX.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                Conditionals(true);
            }

            UpdateFXText();
        }

        private void cbModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbModifier.SelectedIndex < 0)
                return;
            myFX.ModifierTable = cbModifier.Text;
            myFX.nModifierTable = DatabaseAPI.NidFromUidAttribMod(myFX.ModifierTable);
            UpdateFXText();
        }

        private void cbPercentageOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbPercentageOverride.SelectedIndex < 0)
                return;
            myFX.DisplayPercentageOverride = (Enums.eOverrideBoolean)cbPercentageOverride.SelectedIndex;
            UpdateFXText();
        }

        private void chkFXBuffable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Buffable = !chkFXBuffable.Checked;
            UpdateFXText();
        }

        private void chkFxNoStack_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Stacking = !chkStack.Checked ? Enums.eStacking.No : Enums.eStacking.Yes;
            UpdateFXText();
        }

        private void chkFXResistible_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Resistible = !chkFXResistable.Checked;
            UpdateFXText();
        }

        private void chkNearGround_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.NearGround = chkNearGround.Checked;
            UpdateFXText();
        }

        private void chkCancelOnMiss_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.CancelOnMiss = chkCancelOnMiss.Checked;
            UpdateFXText();
        }

        private void chkVariable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.VariableModifiedOverride = chkVariable.Checked;
            UpdateFXText();
        }

        private void chkIgnoreScale_CheckChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.IgnoreScaling = chkIgnoreScale.Checked;
            UpdateFXText();
        }

        private void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreSuppression();
            UpdateFXText();
        }

        private void cmbEffectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;

            var index = cmbEffectId.SelectedIndex;
            if (cmbEffectId.SelectedIndex < 0)
                return;

            myFX.EffectId = cmbEffectId.Items[cmbEffectId.SelectedIndex].ToString();
            UpdateFXText();
        }

        private void DisplayEffectData()
        {
            cbPercentageOverride.SelectedIndex = (int)myFX.DisplayPercentageOverride;
            txtFXScale.Text = $"{myFX.Scale:####0.0##}";
            txtFXDuration.Text = $"{myFX.nDuration:####0.0##}";
            txtFXMag.Text = $"{myFX.nMagnitude:####0.0##}";
            txtFXTicks.Text = $"{myFX.Ticks:####0}";
            txtOverride.Text = myFX.Override;
            txtFXDelay.Text = $"{myFX.DelayedTime:####0.0##}";
            txtFXProb.Text = $"{myFX.BaseProbability:####0.0##}";
            txtPPM.Text = $"{myFX.ProcsPerMinute:####0.0##}";
            cbAttribute.SelectedIndex = (int)myFX.AttribType;
            cbAspect.SelectedIndex = (int)myFX.Aspect;
            cbModifier.SelectedIndex = DatabaseAPI.NidFromUidAttribMod(myFX.ModifierTable);
            lblAffectsCaster.Text = "";
            if (myFX.ToWho == Enums.eToWho.All)
            {
                cbAffects.SelectedIndex = 1;
            }
            else
            {
                cbAffects.SelectedIndex = (int)myFX.ToWho;
            }

            var nbFxId = cmbEffectId.Items.Count;
            for (var i = 0; i < nbFxId; i++)
            {
                if (cmbEffectId.Items[i].ToString() != myFX.EffectId) continue;

                cmbEffectId.SelectedIndex = i;
                break;
            }

            var power = myFX.GetPower();
            FillPowerAttribs();
            if (power != null && (power.EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
            {
                lblAffectsCaster.Text = "Power also affects Self";
            }

            cbTarget.SelectedIndex = (int)myFX.PvMode;
            chkStack.Checked = myFX.Stacking == Enums.eStacking.Yes;
            chkFXBuffable.Checked = !myFX.Buffable;
            chkFXResistable.Checked = !myFX.Resistible;
            chkNearGround.Checked = myFX.NearGround;
            chkCancelOnMiss.Checked = myFX.CancelOnMiss;
            IgnoreED.Checked = myFX.IgnoreED;
            cbFXSpecialCase.SelectedIndex = (int)myFX.SpecialCase;
            if (myFX.SpecialCase != Enums.eSpecialCase.None)
            {
                Conditionals(false);
            }
            else
            {
                Conditionals(true);
                UpdateConditionals();
            }

            cbFXClass.SelectedIndex = (int)myFX.EffectClass;
            chkVariable.Checked = myFX.VariableModifiedOverride;
            chkIgnoreScale.Checked = myFX.IgnoreScaling;
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            var names1 = Enum.GetNames(myFX.Suppression.GetType());
            var values = (int[])Enum.GetValues(myFX.Suppression.GetType());
            var num1 = names1.Length;
            for (var index = 0; index < num1; index++)
            {
                clbSuppression.Items.Add(names1[index],
                    (myFX.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            }

            clbSuppression.EndUpdate();
            lvEffectType.BeginUpdate();
            lvEffectType.Items.Clear();
            var index1 = -1;
            var names2 = Enum.GetNames(myFX.EffectType.GetType());
            int num2 = names2.Length - (myPower == null ? 1 : 0);
            for (var index2 = 0; index2 < num2; index2++)
            {
                lvEffectType.Items.Add(names2[index2]);
                if ((Enums.eEffectType)index2 == myFX.EffectType)
                    index1 = index2;
            }

            if (index1 > -1)
            {
                lvEffectType.Items[index1].Selected = true;
                lvEffectType.Items[index1].EnsureVisible();
            }

            lvEffectType.EndUpdate();
            UpdateEffectSubAttribList();

            txtMagExpression.Text = myFX.MagnitudeExpression;
            txtMagExpression.Visible = myFX.AttribType == Enums.eAttribType.Expression;
            label7.Visible = myFX.AttribType == Enums.eAttribType.Expression;
        }

        private void FillComboBoxes()
        {
            cbFXClass.BeginUpdate();
            cbFXSpecialCase.BeginUpdate();
            cbPercentageOverride.BeginUpdate();
            cbAttribute.BeginUpdate();
            cbAspect.BeginUpdate();
            cbModifier.BeginUpdate();
            cbAffects.BeginUpdate();
            cbFXClass.Items.Clear();
            cbFXSpecialCase.Items.Clear();
            cmbEffectId.BeginUpdate();
            cmbEffectId.Items.Clear();
            cbPercentageOverride.Items.Clear();
            cbAttribute.Items.Clear();
            cbAspect.Items.Clear();
            cbModifier.Items.Clear();
            cbAffects.Items.Clear();
            cbFXClass.Items.AddRange(Enum.GetNames(myFX.EffectClass.GetType()));
            cbFXSpecialCase.Items.AddRange(Enum.GetNames(myFX.SpecialCase.GetType()));
            UpdateConditionalTypes();
            cbPercentageOverride.Items.Add("Auto");
            cbPercentageOverride.Items.Add("Yes");
            cbPercentageOverride.Items.Add("No");
            cbAttribute.Items.AddRange(Enum.GetNames(myFX.AttribType.GetType()));
            cbAspect.Items.AddRange(Enum.GetNames(myFX.Aspect.GetType()));
            var num1 = DatabaseAPI.Database.AttribMods.Modifier.Count;
            for (var index = 0; index < num1; index++)
                cbModifier.Items.Add(DatabaseAPI.Database.AttribMods.Modifier[index].ID);
            cbAffects.Items.Add("None");
            cbAffects.Items.Add("Target");
            cbAffects.Items.Add("Self");
            cbAffects.Items.Add("Ally");
            foreach (var effectId in DatabaseAPI.Database.EffectIds)
            {
                cmbEffectId.Items.Add(effectId);
            }
            cmbEffectId.EndUpdate();
            cbFXClass.EndUpdate();
            cbFXSpecialCase.EndUpdate();
            cbPercentageOverride.EndUpdate();
            cbAttribute.EndUpdate();
            cbAspect.EndUpdate();
            cbModifier.EndUpdate();
            cbAffects.EndUpdate();
            /*var strArray = new string[DatabaseAPI.Database.EffectIds.Count - 1 + 1];
            var num2 = DatabaseAPI.Database.EffectIds.Count - 1;
            for (var index = 0; index <= num2; ++index)
                strArray[index] = Convert.ToString(DatabaseAPI.Database.EffectIds[index]);
            if (strArray.Length <= 0)
                return;
            var num3 = strArray.Length - 1;
            for (var index = 0; index <= num3; ++index)
                cmbEffectId.Items.Add(strArray[index]);*/
            lvSubAttribute.Enabled = true;
        }

        private void Conditionals(bool isEnabled)
        {
            if (isEnabled)
            {
                lvConditionalType.Enabled = isEnabled;
                lvSubConditional.Enabled = isEnabled;
                lvConditionalBool.Enabled = isEnabled;
                lvConditionalOp.Enabled = isEnabled;
                lvActiveConditionals.Enabled = isEnabled;
                groupBox2.Enabled = isEnabled;
                lvConditionalType.Refresh();
                lvSubConditional.Refresh();
                lvConditionalBool.Refresh();
                lvConditionalOp.Refresh();
                lvActiveConditionals.Refresh();
                UpdateConditionalTypes();
            }
            else
            {
                lvConditionalType.Enabled = isEnabled;
                lvConditionalType.Items.Clear();
                lvSubConditional.Enabled = isEnabled;
                lvSubConditional.Items.Clear();
                lvConditionalBool.Enabled = isEnabled;
                lvConditionalBool.Items.Clear();
                lvConditionalOp.Enabled = isEnabled;
                lvConditionalOp.Items.Clear();
                lvActiveConditionals.Enabled = isEnabled;
                lvActiveConditionals.Items.Clear();
                myFX.ActiveConditionals.Clear();
                groupBox2.Enabled = isEnabled;
                lvConditionalType.Refresh();
                lvSubConditional.Refresh();
                lvConditionalBool.Refresh();
                lvConditionalOp.Refresh();
                lvActiveConditionals.Refresh();
            }

        }

        private void SelectItemByName(ctlListViewColored lv, string itemName)
        {
            for (var i = 0; i < lv.Items.Count; i++)
            {
                if (lv.Items[i].Text != itemName) continue;

                lv.Items[i].Selected = true;

                // Prevent crash on lv*_Leave()
                lv.FocusedItem = lv.Items[i];
                lv.Select();
                lv.EnsureVisible(i);

                return;
            }
        }
        private void InitSelectedItems()
        {
            if (myFX.EffectType == Enums.eEffectType.None) return;
            SelectItemByName(lvEffectType, myFX.EffectType.ToString());
            if (myFX.EffectType == Enums.eEffectType.ModifyAttrib)
            {
                SelectItemByName(lvSubAttribute, myFX.PowerAttribs.ToString());
            }
            else if (myFX.EffectType == Enums.eEffectType.EntCreate)
            {
                SelectItemByName(lvSubAttribute, myFX.Summon);
            }
            else if (myFX.EffectType == Enums.eEffectType.Mez | myFX.EffectType == Enums.eEffectType.MezResist)
            {
                SelectItemByName(lvSubAttribute, myFX.MezType.ToString());
            }
            else if (myFX.EffectType == Enums.eEffectType.Damage
                     | myFX.EffectType == Enums.eEffectType.DamageBuff
                     | myFX.EffectType == Enums.eEffectType.Defense
                     | myFX.EffectType == Enums.eEffectType.Resistance
                     | myFX.EffectType == Enums.eEffectType.Elusivity)
            {
                SelectItemByName(lvSubAttribute, myFX.DamageType.ToString());
            }
            else if (myFX.EffectType == Enums.eEffectType.Enhancement)
            {
                SelectItemByName(lvSubAttribute, myFX.ETModifies.ToString());
            }
            else if (myFX.EffectType == Enums.eEffectType.PowerRedirect)
            {
                var group = myFX.Override.Split('.');
                SelectItemByName(lvSubAttribute, group[0]);
                UpdateSubSubList();
                SelectItemByName(lvSubSub, myFX.Override);
            }

            if ((myFX.EffectType == Enums.eEffectType.Enhancement | myFX.EffectType == Enums.eEffectType.ResEffect) & myFX.ETModifies == Enums.eEffectType.Mez)
            {
                SelectItemByName(lvSubSub, myFX.MezType.ToString());
            }
            else if (myFX.EffectType == Enums.eEffectType.Enhancement & (myFX.ETModifies == Enums.eEffectType.Defense | myFX.ETModifies == Enums.eEffectType.Damage))
            {
                SelectItemByName(lvSubSub, myFX.DamageType.ToString());
            }
        }

        //change storing of Atr attributes from power to effect
        private void FillPowerAttribs()
        {
            // look at possibly using class to set modified and original values
            if (myPower != null)
            {
                var power = myPower;

                if (Math.Abs(myFX.AtrOrigAccuracy - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigAccuracy = power.Accuracy;
                    myFX.AtrModAccuracy = power.Accuracy;
                }

                if (Math.Abs(myFX.AtrOrigActivatePeriod - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigActivatePeriod = power.ActivatePeriod;
                    myFX.AtrModActivatePeriod = power.ActivatePeriod;
                }

                if (myFX.AtrOrigArc == -1)
                {
                    myFX.AtrOrigArc = power.Arc;
                    myFX.AtrModArc = power.Arc;
                }

                if (Math.Abs(myFX.AtrOrigCastTime - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigCastTime = power.CastTime;
                    myFX.AtrModCastTime = power.CastTime;
                }

                if (myFX.AtrOrigEffectArea == Enums.eEffectArea.None)
                {
                    myFX.AtrOrigEffectArea = power.EffectArea;
                    myFX.AtrModEffectArea = power.EffectArea;
                }

                if (Math.Abs(myFX.AtrOrigEnduranceCost - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigEnduranceCost = power.EndCost;
                    myFX.AtrModEnduranceCost = power.EndCost;
                }

                if (Math.Abs(myFX.AtrOrigInterruptTime - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigInterruptTime = power.InterruptTime;
                    myFX.AtrModInterruptTime = power.InterruptTime;
                }

                if (myFX.AtrOrigMaxTargets == -1)
                {
                    myFX.AtrOrigMaxTargets = power.MaxTargets;
                    myFX.AtrModMaxTargets = power.MaxTargets;
                }

                if (Math.Abs(myFX.AtrOrigRadius - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigRadius = power.Radius;
                    myFX.AtrModRadius = power.Radius;
                }

                if (Math.Abs(myFX.AtrOrigRange - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigRange = power.Range;
                    myFX.AtrModRange = power.Range;
                }

                if (Math.Abs(myFX.AtrOrigRechargeTime - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigRechargeTime = power.RechargeTime;
                    myFX.AtrModRechargeTime = power.RechargeTime;
                }

                if (Math.Abs(myFX.AtrOrigSecondaryRange - (-1)) < float.Epsilon)
                {
                    myFX.AtrOrigSecondaryRange = power.RangeSecondary;
                    myFX.AtrModSecondaryRange = power.RangeSecondary;
                }
            }

            txtFXAccuracy.Text = myFX.AtrModAccuracy.ToString();
            txtFXActivateInterval.Text = myFX.AtrModActivatePeriod.ToString();
            txtFXArc.Text = myFX.AtrModArc.ToString();
            txtFXCastTime.Text = myFX.AtrModCastTime.ToString();
            cbFXEffectArea.Items.AddRange(Enum.GetNames(myFX.AtrModEffectArea.GetType()));
            cbFXEffectArea.SelectedIndex = (int)myFX.AtrModEffectArea;
            txtFXEnduranceCost.Text = myFX.AtrModEnduranceCost.ToString();
            txtFXInterruptTime.Text = myFX.AtrModInterruptTime.ToString();
            txtFXMaxTargets.Text = myFX.AtrModMaxTargets.ToString();
            txtFXRadius.Text = myFX.AtrModRadius.ToString();
            txtFXRange.Text = myFX.AtrModRange.ToString();
            txtFXRechargeTime.Text = myFX.AtrModRechargeTime.ToString();
            txtFXSecondaryRange.Text = myFX.AtrModSecondaryRange.ToString();
        }

        private void FullCopy()
        {
            var format = DataFormats.GetFormat("mhdEffectBIN");
            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);
            myFX.StoreTo(ref writer);
            writer.Close();
            Clipboard.SetDataObject(new DataObject(format.Name, memoryStream.GetBuffer()));
            memoryStream.Close();
        }

        private void FullPaste()
        {
            var format = DataFormats.GetFormat("mhdEffectBIN");
            if (!Clipboard.ContainsData(format.Name))
                MessageBox.Show("No effect data on the clipboard!", "Unable to Paste", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
                using (var memoryStream = new MemoryStream(Clipboard.GetDataObject()?.GetData(format.Name) as byte[] ??
                                                           throw new InvalidOperationException()))
                using (var reader = new BinaryReader(memoryStream))
                {
                    var powerFullName = myFX.PowerFullName;
                    var power = myFX.GetPower();
                    var enhancement = myFX.Enhancement;
                    myFX = new Effect(reader) { PowerFullName = powerFullName };
                    myFX.SetPower(power);
                    myFX.Enhancement = enhancement;
                    DisplayEffectData();
                }
        }

        private void IgnoreED_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.IgnoreED = IgnoreED.Checked;
            UpdateFXText();
        }

        private void lvEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvEffectType.SelectedIndices.Count < 1)
                return;
            myFX.EffectType = (Enums.eEffectType)lvEffectType.SelectedIndices[0];
            if (myFX.EffectType == Enums.eEffectType.ModifyAttrib)
            {
                tableLayoutPanel1.Enabled = false;
                tableLayoutPanel2.Enabled = false;
                tableLayoutPanel3.Enabled = false;
                tableLayoutPanel5.Enabled = false;
                tpPowerAttribs.Visible = true;

                //Old tableLayoutPanel4 contents
                chkStack.Enabled = false;
                chkFXBuffable.Enabled = false;
                IgnoreED.Enabled = false;
                chkFXResistable.Enabled = false;
                chkNearGround.Enabled = false;
                chkCancelOnMiss.Enabled = false;
            }
            else
            {
                tableLayoutPanel1.Enabled = true;
                tableLayoutPanel2.Enabled = true;
                tableLayoutPanel3.Enabled = true;
                tableLayoutPanel5.Enabled = true;
                tpPowerAttribs.Visible = false;

                //Old tableLayoutPanel4 contents
                chkStack.Enabled = true;
                chkFXBuffable.Enabled = true;
                IgnoreED.Enabled = true;
                chkFXResistable.Enabled = true;
                chkNearGround.Enabled = true;
                chkCancelOnMiss.Enabled = true;
            }

            UpdateEffectSubAttribList();
            UpdateFXText();
        }

        private void lvSubAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvSubAttribute.SelectedIndices.Count < 1)
            {
                return;
            }

            var sIndex = lvSubAttribute.SelectedIndices[0];
            var sText = lvSubAttribute.SelectedItems[0].Text;

            if ((myFX.EffectType == Enums.eEffectType.Damage)
                | (myFX.EffectType == Enums.eEffectType.DamageBuff)
                | (myFX.EffectType == Enums.eEffectType.Defense)
                | (myFX.EffectType == Enums.eEffectType.Resistance)
                | (myFX.EffectType == Enums.eEffectType.Elusivity))
            {
                myFX.DamageType = (Enums.eDamage)sIndex;
            }
            else if ((myFX.EffectType == Enums.eEffectType.Mez) | (myFX.EffectType == Enums.eEffectType.MezResist))
            {
                myFX.MezType = (Enums.eMez)sIndex;
            }
            else
            {
                switch (myFX.EffectType)
                {
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                        myFX.ETModifies = (Enums.eEffectType)sIndex;
                        break;
                    case Enums.eEffectType.EntCreate:
                        myFX.Summon = sText;
                        break;
                    case Enums.eEffectType.GlobalChanceMod:
                        myFX.Reward = sText;
                        break;
                    case Enums.eEffectType.GrantPower:
                        myFX.Summon = sText;
                        break;
                    case Enums.eEffectType.ModifyAttrib:
                        myFX.PowerAttribs = (Enums.ePowerAttribs)sIndex;
                        var tpControls = tpPowerAttribs.Controls;
                        for (var rowIndex = 0; rowIndex < tpControls.Count; rowIndex++)
                        {
                            tpControls[rowIndex].Enabled = tpControls[rowIndex].Name.Contains(sText);
                        }
                        //cbTarget.Enabled = true;
                        break;
                }
            }

            UpdateFXText();
            UpdateSubSubList();
        }

        private void lvSubSub_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvSubSub.SelectedIndices.Count < 1)
                return;
            var fx = myFX;
            if ((fx.EffectType == Enums.eEffectType.Enhancement) & (fx.ETModifies == Enums.eEffectType.Mez))
            {
                fx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];
            }
            if ((fx.EffectType == Enums.eEffectType.Enhancement) & (fx.ETModifies == Enums.eEffectType.Damage) | (fx.ETModifies == Enums.eEffectType.Defense))
            {
                fx.DamageType = (Enums.eDamage)lvSubSub.SelectedIndices[0];
            }
            if ((fx.EffectType == Enums.eEffectType.ResEffect) & (fx.ETModifies == Enums.eEffectType.Mez))
            {
                fx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];
            }

            if (fx.EffectType == Enums.eEffectType.PowerRedirect)
            {
                txtOverride.Text = lvSubSub.SelectedItems[0].Text;
            }

            UpdateFXText();
        }

        private void cbTarget_IndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.PvMode = (Enums.ePvX)cbTarget.SelectedIndex;
            UpdateFXText();
        }

        private void StoreSuppression()
        {
            var values = (int[])Enum.GetValues(myFX.Suppression.GetType());
            myFX.Suppression = Enums.eSuppress.None;
            var num = clbSuppression.CheckedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
                //this.myFX.Suppression += (Enums.eSuppress) values[this.clbSuppression.CheckedIndices[index]];
                myFX.Suppression += values[clbSuppression.CheckedIndices[index]];
        }

        private void txtFXDelay_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXDelay.Text = myFX.DelayedTime.ToString(CultureInfo.InvariantCulture);
            UpdateFXText();
        }

        private void txtFXDelay_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fx = myFX;
            var ret = float.TryParse(txtFXDelay.Text, out var num);
            if (!ret)
                return;
            if ((num >= 0.0) & (num <= 2147483904.0))
                fx.DelayedTime = num;
            UpdateFXText();
        }

        private void txtFXDuration_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXDuration.Text = $"{myFX.nDuration:##0.0##}";
            UpdateFXText();
        }

        private void txtFXDuration_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fx = myFX;
            var ret = float.TryParse(txtFXDuration.Text, out var num);
            if (!ret)
                return;
            if ((num >= 0.0) & (num <= 2147483904.0))
                fx.nDuration = num;
            UpdateFXText();
        }

        private void txtFXMag_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXMag.Text = myFX.nMagnitude.ToString(CultureInfo.InvariantCulture);
            UpdateFXText();
        }

        private void txtFXMag_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var inputStr = txtFXMag.Text;
            if (inputStr.EndsWith("%", StringComparison.InvariantCulture))
                inputStr = inputStr.Substring(0, inputStr.Length - 1);
            var ret = float.TryParse(inputStr, out var num);
            if (!ret)
                return;
            if ((num >= -2147483904.0) & (num <= 2147483904.0))
                myFX.nMagnitude = num;
            UpdateFXText();
        }

        void txtFXProb_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var ret = float.TryParse(txtFXProb.Text, out var num);
            if (!ret)
                return;
            if (num >= 0.0 & num <= 2147483904.0)
            {
                if (num > 1.0)
                    num /= 100f;
                myFX.BaseProbability = num;
                //lblProb.Text = $"({fx.BaseProbability * 100:###0}%)";
            }
            UpdateFXText();
        }

        private void txtFXProb_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXProb.Text = myFX.BaseProbability.ToString(CultureInfo.InvariantCulture);
            UpdateFXText();
        }

        private void txtFXScale_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXScale.Text = $"{myFX.Scale:####0.0##}";
            UpdateFXText();
        }

        private void txtFXScale_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fxScaleRaw = txtFXScale.Text;
            if (fxScaleRaw.EndsWith("%", StringComparison.InvariantCulture))
                fxScaleRaw = fxScaleRaw.Substring(0, fxScaleRaw.Length - 1);
            var ret = float.TryParse(fxScaleRaw, out var fxScale);
            if (!ret)
                return;
            //var fxScale = (float)Conversion.Val(fxScaleRaw);
            if ((fxScale >= -2147483904.0) & (fxScale <= 2147483904.0))
                myFX.Scale = fxScale;
            UpdateFXText();
        }

        private void txtFXTicks_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXTicks.Text = Convert.ToString(myFX.Ticks);
            UpdateFXText();
        }

        private void txtFXTicks_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            //var fxTicks = (float)Conversion.Val(txtFXTicks.Text);
            var ret = float.TryParse(txtFXTicks.Text, out var fxTicks);
            if ((fxTicks >= 0.0) & (fxTicks <= 2147483904.0))
                myFX.Ticks = (int)Math.Round(fxTicks);
            UpdateFXText();
        }

        private void txtOverride_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Override = txtOverride.Text;
            UpdateFXText();
        }

        private void txtPPM_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtPPM.Text = myFX.ProcsPerMinute.ToString(CultureInfo.InvariantCulture);
        }

        private void txtPPM_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            //var ppm = (float)Conversion.Val(txtPPM.Text);
            var ret = float.TryParse(txtPPM.Text, out var ppm);
            if ((ppm >= 0.0) & (ppm < 2147483904.0))
                myFX.ProcsPerMinute = ppm;
        }

        private void txtFXAccuracy_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXAccuracy.Text, out var num);
            if (!ret) return;
            myFX.AtrModAccuracy = num;
            UpdateFXText();
        }
        private void txtFXActivateInterval_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXActivateInterval.Text, out var num);
            if (!ret) return;
            myFX.AtrModActivatePeriod = num;
            UpdateFXText();
        }
        private void txtFXArc_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = int.TryParse(txtFXArc.Text, out var num);
            if (!ret) return;
            myFX.AtrModArc = num;
            UpdateFXText();
        }
        private void txtFXCastTime_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXCastTime.Text, out var num);
            if (!ret) return;
            myFX.AtrModCastTime = num;
            UpdateFXText();
        }
        private void cbFXEffectArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            myFX.AtrModEffectArea = (Enums.eEffectArea)cbFXEffectArea.SelectedIndex;
            UpdateFXText();
        }
        private void txtFXEnduranceCost_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXEnduranceCost.Text, out var num);
            if (!ret) return;
            myFX.AtrModEnduranceCost = num;
            UpdateFXText();
        }
        private void txtFXInterruptTime_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXInterruptTime.Text, out var num);
            if (!ret) return;
            myFX.AtrModInterruptTime = num;
            UpdateFXText();
        }
        private void txtFXMaxTargets_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = int.TryParse(txtFXMaxTargets.Text, out var num);
            if (!ret) return;
            myFX.AtrModMaxTargets = num;
            UpdateFXText();
        }
        private void txtFXRadius_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXRadius.Text, out var num);
            if (!ret) return;
            myFX.AtrModRadius = num;
            UpdateFXText();
        }
        private void txtFXRange_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXRange.Text, out var num);
            if (!ret) return;
            myFX.AtrModRange = num;
            UpdateFXText();
        }
        private void txtFXRechargeTime_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXRechargeTime.Text, out var num);
            if (!ret) return;
            myFX.AtrModRechargeTime = num;
            UpdateFXText();
        }
        private void txtFXSecondaryRange_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            var ret = float.TryParse(txtFXSecondaryRange.Text, out var num);
            if (!ret) return;
            myFX.AtrModSecondaryRange = num;
            UpdateFXText();
        }

        private void ListView_Leave(object sender, EventArgs e)
        {
            try
            {
                var lvControl = (ctlListViewColored)sender;
                lvControl.LostFocusItem = lvControl.FocusedItem.Index;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("frmPowerEffect.ListView_Leave(): null sender object");
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            e.DrawBackground();
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var lvControl = (ctlListViewColored) sender;
            if (lvControl.Enabled)
            {
                if (e.Item.Selected)
                {
                    if (lvControl.LostFocusItem == e.Item.Index)
                    {
                        e.Item.BackColor = Color.Goldenrod;
                        e.Item.ForeColor = Color.Black;
                        lvControl.LostFocusItem = -1;
                    }
                    else if (lvControl.Focused)
                    {
                        e.Item.ForeColor = SystemColors.HighlightText;
                        e.Item.BackColor = SystemColors.Highlight;
                    }
                }
                else
                {
                    e.Item.BackColor = lvControl.BackColor;
                    e.Item.ForeColor = lvControl.ForeColor;
                }
            }
            else
            {
                e.Item.ForeColor = SystemColors.GrayText;
            }
            e.DrawBackground();
            e.DrawText();
        }

        private void addConditional_Click(object sender, EventArgs e)
        {
            string powerName;
            string cOp = string.Empty;
            IPower? power;
            string value;
            ListViewItem item;

            if (lvConditionalType.SelectedItems.Count <= 0) return;

            switch (lvConditionalType.SelectedItems[0].Text)
            {
                case "Power Active":
                    if (lvSubConditional.SelectedItems.Count <= 0) return;
                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = $@"Active:{power?.DisplayName}", Name = power?.FullName };
                    item.SubItems.Add("");
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    lvActiveConditionals.Columns[0].Text = @"Currently Active Conditionals";
                    lvActiveConditionals.Columns[0].Width = -2;
                    lvActiveConditionals.Columns[1].Text = @"Value";
                    lvActiveConditionals.Columns[1].Width = -2;
                    myFX.ActiveConditionals.Add(new KeyValue<string, string>($"Active:{powerName}", value));
                    break;
                case "Power Taken":
                    if (lvSubConditional.SelectedItems.Count <= 0) return;
                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = $@"Taken:{power?.DisplayName}", Name = power?.FullName };
                    item.SubItems.Add("");
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    lvActiveConditionals.Columns[0].Text = @"Currently Active Conditionals";
                    lvActiveConditionals.Columns[0].Width = -2;
                    lvActiveConditionals.Columns[1].Text = @"Value";
                    lvActiveConditionals.Columns[1].Width = -2;
                    myFX.ActiveConditionals.Add(new KeyValue<string, string>($"Taken:{powerName}", value));
                    break;
                case "Stacks":
                    if (lvSubConditional.SelectedItems.Count <= 0) return;
                    if (lvConditionalOp.SelectedItems.Count <= 0) return;
                    if (lvConditionalBool.SelectedItems.Count <= 0) return;
                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = $@"Stacks:{power?.DisplayName}", Name = power?.FullName };
                    item.SubItems.Add(cOp);
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    lvActiveConditionals.Columns[0].Text = @"Currently Active Conditionals";
                    lvActiveConditionals.Columns[0].Width = -2;
                    lvActiveConditionals.Columns[1].Text = "";
                    lvActiveConditionals.Columns[1].Width = -2;
                    lvActiveConditionals.Columns[2].Text = @"Value";
                    lvActiveConditionals.Columns[2].Width = -2;
                    myFX.ActiveConditionals.Add(new KeyValue<string, string>($"Stacks:{powerName}", $"{cOp} {value}"));
                    break;
                case "Team Members":
                    if (lvSubConditional.SelectedItems.Count <= 0) return;
                    if (lvConditionalBool.SelectedItems.Count <= 0) return;
                    var archetype = lvSubConditional.SelectedItems[0].Text;
                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = $@"Team:{archetype}", Name = archetype };
                    item.SubItems.Add(cOp);
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    lvActiveConditionals.Columns[0].Text = @"Currently Active Conditionals";
                    lvActiveConditionals.Columns[0].Width = -2;
                    lvActiveConditionals.Columns[1].Text = "";
                    lvActiveConditionals.Columns[1].Width = -2;
                    lvActiveConditionals.Columns[2].Text = @"Value";
                    lvActiveConditionals.Columns[2].Width = -2;
                    myFX.ActiveConditionals.Add(new KeyValue<string, string>($"Team:{archetype}", $"{cOp} {value}"));
                    break;
            }

            UpdateFXText();
        }

        private void removeConditional_Click(object sender, EventArgs e)
        {
            if (lvActiveConditionals.SelectedItems.Count <= 0) return;

            foreach (var cVp in myFX.ActiveConditionals
                .Where(kv => kv.Key.Contains(lvActiveConditionals.SelectedItems[0].Name)).ToList())
            {
                myFX.ActiveConditionals.Remove(cVp);
            }
            lvActiveConditionals.SelectedItems[0].Remove();
            UpdateFXText();
        }

        private void lvSubConditional_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var powName = string.Empty;

            if (lvSubConditional.SelectedItems.Count != 0)
            {
                powName = lvSubConditional.SelectedItems[0].Name;
            }

            var selected = DatabaseAPI.GetPowerByFullName(powName);

            lvConditionalBool.Items.Clear();
            switch (lvConditionalType.SelectedItems[0].Text)
            {
                case "Power Active":
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Add("True");
                    lvConditionalBool.Items.Add("False");
                    lvConditionalBool.Columns[0].Text = @"Power Active?";
                    lvConditionalBool.EndUpdate();
                    break;
                case "Power Taken":
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Add("True");
                    lvConditionalBool.Items.Add("False");
                    lvConditionalBool.Columns[0].Text = @"Power Taken?";
                    lvConditionalBool.EndUpdate();
                    break;
                case "Stacks":
                    lvConditionalBool.BeginUpdate();
                    if (selected != null)
                    {
                        var stackRange = selected.VariableMin == 0 ? Enumerable.Range(selected.VariableMin, selected.VariableMax + 1) : Enumerable.Range(selected.VariableMin, selected.VariableMax);

                        foreach (var stackNum in stackRange)
                        {
                            lvConditionalBool.Items.Add(stackNum.ToString());
                        }
                    }

                    lvConditionalBool.Columns[0].Text = @"# of Stacks?";
                    lvConditionalBool.EndUpdate();
                    break;
                case "Team Members":
                    var tRange = Enumerable.Range(1, 7);
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Clear();
                    foreach (var num in tRange)
                    {
                        lvConditionalBool.Items.Add(num.ToString());
                    }
                    lvConditionalBool.Columns[0].Text = @"# of Members";
                    lvConditionalBool.EndUpdate();
                    break;
            }
        }

        private void lvConditionalType_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            switch (e.Item.Text)
            {
                case "Power Active":
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    var pArray = DatabaseAPI.Database.Power;
                    var eArray = new[] { 6, 7, 8, 9, 10, 11 };
                    foreach (var power in pArray)
                    {
                        var pSetType = power.GetPowerSet().SetType;
                        var pType = power.PowerType;
                        var isType = pType == Enums.ePowerType.Auto_ || pType == Enums.ePowerType.Toggle || (pType == Enums.ePowerType.Click && power.ClickBuff);
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (isUsable && isType)
                        {
                            var pItem = new Regex("[_]");
                            var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                            var pMatch = new Regex("[ ].*");
                            var pArchetype = pMatch.Replace(pStrings[0], "");
                            lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                        }
                    }
                    lvConditionalBool.Size = new Size(97, 259);
                    lvConditionalBool.Location = new Point(460, 14);
                    lvConditionalOp.Visible = false;
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.Columns[0].Width = -2;
                    lvSubConditional.EndUpdate();
                    break;

                case "Power Taken":
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    pArray = DatabaseAPI.Database.Power;
                    eArray = new[] { 6, 7, 8, 9, 10, 11 };
                    foreach (var power in pArray)
                    {
                        var pSetType = power.GetPowerSet().SetType;
                        var pType = power.PowerType;
                        var isType = pType == Enums.ePowerType.Auto_ || pType == Enums.ePowerType.Toggle || (pType == Enums.ePowerType.Click && power.ClickBuff);
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (isUsable || isType)
                        {
                            var pItem = new Regex("[_]");
                            var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                            var pMatch = new Regex("[ ].*");
                            var pArchetype = pMatch.Replace(pStrings[0], "");
                            lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                        }
                    }
                    lvConditionalBool.Size = new Size(97, 259);
                    lvConditionalBool.Location = new Point(460, 14);
                    lvConditionalOp.Visible = false;
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.Columns[0].Width = -2;
                    lvSubConditional.EndUpdate();
                    break;

                case "Stacks":
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    pArray = DatabaseAPI.Database.Power;
                    eArray = new[] { 6, 7, 8, 9, 10, 11 };
                    foreach (var power in pArray)
                    {
                        var pSetType = power.GetPowerSet().SetType;
                        var isType = power.VariableEnabled;
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (isUsable && isType)
                        {
                            var pItem = new Regex("[_]");
                            var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                            var pMatch = new Regex("[ ].*");
                            var pArchetype = pMatch.Replace(pStrings[0], "");
                            lvConditionalBool.Size = new Size(97, 170);
                            lvConditionalBool.Location = new Point(460, 103);
                            lvConditionalOp.Visible = true;
                            lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                        }
                    }

                    lvConditionalOp.Columns[0].Text = @"Stacks are?";
                    lvConditionalBool.Columns[0].Text = @"# of Stacks";
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.Columns[0].Width = -2;
                    lvSubConditional.EndUpdate();
                    break;
                case "Team Members":
                    lvConditionalBool.Size = new Size(97, 170);
                    lvConditionalBool.Location = new Point(460, 103);
                    lvConditionalOp.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    var teamATs = new List<string>
                    {
                        "Any",
                        "Blaster",
                        "Controller",
                        "Defender",
                        "Scrapper",
                        "Tanker",
                        "Peacebringer",
                        "Warshade",
                        "Sentinel",
                        "Brute",
                        "Stalker",
                        "Mastermind",
                        "Dominator",
                        "Corruptor",
                        "Arachnos Soldier",
                        "Arachnos Widow"

                    };
                    foreach (var member in teamATs)
                    {
                        lvSubConditional.Items.Add(member);
                    }
                    lvConditionalOp.Columns[0].Text = @"Members are?";
                    lvSubConditional.Columns[0].Text = @"Team Members";
                    lvSubConditional.Columns[0].Width = -2;
                    lvSubConditional.EndUpdate();
                    break;
            }
        }

        private void UpdateConditionals()
        {
            lvActiveConditionals.BeginUpdate();
            var getCondition = new Regex("(:.*)");
            var getConditionPower = new Regex("(.*:)");
            foreach (var cVp in myFX.ActiveConditionals)
            {
                var condition = getCondition.Replace(cVp.Key, "");
                var conditionPower = getConditionPower.Replace(cVp.Key, "").Replace(":", "");
                var power = DatabaseAPI.GetPowerByFullName(conditionPower);
                switch (condition)
                {
                    case "Active":
                        var item = new ListViewItem { Text = $@"{condition}:{power?.DisplayName}", Name = power?.FullName };
                        item.SubItems.Add("");
                        item.SubItems.Add(cVp.Value);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    case "Taken":
                        item = new ListViewItem { Text = $@"{condition}:{power?.DisplayName}", Name = power?.FullName };
                        item.SubItems.Add("");
                        item.SubItems.Add(cVp.Value);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    case "Stacks":
                        item = new ListViewItem { Text = $@"{condition}:{power?.DisplayName}", Name = power?.FullName };
                        var cVSplit = cVp.Value.Split(' ');
                        item.SubItems.Add(cVSplit[0]);
                        item.SubItems.Add(cVSplit[1]);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    case "Team":
                        item = new ListViewItem { Text = $@"{condition}:{conditionPower}", Name = conditionPower };
                        cVSplit = cVp.Value.Split(' ');
                        item.SubItems.Add(cVSplit[0]);
                        item.SubItems.Add(cVSplit[1]);
                        lvActiveConditionals.Items.Add(item);
                        break;
                }
            }
            lvActiveConditionals.Columns[0].Text = @"Currently Active Conditionals";
            lvActiveConditionals.Columns[0].Width = -2;
            lvActiveConditionals.Columns[1].Text = "";
            lvActiveConditionals.Columns[1].Width = -2;
            lvActiveConditionals.Columns[2].Text = @"Value";
            lvActiveConditionals.Columns[2].Width = -2;
            lvActiveConditionals.EndUpdate();
        }

        private void UpdateConditionalTypes()
        {
            lvConditionalType.BeginUpdate();
            lvConditionalType.Items.Clear();
            var indexVal = ConditionalTypes.Count - 1;
            foreach (var c in ConditionalTypes)
            {
                lvConditionalType.Items.Add(c);
            }

            if (indexVal > -1)
            {
                lvConditionalType.Items[indexVal].Selected = true;
                lvConditionalType.Items[indexVal].EnsureVisible();
            }

            lvConditionalType.View = View.Details;
            lvConditionalType.EndUpdate();

            if (lvConditionalOp.Items.Count != 0) return;

            
            foreach (var op in ConditionalOps)
            {
                lvConditionalOp.Items.Add(op);
            }
        }

        private void UpdateEffectSubAttribList()
        {
            var index1 = 0;
            lvSubAttribute.BeginUpdate();
            lvSubAttribute.Items.Clear();
            var strArray = Array.Empty<string>();
            var fx = myFX;
            if ((fx.EffectType == Enums.eEffectType.Damage) | (fx.EffectType == Enums.eEffectType.DamageBuff) | (fx.EffectType == Enums.eEffectType.Defense) | (fx.EffectType == Enums.eEffectType.Resistance) | (fx.EffectType == Enums.eEffectType.Elusivity))
            {
                strArray = Enum.GetNames(fx.DamageType.GetType());
                index1 = (int)fx.DamageType;
                lvSubAttribute.Columns[0].Text = "Damage Type / Vector";
                lvSubAttribute.Columns[0].Width = -2;
            }
            else if ((fx.EffectType == Enums.eEffectType.Mez) | (fx.EffectType == Enums.eEffectType.MezResist))
            {
                strArray = Enum.GetNames(fx.MezType.GetType());
                index1 = (int)fx.MezType;
                lvSubAttribute.Columns[0].Text = "Mez Type";
                lvSubAttribute.Columns[0].Width = -2;
            }
            else
            {
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.ModifyAttrib:
                        strArray = Enum.GetNames(fx.PowerAttribs.GetType());
                        index1 = (int)fx.PowerAttribs;
                        lvSubAttribute.Columns[0].Text = "Power Attrib";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    case Enums.eEffectType.ResEffect:
                        strArray = Enum.GetNames(fx.EffectType.GetType());
                        index1 = (int)fx.ETModifies;
                        lvSubAttribute.Columns[0].Text = "Effect Type";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    case Enums.eEffectType.EntCreate:
                    {
                        strArray = new string[DatabaseAPI.Database.Entities.Length - 1 + 1];
                        var lower = fx.Summon.ToLower();
                        var num = DatabaseAPI.Database.Entities.Length - 1;
                        for (var index2 = 0; index2 <= num; ++index2)
                        {
                            strArray[index2] = DatabaseAPI.Database.Entities[index2].UID;
                            if (strArray[index2].ToLower() == lower)
                                index1 = index2;
                        }

                        lvSubAttribute.Columns[0].Text = "Entity Name";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }
                    case Enums.eEffectType.GrantPower:
                    {
                        strArray = new string[DatabaseAPI.Database.Power.Length - 1 + 1];
                        var lower = fx.Summon.ToLower();
                        var num = DatabaseAPI.Database.Power.Length - 1;
                        for (var index2 = 0; index2 <= num; ++index2)
                        {
                            strArray[index2] = DatabaseAPI.Database.Power[index2].FullName;
                            if (strArray[index2].ToLower() == lower)
                                index1 = index2;
                        }

                        lvSubAttribute.Columns[0].Text = "Power Name";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }
                    case Enums.eEffectType.Enhancement:
                        strArray = Enum.GetNames(fx.EffectType.GetType());
                        index1 = (int)fx.ETModifies;
                        lvSubAttribute.Columns[0].Text = "Effect Type";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    case Enums.eEffectType.GlobalChanceMod:
                    {
                        strArray = new string[DatabaseAPI.Database.EffectIds.Count - 1 + 1];
                        var lower = fx.Reward.ToLower();
                        var num = DatabaseAPI.Database.EffectIds.Count - 1;
                        for (var index2 = 0; index2 <= num; ++index2)
                        {
                            strArray[index2] = Convert.ToString(DatabaseAPI.Database.EffectIds[index2]);
                            if (strArray[index2].ToLower() == lower)
                                index1 = index2;
                        }

                        lvSubAttribute.Columns[0].Text = "GlobalChanceMod Flag";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }

                    case Enums.eEffectType.PowerRedirect:
                    {
                        var allowedTypes = new List<Enums.ePowerSetType>()
                        {
                            Enums.ePowerSetType.Ancillary,
                            Enums.ePowerSetType.Incarnate,
                            Enums.ePowerSetType.Inherent,
                            Enums.ePowerSetType.Pet,
                            Enums.ePowerSetType.Primary,
                            Enums.ePowerSetType.Secondary,
                            Enums.ePowerSetType.Pool,
                            Enums.ePowerSetType.Temp
                        };
                        strArray = DatabaseAPI.Database.PowersetGroups.Keys.ToArray();
                        lvSubAttribute.Columns[0].Text = @"Powerset Group";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }
                }
            }

            if (strArray.Length > 0)
            {
                var num = strArray.Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
                    lvSubAttribute.Items.Add(strArray[index2]);
                lvSubAttribute.Enabled = true;
            }
            else
            {
                lvSubAttribute.Enabled = false;
                chSub.Text = "";
            }

            if (lvSubAttribute.Items.Count > index1)
            {
                lvSubAttribute.Items[index1].Selected = true;
                lvSubAttribute.Items[index1].EnsureVisible();
            }

            lvSubAttribute.EndUpdate();
            UpdateSubSubList();
        }

        private void UpdateFXText()
        {
            if (Loading)
                return;
            lblEffectDescription.Text = myFX.BuildEffectString();
        }

        private void UpdateSubSubList()
        {
            var index1 = 0;
            lvSubSub.BeginUpdate();
            lvSubSub.Items.Clear();
            var strArray = Array.Empty<string>();
            if (((myFX.EffectType == Enums.eEffectType.Enhancement) | (myFX.EffectType == Enums.eEffectType.ResEffect)) & (myFX.ETModifies == Enums.eEffectType.Mez))
            {
                lvSubSub.Columns[0].Text = "Mez Type";
                lvSubSub.Columns[0].Width = -2;
                strArray = Enum.GetNames(myFX.MezType.GetType());
                index1 = (int)myFX.MezType;
            }

            if (myFX.EffectType == Enums.eEffectType.Enhancement && (myFX.ETModifies == Enums.eEffectType.Defense) | (myFX.ETModifies == Enums.eEffectType.Damage))
            {
                lvSubSub.Columns[0].Text = @"Damage Type";
                lvSubSub.Columns[0].Width = -2;
                strArray = Enum.GetNames(myFX.DamageType.GetType());
                index1 = (int)myFX.DamageType;
            }

            if (myFX.EffectType == Enums.eEffectType.PowerRedirect)
            {
                strArray = DatabaseAPI.Database.Power.ToList().Where(x => x.GroupName == lvSubAttribute.SelectedItems[0].Text).Select(p => p.FullName).ToArray();
                lvSubSub.Columns[0].Text = @"Power";
                lvSubSub.Columns[0].Width = -2;
            }

            if (strArray.Length > 0)
            {
                var num = strArray.Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
                {
                    var lvSubItem = new ListViewItem(strArray[index2])
                    {
                        ToolTipText = strArray[index2]
                    };
                    lvSubSub.Items.Add(lvSubItem);
                }

                lvSubSub.Enabled = true;
            }
            else
            {
                lvSubSub.Enabled = false;
                lvSubSub.Columns[0].Text = "";
            }

            if (lvSubSub.Items.Count > index1)
            {
                lvSubSub.Items[index1].Selected = true;
                lvSubSub.Items[index1].EnsureVisible();
            }

            lvSubSub.EndUpdate();
        }

        private void lvSubConditional_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvSubConditional.Items.Count <= 0) return;
            if (e.Button != MouseButtons.Right) return;

            var conditionalType = lvConditionalType.SelectedItems.Count <= 0
                ? ""
                : lvConditionalType.Items[lvConditionalType.SelectedItems[0].Index].Text;

            if (conditionalType != "Power Taken" & conditionalType != "Power Active") return;

            using var sf = new frmConditionalAttributeSearch();
            var ret = sf.ShowDialog();
            if (ret == DialogResult.Cancel) return;
            if (sf.SearchTerms.PowerName == "") return;

            var searchAtGroup = sf.SearchTerms.AtGroup switch
            {
                "Any" => "",
                "None" => "",
                _ => sf.SearchTerms.AtGroup.ToLowerInvariant()
            };
            var searchPowerName = sf.SearchTerms.PowerName.ToLowerInvariant();

            var n = lvSubConditional.Items.Count;
            
            for (var i = 0; i < n; i++)
            {
                var lvItem = lvSubConditional.Items[i].Text.ToLowerInvariant();
                if (lvItem.StartsWith(searchPowerName))
                {
                    if (searchAtGroup == "" | lvItem.Contains($"[{searchAtGroup}"))
                    {
                        lvSubConditional.Items[i].Selected = true;
                        lvSubConditional.Items[i].EnsureVisible();
                        
                        return;
                    }
                }
            }

            MessageBox.Show(
                $"No match found for '{sf.SearchTerms.PowerName}'{(searchAtGroup == "" ? "" : $" in AT/group {sf.SearchTerms.AtGroup}")}",
                "Dammit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbCoDFormat_CheckedChanged(object sender, EventArgs e)
        {
            MidsContext.Config.CoDEffectFormat = cbCoDFormat.Checked;
            UpdateFXText();
        }

        private void txtMagExpression_TextChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            myFX.MagnitudeExpression = txtMagExpression.Text;
            UpdateFXText();
            CheckMagExpression();
        }

        private void CheckMagExpression()
        {
            ValidExpression = true;

            var err = "";
            try
            {
                _ = myFX.ParseMagnitudeExpression(0);
            }
            catch (ParseException ex)
            {
                err = $@"{ex.Message}";
                ValidExpression = false;
            }
            catch (InvalidOperationException ex)
            {
                err = $@"{ex.Message}";
                ValidExpression = false;
            }

            if (myFX.MagnitudeExpression.Contains(Effect.MagExprSeparator))
            {
                try
                {
                    _ = myFX.ParseMagnitudeExpression(1);
                }
                catch (ParseException ex)
                {
                    if (string.IsNullOrEmpty(err))
                    {
                        err = $@"{ex.Message}";
                    }
                    else
                    {
                        err += $@" | {ex.Message}";
                    }

                    ValidExpression = false;
                }
                catch (InvalidOperationException ex)
                {
                    if (string.IsNullOrEmpty(err))
                    {
                        err = $@"{ex.Message}";
                    }
                    else
                    {
                        err += $@" | {ex.Message}";
                    }

                    ValidExpression = false;
                }
            }

            label8.Text = err;
        }
    }
}