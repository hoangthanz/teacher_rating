using CKLS.Services.Models.MongoDb.Common;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Common;
using teacher_rating.Common.Contracts;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class FileDetailsRepository : IFileDetailsRepository
{
    private readonly IMongoCollection<FileDetails> _mongoCollection;

    public FileDetailsRepository(IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _mongoCollection = mongoDatabase.GetCollection<FileDetails>(
            teacherRatingSettings.Value.FileCollectionName);
    }


    public async Task<RespondApi<object>> Insert(FileDetails fileDetails)
    {
        await _mongoCollection.InsertOneAsync(fileDetails);
        return new RespondApi<object>() { Data = fileDetails, Message = "File details inserted successfully" };
    }

    public async Task<RespondApi<List<FileDetails>>> Search(List<string> ids)
    {
        var filter = Builders<FileDetails>.Filter.In("_id", ids);
        var result = await _mongoCollection.FindAsync(filter);
        return new RespondApi<List<FileDetails>>() { Data = result.ToList(), Message = "File details fetched successfully" };
    }

    public async Task<RespondApi<List<FileDetails>>> GetAllBySchool(string schoolId)
    {
        var filter = Builders<FileDetails>.Filter.Eq("SchoolId", schoolId);
        var result = await _mongoCollection.FindAsync(filter);
        return new RespondApi<List<FileDetails>>() { Data = result.ToList(), Message = "File details fetched successfully" };
    }

    public async Task<RespondApi<object>> Update(FileDetails fileDetails)
    {
        await _mongoCollection.ReplaceOneAsync(configuration => configuration.Id == fileDetails.Id, fileDetails);
        return new RespondApi<object>() { Data = fileDetails, Message = "File details updated successfully" };
    }

    public async Task<RespondApi<object>> Remove(string id)
    {
        await _mongoCollection.DeleteOneAsync(configuration => configuration.Id == id);
        return new RespondApi<object>() { Data = null, Message = "File details deleted successfully" };
    }
}