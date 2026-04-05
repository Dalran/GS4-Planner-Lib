using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data.Repositories;

/// <summary>
/// Entity Framework Core repository for <see cref="TrainingGoal"/> entities.
/// </summary>
public class TrainingGoalRepository : Repository<TrainingGoal>, ITrainingGoalRepository
{
    public TrainingGoalRepository(GS4PlannerDbContext context) : base(context) { }

    public async Task<IEnumerable<TrainingGoal>> GetByTrainingPlanIdAsync(
        int trainingPlanId,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(g => g.TrainingPlanId == trainingPlanId)
            .OrderBy(g => g.Priority)
            .ToListAsync(cancellationToken);
}
