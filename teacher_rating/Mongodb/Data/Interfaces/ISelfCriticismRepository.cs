using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ISelfCriticismRepository
{
    Task<SelfCriticism> GetSelfCriticismById(string id);
    Task<IEnumerable<SelfCriticism>> GetSelfCriticisms();
    Task<List<SelfCriticism>> GetSelfCriticismsByUser(string userId);
    Task AddSelfCriticism(SelfCriticism teacher);
    Task UpdateSelfCriticism(SelfCriticism teacher);
    Task RemoveSelfCriticism(string id);
}