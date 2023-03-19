using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
[Description("Nhóm tiêu chí đánh giá")]
public class AssessmentCriteriaGroup : BaseEntity
{
    [BsonElement("name")] 
    public string Name { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
}