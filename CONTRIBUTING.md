# Contributing to GS4-Planner-Lib

Thank you for your interest in contributing! This document provides guidelines for contributing to the GS4-Planner-Lib project.

## Getting Started

1. **Fork** the repository on GitHub
2. **Clone** your fork locally:
   ```bash
   git clone https://github.com/<your-username>/GS4-Planner-Lib.git
   cd GS4-Planner-Lib
   ```
3. **Create a branch** for your feature or bug fix:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A C# compatible IDE (Visual Studio, Visual Studio Code with C# extension, JetBrains Rider)

### Build & Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test
```

## Project Structure

- `src/GS4PlannerLib/` – Core planner library
  - `Core/` – Planner algorithms and core logic
  - `Models/` – Data models (Character, TrainingPlan, Goals, etc.)
  - `Interfaces/` – Abstractions and contracts
  - `Services/` – Business logic services
- `src/GS4PlannerLib.Data/` – Data access layer (SQLite)
- `tests/GS4PlannerLib.Tests/` – Unit tests

## Code Style

- Target **C# 10** language features where appropriate
- Use **nullable reference types** (`Nullable` is enabled globally)
- Follow standard .NET naming conventions:
  - `PascalCase` for types, methods, and properties
  - `camelCase` for local variables and parameters
  - `_camelCase` for private fields
- Add XML documentation comments (`///`) to all public APIs
- Keep classes and methods focused and single-purpose

## Pull Request Guidelines

1. Ensure all existing tests pass: `dotnet test`
2. Add tests for any new functionality
3. Keep pull requests focused – one feature or fix per PR
4. Write a clear PR description explaining the what and why
5. Reference any related GitHub issues (e.g., `Closes #2`)

## Reporting Issues

Use the GitHub Issues tracker to report bugs or request features. Please include:
- A clear description of the issue
- Steps to reproduce (for bugs)
- Expected vs actual behaviour
- .NET SDK version and OS

## Code of Conduct

Be respectful and constructive in all interactions. Harassment or hostile behaviour will not be tolerated.
