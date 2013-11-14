using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public interface IRepository<T, in TId>
    {
        T Get(Func<T, bool> filter);

        IQueryable<T> GetAll();

        IEnumerable<T> GetAll(Func<T, bool> filter);

        void SaveOrUpdate(params T[] entities);

        T Delete(T entity);
    }
}
