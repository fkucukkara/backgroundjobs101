namespace API.BackgroundProcessors;

public class ConsumeScopedServiceHostedService(
    IServiceProvider services,
    ILogger<ConsumeScopedServiceHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Consume Scoped Service Hosted Service running.");
        logger.LogInformation("Consume Scoped Service Hosted Service is working.");

        using var scope = services.CreateScope();
        var scopedProcessingService =
            scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

        await scopedProcessingService.DoWork(stoppingToken);

        logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");
    }
}
