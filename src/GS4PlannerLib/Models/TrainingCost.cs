namespace GS4PlannerLib.Models;

public class TrainingCost
{
    public int Id { get; set; }
    public int ProfessionId { get; set; }
    public int SkillId { get; set; }

    /// <summary>Physical training point cost per rank.</summary>
    public int PhysicalCost { get; set; }

    /// <summary>Mental training point cost per rank.</summary>
    public int MentalCost { get; set; }

    public Profession Profession { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
