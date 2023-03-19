using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
[Description("Nhóm giáo viên")]
public class AssessmentGroup : BaseEntity
{
    [BsonElement("group_name")]
    public string? GroupName { get; set; }

    [BsonElement("teacher_ids")]
    public List<string>? TeacherIds { get; set; }
    
    public string? SchoolId { get; set; }
}