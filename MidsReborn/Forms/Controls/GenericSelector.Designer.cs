namespace Mids_Reborn.Forms.Controls
{
    partial class GenericSelector
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
        protected void InitializeComponent()
        {
            formPages1 = new Mids_Reborn.Controls.FormPages();
            page1 = new Mids_Reborn.Controls.Page();
            p0NoneBtn = new System.Windows.Forms.Button();
            cbGroup = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            p0CancelBtn = new System.Windows.Forms.Button();
            p0OkBtn = new System.Windows.Forms.Button();
            cbPower = new System.Windows.Forms.ComboBox();
            cbPowerset = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            page2 = new Mids_Reborn.Controls.Page();
            checkPlayableAtOnly = new System.Windows.Forms.CheckBox();
            p1NoneBtn = new System.Windows.Forms.Button();
            p1CancelBtn = new System.Windows.Forms.Button();
            p1OkBtn = new System.Windows.Forms.Button();
            cbArchetypes = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            page3 = new Mids_Reborn.Controls.Page();
            p2NoneBtn = new System.Windows.Forms.Button();
            cbP2Group = new System.Windows.Forms.ComboBox();
            label7 = new System.Windows.Forms.Label();
            p2CancelBtn = new System.Windows.Forms.Button();
            p2OkBtn = new System.Windows.Forms.Button();
            cbP2Powerset = new System.Windows.Forms.ComboBox();
            label9 = new System.Windows.Forms.Label();
            page4 = new Mids_Reborn.Controls.Page();
            p3NoneBtn = new System.Windows.Forms.Button();
            p3CancelBtn = new System.Windows.Forms.Button();
            p3OkBtn = new System.Windows.Forms.Button();
            cbVectors = new System.Windows.Forms.ComboBox();
            label12 = new System.Windows.Forms.Label();
            page5 = new Mids_Reborn.Controls.Page();
            button1 = new System.Windows.Forms.Button();
            p4CancelButton = new System.Windows.Forms.Button();
            p4OkBtn = new System.Windows.Forms.Button();
            cbModifier = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            page2.SuspendLayout();
            page3.SuspendLayout();
            page4.SuspendLayout();
            page5.SuspendLayout();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Controls.Add(page3);
            formPages1.Controls.Add(page4);
            formPages1.Controls.Add(page5);
            formPages1.Location = new System.Drawing.Point(0, 0);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.Pages.Add(page3);
            formPages1.Pages.Add(page4);
            formPages1.Pages.Add(page5);
            formPages1.SelectedIndex = 2;
            formPages1.Size = new System.Drawing.Size(228, 300);
            formPages1.TabIndex = 11;
            // 
            // page1
            // 
            page1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page1.Anchor = System.Windows.Forms.AnchorStyles.None;
            page1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page1.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page1.Controls.Add(p0NoneBtn);
            page1.Controls.Add(cbGroup);
            page1.Controls.Add(label4);
            page1.Controls.Add(p0CancelBtn);
            page1.Controls.Add(p0OkBtn);
            page1.Controls.Add(cbPower);
            page1.Controls.Add(cbPowerset);
            page1.Controls.Add(label3);
            page1.Controls.Add(label2);
            page1.Dock = System.Windows.Forms.DockStyle.Fill;
            page1.ForeColor = System.Drawing.Color.WhiteSmoke;
            page1.Location = new System.Drawing.Point(0, 0);
            page1.Name = "page1";
            page1.Size = new System.Drawing.Size(228, 300);
            page1.TabIndex = 0;
            page1.Title = "Powers";
            // 
            // p0NoneBtn
            // 
            p0NoneBtn.BackColor = System.Drawing.Color.FromArgb(83, 70, 170);
            p0NoneBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p0NoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p0NoneBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p0NoneBtn.Location = new System.Drawing.Point(35, 265);
            p0NoneBtn.Name = "p0NoneBtn";
            p0NoneBtn.Size = new System.Drawing.Size(153, 23);
            p0NoneBtn.TabIndex = 19;
            p0NoneBtn.Text = "Choose None";
            p0NoneBtn.UseVisualStyleBackColor = false;
            p0NoneBtn.Click += btnChooseNone_Click;
            // 
            // cbGroup
            // 
            cbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGroup.FormattingEnabled = true;
            cbGroup.Location = new System.Drawing.Point(12, 37);
            cbGroup.Name = "cbGroup";
            cbGroup.Size = new System.Drawing.Size(204, 23);
            cbGroup.TabIndex = 17;
            cbGroup.SelectedIndexChanged += CbGroupOnSelectedIndexChanged;
            // 
            // label4
            // 
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(12, 11);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(204, 23);
            label4.TabIndex = 16;
            label4.Text = "Group";
            // 
            // p0CancelBtn
            // 
            p0CancelBtn.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            p0CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            p0CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p0CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p0CancelBtn.Location = new System.Drawing.Point(35, 227);
            p0CancelBtn.Name = "p0CancelBtn";
            p0CancelBtn.Size = new System.Drawing.Size(153, 23);
            p0CancelBtn.TabIndex = 15;
            p0CancelBtn.Text = "Cancel";
            p0CancelBtn.UseVisualStyleBackColor = false;
            p0CancelBtn.Click += btnCancel_Click;
            // 
            // p0OkBtn
            // 
            p0OkBtn.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            p0OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p0OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p0OkBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p0OkBtn.Location = new System.Drawing.Point(35, 198);
            p0OkBtn.Name = "p0OkBtn";
            p0OkBtn.Size = new System.Drawing.Size(153, 23);
            p0OkBtn.TabIndex = 14;
            p0OkBtn.Text = "Ok";
            p0OkBtn.UseVisualStyleBackColor = false;
            p0OkBtn.Click += btnOk_Click;
            // 
            // cbPower
            // 
            cbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPower.FormattingEnabled = true;
            cbPower.Location = new System.Drawing.Point(12, 160);
            cbPower.Name = "cbPower";
            cbPower.Size = new System.Drawing.Size(204, 23);
            cbPower.TabIndex = 13;
            cbPower.SelectedIndexChanged += CbPowerOnSelectedIndexChanged;
            // 
            // cbPowerset
            // 
            cbPowerset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPowerset.FormattingEnabled = true;
            cbPowerset.Location = new System.Drawing.Point(12, 99);
            cbPowerset.Name = "cbPowerset";
            cbPowerset.Size = new System.Drawing.Size(204, 23);
            cbPowerset.TabIndex = 12;
            cbPowerset.SelectedIndexChanged += CbPowersetOnSelectedIndexChanged;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(12, 134);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(204, 23);
            label3.TabIndex = 11;
            label3.Text = "Power";
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(12, 73);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(204, 23);
            label2.TabIndex = 10;
            label2.Text = "Powerset";
            // 
            // page2
            // 
            page2.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page2.Anchor = System.Windows.Forms.AnchorStyles.None;
            page2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page2.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page2.Controls.Add(checkPlayableAtOnly);
            page2.Controls.Add(p1NoneBtn);
            page2.Controls.Add(p1CancelBtn);
            page2.Controls.Add(p1OkBtn);
            page2.Controls.Add(cbArchetypes);
            page2.Controls.Add(label6);
            page2.Dock = System.Windows.Forms.DockStyle.Fill;
            page2.ForeColor = System.Drawing.Color.WhiteSmoke;
            page2.Location = new System.Drawing.Point(0, 0);
            page2.Name = "page2";
            page2.Size = new System.Drawing.Size(228, 300);
            page2.TabIndex = 1;
            page2.Title = "Archetypes";
            // 
            // checkPlayableAtOnly
            // 
            checkPlayableAtOnly.AutoSize = true;
            checkPlayableAtOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            checkPlayableAtOnly.Location = new System.Drawing.Point(48, 131);
            checkPlayableAtOnly.Name = "checkPlayableAtOnly";
            checkPlayableAtOnly.Size = new System.Drawing.Size(123, 19);
            checkPlayableAtOnly.TabIndex = 20;
            checkPlayableAtOnly.Text = "Player ATs only";
            checkPlayableAtOnly.UseVisualStyleBackColor = true;
            checkPlayableAtOnly.CheckedChanged += checkPlayableAtOnly_CheckedChanged;
            // 
            // p1NoneBtn
            // 
            p1NoneBtn.BackColor = System.Drawing.Color.FromArgb(83, 70, 170);
            p1NoneBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p1NoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p1NoneBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p1NoneBtn.Location = new System.Drawing.Point(35, 265);
            p1NoneBtn.Name = "p1NoneBtn";
            p1NoneBtn.Size = new System.Drawing.Size(153, 23);
            p1NoneBtn.TabIndex = 19;
            p1NoneBtn.Text = "Choose None";
            p1NoneBtn.UseVisualStyleBackColor = false;
            p1NoneBtn.Click += btnChooseNone_Click;
            // 
            // p1CancelBtn
            // 
            p1CancelBtn.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            p1CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            p1CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p1CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p1CancelBtn.Location = new System.Drawing.Point(35, 227);
            p1CancelBtn.Name = "p1CancelBtn";
            p1CancelBtn.Size = new System.Drawing.Size(153, 23);
            p1CancelBtn.TabIndex = 15;
            p1CancelBtn.Text = "Cancel";
            p1CancelBtn.UseVisualStyleBackColor = false;
            p1CancelBtn.Click += btnCancel_Click;
            // 
            // p1OkBtn
            // 
            p1OkBtn.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            p1OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p1OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p1OkBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p1OkBtn.Location = new System.Drawing.Point(35, 198);
            p1OkBtn.Name = "p1OkBtn";
            p1OkBtn.Size = new System.Drawing.Size(153, 23);
            p1OkBtn.TabIndex = 14;
            p1OkBtn.Text = "Ok";
            p1OkBtn.UseVisualStyleBackColor = false;
            p1OkBtn.Click += btnOk_Click;
            // 
            // cbArchetypes
            // 
            cbArchetypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbArchetypes.FormattingEnabled = true;
            cbArchetypes.Location = new System.Drawing.Point(12, 62);
            cbArchetypes.Name = "cbArchetypes";
            cbArchetypes.Size = new System.Drawing.Size(204, 23);
            cbArchetypes.TabIndex = 12;
            // 
            // label6
            // 
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(12, 36);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(204, 23);
            label6.TabIndex = 10;
            label6.Text = "Archetype";
            // 
            // page3
            // 
            page3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page3.Anchor = System.Windows.Forms.AnchorStyles.None;
            page3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page3.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page3.Controls.Add(p2NoneBtn);
            page3.Controls.Add(cbP2Group);
            page3.Controls.Add(label7);
            page3.Controls.Add(p2CancelBtn);
            page3.Controls.Add(p2OkBtn);
            page3.Controls.Add(cbP2Powerset);
            page3.Controls.Add(label9);
            page3.Dock = System.Windows.Forms.DockStyle.Fill;
            page3.ForeColor = System.Drawing.Color.WhiteSmoke;
            page3.Location = new System.Drawing.Point(0, 0);
            page3.Name = "page3";
            page3.Size = new System.Drawing.Size(228, 300);
            page3.TabIndex = 2;
            page3.Title = "Power Groups";
            // 
            // p2NoneBtn
            // 
            p2NoneBtn.BackColor = System.Drawing.Color.FromArgb(83, 70, 170);
            p2NoneBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p2NoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p2NoneBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p2NoneBtn.Location = new System.Drawing.Point(35, 265);
            p2NoneBtn.Name = "p2NoneBtn";
            p2NoneBtn.Size = new System.Drawing.Size(153, 23);
            p2NoneBtn.TabIndex = 19;
            p2NoneBtn.Text = "Choose None";
            p2NoneBtn.UseVisualStyleBackColor = false;
            p2NoneBtn.Click += btnChooseNone_Click;
            // 
            // cbP2Group
            // 
            cbP2Group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbP2Group.FormattingEnabled = true;
            cbP2Group.Location = new System.Drawing.Point(12, 46);
            cbP2Group.Name = "cbP2Group";
            cbP2Group.Size = new System.Drawing.Size(204, 23);
            cbP2Group.TabIndex = 17;
            cbP2Group.SelectedIndexChanged += cbP2Group_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(12, 20);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(204, 23);
            label7.TabIndex = 16;
            label7.Text = "Group";
            // 
            // p2CancelBtn
            // 
            p2CancelBtn.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            p2CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            p2CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p2CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p2CancelBtn.Location = new System.Drawing.Point(35, 227);
            p2CancelBtn.Name = "p2CancelBtn";
            p2CancelBtn.Size = new System.Drawing.Size(153, 23);
            p2CancelBtn.TabIndex = 15;
            p2CancelBtn.Text = "Cancel";
            p2CancelBtn.UseVisualStyleBackColor = false;
            p2CancelBtn.Click += btnCancel_Click;
            // 
            // p2OkBtn
            // 
            p2OkBtn.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            p2OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p2OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p2OkBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p2OkBtn.Location = new System.Drawing.Point(35, 198);
            p2OkBtn.Name = "p2OkBtn";
            p2OkBtn.Size = new System.Drawing.Size(153, 23);
            p2OkBtn.TabIndex = 14;
            p2OkBtn.Text = "Ok";
            p2OkBtn.UseVisualStyleBackColor = false;
            p2OkBtn.Click += btnOk_Click;
            // 
            // cbP2Powerset
            // 
            cbP2Powerset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbP2Powerset.FormattingEnabled = true;
            cbP2Powerset.Location = new System.Drawing.Point(12, 138);
            cbP2Powerset.Name = "cbP2Powerset";
            cbP2Powerset.Size = new System.Drawing.Size(204, 23);
            cbP2Powerset.TabIndex = 12;
            cbP2Powerset.SelectedIndexChanged += cbP2Powerset_SelectedIndexChanged;
            // 
            // label9
            // 
            label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label9.Location = new System.Drawing.Point(12, 112);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(204, 23);
            label9.TabIndex = 10;
            label9.Text = "Powerset";
            // 
            // page4
            // 
            page4.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page4.Anchor = System.Windows.Forms.AnchorStyles.None;
            page4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page4.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page4.Controls.Add(p3NoneBtn);
            page4.Controls.Add(p3CancelBtn);
            page4.Controls.Add(p3OkBtn);
            page4.Controls.Add(cbVectors);
            page4.Controls.Add(label12);
            page4.Dock = System.Windows.Forms.DockStyle.Fill;
            page4.ForeColor = System.Drawing.Color.WhiteSmoke;
            page4.Location = new System.Drawing.Point(0, 0);
            page4.Name = "page4";
            page4.Size = new System.Drawing.Size(228, 300);
            page4.TabIndex = 3;
            page4.Title = "Attack Vectors";
            // 
            // p3NoneBtn
            // 
            p3NoneBtn.BackColor = System.Drawing.Color.FromArgb(83, 70, 170);
            p3NoneBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p3NoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p3NoneBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p3NoneBtn.Location = new System.Drawing.Point(35, 265);
            p3NoneBtn.Name = "p3NoneBtn";
            p3NoneBtn.Size = new System.Drawing.Size(153, 23);
            p3NoneBtn.TabIndex = 18;
            p3NoneBtn.Text = "Choose None";
            p3NoneBtn.UseVisualStyleBackColor = false;
            p3NoneBtn.Click += btnChooseNone_Click;
            // 
            // p3CancelBtn
            // 
            p3CancelBtn.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            p3CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            p3CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p3CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p3CancelBtn.Location = new System.Drawing.Point(35, 227);
            p3CancelBtn.Name = "p3CancelBtn";
            p3CancelBtn.Size = new System.Drawing.Size(153, 23);
            p3CancelBtn.TabIndex = 15;
            p3CancelBtn.Text = "Cancel";
            p3CancelBtn.UseVisualStyleBackColor = false;
            p3CancelBtn.Click += btnCancel_Click;
            // 
            // p3OkBtn
            // 
            p3OkBtn.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            p3OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p3OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p3OkBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p3OkBtn.Location = new System.Drawing.Point(35, 198);
            p3OkBtn.Name = "p3OkBtn";
            p3OkBtn.Size = new System.Drawing.Size(153, 23);
            p3OkBtn.TabIndex = 14;
            p3OkBtn.Text = "Ok";
            p3OkBtn.UseVisualStyleBackColor = false;
            p3OkBtn.Click += btnOk_Click;
            // 
            // cbVectors
            // 
            cbVectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbVectors.FormattingEnabled = true;
            cbVectors.Location = new System.Drawing.Point(12, 99);
            cbVectors.Name = "cbVectors";
            cbVectors.Size = new System.Drawing.Size(204, 23);
            cbVectors.TabIndex = 12;
            cbVectors.SelectedIndexChanged += cbVectors_SelectedIndexChanged;
            // 
            // label12
            // 
            label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label12.Location = new System.Drawing.Point(12, 73);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(204, 23);
            label12.TabIndex = 10;
            label12.Text = "Attack Vector";
            // 
            // page5
            // 
            page5.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            page5.Anchor = System.Windows.Forms.AnchorStyles.None;
            page5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            page5.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page5.Controls.Add(button1);
            page5.Controls.Add(p4CancelButton);
            page5.Controls.Add(p4OkBtn);
            page5.Controls.Add(cbModifier);
            page5.Controls.Add(label1);
            page5.Dock = System.Windows.Forms.DockStyle.Fill;
            page5.ForeColor = System.Drawing.Color.WhiteSmoke;
            page5.Location = new System.Drawing.Point(0, 0);
            page5.Name = "page5";
            page5.Size = new System.Drawing.Size(228, 300);
            page5.TabIndex = 4;
            page5.Title = "Modifiers";
            // 
            // button1
            // 
            button1.BackColor = System.Drawing.Color.FromArgb(83, 70, 170);
            button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button1.Location = new System.Drawing.Point(35, 265);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(153, 23);
            button1.TabIndex = 24;
            button1.Text = "Choose None";
            button1.UseVisualStyleBackColor = false;
            button1.Click += btnChooseNone_Click;
            // 
            // p4CancelButton
            // 
            p4CancelButton.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            p4CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            p4CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p4CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p4CancelButton.Location = new System.Drawing.Point(35, 227);
            p4CancelButton.Name = "p4CancelButton";
            p4CancelButton.Size = new System.Drawing.Size(153, 23);
            p4CancelButton.TabIndex = 23;
            p4CancelButton.Text = "Cancel";
            p4CancelButton.UseVisualStyleBackColor = false;
            p4CancelButton.Click += btnCancel_Click;
            // 
            // p4OkBtn
            // 
            p4OkBtn.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            p4OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            p4OkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            p4OkBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            p4OkBtn.Location = new System.Drawing.Point(35, 198);
            p4OkBtn.Name = "p4OkBtn";
            p4OkBtn.Size = new System.Drawing.Size(153, 23);
            p4OkBtn.TabIndex = 22;
            p4OkBtn.Text = "Ok";
            p4OkBtn.UseVisualStyleBackColor = false;
            p4OkBtn.Click += btnOk_Click;
            // 
            // cbModifier
            // 
            cbModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbModifier.FormattingEnabled = true;
            cbModifier.Location = new System.Drawing.Point(12, 99);
            cbModifier.Name = "cbModifier";
            cbModifier.Size = new System.Drawing.Size(204, 23);
            cbModifier.TabIndex = 21;
            cbModifier.SelectedIndexChanged += cbModifier_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(12, 73);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(204, 23);
            label1.TabIndex = 20;
            label1.Text = "Modifier";
            // 
            // GenericSelector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(228, 300);
            Controls.Add(formPages1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.Azure;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "GenericSelector";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "GenericSelector";
            Load += OnLoad;
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            page2.ResumeLayout(false);
            page2.PerformLayout();
            page3.ResumeLayout(false);
            page4.ResumeLayout(false);
            page5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Mids_Reborn.Controls.FormPages formPages1;
        private Mids_Reborn.Controls.Page page1;
        private System.Windows.Forms.Button p0NoneBtn;
        private System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button p0CancelBtn;
        private System.Windows.Forms.Button p0OkBtn;
        private System.Windows.Forms.ComboBox cbPower;
        private System.Windows.Forms.ComboBox cbPowerset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Mids_Reborn.Controls.Page page2;
        private System.Windows.Forms.ComboBox cbModifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button p1CancelBtn;
        private System.Windows.Forms.Button p1OkBtn;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox cbArchetypes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Mids_Reborn.Controls.Page page3;
        private System.Windows.Forms.ComboBox cbP2Group;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button p2CancelBtn;
        private System.Windows.Forms.Button p2OkBtn;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ComboBox cbP2Powerset;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private Mids_Reborn.Controls.Page page4;
        private System.Windows.Forms.Button p3NoneBtn;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button p3CancelBtn;
        private System.Windows.Forms.Button p3OkBtn;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.ComboBox cbVectors;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button p1NoneBtn;
        private System.Windows.Forms.Button p2NoneBtn;
        private System.Windows.Forms.CheckBox checkPlayableAtOnly;
        private Mids_Reborn.Controls.Page page5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button p4CancelButton;
        private System.Windows.Forms.Button p4OkBtn;
    }
}
