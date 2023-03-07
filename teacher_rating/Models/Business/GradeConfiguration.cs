using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

public class GradeConfiguration : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("minimum_score")]
    public int MinimumScore { get; set; }

    [BsonElement("maximum_score")]
    public int MaximumScore { get; set; }
    
    public string? SchoolId { get; set; }
    
}
