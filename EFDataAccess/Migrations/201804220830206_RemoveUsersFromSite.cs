namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUsersFromSite : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "SiteID", "dbo.Site");
            DropIndex("dbo.User", "IX_Site_SiteID");
            DropColumn("dbo.User", "SiteID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "SiteID", c => c.Int());
            CreateIndex("dbo.User", "SiteID", name: "IX_Site_SiteID");
            AddForeignKey("dbo.User", "SiteID", "dbo.Site", "SiteID");
        }
    }
}
