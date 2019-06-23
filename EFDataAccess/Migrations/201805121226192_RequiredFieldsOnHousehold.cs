namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFieldsOnHousehold : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HouseHold", "OrgUnitID", "dbo.OrgUnit");
            DropForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner");
            DropIndex("dbo.HouseHold", new[] { "OrgUnitID" });
            DropIndex("dbo.HouseHold", new[] { "PartnerID" });
            AlterColumn("dbo.HouseHold", "HouseholdName", c => c.String(nullable: false));
            AlterColumn("dbo.HouseHold", "OrgUnitID", c => c.Int(nullable: false));
            AlterColumn("dbo.HouseHold", "PartnerID", c => c.Int(nullable: false));
            AlterColumn("dbo.HouseHold", "RegistrationDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.HouseHold", "OrgUnitID");
            CreateIndex("dbo.HouseHold", "PartnerID");
            AddForeignKey("dbo.HouseHold", "OrgUnitID", "dbo.OrgUnit", "OrgUnitID", cascadeDelete: true);
            AddForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner", "PartnerID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner");
            DropForeignKey("dbo.HouseHold", "OrgUnitID", "dbo.OrgUnit");
            DropIndex("dbo.HouseHold", new[] { "PartnerID" });
            DropIndex("dbo.HouseHold", new[] { "OrgUnitID" });
            AlterColumn("dbo.HouseHold", "RegistrationDate", c => c.DateTime());
            AlterColumn("dbo.HouseHold", "PartnerID", c => c.Int());
            AlterColumn("dbo.HouseHold", "OrgUnitID", c => c.Int());
            AlterColumn("dbo.HouseHold", "HouseholdName", c => c.String());
            CreateIndex("dbo.HouseHold", "PartnerID");
            CreateIndex("dbo.HouseHold", "OrgUnitID");
            AddForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner", "PartnerID");
            AddForeignKey("dbo.HouseHold", "OrgUnitID", "dbo.OrgUnit", "OrgUnitID");
        }
    }
}
