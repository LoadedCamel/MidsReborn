namespace Mids_Reborn.Core.BuildFile.DataModels
{
    public class SlotData
    {
        public int Level { get; set; }
        public bool IsInherent { get; set; }
        public EnhancementData? Enhancement { get; set; }
        public EnhancementData? FlippedEnhancement { get; set; }
    }
}
