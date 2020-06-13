using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using midsControls;

namespace Hero_Designer.Forms
{
    public partial class frmColorOptions : Form
    {
        private readonly ConfigData.FontSettings _myFs = new ConfigData.FontSettings();

        public frmColorOptions()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void BGColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorBackgroundHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorBackgroundHero = colorSelector.Color;
            //updateColours();
        }

        private void TextColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorText;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorText = colorSelector.Color;
            //updateColours();
        }

        private void InventionsColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorInvention;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorInvention = colorSelector.Color;
            //updateColours();
        }

        private void InventionsWhiteColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorInventionInv;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorInventionInv = colorSelector.Color;
            //updateColours();
        }

        private void FadedColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorFaded;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorFaded = colorSelector.Color;
            //updateColours();
        }

        private void EnhancementsColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorEnhancement;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorEnhancement = colorSelector.Color;
            //updateColours();
        }

        private void AlertColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorWarning;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorWarning = colorSelector.Color;
            //updateColours();
        }

        private void ValueColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPlName;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPlName = colorSelector.Color;
            //updateColours();
        }

        private void SpecialCaseColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPlSpecial;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPlSpecial = colorSelector.Color;
            //updateColours();
        }

        private void AvailPowerColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerAvailable;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerAvailable = colorSelector.Color;
            //updateColours();
        }

        private void UnavailPowerColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerDisabled;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerDisabled = colorSelector.Color;
            //updateColours();
        }

        private void TakenHeroColor_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenHero = colorSelector.Color;
            //updateColours();
        }

        private void DarkTakenHero_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenDarkHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenDarkHero = colorSelector.Color;
            //updateColours();
        }

        private void HighlightHero_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerHighlightHero;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerHighlightHero = colorSelector.Color;
            //updateColours();
        }

        private void TakenVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenVillain = colorSelector.Color;
            //updateColours();
        }

        private void DarkTakenVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerTakenDarkVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerTakenDarkVillain = colorSelector.Color;
            //updateColours();
        }

        private void HighlightVillain_Click(object sender, EventArgs e)
        {
            colorSelector.Color = _myFs.ColorPowerHighlightVillain;
            if (colorSelector.ShowDialog(this) == DialogResult.OK)
                _myFs.ColorPowerHighlightVillain = colorSelector.Color;
            //updateColours();
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {

        }

        private void updateColors()
        {
            ConfigData.FontSettings iFs = new ConfigData.FontSettings();
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
            iFs.Assign(MidsContext.Config.RtFont);
            MidsContext.Config.RtFont.Assign(_myFs);
            richTextBox1.BackColor = _myFs.ColorBackgroundHero;
            MidsContext.Config.RtFont.ColorBackgroundHero = _myFs.ColorPlName;
            MidsContext.Config.RtFont.ColorBackgroundVillain = _myFs.ColorPlSpecial;
            richTextBox1.Rtf = RTF.StartRTF() + RTF.Color(RTF.ElementID.Invention) + RTF.Underline("Invention Name") + RTF.Crlf() + RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic("Enhancement Text") + RTF.Color(RTF.ElementID.Warning) + " (Alert)" + RTF.Crlf() + RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "  Faded Text" + RTF.Crlf() + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Text) + "Normal Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.BackgroundVillain) + "Special Case" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Enhancement) + "Enhanced value" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Invention) + "Invention Effect" + RTF.Crlf() + RTF.EndRTF();
            /*Listlabel1.SuspendRedraw = true;
            Listlabel1.ClearItems();
            Listlabel1.AddItem(new ListLabelV3.ListLabelItemV3("Available Power", ListLabelV3.LLItemState.Enabled));
            Listlabel1.AddItem(new ListLabelV3.ListLabelItemV3("Taken Power", ListLabelV3.LLItemState.Selected));
            Listlabel1.AddItem(new ListLabelV3.ListLabelItemV3("Taken Power (Dark)", ListLabelV3.LLItemState.SelectedDisabled));
            Listlabel1.AddItem(new ListLabelV3.ListLabelItemV3("Unavailable Power", ListLabelV3.LLItemState.Disabled));
            Listlabel1.AddItem(new ListLabelV3.ListLabelItemV3("Highlight Colour", ListLabelV3.LLItemState.Enabled));
            Listlabel1.HoverColor = myFS.ColorPowerHighlight;
            Listlabel1.UpdateTextColors(ListLabelV3.LLItemState.Enabled, myFS.ColorPowerAvailable);
            Listlabel1.UpdateTextColors(ListLabelV3.LLItemState.Selected, myFS.ColorPowerTaken);
            Listlabel1.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled, myFS.ColorPowerTakenDark);
            Listlabel1.UpdateTextColors(ListLabelV3.LLItemState.Disabled, myFS.ColorPowerDisabled);
            Listlabel1.Font = new Font(Listlabel1.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            int num = Listlabel1.Items.Length - 1;
            for (int index = 0; index <= num; ++index)
                Listlabel1.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
            Listlabel1.SuspendRedraw = false;
            Listlabel1.Refresh();*/
            MidsContext.Config.RtFont.Assign(iFs);
        }
    }
}
