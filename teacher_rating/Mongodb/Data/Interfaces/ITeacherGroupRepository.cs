using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ITeacherGroupRepository
{
    Task<TeacherGroup> GetTeacherGroupById(string id);
    Task<TeacherGroup?> GetTeacherGroupByName(string name);
    Task<IEnumerable<TeacherGroup>> GetAllTeacherGroups();
    Task AddTeacherGroup(TeacherGroup teacher);
    Task UpdateTeacherGroup(TeacherGroup teacher);
    Task RemoveTeacherGroup(string id);
}