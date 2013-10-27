using InfrastructureLayer.Entities;
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
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Account>()
            //    .Property(u => u.Number)
            //    .HasColumnName("Number");
        }
    }
}
