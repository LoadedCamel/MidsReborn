namespace MRB_Boostrap.Services;

public interface IRestoreFlowManager
{
    Task<int> ExecuteRestoreAsync(string name, string patchType, CancellationToken cancellationToken);
}