using Domain.Models.Accounts;

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
                Id = Guid.Parse("57A78920-B1AA-47DE-8B9A-C9EACAEC38E3"),
                Amount = 1.0E11M,
                Date = bankCreationDate,
                Type = EntryType.Payment,
                SubType = EntrySubType.CharterCapital
            };
            context.Accounts.AddOrUpdate(
                new Account
                {
                    Id = Guid.Parse("34D8EE16-DDA9-48D7-9FCF-AA65910DF77A"),
                    Currency = Currency.BYR,
                    DateOpened = new DateTime(2013, 9, 2),
                    DateClosed = null,
                    Number = (int)Currency.BYR,
                    Type = AccountType.BankBalance,
                    Entries = new List<Entry> { entry },
                    IsClosed = false,
                });
            context.SaveChanges();
        }
    }
}
