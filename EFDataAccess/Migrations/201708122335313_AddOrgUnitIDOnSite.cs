namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrgUnitIDOnSite : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Site", name: "IX_orgUnit_OrgUnitID", newName: "IX_orgUnitID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Site", name: "IX_orgUnitID", newName: "IX_orgUnit_OrgUnitID");
        }
    }
}
