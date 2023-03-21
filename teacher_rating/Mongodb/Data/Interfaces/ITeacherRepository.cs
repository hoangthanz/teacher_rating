using teacher_rating.Models;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ITeacherRepository
{
    Task<Teacher> GetTeacherById(string id);
    Task<Teacher?> GetTeacherByUserId(string userId);
    Task<List<Teacher>> GetAllTeachers();
    Task<List<Teacher>> GetByIds(List<string> ids);

    Task AddTeacher(Teacher teacher);
    Task UpdateTeacher(Teacher teacher);
    Task RemoveTeacher(string id);
    Task<List<Teacher>> GetTeachersOfGroup(string groupId);
    Task<List<Teacher>> GetTeachersOfSchool(string id);
    Task<List<Teacher>> GetTeachersForRating(SearchTeacherForRating model);
}