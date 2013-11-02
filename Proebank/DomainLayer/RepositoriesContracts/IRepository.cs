using System;
using System.Collections.Generic;

namespace RepositoriesContracts
{
    public interface IRepository<T, in TId>
    {
        T Get(TId id);

        IList<T> GetAll();

        IList<T> FindAll(Func<T, bool> filter);

        T FindFirst(Func<T, bool> filter);

        void SaveOrUpdate(params T[] entities);

        T Delete(T entity);
    }
}
