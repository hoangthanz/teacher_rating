namespace teacher_rating.Models.ViewModels;

public class CreateConfiguration
{
    public string Name { get; set; }

    public int MinimumScore { get; set; }

    public int MaximumScore { get; set; }

    public string? SchoolId { get; set; }
}