using Microsoft.AspNetCore.Mvc;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly ITeacherRepository _teacherRepository;

    public TeacherController(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
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
        return Ok();
    }
}