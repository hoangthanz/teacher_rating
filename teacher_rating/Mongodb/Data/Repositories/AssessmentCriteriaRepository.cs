using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class AssessmentCriteriaRepository : IAssessmentCriteriaRepository
{
      
    private readonly IMongoCollection<AssessmentCriteria> _mongoCollection;

    public AssessmentCriteriaRepository(
        IOptions<TeacherRatingDatabaseSettings> teacherRatingSettings)
    {
        var mongoClient = new MongoClient(
            teacherRatingSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            teacherRatingSettings.Value.DatabaseName);

        _mongoCollection = mongoDatabase.GetCollection<AssessmentCriteria>(
            teacherRatingSettings.Value.AssessmentCriteriaCollectionName);
    }
    public async Task<AssessmentCriteria> GetAssessmentCriterById(string id)
    {
        return await _mongoCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AssessmentCriteria>> GetAllAssessmentCriters()
    {
        return await _mongoCollection.Find(teacher => true).ToListAsync();
    }

    public async Task<List<AssessmentCriteria>> GetAllAssessmentCritersByGroupId(string id)
    {
        return await _mongoCollection.Find(t => t.AssessmentCriteriaGroupId == id).ToListAsync();
    }

    public async Task AddAssessmentCriter(AssessmentCriteria criteria)
    {
        await _mongoCollection.InsertOneAsync(criteria);
    }

    public async Task AddAssessmentCriterList(List<AssessmentCriteria> criteriaList)
    {
        await _mongoCollection.InsertManyAsync(criteriaList);
    }

    public async Task UpdateAssessmentCriter(AssessmentCriteria criteria)
    {
        await _mongoCollection.ReplaceOneAsync(t => criteria.Id == t.Id, criteria);
    }

    public async Task RemoveAssessmentCriter(string id)
    {
        await _mongoCollection.DeleteOneAsync(t => t.Id == id);
    }
}