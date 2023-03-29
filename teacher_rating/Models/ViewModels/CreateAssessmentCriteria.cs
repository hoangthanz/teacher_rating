namespace teacher_rating.Models.ViewModels;

public class CreateAssessmentCriteria
{
    public string Name { get; set; }
    
    public double DeductScore { get; set; }
    public bool IsDeduct { get; set; }
    
    public double Value { get; set; }
    
    public string Unit { get; set; }
    
    public string? AssessmentCriteriaGroupId { get; set; }
    
    public string SchoolId { get; set; }

    public int Quantity { get; set; } = 1;
    public bool? AllowUpdateScore { get; set; } = false;

}