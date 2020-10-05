using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms
{
    public partial class frmPowerEffect : Form
    {
        private bool Loading;

        public IEffect myFX;

        public frmPowerEffect(IEffect iFX)
        {
            Loading = true;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            var componentResourceManager = new ComponentResourceManager(typeof(frmPowerEffect));
            Icon = Resources.reborn;
            if (iFX != null) myFX = (IEffect) iFX.Clone();
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

        private void btnCSV_Click(object sender, EventArgs e)
        {
            var effect = (IEffect) myFX.Clone();
            try
            {
                effect.ImportFromCSV(Clipboard.GetDataObject()?.GetData("System.String", true).ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            myFX.ImportFromCSV(Clipboard.GetDataObject()?.GetData("System.String", true).ToString());
            DisplayEffectData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateFXText();
            StoreSuppression();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            FullPaste();
        }

        private void cbAffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAffects.SelectedIndex < 0)
                return;
            myFX.ToWho = (Enums.eToWho) cbAffects.SelectedIndex;
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
            myFX.Aspect = (Enums.eAspect) cbAspect.SelectedIndex;
            UpdateFXText();
        }

        private void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAttribute.SelectedIndex < 0)
                return;
            myFX.AttribType = (Enums.eAttribType) cbAttribute.SelectedIndex;
            UpdateFXText();
        }

        private void cbFXClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.EffectClass = (Enums.eEffectClass) cbFXClass.SelectedIndex;
            UpdateFXText();
        }

        private void cbFXSpecialCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }
            //Case1 = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
            myFX.SpecialCase = (Enums.eSpecialCase) cbFXSpecialCase.SelectedIndex;
            //UpdateFXText();
        }

        private void cbFXSpecialCase2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }
            //Case2 = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
            myFX.SpecialCase2 = (Enums.eSpecialCase) cbFXSpecialCase2.SelectedIndex;
            //UpdateFXText();
        }

        private void cbFXOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }
            //CaseOp = (Enums.eSpecialOperator)cbFXOperator.SelectedIndex;
            myFX.SpecialOperator = (Enums.eSpecialOperator)cbFXOperator.SelectedIndex;
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
            myFX.DisplayPercentageOverride = (Enums.eOverrideBoolean) cbPercentageOverride.SelectedIndex;
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

        private void chkFXResistable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Resistible = !chkFXResistable.Checked;
            UpdateFXText();
        }

        private void chkVariable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.VariableModifiedOverride = chkVariable.Checked;
            UpdateFXText();
        }

        private void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreSuppression();
            UpdateFXText();
        }

        private void cmbEffectId_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.EffectId = cmbEffectId.Text;
            UpdateFXText();
        }

        private void DisplayEffectData()
        {
            var Style = "####0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0##";
            var fx = myFX;
            cbPercentageOverride.SelectedIndex = (int) fx.DisplayPercentageOverride;
            txtFXScale.Text = Strings.Format(fx.Scale, Style);
            txtFXDuration.Text = Strings.Format(fx.nDuration, Style);
            txtFXMag.Text = Strings.Format(fx.nMagnitude, Style);
            cmbEffectId.Text = fx.EffectId;
            txtFXTicks.Text = Strings.Format(fx.Ticks, "####0");
            txtOverride.Text = fx.Override;
            txtFXDelay.Text = Strings.Format(fx.DelayedTime, Style);
            txtFXProb.Text = Strings.Format(fx.BaseProbability, Style);
            txtPPM.Text = Strings.Format(fx.ProcsPerMinute, Style);
            lblProb.Text = "(" + Strings.Format((float) (fx.BaseProbability * 100.0), "####0") + "%)";
            cbAttribute.SelectedIndex = (int) fx.AttribType;
            cbAspect.SelectedIndex = (int) fx.Aspect;
            cbModifier.SelectedIndex = DatabaseAPI.NidFromUidAttribMod(fx.ModifierTable);
            lblAffectsCaster.Text = "";
            if (fx.ToWho == Enums.eToWho.All)
                cbAffects.SelectedIndex = 1;
            else
                cbAffects.SelectedIndex = (int) fx.ToWho;
            var power = fx.GetPower();
            if (power != null && (power.EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
                lblAffectsCaster.Text = "Power also affects Self";
            rbIfAny.Checked = fx.PvMode == Enums.ePvX.Any;
            rbIfCritter.Checked = fx.PvMode == Enums.ePvX.PvE;
            rbIfPlayer.Checked = fx.PvMode == Enums.ePvX.PvP;
            chkStack.Checked = fx.Stacking == Enums.eStacking.Yes;
            chkFXBuffable.Checked = !fx.Buffable;
            chkFXResistable.Checked = !fx.Resistible;
            chkNearGround.Checked = fx.NearGround;
            IgnoreED.Checked = fx.IgnoreED;
            cbFXSpecialCase.SelectedIndex = (int) fx.SpecialCase;
            cbFXClass.SelectedIndex = (int) fx.EffectClass;
            chkVariable.Checked = fx.VariableModifiedOverride;
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            var names1 = Enum.GetNames(fx.Suppression.GetType());
            var values = (int[]) Enum.GetValues(fx.Suppression.GetType());
            var num1 = names1.Length - 1;
            for (var index = 0; index <= num1; ++index)
                clbSuppression.Items.Add(names1[index],
                    (fx.Suppression & (Enums.eSuppress) values[index]) != Enums.eSuppress.None);
            clbSuppression.EndUpdate();
            lvEffectType.BeginUpdate();
            lvEffectType.Items.Clear();
            var index1 = -1;
            var names2 = Enum.GetNames(fx.EffectType.GetType());
            var num2 = names2.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
            {
                lvEffectType.Items.Add(names2[index2]);
                if ((Enums.eEffectType) index2 == fx.EffectType)
                    index1 = index2;
            }

            if (index1 > -1)
            {
                lvEffectType.Items[index1].Selected = true;
                lvEffectType.Items[index1].EnsureVisible();
            }

            lvEffectType.EndUpdate();
            UpdateEffectSubAttribList();
        }

        private void FillComboBoxes()
        {
            cbFXClass.BeginUpdate();
            cbFXSpecialCase.BeginUpdate();
            cbFXOperator.BeginUpdate();
            cbFXSpecialCase2.BeginUpdate();
            cbPercentageOverride.BeginUpdate();
            cbAttribute.BeginUpdate();
            cbAspect.BeginUpdate();
            cbModifier.BeginUpdate();
            cbAffects.BeginUpdate();
            cbFXClass.Items.Clear();
            cbFXSpecialCase.Items.Clear();
            cbPercentageOverride.Items.Clear();
            cbAttribute.Items.Clear();
            cbAspect.Items.Clear();
            cbModifier.Items.Clear();
            cbAffects.Items.Clear();
            cbFXClass.Items.AddRange(Enum.GetNames(myFX.EffectClass.GetType()));
            cbFXSpecialCase.Items.AddRange(Enum.GetNames(myFX.SpecialCase.GetType()));
            cbFXOperator.Items.AddRange(Enum.GetNames(myFX.SpecialOperator.GetType()));
            cbFXSpecialCase2.Items.AddRange(Enum.GetNames(myFX.SpecialCase.GetType()));
            cbPercentageOverride.Items.Add("Auto");
            cbPercentageOverride.Items.Add("Yes");
            cbPercentageOverride.Items.Add("No");
            cbAttribute.Items.AddRange(Enum.GetNames(myFX.AttribType.GetType()));
            cbAspect.Items.AddRange(Enum.GetNames(myFX.Aspect.GetType()));
            var num1 = DatabaseAPI.Database.AttribMods.Modifier.Length - 1;
            for (var index = 0; index <= num1; ++index)
                cbModifier.Items.Add(DatabaseAPI.Database.AttribMods.Modifier[index].ID);
            cbAffects.Items.Add("None");
            cbAffects.Items.Add("Target");
            cbAffects.Items.Add("Self");
            cbFXClass.EndUpdate();
            cbFXSpecialCase.EndUpdate();
            cbFXOperator.EndUpdate();
            cbFXSpecialCase2.EndUpdate();
            cbPercentageOverride.EndUpdate();
            cbAttribute.EndUpdate();
            cbAspect.EndUpdate();
            cbModifier.EndUpdate();
            cbAffects.EndUpdate();
            var strArray = new string[DatabaseAPI.Database.EffectIds.Count - 1 + 1];
            var num2 = DatabaseAPI.Database.EffectIds.Count - 1;
            for (var index = 0; index <= num2; ++index)
                strArray[index] = Convert.ToString(DatabaseAPI.Database.EffectIds[index]);
            if (strArray.Length <= 0)
                return;
            var num3 = strArray.Length - 1;
            for (var index = 0; index <= num3; ++index)
                cmbEffectId.Items.Add(strArray[index]);
            lvSubAttribute.Enabled = true;
        }

        private void frmPowerEffect_Load(object sender, EventArgs e)
        {
            FillComboBoxes();
            DisplayEffectData();
            if (myFX.GetPower() is IPower power)
                Text = "Edit Effect " + Convert.ToString(myFX.nID) + " for: " + power.FullName;
            else if (myFX.Enhancement != null)
                Text = "Edit Effect for: " + myFX.Enhancement.UID;
            else
                Text = "Edit Effect";
            Loading = false;
            UpdateFXText();
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
                using (var memoryStream = new MemoryStream((byte[]) Clipboard.GetDataObject()?.GetData(format.Name) ??
                                                           throw new InvalidOperationException()))
                using (var reader = new BinaryReader(memoryStream))
                {
                    var powerFullName = myFX.PowerFullName;
                    var power = myFX.GetPower();
                    var enhancement = myFX.Enhancement;
                    myFX = new Effect(reader) {PowerFullName = powerFullName};
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
            myFX.EffectType = (Enums.eEffectType) lvEffectType.SelectedIndices[0];
            UpdateEffectSubAttribList();
            UpdateFXText();
        }

        private void lvSubAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvSubAttribute.SelectedIndices.Count < 1)
                return;
            var fx = myFX;
            if ((fx.EffectType == Enums.eEffectType.Damage) | (fx.EffectType == Enums.eEffectType.DamageBuff) |
                (fx.EffectType == Enums.eEffectType.Defense) | (fx.EffectType == Enums.eEffectType.Resistance))
                fx.DamageType = (Enums.eDamage) lvSubAttribute.SelectedIndices[0];
            else if ((fx.EffectType == Enums.eEffectType.Mez) | (fx.EffectType == Enums.eEffectType.MezResist))
                fx.MezType = (Enums.eMez) lvSubAttribute.SelectedIndices[0];
            else
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.ResEffect:
                        fx.ETModifies = (Enums.eEffectType) lvSubAttribute.SelectedIndices[0];
                        break;
                    case Enums.eEffectType.EntCreate:
                        fx.Summon = lvSubAttribute.SelectedItems[0].Text;
                        break;
                    case Enums.eEffectType.Enhancement:
                        fx.ETModifies = (Enums.eEffectType) lvSubAttribute.SelectedIndices[0];
                        break;
                    case Enums.eEffectType.GlobalChanceMod:
                        fx.Reward = lvSubAttribute.SelectedItems[0].Text;
                        break;
                    case Enums.eEffectType.GrantPower:
                        fx.Summon = lvSubAttribute.SelectedItems[0].Text;
                        break;
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
                fx.MezType = (Enums.eMez) lvSubSub.SelectedIndices[0];
            }
            if ((fx.EffectType == Enums.eEffectType.Enhancement) & (fx.ETModifies == Enums.eEffectType.Damage) | (fx.ETModifies == Enums.eEffectType.Defense))
            {
                fx.DamageType = (Enums.eDamage) lvSubSub.SelectedIndices[0];
            }

            UpdateFXText();
        }

        private void rbIfACP_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.PvMode = !rbIfCritter.Checked ? !rbIfPlayer.Checked ? Enums.ePvX.Any : Enums.ePvX.PvP : Enums.ePvX.PvE;
            UpdateFXText();
        }

        private void StoreSuppression()
        {
            var values = (int[]) Enum.GetValues(myFX.Suppression.GetType());
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
            var num = (float) Conversion.Val(txtFXDelay.Text);
            if ((num >= 0.0) & (num <= 2147483904.0))
                fx.DelayedTime = num;
            UpdateFXText();
        }

        private void txtFXDuration_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXDuration.Text = Strings.Format(myFX.nDuration,
                "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0##");
            UpdateFXText();
        }

        private void txtFXDuration_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fx = myFX;
            var num = (float) Conversion.Val(txtFXDuration.Text);
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
            var fx = myFX;
            var InputStr = txtFXMag.Text;
            if (InputStr.EndsWith("%", StringComparison.InvariantCulture))
                InputStr = InputStr.Substring(0, InputStr.Length - 1);
            var num = (float) Conversion.Val(InputStr);
            if ((num >= -2147483904.0) & (num <= 2147483904.0))
                fx.nMagnitude = num;
            UpdateFXText();
        }

        private void txtFXProb_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXProb.Text = myFX.BaseProbability.ToString(CultureInfo.InvariantCulture);
            UpdateFXText();
        }

        private void txtFXProb_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fx = myFX;
            var num = (float) Conversion.Val(txtFXProb.Text);
            if ((num >= 0.0) & (num <= 2147483904.0))
            {
                if (num > 1.0)
                    num /= 100f;
                fx.BaseProbability = num;
                lblProb.Text = "(" + Strings.Format((float) (fx.BaseProbability * 100.0), "###0") + "%)";
            }

            UpdateFXText();
        }

        private void txtFXScale_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXScale.Text = Strings.Format(myFX.Scale, "####0.0##");
            UpdateFXText();
        }

        private void txtFXScale_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var fx = myFX;
            var fxScaleRaw = txtFXScale.Text;
            if (fxScaleRaw.EndsWith("%", StringComparison.InvariantCulture))
                fxScaleRaw = fxScaleRaw.Substring(0, fxScaleRaw.Length - 1);
            var fxScale = (float) Conversion.Val(fxScaleRaw);
            if ((fxScale >= -2147483904.0) & (fxScale <= 2147483904.0))
                fx.Scale = fxScale;
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
            var fx = myFX;
            var fxTicks = (float) Conversion.Val(txtFXTicks.Text);
            if ((fxTicks >= 0.0) & (fxTicks <= 2147483904.0))
                fx.Ticks = (int) Math.Round(fxTicks);
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
            var ppm = (float) Conversion.Val(txtPPM.Text);
            if ((ppm >= 0.0) & (ppm < 2147483904.0))
                myFX.ProcsPerMinute = ppm;
        }

        private void UpdateEffectSubAttribList()
        {
            var index1 = 0;
            lvSubAttribute.BeginUpdate();
            lvSubAttribute.Items.Clear();
            var strArray = new string[0];
            var fx = myFX;
            if ((fx.EffectType == Enums.eEffectType.Damage) | (fx.EffectType == Enums.eEffectType.DamageBuff) |
                (fx.EffectType == Enums.eEffectType.Defense) | (fx.EffectType == Enums.eEffectType.Resistance) |
                (fx.EffectType == Enums.eEffectType.Elusivity))
            {
                strArray = Enum.GetNames(fx.DamageType.GetType());
                index1 = (int) fx.DamageType;
                lvSubAttribute.Columns[0].Text = "Damage Type / Vector";
            }
            else if ((fx.EffectType == Enums.eEffectType.Mez) | (fx.EffectType == Enums.eEffectType.MezResist))
            {
                strArray = Enum.GetNames(fx.MezType.GetType());
                index1 = (int) fx.MezType;
                lvSubAttribute.Columns[0].Text = "Mez Type";
            }
            else
            {
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.ResEffect:
                        strArray = Enum.GetNames(fx.EffectType.GetType());
                        index1 = (int) fx.ETModifies;
                        lvSubAttribute.Columns[0].Text = "Effect Type";
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
                        break;
                    }
                    case Enums.eEffectType.Enhancement:
                        strArray = Enum.GetNames(fx.EffectType.GetType());
                        index1 = (int) fx.ETModifies;
                        lvSubAttribute.Columns[0].Text = "Effect Type";
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
            var strArray = new string[0];
            var fx = myFX;
            if (((fx.EffectType == Enums.eEffectType.Enhancement) | (fx.EffectType == Enums.eEffectType.ResEffect)) & (fx.ETModifies == Enums.eEffectType.Mez))
            {
                lvSubSub.Columns[0].Text = "Mez Type";
                strArray = Enum.GetNames(fx.MezType.GetType());
                index1 = (int) fx.MezType;
            }

            if (((fx.EffectType == Enums.eEffectType.Enhancement) | (fx.EffectType == Enums.eEffectType.ResEffect)) & (fx.ETModifies == Enums.eEffectType.Defense) | (fx.ETModifies == Enums.eEffectType.Damage))
            {
                lvSubSub.Columns[0].Text = @"Damage Type";
                strArray = Enum.GetNames(fx.DamageType.GetType());
                index1 = (int) fx.DamageType;
            }


            if (strArray.Length > 0)
            {
                var num = strArray.Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
                    lvSubSub.Items.Add(strArray[index2]);
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

        private void Label3_Click(object sender, EventArgs e)
        {

        }
    }
}