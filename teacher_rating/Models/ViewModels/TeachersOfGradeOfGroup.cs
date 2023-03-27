namespace teacher_rating.Models.ViewModels;

public class TeachersOfGradeOfGroup
{
    public TeacherGroup Group { get; set; }
    public List<TeachersOfGrade> TeachersOfGrade { get; set; }
}

public class TeachersOfGrade
{
    public List<Teacher> Teachers { get; set; }
    public GradeConfiguration Grade { get; set; }
}