using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IAssessmentCriteriaRepository
{
    Task<AssessmentCriteria> GetAssessmentCriterById(string id);
    Task<IEnumerable<AssessmentCriteria>> GetAllAssessmentCriters();
    Task AddAssessmentCriter(AssessmentCriteria teacher);
    Task UpdateAssessmentCriter(AssessmentCriteria teacher);
    Task RemoveAssessmentCriter(string id);
}