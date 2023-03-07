using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class School: BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("address")]
    public string Address { get; set; }
    
    [BsonElement("teachers")]
    public List<Teacher> Teachers { get; set; }

    [BsonElement("assessment_groups")]
    public List<AssessmentGroup> AssessmentGroups { get; set; }

    public School(string name = "", string address= "")
    {
        Name = name;
        Address = address;
        Teachers = new List<Teacher>();
        AssessmentGroups = new List<AssessmentGroup>();
    }
    
}
