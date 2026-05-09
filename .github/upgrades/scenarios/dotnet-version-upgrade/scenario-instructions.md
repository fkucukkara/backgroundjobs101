# Scenario Instructions: .NET Version Upgrade

## Scenario
Upgrade solution from .NET 9 to .NET 10.

## Parameters
- **Solution**: `backgroundJobs101.sln`
- **Target Framework**: `net10.0`
- **Source Branch**: `master`
- **Working Branch**: `upgrade-to-NET10`

## Preferences
### Flow Mode
Automatic — run end-to-end, pause only when blocked.

## Strategy
**Selected**: All-At-Once
**Rationale**: 1 project on net9.0, straightforward TFM bump with one API fix needed.

### Execution Constraints
- Single atomic upgrade — all projects updated together
- Validate full solution build after upgrade before running tests
- Fix all compilation errors in one bounded pass

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task

## Key Decisions Log
- 2024: User confirmed upgrade to .NET 10.0 (LTS) on branch `upgrade-to-NET10`
