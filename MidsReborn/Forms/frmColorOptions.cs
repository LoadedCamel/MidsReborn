using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmColorOptions : Form
    {
        private readonly ConfigData.FontSettings _myFs = new ConfigData.FontSettings();

        public frmColorOptions()
        {
            Load += ColorOptions_OnLoad;
            InitializeComponent();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw,
                true);
            Name = nameof(frmColorOptions);
            var componentResourceManager = new ComponentResourceManager(typeof(frmColorOptions));
            Icon = Resources.reborn;
            _myFs.Assign(MidsContext.Config.RtFont);
        }

        private void ColorOptions_OnLoad(object sender, EventArgs e)
        {
            updateColors();
        }

        private void BGColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorBackgroundHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorBackgroundHero = colorSelector.Color;
            updateColors();
        }

        private void TextColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorText;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorText = colorSelector.Color;
            updateColors();
        }

        private void InventionsColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorInvention;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorInvention = colorSelector.Color;
            updateColors();
        }

        private void InventionsWhiteColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorInventionInv;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorInventionInv = colorSelector.Color;
            updateColors();
        }

        private void FadedColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorFaded;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorFaded = colorSelector.Color;
            updateColors();
        }

        private void EnhancementsColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorEnhancement;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorEnhancement = colorSelector.Color;
            updateColors();
        }

        private void AlertColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorWarning;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorWarning = colorSelector.Color;
            updateColors();
        }

        private void ValueColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPlName;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPlName = colorSelector.Color;
            updateColors();
        }

        private void SpecialCaseColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPlSpecial;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPlSpecial = colorSelector.Color;
            updateColors();
        }

        private void AvailPowerColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerAvailable;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerAvailable = colorSelector.Color;
            updateColors();
        }

        private void UnavailPowerColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerDisabled;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerDisabled = colorSelector.Color;
            updateColors();
        }

        private void TakenHeroColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenHero = colorSelector.Color;
            updateColors();
        }

        private void DarkTakenHero_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenDarkHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenDarkHero = colorSelector.Color;
            updateColors();
        }

        private void HighlightHero_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerHighlightHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerHighlightHero = colorSelector.Color;
            updateColors();
        }

        private void TakenVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenVillain = colorSelector.Color;
            updateColors();
        }

        private void DarkTakenVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenDarkVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenDarkVillain = colorSelector.Color;
            updateColors();
        }

        private void HighlightVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerHighlightVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerHighlightVillain = colorSelector.Color;
            updateColors();
        }

        private void baseDamagebar_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorDamageBarBase;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorDamageBarBase = colorSelector.Color;
            updateColors();
        }

        private void enhDamagebar_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorDamageBarEnh;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorDamageBarEnh = colorSelector.Color;
            updateColors();
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            MidsContext.Config.RtFont.Assign(_myFs);
            Hide();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            _myFs.SetDefault();
            updateColors();
        }

        private void updateColors()
        {
            var iFs = new ConfigData.FontSettings();
            BGColor.BackColor = _myFs.ColorBackgroundHero;
            //csVillain.BackColor = _myFs.ColorBackgroundVillain;
            TextColor.BackColor = _myFs.ColorText;
            InventionsColor.BackColor = _myFs.ColorInvention;
            InventionsWhiteColor.BackColor = _myFs.ColorInventionInv;
            FadedColor.BackColor = _myFs.ColorFaded;
            EnhancementsColor.BackColor = _myFs.ColorEnhancement;
            AlertColor.BackColor = _myFs.ColorWarning;
            ValueColor.BackColor = _myFs.ColorPlName;
            SpecialCaseColor.BackColor = _myFs.ColorPlSpecial;
            AvailPowerColor.BackColor = _myFs.ColorPowerAvailable;
            UnavailPowerColor.BackColor = _myFs.ColorPowerDisabled;
            TakenHeroColor.BackColor = _myFs.ColorPowerTakenHero;
            DarkTakenHero.BackColor = _myFs.ColorPowerTakenDarkHero;
            HighlightHero.BackColor = _myFs.ColorPowerHighlightHero;
            TakenVillain.BackColor = _myFs.ColorPowerTakenVillain;
            DarkTakenVillain.BackColor = _myFs.ColorPowerTakenDarkVillain;
            HighlightVillain.BackColor = _myFs.ColorPowerHighlightVillain;
            iFs.Assign(MidsContext.Config.RtFont);
            MidsContext.Config.RtFont.Assign(_myFs);
            richTextBox1.BackColor = _myFs.ColorBackgroundHero;
            ctlColorList1.BackColor = _myFs.ColorBackgroundHero;
            baseDamagebar.BackColor = _myFs.ColorDamageBarBase;
            enhDamagebar.BackColor = _myFs.ColorDamageBarEnh;
            MidsContext.Config.RtFont.ColorBackgroundHero = _myFs.ColorPlName;
            MidsContext.Config.RtFont.ColorBackgroundVillain = _myFs.ColorPlSpecial;
            richTextBox1.Rtf = RTF.StartRTF() + RTF.Color(RTF.ElementID.Invention) + RTF.Underline("Invention Name") +
                               RTF.Crlf() + RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic("Enhancement Text") +
                               RTF.Color(RTF.ElementID.Warning) + " (Alert)" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.Faded) + "  Faded Text" + RTF.Crlf() + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") +
                               RTF.Color(RTF.ElementID.Text) + "Normal Text" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") +
                               RTF.Color(RTF.ElementID.BackgroundVillain) + "Special Case" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") +
                               RTF.Color(RTF.ElementID.Enhancement) + "Enhanced value" + RTF.Crlf() +
                               RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") +
                               RTF.Color(RTF.ElementID.Invention) + "Invention Effect" + RTF.Crlf() + RTF.EndRTF();
            _myFs.ColorList = new List<Color>
            {
                _myFs.ColorPowerTakenHero, _myFs.ColorPowerTakenDarkHero, _myFs.ColorPowerHighlightHero,
                _myFs.ColorPowerTakenVillain, _myFs.ColorPowerTakenDarkVillain, _myFs.ColorPowerHighlightVillain
            };
            ctlColorList1.Colors = _myFs.ColorList;
            ctlColorList1.Items.Clear();
            var powerColors = new List<string>
            {
                "Power Taken (Hero)", "Power Taken Dark (Hero)", "Power Highlight (Hero)", "Power Taken (Villain)",
                "Power Taken Dark (Villain)", "Power Highlight (Villain)"
            };
            foreach (var itemString in powerColors) ctlColorList1.Items.Add(itemString);
            ctlColorList1.Invalidate();
            MidsContext.Config.RtFont.Assign(iFs);
        }
    }
}