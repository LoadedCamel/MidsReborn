using System;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UIv2.Controls
{
    internal class ThemedScrollPanel : Panel
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _ = WinApi.SetWindowTheme(Handle, "Explorer", null);
        }
    }
}
