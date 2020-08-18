using System.ComponentModel;

namespace Hero_Designer.Forms
{
    public partial class frmReadme
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
            this.rtfRead = new System.Windows.Forms.RichTextBox();
            this.pbBackground = new System.Windows.Forms.PictureBox();
            this.pbBottom = new System.Windows.Forms.PictureBox();
            this.BtnClose = new midsControls.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // rtfRead
            // 
            this.rtfRead.BackColor = System.Drawing.Color.White;
            this.rtfRead.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtfRead.ForeColor = System.Drawing.Color.Black;
            this.rtfRead.Location = new System.Drawing.Point(12, 31);
            this.rtfRead.Name = "rtfRead";
            this.rtfRead.ReadOnly = true;
            this.rtfRead.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtfRead.Size = new System.Drawing.Size(616, 409);
            this.rtfRead.TabIndex = 0;
            this.rtfRead.Text = "";
            // 
            // pbBackground
            // 
            this.pbBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbBackground.Location = new System.Drawing.Point(0, 0);
            this.pbBackground.Name = "pbBackground";
            this.pbBackground.Size = new System.Drawing.Size(642, 41);
            this.pbBackground.TabIndex = 101;
            this.pbBackground.TabStop = false;
            this.pbBackground.Click += new System.EventHandler(this.pbBackground_Click);
            this.pbBackground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbBackground_MouseDown);
            this.pbBackground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbBackground_MouseMove);
            // 
            // pbBottom
            // 
            this.pbBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbBottom.Location = new System.Drawing.Point(0, 427);
            this.pbBottom.Name = "pbBottom";
            this.pbBottom.Size = new System.Drawing.Size(642, 53);
            this.pbBottom.TabIndex = 102;
            this.pbBottom.TabStop = false;
            this.pbBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbBottom_MouseDown);
            this.pbBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbBottom_MouseMove);
            // 
            // btnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(143)))), ((int)(((byte)(233)))));
            this.BtnClose.Checked = false;
            this.BtnClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.BtnClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.BtnClose.Location = new System.Drawing.Point(268, 449);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(105, 22);
            this.BtnClose.TabIndex = 100;
            this.BtnClose.TextOff = "Close";
            this.BtnClose.TextOn = "Alt Text";
            this.BtnClose.Toggle = false;
            this.BtnClose.ButtonClicked += new midsControls.ImageButton.ButtonClickedEventHandler(this.btnClose_ButtonClicked);
            this.BtnClose.Load += new System.EventHandler(this.btnClose_Load);
            // 
            // frmReadme
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(642, 480);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.rtfRead);
            this.Controls.Add(this.pbBottom);
            this.Controls.Add(this.pbBackground);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "frmReadme";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Readme / Help";
            this.Load += new System.EventHandler(this.frmReadme_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmReadme_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmReadme_MouseMove);
            this.Resize += new System.EventHandler(this.frmReadme_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBottom)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}