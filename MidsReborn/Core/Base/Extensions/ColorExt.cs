using System.Drawing;

namespace Mids_Reborn.Core.Base.Extensions
{
    internal static class ColorExt
    {
        public static string ToHex(this Color color, bool sharpPrefix=true)
        {
            return $@"{(sharpPrefix ? "#" : "")}{ColorTranslator.ToWin32(color):X8}";
        }
    }
}
