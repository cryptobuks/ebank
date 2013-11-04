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

        protected AbstractRepository()
        {
            _entities = new ObservableCollection<T>();
        }

        public T Get(Func<T, bool> filter)
        {
            return _entities.FirstOrDefault(filter);
        }

        public IList<T> GetAll()
        {
            return _entities;
        }

        public IList<T> GetAll(Func<T, bool> filter)
        {
            return _entities.Where(filter).ToList();
        }

        public void SaveOrUpdate(params T[] entities)
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

        public T Delete(T entity)
        {
            var removalSucceeded = _entities.Remove(entity);
            return removalSucceeded ? entity : null;
        }
    }
}
