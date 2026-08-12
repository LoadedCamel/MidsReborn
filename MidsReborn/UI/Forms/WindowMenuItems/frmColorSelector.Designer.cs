namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    partial class frmColorSelector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            borderPanel1 = new Mids_Reborn.UI.Forms.Controls.BorderPanel();
            btnResetColor = new Button();
            pnlColorPreview = new Mids_Reborn.UI.Forms.Controls.BorderPanel();
            btnCancel = new Button();
            btnOk = new Button();
            trkB = new TrackBar();
            nudB = new NumericUpDown();
            trkG = new TrackBar();
            nudG = new NumericUpDown();
            trkR = new TrackBar();
            nudR = new NumericUpDown();
            colorWheel1 = new Mids_Reborn.UI.Forms.Controls.ColorWheel();
            borderPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkG).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudG).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudR).BeginInit();
            SuspendLayout();
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = Color.FromArgb(224, 224, 224);
            borderPanel1.Border.Style = ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(btnResetColor);
            borderPanel1.Controls.Add(pnlColorPreview);
            borderPanel1.Controls.Add(btnCancel);
            borderPanel1.Controls.Add(btnOk);
            borderPanel1.Controls.Add(trkB);
            borderPanel1.Controls.Add(nudB);
            borderPanel1.Controls.Add(trkG);
            borderPanel1.Controls.Add(nudG);
            borderPanel1.Controls.Add(trkR);
            borderPanel1.Controls.Add(nudR);
            borderPanel1.Controls.Add(colorWheel1);
            borderPanel1.Dock = DockStyle.Fill;
            borderPanel1.Location = new Point(0, 0);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new Size(604, 334);
            borderPanel1.TabIndex = 0;
            // 
            // btnResetColor
            // 
            btnResetColor.Location = new Point(286, 299);
            btnResetColor.Name = "btnResetColor";
            btnResetColor.Size = new Size(75, 23);
            btnResetColor.TabIndex = 10;
            btnResetColor.Text = "Reset";
            btnResetColor.UseVisualStyleBackColor = true;
            btnResetColor.Click += btnResetColor_Click;
            // 
            // pnlColorPreview
            // 
            pnlColorPreview.Border.Color = Color.FromArgb(160, 160, 160);
            pnlColorPreview.Border.Style = ButtonBorderStyle.Solid;
            pnlColorPreview.Border.Thickness = 1;
            pnlColorPreview.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            pnlColorPreview.Location = new Point(427, 188);
            pnlColorPreview.Name = "pnlColorPreview";
            pnlColorPreview.Size = new Size(77, 40);
            pnlColorPreview.TabIndex = 9;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(519, 299);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(427, 299);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 7;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // trkB
            // 
            trkB.LargeChange = 32;
            trkB.Location = new Point(361, 137);
            trkB.Maximum = 255;
            trkB.Name = "trkB";
            trkB.Size = new Size(143, 45);
            trkB.TabIndex = 6;
            trkB.TickFrequency = 32;
            trkB.TickStyle = TickStyle.None;
            trkB.Scroll += trkB_Scroll;
            // 
            // nudB
            // 
            nudB.Location = new Point(510, 137);
            nudB.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudB.Name = "nudB";
            nudB.Size = new Size(62, 23);
            nudB.TabIndex = 5;
            nudB.TextAlign = HorizontalAlignment.Right;
            nudB.ValueChanged += nudB_ValueChanged;
            // 
            // trkG
            // 
            trkG.LargeChange = 32;
            trkG.Location = new Point(361, 86);
            trkG.Maximum = 255;
            trkG.Name = "trkG";
            trkG.Size = new Size(143, 45);
            trkG.TabIndex = 4;
            trkG.TickFrequency = 32;
            trkG.TickStyle = TickStyle.None;
            trkG.Scroll += trkG_Scroll;
            // 
            // nudG
            // 
            nudG.Location = new Point(510, 86);
            nudG.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudG.Name = "nudG";
            nudG.Size = new Size(62, 23);
            nudG.TabIndex = 3;
            nudG.TextAlign = HorizontalAlignment.Right;
            nudG.ValueChanged += nudG_ValueChanged;
            // 
            // trkR
            // 
            trkR.LargeChange = 32;
            trkR.Location = new Point(361, 35);
            trkR.Maximum = 255;
            trkR.Name = "trkR";
            trkR.Size = new Size(143, 45);
            trkR.TabIndex = 2;
            trkR.TickFrequency = 32;
            trkR.TickStyle = TickStyle.None;
            trkR.Scroll += trkR_Scroll;
            // 
            // nudR
            // 
            nudR.Location = new Point(510, 35);
            nudR.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudR.Name = "nudR";
            nudR.Size = new Size(62, 23);
            nudR.TabIndex = 1;
            nudR.TextAlign = HorizontalAlignment.Right;
            nudR.ValueChanged += nudR_ValueChanged;
            // 
            // colorWheel1
            // 
            colorWheel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            colorWheel1.BackColor = Color.Black;
            colorWheel1.Location = new Point(12, 18);
            colorWheel1.Name = "colorWheel1";
            colorWheel1.Size = new Size(306, 306);
            colorWheel1.TabIndex = 0;
            colorWheel1.SelectionChanged += colorWheel1_SelectionChanged;
            // 
            // frmColorSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            CancelButton = btnCancel;
            ClientSize = new Size(604, 334);
            Controls.Add(borderPanel1);
            DoubleBuffered = true;
            ForeColor = Color.Gainsboro;
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmColorSelector";
            Text = "frmColorSelector";
            Load += frmColorSelector_Load;
            borderPanel1.ResumeLayout(false);
            borderPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkB).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudB).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkG).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudG).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkR).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudR).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Controls.BorderPanel borderPanel1;
        private TrackBar trkB;
        private NumericUpDown nudB;
        private TrackBar trkG;
        private NumericUpDown nudG;
        private TrackBar trkR;
        private NumericUpDown nudR;
        private Controls.ColorWheel colorWheel1;
        private Button btnCancel;
        private Button btnOk;
        private Controls.BorderPanel pnlColorPreview;
        private Button btnResetColor;
    }
}