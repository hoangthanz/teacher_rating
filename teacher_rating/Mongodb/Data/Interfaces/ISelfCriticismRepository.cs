using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ISelfCriticismRepository
{
    Task<SelfCriticism> GetSelfCriticismById(string id);
    Task<IEnumerable<SelfCriticism>> GetSelfCriticisms();
    Task<List<SelfCriticism>> GetSelfCriticismsByUser(string userId);
    Task<List<SelfCriticism>> GetSelfCriticismsByTeacher(string teacherId, int? month, int? year);
    Task AddSelfCriticism(SelfCriticism teacher);
    Task UpdateSelfCriticism(SelfCriticism teacher);
    Task RemoveSelfCriticism(string id);
    Task<RespondAPIPaging<List<SelfCriticism>>> GetByCondition(SearchSelfCriticism model);
}