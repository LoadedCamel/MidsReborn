using System.Collections.Generic;

namespace Mids_Reborn.Core.BuildFile.DataModels
{
    public class PowerData
    {
        public string PowerName { get; set; } = "";
        public int Level { get; set; } = -1;
        public bool StatInclude { get; set; }
        public bool ProcInclude { get; set; }
        public int VariableValue { get; set; }
        public int InherentSlotsUsed { get; set; }
        public List<SubPowerData> SubPowerEntries { get; set; } = new();
        public List<SlotData> SlotEntries { get; set; } = new();
    }
}
