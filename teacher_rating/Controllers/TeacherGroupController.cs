using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Mongodb.Services;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherGroupController : ControllerBase
    {
        private readonly ITeacherGroupRepository _teacherGroupRepository;
        private readonly IGroupTeacherService _groupTeacherService;
        private readonly string? _userId;

        public TeacherGroupController(
            ITeacherGroupRepository teacherGroupRepository,
            IHttpContextAccessor httpContext, IGroupTeacherService groupTeacherService)
        {
            _teacherGroupRepository = teacherGroupRepository;
            _groupTeacherService = groupTeacherService;
            _userId = httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) != null
                ? httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeacherGroups()
        {
            var teacherGroups = await _teacherGroupRepository.GetAllTeacherGroups();
            return Ok(new RespondApi<IEnumerable<TeacherGroup>>("Get all teacher groups successfully.", teacherGroups,
                null));
        }

        [HttpGet("{id}", Name = "GetTeacherGroupById")]
        public async Task<IActionResult> GetTeacherGroupById(string id)
        {
            var teacherGroup = await _teacherGroupRepository.GetTeacherGroupById(id);

            if (teacherGroup == null)
            {
                return NotFound(new RespondApi<TeacherGroup>(ResultRespond.NotFound, "01",
                    "Not found teacher group by id.", null));
            }

            return Ok(new RespondApi<TeacherGroup>("Get teacher group by id successfully.", teacherGroup, null));
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddTeacherGroup(CreateTeacherGroup create)
        {
            var teacherGroupExist = await _teacherGroupRepository.GetTeacherGroupByName(create.Name);

            if (teacherGroupExist is not null)
                return Ok(new RespondApi<object>()
                    { Data = null, Code = "0", Result = ResultRespond.Error, Message = "Trùng tên tổ" });

            var teacherGroup = new TeacherGroup
            {
                Name = create.Name,
                SchoolId = create.SchoolId,
                Period1Score = create.Period1Score,
                Period2Score = create.Period2Score,
                TeacherIds = create.TeacherIds,
                TotalMember = create.TotalMember,
                YearScore = create.YearScore,

                IsDeleted = false
            };
            teacherGroup.Id = Guid.NewGuid().ToString();
            await _teacherGroupRepository.AddTeacherGroup(teacherGroup);

            return Ok(new RespondApi<TeacherGroup>("Create teacher group successfully.", teacherGroup, null));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTeacherGroup(string id, TeacherGroup teacherGroupIn)
        {
            var teacherGroup = await _teacherGroupRepository.GetTeacherGroupById(id);

            if (teacherGroup == null)
            {
                return NotFound(new RespondApi<TeacherGroup>(ResultRespond.NotFound, "01",
                    "Not found teacher group by id.", null));
            }

            teacherGroupIn.Id = id;

            var teacherGroupExist = await _teacherGroupRepository.GetTeacherGroupByName(teacherGroupIn.Name);

            if (teacherGroupExist is not null)
                return Ok(new RespondApi<object>()
                    { Data = null, Code = "0", Result = ResultRespond.Error, Message = "Trùng tên tổ" });

            await _teacherGroupRepository.UpdateTeacherGroup(teacherGroupIn);

            return Ok(new RespondApi<TeacherGroup>("Update teacher group successfully.", teacherGroupIn, null));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTeacherGroup(string id)
        {
            var teacherGroup = await _teacherGroupRepository.GetTeacherGroupById(id);

            if (teacherGroup == null)
            {
                return NotFound(new RespondApi<TeacherGroup>(ResultRespond.NotFound, "01",
                    "Not found teacher group by id.", null));
            }

            await _teacherGroupRepository.RemoveTeacherGroup(teacherGroup.Id);

            return Ok(new RespondApi<TeacherGroup>("Remove teacher group successfully.", teacherGroup, null));
        }

        [HttpPut("add-teacher-to-group/{groupid}")]
        public async Task<IActionResult> AddTeacherToGroup([FromBody] List<string> teacherIds, string groupId)
        {
            var result = _groupTeacherService.AddTeachersToGroup(teacherIds, groupId);
            return Ok(result);
        }
    }
}