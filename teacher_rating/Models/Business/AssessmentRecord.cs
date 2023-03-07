using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class AssessmentRecord : BaseEntity
{
    [BsonElement("month")] 
    public int Month { get; set; }
    
    [BsonElement("score")] 
    public double Score { get; set; }

    [BsonElement("competition_score_period_1")] 
    public double CompetitionScorePeriod1 { get; set; }

    [BsonElement("competition_score_period_2")] 
    public double CompetitionScorePeriod2 { get; set; }

    [BsonElement("competition_score_academic_year")] 
    public double CompetitionScoreAcademicYear { get; set; }
    
    public string? SchoolId { get; set; }
}