namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableNewMandatoryFieldsForBeneficiaryAndHousehold : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Adult", new[] { "KinshipToFamilyHeadID" });
            DropIndex("dbo.Child", new[] { "OVCTypeID" });
            DropIndex("dbo.Child", new[] { "KinshipToFamilyHeadID" });
            DropIndex("dbo.HouseHold", new[] { "FamilyHeadId" });
            DropIndex("dbo.HouseHold", new[] { "FamilyOriginRefId" });
            AlterColumn("dbo.Adult", "KinshipToFamilyHeadID", c => c.Int(nullable: false));
            AlterColumn("dbo.Child", "OVCTypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.Child", "KinshipToFamilyHeadID", c => c.Int(nullable: false));
            AlterColumn("dbo.HouseHold", "FamilyHeadId", c => c.Int(nullable: false));
            AlterColumn("dbo.HouseHold", "FamilyOriginRefId", c => c.Int(nullable: false));
            CreateIndex("dbo.Adult", "KinshipToFamilyHeadID");
            CreateIndex("dbo.Child", "OVCTypeID");
            CreateIndex("dbo.Child", "KinshipToFamilyHeadID");
            CreateIndex("dbo.HouseHold", "FamilyHeadId");
            CreateIndex("dbo.HouseHold", "FamilyOriginRefId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.HouseHold", new[] { "FamilyOriginRefId" });
            DropIndex("dbo.HouseHold", new[] { "FamilyHeadId" });
            DropIndex("dbo.Child", new[] { "KinshipToFamilyHeadID" });
            DropIndex("dbo.Child", new[] { "OVCTypeID" });
            DropIndex("dbo.Adult", new[] { "KinshipToFamilyHeadID" });
            AlterColumn("dbo.HouseHold", "FamilyOriginRefId", c => c.Int());
            AlterColumn("dbo.HouseHold", "FamilyHeadId", c => c.Int());
            AlterColumn("dbo.Child", "KinshipToFamilyHeadID", c => c.Int());
            AlterColumn("dbo.Child", "OVCTypeID", c => c.Int());
            AlterColumn("dbo.Adult", "KinshipToFamilyHeadID", c => c.Int());
            CreateIndex("dbo.HouseHold", "FamilyOriginRefId");
            CreateIndex("dbo.HouseHold", "FamilyHeadId");
            CreateIndex("dbo.Child", "KinshipToFamilyHeadID");
            CreateIndex("dbo.Child", "OVCTypeID");
            CreateIndex("dbo.Adult", "KinshipToFamilyHeadID");
        }
    }
}
