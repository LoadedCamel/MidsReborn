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
            this.ibClose = new mrbControls.ImageButton();
            this.ibTopmost = new mrbControls.ImageButton();
            this.lvPowers = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.cbArchetype = new System.Windows.Forms.ComboBox();
            this.cbPowerset = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ibSelPowersetSec = new mrbControls.ImageButton();
            this.label2 = new System.Windows.Forms.Label();
            this.ibSelAt = new mrbControls.ImageButton();
            this.ibSelPowersetPri = new mrbControls.ImageButton();
            this.Panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.lvBonus.Size = new System.Drawing.Size(280, 292);
            this.lvBonus.TabIndex = 0;
            this.lvBonus.UseCompatibleStateImageBehavior = false;
            this.lvBonus.View = System.Windows.Forms.View.Details;
            this.lvBonus.SelectedIndexChanged += new System.EventHandler(this.lvBonus_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Bonus Effect";
            this.ColumnHeader1.Width = 254;
            // 
            // lvMag
            // 
            this.lvMag.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader2});
            this.lvMag.FullRowSelect = true;
            this.lvMag.HideSelection = false;
            this.lvMag.Location = new System.Drawing.Point(298, 12);
            this.lvMag.MultiSelect = false;
            this.lvMag.Name = "lvMag";
            this.lvMag.Size = new System.Drawing.Size(127, 292);
            this.lvMag.TabIndex = 1;
            this.lvMag.UseCompatibleStateImageBehavior = false;
            this.lvMag.View = System.Windows.Forms.View.Details;
            this.lvMag.SelectedIndexChanged += new System.EventHandler(this.lvMag_SelectedIndexChanged);
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Effect Strength";
            this.ColumnHeader2.Width = 99;
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
            this.Panel1.Location = new System.Drawing.Point(431, 12);
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
            // ibClose
            // 
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(857, 540);
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
            this.ibTopmost.Location = new System.Drawing.Point(746, 540);
            this.ibTopmost.Name = "ibTopmost";
            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 4;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
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
            this.lvPowers.Location = new System.Drawing.Point(502, 310);
            this.lvPowers.MultiSelect = false;
            this.lvPowers.Name = "lvPowers";
            this.lvPowers.ShowItemToolTips = true;
            this.lvPowers.Size = new System.Drawing.Size(460, 215);
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
            this.columnHeader8.Width = 140;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Power";
            this.columnHeader9.Width = 145;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(554, 320);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Filter by:";
            this.label1.Visible = false;
            // 
            // cbArchetype
            // 
            this.cbArchetype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchetype.FormattingEnabled = true;
            this.cbArchetype.Location = new System.Drawing.Point(611, 317);
            this.cbArchetype.Name = "cbArchetype";
            this.cbArchetype.Size = new System.Drawing.Size(121, 22);
            this.cbArchetype.TabIndex = 8;
            this.cbArchetype.Visible = false;
            this.cbArchetype.SelectedIndexChanged += new System.EventHandler(this.cbArchetype_SelectedIndexChanged);
            // 
            // cbPowerset
            // 
            this.cbPowerset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPowerset.FormattingEnabled = true;
            this.cbPowerset.Location = new System.Drawing.Point(759, 317);
            this.cbPowerset.Name = "cbPowerset";
            this.cbPowerset.Size = new System.Drawing.Size(121, 22);
            this.cbPowerset.TabIndex = 9;
            this.cbPowerset.Visible = false;
            this.cbPowerset.SelectedIndexChanged += new System.EventHandler(this.cbPowerset_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ibSelPowersetSec);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.ibSelAt);
            this.panel2.Controls.Add(this.ibSelPowersetPri);
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(790, 73);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(174, 173);
            this.panel2.TabIndex = 15;
            this.panel2.Visible = false;
            // 
            // ibSelPowersetSec
            // 
            this.ibSelPowersetSec.Checked = false;
            this.ibSelPowersetSec.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSelPowersetSec.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibSelPowersetSec.Location = new System.Drawing.Point(34, 124);
            this.ibSelPowersetSec.Name = "ibSelPowersetSec";
            this.ibSelPowersetSec.Size = new System.Drawing.Size(105, 22);
            this.ibSelPowersetSec.TabIndex = 18;
            this.ibSelPowersetSec.TextOff = "By Powerset (Sec)";
            this.ibSelPowersetSec.TextOn = "Alt Text";
            this.ibSelPowersetSec.Toggle = false;
            this.ibSelPowersetSec.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibSelPowersetSec_ButtonClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 14);
            this.label2.TabIndex = 17;
            this.label2.Text = "Current build filters:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ibSelAt
            // 
            this.ibSelAt.Checked = false;
            this.ibSelAt.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSelAt.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibSelAt.Location = new System.Drawing.Point(34, 46);
            this.ibSelAt.Name = "ibSelAt";
            this.ibSelAt.Size = new System.Drawing.Size(105, 22);
            this.ibSelAt.TabIndex = 16;
            this.ibSelAt.TextOff = "By Archetype";
            this.ibSelAt.TextOn = "Alt Text";
            this.ibSelAt.Toggle = false;
            this.ibSelAt.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibSelAt_ButtonClicked);
            // 
            // ibSelPowersetPri
            // 
            this.ibSelPowersetPri.Checked = false;
            this.ibSelPowersetPri.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibSelPowersetPri.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibSelPowersetPri.Location = new System.Drawing.Point(34, 85);
            this.ibSelPowersetPri.Name = "ibSelPowersetPri";
            this.ibSelPowersetPri.Size = new System.Drawing.Size(105, 22);
            this.ibSelPowersetPri.TabIndex = 15;
            this.ibSelPowersetPri.TextOff = "By Powerset (Pri)";
            this.ibSelPowersetPri.TextOn = "Alt Text";
            this.ibSelPowersetPri.Toggle = false;
            this.ibSelPowersetPri.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibSelPowersetPri_ButtonClicked);
            // 
            // frmSetFind
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(974, 574);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.cbPowerset);
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
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private ComboBox cbPowerset;
        private Panel panel2;
        private Label label2;
        private ImageButton ibSelAt;
        private ImageButton ibSelPowersetPri;
        private ImageButton ibSelPowersetSec;
    }
}