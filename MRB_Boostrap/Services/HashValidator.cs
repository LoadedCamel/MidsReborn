using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using MRB_Boostrap.UI;

namespace MRB_Boostrap.Services;

public sealed class HashValidator : IHashValidator
{
    private readonly ILogger<HashValidator> _logger;
    private readonly IUIManager _ui;

    private record HashEntry(string Directory, string FileName, string Hash);

    public HashValidator(ILogger<HashValidator> logger, IUIManager ui)
    {
        _logger = logger;
        _ui = ui;
    }

    public async Task<bool> ValidateAsync(string stagingPath, string hashPath, CancellationToken cancellationToken)
    {
        if (!File.Exists(hashPath))
        {
            _logger.LogError("Hash file missing: {Path}", hashPath);
            _ui.UpdateStatus("Hash file not found.");
            return false;
        }

        List<HashEntry>? entries;
        try
        {
            string json = await File.ReadAllTextAsync(hashPath, cancellationToken);
            entries = JsonSerializer.Deserialize<List<HashEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read or parse hash file.");
            _ui.UpdateStatus("Invalid hash file.");
            return false;
        }

        if (entries == null || entries.Count == 0)
        {
            _logger.LogWarning("Hash file is empty or invalid: {Path}", hashPath);
            _ui.UpdateStatus("Invalid hash file.");
            return false;
        }

        _ui.UpdateStatus("Validating files...");
        _ui.ShowProgressBar(true);
        _ui.UpdateProgress(0f);

        int total = entries.Count;
        for (int i = 0; i < total; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HashEntry entry = entries[i];
            string fullPath = Path.Combine(stagingPath, entry.Directory ?? "", entry.FileName ?? "");

            if (!File.Exists(fullPath))
            {
                _logger.LogError("Validation failed: Missing file '{File}' in staging path '{Staging}'", entry.FileName, stagingPath);
                _ui.UpdateStatus("Validation failed.");
                return false;
            }

            string actualHash = await ComputeSha256Async(fullPath, cancellationToken);

            if (!actualHash.Equals(entry.Hash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Validation failed: Hash mismatch for file '{File}' in directory '{Dir}'", entry.FileName, entry.Directory);
                _logger.LogError("  Expected: {Expected}", entry.Hash);
                _logger.LogError("  Actual  : {Actual}", actualHash);
                _ui.UpdateStatus("Validation failed (hash mismatch).");
                return false;
            }

            _ui.UpdateProgress((float)(i + 1) / total);
        }

        _logger.LogInformation("Validation passed for all files.");
        return true;
    }

    private static async Task<string> ComputeSha256Async(string filePath, CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(filePath);
        using var sha256 = SHA256.Create();
        byte[] hash = await sha256.ComputeHashAsync(stream, cancellationToken);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}