using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ISchoolRepository
{
    Task<School?> GetById(string id);
    Task<List<School>> GetAll();
    Task<RespondApi<School>> Create(CreateSchool school);
    Task<RespondApi<School>> Create(School school);
    Task<bool> Update(string id, School school);
    Task<bool> Delete(string id);
}