using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms.JsonImport
{
    partial class frmJsonImportMain
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
            button1 = new Button();
            openFileDialog1 = new OpenFileDialog();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackgroundImageLayout = ImageLayout.None;
            button1.Location = new System.Drawing.Point(12, 12);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(119, 23);
            button1.TabIndex = 0;
            button1.Text = "Import AttribMod";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog";
            // 
            // frmJsonImportMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(141, 46);
            Controls.Add(button1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmJsonImportMain";
            StartPosition = FormStartPosition.CenterParent;
            Text = "JSON Importer";
            ResumeLayout(false);

        }

        #endregion

        private Button button1;
        private OpenFileDialog openFileDialog1;
    }
}