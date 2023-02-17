using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class SelfCriticismRepository: ISelfCriticismRepository
{
    private readonly IMongoCollection<SelfCriticism> _mongoCollection;
    
    public SelfCriticismRepository(
        IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _mongoCollection = mongoDatabase.GetCollection<SelfCriticism>(
            teacherRatingSettings.Value.SelfCriticismCollectionName);
    }

    public async Task<SelfCriticism> GetSelfCriticismById(string id)
    {
        return await _mongoCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SelfCriticism>> GetSelfCriticisms()
    {
        return await _mongoCollection.Find(teacher => true).ToListAsync();
    }

    public async Task AddSelfCriticism(SelfCriticism teacher)
    {
        await _mongoCollection.InsertOneAsync(teacher);
    }

    public async Task UpdateSelfCriticism(SelfCriticism teacher)
    {
        await _mongoCollection.ReplaceOneAsync(t => teacher.Id == t.Id, teacher);
    }

    public async Task RemoveSelfCriticism(string id)
    {
        await _mongoCollection.DeleteOneAsync(t => t.Id == id);
    }
}