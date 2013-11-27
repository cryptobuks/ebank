namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedIsSecondaryDocNeededFromLoanApplication : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tariffs", "IsSecondaryDocumentNeeded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tariffs", "IsSecondaryDocumentNeeded", c => c.Boolean(nullable: false));
        }
    }
}
