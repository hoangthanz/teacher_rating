using teacher_rating.Common.Models;
using teacher_rating.Enums;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IFileService
{
    public Task<RespondApi<FileDetails>> PostFileAsync(IFormFile fileData, FileType fileType, FileUploadModel fileDetails);

    public Task PostMultiFileAsync(List<FileUploadModel> fileData);

    public Task DownloadFileById(List<string> ids);
    Task<RespondApi<List<FileDetails>>> GetAll(string schoolId);
    Task<RespondApi<FileDetails>> GetById(string id);
}