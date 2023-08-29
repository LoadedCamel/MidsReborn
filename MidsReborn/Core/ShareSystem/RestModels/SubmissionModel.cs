namespace Mids_Reborn.Core.ShareSystem.RestModels
{
    internal class SubmissionModel
    {
        public string? Id { get; set; }
        public string? Data { get; set; }
        public string? Image { get; set; }

        public SubmissionModel(string? id, string data, string image)
        {
            Id = id;
            Data = data;
            Image = image;
        }
    }
}
