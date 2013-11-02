using DomainLayer.Models;
using System.Data.Entity;

namespace InfrastructureLayer
{
    class DataContext : DbContext
    {
        public DataContext() : base("Proebank")
        {
        }

        public DbSet<AccountModel> Accounts { get; set; }

        public DbSet<LoanModel> Loans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Account>()
            //    .Property(u => u.Number)
            //    .HasColumnName("Number");
        }
    }
}
