using System;
using System.Collections.Generic;

namespace Domain
{
    public interface IRepository<T, in TId>
    {
        T Get(TId id);

        T Get(Func<T, bool> filter);

        IList<T> GetAll();

        IList<T> GetAll(Func<T, bool> filter);

        void SaveOrUpdate(params T[] entities);

        T Delete(T entity);
    }
}
