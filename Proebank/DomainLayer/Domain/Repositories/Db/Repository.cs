using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Domain.Repositories.Db
{
    // TODO:  where T : IEntity
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _ctx;

        public Repository(DataContext context)
        {
            _ctx = context;
        }

        public IQueryable<T> GetAll()
        {
            return _ctx.Set<T>();
        }

        public IQueryable<T> Where(Func<T, bool> predicate)
        {
            return _ctx.Set<T>().Where(predicate).AsQueryable();
        }

        public void AddOrUpdate(T entity)
        {
            var set = _ctx.Set<T>();
            set.AddOrUpdate(entity);
        }

        public void Remove(T entity)
        {
            _ctx.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
