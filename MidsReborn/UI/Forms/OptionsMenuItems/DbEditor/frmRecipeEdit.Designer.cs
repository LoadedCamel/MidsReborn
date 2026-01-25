using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmRecipeEdit
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
            lvDPA = new ListView();
            ColumnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            ColumnHeader2 = new ColumnHeader();
            ColumnHeader3 = new ColumnHeader();
            ColumnHeader4 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            btnCancel = new Button();
            btnOK = new Button();
            btnReGuess = new Button();
            GroupBox1 = new GroupBox();
            btnGuessCost = new Button();
            udSal4 = new NumericUpDown();
            Label14 = new Label();
            cbSal4 = new ComboBox();
            udSal3 = new NumericUpDown();
            Label13 = new Label();
            cbSal3 = new ComboBox();
            udSal2 = new NumericUpDown();
            Label12 = new Label();
            cbSal2 = new ComboBox();
            udSal1 = new NumericUpDown();
            Label11 = new Label();
            cbSal1 = new ComboBox();
            udSal0 = new NumericUpDown();
            Label10 = new Label();
            cbSal0 = new ComboBox();
            Label9 = new Label();
            udCraftM = new NumericUpDown();
            Label8 = new Label();
            udCraft = new NumericUpDown();
            Label7 = new Label();
            udBuyM = new NumericUpDown();
            Label6 = new Label();
            udBuy = new NumericUpDown();
            Label5 = new Label();
            udLevel = new NumericUpDown();
            lstItems = new ListBox();
            Label3 = new Label();
            cbRarity = new ComboBox();
            Label1 = new Label();
            txtRecipeName = new TextBox();
            Label2 = new Label();
            cbEnh = new ComboBox();
            GroupBox2 = new GroupBox();
            btnAutoMarkGeneric = new Button();
            udStaticIndex = new NumericUpDown();
            Label16 = new Label();
            cbIsHidden = new CheckBox();
            cbIsVirtual = new CheckBox();
            cbIsGeneric = new CheckBox();
            lblEnh = new Label();
            txtExtern = new TextBox();
            Label15 = new Label();
            btnI50 = new Button();
            btnI40 = new Button();
            btnI25 = new Button();
            btnI20 = new Button();
            btnIncrement = new Button();
            btnDel = new Button();
            btnAdd = new Button();
            btnRAdd = new Button();
            btnRDel = new Button();
            btnRunSeq = new Button();
            groupBox3 = new GroupBox();
            btnMassUpdateTags = new Button();
            panel1 = new Panel();
            progressBar1 = new ProgressBar();
            label17 = new Label();
            GroupBox1.SuspendLayout();
            ((ISupportInitialize)udSal4).BeginInit();
            ((ISupportInitialize)udSal3).BeginInit();
            ((ISupportInitialize)udSal2).BeginInit();
            ((ISupportInitialize)udSal1).BeginInit();
            ((ISupportInitialize)udSal0).BeginInit();
            ((ISupportInitialize)udCraftM).BeginInit();
            ((ISupportInitialize)udCraft).BeginInit();
            ((ISupportInitialize)udBuyM).BeginInit();
            ((ISupportInitialize)udBuy).BeginInit();
            ((ISupportInitialize)udLevel).BeginInit();
            GroupBox2.SuspendLayout();
            ((ISupportInitialize)udStaticIndex).BeginInit();
            groupBox3.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lvDPA
            // 
            lvDPA.Columns.AddRange(new ColumnHeader[] { ColumnHeader1, columnHeader5, ColumnHeader2, ColumnHeader3, ColumnHeader4, columnHeader6 });
            lvDPA.FullRowSelect = true;
            lvDPA.Location = new System.Drawing.Point(12, 12);
            lvDPA.MultiSelect = false;
            lvDPA.Name = "lvDPA";
            lvDPA.Size = new System.Drawing.Size(662, 322);
            lvDPA.Sorting = SortOrder.Ascending;
            lvDPA.TabIndex = 0;
            lvDPA.UseCompatibleStateImageBehavior = false;
            lvDPA.View = View.Details;
            lvDPA.ColumnClick += lvDPA_ColumnClick;
            lvDPA.SelectedIndexChanged += lvDPA_SelectedIndexChanged;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Recipe";
            ColumnHeader1.Width = 201;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Index";
            columnHeader5.Width = 45;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Enhancement";
            ColumnHeader2.Width = 208;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Rarity";
            ColumnHeader3.Width = 84;
            // 
            // ColumnHeader4
            // 
            ColumnHeader4.Text = "Entries";
            ColumnHeader4.Width = 45;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Flags";
            columnHeader6.Width = 40;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(12, 536);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(113, 24);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(131, 536);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(113, 24);
            btnOK.TabIndex = 4;
            btnOK.Text = "Save && Close";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnReGuess
            // 
            btnReGuess.Location = new System.Drawing.Point(269, 536);
            btnReGuess.Name = "btnReGuess";
            btnReGuess.Size = new System.Drawing.Size(147, 24);
            btnReGuess.TabIndex = 7;
            btnReGuess.Text = "Re-Guess all recipes";
            btnReGuess.UseVisualStyleBackColor = true;
            btnReGuess.Click += btnReGuess_Click;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(btnGuessCost);
            GroupBox1.Controls.Add(udSal4);
            GroupBox1.Controls.Add(Label14);
            GroupBox1.Controls.Add(cbSal4);
            GroupBox1.Controls.Add(udSal3);
            GroupBox1.Controls.Add(Label13);
            GroupBox1.Controls.Add(cbSal3);
            GroupBox1.Controls.Add(udSal2);
            GroupBox1.Controls.Add(Label12);
            GroupBox1.Controls.Add(cbSal2);
            GroupBox1.Controls.Add(udSal1);
            GroupBox1.Controls.Add(Label11);
            GroupBox1.Controls.Add(cbSal1);
            GroupBox1.Controls.Add(udSal0);
            GroupBox1.Controls.Add(Label10);
            GroupBox1.Controls.Add(cbSal0);
            GroupBox1.Controls.Add(Label9);
            GroupBox1.Controls.Add(udCraftM);
            GroupBox1.Controls.Add(Label8);
            GroupBox1.Controls.Add(udCraft);
            GroupBox1.Controls.Add(Label7);
            GroupBox1.Controls.Add(udBuyM);
            GroupBox1.Controls.Add(Label6);
            GroupBox1.Controls.Add(udBuy);
            GroupBox1.Controls.Add(Label5);
            GroupBox1.Controls.Add(udLevel);
            GroupBox1.Location = new System.Drawing.Point(12, 366);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(688, 164);
            GroupBox1.TabIndex = 8;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Recipe Info:";
            // 
            // btnGuessCost
            // 
            btnGuessCost.Location = new System.Drawing.Point(185, 107);
            btnGuessCost.Name = "btnGuessCost";
            btnGuessCost.Size = new System.Drawing.Size(58, 22);
            btnGuessCost.TabIndex = 36;
            btnGuessCost.Text = "Guess";
            btnGuessCost.UseVisualStyleBackColor = true;
            btnGuessCost.Click += btnGuessCost_Click;
            // 
            // udSal4
            // 
            udSal4.Location = new System.Drawing.Point(568, 133);
            udSal4.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            udSal4.Name = "udSal4";
            udSal4.Size = new System.Drawing.Size(59, 22);
            udSal4.TabIndex = 350;
            udSal4.TextAlign = HorizontalAlignment.Center;
            udSal4.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udSal4.ValueChanged += udSalX_ValueChanged;
            udSal4.Leave += udSalX_Leave;
            // 
            // Label14
            // 
            Label14.Location = new System.Drawing.Point(270, 131);
            Label14.Name = "Label14";
            Label14.Size = new System.Drawing.Size(86, 22);
            Label14.TabIndex = 34;
            Label14.Text = "Ingredient 5:";
            Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal4
            // 
            cbSal4.AutoCompleteMode = AutoCompleteMode.Append;
            cbSal4.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbSal4.FormattingEnabled = true;
            cbSal4.Location = new System.Drawing.Point(362, 133);
            cbSal4.Name = "cbSal4";
            cbSal4.Size = new System.Drawing.Size(202, 21);
            cbSal4.TabIndex = 33;
            cbSal4.SelectedIndexChanged += cbSalX_SelectedIndexChanged;
            // 
            // udSal3
            // 
            udSal3.Location = new System.Drawing.Point(568, 105);
            udSal3.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            udSal3.Name = "udSal3";
            udSal3.Size = new System.Drawing.Size(59, 22);
            udSal3.TabIndex = 320;
            udSal3.TextAlign = HorizontalAlignment.Center;
            udSal3.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udSal3.ValueChanged += udSalX_ValueChanged;
            udSal3.Leave += udSalX_Leave;
            // 
            // Label13
            // 
            Label13.Location = new System.Drawing.Point(270, 103);
            Label13.Name = "Label13";
            Label13.Size = new System.Drawing.Size(86, 22);
            Label13.TabIndex = 31;
            Label13.Text = "Ingredient 4:";
            Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal3
            // 
            cbSal3.AutoCompleteMode = AutoCompleteMode.Append;
            cbSal3.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbSal3.FormattingEnabled = true;
            cbSal3.Location = new System.Drawing.Point(362, 105);
            cbSal3.Name = "cbSal3";
            cbSal3.Size = new System.Drawing.Size(202, 21);
            cbSal3.TabIndex = 30;
            cbSal3.SelectedIndexChanged += cbSalX_SelectedIndexChanged;
            // 
            // udSal2
            // 
            udSal2.Location = new System.Drawing.Point(568, 77);
            udSal2.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            udSal2.Name = "udSal2";
            udSal2.Size = new System.Drawing.Size(59, 22);
            udSal2.TabIndex = 290;
            udSal2.TextAlign = HorizontalAlignment.Center;
            udSal2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udSal2.ValueChanged += udSalX_ValueChanged;
            udSal2.Leave += udSalX_Leave;
            // 
            // Label12
            // 
            Label12.Location = new System.Drawing.Point(270, 75);
            Label12.Name = "Label12";
            Label12.Size = new System.Drawing.Size(86, 22);
            Label12.TabIndex = 28;
            Label12.Text = "Ingredient 3:";
            Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal2
            // 
            cbSal2.AutoCompleteMode = AutoCompleteMode.Append;
            cbSal2.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbSal2.FormattingEnabled = true;
            cbSal2.Location = new System.Drawing.Point(362, 77);
            cbSal2.Name = "cbSal2";
            cbSal2.Size = new System.Drawing.Size(202, 21);
            cbSal2.TabIndex = 27;
            cbSal2.SelectedIndexChanged += cbSalX_SelectedIndexChanged;
            // 
            // udSal1
            // 
            udSal1.Location = new System.Drawing.Point(568, 49);
            udSal1.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            udSal1.Name = "udSal1";
            udSal1.Size = new System.Drawing.Size(59, 22);
            udSal1.TabIndex = 260;
            udSal1.TextAlign = HorizontalAlignment.Center;
            udSal1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udSal1.ValueChanged += udSalX_ValueChanged;
            udSal1.Leave += udSalX_Leave;
            // 
            // Label11
            // 
            Label11.Location = new System.Drawing.Point(270, 47);
            Label11.Name = "Label11";
            Label11.Size = new System.Drawing.Size(86, 22);
            Label11.TabIndex = 25;
            Label11.Text = "Ingredient 2:";
            Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal1
            // 
            cbSal1.AutoCompleteMode = AutoCompleteMode.Append;
            cbSal1.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbSal1.FormattingEnabled = true;
            cbSal1.Location = new System.Drawing.Point(362, 49);
            cbSal1.Name = "cbSal1";
            cbSal1.Size = new System.Drawing.Size(202, 21);
            cbSal1.TabIndex = 24;
            cbSal1.SelectedIndexChanged += cbSalX_SelectedIndexChanged;
            // 
            // udSal0
            // 
            udSal0.Location = new System.Drawing.Point(568, 21);
            udSal0.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
            udSal0.Name = "udSal0";
            udSal0.Size = new System.Drawing.Size(59, 22);
            udSal0.TabIndex = 230;
            udSal0.TextAlign = HorizontalAlignment.Center;
            udSal0.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udSal0.ValueChanged += udSalX_ValueChanged;
            udSal0.Leave += udSalX_Leave;
            // 
            // Label10
            // 
            Label10.Location = new System.Drawing.Point(270, 19);
            Label10.Name = "Label10";
            Label10.Size = new System.Drawing.Size(86, 22);
            Label10.TabIndex = 22;
            Label10.Text = "Ingredient 1:";
            Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal0
            // 
            cbSal0.AutoCompleteMode = AutoCompleteMode.Append;
            cbSal0.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbSal0.FormattingEnabled = true;
            cbSal0.Location = new System.Drawing.Point(362, 21);
            cbSal0.Name = "cbSal0";
            cbSal0.Size = new System.Drawing.Size(202, 21);
            cbSal0.TabIndex = 21;
            cbSal0.SelectedIndexChanged += cbSalX_SelectedIndexChanged;
            // 
            // Label9
            // 
            Label9.Location = new System.Drawing.Point(6, 133);
            Label9.Name = "Label9";
            Label9.Size = new System.Drawing.Size(86, 20);
            Label9.TabIndex = 20;
            Label9.Text = "Craft Cost (M):";
            Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udCraftM
            // 
            udCraftM.Location = new System.Drawing.Point(98, 135);
            udCraftM.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            udCraftM.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            udCraftM.Name = "udCraftM";
            udCraftM.Size = new System.Drawing.Size(112, 22);
            udCraftM.TabIndex = 19;
            udCraftM.ThousandsSeparator = true;
            udCraftM.ValueChanged += udCostX_ValueChanged;
            udCraftM.Leave += udCostX_Leave;
            // 
            // Label8
            // 
            Label8.Location = new System.Drawing.Point(6, 105);
            Label8.Name = "Label8";
            Label8.Size = new System.Drawing.Size(86, 20);
            Label8.TabIndex = 18;
            Label8.Text = "Craft Cost:";
            Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udCraft
            // 
            udCraft.Location = new System.Drawing.Point(98, 107);
            udCraft.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            udCraft.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            udCraft.Name = "udCraft";
            udCraft.Size = new System.Drawing.Size(81, 22);
            udCraft.TabIndex = 17;
            udCraft.ThousandsSeparator = true;
            udCraft.ValueChanged += udCostX_ValueChanged;
            udCraft.Leave += udCostX_Leave;
            // 
            // Label7
            // 
            Label7.Location = new System.Drawing.Point(6, 77);
            Label7.Name = "Label7";
            Label7.Size = new System.Drawing.Size(86, 20);
            Label7.TabIndex = 16;
            Label7.Text = "Buy Cost (M):";
            Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBuyM
            // 
            udBuyM.Location = new System.Drawing.Point(98, 79);
            udBuyM.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            udBuyM.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            udBuyM.Name = "udBuyM";
            udBuyM.Size = new System.Drawing.Size(112, 22);
            udBuyM.TabIndex = 15;
            udBuyM.ThousandsSeparator = true;
            udBuyM.ValueChanged += udCostX_ValueChanged;
            udBuyM.Leave += udCostX_Leave;
            // 
            // Label6
            // 
            Label6.Location = new System.Drawing.Point(6, 49);
            Label6.Name = "Label6";
            Label6.Size = new System.Drawing.Size(86, 20);
            Label6.TabIndex = 14;
            Label6.Text = "Buy Cost:";
            Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBuy
            // 
            udBuy.Location = new System.Drawing.Point(98, 51);
            udBuy.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            udBuy.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            udBuy.Name = "udBuy";
            udBuy.Size = new System.Drawing.Size(112, 22);
            udBuy.TabIndex = 13;
            udBuy.ThousandsSeparator = true;
            udBuy.ValueChanged += udCostX_ValueChanged;
            udBuy.Leave += udCostX_Leave;
            // 
            // Label5
            // 
            Label5.Location = new System.Drawing.Point(6, 21);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(86, 20);
            Label5.TabIndex = 12;
            Label5.Text = "Level:";
            Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udLevel
            // 
            udLevel.Location = new System.Drawing.Point(98, 23);
            udLevel.Maximum = new decimal(new int[] { 53, 0, 0, 0 });
            udLevel.Name = "udLevel";
            udLevel.Size = new System.Drawing.Size(70, 22);
            udLevel.TabIndex = 0;
            udLevel.Value = new decimal(new int[] { 1, 0, 0, 0 });
            udLevel.ValueChanged += udCostX_ValueChanged;
            udLevel.Leave += udCostX_Leave;
            // 
            // lstItems
            // 
            lstItems.FormattingEnabled = true;
            lstItems.ItemHeight = 13;
            lstItems.Location = new System.Drawing.Point(6, 22);
            lstItems.Name = "lstItems";
            lstItems.Size = new System.Drawing.Size(202, 225);
            lstItems.TabIndex = 0;
            lstItems.SelectedIndexChanged += lstItems_SelectedIndexChanged;
            // 
            // Label3
            // 
            Label3.Location = new System.Drawing.Point(6, 162);
            Label3.Name = "Label3";
            Label3.Size = new System.Drawing.Size(86, 22);
            Label3.TabIndex = 11;
            Label3.Text = "Rarity:";
            Label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cbRarity
            // 
            cbRarity.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRarity.FormattingEnabled = true;
            cbRarity.Location = new System.Drawing.Point(6, 187);
            cbRarity.Name = "cbRarity";
            cbRarity.Size = new System.Drawing.Size(202, 21);
            cbRarity.TabIndex = 10;
            cbRarity.SelectedIndexChanged += cbRarity_SelectedIndexChanged;
            // 
            // Label1
            // 
            Label1.Location = new System.Drawing.Point(6, 61);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(126, 20);
            Label1.TabIndex = 13;
            Label1.Text = "Internal Name:";
            Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtRecipeName
            // 
            txtRecipeName.Location = new System.Drawing.Point(6, 84);
            txtRecipeName.Name = "txtRecipeName";
            txtRecipeName.Size = new System.Drawing.Size(202, 22);
            txtRecipeName.TabIndex = 12;
            txtRecipeName.TextChanged += txtRecipeName_TextChanged;
            // 
            // Label2
            // 
            Label2.Location = new System.Drawing.Point(6, 218);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(86, 18);
            Label2.TabIndex = 15;
            Label2.Text = "Enhancement:";
            Label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cbEnh
            // 
            cbEnh.AutoCompleteMode = AutoCompleteMode.Append;
            cbEnh.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbEnh.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEnh.FormattingEnabled = true;
            cbEnh.Location = new System.Drawing.Point(6, 236);
            cbEnh.Name = "cbEnh";
            cbEnh.Size = new System.Drawing.Size(202, 21);
            cbEnh.Sorted = true;
            cbEnh.TabIndex = 14;
            cbEnh.SelectedIndexChanged += cbEnh_SelectedIndexChanged;
            // 
            // GroupBox2
            // 
            GroupBox2.Controls.Add(btnAutoMarkGeneric);
            GroupBox2.Controls.Add(udStaticIndex);
            GroupBox2.Controls.Add(Label16);
            GroupBox2.Controls.Add(cbIsHidden);
            GroupBox2.Controls.Add(cbIsVirtual);
            GroupBox2.Controls.Add(cbIsGeneric);
            GroupBox2.Controls.Add(lblEnh);
            GroupBox2.Controls.Add(txtExtern);
            GroupBox2.Controls.Add(Label15);
            GroupBox2.Controls.Add(Label2);
            GroupBox2.Controls.Add(txtRecipeName);
            GroupBox2.Controls.Add(cbEnh);
            GroupBox2.Controls.Add(cbRarity);
            GroupBox2.Controls.Add(Label1);
            GroupBox2.Controls.Add(Label3);
            GroupBox2.Location = new System.Drawing.Point(680, 12);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(214, 352);
            GroupBox2.TabIndex = 9;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Recipe:";
            // 
            // btnAutoMarkGeneric
            // 
            btnAutoMarkGeneric.Location = new System.Drawing.Point(107, 300);
            btnAutoMarkGeneric.Name = "btnAutoMarkGeneric";
            btnAutoMarkGeneric.Size = new System.Drawing.Size(98, 24);
            btnAutoMarkGeneric.TabIndex = 358;
            btnAutoMarkGeneric.Text = "Auto-mark all";
            btnAutoMarkGeneric.UseVisualStyleBackColor = true;
            btnAutoMarkGeneric.Click += btnAutoMarkGeneric_Click;
            // 
            // udStaticIndex
            // 
            udStaticIndex.Enabled = false;
            udStaticIndex.Location = new System.Drawing.Point(6, 39);
            udStaticIndex.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            udStaticIndex.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            udStaticIndex.Name = "udStaticIndex";
            udStaticIndex.Size = new System.Drawing.Size(202, 22);
            udStaticIndex.TabIndex = 357;
            udStaticIndex.ValueChanged += udStaticIndex_ValueChanged;
            udStaticIndex.Leave += udStaticIndex_Leave;
            // 
            // Label16
            // 
            Label16.Location = new System.Drawing.Point(6, 16);
            Label16.Name = "Label16";
            Label16.Size = new System.Drawing.Size(126, 20);
            Label16.TabIndex = 356;
            Label16.Text = "Static Index:";
            Label16.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cbIsHidden
            // 
            cbIsHidden.AutoSize = true;
            cbIsHidden.Location = new System.Drawing.Point(131, 326);
            cbIsHidden.Name = "cbIsHidden";
            cbIsHidden.Size = new System.Drawing.Size(64, 17);
            cbIsHidden.TabIndex = 354;
            cbIsHidden.Text = "Hidden";
            cbIsHidden.UseVisualStyleBackColor = true;
            cbIsHidden.Click += cbIsHidden_Click;
            // 
            // cbIsVirtual
            // 
            cbIsVirtual.AutoSize = true;
            cbIsVirtual.Location = new System.Drawing.Point(16, 326);
            cbIsVirtual.Name = "cbIsVirtual";
            cbIsVirtual.Size = new System.Drawing.Size(60, 17);
            cbIsVirtual.TabIndex = 353;
            cbIsVirtual.Text = "Virtual";
            cbIsVirtual.UseVisualStyleBackColor = true;
            cbIsVirtual.Click += cbIsVirtual_Click;
            // 
            // cbIsGeneric
            // 
            cbIsGeneric.AutoSize = true;
            cbIsGeneric.Location = new System.Drawing.Point(16, 302);
            cbIsGeneric.Name = "cbIsGeneric";
            cbIsGeneric.Size = new System.Drawing.Size(65, 17);
            cbIsGeneric.TabIndex = 352;
            cbIsGeneric.Text = "Generic";
            cbIsGeneric.UseVisualStyleBackColor = true;
            cbIsGeneric.Click += cbIsGeneric_Click;
            // 
            // lblEnh
            // 
            lblEnh.Location = new System.Drawing.Point(6, 267);
            lblEnh.Name = "lblEnh";
            lblEnh.Size = new System.Drawing.Size(202, 28);
            lblEnh.TabIndex = 17;
            lblEnh.Text = "EnhancementName";
            lblEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtExtern
            // 
            txtExtern.Location = new System.Drawing.Point(6, 135);
            txtExtern.Name = "txtExtern";
            txtExtern.Size = new System.Drawing.Size(202, 22);
            txtExtern.TabIndex = 18;
            txtExtern.TextChanged += txtExtern_TextChanged;
            // 
            // Label15
            // 
            Label15.Location = new System.Drawing.Point(6, 112);
            Label15.Name = "Label15";
            Label15.Size = new System.Drawing.Size(86, 20);
            Label15.TabIndex = 19;
            Label15.Text = "External Name:";
            Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnI50
            // 
            btnI50.Location = new System.Drawing.Point(117, 319);
            btnI50.Name = "btnI50";
            btnI50.Size = new System.Drawing.Size(31, 24);
            btnI50.TabIndex = 28;
            btnI50.Text = "50";
            btnI50.UseVisualStyleBackColor = true;
            btnI50.Click += btnI50_Click;
            // 
            // btnI40
            // 
            btnI40.Location = new System.Drawing.Point(80, 319);
            btnI40.Name = "btnI40";
            btnI40.Size = new System.Drawing.Size(31, 24);
            btnI40.TabIndex = 27;
            btnI40.Text = "40";
            btnI40.UseVisualStyleBackColor = true;
            btnI40.Click += btnI40_Click;
            // 
            // btnI25
            // 
            btnI25.Location = new System.Drawing.Point(43, 319);
            btnI25.Name = "btnI25";
            btnI25.Size = new System.Drawing.Size(31, 24);
            btnI25.TabIndex = 26;
            btnI25.Text = "25";
            btnI25.UseVisualStyleBackColor = true;
            btnI25.Click += btnI25_Click;
            // 
            // btnI20
            // 
            btnI20.Location = new System.Drawing.Point(6, 319);
            btnI20.Name = "btnI20";
            btnI20.Size = new System.Drawing.Size(31, 24);
            btnI20.TabIndex = 25;
            btnI20.Text = "20";
            btnI20.UseVisualStyleBackColor = true;
            btnI20.Click += btnI20_Click;
            // 
            // btnIncrement
            // 
            btnIncrement.Location = new System.Drawing.Point(154, 319);
            btnIncrement.Name = "btnIncrement";
            btnIncrement.Size = new System.Drawing.Size(54, 24);
            btnIncrement.TabIndex = 24;
            btnIncrement.Text = "+ 1";
            btnIncrement.UseVisualStyleBackColor = true;
            btnIncrement.Click += btnIncrement_Click;
            // 
            // btnDel
            // 
            btnDel.Location = new System.Drawing.Point(54, 289);
            btnDel.Name = "btnDel";
            btnDel.Size = new System.Drawing.Size(100, 24);
            btnDel.TabIndex = 21;
            btnDel.Text = "Delete";
            btnDel.UseVisualStyleBackColor = true;
            btnDel.Click += btnDel_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(54, 259);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(100, 24);
            btnAdd.TabIndex = 20;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRAdd
            // 
            btnRAdd.Location = new System.Drawing.Point(12, 340);
            btnRAdd.Name = "btnRAdd";
            btnRAdd.Size = new System.Drawing.Size(100, 24);
            btnRAdd.TabIndex = 21;
            btnRAdd.Text = "Add";
            btnRAdd.UseVisualStyleBackColor = true;
            btnRAdd.Click += btnRAdd_Click;
            // 
            // btnRDel
            // 
            btnRDel.Location = new System.Drawing.Point(118, 340);
            btnRDel.Name = "btnRDel";
            btnRDel.Size = new System.Drawing.Size(100, 24);
            btnRDel.TabIndex = 22;
            btnRDel.Text = "Delete";
            btnRDel.UseVisualStyleBackColor = true;
            btnRDel.Click += btnRDel_Click;
            // 
            // btnRunSeq
            // 
            btnRunSeq.Enabled = false;
            btnRunSeq.Location = new System.Drawing.Point(574, 340);
            btnRunSeq.Name = "btnRunSeq";
            btnRunSeq.Size = new System.Drawing.Size(100, 24);
            btnRunSeq.TabIndex = 26;
            btnRunSeq.Text = "Run Sequence";
            btnRunSeq.UseVisualStyleBackColor = true;
            btnRunSeq.Visible = false;
            btnRunSeq.Click += btnRunSeq_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnI50);
            groupBox3.Controls.Add(lstItems);
            groupBox3.Controls.Add(btnI40);
            groupBox3.Controls.Add(btnAdd);
            groupBox3.Controls.Add(btnI25);
            groupBox3.Controls.Add(btnDel);
            groupBox3.Controls.Add(btnI20);
            groupBox3.Controls.Add(btnIncrement);
            groupBox3.Location = new System.Drawing.Point(900, 12);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(214, 352);
            groupBox3.TabIndex = 27;
            groupBox3.TabStop = false;
            groupBox3.Text = "Recipe Entries:";
            // 
            // btnMassUpdateTags
            // 
            btnMassUpdateTags.Location = new System.Drawing.Point(456, 536);
            btnMassUpdateTags.Name = "btnMassUpdateTags";
            btnMassUpdateTags.Size = new System.Drawing.Size(125, 24);
            btnMassUpdateTags.TabIndex = 31;
            btnMassUpdateTags.Text = "Bulk update tags";
            btnMassUpdateTags.UseVisualStyleBackColor = true;
            btnMassUpdateTags.Click += btnMassUpdateTags_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(label17);
            panel1.Location = new System.Drawing.Point(592, 536);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(293, 28);
            panel1.TabIndex = 32;
            panel1.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(122, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(155, 17);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 31;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(3, 5);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(120, 13);
            label17.TabIndex = 30;
            label17.Text = "Updating tags... 100%";
            // 
            // frmRecipeEdit
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new System.Drawing.Size(1127, 568);
            Controls.Add(panel1);
            Controls.Add(btnMassUpdateTags);
            Controls.Add(groupBox3);
            Controls.Add(btnRunSeq);
            Controls.Add(btnRDel);
            Controls.Add(btnRAdd);
            Controls.Add(GroupBox2);
            Controls.Add(GroupBox1);
            Controls.Add(btnReGuess);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(lvDPA);
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmRecipeEdit";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Recipe Editor";
            GroupBox1.ResumeLayout(false);
            ((ISupportInitialize)udSal4).EndInit();
            ((ISupportInitialize)udSal3).EndInit();
            ((ISupportInitialize)udSal2).EndInit();
            ((ISupportInitialize)udSal1).EndInit();
            ((ISupportInitialize)udSal0).EndInit();
            ((ISupportInitialize)udCraftM).EndInit();
            ((ISupportInitialize)udCraft).EndInit();
            ((ISupportInitialize)udBuyM).EndInit();
            ((ISupportInitialize)udBuy).EndInit();
            ((ISupportInitialize)udLevel).EndInit();
            GroupBox2.ResumeLayout(false);
            GroupBox2.PerformLayout();
            ((ISupportInitialize)udStaticIndex).EndInit();
            groupBox3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

        }
        #endregion

        Button btnAdd;
        Button btnCancel;
        Button btnDel;
        Button btnGuessCost;
        Button btnI20;
        Button btnI25;
        Button btnI40;
        Button btnI50;
        Button btnIncrement;
        Button btnOK;
        Button btnRAdd;
        Button btnRDel;
        Button btnReGuess;
        Button btnRunSeq;
        ComboBox cbEnh;
        ComboBox cbRarity;
        ComboBox cbSal0;
        ComboBox cbSal1;
        ComboBox cbSal2;
        ComboBox cbSal3;
        ComboBox cbSal4;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        Label Label1;
        Label Label10;
        Label Label11;
        Label Label12;
        Label Label13;
        Label Label14;
        Label Label15;
        Label Label2;
        Label Label3;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lblEnh;
        ListBox lstItems;
        ListView lvDPA;
        TextBox txtExtern;
        TextBox txtRecipeName;
        NumericUpDown udBuy;
        NumericUpDown udBuyM;
        NumericUpDown udCraft;
        NumericUpDown udCraftM;
        NumericUpDown udLevel;
        NumericUpDown udSal0;
        NumericUpDown udSal1;
        NumericUpDown udSal2;
        NumericUpDown udSal3;
        NumericUpDown udSal4;
        private GroupBox groupBox3;
        private CheckBox cbIsVirtual;
        private CheckBox cbIsGeneric;
        private CheckBox cbIsHidden;
        private ColumnHeader columnHeader5;
        private Label Label16;
        private ColumnHeader columnHeader6;
        private NumericUpDown udStaticIndex;
        private Button btnAutoMarkGeneric;
        private Button btnMassUpdateTags;
        private Panel panel1;
        private ProgressBar progressBar1;
        private Label label17;
    }
}