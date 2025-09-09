using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls;

public sealed class ResizableTableLayoutPanel : TableLayoutPanel
{
    protected override void WndProc(ref Message m)
    {
        const int WM_NCHITTEST = 0x84;
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 16;
        const int HTBOTTOMRIGHT = 17;

        if (m.Msg == WM_NCHITTEST)
        {
            Point screenPoint = new((int)(m.LParam.ToInt64() & 0xFFFF), (int)(m.LParam.ToInt64() >> 16 & 0xFFFF));
            Point clientPoint = PointToClient(screenPoint);
            Rectangle clientRect = ClientRectangle;

            int dpi = DeviceDpi;
            int borderWidth = (int)(6 * dpi / 96f);

            bool left = clientPoint.X <= borderWidth;
            bool right = clientPoint.X >= clientRect.Width - borderWidth;
            bool top = clientPoint.Y <= borderWidth;
            bool bottom = clientPoint.Y >= clientRect.Height - borderWidth;

            if (top && left)
                m.Result = HTTOPLEFT;
            else if (top && right)
                m.Result = HTTOPRIGHT;
            else if (bottom && left)
                m.Result = HTBOTTOMLEFT;
            else if (bottom && right)
                m.Result = HTBOTTOMRIGHT;
            else if (top)
                m.Result = HTTOP;
            else if (bottom)
                m.Result = HTBOTTOM;
            else if (left)
                m.Result = HTLEFT;
            else if (right)
                m.Result = HTRIGHT;

            if ((int)m.Result != 0)
                return;
        }

        base.WndProc(ref m);
    }
}