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
            infoTip = new ToolTip(components);
            formPages1 = new FormPages();
            page1 = new Page();
            panel1 = new Panel();
            info_Damage = new ctlDamageDisplay();
            panelSeparator12 = new Panel();
            lblDmg = new Label();
            panelSeparator11 = new Panel();
            info_DataList = new PairedListEx();
            panelSeparator10 = new Panel();
            powerScaler = new CtlMultiGraph();
            panelSeparator9 = new Panel();
            info_TxtLarge = new RichTextBox();
            panelSeparator8 = new Panel();
            info_TxtSmall = new RichTextBox();
            panelSeparator7 = new Panel();
            info_Title = new Label();
            page2 = new Page();
            containerPanel = new Panel();
            fx_List3 = new PairedListEx();
            panelSeparator6 = new Panel();
            fx_LblHead3 = new Label();
            panelSeparator5 = new Panel();
            fx_List2 = new PairedListEx();
            panelSeparator4 = new Panel();
            fx_lblHead2 = new Label();
            panelSeparator3 = new Panel();
            fx_List1 = new PairedListEx();
            panelSeparator2 = new Panel();
            fx_lblHead1 = new Label();
            panelSeparator1 = new Panel();
            fx_Title = new Label();
            navStrip1 = new NavStrip();
            borderPanel1 = new BorderPanel();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            panel1.SuspendLayout();
            page2.SuspendLayout();
            containerPanel.SuspendLayout();
            borderPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // infoTip
            // 
            infoTip.AutoPopDelay = 15000;
            infoTip.InitialDelay = 350;
            infoTip.ReshowDelay = 100;
            // 
            // formPages1
            // 
            formPages1.Controls.Add(page1);
            formPages1.Controls.Add(page2);
            formPages1.Dock = DockStyle.Fill;
            formPages1.Location = new Point(0, 0);
            formPages1.Name = "formPages1";
            formPages1.Padding = new Padding(3);
            formPages1.Pages.Add(page1);
            formPages1.Pages.Add(page2);
            formPages1.SelectedIndex = 0;
            formPages1.Size = new Size(436, 529);
            formPages1.TabIndex = 1;
            // 
            // page1
            // 
            page1.AccessibleRole = AccessibleRole.None;
            page1.Anchor = AnchorStyles.None;
            page1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page1.BackColor = Color.FromArgb(12, 56, 100);
            page1.Controls.Add(panel1);
            page1.Dock = DockStyle.Fill;
            page1.ForeColor = Color.WhiteSmoke;
            page1.Location = new Point(3, 3);
            page1.Name = "page1";
            page1.Size = new Size(430, 523);
            page1.TabIndex = 0;
            page1.Title = "Info";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(info_Damage);
            panel1.Controls.Add(panelSeparator12);
            panel1.Controls.Add(lblDmg);
            panel1.Controls.Add(panelSeparator11);
            panel1.Controls.Add(info_DataList);
            panel1.Controls.Add(panelSeparator10);
            panel1.Controls.Add(powerScaler);
            panel1.Controls.Add(panelSeparator9);
            panel1.Controls.Add(info_TxtLarge);
            panel1.Controls.Add(panelSeparator8);
            panel1.Controls.Add(info_TxtSmall);
            panel1.Controls.Add(panelSeparator7);
            panel1.Controls.Add(info_Title);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(430, 523);
            panel1.TabIndex = 5;
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
            info_Damage.Size = new Size(430, 98);
            info_Damage.Style = Core.Enums.eDDStyle.TextUnderGraph;
            info_Damage.TabIndex = 21;
            info_Damage.TextAlign = Core.Enums.eDDAlign.Center;
            info_Damage.TextColor = Color.WhiteSmoke;
            // 
            // panelSeparator12
            // 
            panelSeparator12.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator12.Dock = DockStyle.Top;
            panelSeparator12.ForeColor = Color.WhiteSmoke;
            panelSeparator12.Location = new Point(0, 399);
            panelSeparator12.Name = "panelSeparator12";
            panelSeparator12.Size = new Size(430, 3);
            panelSeparator12.TabIndex = 22;
            // 
            // lblDmg
            // 
            lblDmg.Dock = DockStyle.Top;
            lblDmg.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblDmg.ForeColor = Color.WhiteSmoke;
            lblDmg.Location = new Point(0, 373);
            lblDmg.Name = "lblDmg";
            lblDmg.Size = new Size(430, 26);
            lblDmg.TabIndex = 22;
            lblDmg.Text = "Damage (Blue = Base | Gold = Enhanced)";
            lblDmg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelSeparator11
            // 
            panelSeparator11.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator11.Dock = DockStyle.Top;
            panelSeparator11.ForeColor = Color.WhiteSmoke;
            panelSeparator11.Location = new Point(0, 370);
            panelSeparator11.Name = "panelSeparator11";
            panelSeparator11.Size = new Size(430, 3);
            panelSeparator11.TabIndex = 23;
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
            info_DataList.Size = new Size(430, 155);
            info_DataList.TabIndex = 79;
            info_DataList.UseHighlighting = true;
            info_DataList.ValueAlternateColor = Color.Chartreuse;
            info_DataList.ValueColor = Color.Azure;
            info_DataList.ValueConditionColor = Color.Firebrick;
            info_DataList.ValueSpecialColor = Color.SlateBlue;
            info_DataList.ItemHover += PairedList_Hover;
            info_DataList.ItemOut += PairedList_ItemOut;
            // 
            // panelSeparator10
            // 
            panelSeparator10.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator10.Dock = DockStyle.Top;
            panelSeparator10.ForeColor = Color.WhiteSmoke;
            panelSeparator10.Location = new Point(0, 212);
            panelSeparator10.Name = "panelSeparator10";
            panelSeparator10.Size = new Size(430, 3);
            panelSeparator10.TabIndex = 80;
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
            powerScaler.Size = new Size(430, 20);
            powerScaler.Style = Core.Enums.GraphStyle.baseOnly;
            powerScaler.TabIndex = 72;
            powerScaler.TextWidth = 100;
            powerScaler.BarClick += powerScaler_BarClick;
            // 
            // panelSeparator9
            // 
            panelSeparator9.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator9.Dock = DockStyle.Top;
            panelSeparator9.ForeColor = Color.WhiteSmoke;
            panelSeparator9.Location = new Point(0, 189);
            panelSeparator9.Name = "panelSeparator9";
            panelSeparator9.Size = new Size(430, 3);
            panelSeparator9.TabIndex = 81;
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
            info_TxtLarge.Size = new Size(430, 101);
            info_TxtLarge.TabIndex = 73;
            info_TxtLarge.Text = "info_Rich";
            // 
            // panelSeparator8
            // 
            panelSeparator8.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator8.Dock = DockStyle.Top;
            panelSeparator8.ForeColor = Color.WhiteSmoke;
            panelSeparator8.Location = new Point(0, 85);
            panelSeparator8.Name = "panelSeparator8";
            panelSeparator8.Size = new Size(430, 3);
            panelSeparator8.TabIndex = 82;
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
            info_TxtSmall.Size = new Size(430, 43);
            info_TxtSmall.TabIndex = 74;
            info_TxtSmall.Text = "info_Rich";
            // 
            // panelSeparator7
            // 
            panelSeparator7.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator7.Dock = DockStyle.Top;
            panelSeparator7.ForeColor = Color.WhiteSmoke;
            panelSeparator7.Location = new Point(0, 39);
            panelSeparator7.Name = "panelSeparator7";
            panelSeparator7.Size = new Size(430, 3);
            panelSeparator7.TabIndex = 83;
            // 
            // info_Title
            // 
            info_Title.BackColor = Color.Black;
            info_Title.Dock = DockStyle.Top;
            info_Title.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            info_Title.ForeColor = Color.White;
            info_Title.Location = new Point(0, 0);
            info_Title.Name = "info_Title";
            info_Title.Size = new Size(430, 39);
            info_Title.TabIndex = 75;
            info_Title.Text = "Title";
            info_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // page2
            // 
            page2.AccessibleRole = AccessibleRole.None;
            page2.Anchor = AnchorStyles.None;
            page2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page2.BackColor = Color.FromArgb(12, 56, 100);
            page2.Controls.Add(containerPanel);
            page2.Dock = DockStyle.Fill;
            page2.ForeColor = Color.WhiteSmoke;
            page2.Location = new Point(3, 3);
            page2.Name = "page2";
            page2.Size = new Size(430, 523);
            page2.TabIndex = 1;
            page2.Title = "Effects";
            // 
            // containerPanel
            // 
            containerPanel.BackColor = Color.Black;
            containerPanel.Controls.Add(fx_List3);
            containerPanel.Controls.Add(panelSeparator6);
            containerPanel.Controls.Add(fx_LblHead3);
            containerPanel.Controls.Add(panelSeparator5);
            containerPanel.Controls.Add(fx_List2);
            containerPanel.Controls.Add(panelSeparator4);
            containerPanel.Controls.Add(fx_lblHead2);
            containerPanel.Controls.Add(panelSeparator3);
            containerPanel.Controls.Add(fx_List1);
            containerPanel.Controls.Add(panelSeparator2);
            containerPanel.Controls.Add(fx_lblHead1);
            containerPanel.Controls.Add(panelSeparator1);
            containerPanel.Controls.Add(fx_Title);
            containerPanel.Dock = DockStyle.Fill;
            containerPanel.Location = new Point(0, 0);
            containerPanel.Name = "containerPanel";
            containerPanel.Size = new Size(430, 523);
            containerPanel.TabIndex = 4;
            // 
            // fx_List3
            // 
            fx_List3.AutoSizeLineHeight = true;
            fx_List3.BackColor = Color.Black;
            fx_List3.Columns = 2;
            fx_List3.Dock = DockStyle.Top;
            fx_List3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            fx_List3.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List3.HighlightTextColor = Color.Black;
            fx_List3.ItemColor = Color.White;
            fx_List3.Location = new Point(0, 375);
            fx_List3.Margin = new Padding(4);
            fx_List3.Name = "fx_List3";
            fx_List3.Rows = 5;
            fx_List3.SetItemsBold = false;
            fx_List3.Size = new Size(430, 125);
            fx_List3.TabIndex = 89;
            fx_List3.UseHighlighting = true;
            fx_List3.ValueAlternateColor = Color.Chartreuse;
            fx_List3.ValueColor = Color.WhiteSmoke;
            fx_List3.ValueConditionColor = Color.Firebrick;
            fx_List3.ValueSpecialColor = Color.SlateBlue;
            fx_List3.ItemHover += PairedList_Hover;
            fx_List3.ItemOut += PairedList_ItemOut;
            // 
            // panelSeparator6
            // 
            panelSeparator6.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator6.Dock = DockStyle.Top;
            panelSeparator6.ForeColor = Color.WhiteSmoke;
            panelSeparator6.Location = new Point(0, 372);
            panelSeparator6.Name = "panelSeparator6";
            panelSeparator6.Size = new Size(430, 3);
            panelSeparator6.TabIndex = 22;
            // 
            // fx_LblHead3
            // 
            fx_LblHead3.Dock = DockStyle.Top;
            fx_LblHead3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            fx_LblHead3.ForeColor = Color.White;
            fx_LblHead3.Location = new Point(0, 351);
            fx_LblHead3.Name = "fx_LblHead3";
            fx_LblHead3.Size = new Size(430, 21);
            fx_LblHead3.TabIndex = 88;
            fx_LblHead3.Text = "Misc Effects:";
            fx_LblHead3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelSeparator5
            // 
            panelSeparator5.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator5.Dock = DockStyle.Top;
            panelSeparator5.ForeColor = Color.WhiteSmoke;
            panelSeparator5.Location = new Point(0, 348);
            panelSeparator5.Name = "panelSeparator5";
            panelSeparator5.Size = new Size(430, 3);
            panelSeparator5.TabIndex = 23;
            // 
            // fx_List2
            // 
            fx_List2.AutoSizeLineHeight = true;
            fx_List2.BackColor = Color.Black;
            fx_List2.Columns = 2;
            fx_List2.Dock = DockStyle.Top;
            fx_List2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            fx_List2.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List2.HighlightTextColor = Color.Black;
            fx_List2.ItemColor = Color.White;
            fx_List2.Location = new Point(0, 223);
            fx_List2.Margin = new Padding(4);
            fx_List2.Name = "fx_List2";
            fx_List2.Rows = 5;
            fx_List2.SetItemsBold = false;
            fx_List2.Size = new Size(430, 125);
            fx_List2.TabIndex = 87;
            fx_List2.UseHighlighting = true;
            fx_List2.ValueAlternateColor = Color.Chartreuse;
            fx_List2.ValueColor = Color.WhiteSmoke;
            fx_List2.ValueConditionColor = Color.Firebrick;
            fx_List2.ValueSpecialColor = Color.SlateBlue;
            fx_List2.ItemHover += PairedList_Hover;
            fx_List2.ItemOut += PairedList_ItemOut;
            // 
            // panelSeparator4
            // 
            panelSeparator4.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator4.Dock = DockStyle.Top;
            panelSeparator4.ForeColor = Color.WhiteSmoke;
            panelSeparator4.Location = new Point(0, 220);
            panelSeparator4.Name = "panelSeparator4";
            panelSeparator4.Size = new Size(430, 3);
            panelSeparator4.TabIndex = 80;
            // 
            // fx_lblHead2
            // 
            fx_lblHead2.Dock = DockStyle.Top;
            fx_lblHead2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            fx_lblHead2.ForeColor = Color.White;
            fx_lblHead2.Location = new Point(0, 195);
            fx_lblHead2.Name = "fx_lblHead2";
            fx_lblHead2.Size = new Size(430, 25);
            fx_lblHead2.TabIndex = 86;
            fx_lblHead2.Text = "Secondary Effects:";
            fx_lblHead2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelSeparator3
            // 
            panelSeparator3.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator3.Dock = DockStyle.Top;
            panelSeparator3.ForeColor = Color.WhiteSmoke;
            panelSeparator3.Location = new Point(0, 192);
            panelSeparator3.Name = "panelSeparator3";
            panelSeparator3.Size = new Size(430, 3);
            panelSeparator3.TabIndex = 81;
            // 
            // fx_List1
            // 
            fx_List1.AutoSizeLineHeight = true;
            fx_List1.BackColor = Color.Black;
            fx_List1.Columns = 2;
            fx_List1.Dock = DockStyle.Top;
            fx_List1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            fx_List1.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List1.HighlightTextColor = Color.Black;
            fx_List1.ItemColor = Color.White;
            fx_List1.Location = new Point(0, 67);
            fx_List1.Margin = new Padding(4);
            fx_List1.Name = "fx_List1";
            fx_List1.Rows = 5;
            fx_List1.SetItemsBold = false;
            fx_List1.Size = new Size(430, 125);
            fx_List1.TabIndex = 84;
            fx_List1.UseHighlighting = true;
            fx_List1.ValueAlternateColor = Color.Chartreuse;
            fx_List1.ValueColor = Color.WhiteSmoke;
            fx_List1.ValueConditionColor = Color.Firebrick;
            fx_List1.ValueSpecialColor = Color.SlateBlue;
            fx_List1.ItemHover += PairedList_Hover;
            fx_List1.ItemOut += PairedList_ItemOut;
            // 
            // panelSeparator2
            // 
            panelSeparator2.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator2.Dock = DockStyle.Top;
            panelSeparator2.ForeColor = Color.WhiteSmoke;
            panelSeparator2.Location = new Point(0, 64);
            panelSeparator2.Name = "panelSeparator2";
            panelSeparator2.Size = new Size(430, 3);
            panelSeparator2.TabIndex = 82;
            // 
            // fx_lblHead1
            // 
            fx_lblHead1.Dock = DockStyle.Top;
            fx_lblHead1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            fx_lblHead1.ForeColor = Color.White;
            fx_lblHead1.Location = new Point(0, 42);
            fx_lblHead1.Name = "fx_lblHead1";
            fx_lblHead1.Size = new Size(430, 22);
            fx_lblHead1.TabIndex = 85;
            fx_lblHead1.Text = "Primary Effects:";
            fx_lblHead1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelSeparator1
            // 
            panelSeparator1.BackColor = Color.FromArgb(7, 33, 59);
            panelSeparator1.Dock = DockStyle.Top;
            panelSeparator1.ForeColor = Color.WhiteSmoke;
            panelSeparator1.Location = new Point(0, 39);
            panelSeparator1.Name = "panelSeparator1";
            panelSeparator1.Size = new Size(430, 3);
            panelSeparator1.TabIndex = 83;
            // 
            // fx_Title
            // 
            fx_Title.BackColor = Color.Black;
            fx_Title.Dock = DockStyle.Top;
            fx_Title.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            fx_Title.ForeColor = Color.White;
            fx_Title.Location = new Point(0, 0);
            fx_Title.Name = "fx_Title";
            fx_Title.Size = new Size(430, 39);
            fx_Title.TabIndex = 75;
            fx_Title.Text = "Title";
            fx_Title.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // navStrip1
            // 
            navStrip1.ActiveTabColor = Color.Goldenrod;
            navStrip1.DataSource = formPages1;
            navStrip1.DimmedColor = Color.FromArgb(21, 61, 93);
            navStrip1.DisabledTabColor = Color.DarkGray;
            navStrip1.Dock = DockStyle.Top;
            navStrip1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            navStrip1.InactiveTabColor = Color.FromArgb(30, 85, 130);
            navStrip1.InactiveTabHoverColor = Color.FromArgb(43, 122, 187);
            navStrip1.Location = new Point(0, 0);
            navStrip1.Margin = new Padding(4, 4, 4, 4);
            navStrip1.Name = "navStrip1";
            navStrip1.OutlineColor = Color.Black;
            navStrip1.Size = new Size(436, 41);
            navStrip1.TabIndex = 2;
            navStrip1.Theme = NavStrip.ThemeColor.Hero;
            navStrip1.UseTheme = true;
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = Color.FromArgb(12, 56, 100);
            borderPanel1.Border.Style = ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(formPages1);
            borderPanel1.Dock = DockStyle.Fill;
            borderPanel1.Location = new Point(0, 41);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new Size(436, 529);
            borderPanel1.TabIndex = 85;
            // 
            // PetView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(12, 56, 100);
            Controls.Add(borderPanel1);
            Controls.Add(navStrip1);
            Name = "PetView";
            Size = new Size(436, 570);
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            page2.ResumeLayout(false);
            containerPanel.ResumeLayout(false);
            borderPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public ToolTip infoTip;
        private FormPages formPages1;
        private Page page1;
        private Page page2;
        private NavStrip navStrip1;
        private Panel panel1;
        internal ctlDamageDisplay info_Damage;
        internal Panel panelSeparator12;
        private Label lblDmg;
        internal Panel panelSeparator11;
        private PairedListEx info_DataList;
        internal Panel panelSeparator10;
        private CtlMultiGraph powerScaler;
        internal Panel panelSeparator9;
        internal RichTextBox info_TxtLarge;
        internal Panel panelSeparator8;
        private RichTextBox info_TxtSmall;
        internal Panel panelSeparator7;
        private Label info_Title;
        private Panel containerPanel;
        private PairedListEx fx_List3;
        internal Panel panelSeparator6;
        private Label fx_LblHead3;
        internal Panel panelSeparator5;
        private PairedListEx fx_List2;
        internal Panel panelSeparator4;
        private Label fx_lblHead2;
        internal Panel panelSeparator3;
        private PairedListEx fx_List1;
        internal Panel panelSeparator2;
        private Label fx_lblHead1;
        internal Panel panelSeparator1;
        private Label fx_Title;
        private BorderPanel borderPanel1;
    }
}
