using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    sealed partial class ModernDamageDisplay
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
            if (disposing)
            {
                if (!DesignMode)
                {
                    ThemeManager.ThemeChanged -= ThemeManagerOnThemeChanged;
                }

                _toolTip?.Dispose();
                components?.Dispose();
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
            SuspendLayout();
            // 
            // ModernDamageDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.Transparent;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            Name = "ModernDamageDisplay";
            Size = new System.Drawing.Size(280, 90);
            ResumeLayout(false);
        }

        #endregion
    }
}
