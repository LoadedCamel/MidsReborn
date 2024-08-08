using System.ComponentModel;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    partial class frmTotalsV2
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmTotalsV2));
            panel2 = new Panel();
            ibClose = new ImageButtonEx();
            ibTopMost = new ImageButtonEx();
            panelTab1 = new Panel();
            graphEnd = new CtlMultiGraph();
            graphHP = new CtlMultiGraph();
            graphRes = new CtlMultiGraph();
            graphDef = new CtlMultiGraph();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            panelTab2 = new Panel();
            graphRange = new CtlMultiGraph();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();
            graphThreat = new CtlMultiGraph();
            graphEndRdx = new CtlMultiGraph();
            graphDamage = new CtlMultiGraph();
            graphAccuracy = new CtlMultiGraph();
            graphToHit = new CtlMultiGraph();
            graphHaste = new CtlMultiGraph();
            graphPerception = new CtlMultiGraph();
            graphMovement = new CtlMultiGraph();
            panelTab3 = new Panel();
            label9 = new Label();
            label8 = new Label();
            graphStatusRes = new CtlMultiGraph();
            graphStatusProt = new CtlMultiGraph();
            panelTab4 = new Panel();
            label11 = new Label();
            label10 = new Label();
            graphElusivity = new CtlMultiGraph();
            graphDebuffRes = new CtlMultiGraph();
            panel1 = new Panel();
            ctlTotalsTabStrip1 = new ctlTotalsTabStrip();
            panel2.SuspendLayout();
            panelTab1.SuspendLayout();
            panelTab2.SuspendLayout();
            panelTab3.SuspendLayout();
            panelTab4.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.Black;
            panel2.Controls.Add(ibClose);
            panel2.Controls.Add(ibTopMost);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 686);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(2267, 44);
            panel2.TabIndex = 2;
            // 
            // ibClose
            // 
            ibClose.BackgroundImageLayout = ImageLayout.None;
            ibClose.CurrentText = "Close";
            ibClose.DisplayVertically = false;
            ibClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibClose.Location = new System.Drawing.Point(416, 8);
            ibClose.Lock = false;
            ibClose.Name = "ibClose";
            ibClose.Size = new System.Drawing.Size(122, 27);
            ibClose.TabIndex = 1;
            ibClose.Text = "Close";
            ibClose.TextOutline.Color = System.Drawing.Color.Black;
            ibClose.TextOutline.Width = 2;
            ibClose.ToggleState = ImageButtonEx.States.ToggledOff;
            ibClose.ToggleText.Indeterminate = "Indeterminate State";
            ibClose.ToggleText.ToggledOff = "ToggledOff State";
            ibClose.ToggleText.ToggledOn = "ToggledOn State";
            ibClose.UseAlt = false;
            ibClose.Click += IbCloseClick;
            // 
            // ibTopMost
            // 
            ibTopMost.BackgroundImageLayout = ImageLayout.None;
            ibTopMost.ButtonType = ImageButtonEx.ButtonTypes.Toggle;
            ibTopMost.CurrentText = "ToggledOff State";
            ibTopMost.DisplayVertically = false;
            ibTopMost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibTopMost.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibTopMost.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibTopMost.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibTopMost.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibTopMost.Location = new System.Drawing.Point(10, 8);
            ibTopMost.Lock = false;
            ibTopMost.Name = "ibTopMost";
            ibTopMost.Size = new System.Drawing.Size(122, 27);
            ibTopMost.TabIndex = 0;
            ibTopMost.Text = "Keep on Top";
            ibTopMost.TextOutline.Color = System.Drawing.Color.Black;
            ibTopMost.TextOutline.Width = 2;
            ibTopMost.ToggleState = ImageButtonEx.States.ToggledOff;
            ibTopMost.ToggleText.Indeterminate = "Indeterminate State";
            ibTopMost.ToggleText.ToggledOff = "To Top";
            ibTopMost.ToggleText.ToggledOn = "Keep on Top";
            ibTopMost.UseAlt = false;
            ibTopMost.Click += IbTopMostClick;
            // 
            // panelTab1
            // 
            panelTab1.BackColor = System.Drawing.Color.Black;
            panelTab1.Controls.Add(graphEnd);
            panelTab1.Controls.Add(graphHP);
            panelTab1.Controls.Add(graphRes);
            panelTab1.Controls.Add(graphDef);
            panelTab1.Controls.Add(label1);
            panelTab1.Controls.Add(label2);
            panelTab1.Controls.Add(label3);
            panelTab1.Controls.Add(label4);
            panelTab1.Location = new System.Drawing.Point(6, 30);
            panelTab1.Name = "panelTab1";
            panelTab1.Size = new System.Drawing.Size(550, 650);
            panelTab1.TabIndex = 101;
            panelTab1.Visible = false;
            // 
            // graphEnd
            // 
            graphEnd.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphEnd.BackgroundImage");
            graphEnd.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEnd.BaseBarColors");
            graphEnd.Border = true;
            graphEnd.BorderColor = System.Drawing.Color.RoyalBlue;
            graphEnd.Clickable = false;
            graphEnd.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphEnd.ColorBase = System.Drawing.Color.Blue;
            graphEnd.ColorEnh = System.Drawing.Color.Yellow;
            graphEnd.ColorFadeEnd = System.Drawing.Color.FromArgb(65, 105, 224);
            graphEnd.ColorFadeStart = System.Drawing.Color.Black;
            graphEnd.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphEnd.ColorLines = System.Drawing.Color.Black;
            graphEnd.ColorMarkerInner = System.Drawing.Color.Black;
            graphEnd.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphEnd.ColorOvercap = System.Drawing.Color.Black;
            graphEnd.DifferentiateColors = true;
            graphEnd.DrawRuler = false;
            graphEnd.Dual = true;
            graphEnd.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEnd.EnhBarColors");
            graphEnd.ForcedMax = 0F;
            graphEnd.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphEnd.Highlight = true;
            graphEnd.ItemFontSizeOverride = 0F;
            graphEnd.ItemHeight = 13;
            graphEnd.Lines = true;
            graphEnd.Location = new System.Drawing.Point(12, 574);
            graphEnd.MarkerValue = 0F;
            graphEnd.Max = 100F;
            graphEnd.MaxItems = 3;
            graphEnd.Name = "graphEnd";
            graphEnd.OuterBorder = true;
            graphEnd.Overcap = true;
            graphEnd.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEnd.OvercapColors");
            graphEnd.PaddingX = 4F;
            graphEnd.PaddingY = 6F;
            graphEnd.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphEnd.PerItemScales");
            graphEnd.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphEnd.ScaleHeight = 32;
            graphEnd.ScaleIndex = 8;
            graphEnd.ShowScale = false;
            graphEnd.Size = new System.Drawing.Size(526, 65);
            graphEnd.Style = Core.Enums.GraphStyle.Stacked;
            graphEnd.TabIndex = 116;
            graphEnd.TextWidth = 187;
            // 
            // graphHP
            // 
            graphHP.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphHP.BackgroundImage");
            graphHP.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.BaseBarColors");
            graphHP.Border = true;
            graphHP.BorderColor = System.Drawing.Color.PaleGreen;
            graphHP.Clickable = false;
            graphHP.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphHP.ColorBase = System.Drawing.Color.FromArgb(51, 204, 51);
            graphHP.ColorEnh = System.Drawing.Color.FromArgb(64, 255, 64);
            graphHP.ColorFadeEnd = System.Drawing.Color.FromArgb(150, 251, 150);
            graphHP.ColorFadeStart = System.Drawing.Color.Black;
            graphHP.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphHP.ColorLines = System.Drawing.Color.Black;
            graphHP.ColorMarkerInner = System.Drawing.Color.Black;
            graphHP.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphHP.ColorOvercap = System.Drawing.Color.Black;
            graphHP.DifferentiateColors = true;
            graphHP.DrawRuler = false;
            graphHP.Dual = true;
            graphHP.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.EnhBarColors");
            graphHP.ForcedMax = 0F;
            graphHP.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphHP.Highlight = true;
            graphHP.ItemFontSizeOverride = 0F;
            graphHP.ItemHeight = 13;
            graphHP.Lines = true;
            graphHP.Location = new System.Drawing.Point(12, 487);
            graphHP.MarkerValue = 0F;
            graphHP.Max = 4000F;
            graphHP.MaxItems = 2;
            graphHP.Name = "graphHP";
            graphHP.OuterBorder = true;
            graphHP.Overcap = true;
            graphHP.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.OvercapColors");
            graphHP.PaddingX = 4F;
            graphHP.PaddingY = 6F;
            graphHP.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphHP.PerItemScales");
            graphHP.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphHP.ScaleHeight = 32;
            graphHP.ScaleIndex = 19;
            graphHP.ShowScale = false;
            graphHP.Size = new System.Drawing.Size(526, 46);
            graphHP.Style = Core.Enums.GraphStyle.Stacked;
            graphHP.TabIndex = 115;
            graphHP.TabStop = false;
            graphHP.TextWidth = 187;
            // 
            // graphRes
            // 
            graphRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRes.BackgroundImage");
            graphRes.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.BaseBarColors");
            graphRes.Border = true;
            graphRes.BorderColor = System.Drawing.Color.LightSeaGreen;
            graphRes.Clickable = false;
            graphRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphRes.ColorBase = System.Drawing.Color.FromArgb(0, 160, 160);
            graphRes.ColorEnh = System.Drawing.Color.FromArgb(0, 192, 192);
            graphRes.ColorFadeEnd = System.Drawing.Color.LightSeaGreen;
            graphRes.ColorFadeStart = System.Drawing.Color.Black;
            graphRes.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphRes.ColorLines = System.Drawing.Color.Black;
            graphRes.ColorMarkerInner = System.Drawing.Color.Black;
            graphRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphRes.ColorOvercap = System.Drawing.Color.FromArgb(255, 128, 128);
            graphRes.DifferentiateColors = false;
            graphRes.DrawRuler = false;
            graphRes.Dual = true;
            graphRes.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.EnhBarColors");
            graphRes.ForcedMax = 0F;
            graphRes.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphRes.Highlight = true;
            graphRes.ItemFontSizeOverride = 0F;
            graphRes.ItemHeight = 13;
            graphRes.Lines = true;
            graphRes.Location = new System.Drawing.Point(12, 288);
            graphRes.MarkerValue = 0F;
            graphRes.Max = 100F;
            graphRes.MaxItems = 8;
            graphRes.Name = "graphRes";
            graphRes.OuterBorder = true;
            graphRes.Overcap = true;
            graphRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.OvercapColors");
            graphRes.PaddingX = 4F;
            graphRes.PaddingY = 6F;
            graphRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRes.PerItemScales");
            graphRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRes.ScaleHeight = 32;
            graphRes.ScaleIndex = 8;
            graphRes.ShowScale = false;
            graphRes.Size = new System.Drawing.Size(526, 160);
            graphRes.Style = Core.Enums.GraphStyle.Stacked;
            graphRes.TabIndex = 114;
            graphRes.TabStop = false;
            graphRes.TextWidth = 187;
            // 
            // graphDef
            // 
            graphDef.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDef.BackgroundImage");
            graphDef.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.BaseBarColors");
            graphDef.Border = true;
            graphDef.BorderColor = System.Drawing.Color.BlueViolet;
            graphDef.Clickable = false;
            graphDef.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDef.ColorBase = System.Drawing.Color.Magenta;
            graphDef.ColorEnh = System.Drawing.Color.Magenta;
            graphDef.ColorFadeEnd = System.Drawing.Color.Purple;
            graphDef.ColorFadeStart = System.Drawing.Color.Black;
            graphDef.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphDef.ColorLines = System.Drawing.Color.Black;
            graphDef.ColorMarkerInner = System.Drawing.Color.Black;
            graphDef.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDef.ColorOvercap = System.Drawing.Color.Black;
            graphDef.DifferentiateColors = false;
            graphDef.DrawRuler = false;
            graphDef.Dual = true;
            graphDef.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.EnhBarColors");
            graphDef.ForcedMax = 0F;
            graphDef.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphDef.Highlight = true;
            graphDef.ItemFontSizeOverride = 0F;
            graphDef.ItemHeight = 13;
            graphDef.Lines = true;
            graphDef.Location = new System.Drawing.Point(12, 32);
            graphDef.MarkerValue = 0F;
            graphDef.Max = 100F;
            graphDef.MaxItems = 11;
            graphDef.Name = "graphDef";
            graphDef.OuterBorder = true;
            graphDef.Overcap = false;
            graphDef.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.OvercapColors");
            graphDef.PaddingX = 4F;
            graphDef.PaddingY = 6F;
            graphDef.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDef.PerItemScales");
            graphDef.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDef.ScaleHeight = 32;
            graphDef.ScaleIndex = 8;
            graphDef.ShowScale = false;
            graphDef.Size = new System.Drawing.Size(526, 217);
            graphDef.Style = Core.Enums.GraphStyle.enhOnly;
            graphDef.TabIndex = 113;
            graphDef.TabStop = false;
            graphDef.TextWidth = 187;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.Gainsboro;
            label1.Location = new System.Drawing.Point(16, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 20);
            label1.TabIndex = 117;
            label1.Text = "Defense:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.Transparent;
            label2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.Gainsboro;
            label2.Location = new System.Drawing.Point(16, 264);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(87, 20);
            label2.TabIndex = 118;
            label2.Text = "Resistance:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = System.Drawing.Color.Transparent;
            label3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.ForeColor = System.Drawing.Color.Gainsboro;
            label3.Location = new System.Drawing.Point(16, 463);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(59, 20);
            label3.TabIndex = 119;
            label3.Text = "Health:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label4.ForeColor = System.Drawing.Color.Gainsboro;
            label4.Location = new System.Drawing.Point(16, 549);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(86, 20);
            label4.TabIndex = 120;
            label4.Text = "Endurance:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTab2
            // 
            panelTab2.Controls.Add(graphRange);
            panelTab2.Controls.Add(label7);
            panelTab2.Controls.Add(label6);
            panelTab2.Controls.Add(label5);
            panelTab2.Controls.Add(radioButton1);
            panelTab2.Controls.Add(radioButton2);
            panelTab2.Controls.Add(radioButton3);
            panelTab2.Controls.Add(radioButton4);
            panelTab2.Controls.Add(graphThreat);
            panelTab2.Controls.Add(graphEndRdx);
            panelTab2.Controls.Add(graphDamage);
            panelTab2.Controls.Add(graphAccuracy);
            panelTab2.Controls.Add(graphToHit);
            panelTab2.Controls.Add(graphHaste);
            panelTab2.Controls.Add(graphPerception);
            panelTab2.Controls.Add(graphMovement);
            panelTab2.Location = new System.Drawing.Point(574, 30);
            panelTab2.Name = "panelTab2";
            panelTab2.Size = new System.Drawing.Size(550, 650);
            panelTab2.TabIndex = 102;
            panelTab2.Visible = false;
            // 
            // graphRange
            // 
            graphRange.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRange.BackgroundImage");
            graphRange.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRange.BaseBarColors");
            graphRange.Border = true;
            graphRange.BorderColor = System.Drawing.Color.FromArgb(206, 196, 132);
            graphRange.Clickable = false;
            graphRange.ColorAbsorbed = System.Drawing.Color.FromArgb(229, 218, 147);
            graphRange.ColorBase = System.Drawing.Color.FromArgb(151, 144, 97);
            graphRange.ColorEnh = System.Drawing.Color.FromArgb(206, 196, 132);
            graphRange.ColorFadeEnd = System.Drawing.Color.FromArgb(112, 107, 72);
            graphRange.ColorFadeStart = System.Drawing.Color.Black;
            graphRange.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphRange.ColorLines = System.Drawing.Color.Black;
            graphRange.ColorMarkerInner = System.Drawing.Color.Black;
            graphRange.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphRange.ColorOvercap = System.Drawing.Color.FromArgb(92, 88, 59);
            graphRange.DifferentiateColors = false;
            graphRange.DrawRuler = false;
            graphRange.Dual = true;
            graphRange.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRange.EnhBarColors");
            graphRange.ForcedMax = 0F;
            graphRange.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphRange.Highlight = true;
            graphRange.ItemFontSizeOverride = 0F;
            graphRange.ItemHeight = 13;
            graphRange.Lines = true;
            graphRange.Location = new System.Drawing.Point(12, 511);
            graphRange.MarkerValue = 0F;
            graphRange.Max = 300F;
            graphRange.MaxItems = 1;
            graphRange.Name = "graphRange";
            graphRange.OuterBorder = true;
            graphRange.Overcap = false;
            graphRange.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRange.OvercapColors");
            graphRange.PaddingX = 4F;
            graphRange.PaddingY = 6F;
            graphRange.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRange.PerItemScales");
            graphRange.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRange.ScaleHeight = 32;
            graphRange.ScaleIndex = 11;
            graphRange.ShowScale = false;
            graphRange.Size = new System.Drawing.Size(526, 29);
            graphRange.Style = Core.Enums.GraphStyle.enhOnly;
            graphRange.TabIndex = 128;
            graphRange.TextWidth = 187;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = System.Drawing.Color.Transparent;
            label7.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.ForeColor = System.Drawing.Color.Gainsboro;
            label7.Location = new System.Drawing.Point(16, 349);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(45, 20);
            label7.TabIndex = 127;
            label7.Text = "Misc:";
            label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = System.Drawing.Color.Transparent;
            label6.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.ForeColor = System.Drawing.Color.Gainsboro;
            label6.Location = new System.Drawing.Point(16, 200);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(62, 20);
            label6.TabIndex = 126;
            label6.Text = "Stealth:";
            label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = System.Drawing.Color.Transparent;
            label5.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.ForeColor = System.Drawing.Color.Gainsboro;
            label5.Location = new System.Drawing.Point(16, 6);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(89, 20);
            label5.TabIndex = 125;
            label5.Text = "Movement:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            radioButton1.ForeColor = System.Drawing.Color.WhiteSmoke;
            radioButton1.Location = new System.Drawing.Point(27, 121);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(58, 23);
            radioButton1.TabIndex = 118;
            radioButton1.TabStop = true;
            radioButton1.Text = "MPH";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += rbUnits_CheckChanged;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            radioButton2.ForeColor = System.Drawing.Color.WhiteSmoke;
            radioButton2.Location = new System.Drawing.Point(157, 121);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(54, 23);
            radioButton2.TabIndex = 120;
            radioButton2.TabStop = true;
            radioButton2.Text = "KPH";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += rbUnits_CheckChanged;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            radioButton3.ForeColor = System.Drawing.Color.WhiteSmoke;
            radioButton3.Location = new System.Drawing.Point(280, 121);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new System.Drawing.Size(81, 23);
            radioButton3.TabIndex = 122;
            radioButton3.TabStop = true;
            radioButton3.Text = "Feet/Sec";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += rbUnits_CheckChanged;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            radioButton4.ForeColor = System.Drawing.Color.WhiteSmoke;
            radioButton4.Location = new System.Drawing.Point(425, 121);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new System.Drawing.Size(98, 23);
            radioButton4.TabIndex = 124;
            radioButton4.TabStop = true;
            radioButton4.Text = "Meters/Sec";
            radioButton4.UseVisualStyleBackColor = true;
            radioButton4.CheckedChanged += rbUnits_CheckChanged;
            // 
            // graphThreat
            // 
            graphThreat.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphThreat.BackgroundImage");
            graphThreat.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.BaseBarColors");
            graphThreat.Border = true;
            graphThreat.BorderColor = System.Drawing.Color.FromArgb(131, 112, 255);
            graphThreat.Clickable = false;
            graphThreat.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphThreat.ColorBase = System.Drawing.Color.FromArgb(113, 86, 168);
            graphThreat.ColorEnh = System.Drawing.Color.MediumPurple;
            graphThreat.ColorFadeEnd = System.Drawing.Color.FromArgb(72, 61, 137);
            graphThreat.ColorFadeStart = System.Drawing.Color.Black;
            graphThreat.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphThreat.ColorLines = System.Drawing.Color.Black;
            graphThreat.ColorMarkerInner = System.Drawing.Color.Black;
            graphThreat.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphThreat.ColorOvercap = System.Drawing.Color.MediumPurple;
            graphThreat.DifferentiateColors = false;
            graphThreat.DrawRuler = false;
            graphThreat.Dual = true;
            graphThreat.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.EnhBarColors");
            graphThreat.ForcedMax = 0F;
            graphThreat.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphThreat.Highlight = true;
            graphThreat.ItemFontSizeOverride = 0F;
            graphThreat.ItemHeight = 13;
            graphThreat.Lines = true;
            graphThreat.Location = new System.Drawing.Point(12, 583);
            graphThreat.MarkerValue = 0F;
            graphThreat.Max = 600F;
            graphThreat.MaxItems = 1;
            graphThreat.Name = "graphThreat";
            graphThreat.OuterBorder = true;
            graphThreat.Overcap = false;
            graphThreat.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.OvercapColors");
            graphThreat.PaddingX = 4F;
            graphThreat.PaddingY = 6F;
            graphThreat.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphThreat.PerItemScales");
            graphThreat.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphThreat.ScaleHeight = 32;
            graphThreat.ScaleIndex = 13;
            graphThreat.ShowScale = false;
            graphThreat.Size = new System.Drawing.Size(526, 27);
            graphThreat.Style = Core.Enums.GraphStyle.Stacked;
            graphThreat.TabIndex = 123;
            graphThreat.TextWidth = 187;
            // 
            // graphEndRdx
            // 
            graphEndRdx.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphEndRdx.BackgroundImage");
            graphEndRdx.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.BaseBarColors");
            graphEndRdx.Border = true;
            graphEndRdx.BorderColor = System.Drawing.Color.FromArgb(131, 112, 255);
            graphEndRdx.Clickable = false;
            graphEndRdx.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphEndRdx.ColorBase = System.Drawing.Color.RoyalBlue;
            graphEndRdx.ColorEnh = System.Drawing.Color.RoyalBlue;
            graphEndRdx.ColorFadeEnd = System.Drawing.Color.FromArgb(105, 89, 203);
            graphEndRdx.ColorFadeStart = System.Drawing.Color.Black;
            graphEndRdx.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphEndRdx.ColorLines = System.Drawing.Color.Black;
            graphEndRdx.ColorMarkerInner = System.Drawing.Color.Black;
            graphEndRdx.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphEndRdx.ColorOvercap = System.Drawing.Color.RoyalBlue;
            graphEndRdx.DifferentiateColors = false;
            graphEndRdx.DrawRuler = false;
            graphEndRdx.Dual = true;
            graphEndRdx.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.EnhBarColors");
            graphEndRdx.ForcedMax = 0F;
            graphEndRdx.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphEndRdx.Highlight = true;
            graphEndRdx.ItemFontSizeOverride = 0F;
            graphEndRdx.ItemHeight = 13;
            graphEndRdx.Lines = true;
            graphEndRdx.Location = new System.Drawing.Point(12, 547);
            graphEndRdx.MarkerValue = 0F;
            graphEndRdx.Max = 100F;
            graphEndRdx.MaxItems = 1;
            graphEndRdx.Name = "graphEndRdx";
            graphEndRdx.OuterBorder = true;
            graphEndRdx.Overcap = false;
            graphEndRdx.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.OvercapColors");
            graphEndRdx.PaddingX = 4F;
            graphEndRdx.PaddingY = 6F;
            graphEndRdx.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphEndRdx.PerItemScales");
            graphEndRdx.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphEndRdx.ScaleHeight = 32;
            graphEndRdx.ScaleIndex = 8;
            graphEndRdx.ShowScale = false;
            graphEndRdx.Size = new System.Drawing.Size(526, 27);
            graphEndRdx.Style = Core.Enums.GraphStyle.enhOnly;
            graphEndRdx.TabIndex = 121;
            graphEndRdx.TextWidth = 187;
            // 
            // graphDamage
            // 
            graphDamage.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDamage.BackgroundImage");
            graphDamage.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDamage.BaseBarColors");
            graphDamage.Border = true;
            graphDamage.BorderColor = System.Drawing.Color.FromArgb(227, 113, 113);
            graphDamage.Clickable = false;
            graphDamage.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDamage.ColorBase = System.Drawing.Color.FromArgb(204, 0, 0);
            graphDamage.ColorEnh = System.Drawing.Color.Red;
            graphDamage.ColorFadeEnd = System.Drawing.Color.FromArgb(120, 61, 61);
            graphDamage.ColorFadeStart = System.Drawing.Color.Black;
            graphDamage.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphDamage.ColorLines = System.Drawing.Color.Black;
            graphDamage.ColorMarkerInner = System.Drawing.Color.Black;
            graphDamage.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDamage.ColorOvercap = System.Drawing.Color.FromArgb(112, 0, 0);
            graphDamage.DifferentiateColors = false;
            graphDamage.DrawRuler = false;
            graphDamage.Dual = true;
            graphDamage.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDamage.EnhBarColors");
            graphDamage.ForcedMax = 0F;
            graphDamage.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphDamage.Highlight = true;
            graphDamage.ItemFontSizeOverride = 0F;
            graphDamage.ItemHeight = 13;
            graphDamage.Lines = true;
            graphDamage.Location = new System.Drawing.Point(12, 477);
            graphDamage.MarkerValue = 0F;
            graphDamage.Max = 900F;
            graphDamage.MaxItems = 1;
            graphDamage.Name = "graphDamage";
            graphDamage.OuterBorder = true;
            graphDamage.Overcap = true;
            graphDamage.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDamage.OvercapColors");
            graphDamage.PaddingX = 4F;
            graphDamage.PaddingY = 6F;
            graphDamage.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDamage.PerItemScales");
            graphDamage.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDamage.ScaleHeight = 32;
            graphDamage.ScaleIndex = 14;
            graphDamage.ShowScale = false;
            graphDamage.Size = new System.Drawing.Size(526, 27);
            graphDamage.Style = Core.Enums.GraphStyle.Stacked;
            graphDamage.TabIndex = 119;
            graphDamage.TextWidth = 187;
            // 
            // graphAccuracy
            // 
            graphAccuracy.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphAccuracy.BackgroundImage");
            graphAccuracy.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAccuracy.BaseBarColors");
            graphAccuracy.Border = true;
            graphAccuracy.BorderColor = System.Drawing.Color.FromArgb(242, 242, 0);
            graphAccuracy.Clickable = false;
            graphAccuracy.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphAccuracy.ColorBase = System.Drawing.Color.Yellow;
            graphAccuracy.ColorEnh = System.Drawing.Color.Yellow;
            graphAccuracy.ColorFadeEnd = System.Drawing.Color.FromArgb(123, 123, 0);
            graphAccuracy.ColorFadeStart = System.Drawing.Color.Black;
            graphAccuracy.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphAccuracy.ColorLines = System.Drawing.Color.Black;
            graphAccuracy.ColorMarkerInner = System.Drawing.Color.Black;
            graphAccuracy.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphAccuracy.ColorOvercap = System.Drawing.Color.Yellow;
            graphAccuracy.DifferentiateColors = false;
            graphAccuracy.DrawRuler = false;
            graphAccuracy.Dual = true;
            graphAccuracy.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAccuracy.EnhBarColors");
            graphAccuracy.ForcedMax = 0F;
            graphAccuracy.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphAccuracy.Highlight = true;
            graphAccuracy.ItemFontSizeOverride = 0F;
            graphAccuracy.ItemHeight = 13;
            graphAccuracy.Lines = true;
            graphAccuracy.Location = new System.Drawing.Point(12, 443);
            graphAccuracy.MarkerValue = 0F;
            graphAccuracy.Max = 100F;
            graphAccuracy.MaxItems = 1;
            graphAccuracy.Name = "graphAccuracy";
            graphAccuracy.OuterBorder = true;
            graphAccuracy.Overcap = false;
            graphAccuracy.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAccuracy.OvercapColors");
            graphAccuracy.PaddingX = 4F;
            graphAccuracy.PaddingY = 6F;
            graphAccuracy.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphAccuracy.PerItemScales");
            graphAccuracy.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphAccuracy.ScaleHeight = 32;
            graphAccuracy.ScaleIndex = 8;
            graphAccuracy.ShowScale = false;
            graphAccuracy.Size = new System.Drawing.Size(526, 27);
            graphAccuracy.Style = Core.Enums.GraphStyle.enhOnly;
            graphAccuracy.TabIndex = 117;
            graphAccuracy.TextWidth = 187;
            // 
            // graphToHit
            // 
            graphToHit.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphToHit.BackgroundImage");
            graphToHit.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.BaseBarColors");
            graphToHit.Border = true;
            graphToHit.BorderColor = System.Drawing.Color.FromArgb(242, 242, 0);
            graphToHit.Clickable = false;
            graphToHit.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphToHit.ColorBase = System.Drawing.Color.FromArgb(255, 255, 128);
            graphToHit.ColorEnh = System.Drawing.Color.FromArgb(255, 255, 128);
            graphToHit.ColorFadeEnd = System.Drawing.Color.FromArgb(190, 190, 0);
            graphToHit.ColorFadeStart = System.Drawing.Color.Black;
            graphToHit.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphToHit.ColorLines = System.Drawing.Color.Black;
            graphToHit.ColorMarkerInner = System.Drawing.Color.Black;
            graphToHit.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphToHit.ColorOvercap = System.Drawing.Color.FromArgb(255, 255, 128);
            graphToHit.DifferentiateColors = false;
            graphToHit.DrawRuler = false;
            graphToHit.Dual = true;
            graphToHit.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.EnhBarColors");
            graphToHit.ForcedMax = 0F;
            graphToHit.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphToHit.Highlight = true;
            graphToHit.ItemFontSizeOverride = 0F;
            graphToHit.ItemHeight = 13;
            graphToHit.Lines = true;
            graphToHit.Location = new System.Drawing.Point(12, 409);
            graphToHit.MarkerValue = 0F;
            graphToHit.Max = 100F;
            graphToHit.MaxItems = 1;
            graphToHit.Name = "graphToHit";
            graphToHit.OuterBorder = true;
            graphToHit.Overcap = false;
            graphToHit.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.OvercapColors");
            graphToHit.PaddingX = 4F;
            graphToHit.PaddingY = 6F;
            graphToHit.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphToHit.PerItemScales");
            graphToHit.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphToHit.ScaleHeight = 32;
            graphToHit.ScaleIndex = 8;
            graphToHit.ShowScale = false;
            graphToHit.Size = new System.Drawing.Size(526, 27);
            graphToHit.Style = Core.Enums.GraphStyle.enhOnly;
            graphToHit.TabIndex = 116;
            graphToHit.TextWidth = 187;
            // 
            // graphHaste
            // 
            graphHaste.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphHaste.BackgroundImage");
            graphHaste.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.BaseBarColors");
            graphHaste.Border = true;
            graphHaste.BorderColor = System.Drawing.Color.FromArgb(242, 81, 0);
            graphHaste.Clickable = false;
            graphHaste.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphHaste.ColorBase = System.Drawing.Color.FromArgb(204, 102, 0);
            graphHaste.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 0);
            graphHaste.ColorFadeEnd = System.Drawing.Color.FromArgb(184, 62, 0);
            graphHaste.ColorFadeStart = System.Drawing.Color.Black;
            graphHaste.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphHaste.ColorLines = System.Drawing.Color.Black;
            graphHaste.ColorMarkerInner = System.Drawing.Color.Black;
            graphHaste.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphHaste.ColorOvercap = System.Drawing.Color.FromArgb(112, 56, 0);
            graphHaste.DifferentiateColors = false;
            graphHaste.DrawRuler = false;
            graphHaste.Dual = true;
            graphHaste.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.EnhBarColors");
            graphHaste.ForcedMax = 0F;
            graphHaste.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphHaste.Highlight = true;
            graphHaste.ItemFontSizeOverride = 0F;
            graphHaste.ItemHeight = 13;
            graphHaste.Lines = true;
            graphHaste.Location = new System.Drawing.Point(12, 375);
            graphHaste.MarkerValue = 0F;
            graphHaste.Max = 450F;
            graphHaste.MaxItems = 1;
            graphHaste.Name = "graphHaste";
            graphHaste.OuterBorder = true;
            graphHaste.Overcap = true;
            graphHaste.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.OvercapColors");
            graphHaste.PaddingX = 4F;
            graphHaste.PaddingY = 6F;
            graphHaste.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphHaste.PerItemScales");
            graphHaste.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphHaste.ScaleHeight = 32;
            graphHaste.ScaleIndex = 12;
            graphHaste.ShowScale = false;
            graphHaste.Size = new System.Drawing.Size(526, 27);
            graphHaste.Style = Core.Enums.GraphStyle.Stacked;
            graphHaste.TabIndex = 115;
            graphHaste.TextWidth = 187;
            // 
            // graphPerception
            // 
            graphPerception.BackColor = System.Drawing.Color.Black;
            graphPerception.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphPerception.BackgroundImage");
            graphPerception.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphPerception.BaseBarColors");
            graphPerception.Border = true;
            graphPerception.BorderColor = System.Drawing.Color.FromArgb(124, 104, 237);
            graphPerception.Clickable = false;
            graphPerception.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphPerception.ColorBase = System.Drawing.Color.FromArgb(84, 95, 107);
            graphPerception.ColorEnh = System.Drawing.Color.FromArgb(106, 121, 136);
            graphPerception.ColorFadeEnd = System.Drawing.Color.FromArgb(72, 61, 137);
            graphPerception.ColorFadeStart = System.Drawing.Color.Black;
            graphPerception.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphPerception.ColorLines = System.Drawing.Color.Black;
            graphPerception.ColorMarkerInner = System.Drawing.Color.Black;
            graphPerception.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphPerception.ColorOvercap = System.Drawing.Color.FromArgb(46, 52, 59);
            graphPerception.DifferentiateColors = false;
            graphPerception.DrawRuler = false;
            graphPerception.Dual = true;
            graphPerception.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphPerception.EnhBarColors");
            graphPerception.ForcedMax = 0F;
            graphPerception.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphPerception.Highlight = true;
            graphPerception.ItemFontSizeOverride = 0F;
            graphPerception.ItemHeight = 13;
            graphPerception.Lines = true;
            graphPerception.Location = new System.Drawing.Point(12, 226);
            graphPerception.MarkerValue = 0F;
            graphPerception.Max = 1200F;
            graphPerception.MaxItems = 3;
            graphPerception.Name = "graphPerception";
            graphPerception.OuterBorder = true;
            graphPerception.Overcap = true;
            graphPerception.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphPerception.OvercapColors");
            graphPerception.PaddingX = 4F;
            graphPerception.PaddingY = 6F;
            graphPerception.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphPerception.PerItemScales");
            graphPerception.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphPerception.ScaleHeight = 32;
            graphPerception.ScaleIndex = 15;
            graphPerception.ShowScale = false;
            graphPerception.Size = new System.Drawing.Size(526, 64);
            graphPerception.Style = Core.Enums.GraphStyle.Stacked;
            graphPerception.TabIndex = 114;
            graphPerception.TextWidth = 187;
            // 
            // graphMovement
            // 
            graphMovement.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphMovement.BackgroundImage");
            graphMovement.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.BaseBarColors");
            graphMovement.Border = true;
            graphMovement.BorderColor = System.Drawing.Color.FromArgb(0, 227, 170);
            graphMovement.Clickable = false;
            graphMovement.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphMovement.ColorBase = System.Drawing.Color.FromArgb(0, 140, 94);
            graphMovement.ColorEnh = System.Drawing.Color.FromArgb(0, 192, 128);
            graphMovement.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 127, 95);
            graphMovement.ColorFadeStart = System.Drawing.Color.Black;
            graphMovement.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphMovement.ColorLines = System.Drawing.Color.Black;
            graphMovement.ColorMarkerInner = System.Drawing.Color.Black;
            graphMovement.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphMovement.ColorOvercap = System.Drawing.Color.FromArgb(0, 48, 32);
            graphMovement.DifferentiateColors = false;
            graphMovement.DrawRuler = false;
            graphMovement.Dual = true;
            graphMovement.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.EnhBarColors");
            graphMovement.ForcedMax = 0F;
            graphMovement.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphMovement.Highlight = true;
            graphMovement.ItemFontSizeOverride = 0F;
            graphMovement.ItemHeight = 13;
            graphMovement.Lines = true;
            graphMovement.Location = new System.Drawing.Point(12, 31);
            graphMovement.MarkerValue = 0F;
            graphMovement.Max = 225F;
            graphMovement.MaxItems = 4;
            graphMovement.Name = "graphMovement";
            graphMovement.OuterBorder = true;
            graphMovement.Overcap = true;
            graphMovement.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.OvercapColors");
            graphMovement.PaddingX = 4F;
            graphMovement.PaddingY = 6F;
            graphMovement.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphMovement.PerItemScales");
            graphMovement.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphMovement.ScaleHeight = 32;
            graphMovement.ScaleIndex = 10;
            graphMovement.ShowScale = false;
            graphMovement.Size = new System.Drawing.Size(526, 82);
            graphMovement.Style = Core.Enums.GraphStyle.Stacked;
            graphMovement.TabIndex = 113;
            graphMovement.TextWidth = 187;
            // 
            // panelTab3
            // 
            panelTab3.Controls.Add(label9);
            panelTab3.Controls.Add(label8);
            panelTab3.Controls.Add(graphStatusRes);
            panelTab3.Controls.Add(graphStatusProt);
            panelTab3.Location = new System.Drawing.Point(1145, 30);
            panelTab3.Name = "panelTab3";
            panelTab3.Size = new System.Drawing.Size(550, 650);
            panelTab3.TabIndex = 103;
            panelTab3.Visible = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = System.Drawing.Color.Transparent;
            label9.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label9.ForeColor = System.Drawing.Color.Gainsboro;
            label9.Location = new System.Drawing.Point(16, 300);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(135, 20);
            label9.TabIndex = 117;
            label9.Text = "Status Resistance:";
            label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = System.Drawing.Color.Transparent;
            label8.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label8.ForeColor = System.Drawing.Color.Gainsboro;
            label8.Location = new System.Drawing.Point(16, 6);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(134, 20);
            label8.TabIndex = 116;
            label8.Text = "Status Protection:";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // graphStatusRes
            // 
            graphStatusRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphStatusRes.BackgroundImage");
            graphStatusRes.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusRes.BaseBarColors");
            graphStatusRes.Border = true;
            graphStatusRes.BorderColor = System.Drawing.Color.FromArgb(227, 227, 0);
            graphStatusRes.Clickable = false;
            graphStatusRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphStatusRes.ColorBase = System.Drawing.Color.Yellow;
            graphStatusRes.ColorEnh = System.Drawing.Color.Yellow;
            graphStatusRes.ColorFadeEnd = System.Drawing.Color.FromArgb(127, 127, 0);
            graphStatusRes.ColorFadeStart = System.Drawing.Color.Black;
            graphStatusRes.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphStatusRes.ColorLines = System.Drawing.Color.Black;
            graphStatusRes.ColorMarkerInner = System.Drawing.Color.Black;
            graphStatusRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphStatusRes.ColorOvercap = System.Drawing.Color.Yellow;
            graphStatusRes.DifferentiateColors = false;
            graphStatusRes.DrawRuler = false;
            graphStatusRes.Dual = true;
            graphStatusRes.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusRes.EnhBarColors");
            graphStatusRes.ForcedMax = 0F;
            graphStatusRes.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphStatusRes.Highlight = true;
            graphStatusRes.ItemFontSizeOverride = 0F;
            graphStatusRes.ItemHeight = 13;
            graphStatusRes.Lines = false;
            graphStatusRes.Location = new System.Drawing.Point(12, 326);
            graphStatusRes.MarkerValue = 0F;
            graphStatusRes.Max = 600F;
            graphStatusRes.MaxItems = 11;
            graphStatusRes.Name = "graphStatusRes";
            graphStatusRes.OuterBorder = true;
            graphStatusRes.Overcap = false;
            graphStatusRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusRes.OvercapColors");
            graphStatusRes.PaddingX = 4F;
            graphStatusRes.PaddingY = 6F;
            graphStatusRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphStatusRes.PerItemScales");
            graphStatusRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphStatusRes.ScaleHeight = 32;
            graphStatusRes.ScaleIndex = 13;
            graphStatusRes.ShowScale = false;
            graphStatusRes.Size = new System.Drawing.Size(526, 218);
            graphStatusRes.Style = Core.Enums.GraphStyle.enhOnly;
            graphStatusRes.TabIndex = 115;
            graphStatusRes.TabStop = false;
            graphStatusRes.TextWidth = 187;
            // 
            // graphStatusProt
            // 
            graphStatusProt.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphStatusProt.BackgroundImage");
            graphStatusProt.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusProt.BaseBarColors");
            graphStatusProt.Border = true;
            graphStatusProt.BorderColor = System.Drawing.Color.FromArgb(227, 113, 0);
            graphStatusProt.Clickable = false;
            graphStatusProt.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphStatusProt.ColorBase = System.Drawing.Color.FromArgb(255, 128, 0);
            graphStatusProt.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 0);
            graphStatusProt.ColorFadeEnd = System.Drawing.Color.FromArgb(127, 64, 0);
            graphStatusProt.ColorFadeStart = System.Drawing.Color.Black;
            graphStatusProt.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphStatusProt.ColorLines = System.Drawing.Color.Black;
            graphStatusProt.ColorMarkerInner = System.Drawing.Color.Black;
            graphStatusProt.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphStatusProt.ColorOvercap = System.Drawing.Color.FromArgb(255, 128, 0);
            graphStatusProt.DifferentiateColors = false;
            graphStatusProt.DrawRuler = false;
            graphStatusProt.Dual = true;
            graphStatusProt.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusProt.EnhBarColors");
            graphStatusProt.ForcedMax = 0F;
            graphStatusProt.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphStatusProt.Highlight = true;
            graphStatusProt.ItemFontSizeOverride = 0F;
            graphStatusProt.ItemHeight = 13;
            graphStatusProt.Lines = false;
            graphStatusProt.Location = new System.Drawing.Point(12, 31);
            graphStatusProt.MarkerValue = 0F;
            graphStatusProt.Max = 50F;
            graphStatusProt.MaxItems = 11;
            graphStatusProt.Name = "graphStatusProt";
            graphStatusProt.OuterBorder = true;
            graphStatusProt.Overcap = false;
            graphStatusProt.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStatusProt.OvercapColors");
            graphStatusProt.PaddingX = 4F;
            graphStatusProt.PaddingY = 6F;
            graphStatusProt.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphStatusProt.PerItemScales");
            graphStatusProt.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphStatusProt.ScaleHeight = 32;
            graphStatusProt.ScaleIndex = 6;
            graphStatusProt.ShowScale = false;
            graphStatusProt.Size = new System.Drawing.Size(526, 218);
            graphStatusProt.Style = Core.Enums.GraphStyle.enhOnly;
            graphStatusProt.TabIndex = 114;
            graphStatusProt.TabStop = false;
            graphStatusProt.TextWidth = 187;
            // 
            // panelTab4
            // 
            panelTab4.Controls.Add(label11);
            panelTab4.Controls.Add(label10);
            panelTab4.Controls.Add(graphElusivity);
            panelTab4.Controls.Add(graphDebuffRes);
            panelTab4.Location = new System.Drawing.Point(1721, 30);
            panelTab4.Name = "panelTab4";
            panelTab4.Size = new System.Drawing.Size(550, 650);
            panelTab4.TabIndex = 104;
            panelTab4.Visible = false;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = System.Drawing.Color.Transparent;
            label11.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label11.ForeColor = System.Drawing.Color.Gainsboro;
            label11.Location = new System.Drawing.Point(16, 288);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(71, 20);
            label11.TabIndex = 118;
            label11.Text = "Elusivity:";
            label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = System.Drawing.Color.Transparent;
            label10.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label10.ForeColor = System.Drawing.Color.Gainsboro;
            label10.Location = new System.Drawing.Point(16, 6);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(140, 20);
            label10.TabIndex = 117;
            label10.Text = "Debuff Resistance:";
            label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // graphElusivity
            // 
            graphElusivity.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphElusivity.BackgroundImage");
            graphElusivity.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.BaseBarColors");
            graphElusivity.Border = true;
            graphElusivity.BorderColor = System.Drawing.Color.FromArgb(180, 1, 255);
            graphElusivity.Clickable = false;
            graphElusivity.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphElusivity.ColorBase = System.Drawing.Color.FromArgb(163, 1, 231);
            graphElusivity.ColorEnh = System.Drawing.Color.FromArgb(163, 1, 231);
            graphElusivity.ColorFadeEnd = System.Drawing.Color.FromArgb(141, 2, 200);
            graphElusivity.ColorFadeStart = System.Drawing.Color.Black;
            graphElusivity.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphElusivity.ColorLines = System.Drawing.Color.Black;
            graphElusivity.ColorMarkerInner = System.Drawing.Color.Black;
            graphElusivity.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphElusivity.ColorOvercap = System.Drawing.Color.FromArgb(163, 1, 231);
            graphElusivity.DifferentiateColors = false;
            graphElusivity.DrawRuler = false;
            graphElusivity.Dual = true;
            graphElusivity.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.EnhBarColors");
            graphElusivity.ForcedMax = 0F;
            graphElusivity.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphElusivity.Highlight = true;
            graphElusivity.ItemFontSizeOverride = 0F;
            graphElusivity.ItemHeight = 13;
            graphElusivity.Lines = true;
            graphElusivity.Location = new System.Drawing.Point(12, 314);
            graphElusivity.MarkerValue = 0F;
            graphElusivity.Max = 100F;
            graphElusivity.MaxItems = 13;
            graphElusivity.Name = "graphElusivity";
            graphElusivity.OuterBorder = true;
            graphElusivity.Overcap = false;
            graphElusivity.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.OvercapColors");
            graphElusivity.PaddingX = 10F;
            graphElusivity.PaddingY = 6F;
            graphElusivity.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphElusivity.PerItemScales");
            graphElusivity.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphElusivity.ScaleHeight = 32;
            graphElusivity.ScaleIndex = 8;
            graphElusivity.ShowScale = false;
            graphElusivity.Size = new System.Drawing.Size(526, 235);
            graphElusivity.Style = Core.Enums.GraphStyle.enhOnly;
            graphElusivity.TabIndex = 116;
            graphElusivity.TabStop = false;
            graphElusivity.TextWidth = 187;
            // 
            // graphDebuffRes
            // 
            graphDebuffRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDebuffRes.BackgroundImage");
            graphDebuffRes.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDebuffRes.BaseBarColors");
            graphDebuffRes.Border = true;
            graphDebuffRes.BorderColor = System.Drawing.Color.LightSeaGreen;
            graphDebuffRes.Clickable = false;
            graphDebuffRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDebuffRes.ColorBase = System.Drawing.Color.FromArgb(0, 190, 190);
            graphDebuffRes.ColorEnh = System.Drawing.Color.Cyan;
            graphDebuffRes.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 127, 127);
            graphDebuffRes.ColorFadeStart = System.Drawing.Color.Black;
            graphDebuffRes.ColorHighlight = System.Drawing.Color.FromArgb(128, 128, 255);
            graphDebuffRes.ColorLines = System.Drawing.Color.Black;
            graphDebuffRes.ColorMarkerInner = System.Drawing.Color.Black;
            graphDebuffRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDebuffRes.ColorOvercap = System.Drawing.Color.FromArgb(0, 90, 127);
            graphDebuffRes.DifferentiateColors = false;
            graphDebuffRes.DrawRuler = false;
            graphDebuffRes.Dual = true;
            graphDebuffRes.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDebuffRes.EnhBarColors");
            graphDebuffRes.ForcedMax = 0F;
            graphDebuffRes.ForeColor = System.Drawing.Color.WhiteSmoke;
            graphDebuffRes.Highlight = true;
            graphDebuffRes.ItemFontSizeOverride = 0F;
            graphDebuffRes.ItemHeight = 13;
            graphDebuffRes.Lines = true;
            graphDebuffRes.Location = new System.Drawing.Point(12, 30);
            graphDebuffRes.MarkerValue = 0F;
            graphDebuffRes.Max = 100F;
            graphDebuffRes.MaxItems = 8;
            graphDebuffRes.Name = "graphDebuffRes";
            graphDebuffRes.OuterBorder = true;
            graphDebuffRes.Overcap = true;
            graphDebuffRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDebuffRes.OvercapColors");
            graphDebuffRes.PaddingX = 4F;
            graphDebuffRes.PaddingY = 6F;
            graphDebuffRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDebuffRes.PerItemScales");
            graphDebuffRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDebuffRes.ScaleHeight = 32;
            graphDebuffRes.ScaleIndex = 8;
            graphDebuffRes.ShowScale = false;
            graphDebuffRes.Size = new System.Drawing.Size(526, 160);
            graphDebuffRes.Style = Core.Enums.GraphStyle.Stacked;
            graphDebuffRes.TabIndex = 115;
            graphDebuffRes.TabStop = false;
            graphDebuffRes.TextWidth = 187;
            // 
            // panel1
            // 
            panel1.Controls.Add(ctlTotalsTabStrip1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(2267, 24);
            panel1.TabIndex = 105;
            // 
            // ctlTotalsTabStrip1
            // 
            ctlTotalsTabStrip1.ActiveTabColor = System.Drawing.Color.Goldenrod;
            ctlTotalsTabStrip1.BackColor = System.Drawing.Color.Black;
            ctlTotalsTabStrip1.BackgroundImage = (System.Drawing.Image)resources.GetObject("ctlTotalsTabStrip1.BackgroundImage");
            ctlTotalsTabStrip1.DimmedBackgroundColor = System.Drawing.Color.FromArgb(21, 61, 93);
            ctlTotalsTabStrip1.Dock = DockStyle.Fill;
            ctlTotalsTabStrip1.ForeColor = System.Drawing.Color.WhiteSmoke;
            ctlTotalsTabStrip1.InactiveHoveredTabColor = System.Drawing.Color.FromArgb(43, 122, 187);
            ctlTotalsTabStrip1.InactiveTabColor = System.Drawing.Color.FromArgb(30, 85, 130);
            ctlTotalsTabStrip1.ItemPadding = 18;
            ctlTotalsTabStrip1.Location = new System.Drawing.Point(0, 0);
            ctlTotalsTabStrip1.Name = "ctlTotalsTabStrip1";
            ctlTotalsTabStrip1.OutlineText = true;
            ctlTotalsTabStrip1.Size = new System.Drawing.Size(2267, 24);
            ctlTotalsTabStrip1.StripLineColor = System.Drawing.Color.Goldenrod;
            ctlTotalsTabStrip1.TabIndex = 106;
            ctlTotalsTabStrip1.UseDimmedBackground = false;
            ctlTotalsTabStrip1.TabClick += ctlTotalsTabStrip1_TabClick;
            // 
            // frmTotalsV2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(2267, 730);
            Controls.Add(panel1);
            Controls.Add(panelTab4);
            Controls.Add(panelTab3);
            Controls.Add(panelTab2);
            Controls.Add(panelTab1);
            Controls.Add(panel2);
            DoubleBuffered = true;
            ForeColor = System.Drawing.SystemColors.Control;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(55, 24, 55, 24);
            Name = "frmTotalsV2";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Totals for Self";
            TopMost = true;
            Move += frmTotalsV2_Move;
            Resize += frmTotalsV2_Resize;
            panel2.ResumeLayout(false);
            panelTab1.ResumeLayout(false);
            panelTab1.PerformLayout();
            panelTab2.ResumeLayout(false);
            panelTab2.PerformLayout();
            panelTab3.ResumeLayout(false);
            panelTab3.PerformLayout();
            panelTab4.ResumeLayout(false);
            panelTab4.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private Controls.ImageButtonEx ibClose;
        private Controls.ImageButtonEx ibTopMost;
        private Panel panelTab1;
        private CtlMultiGraph graphEnd;
        private CtlMultiGraph graphHP;
        private CtlMultiGraph graphRes;
        private CtlMultiGraph graphDef;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Panel panelTab2;
        private Label label7;
        private Label label6;
        private Label label5;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private CtlMultiGraph graphThreat;
        private CtlMultiGraph graphEndRdx;
        private CtlMultiGraph graphDamage;
        private CtlMultiGraph graphAccuracy;
        private CtlMultiGraph graphToHit;
        private CtlMultiGraph graphHaste;
        private CtlMultiGraph graphPerception;
        private CtlMultiGraph graphMovement;
        private Panel panelTab3;
        private Label label9;
        private Label label8;
        private CtlMultiGraph graphStatusRes;
        private CtlMultiGraph graphStatusProt;
        private Panel panelTab4;
        private Label label11;
        private Label label10;
        private CtlMultiGraph graphElusivity;
        private CtlMultiGraph graphDebuffRes;
        private Panel panel1;
        private ctlTotalsTabStrip ctlTotalsTabStrip1;
        private CtlMultiGraph graphRange;
    }
}