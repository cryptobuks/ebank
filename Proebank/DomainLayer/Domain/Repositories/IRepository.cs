using System;
using System.Linq;

namespace Domain.Repositories
{
    // TODO: T : IEntity
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> Where(Func<T, bool> predicate);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
