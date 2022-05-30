using System;

namespace MRBUpdater
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string name, double processed, double total)
        {
            Name = name;
            Processed = processed;
            Total = total;
        }

        public string Name { get; }

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
                }
                return (int)result;
            }
        }
    }
}
