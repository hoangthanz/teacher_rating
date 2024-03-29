using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Common.Models.Paging;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Mongodb.Services;
using teacher_rating.Properties.Dtos;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

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
        private readonly ITeacherGroupRepository _teacherGroupRepository;
        private readonly ISelfCriticismService _service;
        private readonly IGradeConfigurationRepository _gradeConfigurationRepository;
        private readonly IMapper _mapper;
        private readonly string? _userId;
        private readonly string? _teacherId;
        private readonly List<string>? _roles;

        public SelfCriticismController(
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IHttpContextAccessor httpContext,
            ISelfCriticismRepository selfCriticismRepository, ITeacherRepository teacherRepository,
            ISelfCriticismService service, IGradeConfigurationRepository gradeConfigurationRepository, IMapper mapper, ITeacherGroupRepository teacherGroupRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _selfCriticismRepository = selfCriticismRepository;
            _teacherRepository = teacherRepository;
            _service = service;
            _gradeConfigurationRepository = gradeConfigurationRepository;
            _mapper = mapper;
            _teacherGroupRepository = teacherGroupRepository;
            _userId = httpContext?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier) != null
                ? httpContext?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
            _teacherId = httpContext?.HttpContext?.User?.FindFirst("TeacherId") != null
                ? httpContext?.HttpContext?.User?.FindFirst("TeacherId")?.Value
                : null;
            _roles = httpContext?.HttpContext?.User?.FindAll(ClaimTypes.Role) != null ?
                httpContext?.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(x => x.Value).ToList() : null;
        }

        [HttpPost]
        [Route("create-self-criticism")]
        public async Task<IActionResult> CreateSelfCriticism([FromBody] CreateSelfCriticism request)
        {
            try
            {
                var selfCriticism = _mapper.Map<SelfCriticism>(request);
                

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
                selfCriticism.UserId = _userId;
                selfCriticism.CreatedDate = DateTime.Now;
                selfCriticism.TeacherId = _teacherId;
                double total = 0;
                foreach (var criteria in request.AssessmentCriterias)
                {
                    if (criteria.IsDeduct)
                    {
                        total -= criteria.Value * criteria.Quantity;
                    }
                    else
                    {
                        total += criteria.Value * criteria.Quantity;
                    }
                }

                selfCriticism.TotalScore = 100 + total;
                if (!string.IsNullOrEmpty(request.TeacherId))
                {
                    var teacher = await _teacherRepository.GetTeacherById(request.TeacherId);
                    selfCriticism.Teacher = teacher;
                }

                var user = await _userManager.FindByIdAsync(request.UserId);
                selfCriticism.User = user;
                selfCriticism.UserId = user != null ? user.Id.ToString() : request.UserId;
                if (_roles != null && _roles.Contains("Admin"))
                {
                    selfCriticism.IsSubmitted = true;
                    selfCriticism.IsCreatedByAdmin = true;
                }

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
        [Route("update-self-criticism")]
        public async Task<IActionResult> UpdateSelfCriticism([FromBody] SelfCriticism model)
        {
            try
            {
                if (model.AssessmentCriterias.Count <= 0)
                {
                    return Ok(new RespondApi<object>()
                    {
                        Code = "400",
                        Message = "Danh sách tiêu chí đánh giá không được để trống",
                        Data = null,
                    });
                }

                var self = await _selfCriticismRepository.GetSelfCriticismById(model.Id);
                if (self == null)
                {
                    return Ok(new RespondApi<string>()
                    {
                        Code = "400",
                        Message = "Không tìm thấy bản tự khai",
                        Data = null,
                    });
                }

                self.AssessmentCriterias = model.AssessmentCriterias;
                self.TotalScore = 100 +
                                  self.AssessmentCriterias.Sum(x =>
                                      (x.IsDeduct) ? x.Value * x.Quantity * -1 : x.Value * x.Quantity);
                await _selfCriticismRepository.UpdateSelfCriticism(self);
                return Ok(new RespondApi<string>()
                {
                    Code = "200",
                    Message = "Thành công",
                    Data = self.Id,
                });
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Fail",
                    Data = null,
                    Error = e
                });
            }
        }

        [HttpPost]
        [Route("update-self-criticism-by-admin")]
        public async Task<IActionResult> UpdateSelfCriticismAdmin([FromBody] UpdateAssessmentCriteriaAdmin model)
        {
            try
            {
                if (model.AssessmentCriterias.Count <= 0)
                {
                    return Ok(new RespondApi<object>()
                    {
                        Code = "400",
                        Message = "Danh sách tiêu chí đánh giá không được để trống",
                        Data = null,
                    });
                }

                var self = await _selfCriticismRepository.GetSelfCriticismById(model.Id);
                if (self == null)
                {
                    return Ok(new RespondApi<string>()
                    {
                        Code = "400",
                        Message = "Không tìm thấy bản tự khai",
                        Data = null,
                    });
                }

                var updated = _mapper.Map<SelfCriticism>(model);

                updated.AssessmentCriterias = model.AssessmentCriterias;
                updated.TotalScore =
                    100 + updated.AssessmentCriterias.Sum(x =>
                        (x.IsDeduct) ? x.Value * x.Quantity * -1 : x.Value * x.Quantity);


                await _selfCriticismRepository.UpdateSelfCriticism(updated);
                return Ok(new RespondApi<string>()
                {
                    Code = "200",
                    Message = "Thành công",
                    Data = self.Id,
                });
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<object>()
                {
                    Code = "200",
                    Message = "Fail",
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
                var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(selfCriticism.TeacherId,
                    selfCriticism.Month, selfCriticism.Year);

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

        [HttpPost("get-by-condition")]
        public async Task<IActionResult> GetByCondition([FromBody] SearchSelfCriticism model)
        {
            bool isLeader = false;
            if (_userId != null)
            {
                var teacher = await _teacherRepository.GetTeacherByUserId(_userId);
                if (teacher != null)
                {
                    var teacherGroup = await _teacherGroupRepository.GetTeacherGroupById(teacher.GroupId);
                    if (teacher.Id == teacherGroup.LeaderId)
                    {
                        isLeader = true;
                        model.GroupId = teacher.GroupId;
                    }
                }
            }
            var dataOfselfCriticisms = await _selfCriticismRepository.GetByCondition(model, isLeader);
            var cofigs = await _gradeConfigurationRepository.GetAllGradeConfigurations();

            var selfCriticisms = _mapper.Map<List<SelfCriticismViewModel>>(dataOfselfCriticisms.Data);
            foreach (var selfCriticism in selfCriticisms)
            {
                var config = cofigs.FirstOrDefault(x =>
                    x.SchoolId == selfCriticism.SchoolId && x.MinimumScore <= selfCriticism.TotalScore &&
                    selfCriticism.TotalScore <= x.MaximumScore);
                if (config != null)
                    selfCriticism.CompetitiveRanking = config.Name;
            }

            PagingResponse paging = null;
            if (model.IsPaging)
            {
                paging = new PagingResponse()
                {
                    CurrentPage = model.PageNumber,
                    PageSize = model.PageSize,
                };
                paging.TotalRecords = (int)selfCriticisms.Count;
                paging.TotalPages = (int)Math.Ceiling(paging.TotalRecords / (double)paging.PageSize);
            }

            var resultOfPaging = new RespondAPIPaging<List<SelfCriticismViewModel>>()
                { Result = ResultRespond.Success, Data = selfCriticisms, Paging = paging };
            return Ok(resultOfPaging);
        }

        [HttpPost("get-excel/{schoolId}/{year:int}/{month:int}/{userId}")]
        public async Task<IActionResult> GetExcel(string schoolId, int year, int month, string userId,
            [FromBody] List<string> groupIds)
        {
            string title ;
            if (_roles.Contains("Admin"))
            {
                title = "BaocaoThiDua.xlsx";
            }
            else
            {
                var user = await _userManager.FindByIdAsync(_userId);
                title = $"BaocaoThiDua_{user.DisplayName}_{month.ToString()}_{year.ToString()}.xlsx";
            }
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await _service.GetSelfCriticismExcelFileNew(schoolId, month, year, userId, groupIds);
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                workbook.Dispose();
                return File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: title
                );
            }
        }

        [HttpPost("get-competition_board")]
        public async Task<IActionResult> GetCompetitionBoard([FromBody] GetCompetitionBoardRequest model)
        {
            string title = "Bảng thi đua tổ viên";
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await _service.GetCompetitionBoard(model);
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                workbook.Dispose();
                return File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: title
                );
            }
        }
        [HttpPost("get-sample-excel")]
        public async Task<IActionResult> GetSampleExcel([FromQuery] int year)
        {
            string title = "MauBaoCaoThiDua.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await _service.GetSampleExcelFile(year);
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                workbook.Dispose();
                return File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: title
                );
            }
        }
        [HttpPost("get-personal-archievement-excel")]
        public async Task<IActionResult> GetPersonalArchivementExcel([FromBody] GetPersonalArchievementRequest model)
        {
            string title = "MauThanhTichCanhan.xlsx";
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = await _service.GetPersonalArchievement(model);
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                workbook.Dispose();
                return File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: title
                );
            }
        }
    }
}