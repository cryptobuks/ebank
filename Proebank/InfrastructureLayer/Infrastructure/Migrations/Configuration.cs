using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Infrastructure.Migrations
{
    using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Infrastructure.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Infrastructure.DataContext context)
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
            var bankCreationDate = new DateTime(2013, 9, 2);
            var entry = new Entry 
            { 
                Amount = 1.0E11M,
                Date = bankCreationDate,
                Type = EntryType.Payment,
                SubType = EntrySubType.CharterCapital
            };
            context.Accounts.AddOrUpdate(
                new Account
                {
                    Currency = Currency.BYR,
                    DateOpened = new DateTime(2013, 9, 2),
                    DateClosed = null,
                    Number = (int)Currency.BYR,
                    Type = AccountType.BankBalance,
                    Entries = new List<Entry> { entry },
                    IsClosed = false,
                });

            var tariff = new Tariff()
            {
                CreationDate = bankCreationDate, EndDate = null,
                InitialFee = 0, InterestRate = 0.75M, IsGuarantorNeeded = false, IsSecondaryDocumentNeeded = false,
                LoanPurpose = LoanPurpose.Common,
                MaxAmount = 1.0E8M,
                MinAge = 18,
                MaxTerm = 36,
                MinTerm = 3,
                MinAmount = 1.0E6M,
                Name = "NeverSeeMeAgain"
            };
            context.Tariffs.AddOrUpdate(tariff);

            context.SaveChanges();
        }
    }
}
