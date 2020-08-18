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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.pbRecipe = new System.Windows.Forms.PictureBox();
            this.lvPower = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvDPA = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.ilSets = new System.Windows.Forms.ImageList(this.components);
            this.chkSortByLevel = new System.Windows.Forms.CheckBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.VScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.RecipeInfo = new midsControls.ctlPopUp();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.chkRecipe = new System.Windows.Forms.CheckBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ibMiniList = new midsControls.ImageButton();
            this.ibClipboard = new midsControls.ImageButton();
            this.ibTopmost = new midsControls.ImageButton();
            this.ibClose = new midsControls.ImageButton();
            ((System.ComponentModel.ISupportInitialize)this.pbRecipe).BeginInit();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();

            this.pbRecipe.Location = new System.Drawing.Point(0, 0);
            this.pbRecipe.Name = "pbRecipe";

            this.pbRecipe.Size = new System.Drawing.Size(60, 30);
            this.pbRecipe.TabIndex = 0;
            this.pbRecipe.TabStop = false;
            this.lvPower.CheckBoxes = true;
            this.lvPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader1
            });
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
            this.ColumnHeader1.Text = "Power";
            this.ColumnHeader1.Width = 148;
            this.lvDPA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3]
            {
        this.ColumnHeader3,
        this.ColumnHeader4,
        this.ColumnHeader5
            });
            this.lvDPA.FullRowSelect = true;
            this.lvDPA.HideSelection = false;

            this.lvDPA.Location = new System.Drawing.Point(194, 12);
            this.lvDPA.MultiSelect = false;
            this.lvDPA.Name = "lvDPA";

            this.lvDPA.Size = new System.Drawing.Size(428, 191);
            this.lvDPA.SmallImageList = this.ilSets;
            this.lvDPA.TabIndex = 8;
            this.lvDPA.UseCompatibleStateImageBehavior = false;
            this.lvDPA.View = System.Windows.Forms.View.Details;
            this.ColumnHeader3.Text = "Enhancement";
            this.ColumnHeader3.Width = 241;
            this.ColumnHeader4.Text = "Level";
            this.ColumnHeader4.Width = 45;
            this.ColumnHeader5.Text = "Power";
            this.ColumnHeader5.Width = 114;
            this.ilSets.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;

            this.ilSets.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSets.TransparentColor = System.Drawing.Color.Transparent;
            this.chkSortByLevel.Checked = true;
            this.chkSortByLevel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortByLevel.ForeColor = System.Drawing.Color.White;

            this.chkSortByLevel.Location = new System.Drawing.Point(12, 209);
            this.chkSortByLevel.Name = "chkSortByLevel";

            this.chkSortByLevel.Size = new System.Drawing.Size(176, 16);
            this.chkSortByLevel.TabIndex = 9;
            this.chkSortByLevel.Text = "Sort By Level";
            this.chkSortByLevel.UseVisualStyleBackColor = true;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 17.5f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);

            this.lblHeader.Location = new System.Drawing.Point(66, 0);
            this.lblHeader.Name = "lblHeader";

            this.lblHeader.Size = new System.Drawing.Size(541, 30);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "Select A Power / Recipe";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.VScrollBar1);
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.RecipeInfo);

            this.Panel1.Location = new System.Drawing.Point(0, 36);
            this.Panel1.Name = "Panel1";

            this.Panel1.Size = new System.Drawing.Size(610, 177);
            this.Panel1.TabIndex = 11;

            this.VScrollBar1.Location = new System.Drawing.Point(593, 0);
            this.VScrollBar1.Maximum = 20;
            this.VScrollBar1.Name = "VScrollBar1";

            this.VScrollBar1.Size = new System.Drawing.Size(17, 176);
            this.VScrollBar1.TabIndex = 3;
            this.RecipeInfo.BXHeight = 2048;
            this.RecipeInfo.ColumnPosition = 0.5f;
            this.RecipeInfo.ColumnRight = false;
            this.RecipeInfo.Font = new System.Drawing.Font("Arial", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.RecipeInfo.ForeColor = System.Drawing.Color.Black;
            this.RecipeInfo.InternalPadding = 3;

            this.RecipeInfo.Location = new System.Drawing.Point(3, 3);
            this.RecipeInfo.Name = "RecipeInfo";
            this.RecipeInfo.ScrollY = 0.0f;
            this.RecipeInfo.SectionPadding = 8;

            this.RecipeInfo.Size = new System.Drawing.Size(587, 212);
            this.RecipeInfo.TabIndex = 2;

            this.Panel2.BackColor = System.Drawing.Color.Black;
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.Panel1);
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.pbRecipe);
            this.Panel2.Controls.Add((System.Windows.Forms.Control)this.lblHeader);

            this.Panel2.Location = new System.Drawing.Point(12, 226);
            this.Panel2.Name = "Panel2";

            this.Panel2.Size = new System.Drawing.Size(610, 213);
            this.Panel2.TabIndex = 12;
            this.chkRecipe.ForeColor = System.Drawing.Color.White;

            this.chkRecipe.Location = new System.Drawing.Point(234, 445);
            this.chkRecipe.Name = "chkRecipe";

            this.chkRecipe.Size = new System.Drawing.Size(166, 22);
            this.chkRecipe.TabIndex = 15;
            this.chkRecipe.Text = "Include Recipes";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkRecipe, "Add a list of recipes to the shopping list");
            this.chkRecipe.UseVisualStyleBackColor = true;
            this.ibMiniList.Checked = false;
            this.ibMiniList.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibMiniList.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.ibMiniList.Location = new System.Drawing.Point(123, 445);
            this.ibMiniList.Name = "ibMiniList";

            this.ibMiniList.Size = new System.Drawing.Size(105, 22);
            this.ibMiniList.TabIndex = 14;
            this.ibMiniList.TextOff = "To I-G Helper";
            this.ibMiniList.TextOn = "Alt Text";
            this.ibMiniList.Toggle = false;
            this.ibClipboard.Checked = false;
            this.ibClipboard.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibClipboard.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.ibClipboard.Location = new System.Drawing.Point(12, 445);
            this.ibClipboard.Name = "ibClipboard";

            this.ibClipboard.Size = new System.Drawing.Size(105, 22);
            this.ibClipboard.TabIndex = 13;
            this.ibClipboard.TextOff = "To Clipboard";
            this.ibClipboard.TextOn = "Alt Text";
            this.ibClipboard.Toggle = false;
            this.ibTopmost.Checked = true;
            this.ibTopmost.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibTopmost.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.ibTopmost.Location = new System.Drawing.Point(406, 445);
            this.ibTopmost.Name = "ibTopmost";

            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 7;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.ibClose.Location = new System.Drawing.Point(517, 445);
            this.ibClose.Name = "ibClose";

            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 6;
            this.ibClose.TextOff = "Close";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(634, 479);
            this.Controls.Add((System.Windows.Forms.Control)this.chkRecipe);
            this.Controls.Add((System.Windows.Forms.Control)this.ibMiniList);
            this.Controls.Add((System.Windows.Forms.Control)this.ibClipboard);
            this.Controls.Add((System.Windows.Forms.Control)this.chkSortByLevel);
            this.Controls.Add((System.Windows.Forms.Control)this.lvDPA);
            this.Controls.Add((System.Windows.Forms.Control)this.ibTopmost);
            this.Controls.Add((System.Windows.Forms.Control)this.ibClose);
            this.Controls.Add((System.Windows.Forms.Control)this.lvPower);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel2);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Recipe Viewer";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)this.pbRecipe).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
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
    }
}