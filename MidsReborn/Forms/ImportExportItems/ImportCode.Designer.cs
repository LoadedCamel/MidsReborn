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
            components = new System.ComponentModel.Container();
            tbChunkBox = new System.Windows.Forms.TextBox();
            bdImport = new Controls.ImageButtonEx();
            errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // tbChunkBox
            // 
            tbChunkBox.Location = new System.Drawing.Point(12, 12);
            tbChunkBox.Multiline = true;
            tbChunkBox.Name = "tbChunkBox";
            tbChunkBox.PlaceholderText = "Paste the build data chunk you received here, then click the import button.";
            tbChunkBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbChunkBox.Size = new System.Drawing.Size(636, 387);
            tbChunkBox.TabIndex = 0;
            // 
            // bdImport
            // 
            bdImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            bdImport.CurrentText = "Import";
            bdImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            bdImport.Images.Background = MRBResourceLib.Resources.HeroButton;
            bdImport.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            bdImport.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            bdImport.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            bdImport.Location = new System.Drawing.Point(250, 406);
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
            // errorProvider1
            // 
            errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
            errorProvider1.ContainerControl = this;
            // 
            // ImportCode
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(659, 446);
            Controls.Add(bdImport);
            Controls.Add(tbChunkBox);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "ImportCode";
            Text = "Import DataChunk";
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbChunkBox;
        private Controls.ImageButtonEx bdImport;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}