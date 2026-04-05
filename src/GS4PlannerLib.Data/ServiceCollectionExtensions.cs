using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GS4PlannerLib.Data;

/// <summary>
/// Extension methods for registering GS4 Planner data services with the Microsoft
/// Dependency Injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the GS4 Planner EF Core data layer with SQLite as the backing store.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="connectionString">The SQLite connection string (e.g. "Data Source=planner.db").</param>
    public static IServiceCollection AddGS4PlannerSqlite(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GS4PlannerDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork, Repositories.UnitOfWork>();
        services.AddScoped<IDataAccessProvider, EfDataAccessProvider>();

        return services;
    }

    /// <summary>
    /// Registers the GS4 Planner EF Core data layer using a custom
    /// <see cref="DbContextOptionsBuilder{GS4PlannerDbContext}"/> configuration action.
    /// Use this overload to switch to SQL Server, PostgreSQL, or any other provider.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configureOptions">Action to configure the DbContext options.</param>
    public static IServiceCollection AddGS4PlannerData(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        services.AddDbContext<GS4PlannerDbContext>(configureOptions);

        services.AddScoped<IUnitOfWork, Repositories.UnitOfWork>();
        services.AddScoped<IDataAccessProvider, EfDataAccessProvider>();

        return services;
    }
}
