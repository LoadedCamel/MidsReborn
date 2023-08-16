using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Forms.Controls;

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
            _panel1 = new Panel();
            _vScrollBar1 = new VScrollBar();
            _popInfo = new ctlPopUp();
            _lblLock = new Label();
            _panel2 = new Panel();
            LlRight = new ListLabelV3();
            LlLeft = new ListLabelV3();
            _omegaButton = new ImageButton();
            _vitaeButton = new ImageButton();
            _stanceButton = new ImageButton();
            _genesisButton = new ImageButton();
            _hybridBtn = new ImageButton();
            _destinyBtn = new ImageButton();
            _loreBtn = new ImageButton();
            _interfaceBtn = new ImageButton();
            _judgementBtn = new ImageButton();
            _alphaBtn = new ImageButton();
            _ibClose = new ImageButton();
            _panel1.SuspendLayout();
            _panel2.SuspendLayout();
            SuspendLayout();
            // 
            // _panel1
            //
            _panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            _panel1.Controls.Add(_vScrollBar1);
            _panel1.Controls.Add(_popInfo);
            _panel1.Location = new System.Drawing.Point(12, 309);
            _panel1.Name = "_panel1";
            _panel1.Size = new System.Drawing.Size(660, 210);
            _panel1.TabIndex = 35;
            // 
            // _vScrollBar1
            // 
            _vScrollBar1.Location = new System.Drawing.Point(641, 0);
            _vScrollBar1.Name = "_vScrollBar1";
            _vScrollBar1.Size = new System.Drawing.Size(17, 210);
            _vScrollBar1.TabIndex = 10;
            _vScrollBar1.Scroll += VScrollBar1_Scroll;
            //
            // _popInfo
            //
            _popInfo.BXHeight = 1024;
            _popInfo.ColumnPosition = 0.5f;
            _popInfo.ColumnRight = false;
            _popInfo.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            _popInfo.ForeColor = System.Drawing.Color.FromArgb(0, 0, 32);
            _popInfo.InternalPadding = 3;
            _popInfo.Location = new System.Drawing.Point(0, 0);
            _popInfo.Name = "_popInfo";
            _popInfo.ScrollY = 0.0f;
            _popInfo.SectionPadding = 8;
            _popInfo.Size = new System.Drawing.Size(635, 212);
            _popInfo.TabIndex = 9;
            // 
            // _lblLock
            // 
            _lblLock.BackColor = System.Drawing.Color.Red;
            _lblLock.BorderStyle = BorderStyle.FixedSingle;
            _lblLock.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _lblLock.ForeColor = System.Drawing.Color.White;
            _lblLock.Location = new System.Drawing.Point(585, 313);
            _lblLock.Name = "_lblLock";
            _lblLock.Size = new System.Drawing.Size(56, 20);
            _lblLock.TabIndex = 69;
            _lblLock.Text = "[Unlock]";
            _lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            _lblLock.Visible = false;
            _lblLock.Click += lblLock_Click;
            // 
            // _panel2
            // 
            _panel2.AutoScroll = false;
            _panel2.HorizontalScroll.Maximum = 0;
            _panel2.AutoScrollMinSize = new Size(0, 220);
            _panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            _panel2.Controls.Add(LlRight);
            _panel2.Controls.Add(LlLeft);
            _panel2.Location = new System.Drawing.Point(12, 87);
            _panel2.Name = "_panel2";
            _panel2.Size = new System.Drawing.Size(660, 219);
            _panel2.TabIndex = 125;
            _panel2.TabStop = true;
            // 
            // LlRight
            // 
            LlRight.AutoSize = true;
            LlRight.Expandable = true;
            LlRight.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            LlRight.HighVis = true;
            LlRight.HoverColor = System.Drawing.Color.WhiteSmoke;
            LlRight.Location = new System.Drawing.Point(330, 0);
            LlRight.MaxHeight = 900;
            LlRight.Name = "LlRight";
            LlRight.PaddingX = 2;
            LlRight.PaddingY = 2;
            LlRight.Scrollable = false;
            LlRight.ScrollBarColor = System.Drawing.Color.Red;
            LlRight.ScrollBarWidth = 11;
            LlRight.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            LlRight.Size = new System.Drawing.Size(330, 429);
            LlRight.SizeNormal = new System.Drawing.Size(330, 140);
            LlRight.SuspendRedraw = false;
            LlRight.TabIndex = 109;
            LlRight.MouseEnter += llRight_MouseEnter;
            LlRight.ItemHover += llRight_ItemHover;
            LlRight.ItemClick += llRight_ItemClick;
            // 
            // LlLeft
            // 
            LlLeft.AutoSize = true;
            LlLeft.Expandable = true;
            LlLeft.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            LlLeft.HighVis = true;
            LlLeft.HoverColor = System.Drawing.Color.WhiteSmoke;
            LlLeft.Location = new System.Drawing.Point(0, 0);
            LlLeft.MaxHeight = 900;
            LlLeft.Name = "LlLeft";
            LlLeft.PaddingX = 2;
            LlLeft.PaddingY = 2;
            LlLeft.Scrollable = false;
            LlLeft.ScrollBarColor = System.Drawing.Color.Red;
            LlLeft.ScrollBarWidth = 11;
            LlLeft.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);
            LlLeft.Size = new System.Drawing.Size(330, 429);
            LlLeft.SizeNormal = new System.Drawing.Size(330, 140);
            LlLeft.SuspendRedraw = false;
            LlLeft.TabIndex = 108;
            LlLeft.ItemClick += llLeft_ItemClick;
            LlLeft.MouseEnter += llLeft_MouseEnter;
            LlLeft.ItemHover += llLeft_ItemHover;
            // 
            // _omegaButton
            // 
            _omegaButton.Checked = false;
            _omegaButton.Enabled = false;
            _omegaButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _omegaButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _omegaButton.Location = new System.Drawing.Point(565, 40);
            _omegaButton.Margin = new Padding(48, 22, 48, 22);
            _omegaButton.Name = "_omegaButton";
            _omegaButton.Size = new System.Drawing.Size(105, 22);
            _omegaButton.TabIndex = 124;
            _omegaButton.TextOff = "Omega";
            _omegaButton.TextOn = "Omega";
            _omegaButton.Toggle = true;
            _omegaButton.ButtonClicked += OmegaButton_ButtonClicked;
            // 
            // _vitaeButton
            // 
            _vitaeButton.Checked = false;
            _vitaeButton.Enabled = false;
            _vitaeButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _vitaeButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _vitaeButton.Location = new System.Drawing.Point(429, 40);
            _vitaeButton.Margin = new Padding(48, 22, 48, 22);
            _vitaeButton.Name = "_vitaeButton";
            _vitaeButton.Size = new System.Drawing.Size(105, 22);
            _vitaeButton.TabIndex = 123;
            _vitaeButton.TextOff = "Vitae";
            _vitaeButton.TextOn = "Vitae";
            _vitaeButton.Toggle = true;
            _vitaeButton.ButtonClicked += VitaeButton_ButtonClicked;
            // 
            // _stanceButton
            // 
            _stanceButton.Checked = false;
            _stanceButton.Enabled = false;
            _stanceButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _stanceButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _stanceButton.Location = new System.Drawing.Point(294, 40);
            _stanceButton.Margin = new Padding(48, 22, 48, 22);
            _stanceButton.Name = "_stanceButton";
            _stanceButton.Size = new System.Drawing.Size(105, 22);
            _stanceButton.TabIndex = 121;
            _stanceButton.TextOff = "Stance";
            _stanceButton.TextOn = "Stance";
            _stanceButton.Toggle = true;
            _stanceButton.ButtonClicked += StanceButton_ButtonClicked;
            // 
            // _genesisButton
            // 
            _genesisButton.Checked = false;
            _genesisButton.Enabled = false;
            _genesisButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _genesisButton.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _genesisButton.Location = new System.Drawing.Point(158, 40);
            _genesisButton.Margin = new Padding(48, 22, 48, 22);
            _genesisButton.Name = "_genesisButton";
            _genesisButton.Size = new System.Drawing.Size(105, 22);
            _genesisButton.TabIndex = 120;
            _genesisButton.TextOff = "Genesis";
            _genesisButton.TextOn = "Genesis";
            _genesisButton.Toggle = true;
            _genesisButton.ButtonClicked += GenesisButton_ButtonClicked;
            // 
            // _hybridBtn
            // 
            _hybridBtn.Checked = false;
            _hybridBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _hybridBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _hybridBtn.Location = new System.Drawing.Point(12, 40);
            _hybridBtn.Margin = new Padding(48, 22, 48, 22);
            _hybridBtn.Name = "_hybridBtn";
            _hybridBtn.Size = new System.Drawing.Size(105, 22);
            _hybridBtn.TabIndex = 119;
            _hybridBtn.TextOff = "Hybrid";
            _hybridBtn.TextOn = "Hybrid";
            _hybridBtn.Toggle = true;
            _hybridBtn.ButtonClicked += hybridBtn_ButtonClicked;
            // 
            // _destinyBtn
            // 
            _destinyBtn.Checked = false;
            _destinyBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _destinyBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _destinyBtn.Location = new System.Drawing.Point(565, 12);
            _destinyBtn.Margin = new Padding(48, 22, 48, 22);
            _destinyBtn.Name = "_destinyBtn";
            _destinyBtn.Size = new System.Drawing.Size(105, 22);
            _destinyBtn.TabIndex = 118;
            _destinyBtn.TextOff = "Destiny";
            _destinyBtn.TextOn = "Destiny";
            _destinyBtn.Toggle = true;
            _destinyBtn.ButtonClicked += destinyBtn_ButtonClicked;
            // 
            // _loreBtn
            // 
            _loreBtn.Checked = false;
            _loreBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _loreBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _loreBtn.Location = new System.Drawing.Point(429, 12);
            _loreBtn.Margin = new Padding(48, 22, 48, 22);
            _loreBtn.Name = "_loreBtn";
            _loreBtn.Size = new System.Drawing.Size(105, 22);
            _loreBtn.TabIndex = 117;
            _loreBtn.TextOff = "Lore";
            _loreBtn.TextOn = "Lore";
            _loreBtn.Toggle = true;
            _loreBtn.ButtonClicked += loreBtn_ButtonClicked;
            // 
            // _interfaceBtn
            // 
            _interfaceBtn.Checked = false;
            _interfaceBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _interfaceBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _interfaceBtn.Location = new System.Drawing.Point(294, 12);
            _interfaceBtn.Margin = new Padding(48, 22, 48, 22);
            _interfaceBtn.Name = "_interfaceBtn";
            _interfaceBtn.Size = new System.Drawing.Size(105, 22);
            _interfaceBtn.TabIndex = 116;
            _interfaceBtn.TextOff = "Interface";
            _interfaceBtn.TextOn = "Interface";
            _interfaceBtn.Toggle = true;
            _interfaceBtn.ButtonClicked += interfaceBtn_ButtonClicked;
            // 
            // _judgementBtn
            // 
            _judgementBtn.Checked = false;
            _judgementBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _judgementBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _judgementBtn.Location = new System.Drawing.Point(158, 12);
            _judgementBtn.Margin = new Padding(48, 22, 48, 22);
            _judgementBtn.Name = "_judgementBtn";
            _judgementBtn.Size = new System.Drawing.Size(105, 22);
            _judgementBtn.TabIndex = 115;
            _judgementBtn.TextOff = "Judgement";
            _judgementBtn.TextOn = "Judgement";
            _judgementBtn.Toggle = true;
            _judgementBtn.ButtonClicked += judgementBtn_ButtonClicked;
            // 
            // _alphaBtn
            // 
            _alphaBtn.Checked = true;
            _alphaBtn.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _alphaBtn.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _alphaBtn.Location = new System.Drawing.Point(12, 12);
            _alphaBtn.Margin = new Padding(48, 22, 48, 22);
            _alphaBtn.Name = "_alphaBtn";
            _alphaBtn.Size = new System.Drawing.Size(105, 22);
            _alphaBtn.TabIndex = 114;
            _alphaBtn.TextOff = "Alpha";
            _alphaBtn.TextOn = "Alpha";
            _alphaBtn.Toggle = true;
            _alphaBtn.ButtonClicked += alphaBtn_ButtonClicked;
            // 
            // _ibClose
            // 
            _ibClose.Checked = false;
            _ibClose.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            _ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            _ibClose.Location = new System.Drawing.Point(294, 527);
            _ibClose.Margin = new Padding(48, 22, 48, 22);
            _ibClose.Name = "_ibClose";
            _ibClose.Size = new System.Drawing.Size(105, 22);
            _ibClose.TabIndex = 7;
            _ibClose.TextOff = "Done";
            _ibClose.TextOn = "Alt Text";
            _ibClose.Toggle = false;
            _ibClose.ButtonClicked += ibClose_ButtonClicked;
            // 
            // FrmIncarnate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(684, 561);
            Controls.Add(_panel2);
            Controls.Add(_omegaButton);
            Controls.Add(_vitaeButton);
            Controls.Add(_stanceButton);
            Controls.Add(_genesisButton);
            Controls.Add(_hybridBtn);
            Controls.Add(_destinyBtn);
            Controls.Add(_loreBtn);
            Controls.Add(_interfaceBtn);
            Controls.Add(_judgementBtn);
            Controls.Add(_alphaBtn);
            Controls.Add(_lblLock);
            Controls.Add(_panel1);
            Controls.Add(_ibClose);
            Font = new System.Drawing.Font("Segoe UI", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "FrmIncarnate";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Incarnate Powers";
            TopMost = true;
            _panel1.ResumeLayout(false);
            _panel2.ResumeLayout(false);
            _panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ImageButton _alphaBtn;
        private ImageButton _destinyBtn;
        private ImageButton _genesisButton;
        private ImageButton _hybridBtn;
        private ImageButton _ibClose;
        private ImageButton _interfaceBtn;
        private ImageButton _judgementBtn;
        private Label _lblLock;
        internal ListLabelV3 LlLeft;
        internal ListLabelV3 LlRight;
        private ImageButton _loreBtn;
        private ImageButton _omegaButton;
        private Panel _panel1;
        private Panel _panel2;
        private ctlPopUp _popInfo;
        private ImageButton _stanceButton;
        private ImageButton _vitaeButton;
        private VScrollBar _vScrollBar1;
    }
}