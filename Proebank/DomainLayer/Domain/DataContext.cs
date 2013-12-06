using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext()
            : base("Proebank")
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

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbExc)
            {
                foreach (var validationError in dbExc.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
                throw;
            }
        }
    }
}
