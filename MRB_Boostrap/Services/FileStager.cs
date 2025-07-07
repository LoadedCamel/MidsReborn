using Microsoft.Extensions.Logging;
using MRB_Boostrap.Interop;
using MRB_Boostrap.Models;
using MRB_Boostrap.UI;
using System.Runtime.InteropServices;

namespace MRB_Boostrap.Services;

public sealed class FileStager : IFileStager
{
    private readonly ILogger<FileStager> _logger;
    private readonly IUIManager _ui;

    public FileStager(ILogger<FileStager> logger, IUIManager ui)
    {
        _logger = logger;
        _ui = ui;
    }

    public async Task<bool> WriteStagingFilesAsync(List<FileEntry> files, string stagingPath, CancellationToken cancellationToken)
    {
        try
        {
            Directory.CreateDirectory(stagingPath);
            int total = files.Count;

            for (int i = 0; i < total; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                FileEntry file = files[i];
                string fullPath = Path.Combine(stagingPath, file.Directory, file.FileName);
                string? dir = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                await File.WriteAllBytesAsync(fullPath, file.Data, cancellationToken);

                long actualSize = new FileInfo(fullPath).Length;
                if (actualSize != file.Data.Length)
                {
                    _logger.LogWarning("Staging warning: size mismatch for file '{File}'", file.FileName);
                    _logger.LogWarning("  Path     : {Path}", fullPath);
                    _logger.LogWarning("  Expected : {ExpectedSize}", file.Data.Length);
                    _logger.LogWarning("  Actual   : {ActualSize}", actualSize);
                }

                float progress = (float)(i + 1) / total;
                _ui.UpdateProgress(progress);
            }

            _logger.LogInformation("Staging completed to {Path}", stagingPath);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Staging cancelled.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write staged files.");
            return false;
        }
    }

    public async Task<bool> ApplyStagedFilesAsync(List<FileEntry> files, string installPath, string stagingPath, CancellationToken cancellationToken)
    {
        int total = files.Count;

        try
        {
            _ui.UpdateProgress(0f);

            for (int i = 0; i < total; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                FileEntry file = files[i];

                // Skip logs — don't apply them
                if (file.Directory.Equals("Logs", StringComparison.OrdinalIgnoreCase))
                    continue;

                string stagedFile = Path.Combine(stagingPath, file.Directory, file.FileName);
                string targetFile = Path.Combine(installPath, file.Directory, file.FileName);
                string? targetDir = Path.GetDirectoryName(targetFile);

                if (!string.IsNullOrEmpty(targetDir))
                    Directory.CreateDirectory(targetDir);

                bool moved = Win32.MoveFileEx(stagedFile, targetFile, Win32.MoveFileFlags.ReplaceExisting);
                if (!moved)
                {
                    int error = Marshal.GetLastWin32Error();
                    _logger.LogError("Patch application failed for file: {File}", file.FileName);
                    _logger.LogError("  Source : {Source}", stagedFile);
                    _logger.LogError("  Target : {Target}", targetFile);
                    _logger.LogError("  Win32 Error Code: {Error}", error);
                    return false;
                }

                float progress = (float)(i + 1) / total;
                _ui.UpdateProgress(progress);
            }

            _logger.LogInformation("Patch applied successfully to {Path}", installPath);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Patch installation cancelled.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while applying staged files.");
            return false;
        }
    }
}