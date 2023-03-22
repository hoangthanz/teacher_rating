using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class TeacherGroupRepository : ITeacherGroupRepository
{
    
    private readonly IMongoCollection<TeacherGroup> _teacherGroupsCollection;

    public TeacherGroupRepository(
        IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _teacherGroupsCollection = mongoDatabase.GetCollection<TeacherGroup>(
            teacherRatingSettings.Value.TeacherGroupCollectionName);
    }

    public async Task<TeacherGroup> GetTeacherGroupById(string id)
    {
        return await _teacherGroupsCollection.Find(teacher => teacher.Id == id).FirstOrDefaultAsync();
    }

    public async Task<TeacherGroup?> GetTeacherGroupByName(string name)
    {
        return await _teacherGroupsCollection.Find(teacher => teacher.Name == name).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TeacherGroup>> GetAllTeacherGroups()
    {
        return await _teacherGroupsCollection.Find(teacher => true).ToListAsync();
    }

    public async Task AddTeacherGroup(TeacherGroup teacher)
    {
        await _teacherGroupsCollection.InsertOneAsync(teacher);
    }

    public async Task AddTeacherGroups(List<TeacherGroup> teachers)
    {
        await _teacherGroupsCollection.InsertManyAsync(teachers);
    }

    public async Task UpdateTeacherGroup(TeacherGroup teacher)
    {
        await _teacherGroupsCollection.ReplaceOneAsync(t => teacher.Id == t.Id, teacher);
    }

    public async Task RemoveTeacherGroup(string id)
    {
        await _teacherGroupsCollection.DeleteOneAsync(teacher => teacher.Id == id);
    }

    public async Task TeacherGrouping(string schoolId, string groupId, List<string> teacherIds)
    {
        var group = await _teacherGroupsCollection.Find(g => g.SchoolId == schoolId && g.Id == groupId)
            .FirstOrDefaultAsync();
        if (group is null)
            return;

        group.TeacherIds = teacherIds;
        await _teacherGroupsCollection.ReplaceOneAsync(g => group.Id == g.Id, group);
    }
}