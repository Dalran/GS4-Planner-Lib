namespace GS4PlannerLib.Models;

public class Profession
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TrainingCost> TrainingCosts { get; set; } = new List<TrainingCost>();
    public ICollection<TrainingCap> TrainingCaps { get; set; } = new List<TrainingCap>();
}
