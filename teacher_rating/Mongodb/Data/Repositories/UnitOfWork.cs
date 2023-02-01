using CKLS.Services.Models.MongoDb.Common;
using teacher_rating.Common.Contracts;

namespace teacher_rating.Mongodb.Data.Repositories;

public class UnitOfWork: IUnitOfWork
{
    private readonly IMongoContext _context;

    public UnitOfWork(IMongoContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChanges();
    }
}