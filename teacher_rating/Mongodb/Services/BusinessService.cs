using teacher_rating.Common.Contracts;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface IBusinessService
{
    Task<RespondApi<object>> CreateDepartmentAsync(Department department);
    Task<RespondApi<object>> UpdateDepartmentAsync(Department department);
    Task<RespondApi<object>> DeleteDepartmentAsync(string id);
    
    Task<RespondApi<object>> CreateTeacherAsync(Teacher teacher);
    Task<RespondApi<object>> UpdateTeacherAsync(Teacher teacher);
    Task<RespondApi<object>> DeleteTeacherAsync(Teacher teacher);
}

public class BusinessService: IBusinessService
{
    private IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BusinessService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RespondApi<object>> CreateDepartmentAsync(Department department)
    {
        var result = await _departmentRepository.InsertDepartment(department);
        return result;
    }

    public async Task<RespondApi<object>> UpdateDepartmentAsync(Department department)
    {
        var result = await _departmentRepository.UpdateDepartment(department);
        return result;
    }

    public async Task<RespondApi<object>> DeleteDepartmentAsync(string id)
    {
        var result = await _departmentRepository.RemoveDepartment(id);
        return result;
    }

    public async Task<RespondApi<object>> CreateTeacherAsync(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public async Task<RespondApi<object>> UpdateTeacherAsync(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public async Task<RespondApi<object>> DeleteTeacherAsync(Teacher teacher)
    {
        throw new NotImplementedException();
    }
}