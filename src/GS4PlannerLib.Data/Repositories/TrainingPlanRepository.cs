using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data.Repositories;

/// <summary>
/// Entity Framework Core repository for <see cref="TrainingPlan"/> entities.
/// </summary>
public class TrainingPlanRepository : Repository<TrainingPlan>, ITrainingPlanRepository
{
    public TrainingPlanRepository(GS4PlannerDbContext context) : base(context) { }

    public async Task<IEnumerable<TrainingPlan>> GetByCharacterIdAsync(
        int characterId,
        CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(p => p.CharacterId == characterId)
            .ToListAsync(cancellationToken);
}
