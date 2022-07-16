using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerEffect : Form
    {
        private bool _loading;

        public IEffect MyFx;
        private IPower? MyPower { get; set; }
        private readonly int _effectIndex;

        public frmPowerEffect(ICloneable iFx, IPower? fxPower, int fxIndex = 0)
        {
            _loading = true;
            MyPower = fxPower;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            Icon = Resources.reborn;
            if (iFx != null) MyFx = (IEffect)iFx.Clone();
            _effectIndex = fxIndex;
        }

        public frmPowerEffect(ICloneable iFx, int fxIndex = 0)
        {
            _loading = true;
            InitializeComponent();
            Load += frmPowerEffect_Load;
            Icon = Resources.reborn;
            if (iFx != null) MyFx = (IEffect)iFx.Clone();
            _effectIndex = fxIndex;
        }

        private void frmPowerEffect_Load(object sender, EventArgs e)
        {
            FillComboBoxes();
            DisplayEffectData();
            if (MyFx.GetPower() is { } power)
            {
                Text = $@"Edit Effect {_effectIndex} for: {power.FullName}";
            }
            else if (MyFx.Enhancement != null)
            {
                Text = $@"Edit Effect for: {MyFx.Enhancement.UID}";
            }
            else
            {
                Text = @"Edit Effect";
            }

            InitSelectedItems();
            _loading = false;
            UpdateFxText();
            if (MyFx.EffectType == Enums.eEffectType.ModifyAttrib)
            {
                tableLayoutPanel1.Enabled = false;
                tpPowerAttribs.Visible = true;
            }
            else
            {
                tableLayoutPanel1.Enabled = true;
                tpPowerAttribs.Visible = false;
            }

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
                UpdateFxText();
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
            if (MyFx.EffectType == Enums.eEffectType.ModifyAttrib)
            {
                tableLayoutPanel1.Enabled = false;
                tpPowerAttribs.Visible = true;
            }
            else
            {
                tableLayoutPanel1.Enabled = true;
                tpPowerAttribs.Visible = false;
            }
        }

        private void btnEditConditions_Click(object sender, EventArgs e)
        {
            var editConditions = new frmEffectConditionals(MyFx.ActiveConditionals);
            var result = editConditions.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                MyFx.ActiveConditionals = editConditions.Conditionals;
            }
            editConditions.Dispose();
        }

        private void btnExprBuilder_Click(object sender, EventArgs e)
        {
            var expression = new frmExpressionBuilder(MyFx);
            var result = expression.ShowDialog(this);
            if (result != DialogResult.OK) return;
            MyFx.Expressions ??= new Expressions();
            MyFx.Expressions.Duration = expression.Duration ?? "";
            MyFx.Expressions.Magnitude = expression.Magnitude ?? "";
            MyFx.Expressions.Probability = expression.Probability ?? "";
            UpdateFxText();
        }

        private void cbAffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || cbAffects.SelectedIndex < 0)
            {
                return;
            }

            MyFx.ToWho = (Enums.eToWho)cbAffects.SelectedIndex;
            lblAffectsCaster.Text = "";
            var power = MyFx.GetPower();
            if (power != null && (power.EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
            {
                lblAffectsCaster.Text = @"Power also affects Self";
            }

            UpdateFxText();
        }

        private void cbAspect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || cbAspect.SelectedIndex < 0)
                return;
            MyFx.Aspect = (Enums.eAspect)cbAspect.SelectedIndex;
            UpdateFxText();
        }

        private void cbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || cbAttribute.SelectedIndex < 0)
                return;
            MyFx.AttribType = (Enums.eAttribType)cbAttribute.SelectedIndex;
            btnExprBuilder.Enabled = MyFx.AttribType == Enums.eAttribType.Expression;

            UpdateFxText();
        }

        private void cbFXClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.EffectClass = (Enums.eEffectClass)cbFXClass.SelectedIndex;
            UpdateFxText();
        }

        private void cbModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || cbModifier.SelectedIndex < 0)
                return;
            MyFx.ModifierTable = cbModifier.Text;
            MyFx.nModifierTable = DatabaseAPI.NidFromUidAttribMod(MyFx.ModifierTable);
            UpdateFxText();
        }

        private void cbPercentageOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || cbPercentageOverride.SelectedIndex < 0)
                return;
            MyFx.DisplayPercentageOverride = (Enums.eOverrideBoolean)cbPercentageOverride.SelectedIndex;
            UpdateFxText();
        }

        private void chkFXBuffable_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.Buffable = !chkFXBuffable.Checked;
            UpdateFxText();
        }

        private void chkFxNoStack_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.Stacking = !chkStack.Checked ? Enums.eStacking.No : Enums.eStacking.Yes;
            UpdateFxText();
        }

        private void chkFXResistible_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.Resistible = !chkFXResistable.Checked;
            UpdateFxText();
        }

        private void chkNearGround_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.NearGround = chkNearGround.Checked;
            UpdateFxText();
        }

        private void chkCancelOnMiss_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.CancelOnMiss = chkCancelOnMiss.Checked;
            UpdateFxText();
        }

        private void chkVariable_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.VariableModifiedOverride = chkVariable.Checked;
            UpdateFxText();
        }

        private void chkIgnoreScale_CheckChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.IgnoreScaling = chkIgnoreScale.Checked;
            UpdateFxText();
        }

        private void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreSuppression();
            UpdateFxText();
        }

        private void cmbEffectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;

            if (cmbEffectId.SelectedIndex < 0)
                return;

            MyFx.EffectId = cmbEffectId.Items[cmbEffectId.SelectedIndex].ToString();
            UpdateFxText();
        }

        private void cbFXSpecialCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            if (MyFx.ActiveConditionals.Count > 0)
            {
                MessageBox.Show(@"You cannot use Special Cases when using Conditionals.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MyFx.SpecialCase = Enums.eSpecialCase.None;
            }
            else switch (cbFXSpecialCase.SelectedIndex)
            {
                case > 0 when MyFx.ActiveConditionals.Count == 0:
                    MyFx.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                    btnEditConditions.Enabled = false;
                    break;
                case 0 when MyFx.ActiveConditionals.Count == 0:
                    MyFx.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                    btnEditConditions.Enabled = true;
                    break;
            }

            UpdateFxText();
        }

        private void DisplayEffectData()
        {
            cbPercentageOverride.SelectedIndex = (int)MyFx.DisplayPercentageOverride;
            txtFXScale.Text = $@"{MyFx.Scale:####0.0##}";
            txtFXDuration.Text = $@"{MyFx.nDuration:####0.0##}";
            txtFXMag.Text = $@"{MyFx.nMagnitude:####0.0##}";
            txtFXTicks.Text = $@"{MyFx.Ticks:####0}";
            txtOverride.Text = MyFx.Override;
            txtFXDelay.Text = $@"{MyFx.DelayedTime:####0.0##}";
            txtFXProb.Text = $@"{MyFx.BaseProbability:####0.0##}";
            txtPPM.Text = $@"{MyFx.ProcsPerMinute:####0.0##}";
            cbAttribute.SelectedIndex = (int)MyFx.AttribType;
            btnExprBuilder.Enabled = MyFx.AttribType == Enums.eAttribType.Expression;
            cbAspect.SelectedIndex = (int)MyFx.Aspect;
            cbModifier.SelectedIndex = DatabaseAPI.NidFromUidAttribMod(MyFx.ModifierTable);
            lblAffectsCaster.Text = "";
            if (MyFx.ToWho == Enums.eToWho.All)
            {
                cbAffects.SelectedIndex = 1;
            }
            else
            {
                cbAffects.SelectedIndex = (int)MyFx.ToWho;
            }

            var nbFxId = cmbEffectId.Items.Count;
            for (var i = 0; i < nbFxId; i++)
            {
                if (cmbEffectId.Items[i].ToString() != MyFx.EffectId) continue;

                cmbEffectId.SelectedIndex = i;
                break;
            }

            var power = MyFx.GetPower();
            FillPowerAttribs();
            if (power != null && (power.EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
            {
                lblAffectsCaster.Text = @"Power also affects Self";
            }

            cbTarget.SelectedIndex = (int)MyFx.PvMode;
            chkStack.Checked = MyFx.Stacking == Enums.eStacking.Yes;
            chkFXBuffable.Checked = !MyFx.Buffable;
            chkFXResistable.Checked = !MyFx.Resistible;
            chkNearGround.Checked = MyFx.NearGround;
            chkCancelOnMiss.Checked = MyFx.CancelOnMiss;
            chkRqToHitCheck.Checked = MyFx.RequiresToHitCheck;
            IgnoreED.Checked = MyFx.IgnoreED;
            cbFXSpecialCase.SelectedIndex = (int)MyFx.SpecialCase;
            if (MyFx.SpecialCase != Enums.eSpecialCase.None)
            {
                btnEditConditions.Enabled = false;
                if (MyFx.ActiveConditionals.Any())
                {
                    MyFx.ActiveConditionals.Clear();
                }
            }
            else
            {
                btnEditConditions.Enabled = true;
            }

            cbFXClass.SelectedIndex = (int)MyFx.EffectClass;
            chkVariable.Checked = MyFx.VariableModifiedOverride;
            chkIgnoreScale.Checked = MyFx.IgnoreScaling;
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            var names1 = Enum.GetNames(MyFx.Suppression.GetType());
            var values = (int[])Enum.GetValues(MyFx.Suppression.GetType());
            var num1 = names1.Length;
            for (var index = 0; index < num1; index++)
            {
                clbSuppression.Items.Add(names1[index], (MyFx.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            }

            clbSuppression.EndUpdate();
            lvEffectType.BeginUpdate();
            lvEffectType.Items.Clear();
            var index1 = -1;
            var names2 = Enum.GetNames(MyFx.EffectType.GetType());
            var num2 = names2.Length - (MyPower == null ? 1 : 0);
            for (var index2 = 0; index2 < num2; index2++)
            {
                lvEffectType.Items.Add(names2[index2]);
                if ((Enums.eEffectType)index2 == MyFx.EffectType)
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

            cbFXClass.DataSource = Enum.GetValues(typeof(Enums.eEffectClass));
            //cbFXClass.Items.AddRange(Enum.GetNames(MyFx.EffectClass.GetType()));
            cbFXSpecialCase.DataSource = Enum.GetValues(typeof(Enums.eSpecialCase));
            //cbFXSpecialCase.Items.AddRange(Enum.GetNames(MyFx.SpecialCase.GetType()));

            cbPercentageOverride.Items.Add("Auto");
            cbPercentageOverride.Items.Add("Yes");
            cbPercentageOverride.Items.Add("No");

            cbAttribute.DataSource = Enum.GetValues(typeof(Enums.eAttribType));
            //cbAttribute.Items.AddRange(Enum.GetNames(MyFx.AttribType.GetType()));

            cbAspect.DataSource = Enum.GetValues(typeof(Enums.eAspect));
            //cbAspect.Items.AddRange(Enum.GetNames(MyFx.Aspect.GetType()));

            var num1 = DatabaseAPI.Database.AttribMods.Modifier.Count;
            for (var index = 0; index < num1; index++)
            {
                cbModifier.Items.Add(DatabaseAPI.Database.AttribMods.Modifier[index].ID);
            }

            cbAffects.Items.Add("None");
            cbAffects.Items.Add("Target");
            cbAffects.Items.Add("Self");
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
            lvSubAttribute.Enabled = true;
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
            if (MyFx.EffectType == Enums.eEffectType.None) return;
            SelectItemByName(lvEffectType, MyFx.EffectType.ToString());
            switch (MyFx.EffectType)
            {
                case Enums.eEffectType.ModifyAttrib:
                    SelectItemByName(lvSubAttribute, MyFx.PowerAttribs.ToString());
                    break;
                case Enums.eEffectType.EntCreate:
                    SelectItemByName(lvSubAttribute, MyFx.Summon);
                    break;
                default:
                {
                    if (MyFx.EffectType == Enums.eEffectType.Mez | MyFx.EffectType == Enums.eEffectType.MezResist)
                    {
                        SelectItemByName(lvSubAttribute, MyFx.MezType.ToString());
                    }
                    else if (MyFx.EffectType == Enums.eEffectType.Damage
                             | MyFx.EffectType == Enums.eEffectType.DamageBuff
                             | MyFx.EffectType == Enums.eEffectType.Defense
                             | MyFx.EffectType == Enums.eEffectType.Resistance
                             | MyFx.EffectType == Enums.eEffectType.Elusivity)
                    {
                        SelectItemByName(lvSubAttribute, MyFx.DamageType.ToString());
                    }
                    else switch (MyFx.EffectType)
                    {
                        case Enums.eEffectType.Enhancement:
                            SelectItemByName(lvSubAttribute, MyFx.ETModifies.ToString());
                            break;
                        case Enums.eEffectType.PowerRedirect:
                        {
                            var group = MyFx.Override.Split('.');
                            SelectItemByName(lvSubAttribute, @group[0]);
                            UpdateSubSubList();
                            SelectItemByName(lvSubSub, MyFx.Override);
                            break;
                        }
                    }

                    break;
                }
            }

            if ((MyFx.EffectType == Enums.eEffectType.Enhancement | MyFx.EffectType == Enums.eEffectType.ResEffect) & MyFx.ETModifies == Enums.eEffectType.Mez)
            {
                SelectItemByName(lvSubSub, MyFx.MezType.ToString());
            }
            else if (MyFx.EffectType == Enums.eEffectType.Enhancement & (MyFx.ETModifies == Enums.eEffectType.Defense | MyFx.ETModifies == Enums.eEffectType.Damage))
            {
                SelectItemByName(lvSubSub, MyFx.DamageType.ToString());
            }
        }

        //change storing of Atr attributes from power to effect
        private void FillPowerAttribs()
        {
            // look at possibly using class to set modified and original values
            if (MyPower != null)
            {
                var power = MyPower;

                if (Math.Abs(MyFx.AtrOrigAccuracy - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigAccuracy = power.Accuracy;
                    MyFx.AtrModAccuracy = power.Accuracy;
                }

                if (Math.Abs(MyFx.AtrOrigActivatePeriod - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigActivatePeriod = power.ActivatePeriod;
                    MyFx.AtrModActivatePeriod = power.ActivatePeriod;
                }

                if (MyFx.AtrOrigArc == -1)
                {
                    MyFx.AtrOrigArc = power.Arc;
                    MyFx.AtrModArc = power.Arc;
                }

                if (Math.Abs(MyFx.AtrOrigCastTime - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigCastTime = power.CastTime;
                    MyFx.AtrModCastTime = power.CastTime;
                }

                if (MyFx.AtrOrigEffectArea == Enums.eEffectArea.None)
                {
                    MyFx.AtrOrigEffectArea = power.EffectArea;
                    MyFx.AtrModEffectArea = power.EffectArea;
                }

                if (Math.Abs(MyFx.AtrOrigEnduranceCost - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigEnduranceCost = power.EndCost;
                    MyFx.AtrModEnduranceCost = power.EndCost;
                }

                if (Math.Abs(MyFx.AtrOrigInterruptTime - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigInterruptTime = power.InterruptTime;
                    MyFx.AtrModInterruptTime = power.InterruptTime;
                }

                if (MyFx.AtrOrigMaxTargets == -1)
                {
                    MyFx.AtrOrigMaxTargets = power.MaxTargets;
                    MyFx.AtrModMaxTargets = power.MaxTargets;
                }

                if (Math.Abs(MyFx.AtrOrigRadius - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigRadius = power.Radius;
                    MyFx.AtrModRadius = power.Radius;
                }

                if (Math.Abs(MyFx.AtrOrigRange - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigRange = power.Range;
                    MyFx.AtrModRange = power.Range;
                }

                if (Math.Abs(MyFx.AtrOrigRechargeTime - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigRechargeTime = power.RechargeTime;
                    MyFx.AtrModRechargeTime = power.RechargeTime;
                }

                if (Math.Abs(MyFx.AtrOrigSecondaryRange - (-1)) < float.Epsilon)
                {
                    MyFx.AtrOrigSecondaryRange = power.RangeSecondary;
                    MyFx.AtrModSecondaryRange = power.RangeSecondary;
                }
            }

            txtFXAccuracy.Text = MyFx.AtrModAccuracy.ToString(CultureInfo.InvariantCulture);
            txtFXActivateInterval.Text = MyFx.AtrModActivatePeriod.ToString(CultureInfo.InvariantCulture);
            txtFXArc.Text = MyFx.AtrModArc.ToString();
            txtFXCastTime.Text = MyFx.AtrModCastTime.ToString(CultureInfo.InvariantCulture);
            cbFXEffectArea.BeginUpdate();
            cbFXEffectArea.DataSource = Enum.GetValues(typeof(Enums.eEffectArea));
            cbFXEffectArea.SelectedIndex = (int)MyFx.AtrModEffectArea;
            cbFXEffectArea.EndUpdate();
            txtFXEnduranceCost.Text = MyFx.AtrModEnduranceCost.ToString(CultureInfo.InvariantCulture);
            txtFXInterruptTime.Text = MyFx.AtrModInterruptTime.ToString(CultureInfo.InvariantCulture);
            txtFXMaxTargets.Text = MyFx.AtrModMaxTargets.ToString();
            txtFXRadius.Text = MyFx.AtrModRadius.ToString(CultureInfo.InvariantCulture);
            txtFXRange.Text = MyFx.AtrModRange.ToString(CultureInfo.InvariantCulture);
            txtFXRechargeTime.Text = MyFx.AtrModRechargeTime.ToString(CultureInfo.InvariantCulture);
            txtFXSecondaryRange.Text = MyFx.AtrModSecondaryRange.ToString(CultureInfo.InvariantCulture);
        }

        private void FullCopy()
        {
            var data = Serializer.GetSerializer().Serialize(MyFx);
            Clipboard.SetData(@"MidsEffectData", data);
        }

        private void FullPaste()
        {
            if (!Clipboard.ContainsData(@"MidsEffectData"))
            {
                MessageBox.Show(@"The clipboard does not contain any effect data.", @"Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var effectData = Serializer.GetSerializer().Deserialize<Effect>((string)Clipboard.GetData(@"MidsEffectData"));
            MyFx = effectData;
            DisplayEffectData();
        }

        private void IgnoreED_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.IgnoreED = IgnoreED.Checked;
            UpdateFxText();
        }

        private void lvEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || lvEffectType.SelectedIndices.Count < 1)
                return;
            MyFx.EffectType = (Enums.eEffectType)lvEffectType.SelectedIndices[0];
            if (MyFx.EffectType == Enums.eEffectType.ModifyAttrib)
            {
                tableLayoutPanel1.Enabled = false;
                tpPowerAttribs.Visible = true;
            }
            else
            {
                tableLayoutPanel1.Enabled = true;
                tpPowerAttribs.Visible = false;
            }

            UpdateEffectSubAttribList();
            UpdateFxText();
        }

        private void lvSubAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || lvSubAttribute.SelectedIndices.Count < 1)
            {
                return;
            }

            var sIndex = lvSubAttribute.SelectedIndices[0];
            var sText = lvSubAttribute.SelectedItems[0].Text;

            if ((MyFx.EffectType == Enums.eEffectType.Damage)
                | (MyFx.EffectType == Enums.eEffectType.DamageBuff)
                | (MyFx.EffectType == Enums.eEffectType.Defense)
                | (MyFx.EffectType == Enums.eEffectType.Resistance)
                | (MyFx.EffectType == Enums.eEffectType.Elusivity))
            {
                MyFx.DamageType = (Enums.eDamage)sIndex;
            }
            else if ((MyFx.EffectType == Enums.eEffectType.Mez) | (MyFx.EffectType == Enums.eEffectType.MezResist))
            {
                MyFx.MezType = (Enums.eMez)sIndex;
            }
            else
            {
                switch (MyFx.EffectType)
                {
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                        MyFx.ETModifies = (Enums.eEffectType)sIndex;
                        break;
                    case Enums.eEffectType.EntCreate:
                        MyFx.Summon = sText;
                        break;
                    case Enums.eEffectType.GlobalChanceMod:
                        MyFx.Reward = sText;
                        break;
                    case Enums.eEffectType.GrantPower:
                        MyFx.Summon = sText;
                        break;
                    case Enums.eEffectType.ModifyAttrib:
                        MyFx.PowerAttribs = (Enums.ePowerAttribs)sIndex;
                        var tpControls = tpPowerAttribs.Controls;
                        for (var rowIndex = 0; rowIndex < tpControls.Count; rowIndex++)
                        {
                            tpControls[rowIndex].Enabled = tpControls[rowIndex].Name.Contains(sText);
                        }

                        //cbTarget.Enabled = true;
                        break;
                }
            }

            UpdateFxText();
            UpdateSubSubList();
        }

        private void lvSubSub_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || lvSubSub.SelectedIndices.Count < 1)
                return;
            var fx = MyFx;
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

            UpdateFxText();
        }

        private void cbTarget_IndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.PvMode = (Enums.ePvX)cbTarget.SelectedIndex;
            UpdateFxText();
        }

        private void StoreSuppression()
        {
            var values = (int[])Enum.GetValues(MyFx.Suppression.GetType());
            MyFx.Suppression = Enums.eSuppress.None;
            var num = clbSuppression.CheckedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
                //this.myFX.Suppression += (Enums.eSuppress) values[this.clbSuppression.CheckedIndices[index]];
                MyFx.Suppression += values[clbSuppression.CheckedIndices[index]];
        }

        private void txtFXDelay_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXDelay.Text = MyFx.DelayedTime.ToString(CultureInfo.InvariantCulture);
            UpdateFxText();
        }

        private void txtFXDelay_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            var fx = MyFx;
            var ret = float.TryParse(txtFXDelay.Text, out var num);
            if (!ret)
                return;
            if ((num >= 0.0) & (num <= 2147483904.0))
                fx.DelayedTime = num;
            UpdateFxText();
        }

        private void txtFXDuration_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXDuration.Text = $"{MyFx.nDuration:##0.0##}";
            UpdateFxText();
        }

        private void txtFXDuration_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            var fx = MyFx;
            var ret = float.TryParse(txtFXDuration.Text, out var num);
            if (!ret)
                return;
            if ((num >= 0.0) & (num <= 2147483904.0))
                fx.nDuration = num;
            UpdateFxText();
        }

        private void txtFXMag_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXMag.Text = MyFx.nMagnitude.ToString(CultureInfo.InvariantCulture);
            UpdateFxText();
        }

        private void txtFXMag_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            var inputStr = txtFXMag.Text;
            if (inputStr.EndsWith("%", StringComparison.InvariantCulture))
                inputStr = inputStr.Substring(0, inputStr.Length - 1);
            var ret = float.TryParse(inputStr, out var num);
            if (!ret)
                return;
            if ((num >= -2147483904.0) & (num <= 2147483904.0))
                MyFx.nMagnitude = num;
            UpdateFxText();
        }

        private void txtFXProb_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            var ret = float.TryParse(txtFXProb.Text, out var num);
            if (!ret)
                return;
            if (num >= 0.0 & num <= 2147483904.0)
            {
                if (num > 1.0)
                    num /= 100f;
                MyFx.BaseProbability = num;
                //lblProb.Text = $"({fx.BaseProbability * 100:###0}%)";
            }

            UpdateFxText();
        }

        private void txtFXProb_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXProb.Text = MyFx.BaseProbability.ToString(CultureInfo.InvariantCulture);
            UpdateFxText();
        }

        private void txtFXScale_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXScale.Text = $"{MyFx.Scale:####0.0##}";
            UpdateFxText();
        }

        private void txtFXScale_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            var fxScaleRaw = txtFXScale.Text;
            if (fxScaleRaw.EndsWith("%", StringComparison.InvariantCulture))
                fxScaleRaw = fxScaleRaw.Substring(0, fxScaleRaw.Length - 1);
            var ret = float.TryParse(fxScaleRaw, out var fxScale);
            if (!ret)
                return;
            //var fxScale = (float)Conversion.Val(fxScaleRaw);
            if ((fxScale >= -2147483904.0) & (fxScale <= 2147483904.0))
                MyFx.Scale = fxScale;
            UpdateFxText();
        }

        private void txtFXTicks_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtFXTicks.Text = Convert.ToString(MyFx.Ticks);
            UpdateFxText();
        }

        private void txtFXTicks_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            //var fxTicks = (float)Conversion.Val(txtFXTicks.Text);
            var ret = float.TryParse(txtFXTicks.Text, out var fxTicks);
            if ((fxTicks >= 0.0) & (fxTicks <= 2147483904.0))
                MyFx.Ticks = (int)Math.Round(fxTicks);
            UpdateFxText();
        }

        private void txtOverride_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.Override = txtOverride.Text;
            UpdateFxText();
        }

        private void txtPPM_Leave(object sender, EventArgs e)
        {
            if (_loading)
                return;
            txtPPM.Text = MyFx.ProcsPerMinute.ToString(CultureInfo.InvariantCulture);
        }

        private void txtPPM_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            //var ppm = (float)Conversion.Val(txtPPM.Text);
            var ret = float.TryParse(txtPPM.Text, out var ppm);
            if ((ppm >= 0.0) & (ppm < 2147483904.0))
                MyFx.ProcsPerMinute = ppm;
        }

        private void txtFXAccuracy_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXAccuracy.Text, out var num);
            if (!ret) return;
            MyFx.AtrModAccuracy = num;
            UpdateFxText();
        }

        private void txtFXActivateInterval_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXActivateInterval.Text, out var num);
            if (!ret) return;
            MyFx.AtrModActivatePeriod = num;
            UpdateFxText();
        }

        private void txtFXArc_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = int.TryParse(txtFXArc.Text, out var num);
            if (!ret) return;
            MyFx.AtrModArc = num;
            UpdateFxText();
        }

        private void txtFXCastTime_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXCastTime.Text, out var num);
            if (!ret) return;
            MyFx.AtrModCastTime = num;
            UpdateFxText();
        }

        private void cbFXEffectArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            MyFx.AtrModEffectArea = (Enums.eEffectArea)cbFXEffectArea.SelectedIndex;
            UpdateFxText();
        }

        private void txtFXEnduranceCost_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXEnduranceCost.Text, out var num);
            if (!ret) return;
            MyFx.AtrModEnduranceCost = num;
            UpdateFxText();
        }

        private void txtFXInterruptTime_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXInterruptTime.Text, out var num);
            if (!ret) return;
            MyFx.AtrModInterruptTime = num;
            UpdateFxText();
        }

        private void txtFXMaxTargets_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = int.TryParse(txtFXMaxTargets.Text, out var num);
            if (!ret) return;
            MyFx.AtrModMaxTargets = num;
            UpdateFxText();
        }

        private void txtFXRadius_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXRadius.Text, out var num);
            if (!ret) return;
            MyFx.AtrModRadius = num;
            UpdateFxText();
        }

        private void txtFXRange_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXRange.Text, out var num);
            if (!ret) return;
            MyFx.AtrModRange = num;
            UpdateFxText();
        }

        private void txtFXRechargeTime_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXRechargeTime.Text, out var num);
            if (!ret) return;
            MyFx.AtrModRechargeTime = num;
            UpdateFxText();
        }

        private void txtFXSecondaryRange_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            var ret = float.TryParse(txtFXSecondaryRange.Text, out var num);
            if (!ret) return;
            MyFx.AtrModSecondaryRange = num;
            UpdateFxText();
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
            var lvControl = (ctlListViewColored)sender;
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

        private void UpdateEffectSubAttribList()
        {
            var index1 = 0;
            lvSubAttribute.BeginUpdate();
            lvSubAttribute.Items.Clear();
            var strArray = Array.Empty<string>();
            var fx = MyFx;
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
                            strArray = new string[DatabaseAPI.Database.Entities.Length];
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
                            strArray = new string[DatabaseAPI.Database.Power.Length];
                            var lower = fx.Summon.ToLower();
                            var num = DatabaseAPI.Database.Power.Length;
                            for (var index2 = 0; index2 < num; index2++)
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
                            strArray = new string[DatabaseAPI.Database.EffectIds.Count];
                            var lower = fx.Reward.ToLower();
                            var num = DatabaseAPI.Database.EffectIds.Count;
                            for (var index2 = 0; index2 < num; index2++)
                            {
                                strArray[index2] = Convert.ToString(DatabaseAPI.Database.EffectIds[index2]);
                                if (strArray[index2].ToLower() == lower)
                                    index1 = index2;
                            }

                            lvSubAttribute.Columns[0].Text = @"GlobalChanceMod Flag";
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

        /*private void UpdateFxText(string? senderName = "")
        {
            if (_loading)
                return;
            if (!string.IsNullOrWhiteSpace(senderName))
            {
                if (IsValidExpression)
                {
                    lblEffectDescription.ForeColor = SystemColors.ControlText;
                    lblEffectDescription.Text = MyFx.BuildEffectString(false, string.Empty, false, false, false, false, true);
                }
                else
                {
                    lblEffectDescription.ForeColor = Color.Red;
                }
            }
            else
            {
                lblEffectDescription.Text = MyFx.BuildEffectString(Simple: false, string.Empty, noMag: false, Grouped: false, useBaseProbability: false, fromPopup: false, editorDisplay: true);
            }
        }*/

        private void UpdateFxText(string? senderName = "")
        {
            if (_loading)
                return;
            
            if (!string.IsNullOrWhiteSpace(senderName))
            {
                var errorString = string.Empty;
                Expressions.Validate(MyFx, out var validationItems);
                if (validationItems.All(x => x.Validated))
                {
                    lblEffectDescription.ForeColor = SystemColors.ControlText;
                    lblEffectDescription.Text = MyFx.BuildEffectString(false, string.Empty, false, false, false, false, true);
                }
                else
                {
                    foreach (var validationItem in validationItems.Where(validationItem => !validationItem.Validated))
                    {
                        if (string.IsNullOrWhiteSpace(errorString))
                        {
                            errorString = $"{validationItem.Message}";
                        }
                        else
                        {
                            errorString += $"\r\n{validationItem.Message}";
                        }
                    }
                    lblEffectDescription.ForeColor = Color.Red;
                    lblEffectDescription.Text = errorString;
                }
            }
            else
            {
                lblEffectDescription.Text = MyFx.BuildEffectString(Simple: false, string.Empty, noMag: false, Grouped: false, useBaseProbability: false, fromPopup: false, editorDisplay: true);
            }
        }

        /*private bool IsValidExpression => ValidExpression();
        private bool ValidExpression()
        {
            var returnedBool = true;
            var returnData = string.Empty;

            if (ExpressionParser.HasSeparator(MyFx))
            {
                ExpressionParser.ParseExpression(MyFx, out var data);
                if (data.ErrorFound)
                {
                    returnData = $@"Expression Error: {data.ErrorString}";
                    returnedBool = false;
                }
            }
            else
            {
                ExpressionParser.ParseExpression(MyFx, out var data, 1);
                if (data.ErrorFound)
                {
                    returnData = $@"Expression Error: {data.ErrorString}";
                    returnedBool = false;
                }
            }

            lblEffectDescription.Text = returnData;

            return returnedBool;
        }*/

        private void UpdateSubSubList()
        {
            var index1 = 0;
            lvSubSub.BeginUpdate();
            lvSubSub.Items.Clear();
            var strArray = Array.Empty<string>();
            if (((MyFx.EffectType == Enums.eEffectType.Enhancement) | (MyFx.EffectType == Enums.eEffectType.ResEffect)) & (MyFx.ETModifies == Enums.eEffectType.Mez))
            {
                lvSubSub.Columns[0].Text = "Mez Type";
                lvSubSub.Columns[0].Width = -2;
                strArray = Enum.GetNames(MyFx.MezType.GetType());
                index1 = (int)MyFx.MezType;
            }

            if (MyFx.EffectType == Enums.eEffectType.Enhancement && (MyFx.ETModifies == Enums.eEffectType.Defense) | (MyFx.ETModifies == Enums.eEffectType.Damage))
            {
                lvSubSub.Columns[0].Text = @"Damage Type";
                lvSubSub.Columns[0].Width = -2;
                strArray = Enum.GetNames(MyFx.DamageType.GetType());
                index1 = (int)MyFx.DamageType;
            }

            if (MyFx.EffectType == Enums.eEffectType.PowerRedirect)
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

        private void cbCoDFormat_CheckedChanged(object sender, EventArgs e)
        {
            MidsContext.Config.CoDEffectFormat = cbCoDFormat.Checked;
            UpdateFxText();
        }

        private void chkRqToHitCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MyFx.RequiresToHitCheck = chkRqToHitCheck.Checked;
            UpdateFxText();
        }
    }
}
