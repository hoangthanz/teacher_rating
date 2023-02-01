using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
public class ApplicationRole: IdentityRole<int>
{
    public string? DisplayName { get; set; }
    public bool CanDelete { get; set; }
    public bool RoleInGate { get; set; }
    public bool HaveOtp { get; set; }
}