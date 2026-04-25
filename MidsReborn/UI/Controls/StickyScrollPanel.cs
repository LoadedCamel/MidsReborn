using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls;

public class StickyScrollPanel : Panel
{
    // Override to avoid scroll to reset upon window re-activation
    protected override Point ScrollToControl(Control activeControl)
    {
        var retPt = DisplayRectangle.Location;
        retPt.Offset(new Point(-Padding.Left, -Padding.Bottom));

        return retPt;
    }
}