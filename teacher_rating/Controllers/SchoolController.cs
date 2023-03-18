using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;

        public SchoolController(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        
        [HttpGet("get-all")]
        public async Task<ActionResult<RespondApi<List<School>>>> GetAll()
        {
            var schools = await _schoolRepository.GetAll();

            return Ok(new RespondApi<List<School>>
            {
                Result = ResultRespond.Success,
                Data = schools,
            });
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<RespondApi<School>>> Get(string id)
        {
            var school = await _schoolRepository.GetById(id);

            if (school == null)
            {
                return NotFound(new RespondApi<School>
                {
                    Result = ResultRespond.NotFound,
                    Code = "01",
                    Message = "School not found",
                });
            }

            return Ok(new RespondApi<School>
            {
                Result = ResultRespond.Success,
                Data = school,
            });
        }

        [HttpPost]
        public async Task<ActionResult<RespondApi<School>>> Create([FromBody] CreateSchool school)
        {
            try
            {
                var result = await _schoolRepository.Create(school);

                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new RespondApi<School>
                {
                    Result = ResultRespond.Success,
                    Message = "School created failed",
                    Error = e
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RespondApi<School>>> Update(string id, [FromBody] School school)
        {
            var existingSchool = await _schoolRepository.GetById(id);

            if (existingSchool == null)
            {
                return NotFound(new RespondApi<School>
                {
                    Result = ResultRespond.NotFound,
                    Code = "01",
                    Message = "School not found",
                });
            }

            school.Id = existingSchool.Id;

            var updatedSchool = await _schoolRepository.Update(id, school);
            if (!updatedSchool)
                return Ok(new RespondApi<School>
                {
                    Result = ResultRespond.Fail,
                    Message = "School update failed",
                    Data = null,
                });
            return Ok(new RespondApi<School>
            {
                Result = ResultRespond.Success,
                Data = null,
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RespondApi<School>>> Delete(string id)
        {
            var existingSchool = await _schoolRepository.GetById(id);

            if (existingSchool == null)
            {
                return NotFound(new RespondApi<School>
                {
                    Result = ResultRespond.NotFound,
                    Code = "01",
                    Message = "School not found",
                });
            }

            await _schoolRepository.Delete(id);

            return Ok(new RespondApi<School>
            {
                Result = ResultRespond.Success,
                Message = "School deleted successfully",
            });
        }
    }
}