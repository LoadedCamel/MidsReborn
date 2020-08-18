using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSetEditPvP
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
            this.components = (System.ComponentModel.IContainer) new System.ComponentModel.Container();
            this.btnNoImage = new System.Windows.Forms.Button();
            this.gbBasic = new System.Windows.Forms.GroupBox();
            this.txtInternal = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.cbSetType = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.udMinLevel = new System.Windows.Forms.NumericUpDown();
            this.udMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtNameShort = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtNameFull = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.btnImage = new System.Windows.Forms.Button();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.lvEnh = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ilEnh = new System.Windows.Forms.ImageList(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.ImagePicker = new System.Windows.Forms.OpenFileDialog();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.txtAlternate = new System.Windows.Forms.TextBox();
            this.lvBonusList = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.lstBonus = new System.Windows.Forms.ListBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.cbSlotCount = new System.Windows.Forms.ComboBox();
            this.rtbBonus = new System.Windows.Forms.RichTextBox();
            this.Label27 = new System.Windows.Forms.Label();
            this.rbIfPlayer = new System.Windows.Forms.RadioButton();
            this.rbIfCritter = new System.Windows.Forms.RadioButton();
            this.rbIfAny = new System.Windows.Forms.RadioButton();
            this.gbBasic.SuspendLayout();
            this.udMinLevel.BeginInit();
            this.udMaxLevel.BeginInit();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();

            this.btnNoImage.Location = new System.Drawing.Point(8, 84);
            this.btnNoImage.Name = "btnNoImage";

            this.btnNoImage.Size = new System.Drawing.Size(80, 20);
            this.btnNoImage.TabIndex = 2;
            this.btnNoImage.Text = "Clear Image";
            this.btnNoImage.Click += new System.EventHandler(btnNoImage_Click);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.txtInternal);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label5);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.cbSetType);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label1);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label7);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label6);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.udMinLevel);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.udMaxLevel);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.txtDesc);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label4);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.txtNameShort);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label3);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.txtNameFull);
            this.gbBasic.Controls.Add((System.Windows.Forms.Control) this.Label2);

            this.gbBasic.Location = new System.Drawing.Point(96, 8);
            this.gbBasic.Name = "gbBasic";

            this.gbBasic.Size = new System.Drawing.Size(248, 202);
            this.gbBasic.TabIndex = 0;
            this.gbBasic.TabStop = false;
            this.gbBasic.Text = "Basic:";

            this.txtInternal.Location = new System.Drawing.Point(84, 66);
            this.txtInternal.Name = "txtInternal";

            this.txtInternal.Size = new System.Drawing.Size(156, 20);
            this.txtInternal.TabIndex = 21;
            this.txtInternal.TextChanged += new System.EventHandler(txtInternal_TextChanged);

            this.Label5.Location = new System.Drawing.Point(8, 66);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(72, 20);
            this.Label5.TabIndex = 22;
            this.Label5.Text = "Internal:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbSetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbSetType.Location = new System.Drawing.Point(84, 174);
            this.cbSetType.Name = "cbSetType";

            this.cbSetType.Size = new System.Drawing.Size(156, 22);
            this.cbSetType.TabIndex = 3;
            this.cbSetType.SelectedIndexChanged += new System.EventHandler(cbSetType_SelectedIndexChanged);

            this.Label1.Location = new System.Drawing.Point(8, 174);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(72, 20);
            this.Label1.TabIndex = 20;
            this.Label1.Text = "SetType:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label7.Location = new System.Drawing.Point(8, 150);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(52, 20);
            this.Label7.TabIndex = 19;
            this.Label7.Text = "MaxLev:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label6.Location = new System.Drawing.Point(8, 126);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(52, 20);
            this.Label6.TabIndex = 18;
            this.Label6.Text = "MinLev:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.udMinLevel.Location = new System.Drawing.Point(64, 126);
            System.Decimal num = new System.Decimal(new int[4]
            {
                53, 0, 0, 0
            });
            this.udMinLevel.Maximum = num;
            num = new System.Decimal(new int[4]
            {
                1, 0, 0, 0
            });
            this.udMinLevel.Minimum = num;
            this.udMinLevel.Name = "udMinLevel";

            this.udMinLevel.Size = new System.Drawing.Size(36, 20);
            this.udMinLevel.TabIndex = 4;
            num = new System.Decimal(new int[4]
            {
                1, 0, 0, 0
            });
            this.udMinLevel.Value = num;

            this.udMaxLevel.Location = new System.Drawing.Point(64, 150);
            num = new System.Decimal(new int[4]
            {
                53, 0, 0, 0
            });
            this.udMaxLevel.Maximum = num;
            num = new System.Decimal(new int[4]
            {
                1, 0, 0, 0
            });
            this.udMaxLevel.Minimum = num;
            this.udMaxLevel.Name = "udMaxLevel";

            this.udMaxLevel.Size = new System.Drawing.Size(36, 20);
            this.udMaxLevel.TabIndex = 5;
            num = new System.Decimal(new int[4]
            {
                53, 0, 0, 0
            });
            this.udMaxLevel.Value = num;

            this.txtDesc.Location = new System.Drawing.Point(100, 102);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";

            this.txtDesc.Size = new System.Drawing.Size(140, 68);
            this.txtDesc.TabIndex = 2;
            this.txtDesc.TextChanged += new System.EventHandler(txtDesc_TextChanged);

            this.Label4.Location = new System.Drawing.Point(8, 102);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(88, 20);
            this.Label4.TabIndex = 14;
            this.Label4.Text = "Description:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNameShort.Location = new System.Drawing.Point(84, 40);
            this.txtNameShort.Name = "txtNameShort";

            this.txtNameShort.Size = new System.Drawing.Size(156, 20);
            this.txtNameShort.TabIndex = 1;
            this.txtNameShort.TextChanged += new System.EventHandler(txtNameShort_TextChanged);

            this.Label3.Location = new System.Drawing.Point(8, 40);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(72, 20);
            this.Label3.TabIndex = 12;
            this.Label3.Text = "Short Name:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtNameFull.Location = new System.Drawing.Point(84, 16);
            this.txtNameFull.Name = "txtNameFull";

            this.txtNameFull.Size = new System.Drawing.Size(156, 20);
            this.txtNameFull.TabIndex = 0;
            this.txtNameFull.TextChanged += new System.EventHandler(txtNameFull_TextChanged);

            this.Label2.Location = new System.Drawing.Point(8, 16);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(72, 20);
            this.Label2.TabIndex = 10;
            this.Label2.Text = "Full Name:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;

            this.btnImage.Location = new System.Drawing.Point(8, 12);
            this.btnImage.Name = "btnImage";

            this.btnImage.Size = new System.Drawing.Size(80, 68);
            this.btnImage.TabIndex = 1;
            this.btnImage.Text = "ImageName";
            this.btnImage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnImage.Click += new System.EventHandler(btnImage_Click);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control) this.lvEnh);

            this.GroupBox2.Location = new System.Drawing.Point(6, 216);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(336, 224);
            this.GroupBox2.TabIndex = 24;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Enhancements belonging to this set:";
            this.lvEnh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2]
            {
                this.ColumnHeader1, this.ColumnHeader2
            });
            this.lvEnh.FullRowSelect = true;
            this.lvEnh.LargeImageList = this.ilEnh;

            this.lvEnh.Location = new System.Drawing.Point(8, 20);
            this.lvEnh.Name = "lvEnh";

            this.lvEnh.Size = new System.Drawing.Size(320, 196);
            this.lvEnh.SmallImageList = this.ilEnh;
            this.lvEnh.TabIndex = 0;
            this.lvEnh.UseCompatibleStateImageBehavior = false;
            this.lvEnh.View = System.Windows.Forms.View.Details;
            this.ColumnHeader1.Text = "Name";
            this.ColumnHeader1.Width = 203;
            this.ColumnHeader2.Text = "Classes";
            this.ColumnHeader2.Width = 91;
            this.ilEnh.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;

            this.ilEnh.ImageSize = new System.Drawing.Size(30, 30);
            this.ilEnh.TransparentColor = System.Drawing.Color.Transparent;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(160, 446);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(84, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnOK.Location = new System.Drawing.Point(252, 446);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(84, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.ImagePicker.Filter = "PNG Images|*.png";
            this.ImagePicker.Title = "Select Image File";
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.btnPaste);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.txtAlternate);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.lvBonusList);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.lstBonus);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.Label16);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.cbSlotCount);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control) this.rtbBonus);

            this.GroupBox3.Location = new System.Drawing.Point(348, 8);
            this.GroupBox3.Name = "GroupBox3";

            this.GroupBox3.Size = new System.Drawing.Size(629, 432);
            this.GroupBox3.TabIndex = 25;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Set Bonuses:";

            this.btnPaste.Location = new System.Drawing.Point(8, 280);
            this.btnPaste.Name = "btnPaste";

            this.btnPaste.Size = new System.Drawing.Size(96, 23);
            this.btnPaste.TabIndex = 19;
            this.btnPaste.Text = "Fill by Paste";
            this.btnPaste.Visible = false;
            this.btnPaste.Click += new System.EventHandler(btnPaste_Click);

            this.txtAlternate.Location = new System.Drawing.Point(8, 406);
            this.txtAlternate.Name = "txtAlternate";

            this.txtAlternate.Size = new System.Drawing.Size(270, 20);
            this.txtAlternate.TabIndex = 18;
            this.txtAlternate.TextChanged += new System.EventHandler(txtAlternate_TextChanged);
            this.lvBonusList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2]
            {
                this.ColumnHeader3, this.ColumnHeader4
            });
            this.lvBonusList.FullRowSelect = true;
            this.lvBonusList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvBonusList.HideSelection = false;

            this.lvBonusList.Location = new System.Drawing.Point(284, 19);
            this.lvBonusList.MultiSelect = false;
            this.lvBonusList.Name = "lvBonusList";

            this.lvBonusList.Size = new System.Drawing.Size(339, 407);
            this.lvBonusList.TabIndex = 17;
            this.lvBonusList.UseCompatibleStateImageBehavior = false;
            this.lvBonusList.View = System.Windows.Forms.View.Details;
            this.ColumnHeader3.Text = "Bonus";
            this.ColumnHeader3.Width = 185;
            this.ColumnHeader4.Text = "Effect";
            this.ColumnHeader4.Width = 110;
            this.lstBonus.ItemHeight = 14;

            this.lstBonus.Location = new System.Drawing.Point(8, 309);
            this.lstBonus.Name = "lstBonus";

            this.lstBonus.Size = new System.Drawing.Size(271, 88);
            this.lstBonus.TabIndex = 16;
            this.lstBonus.DoubleClick += new System.EventHandler(lstBonus_DoubleClick);

            this.Label16.Location = new System.Drawing.Point(8, 281);
            this.Label16.Name = "Label16";

            this.Label16.Size = new System.Drawing.Size(96, 20);
            this.Label16.TabIndex = 15;
            this.Label16.Text = "Bonus for slotting:";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cbSlotCount.Items.AddRange(new object[10]
                {
                    "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"
                }
            );

            this.cbSlotCount.Location = new System.Drawing.Point(110, 281);
            this.cbSlotCount.Name = "cbSlotCount";

            this.cbSlotCount.Size = new System.Drawing.Size(170, 22);
            this.cbSlotCount.TabIndex = 3;
            this.cbSlotCount.SelectedIndexChanged += new System.EventHandler(cbSlotX_SelectedIndexChanged);
            this.rtbBonus.BackColor = System.Drawing.Color.White;
            this.rtbBonus.ForeColor = System.Drawing.Color.Black;

            this.rtbBonus.Location = new System.Drawing.Point(6, 19);
            this.rtbBonus.Name = "rtbBonus";
            this.rtbBonus.ReadOnly = true;
            this.rtbBonus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.rtbBonus.Size = new System.Drawing.Size(272, 256);
            this.rtbBonus.TabIndex = 1;
            this.rtbBonus.Text = "";

            this.Label27.Location = new System.Drawing.Point(351, 450);
            this.Label27.Name = "Label27";

            this.Label27.Size = new System.Drawing.Size(76, 20);
            this.Label27.TabIndex = 106;
            this.Label27.Text = "If Target =";
            this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.rbIfPlayer.Location = new System.Drawing.Point(563, 451);
            this.rbIfPlayer.Name = "rbIfPlayer";

            this.rbIfPlayer.Size = new System.Drawing.Size(76, 20);
            this.rbIfPlayer.TabIndex = 105;
            this.rbIfPlayer.Text = "Players";

            this.rbIfCritter.Location = new System.Drawing.Point(486, 451);
            this.rbIfCritter.Name = "rbIfCritter";

            this.rbIfCritter.Size = new System.Drawing.Size(71, 20);
            this.rbIfCritter.TabIndex = 104;
            this.rbIfCritter.Text = "Critters";
            this.rbIfAny.Checked = true;

            this.rbIfAny.Location = new System.Drawing.Point(433, 451);
            this.rbIfAny.Name = "rbIfAny";

            this.rbIfAny.Size = new System.Drawing.Size(57, 20);
            this.rbIfAny.TabIndex = 103;
            this.rbIfAny.TabStop = true;
            this.rbIfAny.Text = "Any";
            this.AcceptButton = (System.Windows.Forms.IButtonControl) this.btnOK;

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = (System.Windows.Forms.IButtonControl) this.btnCancel;

            this.ClientSize = new System.Drawing.Size(989, 482);
            this.Controls.Add((System.Windows.Forms.Control) this.Label27);
            this.Controls.Add((System.Windows.Forms.Control) this.rbIfPlayer);
            this.Controls.Add((System.Windows.Forms.Control) this.rbIfCritter);
            this.Controls.Add((System.Windows.Forms.Control) this.rbIfAny);
            this.Controls.Add((System.Windows.Forms.Control) this.GroupBox3);
            this.Controls.Add((System.Windows.Forms.Control) this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control) this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control) this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control) this.btnNoImage);
            this.Controls.Add((System.Windows.Forms.Control) this.gbBasic);
            this.Controls.Add((System.Windows.Forms.Control) this.btnImage);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte) 0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PvP Invention Set Editor";
            this.gbBasic.ResumeLayout(false);
            this.gbBasic.PerformLayout();
            this.udMinLevel.EndInit();
            this.udMaxLevel.EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        Button btnCancel;
        Button btnImage;
        Button btnNoImage;
        Button btnOK;
        Button btnPaste;
        ComboBox cbSetType;
        ComboBox cbSlotCount;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        GroupBox gbBasic;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        ImageList ilEnh;
        OpenFileDialog ImagePicker;
        Label Label1;
        Label Label16;
        Label Label2;
        Label Label27;
        Label Label3;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        ListBox lstBonus;
        ListView _lvBonusList;
        ListView lvEnh;
        RadioButton rbIfAny;
        RadioButton rbIfCritter;
        RadioButton rbIfPlayer;
        RichTextBox rtbBonus;
        TextBox txtAlternate;
        TextBox txtDesc;
        TextBox txtInternal;
        TextBox txtNameFull;
        TextBox txtNameShort;
        NumericUpDown _udMaxLevel;
        NumericUpDown _udMinLevel;
    }
}