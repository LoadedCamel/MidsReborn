using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmRecipeViewer
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
            this.pbRecipe = new System.Windows.Forms.PictureBox();
            this.lvPower = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvDPA = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilSets = new System.Windows.Forms.ImageList(this.components);
            this.chkSortByLevel = new System.Windows.Forms.CheckBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.VScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.RecipeInfo = new midsControls.ctlPopUp();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.chkRecipe = new System.Windows.Forms.CheckBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.ibMiniList = new midsControls.ImageButton();
            this.ibClipboard = new midsControls.ImageButton();
            this.ibTopmost = new midsControls.ImageButton();
            this.ibClose = new midsControls.ImageButton();
            this.ibEnhCheckMode = new midsControls.ImageButton();
            this.pSalvageSummary = new System.Windows.Forms.Panel();
            this.lblBoosters = new System.Windows.Forms.Label();
            this.lblCatalysts = new System.Windows.Forms.Label();
            this.lblEnhObtained = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.vScrollBar2 = new System.Windows.Forms.VScrollBar();
            this.ctlPopUp1 = new midsControls.ctlPopUp();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblCatalysts = new System.Windows.Forms.Label();
            this.lblRewardMerits = new System.Windows.Forms.Label();
            this.imageButton1 = new midsControls.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecipe)).BeginInit();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.pSalvageSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbRecipe
            // 
            this.pbRecipe.Location = new System.Drawing.Point(0, 0);
            this.pbRecipe.Name = "pbRecipe";
            this.pbRecipe.Size = new System.Drawing.Size(60, 30);
            this.pbRecipe.TabIndex = 0;
            this.pbRecipe.TabStop = false;
            // 
            // lvPower
            // 
            this.lvPower.CheckBoxes = true;
            this.lvPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1});
            this.lvPower.FullRowSelect = true;
            this.lvPower.HideSelection = false;
            this.lvPower.Location = new System.Drawing.Point(12, 12);
            this.lvPower.MultiSelect = false;
            this.lvPower.Name = "lvPower";
            this.lvPower.Size = new System.Drawing.Size(176, 191);
            this.lvPower.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvPower.TabIndex = 1;
            this.lvPower.UseCompatibleStateImageBehavior = false;
            this.lvPower.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Power";
            this.ColumnHeader1.Width = 148;
            // 
            // lvDPA
            // 
            this.lvDPA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader3,
            this.ColumnHeader4,
            this.ColumnHeader5});
            this.lvDPA.FullRowSelect = true;
            this.lvDPA.HideSelection = false;
            this.lvDPA.Location = new System.Drawing.Point(194, 12);
            this.lvDPA.MultiSelect = false;
            this.lvDPA.Name = "lvDPA";
            this.lvDPA.Size = new System.Drawing.Size(516, 191);
            this.lvDPA.SmallImageList = this.ilSets;
            this.lvDPA.TabIndex = 8;
            this.lvDPA.UseCompatibleStateImageBehavior = false;
            this.lvDPA.View = System.Windows.Forms.View.Details;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Enhancement";
            this.ColumnHeader3.Width = 241;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Text = "Level";
            this.ColumnHeader4.Width = 45;
            // 
            // ColumnHeader5
            // 
            this.ColumnHeader5.Text = "Power";
            this.ColumnHeader5.Width = 114;
            // 
            // ilSets
            // 
            this.ilSets.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilSets.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSets.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // chkSortByLevel
            // 
            this.chkSortByLevel.Checked = true;
            this.chkSortByLevel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortByLevel.ForeColor = System.Drawing.Color.White;
            this.chkSortByLevel.Location = new System.Drawing.Point(12, 217);
            this.chkSortByLevel.Name = "chkSortByLevel";
            this.chkSortByLevel.Size = new System.Drawing.Size(176, 16);
            this.chkSortByLevel.TabIndex = 9;
            this.chkSortByLevel.Text = "Sort By Level";
            this.chkSortByLevel.UseVisualStyleBackColor = true;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Arial", 17.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblHeader.Location = new System.Drawing.Point(66, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(632, 30);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "Select A Power / Recipe";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.VScrollBar1);
            this.Panel1.Controls.Add(this.RecipeInfo);
            this.Panel1.Location = new System.Drawing.Point(0, 38);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(698, 175);
            this.Panel1.TabIndex = 11;
            // 
            // VScrollBar1
            // 
            this.VScrollBar1.Location = new System.Drawing.Point(679, 0);
            this.VScrollBar1.Maximum = 20;
            this.VScrollBar1.Name = "VScrollBar1";
            this.VScrollBar1.Size = new System.Drawing.Size(17, 175);
            this.VScrollBar1.TabIndex = 3;
            // 
            // RecipeInfo
            // 
            this.RecipeInfo.BXHeight = 2048;
            this.RecipeInfo.ColumnPosition = 0.5F;
            this.RecipeInfo.ColumnRight = false;
            this.RecipeInfo.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.RecipeInfo.ForeColor = System.Drawing.Color.Black;
            this.RecipeInfo.InternalPadding = 3;
            this.RecipeInfo.Location = new System.Drawing.Point(3, -17);
            this.RecipeInfo.Name = "RecipeInfo";
            this.RecipeInfo.ScrollY = 0F;
            this.RecipeInfo.SectionPadding = 8;
            this.RecipeInfo.Size = new System.Drawing.Size(692, 175);
            this.RecipeInfo.TabIndex = 2;
            // 
            // Panel2
            // 
            this.Panel2.BackColor = System.Drawing.Color.Black;
            this.Panel2.Controls.Add(this.Panel1);
            this.Panel2.Controls.Add(this.pbRecipe);
            this.Panel2.Controls.Add(this.lblHeader);
            this.Panel2.Location = new System.Drawing.Point(12, 244);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(698, 213);
            this.Panel2.TabIndex = 12;
            // 
            // chkRecipe
            // 
            this.chkRecipe.ForeColor = System.Drawing.Color.White;
            this.chkRecipe.Location = new System.Drawing.Point(234, 470);
            this.chkRecipe.Name = "chkRecipe";
            this.chkRecipe.Size = new System.Drawing.Size(143, 22);
            this.chkRecipe.TabIndex = 15;
            this.chkRecipe.Text = "Include Recipes";
            this.ToolTip1.SetToolTip(this.chkRecipe, "Add a list of recipes to the shopping list");
            this.chkRecipe.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Hero_Designer.Resources.AncientScroll;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.ToolTip1.SetToolTip(this.pictureBox1, "Enhancements obtained so far");
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Hero_Designer.Resources.EnhancementCatalyst;
            this.pictureBox2.Location = new System.Drawing.Point(222, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.ToolTip1.SetToolTip(this.pictureBox2, "Enhancements Catalysts");
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Hero_Designer.Resources.EnhancementBooster;
            this.pictureBox3.Location = new System.Drawing.Point(370, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            this.ToolTip1.SetToolTip(this.pictureBox3, "Enhancements Catalysts");
            // 
            // ibMiniList
            // 
            this.ibMiniList.Checked = false;
            this.ibMiniList.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibMiniList.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibMiniList.Location = new System.Drawing.Point(123, 470);
            this.ibMiniList.Name = "ibMiniList";
            this.ibMiniList.Size = new System.Drawing.Size(105, 22);
            this.ibMiniList.TabIndex = 14;
            this.ibMiniList.TextOff = "To I-G Helper";
            this.ibMiniList.TextOn = "Alt Text";
            this.ibMiniList.Toggle = false;
            // 
            // ibClipboard
            // 
            this.ibClipboard.Checked = false;
            this.ibClipboard.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClipboard.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClipboard.Location = new System.Drawing.Point(12, 470);
            this.ibClipboard.Name = "ibClipboard";
            this.ibClipboard.Size = new System.Drawing.Size(105, 22);
            this.ibClipboard.TabIndex = 13;
            this.ibClipboard.TextOff = "To Clipboard";
            this.ibClipboard.TextOn = "Alt Text";
            this.ibClipboard.Toggle = false;
            // 
            // ibTopmost
            // 
            this.ibTopmost.Checked = true;
            this.ibTopmost.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibTopmost.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibTopmost.Location = new System.Drawing.Point(383, 470);
            this.ibTopmost.Name = "ibTopmost";
            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 7;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
            // 
            // ibClose
            // 
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(605, 470);
            this.ibClose.Name = "ibClose";
            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 6;
            this.ibClose.TextOff = "Close";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            // 
            // ibEnhCheckMode
            // 
            this.ibEnhCheckMode.Checked = false;
            this.ibEnhCheckMode.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibEnhCheckMode.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibEnhCheckMode.Location = new System.Drawing.Point(494, 470);
            this.ibEnhCheckMode.Name = "ibEnhCheckMode";
            this.ibEnhCheckMode.Size = new System.Drawing.Size(105, 22);
            this.ibEnhCheckMode.TabIndex = 16;
            this.ibEnhCheckMode.TextOff = "Enh. check mode";
            this.ibEnhCheckMode.TextOn = "Enh. check mode";
            this.ibEnhCheckMode.Toggle = false;
            // 
            // pSalvageSummary
            // 
            this.pSalvageSummary.BackColor = System.Drawing.Color.Transparent;
            this.pSalvageSummary.Controls.Add(this.lblBoosters);
            this.pSalvageSummary.Controls.Add(this.pictureBox3);
            this.pSalvageSummary.Controls.Add(this.lblCatalysts);
            this.pSalvageSummary.Controls.Add(this.pictureBox2);
            this.pSalvageSummary.Controls.Add(this.lblEnhObtained);
            this.pSalvageSummary.Controls.Add(this.pictureBox1);
            this.pSalvageSummary.Location = new System.Drawing.Point(194, 210);
            this.pSalvageSummary.Name = "pSalvageSummary";
            this.pSalvageSummary.Size = new System.Drawing.Size(516, 32);
            this.pSalvageSummary.TabIndex = 17;
            // 
            // lblBoosters
            // 
            this.lblBoosters.AutoSize = true;
            this.lblBoosters.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblBoosters.ForeColor = System.Drawing.Color.White;
            this.lblBoosters.Location = new System.Drawing.Point(407, 8);
            this.lblBoosters.Name = "lblBoosters";
            this.lblBoosters.Size = new System.Drawing.Size(26, 15);
            this.lblBoosters.TabIndex = 5;
            this.lblBoosters.Text = "x50";
            // 
            // lblCatalysts
            // 
            this.lblCatalysts.AutoSize = true;
            this.lblCatalysts.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCatalysts.ForeColor = System.Drawing.Color.White;
            this.lblCatalysts.Location = new System.Drawing.Point(259, 8);
            this.lblCatalysts.Name = "lblCatalysts";
            this.lblCatalysts.Size = new System.Drawing.Size(26, 15);
            this.lblCatalysts.TabIndex = 3;
            this.lblCatalysts.Text = "x50";
            // 
            // lblEnhObtained
            // 
            this.lblEnhObtained.AutoSize = true;
            this.lblEnhObtained.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnhObtained.ForeColor = System.Drawing.Color.White;
            this.lblEnhObtained.Location = new System.Drawing.Point(36, 8);
            this.lblEnhObtained.Name = "lblEnhObtained";
            this.lblEnhObtained.Size = new System.Drawing.Size(109, 15);
            this.lblEnhObtained.TabIndex = 1;
            this.lblEnhObtained.Text = "Obtained: 100/100";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.pictureBox4);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(12, 244);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(698, 213);
            this.panel3.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.vScrollBar2);
            this.panel4.Controls.Add(this.ctlPopUp1);
            this.panel4.Location = new System.Drawing.Point(0, 38);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(698, 175);
            this.panel4.TabIndex = 11;
            // 
            // vScrollBar2
            // 
            this.vScrollBar2.Location = new System.Drawing.Point(679, 0);
            this.vScrollBar2.Maximum = 20;
            this.vScrollBar2.Name = "vScrollBar2";
            this.vScrollBar2.Size = new System.Drawing.Size(17, 175);
            this.vScrollBar2.TabIndex = 3;
            // 
            // ctlPopUp1
            // 
            this.ctlPopUp1.BXHeight = 2048;
            this.ctlPopUp1.ColumnPosition = 0.5F;
            this.ctlPopUp1.ColumnRight = false;
            this.ctlPopUp1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ctlPopUp1.ForeColor = System.Drawing.Color.Black;
            this.ctlPopUp1.InternalPadding = 3;
            this.ctlPopUp1.Location = new System.Drawing.Point(3, -17);
            this.ctlPopUp1.Name = "ctlPopUp1";
            this.ctlPopUp1.ScrollY = 0F;
            this.ctlPopUp1.SectionPadding = 8;
            this.ctlPopUp1.Size = new System.Drawing.Size(692, 175);
            this.ctlPopUp1.TabIndex = 2;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(0, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(60, 30);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 17.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(66, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(632, 30);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select A Power / Recipe";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox1
            // 
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(12, 217);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(176, 16);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Sort By Level";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.Controls.Add(this.lblCatalysts);
            this.panel5.Controls.Add(this.pictureBox5);
            this.panel5.Controls.Add(this.lblRewardMerits);
            this.panel5.Controls.Add(this.pictureBox6);
            this.panel5.Location = new System.Drawing.Point(194, 538);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(372, 32);
            this.panel5.TabIndex = 18;
            // 
            // lblCatalysts
            // 
            this.lblCatalysts.AutoSize = true;
            this.lblCatalysts.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCatalysts.ForeColor = System.Drawing.Color.White;
            this.lblCatalysts.Location = new System.Drawing.Point(259, 8);
            this.lblCatalysts.Name = "lblCatalysts";
            this.lblCatalysts.Size = new System.Drawing.Size(26, 15);
            this.lblCatalysts.TabIndex = 5;
            this.lblCatalysts.Text = "x50";
            // 
            // lblRewardMerits
            // 
            this.lblRewardMerits.AutoSize = true;
            this.lblRewardMerits.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblRewardMerits.ForeColor = System.Drawing.Color.White;
            this.lblRewardMerits.Location = new System.Drawing.Point(37, 8);
            this.lblRewardMerits.Name = "lblRewardMerits";
            this.lblRewardMerits.Size = new System.Drawing.Size(26, 15);
            this.lblRewardMerits.TabIndex = 3;
            this.lblRewardMerits.Text = "x50";
            // 
            // imageButton1
            // 
            this.imageButton1.Checked = false;
            this.imageButton1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.imageButton1.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.imageButton1.Location = new System.Drawing.Point(374, 470);
            this.imageButton1.Name = "imageButton1";
            this.imageButton1.Size = new System.Drawing.Size(105, 22);
            this.imageButton1.TabIndex = 19;
            this.imageButton1.TextOff = "Shopping List >>";
            this.imageButton1.TextOn = "<< Shopping List";
            this.imageButton1.Toggle = true;
            // 
            // frmRecipeViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(722, 504);
            this.Controls.Add(this.pSalvageSummary);
            this.Controls.Add(this.ibEnhCheckMode);
            this.Controls.Add(this.chkRecipe);
            this.Controls.Add(this.ibMiniList);
            this.Controls.Add(this.ibClipboard);
            this.Controls.Add(this.chkSortByLevel);
            this.Controls.Add(this.lvDPA);
            this.Controls.Add(this.ibTopmost);
            this.Controls.Add(this.ibClose);
            this.Controls.Add(this.lvPower);
            this.Controls.Add(this.Panel2);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmRecipeViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Recipe Viewer";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbRecipe)).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.pSalvageSummary.ResumeLayout(false);
            this.pSalvageSummary.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        CheckBox chkRecipe;
        CheckBox chkSortByLevel;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ImageList ilSets;
        Label lblHeader;
        ListView lvPower;
        ListView lvDPA;
        Panel Panel1;
        Panel Panel2;
        PictureBox pbRecipe;
        ToolTip ToolTip1;
        VScrollBar VScrollBar1;
        private midsControls.ImageButton ibEnhCheckMode;
        private Panel pSalvageSummary;
        private PictureBox pictureBox1;
        private Label lblEnhObtained;
        private Label lblBoosters;
        private PictureBox pictureBox3;
        private Panel panel3;
        private Panel panel4;
        private VScrollBar vScrollBar2;
        private midsControls.ctlPopUp ctlPopUp1;
        private PictureBox pictureBox4;
        private Label label1;
        private CheckBox checkBox1;
        private Panel panel5;
        private Label lblCatalysts;
        private PictureBox pictureBox5;
        private Label lblRewardMerits;
        private PictureBox pictureBox6;
        private midsControls.ImageButton imageButton1;
        private Label lblCatalysts;
        private PictureBox pictureBox2;
    }
}