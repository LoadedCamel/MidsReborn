using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using midsControls;

namespace Hero_Designer
{
    public partial class frmForum : Form
    {
        private clsOutput Exporter;
        Point mouse_offset;

        internal ImageButton IBCancel { get; private set; }

        internal ImageButton IBExport { get; private set; }

        public frmForum()
        {
            Load += frmForum_Load;
            MouseDown += frmForum_MouseDown;
            MouseMove += frmForum_MouseMove;
            Paint += frmForum_Paint;
            InitializeComponent();
            Name = nameof(frmForum);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmForum));
            pbTitle.Image = (Image)componentResourceManager.GetObject("pbTitle.Image");
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        void csList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (csList.SelectedIndex > -1)
            {
                ExportConfig.ColorScheme[] colorSchemes = MidsContext.Config.Export.ColorSchemes;
                int selectedIndex = csList.SelectedIndex;
                csHeading.BackColor = colorSchemes[selectedIndex].Heading;
                csLevel.BackColor = colorSchemes[selectedIndex].Level;
                csSlots.BackColor = colorSchemes[selectedIndex].Slots;
                csTitle.BackColor = colorSchemes[selectedIndex].Title;
            }
            else
            {
                csHeading.BackColor = Color.Navy;
                csLevel.BackColor = Color.DarkSlateBlue;
                csSlots.BackColor = Color.DarkSlateBlue;
                csTitle.BackColor = Color.MediumBlue;
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

        void frmForum_Load(object sender, EventArgs e)
        {
            pbTitle.Left = (int)Math.Round((Width - pbTitle.Width) / 2.0);
            if (MidsContext.Config.Export.ColorSchemes.Length < 1)
            {
                MidsContext.Config.Export.ColorSchemes = new ExportConfig.ColorScheme[1];
                ExportConfig.ColorScheme[] colorSchemes = MidsContext.Config.Export.ColorSchemes;
                int index = 0;
                colorSchemes[index].SchemeName = "Navy";
                colorSchemes[index].Title = Color.MediumBlue;
                colorSchemes[index].Heading = Color.Navy;
                colorSchemes[index].Level = Color.DarkSlateBlue;
                colorSchemes[index].Slots = Color.DarkSlateBlue;
            }
            csPopulateList(MidsContext.Config.ExportScheme);
            if (MidsContext.Config.Export.FormatCode.Length < 1)
            {
                MidsContext.Config.Export.FormatCode = new ExportConfig.FormatCodes[1];
                ExportConfig.FormatCodes[] formatCode = MidsContext.Config.Export.FormatCode;
                int index = 0;
                formatCode[index].Name = "No BBCode";
                formatCode[index].Notes = "Unable to load codes data! Using plain-text mode.";
                formatCode[index].Space = ExportConfig.WhiteSpace.Tab;
            }
            int num = MidsContext.Config.Export.FormatCode.Length - 1;
            for (int index = 0; index <= num; ++index)
                lstCodes.Items.Add(MidsContext.Config.Export.FormatCode[index].Name);
            if (MidsContext.Config.ExportTarget > -1 & MidsContext.Config.ExportTarget < lstCodes.Items.Count)
                lstCodes.SelectedIndex = MidsContext.Config.ExportTarget;
            else
                lstCodes.SelectedIndex = 0;
            lblCodeInf.Text = MidsContext.Config.Export.FormatCode[lstCodes.SelectedIndex].Notes;
            SetTips();
            ConfigData config = MidsContext.Config;
            chkDataChunk.Checked = !config.I9.DisableExportDataChunk;
            chkNoIOLevel.Checked = !config.I9.ExportIOLevels;
            chkNoSetName.Checked = config.I9.ExportStripSetNames;
            chkNoEnh.Checked = config.I9.ExportStripEnh;
            chkBonusList.Checked = MidsContext.Config.ExportBonusTotals;
            chkBreakdown.Checked = MidsContext.Config.ExportBonusList;
            chkChunkOnly.Checked = MidsContext.Config.ExportChunkOnly;
        }

        void frmForum_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        void frmForum_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Point mousePosition = MousePosition;
            mousePosition.Offset(mouse_offset.X, mouse_offset.Y);
            Location = mousePosition;
        }

        void frmForum_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1f);
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(pen, rect);
        }

        void ibCancel_ButtonClicked()
        {
            Hide();
        }

        void ibExport_ButtonClicked()
        {
            MidsContext.Config.ExportScheme = csList.SelectedIndex;
            MidsContext.Config.ExportTarget = lstCodes.SelectedIndex;
            MidsContext.Config.DisableExportHex = false;
            ConfigData config = MidsContext.Config;
            config.I9.DisableExportDataChunk = !chkDataChunk.Checked;
            config.I9.ExportIOLevels = !chkNoIOLevel.Checked;
            config.I9.ExportStripSetNames = chkNoSetName.Checked;
            config.I9.ExportStripEnh = chkNoEnh.Checked;
            MidsContext.Config.ExportBonusTotals = chkBonusList.Checked;
            MidsContext.Config.ExportBonusList = chkBreakdown.Checked;
            MidsContext.Config.ExportChunkOnly = chkChunkOnly.Checked;
            Exporter = new clsOutput();
            string str1 = "";
            //this creates the data link
            string iDataLink = MidsCharacterFileFormat.MxDBuildSaveHyperlink(!Exporter.HTML);
            if (!chkChunkOnly.Checked)
                str1 = Exporter.Build(iDataLink);

            string str2 = ExportConfig.FormatCodes.FillCode(MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].SizeOn, "5");
            string sizeOff = MidsContext.Config.Export.FormatCode[MidsContext.Config.ExportTarget].SizeOff;
            if (!chkChunkOnly.Checked)
                str1 += "\r\n\r\n";
            if (iDataLink == "" | chkAlwaysDataChunk.Checked | chkChunkOnly.Checked && chkDataChunk.Checked | chkAlwaysDataChunk.Checked | chkChunkOnly.Checked)
                str1 = str1 + str2 + MidsCharacterFileFormat.MxDBuildSaveString(false, true) + sizeOff;
            Clipboard.SetDataObject(str1, true);
            int num = (int)Interaction.MsgBox("The build data has been placed on the clipboard and is ready to paste into a forum post. If your forum allows you to disable emoticons/smileys in your post, you should do so.", MsgBoxStyle.Information, "Export Done");
            Hide();
        }

        void lstCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCodeInf.Text = MidsContext.Config.Export.FormatCode[lstCodes.SelectedIndex].Notes;
        }

        void pbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-pbTitle.Left + -e.X, -pbTitle.Top + -e.Y);
        }

        void pbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            frmForum_MouseMove(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void SetTips()
        {
            ToolTip1.SetToolTip(chkDataChunk, "Enable this to include a data chunk which can be copied by other forum users and imported into the Hero Designer.\r\nIf your build contains Inventions or Invention Sets, you should enable this option, as the import filter\r\ncan't interpret those from the human-readable part of a build post.");
        }
    }
}