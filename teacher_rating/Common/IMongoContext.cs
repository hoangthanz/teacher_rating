using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CKLS.Services.Models.MongoDb.Common
{
    public interface IMongoContext : IDisposable
    {
        void AddCommand(Func<Task> func);
        Task<int> SaveChanges();
        IMongoCollection<T> GetCollection<T>(string name);
        void ChangeDatabase<T>(string connection, string databaseName);
    }
}