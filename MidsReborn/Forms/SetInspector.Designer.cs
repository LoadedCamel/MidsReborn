using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.UIv2.Controls;

namespace Mids_Reborn.Forms
{
    partial class SetInspector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cbEffect = new ComboBox();
            cbEffectType = new ComboBox();
            cbEffectStr = new ComboBox();
            alvPowers = new Controls.AdvListView();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            powerImageList = new ImageList(components);
            cbArchetype = new AoCombo();
            enhSetInfo1 = new Mids_Reborn.Controls.EnhSetInfo();
            alvSets = new Controls.AdvListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            setImageList = new ImageList(components);
            SuspendLayout();
            // 
            // cbEffect
            // 
            cbEffect.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEffect.FormattingEnabled = true;
            cbEffect.Location = new Point(12, 12);
            cbEffect.Name = "cbEffect";
            cbEffect.Size = new Size(178, 23);
            cbEffect.TabIndex = 2;
            cbEffect.SelectedIndexChanged += CbEffect_SelectedIndexChanged;
            // 
            // cbEffectType
            // 
            cbEffectType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEffectType.FormattingEnabled = true;
            cbEffectType.Location = new Point(260, 12);
            cbEffectType.Name = "cbEffectType";
            cbEffectType.Size = new Size(160, 23);
            cbEffectType.TabIndex = 3;
            cbEffectType.SelectedIndexChanged += CbEffectType_SelectedIndexChanged;
            // 
            // cbEffectStr
            // 
            cbEffectStr.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEffectStr.FormattingEnabled = true;
            cbEffectStr.Location = new Point(490, 12);
            cbEffectStr.Name = "cbEffectStr";
            cbEffectStr.Size = new Size(102, 23);
            cbEffectStr.TabIndex = 4;
            cbEffectStr.SelectedIndexChanged += CbEffectStr_SelectedIndexChanged;
            // 
            // alvPowers
            // 
            alvPowers.Columns.AddRange(new ColumnHeader[] { columnHeader5, columnHeader6, columnHeader7 });
            alvPowers.FullRowSelect = true;
            alvPowers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            alvPowers.Location = new Point(12, 362);
            alvPowers.MultiSelect = false;
            alvPowers.Name = "alvPowers";
            alvPowers.Size = new Size(580, 237);
            alvPowers.SmallImageList = powerImageList;
            alvPowers.TabIndex = 7;
            alvPowers.UseCompatibleStateImageBehavior = false;
            alvPowers.View = View.Details;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Powerset";
            columnHeader5.Width = 210;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Type";
            columnHeader6.Width = 125;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "Power";
            columnHeader7.Width = 220;
            // 
            // powerImageList
            // 
            powerImageList.ColorDepth = ColorDepth.Depth32Bit;
            powerImageList.ImageSize = new Size(16, 16);
            powerImageList.TransparentColor = Color.Transparent;
            // 
            // cbArchetype
            // 
            cbArchetype.ComboType = AoCombo.ComboBoxType.Archetype;
            cbArchetype.DrawMode = DrawMode.OwnerDrawVariable;
            cbArchetype.DropDownStyle = ComboBoxStyle.DropDownList;
            cbArchetype.FormattingEnabled = true;
            cbArchetype.HighlightColor = Color.Empty;
            cbArchetype.Location = new Point(12, 332);
            cbArchetype.Name = "cbArchetype";
            cbArchetype.Size = new Size(150, 24);
            cbArchetype.TabIndex = 8;
            cbArchetype.SelectedIndexChanged += CbArchetypeOnSelectedIndexChanged;
            // 
            // enhSetInfo1
            // 
            enhSetInfo1.Location = new Point(604, 12);
            enhSetInfo1.Name = "enhSetInfo1";
            enhSetInfo1.Size = new Size(478, 344);
            enhSetInfo1.TabIndex = 0;
            // 
            // alvSets
            // 
            alvSets.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            alvSets.FullRowSelect = true;
            alvSets.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            alvSets.Location = new Point(12, 41);
            alvSets.MultiSelect = false;
            alvSets.Name = "alvSets";
            alvSets.Size = new Size(580, 272);
            alvSets.SmallImageList = setImageList;
            alvSets.TabIndex = 12;
            alvSets.UseCompatibleStateImageBehavior = false;
            alvSets.View = View.Details;
            alvSets.SelectedIndexChanged += AdvListView1OnSelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Set";
            columnHeader1.Width = 235;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Level";
            columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Type";
            columnHeader3.Width = 210;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Req. Enh";
            // 
            // setImageList
            // 
            setImageList.ColorDepth = ColorDepth.Depth32Bit;
            setImageList.ImageSize = new Size(16, 16);
            setImageList.TransparentColor = Color.Transparent;
            // 
            // SetInspector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(1094, 611);
            Controls.Add(enhSetInfo1);
            Controls.Add(alvSets);
            Controls.Add(cbArchetype);
            Controls.Add(alvPowers);
            Controls.Add(cbEffectStr);
            Controls.Add(cbEffectType);
            Controls.Add(cbEffect);
            DoubleBuffered = true;
            ForeColor = Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SetInspector";
            ShowInTaskbar = false;
            Text = "Set Inspector";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ComboBox cbEffect;
        private System.Windows.Forms.ComboBox cbEffectType;
        private System.Windows.Forms.ComboBox cbEffectStr;
        private Controls.AdvListView alvPowers;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private AoCombo cbArchetype;
        private Controls.AdvListView alvSets;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList setImageList;
        private Mids_Reborn.Controls.EnhSetInfo enhSetInfo1;
        private System.Windows.Forms.ImageList powerImageList;
    }
}