using Domain.Models;
using Domain.Models.Accounts;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Infrastructure.Migrations;
using System.Data.Entity;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext() : base("Proebank")
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<LoanApplication> LoanApplications { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DataContextInitializer());
            //modelBuilder.Entity<User>()
            //    .Map(m => m.ToTable("Users"))
            //    .Map<Customer>(m => m.ToTable("Customers"))
            //    .Map<Employee>(m => m.ToTable("Employees"));
            //modelBuilder.Entity<Account>()
            //    .Property(u => u.Number)
            //    .HasColumnName("Number");
        }
    }
}
