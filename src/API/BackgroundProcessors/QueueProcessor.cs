using System.Threading.Channels;

namespace API.BackgroundProcessors;

public class QueueProcessor : BackgroundService
{
    private readonly ILogger<QueueProcessor> _logger;
    private readonly Channel<ChannelRequest> _channel;

    public QueueProcessor(Channel<ChannelRequest> channel, ILogger<QueueProcessor> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is running.");
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _channel.Reader.ReadAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken);
                Console.WriteLine(workItem.Message);
            }
        }
        catch (Exception e) when (e is OperationCanceledException || e is TaskCanceledException)
        {
            _logger.LogInformation("Task was canceled (QueueProcessor).");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}

public record ChannelRequest(string Message);
