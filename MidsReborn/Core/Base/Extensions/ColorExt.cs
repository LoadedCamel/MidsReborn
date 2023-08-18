using System.Drawing;

namespace Mids_Reborn.Core.Base.Extensions
{
    internal static class ColorExt
    {
        public static string ToHex(this Color color, bool sharpPrefix=true)
        {
            //return $@"{(sharpPrefix ? "#" : "")}{ColorTranslator.ToWin32(color):X8}";
            return $@"{(sharpPrefix ? "#" : "")}{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }
    }
}
