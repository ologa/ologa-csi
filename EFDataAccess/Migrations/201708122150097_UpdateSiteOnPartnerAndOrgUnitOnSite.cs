namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSiteOnPartnerAndOrgUnitOnSite : DbMigration
    {
        public override void Up()
        {
            Sql(@"Update S SET S.[orgUnitID] = 149 FROM [Site] S WHERE S.SiteID = 1", false);
        }
        
        public override void Down()
        {
        }
    }
}
