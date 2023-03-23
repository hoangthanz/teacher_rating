using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface IGroupTeacherService
{
    Task<RespondApi<TeacherGroup>> AddTeachersToGroup(List<string> teacherId, string groupId);
}

public class GroupTeacherService : IGroupTeacherService
{
    private ITeacherRepository _teacherRepository;
    private ITeacherGroupRepository _teacherGroupRepository;

    public GroupTeacherService(ITeacherRepository teacherRepository, ITeacherGroupRepository teacherGroupRepository)
    {
        _teacherRepository = teacherRepository;
        _teacherGroupRepository = teacherGroupRepository;
    }

    public async Task<RespondApi<TeacherGroup>> AddTeachersToGroup(List<string> teacherIds, string groupId)
    {
        // remove duplicate teacher id
        teacherIds = teacherIds.Distinct().ToList();
        var teachers = await  _teacherRepository.GetByIds(teacherIds);
        var teacherGroup = await _teacherGroupRepository.GetTeacherGroupById(groupId);
        if (teacherGroup is null)
        {
            return new RespondApi<TeacherGroup>()
                { Result = ResultRespond.Fail, Message = "Không tìm thấy tổ cần cập nhật" };
        }

        if (teachers == null)
        {
            return new RespondApi<TeacherGroup>()
            {
                Result = ResultRespond.Fail,
                Message = "Không tìm thấy giao viên  cần thêm vào tổ"
            };
        }

        if (teacherGroup.TeacherIds == null)
        {
            teacherGroup.TeacherIds = new List<string>();
        }
        foreach (var teacher in teachers)
        {
            teacher.GroupId = teacherGroup.Id;
            await _teacherRepository.UpdateTeacher(teacher);
        }
        teacherGroup.TeacherIds.AddRange(teachers.Select(x => x.Id));
        await _teacherGroupRepository.UpdateTeacherGroup(teacherGroup);
        
        return new RespondApi<TeacherGroup>()
        {
            Result = ResultRespond.Success, Message = "Thành công", Data = teacherGroup
        };
    }
}