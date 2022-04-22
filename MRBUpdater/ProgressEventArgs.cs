using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRBUpdater
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(string name, float progress, float total)
        {
            Name = name;
            Processed = progress;
            Total = total;
        }

        public string Name { get; }

        public float Processed { get; }

        public float Total { get; }

        public float PercentComplete
        {
            get
            {
                float result;
                if (Processed <= 0)
                {
                    result = 0;
                }
                else
                {
                    result = (Processed / Total) * 100.0f;
                }
                return result;
            }
        }

        public bool ContinueRunning { get; set; } = true;
    }
}
