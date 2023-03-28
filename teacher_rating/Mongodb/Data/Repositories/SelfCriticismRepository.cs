using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using teacher_rating.Common.Models;
using teacher_rating.Common.Models.Paging;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
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

    public async Task<List<SelfCriticism>> GetSelfCriticismsByUser(string userId)
    {
        return await _mongoCollection.Find(teacher => teacher.UserId == userId).ToListAsync();
    }

    public async Task<List<SelfCriticism>> GetSelfCriticismsByTeacher(string teacherId, int? month, int? year)
    {
        if(month != null && year != null)
            return await _mongoCollection.Find(x => x.TeacherId == teacherId
            && x.Month == month && x.Year == year && x.IsSubmitted == true).ToListAsync();
        else
        {
            return await _mongoCollection.Find(x => x.TeacherId == teacherId && x.IsSubmitted == true).ToListAsync();
        }
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

    public async Task<RespondAPIPaging<List<SelfCriticism>>> GetByCondition(SearchSelfCriticism model)
    {
        FilterDefinitionBuilder<SelfCriticism> builder = Builders<SelfCriticism>.Filter;
        FilterDefinition<SelfCriticism> query = builder.Where(x => x.IsDeleted == false || x.IsDeleted == null && x.SchoolId == model.SchoolId);
        if (model.Year != null)
        {
            var codeFilter = builder.Where(x => x.Year == model.Year);
            query &= codeFilter;
            if (model.Month != null)
            {
                codeFilter = builder.Where(x => x.Month == model.Month);
                query &= codeFilter;
            }
        }
        
        if(model.GroupId != null)
        {
            var  codeFilter = builder.Where(x => x.Teacher != null && x.Teacher.GroupId == model.GroupId);
            query &= codeFilter;
        }
        
        if(model.AssessmentCriteria != null)
        {
            var codeFilter = builder.Where(x =>
                x.AssessmentCriterias.Select(y => y.Name).Contains(model.AssessmentCriteria));
            query &= codeFilter;
        }

        PagingResponse paging = null;
        List<SelfCriticism> result = null;
        if (model.IsPaging)
        {
            paging = new PagingResponse()
            {
                CurrentPage = model.PageNumber,
                PageSize = model.PageSize,
            };
            paging.TotalRecords = (int)await _mongoCollection.Find(query).CountDocumentsAsync();
            paging.TotalPages = (int)Math.Ceiling(paging.TotalRecords / (double)paging.PageSize);
            result = await _mongoCollection.Find(query).Skip((model.PageNumber - 1) * model.PageSize).Limit(model.PageSize).ToListAsync();
        }
        else
        {
            result = await _mongoCollection.Find(query).ToListAsync();
        }
        return new RespondAPIPaging<List<SelfCriticism>>() {Result = ResultRespond.Success, Data = result, Paging = paging};
    }
}