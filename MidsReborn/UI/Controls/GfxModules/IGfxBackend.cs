using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls.GfxModules;

public interface IGfxBackend
{
    void PrepareTarget(Control ctl);
    void ReInit(Control ctl);
    void Refresh(Rectangle clip);
    void Blank();
    void Blank(Color color);
    void SetGraphicsQuality();
    void ResetTarget();
    void Output(Rectangle destRect, Rectangle srcRect, GraphicsUnit iUnit);
    void OutputUnscaled(Point location);
    void OutputUnscaled();
    Size GetBufferSize();
}