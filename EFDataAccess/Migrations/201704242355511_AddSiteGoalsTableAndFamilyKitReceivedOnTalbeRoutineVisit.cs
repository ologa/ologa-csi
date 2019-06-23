namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteGoalsTableAndFamilyKitReceivedOnTalbeRoutineVisit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteGoals",
                c => new
                    {
                        SiteEvaluationID = c.Int(nullable: false, identity: true),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        SiteEvaluation_guid = c.Guid(nullable: false),
                        Indicator = c.String(nullable: false, maxLength: 30),
                        SitePerformanceComments = c.String(nullable: false, maxLength: 30),
                        GoalDate = c.DateTime(nullable: false),
                        GoalNumber = c.Int(nullable: false),
                        SiteID = c.Int(),
                    })
                .PrimaryKey(t => t.SiteEvaluationID)
                .ForeignKey("dbo.Site", t => t.SiteID)
                .Index(t => t.SiteID, name: "IX_Site_SiteID");
            
            AddColumn("dbo.RoutineVisit", "FamilyKitReceived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteGoals", "SiteID", "dbo.Site");
            DropIndex("dbo.SiteGoals", "IX_Site_SiteID");
            DropColumn("dbo.RoutineVisit", "FamilyKitReceived");
            DropTable("dbo.SiteGoals");
        }
    }
}
