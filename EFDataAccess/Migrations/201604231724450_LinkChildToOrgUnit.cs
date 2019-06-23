namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkChildToOrgUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Child", "OrgUnitID", c => c.Int());
            CreateIndex("dbo.Child", "OrgUnitID", name: "IX_OrgUnit_OrgUnitID");
            AddForeignKey("dbo.Child", "OrgUnitID", "dbo.OrgUnit", "OrgUnitID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Child", "OrgUnitID", "dbo.OrgUnit");
            DropIndex("dbo.Child", "IX_OrgUnit_OrgUnitID");
            DropColumn("dbo.Child", "OrgUnitID");
        }
    }
}
