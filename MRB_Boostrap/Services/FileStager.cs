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
        var total = files.Count;

        try
        {
            _ui.UpdateProgress(0f);

            for (var i = 0; i < total; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var file = files[i];

                // Skip logs — don't apply them
                if (file.Directory.Equals("Logs", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var installPathSubDir = Path.GetFileName(installPath);
                var fDir = file.Directory;
                var fDirChunks = file.Directory.Split(Path.DirectorySeparatorChar);
                
                // Rebirth install updates patch
                // file.Directory includes database name and will create an extra subdirectory with the same database name
                // Only valid 1st level directory for installation path are (empty) and Images
                // Consider anything else as invalid and strip first chunk
                if (!installPathSubDir.Equals("") & !installPathSubDir.Equals("Images", StringComparison.InvariantCultureIgnoreCase))
                {
                    fDir = fDirChunks.Length > 1
                        ? string.Join(Path.DirectorySeparatorChar, fDirChunks.Skip(1))
                        : "";
                }

                var stagedFile = Path.Combine(stagingPath, file.Directory, file.FileName);
                var targetFile = Path.Combine(installPath, fDir, file.FileName);
                var targetDir = Path.GetDirectoryName(targetFile);

                _logger.LogInformation("  Staged file: {StagedFile} -> {TargetFile} (with installPath={InstallPath}, fileDir={FileDirectory}, fileName={FileFileName}", stagedFile, targetFile, installPath, fDir, file.FileName);

                if (!string.IsNullOrEmpty(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var moved = Win32.MoveFileEx(stagedFile, targetFile, Win32.MoveFileFlags.ReplaceExisting);
                if (!moved)
                {
                    var error = Marshal.GetLastWin32Error();
                    _logger.LogError("Patch application failed for file: {File}", file.FileName);
                    _logger.LogError("  Source : {Source}", stagedFile);
                    _logger.LogError("  Target : {Target}", targetFile);
                    _logger.LogError("  Win32 Error Code: {Error}", error);
                    return false;
                }

                var progress = (float)(i + 1) / total;
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