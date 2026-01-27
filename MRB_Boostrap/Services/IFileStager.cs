using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public interface IFileStager
{
    Task<bool> WriteStagingFilesAsync(List<FileEntry> files, string stagingPath, CancellationToken cancellationToken);
    Task<bool> ApplyStagedFilesAsync(List<FileEntry> files, string installPath, string stagingPath, CancellationToken cancellationToken);
}