using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
public class ClaimInfo
{
    public ClaimInfo(string displayName, string name, List<Permission> permissions)
    {
        DisplayName = displayName;
        Name = name;
        Permissions = permissions;
    }
    public string DisplayName { get; set; }
    public string Name { get; set; }
    public List<Permission> Permissions { get; set; }
}