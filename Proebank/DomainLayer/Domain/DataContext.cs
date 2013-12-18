using System;
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

        public DbSet<Employee> Employees { get; set; }

        public DbSet<LoanHistory> History { get; set; }

        public DbSet<PersonalData> PersonalData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<Employee>()
                .ToTable("Employees");
            modelBuilder.Entity<Customer>()
                .ToTable("Customers");
            modelBuilder.Entity<Account>()
                .Property(f => f.DateOpened)
                .HasColumnType("datetime2");
            modelBuilder.Entity<Account>()
                .Property(f => f.DateClosed)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<Entry>()
                .Property(f => f.Date)
                .HasColumnType("datetime2");
            modelBuilder.Entity<Calendar>()
                .Property(f => f.CurrentTime)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<Calendar>()
                .Property(f => f.LastDailyProcessingTime)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<Calendar>()
                .Property(f => f.LastMonthlyProcessingTime)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<PersonalData>()
                .Property(f => f.DateOfBirth)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<Employee>()
                .Property(f => f.FiredOn)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<Employee>()
                .Property(f => f.HiredOn)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<LoanApplication>()
                .Property(f => f.TimeContracted)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<LoanApplication>()
                .Property(f => f.TimeCreated)
                .HasColumnType("datetime2");
            modelBuilder.Entity<LoanHistory>()
                .Property(f => f.WhenClosed)
                .HasColumnType("datetime2").IsOptional();
            modelBuilder.Entity<LoanHistory>()
                .Property(f => f.WhenOpened)
                .HasColumnType("datetime2");
            modelBuilder.Entity<Payment>()
                .Property(f => f.ShouldBePaidBefore)
                .HasColumnType("datetime2");
            modelBuilder.Entity<Tariff>()
                .Property(f => f.CreationDate)
                .HasColumnType("datetime2");
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
