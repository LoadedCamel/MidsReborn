using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
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
            this.lvDPA = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnReGuess = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.cbIsRecipe4 = new System.Windows.Forms.CheckBox();
            this.cbIsRecipe3 = new System.Windows.Forms.CheckBox();
            this.cbIsRecipe2 = new System.Windows.Forms.CheckBox();
            this.cbIsRecipe1 = new System.Windows.Forms.CheckBox();
            this.cbIsRecipe0 = new System.Windows.Forms.CheckBox();
            this.btnGuessCost = new System.Windows.Forms.Button();
            this.udSal4 = new System.Windows.Forms.NumericUpDown();
            this.Label14 = new System.Windows.Forms.Label();
            this.cbSal4 = new System.Windows.Forms.ComboBox();
            this.udSal3 = new System.Windows.Forms.NumericUpDown();
            this.Label13 = new System.Windows.Forms.Label();
            this.cbSal3 = new System.Windows.Forms.ComboBox();
            this.udSal2 = new System.Windows.Forms.NumericUpDown();
            this.Label12 = new System.Windows.Forms.Label();
            this.cbSal2 = new System.Windows.Forms.ComboBox();
            this.udSal1 = new System.Windows.Forms.NumericUpDown();
            this.Label11 = new System.Windows.Forms.Label();
            this.cbSal1 = new System.Windows.Forms.ComboBox();
            this.udSal0 = new System.Windows.Forms.NumericUpDown();
            this.Label10 = new System.Windows.Forms.Label();
            this.cbSal0 = new System.Windows.Forms.ComboBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.udCraftM = new System.Windows.Forms.NumericUpDown();
            this.Label8 = new System.Windows.Forms.Label();
            this.udCraft = new System.Windows.Forms.NumericUpDown();
            this.Label7 = new System.Windows.Forms.Label();
            this.udBuyM = new System.Windows.Forms.NumericUpDown();
            this.Label6 = new System.Windows.Forms.Label();
            this.udBuy = new System.Windows.Forms.NumericUpDown();
            this.Label5 = new System.Windows.Forms.Label();
            this.udLevel = new System.Windows.Forms.NumericUpDown();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cbRarity = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtRecipeName = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.cbEnh = new System.Windows.Forms.ComboBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.cbIsHidden = new System.Windows.Forms.CheckBox();
            this.cbIsVirtual = new System.Windows.Forms.CheckBox();
            this.cbIsGeneric = new System.Windows.Forms.CheckBox();
            this.lblEnh = new System.Windows.Forms.Label();
            this.txtExtern = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.btnI50 = new System.Windows.Forms.Button();
            this.btnI40 = new System.Windows.Forms.Button();
            this.btnI25 = new System.Windows.Forms.Button();
            this.btnI20 = new System.Windows.Forms.Button();
            this.btnIncrement = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRAdd = new System.Windows.Forms.Button();
            this.btnRDel = new System.Windows.Forms.Button();
            this.btnRUp = new System.Windows.Forms.Button();
            this.btnRDown = new System.Windows.Forms.Button();
            this.btnRunSeq = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lstSubRecipeComponents = new System.Windows.Forms.ListBox();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udSal4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCraftM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCraft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBuyM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBuy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLevel)).BeginInit();
            this.GroupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvDPA
            // 
            this.lvDPA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2,
            this.ColumnHeader3,
            this.ColumnHeader4});
            this.lvDPA.FullRowSelect = true;
            this.lvDPA.HideSelection = false;
            this.lvDPA.Location = new System.Drawing.Point(12, 12);
            this.lvDPA.MultiSelect = false;
            this.lvDPA.Name = "lvDPA";
            this.lvDPA.Size = new System.Drawing.Size(599, 273);
            this.lvDPA.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvDPA.TabIndex = 0;
            this.lvDPA.UseCompatibleStateImageBehavior = false;
            this.lvDPA.View = System.Windows.Forms.View.Details;
            this.lvDPA.SelectedIndexChanged += new System.EventHandler(this.lvDPA_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Recipe";
            this.ColumnHeader1.Width = 226;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Enhancement";
            this.ColumnHeader2.Width = 183;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Rarity";
            this.ColumnHeader3.Width = 84;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Text = "Entries";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 491);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(113, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(131, 491);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(113, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Save && Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnReGuess
            // 
            this.btnReGuess.Location = new System.Drawing.Point(269, 491);
            this.btnReGuess.Name = "btnReGuess";
            this.btnReGuess.Size = new System.Drawing.Size(147, 24);
            this.btnReGuess.TabIndex = 7;
            this.btnReGuess.Text = "Re-Guess all recipes";
            this.btnReGuess.UseVisualStyleBackColor = true;
            this.btnReGuess.Click += new System.EventHandler(this.Button1_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.cbIsRecipe4);
            this.GroupBox1.Controls.Add(this.cbIsRecipe3);
            this.GroupBox1.Controls.Add(this.cbIsRecipe2);
            this.GroupBox1.Controls.Add(this.cbIsRecipe1);
            this.GroupBox1.Controls.Add(this.cbIsRecipe0);
            this.GroupBox1.Controls.Add(this.btnGuessCost);
            this.GroupBox1.Controls.Add(this.udSal4);
            this.GroupBox1.Controls.Add(this.Label14);
            this.GroupBox1.Controls.Add(this.cbSal4);
            this.GroupBox1.Controls.Add(this.udSal3);
            this.GroupBox1.Controls.Add(this.Label13);
            this.GroupBox1.Controls.Add(this.cbSal3);
            this.GroupBox1.Controls.Add(this.udSal2);
            this.GroupBox1.Controls.Add(this.Label12);
            this.GroupBox1.Controls.Add(this.cbSal2);
            this.GroupBox1.Controls.Add(this.udSal1);
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Controls.Add(this.cbSal1);
            this.GroupBox1.Controls.Add(this.udSal0);
            this.GroupBox1.Controls.Add(this.Label10);
            this.GroupBox1.Controls.Add(this.cbSal0);
            this.GroupBox1.Controls.Add(this.Label9);
            this.GroupBox1.Controls.Add(this.udCraftM);
            this.GroupBox1.Controls.Add(this.Label8);
            this.GroupBox1.Controls.Add(this.udCraft);
            this.GroupBox1.Controls.Add(this.Label7);
            this.GroupBox1.Controls.Add(this.udBuyM);
            this.GroupBox1.Controls.Add(this.Label6);
            this.GroupBox1.Controls.Add(this.udBuy);
            this.GroupBox1.Controls.Add(this.Label5);
            this.GroupBox1.Controls.Add(this.udLevel);
            this.GroupBox1.Location = new System.Drawing.Point(12, 321);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(688, 164);
            this.GroupBox1.TabIndex = 8;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Recipe Entry:";
            // 
            // cbIsRecipe4
            // 
            this.cbIsRecipe4.AutoSize = true;
            this.cbIsRecipe4.Location = new System.Drawing.Point(606, 135);
            this.cbIsRecipe4.Name = "cbIsRecipe4";
            this.cbIsRecipe4.Size = new System.Drawing.Size(76, 18);
            this.cbIsRecipe4.TabIndex = 355;
            this.cbIsRecipe4.Text = "Is a recipe";
            this.cbIsRecipe4.UseVisualStyleBackColor = true;
            this.cbIsRecipe4.CheckedChanged += new System.EventHandler(this.cbIsRecipe_CheckedChanged);
            // 
            // cbIsRecipe3
            // 
            this.cbIsRecipe3.AutoSize = true;
            this.cbIsRecipe3.Location = new System.Drawing.Point(606, 107);
            this.cbIsRecipe3.Name = "cbIsRecipe3";
            this.cbIsRecipe3.Size = new System.Drawing.Size(76, 18);
            this.cbIsRecipe3.TabIndex = 354;
            this.cbIsRecipe3.Text = "Is a recipe";
            this.cbIsRecipe3.UseVisualStyleBackColor = true;
            this.cbIsRecipe3.CheckedChanged += new System.EventHandler(this.cbIsRecipe_CheckedChanged);
            // 
            // cbIsRecipe2
            // 
            this.cbIsRecipe2.AutoSize = true;
            this.cbIsRecipe2.Location = new System.Drawing.Point(606, 78);
            this.cbIsRecipe2.Name = "cbIsRecipe2";
            this.cbIsRecipe2.Size = new System.Drawing.Size(76, 18);
            this.cbIsRecipe2.TabIndex = 353;
            this.cbIsRecipe2.Text = "Is a recipe";
            this.cbIsRecipe2.UseVisualStyleBackColor = true;
            this.cbIsRecipe2.CheckedChanged += new System.EventHandler(this.cbIsRecipe_CheckedChanged);
            // 
            // cbIsRecipe1
            // 
            this.cbIsRecipe1.AutoSize = true;
            this.cbIsRecipe1.Location = new System.Drawing.Point(606, 49);
            this.cbIsRecipe1.Name = "cbIsRecipe1";
            this.cbIsRecipe1.Size = new System.Drawing.Size(76, 18);
            this.cbIsRecipe1.TabIndex = 352;
            this.cbIsRecipe1.Text = "Is a recipe";
            this.cbIsRecipe1.UseVisualStyleBackColor = true;
            this.cbIsRecipe1.CheckedChanged += new System.EventHandler(this.cbIsRecipe_CheckedChanged);
            // 
            // cbIsRecipe0
            // 
            this.cbIsRecipe0.AutoSize = true;
            this.cbIsRecipe0.Location = new System.Drawing.Point(606, 23);
            this.cbIsRecipe0.Name = "cbIsRecipe0";
            this.cbIsRecipe0.Size = new System.Drawing.Size(76, 18);
            this.cbIsRecipe0.TabIndex = 351;
            this.cbIsRecipe0.Text = "Is a recipe";
            this.cbIsRecipe0.UseVisualStyleBackColor = true;
            this.cbIsRecipe0.CheckedChanged += new System.EventHandler(this.cbIsRecipe_CheckedChanged);
            // 
            // btnGuessCost
            // 
            this.btnGuessCost.Location = new System.Drawing.Point(185, 105);
            this.btnGuessCost.Name = "btnGuessCost";
            this.btnGuessCost.Size = new System.Drawing.Size(58, 20);
            this.btnGuessCost.TabIndex = 36;
            this.btnGuessCost.Text = "Guess";
            this.btnGuessCost.UseVisualStyleBackColor = true;
            this.btnGuessCost.Click += new System.EventHandler(this.btnGuessCost_Click);
            // 
            // udSal4
            // 
            this.udSal4.Location = new System.Drawing.Point(523, 133);
            this.udSal4.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.udSal4.Name = "udSal4";
            this.udSal4.Size = new System.Drawing.Size(59, 20);
            this.udSal4.TabIndex = 350;
            this.udSal4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSal4.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal4.Leave += new System.EventHandler(this.udSalX_Leave);
            // 
            // Label14
            // 
            this.Label14.Location = new System.Drawing.Point(225, 131);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(86, 22);
            this.Label14.TabIndex = 34;
            this.Label14.Text = "Ingredient 5:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal4
            // 
            this.cbSal4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal4.FormattingEnabled = true;
            this.cbSal4.Location = new System.Drawing.Point(317, 131);
            this.cbSal4.Name = "cbSal4";
            this.cbSal4.Size = new System.Drawing.Size(202, 22);
            this.cbSal4.TabIndex = 33;
            this.cbSal4.SelectedIndexChanged += new System.EventHandler(this.cbSalX_SelectedIndexChanged);
            // 
            // udSal3
            // 
            this.udSal3.Location = new System.Drawing.Point(523, 105);
            this.udSal3.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.udSal3.Name = "udSal3";
            this.udSal3.Size = new System.Drawing.Size(59, 20);
            this.udSal3.TabIndex = 320;
            this.udSal3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSal3.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal3.Leave += new System.EventHandler(this.udSalX_Leave);
            // 
            // Label13
            // 
            this.Label13.Location = new System.Drawing.Point(225, 103);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(86, 22);
            this.Label13.TabIndex = 31;
            this.Label13.Text = "Ingredient 4:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal3
            // 
            this.cbSal3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal3.FormattingEnabled = true;
            this.cbSal3.Location = new System.Drawing.Point(317, 103);
            this.cbSal3.Name = "cbSal3";
            this.cbSal3.Size = new System.Drawing.Size(202, 22);
            this.cbSal3.TabIndex = 30;
            this.cbSal3.SelectedIndexChanged += new System.EventHandler(this.cbSalX_SelectedIndexChanged);
            // 
            // udSal2
            // 
            this.udSal2.Location = new System.Drawing.Point(523, 77);
            this.udSal2.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.udSal2.Name = "udSal2";
            this.udSal2.Size = new System.Drawing.Size(59, 20);
            this.udSal2.TabIndex = 290;
            this.udSal2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSal2.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal2.Leave += new System.EventHandler(this.udSalX_Leave);
            // 
            // Label12
            // 
            this.Label12.Location = new System.Drawing.Point(225, 75);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(86, 22);
            this.Label12.TabIndex = 28;
            this.Label12.Text = "Ingredient 3:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal2
            // 
            this.cbSal2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal2.FormattingEnabled = true;
            this.cbSal2.Location = new System.Drawing.Point(317, 75);
            this.cbSal2.Name = "cbSal2";
            this.cbSal2.Size = new System.Drawing.Size(202, 22);
            this.cbSal2.TabIndex = 27;
            this.cbSal2.SelectedIndexChanged += new System.EventHandler(this.cbSalX_SelectedIndexChanged);
            // 
            // udSal1
            // 
            this.udSal1.Location = new System.Drawing.Point(523, 49);
            this.udSal1.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.udSal1.Name = "udSal1";
            this.udSal1.Size = new System.Drawing.Size(59, 20);
            this.udSal1.TabIndex = 260;
            this.udSal1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSal1.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal1.Leave += new System.EventHandler(this.udSalX_Leave);
            // 
            // Label11
            // 
            this.Label11.Location = new System.Drawing.Point(225, 47);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(86, 22);
            this.Label11.TabIndex = 25;
            this.Label11.Text = "Ingredient 2:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal1
            // 
            this.cbSal1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal1.FormattingEnabled = true;
            this.cbSal1.Location = new System.Drawing.Point(317, 47);
            this.cbSal1.Name = "cbSal1";
            this.cbSal1.Size = new System.Drawing.Size(202, 22);
            this.cbSal1.TabIndex = 24;
            this.cbSal1.SelectedIndexChanged += new System.EventHandler(this.cbSalX_SelectedIndexChanged);
            // 
            // udSal0
            // 
            this.udSal0.Location = new System.Drawing.Point(523, 21);
            this.udSal0.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.udSal0.Name = "udSal0";
            this.udSal0.Size = new System.Drawing.Size(59, 20);
            this.udSal0.TabIndex = 230;
            this.udSal0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal0.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSal0.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal0.Leave += new System.EventHandler(this.udSalX_Leave);
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(225, 19);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(86, 22);
            this.Label10.TabIndex = 22;
            this.Label10.Text = "Ingredient 1:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSal0
            // 
            this.cbSal0.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal0.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal0.FormattingEnabled = true;
            this.cbSal0.Location = new System.Drawing.Point(317, 19);
            this.cbSal0.Name = "cbSal0";
            this.cbSal0.Size = new System.Drawing.Size(202, 22);
            this.cbSal0.TabIndex = 21;
            this.cbSal0.SelectedIndexChanged += new System.EventHandler(this.cbSalX_SelectedIndexChanged);
            this.cbSal0.Enter += new System.EventHandler(this.cbSal0_Enter);
            this.cbSal0.Leave += new System.EventHandler(this.cbSal0_Leave);
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(6, 133);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(86, 20);
            this.Label9.TabIndex = 20;
            this.Label9.Text = "Craft Cost (M):";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udCraftM
            // 
            this.udCraftM.Location = new System.Drawing.Point(98, 133);
            this.udCraftM.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udCraftM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udCraftM.Name = "udCraftM";
            this.udCraftM.Size = new System.Drawing.Size(112, 20);
            this.udCraftM.TabIndex = 19;
            this.udCraftM.ThousandsSeparator = true;
            this.udCraftM.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udCraftM.Leave += new System.EventHandler(this.udCostX_Leave);
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(6, 105);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(86, 20);
            this.Label8.TabIndex = 18;
            this.Label8.Text = "Craft Cost:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udCraft
            // 
            this.udCraft.Location = new System.Drawing.Point(98, 105);
            this.udCraft.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udCraft.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udCraft.Name = "udCraft";
            this.udCraft.Size = new System.Drawing.Size(81, 20);
            this.udCraft.TabIndex = 17;
            this.udCraft.ThousandsSeparator = true;
            this.udCraft.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udCraft.Leave += new System.EventHandler(this.udCostX_Leave);
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(6, 77);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(86, 20);
            this.Label7.TabIndex = 16;
            this.Label7.Text = "Buy Cost (M):";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBuyM
            // 
            this.udBuyM.Location = new System.Drawing.Point(98, 77);
            this.udBuyM.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udBuyM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udBuyM.Name = "udBuyM";
            this.udBuyM.Size = new System.Drawing.Size(112, 20);
            this.udBuyM.TabIndex = 15;
            this.udBuyM.ThousandsSeparator = true;
            this.udBuyM.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udBuyM.Leave += new System.EventHandler(this.udCostX_Leave);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(6, 49);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(86, 20);
            this.Label6.TabIndex = 14;
            this.Label6.Text = "Buy Cost:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udBuy
            // 
            this.udBuy.Location = new System.Drawing.Point(98, 49);
            this.udBuy.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udBuy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udBuy.Name = "udBuy";
            this.udBuy.Size = new System.Drawing.Size(112, 20);
            this.udBuy.TabIndex = 13;
            this.udBuy.ThousandsSeparator = true;
            this.udBuy.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udBuy.Leave += new System.EventHandler(this.udCostX_Leave);
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(6, 21);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(86, 20);
            this.Label5.TabIndex = 12;
            this.Label5.Text = "Level:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udLevel
            // 
            this.udLevel.Location = new System.Drawing.Point(98, 21);
            this.udLevel.Maximum = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.udLevel.Name = "udLevel";
            this.udLevel.Size = new System.Drawing.Size(70, 20);
            this.udLevel.TabIndex = 0;
            this.udLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udLevel.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udLevel.Leave += new System.EventHandler(this.udCostX_Leave);
            // 
            // lstItems
            // 
            this.lstItems.FormattingEnabled = true;
            this.lstItems.ItemHeight = 14;
            this.lstItems.Location = new System.Drawing.Point(6, 22);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(202, 172);
            this.lstItems.TabIndex = 0;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.lstItems_SelectedIndexChanged);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(6, 104);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(86, 22);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Rarity:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cbRarity
            // 
            this.cbRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRarity.FormattingEnabled = true;
            this.cbRarity.Location = new System.Drawing.Point(6, 129);
            this.cbRarity.Name = "cbRarity";
            this.cbRarity.Size = new System.Drawing.Size(202, 22);
            this.cbRarity.TabIndex = 10;
            this.cbRarity.SelectedIndexChanged += new System.EventHandler(this.cbRarity_SelectedIndexChanged);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(6, 16);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(126, 20);
            this.Label1.TabIndex = 13;
            this.Label1.Text = "Internal Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtRecipeName
            // 
            this.txtRecipeName.Location = new System.Drawing.Point(6, 39);
            this.txtRecipeName.Name = "txtRecipeName";
            this.txtRecipeName.Size = new System.Drawing.Size(202, 20);
            this.txtRecipeName.TabIndex = 12;
            this.txtRecipeName.TextChanged += new System.EventHandler(this.txtRecipeName_TextChanged);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(6, 154);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(86, 18);
            this.Label2.TabIndex = 15;
            this.Label2.Text = "Enhancement:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cbEnh
            // 
            this.cbEnh.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbEnh.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbEnh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnh.FormattingEnabled = true;
            this.cbEnh.Location = new System.Drawing.Point(6, 172);
            this.cbEnh.Name = "cbEnh";
            this.cbEnh.Size = new System.Drawing.Size(202, 22);
            this.cbEnh.Sorted = true;
            this.cbEnh.TabIndex = 14;
            this.cbEnh.SelectedIndexChanged += new System.EventHandler(this.cbEnh_SelectedIndexChanged);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.cbIsHidden);
            this.GroupBox2.Controls.Add(this.cbIsVirtual);
            this.GroupBox2.Controls.Add(this.cbIsGeneric);
            this.GroupBox2.Controls.Add(this.lblEnh);
            this.GroupBox2.Controls.Add(this.txtExtern);
            this.GroupBox2.Controls.Add(this.Label15);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.txtRecipeName);
            this.GroupBox2.Controls.Add(this.cbEnh);
            this.GroupBox2.Controls.Add(this.cbRarity);
            this.GroupBox2.Controls.Add(this.Label1);
            this.GroupBox2.Controls.Add(this.Label3);
            this.GroupBox2.Location = new System.Drawing.Point(619, 12);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(214, 303);
            this.GroupBox2.TabIndex = 9;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Recipe:";
            // 
            // cbIsHidden
            // 
            this.cbIsHidden.AutoSize = true;
            this.cbIsHidden.Location = new System.Drawing.Point(16, 279);
            this.cbIsHidden.Name = "cbIsHidden";
            this.cbIsHidden.Size = new System.Drawing.Size(59, 18);
            this.cbIsHidden.TabIndex = 354;
            this.cbIsHidden.Text = "Hidden";
            this.cbIsHidden.UseVisualStyleBackColor = true;
            this.cbIsHidden.Click += new System.EventHandler(this.cbIsHidden_Click);
            // 
            // cbIsVirtual
            // 
            this.cbIsVirtual.AutoSize = true;
            this.cbIsVirtual.Location = new System.Drawing.Point(16, 255);
            this.cbIsVirtual.Name = "cbIsVirtual";
            this.cbIsVirtual.Size = new System.Drawing.Size(57, 18);
            this.cbIsVirtual.TabIndex = 353;
            this.cbIsVirtual.Text = "Virtual";
            this.cbIsVirtual.UseVisualStyleBackColor = true;
            this.cbIsVirtual.Click += new System.EventHandler(this.cbIsVirtual_Click);
            // 
            // cbIsGeneric
            // 
            this.cbIsGeneric.AutoSize = true;
            this.cbIsGeneric.Location = new System.Drawing.Point(16, 231);
            this.cbIsGeneric.Name = "cbIsGeneric";
            this.cbIsGeneric.Size = new System.Drawing.Size(64, 18);
            this.cbIsGeneric.TabIndex = 352;
            this.cbIsGeneric.Text = "Generic";
            this.cbIsGeneric.UseVisualStyleBackColor = true;
            this.cbIsGeneric.Click += new System.EventHandler(this.cbIsGeneric_Click);
            // 
            // lblEnh
            // 
            this.lblEnh.Location = new System.Drawing.Point(6, 200);
            this.lblEnh.Name = "lblEnh";
            this.lblEnh.Size = new System.Drawing.Size(202, 28);
            this.lblEnh.TabIndex = 17;
            this.lblEnh.Text = "EnhancementName";
            this.lblEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtExtern
            // 
            this.txtExtern.Location = new System.Drawing.Point(6, 85);
            this.txtExtern.Name = "txtExtern";
            this.txtExtern.Size = new System.Drawing.Size(202, 20);
            this.txtExtern.TabIndex = 18;
            this.txtExtern.TextChanged += new System.EventHandler(this.txtExtern_TextChanged);
            // 
            // Label15
            // 
            this.Label15.Location = new System.Drawing.Point(6, 62);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(86, 20);
            this.Label15.TabIndex = 19;
            this.Label15.Text = "External Name:";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnI50
            // 
            this.btnI50.Location = new System.Drawing.Point(117, 260);
            this.btnI50.Name = "btnI50";
            this.btnI50.Size = new System.Drawing.Size(31, 24);
            this.btnI50.TabIndex = 28;
            this.btnI50.Text = "50";
            this.btnI50.UseVisualStyleBackColor = true;
            this.btnI50.Click += new System.EventHandler(this.btnI50_Click);
            // 
            // btnI40
            // 
            this.btnI40.Location = new System.Drawing.Point(80, 260);
            this.btnI40.Name = "btnI40";
            this.btnI40.Size = new System.Drawing.Size(31, 24);
            this.btnI40.TabIndex = 27;
            this.btnI40.Text = "40";
            this.btnI40.UseVisualStyleBackColor = true;
            this.btnI40.Click += new System.EventHandler(this.btnI40_Click);
            // 
            // btnI25
            // 
            this.btnI25.Location = new System.Drawing.Point(43, 260);
            this.btnI25.Name = "btnI25";
            this.btnI25.Size = new System.Drawing.Size(31, 24);
            this.btnI25.TabIndex = 26;
            this.btnI25.Text = "25";
            this.btnI25.UseVisualStyleBackColor = true;
            this.btnI25.Click += new System.EventHandler(this.btnI25_Click);
            // 
            // btnI20
            // 
            this.btnI20.Location = new System.Drawing.Point(6, 260);
            this.btnI20.Name = "btnI20";
            this.btnI20.Size = new System.Drawing.Size(31, 24);
            this.btnI20.TabIndex = 25;
            this.btnI20.Text = "20";
            this.btnI20.UseVisualStyleBackColor = true;
            this.btnI20.Click += new System.EventHandler(this.btnI20_Click);
            // 
            // btnIncrement
            // 
            this.btnIncrement.Location = new System.Drawing.Point(154, 260);
            this.btnIncrement.Name = "btnIncrement";
            this.btnIncrement.Size = new System.Drawing.Size(54, 24);
            this.btnIncrement.TabIndex = 24;
            this.btnIncrement.Text = "+ 1";
            this.btnIncrement.UseVisualStyleBackColor = true;
            this.btnIncrement.Click += new System.EventHandler(this.btnIncrement_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(108, 230);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(100, 24);
            this.btnDown.TabIndex = 23;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(108, 200);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(100, 24);
            this.btnUp.TabIndex = 22;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(6, 230);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(100, 24);
            this.btnDel.TabIndex = 21;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 200);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 24);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRAdd
            // 
            this.btnRAdd.Location = new System.Drawing.Point(12, 291);
            this.btnRAdd.Name = "btnRAdd";
            this.btnRAdd.Size = new System.Drawing.Size(100, 24);
            this.btnRAdd.TabIndex = 21;
            this.btnRAdd.Text = "Add";
            this.btnRAdd.UseVisualStyleBackColor = true;
            this.btnRAdd.Click += new System.EventHandler(this.btnRAdd_Click);
            // 
            // btnRDel
            // 
            this.btnRDel.Location = new System.Drawing.Point(118, 291);
            this.btnRDel.Name = "btnRDel";
            this.btnRDel.Size = new System.Drawing.Size(100, 24);
            this.btnRDel.TabIndex = 22;
            this.btnRDel.Text = "Delete";
            this.btnRDel.UseVisualStyleBackColor = true;
            this.btnRDel.Click += new System.EventHandler(this.btnRDel_Click);
            // 
            // btnRUp
            // 
            this.btnRUp.Location = new System.Drawing.Point(405, 291);
            this.btnRUp.Name = "btnRUp";
            this.btnRUp.Size = new System.Drawing.Size(100, 24);
            this.btnRUp.TabIndex = 23;
            this.btnRUp.Text = "Up";
            this.btnRUp.UseVisualStyleBackColor = true;
            // 
            // btnRDown
            // 
            this.btnRDown.Location = new System.Drawing.Point(511, 291);
            this.btnRDown.Name = "btnRDown";
            this.btnRDown.Size = new System.Drawing.Size(100, 24);
            this.btnRDown.TabIndex = 24;
            this.btnRDown.Text = "Down";
            this.btnRDown.UseVisualStyleBackColor = true;
            // 
            // btnRunSeq
            // 
            this.btnRunSeq.Enabled = false;
            this.btnRunSeq.Location = new System.Drawing.Point(250, 291);
            this.btnRunSeq.Name = "btnRunSeq";
            this.btnRunSeq.Size = new System.Drawing.Size(100, 24);
            this.btnRunSeq.TabIndex = 26;
            this.btnRunSeq.Text = "Run Sequence";
            this.btnRunSeq.UseVisualStyleBackColor = true;
            this.btnRunSeq.Click += new System.EventHandler(this.btnRunSeq_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnI50);
            this.groupBox3.Controls.Add(this.lstItems);
            this.groupBox3.Controls.Add(this.btnI40);
            this.groupBox3.Controls.Add(this.btnAdd);
            this.groupBox3.Controls.Add(this.btnI25);
            this.groupBox3.Controls.Add(this.btnDel);
            this.groupBox3.Controls.Add(this.btnI20);
            this.groupBox3.Controls.Add(this.btnUp);
            this.groupBox3.Controls.Add(this.btnIncrement);
            this.groupBox3.Controls.Add(this.btnDown);
            this.groupBox3.Location = new System.Drawing.Point(839, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 303);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Recipe entries:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.lstSubRecipeComponents);
            this.groupBox4.Location = new System.Drawing.Point(706, 321);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(347, 164);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Recipe entries:";
            this.groupBox4.Visible = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 18);
            this.label4.TabIndex = 16;
            this.label4.Text = "Sub-recipe components (Ingredient #1):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lstSubRecipeComponents
            // 
            this.lstSubRecipeComponents.FormattingEnabled = true;
            this.lstSubRecipeComponents.ItemHeight = 14;
            this.lstSubRecipeComponents.Location = new System.Drawing.Point(6, 39);
            this.lstSubRecipeComponents.Name = "lstSubRecipeComponents";
            this.lstSubRecipeComponents.Size = new System.Drawing.Size(335, 116);
            this.lstSubRecipeComponents.TabIndex = 0;
            // 
            // frmRecipeEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1060, 526);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnRunSeq);
            this.Controls.Add(this.btnRDown);
            this.Controls.Add(this.btnRUp);
            this.Controls.Add(this.btnRDel);
            this.Controls.Add(this.btnRAdd);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.btnReGuess);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lvDPA);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecipeEdit";
            this.ShowInTaskbar = false;
            this.Text = "Recipe Editor";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udSal4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udSal0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCraftM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udCraft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBuyM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udBuy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLevel)).EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        Button btnAdd;
        Button btnCancel;
        Button btnDel;
        Button btnDown;
        Button btnGuessCost;
        Button btnI20;
        Button btnI25;
        Button btnI40;
        Button btnI50;
        Button btnIncrement;
        Button btnOK;
        Button btnRAdd;
        Button btnRDel;
        Button btnRDown;
        Button btnReGuess;
        Button btnRunSeq;
        Button btnRUp;
        Button btnUp;
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
        private CheckBox cbIsRecipe4;
        private CheckBox cbIsRecipe3;
        private CheckBox cbIsRecipe2;
        private CheckBox cbIsRecipe1;
        private CheckBox cbIsRecipe0;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label label4;
        private ListBox lstSubRecipeComponents;
        private CheckBox cbIsVirtual;
        private CheckBox cbIsGeneric;
        private CheckBox cbIsHidden;
    }
}