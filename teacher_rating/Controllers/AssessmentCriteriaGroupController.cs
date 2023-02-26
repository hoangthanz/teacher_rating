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
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
        public AssessmentCriteriaGroupController(IAssessmentCriteriaGroupRepository assessmentCriteriaGroupRepository, IAssessmentCriteriaRepository assessmentCriteriaRepository)
        {
            _assessmentCriteriaGroupRepository = assessmentCriteriaGroupRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
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
        
        [HttpGet]
        [Route("get-all-assessment-criteria")]
        public async Task<IActionResult> GetAllAssessmentCriteria()
        {
            var assessmentCriteriaList  = await _assessmentCriteriaRepository.GetAllAssessmentCriters();
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = assessmentCriteriaList
            };
            return Ok(result);
        }
        
        [HttpGet]
        [Route("get-assessment-criteria/{id}")]
        public async Task<IActionResult> GetAssessmentByGroupId(string id)
        {
            var assessmentCriteriaList  = await _assessmentCriteriaRepository.GetAllAssessmentCritersByGroupId(id);
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = assessmentCriteriaList
            };
            return Ok(result);
        }
    }
}
