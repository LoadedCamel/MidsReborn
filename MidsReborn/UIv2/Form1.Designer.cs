using System.Windows.Forms;
using mrbControls;

namespace Mids_Reborn.UIv2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuSlideTimer = new System.Windows.Forms.Timer(this.components);
            this.BGImage = new System.Windows.Forms.PictureBox();
            this.MainPanel = new mrbControls.ctlPanel();
            this.Ancillary_Powers = new mrbControls.ctlPowerList();
            this.cbAncillary = new mrbControls.ctlCombo();
            this.label11 = new System.Windows.Forms.Label();
            this.Pool3_Powers = new mrbControls.ctlPowerList();
            this.cbPool3 = new mrbControls.ctlCombo();
            this.label10 = new System.Windows.Forms.Label();
            this.Pool2_Powers = new mrbControls.ctlPowerList();
            this.cbPool2 = new mrbControls.ctlCombo();
            this.label9 = new System.Windows.Forms.Label();
            this.Pool1_Powers = new mrbControls.ctlPowerList();
            this.cbPool1 = new mrbControls.ctlCombo();
            this.label8 = new System.Windows.Forms.Label();
            this.Pool0_Powers = new mrbControls.ctlPowerList();
            this.cbPool0 = new mrbControls.ctlCombo();
            this.label7 = new System.Windows.Forms.Label();
            this.Secondary_Powers = new mrbControls.ctlPowerList();
            this.Primary_Powers = new mrbControls.ctlPowerList();
            this.cbSecondary = new mrbControls.ctlCombo();
            this.label6 = new System.Windows.Forms.Label();
            this.cbPrimary = new mrbControls.ctlCombo();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOrigin = new mrbControls.ctlCombo();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAT = new mrbControls.ctlCombo();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SlidePanel = new mrbControls.ctlPanel();
            this.MenuGrip = new FontAwesome.Sharp.IconButton();
            this.menuPanel = new mrbControls.ctlPanel();
            this.TopPanel = new mrbControls.ctlPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ExitPanel = new mrbControls.ctlPanel();
            this.MinimizeButton = new FontAwesome.Sharp.IconButton();
            this.CloseButton = new FontAwesome.Sharp.IconButton();
            ((System.ComponentModel.ISupportInitialize)(this.BGImage)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.SlidePanel.SuspendLayout();
            this.TopPanel.SuspendLayout();
            this.ExitPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuSlideTimer
            // 
            this.menuSlideTimer.Interval = 1;
            this.menuSlideTimer.Tick += new System.EventHandler(this.MenuSlideTimer_Tick);
            // 
            // BGImage
            // 
            this.BGImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BGImage.BackgroundImage")));
            this.BGImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BGImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BGImage.Location = new System.Drawing.Point(0, 0);
            this.BGImage.Name = "BGImage";
            this.BGImage.Size = new System.Drawing.Size(1375, 1017);
            this.BGImage.TabIndex = 0;
            this.BGImage.TabStop = false;
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MainPanel.Controls.Add(this.Ancillary_Powers);
            this.MainPanel.Controls.Add(this.cbAncillary);
            this.MainPanel.Controls.Add(this.label11);
            this.MainPanel.Controls.Add(this.Pool3_Powers);
            this.MainPanel.Controls.Add(this.cbPool3);
            this.MainPanel.Controls.Add(this.label10);
            this.MainPanel.Controls.Add(this.Pool2_Powers);
            this.MainPanel.Controls.Add(this.cbPool2);
            this.MainPanel.Controls.Add(this.label9);
            this.MainPanel.Controls.Add(this.Pool1_Powers);
            this.MainPanel.Controls.Add(this.cbPool1);
            this.MainPanel.Controls.Add(this.label8);
            this.MainPanel.Controls.Add(this.Pool0_Powers);
            this.MainPanel.Controls.Add(this.cbPool0);
            this.MainPanel.Controls.Add(this.label7);
            this.MainPanel.Controls.Add(this.Secondary_Powers);
            this.MainPanel.Controls.Add(this.Primary_Powers);
            this.MainPanel.Controls.Add(this.cbSecondary);
            this.MainPanel.Controls.Add(this.label6);
            this.MainPanel.Controls.Add(this.cbPrimary);
            this.MainPanel.Controls.Add(this.label5);
            this.MainPanel.Controls.Add(this.cbOrigin);
            this.MainPanel.Controls.Add(this.label4);
            this.MainPanel.Controls.Add(this.cbAT);
            this.MainPanel.Controls.Add(this.label3);
            this.MainPanel.Controls.Add(this.textBox1);
            this.MainPanel.Controls.Add(this.label2);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(50, 41);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1325, 976);
            this.MainPanel.TabIndex = 3;
            // 
            // Ancillary_Powers
            // 
            this.Ancillary_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Ancillary_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Ancillary_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Ancillary_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ancillary_Powers.ForeColor = System.Drawing.Color.White;
            this.Ancillary_Powers.FormattingEnabled = true;
            this.Ancillary_Powers.ItemHeight = 16;
            this.Ancillary_Powers.Items.AddRange(new object[] {
            "Pool Power 1",
            "Pool Power 2",
            "Pool Power 3",
            "Pool Power 4",
            "Pool Power 5"});
            this.Ancillary_Powers.Location = new System.Drawing.Point(424, 849);
            this.Ancillary_Powers.Name = "Ancillary_Powers";
            this.Ancillary_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Ancillary_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Ancillary_Powers.Size = new System.Drawing.Size(185, 112);
            this.Ancillary_Powers.TabIndex = 33;
            // 
            // cbAncillary
            // 
            this.cbAncillary.ComboType = mrbControls.ctlCombo.ComboBoxType.Ancillary;
            this.cbAncillary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbAncillary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAncillary.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbAncillary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAncillary.FormattingEnabled = true;
            this.cbAncillary.HighlightColor = System.Drawing.Color.Empty;
            this.cbAncillary.Location = new System.Drawing.Point(424, 820);
            this.cbAncillary.Name = "cbAncillary";
            this.cbAncillary.Size = new System.Drawing.Size(185, 23);
            this.cbAncillary.TabIndex = 32;
            this.cbAncillary.SelectedIndexChanged += new System.EventHandler(this.cbAncillary_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(421, 804);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Epic Pool";
            // 
            // Pool3_Powers
            // 
            this.Pool3_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Pool3_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Pool3_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Pool3_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pool3_Powers.ForeColor = System.Drawing.Color.White;
            this.Pool3_Powers.FormattingEnabled = true;
            this.Pool3_Powers.ItemHeight = 16;
            this.Pool3_Powers.Items.AddRange(new object[] {
            "Pool Power 1",
            "Pool Power 2",
            "Pool Power 3",
            "Pool Power 4",
            "Pool Power 5"});
            this.Pool3_Powers.Location = new System.Drawing.Point(424, 677);
            this.Pool3_Powers.Name = "Pool3_Powers";
            this.Pool3_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Pool3_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Pool3_Powers.Size = new System.Drawing.Size(185, 112);
            this.Pool3_Powers.TabIndex = 30;
            // 
            // cbPool3
            // 
            this.cbPool3.ComboType = mrbControls.ctlCombo.ComboBoxType.Pool;
            this.cbPool3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPool3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPool3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPool3.FormattingEnabled = true;
            this.cbPool3.HighlightColor = System.Drawing.Color.Empty;
            this.cbPool3.Location = new System.Drawing.Point(424, 648);
            this.cbPool3.Name = "cbPool3";
            this.cbPool3.Size = new System.Drawing.Size(185, 23);
            this.cbPool3.TabIndex = 29;
            this.cbPool3.SelectedIndexChanged += new System.EventHandler(this.cbPool3_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(421, 632);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Pool 4";
            // 
            // Pool2_Powers
            // 
            this.Pool2_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Pool2_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Pool2_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Pool2_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pool2_Powers.ForeColor = System.Drawing.Color.White;
            this.Pool2_Powers.FormattingEnabled = true;
            this.Pool2_Powers.ItemHeight = 16;
            this.Pool2_Powers.Items.AddRange(new object[] {
            "Pool Power 1",
            "Pool Power 2",
            "Pool Power 3",
            "Pool Power 4",
            "Pool Power 5"});
            this.Pool2_Powers.Location = new System.Drawing.Point(422, 505);
            this.Pool2_Powers.Name = "Pool2_Powers";
            this.Pool2_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Pool2_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Pool2_Powers.Size = new System.Drawing.Size(185, 112);
            this.Pool2_Powers.TabIndex = 27;
            // 
            // cbPool2
            // 
            this.cbPool2.ComboType = mrbControls.ctlCombo.ComboBoxType.Pool;
            this.cbPool2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPool2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPool2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPool2.FormattingEnabled = true;
            this.cbPool2.HighlightColor = System.Drawing.Color.Empty;
            this.cbPool2.Location = new System.Drawing.Point(424, 476);
            this.cbPool2.Name = "cbPool2";
            this.cbPool2.Size = new System.Drawing.Size(185, 23);
            this.cbPool2.TabIndex = 26;
            this.cbPool2.SelectedIndexChanged += new System.EventHandler(this.cbPool2_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(421, 460);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Pool 3";
            // 
            // Pool1_Powers
            // 
            this.Pool1_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Pool1_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Pool1_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Pool1_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pool1_Powers.ForeColor = System.Drawing.Color.White;
            this.Pool1_Powers.FormattingEnabled = true;
            this.Pool1_Powers.ItemHeight = 16;
            this.Pool1_Powers.Items.AddRange(new object[] {
            "Pool Power 1",
            "Pool Power 2",
            "Pool Power 3",
            "Pool Power 4",
            "Pool Power 5"});
            this.Pool1_Powers.Location = new System.Drawing.Point(424, 333);
            this.Pool1_Powers.Name = "Pool1_Powers";
            this.Pool1_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Pool1_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Pool1_Powers.Size = new System.Drawing.Size(185, 112);
            this.Pool1_Powers.TabIndex = 24;
            // 
            // cbPool1
            // 
            this.cbPool1.ComboType = mrbControls.ctlCombo.ComboBoxType.Pool;
            this.cbPool1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPool1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPool1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPool1.FormattingEnabled = true;
            this.cbPool1.HighlightColor = System.Drawing.Color.Empty;
            this.cbPool1.Location = new System.Drawing.Point(424, 304);
            this.cbPool1.Name = "cbPool1";
            this.cbPool1.Size = new System.Drawing.Size(185, 23);
            this.cbPool1.TabIndex = 23;
            this.cbPool1.SelectedIndexChanged += new System.EventHandler(this.cbPool1_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(421, 288);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Pool 2";
            // 
            // Pool0_Powers
            // 
            this.Pool0_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Pool0_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Pool0_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Pool0_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pool0_Powers.ForeColor = System.Drawing.Color.White;
            this.Pool0_Powers.FormattingEnabled = true;
            this.Pool0_Powers.ItemHeight = 16;
            this.Pool0_Powers.Items.AddRange(new object[] {
            "Pool Power 1",
            "Pool Power 2",
            "Pool Power 3",
            "Pool Power 4",
            "Pool Power 5"});
            this.Pool0_Powers.Location = new System.Drawing.Point(424, 161);
            this.Pool0_Powers.Name = "Pool0_Powers";
            this.Pool0_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Pool0_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Pool0_Powers.Size = new System.Drawing.Size(185, 112);
            this.Pool0_Powers.TabIndex = 21;
            // 
            // cbPool0
            // 
            this.cbPool0.ComboType = mrbControls.ctlCombo.ComboBoxType.Pool;
            this.cbPool0.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPool0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPool0.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPool0.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPool0.FormattingEnabled = true;
            this.cbPool0.HighlightColor = System.Drawing.Color.Empty;
            this.cbPool0.Location = new System.Drawing.Point(424, 132);
            this.cbPool0.Name = "cbPool0";
            this.cbPool0.Size = new System.Drawing.Size(185, 23);
            this.cbPool0.TabIndex = 20;
            this.cbPool0.SelectedIndexChanged += new System.EventHandler(this.cbPool0_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(421, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Pool 1";
            // 
            // Secondary_Powers
            // 
            this.Secondary_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Secondary_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Secondary_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Secondary_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Secondary_Powers.ForeColor = System.Drawing.Color.White;
            this.Secondary_Powers.FormattingEnabled = true;
            this.Secondary_Powers.ItemHeight = 16;
            this.Secondary_Powers.Items.AddRange(new object[] {
            "Sec Power 1",
            "Sec Power 2",
            "Sec Power 3",
            "Sec Power 4",
            "Sec Power 5",
            "Sec Power 6",
            "Sec Power 7",
            "Sec Power 8",
            "Sec Power 9"});
            this.Secondary_Powers.Location = new System.Drawing.Point(216, 161);
            this.Secondary_Powers.Name = "Secondary_Powers";
            this.Secondary_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Secondary_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Secondary_Powers.Size = new System.Drawing.Size(200, 320);
            this.Secondary_Powers.TabIndex = 18;
            // 
            // Primary_Powers
            // 
            this.Primary_Powers.BackColor = System.Drawing.Color.Transparent;
            this.Primary_Powers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Primary_Powers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Primary_Powers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Primary_Powers.ForeColor = System.Drawing.Color.White;
            this.Primary_Powers.FormattingEnabled = true;
            this.Primary_Powers.ItemHeight = 16;
            this.Primary_Powers.Items.AddRange(new object[] {
            "Pri Power 1",
            "Pri Power 2",
            "Pri Power 3",
            "Pri Power 4",
            "Pri Power 5",
            "Pri Power 6",
            "Pri Power 7",
            "Pri Power 8",
            "Pri Power 9"});
            this.Primary_Powers.Location = new System.Drawing.Point(10, 161);
            this.Primary_Powers.Name = "Primary_Powers";
            this.Primary_Powers.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.Primary_Powers.SelectionColor = System.Drawing.Color.DodgerBlue;
            this.Primary_Powers.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Primary_Powers.Size = new System.Drawing.Size(200, 304);
            this.Primary_Powers.TabIndex = 17;
            // 
            // cbSecondary
            // 
            this.cbSecondary.ComboType = mrbControls.ctlCombo.ComboBoxType.Secondary;
            this.cbSecondary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecondary.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbSecondary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSecondary.FormattingEnabled = true;
            this.cbSecondary.HighlightColor = System.Drawing.Color.Empty;
            this.cbSecondary.Location = new System.Drawing.Point(216, 132);
            this.cbSecondary.Name = "cbSecondary";
            this.cbSecondary.Size = new System.Drawing.Size(200, 23);
            this.cbSecondary.TabIndex = 16;
            this.cbSecondary.SelectedIndexChanged += new System.EventHandler(this.cbSecondary_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(213, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Secondary Power Set";
            // 
            // cbPrimary
            // 
            this.cbPrimary.ComboType = mrbControls.ctlCombo.ComboBoxType.Primary;
            this.cbPrimary.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrimary.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbPrimary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPrimary.FormattingEnabled = true;
            this.cbPrimary.HighlightColor = System.Drawing.Color.Empty;
            this.cbPrimary.Location = new System.Drawing.Point(9, 132);
            this.cbPrimary.Name = "cbPrimary";
            this.cbPrimary.Size = new System.Drawing.Size(200, 23);
            this.cbPrimary.TabIndex = 14;
            this.cbPrimary.SelectedIndexChanged += new System.EventHandler(this.cbPrimary_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(6, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Primary Power Set";
            // 
            // cbOrigin
            // 
            this.cbOrigin.ComboType = mrbControls.ctlCombo.ComboBoxType.Origin;
            this.cbOrigin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrigin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbOrigin.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbOrigin.FormattingEnabled = true;
            this.cbOrigin.HighlightColor = System.Drawing.Color.Crimson;
            this.cbOrigin.Location = new System.Drawing.Point(109, 72);
            this.cbOrigin.Name = "cbOrigin";
            this.cbOrigin.Size = new System.Drawing.Size(177, 23);
            this.cbOrigin.TabIndex = 12;
            this.cbOrigin.SelectedIndexChanged += new System.EventHandler(this.cbOrigin_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(50, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Origin:";
            // 
            // cbAT
            // 
            this.cbAT.ComboType = mrbControls.ctlCombo.ComboBoxType.Archetype;
            this.cbAT.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbAT.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAT.FormattingEnabled = true;
            this.cbAT.HighlightColor = System.Drawing.Color.Empty;
            this.cbAT.Location = new System.Drawing.Point(109, 42);
            this.cbAT.Name = "cbAT";
            this.cbAT.Size = new System.Drawing.Size(177, 23);
            this.cbAT.TabIndex = 10;
            this.cbAT.SelectedIndexChanged += new System.EventHandler(this.cbAT_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(21, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Archetype:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(109, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(177, 22);
            this.textBox1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(50, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name:";
            // 
            // SlidePanel
            // 
            this.SlidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SlidePanel.Controls.Add(this.MenuGrip);
            this.SlidePanel.Controls.Add(this.menuPanel);
            this.SlidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.SlidePanel.Location = new System.Drawing.Point(0, 41);
            this.SlidePanel.Name = "SlidePanel";
            this.SlidePanel.Size = new System.Drawing.Size(50, 976);
            this.SlidePanel.TabIndex = 2;
            // 
            // MenuGrip
            // 
            this.MenuGrip.BackColor = System.Drawing.Color.Transparent;
            this.MenuGrip.FlatAppearance.BorderSize = 0;
            this.MenuGrip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MenuGrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuGrip.IconChar = FontAwesome.Sharp.IconChar.Bars;
            this.MenuGrip.IconColor = System.Drawing.Color.White;
            this.MenuGrip.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.MenuGrip.IconSize = 32;
            this.MenuGrip.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.MenuGrip.Location = new System.Drawing.Point(9, 6);
            this.MenuGrip.Name = "MenuGrip";
            this.MenuGrip.Size = new System.Drawing.Size(35, 38);
            this.MenuGrip.TabIndex = 0;
            this.MenuGrip.UseVisualStyleBackColor = false;
            this.MenuGrip.Click += new System.EventHandler(this.MenuGrip_Click);
            this.MenuGrip.MouseEnter += new System.EventHandler(this.MenuGrip_Enter);
            this.MenuGrip.MouseLeave += new System.EventHandler(this.MenuGrip_Leave);
            // 
            // menuPanel
            // 
            this.menuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.menuPanel.Location = new System.Drawing.Point(0, 0);
            this.menuPanel.Name = "menuPanel";
            this.menuPanel.Size = new System.Drawing.Size(350, 750);
            this.menuPanel.TabIndex = 34;
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TopPanel.Controls.Add(this.label1);
            this.TopPanel.Controls.Add(this.button1);
            this.TopPanel.Controls.Add(this.ExitPanel);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(1375, 41);
            this.TopPanel.TabIndex = 1;
            this.TopPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Move_OnMouseDown);
            this.TopPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Move_OnMouseMove);
            this.TopPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Move_OnMouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(47, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mids Reborn";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(9, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(31, 23);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // ExitPanel
            // 
            this.ExitPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ExitPanel.Controls.Add(this.MinimizeButton);
            this.ExitPanel.Controls.Add(this.CloseButton);
            this.ExitPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ExitPanel.Location = new System.Drawing.Point(1312, 0);
            this.ExitPanel.Name = "ExitPanel";
            this.ExitPanel.Size = new System.Drawing.Size(63, 41);
            this.ExitPanel.TabIndex = 0;
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.FlatAppearance.BorderSize = 0;
            this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinimizeButton.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
            this.MinimizeButton.IconColor = System.Drawing.Color.White;
            this.MinimizeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.MinimizeButton.IconSize = 32;
            this.MinimizeButton.Location = new System.Drawing.Point(3, 10);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(23, 23);
            this.MinimizeButton.TabIndex = 2;
            this.MinimizeButton.UseVisualStyleBackColor = true;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            this.MinimizeButton.MouseEnter += new System.EventHandler(this.ButtonMouse_Enter);
            this.MinimizeButton.MouseLeave += new System.EventHandler(this.ButtonMouse_Leave);
            // 
            // CloseButton
            // 
            this.CloseButton.FlatAppearance.BorderSize = 0;
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.IconChar = FontAwesome.Sharp.IconChar.WindowClose;
            this.CloseButton.IconColor = System.Drawing.Color.White;
            this.CloseButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.CloseButton.IconSize = 32;
            this.CloseButton.Location = new System.Drawing.Point(32, 10);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(23, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            this.CloseButton.MouseEnter += new System.EventHandler(this.ButtonMouse_Enter);
            this.CloseButton.MouseLeave += new System.EventHandler(this.ButtonMouse_Leave);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1375, 1017);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.SlidePanel);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.BGImage);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mids` Reborn : Hero Designer";
            ((System.ComponentModel.ISupportInitialize)(this.BGImage)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.SlidePanel.ResumeLayout(false);
            this.TopPanel.ResumeLayout(false);
            this.TopPanel.PerformLayout();
            this.ExitPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer menuSlideTimer;
        private PictureBox BGImage;
        private ctlPanel TopPanel;
        private ctlPanel ExitPanel;
        private ctlPanel SlidePanel;
        private ctlPanel MainPanel;
        private FontAwesome.Sharp.IconButton CloseButton;
        private FontAwesome.Sharp.IconButton MinimizeButton;
        private FontAwesome.Sharp.IconButton MenuGrip;
        private Button button1;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private ctlCombo cbAT;
        private Label label3;
        private ctlCombo cbOrigin;
        private Label label4;
        private ctlCombo cbSecondary;
        private Label label6;
        private ctlCombo cbPrimary;
        private Label label5;
        private ctlPowerList Secondary_Powers;
        private ctlPowerList Primary_Powers;
        private Label label9;
        private ctlPowerList Pool1_Powers;
        private ctlCombo cbPool1;
        private Label label8;
        private ctlPowerList Pool0_Powers;
        private ctlCombo cbPool0;
        private Label label7;
        private ctlPowerList Pool3_Powers;
        private ctlCombo cbPool3;
        private Label label10;
        private ctlPowerList Pool2_Powers;
        private ctlCombo cbPool2;
        private ctlPowerList Ancillary_Powers;
        private ctlCombo cbAncillary;
        private Label label11;
        private ctlPanel menuPanel;
    }
}