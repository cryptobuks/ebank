using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    // TODO: T : IEntity
    public interface IRepository<T> : IDisposable where T : class
    {
        // TODO: rename to GetAll?
        IQueryable<T> GetTable();
        // TODO: rename to Where
        IQueryable<T> Filter(Func<T, bool> predicate);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
