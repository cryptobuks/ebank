using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    public interface IRepository<T> : IDisposable where T : IEntity
    {
        T Create();
        T Find(Guid? id);
        IQueryable<T> GetAll(bool showRemoved = false);
        void AddOrUpdate(T entity);
        void Remove(T entity);
        void SaveChanges();
        bool IsDisposed { get; }
    }
}
