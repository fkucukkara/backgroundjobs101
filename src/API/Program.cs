using API.BackgroundProcessors;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<HostedService>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
builder.Services.AddHostedService<ConsumeScopedServiceHostedService>();

builder.Services.AddHostedService<QueueProcessor>();
builder.Services.AddSingleton<Channel<ChannelRequest>>(
    _ => Channel.CreateUnbounded<ChannelRequest>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = false
        }));

var app = builder.Build();

app.MapGet("/push", async (Channel<ChannelRequest> channel) =>
{
    await channel.Writer.WriteAsync(new ChannelRequest($"Hello, Channel {DateTime.UtcNow}!"));
    return TypedResults.Ok();
});

app.Run();
