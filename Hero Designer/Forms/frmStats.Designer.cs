using System.ComponentModel;
using mrbBase;
using mrbControls;

namespace Mids_Reborn.Forms
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
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

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
            this.Graph = new ctlMultiGraph();
            this.chkOnTop = new ImageButton();
            this.btnClose = new ImageButton();
            this.tbScaleX.BeginInit();
            this.SuspendLayout();
            this.lblKey2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;

            this.lblKey2.Location = new System.Drawing.Point(56, 463);
            this.lblKey2.Name = "lblKey2";

            this.lblKey2.Size = new System.Drawing.Size(78, 16);
            this.lblKey2.TabIndex = 3;
            this.lblKey2.Text = "Enhanced";
            this.lblKey2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKey1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;

            this.lblKey1.Location = new System.Drawing.Point(56, 443);
            this.lblKey1.Name = "lblKey1";

            this.lblKey1.Size = new System.Drawing.Size(78, 16);
            this.lblKey1.TabIndex = 2;
            this.lblKey1.Text = "Base";
            this.lblKey1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKeyColor2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyColor2.BackColor = System.Drawing.Color.Yellow;
            this.lblKeyColor2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblKeyColor2.Location = new System.Drawing.Point(12, 463);
            this.lblKeyColor2.Name = "lblKeyColor2";

            this.lblKeyColor2.Size = new System.Drawing.Size(40, 16);
            this.lblKeyColor2.TabIndex = 1;
            this.lblKeyColor1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyColor1.BackColor = System.Drawing.Color.Blue;
            this.lblKeyColor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblKeyColor1.Location = new System.Drawing.Point(12, 443);
            this.lblKeyColor1.Name = "lblKeyColor1";

            this.lblKeyColor1.Size = new System.Drawing.Size(40, 16);
            this.lblKeyColor1.TabIndex = 0;
            this.tbScaleX.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.tbScaleX.LargeChange = 1;

            this.tbScaleX.Location = new System.Drawing.Point(140, 438);
            this.tbScaleX.Minimum = 1;
            this.tbScaleX.Name = "tbScaleX";

            this.tbScaleX.Size = new System.Drawing.Size(237, 45);
            this.tbScaleX.TabIndex = 6;
            this.tbScaleX.TickFrequency = 10;
            this.tbScaleX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbScaleX.Scroll += new System.EventHandler(tbScaleX_Scroll);
            this.tTip.SetToolTip((System.Windows.Forms.Control)this.tbScaleX, "Move the slider to the left to zoom in on lower values.");
            this.tbScaleX.Value = 10;
            this.lblScale.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;

            this.lblScale.Location = new System.Drawing.Point(212, 462);
            this.lblScale.Name = "lblScale";

            this.lblScale.Size = new System.Drawing.Size(108, 20);
            this.lblScale.TabIndex = 7;
            this.lblScale.Text = "Scale: 100%";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tTip.AutoPopDelay = 10000;
            this.tTip.InitialDelay = 500;
            this.tTip.ReshowDelay = 100;
            this.cbSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSet.FormattingEnabled = true;

            this.cbSet.Location = new System.Drawing.Point(6, 5);
            this.cbSet.MaxDropDownItems = 16;
            this.cbSet.Name = "cbSet";

            this.cbSet.Size = new System.Drawing.Size(158, 21);
            this.cbSet.TabIndex = 10;
            this.cbSet.SelectedIndexChanged += new System.EventHandler(cbSet_SelectedIndexChanged);
            this.cbValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbValues.FormattingEnabled = true;

            this.cbValues.Location = new System.Drawing.Point(170, 5);
            this.cbValues.MaxDropDownItems = 16;
            this.cbValues.Name = "cbValues";

            this.cbValues.Size = new System.Drawing.Size(101, 21);
            this.cbValues.TabIndex = 11;
            this.cbValues.SelectedIndexChanged += new System.EventHandler(cbValues_SelectedIndexChanged);
            this.cbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStyle.FormattingEnabled = true;

            this.cbStyle.Location = new System.Drawing.Point(277, 5);
            this.cbStyle.Name = "cbStyle";

            this.cbStyle.Size = new System.Drawing.Size(154, 21);
            this.cbStyle.TabIndex = 12;
            this.cbStyle.SelectedIndexChanged += new System.EventHandler(cbStyle_SelectedIndexChanged);
            this.Graph.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.Graph.Border = true;
            this.Graph.ColorBase = System.Drawing.Color.Blue;
            this.Graph.ColorEnh = System.Drawing.Color.Yellow;
            this.Graph.ColorFadeEnd = System.Drawing.Color.Red;
            this.Graph.ColorFadeStart = System.Drawing.Color.Black;
            this.Graph.ColorHighlight = System.Drawing.Color.White;
            this.Graph.ColorLines = System.Drawing.Color.Black;
            this.Graph.ColorMarkerInner = System.Drawing.Color.Black;
            this.Graph.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.Graph.Dual = false;
            this.Graph.Font = new System.Drawing.Font("Arial", 7.5f);
            this.Graph.ForeColor = System.Drawing.Color.FromArgb(192, 192, (int)byte.MaxValue);
            this.Graph.Highlight = true;
            this.Graph.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Graph.ItemHeight = 12;
            this.Graph.Lines = true;

            this.Graph.Location = new System.Drawing.Point(4, 28);
            this.Graph.MarkerValue = 0.0f;
            this.Graph.Max = 75f;
            this.Graph.Name = "Graph";
            this.Graph.PaddingX = 2f;
            this.Graph.PaddingY = 4f;
            this.Graph.ScaleHeight = 16;
            this.Graph.ScaleIndex = 7;
            this.Graph.ShowScale = true;

            this.Graph.Size = new System.Drawing.Size(484, 405);
            this.Graph.Style = Enums.GraphStyle.Stacked;
            this.Graph.TabIndex = 0;
            this.Graph.TextWidth = 100;
            this.chkOnTop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.chkOnTop.Checked = true;
            this.chkOnTop.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.chkOnTop.Location = new System.Drawing.Point(383, 438);
            this.chkOnTop.Name = "chkOnTop";

            this.chkOnTop.Size = new System.Drawing.Size(105, 22);
            this.chkOnTop.TabIndex = 17;
            this.chkOnTop.TextOff = "Keep On Top";
            this.chkOnTop.TextOn = "Keep On Top";
            this.chkOnTop.Toggle = true;
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Checked = false;
            this.btnClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.btnClose.Location = new System.Drawing.Point(383, 465);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";

            this.btnClose.Size = new System.Drawing.Size(105, 22);
            this.btnClose.TabIndex = 16;
            this.btnClose.TextOff = "Close";
            this.btnClose.TextOn = "Close";
            this.btnClose.Toggle = false;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(492, 491);
            this.Controls.Add((System.Windows.Forms.Control)this.chkOnTop);
            this.Controls.Add((System.Windows.Forms.Control)this.btnClose);
            this.Controls.Add((System.Windows.Forms.Control)this.cbStyle);
            this.Controls.Add((System.Windows.Forms.Control)this.lblKey2);
            this.Controls.Add((System.Windows.Forms.Control)this.cbValues);
            this.Controls.Add((System.Windows.Forms.Control)this.lblKey1);
            this.Controls.Add((System.Windows.Forms.Control)this.cbSet);
            this.Controls.Add((System.Windows.Forms.Control)this.lblKeyColor2);
            this.Controls.Add((System.Windows.Forms.Control)this.lblKeyColor1);
            this.Controls.Add((System.Windows.Forms.Control)this.lblScale);
            this.Controls.Add((System.Windows.Forms.Control)this.tbScaleX);
            this.Controls.Add((System.Windows.Forms.Control)this.Graph);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.MinimumSize = new System.Drawing.Size(400, 340);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Power Stats";
            this.TopMost = true;
            this.tbScaleX.EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
    }
}