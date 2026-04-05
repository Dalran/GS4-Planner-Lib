using GS4PlannerLib.Data.Context;
using GS4PlannerLib.Interfaces.Data;
using GS4PlannerLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GS4PlannerLib.Data.Repositories;

/// <summary>
/// Entity Framework Core repository for <see cref="Character"/> entities.
/// </summary>
public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(GS4PlannerDbContext context) : base(context) { }

    public async Task<Character?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet
            .FirstOrDefaultAsync(c => EF.Functions.Like(c.Name, name), cancellationToken);
}
