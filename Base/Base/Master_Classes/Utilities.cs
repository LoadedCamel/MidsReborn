namespace Base.Master_Classes
{
    public static class Utilities
    {
        public static string FixDP(float iNum)
        {
            return (double) iNum < 100.0 && (double) iNum > -100.0 ? FixDP(iNum, 2) : FixDP(iNum, 1);
        }

        public static string FixDP(float iNum, int maxDecimal)
        {
            var format = "0.";
            if (iNum >= 10.0 || iNum <= -10.0)
                format = "###0.";
            for (var index = 0; index < maxDecimal; ++index)
                format += "#";
            return iNum.ToString(format);
        }
    }
}