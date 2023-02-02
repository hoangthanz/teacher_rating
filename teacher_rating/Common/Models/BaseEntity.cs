using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace teacher_rating.Common.Models
{
    public class BaseEntity
    {
        [BsonId]
        [BsonElement("_id")] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("decription")] 
        public string Description { get; set; }
    }
}