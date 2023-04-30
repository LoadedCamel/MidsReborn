using System.ComponentModel;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStats));
            this.lblKey2 = new System.Windows.Forms.Label();
            this.lblKey1 = new System.Windows.Forms.Label();
            this.lblKeyColor2 = new System.Windows.Forms.Label();
            this.lblKeyColor1 = new System.Windows.Forms.Label();
            this.tbScaleX = new System.Windows.Forms.TrackBar();
            this.lblScale = new System.Windows.Forms.Label();
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbSet = new System.Windows.Forms.ComboBox();
            this.cbValues = new System.Windows.Forms.ComboBox();
            this.cbStyle = new System.Windows.Forms.ComboBox();
            this.Graph = new Mids_Reborn.Controls.CtlMultiGraph();
            this.chkOnTop = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            this.btnClose = new Mids_Reborn.Forms.Controls.ImageButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.tbScaleX)).BeginInit();
            this.SuspendLayout();
            // 
            // lblKey2
            // 
            this.lblKey2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKey2.Location = new System.Drawing.Point(56, 463);
            this.lblKey2.Name = "lblKey2";
            this.lblKey2.Size = new System.Drawing.Size(78, 16);
            this.lblKey2.TabIndex = 3;
            this.lblKey2.Text = "Enhanced";
            this.lblKey2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKey1
            // 
            this.lblKey1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKey1.Location = new System.Drawing.Point(56, 443);
            this.lblKey1.Name = "lblKey1";
            this.lblKey1.Size = new System.Drawing.Size(78, 16);
            this.lblKey1.TabIndex = 2;
            this.lblKey1.Text = "Base";
            this.lblKey1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKeyColor2
            // 
            this.lblKeyColor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKeyColor2.BackColor = System.Drawing.Color.Yellow;
            this.lblKeyColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKeyColor2.Location = new System.Drawing.Point(12, 463);
            this.lblKeyColor2.Name = "lblKeyColor2";
            this.lblKeyColor2.Size = new System.Drawing.Size(40, 16);
            this.lblKeyColor2.TabIndex = 1;
            // 
            // lblKeyColor1
            // 
            this.lblKeyColor1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKeyColor1.BackColor = System.Drawing.Color.Blue;
            this.lblKeyColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKeyColor1.Location = new System.Drawing.Point(12, 443);
            this.lblKeyColor1.Name = "lblKeyColor1";
            this.lblKeyColor1.Size = new System.Drawing.Size(40, 16);
            this.lblKeyColor1.TabIndex = 0;
            // 
            // tbScaleX
            // 
            this.tbScaleX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbScaleX.LargeChange = 1;
            this.tbScaleX.Location = new System.Drawing.Point(140, 438);
            this.tbScaleX.Minimum = 1;
            this.tbScaleX.Name = "tbScaleX";
            this.tbScaleX.Size = new System.Drawing.Size(237, 45);
            this.tbScaleX.TabIndex = 6;
            this.tbScaleX.TickFrequency = 10;
            this.tbScaleX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tTip.SetToolTip(this.tbScaleX, "Move the slider to the left to zoom in on lower values.");
            this.tbScaleX.Value = 10;
            this.tbScaleX.Scroll += new System.EventHandler(this.tbScaleX_Scroll);
            // 
            // lblScale
            // 
            this.lblScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScale.Location = new System.Drawing.Point(212, 462);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(108, 20);
            this.lblScale.TabIndex = 7;
            this.lblScale.Text = "Scale: 100%";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tTip
            // 
            this.tTip.AutoPopDelay = 10000;
            this.tTip.InitialDelay = 500;
            this.tTip.ReshowDelay = 100;
            // 
            // cbSet
            // 
            this.cbSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSet.FormattingEnabled = true;
            this.cbSet.Location = new System.Drawing.Point(6, 5);
            this.cbSet.MaxDropDownItems = 16;
            this.cbSet.Name = "cbSet";
            this.cbSet.Size = new System.Drawing.Size(158, 24);
            this.cbSet.TabIndex = 10;
            this.cbSet.SelectedIndexChanged += new System.EventHandler(this.cbSet_SelectedIndexChanged);
            // 
            // cbValues
            // 
            this.cbValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbValues.FormattingEnabled = true;
            this.cbValues.Location = new System.Drawing.Point(170, 5);
            this.cbValues.MaxDropDownItems = 16;
            this.cbValues.Name = "cbValues";
            this.cbValues.Size = new System.Drawing.Size(101, 24);
            this.cbValues.TabIndex = 11;
            this.cbValues.SelectedIndexChanged += new System.EventHandler(this.cbValues_SelectedIndexChanged);
            // 
            // cbStyle
            // 
            this.cbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStyle.FormattingEnabled = true;
            this.cbStyle.Location = new System.Drawing.Point(277, 5);
            this.cbStyle.Name = "cbStyle";
            this.cbStyle.Size = new System.Drawing.Size(186, 24);
            this.cbStyle.TabIndex = 12;
            this.cbStyle.SelectedIndexChanged += new System.EventHandler(this.cbStyle_SelectedIndexChanged);
            // 
            // Graph
            // 
            this.Graph.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.Graph.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Graph.BackgroundImage")));
            this.Graph.BaseBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("Graph.BaseBarColors")));
            this.Graph.Border = false;
            this.Graph.BorderColor = System.Drawing.Color.Black;
            this.Graph.Clickable = false;
            this.Graph.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.Graph.ColorBase = System.Drawing.Color.Blue;
            this.Graph.ColorEnh = System.Drawing.Color.Yellow;
            this.Graph.ColorFadeEnd = System.Drawing.Color.DarkRed;
            this.Graph.ColorFadeStart = System.Drawing.Color.Black;
            this.Graph.ColorHighlight = System.Drawing.Color.White;
            this.Graph.ColorLines = System.Drawing.Color.Black;
            this.Graph.ColorMarkerInner = System.Drawing.Color.Black;
            this.Graph.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.Graph.ColorOvercap = System.Drawing.Color.Cyan;
            this.Graph.DifferentiateColors = false;
            this.Graph.DrawRuler = true;
            this.Graph.Dual = true;
            this.Graph.EnhBarColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("Graph.EnhBarColors")));
            this.Graph.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Graph.ForcedMax = 0F;
            this.Graph.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Graph.Highlight = true;
            this.Graph.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Graph.ItemFontSizeOverride = 11F;
            this.Graph.ItemHeight = 12;
            this.Graph.Lines = true;
            this.Graph.Location = new System.Drawing.Point(4, 35);
            this.Graph.MarkerValue = 0F;
            this.Graph.Max = 75F;
            this.Graph.MaxItems = 60;
            this.Graph.Name = "Graph";
            this.Graph.OuterBorder = false;
            this.Graph.Overcap = false;
            this.Graph.OvercapColors = ((System.Collections.Generic.List<System.Drawing.Color>)(resources.GetObject("Graph.OvercapColors")));
            this.Graph.PaddingX = 2F;
            this.Graph.PaddingY = 4F;
            this.Graph.PerItemScales = ((System.Collections.Generic.List<float>)(resources.GetObject("Graph.PerItemScales")));
            this.Graph.RulerPos = Mids_Reborn.Controls.CtlMultiGraph.RulerPosition.Top;
            this.Graph.ScaleHeight = 16;
            this.Graph.ScaleIndex = 7;
            this.Graph.ShowScale = false;
            this.Graph.Size = new System.Drawing.Size(484, 398);
            this.Graph.Style = Mids_Reborn.Core.Enums.GraphStyle.Stacked;
            this.Graph.TabIndex = 0;
            this.Graph.TextWidth = 100;
            // 
            // chkOnTop
            // 
            this.chkOnTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.chkOnTop.ButtonType = Mids_Reborn.Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            this.chkOnTop.CurrentText = "To Top Most";
            this.chkOnTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkOnTop.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.chkOnTop.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.chkOnTop.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.chkOnTop.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.chkOnTop.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.chkOnTop.Location = new System.Drawing.Point(383, 438);
            this.chkOnTop.Lock = false;
            this.chkOnTop.Name = "chkOnTop";
            this.chkOnTop.Size = new System.Drawing.Size(105, 22);
            this.chkOnTop.TabIndex = 13;
            this.chkOnTop.Text = "imageButtonEx1";
            this.chkOnTop.TextOutline.Color = System.Drawing.Color.Black;
            this.chkOnTop.TextOutline.Width = 2;
            this.chkOnTop.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.chkOnTop.ToggleText.Indeterminate = "Indeterminate State";
            this.chkOnTop.ToggleText.ToggledOff = "To Top Most";
            this.chkOnTop.ToggleText.ToggledOn = "Top Most";
            this.chkOnTop.UseAlt = false;
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.CurrentText = "Close";
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnClose.Images.Background = global::MRBResourceLib.Resources.HeroButton;
            this.btnClose.Images.Hover = global::MRBResourceLib.Resources.HeroButtonHover;
            this.btnClose.ImagesAlt.Background = global::MRBResourceLib.Resources.VillainButton;
            this.btnClose.ImagesAlt.Hover = global::MRBResourceLib.Resources.VillainButtonHover;
            this.btnClose.Location = new System.Drawing.Point(383, 465);
            this.btnClose.Lock = false;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 22);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.TextOutline.Color = System.Drawing.Color.Black;
            this.btnClose.TextOutline.Width = 2;
            this.btnClose.ToggleState = Mids_Reborn.Forms.Controls.ImageButtonEx.States.ToggledOff;
            this.btnClose.ToggleText.Indeterminate = "Indeterminate State";
            this.btnClose.ToggleText.ToggledOff = "ToggledOff State";
            this.btnClose.ToggleText.ToggledOn = "ToggledOn State";
            this.btnClose.UseAlt = false;
            // 
            // frmStats
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(492, 491);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkOnTop);
            this.Controls.Add(this.cbStyle);
            this.Controls.Add(this.lblKey2);
            this.Controls.Add(this.cbValues);
            this.Controls.Add(this.lblKey1);
            this.Controls.Add(this.cbSet);
            this.Controls.Add(this.lblKeyColor2);
            this.Controls.Add(this.lblKeyColor1);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.tbScaleX);
            this.Controls.Add(this.Graph);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 340);
            this.Name = "frmStats";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Power Stats";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.tbScaleX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Controls.ImageButtonEx chkOnTop;
        private Controls.ImageButtonEx btnClose;
    }
}