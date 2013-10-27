using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer
{
    class DataContext : DbContext
    {
        public DbSet<IAccount> Accounts { get; set; }

        public DbSet<ILoan> Loans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Account>()
            //    .Property(u => u.Number)
            //    .HasColumnName("Number");
        }
    }
}
