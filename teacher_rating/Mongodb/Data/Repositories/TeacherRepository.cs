﻿using MongoDB.Driver;
using ServiceStack;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly IMongoCollection<Teacher> _teachersCollection;

    public TeacherRepository(Microsoft.Extensions.Options.IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _teachersCollection = mongoDatabase.GetCollection<Teacher>(
            teacherRatingSettings.Value.TeacherCollectionName);
    }

    public async Task<Teacher> GetTeacherByCMND(string cmnd)
    {
        return await _teachersCollection.Find(teacher => teacher.CMND == cmnd).FirstOrDefaultAsync();
    }

    public async Task<Teacher?> GetTeacherByUserId(string userId)
    {
        return await _teachersCollection.Find(teacher => teacher.User != null && teacher.User.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
    }

    public async Task<List<Teacher>> GetAllTeachers()
    {
        return await _teachersCollection.Find(teacher => true).ToListAsync();
    }

    public async Task<List<Teacher>> GetByIds(List<string> ids)
    {
        return await _teachersCollection.Find(teacher => ids.Contains(teacher.Id.ToString())).ToListAsync();
    }

    public async Task AddTeacher(Teacher teacher)
    {
        await _teachersCollection.InsertOneAsync(teacher);
    }

    public async Task UpdateTeacher(Teacher teacher)
    {
        await _teachersCollection.ReplaceOneAsync(t => teacher.Id == t.Id, teacher);
    }

    public async Task<Teacher> GetTeacherById(string id)
    {
        return await _teachersCollection.Find(teacher => teacher.Id == id).FirstOrDefaultAsync();
    }


    public async Task RemoveTeacher(string id)
    {
        await _teachersCollection.DeleteOneAsync(teacher => teacher.Id == id);
    }

    public async Task<List<Teacher>> GetTeachersOfGroup(string groupId)
    {
        return await _teachersCollection.Find(teacher => teacher.GroupId.Contains(groupId))
            .ToListAsync();
    }

    public async Task<List<Teacher>> GetTeachersOfSchool(string id)
    {
        return await _teachersCollection.Find(teacher => teacher.SchoolId == id)
            .ToListAsync();
    }

    public Task<List<Teacher>> GetTeachersForRating(SearchTeacherForRating model)
    {
        throw new NotImplementedException();
    }
}