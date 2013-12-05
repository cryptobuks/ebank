using Domain.Enums;
using Domain.Models.Calendars;
using Domain.Models.Loans;
using System;
using System.Data.Entity.Migrations;

namespace Domain.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var tariff0 = new Tariff()
            {
                Id = Guid.Parse("DEF8A3B2-8439-4714-8084-CA30364D1E92"),
                Name = "Common Tariff",
                CreationDate = new DateTime(2013, 9, 1),
                EndDate = null,
                InitialFee = 0M,
                InterestRate = 0.5M,
                IsGuarantorNeeded = false,            
                MinAmount = 10000,
                MaxAmount = 100000000,
                LoanPurpose = LoanPurpose.Common,
                MinAge = 18,
                MaxAge = 60,
                MinTerm = 1,
                MaxTerm = 24
            };
            var tariff1 = new Tariff()
            {
                Id = Guid.Parse("52A139D6-E673-4F72-B5D6-10D1F33FB878"),
                Name = "Car Tariff",
                CreationDate = new DateTime(2013, 9, 1),
                EndDate = null,
                InitialFee = 0M,
                InterestRate = 0.4M,
                IsGuarantorNeeded = false,           
                MinAmount = 1000000,
                MaxAmount = 100000000,
                LoanPurpose = LoanPurpose.Car,
                MinAge = 18,
                MaxAge = 60,
                MinTerm = 6,
                MaxTerm = 36
            };
            context.Tariffs.AddOrUpdate(tariff0);
            context.Tariffs.AddOrUpdate(tariff1);

            var calendarEntry = new Calendar
            {
                Id = Calendar.ConstGuid,
                CurrentTime = new DateTime(2013, 11, 1, 15, 0, 0),
                ProcessingLock = false
            };
            context.Calendars.AddOrUpdate(calendarEntry);

            context.SaveChanges();
        }
    }
}
