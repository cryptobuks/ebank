using System.Collections.Generic;
using Domain.Enums;
using Domain.Models.Loans;
using Domain.Models.Users;

namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.DataContext>
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
            var rand = new Random();
            var tariffs = new List<Tariff>();
            for (var i = 0; i < 3; i++)
            {
                var tariff = new Tariff()
                {
                    Name = "tariff_" + i.ToString(),
                    CreationDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    InitialFee = 0,
                    InterestRate = rand.Next(4, 64),
                    IsGuarantorNeeded = false,
                    IsSecondaryDocumentNeeded = false,
                    MaxAmount = rand.Next(100, 1000000),
                    LoanPurpose = LoanPurpose.Common,
                    MinAge = 18,
                    MaxAge = 60,
                    MinAmount = rand.Next(1, 100),
                    MinTerm = rand.Next(3, 12),
                    MaxTerm = rand.Next(12, 36)
                };
                context.Tariffs.AddOrUpdate(tariff);
                tariffs.Add(tariff);
            }

            for (var i = 0; i < 10; i++)
            {
                context.LoanApplications.AddOrUpdate(
                    new LoanApplication()
                    {
                        CellPhone = rand.Next(1111111, 9999999).ToString(),
                        Currency = Currency.BYR,
                        Documents = null,
                        LoanAmount = rand.Next(100, 10000),
                        LoanPurpose = LoanPurpose.Common,
                        Tariff = tariffs[i % tariffs.Count],
                        Term = rand.Next(3, 36),
                        TimeCreated = DateTime.Now,
                        TimeContracted = DateTime.Now
                    });
            }

        }
    }
}
