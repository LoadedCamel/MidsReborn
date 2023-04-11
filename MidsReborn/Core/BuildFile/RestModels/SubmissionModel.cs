namespace Mids_Reborn.Core.BuildFile.RestModels
{
    internal class SubmissionModel
    {
        public long Id { get; set; }
        public string? Data { get; set; }

        public SubmissionModel(long id, string data)
        {
            Id = id;
            Data = data;
        }
    }
}
