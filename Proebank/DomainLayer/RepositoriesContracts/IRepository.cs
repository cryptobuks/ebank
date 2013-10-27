using System;
using System.Collections.Generic;

namespace RepositoriesContracts
{
    public interface IRepository<T, in TId>
    {
        T Get(TId id);

        IList<T> GetAll();

        IList<T> FindAll(Func<T, bool> filter);

        T FindOne(Func<T, bool> filter);

        T SaveOrUpdate(T entity);

        void Delete(T entity);
    }
}
