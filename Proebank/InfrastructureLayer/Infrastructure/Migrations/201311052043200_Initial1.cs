namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "HiredOn", c => c.DateTime());
            AlterColumn("dbo.Employees", "FiredOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "FiredOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Employees", "HiredOn", c => c.DateTime(nullable: false));
        }
    }
}
