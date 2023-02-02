using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class Teacher: BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("phone_number")]
    public string PhoneNumber { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("assessment_records")]
    public List<AssessmentRecord> AssessmentRecords { get; set; }
}