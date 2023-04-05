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
        private readonly ITeacherService _teacherService;
        private readonly string? _userId;

        public TeacherGroupController(
            ITeacherGroupRepository teacherGroupRepository,
            IHttpContextAccessor httpContext, IGroupTeacherService groupTeacherService, ITeacherService teacherService)
        {
            _teacherGroupRepository = teacherGroupRepository;
            _groupTeacherService = groupTeacherService;
            _teacherService = teacherService;
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
                LeaderId = create.LeaderId,
                IsDeleted = false
            };
            teacherGroup.Id = Guid.NewGuid().ToString();
            if (teacherGroup.TeacherIds != null)
            {
                teacherGroup.TeacherIds = teacherGroup.TeacherIds.Distinct().ToList();
            }
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

            // remove duplicate teacher id
            if (teacherGroupIn.TeacherIds != null)
            {
                teacherGroupIn.TeacherIds = teacherGroupIn.TeacherIds.Distinct().ToList();
            }
            
            var teacherGroupExist = await _teacherGroupRepository.GetTeacherGroupByName(teacherGroupIn.Name);

            if(teacherGroupExist is not null && teacherGroupExist.Id != id)
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

            var repondOfteachers = await _teacherService.GetTeacherByGroup(teacherGroup.Id);
            if (repondOfteachers.Result == ResultRespond.Success)
            {
                // set teacher group id to null
                var teachers = repondOfteachers.Data;
                foreach (var teacher in teachers)
                {
                    teacher.GroupId = null;
                }
                
                await _teacherService.UpdateTeachers(teachers);
            }
            await _teacherGroupRepository.RemoveTeacherGroup(teacherGroup.Id);

            return Ok(new RespondApi<TeacherGroup>("Remove teacher group successfully.", teacherGroup, null));
        }

        [HttpPut("add-teacher-to-group")]
        public async Task<IActionResult> AddTeacherToGroup([FromBody] AddTeacherToGroupVM request)
        {
            var result = await  _groupTeacherService.AddTeachersToGroup(request.teacherIds, request.groupId);
            return Ok(result);
        }
        [HttpPost("get-teacher-by-group-by-grade")]
        public async Task<IActionResult> GetTeacherByGroupByGrade([FromBody] GetExcelTeacherByGroupByGrade request)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await  _teacherService.GetAccessTeacherExcelFile(request.SchoolId, request.Month, request.Year, request.UserId);
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                workbook.Dispose();
                return File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: "BaocaoThiDua.xlsx"
                );
            }
        }
    }
}