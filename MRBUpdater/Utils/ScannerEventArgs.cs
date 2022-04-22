using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRBUpdater.Utils
{
    public class ScanFailureEventArgs : EventArgs
    {
        #region Constructors

        public ScanFailureEventArgs(string name, Exception e)
        {
            Name = name;
            Exception = e;
            ContinueRunning = true;
        }

        #endregion Constructors

        public string Name { get; }

        public Exception Exception { get; }

        public bool ContinueRunning { get; set; }
    }

    public class ScanEventArgs : EventArgs
    {
        #region Constructors

        public ScanEventArgs(string name)
        {
            Name = name;
        }

        #endregion Constructors

        public string Name { get; }

        public bool ContinueRunning { get; set; } = true;
    }
}
