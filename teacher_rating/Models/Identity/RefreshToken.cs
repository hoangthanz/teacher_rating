using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
public class RefreshToken
{
    [Key]
    public string Id { get; set; }

    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string RevokedByIp { get; set; }
    public string ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }
}