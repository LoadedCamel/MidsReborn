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
            mainPanel = new System.Windows.Forms.Panel();
            messageText = new System.Windows.Forms.Label();
            spacer = new System.Windows.Forms.Panel();
            buttonPanel = new System.Windows.Forms.Panel();
            btnOkay = new System.Windows.Forms.Button();
            spacerButton = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            iconImage = new System.Windows.Forms.PictureBox();
            titlePanel = new BorderPanel();
            titleText = new System.Windows.Forms.Label();
            mainPanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconImage).BeginInit();
            titlePanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            mainPanel.Controls.Add(messageText);
            mainPanel.Controls.Add(spacer);
            mainPanel.Controls.Add(buttonPanel);
            mainPanel.Controls.Add(iconImage);
            mainPanel.Controls.Add(titlePanel);
            mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            mainPanel.Location = new System.Drawing.Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(420, 150);
            mainPanel.TabIndex = 0;
            // 
            // messageText
            // 
            messageText.AutoSize = true;
            messageText.Dock = System.Windows.Forms.DockStyle.Fill;
            messageText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            messageText.Location = new System.Drawing.Point(100, 22);
            messageText.Name = "messageText";
            messageText.Size = new System.Drawing.Size(154, 17);
            messageText.TabIndex = 4;
            messageText.Text = "Message Text Goes Here";
            messageText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacer
            // 
            spacer.Dock = System.Windows.Forms.DockStyle.Bottom;
            spacer.Location = new System.Drawing.Point(100, 104);
            spacer.Name = "spacer";
            spacer.Size = new System.Drawing.Size(316, 10);
            spacer.TabIndex = 8;
            // 
            // buttonPanel
            // 
            buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            buttonPanel.Controls.Add(btnOkay);
            buttonPanel.Controls.Add(spacerButton);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            buttonPanel.Location = new System.Drawing.Point(100, 114);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new System.Drawing.Size(316, 32);
            buttonPanel.TabIndex = 6;
            // 
            // btnOkay
            // 
            btnOkay.AutoSize = true;
            btnOkay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnOkay.BackColor = System.Drawing.Color.FromArgb(13, 135, 255);
            btnOkay.Dock = System.Windows.Forms.DockStyle.Right;
            btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnOkay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOkay.Location = new System.Drawing.Point(157, 0);
            btnOkay.Name = "btnOkay";
            btnOkay.Size = new System.Drawing.Size(53, 32);
            btnOkay.TabIndex = 2;
            btnOkay.Text = "Okay";
            btnOkay.UseVisualStyleBackColor = false;
            btnOkay.Click += BtnOkay_Click;
            // 
            // spacerButton
            // 
            spacerButton.AutoSize = true;
            spacerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            spacerButton.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            spacerButton.Dock = System.Windows.Forms.DockStyle.Right;
            spacerButton.Enabled = false;
            spacerButton.FlatAppearance.BorderSize = 0;
            spacerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            spacerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            spacerButton.Location = new System.Drawing.Point(210, 0);
            spacerButton.Name = "spacerButton";
            spacerButton.Size = new System.Drawing.Size(41, 32);
            spacerButton.TabIndex = 4;
            spacerButton.Text = "      ";
            spacerButton.UseVisualStyleBackColor = false;
            spacerButton.Visible = false;
            // 
            // btnCancel
            // 
            btnCancel.AutoSize = true;
            btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnCancel.BackColor = System.Drawing.Color.FromArgb(88, 40, 18);
            btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnCancel.Location = new System.Drawing.Point(251, 0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(65, 32);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // iconImage
            // 
            iconImage.Dock = System.Windows.Forms.DockStyle.Left;
            iconImage.Location = new System.Drawing.Point(0, 22);
            iconImage.Name = "iconImage";
            iconImage.Size = new System.Drawing.Size(100, 124);
            iconImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            iconImage.TabIndex = 7;
            iconImage.TabStop = false;
            // 
            // titlePanel
            // 
            titlePanel.Border.Color = System.Drawing.Color.Snow;
            titlePanel.Border.Style = System.Windows.Forms.ButtonBorderStyle.Inset;
            titlePanel.Border.Thickness = 2;
            titlePanel.Border.Which = BorderPanel.PanelBorder.BorderToDraw.Bottom;
            titlePanel.Controls.Add(titleText);
            titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            titlePanel.Location = new System.Drawing.Point(0, 0);
            titlePanel.Name = "titlePanel";
            titlePanel.Size = new System.Drawing.Size(416, 22);
            titlePanel.TabIndex = 10;
            // 
            // titleText
            // 
            titleText.AutoSize = true;
            titleText.Dock = System.Windows.Forms.DockStyle.Fill;
            titleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            titleText.Location = new System.Drawing.Point(0, 0);
            titleText.Name = "titleText";
            titleText.Size = new System.Drawing.Size(76, 17);
            titleText.TabIndex = 0;
            titleText.Text = "Title Text";
            titleText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MessageBoxEx
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            ClientSize = new System.Drawing.Size(420, 150);
            Controls.Add(mainPanel);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ForeColor = System.Drawing.Color.Azure;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "MessageBoxEx";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "MessageBoxEx";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            buttonPanel.ResumeLayout(false);
            buttonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)iconImage).EndInit();
            titlePanel.ResumeLayout(false);
            titlePanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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