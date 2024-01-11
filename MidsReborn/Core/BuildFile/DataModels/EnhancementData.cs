using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile.DataModels
{
    public class EnhancementData
    {
        public string Enhancement { get; set; } = "";

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Uid { get; set; } = "";

        public string Grade { get; set; } = "None";

        public int IoLevel { get; set; } = 1;

        public string RelativeLevel { get; set; } = "Even";

        public bool Obtained { get; set; } = false;
    }
}
