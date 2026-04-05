using GS4PlannerLib.Interfaces.Data;
using GS4PlannerLib.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GS4PlannerLib.Data.Repositories;

/// <summary>
/// Generic Entity Framework Core repository implementing basic CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly GS4PlannerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(GS4PlannerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);
}
