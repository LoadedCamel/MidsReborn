using System.ComponentModel;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.Panel1 = new System.Windows.Forms.Panel();
            this.VScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.PopInfo = new ctlPopUp();
            this.lblLock = new System.Windows.Forms.Label();
            this.ibClose = new ImageButton();
            this.Panel2 = new FrmIncarnate.CustomPanel();
            this.llRight = new ListLabelV3();
            this.llLeft = new ListLabelV3();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.VScrollBar1);
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.PopInfo);

            this.Panel1.Location = new System.Drawing.Point(12, 174);
            this.Panel1.Name = "Panel1";

            this.Panel1.Size = new System.Drawing.Size(414, 189);
            this.Panel1.TabIndex = 35;

            this.VScrollBar1.Location = new System.Drawing.Point(393, 0);
            this.VScrollBar1.Name = "VScrollBar1";

            this.VScrollBar1.Size = new System.Drawing.Size(17, 185);
            this.VScrollBar1.TabIndex = 11;
            this.VScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(VScrollBar1_Scroll);
            this.PopInfo.BXHeight = 1024;
            this.PopInfo.ColumnPosition = 0.5f;
            this.PopInfo.ColumnRight = false;
            this.PopInfo.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.PopInfo.ForeColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.PopInfo.InternalPadding = 3;

            this.PopInfo.Location = new System.Drawing.Point(0, 0);
            this.PopInfo.Name = "PopInfo";
            this.PopInfo.ScrollY = 0.0f;
            this.PopInfo.SectionPadding = 8;

            this.PopInfo.Size = new System.Drawing.Size(391, 200);
            this.PopInfo.TabIndex = 9;
            this.PopInfo.MouseWheel += new System.Windows.Forms.MouseEventHandler(PopInfo_MouseWheel);
            this.PopInfo.MouseEnter += new System.EventHandler(PopInfo_MouseEnter);

            this.lblLock.BackColor = System.Drawing.Color.Red;
            this.lblLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLock.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.lblLock.ForeColor = System.Drawing.Color.White;

            this.lblLock.Location = new System.Drawing.Point(12, 155);
            this.lblLock.Name = "lblLock";

            this.lblLock.Size = new System.Drawing.Size(56, 16);
            this.lblLock.TabIndex = 69;
            this.lblLock.Text = "[Unlock]";
            this.lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLock.Visible = false;
            this.lblLock.Click += new System.EventHandler(lblLock_Click);
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.ibClose.Location = new System.Drawing.Point(166, 369);
            this.ibClose.Name = "ibClose";

            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 7;
            this.ibClose.TextOff = "Done";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            this.ibClose.ButtonClicked += new ImageButton.ButtonClickedEventHandler(ibClose_ButtonClicked);
            this.Panel2.AutoScroll = true;
            this.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.llRight);
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.llLeft);

            this.Panel2.Location = new System.Drawing.Point(12, 12);
            this.Panel2.Name = "Panel2";

            this.Panel2.Size = new System.Drawing.Size(414, 140);
            this.Panel2.TabIndex = 126;
            this.Panel2.TabStop = true;
            this.llRight.AutoSize = true;
            this.llRight.Expandable = false;
            this.llRight.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.llRight.HighVis = true;
            this.llRight.HoverColor = System.Drawing.Color.WhiteSmoke;

            this.llRight.Location = new System.Drawing.Point(196, 3);
            this.llRight.MaxHeight = 600;
            this.llRight.Name = "llRight";
            this.llRight.PaddingX = 2;
            this.llRight.PaddingY = 2;
            this.llRight.Scrollable = false;
            this.llRight.ScrollBarColor = System.Drawing.Color.Red;
            this.llRight.ScrollBarWidth = 11;
            this.llRight.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);

            this.llRight.Size = new System.Drawing.Size(190, 414);

            this.llRight.SizeNormal = new System.Drawing.Size(190, 120);
            this.llRight.SuspendRedraw = false;
            this.llRight.TabIndex = 111;
            this.llRight.ItemHover += new ListLabelV3.ItemHoverEventHandler(llRight_ItemHover);
            this.llRight.ItemClick += new ListLabelV3.ItemClickEventHandler(llRight_ItemClick);
            this.llRight.MouseEnter += new System.EventHandler(llRight_MouseEnter);
            this.llLeft.AutoSize = true;
            this.llLeft.Expandable = false;
            this.llLeft.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.llLeft.HighVis = true;
            this.llLeft.HoverColor = System.Drawing.Color.WhiteSmoke;

            this.llLeft.Location = new System.Drawing.Point(3, 3);
            this.llLeft.MaxHeight = 600;
            this.llLeft.Name = "llLeft";
            this.llLeft.PaddingX = 2;
            this.llLeft.PaddingY = 2;
            this.llLeft.Scrollable = false;
            this.llLeft.ScrollBarColor = System.Drawing.Color.Red;
            this.llLeft.ScrollBarWidth = 11;
            this.llLeft.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);

            this.llLeft.Size = new System.Drawing.Size(187, 414);

            this.llLeft.SizeNormal = new System.Drawing.Size(187, 120);
            this.llLeft.SuspendRedraw = false;
            this.llLeft.TabIndex = 110;
            this.llLeft.MouseEnter += new System.EventHandler(llLeft_MouseEnter);
            this.llLeft.ItemHover += new ListLabelV3.ItemHoverEventHandler(llLeft_ItemHover);
            this.llLeft.ItemClick += new ListLabelV3.ItemClickEventHandler(llLeft_ItemClick);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(438, 403);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel2);
            this.Controls.Add((System.Windows.Forms.Control)this.lblLock);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel1);
            this.Controls.Add((System.Windows.Forms.Control)this.ibClose);
            this.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Powers";
            this.TopMost = true;
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();

            this.ResumeLayout(false);
        }
        #endregion
    }
}