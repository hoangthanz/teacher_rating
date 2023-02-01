using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class Teacher: BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Avatar { get; set; }
    public string Description { get; set; }
    public string Degree { get; set; }
    public string Position { get; set; }
    public string DepartmentId { get; set; }
}