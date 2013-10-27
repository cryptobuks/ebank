using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.RepositoriesContracts
{
    public interface IRepository<T, IdT>
    {
        T Get(IdT id);

        IList<T> GetAll();

        IList<T> FindAll(IDictionary<string, object> propertyValuePairs);

        T FindOne(IDictionary<string, object> propertyValuePairs);

        T SaveOrUpdate(T entity);

        void Delete(T entity);
    }
}
