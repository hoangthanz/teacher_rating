using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Properties.Dtos;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthencationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITeacherRepository _teacherRepository;

        public AuthencationController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ITeacherRepository teacherRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _teacherRepository = teacherRepository;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user is null)
                    return new LoginResponse
                    {
                        AccessToken = "",
                        RefreshToken = "",
                        Name = "",
                        Email = "",
                        PhoneNumber = "",
                        Message = "User not found",
                        Success = false
                    };

                var teacher = await _teacherRepository.GetTeacherByUserId(user.Id.ToString());
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.MobilePhone, user.PhoneNumber),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new("TeacherId", teacher?.Id.ToString() ?? string.Empty)
                };

                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
                claims.AddRange(roleClaims);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("2444666668888888"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );
                var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Message = "Login successfully",

                    Success = true
                };
            }
            catch (Exception e)
            {
                return new LoginResponse
                {
                    AccessToken = "",
                    RefreshToken = "",
                    Name = "",
                    Email = "",
                    PhoneNumber = "",
                    Message = e.Message,
                    Success = false
                };
            }
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterResponse))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                    return new RegisterResponse
                    {
                        Message = "User already exist",
                        Success = false
                    };

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsActive = false,
                    ActiveCode = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    var addRole = await _userManager.AddToRoleAsync(user, "User");
                    if (!addRole.Succeeded)
                        return new RegisterResponse
                        {
                            Message = "Register failed",
                            Success = false
                        };

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

                    return new RegisterResponse
                    {
                        Message = "Register successfully",
                        Success = true
                    };
                }


                return new RegisterResponse
                {
                    Message = result.Errors.First().Description,
                    Success = false
                };
            }
            catch (Exception e)
            {
                return new RegisterResponse
                {
                    Message = e.Message,
                    Success = false
                };
            }
        }


        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest request)
        {
            var appRole = new ApplicationRole
            {
                Name = request.RoleName,
            };

            var createRole = await _roleManager.CreateAsync(appRole);
            if (createRole.Succeeded)
            {
                return Ok(new { message = "Create role successfully" });
            }

            return BadRequest(new { message = "Create role failed" });
        }

        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "00",
                Message = "Thành công",
                Data = users
            };
            return Ok(result);
        }
    }
}