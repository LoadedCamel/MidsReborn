using System.Drawing;
using System.Windows.Forms;

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
            borderPanel1 = new BorderPanel();
            p0NoneBtn = new Button();
            cbGroup = new ComboBox();
            label4 = new Label();
            p0CancelBtn = new Button();
            p0OkBtn = new Button();
            cbPower = new ComboBox();
            cbPowerset = new ComboBox();
            label3 = new Label();
            label2 = new Label();
            page2 = new Mids_Reborn.Controls.Page();
            borderPanel2 = new BorderPanel();
            checkPlayableAtOnly = new CheckBox();
            p1NoneBtn = new Button();
            p1CancelBtn = new Button();
            p1OkBtn = new Button();
            cbArchetypes = new ComboBox();
            label6 = new Label();
            page3 = new Mids_Reborn.Controls.Page();
            borderPanel3 = new BorderPanel();
            p2NoneBtn = new Button();
            cbP2Group = new ComboBox();
            label7 = new Label();
            p2CancelBtn = new Button();
            p2OkBtn = new Button();
            cbP2Powerset = new ComboBox();
            label9 = new Label();
            page4 = new Mids_Reborn.Controls.Page();
            borderPanel4 = new BorderPanel();
            p3NoneBtn = new Button();
            p3CancelBtn = new Button();
            p3OkBtn = new Button();
            cbVectors = new ComboBox();
            label12 = new Label();
            page5 = new Mids_Reborn.Controls.Page();
            borderPanel5 = new BorderPanel();
            p4NoneBtn = new Button();
            p4CancelButton = new Button();
            p4OkBtn = new Button();
            cbModifier = new ComboBox();
            label1 = new Label();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            borderPanel1.SuspendLayout();
            page2.SuspendLayout();
            borderPanel2.SuspendLayout();
            page3.SuspendLayout();
            borderPanel3.SuspendLayout();
            page4.SuspendLayout();
            borderPanel4.SuspendLayout();
            page5.SuspendLayout();
            borderPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // formPages1
            // 
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Controls.Add(page3);
            formPages1.Controls.Add(page4);
            formPages1.Controls.Add(page5);
            formPages1.Location = new Point(0, 0);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.Pages.Add(page3);
            formPages1.Pages.Add(page4);
            formPages1.Pages.Add(page5);
            formPages1.SelectedIndex = 1;
            formPages1.Size = new Size(228, 300);
            formPages1.TabIndex = 11;
            // 
            // page1
            // 
            page1.AccessibleRole = AccessibleRole.None;
            page1.Anchor = AnchorStyles.None;
            page1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page1.BackColor = Color.FromArgb(44, 47, 51);
            page1.Controls.Add(borderPanel1);
            page1.Dock = DockStyle.Fill;
            page1.ForeColor = Color.WhiteSmoke;
            page1.Location = new Point(0, 0);
            page1.Name = "page1";
            page1.Size = new Size(228, 300);
            page1.TabIndex = 0;
            page1.Title = "Powers";
            // 
            // borderPanel1
            // 
            borderPanel1.BackColor = Color.FromArgb(44, 47, 51);
            borderPanel1.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel1.Border.Style = ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(p0NoneBtn);
            borderPanel1.Controls.Add(cbGroup);
            borderPanel1.Controls.Add(label4);
            borderPanel1.Controls.Add(p0CancelBtn);
            borderPanel1.Controls.Add(p0OkBtn);
            borderPanel1.Controls.Add(cbPower);
            borderPanel1.Controls.Add(cbPowerset);
            borderPanel1.Controls.Add(label3);
            borderPanel1.Controls.Add(label2);
            borderPanel1.ForeColor = Color.Azure;
            borderPanel1.Location = new Point(0, 0);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new Size(228, 300);
            borderPanel1.TabIndex = 0;
            // 
            // p0NoneBtn
            // 
            p0NoneBtn.BackColor = Color.FromArgb(83, 70, 170);
            p0NoneBtn.DialogResult = DialogResult.OK;
            p0NoneBtn.FlatStyle = FlatStyle.Popup;
            p0NoneBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            p0NoneBtn.Location = new Point(35, 265);
            p0NoneBtn.Name = "p0NoneBtn";
            p0NoneBtn.Size = new Size(153, 23);
            p0NoneBtn.TabIndex = 19;
            p0NoneBtn.Text = "Choose None";
            p0NoneBtn.UseVisualStyleBackColor = false;
            p0NoneBtn.Click += btnChooseNone_Click;
            // 
            // cbGroup
            // 
            cbGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGroup.FormattingEnabled = true;
            cbGroup.Location = new Point(12, 37);
            cbGroup.Name = "cbGroup";
            cbGroup.Size = new Size(204, 23);
            cbGroup.TabIndex = 17;
            cbGroup.SelectedIndexChanged += CbGroupOnSelectedIndexChanged;
            // 
            // label4
            // 
            label4.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(12, 11);
            label4.Name = "label4";
            label4.Size = new Size(204, 23);
            label4.TabIndex = 16;
            label4.Text = "Group";
            // 
            // p0CancelBtn
            // 
            p0CancelBtn.BackColor = Color.FromArgb(88, 40, 18);
            p0CancelBtn.DialogResult = DialogResult.Cancel;
            p0CancelBtn.FlatStyle = FlatStyle.Popup;
            p0CancelBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p0CancelBtn.Location = new Point(35, 227);
            p0CancelBtn.Name = "p0CancelBtn";
            p0CancelBtn.Size = new Size(153, 23);
            p0CancelBtn.TabIndex = 15;
            p0CancelBtn.Text = "Cancel";
            p0CancelBtn.UseVisualStyleBackColor = false;
            p0CancelBtn.Click += btnCancel_Click;
            // 
            // p0OkBtn
            // 
            p0OkBtn.BackColor = Color.FromArgb(64, 78, 237);
            p0OkBtn.DialogResult = DialogResult.OK;
            p0OkBtn.FlatStyle = FlatStyle.Popup;
            p0OkBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p0OkBtn.Location = new Point(35, 198);
            p0OkBtn.Name = "p0OkBtn";
            p0OkBtn.Size = new Size(153, 23);
            p0OkBtn.TabIndex = 14;
            p0OkBtn.Text = "Ok";
            p0OkBtn.UseVisualStyleBackColor = false;
            p0OkBtn.Click += btnOk_Click;
            // 
            // cbPower
            // 
            cbPower.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPower.FormattingEnabled = true;
            cbPower.Location = new Point(12, 160);
            cbPower.Name = "cbPower";
            cbPower.Size = new Size(204, 23);
            cbPower.TabIndex = 13;
            cbPower.SelectedIndexChanged += CbPowerOnSelectedIndexChanged;
            // 
            // cbPowerset
            // 
            cbPowerset.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPowerset.FormattingEnabled = true;
            cbPowerset.Location = new Point(12, 99);
            cbPowerset.Name = "cbPowerset";
            cbPowerset.Size = new Size(204, 23);
            cbPowerset.TabIndex = 12;
            cbPowerset.SelectedIndexChanged += CbPowersetOnSelectedIndexChanged;
            // 
            // label3
            // 
            label3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(12, 134);
            label3.Name = "label3";
            label3.Size = new Size(204, 23);
            label3.TabIndex = 11;
            label3.Text = "Power";
            // 
            // label2
            // 
            label2.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(12, 73);
            label2.Name = "label2";
            label2.Size = new Size(204, 23);
            label2.TabIndex = 10;
            label2.Text = "Powerset";
            // 
            // page2
            // 
            page2.AccessibleRole = AccessibleRole.None;
            page2.Anchor = AnchorStyles.None;
            page2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page2.BackColor = Color.FromArgb(44, 47, 51);
            page2.Controls.Add(borderPanel2);
            page2.Dock = DockStyle.Fill;
            page2.ForeColor = Color.WhiteSmoke;
            page2.Location = new Point(0, 0);
            page2.Name = "page2";
            page2.Size = new Size(228, 300);
            page2.TabIndex = 1;
            page2.Title = "Archetypes";
            // 
            // borderPanel2
            // 
            borderPanel1.BackColor = Color.FromArgb(44, 47, 51);
            borderPanel2.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel2.Border.Style = ButtonBorderStyle.Solid;
            borderPanel2.Border.Thickness = 1;
            borderPanel2.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel2.Controls.Add(checkPlayableAtOnly);
            borderPanel2.Controls.Add(p1NoneBtn);
            borderPanel2.Controls.Add(p1CancelBtn);
            borderPanel2.Controls.Add(p1OkBtn);
            borderPanel2.Controls.Add(cbArchetypes);
            borderPanel2.Controls.Add(label6);
            borderPanel1.ForeColor = Color.Azure;
            borderPanel2.Location = new Point(0, 0);
            borderPanel2.Name = "borderPanel2";
            borderPanel2.Size = new Size(228, 300);
            borderPanel2.TabIndex = 0;
            // 
            // checkPlayableAtOnly
            // 
            checkPlayableAtOnly.AutoSize = true;
            checkPlayableAtOnly.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            checkPlayableAtOnly.Location = new Point(48, 131);
            checkPlayableAtOnly.Name = "checkPlayableAtOnly";
            checkPlayableAtOnly.Size = new Size(123, 19);
            checkPlayableAtOnly.TabIndex = 20;
            checkPlayableAtOnly.Text = "Player ATs only";
            checkPlayableAtOnly.UseVisualStyleBackColor = true;
            checkPlayableAtOnly.CheckedChanged += checkPlayableAtOnly_CheckedChanged;
            // 
            // p1NoneBtn
            // 
            p1NoneBtn.BackColor = Color.FromArgb(83, 70, 170);
            p1NoneBtn.DialogResult = DialogResult.OK;
            p1NoneBtn.FlatStyle = FlatStyle.Popup;
            p1NoneBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            p1NoneBtn.Location = new Point(35, 265);
            p1NoneBtn.Name = "p1NoneBtn";
            p1NoneBtn.Size = new Size(153, 23);
            p1NoneBtn.TabIndex = 19;
            p1NoneBtn.Text = "Choose None";
            p1NoneBtn.UseVisualStyleBackColor = false;
            p1NoneBtn.Click += btnChooseNone_Click;
            // 
            // p1CancelBtn
            // 
            p1CancelBtn.BackColor = Color.FromArgb(88, 40, 18);
            p1CancelBtn.DialogResult = DialogResult.Cancel;
            p1CancelBtn.FlatStyle = FlatStyle.Popup;
            p1CancelBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p1CancelBtn.Location = new Point(35, 227);
            p1CancelBtn.Name = "p1CancelBtn";
            p1CancelBtn.Size = new Size(153, 23);
            p1CancelBtn.TabIndex = 15;
            p1CancelBtn.Text = "Cancel";
            p1CancelBtn.UseVisualStyleBackColor = false;
            p1CancelBtn.Click += btnCancel_Click;
            // 
            // p1OkBtn
            // 
            p1OkBtn.BackColor = Color.FromArgb(64, 78, 237);
            p1OkBtn.DialogResult = DialogResult.OK;
            p1OkBtn.FlatStyle = FlatStyle.Popup;
            p1OkBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p1OkBtn.Location = new Point(35, 198);
            p1OkBtn.Name = "p1OkBtn";
            p1OkBtn.Size = new Size(153, 23);
            p1OkBtn.TabIndex = 14;
            p1OkBtn.Text = "Ok";
            p1OkBtn.UseVisualStyleBackColor = false;
            p1OkBtn.Click += btnOk_Click;
            // 
            // cbArchetypes
            // 
            cbArchetypes.DropDownStyle = ComboBoxStyle.DropDownList;
            cbArchetypes.FormattingEnabled = true;
            cbArchetypes.Location = new Point(12, 62);
            cbArchetypes.Name = "cbArchetypes";
            cbArchetypes.Size = new Size(204, 23);
            cbArchetypes.TabIndex = 12;
            cbArchetypes.SelectedIndexChanged += cbArchetypes_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(12, 36);
            label6.Name = "label6";
            label6.Size = new Size(204, 23);
            label6.TabIndex = 10;
            label6.Text = "Archetype";
            // 
            // page3
            // 
            page3.AccessibleRole = AccessibleRole.None;
            page3.Anchor = AnchorStyles.None;
            page3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page3.BackColor = Color.FromArgb(44, 47, 51);
            page3.Controls.Add(borderPanel3);
            page3.Dock = DockStyle.Fill;
            page3.ForeColor = Color.WhiteSmoke;
            page3.Location = new Point(0, 0);
            page3.Name = "page3";
            page3.Size = new Size(228, 300);
            page3.TabIndex = 2;
            page3.Title = "Power Groups";
            // 
            // borderPanel3
            // 
            borderPanel1.BackColor = Color.FromArgb(44, 47, 51);
            borderPanel3.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel3.Border.Style = ButtonBorderStyle.Solid;
            borderPanel3.Border.Thickness = 1;
            borderPanel3.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel3.Controls.Add(p2NoneBtn);
            borderPanel3.Controls.Add(cbP2Group);
            borderPanel3.Controls.Add(label7);
            borderPanel3.Controls.Add(p2CancelBtn);
            borderPanel3.Controls.Add(p2OkBtn);
            borderPanel3.Controls.Add(cbP2Powerset);
            borderPanel3.Controls.Add(label9);
            borderPanel1.ForeColor = Color.Azure;
            borderPanel3.Location = new Point(0, 0);
            borderPanel3.Name = "borderPanel3";
            borderPanel3.Size = new Size(228, 300);
            borderPanel3.TabIndex = 0;
            // 
            // p2NoneBtn
            // 
            p2NoneBtn.BackColor = Color.FromArgb(83, 70, 170);
            p2NoneBtn.DialogResult = DialogResult.OK;
            p2NoneBtn.FlatStyle = FlatStyle.Popup;
            p2NoneBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            p2NoneBtn.Location = new Point(35, 265);
            p2NoneBtn.Name = "p2NoneBtn";
            p2NoneBtn.Size = new Size(153, 23);
            p2NoneBtn.TabIndex = 19;
            p2NoneBtn.Text = "Choose None";
            p2NoneBtn.UseVisualStyleBackColor = false;
            p2NoneBtn.Click += btnChooseNone_Click;
            // 
            // cbP2Group
            // 
            cbP2Group.DropDownStyle = ComboBoxStyle.DropDownList;
            cbP2Group.FormattingEnabled = true;
            cbP2Group.Location = new Point(12, 46);
            cbP2Group.Name = "cbP2Group";
            cbP2Group.Size = new Size(204, 23);
            cbP2Group.TabIndex = 17;
            cbP2Group.SelectedIndexChanged += cbP2Group_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label7.Location = new Point(12, 20);
            label7.Name = "label7";
            label7.Size = new Size(204, 23);
            label7.TabIndex = 16;
            label7.Text = "Group";
            // 
            // p2CancelBtn
            // 
            p2CancelBtn.BackColor = Color.FromArgb(88, 40, 18);
            p2CancelBtn.DialogResult = DialogResult.Cancel;
            p2CancelBtn.FlatStyle = FlatStyle.Popup;
            p2CancelBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p2CancelBtn.Location = new Point(35, 227);
            p2CancelBtn.Name = "p2CancelBtn";
            p2CancelBtn.Size = new Size(153, 23);
            p2CancelBtn.TabIndex = 15;
            p2CancelBtn.Text = "Cancel";
            p2CancelBtn.UseVisualStyleBackColor = false;
            p2CancelBtn.Click += btnCancel_Click;
            // 
            // p2OkBtn
            // 
            p2OkBtn.BackColor = Color.FromArgb(64, 78, 237);
            p2OkBtn.DialogResult = DialogResult.OK;
            p2OkBtn.FlatStyle = FlatStyle.Popup;
            p2OkBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p2OkBtn.Location = new Point(35, 198);
            p2OkBtn.Name = "p2OkBtn";
            p2OkBtn.Size = new Size(153, 23);
            p2OkBtn.TabIndex = 14;
            p2OkBtn.Text = "Ok";
            p2OkBtn.UseVisualStyleBackColor = false;
            p2OkBtn.Click += btnOk_Click;
            // 
            // cbP2Powerset
            // 
            cbP2Powerset.DropDownStyle = ComboBoxStyle.DropDownList;
            cbP2Powerset.FormattingEnabled = true;
            cbP2Powerset.Location = new Point(12, 138);
            cbP2Powerset.Name = "cbP2Powerset";
            cbP2Powerset.Size = new Size(204, 23);
            cbP2Powerset.TabIndex = 12;
            cbP2Powerset.SelectedIndexChanged += cbP2Powerset_SelectedIndexChanged;
            // 
            // label9
            // 
            label9.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label9.Location = new Point(12, 112);
            label9.Name = "label9";
            label9.Size = new Size(204, 23);
            label9.TabIndex = 10;
            label9.Text = "Powerset";
            // 
            // page4
            // 
            page4.AccessibleRole = AccessibleRole.None;
            page4.Anchor = AnchorStyles.None;
            page4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page4.BackColor = Color.FromArgb(44, 47, 51);
            page4.Controls.Add(borderPanel4);
            page4.Dock = DockStyle.Fill;
            page4.ForeColor = Color.WhiteSmoke;
            page4.Location = new Point(0, 0);
            page4.Name = "page4";
            page4.Size = new Size(228, 300);
            page4.TabIndex = 3;
            page4.Title = "Attack Vectors";
            // 
            // borderPanel4
            // 
            borderPanel1.BackColor = Color.FromArgb(44, 47, 51);
            borderPanel4.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel4.Border.Style = ButtonBorderStyle.Solid;
            borderPanel4.Border.Thickness = 1;
            borderPanel4.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel4.Controls.Add(p3NoneBtn);
            borderPanel4.Controls.Add(p3CancelBtn);
            borderPanel4.Controls.Add(p3OkBtn);
            borderPanel4.Controls.Add(cbVectors);
            borderPanel4.Controls.Add(label12);
            borderPanel1.ForeColor = Color.Azure;
            borderPanel4.Location = new Point(0, 0);
            borderPanel4.Name = "borderPanel4";
            borderPanel4.Size = new Size(228, 300);
            borderPanel4.TabIndex = 0;
            // 
            // p3NoneBtn
            // 
            p3NoneBtn.BackColor = Color.FromArgb(83, 70, 170);
            p3NoneBtn.DialogResult = DialogResult.OK;
            p3NoneBtn.FlatStyle = FlatStyle.Popup;
            p3NoneBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            p3NoneBtn.Location = new Point(35, 265);
            p3NoneBtn.Name = "p3NoneBtn";
            p3NoneBtn.Size = new Size(153, 23);
            p3NoneBtn.TabIndex = 18;
            p3NoneBtn.Text = "Choose None";
            p3NoneBtn.UseVisualStyleBackColor = false;
            p3NoneBtn.Click += btnChooseNone_Click;
            // 
            // p3CancelBtn
            // 
            p3CancelBtn.BackColor = Color.FromArgb(88, 40, 18);
            p3CancelBtn.DialogResult = DialogResult.Cancel;
            p3CancelBtn.FlatStyle = FlatStyle.Popup;
            p3CancelBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p3CancelBtn.Location = new Point(35, 227);
            p3CancelBtn.Name = "p3CancelBtn";
            p3CancelBtn.Size = new Size(153, 23);
            p3CancelBtn.TabIndex = 15;
            p3CancelBtn.Text = "Cancel";
            p3CancelBtn.UseVisualStyleBackColor = false;
            p3CancelBtn.Click += btnCancel_Click;
            // 
            // p3OkBtn
            // 
            p3OkBtn.BackColor = Color.FromArgb(64, 78, 237);
            p3OkBtn.DialogResult = DialogResult.OK;
            p3OkBtn.FlatStyle = FlatStyle.Popup;
            p3OkBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p3OkBtn.Location = new Point(35, 198);
            p3OkBtn.Name = "p3OkBtn";
            p3OkBtn.Size = new Size(153, 23);
            p3OkBtn.TabIndex = 14;
            p3OkBtn.Text = "Ok";
            p3OkBtn.UseVisualStyleBackColor = false;
            p3OkBtn.Click += btnOk_Click;
            // 
            // cbVectors
            // 
            cbVectors.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVectors.FormattingEnabled = true;
            cbVectors.Location = new Point(12, 99);
            cbVectors.Name = "cbVectors";
            cbVectors.Size = new Size(204, 23);
            cbVectors.TabIndex = 12;
            cbVectors.SelectedIndexChanged += cbVectors_SelectedIndexChanged;
            // 
            // label12
            // 
            label12.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label12.Location = new Point(12, 73);
            label12.Name = "label12";
            label12.Size = new Size(204, 23);
            label12.TabIndex = 10;
            label12.Text = "Attack Vector";
            // 
            // page5
            // 
            page5.AccessibleRole = AccessibleRole.None;
            page5.Anchor = AnchorStyles.None;
            page5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page5.BackColor = Color.FromArgb(44, 47, 51);
            page5.Controls.Add(borderPanel5);
            page5.Dock = DockStyle.Fill;
            page5.ForeColor = Color.WhiteSmoke;
            page5.Location = new Point(0, 0);
            page5.Name = "page5";
            page5.Size = new Size(228, 300);
            page5.TabIndex = 4;
            page5.Title = "Modifiers";
            // 
            // borderPanel5
            // 
            borderPanel1.BackColor = Color.FromArgb(44, 47, 51);
            borderPanel5.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel5.Border.Style = ButtonBorderStyle.Solid;
            borderPanel5.Border.Thickness = 1;
            borderPanel5.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel5.Controls.Add(p4NoneBtn);
            borderPanel5.Controls.Add(p4CancelButton);
            borderPanel5.Controls.Add(p4OkBtn);
            borderPanel5.Controls.Add(cbModifier);
            borderPanel5.Controls.Add(label1);
            borderPanel1.ForeColor = Color.Azure;
            borderPanel5.Location = new Point(0, 0);
            borderPanel5.Name = "borderPanel5";
            borderPanel5.Size = new Size(228, 300);
            borderPanel5.TabIndex = 0;
            // 
            // p4NoneBtn
            // 
            p4NoneBtn.BackColor = Color.FromArgb(83, 70, 170);
            p4NoneBtn.DialogResult = DialogResult.OK;
            p4NoneBtn.FlatStyle = FlatStyle.Popup;
            p4NoneBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            p4NoneBtn.Location = new Point(35, 265);
            p4NoneBtn.Name = "p4NoneBtn";
            p4NoneBtn.Size = new Size(153, 23);
            p4NoneBtn.TabIndex = 24;
            p4NoneBtn.Text = "Choose None";
            p4NoneBtn.UseVisualStyleBackColor = false;
            p4NoneBtn.Click += btnChooseNone_Click;
            // 
            // p4CancelButton
            // 
            p4CancelButton.BackColor = Color.FromArgb(88, 40, 18);
            p4CancelButton.DialogResult = DialogResult.Cancel;
            p4CancelButton.FlatStyle = FlatStyle.Popup;
            p4CancelButton.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p4CancelButton.Location = new Point(35, 227);
            p4CancelButton.Name = "p4CancelButton";
            p4CancelButton.Size = new Size(153, 23);
            p4CancelButton.TabIndex = 23;
            p4CancelButton.Text = "Cancel";
            p4CancelButton.UseVisualStyleBackColor = false;
            p4CancelButton.Click += btnCancel_Click;
            // 
            // p4OkBtn
            // 
            p4OkBtn.BackColor = Color.FromArgb(64, 78, 237);
            p4OkBtn.DialogResult = DialogResult.OK;
            p4OkBtn.FlatStyle = FlatStyle.Popup;
            p4OkBtn.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            p4OkBtn.Location = new Point(35, 198);
            p4OkBtn.Name = "p4OkBtn";
            p4OkBtn.Size = new Size(153, 23);
            p4OkBtn.TabIndex = 22;
            p4OkBtn.Text = "Ok";
            p4OkBtn.UseVisualStyleBackColor = false;
            p4OkBtn.Click += btnOk_Click;
            // 
            // cbModifier
            // 
            cbModifier.DropDownStyle = ComboBoxStyle.DropDownList;
            cbModifier.FormattingEnabled = true;
            cbModifier.Location = new Point(12, 99);
            cbModifier.Name = "cbModifier";
            cbModifier.Size = new Size(204, 23);
            cbModifier.TabIndex = 21;
            cbModifier.SelectedIndexChanged += cbModifier_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 73);
            label1.Name = "label1";
            label1.Size = new Size(204, 23);
            label1.TabIndex = 20;
            label1.Text = "Modifier";
            // 
            // GenericSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(44, 47, 51);
            ClientSize = new Size(228, 300);
            Controls.Add(formPages1);
            DoubleBuffered = true;
            ForeColor = Color.Azure;
            FormBorderStyle = FormBorderStyle.None;
            Name = "GenericSelector";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "GenericSelector";
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            borderPanel1.ResumeLayout(false);
            page2.ResumeLayout(false);
            borderPanel2.ResumeLayout(false);
            borderPanel2.PerformLayout();
            page3.ResumeLayout(false);
            borderPanel3.ResumeLayout(false);
            page4.ResumeLayout(false);
            borderPanel4.ResumeLayout(false);
            page5.ResumeLayout(false);
            borderPanel5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Mids_Reborn.Controls.FormPages formPages1;
        private Mids_Reborn.Controls.Page page1;
        private Button p0NoneBtn;
        private ComboBox cbGroup;
        private Label label4;
        private Button p0CancelBtn;
        private Button p0OkBtn;
        private ComboBox cbPower;
        private ComboBox cbPowerset;
        private Label label3;
        private Label label2;
        private Mids_Reborn.Controls.Page page2;
        private ComboBox cbModifier;
        private Label label1;
        private Button p1CancelBtn;
        private Button p1OkBtn;
        private ComboBox comboBox2;
        private ComboBox cbArchetypes;
        private Label label5;
        private Label label6;
        private Mids_Reborn.Controls.Page page3;
        private ComboBox cbP2Group;
        private Label label7;
        private Button p2CancelBtn;
        private Button p2OkBtn;
        private ComboBox comboBox5;
        private ComboBox cbP2Powerset;
        private Label label8;
        private Label label9;
        private Mids_Reborn.Controls.Page page4;
        private Button p3NoneBtn;
        private ComboBox comboBox7;
        private Label label10;
        private Button p3CancelBtn;
        private Button p3OkBtn;
        private ComboBox comboBox8;
        private ComboBox cbVectors;
        private Label label11;
        private Label label12;
        private Button p1NoneBtn;
        private Button p2NoneBtn;
        private CheckBox checkPlayableAtOnly;
        private Mids_Reborn.Controls.Page page5;
        private Button p4NoneBtn;
        private Button p4CancelButton;
        private Button p4OkBtn;
        private BorderPanel borderPanel1;
        private BorderPanel borderPanel2;
        private BorderPanel borderPanel3;
        private BorderPanel borderPanel4;
        private BorderPanel borderPanel5;
    }
}
