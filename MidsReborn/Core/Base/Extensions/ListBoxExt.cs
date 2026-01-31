using System.Reflection;
using System.Windows.Forms;

namespace Mids_Reborn.Core.Base.Extensions
{
    public static class ListBoxExt
    {
        public static void EnableDoubleBuffer(this ListBox lbw)
        {
            lbw
                .GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(lbw, true, null);
        }
    }
}
