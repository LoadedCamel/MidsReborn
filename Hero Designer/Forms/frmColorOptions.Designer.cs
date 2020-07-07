using System.ComponentModel;
using System.Windows.Forms;
using midsControls;

namespace Hero_Designer.Forms
{
    partial class frmColorOptions
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
            this.colorSelector = new System.Windows.Forms.ColorDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HighlightVillain = new System.Windows.Forms.Button();
            this.DarkTakenVillain = new System.Windows.Forms.Button();
            this.TakenVillain = new System.Windows.Forms.Button();
            this.HighlightHero = new System.Windows.Forms.Button();
            this.DarkTakenHero = new System.Windows.Forms.Button();
            this.TakenHeroColor = new System.Windows.Forms.Button();
            this.UnavailPowerColor = new System.Windows.Forms.Button();
            this.AvailPowerColor = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.SpecialCaseColor = new System.Windows.Forms.Button();
            this.ValueColor = new System.Windows.Forms.Button();
            this.AlertColor = new System.Windows.Forms.Button();
            this.EnhancementsColor = new System.Windows.Forms.Button();
            this.FadedColor = new System.Windows.Forms.Button();
            this.InventionsWhiteColor = new System.Windows.Forms.Button();
            this.InventionsColor = new System.Windows.Forms.Button();
            this.TextColor = new System.Windows.Forms.Button();
            this.BGColor = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.ButtonDefault = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOkay = new System.Windows.Forms.Button();
            this.ctlColorList1 = new midsControls.ctlColorList();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorSelector
            // 
            this.colorSelector.AnyColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Click on a color box below to change\r\nthe corresponding item\'s color.\r\n\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(347, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Previews";
            // 
            // richTextBox1
            // 
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(277, 51);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(205, 206);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(102, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 14);
            this.label3.TabIndex = 0;
            this.label3.Text = "Background:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(140, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "Text:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(111, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 14);
            this.label5.TabIndex = 2;
            this.label5.Text = "Inventions:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(56, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 14);
            this.label6.TabIndex = 3;
            this.label6.Text = "Inventions (On White):";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(130, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 14);
            this.label7.TabIndex = 4;
            this.label7.Text = "Faded:";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(89, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 14);
            this.label8.TabIndex = 5;
            this.label8.Text = "Enhancements:";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(137, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 14);
            this.label9.TabIndex = 6;
            this.label9.Text = "Alert:";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(103, 150);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 14);
            this.label10.TabIndex = 7;
            this.label10.Text = "Value Name:";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(67, 171);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 14);
            this.label11.TabIndex = 8;
            this.label11.Text = "Special Case Value:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.23529F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.76471F));
            this.tableLayoutPanel1.Controls.Add(this.HighlightVillain, 1, 16);
            this.tableLayoutPanel1.Controls.Add(this.DarkTakenVillain, 1, 15);
            this.tableLayoutPanel1.Controls.Add(this.TakenVillain, 1, 14);
            this.tableLayoutPanel1.Controls.Add(this.HighlightHero, 1, 13);
            this.tableLayoutPanel1.Controls.Add(this.DarkTakenHero, 1, 12);
            this.tableLayoutPanel1.Controls.Add(this.TakenHeroColor, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.UnavailPowerColor, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.AvailPowerColor, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.label19, 0, 16);
            this.tableLayoutPanel1.Controls.Add(this.label17, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.label16, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.label15, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.label14, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.SpecialCaseColor, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.ValueColor, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.AlertColor, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.EnhancementsColor, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.FadedColor, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.InventionsWhiteColor, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.InventionsColor, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.TextColor, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.BGColor, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.label18, 0, 15);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 51);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 19;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(255, 402);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // HighlightVillain
            // 
            this.HighlightVillain.BackColor = System.Drawing.Color.MidnightBlue;
            this.HighlightVillain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HighlightVillain.Location = new System.Drawing.Point(176, 339);
            this.HighlightVillain.Name = "HighlightVillain";
            this.HighlightVillain.Size = new System.Drawing.Size(75, 14);
            this.HighlightVillain.TabIndex = 11;
            this.HighlightVillain.UseVisualStyleBackColor = false;
            this.HighlightVillain.Click += new System.EventHandler(this.HighlightVillain_Click);
            // 
            // DarkTakenVillain
            // 
            this.DarkTakenVillain.BackColor = System.Drawing.Color.MidnightBlue;
            this.DarkTakenVillain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DarkTakenVillain.Location = new System.Drawing.Point(176, 318);
            this.DarkTakenVillain.Name = "DarkTakenVillain";
            this.DarkTakenVillain.Size = new System.Drawing.Size(75, 14);
            this.DarkTakenVillain.TabIndex = 11;
            this.DarkTakenVillain.UseVisualStyleBackColor = false;
            this.DarkTakenVillain.Click += new System.EventHandler(this.DarkTakenVillain_Click);
            // 
            // TakenVillain
            // 
            this.TakenVillain.BackColor = System.Drawing.Color.MidnightBlue;
            this.TakenVillain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TakenVillain.Location = new System.Drawing.Point(176, 297);
            this.TakenVillain.Name = "TakenVillain";
            this.TakenVillain.Size = new System.Drawing.Size(75, 14);
            this.TakenVillain.TabIndex = 11;
            this.TakenVillain.UseVisualStyleBackColor = false;
            this.TakenVillain.Click += new System.EventHandler(this.TakenVillain_Click);
            // 
            // HighlightHero
            // 
            this.HighlightHero.BackColor = System.Drawing.Color.MidnightBlue;
            this.HighlightHero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HighlightHero.Location = new System.Drawing.Point(176, 276);
            this.HighlightHero.Name = "HighlightHero";
            this.HighlightHero.Size = new System.Drawing.Size(75, 14);
            this.HighlightHero.TabIndex = 11;
            this.HighlightHero.UseVisualStyleBackColor = false;
            this.HighlightHero.Click += new System.EventHandler(this.HighlightHero_Click);
            // 
            // DarkTakenHero
            // 
            this.DarkTakenHero.BackColor = System.Drawing.Color.MidnightBlue;
            this.DarkTakenHero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DarkTakenHero.Location = new System.Drawing.Point(176, 255);
            this.DarkTakenHero.Name = "DarkTakenHero";
            this.DarkTakenHero.Size = new System.Drawing.Size(75, 14);
            this.DarkTakenHero.TabIndex = 11;
            this.DarkTakenHero.UseVisualStyleBackColor = false;
            this.DarkTakenHero.Click += new System.EventHandler(this.DarkTakenHero_Click);
            // 
            // TakenHeroColor
            // 
            this.TakenHeroColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.TakenHeroColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TakenHeroColor.Location = new System.Drawing.Point(176, 234);
            this.TakenHeroColor.Name = "TakenHeroColor";
            this.TakenHeroColor.Size = new System.Drawing.Size(75, 14);
            this.TakenHeroColor.TabIndex = 11;
            this.TakenHeroColor.UseVisualStyleBackColor = false;
            this.TakenHeroColor.Click += new System.EventHandler(this.TakenHeroColor_Click);
            // 
            // UnavailPowerColor
            // 
            this.UnavailPowerColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.UnavailPowerColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnavailPowerColor.Location = new System.Drawing.Point(176, 213);
            this.UnavailPowerColor.Name = "UnavailPowerColor";
            this.UnavailPowerColor.Size = new System.Drawing.Size(75, 14);
            this.UnavailPowerColor.TabIndex = 11;
            this.UnavailPowerColor.UseVisualStyleBackColor = false;
            this.UnavailPowerColor.Click += new System.EventHandler(this.UnavailPowerColor_Click);
            // 
            // AvailPowerColor
            // 
            this.AvailPowerColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.AvailPowerColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AvailPowerColor.Location = new System.Drawing.Point(176, 192);
            this.AvailPowerColor.Name = "AvailPowerColor";
            this.AvailPowerColor.Size = new System.Drawing.Size(75, 14);
            this.AvailPowerColor.TabIndex = 11;
            this.AvailPowerColor.UseVisualStyleBackColor = false;
            this.AvailPowerColor.Click += new System.EventHandler(this.AvailPowerColor_Click);
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(46, 339);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(124, 14);
            this.label19.TabIndex = 11;
            this.label19.Text = "Highlight Power (Villain):";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(58, 297);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(112, 14);
            this.label17.TabIndex = 11;
            this.label17.Text = "Taken Power (Villain):";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(51, 276);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(119, 14);
            this.label16.TabIndex = 11;
            this.label16.Text = "Highlight Power (Hero):";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(38, 255);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(132, 14);
            this.label15.TabIndex = 11;
            this.label15.Text = "Dark Taken Power (Hero):";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(63, 234);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(107, 14);
            this.label14.TabIndex = 11;
            this.label14.Text = "Taken Power (Hero):";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(81, 192);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 14);
            this.label12.TabIndex = 11;
            this.label12.Text = "Available Power:";
            // 
            // SpecialCaseColor
            // 
            this.SpecialCaseColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.SpecialCaseColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SpecialCaseColor.Location = new System.Drawing.Point(176, 171);
            this.SpecialCaseColor.Name = "SpecialCaseColor";
            this.SpecialCaseColor.Size = new System.Drawing.Size(75, 15);
            this.SpecialCaseColor.TabIndex = 10;
            this.SpecialCaseColor.UseVisualStyleBackColor = false;
            this.SpecialCaseColor.Click += new System.EventHandler(this.SpecialCaseColor_Click);
            // 
            // ValueColor
            // 
            this.ValueColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.ValueColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ValueColor.Location = new System.Drawing.Point(176, 150);
            this.ValueColor.Name = "ValueColor";
            this.ValueColor.Size = new System.Drawing.Size(75, 15);
            this.ValueColor.TabIndex = 10;
            this.ValueColor.UseVisualStyleBackColor = false;
            this.ValueColor.Click += new System.EventHandler(this.ValueColor_Click);
            // 
            // AlertColor
            // 
            this.AlertColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.AlertColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AlertColor.Location = new System.Drawing.Point(176, 129);
            this.AlertColor.Name = "AlertColor";
            this.AlertColor.Size = new System.Drawing.Size(75, 15);
            this.AlertColor.TabIndex = 10;
            this.AlertColor.UseVisualStyleBackColor = false;
            this.AlertColor.Click += new System.EventHandler(this.AlertColor_Click);
            // 
            // EnhancementsColor
            // 
            this.EnhancementsColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.EnhancementsColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnhancementsColor.Location = new System.Drawing.Point(176, 108);
            this.EnhancementsColor.Name = "EnhancementsColor";
            this.EnhancementsColor.Size = new System.Drawing.Size(75, 15);
            this.EnhancementsColor.TabIndex = 10;
            this.EnhancementsColor.UseVisualStyleBackColor = false;
            this.EnhancementsColor.Click += new System.EventHandler(this.EnhancementsColor_Click);
            // 
            // FadedColor
            // 
            this.FadedColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.FadedColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FadedColor.Location = new System.Drawing.Point(176, 87);
            this.FadedColor.Name = "FadedColor";
            this.FadedColor.Size = new System.Drawing.Size(75, 15);
            this.FadedColor.TabIndex = 10;
            this.FadedColor.UseVisualStyleBackColor = false;
            this.FadedColor.Click += new System.EventHandler(this.FadedColor_Click);
            // 
            // InventionsWhiteColor
            // 
            this.InventionsWhiteColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.InventionsWhiteColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InventionsWhiteColor.Location = new System.Drawing.Point(176, 66);
            this.InventionsWhiteColor.Name = "InventionsWhiteColor";
            this.InventionsWhiteColor.Size = new System.Drawing.Size(75, 15);
            this.InventionsWhiteColor.TabIndex = 10;
            this.InventionsWhiteColor.UseVisualStyleBackColor = false;
            this.InventionsWhiteColor.Click += new System.EventHandler(this.InventionsWhiteColor_Click);
            // 
            // InventionsColor
            // 
            this.InventionsColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.InventionsColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InventionsColor.Location = new System.Drawing.Point(176, 45);
            this.InventionsColor.Name = "InventionsColor";
            this.InventionsColor.Size = new System.Drawing.Size(75, 15);
            this.InventionsColor.TabIndex = 10;
            this.InventionsColor.UseVisualStyleBackColor = false;
            this.InventionsColor.Click += new System.EventHandler(this.InventionsColor_Click);
            // 
            // TextColor
            // 
            this.TextColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.TextColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TextColor.Location = new System.Drawing.Point(176, 24);
            this.TextColor.Name = "TextColor";
            this.TextColor.Size = new System.Drawing.Size(75, 15);
            this.TextColor.TabIndex = 10;
            this.TextColor.UseVisualStyleBackColor = false;
            this.TextColor.Click += new System.EventHandler(this.TextColor_Click);
            // 
            // BGColor
            // 
            this.BGColor.BackColor = System.Drawing.Color.MidnightBlue;
            this.BGColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BGColor.Location = new System.Drawing.Point(176, 3);
            this.BGColor.Name = "BGColor";
            this.BGColor.Size = new System.Drawing.Size(75, 15);
            this.BGColor.TabIndex = 9;
            this.BGColor.UseVisualStyleBackColor = false;
            this.BGColor.Click += new System.EventHandler(this.BGColor_Click);
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(70, 213);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 14);
            this.label13.TabIndex = 12;
            this.label13.Text = "Unavailable Power:";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(36, 318);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(134, 14);
            this.label18.TabIndex = 11;
            this.label18.Text = "Dark Taken Power (Villain)";
            // 
            // ButtonDefault
            // 
            this.ButtonDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonDefault.Location = new System.Drawing.Point(121, 479);
            this.ButtonDefault.Name = "ButtonDefault";
            this.ButtonDefault.Size = new System.Drawing.Size(94, 23);
            this.ButtonDefault.TabIndex = 11;
            this.ButtonDefault.Text = "Set Defaults";
            this.ButtonDefault.UseVisualStyleBackColor = true;
            this.ButtonDefault.Click += new System.EventHandler(this.DefaultButton_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonCancel.Location = new System.Drawing.Point(221, 479);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 12;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ButtonOkay
            // 
            this.ButtonOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonOkay.Location = new System.Drawing.Point(302, 479);
            this.ButtonOkay.Name = "ButtonOkay";
            this.ButtonOkay.Size = new System.Drawing.Size(75, 23);
            this.ButtonOkay.TabIndex = 13;
            this.ButtonOkay.Text = "OK";
            this.ButtonOkay.UseVisualStyleBackColor = true;
            this.ButtonOkay.Click += new System.EventHandler(this.OkayButton_Click);
            // 
            // ctlColorList1
            // 
            this.ctlColorList1.BackColor = System.Drawing.Color.Black;
            this.ctlColorList1.Colors = null;
            this.ctlColorList1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ctlColorList1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctlColorList1.ForeColor = System.Drawing.Color.White;
            this.ctlColorList1.FormattingEnabled = true;
            this.ctlColorList1.Location = new System.Drawing.Point(277, 264);
            this.ctlColorList1.Name = "ctlColorList1";
            this.ctlColorList1.Size = new System.Drawing.Size(204, 186);
            this.ctlColorList1.TabIndex = 14;
            // 
            // frmColorOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 507);
            this.Controls.Add(this.ctlColorList1);
            this.Controls.Add(this.ButtonOkay);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonDefault);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmColorOptions";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Options";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorDialog colorSelector;
        private Label label1;
        private Label label2;
        private RichTextBox richTextBox1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private TableLayoutPanel tableLayoutPanel1;
        private Button HighlightVillain;
        private Button DarkTakenVillain;
        private Button TakenVillain;
        private Button HighlightHero;
        private Button DarkTakenHero;
        private Button TakenHeroColor;
        private Button UnavailPowerColor;
        private Button AvailPowerColor;
        private Label label19;
        private Label label17;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label12;
        private Button SpecialCaseColor;
        private Button ValueColor;
        private Button AlertColor;
        private Button EnhancementsColor;
        private Button FadedColor;
        private Button InventionsWhiteColor;
        private Button InventionsColor;
        private Button TextColor;
        private Button BGColor;
        private Label label13;
        private Label label18;
        private Button ButtonDefault;
        private Button ButtonCancel;
        private Button ButtonOkay;
        private ctlColorList ctlColorList1;
    }
}