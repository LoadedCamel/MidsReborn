namespace Mids_Reborn.Forms.Controls
{
    sealed partial class EnhPicker
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnhPicker));
            enhTypeImages = new System.Windows.Forms.ImageList(components);
            SuspendLayout();
            // 
            // enhTypeImages
            // 
            enhTypeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            enhTypeImages.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("enhTypeImages.ImageStream");
            enhTypeImages.TransparentColor = System.Drawing.Color.Transparent;
            enhTypeImages.Images.SetKeyName(0, "None.png");
            enhTypeImages.Images.SetKeyName(1, "Normal.png");
            enhTypeImages.Images.SetKeyName(2, "InventO.png");
            enhTypeImages.Images.SetKeyName(3, "HamiO.png");
            enhTypeImages.Images.SetKeyName(4, "SetO.png");
            // 
            // EnhPicker
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            BackColor = System.Drawing.Color.Gray;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            Name = "EnhPicker";
            Size = new System.Drawing.Size(336, 368);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ImageList enhTypeImages;
    }
}
