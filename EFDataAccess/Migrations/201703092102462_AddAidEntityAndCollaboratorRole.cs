namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAidEntityAndCollaboratorRole : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HouseHold", "AidTypeID", "dbo.SimpleEntity");
            DropIndex("dbo.HouseHold", "IX_AidType_SimpleEntityID");
            RenameIndex(table: "dbo.Partner", name: "IX_CollaboratorRole_CollaboratorRoleID", newName: "IX_CollaboratorRoleID");
            CreateTable(
                "dbo.Aid",
                c => new
                    {
                        AidID = c.Int(nullable: false, identity: true),
                        Aid_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        InstitutionalAid = c.Boolean(nullable: false),
                        InstitutionalAidDetail = c.String(maxLength: 30),
                        communityAid = c.Boolean(nullable: false),
                        communityAidDetail = c.String(maxLength: 30),
                        individualAid = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.AidID);
            
            AddColumn("dbo.HouseHold", "AidID", c => c.Int());
            CreateIndex("dbo.HouseHold", "AidID", name: "IX_Aid_AidID");
            AddForeignKey("dbo.HouseHold", "AidID", "dbo.Aid", "AidID");
            DropColumn("dbo.HouseHold", "OtherAidType");
            DropColumn("dbo.HouseHold", "AidTypeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HouseHold", "AidTypeID", c => c.Int());
            AddColumn("dbo.HouseHold", "OtherAidType", c => c.String());
            DropForeignKey("dbo.HouseHold", "AidID", "dbo.Aid");
            DropIndex("dbo.HouseHold", "IX_Aid_AidID");
            DropColumn("dbo.HouseHold", "AidID");
            DropTable("dbo.Aid");
            RenameIndex(table: "dbo.Partner", name: "IX_CollaboratorRoleID", newName: "IX_CollaboratorRole_CollaboratorRoleID");
            CreateIndex("dbo.HouseHold", "AidTypeID", name: "IX_AidType_SimpleEntityID");
            AddForeignKey("dbo.HouseHold", "AidTypeID", "dbo.SimpleEntity", "SimpleEntityID");
        }
    }
}
