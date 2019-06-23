namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncronizationPremisses : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE dbo.[Partner] SET siteID = 1", false);
            Sql(@"UPDATE dbo.[Site] SET Site_guid = newid() Where SiteID = 1", false);
        }
        
        public override void Down()
        {
        }
    }
}
