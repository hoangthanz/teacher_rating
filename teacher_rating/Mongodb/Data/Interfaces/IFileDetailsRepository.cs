using teacher_rating.Common;
using teacher_rating.Common.Models;
using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IFileDetailsRepository
{
    public Task<RespondApi<object>> Insert(FileDetails fileDetails);
    public Task<RespondApi<List<FileDetails>>> Search(List<string> ids);
    public Task<RespondApi<object>> Update(FileDetails fileDetails);
    public Task<RespondApi<object>> Remove(string id);
}