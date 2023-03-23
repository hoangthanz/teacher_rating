using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Mongodb.Services;
using teacher_rating.Properties.Dtos;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelfCriticismController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ISelfCriticismRepository _selfCriticismRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISelfCriticismService _service;

        public SelfCriticismController(
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ISelfCriticismRepository selfCriticismRepository, ITeacherRepository teacherRepository, ISelfCriticismService service)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _selfCriticismRepository = selfCriticismRepository;
            _teacherRepository = teacherRepository;
            _service = service;
        }

        [HttpPost]
        [Route("create-self-criticism")]
        public async Task<IActionResult> CreateSelfCriticism([FromBody] SelfCriticism request)
        {
            try
            {
                var selfCriticism = request;

                if (selfCriticism.AssessmentCriterias.Count == 0)
                    return Ok(new RespondApi<object>()
                    {
                        Result = ResultRespond.NoContent,
                        Code = "400",
                        Message = "Danh sách tiêu chí đánh giá không được để trống",
                        Data = null
                    });
                
                selfCriticism.Id = Guid.NewGuid().ToString();
                selfCriticism.IsSubmitted = false;
                double total = 0;
                foreach (var criteria in request.AssessmentCriterias)
                {
                    if (criteria.IsDeduct)
                    {
                        total -= criteria.Value;
                    }
                    else
                    {
                        total += criteria.Value;
                    }
                }

                selfCriticism.TotalScore = total;
                if (request.TeacherId != null)
                {
                    var teacher = await _teacherRepository.GetTeacherById(request.TeacherId);
                    selfCriticism.Teacher = teacher;
                }

                var user = await _userManager.FindByIdAsync(request.UserId);
                selfCriticism.User = user;

                await _selfCriticismRepository.AddSelfCriticism(selfCriticism);

                var result = new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Success",
                    Data = selfCriticism
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Success",
                    Data = null,
                    Error = e
                });
            }
        }

        [HttpPost]
        [Route("update-status-self-criticism")]
        public async Task<IActionResult> UpdateStatus([FromBody] ChangSubmitAssessment request)
        {
            try
            {
                var selfCriticism = await _selfCriticismRepository.GetSelfCriticismById(request.Id);
                if (selfCriticism == null)
                    return Ok(new RespondApi<object>()
                    {
                        Result = ResultRespond.Fail,
                        Code = "200",
                        Message = "Không tìm thấy tự đánh giá",
                        Data = null
                    }); 
                
                if (selfCriticism.IsSubmitted == request.IsSubmitted)
                    return Ok(new RespondApi<object>()
                    {
                        Result = ResultRespond.Fail,
                        Code = "200",
                        Message = "Trạng thái đã được cập nhật",
                        Data = selfCriticism
                    });
                
                // get self criticism by teacher and month and year
                var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(selfCriticism.TeacherId, selfCriticism.Month, selfCriticism.Year);

                foreach (var self in selfCriticisms)
                {
                    if (self.IsSubmitted == true && self.Id != request.Id)
                        return Ok(new RespondApi<object>()
                        {
                            Result = ResultRespond.Fail,
                            Code = "200",
                            Message = "Đã tồn tại tự đánh giá đã được gửi",
                            Data = selfCriticism
                        });

                }
                
                selfCriticism.IsSubmitted = request.IsSubmitted;
                await _selfCriticismRepository.UpdateSelfCriticism(selfCriticism);

                var result = new RespondApi<object>()
                {
                    Result = ResultRespond.Success,
                    Code = "200",
                    Message = "Success",
                    Data = selfCriticism 
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Success",
                    Data = null,
                    Error = e
                });
            }
        }


        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetSelfCriticismByUser(string userId)
        {
            try
            {
                var selfCriticism = await _selfCriticismRepository.GetSelfCriticismsByUser(userId);
                var result = new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Success",
                    Data = selfCriticism
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Success",
                    Data = new object(),
                    Error = e
                });
            }
        }

        [HttpGet("get-by-condition")]
        public async Task<IActionResult> GetByCondition([FromQuery] SearchSelfCriticism model)
        {
            var result = await _selfCriticismRepository.GetByCondition(model);
            return Ok(result);
        }
        [HttpPost("get-excel/{schoolId}/{year:int}/{month:int}/{userId}")]
        public async Task<IActionResult> GetExcel(string schoolId, int year, int month, string userId)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await _service.GetSelfCriticismExcelFile(schoolId, month, year, userId);
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