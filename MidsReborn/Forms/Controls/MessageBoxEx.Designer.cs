using System.Reflection.PortableExecutable;
using System.Windows.Controls;

namespace Mids_Reborn.Forms.Controls
{
    partial class MessageBoxEx
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.messageText = new System.Windows.Forms.Label();
            this.spacer = new System.Windows.Forms.Panel();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.btnOkay = new System.Windows.Forms.Button();
            this.spacerButton = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.iconImage = new System.Windows.Forms.PictureBox();
            this.titlePanel = new Mids_Reborn.Forms.Controls.BorderPanel();
            this.titleText = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).BeginInit();
            this.titlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mainPanel.Controls.Add(this.messageText);
            this.mainPanel.Controls.Add(this.spacer);
            this.mainPanel.Controls.Add(this.buttonPanel);
            this.mainPanel.Controls.Add(this.iconImage);
            this.mainPanel.Controls.Add(this.titlePanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(420, 150);
            this.mainPanel.TabIndex = 0;
            // 
            // messageText
            // 
            this.messageText.AutoSize = true;
            this.messageText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.messageText.Location = new System.Drawing.Point(100, 22);
            this.messageText.Name = "messageText";
            this.messageText.Size = new System.Drawing.Size(154, 17);
            this.messageText.TabIndex = 4;
            this.messageText.Text = "Message Text Goes Here";
            this.messageText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacer
            // 
            this.spacer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.spacer.Location = new System.Drawing.Point(100, 104);
            this.spacer.Name = "spacer";
            this.spacer.Size = new System.Drawing.Size(316, 10);
            this.spacer.TabIndex = 8;
            // 
            // buttonPanel
            // 
            this.buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPanel.Controls.Add(this.btnOkay);
            this.buttonPanel.Controls.Add(this.spacerButton);
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(100, 114);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(316, 32);
            this.buttonPanel.TabIndex = 6;
            // 
            // btnOkay
            // 
            this.btnOkay.AutoSize = true;
            this.btnOkay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(135)))), ((int)(((byte)(255)))));
            this.btnOkay.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnOkay.Location = new System.Drawing.Point(157, 0);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(53, 32);
            this.btnOkay.TabIndex = 2;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // spacerButton
            // 
            this.spacerButton.AutoSize = true;
            this.spacerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.spacerButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.spacerButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.spacerButton.Enabled = false;
            this.spacerButton.FlatAppearance.BorderSize = 0;
            this.spacerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spacerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.spacerButton.Location = new System.Drawing.Point(210, 0);
            this.spacerButton.Name = "spacerButton";
            this.spacerButton.Size = new System.Drawing.Size(41, 32);
            this.spacerButton.TabIndex = 4;
            this.spacerButton.Text = "      ";
            this.spacerButton.UseVisualStyleBackColor = false;
            this.spacerButton.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(40)))), ((int)(((byte)(18)))));
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.Location = new System.Drawing.Point(251, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 32);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // iconImage
            // 
            this.iconImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.iconImage.Location = new System.Drawing.Point(0, 22);
            this.iconImage.Name = "iconImage";
            this.iconImage.Size = new System.Drawing.Size(100, 124);
            this.iconImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.iconImage.TabIndex = 7;
            this.iconImage.TabStop = false;
            // 
            // titlePanel
            // 
            this.titlePanel.Border.Which = Mids_Reborn.Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.Bottom;
            this.titlePanel.Border.Color = System.Drawing.Color.Snow;
            this.titlePanel.Border.Style = System.Windows.Forms.ButtonBorderStyle.Inset;
            this.titlePanel.Border.Thickness = 2;
            this.titlePanel.Controls.Add(this.titleText);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(416, 22);
            this.titlePanel.TabIndex = 10;
            // 
            // titleText
            // 
            this.titleText.AutoSize = true;
            this.titleText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleText.Location = new System.Drawing.Point(0, 0);
            this.titleText.Name = "titleText";
            this.titleText.Size = new System.Drawing.Size(76, 17);
            this.titleText.TabIndex = 0;
            this.titleText.Text = "Title Text";
            this.titleText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MessageBoxEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(420, 150);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.Azure;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MessageBoxEx";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MessageBoxEx";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconImage)).EndInit();
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOkay;

        #endregion

        private System.Windows.Forms.Label messageText;
        private System.Windows.Forms.PictureBox iconImage;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Panel spacer;
        private System.Windows.Forms.Button spacerButton;
        private BorderPanel titlePanel;
        private System.Windows.Forms.Label titleText;
    }
}