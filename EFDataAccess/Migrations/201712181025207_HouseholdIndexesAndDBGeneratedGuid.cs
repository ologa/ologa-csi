namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HouseholdIndexesAndDBGeneratedGuid : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.HouseHold", name: "IX_OrgUnit_OrgUnitID", newName: "IX_OrgUnitID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_Partner_PartnerID", newName: "IX_PartnerID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_FamilyHead_SimpleEntityID", newName: "IX_FamilyHeadId");
            RenameIndex(table: "dbo.HouseHold", name: "IX_FamilyOriginRef_SimpleEntityID", newName: "IX_FamilyOriginRefId");
            AlterColumn("dbo.HouseHold", "HouseholdUniqueIdentifier", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HouseHold", "HouseholdUniqueIdentifier", c => c.Guid(nullable: false));
            RenameIndex(table: "dbo.HouseHold", name: "IX_FamilyOriginRefId", newName: "IX_FamilyOriginRef_SimpleEntityID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_FamilyHeadId", newName: "IX_FamilyHead_SimpleEntityID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_PartnerID", newName: "IX_Partner_PartnerID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_OrgUnitID", newName: "IX_OrgUnit_OrgUnitID");
        }
    }
}
