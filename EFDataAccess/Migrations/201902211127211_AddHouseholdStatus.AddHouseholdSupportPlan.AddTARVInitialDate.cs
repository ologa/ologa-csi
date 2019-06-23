namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHouseholdStatusAddHouseholdSupportPlanAddTARVInitialDate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HouseholdStatusHistory",
                c => new
                    {
                        HouseholdStatusHistoryID = c.Int(nullable: false, identity: true),
                        RegistrationDate = c.DateTime(nullable: false),
                        HouseholdID = c.Int(),
                        HouseholdStatusID = c.Int(),
                        HouseholdStatusHistory_Guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                    })
                .PrimaryKey(t => t.HouseholdStatusHistoryID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.HouseHold", t => t.HouseholdID)
                .ForeignKey("dbo.HouseholdStatus", t => t.HouseholdStatusID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.HouseholdID)
                .Index(t => t.HouseholdStatusID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);
            
            CreateTable(
                "dbo.HouseholdStatus",
                c => new
                    {
                        HouseholdStatusID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        HouseholdStatus_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                    })
                .PrimaryKey(t => t.HouseholdStatusID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);
            
            CreateTable(
                "dbo.HouseholdSupportPlan",
                c => new
                    {
                        HouseHoldSupportPlanID = c.Int(nullable: false, identity: true),
                        HouseHoldSupportPlanStatus = c.Int(nullable: false),
                        SupportPlanInitialDate = c.DateTime(nullable: false),
                        SupportPlanFinalDate = c.DateTime(),
                        HouseholdID = c.Int(),
                        Householdsupportplan_Guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                    })
                .PrimaryKey(t => t.HouseHoldSupportPlanID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.HouseHold", t => t.HouseholdID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.HouseholdID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);
            
            AddColumn("dbo.HIVStatus", "TarvInitialDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HouseholdSupportPlan", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseholdSupportPlan", "HouseholdID", "dbo.HouseHold");
            DropForeignKey("dbo.HouseholdSupportPlan", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseholdStatusHistory", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseholdStatusHistory", "HouseholdStatusID", "dbo.HouseholdStatus");
            DropForeignKey("dbo.HouseholdStatus", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseholdStatus", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseholdStatusHistory", "HouseholdID", "dbo.HouseHold");
            DropForeignKey("dbo.HouseholdStatusHistory", "CreatedUserID", "dbo.User");
            DropIndex("dbo.HouseholdSupportPlan", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.HouseholdSupportPlan", new[] { "CreatedUserID" });
            DropIndex("dbo.HouseholdSupportPlan", new[] { "HouseholdID" });
            DropIndex("dbo.HouseholdStatus", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.HouseholdStatus", new[] { "CreatedUserID" });
            DropIndex("dbo.HouseholdStatusHistory", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.HouseholdStatusHistory", new[] { "CreatedUserID" });
            DropIndex("dbo.HouseholdStatusHistory", new[] { "HouseholdStatusID" });
            DropIndex("dbo.HouseholdStatusHistory", new[] { "HouseholdID" });
            DropColumn("dbo.HIVStatus", "TarvInitialDate");
            DropTable("dbo.HouseholdSupportPlan");
            DropTable("dbo.HouseholdStatus");
            DropTable("dbo.HouseholdStatusHistory");
        }
    }
}
