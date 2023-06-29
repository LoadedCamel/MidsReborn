using System;

namespace Mids_Reborn.Core.Utils
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string text, double processed, double total)
        {
            Text = text;
            Processed = processed;
            Total = total;
        }

        public ProgressEventArgs(double processed, double total)
        {
            Text = null;
            Processed = processed;
            Total = total;
        }

        public string? Text { get; }

        public double Processed { get; }

        public double Total { get; }

        public int PercentComplete
        {
            get
            {
                double result;
                if (Processed <= 0)
                {
                    result = 0;
                }
                else
                {
                    result = Processed * 100 / Total;
                    if (result > 100) result = 100;
                }
                return (int)result;
            }
        }
    }
}
