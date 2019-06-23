namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStateFieldsOnAuditAndSyncEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "SiteID", "dbo.Site");
            DropIndex("dbo.User", "IX_Site_SiteID");
            AddColumn("dbo.Adult", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Adult", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.User", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Partner", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Partner", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Child", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Child", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.ChildStatusHistory", "State", c => c.Int(nullable: false));
            AddColumn("dbo.ChildStatusHistory", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.ChildStatus", "State", c => c.Int(nullable: false));
            AddColumn("dbo.ChildStatus", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CSI", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CSI", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlan", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlan", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlanDomain", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlanDomain", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlanDomainSupportService", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlanDomainSupportService", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CSIDomainScore", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CSIDomainScore", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.CSIDomain", "State", c => c.Int(nullable: false));
            AddColumn("dbo.CSIDomain", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Site", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "State", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnit", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnitType", "State", c => c.Int(nullable: false));
            AddColumn("dbo.OrgUnitType", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.SiteGoal", "State", c => c.Int(nullable: false));
            AddColumn("dbo.SiteGoal", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Tasks", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.HIVStatus", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.HouseHold", "State", c => c.Int(nullable: false));
            AddColumn("dbo.HouseHold", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Aid", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Aid", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.SimpleEntity", "State", c => c.Int(nullable: false));
            AddColumn("dbo.SimpleEntity", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisit", "State", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisit", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisitMember", "State", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisitMember", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisitSupport", "State", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisitSupport", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.Reference", "State", c => c.Int(nullable: false));
            AddColumn("dbo.Reference", "SyncState", c => c.Int(nullable: false));
            AddColumn("dbo.ReferenceService", "State", c => c.Int(nullable: false));
            AddColumn("dbo.ReferenceService", "SyncState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "SiteID", c => c.Int());
            DropColumn("dbo.ReferenceService", "SyncState");
            DropColumn("dbo.ReferenceService", "State");
            DropColumn("dbo.Reference", "SyncState");
            DropColumn("dbo.Reference", "State");
            DropColumn("dbo.RoutineVisitSupport", "SyncState");
            DropColumn("dbo.RoutineVisitSupport", "State");
            DropColumn("dbo.RoutineVisitMember", "SyncState");
            DropColumn("dbo.RoutineVisitMember", "State");
            DropColumn("dbo.RoutineVisit", "SyncState");
            DropColumn("dbo.RoutineVisit", "State");
            DropColumn("dbo.SimpleEntity", "SyncState");
            DropColumn("dbo.SimpleEntity", "State");
            DropColumn("dbo.Aid", "SyncState");
            DropColumn("dbo.Aid", "State");
            DropColumn("dbo.HouseHold", "SyncState");
            DropColumn("dbo.HouseHold", "State");
            DropColumn("dbo.HIVStatus", "SyncState");
            DropColumn("dbo.Tasks", "SyncState");
            DropColumn("dbo.Tasks", "State");
            DropColumn("dbo.SiteGoal", "SyncState");
            DropColumn("dbo.SiteGoal", "State");
            DropColumn("dbo.OrgUnitType", "SyncState");
            DropColumn("dbo.OrgUnitType", "State");
            DropColumn("dbo.OrgUnit", "SyncState");
            DropColumn("dbo.OrgUnit", "State");
            DropColumn("dbo.Site", "SyncState");
            DropColumn("dbo.CSIDomain", "SyncState");
            DropColumn("dbo.CSIDomain", "State");
            DropColumn("dbo.CSIDomainScore", "SyncState");
            DropColumn("dbo.CSIDomainScore", "State");
            DropColumn("dbo.CarePlanDomainSupportService", "SyncState");
            DropColumn("dbo.CarePlanDomainSupportService", "State");
            DropColumn("dbo.CarePlanDomain", "SyncState");
            DropColumn("dbo.CarePlanDomain", "State");
            DropColumn("dbo.CarePlan", "SyncState");
            DropColumn("dbo.CarePlan", "State");
            DropColumn("dbo.CSI", "SyncState");
            DropColumn("dbo.CSI", "State");
            DropColumn("dbo.ChildStatus", "SyncState");
            DropColumn("dbo.ChildStatus", "State");
            DropColumn("dbo.ChildStatusHistory", "SyncState");
            DropColumn("dbo.ChildStatusHistory", "State");
            DropColumn("dbo.Child", "SyncState");
            DropColumn("dbo.Child", "State");
            DropColumn("dbo.Partner", "SyncState");
            DropColumn("dbo.Partner", "State");
            DropColumn("dbo.User", "SyncState");
            DropColumn("dbo.Adult", "SyncState");
            DropColumn("dbo.Adult", "State");
        }
    }
}
