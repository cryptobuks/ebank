using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contexts.Factories
{
    public class FakeDataContextFactory : Disposable, IDataContextFactory
    {
        private DataContext _database;
        public DataContext Get()
        {
            return _database ?? (_database = new FakeDataContext());
        }
        protected override void DisposeCore()
        {
            if (_database != null)
                _database.Dispose();
        }
    }
}
