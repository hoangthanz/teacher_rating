using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ITeacherRepository
{
    Task<Teacher> GetTeacherById(string id);
    Task<Teacher?> GetTeacherByUserId(string userId);
    Task<IEnumerable<Teacher>> GetAllTeachers();

    Task AddTeacher(Teacher teacher);
    Task UpdateTeacher(Teacher teacher);
    Task RemoveTeacher(string id);
}