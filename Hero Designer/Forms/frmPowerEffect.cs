using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmPowerEffect : Form
    {
        bool Loading;

        public IEffect myFX;

        public frmPowerEffect(IEffect iFX)
        {
            Loading = true;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmPowerEffect));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            myFX = (IEffect)iFX.Clone();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnCopy_Click(object sender, EventArgs e)
        {
            FullCopy();
        }

        void btnCSV_Click(object sender, EventArgs e)
        {
            IEffect effect = (IEffect)myFX.Clone();
            try
            {
                effect.ImportFromCSV(Clipboard.GetDataObject()?.GetData("System.String", true).ToString());
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num = (int)Interaction.MsgBox(ex.Message);
                ProjectData.ClearProjectError();
                return;
            }
            myFX.ImportFromCSV(Clipboard.GetDataObject()?.GetData("System.String", true).ToString());
            DisplayEffectData();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            StoreSuppression();
            DialogResult = DialogResult.OK;
            Hide();
        }

        void btnPaste_Click(object sender, EventArgs e)
        {
            FullPaste();
        }

        void cbAffects_SelectedIndexChanged(object sender, EventArgs e)
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

        void cbAspect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAspect.SelectedIndex < 0)
                return;
            myFX.Aspect = (Enums.eAspect)cbAspect.SelectedIndex;
            UpdateFXText();
        }

        void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbAttribute.SelectedIndex < 0)
                return;
            myFX.AttribType = (Enums.eAttribType)cbAttribute.SelectedIndex;
            UpdateFXText();
        }

        void cbFXClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.EffectClass = (Enums.eEffectClass)cbFXClass.SelectedIndex;
            UpdateFXText();
        }

        void cbFXSpecialCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
            UpdateFXText();
        }

        void cbModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbModifier.SelectedIndex < 0)
                return;
            myFX.ModifierTable = cbModifier.Text;
            myFX.nModifierTable = DatabaseAPI.NidFromUidAttribMod(myFX.ModifierTable);
            UpdateFXText();
        }

        void cbPercentageOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || cbPercentageOverride.SelectedIndex < 0)
                return;
            myFX.DisplayPercentageOverride = (Enums.eOverrideBoolean)cbPercentageOverride.SelectedIndex;
            UpdateFXText();
        }

        void chkFXBuffable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Buffable = !chkFXBuffable.Checked;
            UpdateFXText();
        }

        void chkFxNoStack_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Stacking = !chkStack.Checked ? Enums.eStacking.No : Enums.eStacking.Yes;
            UpdateFXText();
        }

        void chkFXResistable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Resistible = !chkFXResistable.Checked;
            UpdateFXText();
        }

        void chkVariable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.VariableModifiedOverride = chkVariable.Checked;
            UpdateFXText();
        }

        void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreSuppression();
            UpdateFXText();
        }

        void cmbEffectId_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.EffectId = cmbEffectId.Text;
            UpdateFXText();
        }

        public void DisplayEffectData()
        {
            string Style = "####0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0##";
            IEffect fx = myFX;
            cbPercentageOverride.SelectedIndex = (int)fx.DisplayPercentageOverride;
            txtFXScale.Text = Strings.Format(fx.Scale, Style);
            txtFXDuration.Text = Strings.Format(fx.nDuration, Style);
            txtFXMag.Text = Strings.Format(fx.nMagnitude, Style);
            cmbEffectId.Text = fx.EffectId;
            txtFXTicks.Text = Strings.Format(fx.Ticks, "####0");
            txtOverride.Text = fx.Override;
            txtFXDelay.Text = Strings.Format(fx.DelayedTime, Style);
            txtFXProb.Text = Strings.Format(fx.BaseProbability, Style);
            lblProb.Text = "(" + Strings.Format((float)(fx.BaseProbability * 100.0), "####0") + "%)";
            cbAttribute.SelectedIndex = (int)fx.AttribType;
            cbAspect.SelectedIndex = (int)fx.Aspect;
            cbModifier.SelectedIndex = DatabaseAPI.NidFromUidAttribMod(fx.ModifierTable);
            lblAffectsCaster.Text = "";
            if (fx.ToWho == Enums.eToWho.All)
                cbAffects.SelectedIndex = 1;
            else
                cbAffects.SelectedIndex = (int)fx.ToWho;
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
            cbFXSpecialCase.SelectedIndex = (int)fx.SpecialCase;
            cbFXClass.SelectedIndex = (int)fx.EffectClass;
            chkVariable.Checked = fx.VariableModifiedOverride;
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            string[] names1 = Enum.GetNames(fx.Suppression.GetType());
            int[] values = (int[])Enum.GetValues(fx.Suppression.GetType());
            int num1 = names1.Length - 1;
            for (int index = 0; index <= num1; ++index)
                clbSuppression.Items.Add(names1[index], (fx.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            clbSuppression.EndUpdate();
            lvEffectType.BeginUpdate();
            lvEffectType.Items.Clear();
            int index1 = -1;
            string[] names2 = Enum.GetNames(fx.EffectType.GetType());
            int num2 = names2.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
            {
                lvEffectType.Items.Add(names2[index2]);
                if ((Enums.eEffectType)index2 == fx.EffectType)
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

        void FillComboBoxes()
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
            cbPercentageOverride.Items.Clear();
            cbAttribute.Items.Clear();
            cbAspect.Items.Clear();
            cbModifier.Items.Clear();
            cbAffects.Items.Clear();
            cbFXClass.Items.AddRange(Enum.GetNames(myFX.EffectClass.GetType()));
            cbFXSpecialCase.Items.AddRange(Enum.GetNames(myFX.SpecialCase.GetType()));
            cbPercentageOverride.Items.Add("Auto");
            cbPercentageOverride.Items.Add("Yes");
            cbPercentageOverride.Items.Add("No");
            cbAttribute.Items.AddRange(Enum.GetNames(myFX.AttribType.GetType()));
            cbAspect.Items.AddRange(Enum.GetNames(myFX.Aspect.GetType()));
            int num1 = DatabaseAPI.Database.AttribMods.Modifier.Length - 1;
            for (int index = 0; index <= num1; ++index)
                cbModifier.Items.Add(DatabaseAPI.Database.AttribMods.Modifier[index].ID);
            cbAffects.Items.Add("None");
            cbAffects.Items.Add("Target");
            cbAffects.Items.Add("Self");
            cbFXClass.EndUpdate();
            cbFXSpecialCase.EndUpdate();
            cbPercentageOverride.EndUpdate();
            cbAttribute.EndUpdate();
            cbAspect.EndUpdate();
            cbModifier.EndUpdate();
            cbAffects.EndUpdate();
            string[] strArray = new string[DatabaseAPI.Database.EffectIds.Count - 1 + 1];
            int num2 = DatabaseAPI.Database.EffectIds.Count - 1;
            for (int index = 0; index <= num2; ++index)
                strArray[index] = Convert.ToString(DatabaseAPI.Database.EffectIds[index]);
            if (strArray.Length <= 0)
                return;
            int num3 = strArray.Length - 1;
            for (int index = 0; index <= num3; ++index)
                cmbEffectId.Items.Add(strArray[index]);
            lvSubAttribute.Enabled = true;
        }

        void frmPowerEffect_Load(object sender, EventArgs e)
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

        void FullCopy()
        {
            DataFormats.Format format = DataFormats.GetFormat("mhdEffectBIN");
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            myFX.StoreTo(ref writer);
            writer.Close();
            Clipboard.SetDataObject(new DataObject(format.Name, memoryStream.GetBuffer()));
            memoryStream.Close();
        }

        void FullPaste()
        {
            DataFormats.Format format = DataFormats.GetFormat("mhdEffectBIN");
            if (!Clipboard.ContainsData(format.Name))
            {
                Interaction.MsgBox("No effect data on the clipboard!", MsgBoxStyle.Information, "Unable to Paste");
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream((byte[])Clipboard.GetDataObject()?.GetData(format.Name) ?? throw new InvalidOperationException()))
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    string powerFullName = myFX.PowerFullName;
                    IPower power = myFX.GetPower();
                    IEnhancement enhancement = myFX.Enhancement;
                    myFX = new Effect(reader) {PowerFullName = powerFullName};
                    myFX.SetPower(power);
                    myFX.Enhancement = enhancement;
                    DisplayEffectData();
                }
            }
        }

        void IgnoreED_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.IgnoreED = IgnoreED.Checked;
            UpdateFXText();
        }

        void lvEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvEffectType.SelectedIndices.Count < 1)
                return;
            myFX.EffectType = (Enums.eEffectType)lvEffectType.SelectedIndices[0];
            UpdateEffectSubAttribList();
            UpdateFXText();
        }

        void lvSubAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvSubAttribute.SelectedIndices.Count < 1)
                return;
            IEffect fx = myFX;
            if (fx.EffectType == Enums.eEffectType.Damage | fx.EffectType == Enums.eEffectType.DamageBuff | fx.EffectType == Enums.eEffectType.Defense | fx.EffectType == Enums.eEffectType.Resistance)
                fx.DamageType = (Enums.eDamage)lvSubAttribute.SelectedIndices[0];
            else if (fx.EffectType == Enums.eEffectType.Mez | fx.EffectType == Enums.eEffectType.MezResist)
                fx.MezType = (Enums.eMez)lvSubAttribute.SelectedIndices[0];
            else switch (fx.EffectType)
            {
                case Enums.eEffectType.ResEffect:
                    fx.ETModifies = (Enums.eEffectType)lvSubAttribute.SelectedIndices[0];
                    break;
                case Enums.eEffectType.EntCreate:
                    fx.Summon = lvSubAttribute.SelectedItems[0].Text;
                    break;
                case Enums.eEffectType.Enhancement:
                    fx.ETModifies = (Enums.eEffectType)lvSubAttribute.SelectedIndices[0];
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

        void lvSubSub_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading || lvSubSub.SelectedIndices.Count < 1)
                return;
            IEffect fx = myFX;
            if (fx.EffectType == Enums.eEffectType.Enhancement & fx.ETModifies == Enums.eEffectType.Mez)
                fx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];
            UpdateFXText();
        }

        void rbIfACP_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.PvMode = !rbIfCritter.Checked ? (!rbIfPlayer.Checked ? Enums.ePvX.Any : Enums.ePvX.PvP) : Enums.ePvX.PvE;
            UpdateFXText();
        }

        void StoreSuppression()
        {
            int[] values = (int[])Enum.GetValues(myFX.Suppression.GetType());
            myFX.Suppression = Enums.eSuppress.None;
            int num = clbSuppression.CheckedIndices.Count - 1;
            for (int index = 0; index <= num; ++index)
                //this.myFX.Suppression += (Enums.eSuppress) values[this.clbSuppression.CheckedIndices[index]];
                myFX.Suppression += values[clbSuppression.CheckedIndices[index]];
        }

        void txtFXDelay_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXDelay.Text = Convert.ToString(myFX.DelayedTime);
            UpdateFXText();
        }

        void txtFXDelay_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            float num = (float)Conversion.Val(txtFXDelay.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                fx.DelayedTime = num;
            UpdateFXText();
        }

        void txtFXDuration_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXDuration.Text = Strings.Format(myFX.nDuration, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0##");
            UpdateFXText();
        }

        void txtFXDuration_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            float num = (float)Conversion.Val(txtFXDuration.Text);
            if (num >= 0.0 & num <= 2147483904.0)
                fx.nDuration = num;
            UpdateFXText();
        }

        void txtFXMag_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXMag.Text = Convert.ToString(myFX.nMagnitude);
            UpdateFXText();
        }

        void txtFXMag_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            string InputStr = txtFXMag.Text;
            if (InputStr.EndsWith("%"))
                InputStr = InputStr.Substring(0, InputStr.Length - 1);
            float num = (float)Conversion.Val(InputStr);
            if (num >= -2147483904.0 & num <= 2147483904.0)
                fx.nMagnitude = num;
            UpdateFXText();
        }

        void txtFXProb_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXProb.Text = Convert.ToString(myFX.BaseProbability);
            UpdateFXText();
        }

        void txtFXProb_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            float num = (float)Conversion.Val(txtFXProb.Text);
            if (num >= 0.0 & num <= 2147483904.0)
            {
                if (num > 1.0)
                    num /= 100f;
                fx.BaseProbability = num;
                lblProb.Text = "(" + Strings.Format((float)(fx.BaseProbability * 100.0), "###0") + "%)";
            }
            UpdateFXText();
        }

        void txtFXScale_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXScale.Text = Strings.Format(myFX.Scale, "####0.0##");
            UpdateFXText();
        }

        void txtFXScale_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            string fxScaleRaw = txtFXScale.Text;
            if (fxScaleRaw.EndsWith("%"))
                fxScaleRaw = fxScaleRaw.Substring(0, fxScaleRaw.Length - 1);
            float fxScale = (float)Conversion.Val(fxScaleRaw);
            if (fxScale >= -2147483904.0 & fxScale <= 2147483904.0)
                fx.Scale = fxScale;
            UpdateFXText();
        }

        void txtFXTicks_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtFXTicks.Text = Convert.ToString(myFX.Ticks);
            UpdateFXText();
        }

        void txtFXTicks_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            IEffect fx = myFX;
            float fxTicks = (float)Conversion.Val(txtFXTicks.Text);
            if (fxTicks >= 0.0 & fxTicks <= 2147483904.0)
                fx.Ticks = (int)Math.Round(fxTicks);
            UpdateFXText();
        }

        void txtOverride_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myFX.Override = txtOverride.Text;
            UpdateFXText();
        }

        void txtPPM_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtPPM.Text = Convert.ToString(myFX.ProcsPerMinute);
        }

        void txtPPM_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            float ppm = (float)Conversion.Val(txtPPM.Text);
            if (ppm >= 0.0 & ppm < 2147483904.0)
                myFX.ProcsPerMinute = ppm;
        }

        public void UpdateEffectSubAttribList()
        {
            int index1 = 0;
            lvSubAttribute.BeginUpdate();
            lvSubAttribute.Items.Clear();
            string[] strArray = new string[0];
            IEffect fx = myFX;
            if (fx.EffectType == Enums.eEffectType.Damage | fx.EffectType == Enums.eEffectType.DamageBuff | fx.EffectType == Enums.eEffectType.Defense | fx.EffectType == Enums.eEffectType.Resistance | fx.EffectType == Enums.eEffectType.Elusivity)
            {
                strArray = Enum.GetNames(fx.DamageType.GetType());
                index1 = (int)fx.DamageType;
                lvSubAttribute.Columns[0].Text = "Damage Type / Vector";
            }
            else if (fx.EffectType == Enums.eEffectType.Mez | fx.EffectType == Enums.eEffectType.MezResist)
            {
                strArray = Enum.GetNames(fx.MezType.GetType());
                index1 = (int)fx.MezType;
                lvSubAttribute.Columns[0].Text = "Mez Type";
            }
            else switch (fx.EffectType)
            {
                case Enums.eEffectType.ResEffect:
                    strArray = Enum.GetNames(fx.EffectType.GetType());
                    index1 = (int)fx.ETModifies;
                    lvSubAttribute.Columns[0].Text = "Effect Type";
                    break;
                case Enums.eEffectType.EntCreate:
                {
                    strArray = new string[DatabaseAPI.Database.Entities.Length - 1 + 1];
                    string lower = fx.Summon.ToLower();
                    int num = DatabaseAPI.Database.Entities.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
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
                    string lower = fx.Summon.ToLower();
                    int num = DatabaseAPI.Database.Power.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
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
                    index1 = (int)fx.ETModifies;
                    lvSubAttribute.Columns[0].Text = "Effect Type";
                    break;
                case Enums.eEffectType.GlobalChanceMod:
                {
                    strArray = new string[DatabaseAPI.Database.EffectIds.Count - 1 + 1];
                    string lower = fx.Reward.ToLower();
                    int num = DatabaseAPI.Database.EffectIds.Count - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
                    {
                        strArray[index2] = Convert.ToString(DatabaseAPI.Database.EffectIds[index2]);
                        if (strArray[index2].ToLower() == lower)
                            index1 = index2;
                    }
                    lvSubAttribute.Columns[0].Text = "GlobalChanceMod Flag";
                    break;
                }
            }
            if (strArray.Length > 0)
            {
                int num = strArray.Length - 1;
                for (int index2 = 0; index2 <= num; ++index2)
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

        void UpdateFXText()
        {
            if (Loading)
                return;
            lblEffectDescription.Text = myFX.BuildEffectString();
        }

        public void UpdateSubSubList()
        {
            int index1 = 0;
            lvSubSub.BeginUpdate();
            lvSubSub.Items.Clear();
            string[] strArray = new string[0];
            IEffect fx = myFX;
            if ((fx.EffectType == Enums.eEffectType.Enhancement | fx.EffectType == Enums.eEffectType.ResEffect) & fx.ETModifies == Enums.eEffectType.Mez)
            {
                lvSubSub.Columns[0].Text = "Mez Type";
                strArray = Enum.GetNames(fx.MezType.GetType());
                index1 = (int)fx.MezType;
            }
            if (strArray.Length > 0)
            {
                int num = strArray.Length - 1;
                for (int index2 = 0; index2 <= num; ++index2)
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
    }
}