using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public interface IFileDecompressor
{
    Task<List<FileEntry>> DecompressAsync(string filePath, CancellationToken cancellationToken);
}