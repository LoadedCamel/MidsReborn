namespace MRB_Boostrap.Services;

public interface IHashValidator
{
    Task<bool> ValidateAsync(string stagingPath, string hashPath, CancellationToken cancellationToken);
}