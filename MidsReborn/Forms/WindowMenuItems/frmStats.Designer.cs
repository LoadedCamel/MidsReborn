using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmStats
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmStats));
            lblKey2 = new Label();
            lblKey1 = new Label();
            lblKeyColor2 = new Label();
            lblKeyColor1 = new Label();
            tbScaleX = new TrackBar();
            lblScale = new Label();
            tTip = new ToolTip(components);
            cbSet = new ComboBox();
            cbValues = new ComboBox();
            cbStyle = new ComboBox();
            Graph = new Mids_Reborn.Controls.CtlMultiGraph();
            chkOnTop = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            btnClose = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            CompareGraph = new Mids_Reborn.Controls.CtlMultiGraph();
            MenuBar = new Mids_Reborn.Forms.Controls.MidsMenuStrip();
            CompareToolStripMenuItem = new ToolStripMenuItem();
            TsCompareImport = new ToolStripMenuItem();
            TsCompareExport = new ToolStripMenuItem();
            ToolStripSeparator1 = new ToolStripSeparator();
            TsEndCompare = new ToolStripMenuItem();
            label1 = new Label();
            label2 = new Label();
            cbCompareGraphStyle = new ComboBox();
            ((ISupportInitialize)tbScaleX).BeginInit();
            MenuBar.SuspendLayout();
            SuspendLayout();
            // 
            // lblKey2
            // 
            lblKey2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblKey2.Location = new System.Drawing.Point(56, 555);
            lblKey2.Name = "lblKey2";
            lblKey2.Size = new System.Drawing.Size(78, 16);
            lblKey2.TabIndex = 3;
            lblKey2.Text = "Enhanced";
            lblKey2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKey1
            // 
            lblKey1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblKey1.Location = new System.Drawing.Point(56, 535);
            lblKey1.Name = "lblKey1";
            lblKey1.Size = new System.Drawing.Size(78, 16);
            lblKey1.TabIndex = 2;
            lblKey1.Text = "Base";
            lblKey1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKeyColor2
            // 
            lblKeyColor2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblKeyColor2.BackColor = System.Drawing.Color.Yellow;
            lblKeyColor2.BorderStyle = BorderStyle.FixedSingle;
            lblKeyColor2.Location = new System.Drawing.Point(12, 555);
            lblKeyColor2.Name = "lblKeyColor2";
            lblKeyColor2.Size = new System.Drawing.Size(40, 16);
            lblKeyColor2.TabIndex = 1;
            // 
            // lblKeyColor1
            // 
            lblKeyColor1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblKeyColor1.BackColor = System.Drawing.Color.Blue;
            lblKeyColor1.BorderStyle = BorderStyle.FixedSingle;
            lblKeyColor1.Location = new System.Drawing.Point(12, 535);
            lblKeyColor1.Name = "lblKeyColor1";
            lblKeyColor1.Size = new System.Drawing.Size(40, 16);
            lblKeyColor1.TabIndex = 0;
            // 
            // tbScaleX
            // 
            tbScaleX.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            tbScaleX.LargeChange = 1;
            tbScaleX.Location = new System.Drawing.Point(140, 530);
            tbScaleX.Minimum = 1;
            tbScaleX.Name = "tbScaleX";
            tbScaleX.Size = new System.Drawing.Size(237, 45);
            tbScaleX.TabIndex = 6;
            tbScaleX.TickFrequency = 10;
            tbScaleX.TickStyle = TickStyle.None;
            tTip.SetToolTip(tbScaleX, "Move the slider to the left to zoom in on lower values.");
            tbScaleX.Value = 10;
            tbScaleX.Scroll += tbScaleX_Scroll;
            // 
            // lblScale
            // 
            lblScale.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblScale.Location = new System.Drawing.Point(212, 554);
            lblScale.Name = "lblScale";
            lblScale.Size = new System.Drawing.Size(108, 20);
            lblScale.TabIndex = 7;
            lblScale.Text = "Scale: 100%";
            lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tTip
            // 
            tTip.AutoPopDelay = 10000;
            tTip.InitialDelay = 500;
            tTip.ReshowDelay = 100;
            // 
            // cbSet
            // 
            cbSet.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSet.FormattingEnabled = true;
            cbSet.Location = new System.Drawing.Point(6, 29);
            cbSet.MaxDropDownItems = 16;
            cbSet.Name = "cbSet";
            cbSet.Size = new System.Drawing.Size(158, 23);
            cbSet.TabIndex = 10;
            cbSet.SelectedIndexChanged += cbSet_SelectedIndexChanged;
            // 
            // cbValues
            // 
            cbValues.DropDownStyle = ComboBoxStyle.DropDownList;
            cbValues.FormattingEnabled = true;
            cbValues.Location = new System.Drawing.Point(170, 29);
            cbValues.MaxDropDownItems = 16;
            cbValues.Name = "cbValues";
            cbValues.Size = new System.Drawing.Size(132, 23);
            cbValues.TabIndex = 11;
            cbValues.SelectedIndexChanged += cbValues_SelectedIndexChanged;
            // 
            // cbStyle
            // 
            cbStyle.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStyle.FormattingEnabled = true;
            cbStyle.Location = new System.Drawing.Point(308, 29);
            cbStyle.Name = "cbStyle";
            cbStyle.Size = new System.Drawing.Size(180, 23);
            cbStyle.TabIndex = 12;
            cbStyle.SelectedIndexChanged += cbStyle_SelectedIndexChanged;
            // 
            // Graph
            // 
            Graph.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            Graph.BackgroundImage = (System.Drawing.Image)resources.GetObject("Graph.BackgroundImage");
            Graph.BarsAlignment = Mids_Reborn.Controls.CtlMultiGraph.BarAlignment.Left;
            Graph.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("Graph.BaseBarColors");
            Graph.Border = false;
            Graph.BorderColor = System.Drawing.Color.Black;
            Graph.Clickable = false;
            Graph.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            Graph.ColorBase = System.Drawing.Color.Blue;
            Graph.ColorEnh = System.Drawing.Color.Yellow;
            Graph.ColorFadeEnd = System.Drawing.Color.DarkRed;
            Graph.ColorFadeStart = System.Drawing.Color.Black;
            Graph.ColorHighlight = System.Drawing.Color.White;
            Graph.ColorLines = System.Drawing.Color.Black;
            Graph.ColorMarkerInner = System.Drawing.Color.Black;
            Graph.ColorMarkerOuter = System.Drawing.Color.Yellow;
            Graph.ColorOvercap = System.Drawing.Color.Cyan;
            Graph.DifferentiateColors = false;
            Graph.DrawRuler = true;
            Graph.Dual = true;
            Graph.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("Graph.EnhBarColors");
            Graph.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            Graph.ForcedMax = 0F;
            Graph.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            Graph.Highlight = true;
            Graph.ImeMode = ImeMode.Off;
            Graph.ItemFontSizeOverride = 11F;
            Graph.ItemHeight = 12;
            Graph.Lines = true;
            Graph.Location = new System.Drawing.Point(4, 121);
            Graph.MarkerValue = 0F;
            Graph.Max = 75F;
            Graph.MaxItems = 60;
            Graph.Name = "Graph";
            Graph.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            Graph.NegativeBaseColor = System.Drawing.Color.Navy;
            Graph.NegativeEnhColor = System.Drawing.Color.Olive;
            Graph.NegativeOvercapColor = System.Drawing.Color.Teal;
            Graph.OuterBorder = false;
            Graph.Overcap = false;
            Graph.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("Graph.OvercapColors");
            Graph.PaddingX = 2F;
            Graph.PaddingY = 4F;
            Graph.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("Graph.PerItemScales");
            Graph.RulerPos = Mids_Reborn.Controls.CtlMultiGraph.RulerPosition.Top;
            Graph.ScaleHeight = 16;
            Graph.ScaleIndex = 7;
            Graph.SecondaryLabelPosition = Mids_Reborn.Controls.CtlMultiGraph.Alignment.Right;
            Graph.ShowScale = false;
            Graph.SingleLineLabels = true;
            Graph.Size = new System.Drawing.Size(484, 398);
            Graph.Style = Core.Enums.GraphStyle.Stacked;
            Graph.TabIndex = 0;
            Graph.TextWidth = 100;
            // 
            // chkOnTop
            // 
            chkOnTop.BackgroundImageLayout = ImageLayout.None;
            chkOnTop.ButtonType = Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            chkOnTop.CurrentText = "ToggledOff State";
            chkOnTop.DisplayVertically = false;
            chkOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            chkOnTop.ForeColor = System.Drawing.Color.WhiteSmoke;
            chkOnTop.Images.Background = MRBResourceLib.Resources.HeroButton;
            chkOnTop.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            chkOnTop.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            chkOnTop.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            chkOnTop.Location = new System.Drawing.Point(383, 524);
            chkOnTop.Lock = false;
            chkOnTop.Name = "chkOnTop";
            chkOnTop.Size = new System.Drawing.Size(105, 22);
            chkOnTop.TabIndex = 13;
            chkOnTop.Text = "imageButtonEx1";
            chkOnTop.TextOutline.Color = System.Drawing.Color.Black;
            chkOnTop.TextOutline.Width = 2;
            chkOnTop.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            chkOnTop.ToggleText.Indeterminate = "Indeterminate State";
            chkOnTop.ToggleText.ToggledOff = "To Top Most";
            chkOnTop.ToggleText.ToggledOn = "Top Most";
            chkOnTop.UseAlt = false;
            // 
            // btnClose
            // 
            btnClose.BackgroundImageLayout = ImageLayout.None;
            btnClose.CurrentText = "Close";
            btnClose.DisplayVertically = false;
            btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btnClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnClose.Location = new System.Drawing.Point(383, 551);
            btnClose.Lock = false;
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(105, 22);
            btnClose.TabIndex = 14;
            btnClose.Text = "Close";
            btnClose.TextOutline.Color = System.Drawing.Color.Black;
            btnClose.TextOutline.Width = 2;
            btnClose.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnClose.ToggleText.Indeterminate = "Indeterminate State";
            btnClose.ToggleText.ToggledOff = "ToggledOff State";
            btnClose.ToggleText.ToggledOn = "ToggledOn State";
            btnClose.UseAlt = false;
            // 
            // CompareGraph
            // 
            CompareGraph.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            CompareGraph.BackgroundImage = (System.Drawing.Image)resources.GetObject("CompareGraph.BackgroundImage");
            CompareGraph.BarsAlignment = Mids_Reborn.Controls.CtlMultiGraph.BarAlignment.Left;
            CompareGraph.BaseBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("CompareGraph.BaseBarColors");
            CompareGraph.Border = false;
            CompareGraph.BorderColor = System.Drawing.Color.Black;
            CompareGraph.Clickable = false;
            CompareGraph.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            CompareGraph.ColorBase = System.Drawing.Color.Blue;
            CompareGraph.ColorEnh = System.Drawing.Color.Yellow;
            CompareGraph.ColorFadeEnd = System.Drawing.Color.DarkRed;
            CompareGraph.ColorFadeStart = System.Drawing.Color.Black;
            CompareGraph.ColorHighlight = System.Drawing.Color.White;
            CompareGraph.ColorLines = System.Drawing.Color.Black;
            CompareGraph.ColorMarkerInner = System.Drawing.Color.Black;
            CompareGraph.ColorMarkerOuter = System.Drawing.Color.Yellow;
            CompareGraph.ColorOvercap = System.Drawing.Color.Cyan;
            CompareGraph.DifferentiateColors = false;
            CompareGraph.DrawRuler = true;
            CompareGraph.Dual = true;
            CompareGraph.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("CompareGraph.EnhBarColors");
            CompareGraph.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            CompareGraph.ForcedMax = 0F;
            CompareGraph.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            CompareGraph.Highlight = true;
            CompareGraph.ImeMode = ImeMode.Off;
            CompareGraph.ItemFontSizeOverride = 11F;
            CompareGraph.ItemHeight = 12;
            CompareGraph.Lines = true;
            CompareGraph.Location = new System.Drawing.Point(510, 121);
            CompareGraph.MarkerValue = 0F;
            CompareGraph.Max = 75F;
            CompareGraph.MaxItems = 60;
            CompareGraph.Name = "CompareGraph";
            CompareGraph.NegativeAbsorbedColor = System.Drawing.Color.SlateGray;
            CompareGraph.NegativeBaseColor = System.Drawing.Color.Navy;
            CompareGraph.NegativeEnhColor = System.Drawing.Color.Olive;
            CompareGraph.NegativeOvercapColor = System.Drawing.Color.Teal;
            CompareGraph.OuterBorder = false;
            CompareGraph.Overcap = false;
            CompareGraph.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("CompareGraph.OvercapColors");
            CompareGraph.PaddingX = 2F;
            CompareGraph.PaddingY = 4F;
            CompareGraph.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("CompareGraph.PerItemScales");
            CompareGraph.RulerPos = Mids_Reborn.Controls.CtlMultiGraph.RulerPosition.Top;
            CompareGraph.ScaleHeight = 16;
            CompareGraph.ScaleIndex = 7;
            CompareGraph.SecondaryLabelPosition = Mids_Reborn.Controls.CtlMultiGraph.Alignment.Right;
            CompareGraph.ShowScale = false;
            CompareGraph.SingleLineLabels = true;
            CompareGraph.Size = new System.Drawing.Size(484, 398);
            CompareGraph.Style = Core.Enums.GraphStyle.Stacked;
            CompareGraph.TabIndex = 15;
            CompareGraph.TextWidth = 100;
            CompareGraph.Visible = false;
            // 
            // MenuBar
            // 
            MenuBar.Items.AddRange(new ToolStripItem[] { CompareToolStripMenuItem });
            MenuBar.Location = new System.Drawing.Point(0, 0);
            MenuBar.Name = "MenuBar";
            MenuBar.Size = new System.Drawing.Size(1004, 24);
            MenuBar.TabIndex = 16;
            MenuBar.Text = "Compare";
            // 
            // CompareToolStripMenuItem
            // 
            CompareToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TsCompareImport, TsCompareExport, ToolStripSeparator1, TsEndCompare });
            CompareToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            CompareToolStripMenuItem.Name = "CompareToolStripMenuItem";
            CompareToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            CompareToolStripMenuItem.Text = "&Compare";
            // 
            // TsCompareImport
            // 
            TsCompareImport.Name = "TsCompareImport";
            TsCompareImport.Size = new System.Drawing.Size(170, 22);
            TsCompareImport.Text = "&Import / Compare";
            TsCompareImport.Click += TsCompareImport_Click;
            // 
            // TsCompareExport
            // 
            TsCompareExport.Name = "TsCompareExport";
            TsCompareExport.Size = new System.Drawing.Size(170, 22);
            TsCompareExport.Text = "&Export";
            TsCompareExport.Click += TsCompareExport_Click;
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            ToolStripSeparator1.Visible = false;
            // 
            // TsEndCompare
            // 
            TsEndCompare.Name = "TsEndCompare";
            TsEndCompare.Size = new System.Drawing.Size(170, 22);
            TsEndCompare.Text = "E&nd Compare";
            TsEndCompare.Visible = false;
            TsEndCompare.Click += TsEndCompare_Click;
            // 
            // label1
            // 
            label1.ForeColor = System.Drawing.Color.LightGray;
            label1.Location = new System.Drawing.Point(6, 55);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(482, 63);
            label1.TabIndex = 17;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.ForeColor = System.Drawing.Color.LightGray;
            label2.Location = new System.Drawing.Point(512, 55);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(482, 63);
            label2.TabIndex = 18;
            label2.Text = "label2";
            // 
            // cbCompareGraphStyle
            // 
            cbCompareGraphStyle.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCompareGraphStyle.FormattingEnabled = true;
            cbCompareGraphStyle.Items.AddRange(new object[] { "Diff", "Raw Values" });
            cbCompareGraphStyle.Location = new System.Drawing.Point(814, 29);
            cbCompareGraphStyle.Name = "cbCompareGraphStyle";
            cbCompareGraphStyle.Size = new System.Drawing.Size(180, 23);
            cbCompareGraphStyle.TabIndex = 19;
            cbCompareGraphStyle.SelectedIndexChanged += cbCompareGraphStyle_SelectedIndexChanged;
            // 
            // frmStats
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            ClientSize = new System.Drawing.Size(1004, 577);
            Controls.Add(cbCompareGraphStyle);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(CompareGraph);
            Controls.Add(btnClose);
            Controls.Add(chkOnTop);
            Controls.Add(cbStyle);
            Controls.Add(lblKey2);
            Controls.Add(cbValues);
            Controls.Add(lblKey1);
            Controls.Add(cbSet);
            Controls.Add(lblKeyColor2);
            Controls.Add(lblKeyColor1);
            Controls.Add(lblScale);
            Controls.Add(tbScaleX);
            Controls.Add(Graph);
            Controls.Add(MenuBar);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(400, 340);
            Name = "frmStats";
            StartPosition = FormStartPosition.Manual;
            Text = "Power Stats";
            TopMost = true;
            ((ISupportInitialize)tbScaleX).EndInit();
            MenuBar.ResumeLayout(false);
            MenuBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        private Controls.ImageButtonEx chkOnTop;
        private Controls.ImageButtonEx btnClose;
        private Mids_Reborn.Controls.CtlMultiGraph CompareGraph;
        private Controls.MidsMenuStrip MenuBar;
        private ToolStripMenuItem CompareToolStripMenuItem;
        private ToolStripMenuItem TsCompareImport;
        private ToolStripMenuItem TsCompareExport;
        private ToolStripMenuItem TsEndCompare;
        private ToolStripSeparator ToolStripSeparator1;
        private Label label1;
        private Label label2;
        private ComboBox cbCompareGraphStyle;
    }
}