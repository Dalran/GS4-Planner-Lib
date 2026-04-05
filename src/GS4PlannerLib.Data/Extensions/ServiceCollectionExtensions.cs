using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Data.Repositories;
using GS4PlannerLib.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GS4PlannerLib.Data.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers GS4 Planner data services backed by a SQLite database at the
    /// given <paramref name="connectionString"/>.
    /// </summary>
    public static IServiceCollection AddGS4PlannerSqlite(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GS4PlannerDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<ITrainingRepository, TrainingRepository>();

        return services;
    }

    /// <summary>
    /// Registers GS4 Planner data services with a pre-configured
    /// <see cref="DbContextOptions{GS4PlannerDbContext}"/>. Useful for testing
    /// (e.g. in-memory databases) or advanced scenarios.
    /// </summary>
    public static IServiceCollection AddGS4PlannerData(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        services.AddDbContext<GS4PlannerDbContext>(configureOptions);
        services.AddScoped<ITrainingRepository, TrainingRepository>();
        return services;
    }
}
