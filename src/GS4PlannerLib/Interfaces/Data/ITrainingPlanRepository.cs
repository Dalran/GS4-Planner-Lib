using GS4PlannerLib.Models;

namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Repository interface for <see cref="TrainingPlan"/> entities.
/// </summary>
public interface ITrainingPlanRepository : IRepository<TrainingPlan>
{
    /// <summary>Returns all training plans for the given character.</summary>
    Task<IEnumerable<TrainingPlan>> GetByCharacterIdAsync(int characterId, CancellationToken cancellationToken = default);
}
