using teacher_rating.Common.Const;

namespace teacher_rating.Models.ViewModels;

public class CreateTeacherGroup
{
    public string Name { get; set; }
    public List<string>? TeacherIds { get; set; }
    public double Period1Score { get; set; } = 0;

    public double Period2Score { get; set; } = 0;

    public double YearScore { get; set; } = 0;

    public string SchoolId { get; set; } = DefaultConfigs.DefaultSchoolId;


    public double TotalMember { get; set; } = 0;
    public string? LeaderId { get; set; }
}