using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contexts;
using Domain.Contexts.Factories;

namespace Domain.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        private DataContext _ctx;
        private readonly IDbSet<T> _dbset;
        protected RepositoryBase(IDataContextFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = Context.Set<T>();
        }

        protected IDataContextFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected DataContext Context
        {
            get { return _ctx ?? (_ctx = DatabaseFactory.Get()); }
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual void Update(T entity)
        {
            _ctx.Entry(entity).State = EntityState.Modified;
        }
        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public virtual IEnumerable<T> All()
        {
            return _dbset.ToList();
        }
        public virtual IEnumerable<T> AllReadOnly()
        {
            return _dbset.AsNoTracking().ToList();
        }
    }
}
