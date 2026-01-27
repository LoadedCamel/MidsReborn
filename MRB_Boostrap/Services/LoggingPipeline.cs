using Microsoft.Extensions.Logging;

namespace MRB_Boostrap.Services;

public sealed class LoggingPipeline
{
    private readonly ILogger<LoggingPipeline> _logger;

    public LoggingPipeline(ILogger<LoggingPipeline> logger)
    {
        _logger = logger;
    }

    public async Task<bool> RunStepAsync(string stepName, Func<Task<bool>> action)
    {
        _logger.LogInformation("Starting: {Step}", stepName);

        try
        {
            bool result = await action();
            if (result)
            {
                _logger.LogInformation("Success: {Step}", stepName);
            }
            else
            {
                _logger.LogWarning("Failed: {Step}", stepName);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Cancelled during: {Step}", stepName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during: {Step}", stepName);
            return false;
        }
    }

    public async Task<T?> RunStepAsync<T>(string stepName, Func<Task<T>> action, Func<T, bool>? successPredicate = null)
    {
        _logger.LogInformation("Starting: {Step}", stepName);

        try
        {
            T result = await action();
            bool success = successPredicate?.Invoke(result) ?? true;

            if (success)
                _logger.LogInformation("Success: {Step}", stepName);
            else
                _logger.LogWarning("Failed (predicate): {Step}", stepName);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Cancelled during: {Step}", stepName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during: {Step}", stepName);
            return default;
        }
    }
}