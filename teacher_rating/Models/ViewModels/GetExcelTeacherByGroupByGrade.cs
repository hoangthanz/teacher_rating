namespace teacher_rating.Models.ViewModels;

public class GetExcelTeacherByGroupByGrade
{
    public string SchoolId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? UserId { get; set; }
}