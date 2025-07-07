using ICSharpCode.SharpZipLib.Zip.Compression;
using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public sealed class FileDecompressor : IFileDecompressor
{
    private readonly ILogger<FileDecompressor> _logger;

    public FileDecompressor(ILogger<FileDecompressor> logger)
    {
        _logger = logger;
    }

    public async Task<List<FileEntry>> DecompressAsync(string filePath, CancellationToken cancellationToken)
    {
        var result = new List<FileEntry>();

        if (!File.Exists(filePath))
        {
            _logger.LogError("File not found: {Path}", filePath);
            return result;
        }

        _logger.LogInformation("Reading compressed file: {Path}", filePath);

        byte[] compressed;
        try
        {
            compressed = await File.ReadAllBytesAsync(filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file: {Path}", filePath);
            return result;
        }

        var inflater = new Inflater();
        inflater.SetInput(compressed);

        using var uncompressed = new MemoryStream();
        byte[] buffer = new byte[8192];

        try
        {
            while (!inflater.IsFinished)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int count = inflater.Inflate(buffer);
                if (count == 0 && inflater.IsNeedingInput)
                    break;

                uncompressed.Write(buffer, 0, count);
            }

            if (!inflater.IsFinished)
                _logger.LogWarning("Inflater finished prematurely. Output may be incomplete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inflater failed during decompression of {Path}", filePath);
            return result;
        }

        uncompressed.Position = 0;

        try
        {
            using var reader = new BinaryReader(uncompressed);

            string magic = ReadDotNetString(reader);
            if (magic != "Mids Reborn Patch Data" && magic != "Mids Reborn Backup Data")
            {
                _logger.LogError("Invalid header: {Header} in file {Path}", magic, filePath);
                return result;
            }

            uint fileCount = reader.ReadUInt32();
            _logger.LogInformation("Expecting {Count} file entries in: {Path}", fileCount, filePath);

            for (uint i = 0; i < fileCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    uint dataLength = reader.ReadUInt32();
                    string fileName = ReadDotNetString(reader);
                    string directory = NormalizeDirectory(ReadDotNetString(reader));
                    byte[] data = reader.ReadBytes((int)dataLength);

                    result.Add(new FileEntry
                    {
                        FileName = fileName,
                        Directory = directory,
                        Data = data
                    });

                    _logger.LogInformation("Parsed file [{Index}/{Total}]: {Dir}/{File} ({Size:N0} bytes)",
                        i + 1, fileCount, directory, fileName, data.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing file entry {Index}", i);
                    break;
                }
            }
            if (result.Count != fileCount)
            {
                _logger.LogWarning("Parsed only {Count} of {Expected} files from: {Path}", result.Count, fileCount, filePath);
            }
            else
            {
                _logger.LogInformation("Decompression complete. Parsed {Count} files from: {Path}", result.Count, filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading decompressed data from {Path}", filePath);
        }

        return result;
    }

    private static string NormalizeDirectory(string dir) =>
        dir.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);

    private static string ReadDotNetString(BinaryReader reader)
    {
        int length = Read7BitEncodedInt(reader);
        if (length < 0 || length > 65536)
            throw new InvalidDataException($"Invalid string length: {length}");

        byte[] bytes = reader.ReadBytes(length);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    private static int Read7BitEncodedInt(BinaryReader reader)
    {
        int result = 0;
        int shift = 0;

        for (int i = 0; i < 5; i++)
        {
            byte b = reader.ReadByte();
            result |= (b & 0x7F) << shift;
            if ((b & 0x80) == 0)
                return result;

            shift += 7;
        }

        return -1;
    }
}