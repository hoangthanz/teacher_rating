using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class TeacherGroup : BaseEntity
{
    [BsonElement("name")] public string Name { get; set; }
    [BsonElement("teacherIds")] public List<string> TeacherIds { get; set; }
    [BsonElement("period1Score")] public double Period1Score { get; set; }

    [BsonElement("period2Score")] public double Period2Score { get; set; }

    [BsonElement("yearScore")] public double YearScore { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }

    [BsonElement("totalMember")] 
    public double TotalMember { get; set; } = 0;
}