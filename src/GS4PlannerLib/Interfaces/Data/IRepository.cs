namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Generic repository interface providing basic CRUD operations for an entity type.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets an entity by its primary key.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Returns all entities of type <typeparamref name="T"/>.</summary>
    Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Finds entities that match the given predicate.</summary>
    Task<IEnumerable<T>> FindAsync(
        System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the repository.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity in the repository.</summary>
    void Update(T entity);

    /// <summary>Removes an entity from the repository.</summary>
    void Remove(T entity);
}
