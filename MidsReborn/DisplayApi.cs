using Mids_Reborn.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Mids_Reborn
{
    internal static class DisplayApi
    {
        internal static frmMain? FrmMain { get; set; }

        public static Size MainWindowSize
        {
            get
            {
                Debug.Assert(FrmMain != null, nameof(FrmMain) + " != null");
                return FrmMain.Size;
            }
        }
    }
}
