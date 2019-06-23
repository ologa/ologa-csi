namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedUser_LastUpdatedUser_For_Entities_with_AuditedEntity_Inheritance : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Adult", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Adult", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.Child", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Child", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.Partner", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Partner", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CarePlan", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CarePlan", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CSI", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CSI", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CSIDomain", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CSIDomain", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.Tasks", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Tasks", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.OrgUnit", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.OrgUnit", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.OrgUnitType", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.OrgUnitType", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.SiteGoal", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.SiteGoal", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.Aid", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Aid", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.SimpleEntity", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.SimpleEntity", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.RoutineVisitSupport", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.RoutineVisitSupport", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.ChildStatus", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.ChildStatus", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.Reference", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.Reference", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
            RenameIndex(table: "dbo.ReferenceService", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            RenameIndex(table: "dbo.ReferenceService", name: "IX_LastUpdatedUser_UserID", newName: "IX_LastUpdatedUserID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ReferenceService", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.ReferenceService", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.Reference", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Reference", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.ChildStatus", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.ChildStatus", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisitSupport", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisitSupport", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.SimpleEntity", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.SimpleEntity", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.Aid", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Aid", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.HouseHold", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.SiteGoal", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.SiteGoal", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.OrgUnitType", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.OrgUnitType", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.OrgUnit", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.OrgUnit", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.Tasks", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Tasks", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CSIDomain", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CSIDomain", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CSI", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CSI", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CarePlan", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CarePlan", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.Partner", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Partner", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.Child", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Child", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Adult", name: "IX_LastUpdatedUserID", newName: "IX_LastUpdatedUser_UserID");
            RenameIndex(table: "dbo.Adult", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
        }
    }
}
