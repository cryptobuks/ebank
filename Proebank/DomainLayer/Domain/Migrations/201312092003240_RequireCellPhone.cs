namespace Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequireCellPhone : DbMigration
    {
        public override void Up()
        {
            //Sql("DELETE FROM dbo.LoanApplications WHERE CellPhone IS NULL");
        }
        
        public override void Down()
        {
        }
    }
}
