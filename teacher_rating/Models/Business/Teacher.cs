﻿using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using teacher_rating.Common.Models;
using teacher_rating.Models.Identity;

namespace teacher_rating.Models;

[BsonIgnoreExtraElements]
[Description("Giáo viên")]
public class Teacher: BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("phone_number")]
    public string PhoneNumber { get; set; }
    [BsonElement("gender")]
    public string? Gender { get; set; }
    [BsonElement("cmnd")]
    public string? CMND { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("assessment_records")]
    public List<AssessmentRecord>? AssessmentRecords { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Subject { get; set; }
    public string? Position { get; set; }
    public string? FunctionalGroup { get; set; }
    public string? SalaryTier { get; set; }
    public string? SalaryCoefficient { get; set; }
    
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    [BsonElement("schoolId")] 
    public string SchoolId { get; set; }
    [BsonElement("group_id")]
    public string GroupId { get; set; }
}