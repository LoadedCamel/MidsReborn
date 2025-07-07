using MRB_Boostrap.Models;

namespace MRB_Boostrap.Services;

public interface IPatchFlowManager
{
    Task<int> ExecutePatchFlowAsync(UpdateEntry entry, CancellationToken cancellationToken = default);
}