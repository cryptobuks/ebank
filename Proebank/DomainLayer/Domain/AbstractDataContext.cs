using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    abstract public class AbstractDataContext : IdentityDbContext<Employee>
    {
        public AbstractDataContext(string dbName)
            : base(dbName)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<LoanApplication> LoanApplications { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<Tariff> Tariffs { get; set; }

        public DbSet<Calendar> Calendars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<Employee>()
                .ToTable("Employees");
            modelBuilder.Entity<Customer>()
                .ToTable("Customers");
        }
    }
}
