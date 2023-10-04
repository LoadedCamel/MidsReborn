using System;

namespace Mids_Reborn.Core.Utils
{
    public class Interval
    {
        public float Start { get; set; }
        public float End { get; set; }
        public float Length => Math.Abs(End - Start);
        public float Center => Start + Length / 2f;

        /// <summary>
        /// Build an interval from start/end values (e.g. length segment)
        /// </summary> 
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        public Interval(float start, float end)
        {
            Start = Math.Min(start, end);
            End = Math.Max(start, end);
        }

        /// <summary>
        /// Build an interval from end value only (e.g. duration), assume start = 0
        /// </summary>
        /// <param name="end">End value</param>
        public Interval(float end)
        {
            Start = 0;
            End = end;
        }

        /// <summary>
        /// Build an interval from another one, copy values
        /// </summary>
        /// <param name="baseInterval">Base interval</param>
        public Interval(Interval baseInterval)
        {
            Start = Math.Min(baseInterval.Start, baseInterval.End);
            End = Math.Max(baseInterval.Start, baseInterval.End);
        }

        /// <summary>
        /// Scale interval by a factor. Will multiply Start and End by a factor.
        /// </summary>
        /// <param name="factor">Scale factor</param>
        /// <returns>Rescaled interval</returns>
        public Interval Scale(float factor)
        {
            return new Interval(Start * factor, End * factor);
        }

        /// <summary>
        /// Scale interval by a factor, keeping the center of the segment identical.
        /// </summary>
        /// <param name="factor">Scale factor</param>
        /// <returns>Rescaled interval</returns>
        public Interval ScaleCenter(float factor)
        {
            var dist2 = Math.Abs(Center - Start);

            return new Interval(Center - dist2 * factor, Center + dist2 * factor);
        }

        /// <summary>
        /// Scale interval by a factor, align center on a different value.
        /// </summary>
        /// <param name="factor">Scale factor</param>
        /// <param name="centerRef">Value to align center to</param>
        /// <returns>Rescaled interval</returns>
        public Interval ScaleCenterAt(float factor, float centerRef)
        {
            var dist2 = Math.Abs(Center - Start);
            var offset = centerRef - Center;

            return new Interval(Center - dist2 * factor + offset, Center + dist2 * factor + offset);
        }

        /// <summary>
        /// Apply an offset on the interval. Will add a value to Start and End.
        /// </summary>
        /// <param name="offset">Offset value to apply</param>
        /// <returns>Moved interval</returns>
        public Interval Offset(float offset)
        {
            return new Interval(Start + offset, End + offset);
        }

        /// <summary>
        /// Check if a value is inside the interval
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="includeBounds">True include bounds, false exclude them</param>
        /// <returns>True if value is inside the interval</returns>
        public bool Contains(float value, bool includeBounds = true)
        {
            return includeBounds
                ? value >= Start & value <= End
                : value > Start & value < End;
        }

        /// <summary>
        /// Cap an interval inside another one. If changeSize is set to true, bounds are just set to container's Start and End, changing length
        /// </summary>
        /// <param name="container">Container interval</param>
        /// <param name="changeSize">Allow interval size to be changed if cap is hit</param>
        /// <returns>Capped interval</returns>
        public Interval MinMax(Interval container, bool changeSize = false)
        {
            if (changeSize)
            {
                return new Interval(Math.Max(container.Start, Start), Math.Min(container.End, End));
            }

            if (Start < container.Start)
            {
                return new Interval(container.Start, container.Start + Length);
            }

            return End > container.End
                ? new Interval(container.End - Length, container.End)
                : new Interval(Start, End); // return this; // ???
        }

        /// <summary>
        /// Cap an interval inside min, max values. If changeSize is set to true, bounds are just set min and max values, changing length
        /// </summary>
        /// <param name="cStart">Lower bound</param>
        /// <param name="cEnd">Upper bound</param>
        /// <param name="changeSize">Allow interval size to be changed if cap is hit</param>
        /// <returns>Capped interval</returns>
        public Interval MinMax(float cStart, float cEnd, bool changeSize = false)
        {
            if (changeSize)
            {
                return new Interval(Math.Max(cStart, Start), Math.Min(cEnd, End));
            }

            if (Start < cStart)
            {
                return new Interval(cStart, cStart + Length);
            }

            return End > cEnd
                ? new Interval(cEnd - Length, cEnd)
                : new Interval(Start, End); // return this; // ???
        }

        public override string ToString()
        {
            return $"<Interval> {{{Start}, {End} (Len={Length})}}";
        }
    }
}
