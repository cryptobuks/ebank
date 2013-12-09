namespace Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoanHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoanHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WhenOpened = c.DateTime(nullable: false),
                        WhenClosed = c.DateTime(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HadProblems = c.Boolean(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                        Person_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.Person_Id)
                .Index(t => t.Person_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoanHistories", "Person_Id", "dbo.Documents");
            DropIndex("dbo.LoanHistories", new[] { "Person_Id" });
            DropTable("dbo.LoanHistories");
        }
    }
}
