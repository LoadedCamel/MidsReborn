using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.ImportExportItems;
using mrbBase;
using mrbBase.Base.Master_Classes;
using mrbControls;
using WK.Libraries.BetterFolderBrowserNS;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class frmCalcOpt : Form
    {
        private readonly short[] defActs;
        private readonly frmMain myParent;
        private readonly string[][] scenActs;

        private readonly string[] scenarioExample;

        private bool fcNoUpdate;

        public frmCalcOpt(ref frmMain iParent)
        {
            Load += frmCalcOpt_Load;
            Closing += frmCalcOpt_Closing;
            fcNoUpdate = false;
            scenarioExample = new string[20];
            scenActs = new string[20][];
            defActs = new short[20];
            InitializeComponent();
            Name = nameof(frmCalcOpt);
            optTO.Image = Resources.optTO_Image;
            optDO.Image = Resources.optDO_Image;
            optSO.Image = Resources.optSO_Image;
            Icon = Resources.reborn;
            myParent = iParent;
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

            var previousHasBuilds = Directory.EnumerateFileSystemEntries(priorPath).ToList().Any();
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
                Directory.Delete(priorPath, true);
                MessageBox.Show(@"All items have been moved to the new location.", @"Operation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(@"Some items could not be moved to the new location.
Please move these items manually.", @"Operation Completed With Exceptions", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void csAdd_Click(object sender, EventArgs e)
        {
            MidsContext.Config.Export.AddScheme();
            csPopulateList(MidsContext.Config.Export.ColorSchemes.Length - 1);
        }

        private void csBtnEdit_Click(object sender, EventArgs e)
        {
            if (csList.Items.Count <= 0)
                return;
            var frmExportColor = new frmExportColor(ref MidsContext.Config.Export.ColorSchemes[csList.SelectedIndex]);
            if (frmExportColor.ShowDialog() == DialogResult.OK)
            {
                MidsContext.Config.Export.ColorSchemes[csList.SelectedIndex].Assign(frmExportColor.myScheme);
                csPopulateList();
            }

            BringToFront();
        }

        private void csDelete_Click(object sender, EventArgs e)
        {
            if (csList.Items.Count <= 0 || MessageBox.Show($"Delete {csList.SelectedItem}?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            MidsContext.Config.Export.RemoveScheme(csList.SelectedIndex);
            csPopulateList();
        }

        private void csList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToString(e.KeyChar) == "[")
            {
                forumColorUp();
            }
            else
            {
                if (Convert.ToString(e.KeyChar) != "]")
                    return;
                ForumColorDown();
            }
        }

        private void csPopulateList(int HighlightID = -1)
        {
            csList.Items.Clear();
            var export = MidsContext.Config.Export;
            var num = export.ColorSchemes.Length - 1;
            for (var index = 0; index <= num; ++index)
                csList.Items.Add(export.ColorSchemes[index].SchemeName);
            if ((csList.Items.Count > 0) & (HighlightID == -1))
                csList.SelectedIndex = 0;
            if (!((HighlightID < csList.Items.Count) & (HighlightID > -1)))
                return;
            csList.SelectedIndex = HighlightID;
        }

        private void csReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will remove all of the colors you have set and replace them with the defaults. Do you want to do this?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            MidsContext.Config.Export.ResetColorsToDefaults();
            csPopulateList();
        }

        private void fcAdd_Click(object sender, EventArgs e)
        {
            MidsContext.Config.Export.AddCodes();
            fcPopulateList(MidsContext.Config.Export.FormatCode.Length - 1);
        }

        private void fcBoldOff_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].BoldOff = fcBoldOff.Text;
        }

        private void fcBoldOn_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].BoldOn = fcBoldOn.Text;
        }

        private void fcColorOff_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ColorOff = fcColorOff.Text;
        }

        private void fcColorOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 || fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ColorOn = fcColorOn.Text;
        }

        private void fcDelete_Click(object sender, EventArgs e)
        {
            if (fcList.Items.Count <= 0 || MessageBox.Show($"Delete {fcList.SelectedItem}?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            MidsContext.Config.Export.RemoveCodes(fcList.SelectedIndex);
            fcPopulateList();
        }

        private void fcDisplay()
        {
            fcNoUpdate = true;
            if (fcList.SelectedIndex > -1)
            {
                var formatCode = MidsContext.Config.Export.FormatCode;
                var selectedIndex = fcList.SelectedIndex;
                fcName.Text = formatCode[selectedIndex].Name;
                fcNotes.Text = formatCode[selectedIndex].Notes;
                fcColorOn.Text = formatCode[selectedIndex].ColorOn;
                fcColorOff.Text = formatCode[selectedIndex].ColorOff;
                fcTextOn.Text = formatCode[selectedIndex].SizeOn;
                fcTextOff.Text = formatCode[selectedIndex].SizeOff;
                fcBoldOn.Text = formatCode[selectedIndex].BoldOn;
                fcBoldOff.Text = formatCode[selectedIndex].BoldOff;
                fcItalicOn.Text = formatCode[selectedIndex].ItalicOn;
                fcItalicOff.Text = formatCode[selectedIndex].ItalicOff;
                fcUnderlineOn.Text = formatCode[selectedIndex].UnderlineOn;
                fcUnderlineOff.Text = formatCode[selectedIndex].UnderlineOff;
                fcWSSpace.Checked = formatCode[selectedIndex].Space == ExportConfig.WhiteSpace.Space;
                fcWSTab.Checked = formatCode[selectedIndex].Space == ExportConfig.WhiteSpace.Tab;
            }
            else
            {
                fcName.Text = "";
                fcNotes.Text = "";
                fcColorOn.Text = "";
                fcColorOff.Text = "";
                fcTextOn.Text = "";
                fcTextOff.Text = "";
                fcBoldOn.Text = "";
                fcBoldOff.Text = "";
                fcItalicOn.Text = "";
                fcItalicOff.Text = "";
                fcUnderlineOn.Text = "";
                fcUnderlineOff.Text = "";
                fcWSSpace.Checked = true;
            }

            fcNoUpdate = false;
        }

        private void fcItalicOff_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ItalicOff = fcItalicOff.Text;
        }

        private void fcItalicOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 || fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ItalicOn = fcItalicOn.Text;
        }

        private void fcList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToString(e.KeyChar) == "[")
            {
                forumCodeUp();
            }
            else
            {
                if (Convert.ToString(e.KeyChar) != "]")
                    return;
                ForumCodeDown();
            }
        }

        private void fcList_SelectedIndexChanged(object sender, EventArgs e)
        {
            fcDisplay();
        }

        private void fcNotes_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Notes = fcNotes.Text;
        }

        private void fcPopulateList(int HighlightID = -1)
        {
            fcList.Items.Clear();
            var export = MidsContext.Config.Export;
            var num = export.FormatCode.Length - 1;
            for (var index = 0; index <= num; ++index)
                fcList.Items.Add(export.FormatCode[index].Name);
            if ((fcList.Items.Count > 0) & (HighlightID == -1))
                fcList.SelectedIndex = 0;
            if (!((HighlightID < fcList.Items.Count) & (HighlightID > -1)))
                return;
            fcList.SelectedIndex = HighlightID;
        }

        private void fcReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will remove all of the formatting code sets and replace them with the default set. Do you want to do this?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            MidsContext.Config.Export.ResetCodesToDefaults();
            fcPopulateList();
        }

        private void fcSet_Click(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Name = fcName.Text;
            fcPopulateList(fcList.SelectedIndex);
        }

        private void fcTextOff_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].SizeOff = fcTextOff.Text;
        }

        private void fcTextOn_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].SizeOn = fcTextOn.Text;
        }

        private void fcUnderlineOff_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].UnderlineOff = fcUnderlineOff.Text;
        }

        private void fcUnderlineOn_TextChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].UnderlineOn = fcUnderlineOn.Text;
        }

        private void fcWSSpace_CheckedChanged(object sender, EventArgs e)
        {
            if ((fcList.SelectedIndex < 0) | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Space =
                !fcWSSpace.Checked ? ExportConfig.WhiteSpace.Tab : ExportConfig.WhiteSpace.Space;
        }

        private void ForumCodeDown()
        {
            var selectedIndex = fcList.SelectedIndex;
            if (selectedIndex >= fcList.Items.Count - 1)
                return;
            var formatCodesArray = new ExportConfig.FormatCodes[2];
            formatCodesArray[0].Assign(MidsContext.Config.Export.FormatCode[selectedIndex]);
            formatCodesArray[1].Assign(MidsContext.Config.Export.FormatCode[selectedIndex + 1]);
            MidsContext.Config.Export.FormatCode[selectedIndex].Assign(formatCodesArray[1]);
            MidsContext.Config.Export.FormatCode[selectedIndex + 1].Assign(formatCodesArray[0]);
            fcPopulateList();
            if ((selectedIndex + 1 > -1) & (fcList.Items.Count > selectedIndex + 1))
                fcList.SelectedIndex = selectedIndex + 1;
            else if (fcList.Items.Count > 0)
                fcList.SelectedIndex = 0;
        }

        private void forumCodeUp()
        {
            var selectedIndex = fcList.SelectedIndex;
            if (selectedIndex < 1)
                return;
            var formatCodesArray = new ExportConfig.FormatCodes[2];
            formatCodesArray[0].Assign(MidsContext.Config.Export.FormatCode[selectedIndex]);
            formatCodesArray[1].Assign(MidsContext.Config.Export.FormatCode[selectedIndex - 1]);
            MidsContext.Config.Export.FormatCode[selectedIndex].Assign(formatCodesArray[1]);
            MidsContext.Config.Export.FormatCode[selectedIndex - 1].Assign(formatCodesArray[0]);
            fcPopulateList();
            if ((selectedIndex - 1 > -1) & (fcList.Items.Count > selectedIndex - 1))
                fcList.SelectedIndex = selectedIndex - 1;
            else if (fcList.Items.Count > 0)
                fcList.SelectedIndex = 0;
        }

        private void ForumColorDown()
        {
            var selectedIndex = csList.SelectedIndex;
            if (selectedIndex >= csList.Items.Count - 1)
                return;
            var colorSchemeArray = new ExportConfig.ColorScheme[2];
            colorSchemeArray[0].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex]);
            colorSchemeArray[1].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex + 1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex].Assign(colorSchemeArray[1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex + 1].Assign(colorSchemeArray[0]);
            csPopulateList();
            if ((selectedIndex + 1 > -1) & (csList.Items.Count > selectedIndex + 1))
                csList.SelectedIndex = selectedIndex + 1;
            else if (csList.Items.Count > 0)
                csList.SelectedIndex = 0;
        }

        private void forumColorUp()
        {
            var selectedIndex = csList.SelectedIndex;
            if (selectedIndex < 1)
                return;
            var colorSchemeArray = new ExportConfig.ColorScheme[2];
            colorSchemeArray[0].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex]);
            colorSchemeArray[1].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex - 1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex].Assign(colorSchemeArray[1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex - 1].Assign(colorSchemeArray[0]);
            csPopulateList();
            if ((selectedIndex - 1 > -1) & (csList.Items.Count > selectedIndex - 1))
                csList.SelectedIndex = selectedIndex - 1;
            else if (csList.Items.Count > 0)
                csList.SelectedIndex = 0;
        }

        private void frmCalcOpt_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult != DialogResult.Abort)
                return;
            e.Cancel = true;
        }

        private void frmCalcOpt_Load(object sender, EventArgs e)
        {
            setupScenarios();
            SetControls();
            csPopulateList();
            fcPopulateList();
            PopulateSuppression();
            var associatedProgram = FileAssociation.CheckdAssociatedProgram();
            var regAssociations = FileAssociation.CheckAssociations();
            if (!regAssociations)
            {
                lblAssocStatus.Text = @"Status: settings missing";
            }
            else if (!string.Equals(associatedProgram, Application.ExecutablePath, StringComparison.InvariantCultureIgnoreCase))
            {
                lblAssocStatus.Text = @"Status: .MXD set to a different program";
            }
            else
            {
                lblAssocStatus.Text = @"Status: Ok";
            }
        }

        private void ServerSpecificEnablement()
        {
            if (!MidsContext.Config.MasterMode)
            {
                TabControl1.TabPages[TabControl1.TabPages.Count - 1].Enabled = false;
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

            chkUpdates.Checked = config.CheckForUpdates;
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
                cbCurrency.Items.Add(clsRewardCurrency.GetCurrencyName((Enums.RewardCurrency) c));
            }

            cbCurrency.SelectedIndex = (int) config.PreferredCurrency;
            chkShowSelfBuffsAny.Checked = config.ShowSelfBuffsAny;
            lblSaveFolder.Text = config.BuildsPath;
            ResumeLayout();
        }

        private void setupScenarios()
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
                config.CalcEnhOrigin = Enums.eEnhGrade.SingleO;
            else if (optDO.Checked)
                config.CalcEnhOrigin = Enums.eEnhGrade.DualO;
            else if (optTO.Checked)
                config.CalcEnhOrigin = Enums.eEnhGrade.TrainingO;
            config.CalcEnhLevel = (Enums.eEnhRelative)cbEnhLevel.SelectedIndex;
            if (rbGraphTwoLine.Checked)
                config.DataGraphType = Enums.eDDGraph.Both;
            else if (rbGraphStacked.Checked)
                config.DataGraphType = Enums.eDDGraph.Stacked;
            else if (rbGraphSimple.Checked)
                config.DataGraphType = Enums.eDDGraph.Simple;
            config.Inc.DisablePvE = !rbPvE.Checked;
            if (rbChanceAverage.Checked)
                config.DamageMath.Calculate = ConfigData.EDamageMath.Average;
            else if (rbChanceMax.Checked)
                config.DamageMath.Calculate = ConfigData.EDamageMath.Max;
            else if (rbChanceIgnore.Checked)
                config.DamageMath.Calculate = ConfigData.EDamageMath.Minimum;
            config.DisableVillainColors = false;
            config.CheckForUpdates = chkUpdates.Checked;
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
            config.TotalsWindowTitleStyle = (ConfigData.ETotalsWindowTitleStyle) cbTotalsWindowTitleOpt.SelectedIndex;
            config.UseOldTotalsWindow = chkOldStyle.Checked;
            var index = 0;
            do
            {
                config.DragDropScenarioAction[index] = defActs[index];
                ++index;
            } while (index <= 19);
            config.PreferredCurrency = (Enums.RewardCurrency) cbCurrency.SelectedIndex;
        }

        private void btnFileAssoc_Click(object sender, EventArgs e)
        {
            var assocStatus = FileAssociation.CheckAssociations();
            var ret = FileAssociation.SetAssociations();
            if (ret)
            {
                MessageBox.Show("File associations updated. Enjoy!", "Woop", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Could not update file associations.", "Boo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkShowSelfBuffsAny_CheckedChanged(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSelfBuffsAny = chkShowSelfBuffsAny.Checked;
        }
    }
}