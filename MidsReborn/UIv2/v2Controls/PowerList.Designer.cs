using System.Windows.Forms;
using Mids_Reborn.Controls;

namespace Mids_Reborn.UIv2.v2Controls
{
    partial class PowerList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ctlOutlinedLabel1 = new ctlOutlinedLabel();
            this.ctlCombo1 = new ctlCombo();
            this.Panel1 = new ctlPanel();
            this.ctlPowerList1 = new ctlPowerList();
            this.SuspendLayout();
            // 
            // ctlOutlinedLabel1
            // 
            this.ctlOutlinedLabel1.AutoSize = true;
            this.ctlOutlinedLabel1.BackColor = System.Drawing.Color.Transparent;
            this.ctlOutlinedLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctlOutlinedLabel1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ctlOutlinedLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctlOutlinedLabel1.ForeColor = System.Drawing.Color.White;
            this.ctlOutlinedLabel1.Location = new System.Drawing.Point(0, 0);
            this.ctlOutlinedLabel1.Name = "ctlOutlinedLabel1";
            this.ctlOutlinedLabel1.OutlineForeColor = System.Drawing.Color.Black;
            this.ctlOutlinedLabel1.OutlineWidth = 0F;
            this.ctlOutlinedLabel1.Size = new System.Drawing.Size(83, 16);
            this.ctlOutlinedLabel1.TabIndex = 0;
            this.ctlOutlinedLabel1.Text = "ControlLabel";
            this.ctlOutlinedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ctlCombo1
            // 
            this.ctlCombo1.ComboType = ctlCombo.ComboBoxType.Archetype;
            this.ctlCombo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctlCombo1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ctlCombo1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ctlCombo1.FormattingEnabled = true;
            this.ctlCombo1.HighlightColor = System.Drawing.Color.Empty;
            this.ctlCombo1.Location = new System.Drawing.Point(0, 16);
            this.ctlCombo1.Name = "ctlCombo1";
            this.ctlCombo1.Size = new System.Drawing.Size(211, 21);
            this.ctlCombo1.TabIndex = 1;
            // 
            // ctlPanel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Enabled = false;
            this.Panel1.Location = new System.Drawing.Point(0, 37);
            this.Panel1.Name = "ctlPanel1";
            this.Panel1.Size = new System.Drawing.Size(211, 8);
            this.Panel1.TabIndex = 2;
            this.Panel1.Visible = false;
            // 
            // ctlPowerList1
            // 
            this.ctlPowerList1.BackColor = System.Drawing.Color.Transparent;
            this.ctlPowerList1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ctlPowerList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlPowerList1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ctlPowerList1.FormattingEnabled = true;
            this.ctlPowerList1.Location = new System.Drawing.Point(0, 45);
            this.ctlPowerList1.Name = "ctlPowerList1";
            this.ctlPowerList1.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.ctlPowerList1.SelectionColor = System.Drawing.Color.Empty;
            this.ctlPowerList1.Size = new System.Drawing.Size(211, 169);
            this.ctlPowerList1.TabIndex = 2;
            // 
            // PowerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ctlPowerList1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.ctlCombo1);
            this.Controls.Add(this.ctlOutlinedLabel1);
            this.Name = "PowerList";
            this.Size = new System.Drawing.Size(211, 214);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected ctlOutlinedLabel ctlOutlinedLabel1;
        protected ctlCombo ctlCombo1;
        protected Panel Panel1;
        protected ctlPowerList ctlPowerList1;
    }
}
