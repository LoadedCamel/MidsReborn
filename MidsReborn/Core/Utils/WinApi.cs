using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mids_Reborn.Core.Utils
{
    internal static class WinApi
    {
        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hWnd, int milliseconds, AnimationFlags flags);

        [Flags]
        public enum AnimationFlags : int
        {
            Roll = 0x0000,
            Slide = 0x40000,
            Blend = 0x80000,
            Hide = 0x10000,
            Center = 0x00000010,
            HorPositive = 0x00000001,
            HorNegative = 0x00000002,
            VerPositive = 0x00000004,
            VerNegative = 0x00000008,
            Activate = 0x20000
        }
    }
}
