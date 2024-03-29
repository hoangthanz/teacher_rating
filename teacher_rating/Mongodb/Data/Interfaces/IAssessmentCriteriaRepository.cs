﻿using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IAssessmentCriteriaRepository
{
    Task<AssessmentCriteria> GetAssessmentCriterById(string id);
    Task<IEnumerable<AssessmentCriteria>> GetAllAssessmentCriters();
    Task<List<AssessmentCriteria>> GetAllAssessmentCritersByGroupId(string id);
    Task AddAssessmentCriter(AssessmentCriteria teacher);
    Task AddAssessmentCriterList(List<AssessmentCriteria> criteriaList);
    Task UpdateAssessmentCriter(AssessmentCriteria teacher);
    Task RemoveAssessmentCriter(string id);
    Task<List<AssessmentCriteria>> GetAssessmentCriteriasBySchoolId(string schoolId);
}