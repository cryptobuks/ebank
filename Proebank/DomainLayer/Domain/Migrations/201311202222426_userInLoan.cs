namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInLoan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "Customer_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Loans", "Customer_Id");
            AddForeignKey("dbo.Loans", "Customer_Id", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loans", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Loans", new[] { "Customer_Id" });
            DropColumn("dbo.Loans", "Customer_Id");
        }
    }
}
