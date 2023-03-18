namespace teacher_rating.Models.ViewModels;

public class SearchTeacherForRating
{
    public string? GroupId { get; set; }
    public string? SchoolId { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}