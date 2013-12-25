using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Domain.Models;

namespace Domain.Repositories.Db
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly IUnitOfWork _uow;

        public bool IsDisposed { get; private set; }

        public Repository(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public T Create()
        {
            return _uow.Context.Set<T>().Create();
        }

        public T Find(Guid? id)
        {
            return _uow.Context.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll(bool showRemoved = false)
        {
            return _uow.Context.Set<T>().Where(e => !e.IsRemoved || showRemoved);
        }

        public void AddOrUpdate(T entity)
        {
            var set = _uow.Context.Set<T>();
            set.AddOrUpdate(entity);
        }

        public void Remove(T entity)
        {
            //_uow.Context.Set<T>().Remove(entity);
            entity.IsRemoved = true;
            AddOrUpdate(entity);
        }

        public void SaveChanges()
        {
            _uow.Context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _uow.Context.Dispose();
                }
                IsDisposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }
    }
}
