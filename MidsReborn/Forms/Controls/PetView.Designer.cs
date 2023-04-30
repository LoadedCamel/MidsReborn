using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.Controls
{
    partial class PetView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PetView));
            this.info_Damage = new Mids_Reborn.Controls.ctlDamageDisplay();
            this.lblDmg = new System.Windows.Forms.Label();
            this.powerScaler = new Mids_Reborn.Controls.ctlMultiGraph();
            this.info_TxtLarge = new System.Windows.Forms.RichTextBox();
            this.info_TxtSmall = new System.Windows.Forms.RichTextBox();
            this.info_Title = new Label();
            this.infoTip = new System.Windows.Forms.ToolTip(this.components);
            this.info_DataList = new Mids_Reborn.Controls.PairedListEx();
            this.panelSeparator1 = new System.Windows.Forms.Panel();
            this.panelSeparator2 = new System.Windows.Forms.Panel();
            this.panelSeparator3 = new System.Windows.Forms.Panel();
            this.panelSeparator4 = new System.Windows.Forms.Panel();
            this.panelSeparator5 = new System.Windows.Forms.Panel();
            this.panelSeparator6 = new System.Windows.Forms.Panel();
            this.containerPanel = new System.Windows.Forms.Panel();
            this.containerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // info_Damage
            // 
            this.info_Damage.BackColor = System.Drawing.Color.Black;
            this.info_Damage.ColorBackEnd = System.Drawing.Color.Black;
            this.info_Damage.ColorBackStart = System.Drawing.Color.Black;
            this.info_Damage.ColorBaseEnd = System.Drawing.Color.Blue;
            this.info_Damage.ColorBaseStart = System.Drawing.Color.DodgerBlue;
            this.info_Damage.ColorEnhEnd = System.Drawing.Color.Goldenrod;
            this.info_Damage.ColorEnhStart = System.Drawing.Color.Gold;
            this.info_Damage.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_Damage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.info_Damage.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.info_Damage.GraphType = Mids_Reborn.Core.Enums.eDDGraph.Both;
            this.info_Damage.Location = new System.Drawing.Point(0, 402);
            this.info_Damage.Name = "info_Damage";
            this.info_Damage.nBaseVal = 100F;
            this.info_Damage.nEnhVal = 150F;
            this.info_Damage.nHighBase = 200F;
            this.info_Damage.nHighEnh = 214F;
            this.info_Damage.nMaxEnhVal = 207F;
            this.info_Damage.PaddingH = 1;
            this.info_Damage.PaddingV = 1;
            this.info_Damage.Size = new System.Drawing.Size(436, 109);
            this.info_Damage.Style = Mids_Reborn.Core.Enums.eDDStyle.TextUnderGraph;
            this.info_Damage.TabIndex = 21;
            this.info_Damage.TextAlign = Mids_Reborn.Core.Enums.eDDAlign.Center;
            this.info_Damage.TextColor = System.Drawing.Color.WhiteSmoke;
            // 
            // lblDmg
            // 
            this.lblDmg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDmg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDmg.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDmg.Location = new System.Drawing.Point(0, 373);
            this.lblDmg.Name = "lblDmg";
            this.lblDmg.Size = new System.Drawing.Size(436, 26);
            this.lblDmg.TabIndex = 22;
            this.lblDmg.Text = "Damage (Blue = Base | Gold = Enhanced)";
            this.lblDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // powerScaler
            // 
            this.powerScaler.BackColor = System.Drawing.Color.Black;
            this.powerScaler.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("powerScaler.BackgroundImage")));
            this.powerScaler.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.powerScaler.BaseBarColors = null;
            this.powerScaler.Border = true;
            this.powerScaler.BorderColor = System.Drawing.Color.Black;
            this.powerScaler.Clickable = true;
            this.powerScaler.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.powerScaler.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(64)))));
            this.powerScaler.ColorEnh = System.Drawing.Color.Yellow;
            this.powerScaler.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.powerScaler.ColorFadeStart = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(10)))), ((int)(((byte)(0)))));
            this.powerScaler.ColorHighlight = System.Drawing.Color.Gray;
            this.powerScaler.ColorLines = System.Drawing.Color.Black;
            this.powerScaler.ColorMarkerInner = System.Drawing.Color.Red;
            this.powerScaler.ColorMarkerOuter = System.Drawing.Color.Black;
            this.powerScaler.ColorOvercap = System.Drawing.Color.Black;
            this.powerScaler.DifferentiateColors = false;
            this.powerScaler.Dock = System.Windows.Forms.DockStyle.Top;
            this.powerScaler.DrawRuler = false;
            this.powerScaler.Dual = false;
            this.powerScaler.EnhBarColors = null;
            this.powerScaler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.powerScaler.ForcedMax = 0F;
            this.powerScaler.ForeColor = System.Drawing.Color.Azure;
            this.powerScaler.Highlight = false;
            this.powerScaler.ItemFontSizeOverride = 0F;
            this.powerScaler.ItemHeight = 12;
            this.powerScaler.Lines = true;
            this.powerScaler.Location = new System.Drawing.Point(0, 192);
            this.powerScaler.MarkerValue = 0F;
            this.powerScaler.Max = 100F;
            this.powerScaler.MaxItems = 1;
            this.powerScaler.Name = "powerScaler";
            this.powerScaler.OuterBorder = true;
            this.powerScaler.Overcap = false;
            this.powerScaler.OvercapColors = null;
            this.powerScaler.PaddingX = 2F;
            this.powerScaler.PaddingY = 2F;
            this.powerScaler.PerItemScales = null;
            this.powerScaler.RulerPos = Mids_Reborn.Controls.ctlMultiGraph.RulerPosition.Top;
            this.powerScaler.ScaleHeight = 32;
            this.powerScaler.ScaleIndex = 8;
            this.powerScaler.ShowScale = true;
            this.powerScaler.Size = new System.Drawing.Size(436, 20);
            this.powerScaler.Style = Mids_Reborn.Core.Enums.GraphStyle.baseOnly;
            this.powerScaler.TabIndex = 72;
            this.powerScaler.TextWidth = 100;
            this.powerScaler.BarClick += new Mids_Reborn.Controls.ctlMultiGraph.BarClickEventHandler(this.powerScaler_BarClick);
            // 
            // info_TxtLarge
            // 
            this.info_TxtLarge.BackColor = System.Drawing.Color.Black;
            this.info_TxtLarge.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.info_TxtLarge.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_TxtLarge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_TxtLarge.ForeColor = System.Drawing.Color.Black;
            this.info_TxtLarge.Location = new System.Drawing.Point(0, 88);
            this.info_TxtLarge.Name = "info_TxtLarge";
            this.info_TxtLarge.ReadOnly = true;
            this.info_TxtLarge.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.info_TxtLarge.Size = new System.Drawing.Size(436, 101);
            this.info_TxtLarge.TabIndex = 73;
            this.info_TxtLarge.Text = "info_Rich";
            // 
            // info_TxtSmall
            // 
            this.info_TxtSmall.BackColor = System.Drawing.Color.Black;
            this.info_TxtSmall.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.info_TxtSmall.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_TxtSmall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_TxtSmall.ForeColor = System.Drawing.Color.Black;
            this.info_TxtSmall.Location = new System.Drawing.Point(0, 42);
            this.info_TxtSmall.Name = "info_TxtSmall";
            this.info_TxtSmall.ReadOnly = true;
            this.info_TxtSmall.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.info_TxtSmall.Size = new System.Drawing.Size(436, 43);
            this.info_TxtSmall.TabIndex = 74;
            this.info_TxtSmall.Text = "info_Rich";
            // 
            // info_Title
            // 
            this.info_Title.BackColor = System.Drawing.Color.Black;
            this.info_Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.info_Title.ForeColor = System.Drawing.Color.White;
            this.info_Title.Text = "Title";
            this.info_Title.Location = new System.Drawing.Point(0, 0);
            this.info_Title.Name = "info_Title";
            this.info_Title.Size = new System.Drawing.Size(436, 39);
            this.info_Title.TabIndex = 75;
            this.info_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // info_DataList
            // 
            this.info_DataList.AutoScroll = true;
            this.info_DataList.Columns = 2;
            this.info_DataList.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_DataList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_DataList.HighlightColor = System.Drawing.Color.CornflowerBlue;
            this.info_DataList.HighlightTextColor = System.Drawing.Color.Black;
            this.info_DataList.ItemColor = System.Drawing.Color.Silver;
            this.info_DataList.Location = new System.Drawing.Point(0, 215);
            this.info_DataList.Name = "info_DataList";
            this.info_DataList.Rows = 5;
            this.info_DataList.Size = new System.Drawing.Size(436, 155);
            this.info_DataList.TabIndex = 79;
            this.info_DataList.UseHighlighting = true;
            this.info_DataList.ValueAlternateColor = System.Drawing.Color.Chartreuse;
            this.info_DataList.ValueColor = System.Drawing.Color.Azure;
            this.info_DataList.ValueConditionColor = System.Drawing.Color.Firebrick;
            this.info_DataList.ValueSpecialColor = System.Drawing.Color.SlateBlue;
            // 
            // panelSeparator1
            // 
            this.panelSeparator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator1.Location = new System.Drawing.Point(0, 39);
            this.panelSeparator1.Name = "panelSeparator1";
            this.panelSeparator1.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator1.TabIndex = 83;
            // 
            // panelSeparator2
            // 
            this.panelSeparator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator2.Location = new System.Drawing.Point(0, 85);
            this.panelSeparator2.Name = "panelSeparator2";
            this.panelSeparator2.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator2.TabIndex = 82;
            // 
            // panelSeparator3
            // 
            this.panelSeparator3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator3.Location = new System.Drawing.Point(0, 189);
            this.panelSeparator3.Name = "panelSeparator3";
            this.panelSeparator3.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator3.TabIndex = 81;
            // 
            // panelSeparator4
            // 
            this.panelSeparator4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator4.Location = new System.Drawing.Point(0, 212);
            this.panelSeparator4.Name = "panelSeparator4";
            this.panelSeparator4.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator4.TabIndex = 80;
            // 
            // panelSeparator5
            // 
            this.panelSeparator5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator5.Location = new System.Drawing.Point(0, 370);
            this.panelSeparator5.Name = "panelSeparator5";
            this.panelSeparator5.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator5.TabIndex = 23;
            // 
            // panelSeparator6
            // 
            this.panelSeparator6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(33)))), ((int)(((byte)(59)))));
            this.panelSeparator6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator6.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.panelSeparator6.Location = new System.Drawing.Point(0, 399);
            this.panelSeparator6.Name = "panelSeparator6";
            this.panelSeparator6.Size = new System.Drawing.Size(436, 3);
            this.panelSeparator6.TabIndex = 22;
            // 
            // containerPanel
            // 
            this.containerPanel.BackColor = System.Drawing.Color.Black;
            this.containerPanel.Controls.Add(this.info_Damage);
            this.containerPanel.Controls.Add(this.panelSeparator6);
            this.containerPanel.Controls.Add(this.lblDmg);
            this.containerPanel.Controls.Add(this.panelSeparator5);
            this.containerPanel.Controls.Add(this.info_DataList);
            this.containerPanel.Controls.Add(this.panelSeparator4);
            this.containerPanel.Controls.Add(this.powerScaler);
            this.containerPanel.Controls.Add(this.panelSeparator3);
            this.containerPanel.Controls.Add(this.info_TxtLarge);
            this.containerPanel.Controls.Add(this.panelSeparator2);
            this.containerPanel.Controls.Add(this.info_TxtSmall);
            this.containerPanel.Controls.Add(this.panelSeparator1);
            this.containerPanel.Controls.Add(this.info_Title);
            this.containerPanel.Location = new System.Drawing.Point(3, 3);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(436, 513);
            this.containerPanel.TabIndex = 0;
            // 
            // PetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(56)))), ((int)(((byte)(100)))));
            this.Controls.Add(this.containerPanel);
            this.Name = "PetView";
            this.Size = new System.Drawing.Size(442, 519);
            this.containerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        internal Mids_Reborn.Controls.ctlDamageDisplay info_Damage;
        private System.Windows.Forms.Label lblDmg;
        private Mids_Reborn.Controls.ctlMultiGraph powerScaler;
        internal System.Windows.Forms.RichTextBox info_TxtLarge;
        private System.Windows.Forms.RichTextBox info_TxtSmall;
        private Label info_Title;
        private System.Windows.Forms.ToolTip infoTip;
        internal System.Windows.Forms.Panel panelSeparator1;
        internal System.Windows.Forms.Panel panelSeparator2;
        internal System.Windows.Forms.Panel panelSeparator3;
        internal System.Windows.Forms.Panel panelSeparator4;
        internal System.Windows.Forms.Panel panelSeparator5;
        internal System.Windows.Forms.Panel panelSeparator6;
        private System.Windows.Forms.Panel containerPanel;
        private PairedListEx info_DataList;
    }
}
