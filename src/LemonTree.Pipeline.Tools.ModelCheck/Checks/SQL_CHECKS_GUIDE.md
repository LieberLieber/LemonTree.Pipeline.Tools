# SQL-Based Checks Implementation Guide

## Overview

The `LemonTree.Pipeline.Tools.ModelCheck` now uses a standardized, declarative SQL-based check system instead of hardcoded check implementations. This makes the system more maintainable, testable, and easier to extend.

## Architecture

### Core Components

1. **SqlCheck Class** (`Checks/SqlCheck.cs`)
   - Represents a single check configuration with SQL query and messages
   - Properties:
     - `Id`: Unique identifier for the check
     - `Query`: SQL query returning a count (scalar value)
     - `PassedTitle/Detail`: Messages when count == 0
     - `FailedTitle/Detail`: Messages when count > 0
     - `PassedLevel/FailedLevel`: Issue severity levels for each outcome
     - `IncludeCountInTitle`: Whether to insert count into the failed title

2. **SqlCheckRegistry** (`Checks/SqlCheckRegistry.cs`)
   - Central registry managing all available checks and their execution order
   - `GetCheckOrder()`: Returns the ordered list of check IDs to execute
   - `InitializeChecks()`: Defines all SQL-based checks with their queries and messages
   - `ExecuteCheck(checkId)`: Execute a single check by ID
   - `ExecuteAllSqlChecks()`: Execute all checks in order and return results
   - Handles database-specific wildcard replacement

3. **Checks.cs** (Refactored)
   - `ExecuteAllSqlChecks()`: Public method to run all SQL-based checks using registry's defined order
   - Individual `Check*()` methods delegate to `SqlCheckRegistry.ExecuteCheck()` for backward compatibility
   - Maintains backward compatibility with existing code

4. **Program.cs** (Updated)
   - Uses `Checks.ExecuteAllSqlChecks()` to execute all SQL checks via a foreach approach
   - Non-SQL checks (Compact, Statistics, Orphans) are executed separately with their individual options

## Adding a New Check

To add a new SQL-based check:

1. Open `SqlCheckRegistry.cs`

2. Add the check ID to `InitializeCheckOrder()` list in the desired position:
```csharp
private static List<string> InitializeCheckOrder()
{
    return new List<string>
    {
        "DiagramImagemaps",
        // ... add your check ID in the correct position
        "UniqueCheckId",
        // ...
    };
}
```

3. Add the check definition to `InitializeChecks()` method:
```csharp
new SqlCheck
{
    Id = "UniqueCheckId",
    Query = "SELECT Count(*) FROM some_table WHERE some_condition",
    PassedTitle = "Title when check passes (count == 0)",
    FailedTitle = "Title when check fails (count > 0), use {count} placeholder",
    PassedDetail = null,  // or a string with explanation
    FailedDetail = "Explanation of why this is a problem",
    PassedLevel = IssueLevel.Passed,
    FailedLevel = IssueLevel.Warning,  // or Error, Information
    IncludeCountInTitle = true  // Set to false if no {count} in FailedTitle
}
```

4. (Optional) Add a delegating method to `Checks.cs` for direct access:
```csharp
internal static Issue CheckMyCheck(string model)
{
    return SqlCheckRegistry.ExecuteCheck("UniqueCheckId");
}
```

Note: Checks are executed in the order defined in `GetCheckOrder()`. The order matters for report readability and logical grouping of related checks.

## Execution Flow

The check execution now uses a centralized foreach approach:

1. **Program.cs** calls `Checks.ExecuteAllSqlChecks()`
2. **Checks.cs** delegates to `SqlCheckRegistry.ExecuteAllSqlChecks()`
3. **SqlCheckRegistry** iterates through `GetCheckOrder()` and executes each check in sequence
4. Results are collected and returned as an `Issues` collection

This approach ensures:
- Consistent, predictable execution order
- Easy reordering of checks without code changes
- Centralized control of check sequence

## Database-Specific Wildcards

Some database operations require different wildcard syntax (e.g., `%` for SQL Server, `*` for SQLite). The registry handles this automatically by replacing `{wildcard}` placeholders in queries:

```csharp
Query = "SELECT Count(*) FROM t_genopt WHERE Option LIKE '{wildcard}enabled=1;{wildcard}'"
```

The registry replaces `{wildcard}` with the value returned by `ModelAccess.GetWildcard()`.

## Benefits of Standardized Implementation

- **Maintainability**: Check logic is centralized and consistent
- **Extensibility**: Adding new checks requires only configuration, not coding
- **Testability**: Each check is isolated and can be tested independently
- **Readability**: Clear separation of concern (definition vs. execution)
- **Consistency**: All checks follow the same patterns for titles, details, and levels

## Current Checks

All the following checks have been migrated to the SQL-based system:

| Check ID | Purpose | Severity on Fail |
|----------|---------|-----------------|
| DiagramImagemaps | Detects DIAGRAMIMAGEMAP entries | Information |
| TImages | Detects binary image data | Information |
| Baselines | Detects baseline documents | Warning |
| ExtDoc | Detects embedded binary files | Warning |
| ModelDocuments | Detects ModelDocument entries | Warning |
| AuditLogs | Detects audit snapshots | Error |
| Journal | Detects journal entries | Error |
| AuditingEnabled | Checks if auditing is enabled | Error |
| ResourceAllocation | Detects resource allocations | Error |
| UserSecurity | Checks if user security is enabled | Warning |
| VCSConnection | Detects VCS-enabled packages | Warning |

## Non-SQL Checks

The following checks remain in `Checks.cs` as they require complex logic:

- `CheckCompact()`: Performs model compaction and analyzes file size ratios
- `CheckStrippedCompact()`: Strips binary content and measures compaction
- `CheckProjectStatistics()`: Generates project statistics from complex SQL
- `CheckTableSize()`: Analyzes database table sizes
- `CheckModelOrphans()`: Detects orphaned objects with complex logic

These checks cannot be easily expressed as simple count-based SQL queries.
