using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmSetFind
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

            this.lvBonus = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lvMag = new System.Windows.Forms.ListView();
            this.ColumnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvSet = new System.Windows.Forms.ListView();
            this.ColumnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.ilSets = new System.Windows.Forms.ImageList(this.components);
            this.Panel1 = new System.Windows.Forms.Panel();
            this.ibClose = new midsControls.ImageButton();
            this.ibTopmost = new midsControls.ImageButton();
            this.SetInfo = new midsControls.ctlPopUp();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            this.lvBonus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader1
            });
            this.lvBonus.FullRowSelect = true;
            this.lvBonus.HideSelection = false;

            this.lvBonus.Location = new System.Drawing.Point(12, 12);
            this.lvBonus.MultiSelect = false;
            this.lvBonus.Name = "lvBonus";

            this.lvBonus.Size = new System.Drawing.Size(280, 292);
            this.lvBonus.TabIndex = 0;
            this.lvBonus.UseCompatibleStateImageBehavior = false;
            this.lvBonus.View = System.Windows.Forms.View.Details;
            this.lvBonus.SelectedIndexChanged += new System.EventHandler(lvBonus_SelectedIndexChanged);
            this.ColumnHeader1.Text = "Bonus Effect";
            this.ColumnHeader1.Width = 254;
            this.lvMag.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1]
            {
        this.ColumnHeader2
            });
            this.lvMag.FullRowSelect = true;
            this.lvMag.HideSelection = false;

            this.lvMag.Location = new System.Drawing.Point(298, 12);
            this.lvMag.MultiSelect = false;
            this.lvMag.Name = "lvMag";

            this.lvMag.Size = new System.Drawing.Size((int)sbyte.MaxValue, 116);
            this.lvMag.TabIndex = 1;
            this.lvMag.UseCompatibleStateImageBehavior = false;
            this.lvMag.View = System.Windows.Forms.View.Details;
            this.lvMag.SelectedIndexChanged += new System.EventHandler(lvMag_SelectedIndexChanged);
            this.ColumnHeader2.Text = "Effect Strength";
            this.ColumnHeader2.Width = 99;
            this.lvSet.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4]
            {
        this.ColumnHeader3,
        this.ColumnHeader4,
        this.ColumnHeader5,
        this.ColumnHeader6
            });
            this.lvSet.FullRowSelect = true;
            this.lvSet.HideSelection = false;

            this.lvSet.Location = new System.Drawing.Point(298, 134);
            this.lvSet.MultiSelect = false;
            this.lvSet.Name = "lvSet";

            this.lvSet.Size = new System.Drawing.Size(484, 170);
            this.lvSet.SmallImageList = this.ilSets;
            this.lvSet.TabIndex = 2;
            this.lvSet.UseCompatibleStateImageBehavior = false;
            this.lvSet.View = System.Windows.Forms.View.Details;
            this.lvSet.SelectedIndexChanged += new System.EventHandler(lvSet_SelectedIndexChanged);
            this.ColumnHeader3.Text = "Set";
            this.ColumnHeader3.Width = 157;
            this.ColumnHeader4.Text = "Level";
            this.ColumnHeader4.Width = 69;
            this.ColumnHeader5.Text = "Type";
            this.ColumnHeader5.Width = 140;
            this.ColumnHeader6.Text = "Required Enh's.";
            this.ColumnHeader6.Width = 90;
            this.ilSets.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;

            this.ilSets.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSets.TransparentColor = System.Drawing.Color.Transparent;
            this.Panel1.AutoScroll = true;
            this.Panel1.BackColor = System.Drawing.Color.Black;
            this.Panel1.Controls.Add((System.Windows.Forms.Control)this.SetInfo);

            this.Panel1.Location = new System.Drawing.Point(431, 12);
            this.Panel1.Name = "Panel1";

            this.Panel1.Size = new System.Drawing.Size(351, 115);
            this.Panel1.TabIndex = 3;
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibClose.Location = new System.Drawing.Point(677, 310);
            this.ibClose.Name = "ibClose";

            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 5;
            this.ibClose.TextOff = "Close";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            this.ibTopmost.Checked = true;
            this.ibTopmost.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);

            this.ibTopmost.Location = new System.Drawing.Point(566, 310);
            this.ibTopmost.Name = "ibTopmost";

            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 4;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
            this.SetInfo.BXHeight = 600;
            this.SetInfo.ColumnPosition = 0.5f;
            this.SetInfo.ColumnRight = false;
            this.SetInfo.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.SetInfo.InternalPadding = 3;

            this.SetInfo.Location = new System.Drawing.Point(0, 0);
            this.SetInfo.Name = "SetInfo";
            this.SetInfo.SectionPadding = 8;

            this.SetInfo.Size = new System.Drawing.Size(331, 198);
            this.SetInfo.TabIndex = 0;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(792, 340);
            this.Controls.Add((System.Windows.Forms.Control)this.ibClose);
            this.Controls.Add((System.Windows.Forms.Control)this.ibTopmost);
            this.Controls.Add((System.Windows.Forms.Control)this.Panel1);
            this.Controls.Add((System.Windows.Forms.Control)this.lvSet);
            this.Controls.Add((System.Windows.Forms.Control)this.lvMag);
            this.Controls.Add((System.Windows.Forms.Control)this.lvBonus);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Bonus Finder";
            this.TopMost = true;
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ColumnHeader ColumnHeader4;
        ColumnHeader ColumnHeader5;
        ColumnHeader ColumnHeader6;
        ImageList ilSets;
        ListView lvBonus;
        ListView lvMag;
        ListView lvSet;
        Panel Panel1;
    }
}