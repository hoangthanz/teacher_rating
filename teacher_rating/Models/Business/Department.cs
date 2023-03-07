using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;


[BsonIgnoreExtraElements]
public class Department: BaseEntity
{
    public string Name { get; set; }
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
}