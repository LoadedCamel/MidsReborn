
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Base.Master_Classes;
using midsControls;

namespace Hero_Designer
{
    public partial class frmColourSettings : Form
    {

        ListLabelV3 Listlabel1;

        private readonly ConfigData.FontSettings myFS = new ConfigData.FontSettings();

        public frmColourSettings()
        {
            Load += frmColourSettings_Load;
            InitializeComponent();
            Listlabel1.ItemClick += Listlabel1_ItemClick;
            Name = nameof(frmColourSettings);
            var componentResourceManager = new ComponentResourceManager(typeof(frmColourSettings));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            myFS.Assign(MidsContext.Config.RtFont);
            Label20.Visible = false;
            csVillain.Visible = false;
            csVillain.Enabled = false;
        }

        void btnCancel_Click(object sender, EventArgs e)

        {
            Hide();
        }

        void btnOK_Click(object sender, EventArgs e)

        {
            MidsContext.Config.RtFont.Assign(myFS);
            Hide();
        }

        void btnReset_Click(object sender, EventArgs e)

        {
            myFS.SetDefault();
            updateColours();
        }

        void csAlert_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorWarning;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorWarning = cPicker.Color;
            updateColours();
        }

        void csEnh_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorEnhancement;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorEnhancement = cPicker.Color;
            updateColours();
        }

        void csFade_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorFaded;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorFaded = cPicker.Color;
            updateColours();
        }

        void csHero_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorBackgroundHero;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorBackgroundHero = cPicker.Color;
            updateColours();
        }

        void csInv_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorInvention;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorInvention = cPicker.Color;
            updateColours();
        }

        void csInvInv_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorInventionInv;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorInventionInv = cPicker.Color;
            updateColours();
        }

        void csSpecial_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorPlSpecial;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorPlSpecial = cPicker.Color;
            updateColours();
        }

        void csText_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorText;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorText = cPicker.Color;
            updateColours();
        }

        void csValue_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorPlName;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorPlName = cPicker.Color;
            updateColours();
        }

        void csVillain_Click(object sender, EventArgs e)

        {
            cPicker.Color = myFS.ColorBackgroundVillain;
            if (cPicker.ShowDialog(this) == DialogResult.OK)
                myFS.ColorBackgroundVillain = cPicker.Color;
            updateColours();
        }

        void frmColourSettings_Load(object sender, EventArgs e)

        {
            updateColours();
        }

        [DebuggerStepThrough]

        void Listlabel1_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)

        {
            switch (Item.Index)
            {
                case 0:
                    cPicker.Color = myFS.ColorPowerAvailable;
                    if (cPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        myFS.ColorPowerAvailable = cPicker.Color;
                    }
                    break;
                case 1:
                    cPicker.Color = myFS.ColorPowerTaken;
                    if (cPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        myFS.ColorPowerTaken = cPicker.Color;
                    }
                    break;
                case 2:
                    cPicker.Color = myFS.ColorPowerTakenDark;
                    if (cPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        myFS.ColorPowerTakenDark = cPicker.Color;
                    }
                    break;
                case 3:
                    cPicker.Color = myFS.ColorPowerDisabled;
                    if (cPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        myFS.ColorPowerDisabled = cPicker.Color;
                    }
                    break;
                case 4:
                    cPicker.Color = myFS.ColorPowerHighlight;
                    if (cPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        myFS.ColorPowerHighlight = cPicker.Color;
                    }
                    break;
            }
            updateColours();
        }

        private void updateColours()
        {
            ConfigData.FontSettings iFs = new ConfigData.FontSettings();
            csHero.BackColor = myFS.ColorBackgroundHero;
            csVillain.BackColor = myFS.ColorBackgroundVillain;
            csText.BackColor = myFS.ColorText;
            csInv.BackColor = myFS.ColorInvention;
            csInvInv.BackColor = myFS.ColorInventionInv;
            csFade.BackColor = myFS.ColorFaded;
            csEnh.BackColor = myFS.ColorEnhancement;
            csAlert.BackColor = myFS.ColorWarning;
            csValue.BackColor = myFS.ColorPlName;
            csSpecial.BackColor = myFS.ColorPlSpecial;
            iFs.Assign(MidsContext.Config.RtFont);
            MidsContext.Config.RtFont.Assign(myFS);
            rtPreview.BackColor = myFS.ColorBackgroundHero;
            MidsContext.Config.RtFont.ColorBackgroundHero = myFS.ColorPlName;
            MidsContext.Config.RtFont.ColorBackgroundVillain = myFS.ColorPlSpecial;
            rtPreview.Rtf = RTF.StartRTF() + RTF.Color(RTF.ElementID.Invention) + RTF.Underline("Invention Name") + RTF.Crlf() + RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic("Enhancement Text") + RTF.Color(RTF.ElementID.Warning) + " (Alert)" + RTF.Crlf() + RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.Text) + "  Regular Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "  Faded Text" + RTF.Crlf() + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Text) + "Normal Text" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.BackgroundVillain) + "Special Case" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Enhancement) + "Enahnced value" + RTF.Crlf() + RTF.Color(RTF.ElementID.BackgroundHero) + RTF.Bold("Value Name: ") + RTF.Color(RTF.ElementID.Invention) + "Invention Effect" + RTF.Crlf() + RTF.EndRTF();
            Listlabel1.SuspendRedraw = true;
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
            Listlabel1.Refresh();
            MidsContext.Config.RtFont.Assign(iFs);
        }
    }
}