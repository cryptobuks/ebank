namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedAllNames : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AccountModels", "Employee_Id", "dbo.EmployeeModels");
            DropForeignKey("dbo.EntryModels", "AccountModel_Id", "dbo.AccountModels");
            DropForeignKey("dbo.LoanApplicationModels", "Tariff_Id", "dbo.TariffModels");
            DropForeignKey("dbo.LoanModels", "Application_Id", "dbo.LoanApplicationModels");
            DropForeignKey("dbo.LoanModels", "PaymentSchedule_Id", "dbo.PaymentScheduleModels");
            DropIndex("dbo.AccountModels", new[] { "Employee_Id" });
            DropIndex("dbo.EntryModels", new[] { "AccountModel_Id" });
            DropIndex("dbo.LoanApplicationModels", new[] { "Tariff_Id" });
            DropIndex("dbo.LoanModels", new[] { "Application_Id" });
            DropIndex("dbo.LoanModels", new[] { "PaymentSchedule_Id" });
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        DateOpened = c.DateTime(nullable: false),
                        DateClosed = c.DateTime(nullable: false),
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
                        Date = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        SubType = c.Int(nullable: false),
                        Account_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Account_Id)
                .Index(t => t.Account_Id);
            
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Application_Id = c.Long(),
                        ConcludedBy_Id = c.Int(),
                        PaymentSchedule_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoanApplications", t => t.Application_Id)
                .ForeignKey("dbo.Employees", t => t.ConcludedBy_Id)
                .ForeignKey("dbo.PaymentSchedules", t => t.PaymentSchedule_Id)
                .Index(t => t.Application_Id)
                .Index(t => t.ConcludedBy_Id)
                .Index(t => t.PaymentSchedule_Id);
            
            CreateTable(
                "dbo.LoanApplications",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeCreated = c.DateTime(nullable: false),
                        Term = c.Int(nullable: false),
                        CellPhone = c.String(),
                        LoanPurpose = c.Int(nullable: false),
                        Tariff_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tariffs", t => t.Tariff_Id)
                .Index(t => t.Tariff_Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.String(),
                        DocType = c.Int(nullable: false),
                        TariffDocType = c.Int(nullable: false),
                        Customer_Id = c.Long(),
                        LoanApplication_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.LoanApplications", t => t.LoanApplication_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.LoanApplication_Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IdentificationNumber = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tariffs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        MinTerm = c.Int(nullable: false),
                        MaxTerm = c.Int(nullable: false),
                        InitialFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsGuarantorNeeded = c.Boolean(nullable: false),
                        IsSecondaryDocumentNeeded = c.Boolean(nullable: false),
                        LoanPurpose = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        HiredOn = c.DateTime(nullable: false),
                        FiredOn = c.DateTime(nullable: false),
                        EmployeeRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentSchedules",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.AccountModels");
            DropTable("dbo.EmployeeModels");
            DropTable("dbo.EntryModels");
            DropTable("dbo.LoanModels");
            DropTable("dbo.LoanApplicationModels");
            DropTable("dbo.TariffModels");
            DropTable("dbo.PaymentScheduleModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PaymentScheduleModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TariffModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        MinTerm = c.Int(nullable: false),
                        MaxTerm = c.Int(nullable: false),
                        InitialFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsGuarantorNeeded = c.Boolean(nullable: false),
                        IsSecondaryDocumentNeeded = c.Boolean(nullable: false),
                        LoanPurpose = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoanApplicationModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeCreated = c.DateTime(nullable: false),
                        Term = c.Int(nullable: false),
                        CellPhone = c.String(),
                        LoanPurpose = c.Int(nullable: false),
                        Tariff_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoanModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Application_Id = c.Long(),
                        PaymentSchedule_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EntryModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        SubType = c.Int(nullable: false),
                        AccountModel_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmployeeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        HiredOn = c.DateTime(nullable: false),
                        FiredOn = c.DateTime(nullable: false),
                        EmployeeRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccountModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        Employee_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Loans", "PaymentSchedule_Id", "dbo.PaymentSchedules");
            DropForeignKey("dbo.Loans", "ConcludedBy_Id", "dbo.Employees");
            DropForeignKey("dbo.Loans", "Application_Id", "dbo.LoanApplications");
            DropForeignKey("dbo.LoanApplications", "Tariff_Id", "dbo.Tariffs");
            DropForeignKey("dbo.Documents", "LoanApplication_Id", "dbo.LoanApplications");
            DropForeignKey("dbo.Documents", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.Accounts", "Loan_Id", "dbo.Loans");
            DropForeignKey("dbo.Entries", "Account_Id", "dbo.Accounts");
            DropIndex("dbo.Loans", new[] { "PaymentSchedule_Id" });
            DropIndex("dbo.Loans", new[] { "ConcludedBy_Id" });
            DropIndex("dbo.Loans", new[] { "Application_Id" });
            DropIndex("dbo.LoanApplications", new[] { "Tariff_Id" });
            DropIndex("dbo.Documents", new[] { "LoanApplication_Id" });
            DropIndex("dbo.Documents", new[] { "Customer_Id" });
            DropIndex("dbo.Accounts", new[] { "Loan_Id" });
            DropIndex("dbo.Entries", new[] { "Account_Id" });
            DropTable("dbo.PaymentSchedules");
            DropTable("dbo.Employees");
            DropTable("dbo.Tariffs");
            DropTable("dbo.Customers");
            DropTable("dbo.Documents");
            DropTable("dbo.LoanApplications");
            DropTable("dbo.Loans");
            DropTable("dbo.Entries");
            DropTable("dbo.Accounts");
            CreateIndex("dbo.LoanModels", "PaymentSchedule_Id");
            CreateIndex("dbo.LoanModels", "Application_Id");
            CreateIndex("dbo.LoanApplicationModels", "Tariff_Id");
            CreateIndex("dbo.EntryModels", "AccountModel_Id");
            CreateIndex("dbo.AccountModels", "Employee_Id");
            AddForeignKey("dbo.LoanModels", "PaymentSchedule_Id", "dbo.PaymentScheduleModels", "Id");
            AddForeignKey("dbo.LoanModels", "Application_Id", "dbo.LoanApplicationModels", "Id");
            AddForeignKey("dbo.LoanApplicationModels", "Tariff_Id", "dbo.TariffModels", "Id");
            AddForeignKey("dbo.EntryModels", "AccountModel_Id", "dbo.AccountModels", "Id");
            AddForeignKey("dbo.AccountModels", "Employee_Id", "dbo.EmployeeModels", "Id");
        }
    }
}
