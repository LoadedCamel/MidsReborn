using Newtonsoft.Json;

namespace Mids_Reborn.Core.BuildFile.RestModels
{
    public class ResponseModel
    {
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public long Id { get; set; }
        public string? Url { get; set; }
        public string? Code { get; set; }
    }
}
