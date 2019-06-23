namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewHouseHoldFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SimpleEntity",
                c => new
                    {
                        SimpleEntityID = c.Int(nullable: false, identity: true),
                        SimpleEntityGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Type = c.String(nullable: false, maxLength: 20),
                        Code = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.SimpleEntityID);
            
            AddColumn("dbo.HouseHold", "AnyoneBedridden", c => c.Boolean(nullable: false));
            AddColumn("dbo.HouseHold", "FamilyPhoneNumber", c => c.String());
            AddColumn("dbo.HouseHold", "OtherFamilyHead", c => c.String());
            AddColumn("dbo.HouseHold", "OtherAidType", c => c.String());
            AddColumn("dbo.HouseHold", "OtherFamilyOriginRef", c => c.String());
            AddColumn("dbo.HouseHold", "ClosePlaceToHome", c => c.String());
            AddColumn("dbo.HouseHold", "AidTypeID", c => c.Int());
            AddColumn("dbo.HouseHold", "FamilyHeadID", c => c.Int());
            AddColumn("dbo.HouseHold", "FamilyOriginRefID", c => c.Int());
            CreateIndex("dbo.HouseHold", "AidTypeID", name: "IX_AidType_SimpleEntityID");
            CreateIndex("dbo.HouseHold", "FamilyHeadID", name: "IX_FamilyHead_SimpleEntityID");
            CreateIndex("dbo.HouseHold", "FamilyOriginRefID", name: "IX_FamilyOriginRef_SimpleEntityID");
            AddForeignKey("dbo.HouseHold", "AidTypeID", "dbo.SimpleEntity", "SimpleEntityID");
            AddForeignKey("dbo.HouseHold", "FamilyHeadID", "dbo.SimpleEntity", "SimpleEntityID");
            AddForeignKey("dbo.HouseHold", "FamilyOriginRefID", "dbo.SimpleEntity", "SimpleEntityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HouseHold", "FamilyOriginRefID", "dbo.SimpleEntity");
            DropForeignKey("dbo.HouseHold", "FamilyHeadID", "dbo.SimpleEntity");
            DropForeignKey("dbo.HouseHold", "AidTypeID", "dbo.SimpleEntity");
            DropIndex("dbo.HouseHold", "IX_FamilyOriginRef_SimpleEntityID");
            DropIndex("dbo.HouseHold", "IX_FamilyHead_SimpleEntityID");
            DropIndex("dbo.HouseHold", "IX_AidType_SimpleEntityID");
            DropColumn("dbo.HouseHold", "FamilyOriginRefID");
            DropColumn("dbo.HouseHold", "FamilyHeadID");
            DropColumn("dbo.HouseHold", "AidTypeID");
            DropColumn("dbo.HouseHold", "ClosePlaceToHome");
            DropColumn("dbo.HouseHold", "OtherFamilyOriginRef");
            DropColumn("dbo.HouseHold", "OtherAidType");
            DropColumn("dbo.HouseHold", "OtherFamilyHead");
            DropColumn("dbo.HouseHold", "FamilyPhoneNumber");
            DropColumn("dbo.HouseHold", "AnyoneBedridden");
            DropTable("dbo.SimpleEntity");
        }
    }
}
