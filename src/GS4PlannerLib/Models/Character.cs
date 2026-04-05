namespace GS4PlannerLib.Models;

/// <summary>
/// Represents a GemStone IV character with attributes used for training plan calculations.
/// </summary>
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty;
    public int Level { get; set; }

    // Navigation property
    public ICollection<TrainingPlan> TrainingPlans { get; set; } = new List<TrainingPlan>();
}
