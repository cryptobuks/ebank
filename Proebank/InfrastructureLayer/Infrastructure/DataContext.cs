using Domain.Models;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Infrastructure
{
    public class DataContext : IdentityDbContext<Employee>
    {
        public DataContext() : base("Proebank")
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
