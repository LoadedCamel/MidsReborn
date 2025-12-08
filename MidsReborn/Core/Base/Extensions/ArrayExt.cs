using System;
using System.Diagnostics;
using System.Linq;

namespace Mids_Reborn.Core.Base.Extensions
{
    public static class ArrayExt
    {
        /// <summary>
        /// Pad an array both directions with a value to match a target length.
        /// </summary>
        /// <typeparam name="T">Array elements type.</typeparam>
        /// <param name="arr">Source array.</param>
        /// <param name="padWith">Item to pad array with.</param>
        /// <param name="targetLength">Target size of the new array.</param>
        /// <param name="padBefore">How many times to repeat <see cref="padWith"/> item</param> before source array items.
        /// <returns>Left and right-padded array with padWith item. The new size of the array is <see cref="targetLength"/>. If the source array has more elements than targetLength, <see cref="arr"/> is returned unchanged.</returns>
        public static T[] Pad<T>(this T[] arr, T padWith, int targetLength, int padBefore = 0)
        {
            if (arr.Length >= targetLength)
            {
                return arr;
            }

            padBefore = Math.Max(0, Math.Min(padBefore, targetLength - arr.Length));

            var padBeforeArr = padBefore == 0
                ? []
                : Enumerable.Repeat(padWith, padBefore).ToArray();

            var padAfterArr = targetLength - arr.Length - padBefore <= 0
                ? []
                : Enumerable.Repeat(padWith, targetLength - arr.Length - padBefore);

            return padBeforeArr.Concat(arr).Concat(padAfterArr).ToArray();
        }
    }
}
