namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Abstracts how a <see cref="IUnitOfWork"/> is created, allowing callers to remain
/// independent of the underlying database technology and connection details.
/// </summary>
public interface IDataAccessProvider
{
    /// <summary>Creates a new unit of work backed by the configured data store.</summary>
    IUnitOfWork CreateUnitOfWork();
}
