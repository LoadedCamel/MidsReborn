using System.ComponentModel;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmSetViewer
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
            this.lstSets = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ilSet = new System.Windows.Forms.ImageList(this.components);
            this.rtxtInfo = new System.Windows.Forms.RichTextBox();
            this.rtxtFX = new System.Windows.Forms.RichTextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.rtApplied = new System.Windows.Forms.RichTextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.chkOnTop = new ImageButton();
            this.btnClose = new ImageButton();
            this.btnSmall = new ImageButton();
            this.SuspendLayout();
            this.lstSets.BackColor = System.Drawing.Color.White;
            this.lstSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3]
            {
        this.ColumnHeader1,
        this.ColumnHeader2,
        this.ColumnHeader3
            });
            this.lstSets.ForeColor = System.Drawing.Color.Black;
            this.lstSets.FullRowSelect = true;
            this.lstSets.HideSelection = false;
            this.lstSets.LargeImageList = this.ilSet;

            this.lstSets.Location = new System.Drawing.Point(12, 168);
            this.lstSets.MultiSelect = false;
            this.lstSets.Name = "lstSets";

            this.lstSets.Size = new System.Drawing.Size(360, 136);
            this.lstSets.SmallImageList = this.ilSet;
            this.lstSets.TabIndex = 0;
            this.lstSets.UseCompatibleStateImageBehavior = false;
            this.lstSets.View = System.Windows.Forms.View.Details;
            this.lstSets.SelectedIndexChanged += new System.EventHandler(lstSets_SelectedIndexChanged);
            this.ColumnHeader1.Text = "Set";
            this.ColumnHeader1.Width = 148;
            this.ColumnHeader2.Text = "Power";
            this.ColumnHeader2.Width = 124;
            this.ColumnHeader3.Text = "Slots";
            this.ilSet.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;

            this.ilSet.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSet.TransparentColor = System.Drawing.Color.Transparent;
            this.rtxtInfo.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.rtxtInfo.ForeColor = System.Drawing.Color.White;

            this.rtxtInfo.Location = new System.Drawing.Point(12, 308);
            this.rtxtInfo.Name = "rtxtInfo";
            this.rtxtInfo.ReadOnly = true;
            this.rtxtInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.rtxtInfo.Size = new System.Drawing.Size(360, 132);
            this.rtxtInfo.TabIndex = 1;
            this.rtxtInfo.Text = "";
            this.rtxtFX.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.rtxtFX.ForeColor = System.Drawing.Color.White;

            this.rtxtFX.Location = new System.Drawing.Point(384, 20);
            this.rtxtFX.Name = "rtxtFX";
            this.rtxtFX.ReadOnly = true;
            this.rtxtFX.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.rtxtFX.Size = new System.Drawing.Size(279, 366);
            this.rtxtFX.TabIndex = 3;
            this.rtxtFX.Text = "";
            this.Label1.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.Label1.ForeColor = System.Drawing.Color.White;

            this.Label1.Location = new System.Drawing.Point(384, 4);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(188, 16);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Effect Breakdown:";
            this.rtApplied.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.rtApplied.ForeColor = System.Drawing.Color.White;

            this.rtApplied.Location = new System.Drawing.Point(12, 20);
            this.rtApplied.Name = "rtApplied";
            this.rtApplied.ReadOnly = true;
            this.rtApplied.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.rtApplied.Size = new System.Drawing.Size(360, 140);
            this.rtApplied.TabIndex = 5;
            this.rtApplied.Text = "";
            this.Label2.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.Label2.ForeColor = System.Drawing.Color.White;

            this.Label2.Location = new System.Drawing.Point(12, 4);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(188, 16);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "Applied Bonus Effects:";
            this.chkOnTop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.chkOnTop.Checked = true;
            this.chkOnTop.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.chkOnTop.Location = new System.Drawing.Point(558, 392);
            this.chkOnTop.Name = "chkOnTop";

            this.chkOnTop.Size = new System.Drawing.Size(105, 22);
            this.chkOnTop.TabIndex = 19;
            this.chkOnTop.TextOff = "Keep On Top";
            this.chkOnTop.TextOn = "Keep On Top";
            this.chkOnTop.Toggle = true;
            this.chkOnTop.ButtonClicked += new ImageButton.ButtonClickedEventHandler(chkOnTop_CheckedChanged);

            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Checked = false;
            this.btnClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.btnClose.Location = new System.Drawing.Point(558, 418);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";

            this.btnClose.Size = new System.Drawing.Size(105, 22);
            this.btnClose.TabIndex = 18;
            this.btnClose.TextOff = "Close";
            this.btnClose.TextOn = "Close";
            this.btnClose.Toggle = false;
            this.btnClose.ButtonClicked += new ImageButton.ButtonClickedEventHandler(btnClose_Click);
            this.btnSmall.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnSmall.Checked = false;
            this.btnSmall.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.btnSmall.Location = new System.Drawing.Point(384, 418);
            this.btnSmall.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSmall.Name = "btnSmall";

            this.btnSmall.Size = new System.Drawing.Size(105, 22);
            this.btnSmall.TabIndex = 20;
            this.btnSmall.TextOff = "<< Shrink";
            this.btnSmall.TextOn = ">>";
            this.btnSmall.Toggle = false;
            this.btnSmall.ButtonClicked += new ImageButton.ButtonClickedEventHandler(btnSmall_Click);

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(675, 448);
            this.Controls.Add((System.Windows.Forms.Control)this.btnSmall);
            this.Controls.Add((System.Windows.Forms.Control)this.chkOnTop);
            this.Controls.Add((System.Windows.Forms.Control)this.btnClose);
            this.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.Controls.Add((System.Windows.Forms.Control)this.rtApplied);
            this.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.Controls.Add((System.Windows.Forms.Control)this.rtxtFX);
            this.Controls.Add((System.Windows.Forms.Control)this.rtxtInfo);
            this.Controls.Add((System.Windows.Forms.Control)this.lstSets);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Currently Active Sets & Bonuses";
            this.TopMost = true;

            this.ResumeLayout(false);
        }
        #endregion
    }
}