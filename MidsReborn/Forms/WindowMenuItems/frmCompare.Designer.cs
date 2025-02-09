
using System.ComponentModel;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmCompare
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmCompare));
            GroupBox1 = new System.Windows.Forms.GroupBox();
            lblKeyColor1 = new System.Windows.Forms.Label();
            cbSet1 = new System.Windows.Forms.ComboBox();
            cbType1 = new System.Windows.Forms.ComboBox();
            cbAT1 = new System.Windows.Forms.ComboBox();
            GroupBox2 = new System.Windows.Forms.GroupBox();
            lblKeyColor2 = new System.Windows.Forms.Label();
            cbSet2 = new System.Windows.Forms.ComboBox();
            cbType2 = new System.Windows.Forms.ComboBox();
            cbAT2 = new System.Windows.Forms.ComboBox();
            lblScale = new System.Windows.Forms.Label();
            tbScaleX = new System.Windows.Forms.TrackBar();
            chkMatching = new System.Windows.Forms.CheckBox();
            tTip = new System.Windows.Forms.ToolTip(components);
            btnTweakMatch = new System.Windows.Forms.Button();
            Graph = new CtlMultiGraph();
            GroupBox4 = new System.Windows.Forms.GroupBox();
            lstDisplay = new System.Windows.Forms.ListBox();
            ibExKeepOnTop = new Controls.ImageButtonEx();
            ibExClose = new Controls.ImageButtonEx();
            GroupBox1.SuspendLayout();
            GroupBox2.SuspendLayout();
            ((ISupportInitialize)tbScaleX).BeginInit();
            GroupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(lblKeyColor1);
            GroupBox1.Controls.Add(cbSet1);
            GroupBox1.Controls.Add(cbType1);
            GroupBox1.Controls.Add(cbAT1);
            GroupBox1.ForeColor = System.Drawing.Color.White;
            GroupBox1.Location = new System.Drawing.Point(4, 4);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(144, 116);
            GroupBox1.TabIndex = 2;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Powerset 1:";
            // 
            // lblKeyColor1
            // 
            lblKeyColor1.BackColor = System.Drawing.Color.Blue;
            lblKeyColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblKeyColor1.Location = new System.Drawing.Point(8, 20);
            lblKeyColor1.Name = "lblKeyColor1";
            lblKeyColor1.Size = new System.Drawing.Size(132, 16);
            lblKeyColor1.TabIndex = 3;
            // 
            // cbSet1
            // 
            cbSet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSet1.Location = new System.Drawing.Point(8, 88);
            cbSet1.Name = "cbSet1";
            cbSet1.Size = new System.Drawing.Size(132, 21);
            cbSet1.TabIndex = 2;
            cbSet1.SelectedIndexChanged += cbSet1_SelectedIndexChanged;
            // 
            // cbType1
            // 
            cbType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbType1.Location = new System.Drawing.Point(8, 64);
            cbType1.Name = "cbType1";
            cbType1.Size = new System.Drawing.Size(132, 21);
            cbType1.TabIndex = 1;
            cbType1.SelectedIndexChanged += cbType1_SelectedIndexChanged;
            // 
            // cbAT1
            // 
            cbAT1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAT1.Location = new System.Drawing.Point(8, 40);
            cbAT1.Name = "cbAT1";
            cbAT1.Size = new System.Drawing.Size(132, 21);
            cbAT1.TabIndex = 0;
            cbAT1.SelectedIndexChanged += cbAT1_SelectedIndexChanged;
            // 
            // GroupBox2
            // 
            GroupBox2.Controls.Add(lblKeyColor2);
            GroupBox2.Controls.Add(cbSet2);
            GroupBox2.Controls.Add(cbType2);
            GroupBox2.Controls.Add(cbAT2);
            GroupBox2.ForeColor = System.Drawing.Color.White;
            GroupBox2.Location = new System.Drawing.Point(4, 126);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(144, 116);
            GroupBox2.TabIndex = 3;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Powerset 2:";
            // 
            // lblKeyColor2
            // 
            lblKeyColor2.BackColor = System.Drawing.Color.Yellow;
            lblKeyColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblKeyColor2.Location = new System.Drawing.Point(8, 20);
            lblKeyColor2.Name = "lblKeyColor2";
            lblKeyColor2.Size = new System.Drawing.Size(132, 16);
            lblKeyColor2.TabIndex = 3;
            // 
            // cbSet2
            // 
            cbSet2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbSet2.Location = new System.Drawing.Point(8, 88);
            cbSet2.Name = "cbSet2";
            cbSet2.Size = new System.Drawing.Size(132, 21);
            cbSet2.TabIndex = 2;
            cbSet2.SelectedIndexChanged += cbSet2_SelectedIndexChanged;
            // 
            // cbType2
            // 
            cbType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbType2.Location = new System.Drawing.Point(8, 64);
            cbType2.Name = "cbType2";
            cbType2.Size = new System.Drawing.Size(132, 21);
            cbType2.TabIndex = 1;
            cbType2.SelectedIndexChanged += cbType2_SelectedIndexChanged;
            // 
            // cbAT2
            // 
            cbAT2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbAT2.Location = new System.Drawing.Point(8, 40);
            cbAT2.Name = "cbAT2";
            cbAT2.Size = new System.Drawing.Size(132, 21);
            cbAT2.TabIndex = 0;
            cbAT2.SelectedIndexChanged += cbAT2_SelectedIndexChanged;
            // 
            // lblScale
            // 
            lblScale.Location = new System.Drawing.Point(312, 500);
            lblScale.Name = "lblScale";
            lblScale.Size = new System.Drawing.Size(108, 20);
            lblScale.TabIndex = 9;
            lblScale.Text = "Scale: 100%";
            lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbScaleX
            // 
            tbScaleX.LargeChange = 1;
            tbScaleX.Location = new System.Drawing.Point(184, 476);
            tbScaleX.Minimum = 1;
            tbScaleX.Name = "tbScaleX";
            tbScaleX.Size = new System.Drawing.Size(340, 45);
            tbScaleX.TabIndex = 8;
            tbScaleX.TickFrequency = 10;
            tbScaleX.TickStyle = System.Windows.Forms.TickStyle.None;
            tbScaleX.Value = 10;
            tbScaleX.Scroll += tbScaleX_Scroll;
            // 
            // chkMatching
            // 
            chkMatching.Location = new System.Drawing.Point(8, 508);
            chkMatching.Name = "chkMatching";
            chkMatching.Size = new System.Drawing.Size(144, 20);
            chkMatching.TabIndex = 11;
            chkMatching.Text = "Attempt Matching";
            chkMatching.CheckedChanged += chkMatching_CheckedChanged;
            // 
            // tTip
            // 
            tTip.AutoPopDelay = 10000;
            tTip.InitialDelay = 500;
            tTip.ReshowDelay = 100;
            // 
            // btnTweakMatch
            // 
            btnTweakMatch.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            btnTweakMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnTweakMatch.ForeColor = System.Drawing.SystemColors.ControlText;
            btnTweakMatch.Location = new System.Drawing.Point(172, 507);
            btnTweakMatch.Name = "btnTweakMatch";
            btnTweakMatch.Size = new System.Drawing.Size(60, 20);
            btnTweakMatch.TabIndex = 12;
            btnTweakMatch.Text = "Tweak";
            tTip.SetToolTip(btnTweakMatch, "Modify the data used to perform power matching");
            btnTweakMatch.UseVisualStyleBackColor = true;
            btnTweakMatch.Visible = false;
            btnTweakMatch.Click += btnTweakMatch_Click;
            // 
            // Graph
            // 
            Graph.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            Graph.BackgroundImage = (System.Drawing.Image)resources.GetObject("Graph.BackgroundImage");
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
            Graph.ColorOvercap = System.Drawing.Color.Black;
            Graph.DifferentiateColors = false;
            Graph.DrawRuler = true;
            Graph.Dual = true;
            Graph.EnhBarColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("Graph.EnhBarColors");
            Graph.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Graph.ForcedMax = 0F;
            Graph.ForeColor = System.Drawing.Color.FromArgb(192, 192, 255);
            Graph.Highlight = true;
            Graph.ImeMode = System.Windows.Forms.ImeMode.Off;
            Graph.ItemFontSizeOverride = 10F;
            Graph.ItemHeight = 26;
            Graph.Lines = true;
            Graph.Location = new System.Drawing.Point(152, 4);
            Graph.MarkerValue = 0F;
            Graph.Max = 100F;
            Graph.MaxItems = 60;
            Graph.Name = "Graph";
            Graph.OuterBorder = false;
            Graph.Overcap = false;
            Graph.OvercapColors = (System.Collections.Generic.List<System.Drawing.Color>)resources.GetObject("Graph.OvercapColors");
            Graph.PaddingX = 2F;
            Graph.PaddingY = 6F;
            Graph.PerItemScales = (System.Collections.Generic.List<float>)resources.GetObject("Graph.PerItemScales");
            Graph.RulerPos = CtlMultiGraph.RulerPosition.Top;
            Graph.SecondaryLabelPosition = CtlMultiGraph.Alignment.Left;
            Graph.ScaleHeight = 16;
            Graph.ScaleIndex = 8;
            Graph.ShowScale = false;
            Graph.SingleLineLabels = false;
            Graph.Size = new System.Drawing.Size(484, 468);
            Graph.Style = Enums.GraphStyle.Twin;
            Graph.TabIndex = 1;
            Graph.TextWidth = 120;
            Graph.Load += Graph_Load;
            // 
            // GroupBox4
            // 
            GroupBox4.Controls.Add(lstDisplay);
            GroupBox4.ForeColor = System.Drawing.Color.White;
            GroupBox4.Location = new System.Drawing.Point(8, 248);
            GroupBox4.Name = "GroupBox4";
            GroupBox4.Size = new System.Drawing.Size(144, 254);
            GroupBox4.TabIndex = 16;
            GroupBox4.TabStop = false;
            GroupBox4.Text = "Display:";
            // 
            // lstDisplay
            // 
            lstDisplay.FormattingEnabled = true;
            lstDisplay.Location = new System.Drawing.Point(6, 19);
            lstDisplay.Name = "lstDisplay";
            lstDisplay.Size = new System.Drawing.Size(130, 225);
            lstDisplay.TabIndex = 0;
            lstDisplay.SelectedIndexChanged += lstDisplay_SelectedIndexChanged;
            // 
            // ibExKeepOnTop
            // 
            ibExKeepOnTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibExKeepOnTop.ButtonType = Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            ibExKeepOnTop.CurrentText = "Keep on Top";
            ibExKeepOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibExKeepOnTop.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibExKeepOnTop.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibExKeepOnTop.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibExKeepOnTop.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibExKeepOnTop.Location = new System.Drawing.Point(530, 476);
            ibExKeepOnTop.Lock = false;
            ibExKeepOnTop.Name = "ibExKeepOnTop";
            ibExKeepOnTop.Size = new System.Drawing.Size(105, 22);
            ibExKeepOnTop.TabIndex = 17;
            ibExKeepOnTop.Text = "Keep on Top";
            ibExKeepOnTop.TextOutline.Color = System.Drawing.Color.Black;
            ibExKeepOnTop.TextOutline.Width = 2;
            ibExKeepOnTop.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibExKeepOnTop.ToggleText.Indeterminate = "Indeterminate State";
            ibExKeepOnTop.ToggleText.ToggledOff = "To Top";
            ibExKeepOnTop.ToggleText.ToggledOn = "Keep on Top";
            ibExKeepOnTop.UseAlt = false;
            ibExKeepOnTop.Click += ibExKeepOnTop_Click;
            // 
            // ibExClose
            // 
            ibExClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ibExClose.CurrentText = "Close";
            ibExClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibExClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibExClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibExClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibExClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibExClose.Location = new System.Drawing.Point(530, 504);
            ibExClose.Lock = false;
            ibExClose.Name = "ibExClose";
            ibExClose.Size = new System.Drawing.Size(105, 22);
            ibExClose.TabIndex = 18;
            ibExClose.Text = "Close";
            ibExClose.TextOutline.Color = System.Drawing.Color.Black;
            ibExClose.TextOutline.Width = 2;
            ibExClose.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibExClose.ToggleText.Indeterminate = "Indeterminate State";
            ibExClose.ToggleText.ToggledOff = "Close";
            ibExClose.ToggleText.ToggledOn = "Close";
            ibExClose.UseAlt = false;
            ibExClose.Click += ibExClose_Click;
            // 
            // frmCompare
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            ClientSize = new System.Drawing.Size(640, 532);
            Controls.Add(ibExClose);
            Controls.Add(ibExKeepOnTop);
            Controls.Add(GroupBox4);
            Controls.Add(btnTweakMatch);
            Controls.Add(chkMatching);
            Controls.Add(lblScale);
            Controls.Add(tbScaleX);
            Controls.Add(GroupBox2);
            Controls.Add(GroupBox1);
            Controls.Add(Graph);
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            ForeColor = System.Drawing.Color.White;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCompare";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Powerset Comparison";
            TopMost = true;
            GroupBox1.ResumeLayout(false);
            GroupBox2.ResumeLayout(false);
            ((ISupportInitialize)tbScaleX).EndInit();
            GroupBox4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Controls.ImageButtonEx ibExKeepOnTop;
        private Controls.ImageButtonEx ibExClose;
    }


}