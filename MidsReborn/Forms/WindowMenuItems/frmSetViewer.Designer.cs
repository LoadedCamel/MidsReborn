using System.ComponentModel;
using Mids_Reborn.Controls;

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
            this.components = new System.ComponentModel.Container();
            this.lstSets = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilSet = new System.Windows.Forms.ImageList(this.components);
            this.rtxtInfo = new System.Windows.Forms.RichTextBox();
            this.rtxtFX = new System.Windows.Forms.RichTextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.rtApplied = new System.Windows.Forms.RichTextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.panelBars = new System.Windows.Forms.Panel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSmall = new ImageButton();
            this.btnDetailFx = new ImageButton();
            this.chkOnTop = new ImageButton();
            this.btnClose = new ImageButton();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstSets
            // 
            this.lstSets.BackColor = System.Drawing.SystemColors.Window;
            this.lstSets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2,
            this.ColumnHeader3});
            this.lstSets.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstSets.FullRowSelect = true;
            this.lstSets.HideSelection = false;
            this.lstSets.LargeImageList = this.ilSet;
            this.lstSets.Location = new System.Drawing.Point(13, 200);
            this.lstSets.MultiSelect = false;
            this.lstSets.Name = "lstSets";
            this.lstSets.Size = new System.Drawing.Size(607, 136);
            this.lstSets.SmallImageList = this.ilSet;
            this.lstSets.TabIndex = 0;
            this.lstSets.UseCompatibleStateImageBehavior = false;
            this.lstSets.View = System.Windows.Forms.View.Details;
            this.lstSets.SelectedIndexChanged += new System.EventHandler(this.lstSets_SelectedIndexChanged);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Set";
            this.ColumnHeader1.Width = 268;
            // 
            // ColumnHeader2
            // 
            this.ColumnHeader2.Text = "Power";
            this.ColumnHeader2.Width = 244;
            // 
            // ColumnHeader3
            // 
            this.ColumnHeader3.Text = "Slots";
            // 
            // ilSet
            // 
            this.ilSet.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilSet.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSet.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // rtxtInfo
            // 
            this.rtxtInfo.BackColor = System.Drawing.Color.Black;
            this.rtxtInfo.ForeColor = System.Drawing.Color.White;
            this.rtxtInfo.Location = new System.Drawing.Point(13, 340);
            this.rtxtInfo.Name = "rtxtInfo";
            this.rtxtInfo.ReadOnly = true;
            this.rtxtInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtInfo.Size = new System.Drawing.Size(607, 132);
            this.rtxtInfo.TabIndex = 1;
            this.rtxtInfo.Text = "";
            // 
            // rtxtFX
            // 
            this.rtxtFX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.rtxtFX.ForeColor = System.Drawing.Color.White;
            this.rtxtFX.Location = new System.Drawing.Point(626, 28);
            this.rtxtFX.Name = "rtxtFX";
            this.rtxtFX.ReadOnly = true;
            this.rtxtFX.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtxtFX.Size = new System.Drawing.Size(329, 388);
            this.rtxtFX.TabIndex = 3;
            this.rtxtFX.Text = "";
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.Color.White;
            this.Label1.Location = new System.Drawing.Point(625, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(188, 16);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Effect Breakdown:";
            // 
            // rtApplied
            // 
            this.rtApplied.BackColor = System.Drawing.Color.Black;
            this.rtApplied.ForeColor = System.Drawing.Color.White;
            this.rtApplied.Location = new System.Drawing.Point(12, 20);
            this.rtApplied.Name = "rtApplied";
            this.rtApplied.ReadOnly = true;
            this.rtApplied.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtApplied.Size = new System.Drawing.Size(607, 174);
            this.rtApplied.TabIndex = 5;
            this.rtApplied.Text = "";
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.White;
            this.Label2.Location = new System.Drawing.Point(12, 4);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(188, 16);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "Applied Bonus Effects:";
            // 
            // panelBars
            // 
            this.panelBars.AutoScroll = true;
            this.panelBars.BackColor = System.Drawing.Color.Transparent;
            this.panelBars.Location = new System.Drawing.Point(626, 28);
            this.panelBars.Name = "panelBars";
            this.panelBars.Size = new System.Drawing.Size(329, 388);
            this.panelBars.TabIndex = 22;
            this.panelBars.Visible = false;
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.Black;
            this.panelButtons.Controls.Add(this.btnSmall);
            this.panelButtons.Controls.Add(this.btnDetailFx);
            this.panelButtons.Controls.Add(this.chkOnTop);
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.Location = new System.Drawing.Point(626, 419);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(329, 53);
            this.panelButtons.TabIndex = 23;
            // 
            // btnSmall
            // 
            this.btnSmall.Checked = false;
            this.btnSmall.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btnSmall.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.btnSmall.Location = new System.Drawing.Point(0, 31);
            this.btnSmall.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSmall.Name = "btnSmall";
            this.btnSmall.Size = new System.Drawing.Size(105, 22);
            this.btnSmall.TabIndex = 20;
            this.btnSmall.TextOff = "<< Shrink";
            this.btnSmall.TextOn = ">>";
            this.btnSmall.Toggle = false;
            this.btnSmall.ButtonClicked += new ImageButton.ButtonClickedEventHandler(this.btnSmall_Click);
            // 
            // btnDetailFx
            // 
            this.btnDetailFx.Checked = false;
            this.btnDetailFx.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btnDetailFx.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.btnDetailFx.Location = new System.Drawing.Point(0, 3);
            this.btnDetailFx.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDetailFx.Name = "btnDetailFx";
            this.btnDetailFx.Size = new System.Drawing.Size(105, 22);
            this.btnDetailFx.TabIndex = 21;
            this.btnDetailFx.TextOff = "FX summary";
            this.btnDetailFx.TextOn = "FX by source";
            this.btnDetailFx.Toggle = true;
            this.btnDetailFx.ButtonClicked += new ImageButton.ButtonClickedEventHandler(this.btnDetailFx_ButtonClicked);
            // 
            // chkOnTop
            // 
            this.chkOnTop.Checked = true;
            this.chkOnTop.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.chkOnTop.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.chkOnTop.Location = new System.Drawing.Point(224, 3);
            this.chkOnTop.Name = "chkOnTop";
            this.chkOnTop.Size = new System.Drawing.Size(105, 22);
            this.chkOnTop.TabIndex = 19;
            this.chkOnTop.TextOff = "Keep On Top";
            this.chkOnTop.TextOn = "Keep On Top";
            this.chkOnTop.Toggle = true;
            this.chkOnTop.ButtonClicked += new ImageButton.ButtonClickedEventHandler(this.chkOnTop_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Checked = false;
            this.btnClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.btnClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.btnClose.Location = new System.Drawing.Point(224, 31);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 22);
            this.btnClose.TabIndex = 18;
            this.btnClose.TextOff = "Close";
            this.btnClose.TextOn = "Close";
            this.btnClose.Toggle = false;
            this.btnClose.ButtonClicked += new ImageButton.ButtonClickedEventHandler(this.btnClose_Click);
            // 
            // frmSetViewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(967, 481);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelBars);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.rtApplied);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.rtxtFX);
            this.Controls.Add(this.rtxtInfo);
            this.Controls.Add(this.lstSets);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Currently Active Sets & Bonuses";
            this.TopMost = true;
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private ImageButton btnDetailFx;
        private System.Windows.Forms.Panel panelBars;
        private System.Windows.Forms.Panel panelButtons;
    }
}