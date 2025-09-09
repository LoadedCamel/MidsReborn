using System;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Core.Base.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this eEnhRelative level)
        {
            // Offset by 4 to make 'Even' (which has a value of 4) equal to 0.
            return (int)level - 4;
        }

        public static eEnhRelative ToEnhRelative(this int value)
        {
            // Clamp the value to the valid mathematical range (-3 to +5)
            int clampedValue = Math.Clamp(value, -3, 5);

            // Add 4 to convert the mathematical value back to the enum's underlying integer.
            return (eEnhRelative)(clampedValue + 4);
        }
    }
}
