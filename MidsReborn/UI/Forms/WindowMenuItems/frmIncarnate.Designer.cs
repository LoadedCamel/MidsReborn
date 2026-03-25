using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class FrmIncarnate
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
            Panel1 = new Panel();
            VScrollBar1 = new VScrollBar();
            PopInfo = new ctlPopUp();
            LblLock = new Label();
            Panel2 = new Panel();
            BtnAlpha = new ImageButtonEx();
            BtnJudgement = new ImageButtonEx();
            BtnInterface = new ImageButtonEx();
            BtnLore = new ImageButtonEx();
            BtnDestiny = new ImageButtonEx();
            BtnHybrid = new ImageButtonEx();
            BtnGenesis = new ImageButtonEx();
            BtnStance = new ImageButtonEx();
            BtnVitae = new ImageButtonEx();
            BtnOmega = new ImageButtonEx();
            BtnDone = new ImageButtonEx();
            SkPairedList1 = new SkPairedList();
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();
            SuspendLayout();
            //
            // SkPairedList1
            //
            SkPairedList1.AutoColumns = false;
            SkPairedList1.BackColor = System.Drawing.Color.Black;
            SkPairedList1.Columns = 2;
            SkPairedList1.Dock = DockStyle.Fill;
            SkPairedList1.HighVis = true;
            SkPairedList1.HoverColor = System.Drawing.Color.WhiteSmoke;
            SkPairedList1.Location = new System.Drawing.Point(0, 0);
            SkPairedList1.Name = "SkPairedList1";
            SkPairedList1.Scrollable = true;
            SkPairedList1.Size = new System.Drawing.Size(414, 140);
            SkPairedList1.ItemClick += SkPairedList1_ItemClick;
            SkPairedList1.ItemHover += SkPairedList1_ItemHover;
            SkPairedList1.EmptyHover += SkPairedList1_EmptyHover;
            SkPairedList1.MouseLeave += SkPairedList1_MouseLeave;
            // 
            // Panel1
            // 
            Panel1.BorderStyle = BorderStyle.Fixed3D;
            Panel1.Controls.Add(VScrollBar1);
            Panel1.Controls.Add(PopInfo);
            Panel1.Location = new Point(12, 309);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(660, 210);
            Panel1.TabIndex = 35;
            // 
            // VScrollBar1
            // 
            VScrollBar1.Location = new Point(641, 0);
            VScrollBar1.Name = "VScrollBar1";
            VScrollBar1.Size = new Size(17, 210);
            VScrollBar1.TabIndex = 10;
            VScrollBar1.Scroll += VScrollBar1_Scroll;
            // 
            // PopInfo
            // 
            PopInfo.ColumnPosition = 0.5F;
            PopInfo.ColumnRight = false;
            PopInfo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 0);
            PopInfo.ForeColor = Color.FromArgb(0, 0, 32);
            PopInfo.InternalPadding = 3;
            PopInfo.Location = new Point(0, 0);
            PopInfo.Margin = new Padding(4, 3, 4, 3);
            PopInfo.Name = "PopInfo";
            PopInfo.ScrollY = 0F;
            PopInfo.SectionPadding = 8;
            PopInfo.Size = new Size(635, 177);
            PopInfo.TabIndex = 9;
            PopInfo.MouseWheel += PopInfo_MouseWheel;
            PopInfo.MouseEnter += PopInfo_MouseEnter;
            // 
            // LblLock
            // 
            LblLock.BackColor = Color.Red;
            LblLock.BorderStyle = BorderStyle.FixedSingle;
            LblLock.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
            LblLock.ForeColor = Color.White;
            LblLock.Location = new Point(585, 313);
            LblLock.Name = "LblLock";
            LblLock.Size = new Size(56, 20);
            LblLock.TabIndex = 69;
            LblLock.Text = "[Unlock]";
            LblLock.TextAlign = ContentAlignment.MiddleCenter;
            LblLock.Visible = false;
            LblLock.Click += LblLock_Click;
            // 
            // Panel2
            // 
            Panel2.AutoScrollMinSize = new Size(0, 220);
            Panel2.BorderStyle = BorderStyle.Fixed3D;
            Panel2.Controls.Add(SkPairedList1);
            Panel2.Location = new Point(12, 81);
            Panel2.Name = "Panel2";
            Panel2.Size = new Size(660, 225);
            Panel2.TabIndex = 125;
            Panel2.TabStop = true;
            // 
            // BtnAlpha
            // 
            BtnAlpha.BackgroundImageLayout = ImageLayout.None;
            BtnAlpha.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnAlpha.CurrentText = "Alpha";
            BtnAlpha.DisplayVertically = false;
            BtnAlpha.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnAlpha.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnAlpha.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnAlpha.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnAlpha.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnAlpha.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnAlpha.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnAlpha.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnAlpha.Location = new Point(12, 12);
            BtnAlpha.Lock = false;
            BtnAlpha.Name = "BtnAlpha";
            BtnAlpha.Size = new Size(105, 22);
            BtnAlpha.TabIndex = 126;
            BtnAlpha.Text = "Alpha";
            BtnAlpha.TextOutline.Color = Color.Black;
            BtnAlpha.TextOutline.Width = 2;
            BtnAlpha.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnAlpha.ToggleText.Indeterminate = "Alpha";
            BtnAlpha.ToggleText.ToggledOff = "Alpha";
            BtnAlpha.ToggleText.ToggledOn = "Alpha";
            BtnAlpha.UseAlt = false;
            BtnAlpha.Click += IncarnateGroupButton_Click;
            // 
            // BtnJudgement
            // 
            BtnJudgement.BackgroundImageLayout = ImageLayout.None;
            BtnJudgement.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnJudgement.CurrentText = "Judgement";
            BtnJudgement.DisplayVertically = false;
            BtnJudgement.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnJudgement.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnJudgement.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnJudgement.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnJudgement.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnJudgement.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnJudgement.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnJudgement.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnJudgement.Location = new Point(151, 12);
            BtnJudgement.Lock = false;
            BtnJudgement.Name = "BtnJudgement";
            BtnJudgement.Size = new Size(105, 22);
            BtnJudgement.TabIndex = 127;
            BtnJudgement.Text = "Judgement";
            BtnJudgement.TextOutline.Color = Color.Black;
            BtnJudgement.TextOutline.Width = 2;
            BtnJudgement.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnJudgement.ToggleText.Indeterminate = "Judgement";
            BtnJudgement.ToggleText.ToggledOff = "Judgement";
            BtnJudgement.ToggleText.ToggledOn = "Judgement";
            BtnJudgement.UseAlt = false;
            BtnJudgement.Click += IncarnateGroupButton_Click;
            // 
            // BtnInterface
            // 
            BtnInterface.BackgroundImageLayout = ImageLayout.None;
            BtnInterface.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnInterface.CurrentText = "Interface";
            BtnInterface.DisplayVertically = false;
            BtnInterface.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnInterface.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnInterface.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnInterface.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnInterface.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnInterface.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnInterface.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnInterface.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnInterface.Location = new Point(289, 12);
            BtnInterface.Lock = false;
            BtnInterface.Name = "BtnInterface";
            BtnInterface.Size = new Size(105, 22);
            BtnInterface.TabIndex = 128;
            BtnInterface.Text = "Interface";
            BtnInterface.TextOutline.Color = Color.Black;
            BtnInterface.TextOutline.Width = 2;
            BtnInterface.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnInterface.ToggleText.Indeterminate = "Interface";
            BtnInterface.ToggleText.ToggledOff = "Interface";
            BtnInterface.ToggleText.ToggledOn = "Interface";
            BtnInterface.UseAlt = false;
            BtnInterface.Click += IncarnateGroupButton_Click;
            // 
            // BtnLore
            // 
            BtnLore.BackgroundImageLayout = ImageLayout.None;
            BtnLore.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnLore.CurrentText = "Lore";
            BtnLore.DisplayVertically = false;
            BtnLore.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnLore.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnLore.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnLore.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnLore.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnLore.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnLore.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnLore.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnLore.Location = new Point(427, 12);
            BtnLore.Lock = false;
            BtnLore.Name = "BtnLore";
            BtnLore.Size = new Size(105, 22);
            BtnLore.TabIndex = 129;
            BtnLore.Text = "Lore";
            BtnLore.TextOutline.Color = Color.Black;
            BtnLore.TextOutline.Width = 2;
            BtnLore.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnLore.ToggleText.Indeterminate = "Lore";
            BtnLore.ToggleText.ToggledOff = "Lore";
            BtnLore.ToggleText.ToggledOn = "Lore";
            BtnLore.UseAlt = false;
            BtnLore.Click += IncarnateGroupButton_Click;
            // 
            // BtnDestiny
            // 
            BtnDestiny.BackgroundImageLayout = ImageLayout.None;
            BtnDestiny.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnDestiny.CurrentText = "Destiny";
            BtnDestiny.DisplayVertically = false;
            BtnDestiny.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnDestiny.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnDestiny.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnDestiny.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnDestiny.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnDestiny.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnDestiny.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnDestiny.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnDestiny.Location = new Point(565, 12);
            BtnDestiny.Lock = false;
            BtnDestiny.Name = "BtnDestiny";
            BtnDestiny.Size = new Size(105, 22);
            BtnDestiny.TabIndex = 130;
            BtnDestiny.Text = "Destiny";
            BtnDestiny.TextOutline.Color = Color.Black;
            BtnDestiny.TextOutline.Width = 2;
            BtnDestiny.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnDestiny.ToggleText.Indeterminate = "Destiny";
            BtnDestiny.ToggleText.ToggledOff = "Destiny";
            BtnDestiny.ToggleText.ToggledOn = "Destiny";
            BtnDestiny.UseAlt = false;
            BtnDestiny.Click += IncarnateGroupButton_Click;
            // 
            // BtnHybrid
            // 
            BtnHybrid.BackgroundImageLayout = ImageLayout.None;
            BtnHybrid.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnHybrid.CurrentText = "Hybrid";
            BtnHybrid.DisplayVertically = false;
            BtnHybrid.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnHybrid.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnHybrid.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnHybrid.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnHybrid.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnHybrid.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnHybrid.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnHybrid.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnHybrid.Location = new Point(12, 40);
            BtnHybrid.Lock = false;
            BtnHybrid.Name = "BtnHybrid";
            BtnHybrid.Size = new Size(105, 22);
            BtnHybrid.TabIndex = 131;
            BtnHybrid.Text = "Hybrid";
            BtnHybrid.TextOutline.Color = Color.Black;
            BtnHybrid.TextOutline.Width = 2;
            BtnHybrid.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnHybrid.ToggleText.Indeterminate = "Hybrid";
            BtnHybrid.ToggleText.ToggledOff = "Hybrid";
            BtnHybrid.ToggleText.ToggledOn = "Hybrid";
            BtnHybrid.UseAlt = false;
            BtnHybrid.Click += IncarnateGroupButton_Click;
            // 
            // BtnGenesis
            // 
            BtnGenesis.BackgroundImageLayout = ImageLayout.None;
            BtnGenesis.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnGenesis.CurrentText = "Genesis";
            BtnGenesis.DisplayVertically = false;
            BtnGenesis.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnGenesis.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnGenesis.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnGenesis.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnGenesis.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnGenesis.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnGenesis.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnGenesis.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnGenesis.Location = new Point(151, 40);
            BtnGenesis.Lock = false;
            BtnGenesis.Name = "BtnGenesis";
            BtnGenesis.Size = new Size(105, 22);
            BtnGenesis.TabIndex = 132;
            BtnGenesis.Text = "Genesis";
            BtnGenesis.TextOutline.Color = Color.Black;
            BtnGenesis.TextOutline.Width = 2;
            BtnGenesis.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnGenesis.ToggleText.Indeterminate = "Genesis";
            BtnGenesis.ToggleText.ToggledOff = "Genesis";
            BtnGenesis.ToggleText.ToggledOn = "Genesis";
            BtnGenesis.UseAlt = false;
            BtnGenesis.Click += IncarnateGroupButton_Click;
            // 
            // BtnStance
            // 
            BtnStance.BackgroundImageLayout = ImageLayout.None;
            BtnStance.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnStance.CurrentText = "Stance";
            BtnStance.DisplayVertically = false;
            BtnStance.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnStance.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnStance.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnStance.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnStance.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnStance.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnStance.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnStance.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnStance.Location = new Point(289, 40);
            BtnStance.Lock = false;
            BtnStance.Name = "BtnStance";
            BtnStance.Size = new Size(105, 22);
            BtnStance.TabIndex = 133;
            BtnStance.Text = "Stance";
            BtnStance.TextOutline.Color = Color.Black;
            BtnStance.TextOutline.Width = 2;
            BtnStance.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnStance.ToggleText.Indeterminate = "Stance";
            BtnStance.ToggleText.ToggledOff = "Stance";
            BtnStance.ToggleText.ToggledOn = "Stance";
            BtnStance.UseAlt = false;
            BtnStance.Click += IncarnateGroupButton_Click;
            // 
            // BtnVitae
            // 
            BtnVitae.BackgroundImageLayout = ImageLayout.None;
            BtnVitae.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnVitae.CurrentText = "Vitae";
            BtnVitae.DisplayVertically = false;
            BtnVitae.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnVitae.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnVitae.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnVitae.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnVitae.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnVitae.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnVitae.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnVitae.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnVitae.Location = new Point(427, 40);
            BtnVitae.Lock = false;
            BtnVitae.Name = "BtnVitae";
            BtnVitae.Size = new Size(105, 22);
            BtnVitae.TabIndex = 134;
            BtnVitae.Text = "Vitae";
            BtnVitae.TextOutline.Color = Color.Black;
            BtnVitae.TextOutline.Width = 2;
            BtnVitae.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnVitae.ToggleText.Indeterminate = "Vitae";
            BtnVitae.ToggleText.ToggledOff = "Vitae";
            BtnVitae.ToggleText.ToggledOn = "Vitae";
            BtnVitae.UseAlt = false;
            BtnVitae.Click += IncarnateGroupButton_Click;
            // 
            // BtnOmega
            // 
            BtnOmega.BackgroundImageLayout = ImageLayout.None;
            BtnOmega.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            BtnOmega.CurrentText = "Omega";
            BtnOmega.DisplayVertically = false;
            BtnOmega.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnOmega.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnOmega.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnOmega.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnOmega.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnOmega.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnOmega.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnOmega.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnOmega.Location = new Point(565, 40);
            BtnOmega.Lock = false;
            BtnOmega.Name = "BtnOmega";
            BtnOmega.Size = new Size(105, 22);
            BtnOmega.TabIndex = 135;
            BtnOmega.Text = "Omega";
            BtnOmega.TextOutline.Color = Color.Black;
            BtnOmega.TextOutline.Width = 2;
            BtnOmega.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnOmega.ToggleText.Indeterminate = "Omega";
            BtnOmega.ToggleText.ToggledOff = "Omega";
            BtnOmega.ToggleText.ToggledOn = "Omega";
            BtnOmega.UseAlt = false;
            BtnOmega.Click += IncarnateGroupButton_Click;
            // 
            // BtnDone
            // 
            BtnDone.BackgroundImageLayout = ImageLayout.None;
            BtnDone.CurrentText = "Done";
            BtnDone.DisplayVertically = false;
            BtnDone.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnDone.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            BtnDone.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnDone.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnDone.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnDone.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnDone.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnDone.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnDone.Location = new Point(294, 527);
            BtnDone.Lock = false;
            BtnDone.Name = "BtnDone";
            BtnDone.Size = new Size(105, 22);
            BtnDone.TabIndex = 136;
            BtnDone.Text = "Done";
            BtnDone.TextOutline.Color = Color.Black;
            BtnDone.TextOutline.Width = 2;
            BtnDone.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnDone.ToggleText.Indeterminate = "Indeterminate State";
            BtnDone.ToggleText.ToggledOff = "ToggledOff State";
            BtnDone.ToggleText.ToggledOn = "ToggledOn State";
            BtnDone.UseAlt = false;
            BtnDone.Click += BtnDone_Click;
            // 
            // FrmIncarnate
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(684, 561);
            Controls.Add(BtnDone);
            Controls.Add(BtnOmega);
            Controls.Add(BtnVitae);
            Controls.Add(BtnStance);
            Controls.Add(BtnGenesis);
            Controls.Add(BtnHybrid);
            Controls.Add(BtnDestiny);
            Controls.Add(BtnLore);
            Controls.Add(BtnInterface);
            Controls.Add(BtnJudgement);
            Controls.Add(BtnAlpha);
            Controls.Add(Panel2);
            Controls.Add(LblLock);
            Controls.Add(Panel1);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "FrmIncarnate";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Incarnate Powers";
            TopMost = true;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            ResumeLayout(false);
            Load += frmIncarnate_Load;
            FormClosing += frmIncarnate_FormClosing;
        }

        #endregion
        private Label LblLock;
        private Panel Panel1;
        private Panel Panel2;
        private ctlPopUp PopInfo;
        private VScrollBar VScrollBar1;
        private ImageButtonEx BtnAlpha;
        private ImageButtonEx BtnJudgement;
        private ImageButtonEx BtnInterface;
        private ImageButtonEx BtnLore;
        private ImageButtonEx BtnDestiny;
        private ImageButtonEx BtnHybrid;
        private ImageButtonEx BtnGenesis;
        private ImageButtonEx BtnStance;
        private ImageButtonEx BtnVitae;
        private ImageButtonEx BtnOmega;
        private ImageButtonEx BtnDone;
        private SkPairedList SkPairedList1;
    }
}