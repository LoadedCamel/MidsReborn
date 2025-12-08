namespace Mids_Reborn.Core.ShareSystem.RestModels
{
    public class OperationResult
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }
    }
}
