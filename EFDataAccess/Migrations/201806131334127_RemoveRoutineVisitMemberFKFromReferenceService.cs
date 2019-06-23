namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRoutineVisitMemberFKFromReferenceService : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReferenceService", "RoutineVisitMemberID", "dbo.RoutineVisitMember");
            DropIndex("dbo.ReferenceService", new[] { "RoutineVisitMemberID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.ReferenceService", "RoutineVisitMemberID");
            AddForeignKey("dbo.ReferenceService", "RoutineVisitMemberID", "dbo.RoutineVisitMember", "RoutineVisitMemberID");
        }
    }
}
