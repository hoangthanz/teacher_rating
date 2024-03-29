﻿using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace teacher_rating.Models.Identity;


[BsonIgnoreExtraElements]
[CollectionName("roles")]
public class ApplicationRole: MongoIdentityRole<Guid>
{
    public string? DisplayName { get; set; }
    public bool CanDelete { get; set; }
}