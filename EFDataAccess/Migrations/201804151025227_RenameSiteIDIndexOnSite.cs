namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSiteIDIndexOnSite : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.SiteGoal", name: "IX_Site_SiteID", newName: "IX_SiteID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.SiteGoal", name: "IX_SiteID", newName: "IX_Site_SiteID");
        }
    }
}
