# 02-upgrade-api: Upgrade API Project to net10.0

Update `src/API/API.csproj` target framework from `net9.0` to `net10.0`. Address the source-incompatible API usage in `src/API/BackgroundProcessors/HostedService.cs` — `TimeSpan.FromSeconds(int)` overload changed in .NET 10; the call at line 13 must be updated to use a compatible overload or a long literal.

Build the solution after changes and fix any remaining compilation errors.

**Done when**: `API.csproj` targets `net10.0`, solution builds with 0 errors, `HostedService.cs` compiles cleanly.
