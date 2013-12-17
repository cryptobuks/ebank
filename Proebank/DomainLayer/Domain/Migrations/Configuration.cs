using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using System;
using System.Data.Entity.Migrations;
using Domain.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        private readonly Guid _customerUserId = Guid.Parse("59A9C686-2CA7-4C2A-B397-FCA49554F8AA");
        private readonly Guid _consultantUserId = Guid.Parse("4819B14F-3098-455C-B8AD-D2FFD53FCAC2");
        private readonly Guid _operatorUserId = Guid.Parse("59B964BD-E155-485C-AB01-7C23F1C72534");
        private readonly Guid _securityUserId = Guid.Parse("CAF1154E-37ED-45E7-80BE-FC490AEB53A8");
        private readonly Guid _committeeUserId = Guid.Parse("894BBEDC-F68A-4683-9003-63F72F9652A5");
        private readonly Guid _customerUserDataId = Guid.Parse("B702E32A-9E41-4E17-80DC-ED87DC4B3BE8");
        private readonly Guid _headUserId = Guid.Parse("219D15DF-1849-4C98-9B3D-8B709572036F");
        private readonly Guid _bankAccByr = Guid.Parse("9B913259-EA90-4D2D-9DF8-2E6780D27F90");
        private readonly Guid _bankAccEur = Guid.Parse("A15D1E0C-CC36-4538-B305-6256C8550EBC");
        private readonly Guid _bankAccUsd = Guid.Parse("F9DB5805-E738-4236-B0C8-A24D839BF60F");

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            base.Seed(context);
            try
            {
                var bankCreationDate = new DateTime(2013, 8, 15);

                #region seed calendar

                if (!context.Calendars.Any(c => c.Id == Calendar.ConstGuid))
                {
                    var calendarEntry = new Calendar
                    {
                        Id = Calendar.ConstGuid,
                        CurrentTime = bankCreationDate,
                        ProcessingLock = false
                    };
                    context.Calendars.AddOrUpdate(c => c.Id, calendarEntry);
                }

                #endregion

                #region seed users and users' roles

                var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var roleNames = new[]
                {
                    "Customer",
                    "Consultant",
                    "Operator",
                    "Security",
                    "Credit committee",
                    "Department head"
                };

                foreach (var roleName in roleNames)
                {
                    if (roleManager.RoleExists(roleName)) continue;
                    var roleResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception(String.Concat("Failed to create role: ", roleName, "\n",
                            String.Join("\n", roleResult.Errors)));
                    }
                }

                var userNames = new[]
                {"testcustomer", "testconsultant", "testoperator", "testsecurity", "testcommittee", "testhead"};
                var users = userNames.Zip<string, string, IdentityUser>(roleNames, (userName, roleName) =>
                {
                    switch (roleName)
                    {
                        case "Customer":
                            return new Customer
                            {
                                Address = "test address",
                                PersonalData = new PersonalData()
                                {
                                    Id = _customerUserDataId,
                                    DateOfBirth = new DateTime(1990, 06, 30),
                                    FirstName = "Karl",
                                    Identification = "AA1234567890XX",
                                    Passport = "XX0101010",
                                    LastName = "Malone",
                                    MiddleName = "Anthony",
                                },
                                Email = "test_customer@mail.by",
                                Id = _customerUserId.ToString(),
                                Phone = "+375291234567",
                                UserName = userName
                            };
                        default:
                            return new Employee
                            {
                                EmployeeRole = GetEmployeeRoleFromUserRole(roleName),
                                FiredOn = null,
                                FirstName = roleName[0] + ".",
                                HiredOn = new DateTime(2013, 9, 2),
                                Id = GetEmployeeIdFromUserRole(roleName),
                                LastName = String.Concat(roleName.Reverse().Take(4)),
                                MiddleName = "A.",
                                UserName = userName
                            };
                    }
                });

                foreach (var user in users)
                {
                    var existingUser = userManager.FindByName(user.UserName);
                    if (existingUser == null || existingUser.Id != user.Id)
                    {
                        if (existingUser != null)
                        {
                            context.Users.Remove(existingUser);
                            context.SaveChanges();
                        }
                        var userResult = userManager.Create(user, "password");
                        if (!userResult.Succeeded)
                        {
                            throw new Exception(String.Concat("Failed to create user: ", user.UserName, "\n",
                                String.Join("\n", userResult.Errors)));
                        }
                    }
                    userManager.AddToRole(user.Id, GetUserRoleFromUserId(user.Id));
                }

                #endregion

                #region seed bank account

                var bankAccountByr = context.Accounts.SingleOrDefault(acc => acc.Id == _bankAccByr);
                if (bankAccountByr == null)
                {
                    bankAccountByr = new Account
                    {
                        Currency = Currency.BYR,
                        DateOpened = bankCreationDate,
                        Id = _bankAccByr,
                        Number = 1,
                        Entries = new List<Entry>(),
                        Type = AccountType.BankBalance
                    };

                    bankAccountByr.Entries.Add(new Entry
                    {
                        Amount = 1E14M,
                        Date = bankCreationDate,
                        Currency = Currency.BYR,
                        Type = EntryType.Capital,
                        SubType = EntrySubType.CharterCapital
                    });
                    context.Accounts.AddOrUpdate(bankAccountByr);
                }

                var bankAccountEur = context.Accounts.SingleOrDefault(acc => acc.Id == _bankAccByr);
                if (bankAccountEur == null)
                {
                    bankAccountEur = new Account
                    {
                        Currency = Currency.EUR,
                        DateOpened = bankCreationDate,
                        Id = _bankAccEur,
                        Number = 1,
                        Entries = new List<Entry>(),
                        Type = AccountType.BankBalance
                    };
                    bankAccountEur.Entries.Add(new Entry
                    {
                        Amount = 1E8M,
                        Date = bankCreationDate,
                        Currency = Currency.EUR,
                        Type = EntryType.Capital,
                        SubType = EntrySubType.CharterCapital
                    });
                    context.Accounts.AddOrUpdate(bankAccountEur);
                }

                var bankAccountUsd = context.Accounts.SingleOrDefault(acc => acc.Id == _bankAccUsd);
                if (bankAccountUsd == null)
                {
                    bankAccountUsd = new Account
                    {
                        Currency = Currency.USD,
                        DateOpened = bankCreationDate,
                        Id = _bankAccUsd,
                        Number = 1,
                        Entries = new List<Entry>(),
                        Type = AccountType.BankBalance
                    };
                    bankAccountUsd.Entries.Add(new Entry
                    {
                        Amount = 1E8M,
                        Date = bankCreationDate,
                        Currency = Currency.USD,
                        Type = EntryType.Capital,
                        SubType = EntrySubType.CharterCapital
                    });
                    context.Accounts.AddOrUpdate(bankAccountUsd);
                }

                #endregion

                #region seed tariffs

                var tariff0 = new Tariff
                {
                    Id = Guid.Parse("DEF8A3B2-8439-4714-8084-CA30364D1E92"),
                    Name = "Common Tariff",
                    CreationDate = new DateTime(2013, 8, 1),
                    Currency = Currency.BYR,
                    IsActive = true,
                    InitialFee = 0M,
                    InterestRate = 0.5M,
                    IsGuarantorNeeded = false,
                    MinAmount = 10000,
                    MaxAmount = 100000000,
                    LoanPurpose = LoanPurpose.Common,
                    MinAge = 18,
                    MaxAge = 60,
                    MinTerm = 1,
                    MaxTerm = 24,
                    PmtFrequency = 1,
                    PmtType = PaymentCalculationType.Annuity,
                };
                var tariff1 = new Tariff
                {
                    Id = Guid.Parse("52A139D6-E673-4F72-B5D6-10D1F33FB878"),
                    Name = "Car Tariff",
                    CreationDate = new DateTime(2013, 8, 1),
                    Currency = Currency.BYR,
                    IsActive = true,
                    InitialFee = 0M,
                    InterestRate = 0.4M,
                    IsGuarantorNeeded = false,
                    MinAmount = 1000000,
                    MaxAmount = 100000000,
                    LoanPurpose = LoanPurpose.Car,
                    MinAge = 18,
                    MaxAge = 60,
                    MinTerm = 6,
                    MaxTerm = 36,
                    PmtFrequency = 1,
                    PmtType = PaymentCalculationType.Standard,
                };

                var tariff2 = new Tariff
                {
                    Id = Guid.Parse("C590CE02-A11A-4702-A7E2-23AE53E3FDDD"),
                    Name = "Euro loan",
                    CreationDate = new DateTime(2013, 8, 1),
                    Currency = Currency.EUR,
                    IsActive = true,
                    InitialFee = 0M,
                    InterestRate = 0.05M,
                    IsGuarantorNeeded = false,
                    MinAmount = 500,
                    MaxAmount = 10000,
                    LoanPurpose = LoanPurpose.Education,
                    MinAge = 18,
                    MaxAge = 65,
                    MinTerm = 3,
                    MaxTerm = 48,
                    PmtFrequency = 1,
                    PmtType = PaymentCalculationType.Annuity,
                };

                var tariff3 = new Tariff
                {
                    Id = Guid.Parse("DF1093E5-8343-47E9-B194-504D40FC97B1"),
                    Name = "US Dollar loan",
                    CreationDate = new DateTime(2013, 8, 1),
                    Currency = Currency.USD,
                    IsActive = true,
                    InitialFee = 0M,
                    InterestRate = 0.08M,
                    IsGuarantorNeeded = false,
                    MinAmount = 500,
                    MaxAmount = 10000,
                    LoanPurpose = LoanPurpose.Common,
                    MinAge = 18,
                    MaxAge = 65,
                    MinTerm = 3,
                    MaxTerm = 48,
                    PmtFrequency = 1,
                    PmtType = PaymentCalculationType.Standard,
                };
                if (context.Tariffs.SingleOrDefault(t => t.Id == tariff0.Id) == null)
                    context.Tariffs.AddOrUpdate(t => t.Id, tariff0);
                if (context.Tariffs.SingleOrDefault(t => t.Id == tariff1.Id) == null)
                    context.Tariffs.AddOrUpdate(t => t.Id, tariff1);
                if (context.Tariffs.SingleOrDefault(t => t.Id == tariff2.Id) == null)
                    context.Tariffs.AddOrUpdate(t => t.Id, tariff2);
                if (context.Tariffs.SingleOrDefault(t => t.Id == tariff3.Id) == null)
                    context.Tariffs.AddOrUpdate(t => t.Id, tariff3);

                #endregion

                context.SaveChanges();

            }
            catch (DbEntityValidationException exc)
            {
                foreach (var dbEntityValidationResult in exc.EntityValidationErrors)
                {
                    foreach (var error in dbEntityValidationResult.ValidationErrors)
                    {
                        Debug.WriteLine("(Db error) {0}: {1}", error.PropertyName, error.ErrorMessage);
                    }
                }
                throw;
            }
        }

        private EmployeeRole GetEmployeeRoleFromUserRole(string roleName)
        {
            switch (roleName)
            {
                case "Consultant":
                    return EmployeeRole.Consultant;
                case "Operator":
                    return EmployeeRole.Operator;
                case "Security":
                    return EmployeeRole.SecurityService;
                case "Credit committee":
                    return EmployeeRole.CreditCommitee;
                case "Department head":
                    return EmployeeRole.Chief;
                default:
                    throw new ArgumentException();
            }
        }

        private string GetEmployeeIdFromUserRole(string roleName)
        {
            switch (roleName)
            {
                case "Consultant":
                    return _consultantUserId.ToString();
                case "Operator":
                    return _operatorUserId.ToString();
                case "Security":
                    return _securityUserId.ToString();
                case "Credit committee":
                    return _committeeUserId.ToString();
                case "Department head":
                    return _headUserId.ToString();
                default:
                    throw new ArgumentException();
            }
        }

        private string GetUserRoleFromUserId(string userId)
        {
            var guid = Guid.Parse(userId);
            if (guid.Equals(_customerUserId)) return "Customer";
            if (guid.Equals(_consultantUserId)) return "Consultant";
            if (guid.Equals(_operatorUserId)) return "Operator";
            if (guid.Equals(_securityUserId)) return "Security";
            if (guid.Equals(_committeeUserId)) return "Credit committee";
            if (guid.Equals(_headUserId)) return "Department head";
            throw new ArgumentException();
        }
    }
}
