using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mids_Reborn.Core.Base.Display
{
    public class InherentDisplayItem
    {
        public int Priority { get; set; }
        public IPower Power { get; set; }

        public InherentDisplayItem(int priority, IPower power)
        {
            Power = power;
            Priority = priority;
        }
    }
}
