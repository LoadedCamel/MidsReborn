using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public sealed class FileCompressor : IFileCompressor
{
    private readonly ILogger<FileCompressor> _logger;

    public FileCompressor(ILogger<FileCompressor> logger)
    {
        _logger = logger;
    }

    public async Task<bool> CompressAsync(List<FileEntry> files, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            // Write patch-style header
            writer.Write("Mids Reborn Backup Data");
            writer.Write(files.Count);

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                writer.Write(file.Data.Length);
                writer.Write(file.FileName);
                writer.Write(file.Directory);
                writer.Write(file.Data);
            }

            memoryStream.Position = 0;

            await using var output = File.Create(outputPath);
            await using var deflater = new DeflaterOutputStream(output, new Deflater(9));

            await memoryStream.CopyToAsync(deflater, cancellationToken);
            await deflater.FinishAsync(cancellationToken);

            _logger.LogInformation("Compressed backup written to {Path}", outputPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to compress backup file: {Path}", outputPath);
            return false;
        }
    }
}