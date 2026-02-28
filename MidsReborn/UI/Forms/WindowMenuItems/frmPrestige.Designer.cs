using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;
using System.ComponentModel;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmPrestige
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
            lblLock = new Label();
            Panel2 = new Mids_Reborn.UI.Forms.FrmIncarnate.CustomPanel();
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
            Panel1.Location = new System.Drawing.Point(12, 174);
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
            PopInfo.BXHeight = 1024;
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
            // lblLock
            // 
            lblLock.BackColor = System.Drawing.Color.Red;
            lblLock.BorderStyle = BorderStyle.FixedSingle;
            lblLock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, 0);
            lblLock.ForeColor = System.Drawing.Color.White;
            lblLock.Location = new System.Drawing.Point(12, 155);
            lblLock.Name = "lblLock";
            lblLock.Size = new System.Drawing.Size(56, 16);
            lblLock.TabIndex = 69;
            lblLock.Text = "[Unlock]";
            lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblLock.Visible = false;
            lblLock.Click += lblLock_Click;
            // 
            // Panel2
            // 
            Panel2.BorderStyle = BorderStyle.Fixed3D;
            Panel2.Controls.Add(SkPairedList1);
            Panel2.Location = new System.Drawing.Point(12, 12);
            Panel2.Name = "Panel2";
            Panel2.Size = new System.Drawing.Size(414, 140);
            Panel2.TabIndex = 126;
            Panel2.TabStop = true;
            // 
            // BtnDone
            // 
            BtnDone.BackgroundImageLayout = ImageLayout.None;
            BtnDone.CurrentText = "Done";
            BtnDone.DisplayVertically = false;
            BtnDone.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            BtnDone.Font = new System.Drawing.Font("MS Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            BtnDone.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnDone.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnDone.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnDone.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnDone.ImagesDis.Flat = null;
            BtnDone.ImagesDis.Gloss = null;
            BtnDone.Location = new System.Drawing.Point(166, 369);
            BtnDone.Lock = false;
            BtnDone.Name = "BtnDone";
            BtnDone.Size = new System.Drawing.Size(105, 22);
            BtnDone.TabIndex = 127;
            BtnDone.Text = "Done";
            BtnDone.TextOutline.Color = System.Drawing.Color.Black;
            BtnDone.TextOutline.Width = 2;
            BtnDone.ToggleState = ImageButtonEx.States.ToggledOff;
            BtnDone.ToggleText.Indeterminate = "Indeterminate State";
            BtnDone.ToggleText.ToggledOff = "ToggledOff State";
            BtnDone.ToggleText.ToggledOn = "ToggledOn State";
            BtnDone.UseAlt = false;
            BtnDone.Click += BtnDone_Click;
            // 
            // frmPrestige
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            ClientSize = new System.Drawing.Size(438, 403);
            Controls.Add(BtnDone);
            Controls.Add(Panel2);
            Controls.Add(lblLock);
            Controls.Add(Panel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmPrestige";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Prestige Powers";
            TopMost = true;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            ResumeLayout(false);
            Load += frmPrestige_Load;
            FormClosing += frmPrestige_FormClosing;
        }
        #endregion

        private Label lblLock;
        private Panel Panel1;
        private FrmIncarnate.CustomPanel Panel2;
        private ctlPopUp PopInfo;
        private VScrollBar VScrollBar1;
        private ImageButtonEx BtnDone;
        private SkPairedList SkPairedList1;
    }
}