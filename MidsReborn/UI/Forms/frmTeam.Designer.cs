#nullable disable

namespace Mids_Reborn.UI.Forms
{
    partial class FrmCombatContext
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(12, 18, 24);
            ClientSize = new Size(900, 560);
            Font = new Font("Noto Sans", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.WhiteSmoke;
            MinimumSize = new Size(760, 520);
            Name = "FrmCombatContext";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Combat Context";
        }
    }
}
