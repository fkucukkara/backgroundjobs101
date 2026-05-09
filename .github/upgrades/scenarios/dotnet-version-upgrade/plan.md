# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade `backgroundJobs101` solution from .NET 9 to .NET 10
**Scope**: 1 project (`src/API/API.csproj`), small solution

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 1 project on .NET 9, no package risks, one potential API compatibility fix needed.

## Tasks

### 01-sdk-validation: Validate .NET 10 SDK Installation

Verify the .NET 10 SDK is installed and any global.json constraints are compatible with net10.0. This is a prerequisite before modifying project files.

**Done when**: .NET 10 SDK confirmed installed; global.json (if present) is compatible or updated.

---

### 02-upgrade-api: Upgrade API Project to net10.0

Update `src/API/API.csproj` target framework from `net9.0` to `net10.0`. Address the source-incompatible API usage in `src/API/BackgroundProcessors/HostedService.cs` — `TimeSpan.FromSeconds(int)` overload changed in .NET 10; the call at line 13 must be updated to use a compatible overload or a long literal.

Build the solution after changes and fix any remaining compilation errors.

**Done when**: `API.csproj` targets `net10.0`, solution builds with 0 errors, `HostedService.cs` compiles cleanly.

---

### 03-test-and-commit: Validate and Commit

Run any available tests to confirm runtime behavior is unaffected. Commit all upgrade changes to the `upgrade-to-NET10` branch.

**Done when**: Build succeeds, tests pass (or no tests exist), changes committed.
