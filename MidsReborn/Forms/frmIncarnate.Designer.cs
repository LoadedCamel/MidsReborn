using System.ComponentModel;
using mrbControls;

namespace Mids_Reborn.Forms
{
    public partial class frmIncarnate
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
            this.Panel2 = new frmIncarnate.CustomPanel();
            this.LLRight = new ListLabelV3();
            this.LLLeft = new ListLabelV3();
            this.OmegaButton = new ImageButton();
            this.VitaeButton = new ImageButton();
            this.StanceButton = new ImageButton();
            this.GenesisButton = new ImageButton();
            this.hybridBtn = new ImageButton();
            this.destinyBtn = new ImageButton();
            this.loreBtn = new ImageButton();
            this.interfaceBtn = new ImageButton();
            this.judgementBtn = new ImageButton();
            this.alphaBtn = new ImageButton();
            this.ibClose = new ImageButton();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();

            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.VScrollBar1);
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.PopInfo);
            this.Panel1.Location = new System.Drawing.Point(12, 309);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(440, 161);
            this.Panel1.TabIndex = 35;

            this.VScrollBar1.Location = new System.Drawing.Point(419, -2);
            this.VScrollBar1.Name = "VScrollBar1";
            this.VScrollBar1.Size = new System.Drawing.Size(17, 159);
            this.VScrollBar1.TabIndex = 10;

            this.PopInfo.BXHeight = 1024;
            this.PopInfo.ColumnPosition = 0.5f;
            this.PopInfo.ColumnRight = false;
            this.PopInfo.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.PopInfo.ForeColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.PopInfo.InternalPadding = 3;
            this.PopInfo.Location = new System.Drawing.Point(0, 0);
            this.PopInfo.Name = "PopInfo";
            this.PopInfo.ScrollY = 0.0f;
            this.PopInfo.SectionPadding = 8;
            this.PopInfo.Size = new System.Drawing.Size(416, 200);
            this.PopInfo.TabIndex = 9;

            this.lblLock.BackColor = System.Drawing.Color.Red;
            this.lblLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLock.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.lblLock.ForeColor = System.Drawing.Color.White;
            this.lblLock.Location = new System.Drawing.Point(12, 473);
            this.lblLock.Name = "lblLock";
            this.lblLock.Size = new System.Drawing.Size(56, 20);
            this.lblLock.TabIndex = 69;
            this.lblLock.Text = "[Unlock]";
            this.lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLock.Visible = false;

            this.Panel2.AutoScroll = true;
            this.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.LLRight);
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.LLLeft);
            this.Panel2.Location = new System.Drawing.Point(12, 96);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(440, 207);
            this.Panel2.TabIndex = 125;
            this.Panel2.TabStop = true;

            this.LLRight.AutoSize = true;
            this.LLRight.Expandable = true;
            this.LLRight.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.LLRight.HighVis = true;
            this.LLRight.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.LLRight.Location = new System.Drawing.Point(218, 0);
            this.LLRight.MaxHeight = 900;
            this.LLRight.Name = "LLRight";
            this.LLRight.PaddingX = 2;
            this.LLRight.PaddingY = 2;
            this.LLRight.Scrollable = false;
            this.LLRight.ScrollBarColor = System.Drawing.Color.Red;
            this.LLRight.ScrollBarWidth = 11;
            this.LLRight.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.LLRight.Size = new System.Drawing.Size(198, 414);
            this.LLRight.SizeNormal = new System.Drawing.Size(198, 140);
            this.LLRight.SuspendRedraw = false;
            this.LLRight.TabIndex = 109;

            this.LLLeft.AutoSize = true;
            this.LLLeft.Expandable = true;
            this.LLLeft.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.LLLeft.HighVis = true;
            this.LLLeft.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.LLLeft.Location = new System.Drawing.Point(0, 0);
            this.LLLeft.MaxHeight = 900;
            this.LLLeft.Name = "LLLeft";
            this.LLLeft.PaddingX = 2;
            this.LLLeft.PaddingY = 2;
            this.LLLeft.Scrollable = false;
            this.LLLeft.ScrollBarColor = System.Drawing.Color.Red;
            this.LLLeft.ScrollBarWidth = 11;
            this.LLLeft.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.LLLeft.Size = new System.Drawing.Size(218, 414);
            this.LLLeft.SizeNormal = new System.Drawing.Size(218, 140);
            this.LLLeft.SuspendRedraw = false;
            this.LLLeft.TabIndex = 108;

            this.OmegaButton.Checked = false;
            this.OmegaButton.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.OmegaButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.OmegaButton.Location = new System.Drawing.Point(236, 68);
            this.OmegaButton.Name = "OmegaButton";
            this.OmegaButton.Size = new System.Drawing.Size(105, 22);
            this.OmegaButton.TabIndex = 124;
            this.OmegaButton.TextOff = "Omega";
            this.OmegaButton.TextOn = "Omega";
            this.OmegaButton.Toggle = true;
            this.OmegaButton.Enabled = false;

            this.VitaeButton.Checked = false;
            this.VitaeButton.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.VitaeButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.VitaeButton.Location = new System.Drawing.Point(125, 68);
            this.VitaeButton.Name = "VitaeButton";
            this.VitaeButton.Size = new System.Drawing.Size(105, 22);
            this.VitaeButton.TabIndex = 123;
            this.VitaeButton.TextOff = "Vitae";
            this.VitaeButton.TextOn = "Vitae";
            this.VitaeButton.Toggle = true;
            this.VitaeButton.Enabled = false;

            this.StanceButton.Checked = false;
            this.StanceButton.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.StanceButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.StanceButton.Location = new System.Drawing.Point(347, 40);
            this.StanceButton.Name = "StanceButton";
            this.StanceButton.Size = new System.Drawing.Size(105, 22);
            this.StanceButton.TabIndex = 121;
            this.StanceButton.TextOff = "Stance";
            this.StanceButton.TextOn = "Stance";
            this.StanceButton.Toggle = true;
            this.StanceButton.Enabled = false;

            this.GenesisButton.Checked = false;
            this.GenesisButton.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.GenesisButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.GenesisButton.Location = new System.Drawing.Point(236, 40);
            this.GenesisButton.Name = "GenesisButton";
            this.GenesisButton.Size = new System.Drawing.Size(105, 22);
            this.GenesisButton.TabIndex = 120;
            this.GenesisButton.TextOff = "Genesis";
            this.GenesisButton.TextOn = "Genesis";
            this.GenesisButton.Toggle = true;
            this.GenesisButton.Enabled = false;

            this.hybridBtn.Checked = false;
            this.hybridBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.hybridBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.hybridBtn.Location = new System.Drawing.Point(125, 40);
            this.hybridBtn.Name = "hybridBtn";
            this.hybridBtn.Size = new System.Drawing.Size(105, 22);
            this.hybridBtn.TabIndex = 119;
            this.hybridBtn.TextOff = "Hybrid";
            this.hybridBtn.TextOn = "Hybrid";
            this.hybridBtn.Toggle = true;

            this.destinyBtn.Checked = false;
            this.destinyBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.destinyBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.destinyBtn.Location = new System.Drawing.Point(14, 40);
            this.destinyBtn.Name = "destinyBtn";
            this.destinyBtn.Size = new System.Drawing.Size(105, 22);
            this.destinyBtn.TabIndex = 118;
            this.destinyBtn.TextOff = "Destiny";
            this.destinyBtn.TextOn = "Destiny";
            this.destinyBtn.Toggle = true;

            this.loreBtn.Checked = false;
            this.loreBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.loreBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.loreBtn.Location = new System.Drawing.Point(347, 12);
            this.loreBtn.Name = "loreBtn";
            this.loreBtn.Size = new System.Drawing.Size(105, 22);
            this.loreBtn.TabIndex = 117;
            this.loreBtn.TextOff = "Lore";
            this.loreBtn.TextOn = "Lore";
            this.loreBtn.Toggle = true;

            this.interfaceBtn.Checked = false;
            this.interfaceBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.interfaceBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.interfaceBtn.Location = new System.Drawing.Point(236, 12);
            this.interfaceBtn.Name = "interfaceBtn";
            this.interfaceBtn.Size = new System.Drawing.Size(105, 22);
            this.interfaceBtn.TabIndex = 116;
            this.interfaceBtn.TextOff = "Interface";
            this.interfaceBtn.TextOn = "Interface";
            this.interfaceBtn.Toggle = true;

            this.judgementBtn.Checked = false;
            this.judgementBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.judgementBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.judgementBtn.Location = new System.Drawing.Point(125, 12);
            this.judgementBtn.Name = "judgementBtn";
            this.judgementBtn.Size = new System.Drawing.Size(105, 22);
            this.judgementBtn.TabIndex = 115;
            this.judgementBtn.TextOff = "Judgement";
            this.judgementBtn.TextOn = "Judgement";
            this.judgementBtn.Toggle = true;

            this.alphaBtn.Checked = true;
            this.alphaBtn.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.alphaBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.alphaBtn.Location = new System.Drawing.Point(14, 12);
            this.alphaBtn.Name = "alphaBtn";
            this.alphaBtn.Size = new System.Drawing.Size(105, 22);
            this.alphaBtn.TabIndex = 114;
            this.alphaBtn.TextOff = "Alpha";
            this.alphaBtn.TextOn = "Alpha";
            this.alphaBtn.Toggle = true;

            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(181, 473);
            this.ibClose.Name = "ibClose";
            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 7;
            this.ibClose.TextOff = "Done";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.ClientSize = new System.Drawing.Size(466, 510);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel2);
            this.Controls.Add((System.Windows.Forms.Control)this.OmegaButton);
            this.Controls.Add((System.Windows.Forms.Control)this.VitaeButton);
            this.Controls.Add((System.Windows.Forms.Control)this.StanceButton);
            this.Controls.Add((System.Windows.Forms.Control)this.GenesisButton);
            this.Controls.Add((System.Windows.Forms.Control)this.hybridBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.destinyBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.loreBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.interfaceBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.judgementBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.alphaBtn);
            this.Controls.Add((System.Windows.Forms.Control)this.lblLock);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel1);
            this.Controls.Add((System.Windows.Forms.Control)this.ibClose);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
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