namespace API.BackgroundProcessors;

internal interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}

internal class ScopedProcessingService : IScopedProcessingService
{
    private int executionCount = 0;
    private readonly ILogger _logger;

    public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                executionCount++;
                _logger.LogInformation("Scoped Processing Service is working. Count: {Count}", executionCount);
                await Task.Delay(5000, stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task was canceled (ScopedProcessingService).");
        }
    }
}