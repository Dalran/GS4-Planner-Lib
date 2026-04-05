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
│   │   ├── Models/                 # Entity models (Character, TrainingPlan, TrainingGoal)
│   │   ├── Interfaces/
│   │   │   └── Data/               # Repository & UoW contracts (IRepository<T>, IUnitOfWork, …)
│   │   ├── Services/               # Business logic services
│   │   └── Properties/             # Assembly metadata
│   └── GS4PlannerLib.Data/         # Data access layer (EF Core)
│       ├── Context/                # GS4PlannerDbContext
│       ├── Repositories/           # Repository<T>, UnitOfWork, specialised repositories
│       ├── EfDataAccessProvider.cs # IDataAccessProvider implementation
│       └── ServiceCollectionExtensions.cs  # DI registration helpers
├── tests/
│   └── GS4PlannerLib.Tests/        # Unit tests (xUnit + EF InMemory)
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
| Microsoft.EntityFrameworkCore | 8.0.0 | ORM & repository abstraction |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.0 | SQLite database provider |
| Microsoft.EntityFrameworkCore.Design | 8.0.0 | Migrations tooling |
| xunit | 2.5.3 | Unit testing framework |
| coverlet.collector | 6.0.0 | Code coverage collection |

## Data Layer – Dependency Injection Setup

### SQLite (default)

Register all data services with a single call in your `Program.cs` or `Startup.cs`:

```csharp
using GS4PlannerLib.Data;

builder.Services.AddGS4PlannerSqlite("Data Source=planner.db");
```

Then inject `IUnitOfWork` wherever you need it:

```csharp
public class PlanService(IUnitOfWork uow)
{
    public async Task<TrainingPlan> CreatePlanAsync(string name, int characterId)
    {
        var plan = new TrainingPlan { Name = name, CharacterId = characterId };
        await uow.TrainingPlans.AddAsync(plan);
        await uow.SaveChangesAsync();
        return plan;
    }
}
```

### Switching to a different database (e.g. SQL Server)

Use the `AddGS4PlannerData` overload and configure the provider manually:

```csharp
builder.Services.AddGS4PlannerData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

No other changes are required – the repository and unit-of-work implementations are
database-agnostic.

## Database Migrations

The `GS4PlannerLib.Data` project is set up for EF Core migrations.

### Add a new migration

```bash
dotnet ef migrations add InitialCreate \
    --project src/GS4PlannerLib.Data \
    --startup-project src/GS4PlannerLib.Data
```

### Apply migrations

```bash
dotnet ef database update \
    --project src/GS4PlannerLib.Data \
    --startup-project src/GS4PlannerLib.Data
```

> **Note:** A design-time `IDesignTimeDbContextFactory` will be needed in
> `GS4PlannerLib.Data` before running migration commands if no host project
> provides the connection string at design time.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on contributing to this project.

## License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

