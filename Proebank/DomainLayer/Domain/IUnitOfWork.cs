using System.Data.Entity;
using System.Runtime.CompilerServices;
using Domain.Contexts;
using Domain.Models;
using Domain.Repositories;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IUnitOfWork
    {
        DataContext Context { get; set; }
        IDbSet<T> GetDbSet<T>() where T : Entity;
        void SaveChanges();
    }
}
