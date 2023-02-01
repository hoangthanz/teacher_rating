using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
public class Permission
{
    public Permission(string displayName, string name, int type)
    {
        DisplayName = displayName;
        Name = name;
        Type = type;
    }
    public string Name { get; set; }
    public int Type { get; set; }
    public string DisplayName { get; set; }
}