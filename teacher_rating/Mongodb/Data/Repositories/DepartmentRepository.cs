using CKLS.Services.Models.MongoDb.Common;
using teacher_rating.Common;
using teacher_rating.Common.Contracts;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Data.Repositories;

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMongoContext _context;
    private readonly string _userId;
    private readonly IEnumerable<string> _role;
    
    public DepartmentRepository(IMongoContext context, IUnitOfWork unitOfWork) : base(context)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<RespondApi<object>> InsertDepartment(Department department)
    {
        try
        {
            await Add(department);
            return new RespondApi<object>()
            {
                Data = department,
                Message = "Success",
                Result = ResultRespond.Success
            };
        }
        catch (Exception e)
        {
            return new RespondApi<object>
            {
                Data = null,
                Message = e.Message,
                Result = ResultRespond.Error
            };
        }
    }

    public async Task<RespondApi<object>> UpdateDepartment(Department department)
    {
        try
        {
            await Update(department);
            return new RespondApi<object>()
            {
                Data = department,
                Message = "Success",
                Result = ResultRespond.Success
            };
        }
        catch (Exception e)
        {
            return new RespondApi<object>
            {
                Data = null,
                Message = e.Message,
                Result = ResultRespond.Error
            };
        }
    }

    public async Task<RespondApi<object>> RemoveDepartment(string id)
    {
        try
        {
            await Remove(id);
            return new RespondApi<object>()
            {
                Data = null,
                Message = "Success",
                Result = ResultRespond.Success
            };
        }
        catch (Exception e)
        {
            return new RespondApi<object>
            {
                Data = null,
                Message = e.Message,
                Result = ResultRespond.Error
            };
        }
    }
}