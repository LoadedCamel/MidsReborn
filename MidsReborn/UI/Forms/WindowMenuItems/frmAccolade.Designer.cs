using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmAccolade
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
            lblLock = new Label();
            Panel2 = new Mids_Reborn.UI.Forms.WindowMenuItems.FrmIncarnate.CustomPanel();
            SkPairedList1 = new SkPairedList();
            ibClose = new ImageButtonEx();
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();
            SuspendLayout();
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
            PopInfo.Size = new System.Drawing.Size(391, 200);
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
            // ibClose
            // 
            ibClose.BackgroundImageLayout = ImageLayout.None;
            ibClose.CurrentText = "Done";
            ibClose.DisplayVertically = false;
            ibClose.EnabledState = ImageButtonEx.EnabledStates.Enabled;
            ibClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            ibClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibClose.ImagesDis.Flat = null;
            ibClose.ImagesDis.Gloss = null;
            ibClose.Location = new System.Drawing.Point(166, 369);
            ibClose.Lock = false;
            ibClose.Name = "ibClose";
            ibClose.Size = new System.Drawing.Size(105, 22);
            ibClose.TabIndex = 127;
            ibClose.Text = "Done";
            ibClose.TextOutline.Color = System.Drawing.Color.Black;
            ibClose.TextOutline.Width = 2;
            ibClose.ToggleState = ImageButtonEx.States.ToggledOff;
            ibClose.ToggleText.Indeterminate = "Indeterminate State";
            ibClose.ToggleText.ToggledOff = "ToggledOff State";
            ibClose.ToggleText.ToggledOn = "ToggledOn State";
            ibClose.UseAlt = false;
            ibClose.Click += ibClose_Click;
            // 
            // frmAccolade
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(438, 403);
            Controls.Add(ibClose);
            Controls.Add(Panel2);
            Controls.Add(lblLock);
            Controls.Add(Panel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmAccolade";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Powers";
            TopMost = true;
            Load += frmAccolade_Load;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Label lblLock;
        private Panel Panel1;
        private FrmIncarnate.CustomPanel Panel2;
        private ctlPopUp PopInfo;
        private VScrollBar VScrollBar1;
        private ImageButtonEx ibClose;
        private SkPairedList SkPairedList1;
        

        #endregion
    }
}