using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeConfigurationController : ControllerBase
    {
        private readonly IGradeConfigurationRepository _gradeConfigurationRepository;

        public GradeConfigurationController(IGradeConfigurationRepository gradeConfigurationRepository)
        {
            _gradeConfigurationRepository = gradeConfigurationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<RespondApi<IEnumerable<GradeConfiguration>>>> GetGradeConfigurations()
        {
            try
            {
                var gradeConfigurations = await _gradeConfigurationRepository.GetAllGradeConfigurations();
                return new RespondApi<IEnumerable<GradeConfiguration>>(ResultRespond.Success, "00",
                    "Get grade configurations successfully", gradeConfigurations);
            }
            catch (Exception ex)
            {
                return new RespondApi<IEnumerable<GradeConfiguration>>(ResultRespond.Error, "01",
                    "Error occurred while getting grade configurations", null, ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetGradeConfiguration")]
        public async Task<ActionResult<RespondApi<GradeConfiguration>>> GetGradeConfiguration(string id)
        {
            try
            {
                var gradeConfiguration = await _gradeConfigurationRepository.GetGradeConfigurationById(id);

                if (gradeConfiguration == null)
                {
                    return new RespondApi<GradeConfiguration>(ResultRespond.NotFound, "02",
                        $"Grade configuration with ID: {id} not found", null);
                }

                return new RespondApi<GradeConfiguration>(ResultRespond.Success, "00",
                    $"Get grade configuration with ID: {id} successfully", gradeConfiguration);
            }
            catch (Exception ex)
            {
                return new RespondApi<GradeConfiguration>(ResultRespond.Error, "03",
                    $"Error occurred while getting grade configuration with ID: {id}", null, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<RespondApi<GradeConfiguration>>> CreateGradeConfiguration(
            GradeConfiguration gradeConfiguration)
        {
            try
            {
                await _gradeConfigurationRepository.AddGradeConfiguration(gradeConfiguration);
                return new RespondApi<GradeConfiguration>(ResultRespond.Success, "00",
                    $"Create grade configuration with ID: {gradeConfiguration.Id} successfully", gradeConfiguration);
            }
            catch (Exception ex)
            {
                return new RespondApi<GradeConfiguration>(ResultRespond.Error, "04",
                    "Error occurred while creating grade configuration", null, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RespondApi<GradeConfiguration>>> UpdateGradeConfiguration(string id,
            GradeConfiguration gradeConfigurationIn)
        {
            try
            {
                var gradeConfiguration = await _gradeConfigurationRepository.GetGradeConfigurationById(id);

                if (gradeConfiguration == null)
                {
                    return new RespondApi<GradeConfiguration>(ResultRespond.NotFound, "05",
                        $"Grade configuration with ID: {id} not found", null);
                }

                await _gradeConfigurationRepository.UpdateGradeConfiguration(gradeConfigurationIn);

                return new RespondApi<GradeConfiguration>(ResultRespond.Success, "00",
                    $"Update grade configuration with ID: {id} successfully", gradeConfigurationIn);
            }
            catch (Exception ex)
            {
                return new RespondApi<GradeConfiguration>(ResultRespond.Error, "06",
                    $"Error occurred while updating grade configuration with ID: {id}", null, ex.Message);
            }
        }
    }
}