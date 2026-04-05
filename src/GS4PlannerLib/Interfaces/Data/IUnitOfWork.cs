namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Unit of Work interface for managing repositories and coordinating database transactions.
/// Dispose to release the underlying database connection.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Repository for <see cref="Models.Character"/> entities.</summary>
    ICharacterRepository Characters { get; }

    /// <summary>Repository for <see cref="Models.TrainingPlan"/> entities.</summary>
    ITrainingPlanRepository TrainingPlans { get; }

    /// <summary>Repository for <see cref="Models.TrainingGoal"/> entities.</summary>
    ITrainingGoalRepository TrainingGoals { get; }

    /// <summary>
    /// Persists all pending changes to the underlying data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
