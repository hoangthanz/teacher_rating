using Microsoft.AspNetCore.Mvc;
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
}