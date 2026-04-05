using GS4PlannerLib.Models;

namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Repository interface for <see cref="TrainingGoal"/> entities.
/// </summary>
public interface ITrainingGoalRepository : IRepository<TrainingGoal>
{
    /// <summary>Returns all training goals for the given training plan, ordered by priority.</summary>
    Task<IEnumerable<TrainingGoal>> GetByTrainingPlanIdAsync(int trainingPlanId, CancellationToken cancellationToken = default);
}
