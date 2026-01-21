# Copilot Instructions for LemonTree.Pipeline.Tools

## Project Overview
- **LemonTree.Pipeline.Tools** is a suite of command-line utilities for working with Enterprise Architect models and LemonTree session files.
- The solution contains multiple tools, each in its own subdirectory under `src/`:
  - `LemonTree.Pipeline.Tools.ModelCheck`: Validates models for LemonTree readiness, outputs Markdown reports, and sets exit codes based on issues found.
  - `LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams`: Cleans up DIAGRAMIMAGEMAPS from EA models.
  - `LemonTree.Pipeline.Tools.SetFilterInSessionFile`: Modifies filters in LemonTree session files.

## Key Workflows
- **Build**: Use the solution file `src/LemonTree.Pipeline.Tools.sln` to build all tools. Standard .NET build commands apply.
- **Run Tools**: Each tool is a standalone .exe. See the [README.md](../README.md) for download and usage examples.
- **Typical usage** (from PowerShell):
  ```powershell
  .\LemonTree.Pipeline.Tools.ModelCheck.exe --model "model.eapx" --out ".\output.md" --FailOnErrors --FailOnWarnings
  .\LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe .\DemoModel.eapx
  .\LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe SampleCompareSession.ltsfs "#Conflicted" "$HideGraphicalChanges"
  ```
- **Exit Codes**: ModelCheck returns specific exit codes for error/warning handling (see README for mapping).

## Project Structure & Patterns
- **Each tool** is a separate C# console project with its own `Program.cs` and `CommandLineOptions/` for argument parsing.
- **Database access** is abstracted in `Database/` (see `IEADatabase.cs`, `SqLiteDatabase.cs`).
- **Checks for ModelCheck** use a standardized SQL-based system:
  - `SqlCheck` class: Declarative check configuration with SQL query and messages
  - `SqlCheckRegistry`: Central registry defining all 11 standardized checks
  - Individual `Check*()` methods in `Checks.cs` delegate to the registry
  - See [SQL_CHECKS_GUIDE.md](src/LemonTree.Pipeline.Tools.ModelCheck/Checks/SQL_CHECKS_GUIDE.md) for extending checks
- **No test projects** are present; validation is manual via tool execution and output inspection.


## Conventions & Integration
- **Command-line arguments** are parsed using custom classes in `CommandLineOptions/`.
- **Output** is typically Markdown or direct model file modification.
- **CI/CD Integration**: The workflow `.github/workflows/TestModelCheck.yml` automates validation of models using the ModelCheck tool. It:
  - Downloads platform-specific ModelCheck binaries and test models as artifacts.
  - Runs ModelCheck on both Linux and Windows runners, using the provided model path and expected exit code.
  - Summarizes results and output in the GitHub Actions summary.
  - Checks exit codes for pipeline validation (see comments in the workflow for exit code mapping).
- **Manual execution** is also supported; see README for local usage.
- **Download links** for binaries are provided in the README, but source can be built locally.

## Examples & References
- See [README.md](../README.md) for full command-line options and sample outputs.
- Key files:
  - `src/LemonTree.Pipeline.Tools.ModelCheck/Checks/Checks.cs` (model validation logic)
  - `src/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams/Program.cs` (diagram cleanup entrypoint)
  - `src/LemonTree.Pipeline.Tools.SetFilterInSessionFile/Program.cs` (session file filter logic)

## Special Notes
- **Exit codes** are meaningful for pipeline integration; always check `$LASTEXITCODE` after running tools.
- **No solution-wide code style or linting rules** are enforced.
- **No automated tests**; rely on manual output review.

---
For updates, review the README and inspect each tool's `Program.cs` and `CommandLineOptions/` for argument details.
