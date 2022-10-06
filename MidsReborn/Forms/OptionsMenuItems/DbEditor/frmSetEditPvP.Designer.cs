using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class FrmSetEditPvP
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSetEditPvP));
            this.btnImage = new System.Windows.Forms.Button();
            this.btnNoImage = new System.Windows.Forms.Button();
            this.ilEnh = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSetType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.udMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.udMinLevel = new System.Windows.Forms.NumericUpDown();
            this.txtInternal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNameShort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNameFull = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvEnh = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtBonusFilter = new System.Windows.Forms.TextBox();
            this.lvBonusList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtAlternate = new System.Windows.Forms.TextBox();
            this.lstBonus = new System.Windows.Forms.ListBox();
            this.cbSlotCount = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rtbBonus = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.rbIfAny = new System.Windows.Forms.RadioButton();
            this.rbIfCritter = new System.Windows.Forms.RadioButton();
            this.rbIfPlayer = new System.Windows.Forms.RadioButton();
            this.ImagePicker = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinLevel)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImage
            // 
            this.btnImage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnImage.Location = new System.Drawing.Point(12, 12);
            this.btnImage.Name = "btnImage";
            this.btnImage.Size = new System.Drawing.Size(97, 68);
            this.btnImage.TabIndex = 0;
            this.btnImage.Text = "ImageName";
            this.btnImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnImage.UseVisualStyleBackColor = true;
            this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
            // 
            // btnNoImage
            // 
            this.btnNoImage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNoImage.Location = new System.Drawing.Point(12, 86);
            this.btnNoImage.Name = "btnNoImage";
            this.btnNoImage.Size = new System.Drawing.Size(97, 23);
            this.btnNoImage.TabIndex = 1;
            this.btnNoImage.Text = "Clear Image";
            this.btnNoImage.UseVisualStyleBackColor = true;
            this.btnNoImage.Click += new System.EventHandler(this.btnNoImage_Click);
            // 
            // ilEnh
            // 
            this.ilEnh.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilEnh.ImageSize = new System.Drawing.Size(30, 30);
            this.ilEnh.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbSetType);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtDesc);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.udMaxLevel);
            this.groupBox1.Controls.Add(this.udMinLevel);
            this.groupBox1.Controls.Add(this.txtInternal);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNameShort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtNameFull);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(115, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 238);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // cbSetType
            // 
            this.cbSetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSetType.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbSetType.FormattingEnabled = true;
            this.cbSetType.Location = new System.Drawing.Point(68, 209);
            this.cbSetType.Name = "cbSetType";
            this.cbSetType.Size = new System.Drawing.Size(156, 22);
            this.cbSetType.TabIndex = 13;
            this.cbSetType.SelectedIndexChanged += new System.EventHandler(this.cbSetType_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 14);
            this.label7.TabIndex = 12;
            this.label7.Text = "SetType:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(68, 99);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtDesc.Size = new System.Drawing.Size(156, 104);
            this.txtDesc.TabIndex = 11;
            this.txtDesc.Text = "";
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "Description:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 14);
            this.label5.TabIndex = 9;
            this.label5.Text = "MaxLevel";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 8;
            this.label4.Text = "MinLevel";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // udMaxLevel
            // 
            this.udMaxLevel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udMaxLevel.Location = new System.Drawing.Point(13, 180);
            this.udMaxLevel.Name = "udMaxLevel";
            this.udMaxLevel.Size = new System.Drawing.Size(32, 20);
            this.udMaxLevel.TabIndex = 7;
            this.udMaxLevel.ValueChanged += new System.EventHandler(this.udMaxLevel_ValueChanged);
            this.udMaxLevel.Leave += new System.EventHandler(this.udMaxLevel_Leave);
            // 
            // udMinLevel
            // 
            this.udMinLevel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udMinLevel.Location = new System.Drawing.Point(13, 140);
            this.udMinLevel.Name = "udMinLevel";
            this.udMinLevel.Size = new System.Drawing.Size(32, 20);
            this.udMinLevel.TabIndex = 6;
            this.udMinLevel.ValueChanged += new System.EventHandler(this.udMinLevel_ValueChanged);
            this.udMinLevel.Leave += new System.EventHandler(this.udMinLevel_Leave);
            // 
            // txtInternal
            // 
            this.txtInternal.Location = new System.Drawing.Point(68, 73);
            this.txtInternal.Name = "txtInternal";
            this.txtInternal.Size = new System.Drawing.Size(156, 20);
            this.txtInternal.TabIndex = 5;
            this.txtInternal.TextChanged += new System.EventHandler(this.txtInternal_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(25, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "Internal:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameShort
            // 
            this.txtNameShort.Location = new System.Drawing.Point(69, 47);
            this.txtNameShort.Name = "txtNameShort";
            this.txtNameShort.Size = new System.Drawing.Size(155, 20);
            this.txtNameShort.TabIndex = 3;
            this.txtNameShort.TextChanged += new System.EventHandler(this.txtNameShort_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Short Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameFull
            // 
            this.txtNameFull.Location = new System.Drawing.Point(69, 21);
            this.txtNameFull.Name = "txtNameFull";
            this.txtNameFull.Size = new System.Drawing.Size(156, 20);
            this.txtNameFull.TabIndex = 1;
            this.txtNameFull.TextChanged += new System.EventHandler(this.txtNameFull_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Full Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvEnh);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(333, 229);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enhancements belonging to this set:";
            // 
            // lvEnh
            // 
            this.lvEnh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvEnh.FullRowSelect = true;
            this.lvEnh.HideSelection = false;
            this.lvEnh.LargeImageList = this.ilEnh;
            this.lvEnh.Location = new System.Drawing.Point(6, 19);
            this.lvEnh.Name = "lvEnh";
            this.lvEnh.Size = new System.Drawing.Size(321, 204);
            this.lvEnh.SmallImageList = this.ilEnh;
            this.lvEnh.TabIndex = 0;
            this.lvEnh.UseCompatibleStateImageBehavior = false;
            this.lvEnh.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 244;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Classes";
            this.columnHeader2.Width = 71;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtBonusFilter);
            this.groupBox3.Controls.Add(this.lvBonusList);
            this.groupBox3.Controls.Add(this.txtAlternate);
            this.groupBox3.Controls.Add(this.lstBonus);
            this.groupBox3.Controls.Add(this.cbSlotCount);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.rtbBonus);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(351, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(785, 473);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Set Bonuses";
            // 
            // txtBonusFilter
            // 
            this.txtBonusFilter.Location = new System.Drawing.Point(318, 447);
            this.txtBonusFilter.Name = "txtBonusFilter";
            this.txtBonusFilter.Size = new System.Drawing.Size(428, 20);
            this.txtBonusFilter.TabIndex = 6;
            this.txtBonusFilter.TextChanged += new System.EventHandler(this.TxtBonusFilter_TextChanged);
            // 
            // lvBonusList
            // 
            this.lvBonusList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvBonusList.HideSelection = false;
            this.lvBonusList.Location = new System.Drawing.Point(281, 20);
            this.lvBonusList.Name = "lvBonusList";
            this.lvBonusList.Size = new System.Drawing.Size(498, 421);
            this.lvBonusList.TabIndex = 5;
            this.lvBonusList.UseCompatibleStateImageBehavior = false;
            this.lvBonusList.View = System.Windows.Forms.View.Details;
            this.lvBonusList.DoubleClick += new System.EventHandler(this.lvBonusList_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Bonus";
            this.columnHeader3.Width = 275;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Effect";
            this.columnHeader4.Width = 350;
            // 
            // txtAlternate
            // 
            this.txtAlternate.Location = new System.Drawing.Point(10, 447);
            this.txtAlternate.Name = "txtAlternate";
            this.txtAlternate.Size = new System.Drawing.Size(263, 20);
            this.txtAlternate.TabIndex = 4;
            this.txtAlternate.TextChanged += new System.EventHandler(this.txtAlternate_TextChanged);
            // 
            // lstBonus
            // 
            this.lstBonus.FormattingEnabled = true;
            this.lstBonus.ItemHeight = 14;
            this.lstBonus.Location = new System.Drawing.Point(10, 337);
            this.lstBonus.Name = "lstBonus";
            this.lstBonus.Size = new System.Drawing.Size(263, 102);
            this.lstBonus.TabIndex = 3;
            this.lstBonus.DoubleClick += new System.EventHandler(this.lstBonus_DoubleClick);
            // 
            // cbSlotCount
            // 
            this.cbSlotCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSlotCount.FormattingEnabled = true;
            this.cbSlotCount.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.cbSlotCount.Location = new System.Drawing.Point(108, 309);
            this.cbSlotCount.Name = "cbSlotCount";
            this.cbSlotCount.Size = new System.Drawing.Size(165, 22);
            this.cbSlotCount.TabIndex = 2;
            this.cbSlotCount.SelectedIndexChanged += new System.EventHandler(this.cbSlotX_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 312);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 14);
            this.label8.TabIndex = 1;
            this.label8.Text = "Bonus for slotting:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rtbBonus
            // 
            this.rtbBonus.Location = new System.Drawing.Point(7, 20);
            this.rtbBonus.Name = "rtbBonus";
            this.rtbBonus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbBonus.Size = new System.Drawing.Size(267, 283);
            this.rtbBonus.TabIndex = 0;
            this.rtbBonus.Text = "";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(980, 491);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(1061, 491);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(376, 496);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "If Target =";
            // 
            // rbIfAny
            // 
            this.rbIfAny.AutoSize = true;
            this.rbIfAny.Checked = true;
            this.rbIfAny.Location = new System.Drawing.Point(438, 494);
            this.rbIfAny.Name = "rbIfAny";
            this.rbIfAny.Size = new System.Drawing.Size(43, 17);
            this.rbIfAny.TabIndex = 8;
            this.rbIfAny.TabStop = true;
            this.rbIfAny.Text = "Any";
            this.rbIfAny.UseVisualStyleBackColor = true;
            // 
            // rbIfCritter
            // 
            this.rbIfCritter.AutoSize = true;
            this.rbIfCritter.Location = new System.Drawing.Point(487, 494);
            this.rbIfCritter.Name = "rbIfCritter";
            this.rbIfCritter.Size = new System.Drawing.Size(57, 17);
            this.rbIfCritter.TabIndex = 9;
            this.rbIfCritter.TabStop = true;
            this.rbIfCritter.Text = "Critters";
            this.rbIfCritter.UseVisualStyleBackColor = true;
            // 
            // rbIfPlayer
            // 
            this.rbIfPlayer.AutoSize = true;
            this.rbIfPlayer.Location = new System.Drawing.Point(550, 494);
            this.rbIfPlayer.Name = "rbIfPlayer";
            this.rbIfPlayer.Size = new System.Drawing.Size(59, 17);
            this.rbIfPlayer.TabIndex = 10;
            this.rbIfPlayer.TabStop = true;
            this.rbIfPlayer.Text = "Players";
            this.rbIfPlayer.UseVisualStyleBackColor = true;
            // 
            // ImagePicker
            // 
            this.ImagePicker.Filter = "PNG Images|*.png";
            this.ImagePicker.Title = "Select Image File";
            // 
            // frmSetEditPvPNEW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1148, 522);
            this.Controls.Add(this.rbIfPlayer);
            this.Controls.Add(this.rbIfCritter);
            this.Controls.Add(this.rbIfAny);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnNoImage);
            this.Controls.Add(this.btnImage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetEditPvPNEW";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PvP Invention Set Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaxLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udMinLevel)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnImage;
        private Button btnNoImage;
        private ImageList ilEnh;
        private GroupBox groupBox1;
        private NumericUpDown udMaxLevel;
        private NumericUpDown udMinLevel;
        private TextBox txtInternal;
        private Label label3;
        private TextBox txtNameShort;
        private Label label2;
        private TextBox txtNameFull;
        private Label label1;
        private Label label4;
        private RichTextBox txtDesc;
        private Label label6;
        private Label label5;
        private ComboBox cbSetType;
        private Label label7;
        private GroupBox groupBox2;
        private ListView lvEnh;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private GroupBox groupBox3;
        private RichTextBox rtbBonus;
        private ListBox lstBonus;
        private ComboBox cbSlotCount;
        private Label label8;
        private ListView lvBonusList;
        private TextBox txtAlternate;
        private TextBox txtBonusFilter;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Button btnCancel;
        private Button btnSave;
        private Label label9;
        private RadioButton rbIfAny;
        private RadioButton rbIfCritter;
        private RadioButton rbIfPlayer;
        private OpenFileDialog ImagePicker;
    }
}