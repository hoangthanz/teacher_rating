using teacher_rating.Common;
using teacher_rating.Common.Models;
using teacher_rating.Models;

namespace teacher_rating.Mongodb.Data.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    public Task<RespondApi<object>> InsertDepartment(Department department);
    public Task<RespondApi<object>> UpdateDepartment(Department department);
    public Task<RespondApi<object>> RemoveDepartment(string id);
}