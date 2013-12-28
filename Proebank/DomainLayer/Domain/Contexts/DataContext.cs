using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Contexts
{
    public abstract class DataContext : IdentityDbContext<IdentityUser>
    {
        private readonly Guid _id;

        public DataContext()
        {
            _id = Guid.NewGuid();
            Debug.WriteLine("Data context created: {0}", _id);
        }

        public DataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            _id = Guid.NewGuid();
            Debug.WriteLine("Data context created: {0}", _id);
        }

        public virtual IDbSet<Account> Accounts { get; set; }

        public virtual IDbSet<LoanApplication> LoanApplications { get; set; }

        public virtual IDbSet<Loan> Loans { get; set; }

        public virtual IDbSet<Tariff> Tariffs { get; set; }

        public virtual IDbSet<Calendar> Calendars { get; set; }

        public virtual IDbSet<Employee> Employees { get; set; }

        public virtual IDbSet<LoanHistory> History { get; set; }

        public virtual IDbSet<PersonalData> PersonalData { get; set; }

        public virtual IDbSet<IdentityUserLogin> IdentityUserLogins { get; set; }

        public virtual IDbSet<IdentityUserRole> IdentityUserRoles { get; set; }

        public override string ToString()
        {
            return _id.ToString();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityUserLogin>().HasKey(ul => new { ul.UserId, ul.ProviderKey, ul.LoginProvider });
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Customer>().ToTable("Customers");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            //modelBuilder.Entity<IdentityRole>().ToTable("Roles");

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Debug.WriteLine("Data context disposed: {1}", disposing, _id);
        }
    }
}
