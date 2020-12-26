
using System.ComponentModel;
using mrbBase;
using mrbControls;

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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.lblKeyColor1 = new System.Windows.Forms.Label();
            this.cbSet1 = new System.Windows.Forms.ComboBox();
            this.cbType1 = new System.Windows.Forms.ComboBox();
            this.cbAT1 = new System.Windows.Forms.ComboBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.lblKeyColor2 = new System.Windows.Forms.Label();
            this.cbSet2 = new System.Windows.Forms.ComboBox();
            this.cbType2 = new System.Windows.Forms.ComboBox();
            this.cbAT2 = new System.Windows.Forms.ComboBox();
            this.lblScale = new System.Windows.Forms.Label();
            this.tbScaleX = new System.Windows.Forms.TrackBar();
            this.chkMatching = new System.Windows.Forms.CheckBox();
            this.tTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnTweakMatch = new System.Windows.Forms.Button();
            this.chkOnTop = new ImageButton();
            this.btnClose = new ImageButton();
            this.Graph = new ctlMultiGraph();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.lstDisplay = new System.Windows.Forms.ListBox();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.tbScaleX.BeginInit();
            this.GroupBox4.SuspendLayout();
            this.SuspendLayout();
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lblKeyColor1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbSet1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbType1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.cbAT1);
            this.GroupBox1.ForeColor = System.Drawing.Color.White;

            this.GroupBox1.Location = new System.Drawing.Point(4, 4);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(144, 116);
            this.GroupBox1.TabIndex = 2;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Powerset 1:";
            this.lblKeyColor1.BackColor = System.Drawing.Color.Blue;
            this.lblKeyColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblKeyColor1.Location = new System.Drawing.Point(8, 20);
            this.lblKeyColor1.Name = "lblKeyColor1";

            this.lblKeyColor1.Size = new System.Drawing.Size(132, 16);
            this.lblKeyColor1.TabIndex = 3;
            this.cbSet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbSet1.Location = new System.Drawing.Point(8, 88);
            this.cbSet1.Name = "cbSet1";

            this.cbSet1.Size = new System.Drawing.Size(132, 22);
            this.cbSet1.TabIndex = 2;
            this.cbSet1.SelectedIndexChanged += new System.EventHandler(cbSet1_SelectedIndexChanged);
            this.cbType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbType1.Location = new System.Drawing.Point(8, 64);
            this.cbType1.Name = "cbType1";

            this.cbType1.Size = new System.Drawing.Size(132, 22);
            this.cbType1.TabIndex = 1;
            this.cbType1.SelectedIndexChanged += new System.EventHandler(cbType1_SelectedIndexChanged);
            this.cbAT1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbAT1.Location = new System.Drawing.Point(8, 40);
            this.cbAT1.Name = "cbAT1";

            this.cbAT1.Size = new System.Drawing.Size(132, 22);
            this.cbAT1.TabIndex = 0;
            this.cbAT1.SelectedIndexChanged += new System.EventHandler(cbAT1_SelectedIndexChanged);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lblKeyColor2);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbSet2);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbType2);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbAT2);
            this.GroupBox2.ForeColor = System.Drawing.Color.White;

            this.GroupBox2.Location = new System.Drawing.Point(4, 126);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(144, 116);
            this.GroupBox2.TabIndex = 3;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Powerset 2:";
            this.lblKeyColor2.BackColor = System.Drawing.Color.Yellow;
            this.lblKeyColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblKeyColor2.Location = new System.Drawing.Point(8, 20);
            this.lblKeyColor2.Name = "lblKeyColor2";

            this.lblKeyColor2.Size = new System.Drawing.Size(132, 16);
            this.lblKeyColor2.TabIndex = 3;
            this.cbSet2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbSet2.Location = new System.Drawing.Point(8, 88);
            this.cbSet2.Name = "cbSet2";

            this.cbSet2.Size = new System.Drawing.Size(132, 22);
            this.cbSet2.TabIndex = 2;
            this.cbSet2.SelectedIndexChanged += new System.EventHandler(cbSet2_SelectedIndexChanged);
            this.cbType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbType2.Location = new System.Drawing.Point(8, 64);
            this.cbType2.Name = "cbType2";

            this.cbType2.Size = new System.Drawing.Size(132, 22);
            this.cbType2.TabIndex = 1;
            this.cbType2.SelectedIndexChanged += new System.EventHandler(cbType2_SelectedIndexChanged);
            this.cbAT2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbAT2.Location = new System.Drawing.Point(8, 40);
            this.cbAT2.Name = "cbAT2";

            this.cbAT2.Size = new System.Drawing.Size(132, 22);
            this.cbAT2.TabIndex = 0;
            this.cbAT2.SelectedIndexChanged += new System.EventHandler(cbAT2_SelectedIndexChanged);

            this.lblScale.Location = new System.Drawing.Point(312, 500);
            this.lblScale.Name = "lblScale";

            this.lblScale.Size = new System.Drawing.Size(108, 20);
            this.lblScale.TabIndex = 9;
            this.lblScale.Text = "Scale: 100%";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tbScaleX.LargeChange = 1;

            this.tbScaleX.Location = new System.Drawing.Point(184, 476);
            this.tbScaleX.Minimum = 1;
            this.tbScaleX.Name = "tbScaleX";

            this.tbScaleX.Size = new System.Drawing.Size(340, 45);
            this.tbScaleX.TabIndex = 8;
            this.tbScaleX.TickFrequency = 10;
            this.tbScaleX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbScaleX.Value = 10;
            this.tbScaleX.Scroll += new System.EventHandler(tbScaleX_Scroll);

            this.chkMatching.Location = new System.Drawing.Point(8, 508);
            this.chkMatching.Name = "chkMatching";

            this.chkMatching.Size = new System.Drawing.Size(116, 20);
            this.chkMatching.TabIndex = 11;
            this.chkMatching.Text = "Attempt Matching";
            this.chkMatching.CheckedChanged += new System.EventHandler(chkMatching_CheckedChanged);
            this.tTip.AutoPopDelay = 10000;
            this.tTip.InitialDelay = 500;
            this.tTip.ReshowDelay = 100;
            this.btnTweakMatch.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.btnTweakMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.btnTweakMatch.ForeColor = System.Drawing.Color.Black;

            this.btnTweakMatch.Location = new System.Drawing.Point(130, 508);
            this.btnTweakMatch.Name = "btnTweakMatch";

            this.btnTweakMatch.Size = new System.Drawing.Size(60, 20);
            this.btnTweakMatch.TabIndex = 12;
            this.btnTweakMatch.Text = "Tweak";
            this.tTip.SetToolTip((System.Windows.Forms.Control)this.btnTweakMatch, "Modify the data used to perform power matching");
            this.btnTweakMatch.UseVisualStyleBackColor = true;
            this.btnTweakMatch.Visible = false;
            this.btnTweakMatch.Click += new System.EventHandler(btnTweakMatch_Click);
            this.chkOnTop.Checked = true;
            this.chkOnTop.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.chkOnTop.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.chkOnTop.Location = new System.Drawing.Point(530, 476);
            this.chkOnTop.Name = "chkOnTop";

            this.chkOnTop.Size = new System.Drawing.Size(105, 22);
            this.chkOnTop.TabIndex = 15;
            this.chkOnTop.TextOff = "Keep On Top";
            this.chkOnTop.TextOn = "Keep On Top";
            this.chkOnTop.Toggle = true;
            this.chkOnTop.ButtonClicked += new ImageButton.ButtonClickedEventHandler(chkOnTop_CheckedChanged);
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Checked = false;
            this.btnClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.btnClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);

            this.btnClose.Location = new System.Drawing.Point(530, 504);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";

            this.btnClose.Size = new System.Drawing.Size(105, 22);
            this.btnClose.TabIndex = 14;
            this.btnClose.TextOff = "Close";
            this.btnClose.TextOn = "Close";
            this.btnClose.Toggle = false;
            this.btnClose.ButtonClicked += new ImageButton.ButtonClickedEventHandler(btnClose_ButtonClicked);
            this.Graph.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.Graph.Border = true;
            this.Graph.Clickable = false;
            this.Graph.ColorBase = System.Drawing.Color.Blue;
            this.Graph.ColorEnh = System.Drawing.Color.Yellow;
            this.Graph.ColorFadeEnd = System.Drawing.Color.Red;
            this.Graph.ColorFadeStart = System.Drawing.Color.Black;
            this.Graph.ColorHighlight = System.Drawing.Color.White;
            this.Graph.ColorLines = System.Drawing.Color.Black;
            this.Graph.ColorMarkerInner = System.Drawing.Color.Black;
            this.Graph.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.Graph.Dual = true;
            this.Graph.Font = new System.Drawing.Font("Arial", 7.5f);
            this.Graph.ForcedMax = 0.0f;
            this.Graph.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);
            this.Graph.Highlight = true;
            this.Graph.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Graph.ItemHeight = 26;
            this.Graph.Lines = true;

            this.Graph.Location = new System.Drawing.Point(152, 4);
            this.Graph.MarkerValue = 0.0f;
            this.Graph.Max = 100f;
            this.Graph.Name = "Graph";
            this.Graph.PaddingX = 2f;
            this.Graph.PaddingY = 6f;
            this.Graph.ScaleHeight = 16;
            this.Graph.ScaleIndex = 8;
            this.Graph.ShowScale = true;

            this.Graph.Size = new System.Drawing.Size(484, 468);
            this.Graph.Style = Enums.GraphStyle.Twin;
            this.Graph.TabIndex = 1;
            this.Graph.TextWidth = 120;
            this.Graph.Load += new System.EventHandler(Graph_Load);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.lstDisplay);
            this.GroupBox4.ForeColor = System.Drawing.Color.White;

            this.GroupBox4.Location = new System.Drawing.Point(8, 248);
            this.GroupBox4.Name = "GroupBox4";

            this.GroupBox4.Size = new System.Drawing.Size(144, 254);
            this.GroupBox4.TabIndex = 16;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Display:";
            this.lstDisplay.FormattingEnabled = true;
            this.lstDisplay.ItemHeight = 14;

            this.lstDisplay.Location = new System.Drawing.Point(6, 19);
            this.lstDisplay.Name = "lstDisplay";

            this.lstDisplay.Size = new System.Drawing.Size(130, 228);
            this.lstDisplay.TabIndex = 0;
            this.lstDisplay.SelectedIndexChanged += new System.EventHandler(lstDisplay_SelectedIndexChanged);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(640, 532);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox4);
            this.Controls.Add((System.Windows.Forms.Control)this.chkOnTop);
            this.Controls.Add((System.Windows.Forms.Control)this.btnClose);
            this.Controls.Add((System.Windows.Forms.Control)this.btnTweakMatch);
            this.Controls.Add((System.Windows.Forms.Control)this.chkMatching);
            this.Controls.Add((System.Windows.Forms.Control)this.lblScale);
            this.Controls.Add((System.Windows.Forms.Control)this.tbScaleX);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.Graph);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Powerset Comparison";
            this.TopMost = true;
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.tbScaleX.EndInit();
            this.GroupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    #endregion
    }


}