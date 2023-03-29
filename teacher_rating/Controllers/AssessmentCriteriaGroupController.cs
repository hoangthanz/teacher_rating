using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentCriteriaGroupController : ControllerBase
    {
        private readonly IAssessmentCriteriaGroupRepository _assessmentCriteriaGroupRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;

        public AssessmentCriteriaGroupController(IAssessmentCriteriaGroupRepository assessmentCriteriaGroupRepository,
            IAssessmentCriteriaRepository assessmentCriteriaRepository)
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
            var assessmentCriteriaList = await _assessmentCriteriaRepository.GetAllAssessmentCriters();
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
            var assessmentCriteriaList = await _assessmentCriteriaRepository.GetAllAssessmentCritersByGroupId(id);
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = assessmentCriteriaList
            };
            return Ok(result);
        }

        [HttpPost]
        [Route("create-assessment-criteria-group")]
        public async Task<IActionResult> CreateAssessmentCriteriaGroup(AssessmentCriteriaGroup group)
        {
            // check group name exist
            var assessmentCriteriaGroup = await _assessmentCriteriaGroupRepository.GetAllAssessmentCriteriaGroups();
            if (assessmentCriteriaGroup.Any(x => x.Name == group.Name && group.SchoolId == x.SchoolId))
            {
                return Ok(new RespondApi<object>()
                {
                    Result = ResultRespond.Error,
                    Code = "0",
                    Message = "Trùng tên nhóm tiêu chí",
                    Data = null
                });
            }

            await _assessmentCriteriaGroupRepository.AddAssessmentCriteriaGroup(group);
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = group
            };
            return Ok(result);
        }

        [HttpPost]
        [Route("update-assessment-criteria-group")]
        public async Task<IActionResult> UpdateAssessmentCriteriaGroup(AssessmentCriteriaGroup group)
        {
            // check group name exist
            var assessmentCriteriaGroup = await _assessmentCriteriaGroupRepository.GetAllAssessmentCriteriaGroups();
            if (assessmentCriteriaGroup.Any(x =>
                    x.Name == group.Name && group.SchoolId == x.SchoolId && x.Id != group.Id))
            {
                return Ok(new RespondApi<object>()
                {
                    Result = ResultRespond.Error,
                    Code = "0",
                    Message = "Trùng tên nhóm tiêu chí",
                    Data = null
                });
            }


            await _assessmentCriteriaGroupRepository.UpdateAssessmentCriteriaGroup(group);
            var result = new RespondApi<object>()
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = group
            };
            return Ok(result);
        }

        [HttpDelete]
        [Route("delete-assessment-criteria-group/{id}")]
        public async Task<IActionResult> UpdateAssessmentCriteriaGroup(string id)
        {
            // check group name exist
            var group = await _assessmentCriteriaGroupRepository.GetAssessmentCriterGroupById(id);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (group is null)
                return Ok(new RespondApi<object>()
                {
                    Result = ResultRespond.Fail,
                    Code = "200",
                    Message = "Không tìm thấy nhóm tiêu chí",
                    Data = null
                });

            // delete all assessment criteria
            await _assessmentCriteriaGroupRepository.RemoveAssessmentCriteriaGroup(id);
            var result = new RespondApi<object>
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = group
            };
            return Ok(result);
        }


        [HttpPost]
        [Route("create-assessment-criteria")]
        public async Task<IActionResult> CreateAssessmentCriteria(CreateAssessmentCriteria assessmentCriteria)
        {
            // check group name exist
            var criteria = new AssessmentCriteria()
            {
                Id = Guid.NewGuid().ToString(),
                AssessmentCriteriaGroupId = assessmentCriteria.AssessmentCriteriaGroupId,
                Name = assessmentCriteria.Name,
                SchoolId = assessmentCriteria.SchoolId,
                Quantity = assessmentCriteria.Quantity,
                Unit = assessmentCriteria.Unit,
                Value = assessmentCriteria.Value,
                IsDeduct = assessmentCriteria.IsDeduct,
                DeductScore = assessmentCriteria.DeductScore,
                IsDeleted = false,
                AllowUpdateScore = assessmentCriteria.AllowUpdateScore
            };
            await _assessmentCriteriaRepository.AddAssessmentCriter(criteria);
            var result = new RespondApi<object>
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = criteria
            };
            return Ok(result);
        }

        [HttpPost]
        [Route("update-assessment-criteria")]
        public async Task<IActionResult> UpdateAssessmentCriteria(AssessmentCriteria assessmentCriteria)
        {
            // check group name exist
            var criteria = await _assessmentCriteriaRepository.GetAssessmentCriterById(assessmentCriteria.Id);
            if (criteria is null)
                return Ok(new RespondApi<object>
                {
                    Result = ResultRespond.Error,
                    Code = "200",
                    Message = "Not found assessment criteria",
                    Data = assessmentCriteria
                });
            
            await _assessmentCriteriaRepository.UpdateAssessmentCriter(assessmentCriteria);
            var result = new RespondApi<object>
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = assessmentCriteria
            };
            return Ok(result);
        }
        
        [HttpDelete]
        [Route("delete-assessment-criteria/{id}")]
        public async Task<IActionResult> DeleteAssessmentCriteria(string id)
        {
            // check group name exist
            var criteria = await _assessmentCriteriaRepository.GetAssessmentCriterById(id);
            if (criteria is null)
                return Ok(new RespondApi<object>
                {
                    Result = ResultRespond.Error,
                    Code = "200",
                    Message = "Not found assessment criteria",
                    Data = null
                });
            
            await _assessmentCriteriaRepository.RemoveAssessmentCriter(id);
            var result = new RespondApi<object>
            {
                Result = ResultRespond.Success,
                Code = "200",
                Message = "Success",
                Data = null
            };
            return Ok(result);
        }
    }
}