
## [2026-05-09 11:34] 01-sdk-validation

.NET 10 SDK confirmed installed. global.json updated with `sdk.rollForward=latestMajor` for net10.0 compatibility.


## [2026-05-09 11:38] 02-upgrade-api

Updated `API.csproj` TargetFramework from net9.0 to net10.0. Fixed `TimeSpan.FromSeconds(5)` → `TimeSpan.FromSeconds(5.0)` in HostedService.cs to resolve .NET 10 overload ambiguity. Build succeeded. Changes committed.

