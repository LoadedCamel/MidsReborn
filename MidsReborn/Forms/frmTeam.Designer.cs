using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
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
            tbTotalTeam = new TextBox();
            btnSave = new Controls.ImageButtonEx();
            btnCancel = new Controls.ImageButtonEx();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
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
            SuspendLayout();
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
            tableLayoutPanel1.Location = new System.Drawing.Point(58, 70);
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
            tableLayoutPanel1.TabIndex = 0;
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
            label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label17.ForeColor = System.Drawing.Color.WhiteSmoke;
            label17.Location = new System.Drawing.Point(14, 10);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(129, 13);
            label17.TabIndex = 2;
            label17.Text = "Total Team Members:";
            // 
            // tbTotalTeam
            // 
            tbTotalTeam.BorderStyle = BorderStyle.FixedSingle;
            tbTotalTeam.ForeColor = System.Drawing.Color.WhiteSmoke;
            tbTotalTeam.Location = new System.Drawing.Point(172, 8);
            tbTotalTeam.Margin = new Padding(4, 3, 4, 3);
            tbTotalTeam.Name = "tbTotalTeam";
            tbTotalTeam.ReadOnly = true;
            tbTotalTeam.Size = new System.Drawing.Size(19, 23);
            tbTotalTeam.TabIndex = 3;
            tbTotalTeam.Text = "0";
            tbTotalTeam.TextAlign = HorizontalAlignment.Center;
            // 
            // btnSave
            // 
            btnSave.BackgroundImageLayout = ImageLayout.None;
            btnSave.CurrentText = "Save & Close";
            btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnSave.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnSave.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnSave.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnSave.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnSave.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnSave.Location = new System.Drawing.Point(450, 300);
            btnSave.Lock = false;
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(122, 25);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save & Close";
            btnSave.TextOutline.Color = System.Drawing.Color.Black;
            btnSave.TextOutline.Width = 2;
            btnSave.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
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
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnCancel.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnCancel.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnCancel.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnCancel.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnCancel.Location = new System.Drawing.Point(321, 300);
            btnCancel.Lock = false;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(122, 25);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.TextOutline.Color = System.Drawing.Color.Black;
            btnCancel.TextOutline.Width = 2;
            btnCancel.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnCancel.ToggleText.Indeterminate = "Indeterminate State";
            btnCancel.ToggleText.ToggledOff = "ToggledOff State";
            btnCancel.ToggleText.ToggledOn = "ToggledOn State";
            btnCancel.UseAlt = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // FrmTeam
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(587, 339);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(tbTotalTeam);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label17);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmTeam";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Team Members Selection";
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private EnhancedUpDown udAny;
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
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbTotalTeam;
        private Controls.ImageButtonEx btnSave;
        private Controls.ImageButtonEx btnCancel;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
    }
}