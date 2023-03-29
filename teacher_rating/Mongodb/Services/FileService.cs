using teacher_rating.Common.Models;
using teacher_rating.Enums;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public class FileService : IFileService
{
    private readonly IFileDetailsRepository _fileDetailsRepository;

    public FileService(IFileDetailsRepository fileDetailsRepository)
    {
        _fileDetailsRepository = fileDetailsRepository;
    }

    public async Task<RespondApi<FileDetails>> PostFileAsync(IFormFile fileData, FileType fileType)
    {
        try
        {
            var fileDetails = new FileDetails()
            {
                Id = Guid.NewGuid().ToString(),
                FileName = fileData.FileName,
                FileType = fileType,
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
    public async Task CopyStream(Stream stream, string downloadPath)
    {
        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
}