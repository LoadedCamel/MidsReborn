using System.Diagnostics;
using System.Globalization;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.UI.Controls;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerEffect : Form
    {
        private bool _loading;

        public IEffect MyFx;
        private IPower? MyPower { get; set; }
        private readonly int _effectIndex;
        private readonly PowerModifierDisplayContextResult? _modifierContext;
        private readonly string _modifierContextClassName = string.Empty;
        private Label? modifierContextLabel;

        public frmPowerEffect(ICloneable iFx, IPower? fxPower, int fxIndex = 0)
            : this(iFx, fxPower, fxIndex, null)
        {
        }

        internal frmPowerEffect(ICloneable iFx, IPower? fxPower, int fxIndex, PowerModifierDisplayContextResult? modifierContext)
        {
            _loading = true;
            MyPower = fxPower;
            _modifierContext = modifierContext;
            _modifierContextClassName = modifierContext?.SelectedClassName ?? string.Empty;
            InitializeComponent();
            InitializeModifierContextLabel();
            Load += frmPowerEffect_Load;
            Icon = Resources.MRB_Icon_Concept;
            if (iFx != null)
            {
                MyFx = (IEffect)iFx.Clone();
            }

            _effectIndex = fxIndex;
        }

        public frmPowerEffect(ICloneable iFx, int fxIndex = 0)
        {
            _loading = true;
            InitializeComponent();
            InitializeModifierContextLabel();
            Load += frmPowerEffect_Load;
            Icon = Resources.MRB_Icon_Concept;
            if (iFx != null)
            {
                MyFx = (IEffect)iFx.Clone();
            }

            _effectIndex = fxIndex;
        }

        private void InitializeModifierContextLabel()
        {
            if (_modifierContext == null)
            {
                return;
            }

            modifierContextLabel = new Label
            {
                AutoEllipsis = true,
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(1326, 11),
                Padding = new Padding(6, 4, 6, 4),
                Size = new Size(284, 110),
                Text = _modifierContext.DisplayText,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            Controls.Add(modifierContextLabel);
            modifierContextLabel.BringToFront();
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
                Debug.WriteLine(exception);
                throw;
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            FullPaste();
        }

        private void btnEditConditions_Click(object sender, EventArgs e)
        {
            var editConditions = new frmEffectConditionals(MyFx.ActiveConditionals);
            editConditions.AdvancedConditions = MyFx.AdvancedConditions?.Clone() ?? AdvancedConditionSet.FromLegacyActiveConditionals(MyFx.ActiveConditionals);
            var result = editConditions.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                MyFx.ActiveConditionals = editConditions.Conditionals;
                MyFx.AdvancedConditions = editConditions.AdvancedConditions.Clone();
            }
            editConditions.Dispose();
            UpdateFxText();
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

            if (MyFx.AdvancedConditions is { Rows.Count: > 0 } || MyFx.ActiveConditionals.Count > 0)
            {
                MessageBox.Show(@"You cannot use Special Cases when using Conditionals.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MyFx.SpecialCase = Enums.eSpecialCase.None;
            }
            else switch (cbFXSpecialCase.SelectedIndex)
            {
                case > 0 when MyFx.AdvancedConditions is not { Rows.Count: > 0 } && MyFx.ActiveConditionals.Count == 0:
                    MyFx.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                    btnEditConditions.Enabled = false;
                    break;
                case 0 when MyFx.AdvancedConditions is not { Rows.Count: > 0 } && MyFx.ActiveConditionals.Count == 0:
                    MyFx.SpecialCase = (Enums.eSpecialCase)cbFXSpecialCase.SelectedIndex;
                    btnEditConditions.Enabled = true;
                    break;
            }

            UpdateFxText();
        }

        private void DisplayEffectData()
        {
            cbPercentageOverride.SelectedIndex = (int)MyFx.DisplayPercentageOverride;
            txtFXScale.Text = $@"{MyFx.Scale:####0.####}";
            txtFXDuration.Text = $@"{MyFx.nDuration:####0.####}";
            txtFXMag.Text = $@"{MyFx.nMagnitude:####0.####}";
            txtFXTicks.Text = $@"{MyFx.Ticks:####0}";
            txtOverride.Text = MyFx.EffectType switch
            {
                Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode => MyFx.ModeName,
                Enums.eEffectType.RevokePower => string.IsNullOrWhiteSpace(MyFx.RevokedPower) ? MyFx.Override : MyFx.RevokedPower,
                _ => MyFx.Override
            };
            txtFXDelay.Text = $@"{MyFx.DelayedTime:####0.####}";
            txtFXProb.Text = $@"{MyFx.BaseProbability:####0.####}";
            txtPPM.Text = $@"{MyFx.ProcsPerMinute:####0.####}";
            cbAttribute.SelectedIndex = (int)MyFx.AttribType;
            btnExprBuilder.Enabled = MyFx.AttribType == Enums.eAttribType.Expression;
            cbAspect.SelectedIndex = (int)MyFx.Aspect;
            SelectModifierTable(MyFx.ModifierTable);
            lblAffectsCaster.Text = "";
            cbAffects.SelectedIndex = MyFx.ToWho == Enums.eToWho.All ? 1 : (int)MyFx.ToWho;

            for (var i = 0; i < cmbEffectId.Items.Count; i++)
            {
                if (cmbEffectId.Items[i].ToString() != MyFx.EffectId) continue;

                cmbEffectId.SelectedIndex = i;
                break;
            }

            var power = MyFx.GetPower();
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

                MyFx.AdvancedConditions.Rows.Clear();
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
            var names1 = Enum.GetNames<Enums.eSuppress>();
            var values = Enum.GetValues<Enums.eSuppress>().Cast<int>().ToArray();
            for (var index = 0; index < names1.Length; index++)
            {
                clbSuppression.Items.Add(names1[index], (MyFx.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            }

            clbSuppression.EndUpdate();
            lvEffectType.BeginUpdate();
            lvEffectType.Items.Clear();
            var index1 = -1;
            var names2 = Enum.GetNames<Enums.eEffectType>();
            for (var index2 = 0; index2 < names2.Length - (MyPower == null ? 1 : 0); index2++)
            {
                lvEffectType.Items.Add(names2[index2]);
                if ((Enums.eEffectType)index2 == MyFx.EffectType)
                {
                    index1 = index2;
                }
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

            foreach (var tableName in DatabaseAPI.GetModifierTableNames())
            {
                cbModifier.Items.Add(tableName);
            }

            cbAffects.Items.Add("None");
            cbAffects.Items.Add("Target");
            cbAffects.Items.Add("Self");
            cbAffects.Items.Add("MyPet");
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

        private void SelectModifierTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                cbModifier.SelectedIndex = cbModifier.Items.Count > 0 ? 0 : -1;
                return;
            }

            var index = cbModifier.FindStringExact(tableName);
            if (index < 0)
            {
                cbModifier.Items.Add(tableName);
                index = cbModifier.Items.Count - 1;
            }

            cbModifier.SelectedIndex = index;
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
                case Enums.eEffectType.EntCreate:
                    SelectItemByName(lvSubAttribute, MyFx.Summon);
                    break;

                default:
                {
                    switch (MyFx.EffectType)
                    {
                        case Enums.eEffectType.Mez:
                        case Enums.eEffectType.MezResist:
                            SelectItemByName(lvSubAttribute, MyFx.MezType.ToString());
                            break;

                        case Enums.eEffectType.Damage:
                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Elusivity:
                            SelectItemByName(lvSubAttribute, MyFx.DamageType.ToString());
                            break;

                        case Enums.eEffectType.Enhancement:
                            SelectItemByName(lvSubAttribute, MyFx.ETModifies.ToString());
                            break;
                        
                        case Enums.eEffectType.PowerRedirect:
                            var group = MyFx.Override.Split('.');
                            SelectItemByName(lvSubAttribute, group[0]);
                            UpdateSubSubList();
                            SelectItemByName(lvSubSub, MyFx.Override);
                            break;

                        case Enums.eEffectType.SetMode:
                        case Enums.eEffectType.UnsetMode:
                            SelectItemByName(lvSubAttribute, MyFx.ModeName);
                            break;

                        case Enums.eEffectType.RevokePower:
                            SelectItemByName(lvSubAttribute,
                                string.IsNullOrWhiteSpace(MyFx.RevokedPower) ? MyFx.Override : MyFx.RevokedPower);
                            break;
                    }

                    break;
                }
            }

            if (MyFx.EffectType is Enums.eEffectType.Enhancement or Enums.eEffectType.ResEffect & MyFx.ETModifies == Enums.eEffectType.Mez)
            {
                SelectItemByName(lvSubSub, MyFx.MezType.ToString());
            }
            else if (MyFx.EffectType == Enums.eEffectType.Enhancement & MyFx.ETModifies is Enums.eEffectType.Defense or Enums.eEffectType.Damage)
            {
                SelectItemByName(lvSubSub, MyFx.DamageType.ToString());
            }
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

            MyFx = Serializer.GetSerializer().Deserialize<Effect>((string)Clipboard.GetData(@"MidsEffectData"));

            DisplayEffectData();
        }

        private void IgnoreED_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            MyFx.IgnoreED = IgnoreED.Checked;
            UpdateFxText();
        }

        private void lvEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || lvEffectType.SelectedIndices.Count < 1)
            {
                return;
            }

            MyFx.EffectType = (Enums.eEffectType)lvEffectType.SelectedIndices[0];
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

            switch (MyFx.EffectType)
            {
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    MyFx.DamageType = (Enums.eDamage)sIndex;
                    break;

                case Enums.eEffectType.Mez:
                case Enums.eEffectType.MezResist:
                    MyFx.MezType = (Enums.eMez)sIndex;
                    break;

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
                case Enums.eEffectType.ExecutePower:
                    MyFx.Summon = sText;
                    break;

                case Enums.eEffectType.SetMode:
                case Enums.eEffectType.UnsetMode:
                    MyFx.ModeName = OmniModeMapper.Normalize(sText);
                    MyFx.ModeFlag = OmniModeMapper.TryToFlag(MyFx.ModeName, out var modeFlag)
                        ? modeFlag
                        : Enums.eModeFlags.None;
                    MyFx.ModeId = OmniModeMapper.TryFromModeId(MyFx.ModeId, out var modeName, out _) &&
                                  modeName.Equals(MyFx.ModeName, StringComparison.OrdinalIgnoreCase)
                        ? MyFx.ModeId
                        : -1;
                    txtOverride.Text = MyFx.ModeName;
                    break;

                case Enums.eEffectType.RevokePower:
                    MyFx.RevokedPower = sText;
                    MyFx.Override = sText;
                    txtOverride.Text = sText;
                    break;
            }

            UpdateFxText();
            UpdateSubSubList();
        }

        private void lvSubSub_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || lvSubSub.SelectedIndices.Count < 1)
            {
                return;
            }

            switch (MyFx.EffectType)
            {
                case Enums.eEffectType.Enhancement when MyFx.ETModifies == Enums.eEffectType.Mez:
                case Enums.eEffectType.Enhancement when MyFx.ETModifies == Enums.eEffectType.MezResist:
                case Enums.eEffectType.ResEffect when MyFx.ETModifies == Enums.eEffectType.Mez:
                    MyFx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];

                    break;

                //case Enums.eEffectType.Enhancement when MyFx.ETModifies is Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff or Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement when MyFx.ETModifies is Enums.eEffectType.Damage or Enums.eEffectType.Defense or Enums.eEffectType.Resistance:
                    MyFx.DamageType = (Enums.eDamage)lvSubSub.SelectedIndices[0];

                    break;

                case Enums.eEffectType.PowerRedirect:
                    txtOverride.Text = lvSubSub.SelectedItems[0].Text;

                    break;
            }

            /*if (fx.EffectType == Enums.eEffectType.Enhancement & fx.ETModifies == Enums.eEffectType.Mez)
            {
                fx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];
            }

            if (fx.EffectType == Enums.eEffectType.Enhancement & fx.ETModifies == Enums.eEffectType.Damage | fx.ETModifies == Enums.eEffectType.Defense)
            {
                fx.DamageType = (Enums.eDamage)lvSubSub.SelectedIndices[0];
            }

            if (fx.EffectType == Enums.eEffectType.ResEffect & fx.ETModifies == Enums.eEffectType.Mez)
            {
                fx.MezType = (Enums.eMez)lvSubSub.SelectedIndices[0];
            }

            if (fx.EffectType == Enums.eEffectType.PowerRedirect)
            {
                txtOverride.Text = lvSubSub.SelectedItems[0].Text;
            }*/
            
            UpdateFxText();
        }

        private void cbTarget_IndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            MyFx.PvMode = (Enums.ePvX)cbTarget.SelectedIndex;
            UpdateFxText();
        }

        private void StoreSuppression()
        {
            var values = Enum.GetValues<Enums.eSuppress>().Cast<int>().ToArray();
            MyFx.Suppression = Enums.eSuppress.None;
            for (var index = 0; index < clbSuppression.CheckedIndices.Count; index++)
            {
                //this.myFX.Suppression += (Enums.eSuppress) values[this.clbSuppression.CheckedIndices[index]];
                MyFx.Suppression += values[clbSuppression.CheckedIndices[index]];
            }
        }

        private void txtFXDelay_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXDelay.Text = $@"{MyFx.DelayedTime:####0.####}";
            UpdateFxText();
        }

        private void txtFXDelay_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var fx = MyFx;
            var ret = float.TryParse(txtFXDelay.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and <= 2147483904)
            {
                fx.DelayedTime = num;
            }

            UpdateFxText();
        }

        private void txtFXDuration_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXDuration.Text = $@"{MyFx.nDuration:##0.0##}";
            UpdateFxText();
        }

        private void txtFXDuration_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var fx = MyFx;
            var ret = float.TryParse(txtFXDuration.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and <= 2147483904)
            {
                fx.nDuration = num;
            }

            UpdateFxText();
        }

        private void txtFXMag_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXMag.Text = $@"{MyFx.nMagnitude:####0.####}";
            UpdateFxText();
        }

        private void txtFXMag_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var inputStr = txtFXMag.Text;
            if (inputStr.EndsWith("%", StringComparison.InvariantCulture))
            {
                inputStr = inputStr.Substring(0, inputStr.Length - 1);
            }

            var ret = float.TryParse(inputStr, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= -2147483904 and <= 2147483904)
            {
                MyFx.nMagnitude = num;
            }

            UpdateFxText();
        }

        private void txtFXProb_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var ret = float.TryParse(txtFXProb.Text, out var num);
            if (!ret)
            {
                return;
            }

            if (num is >= 0 and <= 2147483904)
            {
                if (num > 1)
                {
                    num /= 100f;
                }

                MyFx.BaseProbability = num;
                //lblProb.Text = $"({fx.BaseProbability * 100:###0}%)";
            }

            UpdateFxText();
        }

        private void txtFXProb_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXProb.Text = $@"{MyFx.BaseProbability:####0.####}";
            UpdateFxText();
        }

        private void txtFXScale_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXScale.Text = $@"{MyFx.Scale:####0.####}";
            UpdateFxText();
        }

        private void txtFXScale_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            var fxScaleRaw = txtFXScale.Text;
            if (fxScaleRaw.EndsWith("%", StringComparison.InvariantCulture))
            {
                fxScaleRaw = fxScaleRaw[..^1];
            }

            var ret = float.TryParse(fxScaleRaw, out var fxScale);
            if (!ret)
            {
                return;
            }

            //var fxScale = (float)Conversion.Val(fxScaleRaw);
            if (fxScale is >= -2147483904 and <= 2147483904)
            {
                MyFx.Scale = fxScale;
            }

            UpdateFxText();
        }

        private void txtFXTicks_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtFXTicks.Text = $@"{MyFx.Ticks:####0}";
            UpdateFxText();
        }

        private void txtFXTicks_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            //var fxTicks = (float)Conversion.Val(txtFXTicks.Text);
            var ret = float.TryParse(txtFXTicks.Text, out var fxTicks);
            if (fxTicks is >= 0 and <= 2147483904)
            {
                MyFx.Ticks = (int)Math.Round(fxTicks);
            }

            UpdateFxText();
        }

        private void txtOverride_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            if (MyFx.EffectType is Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode)
            {
                MyFx.ModeName = OmniModeMapper.Normalize(txtOverride.Text);
                MyFx.ModeFlag = OmniModeMapper.TryToFlag(MyFx.ModeName, out var modeFlag)
                    ? modeFlag
                    : Enums.eModeFlags.None;
            }
            else if (MyFx.EffectType == Enums.eEffectType.RevokePower)
            {
                MyFx.RevokedPower = txtOverride.Text;
                MyFx.Override = txtOverride.Text;
            }
            else
            {
                MyFx.Override = txtOverride.Text;
            }

            UpdateFxText();
        }

        private void txtPPM_Leave(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            txtPPM.Text = $@"{MyFx.ProcsPerMinute:####0.####}";
        }

        private void txtPPM_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            //var ppm = (float)Conversion.Val(txtPPM.Text);
            var ret = float.TryParse(txtPPM.Text, out var ppm);
            if (ppm is >= 0 and < 2147483904)
            {
                MyFx.ProcsPerMinute = ppm;
            }
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
            switch (MyFx.EffectType)
            {
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    strArray = Enum.GetNames<Enums.eDamage>();
                    index1 = (int)MyFx.DamageType;
                    lvSubAttribute.Columns[0].Text = "Damage Type / Vector";
                    lvSubAttribute.Columns[0].Width = -2;
                    break;
                
                case Enums.eEffectType.Mez or Enums.eEffectType.MezResist:
                    strArray = Enum.GetNames<Enums.eMez>();
                    index1 = (int)MyFx.MezType;
                    lvSubAttribute.Columns[0].Text = "Mez Type";
                    lvSubAttribute.Columns[0].Width = -2;
                    break;

                case Enums.eEffectType.ResEffect:
                    strArray = Enum.GetNames<Enums.eEffectType>();
                    index1 = (int)MyFx.ETModifies;
                    lvSubAttribute.Columns[0].Text = "Effect Type";
                    lvSubAttribute.Columns[0].Width = -2;
                    break;

                case Enums.eEffectType.EntCreate:
                    {
                        strArray = new string[DatabaseAPI.Database.Entities.Length];
                        var lower = MyFx.Summon.ToLower();
                        for (var index2 = 0; index2 < DatabaseAPI.Database.Entities.Length; index2++)
                        {
                            strArray[index2] = DatabaseAPI.Database.Entities[index2].UID;
                            if (strArray[index2].ToLower() == lower)
                            {
                                index1 = index2;
                            }
                        }

                        lvSubAttribute.Columns[0].Text = "Entity Name";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }

                case Enums.eEffectType.GrantPower:
                case Enums.eEffectType.ExecutePower:
                case Enums.eEffectType.RevokePower:
                    {
                        strArray = new string[DatabaseAPI.Database.Power.Length];
                        var selectedPower = MyFx.EffectType == Enums.eEffectType.RevokePower
                            ? string.IsNullOrWhiteSpace(MyFx.RevokedPower) ? MyFx.Override : MyFx.RevokedPower
                            : MyFx.Summon;
                        var lower = selectedPower.ToLower();
                        for (var index2 = 0; index2 < DatabaseAPI.Database.Power.Length; index2++)
                        {
                            strArray[index2] = DatabaseAPI.Database.Power[index2].FullName;
                            if (strArray[index2].ToLower() == lower)
                            {
                                index1 = index2;
                            }
                        }

                        lvSubAttribute.Columns[0].Text = "Power Name";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }

                case Enums.eEffectType.SetMode:
                case Enums.eEffectType.UnsetMode:
                    {
                        var modeNames = OmniModeMapper.KnownModeNames.ToList();
                        if (!string.IsNullOrWhiteSpace(MyFx.ModeName) &&
                            !modeNames.Contains(MyFx.ModeName, StringComparer.OrdinalIgnoreCase))
                        {
                            modeNames.Add(MyFx.ModeName);
                        }

                        strArray = modeNames
                            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                            .ToArray();
                        var lower = MyFx.ModeName.ToLower();
                        for (var index2 = 0; index2 < strArray.Length; index2++)
                        {
                            if (strArray[index2].ToLower() == lower)
                            {
                                index1 = index2;
                            }
                        }

                        lvSubAttribute.Columns[0].Text = "Mode";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }

                case Enums.eEffectType.Enhancement:
                    strArray = Enum.GetNames(MyFx.EffectType.GetType());
                    index1 = (int)MyFx.ETModifies;
                    lvSubAttribute.Columns[0].Text = "Effect Type";
                    lvSubAttribute.Columns[0].Width = -2;
                    break;

                case Enums.eEffectType.GlobalChanceMod:
                    {
                        strArray = new string[DatabaseAPI.Database.EffectIds.Count];
                        var lower = MyFx.Reward.ToLower();
                        for (var index2 = 0; index2 < DatabaseAPI.Database.EffectIds.Count; index2++)
                        {
                            strArray[index2] = Convert.ToString(DatabaseAPI.Database.EffectIds[index2]);
                            if (strArray[index2].ToLower() == lower)
                            {
                                index1 = index2;
                            }
                        }

                        lvSubAttribute.Columns[0].Text = @"GlobalChanceMod Flag";
                        lvSubAttribute.Columns[0].Width = -2;
                        break;
                    }

                case Enums.eEffectType.PowerRedirect:
                    var allowedTypes = new List<Enums.ePowerSetType>
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

            if (strArray.Length > 0)
            {
                foreach (var s in strArray)
                {
                    lvSubAttribute.Items.Add(s);
                }

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

        private string BuildPreviewEffectString(
            bool simple,
            string specialCat,
            bool noMag,
            bool grouped,
            bool useBaseProbability,
            bool fromPopup,
            bool editorDisplay)
        {
            var previewEffect = PowerModifierDisplayContextResolver.CreateContextualEffectClone(
                MyFx,
                MyPower ?? MyFx.GetPower(),
                _modifierContextClassName);

            return previewEffect.BuildEffectString(
                simple,
                specialCat,
                noMag,
                grouped,
                useBaseProbability,
                fromPopup,
                editorDisplay);
        }

        private void UpdateFxText(string? senderName = "")
        {
            if (_loading)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(senderName))
            {
                var errorString = string.Empty;
                Expressions.Validate(MyFx, out var validationItems);
                if (validationItems.All(x => x.Validated))
                {
                    lblEffectDescription.ForeColor = SystemColors.ControlText;
                    lblEffectDescription.Text = BuildPreviewEffectString(false, string.Empty, false, false, false, false, true);
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
                lblEffectDescription.Text = BuildPreviewEffectString(false, string.Empty, false, false, false, false, true);
            }
        }

        private void UpdateSubSubList()
        {
            var index1 = 0;
            lvSubSub.BeginUpdate();
            lvSubSub.Items.Clear();
            
            var strArray = Array.Empty<string>();
            switch (MyFx.EffectType)
            {
                case Enums.eEffectType.Enhancement when MyFx.ETModifies == Enums.eEffectType.Mez:
                case Enums.eEffectType.Enhancement when MyFx.ETModifies == Enums.eEffectType.MezResist:
                case Enums.eEffectType.ResEffect when MyFx.ETModifies == Enums.eEffectType.Mez:
                    lvSubSub.Columns[0].Text = @"Mez Type";
                    lvSubSub.Columns[0].Width = -2;
                    strArray = Enum.GetNames(typeof(Enums.eMez));
                    index1 = (int)MyFx.MezType;

                    break;

                //case Enums.eEffectType.Enhancement when MyFx.ETModifies is Enums.eEffectType.Damage or Enums.eEffectType.DamageBuff or Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity:
                case Enums.eEffectType.Enhancement when MyFx.ETModifies is Enums.eEffectType.Damage or Enums.eEffectType.Defense or Enums.eEffectType.Resistance:
                    lvSubSub.Columns[0].Text = @"Damage Type";
                    lvSubSub.Columns[0].Width = -2;
                    strArray = Enum.GetNames(typeof(Enums.eDamage));
                    index1 = (int)MyFx.DamageType;

                    break;

                case Enums.eEffectType.PowerRedirect:
                    strArray = DatabaseAPI.Database.Power
                        .ToList()
                        .Where(x => x.GroupName == lvSubAttribute.SelectedItems[0].Text).Select(p => p.FullName)
                        .ToArray();
                    lvSubSub.Columns[0].Text = @"Power";
                    lvSubSub.Columns[0].Width = -2;

                    break;

                /*case Enums.eEffectType.ResEffect:
                    lvSubSub.Columns[0].Text = @"Effect type";
                    lvSubSub.Columns[0].Width = -2;
                    strArray = Enum.GetNames(typeof(Enums.eEffectType));
                    index1 = 0;

                    break;*/
            }

            if (strArray.Length > 0)
            {
                foreach (var s in strArray)
                {
                    var lvSubItem = new ListViewItem(s)
                    {
                        ToolTipText = s
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
