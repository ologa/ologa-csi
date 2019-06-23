namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEmptySiteIDOnPartner : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE Partner SET siteID = 1 WHERE siteID is null");
        }
        
        public override void Down()
        {
        }
    }
}
