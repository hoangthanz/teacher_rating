﻿using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IGradeConfigurationRepository
{
    Task<GradeConfiguration?> GetGradeConfigurationById(string id);
    Task<List<GradeConfiguration>> GetAllGradeConfigurations();
    Task AddGradeConfiguration(GradeConfiguration gradeConfiguration);
    Task UpdateGradeConfiguration(GradeConfiguration gradeConfiguration);
    Task<GradeConfiguration> GetGradeConfigurationByScore(int score, string schoolId);
    Task RemoveGradeConfiguration(string id);
}