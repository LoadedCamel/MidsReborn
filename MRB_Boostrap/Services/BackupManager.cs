using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public sealed class BackupManager : IBackupManager
{
    private readonly ILogger<BackupManager> _logger;
    private readonly IFileCompressor _compressor;
    private readonly IFileDecompressor _decompressor;

    public BackupManager(ILogger<BackupManager> logger, IFileCompressor compressor, IFileDecompressor decompressor)
    {
        _logger = logger;
        _compressor = compressor;
        _decompressor = decompressor;
    }

    public async Task<bool> CreateBackupAsync(string installPath, string backupPath, string patchType, string dbName, CancellationToken cancellationToken)
    {
        try
        {
            List<FileEntry> entries = [];

            if (patchType.Equals("Application", StringComparison.OrdinalIgnoreCase))
            {
                string[] excluded = [
                    "MRBBootstrap.exe", "Logs",
                "Staging", "Backup", "Data", "Patches"
                ];

                foreach (string file in Directory.EnumerateFiles(installPath, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string rel = Path.GetRelativePath(installPath, file);
                    if (excluded.Any(ex => rel.StartsWith(ex, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    entries.Add(new FileEntry
                    {
                        FileName = Path.GetFileName(file),
                        Directory = Path.GetDirectoryName(rel) ?? string.Empty,
                        Data = await File.ReadAllBytesAsync(file, cancellationToken)
                    });
                }
            }
            else if (patchType.Equals("Database", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(dbName))
            {
                string sourceDb = installPath;

                if (!Directory.Exists(sourceDb))
                {
                    _logger.LogError("Database source directory does not exist: {Path}", sourceDb);
                    return false;
                }

                foreach (string file in Directory.EnumerateFiles(sourceDb, "*", SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string rel = Path.GetRelativePath(sourceDb, file);

                    entries.Add(new FileEntry
                    {
                        FileName = Path.GetFileName(file),
                        Directory = Path.GetDirectoryName(rel) ?? string.Empty,
                        Data = await File.ReadAllBytesAsync(file, cancellationToken)
                    });
                }
            }

            if (entries.Count == 0)
            {
                _logger.LogWarning("No files found to backup from {Source}", installPath);
                return false;
            }

            string bakPath = Path.ChangeExtension(backupPath, ".mrbak");

            // Remove old backup if it exists
            if (File.Exists(bakPath))
            {
                try
                {
                    File.Delete(bakPath);
                    _logger.LogInformation("Previous backup deleted: {Path}", bakPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old backup: {Path}", bakPath);
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(bakPath)!);

            bool ok = await _compressor.CompressAsync(entries, bakPath, cancellationToken);
            if (!ok)
            {
                _logger.LogError("Failed to write compressed backup to {Path}", bakPath);
                return false;
            }

            _logger.LogInformation("Compressed backup created at {Path}", bakPath);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Backup creation cancelled.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create backup.");
            return false;
        }
    }

    public async Task<bool> RestoreBackupAsync(string installPath, string backupPath, string patchType, string name, CancellationToken cancellationToken)
    {
        try
        {
            string bakFile = Path.ChangeExtension(backupPath, ".mrbak");

            if (!File.Exists(bakFile))
            {
                _logger.LogError("Missing backup file: {Path}", bakFile);
                return false;
            }

            List<FileEntry> entries = await _decompressor.DecompressAsync(bakFile, cancellationToken);
            if (entries.Count == 0)
            {
                _logger.LogError("Backup decompression yielded no files.");
                return false;
            }

            // Clean existing install path
            if (patchType.Equals("Application", StringComparison.OrdinalIgnoreCase))
            {
                string[] excluded = [
                    "MRBBootstrap.exe", "Logs",
                "Staging", "Backup", "Data", "Patches"
                ];

                foreach (string file in Directory.EnumerateFiles(installPath, "*", SearchOption.AllDirectories))
                {
                    string rel = Path.GetRelativePath(installPath, file);
                    if (excluded.Any(ex => rel.StartsWith(ex, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    try { File.Delete(file); }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else if (patchType.Equals("Database", StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists(installPath))
                {
                    try { Directory.Delete(installPath, recursive: true); }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete existing database directory: {Path}", installPath);
                    }
                }
            }

            // Write restored files
            foreach (var file in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (file.Directory.Equals("Logs", StringComparison.OrdinalIgnoreCase))
                    continue;

                string targetFile = Path.Combine(installPath, file.Directory, file.FileName);
                string? targetDir = Path.GetDirectoryName(targetFile);

                if (!string.IsNullOrEmpty(targetDir))
                    Directory.CreateDirectory(targetDir);

                await File.WriteAllBytesAsync(targetFile, file.Data, cancellationToken);
            }

            _logger.LogInformation("Backup restored from {Path}", bakFile);

            // Delete backup after restoration
            try
            {
                File.Delete(bakFile);
                _logger.LogInformation("Deleted backup after restore: {Path}", bakFile);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete backup after restore: {Path}", bakFile);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Restore cancelled.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Restore failed.");
            return false;
        }
    }
}