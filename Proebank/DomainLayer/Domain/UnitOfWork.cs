using System.Data.Entity;
using Domain.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _ctx;

        public IdentityDbContext<IdentityUser> Context { get { return _ctx; } }

        public UnitOfWork(DataContext context)
        {
            _ctx = context;
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _ctx.SaveChangesAsync();
        }

        public DbSet<T> GetRepository<T>() where T : Entity
        {
            return _ctx.Set<T>();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
