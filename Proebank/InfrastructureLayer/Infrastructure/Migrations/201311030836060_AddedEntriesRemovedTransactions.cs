namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEntriesRemovedTransactions : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountModels", t => t.AccountModel_Id)
                .Index(t => t.AccountModel_Id);
            
            DropColumn("dbo.AccountModels", "Balance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountModels", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.EntryModels", "AccountModel_Id", "dbo.AccountModels");
            DropIndex("dbo.EntryModels", new[] { "AccountModel_Id" });
            DropTable("dbo.EntryModels");
        }
    }
}
