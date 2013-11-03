using Domain.Models;
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

        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DataContextInitializer());
            //modelBuilder.Entity<Account>()
            //    .Property(u => u.Number)
            //    .HasColumnName("Number");
        }
    }
}
