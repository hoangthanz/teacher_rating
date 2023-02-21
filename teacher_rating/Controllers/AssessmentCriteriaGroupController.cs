using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentCriteriaGroupController : ControllerBase
    {
        private readonly IAssessmentCriteriaGroupRepository _assessmentCriteriaGroupRepository;
        public AssessmentCriteriaGroupController(IAssessmentCriteriaGroupRepository assessmentCriteriaGroupRepository)
        {
            _assessmentCriteriaGroupRepository = assessmentCriteriaGroupRepository;
        }

        [HttpGet]
        [Route("get-all-assessment-criteria-group")]
        public async Task<IActionResult> GetAll()
        {
            var assessmentCriteriaGroups = await _assessmentCriteriaGroupRepository.GetAllAssessmentCriteriaGroups();
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = assessmentCriteriaGroups
            };
            return Ok(result);
        }
    }
}
