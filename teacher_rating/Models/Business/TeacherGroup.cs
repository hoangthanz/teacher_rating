using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

public class TeacherGroup : BaseEntity
{
    [BsonElement("teacherIds")] public List<string> TeacherIds { get; set; }
    [BsonElement("period1Score")] public double Period1Score { get; set; }

    [BsonElement("period2Score")] public double Period2Score { get; set; }

    [BsonElement("yearScore")] public double YearScore { get; set; }
}