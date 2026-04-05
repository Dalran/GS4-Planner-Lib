using GS4PlannerLib.Models;

namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Repository interface for <see cref="Character"/> entities.
/// </summary>
public interface ICharacterRepository : IRepository<Character>
{
    /// <summary>Finds a character by name (case-insensitive).</summary>
    Task<Character?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
