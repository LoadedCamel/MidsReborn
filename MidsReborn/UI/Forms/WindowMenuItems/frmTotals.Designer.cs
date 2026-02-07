using System.ComponentModel;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmTotals
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmTotals));
            lblDef = new Label();
            lblRes = new Label();
            lblRegenRec = new Label();
            pnlDRHE = new Panel();
            graphMaxEnd = new CtlMultiGraph();
            graphHP = new CtlMultiGraph();
            graphDef = new CtlMultiGraph();
            graphDrain = new CtlMultiGraph();
            graphRes = new CtlMultiGraph();
            graphRec = new CtlMultiGraph();
            graphRegen = new CtlMultiGraph();
            Panel1 = new Panel();
            pnlMisc = new Panel();
            label1 = new Label();
            rbMSec = new RadioButton();
            rbFPS = new RadioButton();
            rbKPH = new RadioButton();
            rbMPH = new RadioButton();
            lblStealth = new Label();
            graphStealth = new CtlMultiGraph();
            lblMisc = new Label();
            graphMovement = new CtlMultiGraph();
            lblMovement = new Label();
            Panel2 = new Panel();
            tab1 = new PictureBox();
            tab0 = new PictureBox();
            pbTopMost = new PictureBox();
            pbClose = new PictureBox();
            pnlStatus = new Panel();
            graphSRes = new CtlMultiGraph();
            lblSRes = new Label();
            graphSDeb = new CtlMultiGraph();
            lblSDeb = new Label();
            graphSProt = new CtlMultiGraph();
            lblSProt = new Label();
            tab2 = new PictureBox();
            toolTip1 = new ToolTip(components);
            pnlDRHE.SuspendLayout();
            pnlMisc.SuspendLayout();
            ((ISupportInitialize)tab1).BeginInit();
            ((ISupportInitialize)tab0).BeginInit();
            ((ISupportInitialize)pbTopMost).BeginInit();
            ((ISupportInitialize)pbClose).BeginInit();
            pnlStatus.SuspendLayout();
            ((ISupportInitialize)tab2).BeginInit();
            SuspendLayout();
            // 
            // lblDef
            // 
            lblDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblDef.Location = new System.Drawing.Point(3, 0);
            lblDef.Name = "lblDef";
            lblDef.Size = new System.Drawing.Size(89, 16);
            lblDef.TabIndex = 1;
            lblDef.Text = "Defense:";
            lblDef.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRes
            // 
            lblRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblRes.Location = new System.Drawing.Point(3, 174);
            lblRes.Name = "lblRes";
            lblRes.Size = new System.Drawing.Size(125, 16);
            lblRes.TabIndex = 3;
            lblRes.Text = "Resistance:";
            lblRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRegenRec
            // 
            lblRegenRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblRegenRec.Location = new System.Drawing.Point(3, 312);
            lblRegenRec.Name = "lblRegenRec";
            lblRegenRec.Size = new System.Drawing.Size(125, 16);
            lblRegenRec.TabIndex = 5;
            lblRegenRec.Text = "Health & Endurance:";
            lblRegenRec.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            lblRegenRec.UseMnemonic = false;
            // 
            // pnlDRHE
            // 
            pnlDRHE.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            pnlDRHE.Controls.Add(graphMaxEnd);
            pnlDRHE.Controls.Add(graphHP);
            pnlDRHE.Controls.Add(graphDef);
            pnlDRHE.Controls.Add(graphDrain);
            pnlDRHE.Controls.Add(lblDef);
            pnlDRHE.Controls.Add(graphRes);
            pnlDRHE.Controls.Add(graphRec);
            pnlDRHE.Controls.Add(lblRes);
            pnlDRHE.Controls.Add(lblRegenRec);
            pnlDRHE.Controls.Add(graphRegen);
            pnlDRHE.Controls.Add(Panel1);
            pnlDRHE.Location = new System.Drawing.Point(4, 31);
            pnlDRHE.Name = "pnlDRHE";
            pnlDRHE.Size = new System.Drawing.Size(320, 445);
            pnlDRHE.TabIndex = 9;
            // 
            // graphMaxEnd
            // 
            graphMaxEnd.BackColor = System.Drawing.Color.Black;
            graphMaxEnd.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphMaxEnd.BackgroundImage");
            graphMaxEnd.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphMaxEnd.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMaxEnd.BaseBarColors");
            graphMaxEnd.Border = true;
            graphMaxEnd.BorderColor = System.Drawing.Color.Black;
            graphMaxEnd.Clickable = false;
            graphMaxEnd.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphMaxEnd.ColorBase = System.Drawing.Color.CornflowerBlue;
            graphMaxEnd.ColorEnh = System.Drawing.Color.Yellow;
            graphMaxEnd.ColorFadeEnd = System.Drawing.Color.FromArgb(64, 64, 128);
            graphMaxEnd.ColorFadeStart = System.Drawing.Color.Black;
            graphMaxEnd.ColorHighlight = System.Drawing.Color.Gray;
            graphMaxEnd.ColorLines = System.Drawing.Color.Black;
            graphMaxEnd.ColorMarkerInner = System.Drawing.Color.Black;
            graphMaxEnd.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphMaxEnd.ColorOvercap = System.Drawing.Color.Black;
            graphMaxEnd.DifferentiateColors = false;
            graphMaxEnd.DrawRuler = false;
            graphMaxEnd.Dual = true;
            graphMaxEnd.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMaxEnd.EnhBarColors");
            graphMaxEnd.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphMaxEnd.ForcedMax = 0F;
            graphMaxEnd.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphMaxEnd.Highlight = true;
            graphMaxEnd.ItemFontSizeOverride = 0F;
            graphMaxEnd.ItemHeight = 10;
            graphMaxEnd.Lines = true;
            graphMaxEnd.Location = new System.Drawing.Point(15, 404);
            graphMaxEnd.MarkerValue = 0F;
            graphMaxEnd.Max = 100F;
            graphMaxEnd.MaxItems = 60;
            graphMaxEnd.Name = "graphMaxEnd";
            graphMaxEnd.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphMaxEnd.NegativeBaseColor = System.Drawing.Color.Navy;
            graphMaxEnd.NegativeEnhColor = System.Drawing.Color.Olive;
            graphMaxEnd.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphMaxEnd.OuterBorder = false;
            graphMaxEnd.Overcap = false;
            graphMaxEnd.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMaxEnd.OvercapColors");
            graphMaxEnd.PaddingX = 2F;
            graphMaxEnd.PaddingY = 2F;
            graphMaxEnd.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphMaxEnd.PerItemScales");
            graphMaxEnd.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphMaxEnd.ScaleHeight = 32;
            graphMaxEnd.ScaleIndex = 8;
            graphMaxEnd.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphMaxEnd.ShowScale = false;
            graphMaxEnd.SingleLineLabels = true;
            graphMaxEnd.Size = new System.Drawing.Size(300, 15);
            graphMaxEnd.Style = Enums.GraphStyle.baseOnly;
            graphMaxEnd.TabIndex = 7;
            graphMaxEnd.TextWidth = 125;
            // 
            // graphHP
            // 
            graphHP.BackColor = System.Drawing.Color.Black;
            graphHP.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphHP.BackgroundImage");
            graphHP.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphHP.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.BaseBarColors");
            graphHP.Border = true;
            graphHP.BorderColor = System.Drawing.Color.Black;
            graphHP.Clickable = false;
            graphHP.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphHP.ColorBase = System.Drawing.Color.FromArgb(96, 192, 96);
            graphHP.ColorEnh = System.Drawing.Color.Yellow;
            graphHP.ColorFadeEnd = System.Drawing.Color.FromArgb(64, 128, 64);
            graphHP.ColorFadeStart = System.Drawing.Color.Black;
            graphHP.ColorHighlight = System.Drawing.Color.Gray;
            graphHP.ColorLines = System.Drawing.Color.Black;
            graphHP.ColorMarkerInner = System.Drawing.Color.Black;
            graphHP.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphHP.ColorOvercap = System.Drawing.Color.Black;
            graphHP.DifferentiateColors = false;
            graphHP.DrawRuler = false;
            graphHP.Dual = true;
            graphHP.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.EnhBarColors");
            graphHP.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphHP.ForcedMax = 0F;
            graphHP.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphHP.Highlight = true;
            graphHP.ItemFontSizeOverride = 0F;
            graphHP.ItemHeight = 10;
            graphHP.Lines = true;
            graphHP.Location = new System.Drawing.Point(15, 349);
            graphHP.MarkerValue = 0F;
            graphHP.Max = 100F;
            graphHP.MaxItems = 60;
            graphHP.Name = "graphHP";
            graphHP.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphHP.NegativeBaseColor = System.Drawing.Color.Navy;
            graphHP.NegativeEnhColor = System.Drawing.Color.Olive;
            graphHP.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphHP.OuterBorder = false;
            graphHP.Overcap = false;
            graphHP.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.OvercapColors");
            graphHP.PaddingX = 2F;
            graphHP.PaddingY = 2F;
            graphHP.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphHP.PerItemScales");
            graphHP.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphHP.ScaleHeight = 32;
            graphHP.ScaleIndex = 8;
            graphHP.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphHP.ShowScale = false;
            graphHP.SingleLineLabels = true;
            graphHP.Size = new System.Drawing.Size(300, 15);
            graphHP.Style = Enums.GraphStyle.baseOnly;
            graphHP.TabIndex = 9;
            graphHP.TextWidth = 125;
            // 
            // graphDef
            // 
            graphDef.BackColor = System.Drawing.Color.Black;
            graphDef.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDef.BackgroundImage");
            graphDef.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphDef.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.BaseBarColors");
            graphDef.Border = true;
            graphDef.BorderColor = System.Drawing.Color.Black;
            graphDef.Clickable = false;
            graphDef.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDef.ColorBase = System.Drawing.Color.FromArgb(192, 0, 192);
            graphDef.ColorEnh = System.Drawing.Color.Yellow;
            graphDef.ColorFadeEnd = System.Drawing.Color.Purple;
            graphDef.ColorFadeStart = System.Drawing.Color.Black;
            graphDef.ColorHighlight = System.Drawing.Color.Gray;
            graphDef.ColorLines = System.Drawing.Color.Black;
            graphDef.ColorMarkerInner = System.Drawing.Color.Black;
            graphDef.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDef.ColorOvercap = System.Drawing.Color.Black;
            graphDef.DifferentiateColors = false;
            graphDef.DrawRuler = false;
            graphDef.Dual = true;
            graphDef.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.EnhBarColors");
            graphDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphDef.ForcedMax = 0F;
            graphDef.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphDef.Highlight = true;
            graphDef.ItemFontSizeOverride = 0F;
            graphDef.ItemHeight = 10;
            graphDef.Lines = true;
            graphDef.Location = new System.Drawing.Point(15, 17);
            graphDef.MarkerValue = 0F;
            graphDef.Max = 100F;
            graphDef.MaxItems = 60;
            graphDef.Name = "graphDef";
            graphDef.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphDef.NegativeBaseColor = System.Drawing.Color.Navy;
            graphDef.NegativeEnhColor = System.Drawing.Color.Olive;
            graphDef.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphDef.OuterBorder = false;
            graphDef.Overcap = false;
            graphDef.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.OvercapColors");
            graphDef.PaddingX = 2F;
            graphDef.PaddingY = 4F;
            graphDef.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDef.PerItemScales");
            graphDef.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDef.ScaleHeight = 32;
            graphDef.ScaleIndex = 8;
            graphDef.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphDef.ShowScale = false;
            graphDef.SingleLineLabels = true;
            graphDef.Size = new System.Drawing.Size(300, 156);
            graphDef.Style = Enums.GraphStyle.baseOnly;
            graphDef.TabIndex = 0;
            graphDef.TextWidth = 125;
            // 
            // graphDrain
            // 
            graphDrain.BackColor = System.Drawing.Color.Black;
            graphDrain.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDrain.BackgroundImage");
            graphDrain.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphDrain.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDrain.BaseBarColors");
            graphDrain.Border = true;
            graphDrain.BorderColor = System.Drawing.Color.Black;
            graphDrain.Clickable = false;
            graphDrain.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDrain.ColorBase = System.Drawing.Color.LightSteelBlue;
            graphDrain.ColorEnh = System.Drawing.Color.Yellow;
            graphDrain.ColorFadeEnd = System.Drawing.Color.FromArgb(64, 64, 192);
            graphDrain.ColorFadeStart = System.Drawing.Color.Black;
            graphDrain.ColorHighlight = System.Drawing.Color.Gray;
            graphDrain.ColorLines = System.Drawing.Color.Black;
            graphDrain.ColorMarkerInner = System.Drawing.Color.Black;
            graphDrain.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDrain.ColorOvercap = System.Drawing.Color.Black;
            graphDrain.DifferentiateColors = false;
            graphDrain.DrawRuler = false;
            graphDrain.Dual = true;
            graphDrain.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDrain.EnhBarColors");
            graphDrain.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphDrain.ForcedMax = 0F;
            graphDrain.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphDrain.Highlight = true;
            graphDrain.ItemFontSizeOverride = 0F;
            graphDrain.ItemHeight = 10;
            graphDrain.Lines = true;
            graphDrain.Location = new System.Drawing.Point(15, 385);
            graphDrain.MarkerValue = 0F;
            graphDrain.Max = 100F;
            graphDrain.MaxItems = 60;
            graphDrain.Name = "graphDrain";
            graphDrain.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphDrain.NegativeBaseColor = System.Drawing.Color.Navy;
            graphDrain.NegativeEnhColor = System.Drawing.Color.Olive;
            graphDrain.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphDrain.OuterBorder = false;
            graphDrain.Overcap = false;
            graphDrain.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDrain.OvercapColors");
            graphDrain.PaddingX = 2F;
            graphDrain.PaddingY = 2F;
            graphDrain.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDrain.PerItemScales");
            graphDrain.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDrain.ScaleHeight = 32;
            graphDrain.ScaleIndex = 8;
            graphDrain.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphDrain.ShowScale = false;
            graphDrain.SingleLineLabels = true;
            graphDrain.Size = new System.Drawing.Size(300, 15);
            graphDrain.Style = Enums.GraphStyle.baseOnly;
            graphDrain.TabIndex = 8;
            graphDrain.TextWidth = 125;
            // 
            // graphRes
            // 
            graphRes.BackColor = System.Drawing.Color.Black;
            graphRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRes.BackgroundImage");
            graphRes.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphRes.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.BaseBarColors");
            graphRes.Border = true;
            graphRes.BorderColor = System.Drawing.Color.Black;
            graphRes.Clickable = false;
            graphRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphRes.ColorBase = System.Drawing.Color.FromArgb(0, 192, 192);
            graphRes.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 128);
            graphRes.ColorFadeEnd = System.Drawing.Color.Teal;
            graphRes.ColorFadeStart = System.Drawing.Color.Black;
            graphRes.ColorHighlight = System.Drawing.Color.Gray;
            graphRes.ColorLines = System.Drawing.Color.Black;
            graphRes.ColorMarkerInner = System.Drawing.Color.Black;
            graphRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphRes.ColorOvercap = System.Drawing.Color.Black;
            graphRes.DifferentiateColors = false;
            graphRes.DrawRuler = false;
            graphRes.Dual = true;
            graphRes.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.EnhBarColors");
            graphRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphRes.ForcedMax = 0F;
            graphRes.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphRes.Highlight = true;
            graphRes.ItemFontSizeOverride = 0F;
            graphRes.ItemHeight = 10;
            graphRes.Lines = true;
            graphRes.Location = new System.Drawing.Point(15, 193);
            graphRes.MarkerValue = 0F;
            graphRes.Max = 100F;
            graphRes.MaxItems = 60;
            graphRes.Name = "graphRes";
            graphRes.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphRes.NegativeBaseColor = System.Drawing.Color.Navy;
            graphRes.NegativeEnhColor = System.Drawing.Color.Olive;
            graphRes.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphRes.OuterBorder = false;
            graphRes.Overcap = false;
            graphRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.OvercapColors");
            graphRes.PaddingX = 2F;
            graphRes.PaddingY = 4F;
            graphRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRes.PerItemScales");
            graphRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRes.ScaleHeight = 32;
            graphRes.ScaleIndex = 8;
            graphRes.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphRes.ShowScale = false;
            graphRes.SingleLineLabels = true;
            graphRes.Size = new System.Drawing.Size(300, 116);
            graphRes.Style = Enums.GraphStyle.Stacked;
            graphRes.TabIndex = 2;
            graphRes.TextWidth = 125;
            // 
            // graphRec
            // 
            graphRec.BackColor = System.Drawing.Color.Black;
            graphRec.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRec.BackgroundImage");
            graphRec.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphRec.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRec.BaseBarColors");
            graphRec.Border = true;
            graphRec.BorderColor = System.Drawing.Color.Black;
            graphRec.Clickable = false;
            graphRec.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphRec.ColorBase = System.Drawing.Color.RoyalBlue;
            graphRec.ColorEnh = System.Drawing.Color.Yellow;
            graphRec.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 0, 192);
            graphRec.ColorFadeStart = System.Drawing.Color.Black;
            graphRec.ColorHighlight = System.Drawing.Color.Gray;
            graphRec.ColorLines = System.Drawing.Color.Black;
            graphRec.ColorMarkerInner = System.Drawing.Color.Black;
            graphRec.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphRec.ColorOvercap = System.Drawing.Color.Black;
            graphRec.DifferentiateColors = false;
            graphRec.DrawRuler = false;
            graphRec.Dual = true;
            graphRec.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRec.EnhBarColors");
            graphRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphRec.ForcedMax = 0F;
            graphRec.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphRec.Highlight = true;
            graphRec.ItemFontSizeOverride = 0F;
            graphRec.ItemHeight = 10;
            graphRec.Lines = true;
            graphRec.Location = new System.Drawing.Point(15, 367);
            graphRec.MarkerValue = 0F;
            graphRec.Max = 100F;
            graphRec.MaxItems = 60;
            graphRec.Name = "graphRec";
            graphRec.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphRec.NegativeBaseColor = System.Drawing.Color.Navy;
            graphRec.NegativeEnhColor = System.Drawing.Color.Olive;
            graphRec.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphRec.OuterBorder = false;
            graphRec.Overcap = false;
            graphRec.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRec.OvercapColors");
            graphRec.PaddingX = 2F;
            graphRec.PaddingY = 2F;
            graphRec.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRec.PerItemScales");
            graphRec.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRec.ScaleHeight = 32;
            graphRec.ScaleIndex = 8;
            graphRec.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphRec.ShowScale = false;
            graphRec.SingleLineLabels = true;
            graphRec.Size = new System.Drawing.Size(300, 15);
            graphRec.Style = Enums.GraphStyle.baseOnly;
            graphRec.TabIndex = 6;
            graphRec.TextWidth = 125;
            // 
            // graphRegen
            // 
            graphRegen.BackColor = System.Drawing.Color.Black;
            graphRegen.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRegen.BackgroundImage");
            graphRegen.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphRegen.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRegen.BaseBarColors");
            graphRegen.Border = true;
            graphRegen.BorderColor = System.Drawing.Color.Black;
            graphRegen.Clickable = false;
            graphRegen.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphRegen.ColorBase = System.Drawing.Color.FromArgb(64, 255, 64);
            graphRegen.ColorEnh = System.Drawing.Color.Yellow;
            graphRegen.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 192, 0);
            graphRegen.ColorFadeStart = System.Drawing.Color.Black;
            graphRegen.ColorHighlight = System.Drawing.Color.Gray;
            graphRegen.ColorLines = System.Drawing.Color.Black;
            graphRegen.ColorMarkerInner = System.Drawing.Color.Black;
            graphRegen.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphRegen.ColorOvercap = System.Drawing.Color.Black;
            graphRegen.DifferentiateColors = false;
            graphRegen.DrawRuler = false;
            graphRegen.Dual = true;
            graphRegen.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRegen.EnhBarColors");
            graphRegen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphRegen.ForcedMax = 0F;
            graphRegen.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphRegen.Highlight = true;
            graphRegen.ItemFontSizeOverride = 0F;
            graphRegen.ItemHeight = 10;
            graphRegen.Lines = true;
            graphRegen.Location = new System.Drawing.Point(15, 331);
            graphRegen.MarkerValue = 0F;
            graphRegen.Max = 100F;
            graphRegen.MaxItems = 60;
            graphRegen.Name = "graphRegen";
            graphRegen.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphRegen.NegativeBaseColor = System.Drawing.Color.Navy;
            graphRegen.NegativeEnhColor = System.Drawing.Color.Olive;
            graphRegen.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphRegen.OuterBorder = false;
            graphRegen.Overcap = false;
            graphRegen.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRegen.OvercapColors");
            graphRegen.PaddingX = 2F;
            graphRegen.PaddingY = 2F;
            graphRegen.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRegen.PerItemScales");
            graphRegen.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRegen.ScaleHeight = 32;
            graphRegen.ScaleIndex = 8;
            graphRegen.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphRegen.ShowScale = false;
            graphRegen.SingleLineLabels = true;
            graphRegen.Size = new System.Drawing.Size(300, 15);
            graphRegen.Style = Enums.GraphStyle.baseOnly;
            graphRegen.TabIndex = 4;
            graphRegen.TextWidth = 125;
            // 
            // Panel1
            // 
            Panel1.BackColor = System.Drawing.Color.Black;
            Panel1.Location = new System.Drawing.Point(17, 321);
            Panel1.Name = "Panel1";
            Panel1.Size = new System.Drawing.Size(298, 88);
            Panel1.TabIndex = 10;
            // 
            // pnlMisc
            // 
            pnlMisc.BackColor = System.Drawing.Color.FromArgb(32, 0, 32);
            pnlMisc.Controls.Add(label1);
            pnlMisc.Controls.Add(rbMSec);
            pnlMisc.Controls.Add(rbFPS);
            pnlMisc.Controls.Add(rbKPH);
            pnlMisc.Controls.Add(rbMPH);
            pnlMisc.Controls.Add(lblStealth);
            pnlMisc.Controls.Add(graphStealth);
            pnlMisc.Controls.Add(lblMisc);
            pnlMisc.Controls.Add(graphMovement);
            pnlMisc.Controls.Add(lblMovement);
            pnlMisc.Controls.Add(Panel2);
            pnlMisc.Location = new System.Drawing.Point(330, 31);
            pnlMisc.Name = "pnlMisc";
            pnlMisc.Size = new System.Drawing.Size(320, 445);
            pnlMisc.TabIndex = 10;
            pnlMisc.Visible = false;
            // 
            // label1
            // 
            label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(190, 177);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(125, 16);
            label1.TabIndex = 19;
            label1.Text = "[Customize]";
            label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            label1.Click += label1_Click;
            label1.MouseEnter += label1_MouseEnter;
            label1.MouseLeave += label1_MouseLeave;
            // 
            // rbMSec
            // 
            rbMSec.Location = new System.Drawing.Point(225, 81);
            rbMSec.Name = "rbMSec";
            rbMSec.Size = new System.Drawing.Size(84, 24);
            rbMSec.TabIndex = 18;
            rbMSec.Text = "Meters/Sec";
            rbMSec.UseVisualStyleBackColor = true;
            rbMSec.CheckedChanged += RbSpeedCheckedChanged;
            // 
            // rbFPS
            // 
            rbFPS.Location = new System.Drawing.Point(147, 81);
            rbFPS.Name = "rbFPS";
            rbFPS.Size = new System.Drawing.Size(74, 24);
            rbFPS.TabIndex = 17;
            rbFPS.Text = "Feet/Sec";
            rbFPS.UseVisualStyleBackColor = true;
            rbFPS.CheckedChanged += RbSpeedCheckedChanged;
            // 
            // rbKPH
            // 
            rbKPH.Location = new System.Drawing.Point(82, 81);
            rbKPH.Name = "rbKPH";
            rbKPH.Size = new System.Drawing.Size(59, 24);
            rbKPH.TabIndex = 16;
            rbKPH.Text = "KPH";
            toolTip1.SetToolTip(rbKPH, "Kilometers per hour");
            rbKPH.UseVisualStyleBackColor = true;
            rbKPH.CheckedChanged += RbSpeedCheckedChanged;
            // 
            // rbMPH
            // 
            rbMPH.Checked = true;
            rbMPH.Location = new System.Drawing.Point(21, 81);
            rbMPH.Name = "rbMPH";
            rbMPH.Size = new System.Drawing.Size(59, 24);
            rbMPH.TabIndex = 15;
            rbMPH.TabStop = true;
            rbMPH.Text = "MPH";
            toolTip1.SetToolTip(rbMPH, "Miles per hour");
            rbMPH.UseVisualStyleBackColor = true;
            rbMPH.CheckedChanged += RbSpeedCheckedChanged;
            // 
            // lblStealth
            // 
            lblStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblStealth.Location = new System.Drawing.Point(3, 109);
            lblStealth.Name = "lblStealth";
            lblStealth.Size = new System.Drawing.Size(125, 16);
            lblStealth.TabIndex = 13;
            lblStealth.Text = "Stealth:";
            lblStealth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphStealth
            // 
            graphStealth.BackColor = System.Drawing.Color.Black;
            graphStealth.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphStealth.BackgroundImage");
            graphStealth.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphStealth.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStealth.BaseBarColors");
            graphStealth.Border = true;
            graphStealth.BorderColor = System.Drawing.Color.Black;
            graphStealth.Clickable = false;
            graphStealth.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphStealth.ColorBase = System.Drawing.Color.LightSlateGray;
            graphStealth.ColorEnh = System.Drawing.Color.Yellow;
            graphStealth.ColorFadeEnd = System.Drawing.Color.DarkSlateBlue;
            graphStealth.ColorFadeStart = System.Drawing.Color.Black;
            graphStealth.ColorHighlight = System.Drawing.Color.Gray;
            graphStealth.ColorLines = System.Drawing.Color.Black;
            graphStealth.ColorMarkerInner = System.Drawing.Color.Black;
            graphStealth.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphStealth.ColorOvercap = System.Drawing.Color.Black;
            graphStealth.DifferentiateColors = false;
            graphStealth.DrawRuler = false;
            graphStealth.Dual = false;
            graphStealth.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStealth.EnhBarColors");
            graphStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphStealth.ForcedMax = 0F;
            graphStealth.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphStealth.Highlight = true;
            graphStealth.ItemFontSizeOverride = 0F;
            graphStealth.ItemHeight = 10;
            graphStealth.Lines = true;
            graphStealth.Location = new System.Drawing.Point(15, 128);
            graphStealth.MarkerValue = 0F;
            graphStealth.Max = 100F;
            graphStealth.MaxItems = 60;
            graphStealth.Name = "graphStealth";
            graphStealth.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphStealth.NegativeBaseColor = System.Drawing.Color.Navy;
            graphStealth.NegativeEnhColor = System.Drawing.Color.Olive;
            graphStealth.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphStealth.OuterBorder = false;
            graphStealth.Overcap = false;
            graphStealth.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStealth.OvercapColors");
            graphStealth.PaddingX = 2F;
            graphStealth.PaddingY = 4F;
            graphStealth.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphStealth.PerItemScales");
            graphStealth.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphStealth.ScaleHeight = 32;
            graphStealth.ScaleIndex = 8;
            graphStealth.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphStealth.ShowScale = false;
            graphStealth.SingleLineLabels = true;
            graphStealth.Size = new System.Drawing.Size(300, 46);
            graphStealth.Style = Enums.GraphStyle.baseOnly;
            graphStealth.TabIndex = 12;
            graphStealth.TextWidth = 125;
            // 
            // lblMisc
            // 
            lblMisc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblMisc.Location = new System.Drawing.Point(3, 177);
            lblMisc.Name = "lblMisc";
            lblMisc.Size = new System.Drawing.Size(125, 16);
            lblMisc.TabIndex = 8;
            lblMisc.Text = "Misc:";
            lblMisc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphMovement
            // 
            graphMovement.BackColor = System.Drawing.Color.Black;
            graphMovement.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphMovement.BackgroundImage");
            graphMovement.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphMovement.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.BaseBarColors");
            graphMovement.Border = true;
            graphMovement.BorderColor = System.Drawing.Color.Black;
            graphMovement.Clickable = false;
            graphMovement.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphMovement.ColorBase = System.Drawing.Color.FromArgb(0, 192, 128);
            graphMovement.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 128);
            graphMovement.ColorFadeEnd = System.Drawing.Color.FromArgb(0, 128, 96);
            graphMovement.ColorFadeStart = System.Drawing.Color.Black;
            graphMovement.ColorHighlight = System.Drawing.Color.Gray;
            graphMovement.ColorLines = System.Drawing.Color.Black;
            graphMovement.ColorMarkerInner = System.Drawing.Color.Black;
            graphMovement.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphMovement.ColorOvercap = System.Drawing.Color.Black;
            graphMovement.DifferentiateColors = false;
            graphMovement.DrawRuler = false;
            graphMovement.Dual = true;
            graphMovement.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.EnhBarColors");
            graphMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphMovement.ForcedMax = 0F;
            graphMovement.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphMovement.Highlight = true;
            graphMovement.ItemFontSizeOverride = 0F;
            graphMovement.ItemHeight = 10;
            graphMovement.Lines = true;
            graphMovement.Location = new System.Drawing.Point(15, 17);
            graphMovement.MarkerValue = 0F;
            graphMovement.Max = 100F;
            graphMovement.MaxItems = 60;
            graphMovement.Name = "graphMovement";
            graphMovement.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphMovement.NegativeBaseColor = System.Drawing.Color.Navy;
            graphMovement.NegativeEnhColor = System.Drawing.Color.Olive;
            graphMovement.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphMovement.OuterBorder = false;
            graphMovement.Overcap = false;
            graphMovement.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.OvercapColors");
            graphMovement.PaddingX = 2F;
            graphMovement.PaddingY = 4F;
            graphMovement.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphMovement.PerItemScales");
            graphMovement.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphMovement.ScaleHeight = 32;
            graphMovement.ScaleIndex = 8;
            graphMovement.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphMovement.ShowScale = false;
            graphMovement.SingleLineLabels = true;
            graphMovement.Size = new System.Drawing.Size(300, 60);
            graphMovement.Style = Enums.GraphStyle.Stacked;
            graphMovement.TabIndex = 2;
            graphMovement.TextWidth = 125;
            // 
            // lblMovement
            // 
            lblMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblMovement.Location = new System.Drawing.Point(3, 0);
            lblMovement.Name = "lblMovement";
            lblMovement.Size = new System.Drawing.Size(125, 16);
            lblMovement.TabIndex = 3;
            lblMovement.Text = "Movement:";
            lblMovement.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // Panel2
            // 
            Panel2.BackColor = System.Drawing.Color.Black;
            Panel2.Location = new System.Drawing.Point(15, 196);
            Panel2.Name = "Panel2";
            Panel2.Size = new System.Drawing.Size(300, 194);
            Panel2.TabIndex = 14;
            // 
            // tab1
            // 
            tab1.Location = new System.Drawing.Point(112, 3);
            tab1.Name = "tab1";
            tab1.Size = new System.Drawing.Size(105, 22);
            tab1.TabIndex = 94;
            tab1.TabStop = false;
            tab1.Click += Tab1Click;
            tab1.Paint += Tab1Paint;
            // 
            // tab0
            // 
            tab0.Location = new System.Drawing.Point(4, 3);
            tab0.Name = "tab0";
            tab0.Size = new System.Drawing.Size(105, 22);
            tab0.TabIndex = 93;
            tab0.TabStop = false;
            tab0.Click += Tab0Click;
            tab0.Paint += Tab0Paint;
            // 
            // pbTopMost
            // 
            pbTopMost.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbTopMost.Location = new System.Drawing.Point(4, 480);
            pbTopMost.Name = "pbTopMost";
            pbTopMost.Size = new System.Drawing.Size(105, 22);
            pbTopMost.TabIndex = 95;
            pbTopMost.TabStop = false;
            pbTopMost.Click += PbTopMostClick;
            pbTopMost.Paint += PbTopMostPaint;
            // 
            // pbClose
            // 
            pbClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbClose.Location = new System.Drawing.Point(220, 480);
            pbClose.Name = "pbClose";
            pbClose.Size = new System.Drawing.Size(105, 22);
            pbClose.TabIndex = 96;
            pbClose.TabStop = false;
            pbClose.Click += PbCloseClick;
            pbClose.Paint += PbClosePaint;
            // 
            // pnlStatus
            // 
            pnlStatus.BackColor = System.Drawing.Color.FromArgb(0, 32, 0);
            pnlStatus.Controls.Add(graphSRes);
            pnlStatus.Controls.Add(lblSRes);
            pnlStatus.Controls.Add(graphSDeb);
            pnlStatus.Controls.Add(lblSDeb);
            pnlStatus.Controls.Add(graphSProt);
            pnlStatus.Controls.Add(lblSProt);
            pnlStatus.Location = new System.Drawing.Point(656, 31);
            pnlStatus.Name = "pnlStatus";
            pnlStatus.Size = new System.Drawing.Size(320, 445);
            pnlStatus.TabIndex = 97;
            pnlStatus.Visible = false;
            // 
            // graphSRes
            // 
            graphSRes.BackColor = System.Drawing.Color.Black;
            graphSRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphSRes.BackgroundImage");
            graphSRes.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphSRes.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSRes.BaseBarColors");
            graphSRes.Border = true;
            graphSRes.BorderColor = System.Drawing.Color.Black;
            graphSRes.Clickable = false;
            graphSRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphSRes.ColorBase = System.Drawing.Color.Yellow;
            graphSRes.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 128);
            graphSRes.ColorFadeEnd = System.Drawing.Color.Olive;
            graphSRes.ColorFadeStart = System.Drawing.Color.Black;
            graphSRes.ColorHighlight = System.Drawing.Color.Gray;
            graphSRes.ColorLines = System.Drawing.Color.Black;
            graphSRes.ColorMarkerInner = System.Drawing.Color.Black;
            graphSRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphSRes.ColorOvercap = System.Drawing.Color.Black;
            graphSRes.DifferentiateColors = false;
            graphSRes.DrawRuler = false;
            graphSRes.Dual = true;
            graphSRes.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSRes.EnhBarColors");
            graphSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphSRes.ForcedMax = 0F;
            graphSRes.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphSRes.Highlight = true;
            graphSRes.ItemFontSizeOverride = 0F;
            graphSRes.ItemHeight = 9;
            graphSRes.Lines = true;
            graphSRes.Location = new System.Drawing.Point(15, 175);
            graphSRes.MarkerValue = 0F;
            graphSRes.Max = 100F;
            graphSRes.MaxItems = 60;
            graphSRes.Name = "graphSRes";
            graphSRes.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphSRes.NegativeBaseColor = System.Drawing.Color.Navy;
            graphSRes.NegativeEnhColor = System.Drawing.Color.Olive;
            graphSRes.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphSRes.OuterBorder = false;
            graphSRes.Overcap = false;
            graphSRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSRes.OvercapColors");
            graphSRes.PaddingX = 2F;
            graphSRes.PaddingY = 3F;
            graphSRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSRes.PerItemScales");
            graphSRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSRes.ScaleHeight = 32;
            graphSRes.ScaleIndex = 8;
            graphSRes.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphSRes.ShowScale = false;
            graphSRes.SingleLineLabels = true;
            graphSRes.Size = new System.Drawing.Size(300, 134);
            graphSRes.Style = Enums.GraphStyle.baseOnly;
            graphSRes.TabIndex = 14;
            graphSRes.TextWidth = 125;
            // 
            // lblSRes
            // 
            lblSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblSRes.Location = new System.Drawing.Point(3, 156);
            lblSRes.Name = "lblSRes";
            lblSRes.Size = new System.Drawing.Size(125, 16);
            lblSRes.TabIndex = 13;
            lblSRes.Text = "Status Resistance:";
            lblSRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphSDeb
            // 
            graphSDeb.BackColor = System.Drawing.Color.Black;
            graphSDeb.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphSDeb.BackgroundImage");
            graphSDeb.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphSDeb.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSDeb.BaseBarColors");
            graphSDeb.Border = true;
            graphSDeb.BorderColor = System.Drawing.Color.Black;
            graphSDeb.Clickable = false;
            graphSDeb.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphSDeb.ColorBase = System.Drawing.Color.Cyan;
            graphSDeb.ColorEnh = System.Drawing.Color.Yellow;
            graphSDeb.ColorFadeEnd = System.Drawing.Color.Teal;
            graphSDeb.ColorFadeStart = System.Drawing.Color.Black;
            graphSDeb.ColorHighlight = System.Drawing.Color.Gray;
            graphSDeb.ColorLines = System.Drawing.Color.Black;
            graphSDeb.ColorMarkerInner = System.Drawing.Color.Black;
            graphSDeb.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphSDeb.ColorOvercap = System.Drawing.Color.Black;
            graphSDeb.DifferentiateColors = false;
            graphSDeb.DrawRuler = false;
            graphSDeb.Dual = true;
            graphSDeb.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSDeb.EnhBarColors");
            graphSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphSDeb.ForcedMax = 0F;
            graphSDeb.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphSDeb.Highlight = true;
            graphSDeb.ItemFontSizeOverride = 0F;
            graphSDeb.ItemHeight = 9;
            graphSDeb.Lines = true;
            graphSDeb.Location = new System.Drawing.Point(15, 333);
            graphSDeb.MarkerValue = 0F;
            graphSDeb.Max = 100F;
            graphSDeb.MaxItems = 60;
            graphSDeb.Name = "graphSDeb";
            graphSDeb.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphSDeb.NegativeBaseColor = System.Drawing.Color.Navy;
            graphSDeb.NegativeEnhColor = System.Drawing.Color.Olive;
            graphSDeb.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphSDeb.OuterBorder = false;
            graphSDeb.Overcap = false;
            graphSDeb.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSDeb.OvercapColors");
            graphSDeb.PaddingX = 2F;
            graphSDeb.PaddingY = 3F;
            graphSDeb.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSDeb.PerItemScales");
            graphSDeb.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSDeb.ScaleHeight = 32;
            graphSDeb.ScaleIndex = 8;
            graphSDeb.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphSDeb.ShowScale = false;
            graphSDeb.SingleLineLabels = true;
            graphSDeb.Size = new System.Drawing.Size(300, 100);
            graphSDeb.Style = Enums.GraphStyle.baseOnly;
            graphSDeb.TabIndex = 12;
            graphSDeb.TextWidth = 125;
            // 
            // lblSDeb
            // 
            lblSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblSDeb.Location = new System.Drawing.Point(3, 314);
            lblSDeb.Name = "lblSDeb";
            lblSDeb.Size = new System.Drawing.Size(125, 16);
            lblSDeb.TabIndex = 8;
            lblSDeb.Text = "Debuff Resistance:";
            lblSDeb.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphSProt
            // 
            graphSProt.BackColor = System.Drawing.Color.Black;
            graphSProt.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphSProt.BackgroundImage");
            graphSProt.BarsAlignment = CtlMultiGraph.BarAlignment.Left;
            graphSProt.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSProt.BaseBarColors");
            graphSProt.Border = true;
            graphSProt.BorderColor = System.Drawing.Color.Black;
            graphSProt.Clickable = false;
            graphSProt.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphSProt.ColorBase = System.Drawing.Color.FromArgb(255, 128, 0);
            graphSProt.ColorEnh = System.Drawing.Color.FromArgb(255, 128, 128);
            graphSProt.ColorFadeEnd = System.Drawing.Color.FromArgb(128, 64, 0);
            graphSProt.ColorFadeStart = System.Drawing.Color.Black;
            graphSProt.ColorHighlight = System.Drawing.Color.Gray;
            graphSProt.ColorLines = System.Drawing.Color.Black;
            graphSProt.ColorMarkerInner = System.Drawing.Color.Black;
            graphSProt.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphSProt.ColorOvercap = System.Drawing.Color.Black;
            graphSProt.DifferentiateColors = false;
            graphSProt.DrawRuler = false;
            graphSProt.Dual = true;
            graphSProt.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSProt.EnhBarColors");
            graphSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            graphSProt.ForcedMax = 0F;
            graphSProt.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphSProt.Highlight = true;
            graphSProt.ItemFontSizeOverride = 0F;
            graphSProt.ItemHeight = 9;
            graphSProt.Lines = true;
            graphSProt.Location = new System.Drawing.Point(15, 17);
            graphSProt.MarkerValue = 0F;
            graphSProt.Max = 100F;
            graphSProt.MaxItems = 60;
            graphSProt.Name = "graphSProt";
            graphSProt.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            graphSProt.NegativeBaseColor = System.Drawing.Color.Navy;
            graphSProt.NegativeEnhColor = System.Drawing.Color.Olive;
            graphSProt.NegativeOvercapColor = System.Drawing.Color.DarkMagenta;
            graphSProt.OuterBorder = false;
            graphSProt.Overcap = false;
            graphSProt.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSProt.OvercapColors");
            graphSProt.PaddingX = 2F;
            graphSProt.PaddingY = 3F;
            graphSProt.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSProt.PerItemScales");
            graphSProt.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSProt.ScaleHeight = 32;
            graphSProt.ScaleIndex = 8;
            graphSProt.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            graphSProt.ShowScale = false;
            graphSProt.SingleLineLabels = true;
            graphSProt.Size = new System.Drawing.Size(300, 136);
            graphSProt.Style = Enums.GraphStyle.baseOnly;
            graphSProt.TabIndex = 2;
            graphSProt.TextWidth = 125;
            // 
            // lblSProt
            // 
            lblSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            lblSProt.Location = new System.Drawing.Point(3, 0);
            lblSProt.Name = "lblSProt";
            lblSProt.Size = new System.Drawing.Size(125, 16);
            lblSProt.TabIndex = 3;
            lblSProt.Text = "Status Protection:";
            lblSProt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tab2
            // 
            tab2.Location = new System.Drawing.Point(220, 3);
            tab2.Name = "tab2";
            tab2.Size = new System.Drawing.Size(105, 22);
            tab2.TabIndex = 98;
            tab2.TabStop = false;
            tab2.Click += Tab2Click;
            tab2.Paint += Tab2Paint;
            // 
            // frmTotals
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(995, 505);
            Controls.Add(tab2);
            Controls.Add(pnlStatus);
            Controls.Add(pbClose);
            Controls.Add(pbTopMost);
            Controls.Add(tab1);
            Controls.Add(tab0);
            Controls.Add(pnlMisc);
            Controls.Add(pnlDRHE);
            Font = new System.Drawing.Font("Segoe UI", 8.25F);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximumSize = new System.Drawing.Size(1024, 603);
            MinimumSize = new System.Drawing.Size(344, 544);
            Name = "frmTotals";
            StartPosition = FormStartPosition.Manual;
            Text = "Totals for Self";
            TopMost = true;
            pnlDRHE.ResumeLayout(false);
            pnlMisc.ResumeLayout(false);
            ((ISupportInitialize)tab1).EndInit();
            ((ISupportInitialize)tab0).EndInit();
            ((ISupportInitialize)pbTopMost).EndInit();
            ((ISupportInitialize)pbClose).EndInit();
            pnlStatus.ResumeLayout(false);
            ((ISupportInitialize)tab2).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private Label lblDef;
        private Label lblMisc;
        private Label lblMovement;
        private Label lblRegenRec;
        private Label lblRes;
        private Label lblSDeb;
        private Label lblSProt;
        private Label lblSRes;
        private Label lblStealth;
        private Panel Panel1;
        private Panel Panel2;
        private PictureBox pbClose;
        private PictureBox pbTopMost;
        private Panel pnlDRHE;
        private Panel pnlMisc;
        private Panel pnlStatus;
        private RadioButton rbFPS;
        private RadioButton rbKPH;
        private RadioButton rbMPH;
        private RadioButton rbMSec;
        private PictureBox tab0;
        private PictureBox tab1;
        private PictureBox tab2;
        private CtlMultiGraph graphDef;
        private CtlMultiGraph graphDrain;
        private CtlMultiGraph graphHP;
        private CtlMultiGraph graphMaxEnd;
        private CtlMultiGraph graphMovement;
        private CtlMultiGraph graphRec;
        private CtlMultiGraph graphRegen;
        private CtlMultiGraph graphRes;
        private CtlMultiGraph graphSDeb;
        private CtlMultiGraph graphSProt;
        private CtlMultiGraph graphSRes;
        private CtlMultiGraph graphStealth;

        private ToolTip toolTip1;
        private Label label1;
    }
}