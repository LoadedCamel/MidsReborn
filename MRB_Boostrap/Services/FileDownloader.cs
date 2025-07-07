using Microsoft.Extensions.Logging;
using MRB_Boostrap.UI;
using System.Net;

namespace MRB_Boostrap.Services;

public sealed class FileDownloader : IFileDownloader
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileDownloader> _logger;
    private readonly IUIManager _ui;

    public FileDownloader(ILogger<FileDownloader> logger, IUIManager ui)
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        })
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        _logger = logger;
        _ui = ui;
    }

    public void Dispose() => _httpClient.Dispose();

    public bool TryGetLocalFileSize(string path, out long size)
    {
        try
        {
            if (File.Exists(path))
            {
                size = new FileInfo(path).Length;
                _logger.LogInformation("Local file size for {Path}: {Size:N0} bytes", path, size);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get local file size for {Path}", path);
        }

        size = 0;
        return false;
    }

    public async Task<long?> GetRemoteFileSizeAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                long? length = response.Content.Headers.ContentLength;
                _logger.LogInformation("Remote file size for {Url}: {Size:N0} bytes", url, length ?? 0);
                return length;
            }

            _logger.LogWarning("HEAD request failed for {Url}: {Code} {Reason}", url, (int)response.StatusCode, response.ReasonPhrase);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get remote file size: {Url}", url);
            return null;
        }
    }

    public async Task<bool> DownloadFileAsync(
        string url,
        string localPath,
        string statusText,
        bool showProgress,
        IProgress<float>? progress = null,
        CancellationToken cancellationToken = default)
    {
        const int bufferSize = 8192;
        const int maxRetries = 3;
        int attempt = 0;

        Uri uri = new(url);
        string host = uri.Host;

        _logger.LogInformation("Preparing to download from {Host}", host);
        _ui.UpdateStatus($"Connecting to {host}...");
        _ui.ShowProgressBar(false);

        while (attempt++ < maxRetries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _logger.LogInformation("Attempt {Attempt}: Downloading {Url}", attempt, url);
                _ui.UpdateStatus(statusText);
                _ui.ShowProgressBar(showProgress);
                progress?.Report(0f);

                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("HTTP error on attempt {Attempt}: {(Code)} {Reason}", attempt, (int)response.StatusCode, response.ReasonPhrase);
                    continue;
                }

                var contentLength = response.Content.Headers.ContentLength ?? -1;
                long totalRead = 0;

                await using var input = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var output = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);

                byte[] buffer = new byte[bufferSize];
                int read;

                while ((read = await input.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await output.WriteAsync(buffer, 0, read, cancellationToken);
                    totalRead += read;

                    if (showProgress && contentLength > 0)
                    {
                        float percent = (float)totalRead / contentLength;
                        _ui.UpdateProgress(percent);
                        progress?.Report(percent);
                    }
                }

                _logger.LogInformation("Downloaded {File} ({Bytes:N0} bytes)", Path.GetFileName(localPath), totalRead);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Download canceled by user for {Url}", url);
                if (File.Exists(localPath))
                    File.Delete(localPath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Attempt {Attempt} failed to download {Url}", attempt, url);
                if (File.Exists(localPath))
                {
                    try
                    {
                        File.Delete(localPath);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogWarning(deleteEx, "Failed to clean up partial file: {Path}", localPath);
                    }
                }

                if (attempt < maxRetries)
                {
                    int delay = attempt * 1000;
                    _logger.LogInformation("Retrying download in {Delay} ms...", delay);
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        _logger.LogError("Final failure: Could not download file after {MaxRetries} attempts: {Url}", maxRetries, url);
        return false;
    }
}