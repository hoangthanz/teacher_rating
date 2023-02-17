using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Mongodb.Data.Interfaces;
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

        public SelfCriticismController(
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ISelfCriticismRepository selfCriticismRepository
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _selfCriticismRepository = selfCriticismRepository;
        }

        [HttpPost]
        [Route("create-self-criticism")]
        public async Task<IActionResult> CreateSelfCriticism([FromBody] SelfCriticism request)
        {
            try
            {
                var selfCriticism = request;
                selfCriticism.Id = Guid.NewGuid().ToString();
                selfCriticism.IsSubmitted = false;

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
    }
}