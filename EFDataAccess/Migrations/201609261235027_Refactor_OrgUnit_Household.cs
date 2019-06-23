namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor_OrgUnit_Household : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HouseHold", "HouseholdName", c => c.String());
            AddColumn("dbo.HouseHold", "PrincipalChiefName", c => c.String());
            AddColumn("dbo.HouseHold", "NeighborhoodName", c => c.String());
            AddColumn("dbo.HouseHold", "Block", c => c.Int(nullable: false));
            AddColumn("dbo.HouseHold", "HouseNumber", c => c.String());
            AddColumn("dbo.HouseHold", "PartnerID", c => c.Int());
            CreateIndex("dbo.HouseHold", "PartnerID", name: "IX_Partner_PartnerID");
            AddForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner", "PartnerID");
            DropColumn("dbo.OrgUnit", "ChiefName");
            DropColumn("dbo.OrgUnit", "HouseCount");
            DropColumn("dbo.OrgUnit", "MemberCount");
            DropColumn("dbo.OrgUnit", "MaleCount");
            DropColumn("dbo.OrgUnit", "FemaleCount");
            DropColumn("dbo.OrgUnit", "AdultCount");
            DropColumn("dbo.OrgUnit", "ChildCount");
            DropColumn("dbo.OrgUnit", "Quota");
            DropColumn("dbo.OrgUnit", "QuotaSat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrgUnit", "QuotaSat", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "Quota", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "ChildCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "AdultCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "FemaleCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "MaleCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "MemberCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "HouseCount", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "ChiefName", c => c.String(maxLength: 50));
            DropForeignKey("dbo.HouseHold", "PartnerID", "dbo.Partner");
            DropIndex("dbo.HouseHold", "IX_Partner_PartnerID");
            DropColumn("dbo.HouseHold", "PartnerID");
            DropColumn("dbo.HouseHold", "HouseNumber");
            DropColumn("dbo.HouseHold", "Block");
            DropColumn("dbo.HouseHold", "NeighborhoodName");
            DropColumn("dbo.HouseHold", "PrincipalChiefName");
            DropColumn("dbo.HouseHold", "HouseholdName");
        }
    }
}
