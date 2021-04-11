
namespace mrbControls.pnlGFX2
{
    partial class ctlSlotViewer
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
            this.SuspendLayout();
            // 
            // ctlSlotViewer
            // 
            this.Size = new System.Drawing.Size(30, 30);
            this.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs>(this.ctlSlotViewer_PaintSurface);
            this.MouseEnter += new System.EventHandler(this.ctlSlotViewer_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ctlSlotViewer_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion
    }
}