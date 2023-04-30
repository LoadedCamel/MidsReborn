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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTotals));
            this.lblDef = new System.Windows.Forms.Label();
            this.lblRes = new System.Windows.Forms.Label();
            this.lblRegenRec = new System.Windows.Forms.Label();
            this.pnlDRHE = new System.Windows.Forms.Panel();
            this.graphMaxEnd = new CtlMultiGraph();
            this.graphHP = new CtlMultiGraph();
            this.graphDef = new CtlMultiGraph();
            this.graphDrain = new CtlMultiGraph();
            this.graphRes = new CtlMultiGraph();
            this.graphRec = new CtlMultiGraph();
            this.graphRegen = new CtlMultiGraph();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.pnlMisc = new System.Windows.Forms.Panel();
            this.rbMSec = new System.Windows.Forms.RadioButton();
            this.rbFPS = new System.Windows.Forms.RadioButton();
            this.rbKPH = new System.Windows.Forms.RadioButton();
            this.rbMPH = new System.Windows.Forms.RadioButton();
            this.lblStealth = new System.Windows.Forms.Label();
            this.graphStealth = new CtlMultiGraph();
            this.graphAcc = new CtlMultiGraph();
            this.graphToHit = new CtlMultiGraph();
            this.lblMisc = new System.Windows.Forms.Label();
            this.graphMovement = new CtlMultiGraph();
            this.lblMovement = new System.Windows.Forms.Label();
            this.graphHaste = new CtlMultiGraph();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.graphElusivity = new CtlMultiGraph();
            this.graphThreat = new CtlMultiGraph();
            this.graphEndRdx = new CtlMultiGraph();
            this.graphDam = new CtlMultiGraph();
            this.tab1 = new System.Windows.Forms.PictureBox();
            this.tab0 = new System.Windows.Forms.PictureBox();
            this.pbTopMost = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.graphSRes = new CtlMultiGraph();
            this.lblSRes = new System.Windows.Forms.Label();
            this.graphSDeb = new CtlMultiGraph();
            this.lblSDeb = new System.Windows.Forms.Label();
            this.graphSProt = new CtlMultiGraph();
            this.lblSProt = new System.Windows.Forms.Label();
            this.tab2 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlDRHE.SuspendLayout();
            this.pnlMisc.SuspendLayout();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tab1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tab0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTopMost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.pnlStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tab2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDef
            // 
            this.lblDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDef.Location = new System.Drawing.Point(3, 0);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(89, 16);
            this.lblDef.TabIndex = 1;
            this.lblDef.Text = "Defense:";
            this.lblDef.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRes
            // 
            this.lblRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblRes.Location = new System.Drawing.Point(3, 174);
            this.lblRes.Name = "lblRes";
            this.lblRes.Size = new System.Drawing.Size(125, 16);
            this.lblRes.TabIndex = 3;
            this.lblRes.Text = "Resistance:";
            this.lblRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblRegenRec
            // 
            this.lblRegenRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblRegenRec.Location = new System.Drawing.Point(3, 312);
            this.lblRegenRec.Name = "lblRegenRec";
            this.lblRegenRec.Size = new System.Drawing.Size(125, 16);
            this.lblRegenRec.TabIndex = 5;
            this.lblRegenRec.Text = "Health & Endurance:";
            this.lblRegenRec.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblRegenRec.UseMnemonic = false;
            // 
            // pnlDRHE
            // 
            this.pnlDRHE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.pnlDRHE.Controls.Add(this.graphMaxEnd);
            this.pnlDRHE.Controls.Add(this.graphHP);
            this.pnlDRHE.Controls.Add(this.graphDef);
            this.pnlDRHE.Controls.Add(this.graphDrain);
            this.pnlDRHE.Controls.Add(this.lblDef);
            this.pnlDRHE.Controls.Add(this.graphRes);
            this.pnlDRHE.Controls.Add(this.graphRec);
            this.pnlDRHE.Controls.Add(this.lblRes);
            this.pnlDRHE.Controls.Add(this.lblRegenRec);
            this.pnlDRHE.Controls.Add(this.graphRegen);
            this.pnlDRHE.Controls.Add(this.Panel1);
            this.pnlDRHE.Location = new System.Drawing.Point(4, 31);
            this.pnlDRHE.Name = "pnlDRHE";
            this.pnlDRHE.Size = new System.Drawing.Size(320, 433);
            this.pnlDRHE.TabIndex = 9;
            // 
            // graphMaxEnd
            // 
            this.graphMaxEnd.BackColor = System.Drawing.Color.Black;
            this.graphMaxEnd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphMaxEnd.BackgroundImage")));
            this.graphMaxEnd.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMaxEnd.BaseBarColors")));
            this.graphMaxEnd.Border = true;
            this.graphMaxEnd.BorderColor = System.Drawing.Color.Black;
            this.graphMaxEnd.Clickable = false;
            this.graphMaxEnd.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphMaxEnd.ColorBase = System.Drawing.Color.CornflowerBlue;
            this.graphMaxEnd.ColorEnh = System.Drawing.Color.Yellow;
            this.graphMaxEnd.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(128)))));
            this.graphMaxEnd.ColorFadeStart = System.Drawing.Color.Black;
            this.graphMaxEnd.ColorHighlight = System.Drawing.Color.Gray;
            this.graphMaxEnd.ColorLines = System.Drawing.Color.Black;
            this.graphMaxEnd.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphMaxEnd.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphMaxEnd.ColorOvercap = System.Drawing.Color.Black;
            this.graphMaxEnd.DifferentiateColors = false;
            this.graphMaxEnd.Dual = true;
            this.graphMaxEnd.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMaxEnd.EnhBarColors")));
            this.graphMaxEnd.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphMaxEnd.ForcedMax = 0F;
            this.graphMaxEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphMaxEnd.Highlight = true;
            this.graphMaxEnd.ItemHeight = 10;
            this.graphMaxEnd.Lines = true;
            this.graphMaxEnd.Location = new System.Drawing.Point(15, 404);
            this.graphMaxEnd.MarkerValue = 0F;
            this.graphMaxEnd.Max = 100F;
            this.graphMaxEnd.MaxItems = 60;
            this.graphMaxEnd.Name = "graphMaxEnd";
            this.graphMaxEnd.OuterBorder = false;
            this.graphMaxEnd.Overcap = false;
            this.graphMaxEnd.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMaxEnd.OvercapColors")));
            this.graphMaxEnd.PaddingX = 2F;
            this.graphMaxEnd.PaddingY = 2F;
            this.graphMaxEnd.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphMaxEnd.PerItemScales")));
            this.graphMaxEnd.ScaleHeight = 32;
            this.graphMaxEnd.ScaleIndex = 8;
            this.graphMaxEnd.ShowScale = false;
            this.graphMaxEnd.Size = new System.Drawing.Size(300, 15);
            this.graphMaxEnd.Style = Enums.GraphStyle.baseOnly;
            this.graphMaxEnd.TabIndex = 7;
            this.graphMaxEnd.TextWidth = 125;
            // 
            // graphHP
            // 
            this.graphHP.BackColor = System.Drawing.Color.Black;
            this.graphHP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphHP.BackgroundImage")));
            this.graphHP.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHP.BaseBarColors")));
            this.graphHP.Border = true;
            this.graphHP.BorderColor = System.Drawing.Color.Black;
            this.graphHP.Clickable = false;
            this.graphHP.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphHP.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(192)))), ((int)(((byte)(96)))));
            this.graphHP.ColorEnh = System.Drawing.Color.Yellow;
            this.graphHP.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(64)))));
            this.graphHP.ColorFadeStart = System.Drawing.Color.Black;
            this.graphHP.ColorHighlight = System.Drawing.Color.Gray;
            this.graphHP.ColorLines = System.Drawing.Color.Black;
            this.graphHP.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphHP.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphHP.ColorOvercap = System.Drawing.Color.Black;
            this.graphHP.DifferentiateColors = false;
            this.graphHP.Dual = true;
            this.graphHP.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHP.EnhBarColors")));
            this.graphHP.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphHP.ForcedMax = 0F;
            this.graphHP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphHP.Highlight = true;
            this.graphHP.ItemHeight = 10;
            this.graphHP.Lines = true;
            this.graphHP.Location = new System.Drawing.Point(15, 349);
            this.graphHP.MarkerValue = 0F;
            this.graphHP.Max = 100F;
            this.graphHP.MaxItems = 60;
            this.graphHP.Name = "graphHP";
            this.graphHP.OuterBorder = false;
            this.graphHP.Overcap = false;
            this.graphHP.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHP.OvercapColors")));
            this.graphHP.PaddingX = 2F;
            this.graphHP.PaddingY = 2F;
            this.graphHP.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphHP.PerItemScales")));
            this.graphHP.ScaleHeight = 32;
            this.graphHP.ScaleIndex = 8;
            this.graphHP.ShowScale = false;
            this.graphHP.Size = new System.Drawing.Size(300, 15);
            this.graphHP.Style = Enums.GraphStyle.baseOnly;
            this.graphHP.TabIndex = 9;
            this.graphHP.TextWidth = 125;
            // 
            // graphDef
            // 
            this.graphDef.BackColor = System.Drawing.Color.Black;
            this.graphDef.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphDef.BackgroundImage")));
            this.graphDef.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDef.BaseBarColors")));
            this.graphDef.Border = true;
            this.graphDef.BorderColor = System.Drawing.Color.Black;
            this.graphDef.Clickable = false;
            this.graphDef.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphDef.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.graphDef.ColorEnh = System.Drawing.Color.Yellow;
            this.graphDef.ColorFadeEnd = System.Drawing.Color.Purple;
            this.graphDef.ColorFadeStart = System.Drawing.Color.Black;
            this.graphDef.ColorHighlight = System.Drawing.Color.Gray;
            this.graphDef.ColorLines = System.Drawing.Color.Black;
            this.graphDef.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphDef.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphDef.ColorOvercap = System.Drawing.Color.Black;
            this.graphDef.DifferentiateColors = false;
            this.graphDef.Dual = true;
            this.graphDef.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDef.EnhBarColors")));
            this.graphDef.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphDef.ForcedMax = 0F;
            this.graphDef.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphDef.Highlight = true;
            this.graphDef.ItemHeight = 10;
            this.graphDef.Lines = true;
            this.graphDef.Location = new System.Drawing.Point(15, 17);
            this.graphDef.MarkerValue = 0F;
            this.graphDef.Max = 100F;
            this.graphDef.MaxItems = 60;
            this.graphDef.Name = "graphDef";
            this.graphDef.OuterBorder = false;
            this.graphDef.Overcap = false;
            this.graphDef.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDef.OvercapColors")));
            this.graphDef.PaddingX = 2F;
            this.graphDef.PaddingY = 4F;
            this.graphDef.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphDef.PerItemScales")));
            this.graphDef.ScaleHeight = 32;
            this.graphDef.ScaleIndex = 8;
            this.graphDef.ShowScale = false;
            this.graphDef.Size = new System.Drawing.Size(300, 152);
            this.graphDef.Style = Enums.GraphStyle.baseOnly;
            this.graphDef.TabIndex = 0;
            this.graphDef.TextWidth = 125;
            // 
            // graphDrain
            // 
            this.graphDrain.BackColor = System.Drawing.Color.Black;
            this.graphDrain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphDrain.BackgroundImage")));
            this.graphDrain.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDrain.BaseBarColors")));
            this.graphDrain.Border = true;
            this.graphDrain.BorderColor = System.Drawing.Color.Black;
            this.graphDrain.Clickable = false;
            this.graphDrain.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphDrain.ColorBase = System.Drawing.Color.LightSteelBlue;
            this.graphDrain.ColorEnh = System.Drawing.Color.Yellow;
            this.graphDrain.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(192)))));
            this.graphDrain.ColorFadeStart = System.Drawing.Color.Black;
            this.graphDrain.ColorHighlight = System.Drawing.Color.Gray;
            this.graphDrain.ColorLines = System.Drawing.Color.Black;
            this.graphDrain.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphDrain.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphDrain.ColorOvercap = System.Drawing.Color.Black;
            this.graphDrain.DifferentiateColors = false;
            this.graphDrain.Dual = true;
            this.graphDrain.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDrain.EnhBarColors")));
            this.graphDrain.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphDrain.ForcedMax = 0F;
            this.graphDrain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphDrain.Highlight = true;
            this.graphDrain.ItemHeight = 10;
            this.graphDrain.Lines = true;
            this.graphDrain.Location = new System.Drawing.Point(15, 385);
            this.graphDrain.MarkerValue = 0F;
            this.graphDrain.Max = 100F;
            this.graphDrain.MaxItems = 60;
            this.graphDrain.Name = "graphDrain";
            this.graphDrain.OuterBorder = false;
            this.graphDrain.Overcap = false;
            this.graphDrain.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDrain.OvercapColors")));
            this.graphDrain.PaddingX = 2F;
            this.graphDrain.PaddingY = 2F;
            this.graphDrain.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphDrain.PerItemScales")));
            this.graphDrain.ScaleHeight = 32;
            this.graphDrain.ScaleIndex = 8;
            this.graphDrain.ShowScale = false;
            this.graphDrain.Size = new System.Drawing.Size(300, 15);
            this.graphDrain.Style = Enums.GraphStyle.baseOnly;
            this.graphDrain.TabIndex = 8;
            this.graphDrain.TextWidth = 125;
            // 
            // graphRes
            // 
            this.graphRes.BackColor = System.Drawing.Color.Black;
            this.graphRes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphRes.BackgroundImage")));
            this.graphRes.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRes.BaseBarColors")));
            this.graphRes.Border = true;
            this.graphRes.BorderColor = System.Drawing.Color.Black;
            this.graphRes.Clickable = false;
            this.graphRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphRes.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.graphRes.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.graphRes.ColorFadeEnd = System.Drawing.Color.Teal;
            this.graphRes.ColorFadeStart = System.Drawing.Color.Black;
            this.graphRes.ColorHighlight = System.Drawing.Color.Gray;
            this.graphRes.ColorLines = System.Drawing.Color.Black;
            this.graphRes.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphRes.ColorOvercap = System.Drawing.Color.Black;
            this.graphRes.DifferentiateColors = false;
            this.graphRes.Dual = true;
            this.graphRes.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRes.EnhBarColors")));
            this.graphRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphRes.ForcedMax = 0F;
            this.graphRes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphRes.Highlight = true;
            this.graphRes.ItemHeight = 10;
            this.graphRes.Lines = true;
            this.graphRes.Location = new System.Drawing.Point(15, 193);
            this.graphRes.MarkerValue = 0F;
            this.graphRes.Max = 100F;
            this.graphRes.MaxItems = 60;
            this.graphRes.Name = "graphRes";
            this.graphRes.OuterBorder = false;
            this.graphRes.Overcap = false;
            this.graphRes.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRes.OvercapColors")));
            this.graphRes.PaddingX = 2F;
            this.graphRes.PaddingY = 4F;
            this.graphRes.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphRes.PerItemScales")));
            this.graphRes.ScaleHeight = 32;
            this.graphRes.ScaleIndex = 8;
            this.graphRes.ShowScale = false;
            this.graphRes.Size = new System.Drawing.Size(300, 116);
            this.graphRes.Style = Enums.GraphStyle.Stacked;
            this.graphRes.TabIndex = 2;
            this.graphRes.TextWidth = 125;
            // 
            // graphRec
            // 
            this.graphRec.BackColor = System.Drawing.Color.Black;
            this.graphRec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphRec.BackgroundImage")));
            this.graphRec.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRec.BaseBarColors")));
            this.graphRec.Border = true;
            this.graphRec.BorderColor = System.Drawing.Color.Black;
            this.graphRec.Clickable = false;
            this.graphRec.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphRec.ColorBase = System.Drawing.Color.RoyalBlue;
            this.graphRec.ColorEnh = System.Drawing.Color.Yellow;
            this.graphRec.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.graphRec.ColorFadeStart = System.Drawing.Color.Black;
            this.graphRec.ColorHighlight = System.Drawing.Color.Gray;
            this.graphRec.ColorLines = System.Drawing.Color.Black;
            this.graphRec.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphRec.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphRec.ColorOvercap = System.Drawing.Color.Black;
            this.graphRec.DifferentiateColors = false;
            this.graphRec.Dual = true;
            this.graphRec.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRec.EnhBarColors")));
            this.graphRec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphRec.ForcedMax = 0F;
            this.graphRec.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphRec.Highlight = true;
            this.graphRec.ItemHeight = 10;
            this.graphRec.Lines = true;
            this.graphRec.Location = new System.Drawing.Point(15, 367);
            this.graphRec.MarkerValue = 0F;
            this.graphRec.Max = 100F;
            this.graphRec.MaxItems = 60;
            this.graphRec.Name = "graphRec";
            this.graphRec.OuterBorder = false;
            this.graphRec.Overcap = false;
            this.graphRec.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRec.OvercapColors")));
            this.graphRec.PaddingX = 2F;
            this.graphRec.PaddingY = 2F;
            this.graphRec.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphRec.PerItemScales")));
            this.graphRec.ScaleHeight = 32;
            this.graphRec.ScaleIndex = 8;
            this.graphRec.ShowScale = false;
            this.graphRec.Size = new System.Drawing.Size(300, 15);
            this.graphRec.Style = Enums.GraphStyle.baseOnly;
            this.graphRec.TabIndex = 6;
            this.graphRec.TextWidth = 125;
            // 
            // graphRegen
            // 
            this.graphRegen.BackColor = System.Drawing.Color.Black;
            this.graphRegen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphRegen.BackgroundImage")));
            this.graphRegen.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRegen.BaseBarColors")));
            this.graphRegen.Border = true;
            this.graphRegen.BorderColor = System.Drawing.Color.Black;
            this.graphRegen.Clickable = false;
            this.graphRegen.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphRegen.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(64)))));
            this.graphRegen.ColorEnh = System.Drawing.Color.Yellow;
            this.graphRegen.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.graphRegen.ColorFadeStart = System.Drawing.Color.Black;
            this.graphRegen.ColorHighlight = System.Drawing.Color.Gray;
            this.graphRegen.ColorLines = System.Drawing.Color.Black;
            this.graphRegen.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphRegen.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphRegen.ColorOvercap = System.Drawing.Color.Black;
            this.graphRegen.DifferentiateColors = false;
            this.graphRegen.Dual = true;
            this.graphRegen.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRegen.EnhBarColors")));
            this.graphRegen.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphRegen.ForcedMax = 0F;
            this.graphRegen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphRegen.Highlight = true;
            this.graphRegen.ItemHeight = 10;
            this.graphRegen.Lines = true;
            this.graphRegen.Location = new System.Drawing.Point(15, 331);
            this.graphRegen.MarkerValue = 0F;
            this.graphRegen.Max = 100F;
            this.graphRegen.MaxItems = 60;
            this.graphRegen.Name = "graphRegen";
            this.graphRegen.OuterBorder = false;
            this.graphRegen.Overcap = false;
            this.graphRegen.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphRegen.OvercapColors")));
            this.graphRegen.PaddingX = 2F;
            this.graphRegen.PaddingY = 2F;
            this.graphRegen.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphRegen.PerItemScales")));
            this.graphRegen.ScaleHeight = 32;
            this.graphRegen.ScaleIndex = 8;
            this.graphRegen.ShowScale = false;
            this.graphRegen.Size = new System.Drawing.Size(300, 15);
            this.graphRegen.Style = Enums.GraphStyle.baseOnly;
            this.graphRegen.TabIndex = 4;
            this.graphRegen.TextWidth = 125;
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.Black;
            this.Panel1.Location = new System.Drawing.Point(17, 321);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(298, 88);
            this.Panel1.TabIndex = 10;
            // 
            // pnlMisc
            // 
            this.pnlMisc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.pnlMisc.Controls.Add(this.rbMSec);
            this.pnlMisc.Controls.Add(this.rbFPS);
            this.pnlMisc.Controls.Add(this.rbKPH);
            this.pnlMisc.Controls.Add(this.rbMPH);
            this.pnlMisc.Controls.Add(this.lblStealth);
            this.pnlMisc.Controls.Add(this.graphStealth);
            this.pnlMisc.Controls.Add(this.graphAcc);
            this.pnlMisc.Controls.Add(this.graphToHit);
            this.pnlMisc.Controls.Add(this.lblMisc);
            this.pnlMisc.Controls.Add(this.graphMovement);
            this.pnlMisc.Controls.Add(this.lblMovement);
            this.pnlMisc.Controls.Add(this.graphHaste);
            this.pnlMisc.Controls.Add(this.Panel2);
            this.pnlMisc.Location = new System.Drawing.Point(330, 31);
            this.pnlMisc.Name = "pnlMisc";
            this.pnlMisc.Size = new System.Drawing.Size(320, 433);
            this.pnlMisc.TabIndex = 10;
            this.pnlMisc.Visible = false;
            // 
            // rbMSec
            // 
            this.rbMSec.Location = new System.Drawing.Point(225, 81);
            this.rbMSec.Name = "rbMSec";
            this.rbMSec.Size = new System.Drawing.Size(84, 24);
            this.rbMSec.TabIndex = 18;
            this.rbMSec.Text = "Meters/Sec";
            this.rbMSec.UseVisualStyleBackColor = true;
            this.rbMSec.CheckedChanged += new System.EventHandler(this.RbSpeedCheckedChanged);
            // 
            // rbFPS
            // 
            this.rbFPS.Location = new System.Drawing.Point(147, 81);
            this.rbFPS.Name = "rbFPS";
            this.rbFPS.Size = new System.Drawing.Size(74, 24);
            this.rbFPS.TabIndex = 17;
            this.rbFPS.Text = "Feet/Sec";
            this.rbFPS.UseVisualStyleBackColor = true;
            this.rbFPS.CheckedChanged += new System.EventHandler(this.RbSpeedCheckedChanged);
            // 
            // rbKPH
            // 
            this.rbKPH.Location = new System.Drawing.Point(82, 81);
            this.rbKPH.Name = "rbKPH";
            this.rbKPH.Size = new System.Drawing.Size(59, 24);
            this.rbKPH.TabIndex = 16;
            this.rbKPH.Text = "KPH";
            this.toolTip1.SetToolTip(this.rbKPH, "Kilometers per hour");
            this.rbKPH.UseVisualStyleBackColor = true;
            this.rbKPH.CheckedChanged += new System.EventHandler(this.RbSpeedCheckedChanged);
            // 
            // rbMPH
            // 
            this.rbMPH.Checked = true;
            this.rbMPH.Location = new System.Drawing.Point(21, 81);
            this.rbMPH.Name = "rbMPH";
            this.rbMPH.Size = new System.Drawing.Size(59, 24);
            this.rbMPH.TabIndex = 15;
            this.rbMPH.TabStop = true;
            this.rbMPH.Text = "MPH";
            this.toolTip1.SetToolTip(this.rbMPH, "Miles per hour");
            this.rbMPH.UseVisualStyleBackColor = true;
            this.rbMPH.CheckedChanged += new System.EventHandler(this.RbSpeedCheckedChanged);
            // 
            // lblStealth
            // 
            this.lblStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStealth.Location = new System.Drawing.Point(3, 109);
            this.lblStealth.Name = "lblStealth";
            this.lblStealth.Size = new System.Drawing.Size(125, 16);
            this.lblStealth.TabIndex = 13;
            this.lblStealth.Text = "Stealth:";
            this.lblStealth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphStealth
            // 
            this.graphStealth.BackColor = System.Drawing.Color.Black;
            this.graphStealth.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphStealth.BackgroundImage")));
            this.graphStealth.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphStealth.BaseBarColors")));
            this.graphStealth.Border = true;
            this.graphStealth.BorderColor = System.Drawing.Color.Black;
            this.graphStealth.Clickable = false;
            this.graphStealth.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphStealth.ColorBase = System.Drawing.Color.LightSlateGray;
            this.graphStealth.ColorEnh = System.Drawing.Color.Yellow;
            this.graphStealth.ColorFadeEnd = System.Drawing.Color.DarkSlateBlue;
            this.graphStealth.ColorFadeStart = System.Drawing.Color.Black;
            this.graphStealth.ColorHighlight = System.Drawing.Color.Gray;
            this.graphStealth.ColorLines = System.Drawing.Color.Black;
            this.graphStealth.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphStealth.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphStealth.ColorOvercap = System.Drawing.Color.Black;
            this.graphStealth.DifferentiateColors = false;
            this.graphStealth.Dual = false;
            this.graphStealth.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphStealth.EnhBarColors")));
            this.graphStealth.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphStealth.ForcedMax = 0F;
            this.graphStealth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphStealth.Highlight = true;
            this.graphStealth.ItemHeight = 10;
            this.graphStealth.Lines = true;
            this.graphStealth.Location = new System.Drawing.Point(15, 128);
            this.graphStealth.MarkerValue = 0F;
            this.graphStealth.Max = 100F;
            this.graphStealth.MaxItems = 60;
            this.graphStealth.Name = "graphStealth";
            this.graphStealth.OuterBorder = false;
            this.graphStealth.Overcap = false;
            this.graphStealth.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphStealth.OvercapColors")));
            this.graphStealth.PaddingX = 2F;
            this.graphStealth.PaddingY = 4F;
            this.graphStealth.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphStealth.PerItemScales")));
            this.graphStealth.ScaleHeight = 32;
            this.graphStealth.ScaleIndex = 8;
            this.graphStealth.ShowScale = false;
            this.graphStealth.Size = new System.Drawing.Size(300, 46);
            this.graphStealth.Style = Enums.GraphStyle.baseOnly;
            this.graphStealth.TabIndex = 12;
            this.graphStealth.TextWidth = 125;
            // 
            // graphAcc
            // 
            this.graphAcc.BackColor = System.Drawing.Color.Black;
            this.graphAcc.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphAcc.BackgroundImage")));
            this.graphAcc.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphAcc.BaseBarColors")));
            this.graphAcc.Border = true;
            this.graphAcc.BorderColor = System.Drawing.Color.Black;
            this.graphAcc.Clickable = false;
            this.graphAcc.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphAcc.ColorBase = System.Drawing.Color.Yellow;
            this.graphAcc.ColorEnh = System.Drawing.Color.Yellow;
            this.graphAcc.ColorFadeEnd = System.Drawing.Color.Olive;
            this.graphAcc.ColorFadeStart = System.Drawing.Color.Black;
            this.graphAcc.ColorHighlight = System.Drawing.Color.Gray;
            this.graphAcc.ColorLines = System.Drawing.Color.Black;
            this.graphAcc.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphAcc.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphAcc.ColorOvercap = System.Drawing.Color.Black;
            this.graphAcc.DifferentiateColors = false;
            this.graphAcc.Dual = true;
            this.graphAcc.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphAcc.EnhBarColors")));
            this.graphAcc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphAcc.ForcedMax = 0F;
            this.graphAcc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphAcc.Highlight = true;
            this.graphAcc.ItemHeight = 10;
            this.graphAcc.Lines = true;
            this.graphAcc.Location = new System.Drawing.Point(15, 234);
            this.graphAcc.MarkerValue = 0F;
            this.graphAcc.Max = 100F;
            this.graphAcc.MaxItems = 60;
            this.graphAcc.Name = "graphAcc";
            this.graphAcc.OuterBorder = false;
            this.graphAcc.Overcap = false;
            this.graphAcc.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphAcc.OvercapColors")));
            this.graphAcc.PaddingX = 2F;
            this.graphAcc.PaddingY = 2F;
            this.graphAcc.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphAcc.PerItemScales")));
            this.graphAcc.ScaleHeight = 32;
            this.graphAcc.ScaleIndex = 8;
            this.graphAcc.ShowScale = false;
            this.graphAcc.Size = new System.Drawing.Size(300, 15);
            this.graphAcc.Style = Enums.GraphStyle.baseOnly;
            this.graphAcc.TabIndex = 10;
            this.graphAcc.TextWidth = 125;
            // 
            // graphToHit
            // 
            this.graphToHit.BackColor = System.Drawing.Color.Black;
            this.graphToHit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphToHit.BackgroundImage")));
            this.graphToHit.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphToHit.BaseBarColors")));
            this.graphToHit.Border = true;
            this.graphToHit.BorderColor = System.Drawing.Color.Black;
            this.graphToHit.Clickable = false;
            this.graphToHit.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphToHit.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.graphToHit.ColorEnh = System.Drawing.Color.Yellow;
            this.graphToHit.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.graphToHit.ColorFadeStart = System.Drawing.Color.Black;
            this.graphToHit.ColorHighlight = System.Drawing.Color.Gray;
            this.graphToHit.ColorLines = System.Drawing.Color.Black;
            this.graphToHit.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphToHit.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphToHit.ColorOvercap = System.Drawing.Color.Black;
            this.graphToHit.DifferentiateColors = false;
            this.graphToHit.Dual = true;
            this.graphToHit.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphToHit.EnhBarColors")));
            this.graphToHit.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphToHit.ForcedMax = 0F;
            this.graphToHit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphToHit.Highlight = true;
            this.graphToHit.ItemHeight = 10;
            this.graphToHit.Lines = true;
            this.graphToHit.Location = new System.Drawing.Point(15, 215);
            this.graphToHit.MarkerValue = 0F;
            this.graphToHit.Max = 100F;
            this.graphToHit.MaxItems = 60;
            this.graphToHit.Name = "graphToHit";
            this.graphToHit.OuterBorder = false;
            this.graphToHit.Overcap = false;
            this.graphToHit.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphToHit.OvercapColors")));
            this.graphToHit.PaddingX = 2F;
            this.graphToHit.PaddingY = 2F;
            this.graphToHit.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphToHit.PerItemScales")));
            this.graphToHit.ScaleHeight = 32;
            this.graphToHit.ScaleIndex = 8;
            this.graphToHit.ShowScale = false;
            this.graphToHit.Size = new System.Drawing.Size(300, 15);
            this.graphToHit.Style = Enums.GraphStyle.baseOnly;
            this.graphToHit.TabIndex = 9;
            this.graphToHit.TextWidth = 125;
            // 
            // lblMisc
            // 
            this.lblMisc.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMisc.Location = new System.Drawing.Point(3, 177);
            this.lblMisc.Name = "lblMisc";
            this.lblMisc.Size = new System.Drawing.Size(125, 16);
            this.lblMisc.TabIndex = 8;
            this.lblMisc.Text = "Misc:";
            this.lblMisc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphMovement
            // 
            this.graphMovement.BackColor = System.Drawing.Color.Black;
            this.graphMovement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphMovement.BackgroundImage")));
            this.graphMovement.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMovement.BaseBarColors")));
            this.graphMovement.Border = true;
            this.graphMovement.BorderColor = System.Drawing.Color.Black;
            this.graphMovement.Clickable = false;
            this.graphMovement.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphMovement.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.graphMovement.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.graphMovement.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(96)))));
            this.graphMovement.ColorFadeStart = System.Drawing.Color.Black;
            this.graphMovement.ColorHighlight = System.Drawing.Color.Gray;
            this.graphMovement.ColorLines = System.Drawing.Color.Black;
            this.graphMovement.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphMovement.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphMovement.ColorOvercap = System.Drawing.Color.Black;
            this.graphMovement.DifferentiateColors = false;
            this.graphMovement.Dual = true;
            this.graphMovement.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMovement.EnhBarColors")));
            this.graphMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphMovement.ForcedMax = 0F;
            this.graphMovement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphMovement.Highlight = true;
            this.graphMovement.ItemHeight = 10;
            this.graphMovement.Lines = true;
            this.graphMovement.Location = new System.Drawing.Point(15, 17);
            this.graphMovement.MarkerValue = 0F;
            this.graphMovement.Max = 100F;
            this.graphMovement.MaxItems = 60;
            this.graphMovement.Name = "graphMovement";
            this.graphMovement.OuterBorder = false;
            this.graphMovement.Overcap = false;
            this.graphMovement.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphMovement.OvercapColors")));
            this.graphMovement.PaddingX = 2F;
            this.graphMovement.PaddingY = 4F;
            this.graphMovement.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphMovement.PerItemScales")));
            this.graphMovement.ScaleHeight = 32;
            this.graphMovement.ScaleIndex = 8;
            this.graphMovement.ShowScale = false;
            this.graphMovement.Size = new System.Drawing.Size(300, 61);
            this.graphMovement.Style = Enums.GraphStyle.Stacked;
            this.graphMovement.TabIndex = 2;
            this.graphMovement.TextWidth = 125;
            // 
            // lblMovement
            // 
            this.lblMovement.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMovement.Location = new System.Drawing.Point(3, 0);
            this.lblMovement.Name = "lblMovement";
            this.lblMovement.Size = new System.Drawing.Size(125, 16);
            this.lblMovement.TabIndex = 3;
            this.lblMovement.Text = "Movement:";
            this.lblMovement.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphHaste
            // 
            this.graphHaste.BackColor = System.Drawing.Color.Black;
            this.graphHaste.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphHaste.BackgroundImage")));
            this.graphHaste.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHaste.BaseBarColors")));
            this.graphHaste.Border = true;
            this.graphHaste.BorderColor = System.Drawing.Color.Black;
            this.graphHaste.Clickable = false;
            this.graphHaste.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphHaste.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.graphHaste.ColorEnh = System.Drawing.Color.Yellow;
            this.graphHaste.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.graphHaste.ColorFadeStart = System.Drawing.Color.Black;
            this.graphHaste.ColorHighlight = System.Drawing.Color.Gray;
            this.graphHaste.ColorLines = System.Drawing.Color.Black;
            this.graphHaste.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphHaste.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphHaste.ColorOvercap = System.Drawing.Color.Black;
            this.graphHaste.DifferentiateColors = false;
            this.graphHaste.Dual = true;
            this.graphHaste.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHaste.EnhBarColors")));
            this.graphHaste.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphHaste.ForcedMax = 0F;
            this.graphHaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphHaste.Highlight = true;
            this.graphHaste.ItemHeight = 10;
            this.graphHaste.Lines = true;
            this.graphHaste.Location = new System.Drawing.Point(15, 196);
            this.graphHaste.MarkerValue = 0F;
            this.graphHaste.Max = 100F;
            this.graphHaste.MaxItems = 60;
            this.graphHaste.Name = "graphHaste";
            this.graphHaste.OuterBorder = false;
            this.graphHaste.Overcap = false;
            this.graphHaste.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphHaste.OvercapColors")));
            this.graphHaste.PaddingX = 2F;
            this.graphHaste.PaddingY = 2F;
            this.graphHaste.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphHaste.PerItemScales")));
            this.graphHaste.ScaleHeight = 32;
            this.graphHaste.ScaleIndex = 8;
            this.graphHaste.ShowScale = false;
            this.graphHaste.Size = new System.Drawing.Size(300, 15);
            this.graphHaste.Style = Enums.GraphStyle.baseOnly;
            this.graphHaste.TabIndex = 7;
            this.graphHaste.TextWidth = 125;
            // 
            // Panel2
            // 
            this.Panel2.BackColor = System.Drawing.Color.Black;
            this.Panel2.Controls.Add(this.graphElusivity);
            this.Panel2.Controls.Add(this.graphThreat);
            this.Panel2.Controls.Add(this.graphEndRdx);
            this.Panel2.Controls.Add(this.graphDam);
            this.Panel2.Location = new System.Drawing.Point(15, 196);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(300, 194);
            this.Panel2.TabIndex = 14;
            // 
            // graphElusivity
            // 
            this.graphElusivity.BackColor = System.Drawing.Color.Black;
            this.graphElusivity.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphElusivity.BackgroundImage")));
            this.graphElusivity.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphElusivity.BaseBarColors")));
            this.graphElusivity.Border = true;
            this.graphElusivity.BorderColor = System.Drawing.Color.Black;
            this.graphElusivity.Clickable = false;
            this.graphElusivity.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphElusivity.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.graphElusivity.ColorEnh = System.Drawing.Color.Yellow;
            this.graphElusivity.ColorFadeEnd = System.Drawing.Color.Purple;
            this.graphElusivity.ColorFadeStart = System.Drawing.Color.Black;
            this.graphElusivity.ColorHighlight = System.Drawing.Color.Gray;
            this.graphElusivity.ColorLines = System.Drawing.Color.Black;
            this.graphElusivity.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphElusivity.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphElusivity.ColorOvercap = System.Drawing.Color.Black;
            this.graphElusivity.DifferentiateColors = false;
            this.graphElusivity.Dual = true;
            this.graphElusivity.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphElusivity.EnhBarColors")));
            this.graphElusivity.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphElusivity.ForcedMax = 0F;
            this.graphElusivity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphElusivity.Highlight = true;
            this.graphElusivity.ItemHeight = 10;
            this.graphElusivity.Lines = true;
            this.graphElusivity.Location = new System.Drawing.Point(0, 117);
            this.graphElusivity.MarkerValue = 0F;
            this.graphElusivity.Max = 100F;
            this.graphElusivity.MaxItems = 60;
            this.graphElusivity.Name = "graphElusivity";
            this.graphElusivity.OuterBorder = false;
            this.graphElusivity.Overcap = false;
            this.graphElusivity.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphElusivity.OvercapColors")));
            this.graphElusivity.PaddingX = 2F;
            this.graphElusivity.PaddingY = 2F;
            this.graphElusivity.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphElusivity.PerItemScales")));
            this.graphElusivity.ScaleHeight = 32;
            this.graphElusivity.ScaleIndex = 8;
            this.graphElusivity.ShowScale = false;
            this.graphElusivity.Size = new System.Drawing.Size(300, 15);
            this.graphElusivity.Style = Enums.GraphStyle.baseOnly;
            this.graphElusivity.TabIndex = 14;
            this.graphElusivity.TextWidth = 125;
            // 
            // graphThreat
            // 
            this.graphThreat.BackColor = System.Drawing.Color.Black;
            this.graphThreat.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphThreat.BackgroundImage")));
            this.graphThreat.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphThreat.BaseBarColors")));
            this.graphThreat.Border = true;
            this.graphThreat.BorderColor = System.Drawing.Color.Black;
            this.graphThreat.Clickable = false;
            this.graphThreat.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphThreat.ColorBase = System.Drawing.Color.MediumPurple;
            this.graphThreat.ColorEnh = System.Drawing.Color.Yellow;
            this.graphThreat.ColorFadeEnd = System.Drawing.Color.DarkSlateBlue;
            this.graphThreat.ColorFadeStart = System.Drawing.Color.Black;
            this.graphThreat.ColorHighlight = System.Drawing.Color.Gray;
            this.graphThreat.ColorLines = System.Drawing.Color.Black;
            this.graphThreat.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphThreat.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphThreat.ColorOvercap = System.Drawing.Color.Black;
            this.graphThreat.DifferentiateColors = false;
            this.graphThreat.Dual = true;
            this.graphThreat.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphThreat.EnhBarColors")));
            this.graphThreat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphThreat.ForcedMax = 0F;
            this.graphThreat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphThreat.Highlight = true;
            this.graphThreat.ItemHeight = 10;
            this.graphThreat.Lines = true;
            this.graphThreat.Location = new System.Drawing.Point(0, 96);
            this.graphThreat.MarkerValue = 0F;
            this.graphThreat.Max = 100F;
            this.graphThreat.MaxItems = 60;
            this.graphThreat.Name = "graphThreat";
            this.graphThreat.OuterBorder = false;
            this.graphThreat.Overcap = false;
            this.graphThreat.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphThreat.OvercapColors")));
            this.graphThreat.PaddingX = 2F;
            this.graphThreat.PaddingY = 2F;
            this.graphThreat.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphThreat.PerItemScales")));
            this.graphThreat.ScaleHeight = 32;
            this.graphThreat.ScaleIndex = 8;
            this.graphThreat.ShowScale = false;
            this.graphThreat.Size = new System.Drawing.Size(300, 15);
            this.graphThreat.Style = Enums.GraphStyle.baseOnly;
            this.graphThreat.TabIndex = 13;
            this.graphThreat.TextWidth = 125;
            // 
            // graphEndRdx
            // 
            this.graphEndRdx.BackColor = System.Drawing.Color.Black;
            this.graphEndRdx.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphEndRdx.BackgroundImage")));
            this.graphEndRdx.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphEndRdx.BaseBarColors")));
            this.graphEndRdx.Border = true;
            this.graphEndRdx.BorderColor = System.Drawing.Color.Black;
            this.graphEndRdx.Clickable = false;
            this.graphEndRdx.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphEndRdx.ColorBase = System.Drawing.Color.RoyalBlue;
            this.graphEndRdx.ColorEnh = System.Drawing.Color.Yellow;
            this.graphEndRdx.ColorFadeEnd = System.Drawing.Color.SlateBlue;
            this.graphEndRdx.ColorFadeStart = System.Drawing.Color.Black;
            this.graphEndRdx.ColorHighlight = System.Drawing.Color.Gray;
            this.graphEndRdx.ColorLines = System.Drawing.Color.Black;
            this.graphEndRdx.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphEndRdx.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphEndRdx.ColorOvercap = System.Drawing.Color.Black;
            this.graphEndRdx.DifferentiateColors = false;
            this.graphEndRdx.Dual = true;
            this.graphEndRdx.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphEndRdx.EnhBarColors")));
            this.graphEndRdx.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphEndRdx.ForcedMax = 0F;
            this.graphEndRdx.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphEndRdx.Highlight = true;
            this.graphEndRdx.ItemHeight = 10;
            this.graphEndRdx.Lines = true;
            this.graphEndRdx.Location = new System.Drawing.Point(0, 76);
            this.graphEndRdx.MarkerValue = 0F;
            this.graphEndRdx.Max = 100F;
            this.graphEndRdx.MaxItems = 60;
            this.graphEndRdx.Name = "graphEndRdx";
            this.graphEndRdx.OuterBorder = false;
            this.graphEndRdx.Overcap = false;
            this.graphEndRdx.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphEndRdx.OvercapColors")));
            this.graphEndRdx.PaddingX = 2F;
            this.graphEndRdx.PaddingY = 2F;
            this.graphEndRdx.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphEndRdx.PerItemScales")));
            this.graphEndRdx.ScaleHeight = 32;
            this.graphEndRdx.ScaleIndex = 8;
            this.graphEndRdx.ShowScale = false;
            this.graphEndRdx.Size = new System.Drawing.Size(300, 15);
            this.graphEndRdx.Style = Enums.GraphStyle.baseOnly;
            this.graphEndRdx.TabIndex = 12;
            this.graphEndRdx.TextWidth = 125;
            // 
            // graphDam
            // 
            this.graphDam.BackColor = System.Drawing.Color.Black;
            this.graphDam.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphDam.BackgroundImage")));
            this.graphDam.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDam.BaseBarColors")));
            this.graphDam.Border = true;
            this.graphDam.BorderColor = System.Drawing.Color.Black;
            this.graphDam.Clickable = false;
            this.graphDam.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphDam.ColorBase = System.Drawing.Color.Red;
            this.graphDam.ColorEnh = System.Drawing.Color.Brown;
            this.graphDam.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.graphDam.ColorFadeStart = System.Drawing.Color.Black;
            this.graphDam.ColorHighlight = System.Drawing.Color.Gray;
            this.graphDam.ColorLines = System.Drawing.Color.Black;
            this.graphDam.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphDam.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphDam.ColorOvercap = System.Drawing.Color.Black;
            this.graphDam.DifferentiateColors = false;
            this.graphDam.Dual = true;
            this.graphDam.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDam.EnhBarColors")));
            this.graphDam.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphDam.ForcedMax = 0F;
            this.graphDam.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphDam.Highlight = true;
            this.graphDam.ItemHeight = 10;
            this.graphDam.Lines = true;
            this.graphDam.Location = new System.Drawing.Point(0, 57);
            this.graphDam.MarkerValue = 0F;
            this.graphDam.Max = 100F;
            this.graphDam.MaxItems = 60;
            this.graphDam.Name = "graphDam";
            this.graphDam.OuterBorder = false;
            this.graphDam.Overcap = false;
            this.graphDam.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphDam.OvercapColors")));
            this.graphDam.PaddingX = 2F;
            this.graphDam.PaddingY = 2F;
            this.graphDam.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphDam.PerItemScales")));
            this.graphDam.ScaleHeight = 32;
            this.graphDam.ScaleIndex = 8;
            this.graphDam.ShowScale = false;
            this.graphDam.Size = new System.Drawing.Size(300, 15);
            this.graphDam.Style = Enums.GraphStyle.Stacked;
            this.graphDam.TabIndex = 11;
            this.graphDam.TextWidth = 125;
            // 
            // tab1
            // 
            this.tab1.Location = new System.Drawing.Point(112, 3);
            this.tab1.Name = "tab1";
            this.tab1.Size = new System.Drawing.Size(105, 22);
            this.tab1.TabIndex = 94;
            this.tab1.TabStop = false;
            this.tab1.Click += new System.EventHandler(this.Tab1Click);
            this.tab1.Paint += new System.Windows.Forms.PaintEventHandler(this.Tab1Paint);
            // 
            // tab0
            // 
            this.tab0.Location = new System.Drawing.Point(4, 3);
            this.tab0.Name = "tab0";
            this.tab0.Size = new System.Drawing.Size(105, 22);
            this.tab0.TabIndex = 93;
            this.tab0.TabStop = false;
            this.tab0.Click += new System.EventHandler(this.Tab0Click);
            this.tab0.Paint += new System.Windows.Forms.PaintEventHandler(this.Tab0Paint);
            // 
            // pbTopMost
            // 
            this.pbTopMost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbTopMost.Location = new System.Drawing.Point(4, 468);
            this.pbTopMost.Name = "pbTopMost";
            this.pbTopMost.Size = new System.Drawing.Size(105, 22);
            this.pbTopMost.TabIndex = 95;
            this.pbTopMost.TabStop = false;
            this.pbTopMost.Click += new System.EventHandler(this.PbTopMostClick);
            this.pbTopMost.Paint += new System.Windows.Forms.PaintEventHandler(this.PbTopMostPaint);
            // 
            // pbClose
            // 
            this.pbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbClose.Location = new System.Drawing.Point(220, 468);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(105, 22);
            this.pbClose.TabIndex = 96;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.PbCloseClick);
            this.pbClose.Paint += new System.Windows.Forms.PaintEventHandler(this.PbClosePaint);
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.pnlStatus.Controls.Add(this.graphSRes);
            this.pnlStatus.Controls.Add(this.lblSRes);
            this.pnlStatus.Controls.Add(this.graphSDeb);
            this.pnlStatus.Controls.Add(this.lblSDeb);
            this.pnlStatus.Controls.Add(this.graphSProt);
            this.pnlStatus.Controls.Add(this.lblSProt);
            this.pnlStatus.Location = new System.Drawing.Point(656, 31);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(320, 433);
            this.pnlStatus.TabIndex = 97;
            this.pnlStatus.Visible = false;
            // 
            // graphSRes
            // 
            this.graphSRes.BackColor = System.Drawing.Color.Black;
            this.graphSRes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphSRes.BackgroundImage")));
            this.graphSRes.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSRes.BaseBarColors")));
            this.graphSRes.Border = true;
            this.graphSRes.BorderColor = System.Drawing.Color.Black;
            this.graphSRes.Clickable = false;
            this.graphSRes.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphSRes.ColorBase = System.Drawing.Color.Yellow;
            this.graphSRes.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.graphSRes.ColorFadeEnd = System.Drawing.Color.Olive;
            this.graphSRes.ColorFadeStart = System.Drawing.Color.Black;
            this.graphSRes.ColorHighlight = System.Drawing.Color.Gray;
            this.graphSRes.ColorLines = System.Drawing.Color.Black;
            this.graphSRes.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphSRes.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphSRes.ColorOvercap = System.Drawing.Color.Black;
            this.graphSRes.DifferentiateColors = false;
            this.graphSRes.Dual = true;
            this.graphSRes.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSRes.EnhBarColors")));
            this.graphSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphSRes.ForcedMax = 0F;
            this.graphSRes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphSRes.Highlight = true;
            this.graphSRes.ItemHeight = 9;
            this.graphSRes.Lines = true;
            this.graphSRes.Location = new System.Drawing.Point(15, 175);
            this.graphSRes.MarkerValue = 0F;
            this.graphSRes.Max = 100F;
            this.graphSRes.MaxItems = 60;
            this.graphSRes.Name = "graphSRes";
            this.graphSRes.OuterBorder = false;
            this.graphSRes.Overcap = false;
            this.graphSRes.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSRes.OvercapColors")));
            this.graphSRes.PaddingX = 2F;
            this.graphSRes.PaddingY = 3F;
            this.graphSRes.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphSRes.PerItemScales")));
            this.graphSRes.ScaleHeight = 32;
            this.graphSRes.ScaleIndex = 8;
            this.graphSRes.ShowScale = false;
            this.graphSRes.Size = new System.Drawing.Size(300, 136);
            this.graphSRes.Style = Enums.GraphStyle.baseOnly;
            this.graphSRes.TabIndex = 14;
            this.graphSRes.TextWidth = 125;
            // 
            // lblSRes
            // 
            this.lblSRes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSRes.Location = new System.Drawing.Point(3, 156);
            this.lblSRes.Name = "lblSRes";
            this.lblSRes.Size = new System.Drawing.Size(125, 16);
            this.lblSRes.TabIndex = 13;
            this.lblSRes.Text = "Status Resistance:";
            this.lblSRes.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphSDeb
            // 
            this.graphSDeb.BackColor = System.Drawing.Color.Black;
            this.graphSDeb.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphSDeb.BackgroundImage")));
            this.graphSDeb.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSDeb.BaseBarColors")));
            this.graphSDeb.Border = true;
            this.graphSDeb.BorderColor = System.Drawing.Color.Black;
            this.graphSDeb.Clickable = false;
            this.graphSDeb.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphSDeb.ColorBase = System.Drawing.Color.Cyan;
            this.graphSDeb.ColorEnh = System.Drawing.Color.Yellow;
            this.graphSDeb.ColorFadeEnd = System.Drawing.Color.Teal;
            this.graphSDeb.ColorFadeStart = System.Drawing.Color.Black;
            this.graphSDeb.ColorHighlight = System.Drawing.Color.Gray;
            this.graphSDeb.ColorLines = System.Drawing.Color.Black;
            this.graphSDeb.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphSDeb.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphSDeb.ColorOvercap = System.Drawing.Color.Black;
            this.graphSDeb.DifferentiateColors = false;
            this.graphSDeb.Dual = true;
            this.graphSDeb.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSDeb.EnhBarColors")));
            this.graphSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphSDeb.ForcedMax = 0F;
            this.graphSDeb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphSDeb.Highlight = true;
            this.graphSDeb.ItemHeight = 9;
            this.graphSDeb.Lines = true;
            this.graphSDeb.Location = new System.Drawing.Point(15, 333);
            this.graphSDeb.MarkerValue = 0F;
            this.graphSDeb.Max = 100F;
            this.graphSDeb.MaxItems = 60;
            this.graphSDeb.Name = "graphSDeb";
            this.graphSDeb.OuterBorder = false;
            this.graphSDeb.Overcap = false;
            this.graphSDeb.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSDeb.OvercapColors")));
            this.graphSDeb.PaddingX = 2F;
            this.graphSDeb.PaddingY = 3F;
            this.graphSDeb.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphSDeb.PerItemScales")));
            this.graphSDeb.ScaleHeight = 32;
            this.graphSDeb.ScaleIndex = 8;
            this.graphSDeb.ShowScale = false;
            this.graphSDeb.Size = new System.Drawing.Size(300, 88);
            this.graphSDeb.Style = Enums.GraphStyle.baseOnly;
            this.graphSDeb.TabIndex = 12;
            this.graphSDeb.TextWidth = 125;
            // 
            // lblSDeb
            // 
            this.lblSDeb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSDeb.Location = new System.Drawing.Point(3, 314);
            this.lblSDeb.Name = "lblSDeb";
            this.lblSDeb.Size = new System.Drawing.Size(125, 16);
            this.lblSDeb.TabIndex = 8;
            this.lblSDeb.Text = "Debuff Resistance:";
            this.lblSDeb.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // graphSProt
            // 
            this.graphSProt.BackColor = System.Drawing.Color.Black;
            this.graphSProt.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("graphSProt.BackgroundImage")));
            this.graphSProt.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSProt.BaseBarColors")));
            this.graphSProt.Border = true;
            this.graphSProt.BorderColor = System.Drawing.Color.Black;
            this.graphSProt.Clickable = false;
            this.graphSProt.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.graphSProt.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.graphSProt.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.graphSProt.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.graphSProt.ColorFadeStart = System.Drawing.Color.Black;
            this.graphSProt.ColorHighlight = System.Drawing.Color.Gray;
            this.graphSProt.ColorLines = System.Drawing.Color.Black;
            this.graphSProt.ColorMarkerInner = System.Drawing.Color.Black;
            this.graphSProt.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.graphSProt.ColorOvercap = System.Drawing.Color.Black;
            this.graphSProt.DifferentiateColors = false;
            this.graphSProt.Dual = true;
            this.graphSProt.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSProt.EnhBarColors")));
            this.graphSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.graphSProt.ForcedMax = 0F;
            this.graphSProt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.graphSProt.Highlight = true;
            this.graphSProt.ItemHeight = 9;
            this.graphSProt.Lines = true;
            this.graphSProt.Location = new System.Drawing.Point(15, 17);
            this.graphSProt.MarkerValue = 0F;
            this.graphSProt.Max = 100F;
            this.graphSProt.MaxItems = 60;
            this.graphSProt.Name = "graphSProt";
            this.graphSProt.OuterBorder = false;
            this.graphSProt.Overcap = false;
            this.graphSProt.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("graphSProt.OvercapColors")));
            this.graphSProt.PaddingX = 2F;
            this.graphSProt.PaddingY = 3F;
            this.graphSProt.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("graphSProt.PerItemScales")));
            this.graphSProt.ScaleHeight = 32;
            this.graphSProt.ScaleIndex = 8;
            this.graphSProt.ShowScale = false;
            this.graphSProt.Size = new System.Drawing.Size(300, 136);
            this.graphSProt.Style = Enums.GraphStyle.baseOnly;
            this.graphSProt.TabIndex = 2;
            this.graphSProt.TextWidth = 125;
            // 
            // lblSProt
            // 
            this.lblSProt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSProt.Location = new System.Drawing.Point(3, 0);
            this.lblSProt.Name = "lblSProt";
            this.lblSProt.Size = new System.Drawing.Size(125, 16);
            this.lblSProt.TabIndex = 3;
            this.lblSProt.Text = "Status Protection:";
            this.lblSProt.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tab2
            // 
            this.tab2.Location = new System.Drawing.Point(220, 3);
            this.tab2.Name = "tab2";
            this.tab2.Size = new System.Drawing.Size(105, 22);
            this.tab2.TabIndex = 98;
            this.tab2.TabStop = false;
            this.tab2.Click += new System.EventHandler(this.Tab2Click);
            this.tab2.Paint += new System.Windows.Forms.PaintEventHandler(this.Tab2Paint);
            // 
            // frmTotals
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(995, 493);
            this.Controls.Add(this.tab2);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.pbTopMost);
            this.Controls.Add(this.tab1);
            this.Controls.Add(this.tab0);
            this.Controls.Add(this.pnlMisc);
            this.Controls.Add(this.pnlDRHE);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1024, 603);
            this.MinimumSize = new System.Drawing.Size(344, 532);
            this.Name = "frmTotals";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Totals for Self";
            this.TopMost = true;
            this.pnlDRHE.ResumeLayout(false);
            this.pnlMisc.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tab1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tab0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTopMost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.pnlStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tab2)).EndInit();
            this.ResumeLayout(false);

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