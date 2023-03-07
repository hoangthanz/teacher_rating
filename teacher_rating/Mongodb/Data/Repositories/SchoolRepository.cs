using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class SchoolRepository: ISchoolRepository
{
    private readonly IMongoCollection<School> _collection;
    
    public SchoolRepository(IOptions<TeacherRatingDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        _collection = database.GetCollection<School>(settings.Value.SchoolCollectionName);
    }

    public async Task<School?> GetById(string id)
    {
        var filter = Builders<School>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<School>> GetAll()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task Create(School school)
    {
        await _collection.InsertOneAsync(school);
    }

    public async Task<bool> Update(string id, School school)
    {
        var filter = Builders<School>.Filter.Eq("_id", id);
        var update = Builders<School>.Update
            .Set("Name", school.Name)
            .Set("Address", school.Address);

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> Delete(string id)
    {
        var filter = Builders<School>.Filter.Eq("_id", id);
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}