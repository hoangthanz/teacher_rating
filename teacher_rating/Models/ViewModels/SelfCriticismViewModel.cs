using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Models.Identity;

namespace teacher_rating.Models.ViewModels;

public class SelfCriticismViewModel
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public List<AssessmentCriteria> AssessmentCriterias { get; set; }
    public DateTime? SubmitDate { get; set; }
    public bool? IsSubmitted { get; set; } = false;
    public double TotalScore { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public ApplicationUser? User { get; set; }
    public string? UserId { get; set; }
    public string SchoolId { get; set; }
    public string CompetitiveRanking { get; set; }
}