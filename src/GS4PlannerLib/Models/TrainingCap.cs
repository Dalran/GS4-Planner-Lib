namespace GS4PlannerLib.Models;

/// <summary>
/// Represents the maximum number of ranks that can be trained per level for a
/// profession in a given skill or skill category. Caps are cumulative: a cap of
/// 2 ranks/level means that by level 10 the character may have trained up to
/// 22 ranks total in that skill/category (levels 0–10 inclusive).
///
/// Exactly one of <see cref="SkillCategoryId"/> or <see cref="SkillId"/> must
/// be set:
/// <list type="bullet">
///   <item>Set <see cref="SkillCategoryId"/> for Spell Research and Lore
///   categories, where the cap applies to the combined total across all skills
///   in that category.</item>
///   <item>Set <see cref="SkillId"/> for Standard skills, where the cap applies
///   to a single, specific skill.</item>
/// </list>
/// </summary>
public class TrainingCap
{
    public int Id { get; set; }
    public int ProfessionId { get; set; }

    /// <summary>
    /// Foreign key to <see cref="SkillCategory"/>. Used when the cap governs a
    /// Spell Research or Lore category as a whole. Mutually exclusive with
    /// <see cref="SkillId"/>.
    /// </summary>
    public int? SkillCategoryId { get; set; }

    /// <summary>
    /// Foreign key to <see cref="Skill"/>. Used when the cap governs a single
    /// Standard skill. Mutually exclusive with <see cref="SkillCategoryId"/>.
    /// </summary>
    public int? SkillId { get; set; }

    /// <summary>Maximum ranks that may be trained in a single level.</summary>
    public int MaxRanksPerLevel { get; set; }

    public Profession Profession { get; set; } = null!;
    public SkillCategory? SkillCategory { get; set; }
    public Skill? Skill { get; set; }
}
