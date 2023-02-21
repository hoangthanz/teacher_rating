using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;
using teacher_rating.Models.Identity;

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
    
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
}