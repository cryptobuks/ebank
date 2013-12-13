using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IRepository<T> : IDisposable where T : IEntity
    {
        IQueryable<T> GetAll(bool showRemoved = false);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void SaveChanges();
        bool IsDisposed { get; }
    }
}
