
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Hero_Designer
{
    public partial class frmCalcOpt : Form
    {
        readonly short[] defActs;

        private bool fcNoUpdate;
        readonly frmMain myParent;
        readonly clsOAuth clsOAuth;
        readonly string[][] scenActs;
        //public List<bool> checkStats = new List<bool>();
        private readonly List<string> useStats = new List<string>();

        readonly string[] scenarioExample;

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
            var componentResourceManager = new ComponentResourceManager(typeof(frmCalcOpt));
            optTO.Image = (Image)componentResourceManager.GetObject("optTO.Image");
            optDO.Image = (Image)componentResourceManager.GetObject("optDO.Image");
            optSO.Image = (Image)componentResourceManager.GetObject("optSO.Image");
            Label9.Text = componentResourceManager.GetString("Label9.Text");
            Label5.Text = componentResourceManager.GetString("Label5.Text");
            myTip.SetToolTip(udExHigh, componentResourceManager.GetString("udExHigh.ToolTip"));
            Label15.Text = componentResourceManager.GetString("Label15.Text");
            Icon = (Icon)componentResourceManager.GetObject("reborn_wicon");
            myParent = iParent;
        }

        void btnBaseReset_Click(object sender, EventArgs e) => udBaseToHit.Value = new Decimal(75);

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnFontColor_Click(object sender, EventArgs e)
        {
            using frmColorOptions frmColorOptions = new frmColorOptions();
            frmColorOptions.ShowDialog();
        }

        void btnIOReset_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character == null)
                return;
            int int32 = Convert.ToInt32(udIOLevel.Value);
            MidsContext.Character.CurrentBuild.SetIOLevels(int32, false, false);
            myParent.ChildRequestedRedraw();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            var cCount = groupBox21.Controls.OfType<CheckBox>().Count(c => c.Checked);
            if ((cCount > 0 && cCount < 3) || cCount > 6)
            {
                MessageBox.Show(@"Incorrect amount of stats selected, please go back and make sure you have 3 to 6 stats selected.",
                    @"Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Abort;
            }
            else
            {
                DialogResult = DialogResult.OK;
                StoreControls();
                myParent.DoCalcOptUpdates();
                Hide();
            }
        }

        void btnSaveFolder_Click(object sender, EventArgs e)
        {
            fbdSave.SelectedPath = lblSaveFolder.Text;
            if (fbdSave.ShowDialog() != DialogResult.OK)
                return;
            lblSaveFolder.Text = fbdSave.SelectedPath;
        }

        void btnSaveFolderReset_Click(object sender, EventArgs e)
        {
            MidsContext.Config.CreateDefaultSaveFolder();
            MidsContext.Config.DefaultSaveFolderOverride = null;
            lblSaveFolder.Text = MidsContext.Config.GetSaveFolder();
        }

        //void btnUpdatePathReset_Click(object sender, EventArgs e) => this.txtUpdatePath.Text = "http://repo.cohtitan.com/mids_updates/";

        void clbSuppression_SelectedIndexChanged(object sender, EventArgs e)
        {
            int[] values = (int[])Enum.GetValues(MidsContext.Config.Suppression.GetType());
            MidsContext.Config.Suppression = Enums.eSuppress.None;
            int num = clbSuppression.CheckedIndices.Count - 1;
            for (int index = 0; index <= num; ++index)
                MidsContext.Config.Suppression += values[clbSuppression.CheckedIndices[index]];
        }

        void cmbAction_SelectedIndexChanged(object sender, EventArgs e) => defActs[listScenarios.SelectedIndex] = (short)cmbAction.SelectedIndex;

        void csAdd_Click(object sender, EventArgs e)
        {
            MidsContext.Config.Export.AddScheme();
            csPopulateList(MidsContext.Config.Export.ColorSchemes.Length - 1);
        }

        void csBtnEdit_Click(object sender, EventArgs e)
        {
            if (csList.Items.Count <= 0)
                return;
            frmExportColor frmExportColor = new frmExportColor(ref MidsContext.Config.Export.ColorSchemes[csList.SelectedIndex]);
            if (frmExportColor.ShowDialog() == DialogResult.OK)
            {
                MidsContext.Config.Export.ColorSchemes[csList.SelectedIndex].Assign(frmExportColor.myScheme);
                csPopulateList();
            }
            BringToFront();
        }

        void csDelete_Click(object sender, EventArgs e)
        {
            if (csList.Items.Count <= 0 || Interaction.MsgBox("Delete " + csList.SelectedItem + "?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            MidsContext.Config.Export.RemoveScheme(csList.SelectedIndex);
            csPopulateList();
        }

        void csList_KeyPress(object sender, KeyPressEventArgs e)
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

        void csPopulateList(int HighlightID = -1)
        {
            csList.Items.Clear();
            ExportConfig export = MidsContext.Config.Export;
            int num = export.ColorSchemes.Length - 1;
            for (int index = 0; index <= num; ++index)
                csList.Items.Add(export.ColorSchemes[index].SchemeName);
            if (csList.Items.Count > 0 & HighlightID == -1)
                csList.SelectedIndex = 0;
            if (!(HighlightID < csList.Items.Count & HighlightID > -1))
                return;
            csList.SelectedIndex = HighlightID;
        }

        void csReset_Click(object sender, EventArgs e)
        {
            if (Interaction.MsgBox("This will remove all of the color schemes and replace them with the defaults. Are you sure?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            MidsContext.Config.Export.ResetColorsToDefaults();
            csPopulateList();
        }

        void fcAdd_Click(object sender, EventArgs e)
        {
            MidsContext.Config.Export.AddCodes();
            fcPopulateList(MidsContext.Config.Export.FormatCode.Length - 1);
        }

        void fcBoldOff_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].BoldOff = fcBoldOff.Text;
        }

        void fcBoldOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].BoldOn = fcBoldOn.Text;
        }

        void fcColorOff_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ColorOff = fcColorOff.Text;
        }

        void fcColorOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 || fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ColorOn = fcColorOn.Text;
        }

        void fcDelete_Click(object sender, EventArgs e)
        {
            if (fcList.Items.Count <= 0 || Interaction.MsgBox("Delete " + fcList.SelectedItem + "?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            MidsContext.Config.Export.RemoveCodes(fcList.SelectedIndex);
            fcPopulateList();
        }

        void fcDisplay()
        {
            fcNoUpdate = true;
            if (fcList.SelectedIndex > -1)
            {
                ExportConfig.FormatCodes[] formatCode = MidsContext.Config.Export.FormatCode;
                int selectedIndex = fcList.SelectedIndex;
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

        void fcItalicOff_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ItalicOff = fcItalicOff.Text;
        }

        void fcItalicOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 || fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].ItalicOn = fcItalicOn.Text;
        }

        void fcList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToString(e.KeyChar) == "[")
                forumCodeUp();
            else
            {
                if (Convert.ToString(e.KeyChar) != "]")
                    return;
                ForumCodeDown();
            }
        }

        void fcList_SelectedIndexChanged(object sender, EventArgs e) => fcDisplay();

        void dcExList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dcExList.SelectedIndex != -1 && !string.IsNullOrWhiteSpace(dcExList.SelectedItem.ToString()))
            {
                MidsContext.Config.DSelServer = dcExList.SelectedItem.ToString();
            }
        }

        /*void dcAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dcServerName.Text))
                return;
            dcExList.Items.Add(dcServerName.Text);
            if (!MidsContext.Config.DServers.Contains(dcServerName.Text))
                MidsContext.Config.DServers.Add(dcServerName.Text);
            MidsContext.Config.DSelServer = dcServerName.Text;
            dcExList.SelectedItem = MidsContext.Config.DSelServer;
        }

        void dcRemove_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config.DSelServer == dcExList.SelectedItem.ToString() && !string.IsNullOrEmpty(dcExList.SelectedItem.ToString()))
            {
                MidsContext.Config.DSelServer = "";
            }
            if (MidsContext.Config.DServers.Contains(dcExList.SelectedItem.ToString()))
            {
                MidsContext.Config.DServers.Remove(dcExList.SelectedItem.ToString());
            }
            dcExList.Items.Remove(dcExList.SelectedItem);

        }

        void dcNickName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(dcNickName.Text))
            {
                MidsContext.Config.DNickName = dcNickName.Text;
            }
        }

        void dcChannel_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(dcChannel.Text))
            {
                MidsContext.Config.DChannel = dcChannel.Text;
            }
        }*/

        private void InvBot_Click(object sender, EventArgs e)
        {
            var botLink = clsDiscord.ShrinkTheDatalink("https://discordapp.com/api/oauth2/authorize?client_id=593333282234695701&permissions=18432&redirect_uri=https%3A%2F%2Fmidsreborn.com&scope=bot");
            Process.Start(botLink);
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox box in groupBox21.Controls.OfType<CheckBox>())
            {
                var boxNum = box.Name.Replace("checkBox", "");
                var eVal = (Enums.eStats) Convert.ToInt32(boxNum);
                if (box.Checked)
                {
                    if (!useStats.Contains(eVal.ToString()))
                    {
                        useStats.Add(eVal.ToString());
                    }
                }
                else
                {
                    if (useStats.Contains(eVal.ToString()))
                    {
                        useStats.Remove(eVal.ToString());
                    }
                }
            }
        }

        void fcNotes_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Notes = fcNotes.Text;
        }

        void fcPopulateList(int HighlightID = -1)
        {
            fcList.Items.Clear();
            ExportConfig export = MidsContext.Config.Export;
            int num = export.FormatCode.Length - 1;
            for (int index = 0; index <= num; ++index)
                fcList.Items.Add(export.FormatCode[index].Name);
            if (fcList.Items.Count > 0 & HighlightID == -1)
                fcList.SelectedIndex = 0;
            if (!(HighlightID < fcList.Items.Count & HighlightID > -1))
                return;
            fcList.SelectedIndex = HighlightID;
        }

        void fcReset_Click(object sender, EventArgs e)
        {
            if (Interaction.MsgBox("This will remove all of the formatting code sets and replace them with the default set. Are you sure?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            MidsContext.Config.Export.ResetCodesToDefaults();
            fcPopulateList();
        }

        void fcSet_Click(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Name = fcName.Text;
            fcPopulateList(fcList.SelectedIndex);
        }

        void fcTextOff_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].SizeOff = fcTextOff.Text;
        }

        void fcTextOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].SizeOn = fcTextOn.Text;
        }

        void fcUnderlineOff_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].UnderlineOff = fcUnderlineOff.Text;
        }

        void fcUnderlineOn_TextChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].UnderlineOn = fcUnderlineOn.Text;
        }

        void fcWSSpace_CheckedChanged(object sender, EventArgs e)
        {
            if (fcList.SelectedIndex < 0 | fcNoUpdate)
                return;
            MidsContext.Config.Export.FormatCode[fcList.SelectedIndex].Space = !fcWSSpace.Checked ? ExportConfig.WhiteSpace.Tab : ExportConfig.WhiteSpace.Space;
        }

        private void ForumCodeDown()
        {
            int selectedIndex = fcList.SelectedIndex;
            if (selectedIndex >= fcList.Items.Count - 1)
                return;
            ExportConfig.FormatCodes[] formatCodesArray = new ExportConfig.FormatCodes[2];
            formatCodesArray[0].Assign(MidsContext.Config.Export.FormatCode[selectedIndex]);
            formatCodesArray[1].Assign(MidsContext.Config.Export.FormatCode[selectedIndex + 1]);
            MidsContext.Config.Export.FormatCode[selectedIndex].Assign(formatCodesArray[1]);
            MidsContext.Config.Export.FormatCode[selectedIndex + 1].Assign(formatCodesArray[0]);
            fcPopulateList();
            if (selectedIndex + 1 > -1 & fcList.Items.Count > selectedIndex + 1)
                fcList.SelectedIndex = selectedIndex + 1;
            else if (fcList.Items.Count > 0)
                fcList.SelectedIndex = 0;
        }

        private void forumCodeUp()
        {
            int selectedIndex = fcList.SelectedIndex;
            if (selectedIndex < 1)
                return;
            ExportConfig.FormatCodes[] formatCodesArray = new ExportConfig.FormatCodes[2];
            formatCodesArray[0].Assign(MidsContext.Config.Export.FormatCode[selectedIndex]);
            formatCodesArray[1].Assign(MidsContext.Config.Export.FormatCode[selectedIndex - 1]);
            MidsContext.Config.Export.FormatCode[selectedIndex].Assign(formatCodesArray[1]);
            MidsContext.Config.Export.FormatCode[selectedIndex - 1].Assign(formatCodesArray[0]);
            fcPopulateList();
            if (selectedIndex - 1 > -1 & fcList.Items.Count > selectedIndex - 1)
                fcList.SelectedIndex = selectedIndex - 1;
            else if (fcList.Items.Count > 0)
                fcList.SelectedIndex = 0;
        }

        private void ForumColorDown()
        {
            int selectedIndex = csList.SelectedIndex;
            if (selectedIndex >= csList.Items.Count - 1)
                return;
            ExportConfig.ColorScheme[] colorSchemeArray = new ExportConfig.ColorScheme[2];
            colorSchemeArray[0].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex]);
            colorSchemeArray[1].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex + 1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex].Assign(colorSchemeArray[1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex + 1].Assign(colorSchemeArray[0]);
            csPopulateList();
            if (selectedIndex + 1 > -1 & csList.Items.Count > selectedIndex + 1)
                csList.SelectedIndex = selectedIndex + 1;
            else if (csList.Items.Count > 0)
                csList.SelectedIndex = 0;
        }

        private void forumColorUp()
        {
            int selectedIndex = csList.SelectedIndex;
            if (selectedIndex < 1)
                return;
            ExportConfig.ColorScheme[] colorSchemeArray = new ExportConfig.ColorScheme[2];
            colorSchemeArray[0].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex]);
            colorSchemeArray[1].Assign(MidsContext.Config.Export.ColorSchemes[selectedIndex - 1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex].Assign(colorSchemeArray[1]);
            MidsContext.Config.Export.ColorSchemes[selectedIndex - 1].Assign(colorSchemeArray[0]);
            csPopulateList();
            if (selectedIndex - 1 > -1 & csList.Items.Count > selectedIndex - 1)
                csList.SelectedIndex = selectedIndex - 1;
            else if (csList.Items.Count > 0)
                csList.SelectedIndex = 0;
        }

        void frmCalcOpt_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult != DialogResult.Abort)
                return;
            e.Cancel = true;
        }

        void frmCalcOpt_Load(object sender, EventArgs e)
        {
            setupScenarios();
            SetControls();
            csPopulateList();
            fcPopulateList();
            PopulateSuppression();
            MidsContext.Config.DAuth.TryGetValue("access_token", out var token);
            clsOAuth.RequestUser(token?.ToString());
            Task.Delay(1000).ContinueWith(t => PopulateUserData());
        }

        void PopulateUserData()
        {
            var userId = clsOAuth.GetCryptedValue("User", "id");
            var userName = clsOAuth.GetCryptedValue("User", "username");
            var userDiscriminator = clsOAuth.GetCryptedValue("User", "discriminator");
            var userAvatar = clsOAuth.GetCryptedValue("User", "avatar");
            using WebClient webClient = new WebClient();
            byte[] bytes = webClient.DownloadData($"https://cdn.discordapp.com/avatars/{userId}/{userAvatar}.png");
            using MemoryStream memoryStream = new MemoryStream(bytes);
            usrAvatar.Image = Image.FromStream(memoryStream);
            dcNickName.Text = $@"{userName}#{userDiscriminator}";
        }

        void listScenarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblExample.Text = scenarioExample[listScenarios.SelectedIndex];
            cmbAction.Items.Clear();
            cmbAction.Items.AddRange(scenActs[listScenarios.SelectedIndex]);
            cmbAction.SelectedIndex = defActs[listScenarios.SelectedIndex];
        }

        void optDO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optDO.Checked)
                return;
            optEnh.Text = "Dual Origin";
        }

        void optSO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optSO.Checked)
                return;
            optEnh.Text = "Single Origin";
        }

        void optTO_CheckedChanged(object sender, EventArgs e)
        {
            if (!optTO.Checked)
                return;
            optEnh.Text = "Training Origin";
        }

        // Ref: chkIOLevel
        void chkShowSOLevels_CheckedChanged(object sender, EventArgs e)
        {
        }

        void chkEnableDmgGraph_CheckedChanged(object sender, EventArgs e)
        {
            rbGraphTwoLine.Enabled = chkEnableDmgGraph.Checked;
            rbGraphStacked.Enabled = chkEnableDmgGraph.Checked;
            rbGraphSimple.Enabled = chkEnableDmgGraph.Checked;
        }

        void PopulateSuppression()
        {
            clbSuppression.BeginUpdate();
            clbSuppression.Items.Clear();
            string[] names = Enum.GetNames(MidsContext.Config.Suppression.GetType());
            int[] values = (int[])Enum.GetValues(MidsContext.Config.Suppression.GetType());
            int num = names.Length - 1;
            for (int index = 0; index <= num; ++index)
                clbSuppression.Items.Add(names[index], (MidsContext.Config.Suppression & (Enums.eSuppress)values[index]) != Enums.eSuppress.None);
            clbSuppression.EndUpdate();
        }

        void SetStatCheck(string box, bool val)
        {
            foreach (var cbox in groupBox21.Controls.OfType<CheckBox>())
            {
                if (cbox.Name == box)
                {
                    cbox.Checked = true;
                }
            }
        }
        void SetControls()
        {
            ConfigData config = MidsContext.Config;
            optSO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.SingleO;
            optDO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.DualO;
            optTO.Checked = config.CalcEnhOrigin == Enums.eEnhGrade.TrainingO;
            cbEnhLevel.SelectedIndex = (int)config.CalcEnhLevel;
            udExHigh.Value = new decimal(config.ExempHigh);
            udExLow.Value = new decimal(config.ExempLow);
            udForceLevel.Value = new decimal(config.ForceLevel);
            chkHighVis.Checked = config.EnhanceVisibility;
            rbGraphTwoLine.Checked = config.DataGraphType == Enums.eDDGraph.Both;
            rbGraphStacked.Checked = config.DataGraphType == Enums.eDDGraph.Stacked;
            rbGraphSimple.Checked = config.DataGraphType == Enums.eDDGraph.Simple;
            rbPvE.Checked = !config.Inc.DisablePvE;
            rbPvP.Checked = config.Inc.DisablePvE;
            rbChanceAverage.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Average;
            rbChanceMax.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Max;
            rbChanceIgnore.Checked = config.DamageMath.Calculate == ConfigData.EDamageMath.Minimum;
            udBaseToHit.Value = new Decimal(config.BaseAcc * 100f);
            chkVillainColor.Checked = !config.DisableVillainColors;
            chkUpdates.Checked = config.CheckForUpdates;
            chkShowSOLevels.Checked = config.ShowSOLevels;
            chkEnableDmgGraph.Checked = !config.DisableDataDamageGraph;
            rbGraphTwoLine.Enabled = chkEnableDmgGraph.Checked;
            rbGraphStacked.Enabled = chkEnableDmgGraph.Checked;
            rbGraphSimple.Enabled = chkEnableDmgGraph.Checked;
            udIOLevel.Value = decimal.Compare(new decimal(config.I9.DefaultIOLevel + 1), udIOLevel.Maximum) <= 0 ? new decimal(config.I9.DefaultIOLevel + 1) : udIOLevel.Maximum;
            chkIOLevel.Checked = !config.I9.HideIOLevels;
            chkIOEffects.Checked = !config.I9.IgnoreEnhFX;
            chkSetBonus.Checked = !config.I9.IgnoreSetBonusFX;
            chkRelSignOnly.Checked = config.ShowRelSymbols;
            chkIOPrintLevels.Checked = !config.I9.DisablePrintIOLevels;
            chkColorPrint.Checked = config.PrintInColor;
            udRTFSize.Value = new decimal(config.RtFont.RTFBase / 2.0);
            udStatSize.Value = new decimal(config.RtFont.PairedBase);
            chkTextBold.Checked = config.RtFont.RTFBold;
            chkStatBold.Checked = config.RtFont.PairedBold;
            chkLoadLastFile.Checked = !config.DisableLoadLastFileOnStart;
            dcNickName.Text = config.DNickName;
            //dcChannel.Text = config.DChannel;
            //commented out for replacement forthcoming
            /*foreach (var item in config.DServers.Append(config.DSelServer).Where(item => !string.IsNullOrWhiteSpace(item) && !dcExList.Items.Contains(config.DSelServer)).Distinct())
                dcExList.Items.Add(item);
            if (!string.IsNullOrWhiteSpace(config.DSelServer))
                dcExList.SelectedItem = config.DSelServer;*/
            lblSaveFolder.Text = config.GetSaveFolder();
            foreach (var item in config.CheckedStatBoxes)
            {
                SetStatCheck(item, true);
            }
            //this.txtUpdatePath.Text = config.UpdatePath;
            chkColorInherent.Checked = !config.DisableDesaturateInherent;
            chkMiddle.Checked = !config.DisableRepeatOnMiddleClick;
            chkNoTips.Checked = config.NoToolTips;
            chkShowAlphaPopup.Checked = !config.DisableAlphaPopup;
            chkUseArcanaTime.Checked = config.UseArcanaTime;
            cbUpdateURL.Text = MidsContext.Config.UpdatePath;
            TeamSize.Value = new decimal(config.TeamSize);
            int index = 0;
            do
            {
                defActs[index] = config.DragDropScenarioAction[index];
                ++index;
            }
            while (index <= 19);            
        }

        void setupScenarios()
        {
            scenarioExample[0] = "Swap a travel power with a power taken at level 2.";
            scenActs[0] = new[] {
                "Show dialog",
                "Cancel",
                "Move/swap power to its lowest possible level",
                "Allow power to be moved anyway (mark as invalid)"
            };
            scenarioExample[1] = "Move a Primary power from level 35 into the level 44 slot of a character with 4 epic powers.";
            scenActs[1] = new[] {
                "Show dialog",
                "Cancel",
                "Move to the last power that isn't at its min level"
            };
            scenarioExample[2] = "Power taken at level 2 with two level 3 slots is swapped with level 4, where there is a power with one slot.";
            scenActs[2] = new[] {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[3] = "A 6-slotted power taken at level 41 is moved to level 49.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[3] = new[] {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[4] = "Power taken at level 4 is swapped with power taken at level 14, which is a travel power.";
            scenActs[4] = new[] {
                "Show dialog",
                "Cancel",
                "Overwrite rather than swap",
                "Allow power to be swapped anyway (mark as invalid)"
            };
            scenarioExample[5] = "Power taken at level 8 is swapped with power taken at level 2, when the level 2 power has level 3 slots.";
            scenActs[5] = new[] {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[6] = "Pool power taken at level 49 is swapped with a 6-slotted power at level 41.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[6] = new[] {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[7] = "Power taken at level 4 is moved to level 8 when the power taken at level 6 is a pool power.\r\n(Note: If the power in the destination slot fails to shift, the 'Moved or swapped too low' scenario applies.)";
            scenActs[7] = new[] {
                "Show dialog",
                "Cancel",
                "Shift other powers around it",
                "Overwrite it; leave previous power slot empty",
                "Allow anyway (mark as invalid)"
            };
            scenarioExample[8] = "Power taken at level 8 has level 9 slots, and a power is being moved from level 12 to level 6, so the power at 8 is shifting up to 10.";
            scenActs[8] = new[] {
                "Show dialog",
                "Cancel",
                "Remove slots",
                "Mark invalid slots",
                "Swap slot levels if valid; remove invalid ones",
                "Swap slot levels if valid; mark invalid ones",
                "Rearrange all slots in build"
            };
            scenarioExample[9] = "Power taken at level 47 has 6 slots, and a power is being moved from level 49 to level 44, so the power at 47 is shifting to 49.\r\n(Note: if the remaining slots have invalid levels after impossible slots are removed, the action set for that scenario will be taken.)";
            scenActs[9] = new[] {
                "Show dialog",
                "Cancel",
                "Remove impossible slots",
                "Allow anyway (Mark slots as invalid)"
            };
            scenarioExample[10] = "Power taken at level 8 is being moved to 14, and the level 10 slot is blank.";
            scenActs[10] = new[] {
                "Show dialog",
                "Cancel",
                "Fill empty slot; don't move powers unnecessarily",
                "Shift empty slot as if it were a power"
            };
            scenarioExample[11] = "Power placed at its minimum level is being shifted up.\r\n(Note: If and only the power in the destination slot fails to shift due to this setting, the next scenario applies.)";
            scenActs[11] = new[] {
                "Show dialog",
                "Cancel",
                "Shift it along with the other powers",
                "Shift other powers around it"
            };
            scenarioExample[12] = "You chose to shift other powers around ones that are at their minimum levels, but you are moving a power in place of one that is at its minimum level. (This will never occur if you chose 'Cancel' or 'Shift it along with the other powers' from the previous scenario.)";
            scenActs[12] = new[] {
                "Show dialog",
                "Cancel",
                "Unlock and shift all level-locked powers",
                "Shift destination power to the first valid and empty slot",
                "Swap instead of move"
            };
            scenarioExample[13] = "Click and drag a level 21 slot from a level 20 power to a level 44 power.";
            scenActs[13] = new[] {
                "Show dialog",
                "Cancel",
                "Allow swap anyway (mark as invalid)"
            };
            scenarioExample[14] = "Click and drag a slot from a level 44 power to a level 20 power in place of a level 21 slot.";
            scenActs[14] = new[] {
                "Show dialog",
                "Cancel",
                "Allow swap anyway (mark as invalid)"
            };
        }

        void StoreControls()
        {
            ConfigData config = MidsContext.Config;
            if (optSO.Checked)
                config.CalcEnhOrigin = Enums.eEnhGrade.SingleO;
            else if (optDO.Checked)
                config.CalcEnhOrigin = Enums.eEnhGrade.DualO;
            else if (optTO.Checked)
                config.CalcEnhOrigin = Enums.eEnhGrade.TrainingO;
            config.CalcEnhLevel = (Enums.eEnhRelative)cbEnhLevel.SelectedIndex;
            config.ExempHigh = Convert.ToInt32(udExHigh.Value);
            config.ExempLow = Convert.ToInt32(udExLow.Value);
            if (config.ExempHigh < config.ExempLow)
                config.ExempHigh = config.ExempLow;
            config.ForceLevel = Convert.ToInt32(udForceLevel.Value);
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
            config.BaseAcc = Convert.ToSingle(decimal.Divide(udBaseToHit.Value, new decimal(100)));
            config.DisableVillainColors = !chkVillainColor.Checked;
            config.CheckForUpdates = chkUpdates.Checked;
            config.I9.DefaultIOLevel = Convert.ToInt32(udIOLevel.Value) - 1;
            config.I9.IgnoreEnhFX = !chkIOEffects.Checked;
            config.I9.IgnoreSetBonusFX = !chkSetBonus.Checked;
            config.I9.HideIOLevels = !chkIOLevel.Checked;
            config.ShowRelSymbols = chkRelSignOnly.Checked;
            config.ShowSOLevels = chkShowSOLevels.Checked;
            config.I9.DisablePrintIOLevels = !chkIOPrintLevels.Checked;
            config.PrintInColor = chkColorPrint.Checked;
            config.RtFont.RTFBase = Convert.ToInt32(decimal.Multiply(udRTFSize.Value, new decimal(2)));
            config.RtFont.PairedBase = Convert.ToSingle(udStatSize.Value);
            config.RtFont.RTFBold = chkTextBold.Checked;
            config.RtFont.PairedBold = chkStatBold.Checked;
            var checkStats = groupBox21.Controls.OfType<CheckBox>().Where(r => r.Checked).Select(r => r.Name);
            config.CheckedStatBoxes = checkStats.ToList();
            config.CheckedStats = useStats;
            config.DisableLoadLastFileOnStart = !chkLoadLastFile.Checked;
            if (config.DefaultSaveFolderOverride != lblSaveFolder.Text)
            {
                config.DefaultSaveFolderOverride = lblSaveFolder.Text;
                myParent.DlgOpen.InitialDirectory = config.DefaultSaveFolderOverride;
                myParent.DlgSave.InitialDirectory = config.DefaultSaveFolderOverride;
            }
            config.EnhanceVisibility = chkHighVis.Checked;
            //config.UpdatePath = this.txtUpdatePath.Text;
            config.DisableDesaturateInherent = !chkColorInherent.Checked;
            config.DisableRepeatOnMiddleClick = !chkMiddle.Checked;
            config.NoToolTips = chkNoTips.Checked;
            config.DisableAlphaPopup = !chkShowAlphaPopup.Checked;
            config.UseArcanaTime = chkUseArcanaTime.Checked;
            config.TeamSize = Convert.ToInt32(TeamSize.Value);
            config.UpdatePath = cbUpdateURL.Text;
            int index = 0;
            do
            {
                config.DragDropScenarioAction[index] = defActs[index];
                ++index;
            }
            while (index <= 19);
        }
    }
}