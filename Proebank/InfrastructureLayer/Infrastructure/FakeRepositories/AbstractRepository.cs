using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain;

namespace Infrastructure.FakeRepositories
{
    public abstract class AbstractRepository<T, TId> : IRepository<T, TId> where T : class
    {
        private readonly ObservableCollection<T> _entities;
        private object _isDisposedIfNull;

        protected AbstractRepository(object disposedIndicator)
        {
            _isDisposedIfNull = disposedIndicator;
            _entities = new ObservableCollection<T>();
        }

        public T Get(Func<T, bool> filter)
        {
            if (_isDisposedIfNull != null)
            {
                return _entities.FirstOrDefault(filter);
            }
            else
            {
                throw new ObjectDisposedException("Disposed context is inaccessible");
            }
        }

        public IList<T> GetAll()
        {
            if (_isDisposedIfNull != null)
            {
                return _entities;
            }
            else
            {
                throw new ObjectDisposedException("Disposed context is inaccessible");
            }
        }

        public IList<T> GetAll(Func<T, bool> filter)
        {
            if (_isDisposedIfNull != null)
            {
                return _entities.Where(filter).ToList();
            }
            else
            {
                throw new ObjectDisposedException("Disposed context is inaccessible");
            }
        }

        public void SaveOrUpdate(params T[] entities)
        {
            if (_isDisposedIfNull != null)
            {
                foreach (var entity in entities)
                {
                    var loopVar = entity;
                    var loanApp = Get(a => a.Equals(loopVar));
                    if (loanApp == null)
                    {
                        _entities.Add(entity);
                    }
                    loanApp = entity;
                }
            }
            else
            {
                throw new ObjectDisposedException("Disposed context is inaccessible");
            }
        }

        public T Delete(T entity)
        {
            if (_isDisposedIfNull != null)
            {
                var removalSucceeded = _entities.Remove(entity);
                return removalSucceeded ? entity : null;
            }
            else
            {
                throw new ObjectDisposedException("Disposed context is inaccessible");
            }
        }
    }
}
