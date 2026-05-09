namespace API.BackgroundProcessors;

internal interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}

internal sealed class ScopedProcessingService(ILogger<ScopedProcessingService> logger) : IScopedProcessingService
{
    private int _executionCount;

    public async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var count = Interlocked.Increment(ref _executionCount);
                logger.LogInformation("Scoped Processing Service is working. Count: {Count}", count);
                await Task.Delay(5000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Scoped Processing Service is stopping.");
        }
    }
}