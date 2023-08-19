using System.Drawing;

namespace Mids_Reborn.Core.Base.Extensions
{
    internal static class ColorExt
    {
        public static string ToHex(this Color color, bool sharpPrefix=true)
        {
            return $@"{(sharpPrefix ? "#" : "")}{color.R:X2}{color.G:X2}{color.B:X2}{(color.A < 255 ? $"{color.A:X2}" : "")}";
        }
    }
}
