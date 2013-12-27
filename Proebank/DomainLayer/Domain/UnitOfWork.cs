using System.Data.Entity;
using Domain.Contexts;
using Domain.Contexts.Factories;
using Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDataContextFactory _dbFactory;
        private DataContext _ctx;

        public DataContext Context
        {
            get { return _ctx ?? (_ctx = _dbFactory.Get()); }
            set { _ctx = value; }
        }

        public UnitOfWork(IDataContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public IDbSet<T> GetDbSet<T>() where T : Entity
        {
            return Context.Set<T>();
        }
    }
}
