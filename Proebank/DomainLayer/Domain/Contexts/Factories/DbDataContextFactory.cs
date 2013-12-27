﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contexts.Factories
{
    public class DbDataContextFactory : Disposable, IDataContextFactory
    {
        private DataContext _database;
        public DataContext Get()
        {
            return _database ?? (_database = new DbDataContext());
        }
        protected override void DisposeCore()
        {
            if (_database != null)
                _database.Dispose();
        }
    }
}
