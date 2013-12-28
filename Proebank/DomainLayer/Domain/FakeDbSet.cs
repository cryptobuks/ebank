using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class FakeDbSet<T> : IDbSet<T> where T : class
    {
        private readonly ObservableCollection<T> _local = new ObservableCollection<T>();
        private readonly PropertyInfo[] _properties = typeof(T).GetProperties();

        public T Add(T entity)
        {
            _local.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            _local.Add(entity);
            return entity;
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            var obj = (TDerivedEntity)Activator.CreateInstance(typeof(TDerivedEntity));
            _local.Add(obj);
            return obj;
        }

        public T Create()
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            _local.Add(obj);
            return obj;
        }

        public T Find(params object[] keyValues)
        {
            // TODO: for the moment supports Guid ids only
            var targetId = keyValues[0].ToString();
            var prop = _properties.First(pi => pi.Name == "Id");
            return _local.FirstOrDefault(item => prop.GetValue(item).ToString().Equals(targetId));
        }

        public ObservableCollection<T> Local
        {
            get { return _local; }
        }

        public T Remove(T entity)
        {
            if (_local.Contains(entity))
            {
                _local.Remove(entity);
                return entity;
            }
            throw new InvalidOperationException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _local.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _local.GetEnumerator();
        }

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryProvider Provider
        {
            get { throw new NotImplementedException(); }
        }
    }
}
