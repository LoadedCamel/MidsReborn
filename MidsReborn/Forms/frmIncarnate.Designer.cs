using System.ComponentModel;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms
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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this._panel1 = new System.Windows.Forms.Panel();
            this._vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this._popInfo = new ctlPopUp();
            this._lblLock = new System.Windows.Forms.Label();
            this._panel2 = new FrmIncarnate.CustomPanel();
            this.LlRight = new ListLabelV3();
            this.LlLeft = new ListLabelV3();
            this._omegaButton = new ImageButton();
            this._vitaeButton = new ImageButton();
            this._stanceButton = new ImageButton();
            this._genesisButton = new ImageButton();
            this._hybridBtn = new ImageButton();
            this._destinyBtn = new ImageButton();
            this._loreBtn = new ImageButton();
            this._interfaceBtn = new ImageButton();
            this._judgementBtn = new ImageButton();
            this._alphaBtn = new ImageButton();
            this._ibClose = new ImageButton();
            this._panel1.SuspendLayout();
            this._panel2.SuspendLayout();
            this.SuspendLayout();

            this._panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._panel1.Controls.Add((System.Windows.Forms.Control)this._vScrollBar1);
            this._panel1.Controls.Add((System.Windows.Forms.Control)this._popInfo);
            this._panel1.Location = new System.Drawing.Point(12, 309);
            this._panel1.Name = "_panel1";
            this._panel1.Size = new System.Drawing.Size(440, 161);
            this._panel1.TabIndex = 35;

            this._vScrollBar1.Location = new System.Drawing.Point(419, -2);
            this._vScrollBar1.Name = "_vScrollBar1";
            this._vScrollBar1.Size = new System.Drawing.Size(17, 159);
            this._vScrollBar1.TabIndex = 10;

            this._popInfo.BXHeight = 1024;
            this._popInfo.ColumnPosition = 0.5f;
            this._popInfo.ColumnRight = false;
            this._popInfo.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._popInfo.ForeColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this._popInfo.InternalPadding = 3;
            this._popInfo.Location = new System.Drawing.Point(0, 0);
            this._popInfo.Name = "_popInfo";
            this._popInfo.ScrollY = 0.0f;
            this._popInfo.SectionPadding = 8;
            this._popInfo.Size = new System.Drawing.Size(416, 200);
            this._popInfo.TabIndex = 9;

            this._lblLock.BackColor = System.Drawing.Color.Red;
            this._lblLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblLock.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._lblLock.ForeColor = System.Drawing.Color.White;
            this._lblLock.Location = new System.Drawing.Point(12, 473);
            this._lblLock.Name = "_lblLock";
            this._lblLock.Size = new System.Drawing.Size(56, 20);
            this._lblLock.TabIndex = 69;
            this._lblLock.Text = "[Unlock]";
            this._lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblLock.Visible = false;

            this._panel2.AutoScroll = true;
            this._panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._panel2.Controls.Add((System.Windows.Forms.Control)this.LlRight);
            this._panel2.Controls.Add((System.Windows.Forms.Control)this.LlLeft);
            this._panel2.Location = new System.Drawing.Point(12, 96);
            this._panel2.Name = "_panel2";
            this._panel2.Size = new System.Drawing.Size(440, 207);
            this._panel2.TabIndex = 125;
            this._panel2.TabStop = true;

            this.LlRight.AutoSize = true;
            this.LlRight.Expandable = true;
            this.LlRight.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.LlRight.HighVis = true;
            this.LlRight.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.LlRight.Location = new System.Drawing.Point(218, 0);
            this.LlRight.MaxHeight = 900;
            this.LlRight.Name = "LlRight";
            this.LlRight.PaddingX = 2;
            this.LlRight.PaddingY = 2;
            this.LlRight.Scrollable = false;
            this.LlRight.ScrollBarColor = System.Drawing.Color.Red;
            this.LlRight.ScrollBarWidth = 11;
            this.LlRight.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.LlRight.Size = new System.Drawing.Size(198, 414);
            this.LlRight.SizeNormal = new System.Drawing.Size(198, 140);
            this.LlRight.SuspendRedraw = false;
            this.LlRight.TabIndex = 109;

            this.LlLeft.AutoSize = true;
            this.LlLeft.Expandable = true;
            this.LlLeft.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.LlLeft.HighVis = true;
            this.LlLeft.HoverColor = System.Drawing.Color.WhiteSmoke;
            this.LlLeft.Location = new System.Drawing.Point(0, 0);
            this.LlLeft.MaxHeight = 900;
            this.LlLeft.Name = "LlLeft";
            this.LlLeft.PaddingX = 2;
            this.LlLeft.PaddingY = 2;
            this.LlLeft.Scrollable = false;
            this.LlLeft.ScrollBarColor = System.Drawing.Color.Red;
            this.LlLeft.ScrollBarWidth = 11;
            this.LlLeft.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.LlLeft.Size = new System.Drawing.Size(218, 414);
            this.LlLeft.SizeNormal = new System.Drawing.Size(218, 140);
            this.LlLeft.SuspendRedraw = false;
            this.LlLeft.TabIndex = 108;

            this._omegaButton.Checked = false;
            this._omegaButton.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._omegaButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._omegaButton.Location = new System.Drawing.Point(236, 68);
            this._omegaButton.Name = "_omegaButton";
            this._omegaButton.Size = new System.Drawing.Size(105, 22);
            this._omegaButton.TabIndex = 124;
            this._omegaButton.TextOff = "Omega";
            this._omegaButton.TextOn = "Omega";
            this._omegaButton.Toggle = true;
            this._omegaButton.Enabled = false;

            this._vitaeButton.Checked = false;
            this._vitaeButton.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._vitaeButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._vitaeButton.Location = new System.Drawing.Point(125, 68);
            this._vitaeButton.Name = "_vitaeButton";
            this._vitaeButton.Size = new System.Drawing.Size(105, 22);
            this._vitaeButton.TabIndex = 123;
            this._vitaeButton.TextOff = "Vitae";
            this._vitaeButton.TextOn = "Vitae";
            this._vitaeButton.Toggle = true;
            this._vitaeButton.Enabled = false;

            this._stanceButton.Checked = false;
            this._stanceButton.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._stanceButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._stanceButton.Location = new System.Drawing.Point(347, 40);
            this._stanceButton.Name = "_stanceButton";
            this._stanceButton.Size = new System.Drawing.Size(105, 22);
            this._stanceButton.TabIndex = 121;
            this._stanceButton.TextOff = "Stance";
            this._stanceButton.TextOn = "Stance";
            this._stanceButton.Toggle = true;
            this._stanceButton.Enabled = false;

            this._genesisButton.Checked = false;
            this._genesisButton.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._genesisButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._genesisButton.Location = new System.Drawing.Point(236, 40);
            this._genesisButton.Name = "_genesisButton";
            this._genesisButton.Size = new System.Drawing.Size(105, 22);
            this._genesisButton.TabIndex = 120;
            this._genesisButton.TextOff = "Genesis";
            this._genesisButton.TextOn = "Genesis";
            this._genesisButton.Toggle = true;
            this._genesisButton.Enabled = false;

            this._hybridBtn.Checked = false;
            this._hybridBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._hybridBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._hybridBtn.Location = new System.Drawing.Point(125, 40);
            this._hybridBtn.Name = "_hybridBtn";
            this._hybridBtn.Size = new System.Drawing.Size(105, 22);
            this._hybridBtn.TabIndex = 119;
            this._hybridBtn.TextOff = "Hybrid";
            this._hybridBtn.TextOn = "Hybrid";
            this._hybridBtn.Toggle = true;

            this._destinyBtn.Checked = false;
            this._destinyBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._destinyBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._destinyBtn.Location = new System.Drawing.Point(14, 40);
            this._destinyBtn.Name = "_destinyBtn";
            this._destinyBtn.Size = new System.Drawing.Size(105, 22);
            this._destinyBtn.TabIndex = 118;
            this._destinyBtn.TextOff = "Destiny";
            this._destinyBtn.TextOn = "Destiny";
            this._destinyBtn.Toggle = true;

            this._loreBtn.Checked = false;
            this._loreBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._loreBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._loreBtn.Location = new System.Drawing.Point(347, 12);
            this._loreBtn.Name = "_loreBtn";
            this._loreBtn.Size = new System.Drawing.Size(105, 22);
            this._loreBtn.TabIndex = 117;
            this._loreBtn.TextOff = "Lore";
            this._loreBtn.TextOn = "Lore";
            this._loreBtn.Toggle = true;

            this._interfaceBtn.Checked = false;
            this._interfaceBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._interfaceBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._interfaceBtn.Location = new System.Drawing.Point(236, 12);
            this._interfaceBtn.Name = "_interfaceBtn";
            this._interfaceBtn.Size = new System.Drawing.Size(105, 22);
            this._interfaceBtn.TabIndex = 116;
            this._interfaceBtn.TextOff = "Interface";
            this._interfaceBtn.TextOn = "Interface";
            this._interfaceBtn.Toggle = true;

            this._judgementBtn.Checked = false;
            this._judgementBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._judgementBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._judgementBtn.Location = new System.Drawing.Point(125, 12);
            this._judgementBtn.Name = "_judgementBtn";
            this._judgementBtn.Size = new System.Drawing.Size(105, 22);
            this._judgementBtn.TabIndex = 115;
            this._judgementBtn.TextOff = "Judgement";
            this._judgementBtn.TextOn = "Judgement";
            this._judgementBtn.Toggle = true;

            this._alphaBtn.Checked = true;
            this._alphaBtn.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._alphaBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._alphaBtn.Location = new System.Drawing.Point(14, 12);
            this._alphaBtn.Name = "_alphaBtn";
            this._alphaBtn.Size = new System.Drawing.Size(105, 22);
            this._alphaBtn.TabIndex = 114;
            this._alphaBtn.TextOff = "Alpha";
            this._alphaBtn.TextOn = "Alpha";
            this._alphaBtn.Toggle = true;

            this._ibClose.Checked = false;
            this._ibClose.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this._ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this._ibClose.Location = new System.Drawing.Point(181, 473);
            this._ibClose.Name = "_ibClose";
            this._ibClose.Size = new System.Drawing.Size(105, 22);
            this._ibClose.TabIndex = 7;
            this._ibClose.TextOff = "Done";
            this._ibClose.TextOn = "Alt Text";
            this._ibClose.Toggle = false;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.ClientSize = new System.Drawing.Size(466, 510);
            this.Controls.Add((System.Windows.Forms.Control)this._panel2);
            this.Controls.Add((System.Windows.Forms.Control)this._omegaButton);
            this.Controls.Add((System.Windows.Forms.Control)this._vitaeButton);
            this.Controls.Add((System.Windows.Forms.Control)this._stanceButton);
            this.Controls.Add((System.Windows.Forms.Control)this._genesisButton);
            this.Controls.Add((System.Windows.Forms.Control)this._hybridBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._destinyBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._loreBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._interfaceBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._judgementBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._alphaBtn);
            this.Controls.Add((System.Windows.Forms.Control)this._lblLock);
            this.Controls.Add((System.Windows.Forms.Control)this._panel1);
            this.Controls.Add((System.Windows.Forms.Control)this._ibClose);
            this.Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Powers";
            this.TopMost = true;
            this._panel1.ResumeLayout(false);
            this._panel2.ResumeLayout(false);
            this._panel2.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion
    }
}