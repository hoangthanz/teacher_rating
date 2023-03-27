using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class GradeConfigurationRepository : IGradeConfigurationRepository
{
    private readonly IMongoCollection<GradeConfiguration> _gradeConfigurationsCollection;

    public GradeConfigurationRepository(IOptions<TeacherRatingDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        _gradeConfigurationsCollection = database.GetCollection<GradeConfiguration>(settings.Value.GradeConfigurationCollectionName);
    }

    public async Task<GradeConfiguration?> GetGradeConfigurationById(string id)
    {
        return await _gradeConfigurationsCollection.Find(configuration => configuration.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<GradeConfiguration>> GetAllGradeConfigurations()
    {
        return await _gradeConfigurationsCollection.Find(configuration => true).ToListAsync();
    }

    public async Task AddGradeConfiguration(GradeConfiguration gradeConfiguration)
    {
        await _gradeConfigurationsCollection.InsertOneAsync(gradeConfiguration);
    }

    public async Task UpdateGradeConfiguration(GradeConfiguration gradeConfiguration)
    {
        await _gradeConfigurationsCollection.ReplaceOneAsync(configuration => configuration.Id == gradeConfiguration.Id, gradeConfiguration);
    }

    public async Task<GradeConfiguration> GetGradeConfigurationByScore(int score, string schoolId)
    {
        return await _gradeConfigurationsCollection.Find(x => x.MinimumScore <= score && x.MaximumScore >= score && x.SchoolId == schoolId).FirstOrDefaultAsync();
    }

    public async Task RemoveGradeConfiguration(string id)
    {
        await _gradeConfigurationsCollection.DeleteOneAsync(configuration => configuration.Id == id);
    }
}