namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNIDOnHIVStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HIVStatus", "NID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HIVStatus", "NID");
        }
    }
}
