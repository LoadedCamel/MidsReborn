namespace MRB_Boostrap.Services;

public interface IFileDownloader : IDisposable
{
    Task<long?> GetRemoteFileSizeAsync(string url, CancellationToken cancellationToken = default);
    bool TryGetLocalFileSize(string path, out long size);
    Task<bool> DownloadFileAsync(
        string url,
        string localPath,
        string statusText,
        bool showProgress,
        IProgress<float>? progress = null,
        CancellationToken cancellationToken = default);
}