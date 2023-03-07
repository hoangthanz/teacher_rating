using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ISchoolRepository
{
    Task<School?> GetById(string id);
    Task<List<School>> GetAll();
    Task Create(School school);
    Task<bool> Update(string id, School school);
    Task<bool> Delete(string id);
}