using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class AssessmentCriteriaGroup : BaseEntity
{
    [BsonElement("name")] 
    public string Name { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
}