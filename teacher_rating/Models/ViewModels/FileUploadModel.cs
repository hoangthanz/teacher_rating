﻿using teacher_rating.Enums;

namespace teacher_rating.Models.ViewModels;

public class FileUploadModel
{
    public IFormFile FileDetails { get; set; }
    public FileType FileType { get; set; }
    public string SchoolId { get; set; }
    public string Description { get; set; }
}