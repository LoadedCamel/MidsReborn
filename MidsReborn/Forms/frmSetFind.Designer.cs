using Mids_Reborn.Controls;
using System.ComponentModel;
using System.Windows.Forms;

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
            components = new Container();
            lvBonus = new ListView();
            ColumnHeader1 = new ColumnHeader();
            lvMag = new ListView();
            ColumnHeader2 = new ColumnHeader();
            lvSet = new ListView();
            ColumnHeader3 = new ColumnHeader();
            ColumnHeader4 = new ColumnHeader();
            ColumnHeader5 = new ColumnHeader();
            ColumnHeader6 = new ColumnHeader();
            ilSets = new ImageList(components);
            Panel1 = new Panel();
            lvPowers = new ListView();
            columnHeader7 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            columnHeader9 = new ColumnHeader();
            label1 = new Label();
            cbArchetype = new ComboBox();
            lvVector = new ListView();
            columnHeader10 = new ColumnHeader();
            ibKeepOnTop = new Controls.ImageButtonEx();
            ibClose = new Controls.ImageButtonEx();
            cbPrimary = new ComboBox();
            cbSecondary = new ComboBox();
            SetInfo = new ctlPopUp();
            SuspendLayout();
            // 
            // lvBonus
            // 
            lvBonus.Columns.AddRange(new ColumnHeader[] { ColumnHeader1 });
            lvBonus.FullRowSelect = true;
            lvBonus.Location = new System.Drawing.Point(12, 12);
            lvBonus.MultiSelect = false;
            lvBonus.Name = "lvBonus";
            lvBonus.Size = new System.Drawing.Size(219, 292);
            lvBonus.TabIndex = 0;
            lvBonus.UseCompatibleStateImageBehavior = false;
            lvBonus.View = View.Details;
            lvBonus.SelectedIndexChanged += lvBonus_SelectedIndexChanged;
            // 
            // ColumnHeader1
            // 
            ColumnHeader1.Text = "Effect Type";
            ColumnHeader1.Width = 193;
            // 
            // lvMag
            // 
            lvMag.Columns.AddRange(new ColumnHeader[] { ColumnHeader2 });
            lvMag.FullRowSelect = true;
            lvMag.Location = new System.Drawing.Point(462, 12);
            lvMag.MultiSelect = false;
            lvMag.Name = "lvMag";
            lvMag.Size = new System.Drawing.Size(140, 292);
            lvMag.TabIndex = 1;
            lvMag.UseCompatibleStateImageBehavior = false;
            lvMag.View = View.Details;
            lvMag.SelectedIndexChanged += lvMag_SelectedIndexChanged;
            // 
            // ColumnHeader2
            // 
            ColumnHeader2.Text = "Effect Strength";
            ColumnHeader2.Width = 114;
            // 
            // lvSet
            // 
            lvSet.Columns.AddRange(new ColumnHeader[] { ColumnHeader3, ColumnHeader4, ColumnHeader5, ColumnHeader6 });
            lvSet.FullRowSelect = true;
            lvSet.Location = new System.Drawing.Point(12, 310);
            lvSet.MultiSelect = false;
            lvSet.Name = "lvSet";
            lvSet.Size = new System.Drawing.Size(484, 215);
            lvSet.SmallImageList = ilSets;
            lvSet.TabIndex = 2;
            lvSet.UseCompatibleStateImageBehavior = false;
            lvSet.View = View.Details;
            lvSet.SelectedIndexChanged += lvSet_SelectedIndexChanged;
            // 
            // ColumnHeader3
            // 
            ColumnHeader3.Text = "Set";
            ColumnHeader3.Width = 157;
            // 
            // ColumnHeader4
            // 
            ColumnHeader4.Text = "Level";
            ColumnHeader4.Width = 69;
            // 
            // ColumnHeader5
            // 
            ColumnHeader5.Text = "Type";
            ColumnHeader5.Width = 140;
            // 
            // ColumnHeader6
            // 
            ColumnHeader6.Text = "Required Enh's.";
            ColumnHeader6.Width = 90;
            // 
            // ilSets
            // 
            ilSets.ColorDepth = ColorDepth.Depth32Bit;
            ilSets.ImageSize = new System.Drawing.Size(16, 16);
            ilSets.TransparentColor = System.Drawing.Color.Transparent;
            //
            // SetInfo
            // 
            SetInfo.BXHeight = 600;
            SetInfo.ColumnPosition = 0.5F;
            SetInfo.ColumnRight = false;
            SetInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            SetInfo.InternalPadding = 3;
            SetInfo.Location = new System.Drawing.Point(0, 0);
            SetInfo.Name = "SetInfo";
            SetInfo.ScrollY = 0F;
            SetInfo.SectionPadding = 8;
            SetInfo.Size = new System.Drawing.Size(331, 203);
            SetInfo.TabIndex = 0;
            // 
            // Panel1
            // 
            Panel1.AutoScroll = true;
            Panel1.BackColor = System.Drawing.Color.Black;
            Panel1.Location = new System.Drawing.Point(638, 12);
            Panel1.Controls.Add(SetInfo);
            Panel1.Name = "Panel1";
            Panel1.Size = new System.Drawing.Size(351, 292);
            Panel1.TabIndex = 3;
            //
            // lvPowers
            // 
            lvPowers.Columns.AddRange(new ColumnHeader[] { columnHeader7, columnHeader8, columnHeader9 });
            lvPowers.FullRowSelect = true;
            lvPowers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvPowers.Location = new System.Drawing.Point(502, 350);
            lvPowers.MultiSelect = false;
            lvPowers.Name = "lvPowers";
            lvPowers.ShowItemToolTips = true;
            lvPowers.Size = new System.Drawing.Size(484, 175);
            lvPowers.TabIndex = 6;
            lvPowers.UseCompatibleStateImageBehavior = false;
            lvPowers.View = View.Details;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Group";
            columnHeader7.Width = 140;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "Powerset";
            columnHeader8.Width = 155;
            // 
            // columnHeader9
            // 
            columnHeader9.Text = "Power";
            columnHeader9.Width = 163;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(512, 319);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 13);
            label1.TabIndex = 7;
            label1.Text = "Filter by:";
            // 
            // cbArchetype
            // 
            cbArchetype.DropDownStyle = ComboBoxStyle.DropDownList;
            cbArchetype.FormattingEnabled = true;
            cbArchetype.Location = new System.Drawing.Point(569, 316);
            cbArchetype.Name = "cbArchetype";
            cbArchetype.Size = new System.Drawing.Size(124, 21);
            cbArchetype.TabIndex = 8;
            cbArchetype.SelectedIndexChanged += cbArchetype_SelectedIndexChanged;
            // 
            // lvVector
            // 
            lvVector.Columns.AddRange(new ColumnHeader[] { columnHeader10 });
            lvVector.Enabled = false;
            lvVector.FullRowSelect = true;
            lvVector.Location = new System.Drawing.Point(237, 12);
            lvVector.MultiSelect = false;
            lvVector.Name = "lvVector";
            lvVector.Size = new System.Drawing.Size(219, 292);
            lvVector.TabIndex = 9;
            lvVector.UseCompatibleStateImageBehavior = false;
            lvVector.View = View.Details;
            lvVector.SelectedIndexChanged += lvVector_SelectedIndexChanged;
            // 
            // columnHeader10
            // 
            columnHeader10.Text = "";
            columnHeader10.Width = 193;
            // 
            // ibKeepOnTop
            // 
            ibKeepOnTop.BackgroundImageLayout = ImageLayout.None;
            ibKeepOnTop.ButtonType = Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            ibKeepOnTop.CurrentText = "ToggledOff State";
            ibKeepOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibKeepOnTop.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibKeepOnTop.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibKeepOnTop.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibKeepOnTop.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibKeepOnTop.Location = new System.Drawing.Point(764, 540);
            ibKeepOnTop.Lock = false;
            ibKeepOnTop.Name = "ibKeepOnTop";
            ibKeepOnTop.Size = new System.Drawing.Size(105, 22);
            ibKeepOnTop.TabIndex = 10;
            ibKeepOnTop.Text = "Keep On Top";
            ibKeepOnTop.TextOutline.Color = System.Drawing.Color.Black;
            ibKeepOnTop.TextOutline.Width = 2;
            ibKeepOnTop.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibKeepOnTop.ToggleText.Indeterminate = "Indeterminate State";
            ibKeepOnTop.ToggleText.ToggledOff = "To Top";
            ibKeepOnTop.ToggleText.ToggledOn = "Keep On Top";
            ibKeepOnTop.UseAlt = false;
            ibKeepOnTop.Click += ibKeepOnTop_Click;
            // 
            // ibClose
            // 
            ibClose.BackgroundImageLayout = ImageLayout.None;
            ibClose.CurrentText = "Close";
            ibClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibClose.Location = new System.Drawing.Point(881, 540);
            ibClose.Lock = false;
            ibClose.Name = "ibClose";
            ibClose.Size = new System.Drawing.Size(105, 22);
            ibClose.TabIndex = 11;
            ibClose.Text = "Close";
            ibClose.TextOutline.Color = System.Drawing.Color.Black;
            ibClose.TextOutline.Width = 2;
            ibClose.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibClose.ToggleText.Indeterminate = "Indeterminate State";
            ibClose.ToggleText.ToggledOff = "Close";
            ibClose.ToggleText.ToggledOn = "Close";
            ibClose.UseAlt = false;
            ibClose.Click += ibClose_Click;
            // 
            // cbPrimary
            // 
            cbPrimary.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPrimary.FormattingEnabled = true;
            cbPrimary.Location = new System.Drawing.Point(714, 316);
            cbPrimary.Name = "cbPrimary";
            cbPrimary.Size = new System.Drawing.Size(124, 21);
            cbPrimary.TabIndex = 12;
            cbPrimary.Visible = false;
            cbPrimary.SelectedIndexChanged += cbPrimary_SelectedIndexChanged;
            // 
            // cbSecondary
            // 
            cbSecondary.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSecondary.FormattingEnabled = true;
            cbSecondary.Location = new System.Drawing.Point(859, 316);
            cbSecondary.Name = "cbSecondary";
            cbSecondary.Size = new System.Drawing.Size(124, 21);
            cbSecondary.TabIndex = 13;
            cbSecondary.Visible = false;
            cbSecondary.SelectedIndexChanged += cbSecondary_SelectedIndexChanged;
            // 
            // frmSetFind
            // 
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            ClientSize = new System.Drawing.Size(1001, 574);
            Controls.Add(cbSecondary);
            Controls.Add(cbPrimary);
            Controls.Add(ibClose);
            Controls.Add(ibKeepOnTop);
            Controls.Add(lvVector);
            Controls.Add(cbArchetype);
            Controls.Add(label1);
            Controls.Add(lvPowers);
            Controls.Add(Panel1);
            Controls.Add(lvSet);
            Controls.Add(lvMag);
            Controls.Add(lvBonus);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "frmSetFind";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Set Bonus Finder";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
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
        private Controls.ImageButtonEx ibKeepOnTop;
        private Controls.ImageButtonEx ibClose;
        private ComboBox cbPrimary;
        private ComboBox cbSecondary;
    }
}