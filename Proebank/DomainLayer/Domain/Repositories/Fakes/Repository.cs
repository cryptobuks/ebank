using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Domain.Models;

namespace Domain.Repositories.Fakes
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private ObservableCollection<T> _collection;
        private readonly object _disposingLock;
        public bool IsDisposed { get; private set; }

        public Repository()
        {
            _disposingLock = new object();
            _collection = new ObservableCollection<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        public IQueryable<T> Where(Func<T, bool> predicate)
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
            lock (_disposingLock)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("data context");
                }
                _collection.Clear();
                _collection = null;
                IsDisposed = true;
            }
        }
    }
}
