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
            Panel1 = new System.Windows.Forms.Panel();
            Panel2 = new System.Windows.Forms.Panel();
            VScrollBar1 = new System.Windows.Forms.VScrollBar();
            PopInfo = new ctlPopUp();
            lblLock = new System.Windows.Forms.Label();
            ibClose = new ImageButtonEx();
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
            Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            PopInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
            lblLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
            // ibClose
            // 
            ibClose.BackgroundImageLayout = ImageLayout.None;
            ibClose.ButtonType = ImageButtonEx.ButtonTypes.Normal;
            ibClose.Font = new System.Drawing.Font("MS Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ibClose.Location = new System.Drawing.Point(165, 366);
            ibClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            ibClose.ThreeState = false;
            ibClose.UseAlt = false;
            ibClose.Click += ibClose_Click;
            // 
            // Panel2
            // 
            Panel2.AutoScroll = false;
            Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            Panel2.Controls.Add(SkPairedList1);
            Panel2.Location = new System.Drawing.Point(12, 12);
            Panel2.Name = "Panel2";
            Panel2.Size = new System.Drawing.Size(414, 140);
            Panel2.TabIndex = 126;
            Panel2.TabStop = true;
            // 
            // frmTemp
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(438, 403);
            Controls.Add(Panel2);
            Controls.Add(lblLock);
            Controls.Add(Panel1);
            Controls.Add(ibClose);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            StartPosition = FormStartPosition.CenterParent;
            Name = "frmTemp";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Temporary Powers";
            TopMost = true;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            ResumeLayout(false);
            Load += frmTemp_Load;
        }

        private ImageButtonEx ibClose;
        private Label lblLock;
        private Panel Panel1;
        private Panel Panel2;
        private ctlPopUp PopInfo;
        private VScrollBar VScrollBar1;
        private SkPairedList SkPairedList1;

        #endregion
    }
}