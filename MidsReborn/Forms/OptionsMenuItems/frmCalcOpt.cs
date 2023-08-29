using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class frmCalcOpt : Form
    {
        private readonly short[] defActs;
        private readonly frmMain myParent;
        private readonly string[][] scenActs;

        private readonly string[] scenarioExample;

        private bool fcNoUpdate;
        private readonly IEnumerable<RadioButton> _updRadios;

        private ConfigData.AutoUpdType _autoUpdType = MidsContext.Config.AutomaticUpdates.Type;

        private ConfigData.AutoUpdType AutoUpdate
        {
            get => _autoUpdType;
            set
            {
                _autoUpdType = value;
                tbUpdDelayDays.Enabled = _autoUpdType == ConfigData.AutoUpdType.Delay;
            }
        }

        public frmCalcOpt(ref frmMain iParent)
        {
            Load += frmCalcOpt_Load;
            Closing += frmCalcOpt_Closing;
            fcNoUpdate = false;
            scenarioExample = new string[20];
            scenActs = new string[20][];
            defActs = new short[20];
            InitializeComponent();
            _updRadios = GroupBox1.Controls.OfType<RadioButton>();
            Name = nameof(frmCalcOpt);
            optTO.Image = Resources.optTO_Image;
            optDO.Image = Resources.optDO_Image;
            optSO.Image = Resources.optSO_Image;
            Icon = Resources.MRB_Icon_Concept;
            myParent = iParent;
        }

        private void BindAndDefaultUpdRadios()
        {
            foreach (var radio in _updRadios)
            {
                radio.Tag = radio.Name switch
                {
                    "rbUpdDisabled" => ConfigData.AutoUpdType.Disabled,
                    "rbUpdStartup" => ConfigData.AutoUpdType.Startup,
                    "rbUpdDelay" => ConfigData.AutoUpdType.Delay,
                    _ => radio.Tag
                };
                radio.CheckedChanged += UpdRadioOnCheckedChanged;
                if ((ConfigData.AutoUpdType)radio.Tag == MidsContext.Config.AutomaticUpdates.Type)
                {
                    radio.Checked = true;
                }
            }


        }

        private void UpdRadioOnCheckedChanged(object? sender, EventArgs e)
        {
            if (sender is not RadioButton selected) return;
            AutoUpdate = (ConfigData.AutoUpdType)selected.Tag;
            if (AutoUpdate is ConfigData.AutoUpdType.Startup or ConfigData.AutoUpdType.Disabled) tbUpdDelayDays.Text = @"0";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnFontColor_Click(object sender, EventArgs e)
        {
            using var frmColorOptions = new frmColorOptions();
            frmColorOptions.ShowDialog();
        }

        private void btnIOReset_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character == null)
                return;
            var int32 = Convert.ToInt32(udIOLevel.Value);
            MidsContext.Character.CurrentBuild.SetIOLevels(int32, false, false);
            myParent.ChildRequestedRedraw();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            StoreControls();
            myParent.DoCalcOptUpdates();
            myParent.UpdateWindowStyle();
            MidsContext.Config!.SaveConfig();
            Hide();
        }

        private void btnSaveFolder_Click(object sender, EventArgs e)
        {
            var priorPath = lblSaveFolder.Text;
            fbdSave.SelectedPath = lblSaveFolder.Text;
            if (fbdSave.ShowDialog() != DialogResult.OK)
                return;
            lblSaveFolder.Text = fbdSave.SelectedPath;
            MovePrior(priorPath, fbdSave.SelectedPath);
        }

        private void MovePrior(string priorPath, string currentPath)
        {
            Debug.WriteLine($"Prev: {priorPath}\nCurrent: {currentPath}");
            if (currentPath == priorPath)
            {
                return;
            }

            var previousHasBuilds = Directory.EnumerateFileSystemEntries(priorPath, "*.*", SearchOption.AllDirectories).ToList().Any();
            if (!previousHasBuilds)
            {
                return;
            }

            var msgResult = MessageBox.Show(@"Do you want to move your builds to the new location?", @"Builds Path Change Detected!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msgResult != DialogResult.Yes)
            {
                return;
            }

            using var fMover = new FileMover(priorPath, currentPath);
            var result = fMover.ShowDialog(this);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show(@"All builds have been moved to the new location.", @"Move Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var dirResult = MessageBox.Show(@"Do you wish to delete the old directory?", @"Remove Prior Directory?", MessageBoxButtons.YesNo);
                if (dirResult == DialogResult.Yes) Directory.Delete(priorPath, true);
            }
            else
            {
                MessageBox.Show(@"Some builds could not be moved to the new location.
Please move these items manually.", @"Move Completed With Exceptions", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSaveFolderReset_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ResetBuildsPath();
            lblSaveFolder.Text = MidsContext.Config.BuildsPath;
        }

        private void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            var values = (int[])Enum.GetValues(MidsContext.Config.Suppression.GetType());
            MidsContext.Config.Suppression = Enums.eSuppress.None;
            var num = clbSuppression.CheckedIndices.Count - 1;
            for (var index = 0; index <= num; ++index)
                MidsContext.Config.Suppression += values[clbSuppression.CheckedIndices[index]];
        }

        private void cmbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            defActs[listScenarios.SelectedIndex] = (short)cmbAction.SelectedIndex;
        }

        private void frmCalcOpt_Closing(object? sender, CancelEventArgs e)
        {
            if (DialogResult != DialogResult.Abort)
                return;
            e.Cancel = true;
        }

        private void frmCalcOpt_Load(object? sender, EventArgs e)
        {
            SetupScenarios();
            SetControls();
            PopulateSuppression();
        }

        private void ServerSpecificEnablement()
        {
            if (!MidsContext.Config.MasterMode)
            {
                TabControl1.TabPages[^1].Enabled = false;
            }
        }

        private void listScenarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblExample.Text = scenarioExample[listScenarios.SelectedIndex];
            cmbAction.Items.Clear();
            cmbAction.Items.AddRange(scenActs[listScenarios.SelectedIndex]);
            cmbAction.SelectedIndex = defActs[listScenarios.SelectedIndex];
        }

        private void optDO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optDO.Checked)
                return;
            optEnh.Text = "Dual Origin";
        }

        private void optSO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optSO.Checked)
                return;
            optEnh.Text = "Single Origin";
        }

        private void optTO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optTO.Checked)
                return;
            optEnh.Text = "Training Origin";
        }

        // Ref: chkIOLevel
        private void chkShowSOLevels_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void chkEnableDmgGraph_CheckedChanged(object sender, EventArgs e)
        {
            rbGraphTwoLine.Enabled = chkEnableDmgGraph.Checked;
            rbGraphStacked.Enabled = chkEnableDmgGraph.Checked;
            rbGraphSimple.Enabled = chkEnableDmgGraph.Checked;
        }

        private void chkOldStyle_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOldStyle.Checked)
            {
                label2.Enabled = false;
                cbTotalsWindowTitleOpt.Enabled = false;
            }
            else
            {
                label2.Enabled = true;
                cbTotalsWindowTitleOpt.Enabled = true;
            }
        }

        private void PopulateSuppression()
        {
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            var names = Enum.GetNames(MidsContext.Config.Suppression.GetType());
            var values = (int[])Enum.GetValues(MidsContext.Config.Suppression.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                clbSuppression.Items.Add(names[index],
                    (MidsContext.Config.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            clbSuppression.EndUpdate();
        }

        private void SetControls()
        {
            var config = MidsContext.Config;
            SuspendLayout();
            optSO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.SingleO;
            optDO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.DualO;
            optTO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.TrainingO;
            cbEnhLevel.SelectedIndex = (int)config.CalcEnhLevel;
            chkHighVis.Checked = config.EnhanceVisibility;
            rbGraphTwoLine.Checked = config.DataGraphType == Enums.eDDGraph.Both;
            rbGraphStacked.Checked = config.DataGraphType == Enums.eDDGraph.Stacked;
            rbGraphSimple.Checked = config.DataGraphType == Enums.eDDGraph.Simple;
            rbPvE.Checked = !config.Inc.DisablePvE;
            rbPvP.Checked = config.Inc.DisablePvE;
            rbChanceAverage.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Average;
            rbChanceMax.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Max;
            rbChanceIgnore.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum;
            BindAndDefaultUpdRadios();
            AutoUpdate = config.AutomaticUpdates.Type;
            chkShowSOLevels.Checked = config.ShowSoLevels;
            chkEnableDmgGraph.Checked = !config.DisableDataDamageGraph;
            rbGraphTwoLine.Enabled = chkEnableDmgGraph.Checked;
            rbGraphStacked.Enabled = chkEnableDmgGraph.Checked;
            rbGraphSimple.Enabled = chkEnableDmgGraph.Checked;
            udIOLevel.Value = decimal.Compare(new decimal(config.I9.DefaultIOLevel + 1), udIOLevel.Maximum) <= 0
                ? new decimal(config.I9.DefaultIOLevel + 1)
                : udIOLevel.Maximum;
            chkIOLevel.Checked = !config.I9.HideIOLevels;
            chkIOEffects.Checked = !config.I9.IgnoreEnhFX;
            chkSetBonus.Checked = !config.I9.IgnoreSetBonusFX;
            chkRelSignOnly.Checked = config.ShowRelSymbols;
            chkIOPrintLevels.Checked = !config.I9.DisablePrintIOLevels;
            chkColorPrint.Checked = config.PrintInColor;
            udRTFSize.Value = new decimal(config.RtFont.RTFBase / 2.0);
            udStatSize.Value = new decimal(config.RtFont.PairedBase);
            udPowSelectSize.Value = new decimal(config.RtFont.PowersSelectBase);
            udPowersSize.Value = new decimal(config.RtFont.PowersBase);
            chkTextBold.Checked = config.RtFont.RTFBold;
            chkStatBold.Checked = config.RtFont.PairedBold;
            chkPowSelBold.Checked = config.RtFont.PowersSelectBold;
            chkPowersBold.Checked = config.RtFont.PowersBold;
            chkLoadLastFile.Checked = !config.DisableLoadLastFileOnStart;
            chkMiddle.Checked = !config.DisableRepeatOnMiddleClick;
            chkNoTips.Checked = config.NoToolTips;
            chkShowAlphaPopup.Checked = !config.DisableAlphaPopup;
            chkUseArcanaTime.Checked = config.UseArcanaTime;
            TeamSize.Value = new decimal(config.TeamSize);
            var index = 0;
            do
            {
                defActs[index] = config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            cbTotalsWindowTitleOpt.SelectedIndex = (int)config.TotalsWindowTitleStyle;
            chkOldStyle.Checked = config.UseOldTotalsWindow;
            cbCurrency.Items.Clear();
            foreach (var c in Enum.GetValues(typeof(Enums.RewardCurrency)))
            {
                cbCurrency.Items.Add(clsRewardCurrency.GetCurrencyName((Enums.RewardCurrency)c));
            }

            cbCurrency.SelectedIndex = (int)config.PreferredCurrency;
            chkShowSelfBuffsAny.Checked = config.ShowSelfBuffsAny;
            lblSaveFolder.Text = config.BuildsPath;
            chkWarnOldAppVersion.Checked = config.WarnOnOldAppMbd;
            chkWarnOldDbVersion.Checked = config.WarnOnOldDbMbd;
            chkDimWindowBorders.Checked = config.DimWindowStyleColors;
            rbEnhPopupCloseStyle1.Checked = config.CloseEnhSelectPopupByMove;
            rbEnhPopupCloseStyle2.Checked = !config.CloseEnhSelectPopupByMove;
            ResumeLayout();
        }

        private void SetupScenarios()
        {
            scenarioExample[0] = "Swap a travel power with a power taken at level 2.";
            scenActs[0] = new[]
            {
                "Show dialog",
                "Cancel",
                "Move/swap power to its lowest possible level",
                "Allow power to be moved anyway (mark as invalid)"
            };
            scenarioExample[1] =
                "Move a Primary power from level 35 into the level 44 slot of a character with 4 epic powers.";
            scenActs[1] = new[]
            {
                "Show dialog",
                "Cancel",
                "Move to the last power that isn't at its min level"
            };
            scenarioExample[2] =
                "Power taken at level 2 with two level 3 slots is swapped with level 4, where there is a power with one slot.";
            scenActs[2] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[3] =
                "A 6-slotted power taken at level 41 is moved to level 49.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[3] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[4] =
                "Power taken at level 4 is swapped with power taken at level 14, which is a travel power.";
            scenActs[4] = new[]
            {
                "Show dialog",
                "Cancel",
                "Overwrite rather than swap",
                "Allow power to be swapped anyway (mark as invalid)"
            };
            scenarioExample[5] =
                "Power taken at level 8 is swapped with power taken at level 2, when the level 2 power has level 3 slots.";
            scenActs[5] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[6] =
                "Pool power taken at level 49 is swapped with a 6-slotted power at level 41.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[6] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[7] =
                "Power taken at level 4 is moved to level 8 when the power taken at level 6 is a pool power.\r\n(Note: If the power in the destination slot fails to shift, the 'Moved or swapped too low' scenario applies.)";
            scenActs[7] = new[]
            {
                "Show dialog",
                "Cancel",
                "Shift other powers around it",
                "Overwrite it; leave previous power slot empty",
                "Allow anyway (mark as invalid)"
            };
            scenarioExample[8] =
                "Power taken at level 8 has level 9 slots, and a power is being moved from level 12 to level 6, so the power at 8 is shifting up to 10.";
            scenActs[8] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[9] =
                "Power taken at level 47 has 6 slots, and a power is being moved from level 49 to level 44, so the power at 47 is shifting to 49.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[9] = new[]
            {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[10] = "Power taken at level 8 is being moved to 14, and the level 10 slot is blank.";
            scenActs[10] = new[]
            {
                "Show dialog",
                "Cancel",
                "Fill empty slot; don't move powers unnecessarily",
                "Shift empty slot as if it were a power"
            };
            scenarioExample[11] =
                "Power placed at its minimum level is being shifted up.\r\n(Note: If and only the power in the destination slot fails to shift due to this setting, the next scenario applies.)";
            scenActs[11] = new[]
            {
                "Show dialog",
                "Cancel",
                "Shift it along with the other powers",
                "Shift other powers around it"
            };
            scenarioExample[12] =
                "You chose to shift other powers around ones that are at their minimum levels, but you are moving a power in place of one that is at its minimum level. (This will never occur if you chose 'Cancel' or 'Shift it along with the other powers' from the previous scenario.)";
            scenActs[12] = new[]
            {
                "Show dialog",
                "Cancel",
                "Unlock and shift all level-locked powers",
                "Shift destination power to the first valid and empty slot",
                "Swap instead of move"
            };
            scenarioExample[13] = "Click and drag a level 21 slot from a level 20 power to a level 44 power.";
            scenActs[13] = new[]
            {
                "Show dialog",
                "Cancel",
                "Allow swap anyway (mark as invalid)"
            };
            scenarioExample[14] =
                "Click and drag a slot from a level 44 power to a level 20 power in place of a level 21 slot.";
            scenActs[14] = new[]
            {
                "Show dialog",
                "Cancel",
                "Allow swap anyway (mark as invalid)"
            };
        }

        private void StoreControls()
        {
            var config = MidsContext.Config;
            if (optSO.Checked)
            {
                config.CalcEnhOrigin = Enums.eEnhGrade.SingleO;
            }
            else if (optDO.Checked)
            {
                config.CalcEnhOrigin = Enums.eEnhGrade.DualO;
            }
            else if (optTO.Checked)
            {
                config.CalcEnhOrigin = Enums.eEnhGrade.TrainingO;
            }

            config.CalcEnhLevel = (Enums.eEnhRelative)cbEnhLevel.SelectedIndex;
            if (rbGraphTwoLine.Checked)
            {
                config.DataGraphType = Enums.eDDGraph.Both;
            }
            else if (rbGraphStacked.Checked)
            {
                config.DataGraphType = Enums.eDDGraph.Stacked;
            }
            else if (rbGraphSimple.Checked)
            {
                config.DataGraphType = Enums.eDDGraph.Simple;
            }

            config.Inc.DisablePvE = !rbPvE.Checked;
            if (rbChanceAverage.Checked)
            {
                config.DamageMath.Calculate = ConfigData.EDamageMath.Average;
            }
            else if (rbChanceMax.Checked)
            {
                config.DamageMath.Calculate = ConfigData.EDamageMath.Max;
            }
            else if (rbChanceIgnore.Checked)
            {
                config.DamageMath.Calculate = ConfigData.EDamageMath.Minimum;
            }

            config.DisableVillainColors = false;
            config.AutomaticUpdates.Type = AutoUpdate;
            config.AutomaticUpdates.Delay = Convert.ToInt32(tbUpdDelayDays.Text);
            //config.CheckForUpdates = chkUpdateStartup.Checked;
            config.I9.DefaultIOLevel = Convert.ToInt32(udIOLevel.Value) - 1;
            config.I9.IgnoreEnhFX = !chkIOEffects.Checked;
            config.I9.IgnoreSetBonusFX = !chkSetBonus.Checked;
            config.I9.HideIOLevels = !chkIOLevel.Checked;
            config.ShowRelSymbols = chkRelSignOnly.Checked;
            config.ShowSoLevels = chkShowSOLevels.Checked;
            config.I9.DisablePrintIOLevels = !chkIOPrintLevels.Checked;
            config.PrintInColor = chkColorPrint.Checked;
            config.RtFont.RTFBase = Convert.ToInt32(decimal.Multiply(udRTFSize.Value, new decimal(2)));
            config.RtFont.PairedBase = Convert.ToSingle(udStatSize.Value);
            config.RtFont.RTFBold = chkTextBold.Checked;
            config.RtFont.PairedBold = chkStatBold.Checked;
            config.RtFont.PowersSelectBase = Convert.ToSingle(udPowSelectSize.Value);
            config.RtFont.PowersSelectBold = chkPowSelBold.Checked;
            config.RtFont.PowersBase = Convert.ToSingle(udPowersSize.Value);
            config.RtFont.PowersBold = chkPowersBold.Checked;
            config.DisableLoadLastFileOnStart = !chkLoadLastFile.Checked;
            if (config.BuildsPath != lblSaveFolder.Text)
            {
                config.BuildsPath = lblSaveFolder.Text;
                myParent.DlgOpen.InitialDirectory = config.BuildsPath;
                myParent.DlgSave.InitialDirectory = config.BuildsPath;
            }

            config.EnhanceVisibility = chkHighVis.Checked;
            //config.UpdatePath = this.txtUpdatePath.Text;
            config.DisableDesaturateInherent = false;
            config.DisableRepeatOnMiddleClick = !chkMiddle.Checked;
            config.NoToolTips = chkNoTips.Checked;
            config.DisableAlphaPopup = !chkShowAlphaPopup.Checked;
            config.UseArcanaTime = chkUseArcanaTime.Checked;
            config.TeamSize = Convert.ToInt32(TeamSize.Value);
            config.TotalsWindowTitleStyle = (ConfigData.ETotalsWindowTitleStyle)cbTotalsWindowTitleOpt.SelectedIndex;
            config.UseOldTotalsWindow = chkOldStyle.Checked;
            var index = 0;
            do
            {
                config.DragDropScenarioAction[index] = defActs[index];
                ++index;
            } while (index <= 19);
            config.PreferredCurrency = (Enums.RewardCurrency)cbCurrency.SelectedIndex;
            config.WarnOnOldAppMbd = chkWarnOldAppVersion.Checked;
            config.WarnOnOldDbMbd = chkWarnOldDbVersion.Checked;
            config.DimWindowStyleColors = chkDimWindowBorders.Checked;
            config.CloseEnhSelectPopupByMove = rbEnhPopupCloseStyle1.Checked;
        }

        private void chkShowSelfBuffsAny_CheckedChanged(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSelfBuffsAny = chkShowSelfBuffsAny.Checked;
        }

        private void FileAssocStatus_Update()
        {
            var filesAssociated = Association.FileTypeScan();
            switch (filesAssociated)
            {
                case false:
                    FileAssocStatus.ForeColor = Color.Orange;
                    FileAssocStatus.Text = @"WARNING";
                    myTip.SetToolTip(FileAssocStatus, "One or more build file types are not associated with this MRB.");
                    break;
                case true:
                    FileAssocStatus.ForeColor = Color.Green;
                    FileAssocStatus.Text = @"VALID";
                    myTip.SetToolTip(FileAssocStatus, "All build files are associated.");
                    break;
            }
        }

        private void SchemaAssocStatus_Update()
        {
            var schemaAssociated = Association.SchemaScan();
            switch (schemaAssociated)
            {
                case false:
                    SchemaStatus.ForeColor = Color.Orange;
                    SchemaStatus.Text = @"WARNING";
                    myTip.SetToolTip(SchemaStatus, "Build sharing schema is not registered to this MRB.");
                    break;
                case true:
                    SchemaStatus.ForeColor = Color.Green;
                    SchemaStatus.Text = @"VALID";
                    myTip.SetToolTip(SchemaStatus, "Build sharing schema is associated");
                    break;
            }
        }

        private void btnRepairFileAssoc_Click(object sender, EventArgs e)
        {
            Association.RepairFileTypes();
            FileAssocStatus_Update();
        }

        private void btnRepairSchemaAssoc_Click(object sender, EventArgs e)
        {
            Association.RepairSchema();
            SchemaAssocStatus_Update();
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (tabControl.SelectedIndex != 3) return;
            FileAssocStatus_Update();
            SchemaAssocStatus_Update();
        }

        private void Status_MouseHover(object sender, EventArgs e)
        {
            var control = sender as Control;
            var controlName = control?.Name;
            if (!string.IsNullOrWhiteSpace(controlName))
            {
                switch (controlName)
                {
                    case "FileAssocStatus":
                        break;
                    case "SchemaAssocStatus":
                        break;
                }
            }
        }

        private void rbEnhPopupCloseStyle1_CheckedChanged(object sender, EventArgs e)
        {
            rbEnhPopupCloseStyle2.Checked = !rbEnhPopupCloseStyle1.Checked;
        }

        private void rbEnhPopupCloseStyle2_CheckedChanged(object sender, EventArgs e)
        {
            rbEnhPopupCloseStyle1.Checked = !rbEnhPopupCloseStyle2.Checked;
        }

        private void TbUpdDelayDaysOnTextChanged(object? sender, EventArgs e)
        {
            if (sender is not TextBox textBox) return;
            if (!textBox.Text.All(char.IsDigit))
            {
                textBox.Text = "3";
            };
        }
    }
}