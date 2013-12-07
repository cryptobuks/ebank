using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    public class RepositoryContainer
    {
        private readonly List<object> _repositories = new List<object>();
        public void Add<T>(IRepository<T> repository) where T : IEntity
        {
            _repositories.Add(repository);
        }

        public IRepository<T> Get<T>() where T : IEntity
        {
            return _repositories.SingleOrDefault(r => r.GetType().GetGenericArguments()[0] == typeof (T)) as IRepository<T>;
        }

        public void Remove(object repository)
        {
            _repositories.Remove(repository);
        }

        /// <summary>
        /// Normally it should be used for enumerating all repositories.
        /// When you need only one of them, you should use Get() method
        /// </summary>
        /// <returns>All repositories in container</returns>
        public List<object> GetAll()
        {
            return _repositories;
        }
    }
}
