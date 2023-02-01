using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
public class ApplicationUser : IdentityUser<int>
{
    public bool IsActive { get; set; }
    public string? ActiveCode { get; set; }
    public string? DisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}