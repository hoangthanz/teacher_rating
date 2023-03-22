namespace teacher_rating.Models.ViewModels;

public class TeacherGroupingModel
{
    public string SchoolId { get; set; }
    public string TeacherGroupId { get; set; }
    public List<string>  TeacherIds{ get; set; }

}