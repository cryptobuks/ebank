using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories.Fakes
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ObservableCollection<T> _collection;

        public Repository()
        {
            _collection = new ObservableCollection<T>();
        }

        public IQueryable<T> GetTable()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<T> Filter(Func<T, bool> predicate)
        {
            return _collection.Where(predicate).AsQueryable();
        }

        public void AddOrUpdate(T entity)
        {
            var oldEntity = _collection.SingleOrDefault(e => e.Equals(entity));
            if (oldEntity == null)
            {
                _collection.Add(entity);
            }
            else
            {
                oldEntity = entity;
            }
        }

        public void Remove(T entity)
        {
            _collection.Remove(entity);
        }

        public void SaveChanges()
        {
            Trace.WriteLine("Changes saved");
        }

        public void Dispose()
        {
            // fake disposing
            _collection.Clear();
            _collection = null;
        }
    }
}
