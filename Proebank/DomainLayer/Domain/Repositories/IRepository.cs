using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        //T Create();
        //T Find(Guid? id);
        //IQueryable<T> GetAll(bool showRemoved = false);
        //void AddOrUpdate(T entity);
        //void Remove(T entity);
        //void SaveChanges();

        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(long Id);
        IEnumerable<T> All();
        IEnumerable<T> AllReadOnly();
    }
}
