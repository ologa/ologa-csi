namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScholarityLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Beneficiary", "ScholarityLevel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Beneficiary", "ScholarityLevel");
        }
    }
}
