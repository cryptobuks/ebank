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
    public interface IUnitOfWork : IDisposable
    {
        IdentityDbContext<IdentityUser> Context { get; }
        DbSet<T> GetRepository<T>() where T : Entity;
        void SaveChanges();
    }
}
