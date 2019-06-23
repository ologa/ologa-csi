namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteOnPartnerAndOrgUnitOnSite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partner", "siteID", c => c.Int());
            AddColumn("dbo.Site", "orgUnitID", c => c.Int());
            CreateIndex("dbo.Partner", "siteID", name: "IX_site_SiteID");
            CreateIndex("dbo.Site", "orgUnitID", name: "IX_orgUnit_OrgUnitID");
            AddForeignKey("dbo.Site", "orgUnitID", "dbo.OrgUnit", "OrgUnitID");
            AddForeignKey("dbo.Partner", "siteID", "dbo.Site", "SiteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partner", "siteID", "dbo.Site");
            DropForeignKey("dbo.Site", "orgUnitID", "dbo.OrgUnit");
            DropIndex("dbo.Site", "IX_orgUnit_OrgUnitID");
            DropIndex("dbo.Partner", "IX_site_SiteID");
            DropColumn("dbo.Site", "orgUnitID");
            DropColumn("dbo.Partner", "siteID");
        }
    }
}
