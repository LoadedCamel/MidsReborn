using System.ComponentModel;
using System.Windows.Forms;
using mrbControls;

namespace Mids_Reborn.Forms
{
    public partial class frmSetFind
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
            this.lvBonus = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvMag = new System.Windows.Forms.ListView();
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSet = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilSets = new System.Windows.Forms.ImageList(this.components);
            this.Panel1 = new System.Windows.Forms.Panel();
            this.SetInfo = new mrbControls.ctlPopUp();
            this.lvPowers = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.cbArchetype = new System.Windows.Forms.ComboBox();
            this.ibClose = new mrbControls.ImageButton();
            this.ibTopmost = new mrbControls.ImageButton();
            this.lvVector = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvBonus
            // 
            this.lvBonus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1});
            this.lvBonus.FullRowSelect = true;
            this.lvBonus.HideSelection = false;
            this.lvBonus.Location = new System.Drawing.Point(12, 12);
            this.lvBonus.MultiSelect = false;
            this.lvBonus.Name = "lvBonus";
            this.lvBonus.Size = new System.Drawing.Size(219, 292);
            this.lvBonus.TabIndex = 0;
            this.lvBonus.UseCompatibleStateImageBehavior = false;
            this.lvBonus.View = System.Windows.Forms.View.Details;
            this.lvBonus.SelectedIndexChanged += new System.EventHandler(this.lvBonus_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Effect Type";
            this.ColumnHeader1.Width = 193;
            // 
            // lvMag
            // 
            this.lvMag.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader2});
            this.lvMag.FullRowSelect = true;
            this.lvMag.HideSelection = false;
            this.lvMag.Location = new System.Drawing.Point(462, 12);
            this.lvMag.MultiSelect = false;
            this.lvMag.Name = "lvMag";
            this.lvMag.Size = new System.Drawing.Size(140, 292);
            this.lvMag.TabIndex = 1;
            this.lvMag.UseCompatibleStateImageBehavior = false;
            this.lvMag.View = System.Windows.Forms.View.Details;
            this.lvMag.SelectedIndexChanged += new System.EventHandler(this.lvMag_SelectedIndexChanged);
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Effect Strength";
            this.ColumnHeader2.Width = 114;
            // 
            // lvSet
            // 
            this.lvSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader3,
            this.ColumnHeader4,
            this.ColumnHeader5,
            this.ColumnHeader6});
            this.lvSet.FullRowSelect = true;
            this.lvSet.HideSelection = false;
            this.lvSet.Location = new System.Drawing.Point(12, 310);
            this.lvSet.MultiSelect = false;
            this.lvSet.Name = "lvSet";
            this.lvSet.Size = new System.Drawing.Size(484, 215);
            this.lvSet.SmallImageList = this.ilSets;
            this.lvSet.TabIndex = 2;
            this.lvSet.UseCompatibleStateImageBehavior = false;
            this.lvSet.View = System.Windows.Forms.View.Details;
            this.lvSet.SelectedIndexChanged += new System.EventHandler(this.lvSet_SelectedIndexChanged);
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Set";
            this.ColumnHeader3.Width = 157;
            // 
            // ColumnHeader4
            // 
            this.ColumnHeader4.Text = "Level";
            this.ColumnHeader4.Width = 69;
            // 
            // ColumnHeader5
            // 
            this.ColumnHeader5.Text = "Type";
            this.ColumnHeader5.Width = 140;
            // 
            // ColumnHeader6
            // 
            this.ColumnHeader6.Text = "Required Enh\'s.";
            this.ColumnHeader6.Width = 90;
            // 
            // ilSets
            // 
            this.ilSets.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilSets.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSets.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Panel1
            // 
            this.Panel1.AutoScroll = true;
            this.Panel1.BackColor = System.Drawing.Color.Black;
            this.Panel1.Controls.Add(this.SetInfo);
            this.Panel1.Location = new System.Drawing.Point(638, 12);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(351, 292);
            this.Panel1.TabIndex = 3;
            // 
            // SetInfo
            // 
            this.SetInfo.BXHeight = 600;
            this.SetInfo.ColumnPosition = 0.5F;
            this.SetInfo.ColumnRight = false;
            this.SetInfo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.SetInfo.InternalPadding = 3;
            this.SetInfo.Location = new System.Drawing.Point(0, 0);
            this.SetInfo.Name = "SetInfo";
            this.SetInfo.ScrollY = 0F;
            this.SetInfo.SectionPadding = 8;
            this.SetInfo.Size = new System.Drawing.Size(331, 203);
            this.SetInfo.TabIndex = 0;
            // 
            // lvPowers
            // 
            this.lvPowers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lvPowers.FullRowSelect = true;
            this.lvPowers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPowers.HideSelection = false;
            this.lvPowers.Location = new System.Drawing.Point(502, 350);
            this.lvPowers.MultiSelect = false;
            this.lvPowers.Name = "lvPowers";
            this.lvPowers.ShowItemToolTips = true;
            this.lvPowers.Size = new System.Drawing.Size(484, 175);
            this.lvPowers.TabIndex = 6;
            this.lvPowers.UseCompatibleStateImageBehavior = false;
            this.lvPowers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Group";
            this.columnHeader7.Width = 140;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Powerset";
            this.columnHeader8.Width = 155;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Power";
            this.columnHeader9.Width = 163;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(512, 319);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Filter by:";
            // 
            // cbArchetype
            // 
            this.cbArchetype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchetype.FormattingEnabled = true;
            this.cbArchetype.Location = new System.Drawing.Point(569, 316);
            this.cbArchetype.Name = "cbArchetype";
            this.cbArchetype.Size = new System.Drawing.Size(151, 22);
            this.cbArchetype.TabIndex = 8;
            this.cbArchetype.SelectedIndexChanged += new System.EventHandler(this.cbArchetype_SelectedIndexChanged);
            // 
            // ibClose
            // 
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(881, 540);
            this.ibClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ibClose.Name = "ibClose";
            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 5;
            this.ibClose.TextOff = "Close";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            // 
            // ibTopmost
            // 
            this.ibTopmost.Checked = true;
            this.ibTopmost.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibTopmost.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibTopmost.Location = new System.Drawing.Point(770, 540);
            this.ibTopmost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ibTopmost.Name = "ibTopmost";
            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 4;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
            // 
            // lvVector
            // 
            this.lvVector.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10});
            this.lvVector.Enabled = false;
            this.lvVector.FullRowSelect = true;
            this.lvVector.HideSelection = false;
            this.lvVector.Location = new System.Drawing.Point(237, 12);
            this.lvVector.MultiSelect = false;
            this.lvVector.Name = "lvVector";
            this.lvVector.Size = new System.Drawing.Size(219, 292);
            this.lvVector.TabIndex = 9;
            this.lvVector.UseCompatibleStateImageBehavior = false;
            this.lvVector.View = System.Windows.Forms.View.Details;
            this.lvVector.SelectedIndexChanged += new System.EventHandler(this.lvVector_SelectedIndexChanged);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "";
            this.columnHeader10.Width = 193;
            // 
            // frmSetFind
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1001, 574);
            this.Controls.Add(this.lvVector);
            this.Controls.Add(this.cbArchetype);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvPowers);
            this.Controls.Add(this.ibClose);
            this.Controls.Add(this.ibTopmost);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.lvSet);
            this.Controls.Add(this.lvMag);
            this.Controls.Add(this.lvBonus);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "frmSetFind";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Bonus Finder";
            this.TopMost = true;
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ColumnHeader ColumnHeader6;
        ImageList ilSets;
        ListView lvBonus;
        ListView lvMag;
        ListView lvSet;
        Panel Panel1;
        private ListView lvPowers;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private Label label1;
        private ComboBox cbArchetype;
        private ListView lvVector;
        private ColumnHeader columnHeader10;
    }
}