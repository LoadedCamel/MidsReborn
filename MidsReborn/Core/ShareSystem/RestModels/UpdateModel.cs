namespace Mids_Reborn.Core.ShareSystem.RestModels
{
    internal class UpdateModel
    {
        public string Code { get; set; }
        public string PageData { get; set; }

        public UpdateModel(string code, string pageData)
        {
            Code = code;
            PageData = pageData;
        }
    }
}
