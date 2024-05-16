namespace Mids_Reborn.Core.ShareSystem
{
    internal class ClientTModel
    {
        public string Id { get; }
        public string Data { get; }

        public ClientTModel(string id, string data)
        {
            Id = id;
            Data = data;
        }
    }
}
