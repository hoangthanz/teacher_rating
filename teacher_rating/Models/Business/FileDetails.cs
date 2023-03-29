using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Enums;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
public class FileDetails
{
    [BsonId] [BsonElement("_id")] public string Id { get; set; }

    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public FileType FileType { get; set; }
    [BsonElement("schoolId")] public string SchoolId { get; set; }

    public string Description { get; set; }
    public string UserId { get; set; }
    public DateTime CreateDate { get; set; }
}