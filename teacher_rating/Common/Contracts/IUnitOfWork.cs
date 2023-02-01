namespace teacher_rating.Common.Contracts;

public interface IUnitOfWork: IDisposable
{
    Task CompleteAsync();
}