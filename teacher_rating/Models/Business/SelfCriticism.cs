using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;
using teacher_rating.Models.Identity;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class SelfCriticism: BaseEntity
{
    [BsonElement("month")] 
    public int Month { get; set; }
    [BsonElement("year")] 
    public int Year { get; set; }
    [BsonElement("teacherId")] 
    public string? TeacherId { get; set; }
    [BsonElement("teacher")] 
    public Teacher? Teacher { get; set; }
    
    [BsonElement("assessmentCriterias")] 
    public List<AssessmentCriteria> AssessmentCriterias { get; set; }
    [BsonElement("submitDate")] 
    public DateTime? SubmitDate { get; set; }

    public bool? IsSubmitted { get; set; } = false;
    
    public double TotalScore { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    [BsonElement("user")] 
    public ApplicationUser? User { get; set; }
    [BsonElement("userId")] 
    public string? UserId { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
    
}