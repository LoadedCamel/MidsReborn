using System.Data;
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PetView));
            info_Damage = new ctlDamageDisplay();
            lblDmg = new Label();
            powerScaler = new CtlMultiGraph();
            info_TxtLarge = new RichTextBox();
            info_TxtSmall = new RichTextBox();
            info_Title = new Label();
            infoTip = new ToolTip(components);
            info_DataList = new PairedListEx();
            panelSeparator1 = new Panel();
            panelSeparator2 = new Panel();
            panelSeparator3 = new Panel();
            panelSeparator4 = new Panel();
            panelSeparator5 = new Panel();
            panelSeparator6 = new Panel();
            containerPanel = new Panel();
            containerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // info_Damage
            // 
            info_Damage.BackColor = Color.Black;
            info_Damage.ColorBackEnd = Color.Black;
            info_Damage.ColorBackStart = Color.Black;
            info_Damage.ColorBaseEnd = Color.Blue;
            info_Damage.ColorBaseStart = Color.DodgerBlue;
            info_Damage.ColorEnhEnd = Color.Goldenrod;
            info_Damage.ColorEnhStart = Color.Gold;
            info_Damage.Dock = DockStyle.Top;
            info_Damage.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            info_Damage.ForeColor = Color.WhiteSmoke;
            info_Damage.GraphType = Core.Enums.eDDGraph.Both;
            info_Damage.Location = new Point(0, 402);
            info_Damage.Name = "info_Damage";
            info_Damage.nBaseVal = 100F;
            info_Damage.nEnhVal = 150F;
            info_Damage.nHighBase = 200F;
            info_Damage.nHighEnh = 214F;
            info_Damage.nMaxEnhVal = 207F;
            info_Damage.PaddingH = 1;
            info_Damage.PaddingV = 1;
            info_Damage.Size = new Size(436, 109);
            info_Damage.Style = Core.Enums.eDDStyle.TextUnderGraph;
            info_Damage.TabIndex = 21;
            info_Damage.TextAlign = Core.Enums.eDDAlign.Center;
            info_Damage.TextColor = Color.WhiteSmoke;
            // 
            // lblDmg
            // 
            lblDmg.Dock = DockStyle.Top;
            lblDmg.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblDmg.ForeColor = Color.WhiteSmoke;
            lblDmg.Location = new Point(0, 373);
            lblDmg.Name = "lblDmg";
            lblDmg.Size = new Size(436, 26);
            lblDmg.TabIndex = 22;
            lblDmg.Text = "Damage (Blue = Base | Gold = Enhanced)";
            lblDmg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // powerScaler
            // 
            powerScaler.BackColor = Color.Black;
            powerScaler.BackgroundImage = (Image)resources.GetObject("powerScaler.BackgroundImage");
            powerScaler.BackgroundImageLayout = ImageLayout.Stretch;
            powerScaler.BaseBarColors = (System.Collections.Generic.List<Color>)resources.GetObject("powerScaler.BaseBarColors");
            powerScaler.Border = true;
            powerScaler.BorderColor = Color.Black;
            powerScaler.Clickable = true;
            powerScaler.ColorAbsorbed = Color.Gainsboro;
            powerScaler.ColorBase = Color.FromArgb(64, 255, 64);
            powerScaler.ColorEnh = Color.Yellow;
            powerScaler.ColorFadeEnd = Color.FromArgb(0, 192, 0);
            powerScaler.ColorFadeStart = Color.FromArgb(0, 10, 0);
            powerScaler.ColorHighlight = Color.Gray;
            powerScaler.ColorLines = Color.Black;
            powerScaler.ColorMarkerInner = Color.Red;
            powerScaler.ColorMarkerOuter = Color.Black;
            powerScaler.ColorOvercap = Color.Black;
            powerScaler.DifferentiateColors = false;
            powerScaler.Dock = DockStyle.Top;
            powerScaler.DrawRuler = false;
            powerScaler.Dual = false;
            powerScaler.EnhBarColors = (System.Collections.Generic.List<Color>)resources.GetObject("powerScaler.EnhBarColors");
            powerScaler.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            powerScaler.ForcedMax = 0F;
            powerScaler.ForeColor = Color.Azure;
            powerScaler.Highlight = false;
            powerScaler.ItemFontSizeOverride = 0F;
            powerScaler.ItemHeight = 11;
            powerScaler.Lines = true;
            powerScaler.Location = new Point(0, 192);
            powerScaler.MarkerValue = 0F;
            powerScaler.Max = 100F;
            powerScaler.MaxItems = 1;
            powerScaler.Name = "powerScaler";
            powerScaler.OuterBorder = true;
            powerScaler.Overcap = false;
            powerScaler.OvercapColors = (System.Collections.Generic.List<Color>)resources.GetObject("powerScaler.OvercapColors");
            powerScaler.PaddingX = 2F;
            powerScaler.PaddingY = 2F;
            powerScaler.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("powerScaler.PerItemScales");
            powerScaler.RulerPos = CtlMultiGraph.RulerPosition.Top;
            powerScaler.ScaleHeight = 32;
            powerScaler.ScaleIndex = 8;
            powerScaler.ShowScale = true;
            powerScaler.Size = new Size(436, 20);
            powerScaler.Style = Core.Enums.GraphStyle.baseOnly;
            powerScaler.TabIndex = 72;
            powerScaler.TextWidth = 100;
            powerScaler.BarClick += powerScaler_BarClick;
            // 
            // info_TxtLarge
            // 
            info_TxtLarge.BackColor = Color.Black;
            info_TxtLarge.BorderStyle = BorderStyle.None;
            info_TxtLarge.Dock = DockStyle.Top;
            info_TxtLarge.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            info_TxtLarge.ForeColor = Color.WhiteSmoke;
            info_TxtLarge.Location = new Point(0, 88);
            info_TxtLarge.Name = "info_TxtLarge";
            info_TxtLarge.ReadOnly = true;
            info_TxtLarge.ScrollBars = RichTextBoxScrollBars.Vertical;
            info_TxtLarge.Size = new Size(436, 101);
            info_TxtLarge.TabIndex = 73;
            info_TxtLarge.Text = "info_Rich";
            // 
            // info_TxtSmall
            // 
            info_TxtSmall.BackColor = Color.Black;
            info_TxtSmall.BorderStyle = BorderStyle.None;
            info_TxtSmall.Dock = DockStyle.Top;
            info_TxtSmall.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            info_TxtSmall.ForeColor = Color.WhiteSmoke;
            info_TxtSmall.Location = new Point(0, 42);
            info_TxtSmall.Name = "info_TxtSmall";
            info_TxtSmall.ReadOnly = true;
            info_TxtSmall.ScrollBars = RichTextBoxScrollBars.None;
            info_TxtSmall.Size = new Size(436, 43);
            info_TxtSmall.TabIndex = 74;
            info_TxtSmall.Text = "info_Rich";
            // 
            // info_Title
            // 
            info_Title.BackColor = Color.Black;
            info_Title.Dock = DockStyle.Top;
            info_Title.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            info_Title.ForeColor = Color.White;
            info_Title.Location = new Point(0, 0);
            info_Title.Name = "info_Title";
            info_Title.Size = new Size(436, 39);
            info_Title.TabIndex = 75;
            info_Title.Text = "Title";
            info_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // infoTip
            // 
            infoTip.AutoPopDelay = 15000;
            infoTip.InitialDelay = 350;
            infoTip.ReshowDelay = 100;
            // 
            // info_DataList
            // 
            info_DataList.AutoScroll = true;
            info_DataList.AutoSizeLineHeight = true;
            info_DataList.Columns = 2;
            info_DataList.Dock = DockStyle.Top;
            info_DataList.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            info_DataList.HighlightColor = Color.CornflowerBlue;
            info_DataList.HighlightTextColor = Color.Black;
            info_DataList.ItemColor = Color.Silver;
            info_DataList.Location = new Point(0, 215);
            info_DataList.Name = "info_DataList";
            info_DataList.Rows = 5;
            info_DataList.SetItemsBold = false;
            info_DataList.Size = new Size(436, 155);
            info_DataList.TabIndex = 79;
            info_DataList.UseHighlighting = true;
            info_DataList.ValueAlternateColor = Color.Chartreuse;
            info_DataList.ValueColor = Color.Azure;
            info_DataList.ValueConditionColor = Color.Firebrick;
            info_DataList.ValueSpecialColor = Color.SlateBlue;
            // 
            // panelSeparator1
            // 
            panelSeparator1.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator1.Dock = DockStyle.Top;
            panelSeparator1.ForeColor = Color.WhiteSmoke;
            panelSeparator1.Location = new Point(0, 39);
            panelSeparator1.Name = "panelSeparator1";
            panelSeparator1.Size = new Size(436, 3);
            panelSeparator1.TabIndex = 83;
            // 
            // panelSeparator2
            // 
            panelSeparator2.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator2.Dock = DockStyle.Top;
            panelSeparator2.ForeColor = Color.WhiteSmoke;
            panelSeparator2.Location = new Point(0, 85);
            panelSeparator2.Name = "panelSeparator2";
            panelSeparator2.Size = new Size(436, 3);
            panelSeparator2.TabIndex = 82;
            // 
            // panelSeparator3
            // 
            panelSeparator3.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator3.Dock = DockStyle.Top;
            panelSeparator3.ForeColor = Color.WhiteSmoke;
            panelSeparator3.Location = new Point(0, 189);
            panelSeparator3.Name = "panelSeparator3";
            panelSeparator3.Size = new Size(436, 3);
            panelSeparator3.TabIndex = 81;
            // 
            // panelSeparator4
            // 
            panelSeparator4.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator4.Dock = DockStyle.Top;
            panelSeparator4.ForeColor = Color.WhiteSmoke;
            panelSeparator4.Location = new Point(0, 212);
            panelSeparator4.Name = "panelSeparator4";
            panelSeparator4.Size = new Size(436, 3);
            panelSeparator4.TabIndex = 80;
            // 
            // panelSeparator5
            // 
            panelSeparator5.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator5.Dock = DockStyle.Top;
            panelSeparator5.ForeColor = Color.WhiteSmoke;
            panelSeparator5.Location = new Point(0, 370);
            panelSeparator5.Name = "panelSeparator5";
            panelSeparator5.Size = new Size(436, 3);
            panelSeparator5.TabIndex = 23;
            // 
            // panelSeparator6
            // 
            panelSeparator6.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator6.Dock = DockStyle.Top;
            panelSeparator6.ForeColor = Color.WhiteSmoke;
            panelSeparator6.Location = new Point(0, 399);
            panelSeparator6.Name = "panelSeparator6";
            panelSeparator6.Size = new Size(436, 3);
            panelSeparator6.TabIndex = 22;
            // 
            // containerPanel
            // 
            containerPanel.BackColor = Color.Black;
            containerPanel.Controls.Add(info_Damage);
            containerPanel.Controls.Add(panelSeparator6);
            containerPanel.Controls.Add(lblDmg);
            containerPanel.Controls.Add(panelSeparator5);
            containerPanel.Controls.Add(info_DataList);
            containerPanel.Controls.Add(panelSeparator4);
            containerPanel.Controls.Add(powerScaler);
            containerPanel.Controls.Add(panelSeparator3);
            containerPanel.Controls.Add(info_TxtLarge);
            containerPanel.Controls.Add(panelSeparator2);
            containerPanel.Controls.Add(info_TxtSmall);
            containerPanel.Controls.Add(panelSeparator1);
            containerPanel.Controls.Add(info_Title);
            containerPanel.Location = new Point(3, 3);
            containerPanel.Name = "containerPanel";
            containerPanel.Size = new Size(436, 513);
            containerPanel.TabIndex = 0;
            // 
            // PetView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(12, 56, 100);
            Controls.Add(containerPanel);
            Name = "PetView";
            Size = new Size(442, 519);
            containerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        internal ctlDamageDisplay info_Damage;
        private Label lblDmg;
        private CtlMultiGraph powerScaler;
        internal RichTextBox info_TxtLarge;
        private RichTextBox info_TxtSmall;
        private Label info_Title;
        public ToolTip infoTip;
        internal Panel panelSeparator1;
        internal Panel panelSeparator2;
        internal Panel panelSeparator3;
        internal Panel panelSeparator4;
        internal Panel panelSeparator5;
        internal Panel panelSeparator6;
        private Panel containerPanel;
        private PairedListEx info_DataList;
    }
}
