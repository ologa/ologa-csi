namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServicesStatusHistory : DbMigration
    {
        public override void Up()
        {
            Sql("Update Beneficiary set ServicesStatusID = null");

            CreateTable(
                "dbo.ServicesStatus",
                c => new
                    {
                        ServicesStatusID = c.Int(nullable: false, identity: true),
                        ServicesStatus_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        BeneficiaryID = c.Int(nullable: false),
                        Beneficiary_guid = c.Guid(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        GenerationDate = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.ServicesStatusID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID");
            
            DropColumn("dbo.Beneficiary", "ServicesStatusDate");
            DropColumn("dbo.Beneficiary", "ServicesStatusForReportID");
            DropColumn("dbo.Beneficiary", "ServicesStatusForReportDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Beneficiary", "ServicesStatusForReportDate", c => c.DateTime());
            AddColumn("dbo.Beneficiary", "ServicesStatusForReportID", c => c.Int());
            AddColumn("dbo.Beneficiary", "ServicesStatusDate", c => c.DateTime());
            DropForeignKey("dbo.ServicesStatus", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.ServicesStatus", "CreatedUserID", "dbo.User");
            DropIndex("dbo.ServicesStatus", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.ServicesStatus", "IX_CreatedUser_UserID");
            DropTable("dbo.ServicesStatus");
        }
    }
}
