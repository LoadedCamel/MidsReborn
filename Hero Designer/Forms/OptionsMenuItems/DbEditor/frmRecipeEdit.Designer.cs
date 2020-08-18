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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.lvDPA = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnReGuess = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
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
            this.btnI50 = new System.Windows.Forms.Button();
            this.btnI40 = new System.Windows.Forms.Button();
            this.btnI25 = new System.Windows.Forms.Button();
            this.btnI20 = new System.Windows.Forms.Button();
            this.btnIncrement = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblEnh = new System.Windows.Forms.Label();
            this.txtExtern = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.btnRAdd = new System.Windows.Forms.Button();
            this.btnRDel = new System.Windows.Forms.Button();
            this.btnRUp = new System.Windows.Forms.Button();
            this.btnRDown = new System.Windows.Forms.Button();
            this.btnImportUpdate = new System.Windows.Forms.Button();
            this.btnRunSeq = new System.Windows.Forms.Button();
            this.GroupBox1.SuspendLayout();
            this.udSal4.BeginInit();
            this.udSal3.BeginInit();
            this.udSal2.BeginInit();
            this.udSal1.BeginInit();
            this.udSal0.BeginInit();
            this.udCraftM.BeginInit();
            this.udCraft.BeginInit();
            this.udBuyM.BeginInit();
            this.udBuy.BeginInit();
            this.udLevel.BeginInit();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            this.lvDPA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4]
            {
        this.ColumnHeader1,
        this.ColumnHeader2,
        this.ColumnHeader3,
        this.ColumnHeader4
            });
            this.lvDPA.FullRowSelect = true;
            this.lvDPA.HideSelection = false;

            this.lvDPA.Location = new System.Drawing.Point(12, 12);
            this.lvDPA.MultiSelect = false;
            this.lvDPA.Name = "lvDPA";

            this.lvDPA.Size = new System.Drawing.Size(599, 273);
            this.lvDPA.TabIndex = 0;
            this.lvDPA.UseCompatibleStateImageBehavior = false;
            this.lvDPA.View = System.Windows.Forms.View.Details;
            this.lvDPA.SelectedIndexChanged += new System.EventHandler(lvDPA_SelectedIndexChanged);
            this.ColumnHeader1.Text = "Recipe";
            this.ColumnHeader1.Width = 226;
            this.ColumnHeader2.Text = "Enhancement";
            this.ColumnHeader2.Width = 183;
            this.ColumnHeader3.Text = "Rarity";
            this.ColumnHeader3.Width = 84;
            this.ColumnHeader4.Text = "Entries";

            this.btnImport.Location = new System.Drawing.Point(356, 491);
            this.btnImport.Name = "btnImport";

            this.btnImport.Size = new System.Drawing.Size(102, 24);
            this.btnImport.TabIndex = 6;
            this.btnImport.Text = "Import w/Clear";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(btnImport_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(12, 491);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(113, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnOK.Location = new System.Drawing.Point(131, 491);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(113, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Save && Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(btnOK_Click);

            this.btnReGuess.Location = new System.Drawing.Point(464, 491);
            this.btnReGuess.Name = "btnReGuess";

            this.btnReGuess.Size = new System.Drawing.Size(147, 24);
            this.btnReGuess.TabIndex = 7;
            this.btnReGuess.Text = "Re-Guess all recipes";
            this.btnReGuess.UseVisualStyleBackColor = true;
            this.btnReGuess.Click += new System.EventHandler(Button1_Click);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnGuessCost);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udSal4);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label14);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSal4);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udSal3);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label13);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSal3);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udSal2);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label12);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSal2);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udSal1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label11);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSal1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udSal0);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label10);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSal0);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label9);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udCraftM);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label8);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udCraft);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label7);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udBuyM);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label6);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udBuy);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.udLevel);

            this.GroupBox1.Location = new System.Drawing.Point(12, 321);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(599, 164);
            this.GroupBox1.TabIndex = 8;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Recipe Entry:";

            this.btnGuessCost.Location = new System.Drawing.Point(185, 105);
            this.btnGuessCost.Name = "btnGuessCost";

            this.btnGuessCost.Size = new System.Drawing.Size(58, 20);
            this.btnGuessCost.TabIndex = 36;
            this.btnGuessCost.Text = "Guess";
            this.btnGuessCost.UseVisualStyleBackColor = true;
            this.btnGuessCost.Click += new System.EventHandler(btnGuessCost_Click);

            this.udSal4.Location = new System.Drawing.Point(523, 133);
            this.udSal4.Maximum = new System.Decimal(new int[4]
            {
        1024,
        0,
        0,
        0
            });
            this.udSal4.Name = "udSal4";

            this.udSal4.Size = new System.Drawing.Size(59, 20);
            this.udSal4.TabIndex = 350;
            this.udSal4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal4.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udSal4.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal4.Leave += new System.EventHandler(this.udSalX_Leave);

            this.Label14.Location = new System.Drawing.Point(225, 131);
            this.Label14.Name = "Label14";

            this.Label14.Size = new System.Drawing.Size(86, 22);
            this.Label14.TabIndex = 34;
            this.Label14.Text = "Ingredient 5:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSal4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal4.FormattingEnabled = true;

            this.cbSal4.Location = new System.Drawing.Point(317, 131);
            this.cbSal4.Name = "cbSal4";

            this.cbSal4.Size = new System.Drawing.Size(202, 22);
            this.cbSal4.TabIndex = 33;
            this.cbSal4.SelectedIndexChanged += new System.EventHandler(cbSalX_SelectedIndexChanged);

            this.udSal3.Location = new System.Drawing.Point(523, 105);
            this.udSal3.Maximum = new System.Decimal(new int[4] { 1024, 0, 0, 0 });
            this.udSal3.Name = "udSal3";

            this.udSal3.Size = new System.Drawing.Size(59, 20);
            this.udSal3.TabIndex = 320;
            this.udSal3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal3.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udSal3.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal3.Leave += new System.EventHandler(this.udSalX_Leave);

            this.Label13.Location = new System.Drawing.Point(225, 103);
            this.Label13.Name = "Label13";

            this.Label13.Size = new System.Drawing.Size(86, 22);
            this.Label13.TabIndex = 31;
            this.Label13.Text = "Ingredient 4:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSal3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal3.FormattingEnabled = true;

            this.cbSal3.Location = new System.Drawing.Point(317, 103);
            this.cbSal3.Name = "cbSal3";

            this.cbSal3.Size = new System.Drawing.Size(202, 22);
            this.cbSal3.TabIndex = 30;
            this.cbSal3.SelectedIndexChanged += new System.EventHandler(cbSalX_SelectedIndexChanged);

            this.udSal2.Location = new System.Drawing.Point(523, 77);
            this.udSal2.Maximum = new System.Decimal(new int[4] { 1024, 0, 0, 0 });
            this.udSal2.Name = "udSal2";

            this.udSal2.Size = new System.Drawing.Size(59, 20);
            this.udSal2.TabIndex = 290;
            this.udSal2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal2.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udSal2.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal2.Leave += new System.EventHandler(this.udSalX_Leave);

            this.Label12.Location = new System.Drawing.Point(225, 75);
            this.Label12.Name = "Label12";

            this.Label12.Size = new System.Drawing.Size(86, 22);
            this.Label12.TabIndex = 28;
            this.Label12.Text = "Ingredient 3:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSal2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal2.FormattingEnabled = true;

            this.cbSal2.Location = new System.Drawing.Point(317, 75);
            this.cbSal2.Name = "cbSal2";

            this.cbSal2.Size = new System.Drawing.Size(202, 22);
            this.cbSal2.TabIndex = 27;
            this.cbSal2.SelectedIndexChanged += new System.EventHandler(cbSalX_SelectedIndexChanged);

            this.udSal1.Location = new System.Drawing.Point(523, 49);
            this.udSal1.Maximum = new System.Decimal(new int[4] { 1024, 0, 0, 0 });
            this.udSal1.Name = "udSal1";

            this.udSal1.Size = new System.Drawing.Size(59, 20);
            this.udSal1.TabIndex = 260;
            this.udSal1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal1.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udSal1.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal1.Leave += new System.EventHandler(this.udSalX_Leave);

            this.Label11.Location = new System.Drawing.Point(225, 47);
            this.Label11.Name = "Label11";

            this.Label11.Size = new System.Drawing.Size(86, 22);
            this.Label11.TabIndex = 25;
            this.Label11.Text = "Ingredient 2:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSal1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal1.FormattingEnabled = true;

            this.cbSal1.Location = new System.Drawing.Point(317, 47);
            this.cbSal1.Name = "cbSal1";

            this.cbSal1.Size = new System.Drawing.Size(202, 22);
            this.cbSal1.TabIndex = 24;
            this.cbSal1.SelectedIndexChanged += new System.EventHandler(cbSalX_SelectedIndexChanged);

            this.udSal0.Location = new System.Drawing.Point(523, 21);
            this.udSal0.Maximum = new System.Decimal(new int[4] { 1024, 0, 0, 0 });
            this.udSal0.Name = "udSal0";

            this.udSal0.Size = new System.Drawing.Size(59, 20);
            this.udSal0.TabIndex = 230;
            this.udSal0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udSal0.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udSal0.ValueChanged += new System.EventHandler(this.udSalX_ValueChanged);
            this.udSal0.Leave += new System.EventHandler(this.udSalX_Leave);

            this.Label10.Location = new System.Drawing.Point(225, 19);
            this.Label10.Name = "Label10";

            this.Label10.Size = new System.Drawing.Size(86, 22);
            this.Label10.TabIndex = 22;
            this.Label10.Text = "Ingredient 1:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSal0.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbSal0.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbSal0.FormattingEnabled = true;

            this.cbSal0.Location = new System.Drawing.Point(317, 19);
            this.cbSal0.Name = "cbSal0";

            this.cbSal0.Size = new System.Drawing.Size(202, 22);
            this.cbSal0.TabIndex = 21;
            this.cbSal0.SelectedIndexChanged += new System.EventHandler(cbSalX_SelectedIndexChanged);

            this.Label9.Location = new System.Drawing.Point(6, 133);
            this.Label9.Name = "Label9";

            this.Label9.Size = new System.Drawing.Size(86, 20);
            this.Label9.TabIndex = 20;
            this.Label9.Text = "Craft Cost (M):";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udCraftM.Location = new System.Drawing.Point(98, 133);
            this.udCraftM.Maximum = new System.Decimal(new int[4] { 1000000, 0, 0, 0 });
            this.udCraftM.Minimum = new System.Decimal(new int[4] { 1, 0, 0, int.MinValue });
            this.udCraftM.Name = "udCraftM";

            this.udCraftM.Size = new System.Drawing.Size(112, 20);
            this.udCraftM.TabIndex = 19;
            this.udCraftM.ThousandsSeparator = true;
            this.udCraftM.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udCraftM.Leave += new System.EventHandler(this.udCostX_Leave);

            this.Label8.Location = new System.Drawing.Point(6, 105);
            this.Label8.Name = "Label8";

            this.Label8.Size = new System.Drawing.Size(86, 20);
            this.Label8.TabIndex = 18;
            this.Label8.Text = "Craft Cost:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udCraft.Location = new System.Drawing.Point(98, 105);
            this.udCraft.Maximum = new System.Decimal(new int[4] { 1000000, 0, 0, 0 });
            this.udCraft.Minimum = new System.Decimal(new int[4] { 1, 0, 0, int.MinValue });
            this.udCraft.Name = "udCraft";

            this.udCraft.Size = new System.Drawing.Size(81, 20);
            this.udCraft.TabIndex = 17;
            this.udCraft.ThousandsSeparator = true;
            this.udCraft.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udCraft.Leave += new System.EventHandler(this.udCostX_Leave);

            this.Label7.Location = new System.Drawing.Point(6, 77);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(86, 20);
            this.Label7.TabIndex = 16;
            this.Label7.Text = "Buy Cost (M):";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udBuyM.Location = new System.Drawing.Point(98, 77);
            this.udBuyM.Maximum = new System.Decimal(new int[4] { 1000000, 0, 0, 0 });
            this.udBuyM.Minimum = new System.Decimal(new int[4] { 1, 0, 0, int.MinValue });
            this.udBuyM.Name = "udBuyM";

            this.udBuyM.Size = new System.Drawing.Size(112, 20);
            this.udBuyM.TabIndex = 15;
            this.udBuyM.ThousandsSeparator = true;
            this.udBuyM.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udBuyM.Leave += new System.EventHandler(this.udCostX_Leave);

            this.Label6.Location = new System.Drawing.Point(6, 49);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(86, 20);
            this.Label6.TabIndex = 14;
            this.Label6.Text = "Buy Cost:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udBuy.Location = new System.Drawing.Point(98, 49);
            this.udBuy.Maximum = new System.Decimal(new int[4] { 1000000, 0, 0, 0 });
            this.udBuy.Minimum = new System.Decimal(new int[4] { 1, 0, 0, int.MinValue });
            this.udBuy.Name = "udBuy";

            this.udBuy.Size = new System.Drawing.Size(112, 20);
            this.udBuy.TabIndex = 13;
            this.udBuy.ThousandsSeparator = true;
            this.udBuy.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udBuy.Leave += new System.EventHandler(this.udCostX_Leave);

            this.Label5.Location = new System.Drawing.Point(6, 21);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(86, 20);
            this.Label5.TabIndex = 12;
            this.Label5.Text = "Level:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udLevel.Location = new System.Drawing.Point(98, 21);
            this.udLevel.Maximum = new System.Decimal(new int[4] { 53, 0, 0, 0 });
            this.udLevel.Name = "udLevel";

            this.udLevel.Size = new System.Drawing.Size(70, 20);
            this.udLevel.TabIndex = 0;
            this.udLevel.Value = new System.Decimal(new int[4] { 1, 0, 0, 0 });
            this.udLevel.ValueChanged += new System.EventHandler(this.udCostX_ValueChanged);
            this.udLevel.Leave += new System.EventHandler(this.udCostX_Leave);
            this.lstItems.FormattingEnabled = true;
            this.lstItems.ItemHeight = 14;

            this.lstItems.Location = new System.Drawing.Point(6, 251);
            this.lstItems.Name = "lstItems";

            this.lstItems.Size = new System.Drawing.Size(202, 172);
            this.lstItems.TabIndex = 0;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(lstItems_SelectedIndexChanged);

            this.Label3.Location = new System.Drawing.Point(6, 104);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(86, 22);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Rarity:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRarity.FormattingEnabled = true;

            this.cbRarity.Location = new System.Drawing.Point(6, 129);
            this.cbRarity.Name = "cbRarity";

            this.cbRarity.Size = new System.Drawing.Size(158, 22);
            this.cbRarity.TabIndex = 10;
            this.cbRarity.SelectedIndexChanged += new System.EventHandler(cbRarity_SelectedIndexChanged);

            this.Label1.Location = new System.Drawing.Point(6, 16);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(126, 20);
            this.Label1.TabIndex = 13;
            this.Label1.Text = "Internal Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.txtRecipeName.Location = new System.Drawing.Point(6, 39);
            this.txtRecipeName.Name = "txtRecipeName";

            this.txtRecipeName.Size = new System.Drawing.Size(158, 20);
            this.txtRecipeName.TabIndex = 12;
            this.txtRecipeName.TextChanged += new System.EventHandler(txtRecipeName_TextChanged);

            this.Label2.Location = new System.Drawing.Point(6, 154);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(86, 18);
            this.Label2.TabIndex = 15;
            this.Label2.Text = "Enhancement:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbEnh.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cbEnh.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbEnh.FormattingEnabled = true;

            this.cbEnh.Location = new System.Drawing.Point(6, 172);
            this.cbEnh.Name = "cbEnh";

            this.cbEnh.Size = new System.Drawing.Size(202, 22);
            this.cbEnh.TabIndex = 14;
            this.cbEnh.SelectedIndexChanged += new System.EventHandler(cbEnh_SelectedIndexChanged);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnI50);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnI40);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnI25);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnI20);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnIncrement);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnDown);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnUp);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnDel);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnAdd);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lblEnh);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtExtern);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label15);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lstItems);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtRecipeName);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbEnh);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbRarity);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label3);

            this.GroupBox2.Location = new System.Drawing.Point(617, 12);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(214, 523);
            this.GroupBox2.TabIndex = 9;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Recipe:";

            this.btnI50.Location = new System.Drawing.Point(117, 489);
            this.btnI50.Name = "btnI50";

            this.btnI50.Size = new System.Drawing.Size(31, 24);
            this.btnI50.TabIndex = 28;
            this.btnI50.Text = "50";
            this.btnI50.UseVisualStyleBackColor = true;
            this.btnI50.Click += new System.EventHandler(btnI50_Click);

            this.btnI40.Location = new System.Drawing.Point(80, 489);
            this.btnI40.Name = "btnI40";

            this.btnI40.Size = new System.Drawing.Size(31, 24);
            this.btnI40.TabIndex = 27;
            this.btnI40.Text = "40";
            this.btnI40.UseVisualStyleBackColor = true;
            this.btnI40.Click += new System.EventHandler(btnI40_Click);

            this.btnI25.Location = new System.Drawing.Point(43, 489);
            this.btnI25.Name = "btnI25";

            this.btnI25.Size = new System.Drawing.Size(31, 24);
            this.btnI25.TabIndex = 26;
            this.btnI25.Text = "25";
            this.btnI25.UseVisualStyleBackColor = true;
            this.btnI25.Click += new System.EventHandler(btnI25_Click);

            this.btnI20.Location = new System.Drawing.Point(6, 489);
            this.btnI20.Name = "btnI20";

            this.btnI20.Size = new System.Drawing.Size(31, 24);
            this.btnI20.TabIndex = 25;
            this.btnI20.Text = "20";
            this.btnI20.UseVisualStyleBackColor = true;
            this.btnI20.Click += new System.EventHandler(btnI20_Click);

            this.btnIncrement.Location = new System.Drawing.Point(154, 489);
            this.btnIncrement.Name = "btnIncrement";

            this.btnIncrement.Size = new System.Drawing.Size(54, 24);
            this.btnIncrement.TabIndex = 24;
            this.btnIncrement.Text = "+ 1";
            this.btnIncrement.UseVisualStyleBackColor = true;
            this.btnIncrement.Click += new System.EventHandler(btnIncrement_Click);

            this.btnDown.Location = new System.Drawing.Point(108, 459);
            this.btnDown.Name = "btnDown";

            this.btnDown.Size = new System.Drawing.Size(100, 24);
            this.btnDown.TabIndex = 23;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;

            this.btnUp.Location = new System.Drawing.Point(108, 429);
            this.btnUp.Name = "btnUp";

            this.btnUp.Size = new System.Drawing.Size(100, 24);
            this.btnUp.TabIndex = 22;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;

            this.btnDel.Location = new System.Drawing.Point(6, 459);
            this.btnDel.Name = "btnDel";

            this.btnDel.Size = new System.Drawing.Size(100, 24);
            this.btnDel.TabIndex = 21;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(btnDel_Click);

            this.btnAdd.Location = new System.Drawing.Point(6, 429);
            this.btnAdd.Name = "btnAdd";

            this.btnAdd.Size = new System.Drawing.Size(100, 24);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(btnAdd_Click);

            this.lblEnh.Location = new System.Drawing.Point(6, 196);
            this.lblEnh.Name = "lblEnh";

            this.lblEnh.Size = new System.Drawing.Size(202, 40);
            this.lblEnh.TabIndex = 17;
            this.lblEnh.Text = "EnhancementName";
            this.lblEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.txtExtern.Location = new System.Drawing.Point(6, 85);
            this.txtExtern.Name = "txtExtern";

            this.txtExtern.Size = new System.Drawing.Size(158, 20);
            this.txtExtern.TabIndex = 18;
            this.txtExtern.TextChanged += new System.EventHandler(txtExtern_TextChanged);

            this.Label15.Location = new System.Drawing.Point(6, 62);
            this.Label15.Name = "Label15";

            this.Label15.Size = new System.Drawing.Size(86, 20);
            this.Label15.TabIndex = 19;
            this.Label15.Text = "External Name:";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.Label4.Location = new System.Drawing.Point(6, 226);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(86, 22);
            this.Label4.TabIndex = 16;
            this.Label4.Text = "Recipe Entries:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.btnRAdd.Location = new System.Drawing.Point(12, 291);
            this.btnRAdd.Name = "btnRAdd";

            this.btnRAdd.Size = new System.Drawing.Size(100, 24);
            this.btnRAdd.TabIndex = 21;
            this.btnRAdd.Text = "Add";
            this.btnRAdd.UseVisualStyleBackColor = true;
            this.btnRAdd.Click += new System.EventHandler(btnRAdd_Click);

            this.btnRDel.Location = new System.Drawing.Point(118, 291);
            this.btnRDel.Name = "btnRDel";

            this.btnRDel.Size = new System.Drawing.Size(100, 24);
            this.btnRDel.TabIndex = 22;
            this.btnRDel.Text = "Delete";
            this.btnRDel.UseVisualStyleBackColor = true;
            this.btnRDel.Click += new System.EventHandler(btnRDel_Click);

            this.btnRUp.Location = new System.Drawing.Point(405, 291);
            this.btnRUp.Name = "btnRUp";

            this.btnRUp.Size = new System.Drawing.Size(100, 24);
            this.btnRUp.TabIndex = 23;
            this.btnRUp.Text = "Up";
            this.btnRUp.UseVisualStyleBackColor = true;

            this.btnRDown.Location = new System.Drawing.Point(511, 291);
            this.btnRDown.Name = "btnRDown";

            this.btnRDown.Size = new System.Drawing.Size(100, 24);
            this.btnRDown.TabIndex = 24;
            this.btnRDown.Text = "Down";
            this.btnRDown.UseVisualStyleBackColor = true;

            this.btnImportUpdate.Location = new System.Drawing.Point(250, 491);
            this.btnImportUpdate.Name = "btnImportUpdate";

            this.btnImportUpdate.Size = new System.Drawing.Size(100, 24);
            this.btnImportUpdate.TabIndex = 25;
            this.btnImportUpdate.Text = "Import Update";
            this.btnImportUpdate.UseVisualStyleBackColor = true;
            this.btnRunSeq.Enabled = false;

            this.btnRunSeq.Location = new System.Drawing.Point(250, 291);
            this.btnRunSeq.Name = "btnRunSeq";

            this.btnRunSeq.Size = new System.Drawing.Size(100, 24);
            this.btnRunSeq.TabIndex = 26;
            this.btnRunSeq.Text = "Run Sequence";
            this.btnRunSeq.UseVisualStyleBackColor = true;
            this.btnRunSeq.Click += new System.EventHandler(btnRunSeq_Click);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            this.ClientSize = new System.Drawing.Size(843, 537);
            this.Controls.Add((System.Windows.Forms.Control)this.btnRunSeq);
            this.Controls.Add((System.Windows.Forms.Control)this.btnImportUpdate);
            this.Controls.Add((System.Windows.Forms.Control)this.btnRDown);
            this.Controls.Add((System.Windows.Forms.Control)this.btnRUp);
            this.Controls.Add((System.Windows.Forms.Control)this.btnRDel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnRAdd);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.btnReGuess);
            this.Controls.Add((System.Windows.Forms.Control)this.btnImport);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control)this.lvDPA);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Text = "Recipe Editor";
            this.GroupBox1.ResumeLayout(false);
            this.udSal4.EndInit();
            this.udSal3.EndInit();
            this.udSal2.EndInit();
            this.udSal1.EndInit();
            this.udSal0.EndInit();
            this.udCraftM.EndInit();
            this.udCraft.EndInit();
            this.udBuyM.EndInit();
            this.udBuy.EndInit();
            this.udLevel.EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
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
        Button btnImport;
        Button btnImportUpdate;
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
        Label Label4;
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
    }
}