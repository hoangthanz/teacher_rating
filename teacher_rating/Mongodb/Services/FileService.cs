using System.Security.Claims;
using teacher_rating.Common.Models;
using teacher_rating.Enums;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public class FileService : IFileService
{
    private readonly IFileDetailsRepository _fileDetailsRepository;
    private readonly string? _userId;
    public FileService(IFileDetailsRepository fileDetailsRepository, IHttpContextAccessor httpContext)
    {
        _fileDetailsRepository = fileDetailsRepository;
        _userId = httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) != null
            ? httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;
    }

    public async Task<RespondApi<FileDetails>> PostFileAsync(IFormFile fileData, FileType fileType, FileUploadModel details)
    {
        try
        {
            var fileDetails = new FileDetails()
            {
                Id = Guid.NewGuid().ToString(),
                FileName = fileData.FileName,
                FileType = fileType,
                SchoolId = details.SchoolId,
                Description = details.Description,
                CreateDate = DateTime.Now,
                UserId = _userId
            };

            using (var stream = new MemoryStream())
            {
                fileData.CopyTo(stream);
                fileDetails.FileData = stream.ToArray();
            }

            var result = await _fileDetailsRepository.Insert(fileDetails);
            if(result.Result != ResultRespond.Success)
            {
                return new RespondApi<FileDetails>() { Message = "Error", Result = ResultRespond.Error };
            }
            return new RespondApi<FileDetails>() { Message = "Error", Result = ResultRespond.Success, Data = fileDetails };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        try
        {
            foreach (FileUploadModel file in fileData)
            {
                var fileDetails = new FileDetails()
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = file.FileDetails.FileName,
                    FileType = file.FileType,
                    SchoolId = file.SchoolId,
                    Description = file.Description
                };

                using (var stream = new MemoryStream())
                {
                    file.FileDetails.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }

                var result = _fileDetailsRepository.Insert(fileDetails);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    public async Task DownloadFileById(List<string> ids)
    {
        try
        {
            var res = await _fileDetailsRepository.Search(ids);

            if (res.Result != ResultRespond.Success)
            {
                return;
            }

            // file

            foreach (var file in res.Data)
            {
                var content = new System.IO.MemoryStream(file.FileData);
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "FileDownloaded",
                    file.FileName);

                await CopyStream(content, path);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<RespondApi<List<FileDetails>>> GetAll(string schoolId)
    {
        try
        {
            var res = await _fileDetailsRepository.GetAllBySchool(schoolId);
            return res;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RespondApi<FileDetails>> GetById(string id)
    {
        var res = await _fileDetailsRepository.Search(new List<string>(){id});
        if (res.Result != ResultRespond.Success)
        {
            return new RespondApi<FileDetails>() { Message = "Error", Result = ResultRespond.Error };
        }
       
        var file = res.Data.FirstOrDefault();
        if (file == null)
        {
            return new RespondApi<FileDetails>() { Message = "Error", Result = ResultRespond.Error };
        }
        return new RespondApi<FileDetails>() { Message = "Error", Result = ResultRespond.Success, Data = file };
    }

    public async Task CopyStream(Stream stream, string downloadPath)
    {
        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
}