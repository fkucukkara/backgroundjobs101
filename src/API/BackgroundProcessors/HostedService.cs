namespace API.BackgroundProcessors;

public class HostedService(ILogger<HostedService> logger) : BackgroundService
{
    private int _executionCount;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Timed Hosted Service running.");

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var count = Interlocked.Increment(ref _executionCount);
            logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }

        logger.LogInformation("Timed Hosted Service is stopping.");
    }
}
