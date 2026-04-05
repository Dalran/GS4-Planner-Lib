# GS4-Planner-Lib

A C# library for planning and managing GS4 (GemStone IV) character builds, training plans, and skill allocations.

## Overview

GS4-Planner-Lib is the core library powering the GS4 Character Planner. It provides:

- **Character modelling** – races, professions, stats, and available training points per level
- **Training plan management** – create and manage multi-level training plans with goal priorities
- **Point allocation algorithms** – automatically distribute physical and mental training points to meet goals
- **Data persistence** – SQLite-backed storage for saving and loading plans

## Project Structure

```
GS4-Planner-Lib/
├── src/
│   ├── GS4PlannerLib/              # Core business logic library
│   │   ├── Core/                   # Core planner logic and algorithms
│   │   ├── Models/                 # Data models (Character, TrainingPlan, Goals, etc.)
│   │   ├── Interfaces/             # Contracts and abstractions
│   │   ├── Services/               # Business logic services
│   │   └── Properties/             # Assembly metadata
│   └── GS4PlannerLib.Data/         # Data access layer
│       └── Data/                   # SQLite implementations
├── tests/
│   └── GS4PlannerLib.Tests/        # Unit tests (xUnit)
├── GS4-Planner-Lib.sln
├── README.md
└── CONTRIBUTING.md
```

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Getting Started

### Clone & Restore

```bash
git clone https://github.com/Dalran/GS4-Planner-Lib.git
cd GS4-Planner-Lib
dotnet restore
```

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| Microsoft.Data.Sqlite | 8.0.0 | SQLite database access |
| xunit | 2.5.3 | Unit testing framework |
| coverlet.collector | 6.0.0 | Code coverage collection |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on contributing to this project.

## License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

