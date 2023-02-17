using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IAssessmentCriteriaGroupRepository
{
    Task<AssessmentCriteriaGroup> GetAssessmentCriterGroupById(string id);
    Task<IEnumerable<AssessmentCriteriaGroup>> GetAllAssessmentCriteriaGroups();
    Task AddAssessmentCriteriaGroup(AssessmentCriteriaGroup teacher);
    Task UpdateAssessmentCriteriaGroup(AssessmentCriteriaGroup teacher);
    Task RemoveAssessmentCriteriaGroup(string id);
}