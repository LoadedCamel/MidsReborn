using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;
using MRB_Boostrap.UI;
using MRB_Boostrap.Utilities;

namespace MRB_Boostrap.Services;

public sealed class PatchFlowManager : IPatchFlowManager
{
    private readonly IFileDownloader _downloader;
    private readonly IFileDecompressor _decompressor;
    private readonly IFileStager _stager;
    private readonly IHashValidator _validator;
    private readonly IBackupManager _backupManager;
    private readonly IUIManager _ui;
    private readonly ILogger<PatchFlowManager> _logger;
    private readonly LoggingPipeline _pipeline;

    public PatchFlowManager(
        IFileDownloader downloader,
        IFileDecompressor decompressor,
        IFileStager stager,
        IHashValidator validator,
        IBackupManager backupManager,
        IUIManager ui,
        ILogger<PatchFlowManager> logger,
        LoggingPipeline pipeline)
    {
        _downloader = downloader;
        _decompressor = decompressor;
        _stager = stager;
        _validator = validator;
        _backupManager = backupManager;
        _ui = ui;
        _logger = logger;
        _pipeline = pipeline;
    }

    public async Task<int> ExecutePatchFlowAsync(UpdateEntry entry, CancellationToken cancellationToken = default)
    {
        using (_logger.BeginScope("PatchFlow {Name} {Version}", entry.Name, entry.Version))
        {
            _logger.LogInformation("Starting patch for: {Name} ({Type}) -> {File}", entry.Name, entry.Type, entry.File);


            string baseDir = AppContext.BaseDirectory;
            string baseUrl = entry.SourceUri.EndsWith("/") ? entry.SourceUri : $"{entry.SourceUri}/";

            string installPath = entry.Type.Equals("Application", StringComparison.OrdinalIgnoreCase)
                ? baseDir
                : Path.Combine(baseDir, "Databases", entry.Name);

            string patchPath = Path.Combine(baseDir, entry.File);
            string hashPath = Path.ChangeExtension(patchPath, ".hash");

            string stagingPath = Path.Combine(baseDir, "Staging", entry.Name);
            string backupPath = Path.Combine(baseDir, "Backup", entry.Name);

            bool installing = false;
            bool backupMade = false;

            try
            {
                // STEP 1: Get Patch Size
                _logger.LogInformation("Step 1: Checking local or remote patch size.");
                if (_downloader.TryGetLocalFileSize(patchPath, out long localSize))
                {
                    _ui.UpdateVersion($"{entry.Name} {entry.Version} ({localSize / 1024.0:F1} KB)");
                }
                else
                {
                    long? remoteSize =
                        await _downloader.GetRemoteFileSizeAsync($"{baseUrl}{entry.File}", cancellationToken);
                    _ui.UpdateVersion(remoteSize.HasValue
                        ? $"{entry.Name} {entry.Version} ({remoteSize.Value / 1024.0:F1} KB)"
                        : $"{entry.Name} {entry.Version}");
                }

                // STEP 2: Download Patch
                _logger.LogInformation("Step 2: Downloading patch file.");
                if (!File.Exists(patchPath))
                {
                    _ui.UpdateStatus("Downloading patch...");
                    await _ui.ShowStatusAsync("Downloading patch...", 500, true, cancellationToken);

                    bool ok = await _downloader.DownloadFileAsync(
                        $"{baseUrl}{entry.File}",
                        patchPath,
                        "Downloading patch...",
                        true,
                        null,
                        cancellationToken);

                    if (!ok)
                        throw new Exception("Failed to download patch.");
                }

                // STEP 3: Download Hash
                _logger.LogInformation("Step 3: Downloading hash file.");
                if (!File.Exists(hashPath))
                {
                    await _ui.ShowStatusAsync("Downloading hashes...", 500, true, cancellationToken);

                    bool ok = await _downloader.DownloadFileAsync(
                        $"{entry.SourceUri}{Path.GetFileName(hashPath)}",
                        hashPath,
                        "Downloading hashes...",
                        true,
                        null,
                        cancellationToken);

                    if (!ok)
                        throw new Exception("Failed to download hash file.");
                }

                // STEP 4: Decompress
                _logger.LogInformation("Step 4: Decompressing patch.");
                await _ui.ShowStatusAsync("Unpacking files...", 500, false, cancellationToken);

                List<FileEntry> files = await _decompressor.DecompressAsync(patchPath, cancellationToken);
                if (files.Count == 0)
                    throw new Exception("Decompression yielded no files.");

                // STEP 5: Stage
                _logger.LogInformation("Step 5: Staging files.");
                await _ui.ShowStatusAsync("Staging files...", 500, false, cancellationToken);
                Directory.CreateDirectory(stagingPath);

                bool staged = await _stager.WriteStagingFilesAsync(files, stagingPath, cancellationToken);
                if (!staged)
                    throw new Exception("Failed to stage patch files.");

                // STEP 6: Validate
                _logger.LogInformation("Step 6: Validating staged files.");
                await _ui.ShowStatusAsync("Validating update...", 500, true, cancellationToken);

                bool valid = await _validator.ValidateAsync(stagingPath, hashPath, cancellationToken);
                if (!valid)
                    throw new Exception("Patch validation failed.");

                // STEP 7: Ensure Mids Reborn is closed before backup and install
                await _ui.ShowStatusAsync("Checking for running processes...", 500, false, cancellationToken);

                if (ProcessUtils.IsProcessRunning("MidsReborn"))
                {
                    _logger.LogWarning("Mids Reborn is running. Attempting to shut it down.");
                    await _ui.ShowStatusAsync("Closing Mids Reborn...", 750, false, cancellationToken);

                    bool closed = await ProcessUtils.KillProcessAsync("MidsReborn", cancellationToken);
                    if (!closed)
                    {
                        await _ui.ShowStatusAsync("Please close Mids Reborn and try again.", 1500, false, cancellationToken);
                        return -1;
                    }
                }

                // STEP 8: Backup
                _logger.LogInformation("Step 8: Creating backup.");
                await _ui.ShowStatusAsync("Creating backup...", 500, true, cancellationToken);
                Directory.CreateDirectory(backupPath);

                bool backupOk = await _backupManager.CreateBackupAsync(installPath, backupPath, entry.Type, entry.Name,
                    cancellationToken);
                if (!backupOk)
                    throw new Exception("Failed to create backup.");

                backupMade = true;

                // STEP 9: Pre-Install Cleanup
                _logger.LogInformation("Step 9: Preparing for installation.");
                await _ui.ShowStatusAsync("Preparing installation...", 500, false, cancellationToken);

                // STEP 10: Apply
                _logger.LogInformation("Step 10: Applying patch.");
                await _ui.ShowStatusAsync("Installing update...", 500, true, cancellationToken);
                installing = true;

                bool installed = await _pipeline.RunStepAsync("Apply patch", () =>
                    _stager.ApplyStagedFilesAsync(files, installPath, stagingPath, cancellationToken));
                if (!installed)
                    throw new Exception("Failed to apply patch.");

                // STEP 11: Final Cleanup
                _logger.LogInformation("Step 11: Cleaning up temporary files.");
                await _ui.ShowStatusAsync("Cleaning up...", 250, false, cancellationToken);

                TryDelete(patchPath);
                TryDelete(hashPath);
                TryDelete(stagingPath, recursive: true);

                await _ui.ShowStatusAsync("Update complete.", 1500, false, cancellationToken);
                return 0;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Patch cancelled by user for {Name} {Version} ({Type})", entry.Name, entry.Version, entry.Type);
                _ui.TriggerCancel();
                await _ui.ShowStatusAsync("Update cancelled.", 1000, false, cancellationToken);
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Patch failed for {Name} {Version} ({Type})", entry.Name, entry.Version, entry.Type);
                _ui.TriggerFailure();
                await _ui.ShowStatusAsync("Update failed.", 1500, false, cancellationToken);

                if (installing && backupMade)
                {
                    await RollbackAsync(installPath, backupPath, entry.Type, entry.Name, cancellationToken);
                }

                return -1;
            }
            finally
            {
                TryDelete(patchPath);
                TryDelete(hashPath);
                TryDelete(stagingPath, recursive: true);
                _ui.ShowProgressBar(false);
                _ui.UpdateProgress(0f);
            }
        }
    }

    private async Task RollbackAsync(string installPath, string backupPath, string patchType, string name, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Attempting rollback to {Backup}", backupPath);

        _ui.TriggerRollback();
        await _ui.ShowStatusAsync("Rolling back...", 500, true, cancellationToken);
        _ui.UpdateProgress(0f);

        bool restored = await _backupManager.RestoreBackupAsync(installPath, backupPath, patchType, name, cancellationToken);
        if (restored)
        {
            _logger.LogInformation("Rollback complete for {Name}", name);
            await _ui.ShowStatusAsync("Rollback complete.", 1500, false, cancellationToken);
        }
        else
        {
            _logger.LogError("Rollback failed for {Name}", name);
            await _ui.ShowStatusAsync("Rollback failed.", 1500, false, cancellationToken);
        }
    }

    private static void TryDelete(string path, bool recursive = false)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
            else if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup
        }
    }
}