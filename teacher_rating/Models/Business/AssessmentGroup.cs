using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class AssessmentGroup : BaseEntity
{
    [BsonElement("group_name")]
    public string GroupName { get; set; }

    [BsonElement("teacher_ids")]
    public List<string> TeacherIds { get; set; }
    
}