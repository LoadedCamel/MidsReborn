namespace Hero_Designer.Forms.Controls
{
	partial class InfoPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.infoTab = new System.Windows.Forms.TabPage();
			this.effectsTab = new System.Windows.Forms.TabPage();
			this.totalsTab = new System.Windows.Forms.TabPage();
			this.enhanceTab = new System.Windows.Forms.TabPage();
			this.panel6 = new System.Windows.Forms.Panel();
			this.popoutButton = new FontAwesome.Sharp.IconButton();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.richTextBox3 = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.infoTab.SuspendLayout();
			this.panel6.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.infoTab);
			this.tabControl1.Controls.Add(this.effectsTab);
			this.tabControl1.Controls.Add(this.totalsTab);
			this.tabControl1.Controls.Add(this.enhanceTab);
			this.tabControl1.Location = new System.Drawing.Point(3, 6);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(364, 525);
			this.tabControl1.TabIndex = 0;
			// 
			// infoTab
			// 
			this.infoTab.BackColor = System.Drawing.Color.DodgerBlue;
			this.infoTab.Controls.Add(this.label1);
			this.infoTab.Controls.Add(this.richTextBox3);
			this.infoTab.Controls.Add(this.listView1);
			this.infoTab.Controls.Add(this.richTextBox2);
			this.infoTab.Controls.Add(this.richTextBox1);
			this.infoTab.Controls.Add(this.textBox1);
			this.infoTab.Location = new System.Drawing.Point(4, 25);
			this.infoTab.Name = "infoTab";
			this.infoTab.Padding = new System.Windows.Forms.Padding(3);
			this.infoTab.Size = new System.Drawing.Size(356, 496);
			this.infoTab.TabIndex = 0;
			this.infoTab.Text = "Info";
			// 
			// effectsTab
			// 
			this.effectsTab.Location = new System.Drawing.Point(4, 25);
			this.effectsTab.Name = "effectsTab";
			this.effectsTab.Padding = new System.Windows.Forms.Padding(3);
			this.effectsTab.Size = new System.Drawing.Size(267, 394);
			this.effectsTab.TabIndex = 1;
			this.effectsTab.Text = "Effects";
			this.effectsTab.UseVisualStyleBackColor = true;
			// 
			// totalsTab
			// 
			this.totalsTab.Location = new System.Drawing.Point(4, 25);
			this.totalsTab.Name = "totalsTab";
			this.totalsTab.Padding = new System.Windows.Forms.Padding(3);
			this.totalsTab.Size = new System.Drawing.Size(267, 394);
			this.totalsTab.TabIndex = 2;
			this.totalsTab.Text = "Totals";
			this.totalsTab.UseVisualStyleBackColor = true;
			// 
			// enhanceTab
			// 
			this.enhanceTab.Location = new System.Drawing.Point(4, 25);
			this.enhanceTab.Name = "enhanceTab";
			this.enhanceTab.Padding = new System.Windows.Forms.Padding(3);
			this.enhanceTab.Size = new System.Drawing.Size(267, 394);
			this.enhanceTab.TabIndex = 3;
			this.enhanceTab.Text = "Enhancements";
			this.enhanceTab.UseVisualStyleBackColor = true;
			// 
			// panel6
			// 
			this.panel6.BackColor = System.Drawing.Color.Transparent;
			this.panel6.Controls.Add(this.popoutButton);
			this.panel6.Controls.Add(this.tabControl1);
			this.panel6.Location = new System.Drawing.Point(4, 4);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(370, 534);
			this.panel6.TabIndex = 1;
			// 
			// popoutButton
			// 
			this.popoutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.popoutButton.BackColor = System.Drawing.Color.Transparent;
			this.popoutButton.FlatAppearance.BorderSize = 0;
			this.popoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.popoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.popoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.popoutButton.Flip = FontAwesome.Sharp.FlipOrientation.Normal;
			this.popoutButton.ForeColor = System.Drawing.Color.White;
			this.popoutButton.IconChar = FontAwesome.Sharp.IconChar.Anchor;
			this.popoutButton.IconColor = System.Drawing.Color.White;
			this.popoutButton.IconSize = 24;
			this.popoutButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.popoutButton.Location = new System.Drawing.Point(339, 6);
			this.popoutButton.Name = "popoutButton";
			this.popoutButton.Rotation = 0D;
			this.popoutButton.Size = new System.Drawing.Size(28, 24);
			this.popoutButton.TabIndex = 1;
			this.popoutButton.UseVisualStyleBackColor = false;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.Black;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.textBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.ForeColor = System.Drawing.Color.White;
			this.textBox1.Location = new System.Drawing.Point(24, 6);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(310, 19);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "Some Test Text";
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.Color.Black;
			this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.richTextBox1.ForeColor = System.Drawing.Color.White;
			this.richTextBox1.Location = new System.Drawing.Point(6, 31);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(344, 74);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "";
			// 
			// richTextBox2
			// 
			this.richTextBox2.BackColor = System.Drawing.Color.Black;
			this.richTextBox2.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.richTextBox2.ForeColor = System.Drawing.Color.White;
			this.richTextBox2.Location = new System.Drawing.Point(6, 111);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ReadOnly = true;
			this.richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBox2.Size = new System.Drawing.Size(344, 119);
			this.richTextBox2.TabIndex = 2;
			this.richTextBox2.Text = "";
			// 
			// listView1
			// 
			this.listView1.BackColor = System.Drawing.Color.Black;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listView1.ForeColor = System.Drawing.Color.White;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(6, 236);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowItemToolTips = true;
			this.listView1.Size = new System.Drawing.Size(344, 189);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.List;
			// 
			// richTextBox3
			// 
			this.richTextBox3.BackColor = System.Drawing.Color.Black;
			this.richTextBox3.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.richTextBox3.ForeColor = System.Drawing.Color.White;
			this.richTextBox3.Location = new System.Drawing.Point(7, 446);
			this.richTextBox3.Name = "richTextBox3";
			this.richTextBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBox3.Size = new System.Drawing.Size(343, 44);
			this.richTextBox3.TabIndex = 4;
			this.richTextBox3.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(149, 427);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Damage";
			// 
			// InfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.panel6);
			this.Name = "InfoPanel";
			this.Size = new System.Drawing.Size(377, 541);
			this.tabControl1.ResumeLayout(false);
			this.infoTab.ResumeLayout(false);
			this.infoTab.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage infoTab;
		private System.Windows.Forms.TabPage effectsTab;
		private System.Windows.Forms.TabPage totalsTab;
		private System.Windows.Forms.TabPage enhanceTab;
		private System.Windows.Forms.Panel panel6;
		private FontAwesome.Sharp.IconButton popoutButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox3;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.TextBox textBox1;
	}
}
