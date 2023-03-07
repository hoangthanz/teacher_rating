using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
[CollectionName("users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public bool IsActive { get; set; }
    public string? ActiveCode { get; set; }
    public string? DisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; } = false;
    
    public string? SchoolId { get; set; }
}