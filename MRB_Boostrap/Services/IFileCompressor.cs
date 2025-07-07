using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public interface IFileCompressor
{
    Task<bool> CompressAsync(List<FileEntry> files, string outputPath, CancellationToken cancellationToken);
}