using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
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
            groupBox2 = new System.Windows.Forms.GroupBox();
            panelLinkType = new System.Windows.Forms.Panel();
            rbLinkTypeOr = new System.Windows.Forms.RadioButton();
            rbLinkTypeAnd = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            lvConditionalOp = new ctlListViewColored();
            columnHeader7 = new ColumnHeader();
            removeConditional = new System.Windows.Forms.Button();
            addConditional = new System.Windows.Forms.Button();
            lvActiveConditionals = new ctlListViewColored();
            columnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            lvConditionalBool = new ctlListViewColored();
            columnHeader4 = new ColumnHeader();
            lvConditionalType = new ctlListViewColored();
            columnHeader3 = new ColumnHeader();
            lvSubConditional = new ctlListViewColored();
            columnHeader2 = new ColumnHeader();
            btnOkay = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            btnClearFilter = new System.Windows.Forms.Button();
            tbFilter = new System.Windows.Forms.TextBox();
            _modernPanel = new System.Windows.Forms.Panel();
            _titleLabel = new System.Windows.Forms.Label();
            _note = new System.Windows.Forms.Label();
            _conditionTypeLabel = new System.Windows.Forms.Label();
            _conditionType = new System.Windows.Forms.ComboBox();
            _linkTypeLabel = new System.Windows.Forms.Label();
            _linkType = new System.Windows.Forms.ComboBox();
            _choiceLabel = new System.Windows.Forms.Label();
            _search = new System.Windows.Forms.TextBox();
            _choices = new System.Windows.Forms.ListView();
            _choicesColumn = new ColumnHeader();
            _expression = new System.Windows.Forms.TextBox();
            _operatorLabel = new System.Windows.Forms.Label();
            _operator = new System.Windows.Forms.ComboBox();
            _valueLabel = new System.Windows.Forms.Label();
            _value = new System.Windows.Forms.ComboBox();
            _addRow = new System.Windows.Forms.Button();
            _removeRow = new System.Windows.Forms.Button();
            _currentRowsLabel = new System.Windows.Forms.Label();
            _conditionRows = new System.Windows.Forms.ListView();
            _rowLinkColumn = new ColumnHeader();
            _rowConditionColumn = new ColumnHeader();
            _rowOperatorColumn = new ColumnHeader();
            _rowValueColumn = new ColumnHeader();
            groupBox2.SuspendLayout();
            panelLinkType.SuspendLayout();
            panel1.SuspendLayout();
            _modernPanel.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panelLinkType);
            groupBox2.Controls.Add(lvConditionalOp);
            groupBox2.Controls.Add(removeConditional);
            groupBox2.Controls.Add(addConditional);
            groupBox2.Controls.Add(lvActiveConditionals);
            groupBox2.Controls.Add(lvConditionalBool);
            groupBox2.Controls.Add(lvConditionalType);
            groupBox2.Controls.Add(lvSubConditional);
            groupBox2.Location = new System.Drawing.Point(13, 0);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(1284, 483);
            groupBox2.TabIndex = 161;
            groupBox2.TabStop = false;
            groupBox2.Visible = false;
            // 
            // panelLinkType
            // 
            panelLinkType.BackColor = System.Drawing.Color.FromArgb(79, 87, 93);
            panelLinkType.Controls.Add(rbLinkTypeOr);
            panelLinkType.Controls.Add(rbLinkTypeAnd);
            panelLinkType.Controls.Add(label1);
            panelLinkType.Location = new System.Drawing.Point(657, 291);
            panelLinkType.Name = "panelLinkType";
            panelLinkType.Size = new System.Drawing.Size(181, 117);
            panelLinkType.TabIndex = 167;
            // 
            // rbLinkTypeOr
            // 
            rbLinkTypeOr.AutoSize = true;
            rbLinkTypeOr.Location = new System.Drawing.Point(31, 71);
            rbLinkTypeOr.Name = "rbLinkTypeOr";
            rbLinkTypeOr.Size = new System.Drawing.Size(41, 19);
            rbLinkTypeOr.TabIndex = 2;
            rbLinkTypeOr.TabStop = true;
            rbLinkTypeOr.Text = "OR";
            rbLinkTypeOr.UseVisualStyleBackColor = true;
            rbLinkTypeOr.CheckedChanged += rbLinkTypeOr_CheckedChanged;
            // 
            // rbLinkTypeAnd
            // 
            rbLinkTypeAnd.AutoSize = true;
            rbLinkTypeAnd.Location = new System.Drawing.Point(31, 45);
            rbLinkTypeAnd.Name = "rbLinkTypeAnd";
            rbLinkTypeAnd.Size = new System.Drawing.Size(50, 19);
            rbLinkTypeAnd.TabIndex = 1;
            rbLinkTypeAnd.TabStop = true;
            rbLinkTypeAnd.Text = "AND";
            rbLinkTypeAnd.UseVisualStyleBackColor = true;
            rbLinkTypeAnd.CheckedChanged += rbLinkTypeAnd_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 15);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(112, 15);
            label1.TabIndex = 0;
            label1.Text = "Link condition with:";
            // 
            // lvConditionalOp
            // 
            lvConditionalOp.Columns.AddRange(new ColumnHeader[] { columnHeader7 });
            lvConditionalOp.FullRowSelect = true;
            lvConditionalOp.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvConditionalOp.Items.AddRange(new System.Windows.Forms.ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            lvConditionalOp.Location = new System.Drawing.Point(537, 16);
            lvConditionalOp.LostFocusItem = -1;
            lvConditionalOp.Margin = new Padding(4, 3, 4, 3);
            lvConditionalOp.MultiSelect = false;
            lvConditionalOp.Name = "lvConditionalOp";
            lvConditionalOp.OwnerDraw = true;
            lvConditionalOp.Size = new System.Drawing.Size(112, 95);
            lvConditionalOp.TabIndex = 166;
            lvConditionalOp.UseCompatibleStateImageBehavior = false;
            lvConditionalOp.View = View.Details;
            lvConditionalOp.Visible = false;
            lvConditionalOp.DrawColumnHeader += ListView_DrawColumnHeader;
            lvConditionalOp.DrawItem += ListView_DrawItem;
            lvConditionalOp.Leave += ListView_Leave;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Stack is?";
            columnHeader7.Width = 81;
            // 
            // removeConditional
            // 
            removeConditional.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            removeConditional.FlatStyle = FlatStyle.Popup;
            removeConditional.Location = new System.Drawing.Point(657, 237);
            removeConditional.Margin = new Padding(4, 3, 4, 3);
            removeConditional.Name = "removeConditional";
            removeConditional.Size = new System.Drawing.Size(181, 33);
            removeConditional.TabIndex = 165;
            removeConditional.Text = "<= Remove Condition";
            removeConditional.UseVisualStyleBackColor = false;
            removeConditional.Click += removeConditional_Click;
            // 
            // addConditional
            // 
            addConditional.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            addConditional.FlatStyle = FlatStyle.Popup;
            addConditional.Location = new System.Drawing.Point(657, 196);
            addConditional.Margin = new Padding(4, 3, 4, 3);
            addConditional.Name = "addConditional";
            addConditional.Size = new System.Drawing.Size(181, 33);
            addConditional.TabIndex = 164;
            addConditional.Text = "Add Condition =>";
            addConditional.UseVisualStyleBackColor = false;
            addConditional.Click += addConditional_Click;
            // 
            // lvActiveConditionals
            // 
            lvActiveConditionals.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader5, columnHeader8, columnHeader6 });
            lvActiveConditionals.FullRowSelect = true;
            lvActiveConditionals.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvActiveConditionals.Location = new System.Drawing.Point(845, 16);
            lvActiveConditionals.LostFocusItem = -1;
            lvActiveConditionals.Margin = new Padding(4, 3, 4, 3);
            lvActiveConditionals.MultiSelect = false;
            lvActiveConditionals.Name = "lvActiveConditionals";
            lvActiveConditionals.Size = new System.Drawing.Size(432, 460);
            lvActiveConditionals.TabIndex = 163;
            lvActiveConditionals.UseCompatibleStateImageBehavior = false;
            lvActiveConditionals.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "";
            columnHeader1.Width = 50;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Active Conditional";
            columnHeader5.Width = 260;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "";
            columnHeader8.Width = 27;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Value";
            columnHeader6.Width = 46;
            // 
            // lvConditionalBool
            // 
            lvConditionalBool.Columns.AddRange(new ColumnHeader[] { columnHeader4 });
            lvConditionalBool.FullRowSelect = true;
            lvConditionalBool.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvConditionalBool.Location = new System.Drawing.Point(537, 117);
            lvConditionalBool.LostFocusItem = -1;
            lvConditionalBool.Margin = new Padding(4, 3, 4, 3);
            lvConditionalBool.MultiSelect = false;
            lvConditionalBool.Name = "lvConditionalBool";
            lvConditionalBool.OwnerDraw = true;
            lvConditionalBool.Size = new System.Drawing.Size(112, 359);
            lvConditionalBool.TabIndex = 162;
            lvConditionalBool.UseCompatibleStateImageBehavior = false;
            lvConditionalBool.View = View.Details;
            lvConditionalBool.DrawColumnHeader += ListView_DrawColumnHeader;
            lvConditionalBool.DrawItem += ListView_DrawItem;
            lvConditionalBool.Leave += ListView_Leave;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Conditional Boolean";
            columnHeader4.Width = 81;
            // 
            // lvConditionalType
            // 
            lvConditionalType.Columns.AddRange(new ColumnHeader[] { columnHeader3 });
            lvConditionalType.FullRowSelect = true;
            lvConditionalType.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvConditionalType.Location = new System.Drawing.Point(7, 16);
            lvConditionalType.LostFocusItem = -1;
            lvConditionalType.Margin = new Padding(4, 3, 4, 3);
            lvConditionalType.MultiSelect = false;
            lvConditionalType.Name = "lvConditionalType";
            lvConditionalType.OwnerDraw = true;
            lvConditionalType.Size = new System.Drawing.Size(135, 460);
            lvConditionalType.TabIndex = 161;
            lvConditionalType.UseCompatibleStateImageBehavior = false;
            lvConditionalType.View = View.Details;
            lvConditionalType.DrawColumnHeader += ListView_DrawColumnHeader;
            lvConditionalType.DrawItem += ListView_DrawItem;
            lvConditionalType.ItemSelectionChanged += lvConditionalType_SelectionChanged;
            lvConditionalType.Leave += ListView_Leave;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Effect Conditional";
            columnHeader3.Width = 102;
            // 
            // lvSubConditional
            // 
            lvSubConditional.Columns.AddRange(new ColumnHeader[] { columnHeader2 });
            lvSubConditional.FullRowSelect = true;
            lvSubConditional.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvSubConditional.Location = new System.Drawing.Point(149, 16);
            lvSubConditional.LostFocusItem = -1;
            lvSubConditional.Margin = new Padding(4, 3, 4, 3);
            lvSubConditional.MultiSelect = false;
            lvSubConditional.Name = "lvSubConditional";
            lvSubConditional.OwnerDraw = true;
            lvSubConditional.Size = new System.Drawing.Size(380, 460);
            lvSubConditional.TabIndex = 160;
            lvSubConditional.UseCompatibleStateImageBehavior = false;
            lvSubConditional.View = View.Details;
            lvSubConditional.DrawColumnHeader += ListView_DrawColumnHeader;
            lvSubConditional.DrawItem += ListView_DrawItem;
            lvSubConditional.ItemSelectionChanged += lvSubConditional_SelectionChanged;
            lvSubConditional.Leave += ListView_Leave;
            lvSubConditional.MouseClick += lvSubConditional_MouseClick;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Sub-Conditional Attribute";
            columnHeader2.Width = 320;
            // 
            // btnOkay
            // 
            btnOkay.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            btnOkay.DialogResult = DialogResult.OK;
            btnOkay.FlatStyle = FlatStyle.Popup;
            btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            btnOkay.Location = new System.Drawing.Point(1108, 490);
            btnOkay.Margin = new Padding(4, 3, 4, 3);
            btnOkay.Name = "btnOkay";
            btnOkay.Size = new System.Drawing.Size(88, 43);
            btnOkay.TabIndex = 162;
            btnOkay.Text = "Ok";
            btnOkay.UseVisualStyleBackColor = false;
            btnOkay.Click += btnOkay_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatStyle = FlatStyle.Popup;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            btnCancel.Location = new System.Drawing.Point(1203, 490);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 43);
            btnCancel.TabIndex = 163;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(btnClearFilter);
            panel1.Controls.Add(tbFilter);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOkay);
            panel1.Controls.Add(_modernPanel);
            panel1.Controls.Add(groupBox2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1310, 542);
            panel1.TabIndex = 164;
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = System.Drawing.Color.FromArgb(46, 56, 171);
            btnClearFilter.FlatStyle = FlatStyle.Popup;
            btnClearFilter.Location = new System.Drawing.Point(550, 494);
            btnClearFilter.Margin = new Padding(4, 3, 4, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new System.Drawing.Size(112, 23);
            btnClearFilter.TabIndex = 165;
            btnClearFilter.Text = "Clear Filter";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Visible = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // tbFilter
            // 
            tbFilter.Location = new System.Drawing.Point(162, 494);
            tbFilter.Name = "tbFilter";
            tbFilter.Size = new System.Drawing.Size(380, 23);
            tbFilter.TabIndex = 164;
            tbFilter.Visible = false;
            tbFilter.TextChanged += tbFilter_TextChanged;
            // 
            // _modernPanel
            // 
            _modernPanel.BackColor = System.Drawing.Color.FromArgb(48, 56, 62);
            _modernPanel.Controls.Add(_titleLabel);
            _modernPanel.Controls.Add(_note);
            _modernPanel.Controls.Add(_conditionTypeLabel);
            _modernPanel.Controls.Add(_conditionType);
            _modernPanel.Controls.Add(_linkTypeLabel);
            _modernPanel.Controls.Add(_linkType);
            _modernPanel.Controls.Add(_choiceLabel);
            _modernPanel.Controls.Add(_search);
            _modernPanel.Controls.Add(_choices);
            _modernPanel.Controls.Add(_expression);
            _modernPanel.Controls.Add(_operatorLabel);
            _modernPanel.Controls.Add(_operator);
            _modernPanel.Controls.Add(_valueLabel);
            _modernPanel.Controls.Add(_value);
            _modernPanel.Controls.Add(_addRow);
            _modernPanel.Controls.Add(_removeRow);
            _modernPanel.Controls.Add(_currentRowsLabel);
            _modernPanel.Controls.Add(_conditionRows);
            _modernPanel.Dock = DockStyle.Fill;
            _modernPanel.Location = new System.Drawing.Point(0, 0);
            _modernPanel.Name = "_modernPanel";
            _modernPanel.Size = new System.Drawing.Size(1306, 538);
            _modernPanel.TabIndex = 166;
            // 
            // _titleLabel
            // 
            _titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            _titleLabel.ForeColor = System.Drawing.Color.White;
            _titleLabel.Location = new System.Drawing.Point(16, 14);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new System.Drawing.Size(260, 24);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Effect Conditions";
            // 
            // _note
            // 
            _note.ForeColor = System.Drawing.Color.Gainsboro;
            _note.Location = new System.Drawing.Point(16, 40);
            _note.Name = "_note";
            _note.Size = new System.Drawing.Size(850, 22);
            _note.TabIndex = 1;
            _note.Text = "Pick a condition type, choose its value, then add it to the list. Advanced rows are saved with the database.";
            // 
            // _conditionTypeLabel
            // 
            _conditionTypeLabel.ForeColor = System.Drawing.Color.White;
            _conditionTypeLabel.Location = new System.Drawing.Point(16, 75);
            _conditionTypeLabel.Name = "_conditionTypeLabel";
            _conditionTypeLabel.Size = new System.Drawing.Size(140, 18);
            _conditionTypeLabel.TabIndex = 2;
            _conditionTypeLabel.Text = "Condition";
            // 
            // _conditionType
            // 
            _conditionType.DropDownStyle = ComboBoxStyle.DropDownList;
            _conditionType.FormattingEnabled = true;
            _conditionType.Location = new System.Drawing.Point(16, 96);
            _conditionType.Name = "_conditionType";
            _conditionType.Size = new System.Drawing.Size(210, 23);
            _conditionType.TabIndex = 3;
            _conditionType.SelectedIndexChanged += ModernConditionTypeChanged;
            // 
            // _linkTypeLabel
            // 
            _linkTypeLabel.ForeColor = System.Drawing.Color.White;
            _linkTypeLabel.Location = new System.Drawing.Point(238, 75);
            _linkTypeLabel.Name = "_linkTypeLabel";
            _linkTypeLabel.Size = new System.Drawing.Size(140, 18);
            _linkTypeLabel.TabIndex = 4;
            _linkTypeLabel.Text = "Join";
            // 
            // _linkType
            // 
            _linkType.DropDownStyle = ComboBoxStyle.DropDownList;
            _linkType.FormattingEnabled = true;
            _linkType.Items.AddRange(new object[] { "AND", "OR" });
            _linkType.Location = new System.Drawing.Point(238, 96);
            _linkType.Name = "_linkType";
            _linkType.SelectedIndex = 0;
            _linkType.Size = new System.Drawing.Size(88, 23);
            _linkType.TabIndex = 5;
            // 
            // _choiceLabel
            // 
            _choiceLabel.ForeColor = System.Drawing.Color.White;
            _choiceLabel.Location = new System.Drawing.Point(16, 132);
            _choiceLabel.Name = "_choiceLabel";
            _choiceLabel.Size = new System.Drawing.Size(160, 18);
            _choiceLabel.TabIndex = 6;
            _choiceLabel.Text = "Pick";
            // 
            // _search
            // 
            _search.Location = new System.Drawing.Point(16, 153);
            _search.Name = "_search";
            _search.PlaceholderText = "Filter choices";
            _search.Size = new System.Drawing.Size(310, 23);
            _search.TabIndex = 7;
            _search.TextChanged += ModernSearchTextChanged;
            // 
            // _choices
            // 
            _choices.Columns.AddRange(new ColumnHeader[] { _choicesColumn });
            _choices.FullRowSelect = true;
            _choices.HideSelection = false;
            _choices.Location = new System.Drawing.Point(16, 182);
            _choices.MultiSelect = false;
            _choices.Name = "_choices";
            _choices.Size = new System.Drawing.Size(430, 260);
            _choices.TabIndex = 8;
            _choices.UseCompatibleStateImageBehavior = false;
            _choices.View = View.Details;
            _choices.SelectedIndexChanged += ModernChoiceSelectedIndexChanged;
            // 
            // _choicesColumn
            // 
            _choicesColumn.Text = "Choice";
            _choicesColumn.Width = 420;
            // 
            // _expression
            // 
            _expression.Location = new System.Drawing.Point(16, 182);
            _expression.Multiline = true;
            _expression.Name = "_expression";
            _expression.ScrollBars = ScrollBars.Vertical;
            _expression.Size = new System.Drawing.Size(430, 260);
            _expression.TabIndex = 9;
            _expression.Visible = false;
            // 
            // _operatorLabel
            // 
            _operatorLabel.ForeColor = System.Drawing.Color.White;
            _operatorLabel.Location = new System.Drawing.Point(462, 132);
            _operatorLabel.Name = "_operatorLabel";
            _operatorLabel.Size = new System.Drawing.Size(140, 18);
            _operatorLabel.TabIndex = 10;
            _operatorLabel.Text = "Compare";
            // 
            // _operator
            // 
            _operator.DropDownStyle = ComboBoxStyle.DropDownList;
            _operator.FormattingEnabled = true;
            _operator.Location = new System.Drawing.Point(462, 153);
            _operator.Name = "_operator";
            _operator.Size = new System.Drawing.Size(130, 23);
            _operator.TabIndex = 11;
            // 
            // _valueLabel
            // 
            _valueLabel.ForeColor = System.Drawing.Color.White;
            _valueLabel.Location = new System.Drawing.Point(606, 132);
            _valueLabel.Name = "_valueLabel";
            _valueLabel.Size = new System.Drawing.Size(120, 18);
            _valueLabel.TabIndex = 12;
            _valueLabel.Text = "Value";
            // 
            // _value
            // 
            _value.DropDownStyle = ComboBoxStyle.DropDownList;
            _value.FormattingEnabled = true;
            _value.Location = new System.Drawing.Point(606, 153);
            _value.Name = "_value";
            _value.Size = new System.Drawing.Size(130, 23);
            _value.TabIndex = 13;
            // 
            // _addRow
            // 
            _addRow.BackColor = System.Drawing.Color.FromArgb(64, 78, 237);
            _addRow.FlatStyle = FlatStyle.Popup;
            _addRow.ForeColor = System.Drawing.Color.White;
            _addRow.Location = new System.Drawing.Point(462, 190);
            _addRow.Name = "_addRow";
            _addRow.Size = new System.Drawing.Size(274, 32);
            _addRow.TabIndex = 14;
            _addRow.Text = "Add";
            _addRow.UseVisualStyleBackColor = false;
            _addRow.Click += AddModernCondition_Click;
            // 
            // _removeRow
            // 
            _removeRow.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            _removeRow.FlatStyle = FlatStyle.Popup;
            _removeRow.ForeColor = System.Drawing.Color.White;
            _removeRow.Location = new System.Drawing.Point(462, 230);
            _removeRow.Name = "_removeRow";
            _removeRow.Size = new System.Drawing.Size(274, 32);
            _removeRow.TabIndex = 15;
            _removeRow.Text = "Remove Selected";
            _removeRow.UseVisualStyleBackColor = false;
            _removeRow.Click += RemoveModernCondition_Click;
            // 
            // _currentRowsLabel
            // 
            _currentRowsLabel.ForeColor = System.Drawing.Color.White;
            _currentRowsLabel.Location = new System.Drawing.Point(760, 75);
            _currentRowsLabel.Name = "_currentRowsLabel";
            _currentRowsLabel.Size = new System.Drawing.Size(160, 18);
            _currentRowsLabel.TabIndex = 16;
            _currentRowsLabel.Text = "Current Conditions";
            // 
            // _conditionRows
            // 
            _conditionRows.Columns.AddRange(new ColumnHeader[] { _rowLinkColumn, _rowConditionColumn, _rowOperatorColumn, _rowValueColumn });
            _conditionRows.FullRowSelect = true;
            _conditionRows.HideSelection = false;
            _conditionRows.Location = new System.Drawing.Point(760, 96);
            _conditionRows.MultiSelect = false;
            _conditionRows.Name = "_conditionRows";
            _conditionRows.Size = new System.Drawing.Size(520, 346);
            _conditionRows.TabIndex = 17;
            _conditionRows.UseCompatibleStateImageBehavior = false;
            _conditionRows.View = View.Details;
            // 
            // _rowLinkColumn
            // 
            _rowLinkColumn.Text = "";
            _rowLinkColumn.Width = 48;
            // 
            // _rowConditionColumn
            // 
            _rowConditionColumn.Text = "Condition";
            _rowConditionColumn.Width = 300;
            // 
            // _rowOperatorColumn
            // 
            _rowOperatorColumn.Text = "Op";
            _rowOperatorColumn.Width = 45;
            // 
            // _rowValueColumn
            // 
            _rowValueColumn.Text = "Value";
            _rowValueColumn.Width = 90;
            // 
            // frmEffectConditionals
            // 
            AcceptButton = btnOkay;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(153, 170, 181);
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(1310, 542);
            Controls.Add(panel1);
            ForeColor = System.Drawing.Color.Azure;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            Name = "frmEffectConditionals";
            ShowInTaskbar = false;
            Text = "Effect Conditions";
            groupBox2.ResumeLayout(false);
            panelLinkType.ResumeLayout(false);
            panelLinkType.PerformLayout();
            _modernPanel.ResumeLayout(false);
            _modernPanel.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private ctlListViewColored lvConditionalOp;
        private ColumnHeader columnHeader7;
        private System.Windows.Forms.Button removeConditional;
        private System.Windows.Forms.Button addConditional;
        private ctlListViewColored lvActiveConditionals;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader6;
        private ctlListViewColored lvConditionalBool;
        private ColumnHeader columnHeader4;
        private ctlListViewColored lvConditionalType;
        private ColumnHeader columnHeader3;
        private ctlListViewColored lvSubConditional;
        private ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelLinkType;
        private System.Windows.Forms.RadioButton rbLinkTypeOr;
        private System.Windows.Forms.RadioButton rbLinkTypeAnd;
        private System.Windows.Forms.Label label1;
        private ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.Panel _modernPanel;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _note;
        private System.Windows.Forms.Label _conditionTypeLabel;
        private System.Windows.Forms.ComboBox _conditionType;
        private System.Windows.Forms.Label _linkTypeLabel;
        private System.Windows.Forms.ComboBox _linkType;
        private System.Windows.Forms.Label _choiceLabel;
        private System.Windows.Forms.TextBox _search;
        private System.Windows.Forms.ListView _choices;
        private ColumnHeader _choicesColumn;
        private System.Windows.Forms.TextBox _expression;
        private System.Windows.Forms.Label _operatorLabel;
        private System.Windows.Forms.ComboBox _operator;
        private System.Windows.Forms.Label _valueLabel;
        private System.Windows.Forms.ComboBox _value;
        private System.Windows.Forms.Button _addRow;
        private System.Windows.Forms.Button _removeRow;
        private System.Windows.Forms.Label _currentRowsLabel;
        private System.Windows.Forms.ListView _conditionRows;
        private ColumnHeader _rowLinkColumn;
        private ColumnHeader _rowConditionColumn;
        private ColumnHeader _rowOperatorColumn;
        private ColumnHeader _rowValueColumn;
    }
}
