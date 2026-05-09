using System.Threading.Channels;

namespace API.BackgroundProcessors;

public class QueueProcessor(Channel<ChannelRequest> channel, ILogger<QueueProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is running.");

        try
        {
            await foreach (var workItem in channel.Reader.ReadAllAsync(stoppingToken))
            {
                await Task.Delay(1000, stoppingToken);
                logger.LogInformation("Processing message: {Message}", workItem.Message);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}

public record ChannelRequest(string Message);
