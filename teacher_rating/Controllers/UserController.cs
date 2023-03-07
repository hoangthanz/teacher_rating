using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Properties.Dtos;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITeacherRepository _teacherRepository;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ITeacherRepository teacherRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _teacherRepository = teacherRepository;
        }

        [HttpGet]
        [Route("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(new RespondApi<object>()
            {
                Message = "Success",
                Result = ResultRespond.Success,
                Code = "00",
                Data = roles
            });
        }
       
        
        [HttpGet]
        [Route("get-user-by-school-id/{schoolId}")]
        public async Task<IActionResult> GetUsersBySchool(string schoolId)
        {
            var users = _userManager.Users.Where(x => x.SchoolId == schoolId).ToList();
            return Ok(new RespondApi<object>()
            {
                Message = "Success",
                Result = ResultRespond.Success,
                Code = "00",
                Data = users
            });
        }
        
        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser request)
        {
            // create user object 
            var user = new ApplicationUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DisplayName = request.DisplayName,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsActive = true,
                ActiveCode = Guid.NewGuid().ToString(),
                IsDeleted = false,
                SchoolId = request.SchoolId
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // create teacher 
                var teacher = new Teacher
                {
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Id = Guid.NewGuid().ToString(),
                    User = user,
                    UserId = user.Id,
                    AssessmentRecords = new List<AssessmentRecord>()
                };

                await _teacherRepository.AddTeacher(teacher);
                
                // add role for user
                await _userManager.AddToRoleAsync(user, "Teacher");
                    

                return Ok(new RespondApi<object>()
                {
                    Message = "Register failed",
                    Result = ResultRespond.Success,
                    Code = "00",
                    Data = user
                });
            }

            return Ok(new RespondApi<object>()
            {
                Message = "Register failed",
                Result = ResultRespond.Fail,
                Code = "00",
                Data = null!
            });
        }

        [HttpPost]
        [Route("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUser request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user is null)
                return Ok(new RespondApi<object>()
                {
                    Message = "Không tìm thấy user",
                    Result = ResultRespond.NotFound,
                    Code = "00",
                    Data = null!
                });

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.DisplayName = request.DisplayName;
            user.UpdatedDate = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var teacher = await _teacherRepository.GetTeacherByUserId(request.Id);
                if (teacher is null)
                    return Ok(new RespondApi<object>()
                    {
                        Message = "Không tìm thấy giáo viên",
                        Result = ResultRespond.NotFound,
                        Code = "00",
                        Data = null!
                    });

                teacher.Email = request.Email;
                teacher.PhoneNumber = request.PhoneNumber;
                teacher.Name = request.DisplayName;
                
                await _teacherRepository.UpdateTeacher(teacher);

                return Ok(new RespondApi<object>()
                {
                    Message = "Register success",
                    Result = ResultRespond.Success,
                    Code = "00",
                    Data = user
                });
            }

            return Ok(new RespondApi<object>()
            {
                Message = "Register failed",
                Result = ResultRespond.Fail,
                Code = "00",
                Data = null!
            });
        }
        
        [HttpDelete("remove-user/{id}")]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return Ok(new RespondApi<object>()
                {
                    Message = "Không tìm thấy user",
                    Result = ResultRespond.NotFound,
                    Code = "00",
                    Data = null!
                });

            if (user.UserName.ToLower() == "admin")
            {
                return Ok(new RespondApi<object>()
                {
                    Message = "Không thể xóa tài khoản admin",
                    Result = ResultRespond.NotFound,
                    Code = "00",
                    Data = null!
                });
            }
            
            user.IsDeleted = true;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var teacher = await _teacherRepository.GetTeacherByUserId(id);
                if (teacher is null)
                    return Ok(new RespondApi<object>()
                    {
                        Message = "Không tìm thấy giáo viên",
                        Result = ResultRespond.NotFound,
                        Code = "00",
                        Data = null!
                    });

                teacher.IsDeleted = true;
                
                await _teacherRepository.UpdateTeacher(teacher);

                return Ok(new RespondApi<object>()
                {
                    Message = "Remove success",
                    Result = ResultRespond.Success,
                    Code = "00",
                    Data = user
                });
            }

            return Ok(new RespondApi<object>()
            {
                Message = "Register failed",
                Result = ResultRespond.Fail,
                Code = "00",
                Data = null!
            });
        }
    }
}