namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTariffStartTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tariffs", "StartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tariffs", "StartDate");
        }
    }
}
