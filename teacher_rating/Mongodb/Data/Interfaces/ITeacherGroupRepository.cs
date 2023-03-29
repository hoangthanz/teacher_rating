using teacher_rating.Common.Models;
using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface ITeacherGroupRepository
{
    Task<TeacherGroup> GetTeacherGroupById(string id);
    Task<TeacherGroup?> GetTeacherGroupByName(string name);
    Task<IEnumerable<TeacherGroup>> GetAllTeacherGroups();
    Task<List<TeacherGroup>> GetTeacherGroupsByIds(List<string> groupIds, string schoolId);
    Task<List<TeacherGroup>> GetTeacherGroupsBySchoolId(string schoolId);
    Task AddTeacherGroup(TeacherGroup teacher);
    Task AddTeacherGroups(List<TeacherGroup> teachers);
    Task UpdateTeacherGroup(TeacherGroup teacher);
    Task RemoveTeacherGroup(string id);
    
    Task TeacherGrouping(string schoolId, string groupId, List<string> teacherIds);
}