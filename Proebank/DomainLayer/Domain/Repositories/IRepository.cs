using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IRepository<T> : IDisposable where T : IEntity
    {
        IEnumerable<T> GetAll(bool showRemoved = false);
        IEnumerable<T> Where(Func<T, bool> predicate);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void SaveChanges();

        bool IsDisposed { get; }
    }
}
