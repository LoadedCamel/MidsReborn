using System;
using System.Windows.Forms;
using FastDeepCloner;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmDv2Settings : Form
    {
        public ConfigData.DvConfig Config { get; }

        public frmDv2Settings()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            Config = MidsContext.Config.DataViewCfg.Clone();
        }

        #region Event handlers

        private void frmDv2Settings_Load(object sender, EventArgs e)
        {
            checkEndDetail.Checked = Config.Tweaks.EnduranceDetail;
            numAnimSpeed.Value = Config.Tweaks.FlipAnimationTickDelay;
            checkIgnoreConst.Checked = Config.Tweaks.GraphIgnoreConstants;

            numInfoPanelFontSize.Value = (decimal)Config.Fonts.InfoPanelSize;
            numDmgFontSize.Value = (decimal)Config.Fonts.DmgGraphSize;
            numGridItemFontSize.Value = (decimal)Config.Fonts.GridViewItemSize;
            numTotalsBarHeight.Value = Config.Fonts.TotalsBarHeight;
            numTotalsValueFontSize.Value = (decimal)Config.Fonts.TotalsItemSize;
            numGraphValueFontSize.Value = (decimal)Config.Fonts.GraphValuesSize;
            numGraphRefFontSize.Value = (decimal)Config.Fonts.GraphRefLabelSize;

            cbColorTheme.SelectedIndex = (int)Config.Colors.Theme;
            checkAutoSwitchAlignmentTheme.Checked = Config.Colors.AlignmentMode == ConfigData.ThemeAlignmentStyle.Auto;
            label14.Visible = Config.Colors.AlignmentMode != ConfigData.ThemeAlignmentStyle.Auto;
            cbThemeAlignmentOverride.Visible = Config.Colors.AlignmentMode != ConfigData.ThemeAlignmentStyle.Auto;
            cbThemeAlignmentOverride.SelectedIndex = Config.Colors.AlignmentMode == ConfigData.ThemeAlignmentStyle.Auto
                ? (int)ConfigData.ThemeAlignmentStyle.Hero
                : (int)Config.Colors.AlignmentMode - 1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DataViewCfg = Config.Clone();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkEndDetail_CheckedChanged(object sender, EventArgs e)
        {
            var target = (CheckBox)sender;

            Config.Tweaks.EnduranceDetail = target.Checked;
        }

        private void checkIgnoreConst_CheckedChanged(object sender, EventArgs e)
        {
            var target = (CheckBox)sender;

            Config.Tweaks.GraphIgnoreConstants = target.Checked;
        }

        private void numAnimSpeed_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Tweaks.FlipAnimationTickDelay = (int)target.Value;
        }

        private void numInfoPanelFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.InfoPanelSize = (float)target.Value;
        }

        private void numDmgFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.DmgGraphSize = (float)target.Value;
        }

        private void numGridItemFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.GridViewItemSize = (float)target.Value;
        }

        private void numTotalsBarHeight_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.TotalsBarHeight = (int)target.Value;
        }

        private void numTotalsValueFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.TotalsItemSize = (float)target.Value;
        }

        private void numGraphValueFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.GraphValuesSize = (float)target.Value;
        }

        private void numGraphRefFontSize_ValueChanged(object sender, EventArgs e)
        {
            var target = (NumericUpDown)sender;

            Config.Fonts.GraphRefLabelSize = (float)target.Value;
        }

        private void cbColorTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var target = (ComboBox)sender;

            Config.Colors.Theme = (ConfigData.ColorTheme)target.SelectedIndex;
        }

        private void checkAutoSwitchAlignmentTheme_CheckedChanged(object sender, EventArgs e)
        {
            var target = (CheckBox)sender;

            Config.Colors.AlignmentMode = target.Checked
                ? ConfigData.ThemeAlignmentStyle.Auto
                : (ConfigData.ThemeAlignmentStyle)(cbThemeAlignmentOverride.SelectedIndex + 1);

            label14.Visible = !target.Checked;
            cbThemeAlignmentOverride.Visible = !target.Checked;
        }

        private void cbThemeAlignmentOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            var target = (ComboBox)sender;

            Config.Colors.AlignmentMode = (ConfigData.ThemeAlignmentStyle)(target.SelectedIndex + 1);
        }

        #endregion

    }
}
