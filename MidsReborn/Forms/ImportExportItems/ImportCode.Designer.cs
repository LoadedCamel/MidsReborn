namespace Mids_Reborn.Forms.ImportExportItems
{
    partial class ImportCode
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
            tbChunkBox = new System.Windows.Forms.TextBox();
            bdImport = new Controls.ImageButtonEx();
            label25 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // tbChunkBox
            // 
            tbChunkBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            tbChunkBox.Location = new System.Drawing.Point(11, 57);
            tbChunkBox.Multiline = true;
            tbChunkBox.Name = "tbChunkBox";
            tbChunkBox.PlaceholderText = "Paste the build data chunk you received here, then click the import button.";
            tbChunkBox.ReadOnly = false;
            tbChunkBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbChunkBox.ShortcutsEnabled = true;
            tbChunkBox.Size = new System.Drawing.Size(746, 294);
            tbChunkBox.TabIndex = 0;
            tbChunkBox.WordWrap = false;
            // 
            // bdImport
            // 
            bdImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            bdImport.CurrentText = "Import";
            bdImport.DisplayVertically = false;
            bdImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            bdImport.Images.Background = MRBResourceLib.Resources.HeroButton;
            bdImport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            bdImport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            bdImport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            bdImport.Location = new System.Drawing.Point(312, 357);
            bdImport.Lock = false;
            bdImport.Name = "bdImport";
            bdImport.Size = new System.Drawing.Size(148, 28);
            bdImport.TabIndex = 70;
            bdImport.Text = "Import";
            bdImport.TextOutline.Color = System.Drawing.Color.Black;
            bdImport.TextOutline.Width = 3;
            bdImport.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            bdImport.ToggleText.Indeterminate = "Indeterminate State";
            bdImport.ToggleText.ToggledOff = "ToggledOff State";
            bdImport.ToggleText.ToggledOn = "ToggledOn State";
            bdImport.UseAlt = false;
            bdImport.Click += BdImport_Click;
            // 
            // label25
            // 
            label25.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label25.ForeColor = System.Drawing.Color.WhiteSmoke;
            label25.Location = new System.Drawing.Point(11, 9);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(746, 45);
            label25.TabIndex = 71;
            label25.Text = "Important!! - When importing a legacy datachunk (mxd) you will need to ensure you are on the correct database.\r\nLegacy builds do not have the ability to detect which database it was built for.";
            // 
            // ImportCode
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(769, 397);
            Controls.Add(label25);
            Controls.Add(bdImport);
            Controls.Add(tbChunkBox);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "ImportCode";
            Text = "Import DataChunk";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbChunkBox;
        private Controls.ImageButtonEx bdImport;
        private System.Windows.Forms.Label label25;
    }
}