using MRBResourceLib;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    sealed partial class frmInitializing
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
            components = new Container();
            Label1 = new TransparentLabel();
            tmrOp = new Timer(components);
            panel1 = new Panel();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panel1.SuspendLayout();
            ((ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // Label1
            // 
            Label1.BackColor = System.Drawing.Color.Transparent;
            Label1.FlatStyle = FlatStyle.Flat;
            Label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            Label1.ForeColor = System.Drawing.Color.Gold;
            Label1.Location = new System.Drawing.Point(27, 505);
            Label1.Margin = new Padding(55, 0, 55, 0);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(704, 30);
            Label1.TabIndex = 2;
            Label1.Text = "Reading data files, please wait.";
            Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Controls.Add(webView21);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(600, 447);
            panel1.TabIndex = 3;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            webView21.Dock = DockStyle.Fill;
            webView21.Location = new System.Drawing.Point(0, 0);
            webView21.Name = "webView21";
            webView21.Size = new System.Drawing.Size(600, 447);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // frmInitializing
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.Black;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new System.Drawing.Size(600, 447);
            Controls.Add(panel1);
            Controls.Add(Label1);
            DoubleBuffered = true;
            Margin = new Padding(55, 18, 55, 18);
            Name = "frmInitializing";
            Text = "frmInitializing";
            TopMost = true;
            TransparencyKey = System.Drawing.Color.Transparent;
            panel1.ResumeLayout(false);
            ((ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TransparentLabel Label1;
        private Timer tmrOp;
        private Panel panel1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
    }
}