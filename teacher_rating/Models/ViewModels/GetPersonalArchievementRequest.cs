namespace teacher_rating.Models.ViewModels;

public class GetPersonalArchievementRequest
{
    public int Month { get; set; }
    public int? Year { get; set; }
    public List<string> TeacherIds { get; set; }
}