namespace GS4PlannerLib.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SkillCategoryId { get; set; }
    public string? Description { get; set; }

    public SkillCategory SkillCategory { get; set; } = null!;
    public ICollection<TrainingCost> TrainingCosts { get; set; } = new List<TrainingCost>();
    public ICollection<TrainingCap> TrainingCaps { get; set; } = new List<TrainingCap>();
}
