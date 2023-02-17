using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class AssessmentCriteriaGroupRepository: IAssessmentCriteriaGroupRepository
{
    private readonly IMongoCollection<AssessmentCriteriaGroup> _mongoCollection;

    public AssessmentCriteriaGroupRepository(
        IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _mongoCollection = mongoDatabase.GetCollection<AssessmentCriteriaGroup>(
            teacherRatingSettings.Value.AssessmentCriteriaGroupCollectionName);
    }

    public async Task<AssessmentCriteriaGroup> GetAssessmentCriterGroupById(string id)
    {
        return await _mongoCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AssessmentCriteriaGroup>> GetAllAssessmentCriteriaGroups()
    {
        return await _mongoCollection.Find(teacher => true).ToListAsync();
    }

    public async Task AddAssessmentCriteriaGroup(AssessmentCriteriaGroup teacher)
    {
        await _mongoCollection.InsertOneAsync(teacher);
    }

    public async Task UpdateAssessmentCriteriaGroup(AssessmentCriteriaGroup teacher)
    {
        await _mongoCollection.ReplaceOneAsync(t => teacher.Id == t.Id, teacher);
    }

    public async Task RemoveAssessmentCriteriaGroup(string id)
    {
        await _mongoCollection.DeleteOneAsync(t => t.Id == id);
    }
}