using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Data.Repositories;
using GS4PlannerLib.Interfaces.Data;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data;

/// <summary>
/// Entity Framework Core implementation of <see cref="IDataAccessProvider"/>.
/// Resolves a <see cref="GS4PlannerDbContext"/> from the provided options factory and
/// returns a new <see cref="UnitOfWork"/> for each call.
/// </summary>
public class EfDataAccessProvider : IDataAccessProvider
{
    private readonly DbContextOptions<GS4PlannerDbContext> _options;

    public EfDataAccessProvider(DbContextOptions<GS4PlannerDbContext> options)
    {
        _options = options;
    }

    /// <inheritdoc/>
    public IUnitOfWork CreateUnitOfWork()
    {
        var context = new GS4PlannerDbContext(_options);
        return new UnitOfWork(context);
    }
}
