namespace GS4PlannerLib.Models;

/// <summary>
/// Represents a prioritized training goal within a training plan.
/// </summary>
public class TrainingGoal
{
    public int Id { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int TargetRanks { get; set; }
    public int Priority { get; set; }
    public string? Notes { get; set; }

    // Foreign key
    public int TrainingPlanId { get; set; }

    // Navigation property
    public TrainingPlan TrainingPlan { get; set; } = null!;
}
