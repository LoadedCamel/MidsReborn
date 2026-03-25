using ABI.Windows.UI;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;
using MRBResourceLib;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmTemp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SkListItem skListItem1 = new SkListItem();
            Panel1 = new Panel();
            VScrollBar1 = new VScrollBar();
            PopInfo = new ctlPopUp();
            Panel2 = new Panel();
            SkPairedList1 = new SkPairedList();
            lblLock = new Label();
            ibClose = new ImageButtonEx();
            BtnTypeAll = new ImageButtonEx();
            BtnTypeEncounters = new ImageButtonEx();
            BtnTypeBaseBuffs = new ImageButtonEx();
            BtnTypeDayJobs = new ImageButtonEx();
            BtnTypeMisc = new ImageButtonEx();
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();
            SuspendLayout();
            // 
            // Panel1
            // 
            Panel1.BorderStyle = BorderStyle.Fixed3D;
            Panel1.Controls.Add(VScrollBar1);
            Panel1.Controls.Add(PopInfo);
            Panel1.Location = new System.Drawing.Point(12, 244);
            Panel1.Name = "Panel1";
            Panel1.Size = new System.Drawing.Size(414, 189);
            Panel1.TabIndex = 35;
            // 
            // VScrollBar1
            // 
            VScrollBar1.Location = new System.Drawing.Point(393, 0);
            VScrollBar1.Name = "VScrollBar1";
            VScrollBar1.Size = new System.Drawing.Size(17, 185);
            VScrollBar1.TabIndex = 11;
            VScrollBar1.Scroll += VScrollBar1_Scroll;
            // 
            // PopInfo
            // 
            PopInfo.ColumnPosition = 0.5F;
            PopInfo.ColumnRight = false;
            PopInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            PopInfo.ForeColor = System.Drawing.Color.FromArgb(0, 0, 32);
            PopInfo.InternalPadding = 3;
            PopInfo.Location = new System.Drawing.Point(0, 0);
            PopInfo.Margin = new Padding(4, 3, 4, 3);
            PopInfo.Name = "PopInfo";
            PopInfo.ScrollY = 0F;
            PopInfo.SectionPadding = 8;
            PopInfo.Size = new System.Drawing.Size(391, 204);
            PopInfo.TabIndex = 9;
            PopInfo.MouseEnter += PopInfo_MouseEnter;
            PopInfo.MouseWheel += PopInfo_MouseWheel;
            // 
            // Panel2
            // 
            Panel2.BorderStyle = BorderStyle.Fixed3D;
            Panel2.Controls.Add(SkPairedList1);
            Panel2.Location = new System.Drawing.Point(12, 82);
            Panel2.Name = "Panel2";
            Panel2.Size = new System.Drawing.Size(414, 140);
            Panel2.TabIndex = 126;
            Panel2.TabStop = true;
            // 
            // SkPairedList1
            // 
            SkPairedList1.ActualLineHeight = 8;
            SkPairedList1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            SkPairedList1.APIVersion = new System.Version(3, 3, 0, 0);
            SkPairedList1.AutoColumns = false;
            SkPairedList1.Columns = 2;
            SkPairedList1.Dock = DockStyle.Fill;
            SkPairedList1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            SkPairedList1.HighVis = true;
            SkPairedList1.HoverColor = System.Drawing.Color.WhiteSmoke;
            SkPairedList1.IsEventDriven = true;
            SkPairedList1.Location = new System.Drawing.Point(0, 0);
            SkPairedList1.MinColumnWidth = 180;
            SkPairedList1.Name = "SkPairedList1";
            SkPairedList1.PaddingX = 4;
            SkPairedList1.PaddingY = 1;
            SkPairedList1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            SkPairedList1.Scrollable = true;
            SkPairedList1.ScrollBarColor = System.Drawing.Color.FromArgb(128, 96, 192);
            SkPairedList1.ScrollBarWidth = 11;
            SkPairedList1.ScrollButtonColor = System.Drawing.Color.FromArgb(96, 0, 192);
            SkPairedList1.SelectedIndex = -1;
            skListItem1.Bold = false;
            skListItem1.FontFlags = EFontFlags.Normal;
            skListItem1.IdxPower = -1;
            skListItem1.Index = -1;
            skListItem1.Italic = false;
            skListItem1.ItemHeight = 1;
            skListItem1.ItemState = EItemState.Enabled;
            skListItem1.LineCount = 1;
            skListItem1.NIdPower = -1;
            skListItem1.NIdSet = -1;
            skListItem1.Strikethrough = false;
            skListItem1.Tag = "";
            skListItem1.Text = "";
            skListItem1.TextAlign = ETextAlign.Left;
            skListItem1.Underline = false;
            skListItem1.WrappedText = "";
            SkPairedList1.SelectedItem = skListItem1;
            SkPairedList1.SharedContext = null;
            SkPairedList1.Size = new System.Drawing.Size(410, 136);
            SkPairedList1.SuspendRedraw = false;
            SkPairedList1.TabIndex = 0;
            SkPairedList1.ItemClick += SkPairedList1_ItemClick;
            SkPairedList1.ItemHover += SkPairedList1_ItemHover;
            SkPairedList1.EmptyHover += SkPairedList1_EmptyHover;
            SkPairedList1.MouseLeave += SkPairedList1_MouseLeave;
            // 
            // lblLock
            // 
            lblLock.BackColor = System.Drawing.Color.Red;
            lblLock.BorderStyle = BorderStyle.FixedSingle;
            lblLock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
            lblLock.ForeColor = System.Drawing.Color.White;
            lblLock.Location = new System.Drawing.Point(12, 225);
            lblLock.Name = "lblLock";
            lblLock.Size = new System.Drawing.Size(56, 16);
            lblLock.TabIndex = 69;
            lblLock.Text = "[Unlock]";
            lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblLock.Visible = false;
            lblLock.Click += lblLock_Click;
            // 
            // ibClose
            // 
            ibClose.BackgroundImageLayout = ImageLayout.None;
            ibClose.CurrentText = "Done";
            ibClose.DisplayVertically = false;
            ibClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            ibClose.Images.Background = Resources.HeroButton;
            ibClose.Images.Hover = Resources.HeroButtonHover;
            ibClose.ImagesAlt.Background = Resources.VillainButton;
            ibClose.ImagesAlt.Hover = Resources.VillainButtonHover;
            ibClose.ImagesDis.Flat = Resources.DisabledButtonFlat;
            ibClose.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            ibClose.Location = new System.Drawing.Point(165, 436);
            ibClose.Lock = false;
            ibClose.Margin = new Padding(3, 4, 3, 4);
            ibClose.Name = "ibClose";
            ibClose.Size = new System.Drawing.Size(107, 28);
            ibClose.TabIndex = 7;
            ibClose.Text = "Done";
            ibClose.TextOutline.Color = System.Drawing.Color.Black;
            ibClose.TextOutline.Width = 3;
            ibClose.ToggleState = ImageButtonEx.States.ToggledOff;
            ibClose.ToggleText.Indeterminate = "Indeterminate State";
            ibClose.ToggleText.ToggledOff = "ToggledOff State";
            ibClose.ToggleText.ToggledOn = "ToggledOn State";
            ibClose.UseAlt = false;
            ibClose.Click += ibClose_Click;
            // 
            // BtnTypeAll
            // 
            BtnTypeAll.BackgroundImageLayout = ImageLayout.None;
            BtnTypeAll.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnTypeAll.CurrentText = "ToggledOff State";
            BtnTypeAll.DisplayVertically = false;
            BtnTypeAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            BtnTypeAll.Images.Background = Resources.HeroButton;
            BtnTypeAll.Images.Hover = Resources.HeroButtonHover;
            BtnTypeAll.ImagesAlt.Background = Resources.VillainButton;
            BtnTypeAll.ImagesAlt.Hover = Resources.VillainButtonHover;
            BtnTypeAll.ImagesDis.Flat = Resources.DisabledButtonFlat;
            BtnTypeAll.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            BtnTypeAll.Location = new System.Drawing.Point(36, 30);
            BtnTypeAll.Lock = false;
            BtnTypeAll.Margin = new Padding(3, 4, 3, 4);
            BtnTypeAll.Name = "BtnTypeAll";
            BtnTypeAll.Size = new System.Drawing.Size(105, 22);
            BtnTypeAll.TabIndex = 127;
            BtnTypeAll.Text = "All";
            BtnTypeAll.TextOutline.Color = System.Drawing.Color.Black;
            BtnTypeAll.TextOutline.Width = 3;
            BtnTypeAll.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnTypeAll.ToggleText.Indeterminate = "All";
            BtnTypeAll.ToggleText.ToggledOff = "All";
            BtnTypeAll.ToggleText.ToggledOn = "All";
            BtnTypeAll.UseAlt = false;
            BtnTypeAll.Click += BtnTypeAll_Click;
            // 
            // BtnTypeEncounters
            // 
            BtnTypeEncounters.BackgroundImageLayout = ImageLayout.None;
            BtnTypeEncounters.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnTypeEncounters.CurrentText = "ToggledOff State";
            BtnTypeEncounters.DisplayVertically = false;
            BtnTypeEncounters.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            BtnTypeEncounters.Images.Background = Resources.HeroButton;
            BtnTypeEncounters.Images.Hover = Resources.HeroButtonHover;
            BtnTypeEncounters.ImagesAlt.Background = Resources.VillainButton;
            BtnTypeEncounters.ImagesAlt.Hover = Resources.VillainButtonHover;
            BtnTypeEncounters.ImagesDis.Flat = Resources.DisabledButtonFlat;
            BtnTypeEncounters.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            BtnTypeEncounters.Location = new System.Drawing.Point(174, 13);
            BtnTypeEncounters.Lock = false;
            BtnTypeEncounters.Margin = new Padding(3, 4, 3, 4);
            BtnTypeEncounters.Name = "BtnTypeEncounters";
            BtnTypeEncounters.Size = new System.Drawing.Size(105, 22);
            BtnTypeEncounters.TabIndex = 128;
            BtnTypeEncounters.Text = "TF/Trial/Zone";
            BtnTypeEncounters.TextOutline.Color = System.Drawing.Color.Black;
            BtnTypeEncounters.TextOutline.Width = 3;
            BtnTypeEncounters.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnTypeEncounters.ToggleText.Indeterminate = "TF/Trial/Zone";
            BtnTypeEncounters.ToggleText.ToggledOff = "TF/Trial/Zone";
            BtnTypeEncounters.ToggleText.ToggledOn = "TF/Trial/Zone";
            BtnTypeEncounters.UseAlt = false;
            BtnTypeEncounters.Click += BtnTypeEncounters_Click;
            // 
            // BtnTypeBaseBuffs
            // 
            BtnTypeBaseBuffs.BackgroundImageLayout = ImageLayout.None;
            BtnTypeBaseBuffs.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnTypeBaseBuffs.CurrentText = "ToggledOff State";
            BtnTypeBaseBuffs.DisplayVertically = false;
            BtnTypeBaseBuffs.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            BtnTypeBaseBuffs.Images.Background = Resources.HeroButton;
            BtnTypeBaseBuffs.Images.Hover = Resources.HeroButtonHover;
            BtnTypeBaseBuffs.ImagesAlt.Background = Resources.VillainButton;
            BtnTypeBaseBuffs.ImagesAlt.Hover = Resources.VillainButtonHover;
            BtnTypeBaseBuffs.ImagesDis.Flat = Resources.DisabledButtonFlat;
            BtnTypeBaseBuffs.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            BtnTypeBaseBuffs.Location = new System.Drawing.Point(287, 13);
            BtnTypeBaseBuffs.Lock = false;
            BtnTypeBaseBuffs.Margin = new Padding(3, 4, 3, 4);
            BtnTypeBaseBuffs.Name = "BtnTypeBaseBuffs";
            BtnTypeBaseBuffs.Size = new System.Drawing.Size(105, 22);
            BtnTypeBaseBuffs.TabIndex = 129;
            BtnTypeBaseBuffs.Text = "Base Buffs";
            BtnTypeBaseBuffs.TextOutline.Color = System.Drawing.Color.Black;
            BtnTypeBaseBuffs.TextOutline.Width = 3;
            BtnTypeBaseBuffs.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnTypeBaseBuffs.ToggleText.Indeterminate = "Base Buffs";
            BtnTypeBaseBuffs.ToggleText.ToggledOff = "Base Buffs";
            BtnTypeBaseBuffs.ToggleText.ToggledOn = "Base Buffs";
            BtnTypeBaseBuffs.UseAlt = false;
            BtnTypeBaseBuffs.Click += BtnTypeBaseBuffs_Click;
            // 
            // BtnTypeDayJobs
            // 
            BtnTypeDayJobs.BackgroundImageLayout = ImageLayout.None;
            BtnTypeDayJobs.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnTypeDayJobs.CurrentText = "ToggledOff State";
            BtnTypeDayJobs.DisplayVertically = false;
            BtnTypeDayJobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            BtnTypeDayJobs.Images.Background = Resources.HeroButton;
            BtnTypeDayJobs.Images.Hover = Resources.HeroButtonHover;
            BtnTypeDayJobs.ImagesAlt.Background = Resources.VillainButton;
            BtnTypeDayJobs.ImagesAlt.Hover = Resources.VillainButtonHover;
            BtnTypeDayJobs.ImagesDis.Flat = Resources.DisabledButtonFlat;
            BtnTypeDayJobs.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            BtnTypeDayJobs.Location = new System.Drawing.Point(174, 47);
            BtnTypeDayJobs.Lock = false;
            BtnTypeDayJobs.Margin = new Padding(3, 4, 3, 4);
            BtnTypeDayJobs.Name = "BtnTypeDayJobs";
            BtnTypeDayJobs.Size = new System.Drawing.Size(105, 22);
            BtnTypeDayJobs.TabIndex = 130;
            BtnTypeDayJobs.Text = "Day Jobs";
            BtnTypeDayJobs.TextOutline.Color = System.Drawing.Color.Black;
            BtnTypeDayJobs.TextOutline.Width = 3;
            BtnTypeDayJobs.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnTypeDayJobs.ToggleText.Indeterminate = "Day Jobs";
            BtnTypeDayJobs.ToggleText.ToggledOff = "Day Jobs";
            BtnTypeDayJobs.ToggleText.ToggledOn = "Day Jobs";
            BtnTypeDayJobs.UseAlt = false;
            BtnTypeDayJobs.Click += BtnTypeDayJobs_Click;
            // 
            // BtnTypeMisc
            // 
            BtnTypeMisc.BackgroundImageLayout = ImageLayout.None;
            BtnTypeMisc.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnTypeMisc.CurrentText = "ToggledOff State";
            BtnTypeMisc.DisplayVertically = false;
            BtnTypeMisc.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            BtnTypeMisc.Images.Background = Resources.HeroButton;
            BtnTypeMisc.Images.Hover = Resources.HeroButtonHover;
            BtnTypeMisc.ImagesAlt.Background = Resources.VillainButton;
            BtnTypeMisc.ImagesAlt.Hover = Resources.VillainButtonHover;
            BtnTypeMisc.ImagesDis.Flat = Resources.DisabledButtonFlat;
            BtnTypeMisc.ImagesDis.Gloss = Resources.DisabledButtonGloss;
            BtnTypeMisc.Location = new System.Drawing.Point(287, 47);
            BtnTypeMisc.Lock = false;
            BtnTypeMisc.Margin = new Padding(3, 4, 3, 4);
            BtnTypeMisc.Name = "BtnTypeMisc";
            BtnTypeMisc.Size = new System.Drawing.Size(105, 22);
            BtnTypeMisc.TabIndex = 131;
            BtnTypeMisc.Text = "Misc";
            BtnTypeMisc.TextOutline.Color = System.Drawing.Color.Black;
            BtnTypeMisc.TextOutline.Width = 3;
            BtnTypeMisc.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnTypeMisc.ToggleText.Indeterminate = "Misc";
            BtnTypeMisc.ToggleText.ToggledOff = "Misc";
            BtnTypeMisc.ToggleText.ToggledOn = "Misc";
            BtnTypeMisc.UseAlt = false;
            BtnTypeMisc.Click += BtnTypeMisc_Click;
            // 
            // frmTemp
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(438, 470);
            Controls.Add(BtnTypeMisc);
            Controls.Add(BtnTypeDayJobs);
            Controls.Add(BtnTypeBaseBuffs);
            Controls.Add(BtnTypeEncounters);
            Controls.Add(BtnTypeAll);
            Controls.Add(Panel2);
            Controls.Add(lblLock);
            Controls.Add(Panel1);
            Controls.Add(ibClose);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmTemp";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Temporary Powers";
            TopMost = true;
            Load += frmTemp_Load;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        private ImageButtonEx ibClose;
        private Label lblLock;
        private Panel Panel1;
        private Panel Panel2;
        private ctlPopUp PopInfo;
        private VScrollBar VScrollBar1;
        private SkPairedList SkPairedList1;

        #endregion

        private ImageButtonEx BtnTypeAll;
        private ImageButtonEx BtnTypeEncounters;
        private ImageButtonEx BtnTypeBaseBuffs;
        private ImageButtonEx BtnTypeDayJobs;
        private ImageButtonEx BtnTypeMisc;
    }
}