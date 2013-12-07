using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models;

namespace Domain.Repositories.Db
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly DataContext _ctx;
        public bool IsDisposed { get; private set; }

        public Repository(DataContext context)
        {
            _ctx = context;
        }

        public IQueryable<T> GetAll(bool showRemoved = false)
        {
            return _ctx.Set<T>().Where(e => showRemoved || !e.IsRemoved);
        }

        public IQueryable<T> Where(Func<T, bool> predicate)
        {
            return _ctx.Set<T>().Where(e => !e.IsRemoved && predicate(e));
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
}
