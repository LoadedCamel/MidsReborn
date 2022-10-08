using System.Windows.Controls;
using System.Windows.Forms;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    sealed partial class frmEffectConditionals
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Equal To");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Greater Than");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Less Than");
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.removeConditional = new System.Windows.Forms.Button();
            this.addConditional = new System.Windows.Forms.Button();
            this.lvConditionalOp = new ctlListViewColored();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.lvActiveConditionals = new ctlListViewColored();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.lvConditionalBool = new ctlListViewColored();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.lvConditionalType = new ctlListViewColored();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lvSubConditional = new ctlListViewColored();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            this.groupBox2.Controls.Add(this.lvConditionalOp);
            this.groupBox2.Controls.Add(this.removeConditional);
            this.groupBox2.Controls.Add(this.addConditional);
            this.groupBox2.Controls.Add(this.lvActiveConditionals);
            this.groupBox2.Controls.Add(this.lvConditionalBool);
            this.groupBox2.Controls.Add(this.lvConditionalType);
            this.groupBox2.Controls.Add(this.lvSubConditional);
            this.groupBox2.Location = new System.Drawing.Point(13, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(1284, 483);
            this.groupBox2.TabIndex = 161;
            this.groupBox2.TabStop = false;
            // 
            // lvConditionalOp
            // 
            this.lvConditionalOp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7});
            this.lvConditionalOp.FullRowSelect = true;
            this.lvConditionalOp.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConditionalOp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.lvConditionalOp.Location = new System.Drawing.Point(537, 16);
            this.lvConditionalOp.LostFocusItem = -1;
            this.lvConditionalOp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvConditionalOp.MultiSelect = false;
            this.lvConditionalOp.Name = "lvConditionalOp";
            this.lvConditionalOp.OwnerDraw = true;
            this.lvConditionalOp.Size = new System.Drawing.Size(112, 95);
            this.lvConditionalOp.TabIndex = 166;
            this.lvConditionalOp.UseCompatibleStateImageBehavior = false;
            this.lvConditionalOp.View = System.Windows.Forms.View.Details;
            this.lvConditionalOp.Visible = false;
            this.lvConditionalOp.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvConditionalOp.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvConditionalOp.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Stack is?";
            this.columnHeader7.Width = 81;
            // 
            // removeConditional
            // 
            this.removeConditional.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(40)))), ((int)(((byte)(18)))));
            this.removeConditional.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.removeConditional.Location = new System.Drawing.Point(657, 237);
            this.removeConditional.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.removeConditional.Name = "removeConditional";
            this.removeConditional.Size = new System.Drawing.Size(181, 33);
            this.removeConditional.TabIndex = 165;
            this.removeConditional.Text = "<= Remove Condition";
            this.removeConditional.UseVisualStyleBackColor = false;
            this.removeConditional.Click += new System.EventHandler(this.removeConditional_Click);
            // 
            // addConditional
            // 
            this.addConditional.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(78)))), ((int)(((byte)(237)))));
            this.addConditional.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.addConditional.Location = new System.Drawing.Point(657, 196);
            this.addConditional.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.addConditional.Name = "addConditional";
            this.addConditional.Size = new System.Drawing.Size(181, 33);
            this.addConditional.TabIndex = 164;
            this.addConditional.Text = "Add Condition =>";
            this.addConditional.UseVisualStyleBackColor = false;
            this.addConditional.Click += new System.EventHandler(this.addConditional_Click);
            // 
            // lvActiveConditionals
            // 
            this.lvActiveConditionals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader8,
            this.columnHeader6});
            this.lvActiveConditionals.FullRowSelect = true;
            this.lvActiveConditionals.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvActiveConditionals.Location = new System.Drawing.Point(845, 16);
            this.lvActiveConditionals.LostFocusItem = -1;
            this.lvActiveConditionals.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvActiveConditionals.MultiSelect = false;
            this.lvActiveConditionals.Name = "lvActiveConditionals";
            this.lvActiveConditionals.Size = new System.Drawing.Size(432, 460);
            this.lvActiveConditionals.TabIndex = 163;
            this.lvActiveConditionals.UseCompatibleStateImageBehavior = false;
            this.lvActiveConditionals.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Active Conditional";
            this.columnHeader5.Width = 138;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "";
            this.columnHeader8.Width = 27;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            this.columnHeader6.Width = 46;
            // 
            // lvConditionalBool
            // 
            this.lvConditionalBool.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.lvConditionalBool.FullRowSelect = true;
            this.lvConditionalBool.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConditionalBool.Location = new System.Drawing.Point(537, 117);
            this.lvConditionalBool.LostFocusItem = -1;
            this.lvConditionalBool.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvConditionalBool.MultiSelect = false;
            this.lvConditionalBool.Name = "lvConditionalBool";
            this.lvConditionalBool.OwnerDraw = true;
            this.lvConditionalBool.Size = new System.Drawing.Size(112, 359);
            this.lvConditionalBool.TabIndex = 162;
            this.lvConditionalBool.UseCompatibleStateImageBehavior = false;
            this.lvConditionalBool.View = System.Windows.Forms.View.Details;
            this.lvConditionalBool.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvConditionalBool.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvConditionalBool.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Conditional Boolean";
            this.columnHeader4.Width = 81;
            // 
            // lvConditionalType
            // 
            this.lvConditionalType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.lvConditionalType.FullRowSelect = true;
            this.lvConditionalType.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvConditionalType.Location = new System.Drawing.Point(7, 16);
            this.lvConditionalType.LostFocusItem = -1;
            this.lvConditionalType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvConditionalType.MultiSelect = false;
            this.lvConditionalType.Name = "lvConditionalType";
            this.lvConditionalType.OwnerDraw = true;
            this.lvConditionalType.Size = new System.Drawing.Size(135, 460);
            this.lvConditionalType.TabIndex = 161;
            this.lvConditionalType.UseCompatibleStateImageBehavior = false;
            this.lvConditionalType.View = System.Windows.Forms.View.Details;
            this.lvConditionalType.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvConditionalType.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvConditionalType.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvConditionalType_SelectionChanged);
            this.lvConditionalType.Leave += new System.EventHandler(this.ListView_Leave);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Effect Conditional";
            this.columnHeader3.Width = 102;
            // 
            // lvSubConditional
            // 
            this.lvSubConditional.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvSubConditional.FullRowSelect = true;
            this.lvSubConditional.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSubConditional.Location = new System.Drawing.Point(149, 16);
            this.lvSubConditional.LostFocusItem = -1;
            this.lvSubConditional.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvSubConditional.MultiSelect = false;
            this.lvSubConditional.Name = "lvSubConditional";
            this.lvSubConditional.OwnerDraw = true;
            this.lvSubConditional.Size = new System.Drawing.Size(380, 460);
            this.lvSubConditional.TabIndex = 160;
            this.lvSubConditional.UseCompatibleStateImageBehavior = false;
            this.lvSubConditional.View = System.Windows.Forms.View.Details;
            this.lvSubConditional.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListView_DrawColumnHeader);
            this.lvSubConditional.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.lvSubConditional.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvSubConditional_SelectionChanged);
            this.lvSubConditional.Leave += new System.EventHandler(this.ListView_Leave);
            this.lvSubConditional.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvSubConditional_MouseClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sub-Conditional Attribute";
            this.columnHeader2.Width = 202;
            // 
            // btnOkay
            // 
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(78)))), ((int)(((byte)(237)))));
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnOkay.Location = new System.Drawing.Point(1108, 490);
            this.btnOkay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(88, 43);
            this.btnOkay.TabIndex = 162;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(40)))), ((int)(((byte)(18)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.Location = new System.Drawing.Point(1203, 490);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 43);
            this.btnCancel.TabIndex = 163;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnOkay);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1310, 542);
            this.panel1.TabIndex = 164;
            // 
            // frmEffectConditionals
            // 
            this.AcceptButton = this.btnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(170)))), ((int)(((byte)(181)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1310, 542);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.Azure;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmEffectConditionals";
            this.Text = "Effect Conditions";
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private ctlListViewColored lvConditionalOp;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Button removeConditional;
        private System.Windows.Forms.Button addConditional;
        private ctlListViewColored lvActiveConditionals;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private ctlListViewColored lvConditionalBool;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private ctlListViewColored lvConditionalType;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private ctlListViewColored lvSubConditional;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
    }
}