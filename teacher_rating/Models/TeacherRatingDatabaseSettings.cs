namespace teacher_rating.Models;

public class TeacherRatingDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string TeacherCollectionName { get; set; } = null!;
    public string TeacherGroupCollectionName { get; set; } = null!;
    public string AssessmentCriteriaCollectionName { get; set; } = null!;
}