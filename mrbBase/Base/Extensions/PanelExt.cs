using System.Windows.Forms;

namespace mrbBase.Base.Extensions
{
    public static class PanelExt
    {
        public static void EnableDoubleBuffer(this Panel panel)
        {
            panel
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panel, true, null);
        }
    }
}