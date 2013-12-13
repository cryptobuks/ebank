using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Domain.Models;

namespace Domain.Repositories.Fakes
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private ObservableCollection<T> _collection;
        public bool IsDisposed { get; private set; }

        public Repository()
        {
            _collection = new ObservableCollection<T>();
        }

        public IQueryable<T> GetAll(bool showRemoved = false)
        {
            return _collection.Where(e => !e.IsRemoved || showRemoved).AsQueryable();
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
            //_collection.Remove(entity);
            entity.IsRemoved = true;
            AddOrUpdate(entity);
        }

        public void SaveChanges()
        {
            Trace.WriteLine("Changes saved (fake repository)");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            // fake disposing
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _collection.Clear();
                    _collection = null;
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
