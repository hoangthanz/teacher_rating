using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Mongodb.Services;

namespace teacher_rating.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherRepository teacherRepository, ITeacherService teacherService)
    {
        _teacherRepository = teacherRepository;
        _teacherService = teacherService;
    }
    
    [HttpGet("get-by-group/{groupId}")]
    public async Task<IActionResult> GetByGroup(string groupId)
    {
        var teachers = await _teacherRepository.GetTeachersOfGroup(groupId);
        return Ok(teachers);
    }
    
    [HttpGet("get-by-school/{schoolId}")]
    public async Task<IActionResult> GetBySchool(string schoolId)
    {
        var teachers = await _teacherRepository.GetTeachersOfSchool(schoolId);
        return Ok(teachers);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Teacher teacher)
    {
        await _teacherRepository.UpdateTeacher(teacher);
        return Ok(new RespondApi<object>()
        {
            Result = ResultRespond.Success
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacher teacher)
    {
        var newTeacher = new Teacher()
        {
            Id = Guid.NewGuid().ToString(),
            Name = teacher.Name,
            SchoolId = teacher.SchoolId,
            GroupId = teacher.GroupId,
            UserId = teacher.UserId,
            Email = teacher.Email,
            IsDeleted = false,
            PhoneNumber = teacher.PhoneNumber,
        };
        await _teacherRepository.AddTeacher(newTeacher);
        return Ok(new RespondApi<object>()
        {
            Result = ResultRespond.Success
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _teacherRepository.RemoveTeacher(id);
        return Ok(new RespondApi<object>()
        {
            Result = ResultRespond.Success
        });
    }
   [HttpPost("add-teacher-by-excel")]
    public async Task<IActionResult> AddTeacherByExcel([FromForm] UploadFile file)
    {
        using(var ms = new MemoryStream()) {
            await file.File.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var respond = await _teacherService.CreateTeachersFromExcel(ms, file.SchoolId);
            return Ok(respond);
        }
    }
}