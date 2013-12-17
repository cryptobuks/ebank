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
        private readonly DataContext _ctx;

        public bool IsDisposed { get; private set; }

        public Repository()
        {
            _ctx = DataContextManager.GetContext();
        }

        public T Create()
        {
            return _ctx.Set<T>().Create();
        }

        public T Find(Guid? id)
        {
            return _ctx.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll(bool showRemoved = false)
        {
            return _ctx.Set<T>().Where(e => !e.IsRemoved || showRemoved);
        }

        public void AddOrUpdate(T entity)
        {
            var set = _ctx.Set<T>();
            set.AddOrUpdate(entity);
        }

        public void Remove(T entity)
        {
            //_ctx.Set<T>().Remove(entity);
            entity.IsRemoved = true;
            AddOrUpdate(entity);
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
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
                    _ctx.Dispose();
                }
                IsDisposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }
    }

    public static class DataContextManager
    {
        private static readonly Lazy<DataContext> Container = new Lazy<DataContext>(() => new DataContext());
        private static DataContext _context;

        internal static DataContext GetContext()
        {
            return _context ?? (_context = Container.Value);
        }
    }
}
