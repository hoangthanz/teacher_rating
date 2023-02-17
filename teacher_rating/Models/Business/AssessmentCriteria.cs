using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class AssessmentCriteria : BaseEntity
{
    [BsonElement("name")] 
    public string Name { get; set; }
    
    [BsonElement("max_score")] 
    public double MaxScore { get; set; }
    
    [BsonElement("deduct_score")] 
    public double DeductScore { get; set; }
    
    [BsonElement("is_deduct")] 
    public bool IsDeduct { get; set; }
    
    [BsonElement("value")] 
    public double Value { get; set; }
}