using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using ZstdSharp.Unsafe;

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
        var result = await _collection.Find(r => r.IsDeleted == false).ToListAsync();
        return result;
    }

    public async Task<RespondApi<School>> Create(CreateSchool model)
    {
        var oldSchool = await _collection.Find(x => x.Name == model.Name && x.Address == model.Address).FirstOrDefaultAsync();
        if (oldSchool != null)
            return new RespondApi<School>()
                { Result = ResultRespond.Fail, Message = "Trường đã tồn tại trong hệ thống" };
        var school = new School
        {
            Id = Guid.NewGuid().ToString(),
            Name = model.Name,
            Address = model.Address,
            Description = model.Description,
            IsDeleted = false
        };
        await _collection.InsertOneAsync(school);
        return new RespondApi<School>() { Result = ResultRespond.Success, Data = school, Message = "Thành công"};
    }

    public async Task<RespondApi<School>> Create(School school)
    {
        await _collection.InsertOneAsync(school);
        return new RespondApi<School>() { Result = ResultRespond.Success, Data = school, Message = "Thành công"};
    }

    public async Task<bool> Update(string id, School school)
    {
        var updated = await _collection.ReplaceOneAsync(t => id == t.Id, school);
        return updated.IsAcknowledged;
    }

    public async Task<bool> Delete(string id)
    {
        var filter = Builders<School>.Filter.Eq("_id", id);
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}