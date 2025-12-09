using System.Windows.Forms;
using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms
{
    partial class FrmTeam
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTeam));
            teamPanel = new Panel();
            tbTotalTeam = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            udWidow = new EnhancedUpDown();
            udSoldier = new EnhancedUpDown();
            udCorruptor = new EnhancedUpDown();
            udDominator = new EnhancedUpDown();
            udMastermind = new EnhancedUpDown();
            udStalker = new EnhancedUpDown();
            udBrute = new EnhancedUpDown();
            udSentGuard = new EnhancedUpDown();
            udWarshade = new EnhancedUpDown();
            udPeacebringer = new EnhancedUpDown();
            udTanker = new EnhancedUpDown();
            udScrapper = new EnhancedUpDown();
            udDefender = new EnhancedUpDown();
            udController = new EnhancedUpDown();
            udBlaster = new EnhancedUpDown();
            label16 = new Label();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label2 = new Label();
            label3 = new Label();
            label1 = new Label();
            udAny = new EnhancedUpDown();
            label17 = new Label();
            panel2 = new Panel();
            btnToTop = new ImageButtonEx();
            btnSave = new ImageButtonEx();
            btnCancel = new ImageButtonEx();
            tabStrip = new ctlTotalsTabStrip();
            playerPanel = new Panel();
            lblPlayerEnd = new Label();
            playerEnd = new Mids_Reborn.UI.Controls.CtlMultiGraph();
            label22 = new Label();
            lblPlayerHP = new Label();
            playerHP = new Mids_Reborn.UI.Controls.CtlMultiGraph();
            rbPlayerStatusDead = new RadioButton();
            rbPlayerStatusAlive = new RadioButton();
            label19 = new Label();
            label18 = new Label();
            targetPanel = new Panel();
            lblTargetEnd = new Label();
            targetEnd = new Mids_Reborn.UI.Controls.CtlMultiGraph();
            label23 = new Label();
            lblTargetHP = new Label();
            targetHP = new Mids_Reborn.UI.Controls.CtlMultiGraph();
            label21 = new Label();
            teamPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)udWidow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udSoldier).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udCorruptor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udDominator).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udMastermind).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udStalker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udBrute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udSentGuard).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udWarshade).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udPeacebringer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udTanker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udScrapper).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udDefender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udController).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udBlaster).BeginInit();
            ((System.ComponentModel.ISupportInitialize)udAny).BeginInit();
            panel2.SuspendLayout();
            playerPanel.SuspendLayout();
            targetPanel.SuspendLayout();
            SuspendLayout();
            // 
            // teamPanel
            // 
            teamPanel.Controls.Add(tbTotalTeam);
            teamPanel.Controls.Add(tableLayoutPanel1);
            teamPanel.Controls.Add(label17);
            teamPanel.Location = new System.Drawing.Point(0, 0);
            teamPanel.Name = "teamPanel";
            teamPanel.Size = new System.Drawing.Size(587, 363);
            teamPanel.TabIndex = 0;
            // 
            // tbTotalTeam
            // 
            tbTotalTeam.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            tbTotalTeam.BorderStyle = BorderStyle.FixedSingle;
            tbTotalTeam.ForeColor = System.Drawing.Color.WhiteSmoke;
            tbTotalTeam.Location = new System.Drawing.Point(173, 36);
            tbTotalTeam.Margin = new Padding(4, 3, 4, 3);
            tbTotalTeam.Name = "tbTotalTeam";
            tbTotalTeam.ReadOnly = true;
            tbTotalTeam.Size = new System.Drawing.Size(19, 23);
            tbTotalTeam.TabIndex = 8;
            tbTotalTeam.Text = "0";
            tbTotalTeam.TextAlign = HorizontalAlignment.Center;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 73.18841F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.81159F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 265F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54F));
            tableLayoutPanel1.Controls.Add(udWidow, 3, 7);
            tableLayoutPanel1.Controls.Add(udSoldier, 3, 6);
            tableLayoutPanel1.Controls.Add(udCorruptor, 3, 5);
            tableLayoutPanel1.Controls.Add(udDominator, 3, 4);
            tableLayoutPanel1.Controls.Add(udMastermind, 3, 3);
            tableLayoutPanel1.Controls.Add(udStalker, 3, 2);
            tableLayoutPanel1.Controls.Add(udBrute, 3, 1);
            tableLayoutPanel1.Controls.Add(udSentGuard, 3, 0);
            tableLayoutPanel1.Controls.Add(udWarshade, 1, 7);
            tableLayoutPanel1.Controls.Add(udPeacebringer, 1, 6);
            tableLayoutPanel1.Controls.Add(udTanker, 1, 5);
            tableLayoutPanel1.Controls.Add(udScrapper, 1, 4);
            tableLayoutPanel1.Controls.Add(udDefender, 1, 3);
            tableLayoutPanel1.Controls.Add(udController, 1, 2);
            tableLayoutPanel1.Controls.Add(udBlaster, 1, 1);
            tableLayoutPanel1.Controls.Add(label16, 2, 7);
            tableLayoutPanel1.Controls.Add(label15, 2, 6);
            tableLayoutPanel1.Controls.Add(label14, 2, 5);
            tableLayoutPanel1.Controls.Add(label13, 2, 4);
            tableLayoutPanel1.Controls.Add(label12, 2, 3);
            tableLayoutPanel1.Controls.Add(label11, 2, 2);
            tableLayoutPanel1.Controls.Add(label10, 2, 1);
            tableLayoutPanel1.Controls.Add(label9, 2, 0);
            tableLayoutPanel1.Controls.Add(label8, 0, 7);
            tableLayoutPanel1.Controls.Add(label7, 0, 6);
            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(udAny, 1, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(59, 98);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 51.38889F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 48.61111F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.Size = new System.Drawing.Size(472, 194);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // udWidow
            // 
            udWidow.BackColor = System.Drawing.Color.Black;
            udWidow.BorderStyle = BorderStyle.None;
            udWidow.ForeColor = System.Drawing.Color.WhiteSmoke;
            udWidow.Location = new System.Drawing.Point(421, 173);
            udWidow.Margin = new Padding(4, 3, 4, 3);
            udWidow.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udWidow.Name = "udWidow";
            udWidow.Size = new System.Drawing.Size(35, 19);
            udWidow.TabIndex = 2;
            udWidow.UpButtonClicked += OnUpClicked;
            udWidow.DownButtonClicked += OnDownClicked;
            // 
            // udSoldier
            // 
            udSoldier.BackColor = System.Drawing.Color.Black;
            udSoldier.BorderStyle = BorderStyle.None;
            udSoldier.ForeColor = System.Drawing.Color.WhiteSmoke;
            udSoldier.Location = new System.Drawing.Point(421, 150);
            udSoldier.Margin = new Padding(4, 3, 4, 3);
            udSoldier.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udSoldier.Name = "udSoldier";
            udSoldier.Size = new System.Drawing.Size(35, 19);
            udSoldier.TabIndex = 2;
            udSoldier.UpButtonClicked += OnUpClicked;
            udSoldier.DownButtonClicked += OnDownClicked;
            // 
            // udCorruptor
            // 
            udCorruptor.BackColor = System.Drawing.Color.Black;
            udCorruptor.BorderStyle = BorderStyle.None;
            udCorruptor.ForeColor = System.Drawing.Color.WhiteSmoke;
            udCorruptor.Location = new System.Drawing.Point(421, 127);
            udCorruptor.Margin = new Padding(4, 3, 4, 3);
            udCorruptor.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udCorruptor.Name = "udCorruptor";
            udCorruptor.Size = new System.Drawing.Size(35, 19);
            udCorruptor.TabIndex = 2;
            udCorruptor.UpButtonClicked += OnUpClicked;
            udCorruptor.DownButtonClicked += OnDownClicked;
            // 
            // udDominator
            // 
            udDominator.BackColor = System.Drawing.Color.Black;
            udDominator.BorderStyle = BorderStyle.None;
            udDominator.ForeColor = System.Drawing.Color.WhiteSmoke;
            udDominator.Location = new System.Drawing.Point(421, 104);
            udDominator.Margin = new Padding(4, 3, 4, 3);
            udDominator.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udDominator.Name = "udDominator";
            udDominator.Size = new System.Drawing.Size(35, 19);
            udDominator.TabIndex = 2;
            udDominator.UpButtonClicked += OnUpClicked;
            udDominator.DownButtonClicked += OnDownClicked;
            // 
            // udMastermind
            // 
            udMastermind.BackColor = System.Drawing.Color.Black;
            udMastermind.BorderStyle = BorderStyle.None;
            udMastermind.ForeColor = System.Drawing.Color.WhiteSmoke;
            udMastermind.Location = new System.Drawing.Point(421, 81);
            udMastermind.Margin = new Padding(4, 3, 4, 3);
            udMastermind.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udMastermind.Name = "udMastermind";
            udMastermind.Size = new System.Drawing.Size(35, 19);
            udMastermind.TabIndex = 2;
            udMastermind.UpButtonClicked += OnUpClicked;
            udMastermind.DownButtonClicked += OnDownClicked;
            // 
            // udStalker
            // 
            udStalker.BackColor = System.Drawing.Color.Black;
            udStalker.BorderStyle = BorderStyle.None;
            udStalker.ForeColor = System.Drawing.Color.WhiteSmoke;
            udStalker.Location = new System.Drawing.Point(421, 58);
            udStalker.Margin = new Padding(4, 3, 4, 3);
            udStalker.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udStalker.Name = "udStalker";
            udStalker.Size = new System.Drawing.Size(35, 19);
            udStalker.TabIndex = 2;
            udStalker.UpButtonClicked += OnUpClicked;
            udStalker.DownButtonClicked += OnDownClicked;
            // 
            // udBrute
            // 
            udBrute.BackColor = System.Drawing.Color.Black;
            udBrute.BorderStyle = BorderStyle.None;
            udBrute.ForeColor = System.Drawing.Color.WhiteSmoke;
            udBrute.Location = new System.Drawing.Point(421, 31);
            udBrute.Margin = new Padding(4, 3, 4, 3);
            udBrute.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udBrute.Name = "udBrute";
            udBrute.Size = new System.Drawing.Size(35, 19);
            udBrute.TabIndex = 2;
            udBrute.UpButtonClicked += OnUpClicked;
            udBrute.DownButtonClicked += OnDownClicked;
            // 
            // udSentGuard
            // 
            udSentGuard.BackColor = System.Drawing.Color.Black;
            udSentGuard.BorderStyle = BorderStyle.None;
            udSentGuard.ForeColor = System.Drawing.Color.WhiteSmoke;
            udSentGuard.Location = new System.Drawing.Point(421, 3);
            udSentGuard.Margin = new Padding(4, 3, 4, 3);
            udSentGuard.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udSentGuard.Name = "udSentGuard";
            udSentGuard.Size = new System.Drawing.Size(35, 19);
            udSentGuard.TabIndex = 2;
            udSentGuard.UpButtonClicked += OnUpClicked;
            udSentGuard.DownButtonClicked += OnDownClicked;
            // 
            // udWarshade
            // 
            udWarshade.BackColor = System.Drawing.Color.Black;
            udWarshade.BorderStyle = BorderStyle.None;
            udWarshade.ForeColor = System.Drawing.Color.WhiteSmoke;
            udWarshade.Location = new System.Drawing.Point(115, 173);
            udWarshade.Margin = new Padding(4, 3, 4, 3);
            udWarshade.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udWarshade.Name = "udWarshade";
            udWarshade.Size = new System.Drawing.Size(33, 19);
            udWarshade.TabIndex = 2;
            udWarshade.UpButtonClicked += OnUpClicked;
            udWarshade.DownButtonClicked += OnDownClicked;
            // 
            // udPeacebringer
            // 
            udPeacebringer.BackColor = System.Drawing.Color.Black;
            udPeacebringer.BorderStyle = BorderStyle.None;
            udPeacebringer.ForeColor = System.Drawing.Color.WhiteSmoke;
            udPeacebringer.Location = new System.Drawing.Point(115, 150);
            udPeacebringer.Margin = new Padding(4, 3, 4, 3);
            udPeacebringer.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udPeacebringer.Name = "udPeacebringer";
            udPeacebringer.Size = new System.Drawing.Size(33, 19);
            udPeacebringer.TabIndex = 2;
            udPeacebringer.UpButtonClicked += OnUpClicked;
            udPeacebringer.DownButtonClicked += OnDownClicked;
            // 
            // udTanker
            // 
            udTanker.BackColor = System.Drawing.Color.Black;
            udTanker.BorderStyle = BorderStyle.None;
            udTanker.ForeColor = System.Drawing.Color.WhiteSmoke;
            udTanker.Location = new System.Drawing.Point(115, 127);
            udTanker.Margin = new Padding(4, 3, 4, 3);
            udTanker.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udTanker.Name = "udTanker";
            udTanker.Size = new System.Drawing.Size(33, 19);
            udTanker.TabIndex = 2;
            udTanker.UpButtonClicked += OnUpClicked;
            udTanker.DownButtonClicked += OnDownClicked;
            // 
            // udScrapper
            // 
            udScrapper.BackColor = System.Drawing.Color.Black;
            udScrapper.BorderStyle = BorderStyle.None;
            udScrapper.ForeColor = System.Drawing.Color.WhiteSmoke;
            udScrapper.Location = new System.Drawing.Point(115, 104);
            udScrapper.Margin = new Padding(4, 3, 4, 3);
            udScrapper.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udScrapper.Name = "udScrapper";
            udScrapper.Size = new System.Drawing.Size(33, 19);
            udScrapper.TabIndex = 2;
            udScrapper.UpButtonClicked += OnUpClicked;
            udScrapper.DownButtonClicked += OnDownClicked;
            // 
            // udDefender
            // 
            udDefender.BackColor = System.Drawing.Color.Black;
            udDefender.BorderStyle = BorderStyle.None;
            udDefender.ForeColor = System.Drawing.Color.WhiteSmoke;
            udDefender.Location = new System.Drawing.Point(115, 81);
            udDefender.Margin = new Padding(4, 3, 4, 3);
            udDefender.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udDefender.Name = "udDefender";
            udDefender.Size = new System.Drawing.Size(33, 19);
            udDefender.TabIndex = 2;
            udDefender.UpButtonClicked += OnUpClicked;
            udDefender.DownButtonClicked += OnDownClicked;
            // 
            // udController
            // 
            udController.BackColor = System.Drawing.Color.Black;
            udController.BorderStyle = BorderStyle.None;
            udController.ForeColor = System.Drawing.Color.WhiteSmoke;
            udController.Location = new System.Drawing.Point(115, 58);
            udController.Margin = new Padding(4, 3, 4, 3);
            udController.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udController.Name = "udController";
            udController.Size = new System.Drawing.Size(33, 19);
            udController.TabIndex = 2;
            udController.UpButtonClicked += OnUpClicked;
            udController.DownButtonClicked += OnDownClicked;
            // 
            // udBlaster
            // 
            udBlaster.BackColor = System.Drawing.Color.Black;
            udBlaster.BorderStyle = BorderStyle.None;
            udBlaster.ForeColor = System.Drawing.Color.WhiteSmoke;
            udBlaster.Location = new System.Drawing.Point(115, 31);
            udBlaster.Margin = new Padding(4, 3, 4, 3);
            udBlaster.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udBlaster.Name = "udBlaster";
            udBlaster.Size = new System.Drawing.Size(33, 19);
            udBlaster.TabIndex = 2;
            udBlaster.UpButtonClicked += OnUpClicked;
            udBlaster.DownButtonClicked += OnDownClicked;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Dock = DockStyle.Fill;
            label16.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label16.ForeColor = System.Drawing.Color.WhiteSmoke;
            label16.Location = new System.Drawing.Point(156, 170);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(257, 24);
            label16.TabIndex = 2;
            label16.Text = "Arachnos Widow:";
            label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Dock = DockStyle.Fill;
            label15.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label15.ForeColor = System.Drawing.Color.WhiteSmoke;
            label15.Location = new System.Drawing.Point(156, 147);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(257, 23);
            label15.TabIndex = 2;
            label15.Text = "Arachnos Soldier:";
            label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Dock = DockStyle.Fill;
            label14.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label14.ForeColor = System.Drawing.Color.WhiteSmoke;
            label14.Location = new System.Drawing.Point(156, 124);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(257, 23);
            label14.TabIndex = 2;
            label14.Text = "Corruptor:";
            label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Dock = DockStyle.Fill;
            label13.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label13.ForeColor = System.Drawing.Color.WhiteSmoke;
            label13.Location = new System.Drawing.Point(156, 101);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(257, 23);
            label13.TabIndex = 2;
            label13.Text = "Dominator:";
            label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Dock = DockStyle.Fill;
            label12.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label12.ForeColor = System.Drawing.Color.WhiteSmoke;
            label12.Location = new System.Drawing.Point(156, 78);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(257, 23);
            label12.TabIndex = 2;
            label12.Text = "Mastermind:";
            label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Dock = DockStyle.Fill;
            label11.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label11.ForeColor = System.Drawing.Color.WhiteSmoke;
            label11.Location = new System.Drawing.Point(156, 55);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(257, 23);
            label11.TabIndex = 2;
            label11.Text = "Stalker:";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Fill;
            label10.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label10.ForeColor = System.Drawing.Color.WhiteSmoke;
            label10.Location = new System.Drawing.Point(156, 28);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(257, 27);
            label10.TabIndex = 2;
            label10.Text = "Brute:";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label9.ForeColor = System.Drawing.Color.WhiteSmoke;
            label9.Location = new System.Drawing.Point(156, 0);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(257, 28);
            label9.TabIndex = 2;
            label9.Text = "Sentinel/Guardian:";
            label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Fill;
            label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label8.ForeColor = System.Drawing.Color.WhiteSmoke;
            label8.Location = new System.Drawing.Point(4, 170);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(103, 24);
            label8.TabIndex = 2;
            label8.Text = "Warshade:";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Fill;
            label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            label7.Location = new System.Drawing.Point(4, 147);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(103, 23);
            label7.TabIndex = 2;
            label7.Text = "Peacebringer:";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label6.ForeColor = System.Drawing.Color.WhiteSmoke;
            label6.Location = new System.Drawing.Point(4, 124);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(103, 23);
            label6.TabIndex = 2;
            label6.Text = "Tanker:";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            label5.Location = new System.Drawing.Point(4, 101);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(103, 23);
            label5.TabIndex = 2;
            label5.Text = "Scrapper:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            label4.Location = new System.Drawing.Point(4, 78);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(103, 23);
            label4.TabIndex = 2;
            label4.Text = "Defender:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            label2.Location = new System.Drawing.Point(4, 28);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(103, 27);
            label2.TabIndex = 1;
            label2.Text = "Blaster:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            label3.Location = new System.Drawing.Point(4, 55);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(103, 23);
            label3.TabIndex = 2;
            label3.Text = "Controller:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(103, 28);
            label1.TabIndex = 0;
            label1.Text = "Any:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // udAny
            // 
            udAny.BackColor = System.Drawing.Color.Black;
            udAny.BorderStyle = BorderStyle.None;
            udAny.ForeColor = System.Drawing.Color.WhiteSmoke;
            udAny.Location = new System.Drawing.Point(115, 3);
            udAny.Margin = new Padding(4, 3, 4, 3);
            udAny.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            udAny.Name = "udAny";
            udAny.Size = new System.Drawing.Size(33, 19);
            udAny.TabIndex = 1;
            udAny.UpButtonClicked += OnUpClicked;
            udAny.DownButtonClicked += OnDownClicked;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label17.ForeColor = System.Drawing.Color.WhiteSmoke;
            label17.Location = new System.Drawing.Point(15, 38);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(117, 13);
            label17.TabIndex = 7;
            label17.Text = "Total Team Members:";
            // 
            // panel2
            // 
            panel2.Controls.Add(btnToTop);
            panel2.Controls.Add(btnSave);
            panel2.Controls.Add(btnCancel);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 321);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(1778, 42);
            panel2.TabIndex = 1;
            // 
            // btnToTop
            // 
            btnToTop.BackgroundImageLayout = ImageLayout.None;
            btnToTop.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            btnToTop.CurrentText = "ToggledOn State";
            btnToTop.DisplayVertically = false;
            btnToTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnToTop.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnToTop.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnToTop.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnToTop.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnToTop.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnToTop.Location = new System.Drawing.Point(15, 9);
            btnToTop.Lock = false;
            btnToTop.Name = "btnToTop";
            btnToTop.Size = new System.Drawing.Size(122, 25);
            btnToTop.TabIndex = 15;
            btnToTop.Text = "Top Most";
            btnToTop.TextOutline.Color = System.Drawing.Color.Black;
            btnToTop.TextOutline.Width = 2;
            btnToTop.ToggleState = ImageButtonEx.States.ToggledOn;
            btnToTop.ToggleText.Indeterminate = "Indeterminate State";
            btnToTop.ToggleText.ToggledOff = "To Top";
            btnToTop.ToggleText.ToggledOn = "Top Most";
            btnToTop.UseAlt = false;
            btnToTop.Click += btnToTop_Click;
            // 
            // btnSave
            // 
            btnSave.BackgroundImageLayout = ImageLayout.None;
            btnSave.CurrentText = "Save & Close";
            btnSave.DisplayVertically = false;
            btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnSave.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnSave.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnSave.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnSave.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnSave.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnSave.Location = new System.Drawing.Point(447, 9);
            btnSave.Lock = false;
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(122, 25);
            btnSave.TabIndex = 14;
            btnSave.Text = "Save & Close";
            btnSave.TextOutline.Color = System.Drawing.Color.Black;
            btnSave.TextOutline.Width = 2;
            btnSave.ToggleState = ImageButtonEx.States.ToggledOff;
            btnSave.ToggleText.Indeterminate = "Indeterminate State";
            btnSave.ToggleText.ToggledOff = "ToggledOff State";
            btnSave.ToggleText.ToggledOn = "ToggledOn State";
            btnSave.UseAlt = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackgroundImageLayout = ImageLayout.None;
            btnCancel.CurrentText = "Cancel";
            btnCancel.DisplayVertically = false;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCancel.Location = new System.Drawing.Point(299, 9);
            btnCancel.Lock = false;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(122, 25);
            btnCancel.TabIndex = 13;
            btnCancel.Text = "Cancel";
            btnCancel.TextOutline.Color = System.Drawing.Color.Black;
            btnCancel.TextOutline.Width = 2;
            btnCancel.ToggleState = ImageButtonEx.States.ToggledOff;
            btnCancel.ToggleText.Indeterminate = "Indeterminate State";
            btnCancel.ToggleText.ToggledOff = "ToggledOff State";
            btnCancel.ToggleText.ToggledOn = "ToggledOn State";
            btnCancel.UseAlt = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // tabStrip
            // 
            tabStrip.ActiveTabColor = System.Drawing.Color.Goldenrod;
            tabStrip.BackColor = System.Drawing.Color.Black;
            tabStrip.DimmedBackgroundColor = System.Drawing.Color.FromArgb(21, 61, 93);
            tabStrip.Dock = DockStyle.Top;
            tabStrip.ForeColor = System.Drawing.Color.WhiteSmoke;
            tabStrip.InactiveHoveredTabColor = System.Drawing.Color.FromArgb(43, 122, 187);
            tabStrip.InactiveTabColor = System.Drawing.Color.FromArgb(30, 85, 130);
            tabStrip.ItemPadding = 18;
            tabStrip.Location = new System.Drawing.Point(0, 0);
            tabStrip.Name = "tabStrip";
            tabStrip.OutlineText = true;
            tabStrip.Size = new System.Drawing.Size(1778, 24);
            tabStrip.StripLineColor = System.Drawing.Color.Goldenrod;
            tabStrip.TabIndex = 2;
            tabStrip.UseDimmedBackground = false;
            tabStrip.TabClick += tabStrip_TabClick;
            // 
            // playerPanel
            // 
            playerPanel.Controls.Add(lblPlayerEnd);
            playerPanel.Controls.Add(playerEnd);
            playerPanel.Controls.Add(label22);
            playerPanel.Controls.Add(lblPlayerHP);
            playerPanel.Controls.Add(playerHP);
            playerPanel.Controls.Add(rbPlayerStatusDead);
            playerPanel.Controls.Add(rbPlayerStatusAlive);
            playerPanel.Controls.Add(label19);
            playerPanel.Controls.Add(label18);
            playerPanel.Location = new System.Drawing.Point(593, 0);
            playerPanel.Name = "playerPanel";
            playerPanel.Size = new System.Drawing.Size(587, 363);
            playerPanel.TabIndex = 3;
            // 
            // lblPlayerEnd
            // 
            lblPlayerEnd.AutoSize = true;
            lblPlayerEnd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblPlayerEnd.Location = new System.Drawing.Point(484, 185);
            lblPlayerEnd.Name = "lblPlayerEnd";
            lblPlayerEnd.Size = new System.Drawing.Size(38, 15);
            lblPlayerEnd.TabIndex = 82;
            lblPlayerEnd.Text = "100%";
            // 
            // playerEnd
            // 
            playerEnd.BackColor = System.Drawing.Color.Black;
            playerEnd.BackgroundImage = (System.Drawing.Image)resources.GetObject("playerEnd.BackgroundImage");
            playerEnd.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerEnd.BaseBarColors");
            playerEnd.Border = true;
            playerEnd.BorderColor = System.Drawing.Color.Black;
            playerEnd.Clickable = true;
            playerEnd.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            playerEnd.ColorBase = System.Drawing.Color.FromArgb(64, 153, 255);
            playerEnd.ColorEnh = System.Drawing.Color.Yellow;
            playerEnd.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 89, 191);
            playerEnd.ColorFadeStart = System.Drawing.Color.Black;
            playerEnd.ColorHighlight = System.Drawing.Color.Gray;
            playerEnd.ColorLines = System.Drawing.Color.Black;
            playerEnd.ColorMarkerInner = System.Drawing.Color.Red;
            playerEnd.ColorMarkerOuter = System.Drawing.Color.Black;
            playerEnd.ColorOvercap = System.Drawing.Color.Black;
            playerEnd.DifferentiateColors = false;
            playerEnd.DrawRuler = false;
            playerEnd.Dual = false;
            playerEnd.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerEnd.EnhBarColors");
            playerEnd.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            playerEnd.ForcedMax = 0F;
            playerEnd.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            playerEnd.Highlight = false;
            playerEnd.ItemFontSizeOverride = 0F;
            playerEnd.ItemHeight = 10;
            playerEnd.Lines = true;
            playerEnd.Location = new System.Drawing.Point(37, 185);
            playerEnd.MarkerValue = 0F;
            playerEnd.Max = 100F;
            playerEnd.MaxItems = 1;
            playerEnd.Name = "playerEnd";
            playerEnd.OuterBorder = false;
            playerEnd.Overcap = false;
            playerEnd.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerEnd.OvercapColors");
            playerEnd.PaddingX = 2F;
            playerEnd.PaddingY = 2F;
            playerEnd.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("playerEnd.PerItemScales");
            playerEnd.RulerPos = CtlMultiGraph.RulerPosition.Top;
            playerEnd.ScaleHeight = 32;
            playerEnd.ScaleIndex = 8;
            playerEnd.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            playerEnd.ShowScale = false;
            playerEnd.SingleLineLabels = true;
            playerEnd.Size = new System.Drawing.Size(441, 15);
            playerEnd.Style = Core.Enums.GraphStyle.baseOnly;
            playerEnd.TabIndex = 81;
            playerEnd.TextWidth = 80;
            playerEnd.BarClick += playerEnd_BarClick;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label22.ForeColor = System.Drawing.Color.WhiteSmoke;
            label22.Location = new System.Drawing.Point(19, 159);
            label22.Margin = new Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(43, 13);
            label22.TabIndex = 80;
            label22.Text = "End %:";
            // 
            // lblPlayerHP
            // 
            lblPlayerHP.AutoSize = true;
            lblPlayerHP.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblPlayerHP.Location = new System.Drawing.Point(484, 110);
            lblPlayerHP.Name = "lblPlayerHP";
            lblPlayerHP.Size = new System.Drawing.Size(38, 15);
            lblPlayerHP.TabIndex = 73;
            lblPlayerHP.Text = "100%";
            // 
            // playerHP
            // 
            playerHP.BackColor = System.Drawing.Color.Black;
            playerHP.BackgroundImage = (System.Drawing.Image)resources.GetObject("playerHP.BackgroundImage");
            playerHP.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerHP.BaseBarColors");
            playerHP.Border = true;
            playerHP.BorderColor = System.Drawing.Color.Black;
            playerHP.Clickable = true;
            playerHP.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            playerHP.ColorBase = System.Drawing.Color.FromArgb(64, 255, 64);
            playerHP.ColorEnh = System.Drawing.Color.Yellow;
            playerHP.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 192, 0);
            playerHP.ColorFadeStart = System.Drawing.Color.Black;
            playerHP.ColorHighlight = System.Drawing.Color.Gray;
            playerHP.ColorLines = System.Drawing.Color.Black;
            playerHP.ColorMarkerInner = System.Drawing.Color.Red;
            playerHP.ColorMarkerOuter = System.Drawing.Color.Black;
            playerHP.ColorOvercap = System.Drawing.Color.Black;
            playerHP.DifferentiateColors = false;
            playerHP.DrawRuler = false;
            playerHP.Dual = false;
            playerHP.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerHP.EnhBarColors");
            playerHP.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            playerHP.ForcedMax = 0F;
            playerHP.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            playerHP.Highlight = false;
            playerHP.ItemFontSizeOverride = 0F;
            playerHP.ItemHeight = 10;
            playerHP.Lines = true;
            playerHP.Location = new System.Drawing.Point(37, 110);
            playerHP.MarkerValue = 0F;
            playerHP.Max = 100F;
            playerHP.MaxItems = 1;
            playerHP.Name = "playerHP";
            playerHP.OuterBorder = false;
            playerHP.Overcap = false;
            playerHP.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("playerHP.OvercapColors");
            playerHP.PaddingX = 2F;
            playerHP.PaddingY = 2F;
            playerHP.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("playerHP.PerItemScales");
            playerHP.RulerPos = CtlMultiGraph.RulerPosition.Top;
            playerHP.ScaleHeight = 32;
            playerHP.ScaleIndex = 8;
            playerHP.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            playerHP.ShowScale = false;
            playerHP.SingleLineLabels = true;
            playerHP.Size = new System.Drawing.Size(441, 15);
            playerHP.Style = Core.Enums.GraphStyle.baseOnly;
            playerHP.TabIndex = 72;
            playerHP.TextWidth = 80;
            playerHP.BarClick += playerHP_BarClick;
            // 
            // rbPlayerStatusDead
            // 
            rbPlayerStatusDead.AutoSize = true;
            rbPlayerStatusDead.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            rbPlayerStatusDead.Location = new System.Drawing.Point(171, 255);
            rbPlayerStatusDead.Name = "rbPlayerStatusDead";
            rbPlayerStatusDead.Size = new System.Drawing.Size(52, 17);
            rbPlayerStatusDead.TabIndex = 11;
            rbPlayerStatusDead.TabStop = true;
            rbPlayerStatusDead.Text = "Dead";
            rbPlayerStatusDead.UseVisualStyleBackColor = true;
            rbPlayerStatusDead.CheckedChanged += rbPlayerStatusDead_CheckedChanged;
            // 
            // rbPlayerStatusAlive
            // 
            rbPlayerStatusAlive.AutoSize = true;
            rbPlayerStatusAlive.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            rbPlayerStatusAlive.Location = new System.Drawing.Point(37, 255);
            rbPlayerStatusAlive.Name = "rbPlayerStatusAlive";
            rbPlayerStatusAlive.Size = new System.Drawing.Size(49, 17);
            rbPlayerStatusAlive.TabIndex = 10;
            rbPlayerStatusAlive.TabStop = true;
            rbPlayerStatusAlive.Text = "Alive";
            rbPlayerStatusAlive.UseVisualStyleBackColor = true;
            rbPlayerStatusAlive.CheckedChanged += rbPlayerStatusAlive_CheckedChanged;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label19.ForeColor = System.Drawing.Color.WhiteSmoke;
            label19.Location = new System.Drawing.Point(19, 228);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(42, 13);
            label19.TabIndex = 9;
            label19.Text = "Status:";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label18.ForeColor = System.Drawing.Color.WhiteSmoke;
            label18.Location = new System.Drawing.Point(19, 84);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(38, 13);
            label18.TabIndex = 8;
            label18.Text = "HP %:";
            // 
            // targetPanel
            // 
            targetPanel.Controls.Add(lblTargetEnd);
            targetPanel.Controls.Add(targetEnd);
            targetPanel.Controls.Add(label23);
            targetPanel.Controls.Add(lblTargetHP);
            targetPanel.Controls.Add(targetHP);
            targetPanel.Controls.Add(label21);
            targetPanel.Location = new System.Drawing.Point(1186, 0);
            targetPanel.Name = "targetPanel";
            targetPanel.Size = new System.Drawing.Size(587, 363);
            targetPanel.TabIndex = 4;
            // 
            // lblTargetEnd
            // 
            lblTargetEnd.AutoSize = true;
            lblTargetEnd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblTargetEnd.Location = new System.Drawing.Point(496, 204);
            lblTargetEnd.Name = "lblTargetEnd";
            lblTargetEnd.Size = new System.Drawing.Size(38, 15);
            lblTargetEnd.TabIndex = 79;
            lblTargetEnd.Text = "100%";
            // 
            // targetEnd
            // 
            targetEnd.BackColor = System.Drawing.Color.Black;
            targetEnd.BackgroundImage = (System.Drawing.Image)resources.GetObject("targetEnd.BackgroundImage");
            targetEnd.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetEnd.BaseBarColors");
            targetEnd.Border = true;
            targetEnd.BorderColor = System.Drawing.Color.Black;
            targetEnd.Clickable = true;
            targetEnd.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            targetEnd.ColorBase = System.Drawing.Color.FromArgb(64, 153, 255);
            targetEnd.ColorEnh = System.Drawing.Color.Yellow;
            targetEnd.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 89, 191);
            targetEnd.ColorFadeStart = System.Drawing.Color.Black;
            targetEnd.ColorHighlight = System.Drawing.Color.Gray;
            targetEnd.ColorLines = System.Drawing.Color.Black;
            targetEnd.ColorMarkerInner = System.Drawing.Color.Red;
            targetEnd.ColorMarkerOuter = System.Drawing.Color.Black;
            targetEnd.ColorOvercap = System.Drawing.Color.Black;
            targetEnd.DifferentiateColors = false;
            targetEnd.DrawRuler = false;
            targetEnd.Dual = false;
            targetEnd.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetEnd.EnhBarColors");
            targetEnd.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            targetEnd.ForcedMax = 0F;
            targetEnd.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            targetEnd.Highlight = false;
            targetEnd.ItemFontSizeOverride = 0F;
            targetEnd.ItemHeight = 10;
            targetEnd.Lines = true;
            targetEnd.Location = new System.Drawing.Point(49, 204);
            targetEnd.MarkerValue = 0F;
            targetEnd.Max = 100F;
            targetEnd.MaxItems = 1;
            targetEnd.Name = "targetEnd";
            targetEnd.OuterBorder = false;
            targetEnd.Overcap = false;
            targetEnd.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetEnd.OvercapColors");
            targetEnd.PaddingX = 2F;
            targetEnd.PaddingY = 2F;
            targetEnd.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("targetEnd.PerItemScales");
            targetEnd.RulerPos = CtlMultiGraph.RulerPosition.Top;
            targetEnd.ScaleHeight = 32;
            targetEnd.ScaleIndex = 8;
            targetEnd.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            targetEnd.ShowScale = false;
            targetEnd.SingleLineLabels = true;
            targetEnd.Size = new System.Drawing.Size(441, 15);
            targetEnd.Style = Core.Enums.GraphStyle.baseOnly;
            targetEnd.TabIndex = 78;
            targetEnd.TextWidth = 80;
            targetEnd.BarClick += targetEnd_BarClick;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label23.ForeColor = System.Drawing.Color.WhiteSmoke;
            label23.Location = new System.Drawing.Point(31, 178);
            label23.Margin = new Padding(4, 0, 4, 0);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(43, 13);
            label23.TabIndex = 77;
            label23.Text = "End %:";
            // 
            // lblTargetHP
            // 
            lblTargetHP.AutoSize = true;
            lblTargetHP.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblTargetHP.Location = new System.Drawing.Point(496, 129);
            lblTargetHP.Name = "lblTargetHP";
            lblTargetHP.Size = new System.Drawing.Size(38, 15);
            lblTargetHP.TabIndex = 76;
            lblTargetHP.Text = "100%";
            // 
            // targetHP
            // 
            targetHP.BackColor = System.Drawing.Color.Black;
            targetHP.BackgroundImage = (System.Drawing.Image)resources.GetObject("targetHP.BackgroundImage");
            targetHP.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetHP.BaseBarColors");
            targetHP.Border = true;
            targetHP.BorderColor = System.Drawing.Color.Black;
            targetHP.Clickable = true;
            targetHP.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            targetHP.ColorBase = System.Drawing.Color.FromArgb(64, 255, 64);
            targetHP.ColorEnh = System.Drawing.Color.Yellow;
            targetHP.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 192, 0);
            targetHP.ColorFadeStart = System.Drawing.Color.Black;
            targetHP.ColorHighlight = System.Drawing.Color.Gray;
            targetHP.ColorLines = System.Drawing.Color.Black;
            targetHP.ColorMarkerInner = System.Drawing.Color.Red;
            targetHP.ColorMarkerOuter = System.Drawing.Color.Black;
            targetHP.ColorOvercap = System.Drawing.Color.Black;
            targetHP.DifferentiateColors = false;
            targetHP.DrawRuler = false;
            targetHP.Dual = false;
            targetHP.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetHP.EnhBarColors");
            targetHP.Font = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            targetHP.ForcedMax = 0F;
            targetHP.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            targetHP.Highlight = false;
            targetHP.ItemFontSizeOverride = 0F;
            targetHP.ItemHeight = 10;
            targetHP.Lines = true;
            targetHP.Location = new System.Drawing.Point(49, 129);
            targetHP.MarkerValue = 0F;
            targetHP.Max = 100F;
            targetHP.MaxItems = 1;
            targetHP.Name = "targetHP";
            targetHP.OuterBorder = false;
            targetHP.Overcap = false;
            targetHP.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("targetHP.OvercapColors");
            targetHP.PaddingX = 2F;
            targetHP.PaddingY = 2F;
            targetHP.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("targetHP.PerItemScales");
            targetHP.RulerPos = CtlMultiGraph.RulerPosition.Top;
            targetHP.ScaleHeight = 32;
            targetHP.ScaleIndex = 8;
            targetHP.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            targetHP.ShowScale = false;
            targetHP.SingleLineLabels = true;
            targetHP.Size = new System.Drawing.Size(441, 15);
            targetHP.Style = Core.Enums.GraphStyle.baseOnly;
            targetHP.TabIndex = 75;
            targetHP.TextWidth = 80;
            targetHP.BarClick += targetHP_BarClick;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label21.ForeColor = System.Drawing.Color.WhiteSmoke;
            label21.Location = new System.Drawing.Point(31, 103);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(38, 13);
            label21.TabIndex = 74;
            label21.Text = "HP %:";
            // 
            // FrmTeam
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1778, 363);
            Controls.Add(tabStrip);
            Controls.Add(panel2);
            Controls.Add(teamPanel);
            Controls.Add(playerPanel);
            Controls.Add(targetPanel);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmTeam";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Combat Settings";
            TopMost = true;
            teamPanel.ResumeLayout(false);
            teamPanel.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)udWidow).EndInit();
            ((System.ComponentModel.ISupportInitialize)udSoldier).EndInit();
            ((System.ComponentModel.ISupportInitialize)udCorruptor).EndInit();
            ((System.ComponentModel.ISupportInitialize)udDominator).EndInit();
            ((System.ComponentModel.ISupportInitialize)udMastermind).EndInit();
            ((System.ComponentModel.ISupportInitialize)udStalker).EndInit();
            ((System.ComponentModel.ISupportInitialize)udBrute).EndInit();
            ((System.ComponentModel.ISupportInitialize)udSentGuard).EndInit();
            ((System.ComponentModel.ISupportInitialize)udWarshade).EndInit();
            ((System.ComponentModel.ISupportInitialize)udPeacebringer).EndInit();
            ((System.ComponentModel.ISupportInitialize)udTanker).EndInit();
            ((System.ComponentModel.ISupportInitialize)udScrapper).EndInit();
            ((System.ComponentModel.ISupportInitialize)udDefender).EndInit();
            ((System.ComponentModel.ISupportInitialize)udController).EndInit();
            ((System.ComponentModel.ISupportInitialize)udBlaster).EndInit();
            ((System.ComponentModel.ISupportInitialize)udAny).EndInit();
            panel2.ResumeLayout(false);
            playerPanel.ResumeLayout(false);
            playerPanel.PerformLayout();
            targetPanel.ResumeLayout(false);
            targetPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel teamPanel;
        private TextBox tbTotalTeam;
        private TableLayoutPanel tableLayoutPanel1;
        private EnhancedUpDown udWidow;
        private EnhancedUpDown udSoldier;
        private EnhancedUpDown udCorruptor;
        private EnhancedUpDown udDominator;
        private EnhancedUpDown udMastermind;
        private EnhancedUpDown udStalker;
        private EnhancedUpDown udBrute;
        private EnhancedUpDown udSentGuard;
        private EnhancedUpDown udWarshade;
        private EnhancedUpDown udPeacebringer;
        private EnhancedUpDown udTanker;
        private EnhancedUpDown udScrapper;
        private EnhancedUpDown udDefender;
        private EnhancedUpDown udController;
        private EnhancedUpDown udBlaster;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label2;
        private Label label3;
        private Label label1;
        private EnhancedUpDown udAny;
        private Label label17;
        private Panel panel2;
        private Controls.ImageButtonEx btnSave;
        private Controls.ImageButtonEx btnCancel;
        private ctlTotalsTabStrip tabStrip;
        private Panel playerPanel;
        private Label label18;
        private RadioButton rbPlayerStatusDead;
        private RadioButton rbPlayerStatusAlive;
        private Label label19;
        private Label lblPlayerHP;
        private CtlMultiGraph playerHP;
        private Panel targetPanel;
        private Label lblTargetHP;
        private CtlMultiGraph targetHP;
        private Label label21;
        private Label lblTargetEnd;
        private CtlMultiGraph targetEnd;
        private Label label23;
        private Controls.ImageButtonEx btnToTop;
        private Label lblPlayerEnd;
        private CtlMultiGraph playerEnd;
        private Label label22;
    }
}