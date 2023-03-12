using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Const;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class TeacherGroup : BaseEntity
{
    [BsonElement("name")] public string Name { get; set; }
    [BsonElement("teacherIds")] public List<string>? TeacherIds { get; set; }
    [BsonElement("period1Score")] public double Period1Score { get; set; } = 0;

    [BsonElement("period2Score")] public double Period2Score { get; set; } = 0;

    [BsonElement("yearScore")] public double YearScore { get; set; } = 0;

    [BsonElement("schoolId")] public string SchoolId { get; set; } = DefaultConfigs.DefaultSchoolId;

    [BsonElement("totalMember")] 
    public double TotalMember { get; set; } = 0;
}