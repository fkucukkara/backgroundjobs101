# BackgroundJobs101

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4) ![SDK](https://img.shields.io/badge/SDK-10.0.203-512BD4)

This project demonstrates the usage of different types of hosted services in an ASP.NET Core application targeting **.NET 10**. It covers timed background work, scoped service consumption, and queue processing via `System.Threading.Channels`.

## Project Structure

- **API/Program.cs**: Configures the web application and registers all hosted services and the unbounded channel.
- **API/BackgroundProcessors/HostedService.cs**: Timed background service using `PeriodicTimer` — async, cancellation-aware, no manual `Dispose`.
- **API/BackgroundProcessors/ScopedProcessingService.cs**: Scoped service that loops until the cancellation token is triggered, with thread-safe counter via `Interlocked.Increment`.
- **API/BackgroundProcessors/ConsumeScopedServiceHostedService.cs**: Background service that creates a DI scope and runs `ScopedProcessingService` inside it.
- **API/BackgroundProcessors/QueueProcessor.cs**: Background service that drains a `Channel<ChannelRequest>` using `ReadAllAsync`, processing one message per second.

## Requirements

- [.NET 10 SDK (10.0.203+)](https://dotnet.microsoft.com/download/dotnet/10.0)

## How to Run

1. Clone the repository:
    ```sh
    git clone https://github.com/fkucukkara/backgroundjobs101.git
    cd src\API
    ```

2. Build and run the application:
    ```sh
    dotnet build
    dotnet run
    ```

3. Push a message to the queue:
    ```sh
    GET /push
    ```

## Services

### HostedService

A timed background service that logs a counter every 5 seconds. Uses [`PeriodicTimer`](https://learn.microsoft.com/dotnet/api/system.threading.periodictimer) (introduced in .NET 6) instead of `System.Threading.Timer`, which provides a clean `await`-able tick loop and safe automatic disposal — no `IDisposable` or manual timer teardown required.

```csharp
using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
while (await timer.WaitForNextTickAsync(stoppingToken)) { ... }
```

### ScopedProcessingService

A scoped service (`IScopedProcessingService`) that loops with a 5-second delay until cancellation. Key improvements:
- Uses `Interlocked.Increment` for a thread-safe execution counter.
- Catches `OperationCanceledException` (the correct base type — `TaskCanceledException` is a subclass).
- Declared `sealed` since it has no subtypes and is not externally visible.

### ConsumeScopedServiceHostedService

A background service that resolves `IScopedProcessingService` from a fresh DI scope on each run. Demonstrates the recommended pattern for consuming **scoped** services from a **singleton** hosted service:

```csharp
using var scope = services.CreateScope();
var svc = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();
await svc.DoWork(stoppingToken);
```

### QueueProcessor

Reads `ChannelRequest` messages from an `UnboundedChannel<ChannelRequest>` (configured with `SingleReader = true`, `SingleWriter = true`) and processes them one at a time. Uses `ReadAllAsync` for idiomatic async-stream consumption:

```csharp
await foreach (var workItem in channel.Reader.ReadAllAsync(stoppingToken))
{
    await Task.Delay(1000, stoppingToken);
    logger.LogInformation("Processing message: {Message}", workItem.Message);
}
```

Messages are produced via the `GET /push` endpoint registered in `Program.cs`.

## Design Decisions

| Area | Choice | Reason |
|---|---|---|
| Timed work | `PeriodicTimer` | Async-native, cancellation-aware, no overlapping ticks |
| Channel drain | `ReadAllAsync` | Idiomatic `IAsyncEnumerable` — cleaner than manual `ReadAsync` loop |
| Counter safety | `Interlocked.Increment` | Thread-safe without a lock |
| Exception handling | `OperationCanceledException` | Correct base; catches `TaskCanceledException` too |
| Build quality | `TreatWarningsAsErrors` + `AnalysisLevel=latest-recommended` | Catches regressions at compile time |

## License
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

This project is licensed under the MIT License, which allows you to freely use, modify, and distribute the code. See the [`LICENSE`](LICENSE) file for full details.
