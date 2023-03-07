using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class AssessmentCriteria : BaseEntity
{
    [BsonElement("name")] 
    public string Name { get; set; }
    
    [BsonElement("deduct_score")] 
    public double DeductScore { get; set; }
    [BsonElement("is_deduct")] 
    public bool IsDeduct { get; set; }
    
    [BsonElement("value")] 
    public double Value { get; set; }
    
    [BsonElement("unit")]
    public string Unit { get; set; }
    
    public string? AssessmentCriteriaGroupId { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
}