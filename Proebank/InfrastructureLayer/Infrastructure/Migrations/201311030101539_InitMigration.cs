namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                        Employee_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeModels", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
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
                "dbo.LoanModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Application_Id = c.Long(),
                        PaymentSchedule_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoanApplicationModels", t => t.Application_Id)
                .ForeignKey("dbo.PaymentScheduleModels", t => t.PaymentSchedule_Id)
                .Index(t => t.Application_Id)
                .Index(t => t.PaymentSchedule_Id);
            
            CreateTable(
                "dbo.LoanApplicationModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LoanAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeCreated = c.DateTime(nullable: false),
                        Term = c.Int(nullable: false),
                        LoanPurpose = c.Int(nullable: false),
                        Tariff_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TariffModels", t => t.Tariff_Id)
                .Index(t => t.Tariff_Id);
            
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
                "dbo.PaymentScheduleModels",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoanModels", "PaymentSchedule_Id", "dbo.PaymentScheduleModels");
            DropForeignKey("dbo.LoanModels", "Application_Id", "dbo.LoanApplicationModels");
            DropForeignKey("dbo.LoanApplicationModels", "Tariff_Id", "dbo.TariffModels");
            DropForeignKey("dbo.AccountModels", "Employee_Id", "dbo.EmployeeModels");
            DropIndex("dbo.LoanModels", new[] { "PaymentSchedule_Id" });
            DropIndex("dbo.LoanModels", new[] { "Application_Id" });
            DropIndex("dbo.LoanApplicationModels", new[] { "Tariff_Id" });
            DropIndex("dbo.AccountModels", new[] { "Employee_Id" });
            DropTable("dbo.PaymentScheduleModels");
            DropTable("dbo.TariffModels");
            DropTable("dbo.LoanApplicationModels");
            DropTable("dbo.LoanModels");
            DropTable("dbo.EmployeeModels");
            DropTable("dbo.AccountModels");
        }
    }
}
