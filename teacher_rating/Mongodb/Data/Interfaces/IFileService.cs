using teacher_rating.Enums;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IFileService
{
    public Task PostFileAsync(IFormFile fileData, FileType fileType);

    public Task PostMultiFileAsync(List<FileUploadModel> fileData);

    public Task DownloadFileById(List<string> ids);
}