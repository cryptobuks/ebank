using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Enums;
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
        private readonly Guid _customerUserId;
        private readonly Guid _consultantUserId;
        private readonly Guid _operatorUserId;
        private readonly Guid _securityUserId;
        private readonly Guid _committeeUserId;
        private readonly Guid _headUserId;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            _customerUserId = Guid.Parse("59A9C686-2CA7-4C2A-B397-FCA49554F8AA");
            _consultantUserId = Guid.Parse("4819B14F-3098-455C-B8AD-D2FFD53FCAC2");
            _operatorUserId = Guid.Parse("59B964BD-E155-485C-AB01-7C23F1C72534");
            _securityUserId = Guid.Parse("CAF1154E-37ED-45E7-80BE-FC490AEB53A8");
            _committeeUserId = Guid.Parse("894BBEDC-F68A-4683-9003-63F72F9652A5");
            _headUserId = Guid.Parse("219D15DF-1849-4C98-9B3D-8B709572036F");
        }

        protected override void Seed(DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            base.Seed(context);

            #region seed user roles
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

            var userNames = new[] { "testcustomer", "testconsultant", "testoperator", "testsecurity", "testcommittee", "testhead" };
            var users = userNames.Zip<string, string, IdentityUser>(roleNames, (userName, roleName) =>
            {
                switch (roleName)
                {
                    case "Customer":
                        return new Customer
                        {
                            Address = "test address",
                            BirthDate = new DateTime(1990, 06, 30),
                            Email = "test_customer@mail.by",
                            FirstName = "Karl",
                            Id = _customerUserId.ToString(),
                            IdentificationNumber = "AA1234567",
                            LastName = "Malone",
                            MiddleName = "Anthony",
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
                var existingUser = userManager.FindById(user.Id);
                if (existingUser == null)
                {
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

            #region seed tariffs
            var tariff0 = new Tariff
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
            var tariff1 = new Tariff
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
            context.Tariffs.AddOrUpdate(t => t.Id, tariff0);
            context.Tariffs.AddOrUpdate(t => t.Id, tariff1); 
            #endregion

            #region seed calendar
            var calendarEntry = new Calendar
                {
                    Id = Calendar.ConstGuid,
                    CurrentTime = new DateTime(2013, 11, 1, 15, 0, 0),
                    ProcessingLock = false
                };
            context.Calendars.AddOrUpdate(c => c.Id, calendarEntry); 
            #endregion

            context.SaveChanges();
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
