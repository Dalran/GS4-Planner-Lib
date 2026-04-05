using GS4PlannerLib.Models;

namespace GS4PlannerLib.Interfaces.Data;

/// <summary>
/// Provides read access to GS4 profession training cost and training cap data.
/// </summary>
public interface ITrainingRepository
{
    /// <summary>Returns all professions.</summary>
    Task<IReadOnlyList<Profession>> GetAllProfessionsAsync();

    /// <summary>Returns a profession by name (case-insensitive), or null if not found.</summary>
    Task<Profession?> GetProfessionByNameAsync(string name);

    /// <summary>Returns all skill categories.</summary>
    Task<IReadOnlyList<SkillCategory>> GetAllSkillCategoriesAsync();

    /// <summary>Returns all skills, including their category.</summary>
    Task<IReadOnlyList<Skill>> GetAllSkillsAsync();

    /// <summary>Returns all skills belonging to the specified category type.</summary>
    Task<IReadOnlyList<Skill>> GetSkillsByCategoryTypeAsync(SkillCategoryType categoryType);

    /// <summary>
    /// Returns all training costs for a given profession, including the related
    /// <see cref="Skill"/> and its <see cref="SkillCategory"/>.
    /// </summary>
    Task<IReadOnlyList<TrainingCost>> GetTrainingCostsByProfessionAsync(int professionId);

    /// <summary>
    /// Returns the training cost for a specific profession and skill, or null if
    /// no entry exists (the profession cannot train that skill).
    /// </summary>
    Task<TrainingCost?> GetTrainingCostAsync(int professionId, int skillId);

    /// <summary>
    /// Returns all training caps for a given profession.
    /// Each cap governs either a skill category (Spell Research / Lore) or an
    /// individual Standard skill.
    /// </summary>
    Task<IReadOnlyList<TrainingCap>> GetTrainingCapsByProfessionAsync(int professionId);

    /// <summary>
    /// Returns the category-level training cap for a profession in a specific
    /// skill category (used for Spell Research and Lore categories).
    /// Returns null when no cap is configured.
    /// </summary>
    Task<TrainingCap?> GetCategoryCapAsync(int professionId, int skillCategoryId);

    /// <summary>
    /// Returns the skill-level training cap for a profession for a specific
    /// Standard skill. Returns null when no cap is configured.
    /// </summary>
    Task<TrainingCap?> GetSkillCapAsync(int professionId, int skillId);
}
