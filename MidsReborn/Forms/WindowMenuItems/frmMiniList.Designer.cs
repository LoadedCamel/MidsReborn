using System.ComponentModel;
using System.Drawing;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmMiniList
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
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = System.Drawing.Color.Black;
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox1.DetectUrls = false;
            richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            richTextBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            richTextBox1.Location = new System.Drawing.Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBox1.ShortcutsEnabled = false;
            richTextBox1.Size = new System.Drawing.Size(249, 284);
            richTextBox1.TabIndex = 0;
            richTextBox1.TabStop = false;
            richTextBox1.Text = "";
            // 
            // frmMiniList
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(249, 284);
            Controls.Add(richTextBox1);
            Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            MaximumSize = new System.Drawing.Size(600, 600);
            MinimumSize = new System.Drawing.Size(100, 150);
            Name = "frmMiniList";
            Text = "Mini List";
            TopMost = true;
            Load += frmMiniList_Load;
            FormClosed += frmMiniList_FormClosed;
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}