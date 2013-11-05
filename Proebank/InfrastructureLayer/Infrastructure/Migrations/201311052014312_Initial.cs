namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        Currency = c.Int(nullable: false),
                        DateOpened = c.DateTime(nullable: false),
                        DateClosed = c.DateTime(),
                        IsClosed = c.Boolean(nullable: false),
                        Loan_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loans", t => t.Loan_Id)
                .Index(t => t.Loan_Id);
            
            CreateTable(
                "dbo.Entries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        SubType = c.Int(nullable: false),
                        Account_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.LoanApplications",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeCreated = c.DateTime(nullable: false),
                        TimeContracted = c.DateTime(),
                        Term = c.Int(nullable: false),
                        CellPhone = c.String(),
                        TariffId = c.Guid(nullable: false),
                        LoanPurpose = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tariffs", t => t.TariffId, cascadeDelete: true)
                .Index(t => t.TariffId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.String(),
                        CustomerId = c.String(maxLength: 128),
                        DocType = c.Int(nullable: false),
                        TariffDocType = c.Int(nullable: false),
                        LoanApplication_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.LoanApplications", t => t.LoanApplication_Id)
                .Index(t => t.CustomerId)
                .Index(t => t.LoanApplication_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tariffs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        MinTerm = c.Int(nullable: false),
                        MaxTerm = c.Int(nullable: false),
                        InitialFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsGuarantorNeeded = c.Boolean(nullable: false),
                        IsSecondaryDocumentNeeded = c.Boolean(nullable: false),
                        LoanPurpose = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Application_Id = c.Guid(),
                        PaymentSchedule_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoanApplications", t => t.Application_Id)
                .ForeignKey("dbo.PaymentSchedules", t => t.PaymentSchedule_Id)
                .Index(t => t.Application_Id)
                .Index(t => t.PaymentSchedule_Id);
            
            CreateTable(
                "dbo.PaymentSchedules",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ShouldBePaidBefore = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentSchedule_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentSchedules", t => t.PaymentSchedule_Id)
                .Index(t => t.PaymentSchedule_Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        HiredOn = c.DateTime(nullable: false),
                        FiredOn = c.DateTime(nullable: false),
                        EmployeeRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        IdentificationNumber = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "Id", "dbo.Users");
            DropForeignKey("dbo.Employees", "Id", "dbo.Users");
            DropForeignKey("dbo.Loans", "PaymentSchedule_Id", "dbo.PaymentSchedules");
            DropForeignKey("dbo.Payments", "PaymentSchedule_Id", "dbo.PaymentSchedules");
            DropForeignKey("dbo.Loans", "Application_Id", "dbo.LoanApplications");
            DropForeignKey("dbo.Accounts", "Loan_Id", "dbo.Loans");
            DropForeignKey("dbo.LoanApplications", "TariffId", "dbo.Tariffs");
            DropForeignKey("dbo.Documents", "LoanApplication_Id", "dbo.LoanApplications");
            DropForeignKey("dbo.Documents", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.Entries", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.Customers", new[] { "Id" });
            DropIndex("dbo.Employees", new[] { "Id" });
            DropIndex("dbo.Loans", new[] { "PaymentSchedule_Id" });
            DropIndex("dbo.Payments", new[] { "PaymentSchedule_Id" });
            DropIndex("dbo.Loans", new[] { "Application_Id" });
            DropIndex("dbo.Accounts", new[] { "Loan_Id" });
            DropIndex("dbo.LoanApplications", new[] { "TariffId" });
            DropIndex("dbo.Documents", new[] { "LoanApplication_Id" });
            DropIndex("dbo.Documents", new[] { "CustomerId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Entries", new[] { "Account_Id" });
            DropTable("dbo.Customers");
            DropTable("dbo.Employees");
            DropTable("dbo.Payments");
            DropTable("dbo.PaymentSchedules");
            DropTable("dbo.Loans");
            DropTable("dbo.Tariffs");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.Documents");
            DropTable("dbo.LoanApplications");
            DropTable("dbo.Entries");
            DropTable("dbo.Accounts");
        }
    }
}
