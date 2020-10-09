using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmTotalsV2 : Form
    {
        public frmTotalsV2()
        {
            InitializeComponent();
        }

        private void frmTotalsV2_Load(object sender, EventArgs e)
        {
            barRegen.Refresh();
            barMaxHp.Refresh();
            barEndRec.Refresh();
            barEndUse.Refresh();
            barMaxEnd.Refresh();

            panelDefBg.Refresh();
            panelResBg.Refresh();

            panelRegenBg.Refresh();
            panelHpBg.Refresh();
            panelEndRecBg.Refresh();
            panelEndUseBg.Refresh();
            panelMaxEndBg.Refresh();
        }

        private void barRegen_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, barRegen.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void barMaxHp_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, barMaxHp.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void barEndRec_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, barEndRec.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void barEndUse_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, barEndUse.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void barMaxEnd_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, barMaxEnd.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void panelDefBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelDefBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(127, 0, 127));
            e.Graphics.FillRectangle(lgb, 0, 0, panelDefBg.ClientRectangle.Width, panelDefBg.ClientRectangle.Height);

            ControlPaint.DrawBorder(e.Graphics, barEndUse.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void panelResBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelResBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(0, 127, 0));
            e.Graphics.FillRectangle(lgb, 0, 0, panelResBg.ClientRectangle.Width, panelResBg.ClientRectangle.Height);

            ControlPaint.DrawBorder(e.Graphics, barEndUse.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void panelRegenBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelResBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(0, 191, 0));
            e.Graphics.FillRectangle(lgb, 0, 0, panelRegenBg.ClientRectangle.Width, panelRegenBg.ClientRectangle.Height);
        }

        private void panelHpBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelResBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(64, 127, 64));
            e.Graphics.FillRectangle(lgb, 0, 0, panelHpBg.ClientRectangle.Width, panelHpBg.ClientRectangle.Height);
        }

        private void panelEndRecBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelEndRecBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(64, 127, 64));
            e.Graphics.FillRectangle(lgb, 0, 0, panelEndRecBg.ClientRectangle.Width, panelEndRecBg.ClientRectangle.Height);
        }

        private void panelEndUseBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelEndUseBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(64, 127, 64));
            e.Graphics.FillRectangle(lgb, 0, 0, panelEndUseBg.ClientRectangle.Width, panelEndUseBg.ClientRectangle.Height);
        }

        private void panelMaxEndBg_Paint(object sender, PaintEventArgs e)
        {
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(panelMaxEndBg.ClientRectangle.Width, 0);

            LinearGradientBrush lgb = new LinearGradientBrush(startPoint, endPoint, Color.Black, Color.FromArgb(64, 127, 64));
            e.Graphics.FillRectangle(lgb, 0, 0, panelMaxEndBg.ClientRectangle.Width, panelMaxEndBg.ClientRectangle.Height);
        }
    }
}
