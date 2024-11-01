using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms
{
    partial class Loader
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
            label = new LabelEx();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            mainPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // label
            // 
            label.BackColor = System.Drawing.Color.Transparent;
            label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label.Location = new System.Drawing.Point(12, 10);
            label.Name = "label";
            label.Size = new System.Drawing.Size(560, 23);
            label.TabIndex = 0;
            label.Text = "label1";
            label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            label.Visible = false;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Dock = System.Windows.Forms.DockStyle.Fill;
            webView.Location = new System.Drawing.Point(0, 0);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(584, 408);
            webView.TabIndex = 1;
            webView.ZoomFactor = 1D;
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(webView);
            mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mainPanel.Location = new System.Drawing.Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(584, 408);
            mainPanel.TabIndex = 2;
            // 
            // Loader
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(584, 408);
            Controls.Add(mainPanel);
            Controls.Add(label);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            Name = "Loader";
            Text = "Loader";
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            mainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private LabelEx label;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Panel mainPanel;
    }
}