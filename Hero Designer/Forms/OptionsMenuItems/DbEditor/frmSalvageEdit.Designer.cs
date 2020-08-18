using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSalvageEdit
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
            this.lvSalvage = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.txtExternal = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtInternal = new System.Windows.Forms.TextBox();
            this.cbRarity = new System.Windows.Forms.ComboBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.cbOrigin = new System.Windows.Forms.ComboBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.cbLevel = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.lvSalvage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4]
            {
                this.ColumnHeader1,
                this.ColumnHeader2,
                this.ColumnHeader3,
                this.ColumnHeader4
            });
            this.lvSalvage.FullRowSelect = true;
            this.lvSalvage.HideSelection = false;

            this.lvSalvage.Location = new System.Drawing.Point(12, 12);
            this.lvSalvage.MultiSelect = false;
            this.lvSalvage.Name = "lvSalvage";

            this.lvSalvage.Size = new System.Drawing.Size(468, 316);
            this.lvSalvage.TabIndex = 0;
            this.lvSalvage.UseCompatibleStateImageBehavior = false;
            this.lvSalvage.View = System.Windows.Forms.View.Details;
            this.lvSalvage.SelectedIndexChanged += new System.EventHandler(lvSalvage_SelectedIndexChanged);
            this.ColumnHeader1.Text = "Name";
            this.ColumnHeader1.Width = 213;
            this.ColumnHeader2.Text = "Origin";
            this.ColumnHeader2.Width = 72;
            this.ColumnHeader3.Text = "Rarity";
            this.ColumnHeader3.Width = 76;
            this.ColumnHeader4.Text = "Level";
            this.ColumnHeader4.Width = 75;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnOK.Location = new System.Drawing.Point(605, 304);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(113, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Save && Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(486, 304);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(113, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);

            this.btnImport.Location = new System.Drawing.Point(486, 274);
            this.btnImport.Name = "btnImport";

            this.btnImport.Size = new System.Drawing.Size(232, 24);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Clear and Import from Spreadsheet";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(btnImport_Click);

            this.txtExternal.Location = new System.Drawing.Point(564, 12);
            this.txtExternal.Name = "txtExternal";

            this.txtExternal.Size = new System.Drawing.Size(154, 20);
            this.txtExternal.TabIndex = 4;
            this.txtExternal.TextChanged += new System.EventHandler(txtExternal_TextChanged);
            this.txtInternal.TextChanged += new System.EventHandler(txtInternal_TextChanged);

            this.Label1.Location = new System.Drawing.Point(486, 12);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(72, 20);
            this.Label1.TabIndex = 5;
            this.Label1.Text = "Name:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label2.Location = new System.Drawing.Point(486, 38);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(72, 20);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "Internal:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtInternal.Location = new System.Drawing.Point(564, 38);
            this.txtInternal.Name = "txtInternal";

            this.txtInternal.Size = new System.Drawing.Size(154, 20);
            this.txtInternal.TabIndex = 6;
            this.cbRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRarity.FormattingEnabled = true;

            this.cbRarity.Location = new System.Drawing.Point(564, 64);
            this.cbRarity.Name = "cbRarity";

            this.cbRarity.Size = new System.Drawing.Size(154, 22);
            this.cbRarity.TabIndex = 8;
            this.cbRarity.SelectedIndexChanged += new System.EventHandler(cbRarity_SelectedIndexChanged);

            this.Label3.Location = new System.Drawing.Point(486, 64);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(72, 22);
            this.Label3.TabIndex = 9;
            this.Label3.Text = "Rarity:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label4.Location = new System.Drawing.Point(486, 92);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(72, 22);
            this.Label4.TabIndex = 11;
            this.Label4.Text = "Origin:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrigin.FormattingEnabled = true;

            this.cbOrigin.Location = new System.Drawing.Point(564, 92);
            this.cbOrigin.Name = "cbOrigin";

            this.cbOrigin.Size = new System.Drawing.Size(154, 22);
            this.cbOrigin.TabIndex = 10;
            this.cbOrigin.SelectedIndexChanged += new System.EventHandler(cbOrigin_SelectedIndexChanged);

            this.Label5.Location = new System.Drawing.Point(486, 120);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(72, 22);
            this.Label5.TabIndex = 13;
            this.Label5.Text = "Level:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLevel.FormattingEnabled = true;

            this.cbLevel.Location = new System.Drawing.Point(564, 120);
            this.cbLevel.Name = "cbLevel";

            this.cbLevel.Size = new System.Drawing.Size(154, 22);
            this.cbLevel.TabIndex = 12;
            this.cbLevel.SelectedIndexChanged += new System.EventHandler(cbLevel_SelectedIndexChanged);

            this.btnDelete.Location = new System.Drawing.Point(486, 214);
            this.btnDelete.Name = "btnDelete";

            this.btnDelete.Size = new System.Drawing.Size(113, 24);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(btnDelete_Click);

            this.btnAdd.Location = new System.Drawing.Point(486, 184);
            this.btnAdd.Name = "btnAdd";

            this.btnAdd.Size = new System.Drawing.Size(113, 24);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
            this.AcceptButton = (System.Windows.Forms.IButtonControl)this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = (System.Windows.Forms.IButtonControl)this.btnCancel;

            this.ClientSize = new System.Drawing.Size(730, 340);
            this.Controls.Add((System.Windows.Forms.Control)this.btnAdd);
            this.Controls.Add((System.Windows.Forms.Control)this.btnDelete);
            this.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.Controls.Add((System.Windows.Forms.Control)this.cbLevel);
            this.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.Controls.Add((System.Windows.Forms.Control)this.cbOrigin);
            this.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.Controls.Add((System.Windows.Forms.Control)this.cbRarity);
            this.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.Controls.Add((System.Windows.Forms.Control)this.txtInternal);
            this.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.Controls.Add((System.Windows.Forms.Control)this.txtExternal);
            this.Controls.Add((System.Windows.Forms.Control)this.btnImport);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control)this.lvSalvage);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Salvage Editor";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        Button btnAdd;
        Button btnCancel;
        Button btnDelete;
        Button btnImport;
        Button btnOK;
        ComboBox cbLevel;
        ComboBox cbOrigin;
        ComboBox cbRarity;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        Label Label1;
        Label Label2;
        Label Label3;
        Label Label4;
        Label Label5;
        ListView lvSalvage;
        TextBox txtExternal;
        TextBox txtInternal;
    }
}