using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    partial class FrmSetEdit
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
            components = new Container();
            btnImage = new Button();
            btnNoImage = new Button();
            ilEnh = new ImageList(components);
            groupBox1 = new GroupBox();
            cbSetType = new ComboBox();
            label7 = new Label();
            txtDesc = new RichTextBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            udMaxLevel = new NumericUpDown();
            udMinLevel = new NumericUpDown();
            txtInternal = new TextBox();
            label3 = new Label();
            txtNameShort = new TextBox();
            label2 = new Label();
            txtNameFull = new TextBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            lvEnh = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            groupBox3 = new GroupBox();
            txtBonusFilter = new TextBox();
            lvBonusList = new ListView();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            txtAlternate = new TextBox();
            lstBonus = new ListBox();
            cbSlotCount = new ComboBox();
            label8 = new Label();
            rtbBonus = new RichTextBox();
            btnCancel = new Button();
            btnSave = new Button();
            label9 = new Label();
            rbIfAny = new RadioButton();
            rbIfCritter = new RadioButton();
            rbIfPlayer = new RadioButton();
            ImagePicker = new OpenFileDialog();
            groupBox1.SuspendLayout();
            ((ISupportInitialize)udMaxLevel).BeginInit();
            ((ISupportInitialize)udMinLevel).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnImage
            // 
            btnImage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            btnImage.Location = new System.Drawing.Point(12, 12);
            btnImage.Name = "btnImage";
            btnImage.Size = new System.Drawing.Size(97, 68);
            btnImage.TabIndex = 0;
            btnImage.Text = "ImageName";
            btnImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            btnImage.UseVisualStyleBackColor = true;
            btnImage.Click += btnImage_Click;
            // 
            // btnNoImage
            // 
            btnNoImage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnNoImage.Location = new System.Drawing.Point(12, 86);
            btnNoImage.Name = "btnNoImage";
            btnNoImage.Size = new System.Drawing.Size(97, 23);
            btnNoImage.TabIndex = 1;
            btnNoImage.Text = "Clear Image";
            btnNoImage.UseVisualStyleBackColor = true;
            btnNoImage.Click += btnNoImage_Click;
            // 
            // ilEnh
            // 
            ilEnh.ColorDepth = ColorDepth.Depth32Bit;
            ilEnh.ImageSize = new System.Drawing.Size(30, 30);
            ilEnh.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbSetType);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtDesc);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(udMaxLevel);
            groupBox1.Controls.Add(udMinLevel);
            groupBox1.Controls.Add(txtInternal);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtNameShort);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtNameFull);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBox1.Location = new System.Drawing.Point(115, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(230, 238);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Info";
            // 
            // cbSetType
            // 
            cbSetType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSetType.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            cbSetType.FormattingEnabled = true;
            cbSetType.Location = new System.Drawing.Point(68, 209);
            cbSetType.Name = "cbSetType";
            cbSetType.Size = new System.Drawing.Size(156, 21);
            cbSetType.TabIndex = 13;
            cbSetType.SelectedIndexChanged += cbSetType_SelectedIndexChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label7.Location = new System.Drawing.Point(13, 212);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(49, 13);
            label7.TabIndex = 12;
            label7.Text = "SetType:";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDesc
            // 
            txtDesc.Location = new System.Drawing.Point(68, 99);
            txtDesc.Name = "txtDesc";
            txtDesc.ScrollBars = RichTextBoxScrollBars.None;
            txtDesc.Size = new System.Drawing.Size(156, 104);
            txtDesc.TabIndex = 11;
            txtDesc.Text = "";
            txtDesc.TextChanged += txtDesc_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label6.Location = new System.Drawing.Point(6, 99);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(69, 13);
            label6.TabIndex = 10;
            label6.Text = "Description:";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(6, 163);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(53, 13);
            label5.TabIndex = 9;
            label5.Text = "MaxLevel";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(6, 123);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(52, 13);
            label4.TabIndex = 8;
            label4.Text = "MinLevel";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // udMaxLevel
            // 
            udMaxLevel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            udMaxLevel.Location = new System.Drawing.Point(13, 180);
            udMaxLevel.Name = "udMaxLevel";
            udMaxLevel.Size = new System.Drawing.Size(32, 22);
            udMaxLevel.TabIndex = 7;
            udMaxLevel.ValueChanged += udMaxLevel_ValueChanged;
            udMaxLevel.Leave += udMaxLevel_Leave;
            // 
            // udMinLevel
            // 
            udMinLevel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            udMinLevel.Location = new System.Drawing.Point(13, 140);
            udMinLevel.Name = "udMinLevel";
            udMinLevel.Size = new System.Drawing.Size(32, 22);
            udMinLevel.TabIndex = 6;
            udMinLevel.ValueChanged += udMinLevel_ValueChanged;
            udMinLevel.Leave += udMinLevel_Leave;
            // 
            // txtInternal
            // 
            txtInternal.Location = new System.Drawing.Point(68, 73);
            txtInternal.Name = "txtInternal";
            txtInternal.Size = new System.Drawing.Size(156, 22);
            txtInternal.TabIndex = 5;
            txtInternal.TextChanged += txtInternal_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(25, 76);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(50, 13);
            label3.TabIndex = 4;
            label3.Text = "Internal:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameShort
            // 
            txtNameShort.Location = new System.Drawing.Point(69, 47);
            txtNameShort.Name = "txtNameShort";
            txtNameShort.Size = new System.Drawing.Size(155, 22);
            txtNameShort.TabIndex = 3;
            txtNameShort.TextChanged += txtNameShort_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(4, 50);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(70, 13);
            label2.TabIndex = 2;
            label2.Text = "Short Name:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNameFull
            // 
            txtNameFull.Location = new System.Drawing.Point(69, 21);
            txtNameFull.Name = "txtNameFull";
            txtNameFull.Size = new System.Drawing.Size(156, 22);
            txtNameFull.TabIndex = 1;
            txtNameFull.TextChanged += txtNameFull_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(14, 24);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(61, 13);
            label1.TabIndex = 0;
            label1.Text = "Full Name:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lvEnh);
            groupBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBox2.Location = new System.Drawing.Point(12, 256);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(333, 229);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Enhancements belonging to this set:";
            // 
            // lvEnh
            // 
            lvEnh.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            lvEnh.FullRowSelect = true;
            lvEnh.LargeImageList = ilEnh;
            lvEnh.Location = new System.Drawing.Point(6, 19);
            lvEnh.Name = "lvEnh";
            lvEnh.Size = new System.Drawing.Size(321, 204);
            lvEnh.SmallImageList = ilEnh;
            lvEnh.TabIndex = 0;
            lvEnh.UseCompatibleStateImageBehavior = false;
            lvEnh.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 244;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Classes";
            columnHeader2.Width = 71;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtBonusFilter);
            groupBox3.Controls.Add(lvBonusList);
            groupBox3.Controls.Add(txtAlternate);
            groupBox3.Controls.Add(lstBonus);
            groupBox3.Controls.Add(cbSlotCount);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(rtbBonus);
            groupBox3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBox3.Location = new System.Drawing.Point(351, 12);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(785, 473);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Set Bonuses";
            // 
            // txtBonusFilter
            // 
            txtBonusFilter.Location = new System.Drawing.Point(318, 447);
            txtBonusFilter.Name = "txtBonusFilter";
            txtBonusFilter.Size = new System.Drawing.Size(428, 22);
            txtBonusFilter.TabIndex = 6;
            txtBonusFilter.TextChanged += TxtBonusFilter_TextChanged;
            // 
            // lvBonusList
            // 
            lvBonusList.Columns.AddRange(new ColumnHeader[] { columnHeader3, columnHeader4 });
            lvBonusList.Location = new System.Drawing.Point(281, 20);
            lvBonusList.MultiSelect = false;
            lvBonusList.Name = "lvBonusList";
            lvBonusList.Size = new System.Drawing.Size(498, 421);
            lvBonusList.TabIndex = 5;
            lvBonusList.UseCompatibleStateImageBehavior = false;
            lvBonusList.View = View.Details;
            lvBonusList.DoubleClick += lvBonusList_DoubleClick;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Bonus";
            columnHeader3.Width = 212;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Effect";
            columnHeader4.Width = 253;
            // 
            // txtAlternate
            // 
            txtAlternate.Location = new System.Drawing.Point(10, 447);
            txtAlternate.Name = "txtAlternate";
            txtAlternate.Size = new System.Drawing.Size(263, 22);
            txtAlternate.TabIndex = 4;
            txtAlternate.TextChanged += txtAlternate_TextChanged;
            // 
            // lstBonus
            // 
            lstBonus.FormattingEnabled = true;
            lstBonus.Location = new System.Drawing.Point(10, 337);
            lstBonus.Name = "lstBonus";
            lstBonus.Size = new System.Drawing.Size(263, 95);
            lstBonus.TabIndex = 3;
            lstBonus.DoubleClick += lstBonus_DoubleClick;
            // 
            // cbSlotCount
            // 
            cbSlotCount.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSlotCount.FormattingEnabled = true;
            cbSlotCount.Items.AddRange(new object[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" });
            cbSlotCount.Location = new System.Drawing.Point(108, 309);
            cbSlotCount.Name = "cbSlotCount";
            cbSlotCount.Size = new System.Drawing.Size(165, 21);
            cbSlotCount.TabIndex = 2;
            cbSlotCount.SelectedIndexChanged += cbSlotX_SelectedIndexChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(7, 312);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(103, 13);
            label8.TabIndex = 1;
            label8.Text = "Bonus for slotting:";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rtbBonus
            // 
            rtbBonus.BackColor = System.Drawing.Color.Black;
            rtbBonus.Location = new System.Drawing.Point(7, 20);
            rtbBonus.Name = "rtbBonus";
            rtbBonus.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbBonus.Size = new System.Drawing.Size(267, 283);
            rtbBonus.TabIndex = 0;
            rtbBonus.Text = "";
            // 
            // btnCancel
            // 
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCancel.Location = new System.Drawing.Point(980, 491);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnSave.Location = new System.Drawing.Point(1061, 491);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(376, 496);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(60, 15);
            label9.TabIndex = 7;
            label9.Text = "If Target =";
            // 
            // rbIfAny
            // 
            rbIfAny.AutoSize = true;
            rbIfAny.Checked = true;
            rbIfAny.Location = new System.Drawing.Point(438, 494);
            rbIfAny.Name = "rbIfAny";
            rbIfAny.Size = new System.Drawing.Size(46, 19);
            rbIfAny.TabIndex = 8;
            rbIfAny.TabStop = true;
            rbIfAny.Text = "Any";
            rbIfAny.UseVisualStyleBackColor = true;
            // 
            // rbIfCritter
            // 
            rbIfCritter.AutoSize = true;
            rbIfCritter.Location = new System.Drawing.Point(487, 494);
            rbIfCritter.Name = "rbIfCritter";
            rbIfCritter.Size = new System.Drawing.Size(63, 19);
            rbIfCritter.TabIndex = 9;
            rbIfCritter.TabStop = true;
            rbIfCritter.Text = "Critters";
            rbIfCritter.UseVisualStyleBackColor = true;
            // 
            // rbIfPlayer
            // 
            rbIfPlayer.AutoSize = true;
            rbIfPlayer.Location = new System.Drawing.Point(550, 494);
            rbIfPlayer.Name = "rbIfPlayer";
            rbIfPlayer.Size = new System.Drawing.Size(62, 19);
            rbIfPlayer.TabIndex = 10;
            rbIfPlayer.TabStop = true;
            rbIfPlayer.Text = "Players";
            rbIfPlayer.UseVisualStyleBackColor = true;
            // 
            // ImagePicker
            // 
            ImagePicker.Filter = "PNG Images|*.png";
            ImagePicker.Title = "Select Image File";
            // 
            // FrmSetEdit
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1148, 522);
            Controls.Add(rbIfPlayer);
            Controls.Add(rbIfCritter);
            Controls.Add(rbIfAny);
            Controls.Add(label9);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnNoImage);
            Controls.Add(btnImage);
            Name = "FrmSetEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PvE Invention Set Editor";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((ISupportInitialize)udMaxLevel).EndInit();
            ((ISupportInitialize)udMinLevel).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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