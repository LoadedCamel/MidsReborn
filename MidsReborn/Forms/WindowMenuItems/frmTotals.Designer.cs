using System.ComponentModel;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.WindowMenuItems
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
            rbMSec = new RadioButton();
            rbFPS = new RadioButton();
            rbKPH = new RadioButton();
            rbMPH = new RadioButton();
            lblStealth = new Label();
            graphStealth = new CtlMultiGraph();
            graphAcc = new CtlMultiGraph();
            graphToHit = new CtlMultiGraph();
            lblMisc = new Label();
            graphMovement = new CtlMultiGraph();
            lblMovement = new Label();
            graphHaste = new CtlMultiGraph();
            Panel2 = new Panel();
            graphElusivity = new CtlMultiGraph();
            graphThreat = new CtlMultiGraph();
            graphEndRdx = new CtlMultiGraph();
            graphDam = new CtlMultiGraph();
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
            Panel2.SuspendLayout();
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
            lblDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblDef.Location = new System.Drawing.Point(3, 0);
            lblDef.Name = "lblDef";
            lblDef.Size = new System.Drawing.Size(89, 16);
            lblDef.TabIndex = 1;
            lblDef.Text = "Defense:";
            lblDef.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRes
            // 
            lblRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblRes.Location = new System.Drawing.Point(3, 174);
            lblRes.Name = "lblRes";
            lblRes.Size = new System.Drawing.Size(125, 16);
            lblRes.TabIndex = 3;
            lblRes.Text = "Resistance:";
            lblRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRegenRec
            // 
            lblRegenRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphMaxEnd.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphMaxEnd.OuterBorder = false;
            graphMaxEnd.Overcap = false;
            graphMaxEnd.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMaxEnd.OvercapColors");
            graphMaxEnd.PaddingX = 2F;
            graphMaxEnd.PaddingY = 2F;
            graphMaxEnd.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphMaxEnd.PerItemScales");
            graphMaxEnd.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphMaxEnd.ScaleHeight = 32;
            graphMaxEnd.ScaleIndex = 8;
            graphMaxEnd.ShowScale = false;
            graphMaxEnd.Size = new System.Drawing.Size(300, 15);
            graphMaxEnd.Style = Enums.GraphStyle.baseOnly;
            graphMaxEnd.TabIndex = 7;
            graphMaxEnd.TextWidth = 125;
            // 
            // graphHP
            // 
            graphHP.BackColor = System.Drawing.Color.Black;
            graphHP.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphHP.BackgroundImage");
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
            graphHP.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphHP.OuterBorder = false;
            graphHP.Overcap = false;
            graphHP.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHP.OvercapColors");
            graphHP.PaddingX = 2F;
            graphHP.PaddingY = 2F;
            graphHP.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphHP.PerItemScales");
            graphHP.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphHP.ScaleHeight = 32;
            graphHP.ScaleIndex = 8;
            graphHP.ShowScale = false;
            graphHP.Size = new System.Drawing.Size(300, 15);
            graphHP.Style = Enums.GraphStyle.baseOnly;
            graphHP.TabIndex = 9;
            graphHP.TextWidth = 125;
            // 
            // graphDef
            // 
            graphDef.BackColor = System.Drawing.Color.Black;
            graphDef.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDef.BackgroundImage");
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
            graphDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphDef.OuterBorder = false;
            graphDef.Overcap = false;
            graphDef.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDef.OvercapColors");
            graphDef.PaddingX = 2F;
            graphDef.PaddingY = 4F;
            graphDef.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDef.PerItemScales");
            graphDef.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDef.ScaleHeight = 32;
            graphDef.ScaleIndex = 8;
            graphDef.ShowScale = false;
            graphDef.Size = new System.Drawing.Size(300, 152);
            graphDef.Style = Enums.GraphStyle.baseOnly;
            graphDef.TabIndex = 0;
            graphDef.TextWidth = 125;
            // 
            // graphDrain
            // 
            graphDrain.BackColor = System.Drawing.Color.Black;
            graphDrain.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDrain.BackgroundImage");
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
            graphDrain.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphDrain.OuterBorder = false;
            graphDrain.Overcap = false;
            graphDrain.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDrain.OvercapColors");
            graphDrain.PaddingX = 2F;
            graphDrain.PaddingY = 2F;
            graphDrain.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDrain.PerItemScales");
            graphDrain.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDrain.ScaleHeight = 32;
            graphDrain.ScaleIndex = 8;
            graphDrain.ShowScale = false;
            graphDrain.Size = new System.Drawing.Size(300, 15);
            graphDrain.Style = Enums.GraphStyle.baseOnly;
            graphDrain.TabIndex = 8;
            graphDrain.TextWidth = 125;
            // 
            // graphRes
            // 
            graphRes.BackColor = System.Drawing.Color.Black;
            graphRes.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRes.BackgroundImage");
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
            graphRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphRes.OuterBorder = false;
            graphRes.Overcap = false;
            graphRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRes.OvercapColors");
            graphRes.PaddingX = 2F;
            graphRes.PaddingY = 4F;
            graphRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRes.PerItemScales");
            graphRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRes.ScaleHeight = 32;
            graphRes.ScaleIndex = 8;
            graphRes.ShowScale = false;
            graphRes.Size = new System.Drawing.Size(300, 116);
            graphRes.Style = Enums.GraphStyle.Stacked;
            graphRes.TabIndex = 2;
            graphRes.TextWidth = 125;
            // 
            // graphRec
            // 
            graphRec.BackColor = System.Drawing.Color.Black;
            graphRec.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRec.BackgroundImage");
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
            graphRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphRec.OuterBorder = false;
            graphRec.Overcap = false;
            graphRec.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRec.OvercapColors");
            graphRec.PaddingX = 2F;
            graphRec.PaddingY = 2F;
            graphRec.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRec.PerItemScales");
            graphRec.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRec.ScaleHeight = 32;
            graphRec.ScaleIndex = 8;
            graphRec.ShowScale = false;
            graphRec.Size = new System.Drawing.Size(300, 15);
            graphRec.Style = Enums.GraphStyle.baseOnly;
            graphRec.TabIndex = 6;
            graphRec.TextWidth = 125;
            // 
            // graphRegen
            // 
            graphRegen.BackColor = System.Drawing.Color.Black;
            graphRegen.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphRegen.BackgroundImage");
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
            graphRegen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphRegen.OuterBorder = false;
            graphRegen.Overcap = false;
            graphRegen.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphRegen.OvercapColors");
            graphRegen.PaddingX = 2F;
            graphRegen.PaddingY = 2F;
            graphRegen.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphRegen.PerItemScales");
            graphRegen.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphRegen.ScaleHeight = 32;
            graphRegen.ScaleIndex = 8;
            graphRegen.ShowScale = false;
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
            pnlMisc.Controls.Add(rbMSec);
            pnlMisc.Controls.Add(rbFPS);
            pnlMisc.Controls.Add(rbKPH);
            pnlMisc.Controls.Add(rbMPH);
            pnlMisc.Controls.Add(lblStealth);
            pnlMisc.Controls.Add(graphStealth);
            pnlMisc.Controls.Add(graphAcc);
            pnlMisc.Controls.Add(graphToHit);
            pnlMisc.Controls.Add(lblMisc);
            pnlMisc.Controls.Add(graphMovement);
            pnlMisc.Controls.Add(lblMovement);
            pnlMisc.Controls.Add(graphHaste);
            pnlMisc.Controls.Add(Panel2);
            pnlMisc.Location = new System.Drawing.Point(330, 31);
            pnlMisc.Name = "pnlMisc";
            pnlMisc.Size = new System.Drawing.Size(320, 445);
            pnlMisc.TabIndex = 10;
            pnlMisc.Visible = false;
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
            lblStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphStealth.OuterBorder = false;
            graphStealth.Overcap = false;
            graphStealth.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphStealth.OvercapColors");
            graphStealth.PaddingX = 2F;
            graphStealth.PaddingY = 4F;
            graphStealth.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphStealth.PerItemScales");
            graphStealth.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphStealth.ScaleHeight = 32;
            graphStealth.ScaleIndex = 8;
            graphStealth.ShowScale = false;
            graphStealth.Size = new System.Drawing.Size(300, 46);
            graphStealth.Style = Enums.GraphStyle.baseOnly;
            graphStealth.TabIndex = 12;
            graphStealth.TextWidth = 125;
            // 
            // graphAcc
            // 
            graphAcc.BackColor = System.Drawing.Color.Black;
            graphAcc.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphAcc.BackgroundImage");
            graphAcc.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAcc.BaseBarColors");
            graphAcc.Border = true;
            graphAcc.BorderColor = System.Drawing.Color.Black;
            graphAcc.Clickable = false;
            graphAcc.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphAcc.ColorBase = System.Drawing.Color.Yellow;
            graphAcc.ColorEnh = System.Drawing.Color.Yellow;
            graphAcc.ColorFadeEnd = System.Drawing.Color.Olive;
            graphAcc.ColorFadeStart = System.Drawing.Color.Black;
            graphAcc.ColorHighlight = System.Drawing.Color.Gray;
            graphAcc.ColorLines = System.Drawing.Color.Black;
            graphAcc.ColorMarkerInner = System.Drawing.Color.Black;
            graphAcc.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphAcc.ColorOvercap = System.Drawing.Color.Black;
            graphAcc.DifferentiateColors = false;
            graphAcc.DrawRuler = false;
            graphAcc.Dual = true;
            graphAcc.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAcc.EnhBarColors");
            graphAcc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphAcc.ForcedMax = 0F;
            graphAcc.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphAcc.Highlight = true;
            graphAcc.ItemFontSizeOverride = 0F;
            graphAcc.ItemHeight = 10;
            graphAcc.Lines = true;
            graphAcc.Location = new System.Drawing.Point(15, 234);
            graphAcc.MarkerValue = 0F;
            graphAcc.Max = 100F;
            graphAcc.MaxItems = 60;
            graphAcc.Name = "graphAcc";
            graphAcc.OuterBorder = false;
            graphAcc.Overcap = false;
            graphAcc.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphAcc.OvercapColors");
            graphAcc.PaddingX = 2F;
            graphAcc.PaddingY = 2F;
            graphAcc.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphAcc.PerItemScales");
            graphAcc.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphAcc.ScaleHeight = 32;
            graphAcc.ScaleIndex = 8;
            graphAcc.ShowScale = false;
            graphAcc.Size = new System.Drawing.Size(300, 15);
            graphAcc.Style = Enums.GraphStyle.baseOnly;
            graphAcc.TabIndex = 10;
            graphAcc.TextWidth = 125;
            // 
            // graphToHit
            // 
            graphToHit.BackColor = System.Drawing.Color.Black;
            graphToHit.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphToHit.BackgroundImage");
            graphToHit.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.BaseBarColors");
            graphToHit.Border = true;
            graphToHit.BorderColor = System.Drawing.Color.Black;
            graphToHit.Clickable = false;
            graphToHit.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphToHit.ColorBase = System.Drawing.Color.FromArgb(255, 255, 128);
            graphToHit.ColorEnh = System.Drawing.Color.Yellow;
            graphToHit.ColorFadeEnd = System.Drawing.Color.FromArgb(192, 192, 0);
            graphToHit.ColorFadeStart = System.Drawing.Color.Black;
            graphToHit.ColorHighlight = System.Drawing.Color.Gray;
            graphToHit.ColorLines = System.Drawing.Color.Black;
            graphToHit.ColorMarkerInner = System.Drawing.Color.Black;
            graphToHit.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphToHit.ColorOvercap = System.Drawing.Color.Black;
            graphToHit.DifferentiateColors = false;
            graphToHit.DrawRuler = false;
            graphToHit.Dual = true;
            graphToHit.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.EnhBarColors");
            graphToHit.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphToHit.ForcedMax = 0F;
            graphToHit.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphToHit.Highlight = true;
            graphToHit.ItemFontSizeOverride = 0F;
            graphToHit.ItemHeight = 10;
            graphToHit.Lines = true;
            graphToHit.Location = new System.Drawing.Point(15, 215);
            graphToHit.MarkerValue = 0F;
            graphToHit.Max = 100F;
            graphToHit.MaxItems = 60;
            graphToHit.Name = "graphToHit";
            graphToHit.OuterBorder = false;
            graphToHit.Overcap = false;
            graphToHit.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphToHit.OvercapColors");
            graphToHit.PaddingX = 2F;
            graphToHit.PaddingY = 2F;
            graphToHit.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphToHit.PerItemScales");
            graphToHit.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphToHit.ScaleHeight = 32;
            graphToHit.ScaleIndex = 8;
            graphToHit.ShowScale = false;
            graphToHit.Size = new System.Drawing.Size(300, 15);
            graphToHit.Style = Enums.GraphStyle.baseOnly;
            graphToHit.TabIndex = 9;
            graphToHit.TextWidth = 125;
            // 
            // lblMisc
            // 
            lblMisc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphMovement.OuterBorder = false;
            graphMovement.Overcap = false;
            graphMovement.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphMovement.OvercapColors");
            graphMovement.PaddingX = 2F;
            graphMovement.PaddingY = 4F;
            graphMovement.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphMovement.PerItemScales");
            graphMovement.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphMovement.ScaleHeight = 32;
            graphMovement.ScaleIndex = 8;
            graphMovement.ShowScale = false;
            graphMovement.Size = new System.Drawing.Size(300, 61);
            graphMovement.Style = Enums.GraphStyle.Stacked;
            graphMovement.TabIndex = 2;
            graphMovement.TextWidth = 125;
            // 
            // lblMovement
            // 
            lblMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblMovement.Location = new System.Drawing.Point(3, 0);
            lblMovement.Name = "lblMovement";
            lblMovement.Size = new System.Drawing.Size(125, 16);
            lblMovement.TabIndex = 3;
            lblMovement.Text = "Movement:";
            lblMovement.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphHaste
            // 
            graphHaste.BackColor = System.Drawing.Color.Black;
            graphHaste.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphHaste.BackgroundImage");
            graphHaste.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.BaseBarColors");
            graphHaste.Border = true;
            graphHaste.BorderColor = System.Drawing.Color.Black;
            graphHaste.Clickable = false;
            graphHaste.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphHaste.ColorBase = System.Drawing.Color.FromArgb(255, 128, 0);
            graphHaste.ColorEnh = System.Drawing.Color.Yellow;
            graphHaste.ColorFadeEnd = System.Drawing.Color.FromArgb(192, 64, 0);
            graphHaste.ColorFadeStart = System.Drawing.Color.Black;
            graphHaste.ColorHighlight = System.Drawing.Color.Gray;
            graphHaste.ColorLines = System.Drawing.Color.Black;
            graphHaste.ColorMarkerInner = System.Drawing.Color.Black;
            graphHaste.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphHaste.ColorOvercap = System.Drawing.Color.Black;
            graphHaste.DifferentiateColors = false;
            graphHaste.DrawRuler = false;
            graphHaste.Dual = true;
            graphHaste.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.EnhBarColors");
            graphHaste.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphHaste.ForcedMax = 0F;
            graphHaste.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphHaste.Highlight = true;
            graphHaste.ItemFontSizeOverride = 0F;
            graphHaste.ItemHeight = 10;
            graphHaste.Lines = true;
            graphHaste.Location = new System.Drawing.Point(15, 196);
            graphHaste.MarkerValue = 0F;
            graphHaste.Max = 100F;
            graphHaste.MaxItems = 60;
            graphHaste.Name = "graphHaste";
            graphHaste.OuterBorder = false;
            graphHaste.Overcap = false;
            graphHaste.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphHaste.OvercapColors");
            graphHaste.PaddingX = 2F;
            graphHaste.PaddingY = 2F;
            graphHaste.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphHaste.PerItemScales");
            graphHaste.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphHaste.ScaleHeight = 32;
            graphHaste.ScaleIndex = 8;
            graphHaste.ShowScale = false;
            graphHaste.Size = new System.Drawing.Size(300, 15);
            graphHaste.Style = Enums.GraphStyle.baseOnly;
            graphHaste.TabIndex = 7;
            graphHaste.TextWidth = 125;
            // 
            // Panel2
            // 
            Panel2.BackColor = System.Drawing.Color.Black;
            Panel2.Controls.Add(graphElusivity);
            Panel2.Controls.Add(graphThreat);
            Panel2.Controls.Add(graphEndRdx);
            Panel2.Controls.Add(graphDam);
            Panel2.Location = new System.Drawing.Point(15, 196);
            Panel2.Name = "Panel2";
            Panel2.Size = new System.Drawing.Size(300, 194);
            Panel2.TabIndex = 14;
            // 
            // graphElusivity
            // 
            graphElusivity.BackColor = System.Drawing.Color.Black;
            graphElusivity.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphElusivity.BackgroundImage");
            graphElusivity.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.BaseBarColors");
            graphElusivity.Border = true;
            graphElusivity.BorderColor = System.Drawing.Color.Black;
            graphElusivity.Clickable = false;
            graphElusivity.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphElusivity.ColorBase = System.Drawing.Color.FromArgb(192, 0, 192);
            graphElusivity.ColorEnh = System.Drawing.Color.Yellow;
            graphElusivity.ColorFadeEnd = System.Drawing.Color.Purple;
            graphElusivity.ColorFadeStart = System.Drawing.Color.Black;
            graphElusivity.ColorHighlight = System.Drawing.Color.Gray;
            graphElusivity.ColorLines = System.Drawing.Color.Black;
            graphElusivity.ColorMarkerInner = System.Drawing.Color.Black;
            graphElusivity.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphElusivity.ColorOvercap = System.Drawing.Color.Black;
            graphElusivity.DifferentiateColors = false;
            graphElusivity.DrawRuler = false;
            graphElusivity.Dual = true;
            graphElusivity.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.EnhBarColors");
            graphElusivity.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphElusivity.ForcedMax = 0F;
            graphElusivity.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphElusivity.Highlight = true;
            graphElusivity.ItemFontSizeOverride = 0F;
            graphElusivity.ItemHeight = 10;
            graphElusivity.Lines = true;
            graphElusivity.Location = new System.Drawing.Point(0, 117);
            graphElusivity.MarkerValue = 0F;
            graphElusivity.Max = 100F;
            graphElusivity.MaxItems = 60;
            graphElusivity.Name = "graphElusivity";
            graphElusivity.OuterBorder = false;
            graphElusivity.Overcap = false;
            graphElusivity.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphElusivity.OvercapColors");
            graphElusivity.PaddingX = 2F;
            graphElusivity.PaddingY = 2F;
            graphElusivity.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphElusivity.PerItemScales");
            graphElusivity.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphElusivity.ScaleHeight = 32;
            graphElusivity.ScaleIndex = 8;
            graphElusivity.ShowScale = false;
            graphElusivity.Size = new System.Drawing.Size(300, 15);
            graphElusivity.Style = Enums.GraphStyle.baseOnly;
            graphElusivity.TabIndex = 14;
            graphElusivity.TextWidth = 125;
            // 
            // graphThreat
            // 
            graphThreat.BackColor = System.Drawing.Color.Black;
            graphThreat.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphThreat.BackgroundImage");
            graphThreat.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.BaseBarColors");
            graphThreat.Border = true;
            graphThreat.BorderColor = System.Drawing.Color.Black;
            graphThreat.Clickable = false;
            graphThreat.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphThreat.ColorBase = System.Drawing.Color.MediumPurple;
            graphThreat.ColorEnh = System.Drawing.Color.Yellow;
            graphThreat.ColorFadeEnd = System.Drawing.Color.DarkSlateBlue;
            graphThreat.ColorFadeStart = System.Drawing.Color.Black;
            graphThreat.ColorHighlight = System.Drawing.Color.Gray;
            graphThreat.ColorLines = System.Drawing.Color.Black;
            graphThreat.ColorMarkerInner = System.Drawing.Color.Black;
            graphThreat.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphThreat.ColorOvercap = System.Drawing.Color.Black;
            graphThreat.DifferentiateColors = false;
            graphThreat.DrawRuler = false;
            graphThreat.Dual = true;
            graphThreat.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.EnhBarColors");
            graphThreat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphThreat.ForcedMax = 0F;
            graphThreat.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphThreat.Highlight = true;
            graphThreat.ItemFontSizeOverride = 0F;
            graphThreat.ItemHeight = 10;
            graphThreat.Lines = true;
            graphThreat.Location = new System.Drawing.Point(0, 96);
            graphThreat.MarkerValue = 0F;
            graphThreat.Max = 100F;
            graphThreat.MaxItems = 60;
            graphThreat.Name = "graphThreat";
            graphThreat.OuterBorder = false;
            graphThreat.Overcap = false;
            graphThreat.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphThreat.OvercapColors");
            graphThreat.PaddingX = 2F;
            graphThreat.PaddingY = 2F;
            graphThreat.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphThreat.PerItemScales");
            graphThreat.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphThreat.ScaleHeight = 32;
            graphThreat.ScaleIndex = 8;
            graphThreat.ShowScale = false;
            graphThreat.Size = new System.Drawing.Size(300, 15);
            graphThreat.Style = Enums.GraphStyle.baseOnly;
            graphThreat.TabIndex = 13;
            graphThreat.TextWidth = 125;
            // 
            // graphEndRdx
            // 
            graphEndRdx.BackColor = System.Drawing.Color.Black;
            graphEndRdx.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphEndRdx.BackgroundImage");
            graphEndRdx.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.BaseBarColors");
            graphEndRdx.Border = true;
            graphEndRdx.BorderColor = System.Drawing.Color.Black;
            graphEndRdx.Clickable = false;
            graphEndRdx.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphEndRdx.ColorBase = System.Drawing.Color.RoyalBlue;
            graphEndRdx.ColorEnh = System.Drawing.Color.Yellow;
            graphEndRdx.ColorFadeEnd = System.Drawing.Color.SlateBlue;
            graphEndRdx.ColorFadeStart = System.Drawing.Color.Black;
            graphEndRdx.ColorHighlight = System.Drawing.Color.Gray;
            graphEndRdx.ColorLines = System.Drawing.Color.Black;
            graphEndRdx.ColorMarkerInner = System.Drawing.Color.Black;
            graphEndRdx.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphEndRdx.ColorOvercap = System.Drawing.Color.Black;
            graphEndRdx.DifferentiateColors = false;
            graphEndRdx.DrawRuler = false;
            graphEndRdx.Dual = true;
            graphEndRdx.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.EnhBarColors");
            graphEndRdx.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphEndRdx.ForcedMax = 0F;
            graphEndRdx.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphEndRdx.Highlight = true;
            graphEndRdx.ItemFontSizeOverride = 0F;
            graphEndRdx.ItemHeight = 10;
            graphEndRdx.Lines = true;
            graphEndRdx.Location = new System.Drawing.Point(0, 76);
            graphEndRdx.MarkerValue = 0F;
            graphEndRdx.Max = 100F;
            graphEndRdx.MaxItems = 60;
            graphEndRdx.Name = "graphEndRdx";
            graphEndRdx.OuterBorder = false;
            graphEndRdx.Overcap = false;
            graphEndRdx.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphEndRdx.OvercapColors");
            graphEndRdx.PaddingX = 2F;
            graphEndRdx.PaddingY = 2F;
            graphEndRdx.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphEndRdx.PerItemScales");
            graphEndRdx.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphEndRdx.ScaleHeight = 32;
            graphEndRdx.ScaleIndex = 8;
            graphEndRdx.ShowScale = false;
            graphEndRdx.Size = new System.Drawing.Size(300, 15);
            graphEndRdx.Style = Enums.GraphStyle.baseOnly;
            graphEndRdx.TabIndex = 12;
            graphEndRdx.TextWidth = 125;
            // 
            // graphDam
            // 
            graphDam.BackColor = System.Drawing.Color.Black;
            graphDam.BackgroundImage = (System.Drawing.Image)resources.GetObject("graphDam.BackgroundImage");
            graphDam.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDam.BaseBarColors");
            graphDam.Border = true;
            graphDam.BorderColor = System.Drawing.Color.Black;
            graphDam.Clickable = false;
            graphDam.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            graphDam.ColorBase = System.Drawing.Color.Red;
            graphDam.ColorEnh = System.Drawing.Color.Brown;
            graphDam.ColorFadeEnd = System.Drawing.Color.FromArgb(128, 64, 64);
            graphDam.ColorFadeStart = System.Drawing.Color.Black;
            graphDam.ColorHighlight = System.Drawing.Color.Gray;
            graphDam.ColorLines = System.Drawing.Color.Black;
            graphDam.ColorMarkerInner = System.Drawing.Color.Black;
            graphDam.ColorMarkerOuter = System.Drawing.Color.Yellow;
            graphDam.ColorOvercap = System.Drawing.Color.Black;
            graphDam.DifferentiateColors = false;
            graphDam.DrawRuler = false;
            graphDam.Dual = true;
            graphDam.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDam.EnhBarColors");
            graphDam.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            graphDam.ForcedMax = 0F;
            graphDam.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            graphDam.Highlight = true;
            graphDam.ItemFontSizeOverride = 0F;
            graphDam.ItemHeight = 10;
            graphDam.Lines = true;
            graphDam.Location = new System.Drawing.Point(0, 57);
            graphDam.MarkerValue = 0F;
            graphDam.Max = 100F;
            graphDam.MaxItems = 60;
            graphDam.Name = "graphDam";
            graphDam.OuterBorder = false;
            graphDam.Overcap = false;
            graphDam.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphDam.OvercapColors");
            graphDam.PaddingX = 2F;
            graphDam.PaddingY = 2F;
            graphDam.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphDam.PerItemScales");
            graphDam.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphDam.ScaleHeight = 32;
            graphDam.ScaleIndex = 8;
            graphDam.ShowScale = false;
            graphDam.Size = new System.Drawing.Size(300, 15);
            graphDam.Style = Enums.GraphStyle.Stacked;
            graphDam.TabIndex = 11;
            graphDam.TextWidth = 125;
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
            graphSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphSRes.OuterBorder = false;
            graphSRes.Overcap = false;
            graphSRes.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSRes.OvercapColors");
            graphSRes.PaddingX = 2F;
            graphSRes.PaddingY = 3F;
            graphSRes.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSRes.PerItemScales");
            graphSRes.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSRes.ScaleHeight = 32;
            graphSRes.ScaleIndex = 8;
            graphSRes.ShowScale = false;
            graphSRes.Size = new System.Drawing.Size(300, 136);
            graphSRes.Style = Enums.GraphStyle.baseOnly;
            graphSRes.TabIndex = 14;
            graphSRes.TextWidth = 125;
            // 
            // lblSRes
            // 
            lblSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphSDeb.OuterBorder = false;
            graphSDeb.Overcap = false;
            graphSDeb.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSDeb.OvercapColors");
            graphSDeb.PaddingX = 2F;
            graphSDeb.PaddingY = 3F;
            graphSDeb.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSDeb.PerItemScales");
            graphSDeb.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSDeb.ScaleHeight = 32;
            graphSDeb.ScaleIndex = 8;
            graphSDeb.ShowScale = false;
            graphSDeb.Size = new System.Drawing.Size(300, 100);
            graphSDeb.Style = Enums.GraphStyle.baseOnly;
            graphSDeb.TabIndex = 12;
            graphSDeb.TextWidth = 125;
            // 
            // lblSDeb
            // 
            lblSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            graphSProt.OuterBorder = false;
            graphSProt.Overcap = false;
            graphSProt.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("graphSProt.OvercapColors");
            graphSProt.PaddingX = 2F;
            graphSProt.PaddingY = 3F;
            graphSProt.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("graphSProt.PerItemScales");
            graphSProt.RulerPos = CtlMultiGraph.RulerPosition.Top;
            graphSProt.ScaleHeight = 32;
            graphSProt.ScaleIndex = 8;
            graphSProt.ShowScale = false;
            graphSProt.Size = new System.Drawing.Size(300, 136);
            graphSProt.Style = Enums.GraphStyle.baseOnly;
            graphSProt.TabIndex = 2;
            graphSProt.TextWidth = 125;
            // 
            // lblSProt
            // 
            lblSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            Panel2.ResumeLayout(false);
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

        private CtlMultiGraph graphAcc;
        private CtlMultiGraph graphDam;
        private CtlMultiGraph graphDef;
        private CtlMultiGraph graphDrain;
        private CtlMultiGraph graphElusivity;
        private CtlMultiGraph graphEndRdx;
        private CtlMultiGraph graphHaste;
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
        private CtlMultiGraph graphThreat;
        private CtlMultiGraph graphToHit;

        private ToolTip toolTip1;
    }
}