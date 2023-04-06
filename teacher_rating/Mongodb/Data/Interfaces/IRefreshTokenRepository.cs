using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetById(string id);
    Task<List<RefreshToken>> GetAll();
    Task<RespondApi<RefreshToken>> Create(School school);
    Task<bool> Update(string id, School school);
    Task<bool> Delete(string id);
}