using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;

namespace GS4PlannerLib.Data.Repositories;

/// <summary>
/// Unit of Work implementation that coordinates repositories over a shared
/// <see cref="GS4PlannerDbContext"/> and wraps persistence in a single transaction boundary.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly GS4PlannerDbContext _context;
    private bool _disposed;

    private ICharacterRepository? _characters;
    private ITrainingPlanRepository? _trainingPlans;
    private ITrainingGoalRepository? _trainingGoals;

    public UnitOfWork(GS4PlannerDbContext context)
    {
        _context = context;
    }

    public ICharacterRepository Characters
        => _characters ??= new CharacterRepository(_context);

    public ITrainingPlanRepository TrainingPlans
        => _trainingPlans ??= new TrainingPlanRepository(_context);

    public ITrainingGoalRepository TrainingGoals
        => _trainingGoals ??= new TrainingGoalRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
