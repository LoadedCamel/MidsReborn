namespace MRB_Boostrap.Services;

public interface IBackupManager
{
    Task<bool> CreateBackupAsync(string installPath, string backupPath, string patchType, string dbName, CancellationToken cancellationToken);
    Task<bool> RestoreBackupAsync(string installPath, string backupPath, string patchType, string dbName, CancellationToken cancellationToken);
}