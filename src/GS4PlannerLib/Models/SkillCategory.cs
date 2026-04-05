namespace GS4PlannerLib.Models;

public class SkillCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillCategoryType Type { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<TrainingCap> TrainingCaps { get; set; } = new List<TrainingCap>();
}
