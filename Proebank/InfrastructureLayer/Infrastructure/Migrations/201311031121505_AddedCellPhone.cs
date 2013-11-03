namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCellPhone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoanApplicationModels", "CellPhone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoanApplicationModels", "CellPhone");
        }
    }
}
