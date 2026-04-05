namespace GS4PlannerLib.Models;

/// <summary>
/// Represents a named training plan associated with a character.
/// </summary>
public class TrainingPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Foreign key
    public int CharacterId { get; set; }

    // Navigation properties
    public Character Character { get; set; } = null!;
    public ICollection<TrainingGoal> TrainingGoals { get; set; } = new List<TrainingGoal>();
}
