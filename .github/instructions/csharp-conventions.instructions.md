---
description: "Use when writing, reviewing, or extending C# code in this repo. Covers .NET 10 conventions, BackgroundService patterns, Channel usage, logging, and build quality rules."
applyTo: "**/*.cs"
---

# C# Conventions for backgroundJobs101

## Target & Build

- Target `net10.0`; SDK version pinned via `global.json` (`rollForward: latestPatch`).
- `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>` are on — never disable them.
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` and `<AnalysisLevel>latest-recommended</AnalysisLevel>` — all code must compile warning-free.
- Suppress only noisy demo-level diagnostics (CA1848, CA1873, CA1805) via `<NoWarn>` in the `.csproj`; never suppress per-site with `#pragma`.

## Language Features

- Use **primary constructors** (C# 12) for constructor injection — no explicit constructor body needed.
- Use **file-scoped namespaces** — no braces, one namespace per file.
- Use `record` for simple immutable data carriers (e.g., `ChannelRequest`).
- Mark classes `sealed` when they have no intended subtypes and are not externally visible.
- Prefer `internal` visibility for types that are not part of the public API.

## BackgroundService Pattern

- All hosted services inherit `BackgroundService` and override `ExecuteAsync(CancellationToken stoppingToken)`.
- **Timed work**: use `PeriodicTimer` + `WaitForNextTickAsync(stoppingToken)` — not `System.Threading.Timer`, `Task.Delay` loops, or manual `IDisposable` timers.

  ```csharp
  using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
  while (await timer.WaitForNextTickAsync(stoppingToken)) { ... }
  ```

- **Graceful shutdown**: catch `OperationCanceledException` (the correct base type — `TaskCanceledException` is a subclass). Never swallow it silently.

  ```csharp
  catch (OperationCanceledException)
  {
      logger.LogInformation("Service is stopping.");
  }
  ```

- **Scoped services from singleton hosted services**: always create a fresh DI scope via `services.CreateScope()` and resolve from `scope.ServiceProvider` — never inject scoped services directly into a singleton.

  ```csharp
  using var scope = services.CreateScope();
  var svc = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();
  await svc.DoWork(stoppingToken);
  ```

## Thread Safety

- Use `Interlocked.Increment(ref _field)` for thread-safe counters — no locks for simple increment.
- Never use `_field++` or `_field += 1` on a shared mutable integer in a multi-threaded context.

## Channel Usage

- Create channels with `Channel.CreateUnbounded<T>` and configure `SingleReader`, `SingleWriter`, and `AllowSynchronousContinuations = false` explicitly.

  ```csharp
  Channel.CreateUnbounded<ChannelRequest>(new UnboundedChannelOptions
  {
      SingleReader = true,
      SingleWriter = true,
      AllowSynchronousContinuations = false
  });
  ```

- Drain channels using `ReadAllAsync` (idiomatic `IAsyncEnumerable`) — not manual `ReadAsync` loops.

  ```csharp
  await foreach (var item in channel.Reader.ReadAllAsync(stoppingToken)) { ... }
  ```

## Logging

- Use structured logging with **named placeholders** — never string interpolation inside `Log*` calls.

  ```csharp
  // Correct
  logger.LogInformation("Count: {Count}", count);

  // Wrong
  logger.LogInformation($"Count: {count}");
  ```

- Log service lifecycle events at `LogInformation`: "running", "is working", "is stopping".

## Minimal API (Program.cs)

- Use `TypedResults.*` (e.g., `TypedResults.Ok()`) instead of `Results.*` for endpoint return values — it provides compile-time type safety.
- Register hosted services with `AddHostedService<T>()`.
- Register channels as singletons via `AddSingleton(_ => Channel.CreateUnbounded<T>(...))`.
- Register scoped services by interface: `AddScoped<IFoo, Foo>()`.
